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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class LaunchConversionManagerCommand : Command
    {
        public LaunchConversionManagerCommand()
        {
        }

        public LaunchConversionManagerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public override string MenuText => Messages.MAINWINDOW_CONVERSION_MANAGER_MENU_ITEM;

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();
            return con != null && con.Cache.VMs.Any(v => v.IsConversionVM());
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();

            if (Helpers.FeatureForbidden(con, Host.RestrictConversion))
            {
                var msg = HiddenFeatures.LinkLabelHidden
                    ? Messages.UPSELL_BLURB_CONVERSION
                    : Messages.UPSELL_BLURB_CONVERSION + Messages.UPSELL_BLURB_TRIAL;

                using (var dlg = new UpsellDialog(msg, InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                    dlg.ShowDialog(Parent);
            }
            else if (!con.Session.IsLocalSuperuser && !Registry.DontSudo && con.Session.Roles.All(r => r.name_label != Role.MR_ROLE_POOL_ADMIN))
            {
                var currentRoles = con.Session.Roles;
                currentRoles.Sort();

                var msg = string.Format(Messages.CONVERSION_RBAC_RESTRICTION, currentRoles[0].FriendlyName(),
                    Role.FriendlyName(Role.MR_ROLE_POOL_ADMIN));

                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Error, msg),
                    ThreeButtonDialog.ButtonOK))
                    dlg.ShowDialog(Parent);
            }
            else if (selection.First is VM vm && vm.IsConversionVM())
            {
                MainWindowCommandInterface.ShowPerConnectionWizard(con, new ConversionDialog(con, vm));
            }
            else
            {
                var conversionVms = con.Cache.VMs.Where(v => v.IsConversionVM()).ToArray();
                MainWindowCommandInterface.ShowPerConnectionWizard(con, new ConversionDialog(con, conversionVms));
            }
        }
    }
}
