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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.CustomFields;
using XenAdmin.XenSearch;
using XenAPI;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Model;
using XenCenterLib;

namespace XenAdmin.Controls.XenSearch
{
    public partial class QueryElement : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly List<QueryType> queryTypes = new List<QueryType>();
        private static readonly List<CustomFieldQueryTypeBase> customFields = new List<CustomFieldQueryTypeBase>();
        private static readonly QueryType DefaultQueryType;

        static QueryElement()
        {
            DefaultQueryType = new DummyQueryType(0, ObjectTypes.None);
            queryTypes.Add(DefaultQueryType);
            queryTypes.Add(new GroupQueryType(0, ObjectTypes.AllIncFolders, GroupQuery.GroupQueryType.And));
            queryTypes.Add(new GroupQueryType(0, ObjectTypes.AllIncFolders, GroupQuery.GroupQueryType.Or));
            queryTypes.Add(new GroupQueryType(0, ObjectTypes.AllIncFolders, GroupQuery.GroupQueryType.Nor));

            queryTypes.Add(new EnumPropertyQueryType<ObjectTypes>(1, ObjectTypes.None, PropertyNames.type));

			queryTypes.Add(new UuidQueryType(1, ObjectTypes.AllIncFolders & ~ObjectTypes.VDI | ObjectTypes.Appliance));  // but only actually shown for parent/child queries (see WantQueryType()). Too slow for VDIs and not very useful.
			queryTypes.Add(new StringPropertyQueryType(1, ObjectTypes.AllIncFolders | ObjectTypes.Appliance, PropertyNames.label));
			queryTypes.Add(new StringPropertyQueryType(1, ObjectTypes.AllExcFolders | ObjectTypes.Appliance, PropertyNames.description));
            queryTypes.Add(new UuidStringQueryType(1, ObjectTypes.AllExcFolders));
            queryTypes.Add(new TagQueryType(1, ObjectTypes.AllExcFolders));

            // Replaced by new ParentChildQueryTypes below
            queryTypes.Add(new XenModelObjectPropertyQueryType<Pool>(2, ObjectTypes.None, PropertyNames.pool));
            queryTypes.Add(new HostQueryType(2, ObjectTypes.None));

            queryTypes.Add(new LongQueryType(3, ObjectTypes.VM, Messages.MEMORY, PropertyNames.memory, Util.BINARY_MEGA, Messages.VAL_MEGB));
            queryTypes.Add(new IPAddressQueryType(3, ObjectTypes.VM | ObjectTypes.Server | ObjectTypes.LocalSR | ObjectTypes.RemoteSR, PropertyNames.ip_address));
            queryTypes.Add(new DatePropertyQueryType(3, ObjectTypes.VM, PropertyNames.start_time));
            queryTypes.Add(new EnumPropertyQueryType<vm_power_state>(3, ObjectTypes.VM, PropertyNames.power_state));
            queryTypes.Add(new EnumPropertyQueryType<VM.VirtualisationStatus>(3, ObjectTypes.VM, PropertyNames.virtualisation_status));
            queryTypes.Add(new ValuePropertyQueryType(3, ObjectTypes.VM, PropertyNames.os_name));
            queryTypes.Add(new EnumPropertyQueryType<VM.HA_Restart_Priority>(3, ObjectTypes.VM, PropertyNames.ha_restart_priority));
            queryTypes.Add(new BooleanQueryType(3, ObjectTypes.VM, PropertyNames.read_caching_enabled));
            queryTypes.Add(new BooleanQueryType(3, ObjectTypes.VM, PropertyNames.vendor_device_state));
            queryTypes.Add(new EnumPropertyQueryType<SR.SRTypes>(3, /*ObjectTypes.LocalSR | ObjectTypes.RemoteSR*/ ObjectTypes.None, PropertyNames.sr_type));

            queryTypes.Add(new LongQueryType(4, ObjectTypes.VDI, Messages.SIZE, PropertyNames.size, Util.BINARY_GIGA, Messages.VAL_GIGB));
            queryTypes.Add(new BooleanQueryType(4, ObjectTypes.LocalSR | ObjectTypes.RemoteSR | ObjectTypes.VDI, PropertyNames.shared));
            queryTypes.Add(new BooleanQueryType(4, ObjectTypes.Pool, PropertyNames.ha_enabled));
            queryTypes.Add(new BooleanQueryType(4, ObjectTypes.Pool, PropertyNames.isNotFullyUpgraded));
            queryTypes.Add(new NullQueryType<Pool>(4, ObjectTypes.Server | ObjectTypes.VM, PropertyNames.pool, false, Messages.IS_IN_A_POOL));
            queryTypes.Add(new NullQueryType<Pool>(4, ObjectTypes.Server | ObjectTypes.VM, PropertyNames.pool, true, Messages.IS_STANDALONE));

            // Replaced by new ParentChildQueryTypes below
            queryTypes.Add(new XenModelObjectListContainsQueryType<XenAPI.Network>(5, ObjectTypes.None, PropertyNames.networks));
            queryTypes.Add(new DiskQueryType(5, ObjectTypes.None, Messages.DISKS));

            // Any new recursive ones should be added to GetSearchForResourceSelectButton() too
            queryTypes.Add(new RecursiveXMOQueryType<Pool>(6, ObjectTypes.AllExcFolders, PropertyNames.pool, ObjectTypes.Pool));
            queryTypes.Add(new RecursiveXMOListQueryType<Host>(6, ObjectTypes.AllExcFolders & ~ObjectTypes.Pool, PropertyNames.host, ObjectTypes.Server));  // i.e. everything except Pool
            queryTypes.Add(new RecursiveXMOListQueryType<VM>(6, ObjectTypes.AllExcFolders, PropertyNames.vm, ObjectTypes.VM));
            queryTypes.Add(new RecursiveXMOListQueryType<XenAPI.Network>(6, ObjectTypes.Network | ObjectTypes.VM, PropertyNames.networks, ObjectTypes.Network));
            queryTypes.Add(new RecursiveXMOListQueryType<SR>(6, ObjectTypes.LocalSR | ObjectTypes.RemoteSR | ObjectTypes.VM | ObjectTypes.VDI,
                PropertyNames.storage, ObjectTypes.LocalSR | ObjectTypes.RemoteSR));
            queryTypes.Add(new RecursiveXMOListQueryType<VDI>(6, ObjectTypes.VM | ObjectTypes.VDI, PropertyNames.disks, ObjectTypes.VDI));

			queryTypes.Add(new RecursiveXMOQueryType<VM_appliance>(7, ObjectTypes.VM, PropertyNames.appliance, ObjectTypes.Appliance));
			queryTypes.Add(new BooleanQueryType(7, ObjectTypes.VM, PropertyNames.in_any_appliance));

            queryTypes.Add(new RecursiveXMOQueryType<Folder>(8, ObjectTypes.AllIncFolders, PropertyNames.folder, ObjectTypes.Folder));
            queryTypes.Add(new RecursiveXMOListQueryType<Folder>(8, ObjectTypes.AllIncFolders, PropertyNames.folders, ObjectTypes.Folder));
            queryTypes.Add(new NullQueryType<Folder>(8, ObjectTypes.AllExcFolders, PropertyNames.folder, true, Messages.NOT_IN_A_FOLDER));

            queryTypes.Add(new BooleanQueryType(9, ObjectTypes.AllExcFolders, PropertyNames.has_custom_fields));

            OtherConfigAndTagsWatcher.OtherConfigChanged += OtherConfigWatcher_OtherConfigChanged;
            CustomFieldsManager.CustomFieldsChanged += OtherConfigWatcher_OtherConfigChanged;
        }

        private static void OtherConfigWatcher_OtherConfigChanged()
        {
            List<CustomFieldDefinition> customFieldDefinitions = CustomFieldsManager.GetCustomFields();

            // Add new custom fields
            foreach (CustomFieldDefinition definition in customFieldDefinitions)
            {
                if (!CustomFieldExists(definition))
                {
                    if (definition.Type == CustomFieldDefinition.Types.String)
                        customFields.Add(new CustomFieldStringQueryType(8, ObjectTypes.AllExcFolders, definition));
                    else
                        customFields.Add(new CustomFieldDateQueryType(8, ObjectTypes.AllExcFolders, definition));
                }
            }

            // Remove old ones
            List<CustomFieldQueryTypeBase> toRemove = new List<CustomFieldQueryTypeBase>();
            foreach (CustomFieldQueryTypeBase customFieldQueryType in customFields)
            {
                if (!CustomFieldDefinitionExists(customFieldDefinitions, customFieldQueryType))
                    toRemove.Add(customFieldQueryType);
            }
            foreach (CustomFieldQueryTypeBase t in toRemove)
            {
                customFields.Remove(t);
            }
        }

        private static bool CustomFieldExists(CustomFieldDefinition definition)
        {
            return customFields.Exists(delegate(CustomFieldQueryTypeBase c) { return c.definition.Equals(definition); });
        }

        private static bool CustomFieldDefinitionExists(List<CustomFieldDefinition> l, CustomFieldQueryTypeBase queryType)
        {
            return l.Exists(delegate(CustomFieldDefinition d) { return d.Equals(queryType.definition); });
        }

        private readonly List<QueryElement> subQueryElements;
        private QueryElement parentQueryElement;
        private Searcher searcher;
        private QueryScope queryScope;  // Normally null, meaning use the scope from searcher (see WantQueryType). Set for the subquery of a parent-child query.
        private QueryFilter lastQueryFilter;

        public event Action QueryChanged;

        protected virtual void OnQueryChanged()
        {
            try
            {
                if (QueryChanged != null)
                    QueryChanged();
            }
            catch (Exception e)
            {
                log.Debug("Exception firing OnQueryChanged in QueryElement", e);
            }
        }

        public QueryElement()
            : this(null)
        {
        }

        public QueryElement(Searcher searcher, QueryScope queryScope, QueryElement parentQueryElement, QueryFilter query)
            : this(searcher, queryScope, parentQueryElement)
        {
            SelectQueryTypeFor(query);
        }

        public QueryElement(Searcher searcher, QueryScope queryScope, QueryElement parentQueryElement)
            : this(parentQueryElement)
        {
            this.Searcher = searcher;
            this.queryScope = queryScope;
        }

        private QueryElement(QueryElement parentQueryElement)  // only use internally because it doesn't set searcher
        {
            subQueryElements = new List<QueryElement>();
            this.parentQueryElement = parentQueryElement;

            InitializeComponent();

            queryTypeComboButton.BeforePopup += new EventHandler(queryTypeComboButton_BeforePopup);
            resourceSelectButton.BeforePopup += new EventHandler(resourceSelectButton_BeforePopup);

            queryTypeComboButton.SelectedItemChanged += OnQueryTypeComboButton_OnSelectedItemChanged;

            SelectDefaultQueryType();
            Setup();
        }

        private void OnQueryTypeComboButton_OnSelectedItemChanged(object sender, EventArgs e)
        {
            var selectedItem = queryTypeComboButton.SelectedItem;
            if (selectedItem.Tag is CustomFieldQueryTypeBase)
            {
                queryTypeComboButton.Text = selectedItem.ToString().Ellipsise(24);
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            CurrentQueryType = null;

            foreach (QueryElement subQueryElement in subQueryElements)
            {
                subQueryElement.Resize -= subQueryElement_Resize;
                subQueryElement.QueryChanged -= subQueryElement_QueryChanged;
                subQueryElement.searcher.SearchForChanged -= subQueryElement.searcher_SearchForChanged;
                subQueryElement.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        void queryTypeComboButton_BeforePopup(object sender, EventArgs e)
        {
            PopulateGroupTypesComboButton(false);
        }

        void resourceSelectButton_BeforePopup(object sender, EventArgs e)
        {
            PopulateResourceSelectButton();
        }

        private QueryType CurrentQueryType
        {
            get
            {
                ToolStripItem selectedItem = queryTypeComboButton.SelectedItem;
                if (selectedItem == null)
                    return null;

                return (QueryType)selectedItem.Tag;
            }

            set
            {
                if (CurrentQueryType != null)
                    CurrentQueryType.SomeThingChanged -= queryType_SomeThingChanged;

                if (value == null)
                    return;

                // We have to include all types in the drop-down temporarily, in case
                // we are deserializing from a type which is now forbidden (CA-25965).
                PopulateGroupTypesComboButton(true);
                queryTypeComboButton.SelectItem<QueryType>(delegate(QueryType queryType)
                    {
                        return queryType == value;
                    });
                PopulateGroupTypesComboButton(false);

                if (CurrentQueryType != null)
                {
                    queryTypeComboButton.Text = CurrentQueryType.ToString();
                    CurrentQueryType.SomeThingChanged += queryType_SomeThingChanged;
                    Setup();
                }
            }
        }

        private bool IsSubQueryElement
        {
            get
            {
                return parentQueryElement != null;
            }
        }

        private void PopulateGroupTypesComboButton(bool showAll)
        {
            int queryTypeGroup = -1;

            queryTypeComboButton.ClearItems();

            List<QueryType> allQueryTypes = new List<QueryType>(queryTypes);
            foreach (QueryType qt in customFields)
                allQueryTypes.Add(qt);  // can't use AddRange() because customFields is List<CustomFieldQueryType> not List<QueryType>!
            foreach (QueryType queryType in allQueryTypes)
            {
                if (!showAll && !WantQueryType(queryType))  // only add relevant items to the drop-down
                    continue;

                if (queryTypeGroup != -1 && queryType.Group != queryTypeGroup)
                {
                    queryTypeComboButton.AddItem(new ToolStripSeparator());
                }
                queryTypeGroup = queryType.Group;

                ToolStripMenuItem queryTypeMenuItem = new ToolStripMenuItem();
                queryTypeMenuItem.Text = queryType.ToString();
                queryTypeMenuItem.Tag = queryType;

                queryTypeComboButton.AddItem(queryTypeMenuItem);
            }
        }

        public void SelectDefaultQueryType()
        {
            CurrentQueryType = DefaultQueryType;
        }

        void queryType_SomeThingChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, Setup);
        }

        private void RefreshSubQueryElements()
        {
            if (CurrentQueryType == null)
                return;

            QueryType.Category category = CurrentQueryType.GetCategory;

            int topOffset = queryTypeComboButton.Height + 2;

            foreach (QueryElement subQueryElement in subQueryElements)
            {
                subQueryElement.Resize -= new EventHandler(subQueryElement_Resize);
                subQueryElement.Resize += new EventHandler(subQueryElement_Resize);

                subQueryElement.QueryChanged -= subQueryElement_QueryChanged;
                subQueryElement.QueryChanged += subQueryElement_QueryChanged;

                if (category == QueryType.Category.ParentChild)
                {
                    int indent = matchTypeComboButton.Left - queryTypeComboButton.Left;  // need to push everything in the subQuery this far right
                    subQueryElement.Left = indent;
                    subQueryElement.Width = this.Width - indent;
                    subQueryElement.Top = 0;
                }
                else  // must be Category.Group
                {
                    subQueryElement.Left = 30;
                    subQueryElement.Width = this.Width - 30;
                    subQueryElement.Top = topOffset;
                    topOffset += subQueryElement.Height;
                }

                if (!Controls.Contains(subQueryElement))
                    Controls.Add(subQueryElement);
            }

            // This line causes OnResize(), which calls this function again recursively
            // (with the same "this"). I haven't messed with it yet because although this
            // function is called far more often than necessary, the number of query elements
            // is typically small, so it's not too painful. SRET 2009-01-09.
            this.Height = topOffset;
        }

        void subQueryElement_QueryChanged()
        {
            OnQueryChanged();
        }

        private void ClearSubQueryElements()
        {
            foreach (QueryElement subQueryElement in subQueryElements.ToArray())
                RemoveSubQueryElement(subQueryElement);
        }

        void subQueryElement_Resize(object sender, EventArgs e)
        {
            RefreshSubQueryElements();

            if (parentQueryElement == null)
                Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            RefreshSubQueryElements();

            // The following line shouldn't be necessary, but the right-anchoring doesn't seem to work
            // in one situation: when you have a parent-child query, and you change it into a group
            // query, so the parent-child query becomes the first child of the group, then the [-] button
            // appears in the wrong place. SRET 2009-01-09.
            removeButton.Left = this.Width - 25;

            base.OnResize(e);
        }

        private bool populatingComboButton = false;

        /// <summary>
        /// Populate a combo box, trying to preserve the selected index
        /// </summary>
        /// <param name="comboButton"></param>
        /// <param name="values"></param>
        /// <param name="imageDelegate"></param>
        private void PopulateComboButton(DropDownComboButton comboButton, Object[] values, ImageDelegate<Object> imageDelegate)
        {
            populatingComboButton = true;
            try
            {
                bool selected = false;

                ToolStripItem selectedItem = comboButton.SelectedItem;
                comboButton.ClearItems();
                ToolStripMenuItem firstMenuItem = null;

                foreach (Object value in values)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem();

                    String text = value.ToString();
                    menuItem.Text = text.EscapeAmpersands().Ellipsise(100);
                    menuItem.Tag = value;

                    if (imageDelegate != null)
                        menuItem.Image = Images.GetImage16For(imageDelegate(value));

                    comboButton.AddItem(menuItem);

                    if (firstMenuItem == null)
                        firstMenuItem = menuItem;

                    if (selectedItem != null && value.Equals(selectedItem.Tag))
                    {
                        selected = true;
                        comboButton.SelectedItem = menuItem;
                    }
                }

                if (!selected)
                    comboButton.SelectedItem = firstMenuItem;
            }
            finally
            {
                populatingComboButton = false;
            }
        }

        private void PopulateResourceSelectButton()
        {
            Search search = GetSearchForResourceSelectButton();
            if (search == null)
                resourceSelectButton.ClearItems();  // shouldn't happen
            else
                resourceSelectButton.Populate(search);
        }

        private Search GetSearchForResourceSelectButton()
        {
            if (queryScope == null)
                return null;

            Query query = new Query(queryScope, null);
            Sort sort = new Sort("name", true);
            Sort[] sorts = { sort };

            Grouping grouping = null;
            switch (queryScope.ObjectTypes)
            {
                // The same list of recursive types as defined in static QueryElement()
                case ObjectTypes.Pool:
                    grouping = null;
                    break;
				case ObjectTypes.Appliance:
                case ObjectTypes.Server:
                    grouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, null);
                    break;
                case ObjectTypes.VM:
                case ObjectTypes.Network:
                case ObjectTypes.LocalSR | ObjectTypes.RemoteSR:
                    {
                        Grouping serverGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, null);
                        grouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, serverGrouping);
                        break;
                    }
                case ObjectTypes.VDI:
                    {
                        Grouping srGrouping = new XenModelObjectPropertyGrouping<SR>(PropertyNames.storage, null);
                        Grouping serverGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, srGrouping);
                        grouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, serverGrouping);
                        break;
                    }
                case ObjectTypes.Folder:
                    grouping = new FolderGrouping((Grouping)null);
                    break;
            }

            return new Search(query, grouping, "", "", null, sorts);
        }

        private void Setup()
        {
            Setup(false);
        }

        private void Setup(bool queryTypeDropDownChanged)
        {
            if (populatingComboButton)
                return;

            // no remove button for "Select a filter",
            // nor for parent/child subqueries (where the main query already has a remove button on the same row)
            removeButton.Visible = !(CurrentQueryType is DummyQueryType) &&
                !(IsSubQueryElement && parentQueryElement.CurrentQueryType.GetCategory == QueryType.Category.ParentChild);

            matchTypeComboButton.Visible = CurrentQueryType.ShowMatchTypeComboButton;
            if (CurrentQueryType.ShowMatchTypeComboButton)
                PopulateComboButton(matchTypeComboButton, CurrentQueryType.MatchTypeComboButtonEntries, null);

            QueryType.Category category = CurrentQueryType.GetCategory;
            if (category == QueryType.Category.Group)
            {
                // if we are changing from non-boolean to boolean, use the old query as the first child
                // of the new boolean query (CA-25127)
                if (queryTypeDropDownChanged && !(lastQueryFilter is GroupQuery))
                {
                    ClearSubQueryElements();
                    subQueryElements.Add(new QueryElement(this.Searcher, null, this, lastQueryFilter));
                }
            }
            else if (category == QueryType.Category.ParentChild && queryTypeDropDownChanged)
                ClearSubQueryElements();

            if (category != QueryType.Category.Single)
            {
                // if we don't have a "Select a filter...", then add one now
                if (!HasDummySubQuery())
                    AddDummySubQuery(CurrentQueryType.SubQueryScope);
            }
            else
            {
                ClearSubQueryElements();
            }

            RefreshSubQueryElements();

            // also if our parent doesn't have a "Select a filter...", then add one now
            // (This only occurs when changing from "Select a filter..." to something else).
            if (queryTypeDropDownChanged && lastQueryFilter is DummyQuery &&
                IsSubQueryElement && parentQueryElement.CurrentQueryType is GroupQueryType &&
                parentQueryElement.subQueryElements.Contains(this) &&  // this excludes the case where the subquery is still being initialized
                !parentQueryElement.HasDummySubQuery())
            {
                parentQueryElement.AddDummySubQuery(null);
                parentQueryElement.RefreshSubQueryElements();
            }

            Setup2();
        }

        private void Setup2()
        {
            textBox.Visible = CurrentQueryType.ShowTextBox(this);
            dateTimePicker.Visible = CurrentQueryType.ShowDateTimePicker(this);
            unitsLabel.Visible = numericUpDown.Visible = CurrentQueryType.ShowNumericUpDown(this);
            unitsLabel.Text = CurrentQueryType.Units(this);

            ComboButton.Visible = CurrentQueryType.ShowComboButton(this);
            if (CurrentQueryType.ShowComboButton(this))
                PopulateComboButton(ComboButton, CurrentQueryType.GetComboButtonEntries(this), null);

            resourceSelectButton.Visible = CurrentQueryType.ShowResourceSelectButton(this);
            if (CurrentQueryType.ShowResourceSelectButton(this))
                PopulateResourceSelectButton();

            OnQueryChanged();
        }

        QueryFilter[] GetSubQueries()
        {
            List<QueryFilter> subQueries = new List<QueryFilter>();

            foreach (QueryElement subQueryElement in subQueryElements)
            {
                QueryFilter query = subQueryElement.QueryFilter;
                if (query != null)
                    subQueries.Add(query);
            }

            return subQueries.ToArray();
        }

        private bool HasDummySubQuery()
        {
            foreach (QueryElement subQueryElement in subQueryElements)
            {
                if (subQueryElement.CurrentQueryType is DummyQueryType)
                    return true;
            }

            return false;
        }

        private void AddDummySubQuery(QueryScope queryScope)
        {
            subQueryElements.Add(new QueryElement(this.Searcher, queryScope, this));
        }

        public Searcher Searcher
        {
            get { return searcher; }
            set
            {
                searcher = value;
                if (searcher != null)
                    searcher.SearchForChanged += searcher_SearchForChanged;
            }
        }

        public QueryFilter QueryFilter
        {
            get
            {
                return (CurrentQueryType == null ? null : CurrentQueryType.GetQuery(this));
            }
            set
            {
                SelectQueryTypeFor(value);
            }
        }

        private void SelectQueryTypeFor(QueryFilter query)
        {
            if (query == null)
            {
                SelectDefaultQueryType();
                return;
            }

            if (CurrentQueryType.ForQuery(query))
            {
                Setup2();
                CurrentQueryType.FromQuery(query, this);
                return;
            }

            foreach (QueryType queryType in queryTypes)
            {
                if (!queryType.ForQuery(query))
                    continue;

                CurrentQueryType = queryType;
                queryType.FromQuery(query, this);
                return;
            }

            foreach (QueryType queryType in customFields)
            {
                if (!queryType.ForQuery(query))
                    continue;

                CurrentQueryType = queryType;
                queryType.FromQuery(query, this);
                return;
            }

            SelectDefaultQueryType();
        }

        // Do we want this query type, based on the search-for?
        private bool WantQueryType(QueryType qt)
        {
            QueryScope scope;
            bool recursive;  // is this element the subquery of a parent-child query?

            if (queryScope != null)
            {
                scope = queryScope;
                recursive = true;
            }
            else
            {
                scope = (searcher == null ? null : searcher.QueryScope);
                recursive = false;
            }

            if (scope == null)
                return true;

            // If this is the subquery of a parent-child query, it can't have children
            if (recursive && qt.GetCategory != QueryType.Category.Single)
                return false;

            // Conversely, the UuidQueryType only shows up as the subquery of a parent-child query
            // (no point in a search for one object)
            if (!recursive && qt is UuidQueryType)
                return false;

            // If this is itself a parent-child query, we only allow it if the global scope
            // includes something other than this element's scope. For example, it's permissible
            // to search for Servers and VMs with a constraint on the Servers, but it's not
            // permissible to search for just Servers and put a constraint on the Servers.
            // Folders are an exception because the search is then for the parent folder, not
            // the folder itself.
            if (scope.WantSubsetOf(qt.SubQueryScope) &&
                !qt.SubQueryScope.Equals(ObjectTypes.Folder))
                return false;

            return scope.WantSubsetOf(qt.AppliesTo);
        }

        private void RemoveUnwantedFilters()
        {
            QueryType qt = CurrentQueryType;
            bool wanted = WantQueryType(qt);
            if (wanted && qt.GetCategory == QueryType.Category.Group)
            {
                // For group queries, remove subqueries recursively
                foreach (QueryElement subQueryElement in subQueryElements.ToArray())
                    subQueryElement.RemoveUnwantedFilters();

                // This element is now unwanted if we've deleted all its children
                if (subQueryElements.Count == 0)
                    wanted = false;

                // If this is an AND or OR (not a NONE), and there is only one
                // child left, remove the conjunction by promoting the child.
                if (subQueryElements.Count == 1)
                {
                    GroupQueryType gqt = qt as GroupQueryType;
                    if (gqt != null && (gqt.Type == GroupQuery.GroupQueryType.And ||
                        gqt.Type == GroupQuery.GroupQueryType.Or))
                    {
                        this.QueryFilter = subQueryElements[0].QueryFilter;
                        RefreshSubQueryElements();
                    }
                }
            }

            if (!wanted)
            {
                if (IsSubQueryElement)
                {
                    parentQueryElement.RemoveSubQueryElement(this);
                    parentQueryElement.RefreshSubQueryElements();
                }
                else
                    SelectDefaultQueryType();
            }
        }

        private void searcher_SearchForChanged()
        {
            // We only run this for the outermost query because subqueries
            // are removed by their parent (RemoveUnwantedFilters() is recursive).
            if (!IsSubQueryElement)
            {
                RemoveUnwantedFilters();
                Setup();
                OnQueryChanged();
            }
        }

        private void queryTypeComboButton_BeforeItemSelected(object sender, EventArgs e)
        {
            lastQueryFilter = this.QueryFilter;

            if (CurrentQueryType != null)
            {
                CurrentQueryType.SomeThingChanged -= queryType_SomeThingChanged;
            }
        }

        private void queryTypeComboButton_ItemSelected(object sender, EventArgs e)
        {
            if (CurrentQueryType != null)
            {
                CurrentQueryType.SomeThingChanged += queryType_SomeThingChanged;
            }
        }

        private void queryTypeComboButton_SelectedItemChanged(object sender, EventArgs e)
        {
            Setup(true);
        }

        private void matchTypeComboButton_SelectedItemChanged(object sender, EventArgs e)
        {
            Setup2();
        }

        private void ComboButton_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnQueryChanged();
        }

        private void resourceSelectButton_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnQueryChanged();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            OnQueryChanged();
        }

        private void numericUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            OnQueryChanged();
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            OnQueryChanged();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            OnQueryChanged();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (IsSubQueryElement)
            {
                parentQueryElement.RemoveSubQueryElement(this);
                parentQueryElement.RefreshSubQueryElements();
                parentQueryElement.OnQueryChanged();
            }
            else
            {
                SelectDefaultQueryType();
                OnQueryChanged();
            }
        }

        // Remove subquery element. Caller needs to call parent.RefreshSubQueryElements() and
        // parent.OnQueryChanged() -- or parent.Setup() which calls both -- afterwards.
        private void RemoveSubQueryElement(QueryElement subQueryElement)
        {
            Controls.Remove(subQueryElement);
            subQueryElements.Remove(subQueryElement);

            subQueryElement.Resize -= subQueryElement_Resize;
            subQueryElement.QueryChanged -= subQueryElement_QueryChanged;
            subQueryElement.searcher.SearchForChanged -= subQueryElement.searcher_SearchForChanged;

            subQueryElement.Dispose();
        }

        #region Nested classes

        internal abstract class QueryType
        {
            private readonly int group = 0;
            private readonly ObjectTypes appliesTo;

            protected QueryType(int group, ObjectTypes appliesTo)
            {
                this.group = group;
                this.appliesTo = appliesTo;
            }

            /// <summary>
            /// If this query type is correct for this query, return true;
            /// Call FromQuery to populate the correct combo boxes etc
            /// </summary>
            /// <param name="query"></param>
            /// <returns></returns>
            public abstract bool ForQuery(QueryFilter query);

            public abstract void FromQuery(QueryFilter query, QueryElement queryElement);

            /// <summary>
            /// May return null if this query element is not ready yet
            /// </summary>
            public abstract QueryFilter GetQuery(QueryElement queryElement);

            public enum Category
            {
                Single,  // match the object itself
                ParentChild,  // match ancestor or descendant object
                Group    // a group query
            }
            public virtual Category GetCategory { get { return Category.Single; } }
            public virtual QueryScope SubQueryScope { get { return null; } }  // QueryScope for subquery (if category == ParentChild)

            public virtual bool ShowMatchTypeComboButton { get { return false; } }
            public virtual Object[] MatchTypeComboButtonEntries { get { return null; } }

            public virtual bool ShowTextBox(QueryElement queryElement) { return false; }
            public virtual bool ShowDateTimePicker(QueryElement queryElement) { return false; }
            public virtual bool ShowComboButton(QueryElement queryElement) { return false; }
            public virtual bool ShowNumericUpDown(QueryElement queryElement) { return false; }
            public virtual bool ShowResourceSelectButton(QueryElement queryElement) { return false; }

            public virtual String Units(QueryElement queryElement) { return String.Empty; }
            public virtual Object[] GetComboButtonEntries(QueryElement queryElement) { return null; }

            public event EventHandler SomeThingChanged;

            public int Group { get { return group; } }

            public ObjectTypes AppliesTo
            {
                get { return appliesTo; }
            }

            /// <summary>
            /// "Something changed" means that some external thing changed, usually in the cache,
            /// and we need to repopulate the drop-downs for this QueryElement as a result.
            /// </summary>
            protected void OnSomeThingChanged()
            {
                if (SomeThingChanged != null)
                    SomeThingChanged(this, new EventArgs());
            }
        }

        // A dummy query type which doesn't contribute to the search
        internal class DummyQueryType : QueryType
        {
            internal DummyQueryType(int group, ObjectTypes appliesTo)
                : base(group, appliesTo)
            { }

            public override String ToString()
            {
                return Messages.SELECT_A_FILTER;
            }

            public override bool ForQuery(QueryFilter query)
            {
                return (query is DummyQuery);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            { }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                return new DummyQuery();
            }
        }

        internal class GroupQueryType : QueryType
        {
            private readonly GroupQuery.GroupQueryType type;

            internal GroupQueryType(int group, ObjectTypes appliesTo, GroupQuery.GroupQueryType type)
                : base(group, appliesTo)
            {
                this.type = type;
            }

            public GroupQuery.GroupQueryType Type
            {
                get { return type; }
            }

            public override String ToString()
            {
                switch (type)
                {
                    case GroupQuery.GroupQueryType.And:
                        return Messages.ALL_OF;

                    case GroupQuery.GroupQueryType.Or:
                        return Messages.ANY_OF;

                    case GroupQuery.GroupQueryType.Nor:
                        return Messages.NONE_OF;

                    default:
                        return "";
                }
            }

            public override Category GetCategory { get { return Category.Group; } }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                return new GroupQuery(queryElement.GetSubQueries(), type);
            }

            public override bool ForQuery(QueryFilter query)
            {
                GroupQuery groupQuery = query as GroupQuery;
                if (groupQuery == null)
                    return false;

                if (groupQuery.type != type)
                    return false;

                return true;
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                GroupQuery groupQuery = query as GroupQuery;

                for (int i = 0; i < groupQuery.subqueries.Length; i++)
                {
                    QueryFilter subquery = groupQuery.subqueries[i];

                    if (queryElement.subQueryElements.Count > i)
                        queryElement.subQueryElements[i].SelectQueryTypeFor(subquery);
                    else
                        queryElement.subQueryElements.Add(new QueryElement(queryElement.Searcher, null, queryElement, subquery));
                }

                while (queryElement.subQueryElements.Count > groupQuery.subqueries.Length)
                    queryElement.RemoveSubQueryElement(
                        queryElement.subQueryElements[queryElement.subQueryElements.Count - 1]);

                queryElement.Setup();
            }
        }

        internal abstract class PropertyQueryType<T> : QueryType
        {
            protected readonly PropertyNames property;
            protected readonly PropertyAccessor propertyAccessor;
            protected readonly String i18n;

            protected PropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property, String i18n)
                : base(group, appliesTo)
            {
                this.property = property;
                this.propertyAccessor = PropertyAccessors.Get(property);
                this.i18n = (i18n == null ? PropertyAccessors.PropertyNames_i18n[property] : i18n);
            }

            protected PropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : this(group, appliesTo, property, null)
            {
            }

            public override String ToString()
            {
                return i18n;
            }

            // Note: This function only tests the values of T and property.
            // This is good enough for most cases and avoids each derived class
            // having to define its own ForQuery. But in some cases there are
            // clashes (particularly when T is XMO<U> or List<XMO<u>>) and in
            // that case we do have to override this function in both derived
            // classes. Derived classes of MatchType may lead to clashes too.
            public override bool ForQuery(QueryFilter query)
            {
                PropertyQuery<T> propertyQuery = query as PropertyQuery<T>;
                if (propertyQuery == null)
                    return false;

                return (propertyQuery.property == this.property);
            }
        }

        internal class StringPropertyQueryType : PropertyQueryType<String>
        {
            internal StringPropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override bool ShowTextBox(QueryElement queryElement) { return true; }

            public class ExtraComboEntry
            {
                public StringPropertyQuery.PropertyQueryType type;
                public ExtraComboEntry(StringPropertyQuery.PropertyQueryType type)
                {
                    this.type = type;
                }

                public override string ToString()
                {
                    switch (type)
                    {
                        case StringPropertyQuery.PropertyQueryType.contains:
                            return Messages.CONTAINS;

                        case StringPropertyQuery.PropertyQueryType.exactmatch:
                            return Messages.EXACT_MATCH;

                        case StringPropertyQuery.PropertyQueryType.endswith:
                            return Messages.ENDS_WITH;

                        case StringPropertyQuery.PropertyQueryType.startswith:
                            return Messages.STARTS_WITH;

                        case StringPropertyQuery.PropertyQueryType.notcontain:
                            return Messages.NOT_CONTAINS;

                        default:
                            return "";
                    }
                }

                public override bool Equals(object obj)
                {
                    ExtraComboEntry other = obj as ExtraComboEntry;
                    if (other == null)
                        return false;

                    return type == other.type;
                }

                public override int GetHashCode()
                {
                    return base.GetHashCode();
                }
            }

            public override object[] MatchTypeComboButtonEntries
            {
                get { return MatchTypeEntries; }
            }

            public static object[] MatchTypeEntries
            {
                get
                {
                    return new ExtraComboEntry[] { 
                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.contains),
                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.notcontain),

                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.startswith),
                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.endswith),

                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.exactmatch)
                    };
                }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                ExtraComboEntry extraComboEntry = (ExtraComboEntry)queryElement.matchTypeComboButton.SelectedItem.Tag;

                return new StringPropertyQuery(property, queryElement.textBox.Text,
                    extraComboEntry.type, false);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                StringPropertyQuery stringQuery = query as StringPropertyQuery;

                queryElement.matchTypeComboButton.SelectItem<ExtraComboEntry>(delegate(ExtraComboEntry entry)
                {
                    return entry.type == stringQuery.type;
                });

                queryElement.textBox.Text = stringQuery.query;
            }
        }

        // The object "Is" query, selected from a list of objects.
        // Internally implemented using UUIDs.
        internal class UuidQueryType : PropertyQueryType<String>
        {
            internal UuidQueryType(int group, ObjectTypes appliesTo)
                : base(group, appliesTo, PropertyNames.uuid, Messages.UUID_SEARCH)
            {
            }

            public override bool ShowResourceSelectButton(QueryElement queryElement) { return true; }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                ToolStripItem item = queryElement.resourceSelectButton.SelectedItem;
                IXenObject ixmo = (item == null ? null : item.Tag as IXenObject);
                String uuid = (string)propertyAccessor(ixmo);
                return new StringPropertyQuery(property, uuid, StringPropertyQuery.PropertyQueryType.exactmatch, true);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                StringPropertyQuery stringQuery = query as StringPropertyQuery;
                queryElement.resourceSelectButton.SelectItem<IXenObject>(delegate(IXenObject entry)
                    {
                        return ((string)propertyAccessor(entry) == stringQuery.query);
                    });
            }
        }

        // Search for UUID as a string
        internal class UuidStringQueryType: StringPropertyQueryType
        {
            internal UuidStringQueryType(int group, ObjectTypes appliesTo)
                : base(group, appliesTo, PropertyNames.uuid)
            { }

            public override object[] MatchTypeComboButtonEntries
            {
                get {
                    return new ExtraComboEntry[] { 
                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.startswith),
                        new ExtraComboEntry(StringPropertyQuery.PropertyQueryType.exactmatch)
                    };
                }
            }
        }

        internal class IPAddressQueryType : PropertyQueryType<List<ComparableAddress>>
        {
            internal IPAddressQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override bool ShowTextBox(QueryElement queryElement) { return true; }

            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.IS };
                }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                ComparableAddress address;
                if (!ComparableAddress.TryParse(queryElement.textBox.Text, true, true, out address))
                    return null;

                return new IPAddressQuery(property, address);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                IPAddressQuery ipAddressQuery = query as IPAddressQuery;

                queryElement.textBox.Text = ipAddressQuery.address.ToString();
            }
        }

        internal class DatePropertyQueryType : PropertyQueryType<DateTime>
        {
            internal DatePropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override bool ShowDateTimePicker(QueryElement queryElement)
            {
                return ShouldShowDateTimePicker(queryElement);
            }

            public static bool ShouldShowDateTimePicker(QueryElement queryElement)
            {
                ExtraComboEntry extraComboEntry = (ExtraComboEntry)queryElement.matchTypeComboButton.SelectedItem.Tag;

                switch (extraComboEntry.type)
                {
                    case DatePropertyQuery.PropertyQueryType.before:
                    case DatePropertyQuery.PropertyQueryType.after:
                    case DatePropertyQuery.PropertyQueryType.exact:
                        return true;

                    default:
                        return false;
                }
            }

            internal class ExtraComboEntry
            {
                public DatePropertyQuery.PropertyQueryType type;
                public ExtraComboEntry(DatePropertyQuery.PropertyQueryType type)
                {
                    this.type = type;
                }

                public override string ToString()
                {
                    switch (type)
                    {
                        case DatePropertyQuery.PropertyQueryType.after:
                            return Messages.AFTER;

                        case DatePropertyQuery.PropertyQueryType.before:
                            return Messages.BEFORE;

                        case DatePropertyQuery.PropertyQueryType.exact:
                            return Messages.EXACT_MATCH;

                        case DatePropertyQuery.PropertyQueryType.lastweek:
                            return Messages.LAST_WEEK;

                        case DatePropertyQuery.PropertyQueryType.thisweek:
                            return Messages.THIS_WEEK;

                        case DatePropertyQuery.PropertyQueryType.today:
                            return Messages.TODAY;

                        case DatePropertyQuery.PropertyQueryType.yesterday:
                            return Messages.YESTERDAY;

                        default:
                            return "";
                    }
                }

                public override bool Equals(object obj)
                {
                    ExtraComboEntry other = obj as ExtraComboEntry;
                    if (other == null)
                        return false;

                    return type == other.type;
                }

                public override int GetHashCode()
                {
                    return base.GetHashCode();
                }
            }

            public override object[] MatchTypeComboButtonEntries
            {
                get { return MatchTypeEntries; }
            }

            public static object[] MatchTypeEntries
            {
                get
                {
                    return new ExtraComboEntry[] { 
                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.today),
                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.yesterday),

                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.thisweek),
                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.lastweek),

                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.before),
                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.after),

                        new ExtraComboEntry(DatePropertyQuery.PropertyQueryType.exact)
                    };
                }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                ExtraComboEntry extraComboEntry = (ExtraComboEntry)queryElement.matchTypeComboButton.SelectedItem.Tag;

                return new DatePropertyQuery(property, queryElement.dateTimePicker.Value, extraComboEntry.type);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                DatePropertyQuery dateQuery = query as DatePropertyQuery;

                queryElement.matchTypeComboButton.SelectItem<ExtraComboEntry>(delegate(ExtraComboEntry entry)
                {
                    return entry.type == dateQuery.type;
                });

                queryElement.dateTimePicker.Value = dateQuery.query;
            }
        }

        internal class EnumPropertyQueryType<T> : PropertyQueryType<T>
        {
            private readonly Dictionary<String, T> i18ns;

            internal EnumPropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
                this.i18ns = (Dictionary<String, T>)PropertyAccessors.Geti18nFor(property);
                System.Diagnostics.Trace.Assert(typeof(T).IsEnum);
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.IS, Messages.IS_NOT };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement) { return true; }
            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                String[] enumStrings = new String[i18ns.Count];
                i18ns.Keys.CopyTo(enumStrings, 0);

                return enumStrings;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                bool equals = queryElement.matchTypeComboButton.SelectedItem.Tag.Equals(Messages.IS);

                T query = i18ns[(String)queryElement.ComboButton.SelectedItem.Tag];

                return new EnumPropertyQuery<T>(property, query, equals);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                EnumPropertyQuery<T> enumQuery = query as EnumPropertyQuery<T>;
                if (enumQuery == null)  // This should never happen, but is the
                    return;             // only cause of CA-18166 reopening

                queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                {
                    return entry == (enumQuery.equals ? Messages.IS : Messages.IS_NOT);
                });

                queryElement.ComboButton.SelectItem<String>(delegate(String entry)
                {
                    return Enum.Equals(i18ns[entry], enumQuery.query);
                });
            }
        }

        // This class is currently only used for the OS, which is only available for VMs.
        // If this changes, we may need to alter the loop in populateCollectedValues() to
        // iterate over other objects. SRET 2010-11-01.
        internal class ValuePropertyQueryType : PropertyQueryType<String>
        {
            private readonly Dictionary<String, Object> collectedValues;

            public ValuePropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
                collectedValues = new Dictionary<String, Object>();

                ConnectionsManager.XenConnections.CollectionChanged += CollectionChanged;

                populateCollectedValues();
            }

            // This object is static for the life of the GUI, so we never have to unhook these events.
            private void populateCollectedValues()
            {
                collectedValues.Clear();

                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.RegisterBatchCollectionChanged<VM_guest_metrics>(CollectionChanged);

                    // Only iterate over VMs: see comments at the top of this class.
                    foreach (VM vm in connection.Cache.VMs)
                    {
                        String value = propertyAccessor(vm) as String;
                        if (value == null)
                            continue;

                        if (collectedValues.ContainsKey(value))
                            continue;

                        collectedValues[value] = null;
                    }
                }

                OnSomeThingChanged();
            }

            void CollectionChanged(object sender, EventArgs e)
            {
                Program.BeginInvoke(Program.MainWindow,populateCollectedValues);
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.IS, Messages.IS_NOT };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement) { return true; }
            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                String[] entries = new String[collectedValues.Keys.Count];
                collectedValues.Keys.CopyTo(entries, 0);

                // Sort the list in alphabetical order, but put "Unknown" at the bottom
                Array.Sort(entries, (a, b) =>
                    {
                        if (a == Messages.UNKNOWN)
                            return (b == Messages.UNKNOWN ? 0 : 1);
                        else if (b == Messages.UNKNOWN)
                            return -1;
                        else
                            return a.CompareTo(b);
                    }
                );

                return entries;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                if (queryElement.ComboButton.SelectedItem == null)
                    return null;

                bool equals = queryElement.matchTypeComboButton.SelectedItem.Tag.Equals(Messages.IS);
                String query = queryElement.ComboButton.SelectedItem.Tag as String;

                return new ValuePropertyQuery(property, query, equals);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                ValuePropertyQuery valueQuery = query as ValuePropertyQuery;

                queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                {
                    return entry == (valueQuery.equals ? Messages.IS : Messages.IS_NOT);
                });

                queryElement.ComboButton.SelectItem<String>(delegate(String entry)
                {
                    return entry.Equals(valueQuery.query);
                });
            }
        }

        internal abstract class RecursiveQueryType<T, O, Q> : PropertyQueryType<T>
            where O : XenObject<O>
            where Q : RecursivePropertyQuery<T>
        {
            QueryScope subQueryScope;

            public RecursiveQueryType(int group, ObjectTypes appliesTo, PropertyNames property, ObjectTypes subQueryScope)
                : base(group, appliesTo, property)
            {
                this.subQueryScope = new QueryScope(subQueryScope);
                ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
                XenConnections_CollectionChanged(null, null);
            }

            public override Category GetCategory
            {
                get
                {
                    return Category.ParentChild;
                }
            }

            public override QueryScope SubQueryScope
            {
                get
                {
                    return subQueryScope;
                }
            }

            void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.RegisterBatchCollectionChanged<O>(dict_BatchCollectionChanged);
                }
            }

            void dict_BatchCollectionChanged(object sender, EventArgs e)
            {
                OnSomeThingChanged();
            }

            protected abstract RecursivePropertyQuery<T> newQuery(PropertyNames property, QueryFilter qf);

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                QueryFilter qf;
                QueryFilter[] subQueries = queryElement.GetSubQueries();
                if (subQueries == null || subQueries.Length == 0)  // shouldn't happen, but let's be safe
                    qf = new DummyQuery();
                else
                    qf = subQueries[0];

                // Special case for CA-30595: interpret "parent folder is (blank)" as "parent folder is /".
                if (property == PropertyNames.folder && qf is StringPropertyQuery &&
                    (qf as StringPropertyQuery).property == PropertyNames.uuid && (qf as StringPropertyQuery).query == null)
                {
                    qf = new StringPropertyQuery(PropertyNames.uuid, "/", StringPropertyQuery.PropertyQueryType.exactmatch, true);
                }

                return newQuery(property, qf);  // this should just be new Q(property, qf) but that's illegal syntax: can only use default constructor because there's no way to specify that all derived classes must implement new(property, qf)
            }

            public override bool ForQuery(QueryFilter query)
            {
                RecursivePropertyQuery<T> recursivePropertyQuery = query as RecursivePropertyQuery<T>;
                return (recursivePropertyQuery != null &&
                    recursivePropertyQuery.property == property);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                Q recursiveQuery = query as Q;
                QueryFilter subQuery = recursiveQuery.subQuery;
                queryElement.ClearSubQueryElements();
                queryElement.subQueryElements.Add(new QueryElement(queryElement.Searcher, SubQueryScope, queryElement, subQuery));
                queryElement.Setup();
            }
        }

        internal class RecursiveXMOQueryType<T> : RecursiveQueryType<T, T, RecursiveXMOPropertyQuery<T>> where T : XenObject<T>
        {
            public RecursiveXMOQueryType(int group, ObjectTypes appliesTo, PropertyNames property, ObjectTypes subQueryScope)
                : base(group, appliesTo, property, subQueryScope)
            { }

            protected override RecursivePropertyQuery<T> newQuery(PropertyNames property, QueryFilter qf)
            {
                return new RecursiveXMOPropertyQuery<T>(property, qf);
            }
        }

        internal class RecursiveXMOListQueryType<T> : RecursiveQueryType<List<T>, T, RecursiveXMOListPropertyQuery<T>> where T : XenObject<T>
        {
            public RecursiveXMOListQueryType(int group, ObjectTypes appliesTo, PropertyNames property, ObjectTypes subQueryScope)
                : base(group, appliesTo, property, subQueryScope)
            { }

            protected override RecursivePropertyQuery<List<T>> newQuery(PropertyNames property, QueryFilter qf)
            {
                return new RecursiveXMOListPropertyQuery<T>(property, qf);
            }
        }

        // NOT USED FOR NEW QUERIES
        internal class XenModelObjectPropertyQueryType<T> : PropertyQueryType<T> where T : XenObject<T>
        {
            public XenModelObjectPropertyQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
                ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
                XenConnections_CollectionChanged(null, null);
            }

            void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.RegisterBatchCollectionChanged<T>(dict_BatchCollectionChanged);
                }
            }

            void dict_BatchCollectionChanged(object sender, EventArgs e)
            {
                OnSomeThingChanged();
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.IS, Messages.IS_NOT };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement) { return true; }
            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                List<T> entries = new List<T>();

                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.AddAll(entries, null);
                }

                // CA-17132: Special case pools because they're silly
                if (typeof(T) == typeof(Pool))
                {
                    entries.RemoveAll((Predicate<T>)delegate(T m)
                    {
                        return Helpers.GetPool(m.Connection) == null;
                    });
                }

                return entries.ToArray();
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                ToolStripItem i = queryElement.ComboButton.SelectedItem;
                if (i == null || !(i.Tag is T))
                    return null;

                bool equals = queryElement.matchTypeComboButton.SelectedItem.Tag.Equals(Messages.IS);
                T query = (T)i.Tag;

                return new XenModelObjectPropertyQuery<T>(property, query, equals);
            }

            public override bool ForQuery(QueryFilter query)
            {
                XenModelObjectPropertyQuery<T> xmoPropertyQuery = query as XenModelObjectPropertyQuery<T>;
                if (xmoPropertyQuery == null)
                    return false;

                return (xmoPropertyQuery.property == this.property);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                XenModelObjectPropertyQuery<T> xmoPropertyQuery = query as XenModelObjectPropertyQuery<T>;

                queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                {
                    return entry == (xmoPropertyQuery.equals ? Messages.IS : Messages.IS_NOT);
                });

                queryElement.ComboButton.SelectItem<T>(delegate(T entry)
                {
                    return entry.opaque_ref.Equals(xmoPropertyQuery.query.opaque_ref);
                });
            }
        }

        // NOT USED FOR NEW QUERIES
        internal class XenModelObjectListContainsQueryType<T> : PropertyQueryType<List<T>> where T : XenObject<T>
        {
            public XenModelObjectListContainsQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
                ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
                XenConnections_CollectionChanged(null, null);
            }

            void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.RegisterBatchCollectionChanged<T>(dict_BatchCollectionChanged);
                }
            }

            void dict_BatchCollectionChanged(object sender, EventArgs e)
            {
                OnSomeThingChanged();
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    if (property == PropertyNames.networks)
                        return new String[] { 
                            Messages.USES, 
                            Messages.DOES_NOT_USE,
                            Messages.STARTS_WITH,
                            Messages.ENDS_WITH,
                            Messages.CONTAINS,
                            Messages.NOT_CONTAINS
                        };

                    return new String[] { 
                        Messages.IS, 
                        Messages.IS_NOT, 
                        Messages.STARTS_WITH,
                        Messages.ENDS_WITH,
                        Messages.CONTAINS,
                        Messages.NOT_CONTAINS
                    };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement)
            {
                String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;

                return matchType == Messages.USES
                    || matchType == Messages.CONTAINED_IN
                    || matchType == Messages.IS
                    || matchType == Messages.DOES_NOT_USE
                    || matchType == Messages.NOT_CONTAINED_IN
                    || matchType == Messages.IS_NOT;
            }

            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                List<T> entries = new List<T>();

                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.Cache.AddAll(entries, null);
                }

                // CA-24314: Filter networks that should be hidden
                if (typeof(T) == typeof(XenAPI.Network))
                {
                    foreach (T entry in entries.ToArray())
                    {
                        XenAPI.Network e = entry as XenAPI.Network;
                        if (!e.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                            entries.Remove(entry);
                    }
                }

                return entries.ToArray();
            }

            public override bool ShowTextBox(QueryElement queryElement)
            {
                return !ShowComboButton(queryElement);
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;

                if (ShowComboButton(queryElement))
                {
                    T t = queryElement.ComboButton.SelectedItem == null ? null : queryElement.ComboButton.SelectedItem.Tag as T;
                    if (matchType == Messages.USES ||
                        matchType == Messages.CONTAINED_IN ||
                        matchType == Messages.IS)
                    {
                        return new XenModelObjectListContainsQuery<T>(property, t, true);
                    }
                    else
                    {
                        System.Diagnostics.Trace.Assert(matchType == Messages.DOES_NOT_USE ||
                                                        matchType == Messages.NOT_CONTAINED_IN ||
                                                        matchType == Messages.IS_NOT);

                        return new XenModelObjectListContainsQuery<T>(property, t, false);
                    }
                }
                else
                {
                    String query = queryElement.textBox.Text;

                    if (matchType == Messages.STARTS_WITH)
                    {
                        return new XenModelObjectListContainsNameQuery<T>(property,
                            StringPropertyQuery.PropertyQueryType.startswith, query);
                    }
                    else if (matchType == Messages.ENDS_WITH)
                    {
                        return new XenModelObjectListContainsNameQuery<T>(property,
                            StringPropertyQuery.PropertyQueryType.endswith, query);
                    }
                    else if (matchType == Messages.CONTAINS)
                    {
                        return new XenModelObjectListContainsNameQuery<T>(property,
                            StringPropertyQuery.PropertyQueryType.contains, query);
                    }
                    else if (matchType == Messages.NOT_CONTAINS)
                    {
                        return new XenModelObjectListContainsNameQuery<T>(property,
                            StringPropertyQuery.PropertyQueryType.notcontain, query);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public override bool ForQuery(QueryFilter query)
            {
                ListContainsQuery<T> listQuery = query as ListContainsQuery<T>;  // includes both XenModelObjectListContainsQuery<T> and XenModelObjectListContainsNameQuery<T>
                if (listQuery == null)
                    return false;

                return (listQuery.property == this.property);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                XenModelObjectListContainsQuery<T> listContainsQuery = query as XenModelObjectListContainsQuery<T>;

                if (listContainsQuery == null)
                {
                    XenModelObjectListContainsNameQuery<T> nameQuery = query as XenModelObjectListContainsNameQuery<T>;

                    queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                    {
                        if (entry == Messages.STARTS_WITH)
                        {
                            return nameQuery.type == StringPropertyQuery.PropertyQueryType.startswith;
                        }
                        else if (entry == Messages.ENDS_WITH)
                        {
                            return nameQuery.type == StringPropertyQuery.PropertyQueryType.endswith;
                        }
                        else if (entry == Messages.CONTAINS)
                        {
                            return nameQuery.type == StringPropertyQuery.PropertyQueryType.contains;
                        }
                        else if (entry == Messages.NOT_CONTAINS)
                        {
                            return nameQuery.type == StringPropertyQuery.PropertyQueryType.notcontain;
                        }

                        return false;
                    });

                    queryElement.textBox.Text = nameQuery.query;
                }
                else
                {
                    queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                    {
                        if (entry == Messages.USES
                            || entry == Messages.CONTAINED_IN
                            || entry == Messages.IS)
                        {
                            return listContainsQuery.contains;
                        }
                        else if (entry == Messages.DOES_NOT_USE
                            || entry == Messages.NOT_CONTAINED_IN
                            || entry == Messages.IS_NOT)
                        {
                            return !listContainsQuery.contains;
                        }

                        return false;
                    });

                    queryElement.ComboButton.SelectItem<T>(delegate(T entry)
                    {
                        return Helpers.GetUuid(entry).Equals(listContainsQuery.queryUUID);
                    });
                }
            }
        }

        // NOT USED FOR NEW QUERIES
        internal class HostQueryType : XenModelObjectListContainsQueryType<Host>
        {
            public HostQueryType(int group, ObjectTypes appliesTo)
                : base(group, appliesTo, PropertyNames.host)
            {
            }

            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    List<Object> matchTypes = new List<Object>(base.MatchTypeComboButtonEntries);

                    matchTypes.Add(Messages.IS_STANDALONE);
                    matchTypes.Add(Messages.IS_IN_A_POOL);

                    return matchTypes.ToArray();
                }
            }

            public override bool ShowTextBox(QueryElement queryElement)
            {
                if (ShowComboButton(queryElement))
                    return false;

                String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;

                return matchType != Messages.IS_STANDALONE &&
                    matchType != Messages.IS_IN_A_POOL;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                if (ShowComboButton(queryElement) || ShowTextBox(queryElement))
                    return base.GetQuery(queryElement);

                String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;
                return new NullQuery<Pool>(PropertyNames.pool, matchType == Messages.IS_STANDALONE);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                NullQuery<Pool> nullQuery = query as NullQuery<Pool>;

                if (nullQuery != null)
                {
                    queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                    {
                        return entry == Messages.IS_STANDALONE == nullQuery.query;
                    });
                }
                else
                {
                    base.FromQuery(query, queryElement);
                }
            }
        }

        internal class TagQueryType : PropertyQueryType<List<String>>
        {
            private String[] tags = new String[] { };
            private String deletedTag = null;

            public TagQueryType(int group, ObjectTypes appliesTo)
                : base(group, appliesTo, PropertyNames.tags)
            {
                OtherConfigAndTagsWatcher.TagsChanged += OtherConfigAndTagsWatcher_TagsChanged;
            }

            void OtherConfigAndTagsWatcher_TagsChanged()
            {
                tags = Tags.GetAllTags();

                OnSomeThingChanged();
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override Object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.CONTAINS, Messages.NOT_CONTAINS, 
                        Messages.ARE_EMPTY, Messages.ARE_NOT_EMPTY };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement)
            {
                String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;

                return matchType == Messages.CONTAINS ||
                    matchType == Messages.NOT_CONTAINS;
            }

            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                if (deletedTag != null)
                {
                    List<String> newTags = new List<String>(tags);
                    newTags.Add(deletedTag);
                    return newTags.ToArray();
                }

                return tags;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                if (!ShowComboButton(queryElement))
                {
                    String matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as String;

                    return new ListEmptyQuery<String>(property, matchType == Messages.ARE_EMPTY);
                }

                if (queryElement.ComboButton.SelectedItem == null)
                    return null;

                bool contains = queryElement.matchTypeComboButton.SelectedItem.Tag.Equals(Messages.CONTAINS);
                String query = queryElement.ComboButton.SelectedItem.Tag as String;

                return new StringListContainsQuery(property, query, contains);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                StringListContainsQuery listContainsQuery = query as StringListContainsQuery;
                if (listContainsQuery != null)
                {
                    if (Array.BinarySearch<String>(tags, listContainsQuery.query) < 0)
                    {
                        deletedTag = listContainsQuery.query;
                        OnSomeThingChanged();
                    }

                    queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                    {
                        return entry == (listContainsQuery.contains ? Messages.CONTAINS : Messages.NOT_CONTAINS);
                    });

                    queryElement.ComboButton.SelectItem<String>(delegate(String entry)
                    {
                        return entry.Equals(listContainsQuery.query);
                    });

                    return;
                }

                ListEmptyQuery<String> listEmptyQuery = query as ListEmptyQuery<String>;
                if (listEmptyQuery != null)
                {
                    queryElement.matchTypeComboButton.SelectItem<String>(delegate(String entry)
                    {
                        return entry == (listEmptyQuery.empty ? Messages.ARE_EMPTY : Messages.ARE_NOT_EMPTY);
                    });
                }
            }
        }

        internal abstract class CustomFieldQueryTypeBase : QueryType
        {
            public readonly CustomFieldDefinition definition;

            public CustomFieldQueryTypeBase(int group, ObjectTypes appliesTo, CustomFieldDefinition definition)
                : base(group, appliesTo)
            {
                this.definition = definition;
            }

            public override string ToString()
            {
                return definition.Name;
            }

            public override bool ForQuery(QueryFilter query)
            {
                CustomFieldQueryBase customFieldQuery = query as CustomFieldQueryBase;
                if (customFieldQuery == null)
                    return false;

                return customFieldQuery.definition.Equals(definition);
            }
        }

        internal class CustomFieldStringQueryType : CustomFieldQueryTypeBase
        {
            public CustomFieldStringQueryType(int group, ObjectTypes appliesTo, CustomFieldDefinition definition)
                : base(group, appliesTo, definition)
            { }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override bool ShowTextBox(QueryElement queryElement) { return true; }

            public override object[] MatchTypeComboButtonEntries
            {
                get { return StringPropertyQueryType.MatchTypeEntries; }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                StringPropertyQueryType.ExtraComboEntry extraComboEntry
                    = (StringPropertyQueryType.ExtraComboEntry)queryElement.matchTypeComboButton.SelectedItem.Tag;

                return new CustomFieldQuery(definition, queryElement.textBox.Text, extraComboEntry.type);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                CustomFieldQuery customFieldQuery = query as CustomFieldQuery;

                queryElement.matchTypeComboButton.SelectItem<StringPropertyQueryType.ExtraComboEntry>(
                    delegate(StringPropertyQueryType.ExtraComboEntry entry)
                    {
                        return entry.type == customFieldQuery.type;
                    });

                queryElement.textBox.Text = customFieldQuery.query;
            }
        }

        internal class CustomFieldDateQueryType : CustomFieldQueryTypeBase
        {
            public CustomFieldDateQueryType(int group, ObjectTypes appliesTo, CustomFieldDefinition definition)
                : base(group, appliesTo, definition)
            { }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override bool ShowDateTimePicker(QueryElement queryElement)
            {
                return DatePropertyQueryType.ShouldShowDateTimePicker(queryElement);
            }

            public override object[] MatchTypeComboButtonEntries
            {
                get { return DatePropertyQueryType.MatchTypeEntries; }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                DatePropertyQueryType.ExtraComboEntry extraComboEntry
                    = (DatePropertyQueryType.ExtraComboEntry)queryElement.matchTypeComboButton.SelectedItem.Tag;

                return new CustomFieldDateQuery(definition, queryElement.dateTimePicker.Value, extraComboEntry.type);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                CustomFieldDateQuery customFieldQuery = query as CustomFieldDateQuery;

                queryElement.matchTypeComboButton.SelectItem<DatePropertyQueryType.ExtraComboEntry>(
                    delegate(DatePropertyQueryType.ExtraComboEntry entry)
                    {
                        return entry.type == customFieldQuery.type;
                    });

                queryElement.dateTimePicker.Value = customFieldQuery.query;
            }
        }

        internal class BooleanQueryType : PropertyQueryType<bool>
        {
            public BooleanQueryType(int group, ObjectTypes appliesTo, PropertyNames property)
                : base(group, appliesTo, property)
            {
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                BooleanQuery booleanQuery = query as BooleanQuery;
                queryElement.ComboButton.SelectItem<String>(delegate(String item)
                {
                    return (item == Messages.TRUE) == booleanQuery.query;
                });
            }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override object[] MatchTypeComboButtonEntries
            {
                get
                {
                    return new String[] { Messages.IS };
                }
            }

            public override bool ShowComboButton(QueryElement queryElement) { return true; }
            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                return new String[] { Messages.TRUE, Messages.FALSE };
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                bool query = queryElement.ComboButton.SelectedItem.Tag as String == Messages.TRUE;
                return new BooleanQuery(property, query);
            }
        }

        internal class NullQueryType<T> : PropertyQueryType<T> where T : XenObject<T>
        {
            bool isNull;

            public NullQueryType(int group, ObjectTypes appliesTo, PropertyNames property, bool isNull, String i18n) :
                base(group, appliesTo, property, i18n)
            {
                this.isNull = isNull;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                return new NullQuery<T>(property, isNull);
            }

            public override bool ForQuery(QueryFilter query)
            {
                NullQuery<T> nullQuery = query as NullQuery<T>;
                return (nullQuery != null &&
                    nullQuery.property == property &&
                    nullQuery.query == isNull);
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                // nothing to do because there are no further choices
            }
        }

        internal abstract class MatchQueryType : QueryType
        {
            private String i18n;

            public MatchQueryType(int group, ObjectTypes appliesTo, String i18n)
                : base(group, appliesTo)
            {
                this.i18n = i18n;
            }

            public override String ToString()
            {
                return i18n;
            }

            protected abstract MatchType[] MatchTypes { get; }

            public override bool ShowMatchTypeComboButton { get { return true; } }
            public override object[] MatchTypeComboButtonEntries { get { return MatchTypes; } }

            public override bool ShowComboButton(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return false;

                return matchType.ShowComboButton(queryElement);
            }

            public override bool ShowNumericUpDown(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return false;

                return matchType.ShowNumericUpDown(queryElement);
            }

            public override Object[] GetComboButtonEntries(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return null;

                return matchType.ComboButtonEntries;
            }

            public override bool ShowTextBox(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return false;

                return matchType.ShowTextBox(queryElement);
            }

            public override string Units(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return String.Empty;

                return matchType.Units(queryElement);
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                MatchType matchType = queryElement.matchTypeComboButton.SelectedItem.Tag as MatchType;
                if (matchType == null)
                    return null;

                return matchType.GetQuery(queryElement);
            }

            public override bool ForQuery(QueryFilter query)
            {
                foreach (MatchType matchType in MatchTypes)
                    if (matchType.ForQuery(query))
                        return true;

                return false;
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                foreach (MatchType matchType in MatchTypes)
                    if (matchType.ForQuery(query))
                    {
                        queryElement.matchTypeComboButton.SelectItem<MatchType>(delegate(MatchType entry)
                        {
                            return entry == matchType;
                        });

                        matchType.FromQuery(query, queryElement);

                        return;
                    }
            }
        }

        public abstract class MatchType
        {
            private String matchText;

            protected MatchType(String matchText)
            {
                this.matchText = matchText;
            }

            public override string ToString() { return matchText; }

            public virtual bool ShowComboButton(QueryElement queryElement) { return false; }
            public virtual bool ShowTextBox(QueryElement queryElement) { return false; }
            public virtual bool ShowNumericUpDown(QueryElement queryElement) { return false; }

            public virtual String Units(QueryElement queryElement) { return String.Empty; }
            public virtual Object[] ComboButtonEntries { get { return null; } }

            public abstract QueryFilter GetQuery(QueryElement queryElement);
            public abstract bool ForQuery(QueryFilter query);
            public abstract void FromQuery(QueryFilter query, QueryElement queryElement);
        }

        internal class XMOListContains<T> : MatchType where T : XenObject<T>
        {
            private PropertyNames property;
            private bool contains;

            private Predicate<T> filter;

            public XMOListContains(PropertyNames property, bool contains, String matchText,
                Predicate<T> filter)
                : base(matchText)
            {
                this.property = property;
                this.contains = contains;
                this.filter = filter;
            }

            public override bool ShowComboButton(QueryElement queryElement) { return true; }
            public override object[] ComboButtonEntries
            {
                get
                {
                    List<T> entries = new List<T>();

                    foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                        connection.Cache.AddAll(entries, filter);

                    return entries.ToArray();
                }
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                T t = queryElement.ComboButton.SelectedItem.Tag as T;
                if (t == null)
                    return null;

                return new XenModelObjectListContainsQuery<T>(property, t, contains);
            }

            public override bool ForQuery(QueryFilter query)
            {
                XenModelObjectListContainsQuery<T> xmoQuery = query as XenModelObjectListContainsQuery<T>;
                if (xmoQuery == null)
                    return false;

                return xmoQuery.property == property && xmoQuery.contains == contains;
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                XenModelObjectListContainsQuery<T> xmoQuery = query as XenModelObjectListContainsQuery<T>;

                queryElement.ComboButton.SelectItem<T>(delegate(T t)
                {
                    return Helpers.GetUuid(t) == xmoQuery.queryUUID;
                });
            }
        }

        internal class IntMatch : MatchType
        {
            private readonly PropertyNames property;
            private readonly IntPropertyQuery.PropertyQueryType type;
            private readonly String units;
            private readonly long multiplier;

            public IntMatch(PropertyNames property, String matchText,
                String units, long multiplier, IntPropertyQuery.PropertyQueryType type)
                : base(matchText)
            {
                this.property = property;
                this.type = type;
                this.units = units;
                this.multiplier = multiplier;
            }

            public override bool ShowNumericUpDown(QueryElement queryElement) { return true; }

            public override string Units(QueryElement queryElement)
            {
                return units;
            }

            public override QueryFilter GetQuery(QueryElement queryElement)
            {
                long value = multiplier * (long)queryElement.numericUpDown.Value;

                return new IntPropertyQuery(property, value, type);
            }

            public override bool ForQuery(QueryFilter query)
            {
                IntPropertyQuery intQuery = query as IntPropertyQuery;
                if (intQuery == null)
                    return false;

                return intQuery.property == property && intQuery.type == type;
            }

            public override void FromQuery(QueryFilter query, QueryElement queryElement)
            {
                IntPropertyQuery intQuery = query as IntPropertyQuery;
                if (intQuery == null)
                    return;

                queryElement.numericUpDown.Value = intQuery.query / multiplier;
            }
        }

        // NOT USED FOR NEW QUERIES
        internal class DiskQueryType : MatchQueryType
        {
            private readonly MatchType[] matchType = new MatchType[] {
                new XMOListContains<SR>(PropertyNames.storage, true, 
                    Messages.CONTAINED_IN, null),
                new XMOListContains<SR>(PropertyNames.storage, false, 
                    Messages.NOT_CONTAINED_IN, null),
                new XMOListContains<VM>(PropertyNames.vm, true, 
                    Messages.ATTACHED_TO, VmFilter),
                new XMOListContains<VM>(PropertyNames.vm, false, 
                    Messages.NOT_ATTACHED_TO, VmFilter),
                new IntMatch(PropertyNames.size, Messages.SIZE_IS, Messages.VAL_GIGB, 
                    Util.BINARY_GIGA, IntPropertyQuery.PropertyQueryType.exactmatch),
                new IntMatch(PropertyNames.size, Messages.BIGGER_THAN, Messages.VAL_GIGB, 
                    Util.BINARY_GIGA, IntPropertyQuery.PropertyQueryType.gt),
                new IntMatch(PropertyNames.size, Messages.SMALLER_THAN, Messages.VAL_GIGB, 
                    Util.BINARY_GIGA, IntPropertyQuery.PropertyQueryType.lt),
            };

            public DiskQueryType(int group, ObjectTypes appliesTo, String i18n)
                : base(group, appliesTo, i18n)
            {
            }

            protected override MatchType[] MatchTypes { get { return matchType; } }

            private static bool VmFilter(VM vm)
            {
                return !(vm.is_a_template || vm.is_control_domain)
                    && vm.VBDs.Count > 0;
            }
        }
        
        internal class LongQueryType : MatchQueryType
        {
            private readonly MatchType[] matchType;

            public LongQueryType(int group, ObjectTypes appliesTo, String i18n, PropertyNames property, long multiplier, String unit)
                : base(group, appliesTo, i18n)
            {
                matchType = new MatchType[] {
                    new IntMatch(property, Messages.BIGGER_THAN, unit, 
                        multiplier, IntPropertyQuery.PropertyQueryType.gt),
                    new IntMatch(property, Messages.SMALLER_THAN, unit, 
                        multiplier, IntPropertyQuery.PropertyQueryType.lt),
                    new IntMatch(property, Messages.IS_EXACTLY, unit, 
                        multiplier, IntPropertyQuery.PropertyQueryType.exactmatch),
                };
            }

            protected override MatchType[] MatchTypes { get { return matchType; } }
        }

        #endregion
    }
}
