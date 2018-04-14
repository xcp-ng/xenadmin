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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Plugins;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class PluginTabsAndMenus : TabsAndMenus
    {
        private List<string> _folders = new List<string>();

        private readonly string[] XenCenterTabs = new[] {"Home", "XenCenterTabPageTest", "Search"};
        private readonly string[] PoolTabs = new[] { "General", "Memory", "Storage", "Networking", "HA", "WLB", "Users", "PoolTabPageTest", "AllTabPageTest", "Search" };

        private readonly string[] HostTabs = new[] { "General", "Memory", "Storage", "Networking", "NICs", "Console", "Performance", "Users", "ServerTabPageTest", "AllTabPageTest", "Search" };
        private readonly string[] VMTabs = new[] { "General", "Memory", "Storage", "Networking", "Console", "Performance", "Snapshots", "VMTabPageTest", "AllTabPageTest", "Search" };
        private readonly string[] DefaultTemplateTabs = new[] { "General", "Memory", "Networking", "DefaultTemplateTabPageTest", "AllTabPageTest", "Search" };
        private readonly string[] UserTemplateTabs = new[] { "General", "Memory", "Storage", "Networking", "UserTemplateTabPageTest", "AllTabPageTest", "Search" };
        private readonly string[] SRTabs = new[] { "General", "Storage", "SRTabPageTest", "AllTabPageTest", "Search" };

        public PluginTabsAndMenus()
            : base("state4.xml")
        {
            CheckHelp = false;
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            string plugins = Path.Combine(Program.AssemblyDir, "Plugins");
            string vendorDir = "plugin-vendor";
            string[] names = new string[] { "MenuItemFeatureTestPlugin", "ParentMenuItemFeatureTestPlugin", "TabPageFeatureTestPlugin" };

            foreach (string name in names)
            {
                string folder = plugins + "\\" + vendorDir + "\\" + name;
                string file = folder + "\\" + name + ".xcplugin.xml";

                if (!Directory.Exists(folder))
                {
                    _folders.Add(folder);
                    Directory.CreateDirectory(folder);
                }

                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream("XenAdminTests.TestResources.PluginResources." + name + ".xcplugin.xml");

                using (StreamReader sr = new StreamReader(stream))
                {
                    File.WriteAllText(file, sr.ReadToEnd());
                }
            }

            MW(delegate
            {
                MainWindowWrapper.PluginManager.ReloadPlugins();
            });

            EnableAllPlugins();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            foreach (string folder in _folders)
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
            MW(delegate
            {
                MainWindowWrapper.PluginManager.ReloadPlugins();
            });
        }

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
            EnsureDefaultTemplatesShown();
            VerifyTabs(GetAnyDefaultTemplate(), DefaultTemplateTabs);
        }

        [Test]
        public void Tabs_UserTemplate()
        {
            EnsureDefaultTemplatesShown();
            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate() && !v.is_a_snapshot))
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
        public void TestMenuItems()
        {
            MW(delegate
            {
                MainWindowWrapper.FileMenu.ShowDropDown();
                Assert.AreEqual("the label", GetVisibleToolStripItems(MainWindowWrapper.FileMenu.DropDownItems)[6].Text);
                Assert.AreEqual("groupTest", GetVisibleToolStripItems(MainWindowWrapper.FileMenu.DropDownItems)[7].Text);

                MainWindowWrapper.ViewMenu.ShowDropDown();
                Assert.AreEqual("view_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.ViewMenu.DropDownItems)[5].Text);

                MainWindowWrapper.PoolMenu.ShowDropDown();
                Assert.AreEqual("pool_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.PoolMenu.DropDownItems)[19].Text);

                MainWindowWrapper.HostMenu.ShowDropDown();
                Assert.AreEqual("server_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.HostMenu.DropDownItems)[21].Text);

                MainWindowWrapper.VMMenu.ShowDropDown();
                Assert.AreEqual("vm_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.VMMenu.DropDownItems)[19].Text);

                MainWindowWrapper.TemplatesMenu.ShowDropDown();
                Assert.AreEqual("templates_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.TemplatesMenu.DropDownItems)[7].Text);

                MainWindowWrapper.ToolsMenu.ShowDropDown();
                Assert.AreEqual("tools_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.ToolsMenu.DropDownItems)[8].Text);

                MainWindowWrapper.HelpMenu.ShowDropDown();
                Assert.AreEqual("help_ShellTest1", GetVisibleToolStripItems(MainWindowWrapper.HelpMenu.DropDownItems)[8].Text);
            });
        }

        [Test]
        public void ClickAllNodes()
        {
            MW(delegate
            {
                foreach (VirtualTreeNode node in new List<VirtualTreeNode>(MainWindowWrapper.TreeView.AllNodes))
                {
                    node.EnsureVisible();
                    node.TreeView.SelectedNode = node;
                    Application.DoEvents();

                    foreach (TabPage page in MainWindowWrapper.TheTabControl.TabPages)
                    {
                        if (page.Tag is TabPageFeature)
                        {
                            MainWindowWrapper.TheTabControl.SelectedTab = page;
                            Application.DoEvents();
                        }
                    }
                }
            });
        }
    }
}
