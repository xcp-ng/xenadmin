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

using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Controls;
using System.Drawing;

namespace XenAdminTests.TreeTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class VirtualTreeViewTests
    {
        private VirtualTreeView _vtv;
        private TreeView _tv;

        private static void AssertNodeIsDummy(TreeNode node)
        {
            Assert.IsFalse(node is VirtualTreeNode);
            Assert.AreEqual("DummyTreeNode", node.GetType().Name, "node isn't DummyTreeNode");
        }

        [SetUp]
        public void Setup()
        {
            _vtv = new VirtualTreeView();
            _tv = _vtv;

            // ensure handle is created. Some tests fail if you don't do this.
            Assert.NotNull(_vtv.Handle);
        }

        [TearDown]
        public void TearDown()
        {
            if (_vtv != null)
                _vtv.Dispose();
        }

        [Test]
        public void TestAddRootNode()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("rootNode"));

            Assert.AreEqual(1, _tv.Nodes.Count);
            Assert.AreEqual("rootNode", _tv.Nodes[0].Text);
        }

        [Test]
        public void TestAddChildNodeWhenRootCollapsed()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            _vtv.Nodes[0].Collapse();

            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));

            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);
            AssertNodeIsDummy(_tv.Nodes[0].Nodes[0]);

            // now expand the rootnode, and the child should be there.

            _vtv.Nodes[0].Expand();

            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);
            Assert.AreEqual("child", _tv.Nodes[0].Nodes[0].Text);
        }

        [Test]
        public void TestAddChildNodesWithRootCollapsed()
        {
            // add root node
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            // ensure root node collapsed
            _vtv.Nodes[0].Collapse();

            // now add some child nodes.
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("firstChild"));
            _vtv.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("secondChild"));

            // now expand the rootnode, and the first child should be there but the second child shouldn't
            _vtv.Nodes[0].Expand();

            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);
            Assert.AreEqual("firstChild", _tv.Nodes[0].Nodes[0].Text);

            Assert.AreEqual(1, _tv.Nodes[0].Nodes[0].Nodes.Count);
            AssertNodeIsDummy(_tv.Nodes[0].Nodes[0].Nodes[0]);

            // now expand firstChild to make secondChild visible
            _tv.Nodes[0].Nodes[0].Expand();

            Assert.AreEqual(1, _tv.Nodes[0].Nodes[0].Nodes.Count);
            Assert.AreEqual("secondChild", _tv.Nodes[0].Nodes[0].Nodes[0].Text);
        }

        [Test]
        public void TestAddChildNodeWhenRootExpanded()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));
            _vtv.Nodes[0].Expand();

            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child2"));

            Assert.AreEqual(2, _tv.Nodes[0].Nodes.Count);
            Assert.AreEqual("child", _tv.Nodes[0].Nodes[0].Text);
            Assert.AreEqual("child2", _tv.Nodes[0].Nodes[1].Text);
        }

        [Test]
        public void TestAddChildNodesWhenRootExpanded()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            // now add some child node and expand.
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));
            _vtv.Nodes[0].Expand();

            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child2"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child3"));

            Assert.AreEqual(3, _tv.Nodes[0].Nodes.Count);
            Assert.AreEqual("child", _tv.Nodes[0].Nodes[0].Text);
            Assert.AreEqual("child2", _tv.Nodes[0].Nodes[1].Text);
            Assert.AreEqual("child3", _tv.Nodes[0].Nodes[2].Text);
        }

        [Test]
        public void TestAddSubtreeWithRootCollapsed()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            // build subtree
            VirtualTreeNode subTree = new VirtualTreeNode("firstChild");
            subTree.Nodes.Add(new VirtualTreeNode("secondChild"));
            subTree.Nodes[0].Nodes.Add(new VirtualTreeNode("thirdChild"));

            // only expand secondChild
            subTree.Nodes[0].Expand();

            // now add subtree
            _vtv.Nodes[0].Nodes.Add(subTree);

            // root node should only have dummy
            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);
            AssertNodeIsDummy(_tv.Nodes[0].Nodes[0]);

            // in the virtual collections, the expanded states should still be correct
            Assert.IsFalse(subTree.IsExpanded);
            Assert.IsTrue(subTree.Nodes[0].IsExpanded);
            Assert.IsFalse(subTree.Nodes[0].Nodes[0].IsExpanded);

            // now expand the root node. only the first child should be visible
            _vtv.Nodes[0].Expand();
            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);
            Assert.AreEqual("firstChild", _tv.Nodes[0].Nodes[0].Text);
            Assert.AreEqual(1, _tv.Nodes[0].Nodes[0].Nodes.Count);
            AssertNodeIsDummy(_tv.Nodes[0].Nodes[0].Nodes[0]);

            // now expand the first child, this should result in the secondChild AND thirdChild becoming visible (as secondChild was expanded before it was added to the treeview)
            _vtv.Nodes[0].Nodes[0].Expand();
            Assert.AreEqual(1, _tv.Nodes[0].Nodes[0].Nodes.Count);
            Assert.AreEqual("secondChild", _tv.Nodes[0].Nodes[0].Nodes[0].Text);
            Assert.AreEqual(1, _tv.Nodes[0].Nodes[0].Nodes[0].Nodes.Count);
            Assert.AreEqual("thirdChild", _tv.Nodes[0].Nodes[0].Nodes[0].Nodes[0].Text);
        }

        [Test]
        public void TestSelectedSiblingWhichIsntVisible()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            _vtv.Nodes[0].Nodes.Insert(0, new VirtualTreeNode("firstChild"));
            _vtv.Nodes[0].Nodes.Insert(1, new VirtualTreeNode("secondChild"));

            _vtv.SelectedNode = _vtv.Nodes[0].Nodes[1];
            Assert.AreEqual(_vtv.SelectedNode, _vtv.Nodes[0].Nodes[1], "Sibling was not correctly selected.");
        }

        [Test]
        public void TestSelectedSiblingWhichIsntVisible2()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            _vtv.Nodes[0].Nodes.Insert(0, new VirtualTreeNode("firstChild"));
            _vtv.Nodes[0].Nodes.Insert(1, new VirtualTreeNode("secondChild"));

            _vtv.SelectedNode = _vtv.Nodes[0].Nodes[0];

            Assert.AreEqual(_vtv.SelectedNode, _vtv.Nodes[0].Nodes[0], "Sibling was not correctly selected.");
        }

        [Test]
        public void RemoveAndReAddTest()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));

            _vtv.Nodes[0].Nodes.Insert(0, new VirtualTreeNode("firstChild"));

            VirtualTreeNode secondChild = new VirtualTreeNode("secondChild");
            _vtv.Nodes[0].Nodes.Insert(1, secondChild);

            // expand and then collapse root node
            _vtv.Nodes[0].Expand();
            _vtv.Nodes[0].Collapse();

            // now remove secondChild
            _vtv.Nodes[0].Nodes.RemoveAt(1);

            // now re-add it
            _vtv.Nodes.Add(secondChild);

            Assert.AreEqual(1, _vtv.Nodes[0].Nodes.Count, "node wasn't removed.");
            Assert.AreEqual(secondChild, _vtv.Nodes[1], "node wasn't re-added");
        }

        [Test]
        public void TestSimpleRootNodeUpdateWhenCollapsed()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));

            VirtualTreeNode newRoot = new VirtualTreeNode("newRoot");
            newRoot.Nodes.Add(new VirtualTreeNode("newChild"));

            _vtv.UpdateRootNodes(new [] { newRoot });

            Assert.AreEqual(1, _vtv.Nodes.Count, "Virtual nodes weren't correctly updated.");
            Assert.AreEqual(1, _vtv.Nodes[0].Nodes.Count, "Virtual nodes weren't correctly updated.");
            Assert.AreEqual("newRoot", _vtv.Nodes[0].Text, "Virtual nodes weren't correctly updated.");
            Assert.AreEqual("newChild", _vtv.Nodes[0].Nodes[0].Text, "Virtual nodes weren't correctly updated.");

            Assert.AreEqual(1, _tv.Nodes.Count, "Nodes weren't correctly updated.");
            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count, "Nodes weren't correctly updated.");
            Assert.AreEqual("newRoot", _tv.Nodes[0].Text, "Nodes weren't correctly updated.");
            AssertNodeIsDummy(_tv.Nodes[0].Nodes[0]);
        }

        [Test]
        public void TestPropertiesOnSimpleRootNodeUpdate()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));

            VirtualTreeNode newRoot = new VirtualTreeNode("newRoot");

            //newRoot.ImageIndexFetcher = delegate { return 1; };
            newRoot.ImageIndex = 3;
            newRoot.SelectedImageIndex = 4;
            newRoot.Text = "hello";
            newRoot.NodeFont = SystemFonts.CaptionFont;
            newRoot.BackColor = Color.Beige;
            newRoot.ForeColor = Color.Bisque;

            _vtv.UpdateRootNodes(new [] { newRoot });

            //Assert.IsNotNull(_vtv.Nodes[0].ImageIndexFetcher, "ImageIndexFetcher");
            Assert.AreEqual(3, _vtv.Nodes[0].ImageIndex);
            Assert.AreEqual(4, _vtv.Nodes[0].SelectedImageIndex);
            Assert.AreEqual("hello", _vtv.Nodes[0].Text);
            Assert.AreEqual(SystemFonts.CaptionFont, _vtv.Nodes[0].NodeFont);
            Assert.AreEqual(Color.Beige, _vtv.Nodes[0].BackColor);
            Assert.AreEqual(Color.Bisque, _vtv.Nodes[0].ForeColor);

            Assert.AreEqual(3, _tv.Nodes[0].ImageIndex);
            Assert.AreEqual(4, _tv.Nodes[0].SelectedImageIndex);
            Assert.AreEqual("hello", _tv.Nodes[0].Text);
            Assert.AreEqual(SystemFonts.CaptionFont, _tv.Nodes[0].NodeFont);
            Assert.AreEqual(Color.Beige, _tv.Nodes[0].BackColor);
            Assert.AreEqual(Color.Bisque, _tv.Nodes[0].ForeColor);
        }

        [Test]
        public void TestNodeDeleteOnRootNodeUpdate()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));
            _vtv.Nodes.Add(new VirtualTreeNode("root2"));
            _vtv.Nodes.Add(new VirtualTreeNode("root3"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child2"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child3"));
            _vtv.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild"));
            _vtv.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild2"));
            _vtv.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild3"));

            _vtv.Nodes[0].Expand();
            _vtv.Nodes[1].Expand();
            _vtv.Nodes[2].Expand();

            _vtv.Nodes[0].Nodes[0].Expand();

            VirtualTreeNode newRoot = new VirtualTreeNode("newRoot");
            newRoot.Nodes.Add(new VirtualTreeNode("newChild"));
            newRoot.Nodes[0].Nodes.Add(new VirtualTreeNode("newGrandChild"));

            _vtv.UpdateRootNodes(new [] { newRoot });

            Assert.AreEqual(1, _vtv.Nodes.Count);
            Assert.AreEqual(1, _vtv.Nodes[0].Nodes.Count);
            Assert.AreEqual(1, _vtv.Nodes[0].Nodes[0].Nodes.Count);

            Assert.AreEqual(1, _tv.Nodes.Count);
            Assert.AreEqual(1, _tv.Nodes[0].Nodes.Count);

            Assert.AreEqual("newRoot", _tv.Nodes[0].Text);
            Assert.AreEqual("newChild", _tv.Nodes[0].Nodes[0].Text);
            Assert.AreEqual("newGrandChild", _tv.Nodes[0].Nodes[0].Nodes[0].Text);
        }

        [Test]
        public void TestChildNodesRemovedWhenUpdatingParentNodes()
        {
            _vtv.Nodes.Add(new VirtualTreeNode("root"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child2"));
            _vtv.Nodes[0].Nodes.Add(new VirtualTreeNode("child3"));

            _vtv.UpdateRootNodes(new [] { new VirtualTreeNode("newRoot") });

            Assert.AreEqual(0, _vtv.Nodes[0].Nodes.Count, "The last virtual child node wasn't removed");
            Assert.AreEqual(0, _tv.Nodes[0].Nodes.Count, "The last real child node wasn't removed");
        }

        [Test]
        public void TestUpdateWhenRemoveAllChildNodesOfExandedParent()
        {
            // this is a test for the bug shown in CA-34486 and CA-36409.

            // When merging sets of tree-nodes using UpdateRootNodes, if you remove all of
            // the child-nodes from a node, the node still remembers its IsExpanded state
            // from before the nodes were removed regardless of whether you call Collapse()
            // or Expand() after the nodes were removed.

            // This causes problems for the Virtual treeview as it
            // relies the BeforeExpanded event to convert DummyTreeNodes into VirtualTreeNodes
            // on population.

            VirtualTreeNode rootNode = new VirtualTreeNode("root");
            rootNode.Nodes.Add(new VirtualTreeNode("child"));
            rootNode.Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild"));
            rootNode.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("grandgrandchild"));

            _vtv.UpdateRootNodes(new [] { rootNode });
            _vtv.EndUpdate();

            // expand all nodes so that all DummyNodes are replaced.
            _vtv.ExpandAll();

            // now merge with smaller tree
            VirtualTreeNode rootNode2 = new VirtualTreeNode("root2");
            rootNode2.Nodes.Add(new VirtualTreeNode("child2"));
            rootNode2.Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild2"));
            _vtv.UpdateRootNodes(new [] { rootNode2 });
            _vtv.EndUpdate();

            // collapse all so the next merge doesn't replace DummyNodes.
            _vtv.CollapseAll();

            // now re-merge with big tree
            VirtualTreeNode rootNode3 = new VirtualTreeNode("root3");
            rootNode3.Nodes.Add(new VirtualTreeNode("child3"));
            rootNode3.Nodes[0].Nodes.Add(new VirtualTreeNode("grandchild3"));
            rootNode3.Nodes[0].Nodes[0].Nodes.Add(new VirtualTreeNode("grandgrandchild3"));

            _vtv.UpdateRootNodes(new [] { rootNode3 });
            _vtv.EndUpdate();
            _vtv.Nodes[0].Expand();
            _vtv.Nodes[0].Nodes[0].Expand();

            Assert.IsFalse(_vtv.Nodes[0].Nodes[0].Nodes[0].IsExpanded, "Expanded state was remembered. It shouldn't be.");
        }
    }
}
