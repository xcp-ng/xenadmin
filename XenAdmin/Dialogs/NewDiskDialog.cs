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
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions;
using System.Drawing;


namespace XenAdmin.Dialogs
{
    public partial class NewDiskDialog : XenDialogBase
    {
        private readonly VM TheVM;
        private readonly SR TheSR;

        private VDI DiskTemplate;
        private bool CanResize;
        private long MinSize;
        private decimal min;
        private decimal max;

        private bool SelectionNull = true;
        private readonly IEnumerable<VDI> _VDINamesInUse = new List<VDI>();

        private NewDiskDialog(IXenConnection connection, IEnumerable<VDI> vdiNamesInUse)
            : base(connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            _VDINamesInUse = vdiNamesInUse;
            InitializeComponent();
            InitDialog(connection);
        }

        private void InitDialog(IXenConnection connection)
        {
            this.Owner = Program.MainWindow;
            SrListBox.Connection = connection;
            SrListBox.SrHint.Visible = false;

            // Add events
            NameTextBox.Text = GetDefaultVDIName();
            SrListBox.srListBox.SelectedIndexChanged += new EventHandler(srListBox_SelectedIndexChanged);
            SrListBox.ItemSelectionNotNull += SrListBox_ItemSelectionNotNull;
            SrListBox.ItemSelectionNull += SrListBox_ItemSelectionNull;
            srListBox_SelectedIndexChanged(null, null);

            DiskSizeNumericUpDown.TextChanged += new EventHandler(DiskSizeNumericUpDown_TextChanged);

            max = (decimal)Math.Pow(1024, 4);//1 Petabit
            min = 0;
            comboBoxUnits.SelectedItem = comboBoxUnits.Items[0];
            comboBoxUnits.SelectedIndexChanged += new EventHandler(comboBoxUnits_SelectedIndexChanged);

            SetNumUpDownIncrementAndDecimals(DiskSizeNumericUpDown, comboBoxUnits.SelectedItem.ToString());
        }

        public NewDiskDialog(IXenConnection connection, SR sr)
            : this(connection, new List<VDI>())
        {
            TheSR = sr;
            PickerUsage = SrPicker.SRPickerType.InstallFromTemplate;
            SrListBox.SetAffinity(null);
            SrListBox.selectSRorNone(TheSR);
        }

        private SrPicker.SRPickerType PickerUsage
        {
            set { SrListBox.Usage = value; }
        }

        public NewDiskDialog(IXenConnection connection, VM vm, VDI diskTemplate, Host affinity, bool canResize, long minSize, IEnumerable<VDI> vdiNamesInUse)
            : this(connection, vm, SrPicker.SRPickerType.VM, diskTemplate, affinity, canResize, minSize, vdiNamesInUse)
        {
        }

        public NewDiskDialog(IXenConnection connection, VM vm)
            : this(connection, vm, null, vm.Home(), true, 0, new List<VDI>()) { }

        public NewDiskDialog(IXenConnection connection, VM vm, SrPicker.SRPickerType PickerUsage, VDI diskTemplate, Host affinity, bool canResize, long minSize, IEnumerable<VDI> vdiNamesInUse)
            : this(connection, vdiNamesInUse)
        {
            TheVM = vm;
            DiskTemplate = diskTemplate;
            CanResize = canResize;
            MinSize = minSize;
            this.PickerUsage = PickerUsage;
            SrListBox.SetAffinity(affinity);

            Pool pool_sr = Helpers.GetPoolOfOne(connection);
            if (pool_sr != null)
            {
                SrListBox.DefaultSR = connection.Resolve(pool_sr.default_SR); //if default sr resolves to null the first sr in the list will be selected
            }
            SrListBox.selectDefaultSROrAny();

            LoadValues();
        }

        private void LoadValues()
        {
            if (DiskTemplate == null)
                return;

            NameTextBox.Text = DiskTemplate.Name();
            DescriptionTextBox.Text = DiskTemplate.Description();
            SrListBox.selectSRorDefaultorAny(connection.Resolve(DiskTemplate.SR));

            // select the appropriate unit, based on size (CA-45905)
            currentSelectedUnits = DiskTemplate.virtual_size >= Util.BINARY_GIGA ? DiskSizeUnits.GB : DiskSizeUnits.MB;
            SelectedUnits = currentSelectedUnits;
            SetNumUpDownIncrementAndDecimals(DiskSizeNumericUpDown, SelectedUnits.ToString());
            decimal newValue = (decimal)Math.Round((double)DiskTemplate.virtual_size / GetUnits(), DiskSizeNumericUpDown.DecimalPlaces);
            DiskSizeNumericUpDown.Value = newValue >= DiskSizeNumericUpDown.Minimum && newValue <= DiskSizeNumericUpDown.Maximum ?
                newValue : DiskSizeNumericUpDown.Maximum;

            if (MinSize > 0)
                min = (decimal)((double)MinSize / GetUnits());
            DiskSizeNumericUpDown.Enabled = CanResize;

            Text = Messages.EDIT_DISK;
            OkButton.Text = Messages.OK;
        }

        private const int DecimalPlacesGB = 3; // show 3 decimal places for GB (CA-91322)
        private const int DecimalPlacesMB = 0;
        private const int IncrementGB = 1;
        private const int IncrementMB = 256;
        
        private string GetDefaultVDIName()
        {
            List<string> usedNames = new List<string>();
            foreach (VDI v in connection.Cache.VDIs.Concat(_VDINamesInUse))
            {
                usedNames.Add(v.Name());
            }
            return Helpers.MakeUniqueName(Messages.DEFAULT_VDI_NAME, usedNames);
        }

        void SrListBox_ItemSelectionNull()
        {
            SelectionNull = true;
        }

        void SrListBox_ItemSelectionNotNull()
        {
            SelectionNull = false;
        }

        void srListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateErrorsAndButtons();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {


            if (SrListBox.SR == null || SelectionNull || NameTextBox.Text == "" || !connection.IsConnected)
                return;

            if (DontCreateVDI)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            XenAPI.SR sr = SrListBox.SR;
            if (!sr.shared && TheVM != null && TheVM.HaPriorityIsRestart())
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.NEW_SR_DIALOG_ATTACH_NON_SHARED_DISK_HA, Messages.XENCENTER),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Program.MainWindow);
                }
                if (dialogResult != DialogResult.Yes)
                    return;
                new HAUnprotectVMAction(TheVM).RunExternal(TheVM.Connection.Session);
            }

            VDI vdi = NewDisk();


            if (TheVM != null)
            {
                var alreadyHasBootableDisk = HasBootableDisk(TheVM);

                Actions.DelegatedAsyncAction action = new Actions.DelegatedAsyncAction(connection,
                    string.Format(Messages.ACTION_DISK_ADDING_TITLE, NameTextBox.Text, sr.NameWithoutHost()),
                    Messages.ACTION_DISK_ADDING, Messages.ACTION_DISK_ADDED,
                    delegate(XenAPI.Session session)
                    {
                        // Get legitimate unused userdevice numbers
                        string[] uds = XenAPI.VM.get_allowed_VBD_devices(session, TheVM.opaque_ref);
                        if (uds.Length == 0)
                        {
                            throw new Exception(FriendlyErrorNames.VBDS_MAX_ALLOWED);
                        }
                        string ud = uds[0];
                        string vdiref = VDI.create(session, vdi);
                        XenAPI.VBD vbd = NewDevice();
                        vbd.VDI = new XenAPI.XenRef<XenAPI.VDI>(vdiref);
                        vbd.VM = new XenAPI.XenRef<XenAPI.VM>(TheVM);

                        // CA-44959: only make bootable if there aren't other bootable VBDs.
                        vbd.bootable = ud == "0" && !alreadyHasBootableDisk;
                        vbd.userdevice = ud;

                        // Now try to plug the VBD.
                        new XenAdmin.Actions.VbdSaveAndPlugAction(TheVM, vbd, vdi.Name(), session, false, ShowMustRebootBoxCD, ShowVBDWarningBox).RunAsync();
                    });
                action.VM = TheVM;
                new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
                if (!action.Succeeded)
                    return;
            }
            else
            {
                CreateDiskAction action = new CreateDiskAction(vdi);
                new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Marquee).ShowDialog();
                if (!action.Succeeded)
                    return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool HasBootableDisk(VM vm)
        {
            var c = vm.Connection;
            foreach (XenRef<VBD> vbdRef in vm.VBDs)
            {
                var vbd = c.Resolve(vbdRef);

                if (vbd != null && !vbd.IsCDROM() && !vbd.IsFloppyDrive() && vbd.bootable)
                {
                    VDI vdi = c.Resolve(vbd.VDI);

                    if (vdi != null)
                    {
                        SR sr = c.Resolve(vdi.SR);
                        if (sr != null && sr.IsToolsSR())
                        {
                            continue;
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        private void SetNumUpDownIncrementAndDecimals(NumericUpDown upDown, string units)
        {
            if (units == Messages.VAL_GIGB)
            {
                upDown.DecimalPlaces = DecimalPlacesGB;
                upDown.Increment = IncrementGB;
            }
            else
            {
                upDown.DecimalPlaces = DecimalPlacesMB;
                upDown.Increment = IncrementMB;
            }
        }

        public VDI NewDisk()
        {
            VDI vdi = new VDI();
            vdi.Connection = connection;
            vdi.read_only = DiskTemplate != null ? DiskTemplate.read_only : false;
            vdi.SR = new XenAPI.XenRef<XenAPI.SR>(SrListBox.SR);

            vdi.virtual_size = Convert.ToInt64(DiskSizeNumericUpDown.Value * GetUnits());
            vdi.name_label = NameTextBox.Text;
            vdi.name_description = DescriptionTextBox.Text;
            vdi.sharable = DiskTemplate != null ? DiskTemplate.sharable : false;
            vdi.type = DiskTemplate != null ? DiskTemplate.type : vdi_type.user;
            vdi.SetVmHint(TheVM != null ? TheVM.uuid : "");
            return vdi;
        }

        private long GetUnits()
        {
            return (SelectedUnits == DiskSizeUnits.GB ? Util.BINARY_GIGA : Util.BINARY_MEGA);
        }

        public VBD NewDevice()
        {

            VBD vbd = new VBD();
            vbd.Connection = connection;
            vbd.device = "";
            vbd.empty = false;
            vbd.type = XenAPI.vbd_type.Disk;
            vbd.mode = XenAPI.vbd_mode.RW;
            vbd.SetIsOwner(true);
            vbd.unpluggable = true;
            return vbd;

        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            updateErrorsAndButtons();
        }

        private enum DiskSizeUnits { MB, GB }

        private DiskSizeUnits SelectedUnits
        {
            get { return comboBoxUnits.SelectedIndex == 0 ? DiskSizeUnits.GB : DiskSizeUnits.MB; }
            set { comboBoxUnits.SelectedIndex = value == DiskSizeUnits.GB ? 0 : 1; }
        }

        private const decimal MinimumDiskSizeGB = 0.001m;
        private const int MinimumDiskSizeMB = 1;

        private decimal GetDiskTooSmallMessageMinSize()
        {
            return min == 0 ? SelectedUnits == DiskSizeUnits.GB ? MinimumDiskSizeGB : MinimumDiskSizeMB : min;
        }

        private void updateErrorsAndButtons()
        {
            // Ordering is important here, we want to show the most relevant message

            if (comboBoxUnits.SelectedItem == null)
                return;

            if (!SrListBox.ValidSelectionExists)
            {
                OkButton.Enabled = false;
                setError(Messages.NO_VALID_DISK_LOCATION);
                return;
            }
            if (SelectionNull)
            {
                OkButton.Enabled = false;
                // shouldn't happen I think, previous if block should catch this, just to be safe
                setError(null);
                return;
            }
            if (DiskSizeNumericUpDown.Text.Trim() == string.Empty)
            {
                OkButton.Enabled = false;
                // too minor for scary error to be displayed, plus it's obvious as they have to 
                // delete the default entry and can see the OK button disable as they do so
                setError(null);
                return;
            }
            if (!DiskSizeValidNumber())
            {
                OkButton.Enabled = false;
                setError(Messages.INVALID_NUMBER);
                return;
            }
            if (!DiskSizeValid())
            {
                OkButton.Enabled = false;
                setError(string.Format(Messages.DISK_TOO_SMALL, GetDiskTooSmallMessageMinSize(),
                                       comboBoxUnits.SelectedItem.ToString()));
                return;
            }
            if (string.IsNullOrEmpty(NameTextBox.Text.Trim()))
            {
                OkButton.Enabled = false;
                // too minor for scary error to be displayed, plus it's obvious as they have to 
                // delete the default entry and can see the OK button disable as they do so
                setError(null);
                return;
            }

            OkButton.Enabled = true;
            setError(null);
        }

        private void setError(string error)
        {
            if (string.IsNullOrEmpty(error))
                labelError.Visible = pictureBoxError.Visible = false;
            else
            {
                labelError.Visible = pictureBoxError.Visible = true;
                labelError.Text = error;
            }
        }

        private bool DiskSizeValidNumber()
        {
            decimal val;
            return decimal.TryParse(DiskSizeNumericUpDown.Text.Trim(), out val);
        }

        private bool DiskSizeValid()
        {
            decimal val;
            if (decimal.TryParse(DiskSizeNumericUpDown.Text.Trim(), out val))
            {
                if (min == 0)
                    return val > min && val <= max;
                return val >= min && val <= max;
            }
            return false;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool dontCreateVDI;
        public bool DontCreateVDI
        {
            get
            {
                return dontCreateVDI;
            }
            set
            {
                dontCreateVDI = value;
            }
        }


        private void DiskSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateDiskSize();
        }

        private void DiskSizeNumericUpDown_TextChanged(object sender, EventArgs e)
        {
            UpdateDiskSize();
        }

        private void DiskSizeNumericUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateDiskSize();
        }
       
        private void UpdateDiskSize()
        {
            // Don't use DiskSizeNumericUpDown.Value here, as it will fire the NumericUpDown built-in validation. Use Text property instead. (CA-46028)
            decimal newValue;
            if (decimal.TryParse(DiskSizeNumericUpDown.Text.Trim(), out newValue))
            {
                try
                {
                    SrListBox.DiskSize = (long)(Math.Round(newValue * GetUnits()));
                }
                catch (OverflowException)
                {
                    //CA-71312
                   SrListBox.DiskSize = newValue < 0 ? long.MinValue : long.MaxValue;
                }
                SrListBox.UpdateDiskSize();
            }
            RefreshMinSize();
            updateErrorsAndButtons();
        }

        private void RefreshMinSize()
        {
            if (DiskTemplate == null)
                return;
            if (MinSize > 0)
                min = (decimal)((double)MinSize / GetUnits());
        }

        private DiskSizeUnits currentSelectedUnits = DiskSizeUnits.GB;
        private void comboBoxUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Check if the new unit is different than the previous one otherwise discard the change
            if (currentSelectedUnits != SelectedUnits)
            {
                currentSelectedUnits = SelectedUnits;
                //Convert the current value to the new units
                decimal newValue = (decimal)Math.Round(SelectedUnits == DiskSizeUnits.GB ? ((double)DiskSizeNumericUpDown.Value / 1024) : ((double)DiskSizeNumericUpDown.Value * 1024), DiskSizeNumericUpDown.DecimalPlaces);
                DiskSizeNumericUpDown.Value = newValue >= DiskSizeNumericUpDown.Minimum && newValue <= DiskSizeNumericUpDown.Maximum ?
                    newValue : DiskSizeNumericUpDown.Maximum;
                SetNumUpDownIncrementAndDecimals(DiskSizeNumericUpDown, comboBoxUnits.SelectedItem.ToString());
                UpdateDiskSize();
            }
        }

        internal override string HelpName
        {
            get
            {
                if (DiskTemplate != null)
                    return "EditNewDiskDialog";
                else
                    return "NewDiskDialog";
            }
        } 

        public static void ShowVBDWarningBox()
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information,
                                                      Messages.NEWDISKWIZARD_MESSAGE,
                                                      Messages.NEWDISKWIZARD_MESSAGE_TITLE)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
            });
        }

        public static void ShowMustRebootBoxCD()
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information,
                                                      Messages.
                                                          NEW_DVD_DRIVE_REBOOT,
                                                      Messages.
                                                          NEW_DVD_DRIVE_CREATED))
                        .ShowDialog(Program.MainWindow);
                }
            });
        }
    }
}
