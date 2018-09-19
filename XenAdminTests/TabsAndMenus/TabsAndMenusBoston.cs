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

using XenAPI;
using System.Collections.Generic;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin.XenSearch;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class TabsAndMenusBoston : TabsAndMenus
    {
        public TabsAndMenusBoston()
            : base("boston-db.xml")
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            DisableAllPlugins();
        }

        private string[] XenCenterTabs = new[] { "Home", "Search" };
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
        private string[] GroupingTagTabs = new[] { "Search" };
        private string[] FolderTabs = new[] { "Search" };

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
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
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
            VirtualTreeNode rootNode = FindInTree(null);
            MW(rootNode.Collapse);
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_RootOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);
            MW(delegate
            {
                rootNode.Collapse();
                rootNode.Expand();
            });
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_PoolOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);

            MW(delegate
            {
                rootNode.Collapse();
                rootNode.Expand();
            });
            VirtualTreeNode poolNode = FindInTree(GetAnyPool());

            MW(delegate
            {
                poolNode.Expand();
            });

            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("&Collapse Children", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_AllOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);

            MW(delegate
            {
                rootNode.ExpandAll();
            });
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("&Collapse Children", true)
            });
        }

        [Test]
        public void ContextMenu_Pool()
        {
            var pool = GetAnyPool();
            var expected = new List<ExpectedMenuItem>
                               {
                                   new ExpectedTextMenuItem("New V&M...", true),
                                   new ExpectedTextMenuItem("&New SR...", true),
                                   new ExpectedTextMenuItem("&Import...", true),
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("&High Availability...", true)
                               };
            expected.Add(new ExpectedTextMenuItem("Manage &vApps...", true));
            expected.Add(new ExpectedTextMenuItem("Di&saster Recovery", true, new ExpectedMenuItem[]
                                                                                    {
                                                                                        new ExpectedTextMenuItem(
                                                                                            "&Configure...", true),
                                                                                        new ExpectedTextMenuItem(
                                                                                            "&Disaster Recovery Wizard...",
                                                                                            true)
                                                                                    }));
            expected.AddRange(new List<ExpectedMenuItem>{
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("&Add Server", true, false,
                                                            new ExpectedMenuItem[]
                                                                {
                                                                    new ExpectedTextMenuItem("&Add New Server...", true)
                                                                }),
                                   new ExpectedTextMenuItem("&Disconnect", true),
                                   new ExpectedTextMenuItem("Reconnec&t As...", true),
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("E&xpand All", true),
                                   new ExpectedTextMenuItem("P&roperties", true)
                               });
            VerifyContextMenu(pool, expected.ToArray());
        }

        [Test]
        public void ContextMenu_Master()
        {
            VerifyContextMenu(GetAnyHost(IsMaster), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("New V&M...", true),
                new ExpectedTextMenuItem("&New SR...", true),
                new ExpectedTextMenuItem("&Import...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Enter Maintenance Mode...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Re&boot", true),
                new ExpectedTextMenuItem("S&hut Down", true),
                new ExpectedTextMenuItem("Restart Toolstac&k", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_Slave()
        {
            VerifyContextMenu(GetAnyHost(IsSlave), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("New V&M...", true),
                new ExpectedTextMenuItem("&New SR...", true),
                new ExpectedTextMenuItem("&Import...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Enter Maintenance Mode...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Re&boot", true),
                new ExpectedTextMenuItem("S&hut Down", true),
                new ExpectedTextMenuItem("Restart Toolstac&k", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Remove Server from &Pool", false),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_VMWithTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(HasTools))
            {
                List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                expectedMenuItems.Add(new ExpectedTextMenuItem("S&hut Down", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("S&uspend", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Reb&oot", true));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Shut Do&wn", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Re&boot", true));
                expectedMenuItems.Add(new ExpectedSeparator());

                if (vm.Connection.Resolve<Host>(vm.resident_on).name_label == "inflames")
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("M&igrate to Server", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Current server)", false),
                                new ExpectedTextMenuItem("inflames (Current server)", false, false, true),
                                new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                            }));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                else if (vm.IsOnSharedStorage() == string.Empty)
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("M&igrate to Server", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Current server)", false),
                                new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                new ExpectedTextMenuItem("incubus (Current server)", false, false, true)
                            }));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }


                expectedMenuItems.Add(new ExpectedTextMenuItem("Ta&ke a Snapshot...", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Assign to vA&pp", true, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("P&roperties", true));

                VerifyContextMenu(vm, expectedMenuItems.ToArray());
            }
        }

        [Test]
        public void ContextMenu_VMWithoutTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(NoTools))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("Force Shut Do&wn", true),
                    new ExpectedTextMenuItem("Force Re&boot", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", true),
                    new ExpectedTextMenuItem("Assign to vA&pp", true, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_VMShutdown()
        {
            foreach (VM vm in GetAllXenObjects<VM>(IsShutdown))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("S&tart", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Start on Ser&ver", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Home Server is not set)", false),
                                new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                            }),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Copy VM...", true),
                    new ExpectedTextMenuItem("&Export...", true),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", true),
                    new ExpectedTextMenuItem("Co&nvert to Template...", true),
                    new ExpectedTextMenuItem("Assign to vA&pp", true, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete VM...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);

            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR()))
            {
                List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                if (CanSetAsDefault(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("Set as Defaul&t", true));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                if (CanDetach(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("&Detach...", true));
                }
                if (CanForget(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("&Forget", true));
                }
                if (CanDestroy(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("Destr&oy...", true));
                }
                if (expectedMenuItems.Count > 0 && !(CanSetAsDefault(sr) && expectedMenuItems.Count == 2))
                {
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                expectedMenuItems.Add(new ExpectedTextMenuItem("P&roperties", true));

                VerifyContextMenu(sr, expectedMenuItems.ToArray());
            }
        }

        [Test]
        public void ContextMenu_UserTemplate()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);

            foreach (VM vm in GetAllXenObjects<VM>(v => v.InstantTemplate()))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("&New VM wizard...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", true),
                    new ExpectedTextMenuItem("&Copy", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Properties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_Snapshot()
        {
            try
            {
                var expectedForAll = new List<ExpectedMenuItem>
                                       {
                                           new ExpectedTextMenuItem("&New VM From Snapshot...", true),
                                           new ExpectedTextMenuItem("&Create Template From Snapshot...", true),
                                           new ExpectedTextMenuItem("&Export Snapshot As Template...", true),
                                           new ExpectedTextMenuItem("&Delete Snapshot", true),
                                           new ExpectedSeparator(),
                                           new ExpectedTextMenuItem("P&roperties", true)
                                       };

                PutInNavigationMode(NavigationPane.NavigationMode.Objects);

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    var expected = new List<ExpectedMenuItem>(expectedForAll);
                    VerifyContextMenu(snapshot, expected.ToArray());
                }

                PutInNavigationMode(NavigationPane.NavigationMode.Tags);

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot && v.tags.Length > 0))
                {
                    var expected = new List<ExpectedMenuItem>(expectedForAll);

                    if (snapshot.tags.Length > 0)
                        expected.Insert(5, new ExpectedTextMenuItem("Untag Ob&ject", true));
                    if (Folders.GetFolder(snapshot) != null)
                        expected.Insert(5, new ExpectedTextMenuItem("Remove from &folder", true));
                    VerifyContextMenu(snapshot, expected.ToArray());
                }

                PutInNavigationMode(NavigationPane.NavigationMode.Folders);

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot && Folders.GetFolder(v) != null))
                {
                    var expected = new List<ExpectedMenuItem>(expectedForAll);

                    if (snapshot.tags.Length > 0)
                        expected.Insert(5, new ExpectedTextMenuItem("Untag Ob&ject", true));
                    if (Folders.GetFolder(snapshot) != null)
                        expected.Insert(5, new ExpectedTextMenuItem("Remove from &folder", true));
                    VerifyContextMenu(snapshot, expected.ToArray());
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ContextMenu_VDI()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                var vdis = GetAllXenObjects<VDI>(v => v.name_label != "base copy" && !v.name_label.StartsWith("XenServer Transfer VM") && !v.is_a_snapshot);
                foreach (VDI v in vdis)
                {
                    VerifyContextMenu(v, new ExpectedMenuItem[] {
                        new ExpectedTextMenuItem("&Move Virtual Disk...", true),//can migrate 
                        new ExpectedTextMenuItem("&Delete Virtual Disk", v.allowed_operations.Contains(vdi_operations.destroy)),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("P&roperties", true),
                        
                    });
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ContextMenu_Network()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Host internal management network"))
                {
                    VerifyContextMenu(network, new ExpectedMenuItem[] {
                        new ExpectedTextMenuItem("P&roperties", true)
                    });
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }


        [Test]
        public void ContextMenu_GroupingTag()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Tags);
            try
            {
                var nodes = GetAllTreeNodes().Where(n => n.Parent != null);

                foreach (VirtualTreeNode node in nodes)
                {
                    GroupingTag gt = node.Tag as GroupingTag;
                    if (gt != null)
                    {
                        var expectedItems = new List<ExpectedMenuItem>();

                        if (gt.Grouping is PropertyGrouping<string>)
                        {
                            expectedItems.Add(new ExpectedTextMenuItem("&Delete Tag...", true));
                            expectedItems.Add(new ExpectedTextMenuItem("&Rename Tag...", true));
                        }

                        AddExpectedExpandAndCollapseItems(node, expectedItems);

                        VerifyContextMenu(gt, expectedItems.ToArray());
                    }
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ContextMenu_Folder()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (VirtualTreeNode node in GetAllTreeNodes())
                {
                    Folder folder = node.Tag as Folder;

                    if (folder != null)
                    {
                        List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                        expectedMenuItems.Add(new ExpectedTextMenuItem("&New Folder...", true));

                        if (!folder.IsRootFolder)
                        {
                            expectedMenuItems.Add(new ExpectedTextMenuItem("&Rename Folder...", true));
                            expectedMenuItems.Add(new ExpectedTextMenuItem("&Delete Folder...", true));
                        }

                        AddExpectedExpandAndCollapseItems(node, expectedMenuItems);

                        VerifyContextMenu(folder, expectedMenuItems.ToArray());
                    }
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ContextMenu_DefaultTemplate()
        {
            EnsureDefaultTemplatesShown();

            VerifyContextMenu(GetAnyDefaultTemplate(), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&New VM wizard...", true),
                new ExpectedSeparator(),                
                new ExpectedTextMenuItem("&Export to File...", true),
                new ExpectedTextMenuItem("&Copy...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_UserTemplate_Instant()
        {
            EnsureDefaultTemplatesShown();

            foreach (VM vm in GetAllXenObjects<VM>(v => v.InstantTemplate()))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("&New VM wizard...", true),
                    new ExpectedTextMenuItem("&Quick Create", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", true),
                    new ExpectedTextMenuItem("&Copy...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_UserTemplate_NoInstant()
        {
            EnsureDefaultTemplatesShown();

            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate() && !v.is_a_snapshot && !v.InstantTemplate() && !v.name_label.StartsWith("XenServer Transfer VM")))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("&New VM wizard...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", true),
                    new ExpectedTextMenuItem("&Copy...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
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
