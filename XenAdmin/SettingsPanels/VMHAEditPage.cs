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
using System.Threading;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.TabPages;
using XenAPI;
using XenAdmin.Controls;


namespace XenAdmin.SettingsPanels
{
    public partial class VMHAEditPage : UserControl, IEditPage
	{
		#region Private fields
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VM vm;
        private bool vmIsAgile;
        private VM.HA_Restart_Priority origRestartPriority;
        private long origNtol;
        private long origOrder;
        private long origStartDelay;
        private VerticalTabs verticalTabs;

        /// <summary>
        /// The vm's pool. May be null.
        /// </summary>
        private Pool pool;

        /// <summary>
        /// Any Hosts we have property changed listeners registered on.
        /// </summary>
        private readonly List<Host> hosts = new List<Host>();

        /// <summary>
        /// Any Host_metrics objects we have property changed listeners registered on.
        /// </summary>
        private readonly List<Host_metrics> hostMetrics = new List<Host_metrics>();

		private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
		#endregion

		public VMHAEditPage()
        {
            InitializeComponent();
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
			Text = Messages.START_UP_OPTIONS;
            nudStartDelay.Maximum = long.MaxValue;
            nudOrder.Maximum = long.MaxValue;
        }

        public VerticalTabs VerticalTabs
        {
            set
            {
                this.verticalTabs = value;
            }
		}

		#region VerticalTabs.IVerticalTab implementation

		public String SubText
        {
            get
            {
                if (vm == null)
                    return "";

                Pool pool = Helpers.GetPool(vm.Connection);
                if (pool == null)
                {
                    return Messages.HA_STANDALONE_SERVER;
                }
                
                if (!pool.ha_enabled)
                {
                	return String.Format(Messages.HA_NOT_CONFIGURED, Helpers.GetName(pool).Ellipsise(30));
                }

                return Helpers.RestartPriorityI18n(SelectedPriority);
            }
        }

        public Image Image
        {
            get
            {
				return Properties.Resources._000_RebootVM_h32bit_16;
            }
		}

		#endregion

		#region Event handlers

		/// <summary>
        /// Called when the Host collection of the current VM's Connection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            // NB this code is unused at the moment, since the HA is not yet capable of adding/removing hosts once switched on
            // This may change in the future
            // ... in which case test this code ;)
            Host host = (Host)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                // Listen for changes on the host's metrics field
                host.PropertyChanged += host_PropertyChanged;
                // Remember the host so we can deregister the listener later
                hosts.Add(host);
                // Register on the new host's metrics object - we need to know if the host goes down
                Host_metrics metrics = vm.Connection.Resolve(host.metrics);
                if (metrics != null)
                {
                    metrics.PropertyChanged += metrics_PropertyChanged;
                    // Remember the metrics object so we can deregister the listener later
                    hostMetrics.Add(metrics);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                host.PropertyChanged -= host_PropertyChanged;
                hosts.Remove(host);
                // Also try deregistering from the host's metrics. Probably not terrible if we cannot.
                Host_metrics metrics = vm.Connection.Resolve(host.metrics);
                if (metrics != null)
                {
                    metrics.PropertyChanged -= metrics_PropertyChanged;
                    hostMetrics.Remove(metrics);
                }
            }
        }

        /// <summary>
        /// Called when the pool of the current VM has a property changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ha_enabled")
            {
                Repopulate();
            }
        }

        /// <summary>
        /// Called when any Host of the current VM's connection has a property changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        	Host host = (Host)sender;
            if (e.PropertyName == "metrics")
            {
                // Just register on the new metrics object - assume the old one is deleted and will fire no more events
                Host_metrics metrics = vm.Connection.Resolve(host.metrics);
                if (metrics != null)
                {
                    metrics.PropertyChanged += metrics_PropertyChanged;
                    // Remember the metrics object so we can deregister the listener later
                    hostMetrics.Add(metrics);
                }
            }
        }

        private void metrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "live")
            {
                // Looks like a host has gone down
                Repopulate();
            }
		}

		#endregion

		private void UpdateVMAgility(object obj)
        {
            Program.AssertOffEventThread();

        	try
            {
                IXenConnection c = vm.Connection;
                Session session = c.DuplicateSession();

                try
                {
                    VM.assert_agile(session, vm.opaque_ref);
                    // The VM was agile
                    vmIsAgile = true;
                }
                catch (Failure)
                {
                    // VM wasn't agile
                    vmIsAgile = false;
                }
                Program.Invoke(Program.MainWindow, RefillPrioritiesComboBox);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                Program.Invoke(this, delegate()
                {
                    if (verticalTabs != null)
                        verticalTabs.Refresh();
                });
            }
        }

		/// <param name="showScan">
		/// True to show the scanning label and hide the HA restart priority groupbox.
		/// </param>
		private void ToggleScanningVmAgile(bool showScan)
		{
			try
			{
				m_tlpMain.SuspendLayout();
				groupBoxHA.Visible = !showScan;
				m_tlpScanning.Visible = showScan;
			}
			finally
			{
				m_tlpMain.ResumeLayout();
			}
		}

        /// <summary>
        /// Called after we determine if the selected VM is agile or not. Fills the combo box with the correct PriorityWrappers.
        /// </summary>
        public void RefillPrioritiesComboBox()
        {
            Program.AssertOnEventThread();

            m_comboBoxProtectionLevel.Items.Clear();
			ToggleScanningVmAgile(false);

            List<VM.HA_Restart_Priority> restartPriorities = VM.GetAvailableRestartPriorities(vm.Connection);
            foreach (var restartPriority in restartPriorities)
            {
                // add "restart" priorities only is vm is agile
                if (VM.HaPriorityIsRestart(vm.Connection, restartPriority) &&
                    (!vmIsAgile || GpuGroup != null || VgpuType != null))
                    continue;
                m_comboBoxProtectionLevel.Items.Add(new PriorityWrapper(restartPriority));
            }

            // Select appropriate entry in combo box
            bool found = false;

            foreach (PriorityWrapper w in m_comboBoxProtectionLevel.Items)
            {
                if (w.Priority == SelectedPriority)
                {
                    found = true;
                    m_comboBoxProtectionLevel.SelectedItem = w;
                    break;
                }
            }

            if (!found)
            {
                // Someone might have set a High/Medium/Low restart priority for a non-agile VM through the CLI,
                // even though this is not possible through the GUI. Hence we need to add that priority to the
                // combo box just to prevent things screwing up.
                m_comboBoxProtectionLevel.Items.Insert(0, new PriorityWrapper(SelectedPriority));
                m_comboBoxProtectionLevel.SelectedIndex = 0;
            }
        }

        private void Repopulate()
        {
            System.Diagnostics.Trace.Assert(vm != null);
			UpdateEnablement();
            // Select the current priority from the list
            foreach (PriorityWrapper p in m_comboBoxProtectionLevel.Items)
            {
                if (p.Priority == vm.HARestartPriority())
                {
                    m_comboBoxProtectionLevel.SelectedItem = p;
                    break;
                }
            }
            nudOrder.Value = vm.order;
            nudStartDelay.Value = vm.start_delay;
        }

        private void UpdateEnablement()
        {
            // Disable editing if pool master doesn't have HA license flag
            Host host = Helpers.GetMaster(vm.Connection);
            if (host == null || Host.RestrictHA(host))
            {
            	m_labelHaStatus.Text = Messages.HA_LICENSE_DISABLED;
				m_tlpPriority.Visible = false;
            	haNtolIndicator.Visible = false;
            	m_linkLabel.Visible = false;
                return;
            }

            // Disable editing if HA isn't switched on
            Pool pool = Helpers.GetPool(vm.Connection);
            if (pool == null)
            {
                m_labelHaStatus.Text = Messages.HA_STANDALONE_SERVER;
                m_tlpPriority.Visible = false;
                haNtolIndicator.Visible = false;
                m_linkLabel.Visible = false;
                return;
            }

            if (!pool.ha_enabled)
            {
                m_labelHaStatus.Text = String.Format(Messages.HA_NOT_CONFIGURED, Helpers.GetName(pool).Ellipsise(30));
                m_tlpPriority.Visible = false;
                haNtolIndicator.Visible = false;
                m_linkLabel.Visible = true;
                m_linkLabel.Text = Messages.HA_CONFIGURE_NOW;
                return;
            }

            // Disable editing if not all hosts are live (because we can't do a db_sync)
            List<string> deadHosts = new List<string>();
            foreach (Host member in pool.Connection.Cache.Hosts)
            {
                if (!member.IsLive())
                {
                    deadHosts.Add(Helpers.GetName(member).Ellipsise(30));
                }
            }
            if (deadHosts.Count > 0)
            {
            	m_labelHaStatus.Text = String.Format(Messages.HA_CANNOT_EDIT_WITH_DEAD_HOSTS_WRAPPED, String.Join("\n", deadHosts.ToArray()));
				m_tlpPriority.Visible = false;
				haNtolIndicator.Visible = false;
				m_linkLabel.Visible = false;
				return;
            }

            // Disable editing if we can't find out the HA number from the server (for less privileged users).
            if (haNtolIndicator.Ntol == -1)
            {
				m_labelHaStatus.Text = Messages.HA_NTOL_UNKNOWN;
				m_tlpPriority.Visible = false;
				haNtolIndicator.Visible = false;
				m_linkLabel.Visible = false;
                return;
            }

			m_labelHaStatus.Text = String.Format(Messages.HA_CONFIGURED, Helpers.GetName(pool).Ellipsise(30));
			m_tlpPriority.Visible = true;
			haNtolIndicator.Visible = true;
			m_linkLabel.Visible = true;
            m_linkLabel.Text = Messages.HA_EDIT_NOW;
        }

        private bool ChangesMadeInHA()
        {
        	return IsHaEditable() && (SelectedPriority != origRestartPriority || haNtolIndicator.Ntol != origNtol);    
        }

        private bool ChangesMadeInStartupOptions()
        {
            return nudOrder.Value != origOrder || nudStartDelay.Value != origStartDelay;
		}

		public VM.HA_Restart_Priority SelectedPriority { get; private set; }

        public GPU_group GpuGroup { private get; set; }

        public VGPU_type VgpuType { private get; set; }

		private bool IsHaEditable()
		{
			return m_tlpPriority.Visible || m_linkLabel.Visible || haNtolIndicator.Visible;
		}

		#region IEditPage implementation

		/// <summary>
		/// Must be a VM.
		/// </summary>
		public void SetXenObjects(IXenObject orig, IXenObject clone)
		{
			// This should only ever be set once
			System.Diagnostics.Trace.Assert(vm == null);
			vm = (VM)clone;

            origRestartPriority = vm.HARestartPriority();
		    SelectedPriority = origRestartPriority;
			origOrder = vm.order;
			origStartDelay = vm.start_delay;

			Repopulate();
			haNtolIndicator.Connection = vm.Connection;
			haNtolIndicator.Settings = Helpers.GetVmHaRestartPriorities(vm.Connection, Properties.Settings.Default.ShowHiddenVMs);
			haNtolIndicator.UpdateInProgressChanged += new EventHandler(haNtolIndicator_UpdateInProgressChanged);

			// Put property changed listener on pool, disable edits if HA becomes disabled
			pool = Helpers.GetPoolOfOne(vm.Connection);
			if (pool != null)
			{
				pool.PropertyChanged += pool_PropertyChanged;
			}
			origNtol = pool.ha_host_failures_to_tolerate;

			// Put property changed listener on all hosts to listen for changes in their Host_metrics
			foreach (Host host in vm.Connection.Cache.Hosts)
			{
				host.PropertyChanged += host_PropertyChanged;
				// Remember the host so we can deregister the listener later
				hosts.Add(host);
				// Also register on the current metrics object - we need to know if a host goes down
				Host_metrics metrics = vm.Connection.Resolve(host.metrics);
				if (metrics != null)
				{
					metrics.PropertyChanged += metrics_PropertyChanged;
					// Remember the metrics object so we can deregister the listener later
					hostMetrics.Add(metrics);
				}
			}

			// Listen for new hosts so that we can add metrics listeners
			vm.Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);

			// Start thread to determine if VM is agile
			ToggleScanningVmAgile(true);

			Thread t = new Thread(UpdateVMAgility);
			t.Name = "Updater for VM agility for " + Helpers.GetName(vm);
			t.IsBackground = true;
			t.Priority = ThreadPriority.Highest;
			t.Start();
		}

		public bool HasChanged
        {
            get
            {
                return ChangesMadeInHA() || ChangesMadeInStartupOptions();
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

		/// <summary>
		/// a.k.a. OnClosing()
		/// </summary>
        public void Cleanup()
        {
            haNtolIndicator.StopNtolUpdate();

            // Deregister listeners
            if (vm != null)
            {
                vm.Connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
            }

            if (pool != null)
            {
                pool.PropertyChanged -= pool_PropertyChanged;
            }

            foreach (Host host in hosts)
            {
                host.PropertyChanged -= host_PropertyChanged;
            }

            foreach (Host_metrics metrics in hostMetrics)
            {
                metrics.PropertyChanged -= metrics_PropertyChanged;
            }
        }

        private bool PoolHasHAEnabled
        {
            get
            {
                return pool != null && pool.ha_enabled;
            }
        }

        public bool ValidToSave
        {
            get
            {
                if (!PoolHasHAEnabled)
                    return true;

                // If skankPanel is disabled, so is editing
            	var validToSaveHA = !IsHaEditable() || this.vm != null && !haNtolIndicator.UpdateInProgress && haNtolIndicator.Ntol >= 0;

                var validToSaveStartupOptions = this.vm != null;

                return validToSaveHA && validToSaveStartupOptions;
            }
        }

        public AsyncAction SaveSettings()
        {

            if (vm == null || (!ChangesMadeInStartupOptions() && !ChangesMadeInHA()))
                return null;

            var settings = new Dictionary<VM, VMStartupOptions>();
            if (ChangesMadeInHA() && PoolHasHAEnabled)
            {
                settings[vm] = new VMStartupOptions((long) nudOrder.Value, (long) nudStartDelay.Value, SelectedPriority);
                return new SetHaPrioritiesAction(vm.Connection, settings, haNtolIndicator.Ntol, true);
            }

            settings[vm] = new VMStartupOptions((long)nudOrder.Value, (long)nudStartDelay.Value);
            return new SetVMStartupOptionsAction(vm.Connection, settings, true);
		}

		#endregion

		#region Control Event handlers

		private void haNtolIndicator_UpdateInProgressChanged(object sender, EventArgs e)
		{
			UpdateEnablement();
		}

		private void comboBoxRestartPriority_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (vm == null)
				return;

		    var pw = m_comboBoxProtectionLevel.SelectedItem as PriorityWrapper;
            if (pw != null)
                SelectedPriority = pw.Priority;

            comboLabel.Text = Helpers.RestartPriorityDescription(SelectedPriority);

			var settings = Helpers.GetVmHaRestartPriorities(vm.Connection, Properties.Settings.Default.ShowHiddenVMs);
			// Supplement with the changed setting
            settings[vm] = SelectedPriority;

			// This will trigger an update in the ntol indicator.
			haNtolIndicator.Settings = settings;
		}

		private void m_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (pool == null)
				return;
			HAPage.EditHA(pool);
		}

		#endregion

		private class PriorityWrapper
        {
            public readonly VM.HA_Restart_Priority Priority;

            public PriorityWrapper(VM.HA_Restart_Priority priority)
            {
                this.Priority = priority;
            }

            public override string ToString()
            {
                return Helpers.RestartPriorityI18n(this.Priority);
            }
        }

        public void StartNtolUpdate()
        {
            haNtolIndicator.Visible = true;
            haNtolIndicator.StartNtolUpdate();
        }
    }
}
