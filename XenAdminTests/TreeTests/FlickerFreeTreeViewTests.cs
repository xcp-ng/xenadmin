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
using NUnit.Framework;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAdminTests.TreeTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class FlickerFreeTreeViewTests
    {
        private FlickerFreeTreeView _tv;

        [SetUp]
        public void Setup()
        {
            _tv = new FlickerFreeTreeView();

            // ensure handle is created. Some tests fail if you don't do this.
            Assert.NotNull(_tv.Handle);
        }

        [TearDown]
        public void TearDown()
        {
           if (_tv != null)
               _tv.Dispose();
        }

        [Test]
        [Description("Tests that the parent node of the selected nodes get selected when the selected nodes' tags disappear.")]
        public void TestParentNodeSelectedWhenSelectedNodeTagsChange()
        {
            // populate the tree-view with some folders

            Folder foldersTag = new Folder(null, "Folders");

            _tv.Nodes.Add(new VirtualTreeNode(BrandManager.BRAND_CONSOLE));

            VirtualTreeNode folders = new VirtualTreeNode("Folders") { Tag = foldersTag };

            _tv.Nodes[0].Nodes.Add(folders);

            folders.Nodes.Add(new VirtualTreeNode("folder1") { Tag = new Folder(null, "folder1") });
            folders.Nodes.Add(new VirtualTreeNode("folder2") { Tag = new Folder(null, "folder2") });

            // set the 2 folders to be the selected nodes.
            _tv.SelectedNodes.SetContents(new [] { folders.Nodes[0], folders.Nodes[1] });

            // now build up a new nodes tree
            VirtualTreeNode newRootNode = new VirtualTreeNode(BrandManager.BRAND_CONSOLE);

            VirtualTreeNode newFolders = new VirtualTreeNode("Folders") { Tag = foldersTag };

            newRootNode.Nodes.Add(newFolders);

            newFolders.Nodes.Add(new VirtualTreeNode("folder1a") { Tag = new Folder(null, "folder1a") });
            newFolders.Nodes.Add(new VirtualTreeNode("folder2a") { Tag = new Folder(null, "folder2a") });

            int count = 0;
            EventHandler handler = delegate
            {
                count++;
                Assert.AreEqual(1, _tv.SelectedNodes.Count, "There should only be one node selected.");
                Assert.AreEqual("Folders", _tv.SelectedNodes[0].Text, "The Folders node should be selected.");
            };

            _tv.SelectionsChanged += handler;

            try
            {
                // merge the new node tree in.
                _tv.BeginUpdate();
                _tv.UpdateRootNodes(new [] { newRootNode });
                _tv.EndUpdate();
                Assert.GreaterOrEqual(count, 1, "SelectionsChanged should fire.");
            }
            finally
            {
                _tv.SelectionsChanged -= handler;
            }
        }

        [Test]
        public void TestNodeStaysSelected()
        {
            VirtualTreeNode root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();
            _tv.SelectedNode = _tv.Nodes[0].Nodes[0];

            root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();

            Assert.AreEqual("1", _tv.SelectedNode.Text);
        }

        [Test]
        public void TestNodeStaysSelectedAsItMoves()
        {
            VirtualTreeNode root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();
            _tv.SelectedNode = _tv.Nodes[0].Nodes[0];

            root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();

            Assert.AreEqual(1, _tv.SelectedNodes.Count, "Node didn't stay selected");
            Assert.AreEqual("1", _tv.SelectedNode.Text, "Node didn't stay selected");
            Assert.AreEqual(new Folder(null, "1"), _tv.SelectedNode.Tag, "Node didn't stay selected");
        }

        [Test]
        public void TestMultipleNodesStaySelectedAsTheyMove()
        {
            VirtualTreeNode root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });
            root.Nodes.Add(new VirtualTreeNode("3") { Tag = new Folder(null, "3") });
            root.Nodes.Add(new VirtualTreeNode("4") { Tag = new Folder(null, "4") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();
            _tv.SelectedNodes.SetContents(new [] { root.Nodes[0], root.Nodes[1] });

            root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("4") { Tag = new Folder(null, "4") });
            root.Nodes.Add(new VirtualTreeNode("3") { Tag = new Folder(null, "3") });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") });
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();

            Assert.AreEqual(2, _tv.SelectedNodes.Count, "Nodes didn't stay selected");
            Assert.AreEqual("1", _tv.SelectedNodes[0].Text, "Nodes didn't stay selected");
            Assert.AreEqual("2", _tv.SelectedNodes[1].Text, "Nodes didn't stay selected");
            Assert.AreEqual(new Folder(null, "1"), _tv.SelectedNodes[0].Tag, "Nodes didn't stay selected");
            Assert.AreEqual(new Folder(null, "2"), _tv.SelectedNodes[1].Tag, "Nodes didn't stay selected");
        }

        [Test]
        public void TestSingleParentNodeGetsSelectedWhenMultipleNodesDisappear()
        {
            IXenConnection con1 = new XenConnection();
            IXenConnection con2 = new XenConnection();
            IXenConnection con3 = new XenConnection();

            VirtualTreeNode root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("1") { Tag = new Folder(null, "1") { Connection = con1 } });
            root.Nodes.Add(new VirtualTreeNode("2") { Tag = new Folder(null, "2") { Connection = con2 } });
            root.Nodes.Add(new VirtualTreeNode("3") { Tag = new Folder(null, "3") { Connection = con3 } });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();
            _tv.SelectedNodes.SetContents(new [] { root.Nodes[0], root.Nodes[1] });

            root = new VirtualTreeNode("root");
            root.Nodes.Add(new VirtualTreeNode("3") { Tag = new Folder(null, "3") { Connection = con3 } });

            _tv.BeginUpdate();
            _tv.UpdateRootNodes(new [] { root });
            _tv.EndUpdate();

            Assert.AreEqual(1, _tv.SelectedNodes.Count, "Nodes didn't stay selected");
            Assert.AreEqual("root", _tv.SelectedNode.Text, "Nodes didn't stay selected");
        }
    }
}
