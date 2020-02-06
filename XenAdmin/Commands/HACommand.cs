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
using System.Text;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the HA wizard.
    /// </summary>
    internal class HACommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HACommand()
        {
        }

        public HACommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HACommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        public HACommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        private void Execute(IXenConnection connection)
        {
            if (connection == null)
                return;

            Pool pool = Helpers.GetPool(connection);
            if (pool == null)
                return;

            if (Helpers.FeatureForbidden(pool, Host.RestrictHA))
            {
                // Show upsell dialog
                using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_HA : Messages.UPSELL_BLURB_HA + Messages.UPSELL_BLURB_TRIAL,
                                                    InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                    dlg.ShowDialog(Parent);
            }
            else if (pool.ha_enabled)
            {
                if (pool.ha_statefiles.Any(sf => pool.Connection.Resolve(new XenRef<VDI>(sf)) != null))
                {
                    // Show VM restart priority editor
                    MainWindowCommandInterface.ShowPerConnectionWizard(connection, new EditVmHaPrioritiesDialog(pool));
                }
                else
                {
                    log.ErrorFormat("Cannot resolve HA statefile VDI (pool {0} has {1} statefiles).",
                        pool.Name(), pool.ha_statefiles.Length);

                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            string.Format(Messages.HA_CONFIGURE_NO_STATEFILE, Helpers.GetName(pool).Ellipsise(30)),
                            Messages.CONFIGURE_HA),
                        "HADisable",
                        ThreeButtonDialog.ButtonOK))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
            }
            else
            {
                // Show wizard to enable HA
                MainWindowCommandInterface.ShowPerConnectionWizard(connection, new HAWizard(pool));
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection[0].Connection);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count != 1)
                return false;

            Pool poolAncestor = selection[0].PoolAncestor;
            if (poolAncestor == null || poolAncestor.Locked)
                return false;

            if (poolAncestor.Connection ==  null || !poolAncestor.Connection.IsConnected)
                return false;

            if (HelpersGUI.FindActiveHaAction(poolAncestor.Connection) != null)
                return false;

            Host master = Helpers.GetMaster(poolAncestor.Connection);
            if (master == null)
                return false;

            return true;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            Pool poolAncestor = item == null ? null : Helpers.GetPool(item.Connection);
            bool inPool = poolAncestor != null;

            if (inPool)
            {
                Host master = Helpers.GetMaster(poolAncestor.Connection);

                if (master == null)
                {
                    return Messages.FIELD_DISABLED;
                }
                else if (HelpersGUI.FindActiveHaAction(poolAncestor.Connection) != null || poolAncestor.Locked)
                {
                    return Messages.POOL_EDIT_IN_PROGRESS;
                }
            }
            return base.GetCantExecuteReasonCore(item);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_HIGH_AVAILABILITY;
            }
        }
    }
}
