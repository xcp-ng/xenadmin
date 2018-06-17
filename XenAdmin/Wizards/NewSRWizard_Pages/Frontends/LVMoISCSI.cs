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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Threading;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using XenAdmin.Utils;
using XenCenterLib;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoISCSI : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// If an SR already exists on this LUN, this will point to the SR info which indicates that 
        /// the SR should be reattached.
        /// 
        /// If this is null, indicates the LUN should be formatted into a new SR.
        /// </summary>
        private SR.SRInfo _srToIntroduce;

        private ISCSIPopulateLunsAction IscsiPopulateLunsAction;
        private ISCSIPopulateIQNsAction IscsiPopulateIqnsAction;

        private readonly Dictionary<String, ISCSIInfo> LunMap = new Dictionary<String, ISCSIInfo>();

        private readonly ToolTip TargetIqnToolTip = new ToolTip();

        private const string TARGET = "target";
        private const string PORT = "port";
        private const string TARGETIQN = "targetIQN";
        private const string LUNSERIAL = "LUNSerial";
        private const string SCSIID = "SCSIid";
        private const string CHAPUSER = "chapuser";
        private const string CHAPPASSWORD = "chappassword";

        private IEnumerable<Control> ErrorIcons
        {
            get { return new Control[] {errorIconAtCHAPPassword, errorIconAtHostOrIP, errorIconAtTargetLUN }; }
        }

        private IEnumerable<Control> ErrorLabels
        {
            get { return new Control[] { errorLabelAtHostname, errorLabelAtCHAPPassword, errorLabelAtTargetLUN }; }
        }

        private IEnumerable<Control> SpinnerControls
        {
            get { return new Control[] { spinnerIconAtScanTargetHostButton, spinnerIconAtTargetIqn, spinnerIconAtTargetLun }; }
        }

        private IEnumerable<Control> UserInputControls
        {
            get
            {
                return new Control[]
                           {
                               textBoxIscsiHost, textBoxIscsiPort, IscsiUseChapCheckBox, IScsiChapUserTextBox,
                               IScsiChapSecretTextBox, scanTargetHostButton, comboBoxIscsiIqns, comboBoxIscsiLuns
                           }; 
            }
        }

        private readonly TemporaryDisablerForControls controlDisabler = new TemporaryDisablerForControls();
        
        public LVMoISCSI()
        {
            InitializeComponent();
            SrType = SR.SRTypes.lvmoiscsi;
        }

        #region XentabPage overrides

        public override string PageTitle { get { return Messages.NEWSR_PATH_ISCSI; } }

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string HelpID { get { return "Location_ISCSI"; } }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            HelpersGUI.PerformIQNCheck();

            if (direction == PageLoadedDirection.Forward)
                HelpersGUI.FocusFirstControl(Controls);
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            // For Miami hosts we need to ensure an SR.probe()
            // has been performed, and that the user has made a decision. Show the iSCSI choices dialog until
            // they click something other than 'Cancel'. For earlier host versions, warn that data loss may
            // occur.

            Host master = Helpers.GetMaster(Connection);
            if (master == null)
            {
                cancel = true;
                return;
            }

            // Start probe

            List<SR.SRInfo> srs;
            var currentSrType = SrType;

            if (!RunProbe(master, currentSrType, out srs))
            {
                cancel = iscsiProbeError = true;
                return;
            }

            var performSecondProbe = Helpers.KolkataOrGreater(Connection) &&
                                     !Helpers.FeatureForbidden(Connection, Host.CorosyncDisabled);
            if (performSecondProbe && srs.Count == 0)
            {
                // Start second probe
                currentSrType = SrType == SR.SRTypes.gfs2 ? SR.SRTypes.lvmoiscsi : SR.SRTypes.gfs2;

                if (!RunProbe(master, currentSrType, out srs))
                {
                    cancel = iscsiProbeError = true;
                    return;
                }
            }

            // Probe has been performed. Now ask the user if they want to Reattach/Format/Cancel.
            // Will return false on cancel
            cancel = iscsiProbeError = !ExamineIscsiProbeResults(currentSrType, srs);
        }

        private bool RunProbe(Host master, SR.SRTypes srType, out List<SR.SRInfo> srs)
        {
            srs = null;

            Dictionary<String, String> dconf = GetDeviceConfig(srType);
            if (dconf == null)
                return false;

            var action = new SrProbeAction(Connection, master, srType, dconf);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            _srToIntroduce = null;
            if (action.Succeeded)
            {
                try
                {
                    srs = action.ProbeExtResult != null ? SR.ParseSRList(action.ProbeExtResult) : SR.ParseSRListXML(action.Result);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            Exception exn = action.Exception;
            log.Warn(exn, exn);

            Failure failure = exn as Failure;
            if (failure != null)
            {
                errorIconAtHostOrIP.Visible = true;
                errorLabelAtHostname.Visible = true;

                errorLabelAtHostname.Text = failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140"
                    ? Messages.INVALID_HOST
                    : (failure.ErrorDescription.Count > 2
                        ? failure.ErrorDescription[2]
                        : failure.ErrorDescription[0]);

                textBoxIscsiHost.Focus();
            }
            return false;
        }

        bool iscsiProbeError = false;
        public override bool EnableNext()
        {
            UInt16 i;
            bool portValid = UInt16.TryParse(textBoxIscsiPort.Text, out i);

            return !String.IsNullOrEmpty(getIscsiHost())
                && portValid
                && !(IscsiUseChapCheckBox.Checked && String.IsNullOrEmpty(IScsiChapUserTextBox.Text))
                && comboBoxIscsiLuns.SelectedItem != null && comboBoxIscsiLuns.SelectedItem as string != Messages.SELECT_TARGET_LUN
                && !iscsiProbeError && !IsLunInUse();
        }

        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            HideAllErrorIconsAndLabels();

            // Enable IQN scanning
            comboBoxIscsiIqns.Visible = true;
            
            // IQN's can be very long, so we will show the value as a mouse over tooltip.
            // Initialize the tooltip here.
            TargetIqnToolTip.Active = true;
            TargetIqnToolTip.AutomaticDelay = 0;
            TargetIqnToolTip.AutoPopDelay = 50000;
            TargetIqnToolTip.InitialDelay = 50;
            TargetIqnToolTip.ReshowDelay = 50;
            TargetIqnToolTip.ShowAlways = true;
        }

        #endregion

        private void UpdateButtons()
        {
            UInt16 i;
            bool portValid = UInt16.TryParse(textBoxIscsiPort.Text, out i);

            scanTargetHostButton.Enabled = 
                !String.IsNullOrEmpty(getIscsiHost())
                && portValid;

            // Cause wizards next etc to update
            OnPageUpdated();
        }

        private void textBoxIscsiHost_TextChanged(object sender, EventArgs e)
        {
            HideAllErrorIconsAndLabels();
            HideAllSpinnerIcons();
            IScsiParams_TextChanged(null, null);
        }

        /// <summary>
        /// Called when any of the iSCSI filer params change: resets the IQNs/LUNs.
        /// Must be called on the event thread.
        /// </summary>
        private void IScsiParams_TextChanged(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();

            spinnerIconAtScanTargetHostButton.Visible = false;

            // User has changed filer hostname/username/password - clear IQN/LUN boxes
            comboBoxIscsiIqns.Items.Clear();
            comboBoxIscsiIqns.Enabled = false;
            labelIscsiIQN.Enabled = false;
            iSCSITargetGroupBox.Enabled = false;

            // Cancel pending IQN/LUN scans
            if (IscsiPopulateIqnsAction != null)
            {
                IscsiPopulateIqnsAction.Cancel();
            }

            ChapSettings_Changed(null, null);
        }

        private bool IsLunInUse()
        {
            SR sr = UniquenessCheck();

            // LUN is not in use iff sr != null

            if (sr == null)
            {
                HideAllErrorIconsAndLabels();
                return false;
            }

            spinnerIconAtTargetLun.Visible = false;

            Pool pool = Helpers.GetPool(sr.Connection);
            if (pool != null)
            {
                errorIconAtTargetLUN.Visible = true;
                errorLabelAtTargetLUN.Visible = true;
                errorLabelAtTargetLUN.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_POOL, sr.Name(), pool.Name());
                return true;
            }

            Host master = Helpers.GetMaster(sr.Connection);
            if (master != null)
            {
                errorIconAtTargetLUN.Visible = true;
                errorLabelAtTargetLUN.Visible = true;
                errorLabelAtTargetLUN.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_SERVER, sr.Name(), master.Name());
                return true;
            }

            errorIconAtTargetLUN.Visible = true;
            errorLabelAtTargetLUN.Visible = true;
            errorLabelAtTargetLUN.Text = Messages.NEWSR_LUN_IN_USE;
            return true;
        }

        private void comboBoxIscsiLuns_SelectedIndexChanged(object sender, EventArgs e)
        {
            iscsiProbeError = false;
            if (comboBoxIscsiLuns.SelectedItem as string != Messages.SELECT_TARGET_LUN)
            {
                spinnerIconAtTargetLun.DisplaySucceededImage();
            }
            else
            {
                spinnerIconAtTargetLun.Visible = false;
                HideAllErrorIconsAndLabels();
            }

            UpdateButtons();
        }

        private void ChapSettings_Changed(object sender, EventArgs e)
        {
            comboBoxIscsiLuns.Items.Clear();
            comboBoxIscsiLuns.Text = "";
            comboBoxIscsiLuns.Enabled = false;
            targetLunLabel.Enabled = false;

            if (IscsiPopulateLunsAction != null)
            {
                IscsiPopulateLunsAction.Cancel();
            }

            UpdateButtons();
        }

        /// <summary>
        /// Check the current config of the iSCSI sr in the wizard is unique across
        /// all active connections.
        /// </summary>
        /// <returns>SR that uses this config if not unique, null if unique</returns>
        private SR UniquenessCheck()
        {
            // Check currently selected lun is unique amongst other connected hosts.
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (SR sr in connection.Cache.SRs)
                {
                    if (sr.GetSRType(false) != SR.SRTypes.lvmoiscsi && sr.GetSRType(false) != SR.SRTypes.gfs2)
                        continue;

                    if (sr.PBDs.Count < 1)
                        continue;

                    PBD pbd = connection.Resolve(sr.PBDs[0]);

                    if (pbd == null)
                        continue;

                    if (UniquenessCheckMiami(connection, pbd))
                        return sr;
                }
            }

            return null;
        }

        /// <summary>
        /// Check currently LUN against miami host
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pbd"></param>
        /// <returns></returns>
        private bool UniquenessCheckMiami(IXenConnection connection, PBD pbd)
        {
            String scsiID;

            if (pbd.device_config.ContainsKey(SCSIID))
                scsiID = pbd.device_config[SCSIID];
            else
                return false;

            String myLUN = getIscsiLUN();

            if (!LunMap.ContainsKey(myLUN))
                return false;

            ISCSIInfo info = LunMap[myLUN];

            return info.ScsiID == scsiID;
        }

        private String getIscsiHost()
        {
            // If the user has selected an IQN, use the host from that IQN (due to multi-homing,
            // this may differ from the host they first entered). Otherwise use the host
            // they first entered,
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper != null)
                return wrapper.item.IpAddress;
            return textBoxIscsiHost.Text.Trim();
        }

        private UInt16 getIscsiPort()
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper != null)
                return wrapper.item.Port;

            // No combobox item was selected
            UInt16 port;
            if (UInt16.TryParse(textBoxIscsiPort.Text, out port))
            {
                return port;
            }
            else
            {
                return Util.DEFAULT_ISCSI_PORT;
            }
        }

        private String getIscsiIQN()
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper == null)
                return "";
            else
                return wrapper.item.TargetIQN;
        }

        private void IScsiTargetIqnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;

            ClearLunMapAndCombo();
            HideAllErrorIconsAndLabels();

            if (wrapper != null)
            {
                TargetIqnToolTip.SetToolTip(comboBoxIscsiIqns, wrapper.ToString());
                IscsiPopulateLUNs();
            }
            else
            {
                TargetIqnToolTip.SetToolTip(comboBoxIscsiIqns, Messages.SELECT_TARGET_IQN);
            }
        }

        private String getIscsiLUN()
        {
            return comboBoxIscsiLuns.Text;
        }

        private void IscsiUseChapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = IscsiUseChapCheckBox.Checked;

            IScsiChapUserTextBox.Enabled = enabled;
            IScsiChapSecretTextBox.Enabled = enabled;
            labelCHAPuser.Enabled = enabled;
            IScsiChapSecretLabel.Enabled = enabled;

            HideAllErrorIconsAndLabels();
            ChapSettings_Changed(null, null);
        }

        private void scanTargetHostButton_Click(object sender, EventArgs e)
        {
            HideAllErrorIconsAndLabels();
            spinnerIconAtTargetIqn.Visible = false;
            spinnerIconAtTargetLun.Visible = false;

            spinnerIconAtScanTargetHostButton.StartSpinning();

            scanTargetHostButton.Enabled = false;
            // For this button to be enabled, we must be Miami or newer
            comboBoxIscsiIqns.Items.Clear();
            // Clear LUNs as they may no longer be valid
            ClearLunMapAndCombo();
            // Cancel any LUN scan in progress, as it is no longer meaningful
            if (IscsiPopulateLunsAction != null)
            {
                IscsiPopulateLunsAction.Cancel();
            }

            UpdateButtons();

            var chapUser = IscsiUseChapCheckBox.Checked ? IScsiChapUserTextBox.Text : null;
            var chapPwd = IscsiUseChapCheckBox.Checked ? IScsiChapSecretTextBox.Text : null;

            IscsiPopulateIqnsAction = SrType == SR.SRTypes.gfs2
                    ? new Gfs2PopulateIQNsAction(Connection, getIscsiHost(), getIscsiPort(), chapUser, chapPwd)
                    : new ISCSIPopulateIQNsAction(Connection, getIscsiHost(), getIscsiPort(), chapUser, chapPwd);
            
            IscsiPopulateIqnsAction.Completed += IscsiPopulateIqnsAction_Completed;

            controlDisabler.Reset();
            controlDisabler.SaveOrUpdateEnabledStates(UserInputControls);
            controlDisabler.DisableAllControls();
            
            scanTargetHostButton.Enabled = false;
            IscsiPopulateIqnsAction.RunAsync();
        }

        private void ClearLunMapAndCombo()
        {
            // Clear LUNs as they may no longer be valid
            comboBoxIscsiLuns.Items.Clear();
            comboBoxIscsiLuns.Text = "";
            comboBoxIscsiLuns.Enabled = false;
            targetLunLabel.Enabled = false;
            LunMap.Clear();
            spinnerIconAtTargetIqn.Visible = false;
            spinnerIconAtTargetLun.Visible = false;

            UpdateButtons();
        }

        private void IscsiPopulateIqnsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, (System.Threading.WaitCallback)IscsiPopulateIqnsAction_Completed_, sender);
        }

        private void IscsiPopulateIqnsAction_Completed_(object o)
        {
            Program.AssertOnEventThread();
            ISCSIPopulateIQNsAction action = (ISCSIPopulateIQNsAction)o;

            controlDisabler.RestoreEnabledOnAllControls();

            if (action.Succeeded)
            {
                if (action.IQNs.Length == 0)
                {
                    // Do nothing: ActionProgressDialog will show Messages.NEWSR_NO_IQNS_FOUND
                }
                else
                {
                    int width = comboBoxIscsiIqns.Width;

                    comboBoxIscsiIqns.Items.Add(Messages.SELECT_TARGET_IQN);

                    foreach (Actions.IScsiIqnInfo iqnInfo in action.IQNs)
                    {
                        if (!String.IsNullOrEmpty(iqnInfo.TargetIQN))
                        {
                            String toString = String.Format("{0} ({1}:{2})", iqnInfo.TargetIQN, iqnInfo.IpAddress, iqnInfo.Port);
                            comboBoxIscsiIqns.Items.Add(new ToStringWrapper<IScsiIqnInfo>(iqnInfo, toString));
                            width = Math.Max(width, Drawing.MeasureText(toString, comboBoxIscsiIqns.Font).Width);
                        }
                    }
                    // Set the combo box dropdown width to accommodate the widest item (within reason)
                    comboBoxIscsiIqns.DropDownWidth = Math.Min(width, Int16.MaxValue);

                    if (comboBoxIscsiIqns.Items.Count > 0)
                    {
                        comboBoxIscsiIqns.SelectedItem = Messages.SELECT_TARGET_IQN;
                        comboBoxIscsiIqns.Enabled = true;
                        labelIscsiIQN.Enabled = true;
                        iSCSITargetGroupBox.Enabled = true;
                    }

                    spinnerIconAtScanTargetHostButton.DisplaySucceededImage();

                    comboBoxIscsiIqns.Focus();
                }
            }
            else
            {
                spinnerIconAtScanTargetHostButton.Visible = false;

                Failure failure = action.Exception as Failure;
                if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                {
                    errorIconAtHostOrIP.Visible = true;
                    errorLabelAtHostname.Text = Messages.INVALID_HOST;
                    errorLabelAtHostname.Visible = true;
                    textBoxIscsiHost.Focus();
                }
                else if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_141")
                {
                    errorIconAtHostOrIP.Visible = true;
                    errorLabelAtHostname.Text = Messages.SR_UNABLE_TO_CONNECT_TO_SCSI_TARGET;
                    errorLabelAtHostname.Visible = true;
                    textBoxIscsiHost.Focus();                    
                }
                else if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_68")
                {
                    errorIconAtHostOrIP.Visible = true;
                    errorLabelAtHostname.Text = Messages.LOGGING_IN_TO_THE_ISCSI_TARGET_FAILED;
                    errorLabelAtHostname.Visible = true;
                    textBoxIscsiHost.Focus();
                }
                else
                {
                    errorIconAtHostOrIP.Visible = true;
                    errorLabelAtHostname.Text = failure.ErrorDescription.Count > 2 ? failure.ErrorDescription[2] : failure.ErrorDescription[0];
                    errorLabelAtHostname.Visible = true;
                    textBoxIscsiHost.Focus();                    
                    
                }
            }
            scanTargetHostButton.Enabled = true;
        }

        private void IscsiPopulateLUNs()
        {
            spinnerIconAtTargetIqn.StartSpinning();

            comboBoxIscsiLuns.Items.Clear();
            LunMap.Clear();

            var chapUser = IscsiUseChapCheckBox.Checked ? IScsiChapUserTextBox.Text : null;
            var chapPwd = IscsiUseChapCheckBox.Checked ? IScsiChapSecretTextBox.Text : null;

            IscsiPopulateLunsAction = SrType == SR.SRTypes.gfs2
                ? new Gfs2PopulateLunsAction(Connection, getIscsiHost(), getIscsiPort(), getIscsiIQN(), chapUser, chapPwd)
                : new ISCSIPopulateLunsAction(Connection, getIscsiHost(), getIscsiPort(), getIscsiIQN(), chapUser, chapPwd);
            
            IscsiPopulateLunsAction.Completed += IscsiPopulateLunsAction_Completed;

            controlDisabler.Reset();
            controlDisabler.SaveOrUpdateEnabledStates(UserInputControls);
            controlDisabler.DisableAllControls();

            IscsiPopulateLunsAction.RunAsync();
        }

        private void IscsiPopulateLunsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, (WaitCallback)IscsiPopulateLunsAction_Completed_, sender);
        }

        private void IscsiPopulateLunsAction_Completed_(object o)
        {
            Program.AssertOnEventThread();

            controlDisabler.RestoreEnabledOnAllControls();

            ISCSIPopulateLunsAction action = (ISCSIPopulateLunsAction)o;

            if (!action.Succeeded)
            {
                spinnerIconAtTargetIqn.Visible = false;

                Failure failure = action.Exception as Failure;

                if (failure != null && failure.ErrorDescription != null && failure.ErrorDescription.Count > 0)
                {
                    if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                    {
                        errorIconAtHostOrIP.Visible = true;
                        errorLabelAtHostname.Text = Messages.INVALID_HOST;
                        errorLabelAtHostname.Visible = true;
                        textBoxIscsiHost.Focus();
                    }
                    else if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_68")
                    {
                        errorIconAtCHAPPassword.Visible = true;
                        errorLabelAtCHAPPassword.Text = Messages.LOGGING_IN_TO_THE_ISCSI_TARGET_FAILED;
                        errorLabelAtCHAPPassword.Visible = true;
                        IScsiChapUserTextBox.Focus();
                    }
                    else
                    {
                        errorIconAtTargetLUN.Visible = true;
                        errorIconAtTargetLUN.Text = failure.ErrorDescription.Count > 2 ? failure.ErrorDescription[2] : failure.ErrorDescription[0];
                        errorIconAtTargetLUN.Visible = true;
                        textBoxIscsiHost.Focus();
                    }
                }
                return;
            }

            if (action.LUNs.Length == 0)
            {
                // Do nothing: ActionProgressDialog will show Messages.NEWSR_NO_LUNS_FOUND
            }
            else
            {
                comboBoxIscsiLuns.Items.Add(Messages.SELECT_TARGET_LUN);

                foreach (Actions.ISCSIInfo i in action.LUNs)
                {
                    String label = "LUN";
                    if (i.LunID != -1)
                        label += String.Format(" {0}", i.LunID);
                    if (i.Serial != "")
                        label += String.Format(": {0}", i.Serial);
                    if (i.Size >= 0)
                        label += String.Format(": {0}", Util.DiskSizeString(i.Size));
                    if (i.Vendor != "")
                        label += String.Format(" ({0})", i.Vendor);
                    comboBoxIscsiLuns.Items.Add(label);
                    LunMap.Add(label, i);
                }
                comboBoxIscsiLuns.SelectedItem = comboBoxIscsiLuns.Items.Count == 2 ? comboBoxIscsiLuns.Items[1] : Messages.SELECT_TARGET_LUN; //if there is only one choice, select that one by default
                comboBoxIscsiLuns.Enabled = true;
                targetLunLabel.Enabled = true;
                comboBoxIscsiLuns.Focus();

                spinnerIconAtTargetIqn.DisplaySucceededImage();
            }

            comboBoxIscsiLuns.Enabled = true;
            comboBoxIscsiIqns.Enabled = true;

            UpdateButtons();
        }

        /// <summary>
        /// Called with the results of an iSCSI SR.probe(), either immediately after the scan, or after the
        /// user has performed a scan, clicked 'cancel' on a dialog, and then clicked 'next' again (this
        /// avoids duplicate probing if none of the settings have changed).
        /// </summary>
        /// <returns>
        /// Whether to continue or not - wheter to format or not is stored in 
        /// iScsiFormatLUN.
        /// </returns>
        private bool ExamineIscsiProbeResults(SR.SRTypes currentSrType, List<SR.SRInfo> srs)
        {
            _srToIntroduce = null;

            if (srs == null)
                return false;

            try
            {
                if (!String.IsNullOrEmpty(SrWizardType.UUID))
                {
                    // Check LUN contains correct SR
                    if (srs.Count == 1 && srs[0].UUID == SrWizardType.UUID)
                    {
                        _srToIntroduce = srs[0];
                        SrType = currentSrType; // the type of the existing SR
                        return true;
                    }

                    errorIconAtTargetLUN.Visible = true;
                    errorLabelAtTargetLUN.Visible = true;
                    errorLabelAtTargetLUN.Text = String.Format(Messages.INCORRECT_LUN_FOR_SR, SrWizardType.SrName); 

                    return false;
                }
                else if (srs.Count == 0)
                {
                    // No existing SRs were found on this LUN. If allowed to create new SR, ask the user if they want to proceed and format.
                    if (!SrWizardType.AllowToCreateNewSr)
                    {
                        using (var dlg = new ThreeButtonDialog(
                           new ThreeButtonDialog.Details(SystemIcons.Error, Messages.NEWSR_LUN_HAS_NO_SRS, Messages.XENCENTER)))
                        {
                            dlg.ShowDialog(this);
                        }

                        return false;
                    }
                    DialogResult result = DialogResult.Yes;
                    if (!Program.RunInAutomatedTestMode)
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.NEWSR_ISCSI_FORMAT_WARNING, this.Text),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                        {
                            result = dlg.ShowDialog(this);
                        }
                    }

                    return result == DialogResult.Yes;
                }
                else
                {
                    // There should be 0 or 1 SRs on the LUN
                    System.Diagnostics.Trace.Assert(srs.Count == 1);

                    // CA-17230
                    // Check this isn't a detached SR
                    SR.SRInfo info = srs[0];
                    SR sr = SrWizardHelpers.SrInUse(info.UUID);
                    if (sr != null)
                    {
                        DialogResult res;
                        using (var d = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(null, string.Format(Messages.DETACHED_ISCI_DETECTED, Helpers.GetName(sr.Connection))),
                            new ThreeButtonDialog.TBDButton(Messages.ATTACH_SR, DialogResult.OK),
                            ThreeButtonDialog.ButtonCancel))
                        {
                            res = d.ShowDialog(Program.MainWindow);
                        }

                        if (res == DialogResult.Cancel)
                            return false;

                        _srToIntroduce = info;
                        SrType = currentSrType; // the type of the existing SR
                        return true;
                    }

                    // An SR exists on this LUN. Ask the user if they want to attach it, format it and
                    // create a new SR, or cancel.
                    DialogResult result = Program.RunInAutomatedTestMode ? DialogResult.Yes :
                        new IscsiChoicesDialog(Connection, info, currentSrType, SrType).ShowDialog(this);
                    
                    switch (result)
                    {
                        case DialogResult.Yes:
                            // Reattach
                            _srToIntroduce = srs[0];
                            SrType = currentSrType; // the type of the existing SR
                            return true;

                        case DialogResult.No:
                            // Format - SrToIntroduce is already null
                            return true;

                        default:
                            return false;
                    }
                }
            }
            catch
            {
                // We really want to prevent the user getting to the next step if there is any kind of
                // exception here, since clicking 'finish' might destroy data: require another probe.
                return false;
            }
        }

        private void HideAllErrorIconsAndLabels()
        {
            foreach (var c in ErrorIcons.Concat(ErrorLabels))
                c.Visible = false;
        }

        private void HideAllSpinnerIcons()
        {
            
            foreach (var c in SpinnerControls)
                c.Visible = false;
        }

        private Dictionary<String, String> GetDeviceConfig(SR.SRTypes srType)
        {
            Dictionary<String, String> dconf = new Dictionary<String, String>();
            ToStringWrapper<IScsiIqnInfo> iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (iqn == null || !LunMap.ContainsKey(getIscsiLUN()))
                return null;

            if (srType == SR.SRTypes.gfs2)
            {
                dconf["provider"] = "iscsi";
                dconf["target"] = iqn.item.IpAddress;
                dconf["port"] = iqn.item.Port.ToString();
                dconf["targetIQN"] = getIscsiIQN();
                dconf["SCSIid"] = LunMap[getIscsiLUN()].ScsiID;
                dconf["chapuser"] = IScsiChapUserTextBox.Text;
                dconf["chappassword"] = IScsiChapSecretTextBox.Text;

                return dconf;
            }

            // Reset target IP address to home address specified in IQN scan.
            // Allows multi-homing - see CA-11607
            dconf[TARGET] = iqn.item.IpAddress;
            dconf[PORT] = iqn.item.Port.ToString();
            dconf[TARGETIQN] = getIscsiIQN();

            ISCSIInfo info = LunMap[getIscsiLUN()];
            if (info.LunID == -1)
            {
                dconf[LUNSERIAL] = info.Serial;
            }
            else
            {
                dconf[SCSIID] = info.ScsiID;
            }

            if (IscsiUseChapCheckBox.Checked)
            {
                dconf[CHAPUSER] = IScsiChapUserTextBox.Text;
                dconf[CHAPPASSWORD] = IScsiChapSecretTextBox.Text;
            }

            return dconf;
        }

        #region Accessors

        public SrWizardType SrWizardType { private get; set; }

        public string UUID { get { return _srToIntroduce == null ? null : _srToIntroduce.UUID; } }

        public long SRSize
        {
            get
            {
                ISCSIInfo info = LunMap[getIscsiLUN()];
                return info.Size;
            }
        }
        
        public Dictionary<String, String> DeviceConfig
        {
            get
            {
                return GetDeviceConfig(SrType);
            }
        }

        public string SrDescription
        {
            get
            {
                ToStringWrapper<IScsiIqnInfo> iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
                return iqn == null ? null : string.Format(Messages.NEWSR_ISCSI_DESCRIPTION, iqn.item.IpAddress, getIscsiIQN(), getIscsiLUN());
            }
        }

        public SR.SRTypes SrType { get; set; }
        #endregion
    }
}
