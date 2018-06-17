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
using XenAdmin.Model;
using XenAdmin.Network;
using System.Collections;
using XenAPI;
using XenAdmin.Core;
using XenCenterLib;

// I think this is more complicated then it needs to be. Rather than have three different types of nodes,
// depending on the grouping of the next level, we should just have one type of node and do something like
// foreach (IXenObject o in XenSearchableObjects)
//   foreach (Grouping g in search.Grouping)
//     add node for the group;
//   add node for the object;
// SRET 2009-04-28

namespace XenAdmin.XenSearch
{
    public interface IAcceptGroups
    {
        IAcceptGroups Add(Grouping grouping, Object group, int indent);

        void FinishedInThisGroup(bool defaultExpand);
    }

    public abstract class Group
    {
        public static Group GetGrouped(Search search)
        {
            Group group = GetGroupFor(null, search, search.EffectiveGrouping);

            GetGrouped(search, group);

            return group;
        }

        protected static Group GetGroupFor(Grouping grouping, Search search, Grouping subgrouping)
        {
            if (grouping is FolderGrouping)
                return new FolderGroup(search, grouping);
            else if (subgrouping == null)
                return new LeafGroup(search);
            else
                return new NodeGroup(search, subgrouping);
        }

        private static void GetGrouped(Search search, Group group)
        {
            search.Items = 0;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (connection.IsConnected && Helpers.GetPoolOfOne(connection) != null)
                {
                    foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                        if (!Hide(o))
                            group.FilterAdd(search.Query,o );
                }
                else
                {
                    // Fake out disconnected connections with a host.
                    // We do this because IXenConnection is not an IXenObject, and so 
                    // is not XenSearchable and cannot be put in the treeview.
                    Host disconnectedHost = new Host();
                    disconnectedHost.opaque_ref = connection.HostnameWithPort;
                    disconnectedHost.name_label = Helpers.GetName(connection);
                    disconnectedHost.Connection = connection;

                    group.FilterAdd(search.Query, disconnectedHost);
                }
            }
        }

        private static bool Hide(IXenObject o)
        {
            if (XenAdminConfigManager.Provider.ObjectIsHidden(o.opaque_ref))
                return true;

            if (o is VM)
            {
                VM vm = o as VM;
                if (vm.is_control_domain
                    || !vm.Show(XenAdminConfigManager.Provider.ShowHiddenVMs))
                    return true;

                // Hide VMs on non-live hosts
                Host host = vm.Home();
                if (host != null && !host.IsLive())
                    return true;
            }
            else if (o is SR)
            {
                SR sr = o as SR;
                if (!sr.Show(XenAdminConfigManager.Provider.ShowHiddenVMs) || sr.IsToolsSR())
                    return true;

                // Hide SRs on non-live hosts
                Host host = sr.Home();
                if (host != null && !host.IsLive())
                    return true;
            }
            else if (o is XenAPI.Network)
            {
                XenAPI.Network network = o as XenAPI.Network;

                return !network.Show(XenAdminConfigManager.Provider.ShowHiddenVMs);
            }
            else if (o is Folder)
            {
                // Hide the root folder
                Folder folder = o as Folder;
                return folder.IsRootFolder;
            }

            return false;
        }

        protected readonly Search search;

        protected Group(Search search)
        {
            this.search = search;
        }

        protected int Compare(object one, object other)
        {
            return Compare(one, other, search);
        }

        protected int CompareGroupKeys(GroupKey one, GroupKey other)
        {
            if (one == null && other == null)
                return 0;
            if (one == null)
                return -1;
            if (other == null)
                return 1;
            return Compare(one.key, other.key);
        }

        /// <summary>
        /// Rules:
        /// - Other objects come always come before IXenObjects. This is because if they are
        ///   at the same level, they represent grouped items vs ungrouped items.
        /// - Then try and compare them using the requested sorting, if any.
        /// - If that fails, sort by type. For IXenObjects try a our own sort and then the
        ///   built-in sort for the type. The result is not always alphabetical; see CA-27829.
        /// - If those still don't separate them, try object name.
        /// </summary>
        public static int Compare(object one, object other, Search search)
        {
            if (one == null && other == null)
                return 0;
            if (one == null)
                return -1;
            if (other == null)
                return 1;

            IXenObject oneXenObject = one as IXenObject;
            IXenObject otherXenObject = other as IXenObject;

            if (oneXenObject == null && otherXenObject == null)
            {
                //neither is an IXenObject => compare them as other objects

                if (search != null && search.Sorting != null)
                {
                    foreach (Sort sort in search.Sorting)
                    {
                        int r = sort.Compare(one, other);
                        if (r != 0)
                            return r;
                    }
                }

                if (one.GetType() == other.GetType() && one is IComparable)
                {
                    int r1 = Comparer.Default.Compare(one, other);
                    if (r1 != 0)
                        return r1;
                }

                return StringUtility.NaturalCompare(one.ToString(), other.ToString());
            }

            if (oneXenObject == null)
                return -1;
            if (otherXenObject == null)
                return 1;

            // both are IXenObjects, compare them as such

            if (search != null && search.Sorting != null)
            {
                foreach (Sort sort in search.Sorting)
                {
                    int r2 = sort.Compare(oneXenObject, otherXenObject);
                    if (r2 != 0)
                        return r2;
                }
            }

            int r3 = CompareByType(oneXenObject, otherXenObject);
            if (r3 != 0)
                return r3;

            r3 = Comparer.Default.Compare(oneXenObject, otherXenObject);
            if (r3 != 0)
                return r3;

            return StringUtility.NaturalCompare(Helpers.GetName(oneXenObject), Helpers.GetName(otherXenObject));
        }

        /// <summary>
        /// Compare two IXMOs by type. I wanted to use the proper user-facing type from
        /// PropertyAccessors.properties[PropertyNames.type] but it turned out to be much
        /// too slow. Instead we use the type of the object with a few tweaks to sort
        /// important objects first.
        /// </summary>
        private static int CompareByType(IXenObject oneXenObject, IXenObject otherXenObject)
        {
            string t1 = TypeOf(oneXenObject);
            string t2 = TypeOf(otherXenObject);
            return t1.CompareTo(t2);
        }

        private static string TypeOf(IXenObject o)
        {
            if (o is Folder)
                return "10";
            if (o is Pool)
                return "20";
            if (o is Host)
                return "30";
            VM vm = o as VM;
            if (vm != null && vm.is_a_real_vm())
                return "40";
            return o.GetType().ToString();
        }

        private void FilterAdd(Query query, IXenObject o)
        {
            if (query == null || !query.Match(o))
                return;

            Add(o);
        }

        public abstract void Add(IXenObject o);

        public virtual bool Populate(IAcceptGroups adapter)
        {
            return Populate(adapter, 0, true);
        }

        public abstract bool Populate(IAcceptGroups adapter, int indent, bool defaultExpand);

        public abstract void PopulateFor(IAcceptGroups adapter, GroupKey group, int indent, bool defaultExpand);
        
        public abstract void GetNextLevel(List<GroupKey> nextLevel);
    }

    public class GroupKey : IEquatable<GroupKey>
    {
        public Grouping grouping;
        public object key;

        public GroupKey(Grouping grouping, object key)
        {
            this.grouping = grouping;
            this.key = key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public bool Equals(GroupKey other)
        {
            return other != null && grouping.Equals(other.grouping) && key.Equals(other.key);
        }

        public override bool Equals(object obj)
        {
            GroupKey other = obj as GroupKey;
            return other != null && Equals(other);
        }
    }

    public abstract class AbstractNodeGroup : Group
    {
        protected readonly Dictionary<GroupKey, Group> grouped;
        protected Group ungrouped = null; //this is late bound
        protected readonly Grouping grouping;
        
        protected AbstractNodeGroup(Search search, Grouping grouping)
            : base(search)
        {
            this.grouped = new Dictionary<GroupKey, Group>();
            this.grouping = grouping;
        }

        public override bool Populate(IAcceptGroups adapter)
        {
            return Populate(adapter, 0, false);
        }

        public override bool Populate(IAcceptGroups adapter, int indent, bool defaultExpand)
        {
            bool added = false;

            List<GroupKey> groups = new List<GroupKey>();

            GetNextLevel(groups);

            groups.Sort(CompareGroupKeys);

            foreach (GroupKey group in groups)
            {
                IAcceptGroups subAdapter = adapter.Add(group.grouping, group.key, indent);

                if (subAdapter == null)
                    continue;

                added = true;

                PopulateFor(subAdapter, group, indent + 1, defaultExpand);
            }

            adapter.FinishedInThisGroup(defaultExpand);

            return added;
        }

        public override void PopulateFor(IAcceptGroups adapter, GroupKey group, int indent, bool defaultExpand)
        {
            if (grouped.ContainsKey(group))
            {
                grouped[group].Populate(adapter, indent, defaultExpand);
            }
            else if (ungrouped != null)
            {
                ungrouped.PopulateFor(adapter, group, indent, defaultExpand);
            }
        }

        public override void GetNextLevel(List<GroupKey> nextLevel)
        {
            nextLevel.AddRange(grouped.Keys);
           
            if(ungrouped != null)
                ungrouped.GetNextLevel(nextLevel);
        }

        public Group FindOrAddSubgroup(Grouping grouping, object o, Grouping subgrouping)
        {
            GroupKey key = new GroupKey(grouping, o);
            if (!grouped.ContainsKey(key))
                grouped[key] = GetGroupFor(grouping, search, subgrouping);
            return grouped[key];
        }
    }

    public class NodeGroup : AbstractNodeGroup
    {
        public NodeGroup(Search search, Grouping grouping)
            : base(search, grouping)
        {
        }

        public override void Add(IXenObject o)
        {
            if (grouping.BelongsAsGroupNotMember(o))
            {
                GroupKey key = new GroupKey(grouping, o);
                if (!grouped.ContainsKey(key))
                    grouped[key] = GetGroupFor(grouping, search, grouping.subgrouping);

                return;
            }

            Object group = grouping.GetGroup(o);

            if (group == null)
            {
                AddUngrouped(o);
                return;
            }

            IList groups = group as IList;

            if (groups == null)
            {
                AddGrouped(o, group);
                return;
            }

            if (groups.Count == 0)
            {
                AddUngrouped(o);
                return;
            }

            foreach (Object g in groups)
            {
                if (g == null)
                {
                    AddUngrouped(o);
                    continue;
                }

                AddGrouped(o, g);
            }
        }

        private void AddGrouped(IXenObject o, Object group)
        {
            // We sometimes need to apply the query to the group. For example, suppose VM 1
            // is connected to Network 1 and Network 2. Then "VMs grouped by Network" will
            // show
            //
            // -- Network 1
            //    -- VM 1
            // -- Network 2
            //    -- VM 1
            //
            // So far so good. Now consider "VMs connected to Network 1 grouped by Network".
            // Without the following piece of code, that would show exactly the same thing:
            // because the second VM 1 is connected to Network 1, and is correctly grouped
            // in Network 2. But what the user presumably wanted to see was just Network 1
            // with its VMs under it: and that is achieved by applying the filter to the
            // networks.
            //
            // The consequence when adding new searches is that a search that returns a
            // XenObject of type T must return (T)o rather than null when o is a T,
            // otherwise if you both group and filter by that type, the group will fail
            // to match the filter and you'll get no results.
            //
            IXenObject groupModelObject = group as IXenObject;

            if (groupModelObject != null && XenAdminConfigManager.Provider.ObjectIsHidden(groupModelObject.opaque_ref))
            {
                return;
            }
            
            if (search.Query != null && groupModelObject != null)
            {
                QueryFilter subquery = grouping.GetRelevantGroupQuery(search);
                if (subquery != null && subquery.Match(groupModelObject) == false)
                    return;
            }

            Group nextGroup;

            // Some types of grouping can add several levels to the hierarchy.
            // This should not be confused with the IList in Add(IXenObject o):
            // that adds the item to several groups, whereas this adds it to a
            // single group several levels deep. In order to reach here,
            // grouping.GetGroup(o) must return a list of arrays.
            //
            // NB We don't do the GetRelevantGroupQuery() check as above for the
            // groups added in this way because we never need it: but if we ever
            // add it, we should probably do a first pass to check all the groups
            // first before adding any.
            Array groups = group as Array;
            if (groups != null)
            {
                nextGroup = this;
                for (int i = 0; i < groups.Length; ++i)
                {
                    Grouping gr = (i == groups.Length - 1 ? grouping.subgrouping : grouping);
                    nextGroup = (nextGroup as AbstractNodeGroup).FindOrAddSubgroup(grouping, groups.GetValue(i), gr);
                }
            }
            else
            {
                nextGroup = FindOrAddSubgroup(grouping, group, grouping.subgrouping);
            }

            nextGroup.Add(o);
        }


        protected void AddUngrouped(IXenObject o)
        {
            if (!XenAdminConfigManager.Provider.ObjectIsHidden(o.opaque_ref))
            {
                if (ungrouped == null)
                    ungrouped = GetGroupFor(grouping, search, grouping.subgrouping);


                ungrouped.Add(o);
            }
        }
    }

    public class FolderGroup : AbstractNodeGroup
    {
        public FolderGroup(Search search, Grouping grouping)
            : base(search, grouping)
        {
        }

        public override void Add(IXenObject o)
        {
            if (o is Folder)
            {
                GroupKey key = new GroupKey(grouping, o);
                if (!grouped.ContainsKey(key))
                    grouped[key] = new FolderGroup(search, grouping);
            }
            else
            {
                if (ungrouped == null)
                    ungrouped = new LeafGroup(search);
                ungrouped.Add(o);
            }
        }
    }

    public class LeafGroup : Group
    {
        internal readonly List<IXenObject> items;

        public LeafGroup(Search search)
            : base(search)
        {
            this.items = new List<IXenObject>();
        }

        public override void Add(IXenObject o)
        {
            if (o is Folder && items.Contains(o))  // one folder can appear on several connections
                return;
            search.Items++;
            items.Add(o);
        }

        public override bool Populate(IAcceptGroups adapter, int indent, bool defaultExpand)
        {
            bool added = false;

            items.Sort(Compare);

            foreach (IXenObject o in items)
            {
                IAcceptGroups subAdapter = adapter.Add(null, o, indent);

                if (subAdapter != null)
                {
                    added = true;
                    subAdapter.FinishedInThisGroup(defaultExpand);
                }
            }

            adapter.FinishedInThisGroup(defaultExpand);

            return added;
        }

        public override void GetNextLevel(List<GroupKey> nextLevel)
        {
            foreach(IXenObject item in items)
                nextLevel.Add(new GroupKey(null, item));
        }

        public override void PopulateFor(IAcceptGroups adapter, GroupKey group, int indent, bool defaultExpand)
        {
            adapter.FinishedInThisGroup(defaultExpand);
        }
    }   
}
