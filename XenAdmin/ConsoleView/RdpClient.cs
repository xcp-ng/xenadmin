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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DotNetVnc;
using XenCenterLib;
using XenAdmin.RDP;

namespace XenAdmin.ConsoleView
{
    class RdpClient: IRemoteConsole
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Size size;

        private bool allowDisplayUpdate;

        private readonly ContainerControl parent;

        public bool needsRdpVersionWarning = false;

        /// <summary>
        /// http://msdn2.microsoft.com/en-us/library/aa383022(VS.85).aspx
        /// </summary>
        private MsRdpClient9 rdpClient9 = null;

        private MsRdpClient6 rdpClient6 = null;

        /// <summary>
        /// This will be equal to rdpClient9, if the DLL that we've got is version 8, otherwise equal to
        /// rdpClient6.
        /// </summary>
        private AxHost rdpControl = null;

        public event EventHandler OnDisconnected = null;

        internal RdpClient(ContainerControl parent, Size size, EventHandler resizeHandler)
        {
            this.parent = parent;
            this.size = size;
            try
            {
                rdpControl = rdpClient9 = new MsRdpClient9();
                RDPConfigure(size);

                //add event handler for when RDP display is resized
                rdpClient9.OnRemoteDesktopSizeChange += rdpClient_OnRemoteDesktopSizeChange;

                // CA-96135: Try adding rdpControl to parent.Controls list; this will throw exception when
                // MsRdpClient8 control cannot be created (there is no appropriate version of dll present)
                parent.Controls.Add(rdpControl);
                allowDisplayUpdate = true;
                needsRdpVersionWarning = false;
            }
            catch
            {
                //any problems: fall back without thinking too much
                if (parent.Controls.Contains(rdpControl))
                    parent.Controls.Remove(rdpControl);
                rdpClient9 = null;
                rdpControl = rdpClient6 = new MsRdpClient6();
                RDPConfigure(size);
                parent.Controls.Add(rdpControl);
                needsRdpVersionWarning = true;
            }
            rdpControl.Resize += resizeHandler;
        }

        private void RDPConfigure(Size currentConsoleSize)
        {
            rdpControl.BeginInit();
            rdpLocationOffset = new Point(3, 3); //small offset to accomodate focus rectangle
            rdpControl.Dock = DockStyle.None;
            rdpControl.Anchor = AnchorStyles.None;
            rdpControl.Size = currentConsoleSize;
            RDPAddOnDisconnected();
            rdpControl.Enter += RdpEnter;
            rdpControl.Leave += rdpClient_Leave;
            rdpControl.GotFocus += rdpClient_GotFocus;
            rdpControl.EndInit();
        }


        public Point rdpLocationOffset
        {
            set 
            {
                if (rdpControl == null)
                    return;

                rdpControl.Location = value; 
            }
        }

        private void RDPAddOnDisconnected()
        {
            if (rdpControl == null)
                return;

            if (rdpClient9 == null)
                rdpClient6.OnDisconnected += rdpClient_OnDisconnected;
            else
                rdpClient9.OnDisconnected += rdpClient_OnDisconnected;
        }

        private void RDPSetSettings()
        {
            if (rdpControl == null)
                return;

            if (rdpClient9 == null)
            {
                rdpClient6.SecuredSettings2.KeyboardHookMode = Properties.Settings.Default.WindowsShortcuts ? 1 : 0;
                rdpClient6.SecuredSettings2.AudioRedirectionMode = Properties.Settings.Default.ReceiveSoundFromRDP ? 0 : 1;
                rdpClient6.AdvancedSettings3.DisableRdpdr = Properties.Settings.Default.ClipboardAndPrinterRedirection ? 0 : 1;
                rdpClient6.AdvancedSettings7.ConnectToAdministerServer = Properties.Settings.Default.ConnectToServerConsole;
                //CA-103910 - enable NLA 
                rdpClient6.AdvancedSettings5.AuthenticationLevel = 2;
                rdpClient6.AdvancedSettings7.EnableCredSspSupport = true;
            }
            else
            {
                rdpClient9.SecuredSettings2.KeyboardHookMode = Properties.Settings.Default.WindowsShortcuts ? 1 : 0;
                rdpClient9.SecuredSettings2.AudioRedirectionMode = Properties.Settings.Default.ReceiveSoundFromRDP ? 0 : 1;
                rdpClient9.AdvancedSettings3.DisableRdpdr = Properties.Settings.Default.ClipboardAndPrinterRedirection ? 0 : 1;
                rdpClient9.AdvancedSettings7.ConnectToAdministerServer = Properties.Settings.Default.ConnectToServerConsole;
                //CA-103910 - enable NLA
                rdpClient9.AdvancedSettings5.AuthenticationLevel = 2;
                rdpClient9.AdvancedSettings7.EnableCredSspSupport = true;
            }
        }

        public void RDPConnect(string rdpIP, int w, int h)
        {
            if (rdpControl == null)
                return;

            if (rdpClient9 == null)
            {
                Log.Debug($"Connecting RDPClient6 using server '{rdpIP}', width '{w}' and height '{h}'");
                rdpClient6.Server = rdpIP;
                rdpClient6.DesktopWidth = w;
                rdpClient6.DesktopHeight = h;
                rdpClient6.Connect();
            }
            else
            {
                Log.Debug($"Connecting RDPClient9 using server '{rdpIP}', width '{w}' and height '{h}'");
                rdpClient9.Server = rdpIP;
                rdpClient9.DesktopWidth = w;
                rdpClient9.DesktopHeight = h;
                rdpClient9.Connect();
            }
        }

        public void UpdateDisplay(int width, int height, Point locationOffset)
        {
            if (rdpControl == null)
                return;

            if (Connected && rdpClient9 != null && allowDisplayUpdate)
            {
                try
                {
                    Log.DebugFormat("Updating display settings using width '{0}' and height '{1}'", width, height);
                    rdpClient9.UpdateSessionDisplaySettings((uint)width, (uint)height, (uint)width, (uint)height, 1, 1, 1);
                    rdpClient9.Size = new Size(width, height);
                    rdpLocationOffset = locationOffset;
                    parent.AutoScroll = false;
                }
                catch
                {
                    allowDisplayUpdate = false;
                    parent.AutoScroll = true;
                    parent.AutoScrollMinSize = rdpClient9.Size;
                }
            }
        }

        private bool Connected
        {
            get { return rdpControl == null ? false : (rdpClient9 == null ? rdpClient6.Connected == 1 : rdpClient9.Connected == 1); }
        }

        private int DesktopHeight
        {
            get { return rdpControl == null ? 0 : (rdpClient9 == null ? rdpClient6.DesktopHeight : rdpClient9.DesktopHeight); }
        }

        private int DesktopWidth
        {
            get { return rdpControl == null ? 0 : (rdpClient9 == null ? rdpClient6.DesktopWidth : rdpClient9.DesktopWidth); }
        }

        private static readonly List<System.Windows.Forms.Timer> RdpCleanupTimers = new List<System.Windows.Forms.Timer>();
        void rdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Program.AssertOnEventThread();

            if (OnDisconnected != null)
                OnDisconnected(this, null);

        }

        //refresh to draw focus border in correct position after display is updated
        void rdpClient_OnRemoteDesktopSizeChange(object sender, AxMSTSCLib.IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            Program.AssertOnEventThread();

            if (rdpControl == null || parent == null)
                return;

            rdpControl.Size = DesktopSize;
            parent.Refresh();
        }

        public void Connect(string rdpIP)
        {
            try
            {
                RDPSetSettings();
            }
            catch (Exception ex)
            {
                if (parent.Controls.Contains(rdpControl))
                    parent.Controls.Remove(rdpControl);
                rdpControl.Dispose();
                rdpControl = null;
                Log.Error("Setting the RDP client properties caused an exception.", ex);
            } 
            RDPConnect(rdpIP, size.Width, size.Height);
        }

        public void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    if (rdpClient9 == null)
                        rdpClient6.Disconnect();
                    else
                        rdpClient9.Disconnect();
                }  
            }
            catch(InvalidComObjectException ex)
            {
                Log.ErrorFormat("Disconnecting RdpClient caused an exception: {0}, {1}", ex.Message, ex.StackTrace);
            }
            catch(AccessViolationException ex)
            {
                //We seem (often unpredictably) to get a read/write into protected memory while disposing the client eg: CA-91482
                Log.ErrorFormat("Disconnecting RdpClient caused an AccessViolationException: {0}, {1}", ex.Message, ex.StackTrace);
            }
            catch(NullReferenceException ex)
            {
                //Sometimes rdpClient.Disconnect() crashes with NullReferenceException eg: CA-94062
                Log.ErrorFormat("Disconnecting RdpClient caused a NullReferenceException: {0}, {1}", ex.Message, ex.StackTrace);
            }
        }

        internal Set<int> pressedScans = new Set<int>();
        private bool modifierKeyPressedAlone = false;

        private void handleRDPKey(bool pressed, int scancode, int keysym)
        {
            bool containsFocus = parent.ParentForm != null && parent.ParentForm.ContainsFocus;

            if (rdpControl == null || !containsFocus)
                return;

            if (KeyHandler.handleExtras<int>(pressed, pressedScans, KeyHandler.ExtraScans, scancode, KeyHandler.ModifierScans, ref modifierKeyPressedAlone))
                parent.Focus();
        }

        void rdpClient_Leave(object sender, EventArgs e)
        {
            Program.MainWindow.MenuShortcutsEnabled = true;
            InterceptKeys.releaseKeys();
            pressedScans = new Set<int>();
        }

        void RdpEnter(object sender, EventArgs e)
        {
            Activate();
        }

        void rdpClient_GotFocus(object sender, EventArgs e)
        {
            Activate();
        }

        #region IRemoteConsole implementation

        public ConsoleKeyHandler KeyHandler
        {
            get;
            set;
        }

        public Control ConsoleControl
        {
            get { return rdpControl; }
        }

        public void Activate()
        {
            Program.MainWindow.MenuShortcutsEnabled = false;
            if (rdpControl != null)
            {
                if (!rdpControl.Focused)
                    rdpControl.Select();

                InterceptKeys.releaseKeys();
                InterceptKeys.grabKeys(new InterceptKeys.KeyEvent(handleRDPKey), true);
            }
        }

        public void DisconnectAndDispose()
        {
            try
            {
                Disconnect();
            }
            catch
            {
            }

            try
            {
                Dispose();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        private bool disposed;
        public void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if (rdpControl != null)
                    {
                        // We need to dispose the rdp control. However, doing it immediately (in the control's own
                        // OnDisconnected event) will cause a horrible crash. Instead, start a timer that will
                        // call the dispose method on the GUI thread at the next available opportunity. CA-12902
                        Timer t = new Timer();
                        t.Tick += delegate
                                      {
                                          try
                                          {
                                              Log.Debug("RdpClient Dispose(): rdpControl.Dispose() in delegate");
                                              rdpControl.Dispose();
                                          }
                                          catch (Exception)
                                          {
                                              // We often get NullReferenceException here
                                          }
                                          t.Stop();
                                          RdpCleanupTimers.Remove(t);
                                          Log.Debug("RdpClient Dispose(): Timer stopped and removed in delegate");
                                      };
                        t.Interval = 1;
                        RdpCleanupTimers.Add(t);
                        Log.DebugFormat("RdpClient Dispose(): Start timer (timers count {0})", RdpCleanupTimers.Count);
                        t.Start();
                    }
                    else
                        Log.Debug("RdpClient Dispose(): rdpControl == null");
                }
                rdpControl = null;
                Log.Debug("RdpClient Dispose(): disposed = true");
                disposed = true;
            }
        }

        public void Pause()
        {
        }

        public void Unpause()
        {
        }

        public void SendCAD()
        {
        }

        public Image Snapshot()
        {
            return null;
        }

        public bool SendScanCodes
        {
            set { }
        }

        public bool Scaling
        {
            get;
            set;
        }

        public bool DisplayBorder
        {
            set { }
        }
        
        public Size DesktopSize
        {
            get { return rdpControl != null ? new Size(DesktopWidth, DesktopHeight) /*rdpControl.Size*/ : Size.Empty; }
            set { }
        }

        public Rectangle ConsoleBounds
        {
            get
            {
                return rdpControl != null ? new Rectangle(rdpControl.Location.X, rdpControl.Location.Y, DesktopWidth, DesktopHeight) : Rectangle.Empty;
            }
        }

        #endregion
    }
}
