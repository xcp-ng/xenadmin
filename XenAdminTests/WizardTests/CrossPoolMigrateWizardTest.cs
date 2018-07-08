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
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAdmin.Wizards.GenericPages;

namespace XenAdminTests.WizardTests.tampa_cpm_one_and_two_host_pools
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class CrossPoolMigrateWizardTest : CrossPoolMigrateWizardTestBase
    {
        public CrossPoolMigrateWizardTest()
            : base(new[] { "Destination Pool", "Storage", "Networking", "Migration Network", "Finish" }, true, true)
        {
            TargetPool = "PoolOfOne";
            SourcePool = "16And23";
            Vm = "16local";
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class CrossPoolMigrateWizardIntraPoolTest : CrossPoolMigrateWizardTestBase
    {
        public CrossPoolMigrateWizardIntraPoolTest()
            : base(new[] { "Destination Pool", "Storage", "Migration Network", "Finish" }, true, true)
        {
            TargetPool = SourcePool = "16And23";
            Vm = "16local";
        }
    }

    internal abstract class CrossPoolMigrateWizardTestBase : WizardTest<CrossPoolMigrateWizard>
    {
        public CrossPoolMigrateWizardTestBase(string[] pageNames, bool canFinish, bool doFinish)
            : base(pageNames, canFinish, doFinish)
            
        {
        }

        protected string TargetPool { set; private get; }
        protected string SourcePool { set; private get; }
        protected string Vm { set; private get; }

        protected override CrossPoolMigrateWizard NewWizard()
        {
            IXenConnection connection = GetAnyConnection(c => c.Name == SourcePool);
            return new CrossPoolMigrateWizard(connection,
                new SelectedItemCollection(new SelectedItem(GetAnyVM(v => v.Name() == Vm))), null, WizardMode.Migrate);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Destination Pool")
                MW(()=>
                       {
                           Thread.Sleep(1000); //Allow the destination combo box to populate
                           ComboBox cb = TestUtils.GetComboBox(wizard, "m_pageDestination.m_comboBoxConnection");
                           SelectItemFromComboBox(cb, TargetPool);
                           Thread.Sleep(3000); //Allow the data grid view to populate
                       });

        }

        private void SelectItemFromComboBox(ComboBox cb, string itemName)
        {
            foreach(IEnableableXenObjectComboBoxItem item in cb.Items)
            {
                if (item.Item.Name() == itemName)
                {
                    cb.SelectedItem = item;
                    return;
                }               
            }
        }
    }
}
