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

using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin.XenSearch;
using XenAPI;


namespace XenAdmin
{
    public abstract class OrganizationalView : Grouping
    {
        protected OrganizationalView()
            : base((Grouping)null)
        {}

        protected abstract Grouping Grouping { get; }
        protected abstract QueryFilter Query { get; }

        /// <summary>
        /// Normally exclude folders even if in search: CA-27260
        /// </summary>
        protected virtual bool IncludeFolders
        {
            get { return false; }
        }

        public override object GetGroup(IXenObject o)
        {
            throw new NotImplementedException();
        }

        public VirtualTreeNode RootNode
        {
            get
            {
                return new VirtualTreeNode(GroupingName)
                {
                    Tag = new GroupingTag(this, null, GroupingName),
                    ImageIndex = (int)GetGroupIcon(null)
                };
            }
        }

        public void Populate(Search search, params IAcceptGroups[] groupAcceptors)
        {
            var orgViewSearch = OrgViewSearch(search);
            orgViewSearch.PopulateAdapters(groupAcceptors);
        }

        private Search OrgViewSearch(Search search)
        {
            ObjectTypes types;

            if (search == null || search.Query == null || search.Query.QueryScope == null)
            {
                types = IncludeFolders ? ObjectTypes.AllIncFolders : ObjectTypes.AllExcFolders;
            }
            else
            {
                types = IncludeFolders
                            ? search.Query.QueryScope.ObjectTypes
                            : (search.Query.QueryScope.ObjectTypes & ~ObjectTypes.Folder);
            }

            QueryScope scope = new QueryScope(types);
            
            QueryFilter filter;

            if (search == null || search.Query == null || search.Query.QueryFilter == null)
                filter = Query;
            else if (Query == null)
                filter = search.Query.QueryFilter;
            else
                filter = new GroupQuery(new[] { search.Query.QueryFilter, Query }, GroupQuery.GroupQueryType.And);

            return new Search(new Query(scope, filter), Grouping, "", null, null, new Sort[] { });
        }
    }


    public class OrganizationViewObjects : OrganizationalView
    {
        private readonly Grouping typesGrouping = new PropertyGrouping<ObjectTypes>(PropertyNames.type, null);

        public override string GroupingName
        {
            get { return Messages.VIEW_OBJECTS; }
        }

        public override Icons GetGroupIcon(object obj)
        {
            return Icons.Objects;
        }

        public override Grouping GetSubgrouping(object val)
        {
            return typesGrouping;
        }

        protected override Grouping Grouping { get { return typesGrouping; } }

        protected override QueryFilter Query { get { return null; } }
    }


    public class OrganizationViewTags : OrganizationalView
    {
        private readonly QueryFilter tagsQuery = new ListEmptyQuery<String>(PropertyNames.tags, false);
        private readonly Grouping tagsGrouping = new PropertyGrouping<String>(PropertyNames.tags, null);

        public override string GroupingName
        {
            get { return Messages.TAGS; }
        }

        public override Icons GetGroupIcon(object obj)
        {
            return Icons.Tag;
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            return tagsQuery;
        }

        public override Grouping GetSubgrouping(object val)
        {
            return tagsGrouping;
        }

        protected override Grouping Grouping { get { return tagsGrouping; } }

        protected override QueryFilter Query { get { return tagsQuery; } }
    }


    public class OrganizationViewFields : OrganizationalView
    {
        private readonly QueryFilter fieldsQuery = new BooleanQuery(PropertyNames.has_custom_fields, true);
        private readonly Grouping fieldsGrouping = new AllCustomFieldsGrouping((Grouping)null);

        public override string GroupingName
        {
            get { return Messages.CUSTOM_FIELDS; }
        }

        public override Icons GetGroupIcon(object obj)
        {
            return Icons.CustomField;
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            return fieldsQuery;
        }

        public override Grouping GetSubgrouping(object val)
        {
            return fieldsGrouping;
        }

        protected override Grouping Grouping { get { return fieldsGrouping; } }

        protected override QueryFilter Query { get { return fieldsQuery; } }
    }


    public class OrganizationViewFolders : OrganizationalView
    {
        private readonly QueryFilter foldersQuery = new NullQuery<Folder>(PropertyNames.folder, false);
        private readonly Grouping foldersGrouping = new FolderGrouping((Grouping)null);

        public override string GroupingName
        {
            get { return Messages.FOLDERS; }
        }

        public override Icons GetGroupIcon(object obj)
        {
            return Icons.Folder;
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            return foldersQuery;
        }

        public override Grouping GetSubgrouping(object val)
        {
            return foldersGrouping;
        }

        protected override Grouping Grouping { get { return foldersGrouping; } }

        protected override QueryFilter Query { get { return foldersQuery; } }

        protected override bool IncludeFolders { get { return true; } }
    }


    public class OrganizationViewVapps : OrganizationalView
    {
        private readonly QueryFilter vAppsQuery = new BooleanQuery(PropertyNames.in_any_appliance, true);
        private readonly Grouping vAppsGrouping = new XenModelObjectPropertyGrouping<VM_appliance>(PropertyNames.appliance, null);

        public override string GroupingName
        {
            get { return Messages.VM_APPLIANCES; }
        }

        public override Icons GetGroupIcon(object obj)
        {
            return Icons.VmAppliance;
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            return vAppsQuery;
        }

        public override Grouping GetSubgrouping(object val)
        {
            return vAppsGrouping;
        }

        protected override Grouping Grouping { get { return vAppsGrouping; } }

        protected override QueryFilter Query { get { return vAppsQuery; } }
    }
}
