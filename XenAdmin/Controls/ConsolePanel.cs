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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.ConsoleView;
using XenAPI;
using System.Drawing;
using XenAdmin.Help;


namespace XenAdmin.Controls
{
    public partial class ConsolePanel : UserControl, IControlWithHelp
    {
        private const int MAX_ACTIVE_VM_CONSOLES = 10;

        public VNCView activeVNCView;
        private Dictionary<VM, VNCView> vncViews = new Dictionary<VM, VNCView>();

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConsolePanel()
        {
            InitializeComponent();
            tableLayoutPanelRbac.Visible = false;
            ClearErrorMessage();
        }

        public virtual string HelpID => "TabPageConsole";

        public void PauseAllViews()
        {
            // We're going to pause all of our VNCViews here, as this gets called when the VNC tab is not selected.
            // The VNCView deals with undocked cases.

            foreach (VNCView vncView in vncViews.Values)
            {
                vncView.Pause();
            }
        }

        public void ResetAllViews()
        {
            vncViews.Clear();
        }

        public void UnpauseActiveView()
        {
            // We're going to explicitly pause all the consoles 
            // except the active one, then explicitly unpause the active one.

            foreach (VNCView vncView in vncViews.Values)
            {
                if (vncView != activeVNCView)
                    vncView.Pause();
            }

            if (activeVNCView != null)
            {
                activeVNCView.Unpause();
            }
        }

        /// <summary>
        /// Gives focus to the console, as if the user had clicked it.
        /// </summary>
        internal void FocusActiveView()
        {
            if (activeVNCView != null)
                activeVNCView.FocusConsole();
        }

        public void UpdateRDPResolution(bool fullscreen = false)
        {
            if (activeVNCView != null)
                activeVNCView.UpdateRDPResolution(fullscreen);
        }

        internal void setCurrentSource(VM source)
        {
            Program.AssertOnEventThread();

            // activeVNCView is going to change, so the current activeVNCView will become inactive
            // Start a timer for closing the inactive VNC connection after an interval (20 seconds)
            StartCloseVNCTimer(activeVNCView);

            tableLayoutPanelRbac.Visible = false;

            if (activeVNCView != null)
            {
                Controls.Remove(activeVNCView);
                activeVNCView = null;
            }

            if (source == null)
                return;

            List<Role> allowedRoles;
            if (RbacDenied(source, out allowedRoles))
            {
                string msg = allowedRoles.Count == 1 ? Messages.RBAC_CONSOLE_WARNING_ONE : Messages.RBAC_CONSOLE_WARNING_MANY;
                lableRbacWarning.Text = string.Format(msg,
                    Role.FriendlyCSVRoleList(source.Connection.Session.Roles),
                    Role.FriendlyCSVRoleList(allowedRoles));

                tableLayoutPanelRbac.Visible = true;
                return;
            }

            StopCloseVncTimer(source);

            //remove one more as we're adding the selected further down
            //Take(arg) returns empty list if the arg <= 0
            var viewsToRemove = vncViews.Where(v => v.Key.opaque_ref != source.opaque_ref).Take(vncViews.Count - 1 - MAX_ACTIVE_VM_CONSOLES).ToList();

            foreach (var view in viewsToRemove)
                closeVNCForSource(view.Key);

            if (vncViews.ContainsKey(source))
            {
                activeVNCView = vncViews[source];
            }
            else
            {
                activeVNCView = new VNCView(source, null, null) { Dock = DockStyle.Fill };
                vncViews[source] = activeVNCView;
            }

            activeVNCView.refreshIsoList();
            Controls.Add(activeVNCView);
            ClearErrorMessage();
        }

        internal virtual void setCurrentSource(Host source)
        {
            if (source == null)
            {
                log.Error("No local copy of host information when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_CONNECT_CONSOLE);
                return;
            }

            VM dom0 = source.ControlDomainZero();
            if (dom0 == null)
            {
                log.Error("No dom0 on host when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_FIND_CONSOLES);
            }
            else
                setCurrentSource(dom0);
        }

        public static bool RbacDenied(VM source, out List<Role> allowedRoles)
        {

            if (source == null || source.Connection == null)
            {
                allowedRoles = null;
                return false;
            }
            
            var session = source.Connection.Session;
            if (session == null || session.IsLocalSuperuser)
            {
                allowedRoles = null;
                return false;
            }

            string roleList = source.IsControlDomainZero() ? "http/connect_console/host_console" : "http/connect_console";
            List<Role> validRoles = Role.ValidRoleList(roleList, source.Connection);
            allowedRoles = validRoles;
            return session.Roles.Find(r => validRoles.Contains(r)) == null;
        }

        internal Image Snapshot(VM vm, string elevatedUsername, string elevatedPassword)
        {
            Program.AssertOffEventThread();

            VNCView view = null;

            bool useElevatedCredentials = false;

            if (!vncViews.ContainsKey(vm))
            {
                Program.Invoke(this, delegate
                {
                    // use elevated credentials, if provided, to create a vncView (CA-91132)
                    useElevatedCredentials = !String.IsNullOrEmpty(elevatedUsername) && !String.IsNullOrEmpty(elevatedPassword);
                    if (useElevatedCredentials)
                        view = new VNCView(vm, elevatedUsername, elevatedPassword) { Dock = DockStyle.Fill };
                    else
                    {
                        setCurrentSource(vm);
                        if (vncViews.ContainsKey(vm))
                            view = vncViews[vm];
                    }
                });
            }
            else
            {
                view = vncViews[vm];
            }

            if (view == null)
                return null;

            Image snapshot = view.Snapshot();

            // TODO: only pause the view if we're not currently using it.
            // view.Pause();

            if (useElevatedCredentials)
            {
                //used the elevated credentials for snapshot, need to close vnc when finished
                Program.Invoke(this, () => view.Dispose());
            }

            return snapshot;
        }

        public void closeVNCForSource(VM source)
        {
            Program.AssertOnEventThread();

            if (!vncViews.ContainsKey(source))
                return;

            VNCView vncView = vncViews[source];

            if (!vncView.isDocked)
                return;

            vncViews.Remove(source);
            vncView.Dispose();
        }

        public void closeVNCForSource(VM source, bool vncOnly)
        {
            if (!vncViews.ContainsKey(source) || vncViews[source] == null
                || (vncOnly && !vncViews[source].IsVNC))
                return;
            closeVNCForSource(source);
        }

        protected void SetErrorMessage(string message)
        {
            errorLabel.Text = message;
            tableLayoutPanelError.Visible = true;
            setCurrentSource((VM)null);
        }

        private void ClearErrorMessage()
        {
            tableLayoutPanelError.Visible = false;
        }

        public void SendCAD()
        {
            if (activeVNCView != null)
                activeVNCView.SendCAD();
        }

        internal void SwitchIfRequired()
        {
            if (activeVNCView != null)
                activeVNCView.SwitchIfRequired();
        }

        #region Close VNC connection

        private const int CLOSE_VNC_INTERVAL = 20000; //20 milliseconds 

        private static readonly Dictionary<VM, Timer> CloseVNCTimers = new Dictionary<VM, Timer>();

        public void StartCloseVNCTimer(VNCView vncView)
        {
            if (vncView == null)
                return;

            // find the <VM, VNCView> pair in vncViews and start timer on the vm
            var views = vncViews.Where(kvp => kvp.Value == vncView).ToList();
            foreach (var kvp in views)
            {
                StartCloseVNCTimer(kvp.Key);
                break;
            }
        }

        private void StartCloseVNCTimer(VM vm)
        {
            Program.AssertOnEventThread();

            if (CloseVNCTimers.ContainsKey(vm) || !vncViews.ContainsKey(vm))
                return;

            var t = new Timer {Interval = CLOSE_VNC_INTERVAL};

            t.Tick += delegate
                          {
                              Program.AssertOnEventThread();
                              try
                              {
                                  log.DebugFormat("ConsolePanel: closeVNCForSource({0}) in delegate", vm.Name());
                                  closeVNCForSource(vm, true);
                              }
                              catch (Exception exception)
                              {
                                  log.ErrorFormat("ConsolePanel: Exception closing the VNC console for {0}: {1}",
                                                  vm.Name(), exception.Message);
                              }

                              t.Stop();
                              CloseVNCTimers.Remove(vm);
                              log.DebugFormat(
                                  "ConsolePanel: CloseVNCTimer({0}): Timer stopped and removed in delegate",
                                  vm.Name());
                          };

            CloseVNCTimers.Add(vm, t);
            log.DebugFormat("ConsolePanel: CloseVNCTimer({0}): Start timer (timers count {1})", vm.Name(), CloseVNCTimers.Count);
            t.Start();
        }

        private static void StopCloseVncTimer(VM vm)
        {
            Program.AssertOnEventThread();

            if (!CloseVNCTimers.ContainsKey(vm) || CloseVNCTimers[vm] == null) 
                return;

            CloseVNCTimers[vm].Stop();
            CloseVNCTimers.Remove(vm);
            log.DebugFormat("ConsolePanel: StopCloseVncTimer({0}): Timer stopped and removed", vm.Name());
        }

        #endregion
    }

    internal class CvmConsolePanel : ConsolePanel
    {
        internal override void setCurrentSource(Host source)
        {
            if (source == null)
            {
                log.Error("No local copy of host information when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_CONNECT_CONSOLE);
                return;
            }

            VM cvm = source.OtherControlDomains().FirstOrDefault();
            if (cvm == null)
            {
                log.Error("Could not find CVM console on host.");
                SetErrorMessage(Messages.VNC_COULD_NOT_FIND_CONSOLES);
            }
            else
                setCurrentSource(cvm);
        }

        public override string HelpID => "TabPageCvmConsole";
    }
}
