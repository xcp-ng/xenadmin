/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Net;
using System.Threading;
using DotNetVnc;
using log4net;
using XenAdmin.Controls.ConsoleTab;
using XenAPI;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Dialogs;
using System.Security.Cryptography;
using XenAdmin.Network;
using XenCenterLib;
using Console = XenAPI.Console;
using Message = System.Windows.Forms.Message;
using Timer = System.Threading.Timer;

namespace XenAdmin.ConsoleView
{
    public class XSVNCScreen : UserControl
    {
        private const int SHORT_RETRY_COUNT = 10;
        private const int SHORT_RETRY_SLEEP_TIME = 100;
        private const int RETRY_SLEEP_TIME = 5000;
        private const int RDP_POLL_INTERVAL = 30000;
        public const int RDP_PORT = 3389;
        private const int VNC_PORT = 5900;
        private const int CONSOLE_SIZE_OFFSET = 6;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int ConnectionRetries = 0;

        private volatile bool useVNC = true;

        private bool autoCaptureKeyboardAndMouse = true;

        private readonly Color focusColor = SystemColors.MenuHighlight;

        /// <summary>
        /// May only be written on the event thread.  May be read off the event thread, to check whether
        /// the VNC source has been switched during connection.
        /// </summary>
        private volatile VNCGraphicsClient vncClient = null;

        private RdpClient rdpClient;

        private Timer connectionPoller = null;

        private VM sourceVM = null;
        private bool sourceIsPV = false;

        private readonly Object hostedConsolesLock = new Object();
        private List<XenRef<Console>> hostedConsoles = null;

        /// <summary>
        /// This is assigned when the hosted connection connects up.  It's used by PollPort to check for
        /// the IP address from the guest metrics, so for the lifetime of that hosted connection, we can
        /// poll for the in-guest VNC using the same session.  activeSession must be accessed only under
        /// the activeSessionLock.
        /// </summary>
        private Session activeSession = null;
        private readonly Object activeSessionLock = new Object();

        /// <summary>
        /// Xvnc will blacklist us if we're too quick with the disconnect and reconnect that we do
        /// when polling for the VNC port.  To get around this, we keep the connection open between poll
        /// and proper connection.  pendingVNCConnection must be accessed only under the
        /// pendingVNCConnectionLock.  Work under this lock must be non-blocking, because it's used on
        /// Dispose.
        /// </summary>
        private Stream pendingVNCConnection = null;
        private readonly Object pendingVNCConnectionLock = new Object();

        internal EventHandler ResizeHandler;

        public event EventHandler UserCancelledAuth;
        public event EventHandler VncConnectionAttemptCancelled;
        public event Action<bool> GpuStatusChanged;
        public event Action<string> ConnectionNameChanged;

        public bool RdpVersionWarningNeeded { get { return rdpClient != null && rdpClient.needsRdpVersionWarning; }}

        internal readonly VNCTabView parentVNCTabView;

        [DefaultValue(false)]
        public bool UserWantsToSwitchProtocol { get; set; }

        private bool hasRDP { get { return Source != null && Source.HasRDP(); } }

        /// <summary>
        /// Whether we have tried to login without providing a password (covers the case where the user
        /// has configured VNC not to require a login password). If no password is saved, passwordless
        /// login is tried once.
        /// </summary>
        private bool haveTriedLoginWithoutPassword = false;
        private bool ignoreNextError = false;

        private Dictionary<string, string> cachedNetworks;

        /// <summary>
        /// The last known VNC password for this VM.
        /// </summary>
        private char[] vncPassword = null;

        internal ConsoleKeyHandler KeyHandler;

        internal string ElevatedUsername;
        internal string ElevatedPassword;

        internal XSVNCScreen(VM source, EventHandler resizeHandler, VNCTabView parent, string elevatedUsername, string elevatedPassword)
            : base()
        {
            this.ResizeHandler = resizeHandler;
            this.parentVNCTabView = parent;
            this.Source = source;
            this.KeyHandler = parentVNCTabView.KeyHandler;
            ElevatedUsername = elevatedUsername;
            ElevatedPassword = elevatedPassword;

#pragma warning disable 0219
            IntPtr _ = Handle;
#pragma warning restore 0219

            initSubControl();

            //We're going to try and catch when the IP address changes for the VM, and re-scan for ports.
            if (source == null)
                return;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            VM_guest_metrics guestMetrics = Source.Connection.Resolve<VM_guest_metrics>(Source.guest_metrics);
            if (guestMetrics == null)
                return;

            cachedNetworks = guestMetrics.networks;

            guestMetrics.PropertyChanged += new PropertyChangedEventHandler(guestMetrics_PropertyChanged);
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnableRDPPolling")
                Program.Invoke(this, startPolling);
        }

        private void UnregisterEventListeners()
        {
            if (Source == null)
                return;

            Source.PropertyChanged -= new PropertyChangedEventHandler(VM_PropertyChanged);

            VM_guest_metrics guestMetrics = Source.Connection.Resolve<VM_guest_metrics>(Source.guest_metrics);
            if (guestMetrics == null)
                return;

            guestMetrics.PropertyChanged -= new PropertyChangedEventHandler(guestMetrics_PropertyChanged);

        }

        void guestMetrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Source == null)
                return;

            if (e.PropertyName == "networks")
            {
                Dictionary<string, string> newNetworks = (sender as VM_guest_metrics).networks;
                if (!equateDictionary<string, string>(newNetworks, cachedNetworks))
                {
                    Log.InfoFormat("Detected IP address change in vm {0}, repolling for VNC/RDP...", Source.Name());

                    cachedNetworks = newNetworks;

                    Program.Invoke(this, startPolling);
                }
            }
        }

        private static bool equateDictionary<T, S>(Dictionary<T, S> d1, Dictionary<T, S> d2) where S : IEquatable<S>
        {
            if (d1.Count != d2.Count)
                return false;

            foreach (T key in d1.Keys)
            {
                if (!d2.ContainsKey(key) || !d2[key].Equals(d1[key]))
                    return false;
            }

            return true;
        }

        private bool wasPaused = true;

        public void Pause()
        {
            if (RemoteConsole != null)
            {
                wasPaused = true;
                RemoteConsole.Pause();
            }
        }

        public void Unpause()
        {
            if (RemoteConsole != null)
            {
                wasPaused = false;
                RemoteConsole.Unpause();
            }
        }

        public Size DesktopSize
        {
            get
            {
                return RemoteConsole != null ? RemoteConsole.DesktopSize : Size.Empty;
            }
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            UnregisterEventListeners();

            if (disposing)
            {
                if (connectionPoller != null)
                {
                    connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
                    connectionPoller.Dispose();
                    connectionPoller = null;
                }

                if (RemoteConsole != null)
                {
                    RemoteConsole.DisconnectAndDispose();
                    RemoteConsole = null;
                }

                Log.DebugFormat("Set Pending Vnc connection to null");
                SetPendingVNCConnection(null);
            }

            base.Dispose(disposing);
        }

        public MethodInvoker OnDetectRDP = null;
        public MethodInvoker OnDetectVNC = null;
        public String rdpIP = null;
        public String vncIP = null;

        private void PollRDPPort(Object Sender)
        {
            if (hasRDP)
            {
                if (connectionPoller != null)
                    connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
                if (OnDetectRDP != null)
                    Program.Invoke(this, OnDetectRDP);
            }
            else
            {
                rdpIP = null;
                String openIP = PollPort(RDP_PORT, false);

                if (openIP != null)
                {
                    rdpIP = openIP;
                    if (connectionPoller != null)
                        connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
                    if (OnDetectRDP != null)
                        Program.Invoke(this, OnDetectRDP);
                }
            }
        }

        private void PollVNCPort(Object Sender)
        {
            vncIP = null;
            String openIP = PollPort(VNC_PORT, true);

            if (openIP != null)
            {
                vncIP = openIP;
                if (connectionPoller != null)
                    connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
                if (OnDetectVNC != null)
                    Program.Invoke(this, OnDetectVNC);
            }
        }

        /// <summary>
        /// scan each ip address (from the guest agent) for an open port
        /// </summary>
        public String PollPort(int port, bool vnc)
        {
            try
            {
                if (Source == null)
                    return null;

                VM vm = Source;

                XenRef<VM_guest_metrics> guestMetricsRef = vm.guest_metrics;
                if (guestMetricsRef == null || Helper.IsNullOrEmptyOpaqueRef(guestMetricsRef.opaque_ref))
                    return null;

                VM_guest_metrics metrics = vm.Connection.Resolve(guestMetricsRef);
                if (metrics == null)
                    return null;
                Dictionary<string, string> networks = metrics.networks;

                if (networks == null)
                    return null;

                List<string> ipAddresses = new List<string>();
                List<string> ipv6Addresses = new List<string>();
                List<string> ipAddressesForNetworksWithoutPifs = new List<string>();
                List<string> ipv6AddressesForNetworksWithoutPifs = new List<string>();

                foreach (VIF vif in vm.Connection.ResolveAll(vm.VIFs))
                {
                    XenAPI.Network network = vif.Connection.Resolve(vif.network);
                    Host host = vm.Connection.Resolve(vm.resident_on);
                    PIF pif = Helpers.FindPIF(network, host);
                    foreach (var networkInfo in networks.Where(n => n.Key.StartsWith(String.Format("{0}/ip", vif.device))))
                    {
                        if (networkInfo.Key.EndsWith("ip") || networkInfo.Key.Contains("ipv4")) // IPv4 address
                        {
                            if (pif == null)
                                ipAddressesForNetworksWithoutPifs.Add(networkInfo.Value);
                            else if (pif.LinkStatus() == PIF.LinkState.Connected)
                                ipAddresses.Add(networkInfo.Value);
                        }
                        else
                        {
                            if (networkInfo.Key.Contains("ipv6")) // IPv6 address, enclose in square brackets
                            {
                                if (pif == null)
                                    ipv6AddressesForNetworksWithoutPifs.Add(String.Format("[{0}]", networkInfo.Value));
                                else if (pif.LinkStatus() == PIF.LinkState.Connected)
                                    ipv6Addresses.Add(String.Format("[{0}]", networkInfo.Value));
                            }
                            else
                                continue;
                        }
                    }
                }
                ipAddresses = ipAddresses.Distinct().ToList();

                ipAddresses.AddRange(ipv6Addresses); // make sure IPv4 addresses are scanned first (CA-102755)
                // add IP addresses for networks without PIFs
                ipAddresses.AddRange(ipAddressesForNetworksWithoutPifs);
                ipAddresses.AddRange(ipv6AddressesForNetworksWithoutPifs); 


                foreach (String ipAddress in ipAddresses)
                {
                    try
                    {
                        Log.DebugFormat("Poll port {0}:{1}", ipAddress, port);
                        Stream s = connectGuest(ipAddress, port, vm.Connection);
                        if (vnc)
                        {
                            Log.DebugFormat("Connected. Set Pending Vnc connection {0}:{1}", ipAddress, port);
                            SetPendingVNCConnection(s);
                        }
                        else
                        {
                            s.Close();
                        }
                        return ipAddress;
                    }
                    catch (Exception exn)
                    {
                        Log.Debug(exn);
                    }
                }
            }
            catch (WebException)
            {
                // xapi has gone away.
            }
            catch (IOException)
            {
                // xapi has gone away.
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription[0] == Failure.HANDLE_INVALID)
                {
                    // HANDLE_INVALID is fine -- the guest metrics are not there yet.
                }
                else if (exn.ErrorDescription[0] == Failure.SESSION_INVALID)
                {
                    // SESSION_INVALID is fine -- these will expire from time to time.
                    // We need to invalidate the session though.
                    lock (activeSessionLock)
                    {
                        activeSession = null;
                    }
                }
                else
                {
                    Log.Warn("Exception while polling VM for port " + port + ".", exn);
                }
            }
            catch (Exception e)
            {
                Log.Warn("Exception while polling VM for port " + port + ".", e);
            }
            return null;
        }

        /// <summary>
        /// Exchange the current value of pendingVNCConnection with the given one, and close the old
        /// connection, if any.
        /// Nothrow guarantee.
        /// </summary>
        /// <param name="s">May be null</param>
        private void SetPendingVNCConnection(Stream s)
        {
            Stream old_pending;
            lock (pendingVNCConnectionLock)
            {
                old_pending = pendingVNCConnection;
                pendingVNCConnection = s;
            }
            if (old_pending != null)
            {
                try
                {
                    old_pending.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        private bool scaling;
        public bool Scaling
        {
            get
            {
                Program.AssertOnEventThread();
                return scaling;
            }

            set
            {
                Program.AssertOnEventThread();
                scaling = value;
                if (RemoteConsole != null)
                    RemoteConsole.Scaling = value;
            }
        }

        public IRemoteConsole RemoteConsole
        {
            get { return vncClient != null ? (IRemoteConsole)vncClient : rdpClient; }
            set 
            {
                if (vncClient != null) 
                    vncClient = (VNCGraphicsClient) value ;
                else if (rdpClient != null) 
                    rdpClient = (RdpClient)value;
            }
        }

        /// <summary>
        /// Creates the actual VNC or RDP client control.
        /// </summary>
        private void initSubControl()
        {
            Program.AssertOnEventThread();

            //When switch to RDP from VNC, if RDP IP is empty, do not try to switch.
            if (String.IsNullOrEmpty(rdpIP) && !UseVNC && RemoteConsole != null)
                return;

            bool wasFocused = false;
            this.Controls.Clear();
            //console size with some offset to accomodate focus rectangle
            Size currentConsoleSize = new Size(this.Size.Width - CONSOLE_SIZE_OFFSET, this.Size.Height - CONSOLE_SIZE_OFFSET);
                
            // Kill the old client.
            if (RemoteConsole != null)
            {
                wasFocused = RemoteConsole.ConsoleControl != null && RemoteConsole.ConsoleControl.Focused;
                RemoteConsole.DisconnectAndDispose();
                RemoteConsole = null;
                this.vncPassword = null;
            }

            // Reset
            haveTriedLoginWithoutPassword = false;

            if (UseVNC || String.IsNullOrEmpty(rdpIP))
            {
                this.AutoScroll = false;
                this.AutoScrollMinSize = new Size(0, 0);

                vncClient = new VNCGraphicsClient(this);

                vncClient.UseSource = UseSource;
                vncClient.DesktopResized += ResizeHandler;
                vncClient.Resize += ResizeHandler;
                vncClient.ErrorOccurred += ErrorHandler;
                vncClient.ConnectionSuccess += ConnectionSuccess;
                vncClient.Dock = DockStyle.Fill;
            }
            else
            {
                if (rdpClient == null)
                {
                    if (this.ParentForm is FullScreenForm)
                        currentConsoleSize = ((FullScreenForm)ParentForm).GetContentSize();
                    this.AutoScroll = true;
                    this.AutoScrollMinSize = oldSize;
                    rdpClient = new RdpClient(this, currentConsoleSize, ResizeHandler);

                    rdpClient.OnDisconnected += new EventHandler(parentVNCTabView.RdpDisconnectedHandler);
                }
            }

            if (RemoteConsole != null && RemoteConsole.ConsoleControl != null)
            {
                RemoteConsole.KeyHandler = this.KeyHandler;
                RemoteConsole.SendScanCodes = !this.sourceIsPV;
                RemoteConsole.Scaling = Scaling;
                RemoteConsole.DisplayBorder = this.displayFocusRectangle;
                SetKeyboardAndMouseCapture(autoCaptureKeyboardAndMouse);
                if (wasPaused)
                    RemoteConsole.Pause();
                else
                    RemoteConsole.Unpause();
                ConnectToRemoteConsole();

                if (wasFocused)
                    RemoteConsole.Activate();
            }

            if (GpuStatusChanged != null)
                GpuStatusChanged(MustConnectRemoteDesktop());
        }

        internal bool MustConnectRemoteDesktop()
        {
            return (UseVNC || string.IsNullOrEmpty(rdpIP)) &&
                Source.HasGPUPassthrough() && Source.power_state == vm_power_state.Running;
        }

        private void SetKeyboardAndMouseCapture(bool value)
        {
            if (RemoteConsole != null && RemoteConsole.ConsoleControl != null)
                RemoteConsole.ConsoleControl.TabStop = value;
        }

        private void ConnectToRemoteConsole()
        {
            if (vncClient != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(Connect), new KeyValuePair<VNCGraphicsClient, Exception>(vncClient, null));
            else if (rdpClient != null)
                rdpClient.Connect(rdpIP);
        }

        void ConnectionSuccess(object sender, EventArgs e)
        {
            ConnectionRetries = 0;
            if (AutoSwitchRDPLater)
            {
                if (OnDetectRDP != null)
                    Program.Invoke(this, OnDetectRDP);
                AutoSwitchRDPLater = false;
            }
            if (parentVNCTabView.IsRDPControlEnabled())
                parentVNCTabView.EnableToggleVNCButton();
        }

        internal bool AutoSwitchRDPLater
        {
            get;
            set; 
        }

        internal bool UseVNC
        {
            get
            {
                return useVNC;
            }
            set
            {
                if (value != useVNC)
                {
                    ConnectionRetries = 0;
                    useVNC = value;
                    scaling = false;
                    initSubControl();
                    // Check if we have really switched. If not, change useVNC back (CA-102755)
                    bool switched = true;
                    if (useVNC) // we wanted VNC
                    {
                        if (vncClient == null && rdpClient != null) // it is actually RDP
                            switched = false;
                    }
                    else // we wanted RDP
                    {
                        if (rdpClient == null && vncClient != null) // it is actually VNC
                            switched = false;
                    }
                    if (!switched) 
                    {
                        useVNC = !useVNC;
                    } 
                }
            }
        }

        private volatile bool useSource = true;
        /// <summary>
        /// Indicates whether to use the source or the detected vncIP
        /// </summary>
        public bool UseSource
        {
            get
            {
                return useSource;
            }
            set
            {
                if (value != useSource)
                {
                    useSource = value;
                    ConnectionRetries = 0;
                    initSubControl();
                }
            }
        }

        private VM Source
        {
            get
            {
                return this.sourceVM;
            }
            set
            {
                if (connectionPoller != null)
                {
                    connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
                    connectionPoller = null;
                }

                if (sourceVM != null)
                {
                    sourceVM.PropertyChanged -= new PropertyChangedEventHandler(VM_PropertyChanged);
                    sourceVM = null;
                }

                sourceVM = value;

                if (value != null)
                {
                    value.PropertyChanged += new PropertyChangedEventHandler(VM_PropertyChanged);

                    sourceIsPV = !value.IsHVM();
                    
                    startPolling();

                    lock (hostedConsolesLock)
                    {
                        hostedConsoles = Source.consoles;
                    }

                    VM_PropertyChanged(value, new PropertyChangedEventArgs("consoles"));
                }
            }
        }

        public string ConnectionName
        {
            get
            {
                if (Source == null)
                    return null;

                if (Source.IsControlDomainZero())
                    return string.Format(Messages.CONSOLE_HOST, Source.AffinityServerString());
                
                if (Source.is_control_domain)
                    return string.Format(Messages.CONSOLE_HOST_NUTANIX, Source.AffinityServerString());
                
                return Source.Name();
            }
        }

        private bool InDefaultConsole()
        {
            // Windows VMs: 
            // - UseVNC indicates whether to use the default desktop (true) or the remote desktop (false); 
            // - UseSource is true by default and not used
            // Linux VMs: 
            // - UseVNC is true by default and not used; 
            // - UseSource indicates whether to use the text console (true) or the graphical vnc console (false); 

            return UseVNC && UseSource;
        }

        private void startPolling()
        {
            //Disable the button first, but only if in text/default console (to allow user to return to the text console - ref. CA-70314)
            if (InDefaultConsole())
            {
                parentVNCTabView.DisableToggleVNCButton();
            }

            if (!parentVNCTabView.IsRDPControlEnabled())
            {
                if (InDefaultConsole())
                {
                    parentVNCTabView.DisableToggleVNCButton();
                }

                //Start the polling again
                if (Source != null && !Source.IsControlDomainZero())
                {
                    if (!Source.IsHVM())
                    {
                        connectionPoller = new Timer(PollVNCPort, null, RETRY_SLEEP_TIME, RDP_POLL_INTERVAL);
                    }
                    else if (hasRDP)
                    {
                        connectionPoller = new Timer(PollRDPPort, null, RETRY_SLEEP_TIME, RDP_POLL_INTERVAL);
                    }
                }
            }
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            VM vm = (VM)sender;

            if (vm.uuid != Source.uuid)
                return;

            //We only want to reconnect the consoles when they change 
            //if the vm is running and if we are using vncTerm / qemu
            if (e.PropertyName == "consoles" && vm.power_state == vm_power_state.Running && UseSource)
            {
                ConnectNewHostedConsole();
            }
            //If the consoles change under us then refresh hostedConsoles
            else if (e.PropertyName == "consoles" && vm.power_state == vm_power_state.Running && !UseSource)
            {
                lock (hostedConsolesLock)
                {
                    hostedConsoles = Source.consoles;
                }
            }
            //Or if the VM legitimately turns on
            else if (e.PropertyName == "power_state" && vm.power_state == vm_power_state.Running)
            {
                parentVNCTabView.VMPowerOn();
                ConnectNewHostedConsole();
                if (connectionPoller != null)
                    connectionPoller.Change(RETRY_SLEEP_TIME, RDP_POLL_INTERVAL);
            }
            else if (e.PropertyName == "power_state" &&
                (vm.power_state == vm_power_state.Halted || vm.power_state == vm_power_state.Suspended))
            {
                parentVNCTabView.VMPowerOff();
                if (connectionPoller != null)
                    connectionPoller.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else if (e.PropertyName == "domid")
            {
                // Reboot / start / shutdown
                Program.Invoke(this, startPolling);
            }

            if (e.PropertyName == "power_state" || e.PropertyName == "VGPUs")
            {
                Program.Invoke(this, () =>
                {
                    if (GpuStatusChanged != null)
                        GpuStatusChanged(MustConnectRemoteDesktop());
                });
            }

            if (e.PropertyName == "name_label" && ConnectionNameChanged != null)
                ConnectionNameChanged(ConnectionName);
        }

        internal void imediatelyPollForConsole()
        {
            if (connectionPoller != null)
                connectionPoller.Change(0, RDP_POLL_INTERVAL);
        }

        private void ConnectNewHostedConsole()
        {
            Program.AssertOnEventThread();

            lock (hostedConsolesLock)
            {
                hostedConsoles = Source.consoles;
            }
            if (UseVNC && vncClient != null && ConnectionSuperceded())
            {
                initSubControl();
            }
        }

        /// <summary>
        /// A connection is superceded if it's connected to a console that's no longer being
        /// advertised by the server and there's a replacement that _is_ being advertised, or
        /// if its not connected at all.
        /// 
        /// The server will leave the console open until we close, so that the user may see any crash messages.
        /// For this reason, we need to close down ourselves when we see that the console has
        /// been replaced by a newer one (i.e. after a reboot).
        /// </summary>
        private bool ConnectionSuperceded()
        {
            return !vncClient.Connected || ConsoleSuperceded((Console)vncClient.Console);
        }

        private bool ConsoleSuperceded(Console old_console)
        {
            if (old_console == null)
                return true;

            List<Console> consoles;
            lock (hostedConsolesLock)
            {
                consoles = Source.Connection.ResolveAll(hostedConsoles);
            }
            bool good_console = false;
            foreach (Console console in consoles)
            {
                if (console.opaque_ref == old_console.opaque_ref &&
                    console.location == old_console.location)
                    return false;
                else if (console.protocol == console_protocol.rfb)
                    good_console = true;
            }
            return good_console;
        }

        /// <summary>
        /// CA-11201: GUI logs are being massively spammed. Prevent "INTERNAL_ERROR Host has disappeared"
        /// appearing more than once.
        /// </summary>
        private bool _suppressHostGoneMessage = false;
        private void Connect(object o)
        {
            if (Program.RunInAutomatedTestMode)
                return;

            Program.AssertOffEventThread();

            KeyValuePair<VNCGraphicsClient, Exception> kvp = (KeyValuePair<VNCGraphicsClient, Exception>)o;

            VNCGraphicsClient v = kvp.Key;
            Exception error = kvp.Value;

            try
            {
                if (UseSource)
                {
                    List<Console> consoles;
                    lock (hostedConsolesLock)
                    {
                        consoles = sourceVM.Connection.ResolveAll(hostedConsoles);
                    }

                    foreach (Console console in consoles)
                    {
                        if (vncClient != v)
                        {
                            // We've been replaced.  Give up.
                            return;
                        }

                        if (console.protocol == console_protocol.rfb)
                        {
                            try
                            {
                                ConnectHostedConsole(v, console);
                                return;
                            }
                            catch (Exception exn)
                            {
                                Failure failure = exn as Failure;
                                bool isHostGoneMessage = failure != null
                                    && failure.ErrorDescription.Count == 2
                                    && failure.ErrorDescription[0] == Failure.INTERNAL_ERROR
                                    && failure.ErrorDescription[1] == Messages.HOST_GONE;

                                if (isHostGoneMessage)
                                {
                                    if (!_suppressHostGoneMessage)
                                    {
                                        Log.Debug(exn, exn);
                                        _suppressHostGoneMessage = true;
                                    }
                                }
                                else
                                {
                                    Log.Debug(exn, exn);
                                    _suppressHostGoneMessage = false;
                                }
                            }
                        }
                    }

                    Log.Debug("Did not find any hosted consoles");
                    SleepAndRetryConnection(v);
                }
                else
                {
                    if (vncIP == null)
                    {
                        Log.DebugFormat("vncIP is null. Abort VNC connection attempt");
                        OnVncConnectionAttemptCancelled();
                        return;
                    }
                    this.vncPassword = Settings.GetVNCPassword(sourceVM.uuid);
                    if (this.vncPassword == null)
                    {
                        bool lifecycleOperationInProgress = sourceVM.current_operations.Values.Any(VM.is_lifecycle_operation);
                        if (haveTriedLoginWithoutPassword && !lifecycleOperationInProgress)
                        {
                            Program.Invoke(this, delegate
                            {
                                promptForPassword(ignoreNextError ? null : error);
                            });
                            ignoreNextError = false;
                            if (this.vncPassword == null)
                            {
                                Log.Debug("User cancelled VNC password prompt: aborting connection attempt");
                                OnUserCancelledAuth();
                                return;
                            }
                        }
                        else
                        {
                            Log.Debug("Attempting passwordless VNC login");
                            this.vncPassword = new char[0];
                            ignoreNextError = true;
                            haveTriedLoginWithoutPassword = true;
                        }
                    }

                    Stream s;
                    lock (pendingVNCConnectionLock)
                    {
                        s = pendingVNCConnection;
                        Log.DebugFormat("Using pending VNC connection");
                        pendingVNCConnection = null;
                    }
                    if (s == null)
                    {
                        Log.DebugFormat("Connecting to vncIP={0}, port={1}", this.vncIP, VNC_PORT);
                        s = connectGuest(this.vncIP, VNC_PORT, sourceVM.Connection);
                        Log.DebugFormat("Connected to vncIP={0}, port={1}", this.vncIP, VNC_PORT);
                    }
                    InvokeConnection(v, s, null);

                    // store the empty vnc password after a successful passwordless login
                    if (haveTriedLoginWithoutPassword && this.vncPassword.Length == 0)
                        Program.Invoke(this, () => Settings.SetVNCPassword(sourceVM.uuid, this.vncPassword)); 
                }
            }
            catch (Exception exn)
            {
                Log.Warn(exn, exn);
                SleepAndRetryConnection(v);
            }
        }

        private void promptForPassword(Exception error)
        {
            Program.AssertOnEventThread();

            // Prompt for password
            VNCPasswordDialog f = new VNCPasswordDialog(error, sourceVM);
            try
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    // Store password for next time
                    this.vncPassword = f.Password;
                    Settings.SetVNCPassword(sourceVM.uuid, this.vncPassword);
                }
                else
                {
                    // User cancelled
                }
            }
            finally
            {
                f.Dispose();
            }
        }

        private void OnUserCancelledAuth()
        {
            Program.Invoke(this, delegate
            {
                Log.Debug("User cancelled during VNC authentication");
                if (UserCancelledAuth != null)
                    UserCancelledAuth(this, null);
            });
        }

        private void OnVncConnectionAttemptCancelled()
        {
            Program.Invoke(this, delegate
            {
                Log.Debug("Cancelled VNC connection attempt");
                if (VncConnectionAttemptCancelled != null)
                    VncConnectionAttemptCancelled(this, null);
            });
        }

        private Stream connectGuest(string ip_address, int port, IXenConnection connection)
        {
            string uriString = String.Format("http://{0}:{1}/", ip_address, port);
            Log.DebugFormat("Trying to connect to: {0}", uriString);          
            return HTTP.ConnectStream(new Uri(uriString), XenAdminConfigManager.Provider.GetProxyFromSettings(connection), true, 0);           
        }

        private void ConnectHostedConsole(VNCGraphicsClient v, Console console)
        {
            Program.AssertOffEventThread();

            Host host = console.Connection.Resolve(Source.resident_on);
            if (host == null)
            {
                throw new Failure(Failure.INTERNAL_ERROR, Messages.HOST_GONE);
            }

            Uri uri = new Uri(console.location);
            string sesssionRef;

            lock (activeSessionLock)
            {
                // use the elevated credentials, if provided, for connecting to the console (CA-91132)
                activeSession = (string.IsNullOrEmpty(ElevatedUsername) || string.IsNullOrEmpty(ElevatedPassword)) ?
                    console.Connection.DuplicateSession() : console.Connection.ElevatedSession(ElevatedUsername, ElevatedPassword);
                sesssionRef = activeSession.opaque_ref;
            }

            Stream stream = HTTPHelper.CONNECT(uri, console.Connection, sesssionRef, false);

            InvokeConnection(v, stream, console);
        }

        private void InvokeConnection(VNCGraphicsClient v, Stream stream, Console console)
        {
            Program.Invoke(this, delegate()
            {
                // This is the last chance that we have to make sure that we've not already
                // connected this VNCGraphicsClient.  Now that we are back on the event thread,
                // we're guaranteed that no-one will beat us to the v.connect() call.  We
                // hand over responsibility for closing the stream at that point, so we have to
                // close it ourselves if the client is already connected.
                if (v.Connected || v.Terminated)
                {
                    stream.Close();
                }
                else
                {
                    v.SendScanCodes = UseSource && !this.sourceIsPV;
                    v.SourceVM = sourceVM;
                    v.Console = console;
                    v.UseQemuExtKeyEncoding = sourceVM != null && Helpers.InvernessOrGreater(sourceVM.Connection);
                    v.connect(stream, this.vncPassword);
                }
            });
        }

        private void RetryConnection(VNCGraphicsClient v, Exception exn)
        {
            if (vncClient == v && !v.Terminated && Source.power_state == vm_power_state.Running)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Connect), new KeyValuePair<VNCGraphicsClient, Exception>(v, exn));
            }
        }

        public void SendCAD()
        {
            Program.AssertOnEventThread();

            if (RemoteConsole != null)
            {
                RemoteConsole.SendCAD();
            }
        }

        private String errorMessage = null;

        private void ErrorHandler(object sender, Exception exn)
        {
            Program.AssertOffEventThread();

            if (this.Disposing || this.IsDisposed)
                return;

            Program.Invoke(this, delegate()
            {
                VNCGraphicsClient v = (VNCGraphicsClient)sender;

                if (exn is VNCAuthenticationException || exn is CryptographicException)
                {
                    Log.Debug(exn, exn);

                    // Clear the stored VNC password for this server.
                    Settings.SetVNCPassword(sourceVM.uuid, null);
                    RetryConnection(v, exn);
                }
                else if (exn is IOException || exn is Failure)
                {
                    Log.Debug(exn, exn);
                    SleepAndRetryConnection_(v);
                }
                else
                {
                    Log.Warn(exn, exn);
                    this.errorMessage = exn.Message;
                }
            });
        }

        private void SleepAndRetryConnection_(VNCGraphicsClient v)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SleepAndRetryConnection), v);
        }

        private void SleepAndRetryConnection(object o)
        {
            VNCGraphicsClient v = (VNCGraphicsClient)o;

            Program.AssertOffEventThread();

            ConnectionRetries++;
            Thread.Sleep(ConnectionRetries < SHORT_RETRY_COUNT ? SHORT_RETRY_SLEEP_TIME : RETRY_SLEEP_TIME);
            RetryConnection(v, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.errorMessage != null)
            {
                SizeF size = e.Graphics.MeasureString(this.errorMessage, this.Font);
                e.Graphics.DrawString(this.errorMessage, this.Font, Brushes.Black,
                    ((this.Width - size.Width) / 2), ((this.Height - size.Height) / 2));
            }

            // draw focus rectangle
            if (DisplayFocusRectangle && this.ContainsFocus && RemoteConsole != null)
            {
                Rectangle focusRect = Rectangle.Inflate(RemoteConsole.ConsoleBounds, VNCGraphicsClient.BORDER_PADDING / 2,
                                                    VNCGraphicsClient.BORDER_PADDING / 2);
                using (Pen pen = new Pen(focusColor, VNCGraphicsClient.BORDER_WIDTH))
                {
                    if (this.Focused)
                        pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, focusRect);
                }
            }
        }

        // Save this for when we init a new vncClient.
        private bool displayFocusRectangle = true;
        public bool DisplayFocusRectangle
        {
            get { return displayFocusRectangle; }
            set
            {
                displayFocusRectangle = value;
                if (RemoteConsole != null)
                {
                    RemoteConsole.DisplayBorder = displayFocusRectangle;
                }
            }
        }

        internal Image Snapshot()
        {
            if (RemoteConsole != null)
                return RemoteConsole.Snapshot();

            return null;
        }

        internal void RefreshScreen()
        {
            Program.AssertOnEventThread();
            if (RemoteConsole != null && RemoteConsole.ConsoleControl != null)
            {
                RemoteConsole.ConsoleControl.Refresh();
            }
            Invalidate();
            Update();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnGotFocus(e);

            RefreshScreen();

        }

        protected override void OnLostFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnLostFocus(e);

            this.pressedKeys = new Set<Keys>();

            // reset tab stop
            SetKeyboardAndMouseCapture(autoCaptureKeyboardAndMouse);

            RefreshScreen();
        }

        protected override void OnEnter(EventArgs e)
        {
            Program.AssertOnEventThread(); 
            base.OnEnter(e);

            CaptureKeyboardAndMouse();
            RefreshScreen();
        }

        protected override void OnLeave(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnLeave(e);

            // reset tab stop
            SetKeyboardAndMouseCapture(autoCaptureKeyboardAndMouse);

            RefreshScreen();
        }

        internal void UncaptureKeyboardAndMouse()
        {
            if (autoCaptureKeyboardAndMouse)
            {
                SetKeyboardAndMouseCapture(false);
            }
            ActiveControl = null;
            
            EnableMenuShortcuts();
        }

        internal void CaptureKeyboardAndMouse()
        {
            if (RemoteConsole != null)
            {
                RemoteConsole.Activate();
                if (autoCaptureKeyboardAndMouse)
                {
                    SetKeyboardAndMouseCapture(true);
                }
                Unpause();
            }

            DisableMenuShortcuts();
        }

        public static void DisableMenuShortcuts()
        {
            Program.MainWindow.MenuShortcutsEnabled = false;
        }

        public static void EnableMenuShortcuts()
        {
            Program.MainWindow.MenuShortcutsEnabled = true;
        }

        private Set<Keys> pressedKeys = new Set<Keys>();
        private bool modifierKeyPressedAlone = false;
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            bool down = ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN));

            Keys key = keyData;

            if ((key & Keys.Control) == Keys.Control)
                key = key & ~Keys.Control;

            if ((key & Keys.Alt) == Keys.Alt)
                key = key & ~Keys.Alt;

            if ((key & Keys.Shift) == Keys.Shift)
                key = key & ~Keys.Shift;

            // use TranslateKeyMessage to identify if Left or Right modifier keys have been pressed/released
            Keys extKey = ConsoleKeyHandler.TranslateKeyMessage(msg);

            return Keysym(down, key, extKey);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            e.Handled = Keysym(false, e.KeyCode, e.KeyCode);
        }

        private bool Keysym(bool pressed, Keys key, Keys extendedKey)
        {
            if (!pressed && pressedKeys.Count == 0) // we received key-up, but not key-down - ignore
                return true;

            if (KeyHandler.handleExtras<Keys>(pressed, pressedKeys, KeyHandler.ExtraKeys, extendedKey, KeyHandler.ModifierKeys, ref modifierKeyPressedAlone))
            {
                this.Focus();
                return true;
            }

            // on keyup, try to remove extended keys (i.e. LControlKey, LControlKey, RShiftKey, LShiftKey, RMenu, LMenu)
            // we need to do this here, because we cannot otherwise distinguish between Left and Right modifier keys on KeyUp
            if (!pressed)
            {
                List<Keys> extendedKeys = ConsoleKeyHandler.GetExtendedKeys(key);
                foreach (var k in extendedKeys)
                {
                    pressedKeys.Remove(k);
                }
            }

            if (key == Keys.Tab || (key == (Keys.Tab | Keys.Shift)))
                return false;           

            return false;
        }

        private Size oldSize;
        public void UpdateRDPResolution(bool fullscreen = false)
        {
            if (rdpClient == null || oldSize.Equals(this.Size))
                return;

            //no offsets in fullscreen mode because there is no need to accomodate focus border 
            if (fullscreen)
                rdpClient.UpdateDisplay(this.Size.Width, this.Size.Height, new Point(0,0));
            else
                rdpClient.UpdateDisplay(this.Size.Width - CONSOLE_SIZE_OFFSET, this.Size.Height - CONSOLE_SIZE_OFFSET, new Point(3,3));
            oldSize = new Size(this.Size.Width, this.Size.Height);
            Refresh();
        }
    }
}
