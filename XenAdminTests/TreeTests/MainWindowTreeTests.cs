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
using System.Threading;
using NUnit.Framework;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using System.Windows.Forms;
using System.Reflection;
using XenAdmin;
using XenAdmin.XenSearch;

namespace XenAdminTests.TreeTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class MainWindowTreeTests : MainWindowLauncher_TestFixture
    {
        public MainWindowTreeTests()
            : base("state1.xml")
        { }

        /// <summary>
        /// Checks that all nodes matched by the specified Predicate are expanded and the rest are collapsed. Returns true if this 
        /// is the case.
        /// </summary>
        private bool CheckExpandedNodes(Predicate<VirtualTreeNode> match)
        {
            return CheckExpandedNodes(match, null);
        }

        /// <summary>
        /// Asserts that all nodes matched by the specified Predicate are expanded and the rest are collapsed.
        /// </summary>
        private bool CheckExpandedNodes(Predicate<VirtualTreeNode> match, string assertMessage)
        {
            return MWWaitFor(delegate
            {
                foreach (VirtualTreeNode n in MainWindowWrapper.TreeView.AllNodes)
                {
                    if (match(n) && n.Nodes.Count > 0)
                    {
                        if (!n.IsExpanded)
                        {
                            return false;
                        }
                    }
                    else if (n.IsExpanded)
                    {
                        return false;
                    }
                }
                return true;
            }, assertMessage);
        }

        private void SetExpandedNodes(Predicate<VirtualTreeNode> match)
        {
            MW(delegate
            {
                foreach (VirtualTreeNode n in MainWindowWrapper.TreeView.AllNodes)
                {
                    if (match(n))
                    {
                        n.Expand();
                    }
                    else
                    {
                        n.Collapse();
                    }
                }
            });
        }

        private void ClearTreeSelections()
        {
            MW(() => MainWindowWrapper.TreeView.SelectedNodes.SetContents(new List<VirtualTreeNode>()));
        }

        [Test]
        public void TestPersistenceBetweenServerAndOrgView()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            ClearTreeSelections();

            // collapse all nodes in server view.
            MW(MainWindowWrapper.TreeView.CollapseAll);

            // wait for all nodes to become collapsed.
            CheckExpandedNodes(n => false, "Couldn't collapse all nodes.");

            // expand root node and pool node.
            MW(() =>
            {
                VirtualTreeNode poolNode = MainWindowWrapper.TreeView.Nodes[0].Nodes[0];
                Assert.AreEqual("Hottub", poolNode.Text);
                MainWindowWrapper.TreeView.Nodes[0].Expand();
                poolNode.Expand();
            });

            // wait for those nodes to become expanded
            CheckExpandedNodes(n => n == MainWindowWrapper.TreeView.Nodes[0].Nodes[0] || n.Tag == null, "Could expand nodes.");

            // expand host nodes in server view.
            MW(() =>
            {
                VirtualTreeNode poolNode = MainWindowWrapper.TreeView.Nodes[0].Nodes[0];
                poolNode.Nodes[0].Expand();
                poolNode.Nodes[1].Expand();
            });

            // check hosts nodes got expanded.
            CheckExpandedNodes(n =>
            {
                VirtualTreeNode poolNode = MainWindowWrapper.TreeView.Nodes[0].Nodes[0];
                return n.Tag == null || n == poolNode || n == poolNode.Nodes[0] || n == poolNode.Nodes[1];
            }, "Couldn't expand host nodes.");

            // now go into objects view.
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);

            // collapse all nodes
            MW(MainWindowWrapper.TreeView.CollapseAll);

            // wait for all nodes to become collapsed.
            CheckExpandedNodes(n => false, "Couldn't collapse all nodes.");

            // expand root node and snapshots nodes in folder view.
            MW(() =>
            {
                MainWindowWrapper.TreeView.Nodes[0].Expand();
                MainWindowWrapper.TreeView.Nodes[0].Nodes[3].Expand();
                VirtualTreeNode typesNode = MainWindowWrapper.TreeView.Nodes[0].Nodes[3];
                Assert.AreEqual("Snapshots", typesNode.Text);
                typesNode.Expand();
            });

            // wait for those nodes to become expanded.
            CheckExpandedNodes(n => n == MainWindowWrapper.TreeView.Nodes[0]
                || n == MainWindowWrapper.TreeView.Nodes[0].Nodes[3], "Couldn't expand nodes.");

            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);

            if (!CheckExpandedNodes(n => n.Tag == null || n.Tag is Pool || n.Tag is Host))
            {
                var expandedNodes = MW(() => GetAllTreeNodes().FindAll(n => n.IsExpanded).ConvertAll(n => n.Text));
                Assert.Fail("Nodes not correctly persisted in infrastructure view. Expanded nodes were: " + string.Join(", ", expandedNodes.ToArray()) + ". They should have only been XenCenter, Hottub, inflames, incubus.");
            }

            PutInNavigationMode(NavigationPane.NavigationMode.Objects);

            if (!CheckExpandedNodes(n => n.Parent == null || n.Text == "Objects by Type" || n.Text == "Snapshots"))
            {
                var expandedNodes = MW(() => GetAllTreeNodes().FindAll(n => n.IsExpanded).ConvertAll(n => n.Text));
                Assert.Fail("Nodes not correctly persisted in organization view. Expanded nodes were: " + string.Join(", ", expandedNodes.ToArray()) + ". They should have only been Objects, Types.");
             }
        }

        [Test]
        public void TestMigratedVMStaysSelectedAndBecomesVisible()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            ClearTreeSelections();

            // this VM is at pool level.
            VM vm = GetAnyVM(v => v.name_label == "Windows Server 2008 (1)");

            SelectInTree(vm);

            MWWaitFor(() => MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.Enabled, "Start button didn't become enabled.");

            // start the VM
            MW(MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.PerformClick);

            // wait for the VM which was at the pool level to become running on one of the hosts.
            MWWaitFor(() =>
            {
                VirtualTreeNode n = FindInTree(vm);
                return n.Parent.Tag is Host && n.IsSelected && n.Parent.IsExpanded && ((TreeNode)n).TreeView != null;
            }, "Node didn't get migrated correctly.");
        }

        [Test]
        [Ignore("")]
        public void TestPersistenceWhenAddingThenRemovingTextSearch()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            ClearTreeSelections();
            Thread.Sleep(1000);

            // expand root, pool and host nodes.
            SetExpandedNodes(n => n.Tag is Host || n.Tag is Pool || n.Tag == null);
            Thread.Sleep(1000);

            // apply a random search
            ApplyTreeSearch("DVD");
            Thread.Sleep(1000);

            // test that all the nodes were expanded for the search.
            CheckExpandedNodes(n => true, "Not all nodes were expanded for search");
            Thread.Sleep(1000);

            // now remove the search
            ApplyTreeSearch(string.Empty);
            Thread.Sleep(1000);

            // now check the original nodes expansion state has been restored.
            CheckExpandedNodes(n => n.Tag == null || n.Tag is Pool || n.Tag is Host, "Nodes weren't correctly persisted");
        }

        [Test]
        public void TestPersistenceWhenAddingThenRemovingSavedSearch()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            ClearTreeSelections();

            // expand root and pool nodes.
            SetExpandedNodes(n => n.Tag is Pool || n.Tag == null);

            // apply a random saved search
            PutInNavigationMode(NavigationPane.NavigationMode.SavedSearch);

            // test that all the nodes were expanded for the saved search.
            CheckExpandedNodes(n => true, "Not all nodes were expanded for search");

            // now remove the saved search by going back into server
            PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);

            // now check the original nodes expansion state has been restored.
            CheckExpandedNodes(n => n.Tag == null || n.Tag is Pool, "Nodes weren't correctly persisted");
        }

        private short GetTreeUpdateCount()
        {
            return (short)typeof(Control).GetField("updateCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(MainWindowWrapper.TreeView);
        }

        [Test]
        public void TestBeginUpdateOnlyCalledIfNecessary()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            ClearTreeSelections();

            MW(delegate
            {
                MainWindowTreeBuilder builder = new MainWindowTreeBuilder(MainWindowWrapper.TreeView);
                VirtualTreeNode newRoot = builder.CreateNewRootNode(TreeSearch.DefaultTreeSearch, NavigationPane.NavigationMode.Objects);
                MainWindowWrapper.TreeView.UpdateRootNodes(new [] { newRoot });  // update once to set all the node names properly
                MainWindowWrapper.TreeView.EndUpdate();

                short updateCount = GetTreeUpdateCount();

                MainWindowWrapper.TreeView.UpdateRootNodes(new[] { newRoot });  // update again: nothing should change this time

                short updateCount2 = GetTreeUpdateCount();

                Assert.AreEqual(updateCount, updateCount2, "BeginUpdate shouldn't have been called.");

                // this time there is a different node, so an update should occur
                newRoot = builder.CreateNewRootNode(TreeSearch.DefaultTreeSearch, NavigationPane.NavigationMode.Objects);
                newRoot.Nodes[0].Text = "bla";

                MainWindowWrapper.TreeView.UpdateRootNodes(new[] { newRoot });

                short updateCount3 = GetTreeUpdateCount();

                Assert.AreEqual(updateCount2 + 1, updateCount3, "BeginUpdate should have been called exactly once");

                MainWindowWrapper.TreeView.EndUpdate();

                Assert.AreEqual(0, GetTreeUpdateCount(), "FlickFreeTreeView didn't pass EndUpdate down to TreeView.");
            });
        }
    }
}
