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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.XenSearch;

namespace XenAdmin.Controls.XenSearch
{
    public partial class Searcher : UserControl
    {
        public event Action SearchChanged;
        public event Action SearchForChanged;
        public event Action SaveRequested;
        public event Action SearchPanelExpandChanged;

        public Searcher()
        {
            InitializeComponent();

            QueryElement.Searcher = this;
            GroupingControl.Searcher = this;

            QueryElement.QueryChanged += QueryElementQueryChanged;
            GroupingControl.GroupingChanged += GroupingControlGroupingChanged;
            searchFor.QueryChanged += SearchForQueryChanged;
        }

        private void GroupingControlGroupingChanged()
        {
            OnSearchChanged();
        }

        private void QueryElementQueryChanged()
        {
            OnSearchChanged();
        }

        private void SearchForQueryChanged()
        {
            OnSearchForChanged();
            OnSearchChanged();
        }

        #region Accessors

        [Browsable(false)]
        public Search Search
        {
            set
            {
                if (value != null)
                {
                    // It's actually necessary for searchFor to come first here. Otherwise when
                    // we set the searchFor, it will remove any inapplicable filters and groups,
                    // which is not the desired behaviour for legacy saved searches.
                    if (searchFor != null)
                    {
                        searchFor.QueryScope = value.Query.QueryScope;
                    }

                    if (QueryElement != null)
                    {
                        QueryElement.QueryFilter = value.Query.QueryFilter;
                    }

                    if (GroupingControl != null)
                    {
                        GroupingControl.Grouping = value.Grouping;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum height for this control.
        /// The contents of this control will autoscroll inside this height.
        /// </summary>
        public int MaxHeight { get; set; } = 400;

        public QueryScope QueryScope
        {
            get { return searchFor.QueryScope; }
        }

        public QueryFilter QueryFilter
        {
            get { return QueryElement.QueryFilter; }
        }

        public Grouping Grouping
        {
            get { return GroupingControl.Grouping; }
        }

        #endregion

        public void ToggleExpandedState(bool expand)
        {
            Visible = expand;

            if (Visible)
                buttonSave.Enabled = (ConnectionsManager.XenConnections.Find(c => c.IsConnected) != null);

            if (SearchPanelExpandChanged != null)
                SearchPanelExpandChanged();
        }

        protected virtual void OnSearchChanged()
        {
            if (SearchChanged != null)
                SearchChanged();
        }

        protected virtual void OnSearchForChanged()
        {
            if (SearchForChanged != null)
                SearchForChanged();
        }

        private void QueryElement_Resize(object sender, EventArgs e)
        {
            // this control has AutoScroll set to true.

            int contentsHeight = QueryElement.Top + QueryElement.Height + 73;
            GroupsLabel.Top = contentsHeight - 64;
            GroupingControl.Top = contentsHeight - 41;

            Height = Math.Min(MaxHeight, contentsHeight);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (SaveRequested != null)
                SaveRequested();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            ToggleExpandedState(false);
        }

        public void BlankSearch()
        {
            searchFor.BlankSearch();
            QueryElement.SelectDefaultQueryType();
        }
    }
}
