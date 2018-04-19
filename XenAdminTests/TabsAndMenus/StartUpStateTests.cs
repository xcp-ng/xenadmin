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

using System.Collections;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Commands;
using XenAdmin.Controls.MainWindowControls;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class StartUpStateTests : TabsAndMenus
    {

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            ConnectToStateDBs("state1.xml");
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            DisableAllPlugins();
        }

        [OneTimeTearDown]
        public new void TearDown()
        {
            MW(RemoveStateDBs);
        }

        [Test]
        public void TestTabs()
        {
            VerifyTabs(null, new [] { "Home", "Search" });
        }

        [Test]
        public void TestSaveSessionKeyboardShortcut()
        {
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.NewVmToolStripMenuItem, Keys.N | Keys.Control);
        }

        private void TestKeyboardShortCut(ToolStripMenuItem item, Keys keys)
        {
            MWWaitFor(() => item.ShortcutKeys == keys, "Shortcut for " + item.Text + " was incorrect");
        }

        [Test]
        public void TestKeyboardShortcutsRunningVM()
        {
            // select VM
            SelectInTree(GetAnyVM(v => v.name_label.StartsWith("Iscsi")));

            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ShutDownVMToolStripMenuItem, Keys.E | Keys.Control);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.SuspendToolStripMenuItem, Keys.Y | Keys.Control);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.RestartToolStripMenuItem, Keys.R | Keys.Control);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.NewVmToolStripMenuItem, Keys.N | Keys.Control);

            // go to console tab
            GoToTabPage(Program.MainWindow.TabPageConsole);

            // check shortcut keys removed.
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ShutDownVMToolStripMenuItem, Keys.None);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.SuspendToolStripMenuItem, Keys.None);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.RestartToolStripMenuItem, Keys.None);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.NewVmToolStripMenuItem, Keys.None);
        }

        [Test]
        public void TestKeyboardShortcutsHaltedVM()
        {
            // select VM
            SelectInTree(GetAnyVM(v => v.name_label.StartsWith("Windows Server 2008 (1)")));

            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.StartToolStripMenuItem, Keys.B | Keys.Control);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.NewVmToolStripMenuItem, Keys.N | Keys.Control);

            // go to console tab
            GoToTabPage(Program.MainWindow.TabPageConsole);

            // check shortcut keys removed.
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.StartToolStripMenuItem, Keys.None);
            TestKeyboardShortCut(MainWindowWrapper.VMMenuItems.NewVmToolStripMenuItem, Keys.None);
        }

        [Test]
        public void TestMainMenuFontCorrect()
        {
            MW(() => TestMenuFontCorrect(MainWindowWrapper.MainMenuBar.Items));
        }

        [Test]
        public void TestContextMenuFontCorrect()
        {
            ContextMenuBuilder builder = new ContextMenuBuilder(MainWindowWrapper.PluginManager, Program.MainWindow);
            MW(() => TestMenuFontCorrect(builder.Build(GetAnyVM())));
        }

        private void TestMenuFontCorrect(IEnumerable toolStripItems)
        {
            foreach (ToolStripItem item in toolStripItems)
            {
                if (item is ToolStripSeparator)
                    continue;

                Assert.AreEqual(Program.DefaultFont, item.Font, "Incorrect font for menu item:" + item.Text);

                ToolStripMenuItem menuItem = item as ToolStripMenuItem;

                if (menuItem != null && menuItem.Enabled)
                {
                    menuItem.ShowDropDown();
                    TestMenuFontCorrect(menuItem.DropDownItems);
                }
            }
        }
    }
}
