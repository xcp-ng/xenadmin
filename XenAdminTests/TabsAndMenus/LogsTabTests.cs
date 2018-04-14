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
using System.Windows.Forms;

using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Core;
using XenAdmin.TabPages;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    public class LogsTabTests : MainWindowLauncher_TestFixture
    {
        private List<HistoryPage.DataGridViewActionRow> GetVisibleRows()
        {
            var rows = new List<HistoryPage.DataGridViewActionRow>();
            MW(() =>
                {
                    var gridView = TestUtils.GetDataGridView(MainWindowWrapper.Item, "eventsPage.dataGridView");
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        var actionRow = row as HistoryPage.DataGridViewActionRow;
                        if (actionRow != null)
                            rows.Add(actionRow);
                    }
                });
            return rows;
        }

        private List<HistoryPage.DataGridViewActionRow> GetUnfinishedRows()
        {
            var rows = new List<HistoryPage.DataGridViewActionRow>();
            MW(() =>
                {
                    var gridView = TestUtils.GetDataGridView(MainWindowWrapper.Item, "eventsPage.dataGridView");

                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        var actionRow = row as HistoryPage.DataGridViewActionRow;
                        if (actionRow != null && !actionRow.Action.IsCompleted)
                            rows.Add(actionRow);
                    }
                });
            return rows;
        }

        [TearDown]
        public new void TearDown()
        {
            RemoveStateDBs();
            //Events tab
            ConnectionsManager.History.Clear();
        }

        [SetUp]
        public void Setup()
        {
            ConnectToStateDBs("state4.xml");
            MW(delegate
            {
                // deselect all the default templates - to speed up tests.
                if (MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem.Checked)
                {
                    MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem.PerformClick();
                }
            });
        }

        [Test]
        [Ignore("")]
        public void TestAfterConnection()
        {
            foreach (VirtualTreeNode n in GetAllTreeNodes())
            {
                SelectInTree(n.Tag);
                MW(n.EnsureVisible);

                //Events tab
                var visibleRows = GetVisibleRows();

                Assert.AreEqual(1, visibleRows.Count, "No connection item found.");
                Assert.IsTrue(visibleRows[0].Action.GetDetails().StartsWith("Connected to "),
                    "Connected message not found. The message was: " + visibleRows[0].Action.GetDetails());
            }
        }

        [Test]
        public void TestHistoryItemsAppearForVMShutdown()
        {
            VM vm1 = GetAnyVM(v => v.name_label == "Iscsi Box");
            VM vm2 = GetAnyVM(v => v.name_label == "Windows Server 2003 x64 (1)");

            SelectInTree(vm1, vm2);

            new List<VirtualTreeNode>(MainWindowWrapper.TreeView.SelectedNodes).ForEach(n => MW(n.EnsureVisible));

            //Events tab

            var rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "History page didn't have 1 message before VMs shut down");

            MW(MainWindowWrapper.MainToolStripItems.ShutDownToolStripButton.PerformClick);

            rows = GetVisibleRows();
            Assert.AreEqual(4, rows.Count, "Items weren't added when VMs shut down.");
        }

        [Test]
        public void TestClear()
        {
            SelectInTree(GetAnyPool());
            
            //Events tab
            var rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "No connection item found.");

            // wait for everything to finish.
            rows = GetUnfinishedRows();
            Assert.AreEqual(0, rows.Count, "No connection item found.");

            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item, "eventsPage.tsmiDismissAll").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(0, rows.Count, "Items weren't cleared.");
        }

        [Test]
        public void TestHide()
        {
            SelectInTree(GetAnyPool());
            //Events tab
            var showAllButton = MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                                                                        "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemAll"));

            var rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "No connection item found.");
            Assert.IsFalse(showAllButton.Enabled);
            MW(() =>
                {
                    TestUtils.GetToolStripItem(MainWindowWrapper.Item,
                        "navigationPane.buttonNotifySmall").PerformClick();
                    TestUtils.GetNotificationsView(MainWindowWrapper.Item,
                        "navigationPane.notificationsView").SelectNotificationsSubMode(NotificationsSubMode.Events);
                });

            // this should clear all items as they are all completed.
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                        "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemComplete").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(0, rows.Count, "Items weren't cleared.");
            Assert.IsTrue(showAllButton.Enabled);

            // this should bring them back.
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemComplete").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "Items were cleared.");
            Assert.IsFalse(showAllButton.Enabled);

            // nothing should change for cancelled items
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemCancelled").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "Items were cleared.");
            Assert.IsTrue(showAllButton.Enabled);

            // nothing should change for failed items
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemError").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "Items were cleared.");
            Assert.IsTrue(showAllButton.Enabled);

            // nothing should change for incomplete items
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemInProgress").PerformClick());
            rows = GetVisibleRows();
            Assert.AreEqual(1, rows.Count, "Items were cleared.");
            Assert.IsTrue(showAllButton.Enabled);

            // put all these back
            MW(() => TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item,
                "eventsPage.toolStripDdbFilterStatus.toolStripMenuItemAll").PerformClick());
            Assert.IsFalse(showAllButton.Enabled);
        }
    }
}
