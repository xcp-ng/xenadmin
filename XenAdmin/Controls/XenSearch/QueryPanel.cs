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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.CustomFields;
using XenAdmin.XenSearch;
using XenAPI;
using System.Resources;
using XenAdmin.Core;
using XenAdmin.Controls.CustomGridView;
using XenAdmin.Dialogs;
using XenAdmin.Model;


namespace XenAdmin.Controls.XenSearch
{
    public class QueryPanel : GridView
    {
        private const int ROW_HEIGHT = 27;
        private const int FOLDER_ROW_HEIGHT = 14;

        public static Brush TextBrush = SystemBrushes.ControlText;
        public static Brush RedBrush = Brushes.Red;
        public static Brush LinkBrush = Brushes.Blue;
        public static Brush GreyBrush = SystemBrushes.GrayText;
        public static Brush DarkGreyBrush = new SolidBrush(Color.FromArgb(255, 100, 100, 100));

        private static readonly Font HeaderBoldFont = new Font(Program.DefaultFont.FontFamily, Program.DefaultFont.Size + 0.2f, FontStyle.Bold);
        private static readonly Font HeaderSmallFont = new Font(Program.DefaultFont.FontFamily, Program.DefaultFont.Size);
        private static MetricUpdater MetricUpdater;

        private Search search;
        public event Action SearchChanged;

        private readonly UpdateManager listUpdateManager = new UpdateManager();
        static readonly ResourceManager resources = new ResourceManager("XenAdmin.Controls.XenSearch.QueryPanel", typeof(QueryPanel).Assembly);
       
        public QueryPanel()
        {
            this.SuspendLayout();
            MetricUpdater = PropertyAccessorHelper.MetricUpdater; // new MetricUpdater();
            MetricUpdater.MetricsUpdated += MetricsUpdated;
            this.ResumeLayout(false);
            base.HasLeftExpanders = false;
            VerticalScroll.Visible = true;
            SetupHeaderRow();
            CustomFieldsManager.CustomFieldsChanged += CustomFields_CustomFieldsChanged;
            listUpdateManager.Update += listUpdateManager_Update;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (MetricUpdater != null)
                    MetricUpdater.MetricsUpdated -= MetricsUpdated;
                CustomFieldsManager.CustomFieldsChanged -= CustomFields_CustomFieldsChanged;
                listUpdateManager.Update -= listUpdateManager_Update;
                listUpdateManager.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnSearchChanged()
        {
            if (SearchChanged != null)
                SearchChanged();
        }

        void CustomFields_CustomFieldsChanged()
        {
            SetupCustomColumns();
        }

        #region Column Handling Code

        private GridHeaderRow GridHeaderRow = null;
        private readonly Dictionary<String, bool> Columns = new Dictionary<String, bool>();
        private static readonly String[] DEFAULT_COLUMNS = new String[]{ "name", 
            "cpu", "memory", "disks", "network", "ip", "ha", "uptime" };

        private static bool IsDefaultColumn(String column)
        {
            return Array.IndexOf<String>(DEFAULT_COLUMNS, column) > -1;
        }

        private bool IsMovableColumn(string column)
        {
            if (GridHeaderRow == null || !GridHeaderRow.Items.ContainsKey(column))
                return true;

            GridHeaderItem item = GridHeaderRow.Items[column] as GridHeaderItem;
            if (item == null)
                return true;

            return !item.Immovable;
        }

        private void ShowColumn(String column)
        {
            Columns[column] = true;

            if (column == "ha" && !HelpersGUI.HAEnabledOnAtLeastOnePool)
            {
                HideColumn("ha");
                return;
            }

            if (!GridHeaderRow.Columns.Contains(column))
            {
                if (IsDefaultColumn(column))
                {
                    GridHeaderRow.AddItem(column, GetDefaultColumn(column));
                }
                else
                {
                    String text = column.StartsWith(CustomFieldsManager.CUSTOM_FIELD) ?
                        column.Substring(CustomFieldsManager.CUSTOM_FIELD.Length) : column;

                    GridHeaderRow.AddItem(column, NewHeaderItem(text, SortOrder.Ascending, 100, 100));
                }
            }
        }

        private void HideColumn(String column)
        {
            Columns[column] = false;

            GridHeaderRow.RemoveItem(column);
        }

        private void ToggleColumn(String column)
        {
            if (Columns[column])
                HideColumn(column);
            else
                ShowColumn(column);
        }

        private void RemoveColumn(String column)
        {
            if (IsDefaultColumn(column))
                return;

            HideColumn(column);
            Columns.Remove(column);
        }

        private void SetupHeaderRow()
        {
            GridHeaderRow = new GridHeaderRow(ROW_HEIGHT);

            foreach (string s in DEFAULT_COLUMNS)
                ShowColumn(s);

            SetHeaderRow(GridHeaderRow);
        }

        private static GridItemBase GetDefaultColumn(string s)
        {
            if (s == "name")
            {
                GridHeaderItem nameHeaderItem = NewHeaderItem(Messages.OVERVIEW_NAME, SortOrder.Ascending, 250, 250);
                nameHeaderItem.IsDefaultSortColumn = true;
                nameHeaderItem.Immovable = true;
                return nameHeaderItem;
            }

            if (s == "cpu")
            {
                return NewHeaderItem(Messages.OVERVIEW_CPU_USAGE, SortOrder.Descending, 115, 115);
            }

            if (s == "memory")
            {
                return NewHeaderItem(Messages.OVERVIEW_MEMORY_USAGE, SortOrder.Descending, 125, 125);
            }

            if (s == "disks")
            {
                int colWidth;
                Int32.TryParse(resources.GetString("DISKS_OVERVIEW_COL_WIDTH"), out colWidth);
                return NewGridHeaderItem(Messages.OVERVIEW_DISKS, SortOrder.Descending, colWidth, colWidth);
            }

            if (s == "network")
            {
                int colWidth;
                Int32.TryParse(resources.GetString("NETWORK_OVERVIEW_COL_WIDTH"), out colWidth);
                return NewGridHeaderItem(Messages.OVERVIEW_NETWORK, SortOrder.Descending, colWidth, colWidth);
            }

            if (s == "ha")
            {
                return NewHeaderItem(Messages.HA, SortOrder.Ascending, 120, 120);
            }

            if (s == "ip")
            {
                return NewHeaderItem(Messages.ADDRESS, SortOrder.Ascending, 120, 120);
            }

            if (s == "uptime")
            {
                return NewHeaderItem(Messages.UPTIME, SortOrder.Descending, 170, 170);
            }

            // dont be a muppet, only ask for a default column
            return NewHeaderItem(s, SortOrder.Ascending, 100, 100);
        }

        private void ShowColumns(List<KeyValuePair<String, int>> columns)
        {
            List<String> columnKeys = new List<String>(Columns.Keys);

            if (columns == null)
            {
                // Show all columns
                foreach (String column in columnKeys)
                    ShowColumn(column);

                foreach (CustomFieldDefinition definition in CustomFieldsManager.GetCustomFields())
                    ShowColumn(CustomFieldsManager.CUSTOM_FIELD + definition.Name);

                return;
            }

            // Hide all columns which are currently visible but not in columns
            // (do not hide default columns)
            foreach (String column in columnKeys)
            {
                if (!IsMovableColumn(column))
                    continue;

                if (columns.Exists(delegate(KeyValuePair<String, int> kvp)
                {
                    return kvp.Key == column;
                }))
                    continue;

                HideColumn(column);
            }

            ShowColumn("ha"); // force decision to show ha

            // Show appropriate columns
            int i = 0;
            foreach (GridHeaderItem item in HeaderRow.Items.Values)
            {
                if (!item.Immovable)
                    break;

                i++;
            }

            foreach (KeyValuePair<String, int> column in columns)
            {
                if (!IsMovableColumn(column.Key))
                    continue;

                String key =
                    (column.Key.StartsWith(CustomFieldsManager.CUSTOM_FIELD) || IsDefaultColumn(column.Key)) ?
                    column.Key : CustomFieldsManager.CUSTOM_FIELD + column.Key;

                ShowColumn(key);

                if (!HeaderRow.Items.ContainsKey(key))
                    continue;

                GridHeaderItem item = HeaderRow.Items[key] as GridHeaderItem;
                if (item == null)
                    continue;

                // Make the column the correct width;
                item.Width = column.Value;

                // Move the column to the correct place;
                GridHeaderRow.Columns.Remove(column.Key);
                GridHeaderRow.Columns.Insert(i, column.Key);
                i++;
            }
        }

        public override void OpenChooseColumnsMenu(Point point)
        {
            ContextMenu contextMenu = new ContextMenu();

            foreach (String column in Columns.Keys)
            {
                if (!IsMovableColumn(column))
                    continue;

                if (column == "ha" && !Columns[column] && !HelpersGUI.HAEnabledOnAtLeastOnePool)  // it's off, and we can't turn it on
                    continue;

                String columnCopy = column;
                MenuItem item = new MenuItem(GetI18nColumnName(column).Ellipsise(30));
                item.Checked = Columns[column];
                item.Enabled = IsMovableColumn(column);

                item.Click += delegate(object sender, EventArgs e)
                {
                    item.Checked = !item.Checked;
                    ToggleColumn(columnCopy);
                    Refresh();
                };

                contextMenu.MenuItems.Add(item);
            }

            contextMenu.Show(this, point);
        }

        public List<ToolStripMenuItem> GetChooseColumnsMenu()
        {
            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

            foreach (String column in Columns.Keys)
            {
                if (!IsMovableColumn(column))
                    continue;

                if (column == "ha" && !Columns[column] && !HelpersGUI.HAEnabledOnAtLeastOnePool)  // it's off, and we can't turn it on
                    continue;

                String columnCopy = column;
                ToolStripMenuItem item = new ToolStripMenuItem(GetI18nColumnName(column).Ellipsise(30));
                item.Checked = Columns[column];
                item.Enabled = IsMovableColumn(column);

                item.Click += delegate(object sender, EventArgs e)
                {
                    item.Checked = !item.Checked;
                    ToggleColumn(columnCopy);
                    Refresh();
                };

                menuItems.Add(item);
            }

            return menuItems;
        }

        private static string GetI18nColumnName(string column)
        {
            switch (column)
            {
                case "name":
                    return Messages.OVERVIEW_NAME;
                case "cpu":
                    return Messages.OVERVIEW_CPU_USAGE;
                case "memory":
                    return Messages.OVERVIEW_MEMORY_USAGE;
                case "disks":
                    return Messages.OVERVIEW_DISKS;
                case "network":
                    return Messages.OVERVIEW_NETWORK;
                case "ha":
                    return Messages.HA;
                case "ip":
                    return Messages.ADDRESS;
                case "uptime":
                    return Messages.UPTIME;
                default:
                    return column.StartsWith(CustomFieldsManager.CUSTOM_FIELD) ?
                        column.Substring(CustomFieldsManager.CUSTOM_FIELD.Length) : column;
            }
        }

        /// <summary>
        /// Add and/or remove custom columns to the grid.
        /// </summary> 
        private void SetupCustomColumns()
        {
            List<CustomFieldDefinition> customFieldDefinitions = CustomFieldsManager.GetCustomFields();

            // Did the user remove a custom field?
            String[] columns = new String[Columns.Keys.Count];
            Columns.Keys.CopyTo(columns, 0);

            foreach (String column in columns)
            {
                // Skip default columns
                if (IsDefaultColumn(column))
                    continue;

                // Does the custom column in table exist in the custom fields
                // collection?  If not remove.
                if (!customFieldDefinitions.Exists(delegate(CustomFieldDefinition definition)
                    {
                        return definition.Name == column;
                    }))
                    RemoveColumn(column);
            }

            // Add any new columns
            foreach (CustomFieldDefinition customFieldDefinition in customFieldDefinitions)
            {
                if (Columns.ContainsKey(customFieldDefinition.Name))
                    continue;

                ShowColumn(CustomFieldsManager.CUSTOM_FIELD + customFieldDefinition.Name);
            }

            BuildList();
        }

        #endregion

        [Browsable(false)]
        public Search Search
        {
            set
            {
                search = value;

                if (search != null)
                {
                    ShowColumns(search.Columns);
                    Sorting = search.Sorting;
                    BuildList();
                }
            }
        }

        internal override void Sort()
        {
            LastClickedRow = null;
            OnSearchChanged();
        }

        public Sort[] Sorting
        {
            get
            {
                List<Sort> sorting = new List<Sort>();

                foreach (SortParams sp in HeaderRow.CompareOrder)
                    sorting.Add(new Sort(sp.Column, sp.SortOrder == SortOrder.Ascending));

                return sorting.ToArray();
            }

            set
            {
                List<SortParams> order = new List<SortParams>();
                foreach (Sort sort in value)
                    order.Add(new SortParams(sort.Column, sort.Ascending ? SortOrder.Ascending : SortOrder.Descending));
                HeaderRow.UpdateCellSorts(order);
            }
        }

        // Are any of the sort columns based on one of the performance metrics?
        public bool SortingByMetrics
        {
            get
            {
                foreach (SortParams sp in HeaderRow.CompareOrder)
                {
                    if (sp.Column == "cpu" || sp.Column == "memory" || sp.Column == "disks" ||
                        sp.Column == "network" || sp.Column == "uptime")
                        return true;
                }
                return false;
            }
        }

        private void listUpdateManager_Update()
        {
            GridRow root = new GridRow(-1);
            RowGroupAcceptor acceptor = new RowGroupAcceptor(root);
            CollectionAcceptor ca = new CollectionAcceptor();

            bool addedAny = search.PopulateAdapters(acceptor, ca);

            Program.Invoke(Program.MainWindow, delegate
            {
                SaveRowStates();
                Clear();

                if (!addedAny)
                {
                    AddNoResultsRow();
                }

                foreach (GridRow row in root.Rows)
                {
                    AddRow(row);
                }

                MetricUpdater.SetXenObjects(ca.XenObjects.ToArray());

                RestoreRowStates();
                Refresh();
            });
        }

        public void BuildList()
        {
            Program.AssertOnEventThread();

            if (search == null)
                return;

            listUpdateManager.RequestUpdate();
        }

        private class RowGroupAcceptor : IAcceptGroups
        {
            private readonly GridRow _gridRow;

            public RowGroupAcceptor(GridRow gridRow)
            {
                _gridRow = gridRow;
            }

            public IAcceptGroups Add(Grouping grouping, object group, int indent)
            {
                GridRow gridRow = CreateRow(grouping, group, indent);
                if (gridRow != null)
                {
                    _gridRow.AddRow(gridRow);
                    return new RowGroupAcceptor(gridRow);
                }
                return null;
            }

            public void FinishedInThisGroup(bool defaultExpand)
            {
            }
        }

        private void AddNoResultsRow()
        {
            GridRow row = new GridRow(ROW_HEIGHT);
            GridStringItem resultsItem = new GridStringItem(Messages.OVERVIEW_NO_RESULTS, HorizontalAlignment.Left, VerticalAlignment.Middle, false, false, TextBrush, Program.DefaultFont, 6);
            row.AddItem("name", resultsItem);
            AddRow(row);
        }

        private static GridHeaderArrayItem NewGridHeaderItem(string label, SortOrder defaultSortOrder, int width, int minwidth)
        {
            return new GridHeaderArrayItem(
                new GridStringItem[] { new GridStringItem(label, HorizontalAlignment.Center, VerticalAlignment.Middle, false, false, TextBrush, HeaderBoldFont, GreyBrush, HeaderBoldFont),
                                       new GridStringItem(Messages.OVERVIEW_UNITS, HorizontalAlignment.Center, VerticalAlignment.Middle, false, false, TextBrush, HeaderSmallFont, GreyBrush, HeaderSmallFont) },
                HorizontalAlignment.Center, VerticalAlignment.Middle, TextBrush, HeaderBoldFont, defaultSortOrder, width, minwidth);
        }

        private static GridHeaderItem NewHeaderItem(string label, SortOrder defaultSortOrder, int width, int minwidth)
        {
            return new GridHeaderItem(HorizontalAlignment.Center, VerticalAlignment.Middle,
                TextBrush, HeaderBoldFont, label, defaultSortOrder, width, minwidth, GreyBrush);
        }

        private static void AddCustomFieldsToRow(IXenObject o, GridRow row)
        {
            foreach (CustomFieldDefinition customFieldDefinition in CustomFieldsManager.GetCustomFields())
            {
                GridStringItem customFieldItem = new GridStringItem(
                    new CustomFieldWrapper(o, customFieldDefinition),
                    HorizontalAlignment.Center, VerticalAlignment.Middle,
                    false, false, TextBrush, Program.DefaultFont,
                    new EventHandler(delegate
                    {
                        using (PropertiesDialog dialog = new PropertiesDialog(o))
                        {
                            dialog.SelectCustomFieldsEditPage();
                            dialog.ShowDialog();
                        }
                    }));

                row.AddItem(CustomFieldsManager.CUSTOM_FIELD + customFieldDefinition.Name, customFieldItem);
            }
        }

        private static GridVerticalArrayItem NewDoubleRowItem(Grouping grouping, object group)
        {
            string line1 = (grouping is BoolGrouping ? (grouping as BoolGrouping).GroupingName2(group) : grouping.GroupingName);    // exception for Boolean groups: CA-165366
            object line2 = (group is DateTime ? group : grouping.GetGroupName(group));    // exception for DateTime: CA-46983
            List<GridStringItem> items = new List<GridStringItem>();
            if (line1 != null)
                items.Add(new GridStringItem(line1, HorizontalAlignment.Left, VerticalAlignment.Middle, false, true, TextBrush, Program.DefaultFont));
            if (line2 != null)
                items.Add(new GridStringItem(line2, HorizontalAlignment.Left, VerticalAlignment.Middle, false, false, DarkGreyBrush, Program.DefaultFont));
            return new GridVerticalArrayItem(items.ToArray(), false);
        }

        private static GridRow NewGroupRow(string opaqueref, object tag, int rowHeight, int priority)
        {
            GridRow row = new GridRow(rowHeight);
            row.OpaqueRef = opaqueref;
            row.Tag = tag;
            row.Expanded = true;
            row.Priority = priority;
            return row;
        }

        private static GridHorizontalArrayItem NewNameItem(GridItemBase iconItem, GridItemBase dataItem, int iconWidth, int indent)
        {
            GridTreeExpanderItem expander = new GridTreeExpanderItem();
            GridItemBase[] items = { expander, iconItem, dataItem };
            int[] widths = { 12, iconWidth };
            return new GridHorizontalArrayItem(indent, items, widths, false);
        }


        private static GridRow CreateRow(Grouping grouping, Object o, int indent)
        {
            IXenObject ixmo = o as IXenObject;
            if (ixmo != null)
            {
                bool isFolderRow = (o is Folder);
                GridRow _row = NewGroupRow(ixmo.opaque_ref, ixmo, isFolderRow ? FOLDER_ROW_HEIGHT : ROW_HEIGHT, 0);

                foreach (ColumnNames column in Enum.GetValues(typeof(ColumnNames)))
                {
                    GridItemBase item = ColumnAccessors.Get(column).GetGridItem(ixmo);
                    if (item != null)
                    {
                        if (column == XenAdmin.XenSearch.ColumnNames.name)
                        {
                            EventHandler onDoubleClickDelegate = isFolderRow ?
                                    (EventHandler)delegate
                                    {
                                        Program.MainWindow.DoSearch(Search.SearchForFolder(ixmo.opaque_ref));
                                    } :
                                    (EventHandler)delegate
                                    {
                                        if (Program.MainWindow.SelectObjectInTree(ixmo)
                                            && Program.MainWindow.TheTabControl.TabPages.Contains(Program.MainWindow.TabPageGeneral))
                                            Program.MainWindow.SwitchToTab(MainWindow.Tab.General);
                                    };
                            GridImageItem _statusItem = new GridImageItem(
                                "foo",
                                new ImageDelegate(delegate()
                                {
                                    return Images.GetImage16For(ixmo);
                                }),
                                HorizontalAlignment.Left, VerticalAlignment.Top, true,
                                onDoubleClickDelegate);
                            _row.AddItem("name", NewNameItem(_statusItem, item, 16, indent));
                        }
                        else
                            _row.AddItem(column.ToString(), item);
                    }
                }

                AddCustomFieldsToRow(ixmo, _row);

                return _row;
            }

            if (grouping == null)
                return null;


            GridRow row = NewGroupRow(String.Format("{0}: {1}", grouping.GroupingName, o), null, ROW_HEIGHT, 0);

            GridImageItem statusItem = new GridImageItem(
                grouping.GroupingName,
                new ImageDelegate(delegate()
                {
                    return Images.GetImage16For(grouping.GetGroupIcon(o));
                }),
                HorizontalAlignment.Left, VerticalAlignment.Top, true);

            GridVerticalArrayItem nameItem = NewDoubleRowItem(grouping, o);

            row.AddItem("name", NewNameItem(statusItem, nameItem, 16, indent));

            return row;
        }

        public override bool IsDraggableRow(GridRow row)
        {
            IXenObject o = row.Tag as IXenObject;
            return o != null;
        }

        private void MetricsUpdated(object o, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                if (SortingByMetrics)
                    Sort();
                else
                    Refresh();
            });
        }

        public static void PanelShown()
        {
            MetricUpdater.Start();
        }

        public static void PanelHidden()
        {
            MetricUpdater.Pause();
        }

        internal static void Prod()
        {
            MetricUpdater.Prod();
        }
    }

    public class CollectionAcceptor : IAcceptGroups
    {
        public List<IXenObject> XenObjects = new List<IXenObject>();

        public IAcceptGroups Add(Grouping grouping, object group, int indent)
        {
            if (group is Host || group is VM)
                XenObjects.Add((IXenObject)group);

            return this;
        }

        public void FinishedInThisGroup(bool defaultExpand)
        {
        }
    }
}
