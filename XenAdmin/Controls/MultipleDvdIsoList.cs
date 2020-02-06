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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Network;


namespace XenAdmin.Controls
{
    public partial class MultipleDvdIsoList : UserControl
    {
        bool inRefresh = false;

        public MultipleDvdIsoList()
        {
            InitializeComponent();
        }

        private VM vm = null;
        public VM VM
        {
            set
            {
                DeregisterEvents();
                cdChanger1.vm = value;
                vm = value;
                if (vm != null)
                {
                    vm.PropertyChanged += new PropertyChangedEventHandler(vm_PropertyChanged);
                }
                refreshDrives();
            }
            get 
            {
                return cdChanger1.vm;
            }
        }

        #region Designer browsable properties

        [Browsable(true)]
        [Category("Appearance")]
        public Color LabelSingleDvdForeColor
        {
            get { return labelSingleDvd.ForeColor; }
            set { labelSingleDvd.ForeColor = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color LabelNewCdForeColor
        {
            get { return newCDLabel.ForeColor; }
            set { newCDLabel.ForeColor = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color LinkLabelLinkColor
        {
            get { return linkLabel1.LinkColor; }
            set { linkLabel1.LinkColor = value; }
        }

        #endregion

        internal virtual void DeregisterEvents()
        {
            if (vm == null)
                return;

            // remove VM listeners
            vm.PropertyChanged -= vm_PropertyChanged;

            // remove cache listener
            vm.Connection.CachePopulated -= CachePopulatedMethod;

            // remove VBD listeners
            var vbds = vm.Connection.ResolveAll(VM.VBDs);
                
            foreach (var vbd in vbds.Where(vbd => vbd.IsCDROM() || vbd.IsFloppyDrive()))
            {
                vbd.PropertyChanged -= vbd_PropertyChanged;
            }
            cdChanger1.DeregisterEvents();
        }

        void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VBDs")
                refreshDrives();
        }

        private void refreshDrives()
        {
            VbdCombiItem prevSelection = comboBoxDrive.SelectedItem as VbdCombiItem;
            inRefresh = true;

            foreach (object o in comboBoxDrive.Items)
            {
                VbdCombiItem v = o as VbdCombiItem;
                v.vbd.PropertyChanged -= new PropertyChangedEventHandler(vbd_PropertyChanged);
            }

            comboBoxDrive.Items.Clear();
            if (VM != null && !VM.is_control_domain)
            {
                List<VBD> vbds = VM.Connection.ResolveAll(VM.VBDs);
                if (vbds == null)
                {
                    // let's come back when the cache is populated
                    VM.Connection.CachePopulated += CachePopulatedMethod;
                    return;
                }
                vbds.RemoveAll(vbd => !vbd.IsCDROM() && !vbd.IsFloppyDrive());
                vbds.Sort();
                int dvdCount = 0;
                int floppyCount = 0;
                foreach (VBD vbd in vbds)
                {
                    vbd.PropertyChanged +=new PropertyChangedEventHandler(vbd_PropertyChanged);
                    if (vbd.IsCDROM())
                    {
                        dvdCount++;
                        VbdCombiItem i = new VbdCombiItem();
                        i.name = string.Format(Messages.DVD_DRIVE_LABEL_NUMBERED, dvdCount);
                        i.vbd = vbd;
                        comboBoxDrive.Items.Add(i);                       
                    }
                    else
                    {
                        floppyCount++;
                        VbdCombiItem i = new VbdCombiItem();
                        i.name = string.Format(Messages.FLOPPY_DRIVE_LABEL_NUMBERED, floppyCount);
                        i.vbd = vbd;
                        comboBoxDrive.Items.Add(i);
                    }               
                }
            }
            if (comboBoxDrive.Items.Count == 0)
            {
                comboBoxDrive.Visible = false;
                cdChanger1.Visible = false;
                labelSingleDvd.Visible = false;
                linkLabel1.Visible = false;
                panel1.Visible = false;
                newCDLabel.Visible = vm != null && !vm.is_control_domain;
                
            }
            else if (comboBoxDrive.Items.Count == 1)
            {
                comboBoxDrive.Visible = false;
                cdChanger1.Visible = true;
                labelSingleDvd.Text = comboBoxDrive.Items[0].ToString();
                labelSingleDvd.Visible = true;
                tableLayoutPanel1.ColumnStyles[0].Width = labelSingleDvd.Width;
                newCDLabel.Visible = false;
                panel1.Visible = true;
                linkLabel1.Visible = true;
            }
            else
            {
                comboBoxDrive.Visible = true;
                cdChanger1.Visible = true;
                labelSingleDvd.Visible = false;
                panel1.Visible = true;
                newCDLabel.Visible = false;
                linkLabel1.Visible = true;
            }
            inRefresh = false;
            // Restore prev selection or select the top item by default
            if (prevSelection != null)
            {
                foreach (object o in comboBoxDrive.Items)
                {
                    VbdCombiItem v = o as VbdCombiItem;
                    if (v.vbd.uuid == prevSelection.vbd.uuid)
                    {
                        comboBoxDrive.SelectedItem = o;
                        return;
                    }
                }
            }
            if (comboBoxDrive.Items.Count == 0)
                comboBoxDrive.SelectedItem = null;
            else
                comboBoxDrive.SelectedItem = comboBoxDrive.Items[0];
        }

        void vbd_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshDrives();
        }

        private void CachePopulatedMethod(IXenConnection conn)
        {
            VM.Connection.CachePopulated -= CachePopulatedMethod;
            refreshDrives();
        }

        internal class VbdCombiItem
        {
            public string name;
            public VBD vbd;

            public override string ToString()
            {
                return name;
            }
        }

        private void comboBoxDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inRefresh)
                return;

            cdChanger1.Drive = comboBoxDrive.SelectedItem != null ? ((VbdCombiItem)comboBoxDrive.SelectedItem).vbd : null;
        }


        private void newCDLabel_Click(object sender, EventArgs e)
        {
            if (VM != null)
            {
                var createDriveAction = new CreateCdDriveAction(VM);
                createDriveAction.ShowUserInstruction += CreateDriveAction_ShowUserInstruction;

                using (var dlg = new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee))
                    dlg.ShowDialog(this);
            }
        }

        private void CreateDriveAction_ShowUserInstruction(string message)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information, message)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cdChanger1.Drive != null)
                cdChanger1.ChangeCD(null);
        }
    }
}
