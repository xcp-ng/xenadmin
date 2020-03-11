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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Model;
using XenAPI;
using XenAdmin.XenSearch;
using XenAdmin.Core;
using System.Drawing;


namespace XenAdmin
{
    internal class MainWindowTreeBuilder
    {
        private readonly FlickerFreeTreeView _treeView;
        private readonly Color _treeViewForeColor;
        private readonly Color _treeViewBackColor;
        private string _lastSearchText = string.Empty;
        private NavigationPane.NavigationMode _lastSearchMode;
        private readonly List<VirtualTreeNode.PersistenceInfo> _infraViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _objectViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _tagsViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _foldersViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _fieldsViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _vappsViewExpanded = new List<VirtualTreeNode.PersistenceInfo>();

        /// <remarks>
        /// The default value is true because the very first time the tree builder calls PersistExpandedNodes
        /// the tree is empty. This happens immediately after initialization of the tree (we shouldn't need to
        /// add by default a root node in the NavigationView constructor at design time).
        /// </remarks>
        private bool _rootExpanded = true;

        private readonly OrganizationViewFields viewFields = new OrganizationViewFields();
        private readonly OrganizationViewFolders viewFolders = new OrganizationViewFolders();
        private readonly OrganizationViewTags viewTags = new OrganizationViewTags();
        private readonly OrganizationViewObjects viewObjects = new OrganizationViewObjects();
        private readonly OrganizationViewVapps viewVapps = new OrganizationViewVapps();

        public MainWindowTreeBuilder(FlickerFreeTreeView treeView)
        {
            Util.ThrowIfParameterNull(treeView, "treeView");
            Program.AssertOnEventThread();

            _treeView = treeView;
            _treeViewForeColor = treeView.ForeColor;
            _treeViewBackColor = treeView.BackColor;
        }

        /// <summary>
        /// Gets or sets an object that should be highlighted.
        /// </summary>
        public object HighlightedDragTarget { private get; set; }

        /// <summary>
        /// Updates the <see cref="TreeView"/> with the specified new root node. This is done by merging the specified
        /// root node with the existing root node to minimize updates to the treeview to reduce flicker.
        /// </summary>
        /// <param name="newRootNode">The new root node.</param>
        /// <param name="searchText">The search text for the currently active search.</param>
        /// <param name="searchMode"></param>
        public void RefreshTreeView(VirtualTreeNode newRootNode, string searchText, NavigationPane.NavigationMode searchMode)
        {
            Util.ThrowIfParameterNull(newRootNode, "newRootNode");
            Util.ThrowIfParameterNull(searchText, "searchText");

            Program.AssertOnEventThread();

            _treeView.BeginUpdate();

            PersistExpandedNodes(searchText);

            _treeView.UpdateRootNodes(new[] { newRootNode });

            RestoreExpandedNodes(searchText, searchMode);

            bool searchTextCleared = (searchText.Length == 0 && searchText != _lastSearchText);

            _lastSearchText = searchText;
            _lastSearchMode = searchMode;
            _treeView.EndUpdate();

            // ensure that the selected nodes are visible when search text is cleared (CA-102127)
            if (searchTextCleared)
            {
                ExpandSelection();
            }
        }

        private void ExpandSelection()
        {
            foreach (var node in _treeView.SelectedNodes)
            {
                node.EnsureVisible();
            }
        }

        public VirtualTreeNode CreateNewRootNode(Search search, NavigationPane.NavigationMode mode)
        {
            IAcceptGroups groupAcceptor;
            VirtualTreeNode newRootNode;

            switch (mode)
            {
                case NavigationPane.NavigationMode.Objects:
                    newRootNode = viewObjects.RootNode;
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    viewObjects.Populate(search, groupAcceptor);
                    break;
                case NavigationPane.NavigationMode.Tags:
                    newRootNode = viewTags.RootNode;
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    viewTags.Populate(search, groupAcceptor);
                    break;
                case NavigationPane.NavigationMode.Folders:
                    newRootNode = viewFolders.RootNode;
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    viewFolders.Populate(search, groupAcceptor);
                    break;
                case NavigationPane.NavigationMode.CustomFields:
                    newRootNode = viewFields.RootNode;
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    viewFields.Populate(search, groupAcceptor);
                    break;
                case NavigationPane.NavigationMode.vApps:
                    newRootNode = viewVapps.RootNode;
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    viewVapps.Populate(search, groupAcceptor);
                    break;
                case NavigationPane.NavigationMode.SavedSearch:
                    Util.ThrowIfParameterNull(search, "search");
                    newRootNode = new VirtualTreeNode(search.Name)
                        {
                            Tag = search,
                            ImageIndex = search.DefaultSearch
                                             ? (int)Icons.DefaultSearch
                                             : (int)Icons.Search
                        };
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    search.PopulateAdapters(groupAcceptor);
                    break;
                default://includes Infrastructure and Notifications
                    Util.ThrowIfParameterNull(search, "search");
                    newRootNode = new VirtualTreeNode(BrandManager.BRAND_CONSOLE) { ImageIndex = (int)Icons.Home };
                    groupAcceptor = CreateGroupAcceptor(newRootNode);
                    search.PopulateAdapters(groupAcceptor);
                    break;
            }
            
            return newRootNode;
        }

        private MainWindowTreeNodeGroupAcceptor CreateGroupAcceptor(VirtualTreeNode parent)
        {
            return new MainWindowTreeNodeGroupAcceptor(HighlightedDragTarget, _treeViewForeColor, _treeViewBackColor, parent);
        }

        private List<VirtualTreeNode.PersistenceInfo> AssignList(NavigationPane.NavigationMode mode)
        {
            switch (mode)
            {
                case NavigationPane.NavigationMode.Objects:
                    return _objectViewExpanded;
                case NavigationPane.NavigationMode.Tags:
                    return _tagsViewExpanded;
                case NavigationPane.NavigationMode.Folders:
                    return _foldersViewExpanded;
                case NavigationPane.NavigationMode.CustomFields:
                    return _fieldsViewExpanded;
                case NavigationPane.NavigationMode.vApps:
                    return _vappsViewExpanded;
                default:
                    return _infraViewExpanded;
            }
        }

        private void PersistExpandedNodes(string searchText)
        {
            if (_treeView.Nodes.Count == 0)
                return;

            // only persist the expansion state of nodes if there isn't an active search.
            //If there's a search then we're just going to expand everything later.

            // also because we want to restore the nodes back to their original state
            //after a search is removed, do the check on _lastSearchText and _lastSearchMode.

            if (searchText.Length == 0 && _lastSearchText.Length == 0 && _lastSearchMode != NavigationPane.NavigationMode.SavedSearch)
            {
                var list = AssignList(_lastSearchMode);
                list.Clear();

                foreach (var node in _treeView.AllNodes)
                {
                    if (node.Tag != null && node.Parent != null && node.IsExpanded)
                        list.Add(node.GetPersistenceInfo());
                }
            }

            // persist the expansion state of the root node separately - it's a special case as its
            // expansion state isn't persisted differently depending on whether it's in org mode or not.
            _rootExpanded = _treeView.Nodes[0].IsExpanded || _treeView.Nodes[0].Nodes.Count == 0;
        }

        private void RestoreExpandedNodes(string searchText, NavigationPane.NavigationMode searchMode)
        {
            if ((searchText != _lastSearchText && searchText.Length > 0) || (searchMode == NavigationPane.NavigationMode.SavedSearch && _lastSearchMode != NavigationPane.NavigationMode.SavedSearch))
            {
                // expand all nodes if there's a search and the search has changed.
                _treeView.ExpandAll();
            }

            if (searchText.Length == 0 && searchMode != NavigationPane.NavigationMode.SavedSearch)
            {
                // if there isn't a search persist the user's expanded nodes.
                var list = AssignList(searchMode);

                var allNodes = new List<VirtualTreeNode>(_treeView.AllNodes);

                foreach (VirtualTreeNode.PersistenceInfo info in list)
                {
                    VirtualTreeNode match;

                    // First, look for the object in the same position
                    if (_treeView.TryExactMatch(info.Path, out match) >= info.Path.Count)
                    {
                        if (!match.IsExpanded)
                            match.Expand();

                        allNodes.Remove(match);
                        continue;
                    }

                    // Second, find the object in the maximal sub tree where it appeared only once
                    if (_treeView.TryExactMatch(info.PathToMaximalSubTree, out match) >= info.PathToMaximalSubTree.Count)
                    {
                        match = _treeView.FindNodeIn(match, info.Tag);
                        if (match != null)
                        {
                            if (!match.IsExpanded)
                                match.Expand();

                            allNodes.Remove(match);
                        }
                    }
                }

                // collapse everything else
                foreach (VirtualTreeNode n in allNodes)
                {
                    if (n.Tag != null && n.Parent != null && n.IsExpanded)
                        n.Collapse();
                }

                // special case for root node
                if (_rootExpanded)
                    _treeView.Nodes[0].Expand();
            }
        }

        #region MainWindowTreeNodeGroupAcceptor class

        private class MainWindowTreeNodeGroupAcceptor : IAcceptGroups
        {
            private readonly VirtualTreeNode _parent;
            private readonly Color _treeViewForeColor;
            private readonly Color _treeViewBackColor;
            private readonly object _highlightedDragTarget;
            private int _index;

            public MainWindowTreeNodeGroupAcceptor(object highlightedDragTarget,
                Color treeViewForeColor, Color treeViewBackColor, VirtualTreeNode parent)
            {
                _parent = parent;
                _treeViewForeColor = treeViewForeColor;
                _treeViewBackColor = treeViewBackColor;
                _highlightedDragTarget = highlightedDragTarget;
            }

            #region IAcceptGroups Members

            public void FinishedInThisGroup(bool defaultExpand)
            {
            }

            public IAcceptGroups Add(Grouping grouping, Object group, int indent)
            {
                if (group == null)
                    return null;

                VirtualTreeNode node;

                if (group is Pool)
                {
                    node = AddPoolNode((Pool)group);
                }
                else if (group is Host)
                {
                    node = AddHostNode((Host)group);
                }
                else if (group is VM)
                {
                    node = AddVMNode((VM)group);
                }
                else if (group is DockerContainer)
                {
                    node = AddDockerContainerNode((DockerContainer)group);
                }
                else if (group is VM_appliance)
                {
                    node = AddVmApplianceNode((VM_appliance)group);
                }
                else if (group is SR)
                {
                    node = AddSRNode((SR)group);
                }
                else if (group is XenAPI.Network)
                {
                    node = AddNetworkNode((XenAPI.Network)group);
                }
                else if (group is VDI)
                {
                    node = AddVDINode((VDI)group);
                }
                else if (group is Folder)
                {
                    node = AddFolderNode((Folder)group);
                }
                else
                {
                    node = AddNode(grouping.GetGroupName(group), grouping.GetGroupIcon(group),
                        false, new GroupingTag(grouping, GetGroupingTagFromNode(_parent), group));

                    if (group is DateTime)
                        Program.BeginInvoke(Program.MainWindow, () => { node.Text = HelpersGUI.DateTimeToString((DateTime)group, Messages.DATEFORMAT_DMY_HMS, true); }); // annoying: has to be on the event thread because of CA-46983
                }

                return new MainWindowTreeNodeGroupAcceptor(_highlightedDragTarget, _treeViewForeColor, _treeViewBackColor, node);
            }

            #endregion

            private static object GetGroupingTagFromNode(VirtualTreeNode node)
            {
                if (node == null)
                    return null;

                GroupingTag gt = node.Tag as GroupingTag;
                if (gt != null)
                {
                    // gt.Grouping is OrganizationalView means that we're at the Tags/Types/Custom Fields level.
                    // Custom field keys are the next level down, and values are the one after that.
                    return gt.Grouping is OrganizationalView ? null : gt.Group;
                }

                return node.Tag;
            }

            private VirtualTreeNode AddPoolNode(Pool pool)
            {
                return AddNode(Helpers.GetName(pool), Images.GetIconFor(pool), false, pool);
            }

            private VirtualTreeNode AddVMNode(VM vm)
            {
                bool hidden = vm.IsHidden();
                string name = hidden ? String.Format(Messages.X_HIDDEN, vm.Name()) : vm.Name();

                return AddNode(name, Images.GetIconFor(vm), hidden, vm);
            }

            private VirtualTreeNode AddDockerContainerNode(DockerContainer cont)
            {
                return AddNode(cont.Name().Ellipsise(1000), Images.GetIconFor(cont), cont.IsHidden(), cont);
            }

            private VirtualTreeNode AddVmApplianceNode(VM_appliance appliance)
            {
                return AddNode(appliance.Name(), Images.GetIconFor(appliance), false, appliance);
            }

            private VirtualTreeNode AddHostNode(Host host)
            {
                return AddNode(host.Name(), Images.GetIconFor(host), false, host);
            }

            private VirtualTreeNode AddSRNode(SR sr)
            {
                bool hidden = sr.IsHidden();
                String name = hidden ? String.Format(Messages.X_HIDDEN, sr.NameWithoutHost()) : sr.NameWithoutHost();
                return AddNode(name, Images.GetIconFor(sr), hidden, sr);
            }

            private VirtualTreeNode AddNetworkNode(XenAPI.Network network)
            {
                bool hidden = network.IsHidden();
                bool slave = network.IsSlave();
                string rawName = network.Name();
                String name = slave
                                  ? String.Format(Messages.NIC_SLAVE, rawName)
                                  : hidden
                                        ? String.Format(Messages.X_HIDDEN, rawName)
                                        : rawName;
                return AddNode(name, Images.GetIconFor(network), slave || hidden, network);
            }

            private VirtualTreeNode AddVDINode(VDI vdi)
            {
                String name = String.IsNullOrEmpty(vdi.Name()) ? Messages.NO_NAME : vdi.Name();
                return AddNode(name, Images.GetIconFor(vdi), false, vdi);
            }

            private VirtualTreeNode AddFolderNode(Folder folder)
            {
                return AddNode(folder.Name(), Images.GetIconFor(folder), false, folder);
            }

            private VirtualTreeNode AddNode(string name, Icons icon, bool grayed, object obj)
            {
                VirtualTreeNode result = new VirtualTreeNode(name.Ellipsise(1000))
                    {
                        Tag = obj,
                        ImageIndex = (int)icon,
                        SelectedImageIndex = (int)icon
                    };

                _parent.Nodes.Insert(_index, result);
                _index++;

                bool highlighted = _highlightedDragTarget != null && obj != null && _highlightedDragTarget.Equals(obj);

                if (highlighted)
                {
                    result.BackColor = SystemColors.Highlight;
                    result.ForeColor = SystemColors.HighlightText;
                }
                else if (grayed)
                {
                    result.BackColor = _treeViewBackColor;
                    result.ForeColor = SystemColors.GrayText;
                }
                else
                {
                    result.BackColor = _treeViewBackColor;
                    result.ForeColor = _treeViewForeColor;
                }

                return result;
            }
        }

        #endregion
    }
}
