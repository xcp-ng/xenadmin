/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using XenCenterLib;
using System.Runtime.InteropServices;

namespace XenAdmin.Controls
{
    public enum TreeViewSelectionMode
    {
        SingleSelect,
        MultiSelect,
        MultiSelectSameRootBranch,
        MultiSelectSameLevel,
        MultiSelectSameLevelAndRootBranch,
        MultiSelectSameParent
    }

    public interface IMultiSelectTreeNodeCollectionOwner
    {
        /// <summary>
        /// Gets a complete collection of nodes owned.
        /// </summary>
        TreeNodeCollection Nodes { get;}
    }

    public partial class MultiSelectTreeView : TreeView, IMultiSelectTreeNodeCollectionOwner
    {
        private bool _nodeProcessedOnMouseDown;
        private bool _selectionChanged;
        private bool _wasDoubleClick;
        private readonly InternalSelectedNodeCollection _internalSelectedNodes = new InternalSelectedNodeCollection();
        private readonly MultiSelectTreeSelectedNodeCollection _selectedNodes;
        private int intMouseClicks;
        private TreeViewSelectionMode _selectionMode;
        private MultiSelectTreeNode _keysStartNode;
        private MultiSelectTreeNode _mostRecentSelectedNode;
        private MultiSelectTreeNode _nodeToStartEditOn;
        private MultiSelectTreeNode _selectionMirrorPoint;
        private MultiSelectTreeNodeCollection _nodes;

        public Point LastMouseDownEventPosition { get; private set; }

        public event TreeViewEventHandler AfterDeselect;
        public event TreeViewEventHandler BeforeDeselect;
        public event EventHandler SelectionsChanged;
        public new event EventHandler<MultiSelectTreeViewCancelEventArgs> BeforeSelect;
        public new event EventHandler<MultiSelectTreeViewItemDragEventArgs> ItemDrag;

        public MultiSelectTreeView()
        {
            _selectedNodes = new MultiSelectTreeSelectedNodeCollection(this);
            _nodes = new MultiSelectTreeNodeCollection(this);
        }

        public new MultiSelectTreeNodeCollection Nodes
        {
            get
            {
                return _nodes;
            }
        }

        private MultiSelectTreeNode GetLastVisibleNode()
        {
            MultiSelectTreeNode nextVisibleNode = Nodes[0];
            while (nextVisibleNode.NextVisibleNode != null)
            {
                nextVisibleNode = nextVisibleNode.NextVisibleNode;
            }
            return nextVisibleNode;
        }

        private MultiSelectTreeNode GetNextTreeNode(MultiSelectTreeNode start, bool down, int intNumber)
        {
            int num = 0;
            MultiSelectTreeNode nextVisibleNode = start;
            while (num < intNumber)
            {
                if (down)
                {
                    if (nextVisibleNode.NextVisibleNode == null)
                    {
                        return nextVisibleNode;
                    }
                    nextVisibleNode = nextVisibleNode.NextVisibleNode;
                }
                else
                {
                    if (nextVisibleNode.PrevVisibleNode == null)
                    {
                        return nextVisibleNode;
                    }
                    nextVisibleNode = nextVisibleNode.PrevVisibleNode;
                }
                num++;
            }
            return nextVisibleNode;
        }

        public int GetNodeLevel(MultiSelectTreeNode node)
        {
            int num = 0;
            while ((node = node.Parent) != null)
            {
                num++;
            }
            return num;
        }

        private int GetNumberOfVisibleNodes()
        {
            int num = 0;
            for (MultiSelectTreeNode node = Nodes[0]; node != null; node = node.NextVisibleNode)
            {
                if (node.IsVisible)
                {
                    num++;
                }
            }
            return num;
        }

        public MultiSelectTreeNode GetRootParent(MultiSelectTreeNode child)
        {
            MultiSelectTreeNode parent = child;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        private bool IsChildOf(MultiSelectTreeNode child, MultiSelectTreeNode parent)
        {
            for (MultiSelectTreeNode node = child; node != null; node = node.Parent)
            {
                if (node == parent)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsClickOnNode(MultiSelectTreeNode node, MouseEventArgs e)
        {
            return node != null && e.X < node.Bounds.X + node.Bounds.Width;
        }

        private bool IsNodeSelected(MultiSelectTreeNode node)
        {
            return node != null && _internalSelectedNodes.Contains(node);
        }

        private bool IsPlusMinusClicked(MultiSelectTreeNode node, MouseEventArgs e)
        {
            return e.X < (20 + (GetNodeLevel(node) * 20)) - HScrollPos;
        }

        /// <summary>
        /// Occurs after a node is collapsed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            _selectionChanged = false;

            // All child nodes should be deselected
            bool childSelected = false;
            foreach (MultiSelectTreeNode node in e.Node.Nodes)
            {
                if (IsNodeSelected(node))
                {
                    childSelected = true;
                }
                UnselectNodesRecursively(node, TreeViewAction.Collapse);
            }

            if (childSelected)
            {
                SelectNode((MultiSelectTreeNode)e.Node, true, TreeViewAction.Collapse);
            }

            OnSelectionsChanged();

            base.OnAfterCollapse(e);
        }

        protected virtual void OnAfterDeselect(TreeViewEventArgs e)
        {
            TreeViewEventHandler handler = AfterDeselect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeDeselect(TreeViewEventArgs e)
        {
            TreeViewEventHandler handler = BeforeDeselect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            _selectionChanged = false;
            SelectNode((MultiSelectTreeNode)e.Node, true, TreeViewAction.ByMouse);
            UnselectAllNodesExceptNode((MultiSelectTreeNode)e.Node, TreeViewAction.ByMouse);
            OnSelectionsChanged();
        }

        protected sealed override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void OnBeforeSelect(MultiSelectTreeViewCancelEventArgs e)
        {
            EventHandler<MultiSelectTreeViewCancelEventArgs> handler = BeforeSelect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected sealed override void OnItemDrag(ItemDragEventArgs e)
        {
            OnItemDrag(new MultiSelectTreeViewItemDragEventArgs(MouseButtons.Left, SelectedNodes));
        }

        protected virtual void OnItemDrag(MultiSelectTreeViewItemDragEventArgs e)
        {
            EventHandler<MultiSelectTreeViewItemDragEventArgs> handler = ItemDrag;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keys none = Keys.None;
            switch (e.Modifiers)
            {
                case Keys.Shift:
                case Keys.Control:
                case (Keys.Control | Keys.Shift):
                    none = Keys.Shift;
                    if (_keysStartNode == null)
                    {
                        _keysStartNode = _mostRecentSelectedNode;
                    }
                    break;

                default:
                    _keysStartNode = null;
                    break;
            }
            int intNumber = 0;
            MultiSelectTreeNode endNode = null;
            switch (e.KeyCode)
            {
                case Keys.Prior:
                    intNumber = GetNumberOfVisibleNodes();
                    endNode = GetNextTreeNode(_mostRecentSelectedNode, false, intNumber);
                    break;

                case Keys.Next:
                    intNumber = GetNumberOfVisibleNodes();
                    endNode = GetNextTreeNode(_mostRecentSelectedNode, true, intNumber);
                    break;

                case Keys.End:
                    endNode = GetLastVisibleNode();
                    break;

                case Keys.Home:
                    endNode = Nodes[0];
                    break;

                case Keys.Left:
                    if (_mostRecentSelectedNode != null)
                    {
                        if (_mostRecentSelectedNode.IsExpanded)
                            _mostRecentSelectedNode.Collapse();
                        else
                            endNode = _mostRecentSelectedNode.Parent;
                    }
                    break;

                case Keys.Up:
                    if (_mostRecentSelectedNode != null)
                        endNode = _mostRecentSelectedNode.PrevVisibleNode;
                    break;

                case Keys.Right:
                    if (_mostRecentSelectedNode != null)
                    {
                        if (_mostRecentSelectedNode.IsExpanded)
                        {
                            endNode = _mostRecentSelectedNode.NextVisibleNode;
                            if (endNode != null && !endNode.Parent.Equals(_mostRecentSelectedNode))
                                endNode = null;
                        }
                        else
                            _mostRecentSelectedNode.Expand();
                    }
                    break;

                case Keys.Down:
                    if (_mostRecentSelectedNode != null)
                        endNode = _mostRecentSelectedNode.NextVisibleNode;
                    break;

                default:
                    base.OnKeyDown(e);
                    return;
            }
            if (endNode != null)
            {
                ProcessNodeRange(_keysStartNode, endNode, new MouseEventArgs(MouseButtons.Left, 1, Cursor.Position.X, Cursor.Position.Y, 0), none, TreeViewAction.ByKeyboard, false);
                _mostRecentSelectedNode = endNode;
            }
            if (_mostRecentSelectedNode != null)
            {
                MultiSelectTreeNode tnMostRecentSelectedNode = null;
                switch (e.KeyCode)
                {
                    case Keys.Prior:
                        tnMostRecentSelectedNode = GetNextTreeNode(_mostRecentSelectedNode, false, intNumber - 2);
                        break;

                    case Keys.Next:
                        tnMostRecentSelectedNode = GetNextTreeNode(_mostRecentSelectedNode, true, intNumber - 2);
                        break;

                    case Keys.End:
                    case Keys.Home:
                        tnMostRecentSelectedNode = _mostRecentSelectedNode;
                        break;

                    case Keys.Up:
                        tnMostRecentSelectedNode = GetNextTreeNode(_mostRecentSelectedNode, false, 5);
                        break;

                    case Keys.Down:
                        tnMostRecentSelectedNode = GetNextTreeNode(_mostRecentSelectedNode, true, 5);
                        break;
                }
                if (tnMostRecentSelectedNode != null)
                {
                    if ((e.KeyData & Keys.Control) != 0 || (e.KeyData & Keys.Shift) != 0)
                    {
                        SuspendLayout();
                        int prevScrollPos = HScrollPos;
                        tnMostRecentSelectedNode.EnsureVisible();
                        HScrollPos = prevScrollPos;
                        ResumeLayout();
                    }
                    else
                    {
                        tnMostRecentSelectedNode.EnsureVisible();
                    }
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            LastMouseDownEventPosition = new Point(e.X, e.Y);
            _keysStartNode = null;
            intMouseClicks = e.Clicks;
            MultiSelectTreeNode nodeAt = (MultiSelectTreeNode)base.GetNodeAt(e.X, e.Y);
            if (nodeAt != null)
            {
                if (!IsPlusMinusClicked(nodeAt, e) && (nodeAt != null) && IsClickOnNode(nodeAt, e) && !IsNodeSelected(nodeAt))
                {
                    _nodeProcessedOnMouseDown = true;
                    ProcessNodeRange(_mostRecentSelectedNode, nodeAt, e, Control.ModifierKeys, TreeViewAction.ByMouse, true);
                }
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!_nodeProcessedOnMouseDown)
            {
                MultiSelectTreeNode nodeAt = (MultiSelectTreeNode)base.GetNodeAt(e.X, e.Y);
                if (IsClickOnNode(nodeAt, e))
                {
                    ProcessNodeRange(_mostRecentSelectedNode, nodeAt, e, Control.ModifierKeys, TreeViewAction.ByMouse, true);
                }
            }
            _nodeProcessedOnMouseDown = false;
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Fires a SelectionsChanged event if the selection has changed.
        /// </summary>
        protected virtual void OnSelectionsChanged()
        {
            if (_selectionChanged)
            {
                EventHandler handler = SelectionsChanged;

                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Forces the SelectionsChanged event to fire.
        /// </summary>
        public void ForceSelectionsChanged()
        {
            EventHandler handler = SelectionsChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Processes a node range.
        /// </summary>
        /// <param name="startNode">Start node of range.</param>
        /// <param name="endNode">End node of range.</param>
        /// <param name="e">MouseEventArgs.</param>
        /// <param name="keys">Keys.</param>
        /// <param name="tva">TreeViewAction.</param>
        /// <param name="allowStartEdit">True if node can go to edit mode, false if not.</param>
        private void ProcessNodeRange(MultiSelectTreeNode startNode, MultiSelectTreeNode endNode, MouseEventArgs e, Keys keys, TreeViewAction tva, bool allowStartEdit)
        {
            _selectionChanged = false; // prepare for OnSelectionsChanged

            if (e.Button == MouseButtons.Left)
            {
                _wasDoubleClick = (intMouseClicks == 2);

                MultiSelectTreeNode tnTemp = null;
                int intNodeLevelStart;

                if (((keys & Keys.Control) == 0) && ((keys & Keys.Shift) == 0))
                {
                    // CTRL and SHIFT not held down							
                    _selectionMirrorPoint = endNode;
                    int intNumberOfSelectedNodes = SelectedNodes.Count;

                    // If it was a double click, select node and suspend further processing					
                    if (_wasDoubleClick)
                    {
                        base.OnMouseDown(e);
                        return;
                    }

                    if (!IsPlusMinusClicked(endNode, e))
                    {
                        bool blnNodeWasSelected = false;
                        if (IsNodeSelected(endNode))
                            blnNodeWasSelected = true;


                        UnselectAllNodesExceptNode(endNode, tva);
                        SelectNode(endNode, true, tva);


                        if ((blnNodeWasSelected) && (LabelEdit) && (allowStartEdit) && (!_wasDoubleClick) && (intNumberOfSelectedNodes <= 1))
                        {
                            // Node should be put in edit mode					
                            _nodeToStartEditOn = endNode;
                            System.Threading.ThreadPool.QueueUserWorkItem(StartEdit);
                        }
                    }
                }
                else if (((keys & Keys.Control) != 0) && ((keys & Keys.Shift) == 0))
                {
                    // CTRL held down
                    _selectionMirrorPoint = null;

                    if (!IsNodeSelected(endNode))
                    {
                        switch (_selectionMode)
                        {
                            case TreeViewSelectionMode.SingleSelect:
                                UnselectAllNodesExceptNode(endNode, tva);
                                break;

                            case TreeViewSelectionMode.MultiSelectSameRootBranch:
                                MultiSelectTreeNode tnAbsoluteParent2 = GetRootParent(endNode);
                                UnselectAllNodesNotBelongingToParent(tnAbsoluteParent2, tva);
                                break;

                            case TreeViewSelectionMode.MultiSelectSameLevel:
                                UnselectAllNodesNotBelongingToLevel(GetNodeLevel(endNode), tva);
                                break;

                            case TreeViewSelectionMode.MultiSelectSameLevelAndRootBranch:
                                MultiSelectTreeNode tnAbsoluteParent = GetRootParent(endNode);
                                UnselectAllNodesNotBelongingToParent(tnAbsoluteParent, tva);
                                UnselectAllNodesNotBelongingToLevel(GetNodeLevel(endNode), tva);
                                break;

                            case TreeViewSelectionMode.MultiSelectSameParent:
                                MultiSelectTreeNode tnParent = endNode.Parent;
                                UnselectAllNodesNotBelongingDirectlyToParent(tnParent, tva);
                                break;
                        }

                        SelectNode(endNode, true, tva);
                    }
                    else
                    {
                        SelectNode(endNode, false, tva);
                    }
                }
                else if (((keys & Keys.Control) == 0) && ((keys & Keys.Shift) != 0))
                {
                    // SHIFT pressed
                    if (_selectionMirrorPoint == null)
                    {
                        _selectionMirrorPoint = startNode;
                    }

                    switch (_selectionMode)
                    {
                        case TreeViewSelectionMode.SingleSelect:
                            UnselectAllNodesExceptNode(endNode, tva);
                            SelectNode(endNode, true, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameRootBranch:
                            MultiSelectTreeNode tnAbsoluteParentStartNode = GetRootParent(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    MultiSelectTreeNode tnAbsoluteParent = GetRootParent(tnTemp);
                                    if (tnAbsoluteParent == tnAbsoluteParentStartNode)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToParent(tnAbsoluteParentStartNode, tva);
                            UnselectNodesOutsideRange(_selectionMirrorPoint, endNode, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameLevel:
                            intNodeLevelStart = GetNodeLevel(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    int intNodeLevel = GetNodeLevel(tnTemp);
                                    if (intNodeLevel == intNodeLevelStart)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToLevel(intNodeLevelStart, tva);
                            UnselectNodesOutsideRange(_selectionMirrorPoint, endNode, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameLevelAndRootBranch:
                            MultiSelectTreeNode tnAbsoluteParentStart = GetRootParent(startNode);
                            intNodeLevelStart = GetNodeLevel(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    int intNodeLevel = GetNodeLevel(tnTemp);
                                    MultiSelectTreeNode tnAbsoluteParent = GetRootParent(tnTemp);
                                    if ((intNodeLevel == intNodeLevelStart) && (tnAbsoluteParent == tnAbsoluteParentStart))
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToParent(tnAbsoluteParentStart, tva);
                            UnselectAllNodesNotBelongingToLevel(intNodeLevelStart, tva);
                            UnselectNodesOutsideRange(_selectionMirrorPoint, endNode, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelect:
                            SelectNodesInsideRange(_selectionMirrorPoint, endNode, tva);
                            UnselectNodesOutsideRange(_selectionMirrorPoint, endNode, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameParent:
                            MultiSelectTreeNode tnParentStartNode = startNode.Parent;
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    MultiSelectTreeNode tnParent = tnTemp.Parent;
                                    if (tnParent == tnParentStartNode)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingDirectlyToParent(tnParentStartNode, tva);
                            UnselectNodesOutsideRange(_selectionMirrorPoint, endNode, tva);
                            break;
                    }
                }
                else if (((keys & Keys.Control) != 0) && ((keys & Keys.Shift) != 0))
                {
                    // SHIFT AND CTRL pressed
                    switch (_selectionMode)
                    {
                        case TreeViewSelectionMode.SingleSelect:
                            UnselectAllNodesExceptNode(endNode, tva);
                            SelectNode(endNode, true, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameRootBranch:
                            MultiSelectTreeNode tnAbsoluteParentStartNode = GetRootParent(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    MultiSelectTreeNode tnAbsoluteParent = GetRootParent(tnTemp);
                                    if (tnAbsoluteParent == tnAbsoluteParentStartNode)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToParent(tnAbsoluteParentStartNode, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameLevel:
                            intNodeLevelStart = GetNodeLevel(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    int intNodeLevel = GetNodeLevel(tnTemp);
                                    if (intNodeLevel == intNodeLevelStart)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToLevel(intNodeLevelStart, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelectSameLevelAndRootBranch:
                            MultiSelectTreeNode tnAbsoluteParentStart = GetRootParent(startNode);
                            intNodeLevelStart = GetNodeLevel(startNode);
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    int intNodeLevel = GetNodeLevel(tnTemp);
                                    MultiSelectTreeNode tnAbsoluteParent = GetRootParent(tnTemp);
                                    if ((intNodeLevel == intNodeLevelStart) && (tnAbsoluteParent == tnAbsoluteParentStart))
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingToParent(tnAbsoluteParentStart, tva);
                            UnselectAllNodesNotBelongingToLevel(intNodeLevelStart, tva);
                            break;

                        case TreeViewSelectionMode.MultiSelect:
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    SelectNode(tnTemp, true, tva);
                                }
                            }
                            break;

                        case TreeViewSelectionMode.MultiSelectSameParent:
                            MultiSelectTreeNode tnParentStartNode = startNode.Parent;
                            tnTemp = startNode;
                            // Check each visible node from startNode to endNode and select it if needed
                            while ((tnTemp != null) && (tnTemp != endNode))
                            {
                                if (startNode.Bounds.Y > endNode.Bounds.Y)
                                    tnTemp = tnTemp.PrevVisibleNode;
                                else
                                    tnTemp = tnTemp.NextVisibleNode;
                                if (tnTemp != null)
                                {
                                    MultiSelectTreeNode tnParent = tnTemp.Parent;
                                    if (tnParent == tnParentStartNode)
                                    {
                                        SelectNode(tnTemp, true, tva);
                                    }
                                }
                            }
                            UnselectAllNodesNotBelongingDirectlyToParent(tnParentStartNode, tva);
                            break;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // if right mouse button clicked, clear selection and select right-clicked node
                if (!IsNodeSelected(endNode))
                {
                    UnselectAllNodes(tva);
                    SelectNode(endNode, true, tva);
                }
            }
            OnSelectionsChanged();
        }

        private bool SelectNode(MultiSelectTreeNode node, bool select, TreeViewAction tva)
        {
            bool selected = false;
            if (node == null)
            {
                return false;
            }
            if (select)
            {
                if (!IsNodeSelected(node))
                {
                    MultiSelectTreeViewCancelEventArgs e = new MultiSelectTreeViewCancelEventArgs(node, false, tva);
                    OnBeforeSelect(e);
                    if (e.Cancel)
                    {
                        return false;
                    }
                    _internalSelectedNodes.Add(node);
                    selected = true;
                    _selectionChanged = true;
                    OnAfterSelect(new TreeViewEventArgs(node, tva));
                }
                _mostRecentSelectedNode = node;
                return selected;
            }
            if (IsNodeSelected(node))
            {
                OnBeforeDeselect(new TreeViewEventArgs(node));
                _internalSelectedNodes.Remove(node);
                _selectionChanged = true;

                OnAfterDeselect(new TreeViewEventArgs(node));
            }
            return selected;
        }

        private void SelectNodesInsideRange(MultiSelectTreeNode startNode, MultiSelectTreeNode endNode, TreeViewAction tva)
        {
            if (startNode != null && endNode != null)
            {
                MultiSelectTreeNode firstNode = null;
                MultiSelectTreeNode lastNode = null;
                if (startNode.Bounds.Y < endNode.Bounds.Y)
                {
                    firstNode = startNode;
                    lastNode = endNode;
                }
                else
                {
                    firstNode = endNode;
                    lastNode = startNode;
                }
                SelectNode(firstNode, true, tva);
                MultiSelectTreeNode nextVisibleNode = firstNode;
                while (nextVisibleNode != lastNode && nextVisibleNode != null)
                {
                    nextVisibleNode = nextVisibleNode.NextVisibleNode;
                    if (nextVisibleNode != null)
                    {
                        SelectNode(nextVisibleNode, true, tva);
                    }
                }
                SelectNode(lastNode, true, tva);
            }
        }

        private void StartEdit(object state)
        {
            Thread.Sleep(200);
            if (!_wasDoubleClick)
            {
                base.SelectedNode = _nodeToStartEditOn;
                _nodeToStartEditOn.BeginEdit();
            }
            else
            {
                _wasDoubleClick = false;
            }
        }

        private void UnselectAllNodes(TreeViewAction tva)
        {
            UnselectAllNodesExceptNode(null, tva);
        }

        private void UnselectAllNodesExceptNode(MultiSelectTreeNode nodeKeepSelected, TreeViewAction tva)
        {
            List<MultiSelectTreeNode> list = new List<MultiSelectTreeNode>();

            foreach (MultiSelectTreeNode node in _internalSelectedNodes)
            {
                if (nodeKeepSelected == null)
                {
                    list.Add(node);
                }
                else if ((nodeKeepSelected != null) && (node != nodeKeepSelected))
                {
                    list.Add(node);
                }
            }
            foreach (MultiSelectTreeNode node2 in list)
            {
                SelectNode(node2, false, tva);
            }
        }

        private void UnselectAllNodesNotBelongingDirectlyToParent(MultiSelectTreeNode parent, TreeViewAction tva)
        {
            ArrayList list = new ArrayList();
            foreach (MultiSelectTreeNode node in _internalSelectedNodes)
            {
                if (node.Parent != parent)
                {
                    list.Add(node);
                }
            }
            foreach (MultiSelectTreeNode node2 in list)
            {
                SelectNode(node2, false, tva);
            }
        }

        private void UnselectAllNodesNotBelongingToLevel(int level, TreeViewAction tva)
        {
            ArrayList list = new ArrayList();
            foreach (MultiSelectTreeNode node in _internalSelectedNodes)
            {
                if (GetNodeLevel(node) != level)
                {
                    list.Add(node);
                }
            }
            foreach (MultiSelectTreeNode node2 in list)
            {
                SelectNode(node2, false, tva);
            }
        }

        private void UnselectAllNodesNotBelongingToParent(MultiSelectTreeNode parent, TreeViewAction tva)
        {
            ArrayList list = new ArrayList();
            foreach (MultiSelectTreeNode node in _internalSelectedNodes)
            {
                if (!IsChildOf(node, parent))
                {
                    list.Add(node);
                }
            }
            foreach (MultiSelectTreeNode node2 in list)
            {
                SelectNode(node2, false, tva);
            }
        }

        private void UnselectNodesOutsideRange(MultiSelectTreeNode startNode, MultiSelectTreeNode endNode, TreeViewAction tva)
        {
            if (startNode != null && endNode != null)
            {
                MultiSelectTreeNode node = null;
                MultiSelectTreeNode node2 = null;
                if (startNode.Bounds.Y < endNode.Bounds.Y)
                {
                    node = startNode;
                    node2 = endNode;
                }
                else
                {
                    node = endNode;
                    node2 = startNode;
                }
                MultiSelectTreeNode tn = node;
                while (tn != null)
                {
                    tn = tn.PrevVisibleNode;
                    if (tn != null)
                    {
                        SelectNode(tn, false, tva);
                    }
                }
                tn = node2;
                while (tn != null)
                {
                    tn = tn.NextVisibleNode;
                    if (tn != null)
                    {
                        SelectNode(tn, false, tva);
                    }
                }
            }
        }

        private void UnselectNodesRecursively(MultiSelectTreeNode tn, TreeViewAction tva)
        {
            SelectNode(tn, false, tva);

            foreach (MultiSelectTreeNode node in tn.Nodes)
            {
                UnselectNodesRecursively(node, tva);
            }
        }

        public new MultiSelectTreeNode SelectedNode
        {
            get
            {
                if (SelectedNodes.Count > 0)
                {
                    return SelectedNodes[0];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (SelectedNode != null)
                    {
                        SelectedNodes.Clear();
                    }
                }
                else if (SelectedNodes.Count == 1)
                {
                    if (SelectedNode != value)
                    {
                        SelectedNodes.SetContents(new MultiSelectTreeNode[] { value });
                    }
                }
                else
                {
                    SelectedNodes.SetContents(new MultiSelectTreeNode[] { value });
                }
            }
        }

        public MultiSelectTreeSelectedNodeCollection SelectedNodes => _selectedNodes;

        [DefaultValue(TreeViewSelectionMode.SingleSelect)]
        public TreeViewSelectionMode SelectionMode
        {
            get
            {
                return _selectionMode;
            }
            set
            {
                _selectionMode = value;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            SetEnabled(Nodes, Enabled);
        }

        private void SetEnabled(MultiSelectTreeNodeCollection nodes, bool enabled)
        {
            foreach (MultiSelectTreeNode node in nodes)
            {
                ((IMultiSelectTreeNode)node).SetEnabled(enabled);
                SetEnabled(node.Nodes, enabled);
            }
        }

        private int ScrollInfo(Win32.ScrollBarConstants fnBar)
        {
            Win32.ScrollInfo si = new Win32.ScrollInfo();
            si.cbSize = (uint)Marshal.SizeOf(si);
            si.fMask = (int)Win32.ScrollInfoMask.SIF_POS;
            if (!Win32.GetScrollInfo(Handle, (int)fnBar, ref si))
                return 0;

            return si.nPos;
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public int HScrollPos
        {
            get
            {
                if (System.Type.GetType("Mono.Runtime") == null)
                {
                    return ScrollInfo(Win32.ScrollBarConstants.SB_HORZ);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (System.Type.GetType("Mono.Runtime") == null)
                {
                    Win32.SendMessage(Handle, Win32.WM_HSCROLL, (IntPtr)(((int)Win32.ScrollBarCommands.SB_THUMBPOSITION) | (value << 16)), (IntPtr)0);
                }
            }
        }

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public int VScrollPos
        {
            get
            {
                return ScrollInfo(Win32.ScrollBarConstants.SB_VERT);
            }
        }

        /// <summary>
        /// Iterates through every node of this <see cref="MultiSelectTreeView"/>.
        /// </summary>
        public IEnumerable<MultiSelectTreeNode> AllNodes
        {
            get
            {
                foreach (MultiSelectTreeNode node in Nodes)
                {
                    yield return node;

                    foreach (MultiSelectTreeNode n in node.Descendants)
                    {
                        yield return n;
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            foreach (MultiSelectTreeNode node in AllNodes)
            {
                UpdateCheckboxVisibility(node);
            }
        }

        public void UpdateCheckboxVisibility(MultiSelectTreeNode treeNode)
        {
            if (CheckBoxes && treeNode.Handle != IntPtr.Zero && Handle != IntPtr.Zero)
            {
                NativeMethods.TVITEM tvItem = new NativeMethods.TVITEM();
                tvItem.hItem = treeNode.Handle;
                tvItem.mask = NativeMethods.TVIF_STATE;
                tvItem.stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                tvItem.state = 0;

                if (treeNode.ShowCheckBox && treeNode.Checked)
                {
                    tvItem.state = 2 << 12;
                }
                else if (treeNode.ShowCheckBox)
                {
                    tvItem.state = 1 << 12;
                }

                IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvItem));
                Marshal.StructureToPtr(tvItem, lparam, false);
                Win32.SendMessage(Handle, NativeMethods.TVM_SETITEM, IntPtr.Zero, lparam);
            }
        }

        #region NativeMethods class

        private class NativeMethods
        {
            public const int TVIF_STATE = 0x8;
            public const int TVIS_STATEIMAGEMASK = 0xF000;
            public const int TV_FIRST = 0x1100;
            public const int TVM_SETITEM = TV_FIRST + 63;

            public struct TVITEM
            {
#pragma warning disable 0649, 414, 169
                public int mask;
                public IntPtr hItem;
                public int state;
                public int stateMask;
                [MarshalAs(UnmanagedType.LPTStr)]
                public String lpszText;
                public int cchTextMax;
                public int iImage;
                public int iSelectedImage;
                public int cChildren;
                public IntPtr lParam;
#pragma warning restore 0649, 414, 169
            }
        }

        #endregion
    }
}
