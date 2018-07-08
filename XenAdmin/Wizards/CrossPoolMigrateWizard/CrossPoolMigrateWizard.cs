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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public enum WizardMode { Migrate, Move, Copy }

    internal partial class CrossPoolMigrateWizard : XenWizardBase
	{
        private CrossPoolMigrateDestinationPage m_pageDestination;
        private CrossPoolMigrateStoragePage m_pageStorage;
        private CrossPoolMigrateNetworkingPage m_pageNetwork;
        private CrossPoolMigrateFinishPage m_pageFinish;
        private CrossPoolMigrateTransferNetworkPage m_pageTransferNetwork;
        private RBACWarningPage m_pageTargetRbac;

        private CrossPoolMigrateCopyModePage m_pageCopyMode;
        private IntraPoolCopyPage m_pageIntraPoolCopy;


        private IXenConnection TargetConnection { get; set; }

		private Dictionary<string, VmMapping> m_vmMappings = new Dictionary<string, VmMapping>();

        private Host hostPreSelection = null;

        private WizardMode wizardMode;

	    private bool _resumeAfterMigrate;

        // Note that resumeAfter is currently only implemented for Migrate mode, used for resume on server functionality
	    public CrossPoolMigrateWizard(IXenConnection con, SelectedItemCollection selection, Host targetHostPreSelection, WizardMode mode, bool resumeAfterMigrate = false)
	        : base(con)
	    {
            InitializeComponent();
            hostPreSelection = targetHostPreSelection;
	        wizardMode = mode;
	        InitialiseWizard(selection);
	        _resumeAfterMigrate = resumeAfterMigrate;
	    }

        private bool HasTemplatesOnly { get; set; }

        private bool IsIntraPoolMigration()
        {
            return m_vmMappings.All(IsIntraPoolMigration);
        }

        private bool IsIntraPoolMigration(KeyValuePair<string, VmMapping> mapping)
        {
            VM vm = xenConnection.Resolve(new XenRef<VM>(mapping.Key));
            if (vm.resident_on == mapping.Value.XenRef)
                return true;

            Pool pool = Helpers.GetPool(vm.Connection);
            if (mapping.Value.XenRef is XenRef<Pool>)
            {
                Pool targetPool = TargetConnection.Resolve(mapping.Value.XenRef as XenRef<Pool>);
                if ( pool == targetPool)
                    return true;
            }

            Host host = xenConnection.Resolve(vm.resident_on) ?? Helpers.GetMaster(xenConnection);
            if (mapping.Value.XenRef is XenRef<Host>)
            {
                Host targetHost = TargetConnection.Resolve(mapping.Value.XenRef as XenRef<Host>);
                Pool targetPool = Helpers.GetPool(targetHost.Connection);

                // 2 stand alone hosts
                if(pool == null && targetPool == null)
                {
                    if(targetHost == host)
                        return true;

                    return false;
                }
                    
                if ( pool == targetPool)
                    return true;
            }

            return false;
        }

        private bool IsIntraPoolMove()
        {
            return wizardMode == WizardMode.Move && m_vmMappings.All(IsIntraPoolMove);
        }

        private bool IsIntraPoolMove(KeyValuePair<string, VmMapping> mapping)
        {
            VM vm = xenConnection.Resolve(new XenRef<VM>(mapping.Key));
            return vm != null && vm.CanBeMoved() && IsIntraPoolMigration(mapping);
        }

        private bool IsCopyTemplate()
        {
            return wizardMode == WizardMode.Copy && m_vmMappings.Any(IsTemplate);
        }

        private bool IsTemplate(KeyValuePair<string, VmMapping> mapping)
        {
            VM vm = xenConnection.Resolve(new XenRef<VM>(mapping.Key));
            return vm != null && vm.is_a_template;
        }


        private void InitialiseWizard(SelectedItemCollection selection)
        {
            var vmsFromSelection = new List<VM>();

            foreach (var item in selection)
            {
                var vm = item.XenObject as VM;
                if (vm == null)
                    continue;

                vmsFromSelection.Add(vm);
                m_vmMappings.Add(item.XenObject.opaque_ref, new VmMapping { VmNameLabel = item.XenObject.Name() });
            }

            HasTemplatesOnly = vmsFromSelection.Count > 0 && vmsFromSelection.All(vm => vm.is_a_template);

            UpdateWindowTitle();

            m_pageDestination = new CrossPoolMigrateDestinationPage(vmsFromSelection, 
                    wizardMode, GetSourceConnectionsForSelection(selection))
            {
                VmMappings = m_vmMappings,
                Connection = selection.GetConnectionOfFirstItem()
            };
            m_pageDestination.SetDefaultTarget(hostPreSelection);

            m_pageStorage = new CrossPoolMigrateStoragePage(wizardMode);
            m_pageNetwork = new CrossPoolMigrateNetworkingPage(HasTemplatesOnly, wizardMode);
            m_pageTransferNetwork = new CrossPoolMigrateTransferNetworkPage(vmsFromSelection, HasTemplatesOnly, wizardMode);
            m_pageFinish = new CrossPoolMigrateFinishPage(selection.Count(), wizardMode, HasTemplatesOnly) { SummaryRetreiver = GetVMMappingSummary };
            m_pageTargetRbac = new RBACWarningPage();

            m_pageCopyMode = new CrossPoolMigrateCopyModePage(vmsFromSelection);
            m_pageIntraPoolCopy = new IntraPoolCopyPage(vmsFromSelection);

            if (wizardMode == WizardMode.Copy)
                AddPages(m_pageCopyMode, m_pageIntraPoolCopy);
            else
                AddPages(m_pageDestination, m_pageStorage, m_pageFinish);
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        protected override void FinishWizard()
        {
            if (!AllVMsAvailable(m_vmMappings, xenConnection))
            {
                base.FinishWizard();
                return;
            }

            if (wizardMode == WizardMode.Copy && m_pageCopyMode.IntraPoolCopySelected)
            {
                if (m_pageIntraPoolCopy.CloneVM)
                    new VMCloneAction(m_pageIntraPoolCopy.TheVM, m_pageIntraPoolCopy.NewVmName, m_pageIntraPoolCopy.NewVMmDescription).RunAsync();

                else if (m_pageIntraPoolCopy.SelectedSR != null)
                    new VMCopyAction(m_pageIntraPoolCopy.TheVM, m_pageIntraPoolCopy.TheVM.GetStorageHost(false),
                        m_pageIntraPoolCopy.SelectedSR, m_pageIntraPoolCopy.NewVmName, m_pageIntraPoolCopy.NewVMmDescription).RunAsync();
                
                base.FinishWizard();
                return;
            }

            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
            {
                VM vm = xenConnection.Resolve(new XenRef<VM>(pair.Key));
                
                Host target = TargetConnection.Resolve(m_vmMappings[pair.Key].XenRef as XenRef<Host>);

                //if a pool has been selected but no specific homeserver Key is the pool opaque ref
                if(target == null)
                {
                    Pool targetPool = TargetConnection.Resolve(m_vmMappings[pair.Key].XenRef as XenRef<Pool>);

                    if (targetPool == null)
                    {
                        ShowErrorMessageBox(Messages.CPM_WIZARD_ERROR_TARGET_DISCONNECTED);
                        base.FinishWizard();
                        return;
                    }
                        
                    target = TargetConnection.Resolve(targetPool.master);
                }

                if(target == null)
                    throw new ApplicationException("Cannot resolve the target host");

                if (wizardMode == WizardMode.Move && IsIntraPoolMove(pair))
                    new VMMoveAction(vm, pair.Value.Storage, target).RunAsync();
                else
                {
                    var isCopy = wizardMode == WizardMode.Copy;
                    var migrateAction = 
                        new VMCrossPoolMigrateAction(vm, target, SelectedTransferNetwork, pair.Value, isCopy);

                    if (_resumeAfterMigrate)
                    {
                        var title = VMCrossPoolMigrateAction.GetTitle(vm, target, isCopy);
                        var startDescription = isCopy ? Messages.ACTION_VM_COPYING: Messages.ACTION_VM_MIGRATING;
                        var endDescription = isCopy ? Messages.ACTION_VM_COPIED: Messages.ACTION_VM_MIGRATED;

                        var actions = new List<AsyncAction>()
                        {
                            migrateAction,
                            new ResumeAndStartVMsAction(vm.Connection, target, new List<VM>{vm}, new List<VM>(), null, null)
                        };

                        new MultipleAction(vm.Connection, title, startDescription, endDescription,
                            actions, true, false, true).RunAsync();
                    }
                    else
                    {
                        migrateAction.RunAsync();
                    }
                }
            }
            
            base.FinishWizard();
        }

        private static void ShowErrorMessageBox(string message)
        {
            using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, message)))
            {
                dlg.ShowDialog(Program.MainWindow);
            }
        }

	    private List<IXenConnection> GetSourceConnectionsForSelection(IEnumerable<SelectedItem> selection)
        {
            return 
                wizardMode == WizardMode.Copy 
                    ?   selection.Select(item => item.Connection).Where(conn => conn != null).Distinct().ToList()
                    :   new List<IXenConnection>();
        }

        private void UpdateWindowTitle()
        {
            if(m_vmMappings != null &&  m_vmMappings.Count > 0 && !string.IsNullOrEmpty(m_vmMappings.First().Value.TargetName))
            {
                var messageText = wizardMode == WizardMode.Migrate
                    ? Messages.CPM_WIZARD_TITLE_AND_LOCATION
                    : wizardMode == WizardMode.Move 
                        ? Messages.MOVE_VM_WIZARD_TITLE_AND_LOCATION
                        : IsCopyTemplate() ? Messages.COPY_TEMPLATE_WIZARD_TITLE_AND_LOCATION : Messages.COPY_VM_WIZARD_TITLE_AND_LOCATION;
                Text = String.Format(messageText, m_vmMappings.First().Value.TargetName);
            }
            else
                Text = wizardMode == WizardMode.Migrate
                    ? Messages.CPM_WIZARD_TITLE 
                    : wizardMode == WizardMode.Move 
                        ? Messages.MOVE_VM_WIZARD_TITLE 
                        : IsCopyTemplate() ? Messages.COPY_TEMPLATE_WIZARD_TITLE : Messages.COPY_VM_WIZARD_TITLE;
        }

        protected override void UpdateWizardContent(XenTabPage page)
        {
  
            Type type = page.GetType();
 
            if (type == typeof(CrossPoolMigrateDestinationPage))
            {
                RemovePage(m_pageNetwork);
                RemovePage(m_pageTransferNetwork);
                RemovePage(m_pageTargetRbac);
                m_vmMappings = m_pageDestination.VmMappings;
                TargetConnection = m_pageDestination.ChosenItem == null ? null : m_pageDestination.ChosenItem.Connection;
                m_pageStorage.TargetConnection = TargetConnection;
                m_pageNetwork.TargetConnection = TargetConnection;
                ConfigureRbacPage();
                UpdateWindowTitle();

                // add Transfer network page for all cases except intra-pool move (which is performed via VMMoveAction) 
                if (!IsIntraPoolMove())
                    AddAfterPage(m_pageStorage, m_pageTransferNetwork);
                m_pageTransferNetwork.Connection = TargetConnection;

                if(!IsIntraPoolMigration())
                {
                    AddAfterPage(m_pageStorage, m_pageNetwork);
                }
                //If no network mappings xapi should map the networks
                //with the same names together - ideal for intra-pool
                ClearAllNetworkMappings();
                m_pageStorage.VmMappings = m_vmMappings;
            }
            else if (type == typeof(CrossPoolMigrateStoragePage))
            {
                UpdateWindowTitle();
                m_vmMappings = m_pageStorage.VmMappings;
                m_pageNetwork.VmMappings = m_vmMappings;
            }
            else if (type == typeof(CrossPoolMigrateNetworkingPage))
            {
                UpdateWindowTitle();
                m_vmMappings = m_pageNetwork.VmMappings;
            }
            else if (type == typeof(CrossPoolMigrateTransferNetworkPage))
            {
                UpdateWindowTitle();
                string netRef = m_pageTransferNetwork.NetworkUuid.Key;
                SelectedTransferNetwork = TargetConnection.Cache.Networks.FirstOrDefault(n => n.uuid == netRef);
            }
            else if (type == typeof(CrossPoolMigrateCopyModePage))
            {
                if (m_pageCopyMode.IntraPoolCopySelected)
                {
                    RemovePagesFrom(1);
                    AddAfterPage(m_pageCopyMode, m_pageIntraPoolCopy);
                }
                else
                {
                    RemovePagesFrom(1);
                    AddAfterPage(m_pageCopyMode, m_pageDestination, m_pageStorage, m_pageFinish);
                }
            }

            if (type != typeof(CrossPoolMigrateFinishPage))
                NotifyNextPagesOfChange(m_pageDestination, m_pageStorage, m_pageNetwork, m_pageTransferNetwork, m_pageFinish);
        }

        private XenAPI.Network SelectedTransferNetwork { get; set; }

        private void ClearAllNetworkMappings()
        {
            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
                pair.Value.Networks = new Dictionary<string, XenAPI.Network>();
        }

        protected override string WizardPaneHelpID()
        {
            return FormatHelpId(CurrentStepTabPage.HelpID);
        }

        private bool ConnectionDoesNotRequireRBAC(IXenConnection connection)
        {
            if( connection == null )
                throw new NullReferenceException("RBAC check was given a null connection");

            if (connection.Session.IsLocalSuperuser)
                return true;

            if (Helpers.GetMaster(connection).external_auth_type == Auth.AUTH_TYPE_NONE)
                return true;

            return false;
        }


        private void ConfigureRbacPage()
        {
            if (ConnectionDoesNotRequireRBAC(xenConnection) && ConnectionDoesNotRequireRBAC(TargetConnection))
                return;

            m_pageTargetRbac.ClearPermissionChecks();
            RBACWarningPage.WizardPermissionCheck migrateCheck =
                new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_CROSS_POOL_MIGRATE_VM_BLOCKED) { Blocking = true };
            migrateCheck.AddApiCheckRange(VMCrossPoolMigrateAction.StaticRBACDependencies);

            m_pageTargetRbac.AddPermissionChecks(xenConnection, migrateCheck);
            if (!xenConnection.Equals(TargetConnection))
                m_pageTargetRbac.AddPermissionChecks(TargetConnection, migrateCheck);

            AddAfterPage(m_pageDestination, m_pageTargetRbac);
        }

        private IEnumerable<SummaryDetails> GetVMMappingSummary()
        {
            //Use decorators to build a summary
            MappingSummary summary = new VMMappingSummary();
            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
            {
                if (HasTemplatesOnly)
                    summary = new TemplateTitleSummary(summary, pair.Value);
                else
                    summary = new VmTitleSummary(summary, pair.Value);

                summary = new DestinationPoolSummary(summary, pair.Value, TargetConnection);
                summary = new TargetServerSummary(summary, pair.Value, TargetConnection);
                summary = new TransferNetworkSummary(summary, m_pageTransferNetwork.NetworkUuid.Value);
                summary = new StorageSummary(summary, pair.Value, xenConnection);
                summary = new NetworkSummary(summary, pair.Value, xenConnection); 
                summary = new SummarySplitter(summary);
            }
            return summary.Details;
        }

        /// <summary>
        /// Checks if all VMs are still available for migration and shows a warning message if the check fails
        /// </summary>
        /// <returns>true if check succeded, false if failed</returns>
        internal static bool AllVMsAvailable(List<VM> vms)
        {
            Func<bool> vmCheck = delegate
                                     {
                                         if (vms == null || vms.Count == 0 || vms[0] == null || vms[0].Connection == null)
                                             return false;
                                         var connection = vms[0].Connection; // the connection on which to check VM availability
                                         return vms.All(vm => connection.Resolve(new XenRef<VM>(vm.opaque_ref)) != null);
                                     };

            return PerformCheck(vmCheck);
        }

        /// <summary>
        /// Checks if all VMs are still available for migration and shows a warning message if the check fails
        /// </summary>
        /// <returns>true if check succeded, false if failed</returns>
        internal static bool AllVMsAvailable(Dictionary<string, VmMapping> vmMappings, IXenConnection connection)
        {
            Func<bool> vmCheck = delegate
            {
                if (vmMappings == null || vmMappings.Count == 0 || connection == null)
                    return false;
                return vmMappings.All(kvp => connection.Resolve(new XenRef<VM>(kvp.Key)) != null);
            };

            return PerformCheck(vmCheck);
        }

        /// <summary>
        /// Performs a certain check and shows a warning message if the check fails
        /// </summary>
        /// <param name="check">The check to perform</param>
        /// <returns>true if check succeded, false if failed</returns>
        private static bool PerformCheck(Func<bool> check)
        {
            if (check())
                return true;
            ShowErrorMessageBox(Messages.CPM_WIZARD_VM_MISSING_ERROR);
            return false;
        }

        internal static void ShowWarningMessageBox(string message)
        {
            using (var dlg = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, message, Messages.CPM_WIZARD_TITLE)))
            {
                dlg.ShowDialog(Program.MainWindow);
            }
        }
	}
}
