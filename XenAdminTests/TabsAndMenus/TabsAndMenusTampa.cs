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

using System.Linq;

using NUnit.Framework;

using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Core;
using XenAPI;
using System.Collections.Generic;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Model;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class TabsAndMenusTampa : TabsAndMenus
    {
        public TabsAndMenusTampa()
            : base("tampa-poolof16and23-xapi-db.xml")
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            DisableAllPlugins();
        }

        private string[] XenCenterTabs = new [] { "Home", "Search" };
        private string[] PoolTabs = new[] { "General", "Memory", "Storage", "Networking", "HA", "WLB", "Users", "Search" };
        private string[] HostTabs = new[] { "General", "Memory", "Storage", "Networking", "NICs", "Console", "Performance", "Users", "Search" };
        private string[] VMTabs = new[] { "General", "Memory", "Storage", "Networking", "Console", "Performance", "Snapshots", "Search" };
        private string[] DefaultTemplateTabs = new[] { "General", "Memory", "Networking", "Search" };
        private string[] OtherInstallMediaTabs = new[] { "General", "Memory", "Storage", "Networking", "Search" };
        private string[] UserTemplateTabs = new[] { "General", "Memory", "Storage", "Networking", "Search" };
        private string[] SRTabs = new[] { "General", "Storage", "Search" };
        private string[] SnapshotTabs = new[] { "General", "Memory", "Networking", "Search" };
        private string[] VDITabs = new [] { "General", "Search" };
        private string[] NetworkTabs = new [] { "Search" };
        private string[] GroupingTagTabs = new [] { "Search" };
        private string[] FolderTabs = new [] { "Search" };

        [Test]
        public void Tabs_XenCenterNode()
        {
            VerifyTabs(null, XenCenterTabs);
        }

        [Test]
        public void Tabs_Pool()
        {
            VerifyTabs(GetAnyPool(), PoolTabs);
        }

        [Test]
        public void Tabs_Host()
        {
            foreach (Host host in GetAllXenObjects<Host>())
            {
                VerifyTabs(host, HostTabs);
            }
        }

        [Test]
        public void Tabs_VM()
        {
            foreach (VM vm in GetAllXenObjects<VM>(v => !v.is_a_template && !v.is_control_domain))
            {
                VerifyTabs(vm, VMTabs);
            }
        }

        [Test]
        public void Tabs_DefaultTemplate()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);
            VerifyTabs(GetAnyDefaultTemplate(), DefaultTemplateTabs);
        }

        [Test]
        public void Tabs_OtherInstallMedia()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);
            VerifyTabs(GetAnyDefaultTemplate(v => v.name_label == "Other install media"), OtherInstallMediaTabs);
        }


        [Test]
        public void Tabs_UserTemplate()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);
            foreach (VM vm in GetAllXenObjects<VM>(v =>!v.IsHidden() &&v.is_a_template && !v.DefaultTemplate() && !v.is_a_snapshot))
            {
                VerifyTabs(vm, UserTemplateTabs);
            }
        }

        [Test]
        public void Tabs_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);
            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR()))
            {
                VerifyTabs(sr, SRTabs);
            }
        }

        [Test]
        public void Tabs_Snapshot()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    VerifyTabs(snapshot, SnapshotTabs);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void Tabs_VDI()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                VerifyTabs(GetAnyVDI(v => v.name_label != "base copy" && !v.IsHidden()), VDITabs);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void Tabs_Network()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Host internal management network"))
                {
                    VerifyTabs(network, NetworkTabs);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void Tabs_GroupingTag()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                VirtualTreeNode n = GetAllTreeNodes().Find(v => v.Tag is GroupingTag);
                VerifyTabs((GroupingTag)n.Tag, GroupingTagTabs);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        public void Tabs_Folder()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Folders);
            try
            {
                var folders = GetAllXenObjects<Folder>().Where(f => !(string.IsNullOrEmpty(f.ToString())));
                foreach (Folder folder in folders)
                    VerifyTabs(folder, FolderTabs);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ContextMenu_XenCenterNode_AllClosed()
        {
            new TabsAndMenusBoston().ContextMenu_XenCenterNode_AllClosed();
        }

        [Test]
        public void ContextMenu_XenCenterNode_RootOpen()
        {
            new TabsAndMenusBoston().ContextMenu_XenCenterNode_RootOpen();
        }

        [Test]
        public void ContextMenu_XenCenterNode_PoolOpen()
        {
            new TabsAndMenusBoston().ContextMenu_XenCenterNode_PoolOpen();
        }

        [Test]
        public void ContextMenu_XenCenterNode_AllOpen()
        {
            new TabsAndMenusBoston().ContextMenu_XenCenterNode_AllOpen();
        }

        [Test]
        public void ContextMenu_Pool()
        {
            new TabsAndMenusBoston().ContextMenu_Pool();
        }

        [Test]
        public void ContextMenu_Master()
        {
            new TabsAndMenusBoston().ContextMenu_Master();
        }

        [Test]
        public void ContextMenu_Slave()
        {
            new TabsAndMenusBoston().ContextMenu_Slave();
        }

        //[Test]
        //public void ContextMenu_VMWithTools()
        //{
        //    new TabsAndMenusGeorge().ContextMenu_VMWithTools();
        //}

        [Test]
        [Ignore("Ignore this test, the test needs to be fixed, see CA-123967.")]
        public void ContextMenu_VMWithTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(HasTools))
            {
                if(vm.Name().Contains("22")) //Skip over the slave servers VMs 
                    continue;

                List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                expectedMenuItems.Add(new ExpectedTextMenuItem("S&hut Down", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("S&uspend", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Reb&oot", true));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Shut Do&wn", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Re&boot", true));
                expectedMenuItems.Add(new ExpectedSeparator());

                expectedMenuItems.Add(new ExpectedTextMenuItem("M&igrate to Server", true, false,
                                                                   new ExpectedMenuItem[]
                                                                       {
                                                                           new ExpectedTextMenuItem(
                                                                               "&Home Server (Current server)", false),
                                                                           new ExpectedTextMenuItem(
                                                                               "dt16 (Current server)", false, false,
                                                                               true),
                                                                           new ExpectedTextMenuItem("dt23", true, false,
                                                                                                    true),
                                                                           new ExpectedSeparator(),
                                                                           new ExpectedTextMenuItem(
                                                                               "&Migrate VM wizard...", true, false,
                                                                               true)
                                                                       }));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("Ta&ke a Snapshot...", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Assign to vA&pp", true, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true)}));
                
                
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("P&roperties", true));

                VerifyContextMenu(vm, expectedMenuItems.ToArray());
            }
        }


        [Test]
        public void ContextMenu_SR()
        {
            new TabsAndMenusBoston().ContextMenu_SR();
        }

        [Test]
        public void ContextMenu_DefaultTemplate()
        {
            new TabsAndMenusBoston().ContextMenu_DefaultTemplate();
        }

        //[Test]
        //public void ContextMenu_UserTemplate()
        //{
        //    EnsureDefaultTemplatesShown();
        //    foreach (VM vm in GetAllXenObjects<VM>(v => v.InstantTemplate))
        //    {
        //        VerifyContextMenu(vm, new ExpectedMenuItem[] {
        //            new ExpectedTextMenuItem("&New VM wizard...", true),
        //            new ExpectedSeparator(),
        //            new ExpectedTextMenuItem("&Export to File...", true),
        //            new ExpectedTextMenuItem("&Copy", true),
        //            new ExpectedSeparator(),
        //            new ExpectedTextMenuItem("&Delete Template...", true),
        //            new ExpectedSeparator(),
        //            new ExpectedTextMenuItem("Properties", true)
        //        });
        //    }
        //}

        [Test]
        public void ContextMenu_Snapshot()
        {
            new TabsAndMenusBoston().ContextMenu_Snapshot();
        }

        [Test]
        public void ContextMenu_VDI()
        {
            new TabsAndMenusBoston().ContextMenu_VDI();
        }

        //[Test]
        //public void ContextMenu_Network()
        //{
        //    new TabsAndMenusBoston().ContextMenu_Network();
        //}

        [Test]
        public void ContextMenu_GroupingTag()
        {
            new TabsAndMenusBoston().ContextMenu_GroupingTag();
        }

        [Test]
        public void ContextMenu_Folder()
        {
            new TabsAndMenusBoston().ContextMenu_Folder();
        }

        [Test]
        public void TestPowerStateChangeUpdatesToolBar()
        {
            // select a running VM
            VM vm = GetAnyVM(v => v.power_state == vm_power_state.Running);

            Assert.IsTrue(SelectInTree(vm), "Couldn't select VM");

            // assert that start button is disabled (as VM is running.)
            Assert.IsTrue(!MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.Enabled, "Start button should be disabled.");

            // click the force-shutdown menu item from the VM menu.
            MW(delegate
            {
                MainWindowWrapper.VMMenu.ShowDropDown();
                MainWindowWrapper.VMMenuItems.StartShutdownMenu.ShowDropDown();
                MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ForceShutdownToolStripMenuItem.PerformClick();
            });

            // assert it has halted and that the start-vm toolbar button has become enabled.
            MWWaitFor(() => vm.power_state == vm_power_state.Halted && MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.Enabled, "Toolbar wasn't updated on VM shutdown.");

            // now restart VM.
            MW(MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.PerformClick);
            MWWaitFor(() => vm.power_state == vm_power_state.Running, "Couldn't start VM.");
        }
    }
}
