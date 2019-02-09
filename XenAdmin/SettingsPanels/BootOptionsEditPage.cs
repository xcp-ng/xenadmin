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
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAPI;
using System.Collections;
using XenAdmin.Properties;
using XenAdmin.Actions;
using XenCenterLib;
using XenAdmin.Core;
using System.Linq;

namespace XenAdmin.SettingsPanels
{
	public partial class BootOptionsEditPage : UserControl, IEditPage
	{
		#region Private fields
		private VM vm;
        private Pool pool;
        private bool clusteringEnabled;
        private bool bootFromCD;
		#endregion

		public BootOptionsEditPage()
		{
			InitializeComponent();
			Text = Messages.GENERAL_HEADING_BOOT_OPTIONS;
		}

		#region IEditPage implementation

		public bool ValidToSave { get { return true; } }

		/// <summary>
		/// Show local validation balloon tooltips
		/// </summary>
		public void ShowLocalValidationMessages()
		{ }

		/// <summary>
		/// Unregister listeners, dispose balloon tooltips, etc.
		/// </summary>
		public void Cleanup()
		{ }

		public bool HasChanged
		{
			get
			{
                bool autoBootChanged = m_checkBoxAutoBoot.Checked != vm.GetAutoPowerOn();
                return autoBootChanged || (vm.IsHVM() && GetOrder() != vm.GetBootOrder()) || (m_textBoxOsParams.Text != vm.PV_args) || (VMPVBootableDVD() != bootFromCD);
			}
		}

		public AsyncAction SaveSettings()
		{
			vm.SetBootOrder(GetOrder());
			
			vm.PV_args = m_textBoxOsParams.Text;

            vm.SetAutoPowerOn(m_checkBoxAutoBoot.Checked);

            return new DelegatedAsyncAction(vm.Connection, "Change VBDs bootable", "Change VBDs bootable", null,
			                                delegate(Session session)
			                                	{
			                                		if (bootFromCD)
			                                		{
			                                			foreach (var vbd in vm.Connection.ResolveAll(vm.VBDs))
			                                				VBD.set_bootable(session, vbd.opaque_ref, vbd.IsCDROM());
			                                		}
			                                		else
			                                		{
			                                			// The lowest numbered disk is the system disk and we should set it to bootable: see CA-47457
			                                			List<VBD> vbds = vm.Connection.ResolveAll(vm.VBDs);
                                                        vbds.Sort((vbd1, vbd2) =>
                                                                      {
                                                                          if (vbd1.userdevice == "xvda")
                                                                              return -1;
                                                                          if (vbd2.userdevice == "xvda")
                                                                              return 1;
                                                                          return StringUtility.NaturalCompare(vbd1.userdevice,
                                                                                                 vbd2.userdevice);
                                                                      });
			                                			bool foundSystemDisk = false;
			                                			foreach (var vbd in vbds)
			                                			{
			                                				bool bootable = (!foundSystemDisk && vbd.type == vbd_type.Disk);
			                                				if (bootable)
			                                					foundSystemDisk = true;
			                                				VBD.set_bootable(session, vbd.opaque_ref, bootable);
			                                			}
			                                		}
			                                	},
                                            true, // supress history
			                                "VBD.set_bootable"
				);
		}

		public void SetXenObjects(IXenObject orig, IXenObject clone)
		{
			vm = clone as VM;
			if (vm == null)
				return;

            pool = Helpers.GetPoolOfOne(clone.Connection);
            var existingCluster = pool.Connection.Cache.Clusters.FirstOrDefault();
            clusteringEnabled = existingCluster != null;

            Repopulate();
		}

		#endregion

		#region VerticalTabs.IVerticalTab implementation

		public String SubText
		{
			get
			{
				if (vm == null)
					return "";

				if (vm.IsHVM())
				{
					List<String> driveLetters = new List<String>();

					foreach (object o in m_checkedListBox.Items)
					{
						BootDevice device = o as BootDevice;

						if (device != null)
							driveLetters.Add(device.ToString());
					}

					string order = String.Join(", ", driveLetters.ToArray());

                    if (m_checkBoxAutoBoot.Checked)
                        return String.Format(Messages.BOOTORDER_AUTOSTART, order);
                    else
                        return String.Format(Messages.BOOTORDER, order);
				}

				return Messages.NONE_DEFINED;
			}
		}

		public Image Image
		{
			get
			{
				return Resources._001_PowerOn_h32bit_16;
			}
		}

		#endregion

        private void BootDeviceAndOrderEnabled(bool enabledState)
        {
            m_checkedListBox.Enabled = enabledState;
            m_comboBoxBootDevice.Enabled = enabledState;
        }

		private void Repopulate()
		{
            m_picInfoAutoBoot.Visible = false;
            m_autoHeightLabelAutoBoot.Visible = false;
            m_autoHeightLabelAutoBootHAWarning.Visible = false;
            m_checkBoxAutoBoot.Visible = false;

            if (clusteringEnabled)
            {
                m_picInfoAutoBoot.Visible = true;

                if (vm.GetAutoPowerOn())
                {
                    m_checkBoxAutoBoot.Visible = true;
                    m_checkBoxAutoBoot.Checked = true;
                    m_autoHeightLabelAutoBootHAWarning.Visible = true;
                }
                else
                {
                    m_autoHeightLabelAutoBoot.Visible = true;
                }
            } 
            else
            {
                m_checkBoxAutoBoot.Visible = true;
                m_checkBoxAutoBoot.Checked = vm.GetAutoPowerOn();
            }

            BootDeviceAndOrderEnabled(vm.IsHVM());

			if (vm.IsHVM())
			{
				m_tlpHvm.Visible = true;
				m_autoHeightLabelHvm.Visible = true;
				m_tlpNonHvm.Visible = false;
				m_autoHeightLabelNonHvm.Visible = false;
				
				m_checkedListBox.Items.Clear();
				string order = vm.GetBootOrder().ToUpper();

				foreach (char c in order)
					m_checkedListBox.Items.Add(new BootDevice(c),true);

				// then add any 'missing' entries
				foreach (char c in BootDevice.BootOptions)
				{
					if (!order.Contains(c.ToString()))
						m_checkedListBox.Items.Add(new BootDevice(c), false);
				}

				ToggleUpDownButtonsEnabledState();
			    
			}
			else
			{
				m_tlpHvm.Visible = false;
				m_autoHeightLabelHvm.Visible = false;
				m_tlpNonHvm.Visible = true;
				m_autoHeightLabelNonHvm.Visible = true;

				m_comboBoxBootDevice.Items.Clear();
				m_comboBoxBootDevice.Items.Add(Messages.BOOT_HARD_DISK);

				if (vm.HasCD())
				{
					m_comboBoxBootDevice.Items.Add(Messages.DVD_DRIVE);
					m_comboBoxBootDevice.SelectedItem = VMPVBootableDVD() ? Messages.DVD_DRIVE : Messages.BOOT_HARD_DISK;
				}
				else
					m_comboBoxBootDevice.SelectedItem = Messages.BOOT_HARD_DISK;

				m_textBoxOsParams.Text = vm.PV_args;
			}
		}

		private bool VMPVBootableDVD()
		{
			foreach (var vbd in vm.Connection.ResolveAll(vm.VBDs))
			{
				if (vbd.IsCDROM() && vbd.bootable)
					return true;
			}
			return false;
		}

		private string GetOrder()
		{
			string bootOrder = "";

			foreach (object o in m_checkedListBox.CheckedItems)
			{
				BootDevice device = o as BootDevice;
				
				if (device != null)
					bootOrder += device.GetChar().ToString();
			}

			return bootOrder;
		}
		
		private void ToggleUpDownButtonsEnabledState()
		{
			m_buttonUp.Enabled = 0 < m_checkedListBox.SelectedIndex && m_checkedListBox.SelectedIndex <= m_checkedListBox.Items.Count - 1;
			m_buttonDown.Enabled = 0 <= m_checkedListBox.SelectedIndex && m_checkedListBox.SelectedIndex < m_checkedListBox.Items.Count - 1;
            m_buttonConvertToHVM.Enabled = vm.power_state == vm_power_state.Halted;
            m_buttonConvertToPV.Enabled = vm.power_state == vm_power_state.Halted;
		}

		/// <param name="up">
		/// True moves the item up, false moves it down
		/// </param>
		private void MoveItem(bool up)
		{
			int oldIndex = m_checkedListBox.SelectedIndex;
			
			//check selection valid
			if (oldIndex < 0 || oldIndex > m_checkedListBox.Items.Count - 1)
				return;

			//check operation valid
			if (up && oldIndex == 0)
				return;
			if (!up && oldIndex == m_checkedListBox.Items.Count - 1)
				return;

			int newIndex = up ? oldIndex - 1 : oldIndex + 1;

			object item = m_checkedListBox.SelectedItem;
			bool isChecked = m_checkedListBox.GetItemChecked(oldIndex);
			m_checkedListBox.Items.Remove(item);
			m_checkedListBox.Items.Insert(newIndex, item);
			m_checkedListBox.SetItemChecked(newIndex, isChecked);
			m_checkedListBox.SelectedIndex = newIndex;

			ToggleUpDownButtonsEnabledState();
		}

        /// <param name="toPV">
        /// True converts to PV, false converts to HVM
        /// </param>
        private void Convert(bool toPV)
        {
            if (toPV)
            {
                vm.HVM_boot_policy = "";

                vm.PV_args = "console=tty0 xencons=tty";
                vm.PV_bootloader = "pygrub";
            }
            else
            {
                vm.HVM_boot_policy = "BIOS Order";
            }

            Repopulate();
        }


		#region Control Event Handlers

		private void m_checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ToggleUpDownButtonsEnabledState();
		}

		private void m_buttonUp_Click(object sender, EventArgs e)
		{
			MoveItem(true);
		}

		private void m_buttonDown_Click(object sender, EventArgs e)
		{
			MoveItem(false);
		}

		private void m_comboBoxBootDevice_SelectedIndexChanged(object sender, EventArgs e)
		{
			bootFromCD = (string)m_comboBoxBootDevice.SelectedItem == Messages.DVD_DRIVE;
		}

        private void m_buttonConvertToPV_Click(object sender, EventArgs e)
        {
            Convert(true);
        }

        private void m_buttonConvertToHVM_Click(object sender, EventArgs e)
        {
            Convert(false);
        }

        #endregion


    }
}
