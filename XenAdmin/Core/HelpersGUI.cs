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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Alerts;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAPI;
using System.Linq;

namespace XenAdmin.Core
{
    public static class HelpersGUI
    {

        private static Image[] progress_images = new Image[11];

        static HelpersGUI()
        {
            progress_images[0] = Properties.Resources.usagebar_0;
            progress_images[1] = Properties.Resources.usagebar_1;
            progress_images[2] = Properties.Resources.usagebar_2;
            progress_images[3] = Properties.Resources.usagebar_3;
            progress_images[4] = Properties.Resources.usagebar_4;
            progress_images[5] = Properties.Resources.usagebar_5;
            progress_images[6] = Properties.Resources.usagebar_6;
            progress_images[7] = Properties.Resources.usagebar_7;
            progress_images[8] = Properties.Resources.usagebar_8;
            progress_images[9] = Properties.Resources.usagebar_9;
            progress_images[10] = Properties.Resources.usagebar_10;
        }

        internal static Image GetProgressImage(int pct)
        {
            int p = pct / 10;
            return 0 <= p && p < 11 ? progress_images[p] : progress_images[0];
        }

        public static bool WindowIsOnScreen(Point location, Size size)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    Rectangle r = new Rectangle(location, size);
                    return screen.WorkingArea.Contains(r);
                }
            }

            return false;
        }

        /// <summary>
        /// Find which items from a list need some action, and get permission to perform the action.
        /// </summary>
        /// <typeparam name="T">Type of the items</typeparam>
        /// <param name="items">List of items</param>
        /// <param name="pred">Condition under which the items need action</param>
        /// <param name="msg_single">Dialog message when one item needs action. {0} will be substituted for the name of the item.</param>
        /// <param name="msg_multiple">Dialog message when more than one item needs action. {0} will be substituted for a list of the items.</param>
        /// <param name="defaultYes">Whether the default button should be Proceed (as opposed to Cancel)</param>
        /// <param name="helpName">Help ID for the dialog</param>
        /// <param name="icon">Severity icon for the dialog</param>
        /// <returns>Whether permission was obtained (also true if no items needed action)</returns>
        public static bool GetPermissionFor<T>(List<T> items, Predicate<T> pred, string msg_single, string msg_multiple, bool defaultYes, string helpName, Icon icon = null)
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            List<T> itemsToFixup = items.FindAll(pred);
            if (itemsToFixup.Count > 0)
            {
                string msg;
                if (itemsToFixup.Count == 1)
                    msg = string.Format(msg_single, itemsToFixup[0]);
                else
                    msg = string.Format(msg_multiple, string.Join("\n", itemsToFixup.ConvertAll(item => item.ToString()).ToArray()));

                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(icon ?? SystemIcons.Exclamation, msg),
                    helpName,
                    new ThreeButtonDialog.TBDButton(Messages.PROCEED, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.No)))
                {
                    dialogResult = dlg.ShowDialog(Program.MainWindow);
                }

                return DialogResult.Yes == dialogResult;
            }
            return true;
        }

        public static int BALLOON_DURATION = 5000;
        public static void ShowBalloonMessage(Control control, string caption, ToolTip toolTip)
        {
            toolTip.Hide(control);
            toolTip.RemoveAll();
            toolTip.IsBalloon = true;
            toolTip.Active = true;
            toolTip.SetToolTip(control, caption); // required to improve the ballon position.
            toolTip.Show(caption, control, BALLOON_DURATION);
        }

        public static void ShowBalloonMessage(Control control, ToolTip toolTip)
        {
            ShowBalloonMessage(control, " ", toolTip);
        }

        /// <summary>
        /// Brings the sepcified form to the front.
        /// </summary>
        /// <param name="f">The form to be brought to the front.</param>
        public static void BringFormToFront(Form f)
        {
            if (f.WindowState == FormWindowState.Minimized)
            {
                f.WindowState = FormWindowState.Normal;
            }

            f.BringToFront();
            f.Activate();
        }

        public static bool FocusFirstControl(Control.ControlCollection cc)
        {
            bool found = false;

            List<Control> controls = new List<Control>();
            foreach (Control control in cc)
                controls.Add(control);
            controls.Sort((c1, c2) => c1.TabIndex.CompareTo(c2.TabIndex));
            if (controls.Count > 0)
            {
                foreach (Control control in controls)
                {
                    if (control.HasChildren)
                    {
                        found = FocusFirstControl(control.Controls);
                    }

                    if (!found)
                    {
                        if (control is Label)
                            continue;

                        if (control is TextBox && (control as TextBox).ReadOnly)
                            continue;

                        if (control.CanSelect)
                        {
                            found = control.Focus();
                        }
                    }

                    if (found)
                        break;
                }
            }

            return found;
        }

        public static Dictionary<Host, Host> CheckHostIQNsDiffer()
        {
            Dictionary<Host, string> hosts = new Dictionary<Host, string>();
            Dictionary<Host, string> hostduplicates = new Dictionary<Host, string>();
            Dictionary<Host, Host> output = new Dictionary<Host, Host>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host host in connection.Cache.Hosts)
                {
                    var iscsiIqn = host.GetIscsiIqn();
                    if (!hosts.ContainsValue(iscsiIqn) || iscsiIqn == "")
                        hosts.Add(host, iscsiIqn);
                    else
                        hostduplicates[host] = iscsiIqn;
                }
            }

            foreach (Host host in hostduplicates.Keys)
            {
                foreach (Host host2 in hosts.Keys)
                {
                    if (host.GetIscsiIqn() == host2.GetIscsiIqn())
                        output.Add(host, host2);
                }
            }
            return output;
        }

        public static List<Host> CheckHostIQNsExist()
        {
            List<Host> badHosts = new List<Host>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host host in connection.Cache.Hosts)
                {
                    Host_metrics metrics = connection.Resolve<Host_metrics>(host.metrics);
                    if (metrics != null && metrics.live && host.GetIscsiIqn() == "")
                        badHosts.Add(host);
                }
            }
            return badHosts;
        }

        /// <summary>
        /// Finds the EnableHAAction or DisableHAAction in progress that pertains to the given connection, or null
        /// if there is no such action.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveHaAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                EnableHAAction enableAction = action as EnableHAAction;
                if (enableAction != null && !enableAction.Cancelled && enableAction.Connection == connection)
                    return enableAction;

                DisableHAAction disableAction = action as DisableHAAction;
                if (disableAction != null && !disableAction.Cancelled && disableAction.Connection == connection)
                    return disableAction;
            }
            return null;
        }

        /// <summary>
        /// Does the given VM have any running tasks?  Must be called on the event thread.
        /// </summary>
        internal static bool HasRunningTasks(VM vm)
        {
            Program.AssertOnEventThread(); // Program.History is event-thread only

            if (vm == null)
                return false;

            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                if (!vm.Equals(action.VM))
                    continue;

                return true;
            }

            return false;
        }


        /// <summary>
        /// WLB Optimize Pool
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveOptAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                WlbOptimizePoolAction optAction = action as WlbOptimizePoolAction;
                if (optAction != null && optAction.Connection == connection)
                    return optAction;

                WlbRetrieveRecommendationAction optRecAction = action as WlbRetrieveRecommendationAction;
                if (optRecAction != null && optRecAction.Connection == connection)
                    return optRecAction;
            }
            return null;
        }

        /// <summary>
        /// Finds the WLBAction in progress that pertains to the given connection, or null
        /// if there is no such action.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveWLBAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                InitializeWLBAction configureAction = action as InitializeWLBAction;
                if (configureAction != null && !configureAction.Cancelled && configureAction.Connection == connection)
                    return configureAction;

                EnableWLBAction enableAction = action as EnableWLBAction;
                if (enableAction != null && !enableAction.Cancelled && enableAction.Connection == connection)
                    return enableAction;

                DisableWLBAction disableAction = action as DisableWLBAction;
                if (disableAction != null && !disableAction.Cancelled && disableAction.Connection == connection)
                    return disableAction;

                RetrieveWlbConfigurationAction retrieveAction = action as RetrieveWlbConfigurationAction;
                if (retrieveAction != null && !retrieveAction.Cancelled && retrieveAction.Connection == connection)
                    return retrieveAction;

                SendWlbConfigurationAction sendAction = action as SendWlbConfigurationAction;
                if (sendAction != null && !sendAction.Cancelled && sendAction.Connection == connection)
                    return sendAction;
            }
            return null;
        }

        /// <summary>
        /// Whether there is a HostAction in progress that pertains to the given host.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="host">May not be null.</param>
        /// <returns></returns>
        internal static bool HasActiveHostAction(Host host)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                 HostAbstractAction hostAction = action as HostAbstractAction;
                if (hostAction != null && !hostAction.Cancelled && !hostAction.IsCompleted && host.Connection == hostAction.Connection)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool iSCSIisUsed()
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (SR sr in connection.Cache.SRs)
                {
                    if (sr.GetSRType(false) == SR.SRTypes.lvmoiscsi)
                        return true;
                }
            }
            return false;
        }

        public static bool HAEnabledOnAtLeastOnePool
        {
            get
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                    if (Helpers.HAEnabled(connection))
                        return true;

                return false;
            }
        }


        internal static void PerformIQNCheck()
        {
            List<Host> missingIQNs = HelpersGUI.CheckHostIQNsExist();
            Dictionary<Host, Host> repeatedIQNs = HelpersGUI.CheckHostIQNsDiffer();

            foreach (Host host in missingIQNs)
            {
                MissingIqnAlert alert = new MissingIqnAlert(host);
                if (Alert.FindAlert(alert) == null)
                    Alert.AddAlert(alert);
            }

            foreach (Host host in repeatedIQNs.Keys)
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    DuplicateIqnAlert alert = new DuplicateIqnAlert(host, repeatedIQNs);
                    if (Alert.FindAlert(alert) == null)
                        Alert.AddAlert(alert);
                });
            }

        }

        public static bool GetActionInProgress(SR sr)
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted)
                {
                    if (a is SrAction && a.SR == sr)
                        return true;

                    EnableHAAction haAction = a as EnableHAAction;
                    if (haAction != null && haAction.HeartbeatSRs.Contains(sr))
                        return true;
                }

            }
            return false;
        }

        public static bool BeingScanned(SR sr)
        {
            foreach (ActionBase a in ConnectionsManager.History)
                {
                    if (!a.IsCompleted)
                    {
                        if (a is SrRefreshAction && a.SR == sr)
                            return true;
                    }
                }
                return false;
            
        }

        /// <summary>
        /// A wrapper around DateTime.ToString(), making sure that the resultant string is localised if and only if necessary.
        /// (What do we mean by localised here? Unlocalised means: in invariant culture, so the same whatever the language of
        /// the program and the OS. Localised means: in the language of the program. Localised doesn't mean: in the language
        /// of the OS (see CA-46983 for the rationale).
        /// </summary>
        /// <param name="dt">The DateTime to be stringified</param>
        /// <param name="format">The format string</param>
        /// <param name="localise">Whether the output should be localised</param>
        /// <returns></returns>
        public static string DateTimeToString(DateTime dt, string format, bool localise)
        {
            if (localise)
            {
                Program.AssertOnEventThread();  // otherwise it won't get localised: see CA-46983
                // If English, check for 'bad' formats: i.e., "standard" (single letter) formats,
                // and months in digits (M or MM, but MMM and MMMM are OK).
                Trace.Assert(!InvisibleMessages.LOCALE.StartsWith("en-") ||
                    format.Length > 1 && (!format.Contains("M") || format.Contains("MMM")),
                    "Bad date format");
                return dt.ToString(format);
            }
            else
                return dt.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the full name of the specified DayOfWeek,
        /// making sure that the resultant string is localised if and only if necessary.
        /// Localised means: in the language of the program (not in the language of the OS).
        /// </summary>
        public static string DayOfWeekToString(DayOfWeek dayOfWeek, bool localise = true)
        {
            if (localise)
            {
                Program.AssertOnEventThread();
                // get the day of the week localized to culture of the current thread
                return DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
            }
            
            return dayOfWeek.ToString();
        }

        /// <summary>
        /// Returns the abbreviated name of the specified DayOfWeek,
        /// making sure that the resultant string is localised if and only if necessary.
        /// Localised means: in the language of the program (not in the language of the OS).
        /// </summary>
        public static string DayOfWeekToShortString(DayOfWeek dayOfWeek, bool localise = true)
        {
            if (localise)
            {
                Program.AssertOnEventThread();
                // get the day of the week localized to culture of the current thread
                return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(dayOfWeek);
            }
            
            return dayOfWeek.ToString();
        }

        public static DateTime RoundToNearestQuarter(DateTime time)
        {
            var baseTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);

            if (time < baseTime.AddMinutes(7.5))
                return baseTime;
            if (baseTime.AddMinutes(7.5) <= time && time < baseTime.AddMinutes(22.5))
                return baseTime.AddMinutes(15);
            if (baseTime.AddMinutes(22.5) <= time && time < baseTime.AddMinutes(37.5))
                return baseTime.AddMinutes(30);
            if (baseTime.AddMinutes(37.5) <= time && time < baseTime.AddMinutes(52.5))
                return baseTime.AddMinutes(45);
            else
                return baseTime.AddHours(1);
        }

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control control)
        {
            var msgSuspendUpdate = System.Windows.Forms.Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                                                                       IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumeDrawing(Control control)
        {
            IntPtr wparam = new IntPtr(1);
            var msgResumeUpdate = System.Windows.Forms.Message.Create(control.Handle, WM_SETREDRAW, wparam, IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
        }

        /// <summary>
        /// Remember to call this after populating the gridview
        /// </summary>
        public static void ResizeGridViewColumnToAllCells(DataGridViewColumn col)
        {
            //the last column of the gridviews used on these pages should be autosized to Fill, but should not
            //become smaller than a minimum width, which is chosen to be the column's contents (including header)
            //width. To find what this is set temporarily the column's autosize mode to AllCells.

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            int storedWidth = col.Width;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.MinimumWidth = storedWidth;
        }

        public static void ResizeGridViewColumnToHeader(DataGridViewTextBoxColumn col)
        {
            //the column should be autosized to Fill, but should not become smaller than a minimum
            //width, which here is chosen to be the column header width. To find what this width is 
            //set temporarily the column's autosize mode to ColumnHeader.

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            int storedWidth = col.Width;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.MinimumWidth = storedWidth;
        }

        /// <summary>
        /// Get the localized Search name
        /// </summary>
        public static string GetLocalizedSearchName(Search search)
        {
            return search.Query.QueryFilter is CustomFieldDateQuery && search.Grouping == null
                       ? DateTimeToString(((CustomFieldDateQuery) (search.Query.QueryFilter)).query,
                                          Messages.DATEFORMAT_DMY_HMS, true)
                       : search.Name;
        }

        public static bool GetPermissionForCpuFeatureLevelling(List<Host> hosts, Pool pool)
        {
            if (hosts == null || pool == null || !Helpers.DundeeOrGreater(pool.Connection))
                return true;

            List<Host> hostsWithFewerFeatures = hosts.Where(host => PoolJoinRules.HostHasFewerFeatures(host, pool)).ToList();
            List<Host> hostsWithMoreFeatures = hosts.Where(host => PoolJoinRules.HostHasMoreFeatures(host, pool)).ToList();

            if (hostsWithFewerFeatures.Count > 0 && hostsWithMoreFeatures.Count > 0)
            {
                return GetPermissionFor(hostsWithFewerFeatures.Union(hostsWithMoreFeatures).ToList(), host => true,
                     Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_AND_HOST_MESSAGE, Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_AND_HOST_MESSAGE_MULTIPLE, true, "PoolJoinCpuMasking");
            }

            if (hostsWithFewerFeatures.Count > 0)
                return GetPermissionFor(hostsWithFewerFeatures, host => true,
                    Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_MESSAGE, Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_MESSAGE_MULTIPLE, true, "PoolJoinCpuMasking");
            
            return GetPermissionFor(hostsWithMoreFeatures, host => true,
                Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_HOST_MESSAGE, Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_HOST_MESSAGE_MULTIPLE, true, "PoolJoinCpuMasking", SystemIcons.Information);
        }
    }
}
