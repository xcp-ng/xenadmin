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


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the HA wizard.
    /// </summary>
    internal class HACommand : Command
    {
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
                // Show VM restart priority editor
                MainWindowCommandInterface.ShowPerConnectionWizard(connection, new EditVmHaPrioritiesDialog(pool));
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
            if (selection.Count == 1)
            {
                Pool poolAncestor = selection[0].PoolAncestor;
                bool inPool = poolAncestor != null;

                if (inPool )
                {
                    Host master = Helpers.GetMaster(poolAncestor.Connection);

                    if (master == null || HelpersGUI.FindActiveHaAction(poolAncestor.Connection) != null || poolAncestor.Locked)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
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
