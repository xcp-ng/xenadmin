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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.XenSearch;
using XenAdmin.Core;
using XenAdmin.XenSearch;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    public partial class SearchPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool ignoreSearchUpdate;
        private List<IXenObject> xenObjects;
        public event Action SearchChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();

            Searcher.SearchPanelExpandChanged += Searcher_SearchPanelExpandChanged;
            Searcher.SearchChanged += UI_SearchChanged;
            Searcher.SaveRequested += Searcher_SaveRequested;
            OutputPanel.QueryPanel.SearchChanged += UI_SearchChanged;

            if (!Application.RenderWithVisualStyles)
            {
                panel2.BackColor = Searcher.BackColor = SystemColors.Control;
                OutputPanel.BackColor = SystemColors.Control;
                tableLayoutPanel.BackColor = SystemColors.ControlDark;
            }

            Searcher.ToggleExpandedState(false);
        }

        public override string HelpID => "TabPageSearch";

        protected virtual void OnSearchChanged()
        {
            if (SearchChanged != null)
                SearchChanged();
        }

        private void Searcher_SaveRequested()
        {
            Search newSearch = Search;
            
            string saveName = newSearch.Name ?? String.Empty;
            List<Search> existingSearches = new List<Search>(Search.Searches);
            if (null != existingSearches.Find(search => search.Name == saveName))
            {
                // name already exists: choose a new name by appending an integer (CA-34780)
                for (int i = 2; ; ++i)
                {
                    string possName = string.Format("{0} ({1})", saveName, i);
                    if (null == existingSearches.Find(search => search.Name == possName))  // here's a good name
                    {
                        saveName = possName;
                        break;
                    }
                }
            }

            using (var dialog = new NameAndConnectionPrompt
            {
                PromptedName = saveName,
                HelpID = "SaveSearchDialog"
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK &&
                    dialog.Connection != null) // CA-40307
                {
                    newSearch.Name = dialog.PromptedName;
                    newSearch.Connection = dialog.Connection;

                    new SearchAction(newSearch, SearchAction.Operation.save).RunAsync();
                }
            }
        }

        private void UI_SearchChanged()
        {
            if (!ignoreSearchUpdate && !Program.Exiting)
            {
                OutputPanel.Search = Search;
                OnSearchChanged();
            }
        }

        public IXenObject XenObject
        {
            set
            {
                XenObjects = new IXenObject[] { value };
            }
        }

        public IEnumerable<IXenObject> XenObjects
        {
            set
            {
                Util.ThrowIfParameterNull(value, "value");

                xenObjects = new List<IXenObject>(value);

                if (xenObjects.Count == 0 && TreeSearch.DefaultTreeSearch != null)
                {
                    Search = TreeSearch.DefaultTreeSearch;
                }
                else
                {
                    Search = Search.SearchFor(value);
                }
            }
        }

        public Search Search
        {
            get
            {
                QueryScope scope = Searcher.QueryScope;
                QueryFilter filter = Searcher.QueryFilter;
                Query query = new Query(scope, filter);
                Grouping grouping = Searcher.Grouping;
                string name = (base.Text == Messages.CUSTOM_SEARCH ? null : base.Text);
                string uuid = null;
                List<KeyValuePair<String, int>> columns = OutputPanel.QueryPanel.ColumnsAndWidths;
                Sort[] sorting = OutputPanel.QueryPanel.Sorting;

                return new Search(query, grouping, name, uuid, columns, sorting);
            }
            set
            {
                ignoreSearchUpdate = true;
                try
                {
                    Searcher.Search = value;
                }
                finally
                {
                    ignoreSearchUpdate = false;
                }

                OutputPanel.Search = value;

                UpdateTitle(value);
            }
        }

        private void UpdateTitle(Search search)
        {
            base.Text = (search == null || search.Name == null)
                            ? Messages.CUSTOM_SEARCH
                            : HelpersGUI.GetLocalizedSearchName(search);
        }

        public void BuildList()
        {
            if (!Visible)
                return;
            OutputPanel.BuildList();
        }
    
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Searcher != null)
                Searcher.MaxHeight = Height / 2;
        }

        private void Searcher_SearchPanelExpandChanged()
        {
            buttonEditSearch.Enabled = !Searcher.Visible;
        }

        #region Search menu

        private void contextMenuStripSearches_Opening(object sender, CancelEventArgs e)
        {
            var theDefaultSearch = new ToolStripMenuItem(Messages.DEFAULT_SEARCH, null, applyDefaultSearch_Click);

            var defaultSearches = Search.Searches.Where(s => s.DefaultSearch).OrderBy(s => s);
            var customSearches = Search.Searches.Where(s => !s.DefaultSearch).OrderBy(s => s);

            var customItems = new List<ToolStripMenuItem>();
            foreach (Search search in customSearches)
            {
                customItems.Add(new ToolStripMenuItem(search.Name.EscapeAmpersands(),
                                                 Properties.Resources._000_Search_h32bit_16,
                                                 applySavedSearch_Click) { Tag = search });
            }

            var defaultItems = new List<ToolStripMenuItem>();
            foreach (Search search in defaultSearches)
            {
                defaultItems.Add(new ToolStripMenuItem(search.Name.EscapeAmpersands(),
                                                       Properties.Resources._000_defaultSpyglass_h32bit_16,
                                                       applySavedSearch_Click) { Tag = search });
            }

            contextMenuStripSearches.Items.Clear();
            contextMenuStripSearches.Items.Add(theDefaultSearch);
            contextMenuStripSearches.Items.AddRange(customItems.ToArray());
            contextMenuStripSearches.Items.AddRange(defaultItems.ToArray());
            contextMenuStripSearches.Items.Add(new ToolStripSeparator());

            var deleteItem = new ToolStripMenuItem(Messages.DELETE_SEARCH_MENU_ITEM);

            if (customSearches.Count() > 0)
            {
                deleteItem.Enabled = true;

                var deleteableItems = new List<ToolStripMenuItem>();
                foreach (Search search in customSearches)
                {
                    deleteableItems.Add(new ToolStripMenuItem(search.Name.EscapeAmpersands(),
                                                              Properties.Resources._000_Search_h32bit_16,
                                                              deleteSavedSearch_Click) { Tag = search });
                }
                deleteItem.DropDownItems.AddRange(deleteableItems.ToArray());
            }
            else
            {
                deleteItem.Enabled = false;
            }
            contextMenuStripSearches.Items.Add(deleteItem);
        }

        private void buttonEditSearch_Click(object sender, EventArgs e)
        {
            Searcher.ToggleExpandedState(true);
        }

        private void buttonNewSearch_Click(object sender, EventArgs e)
        {
            Searcher.ToggleExpandedState(true);
            Searcher.BlankSearch();
            base.Text = Messages.NEW_SEARCH;
        }

        private void applyDefaultSearch_Click(object sender, EventArgs e)
        {
            Search search = Search.SearchFor(xenObjects);
            Search = search;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.AddExtension = true;
                dialog.Filter = string.Format(Messages.XENSEARCH_SAVED_SEARCH, BrandManager.ExtensionSearch);
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = BrandManager.ExtensionSearch;
                dialog.CheckPathExists = false;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    log.InfoFormat("Exporting search to {0}", dialog.FileName);
                    Search.Save(dialog.FileName);
                    log.InfoFormat("Exported search to {0} successfully.", dialog.FileName);
                }
                catch
                {
                    log.ErrorFormat("Failed to export search to {0}", dialog.FileName);
                    throw;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            new ImportSearchCommand(Program.MainWindow).Execute();
        }

        private void applySavedSearch_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item == null)
                return;

            var search = item.Tag as Search;
            if (search == null)
                return;

            Search = search;
        }

        private void deleteSavedSearch_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            if (item == null)
                return;

            var search = item.Tag as Search;
            if (search == null)
                return;

            using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        String.Format(Messages.DELETE_SEARCH_PROMPT, search.Name),
                        String.Format(Messages.DELETE_SEARCH, search.Name)),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                if (dlog.ShowDialog(this) == DialogResult.Yes)
                    new SearchAction(search, SearchAction.Operation.delete).RunAsync();
            }
        }

        #endregion

        public void PanelShown()
        {
            QueryPanel.PanelShown();
        }

        public void PanelHidden()
        {
            QueryPanel.PanelHidden();
        }

        internal void PanelProd()
        {
            QueryPanel.Prod();
        }
    }
}
