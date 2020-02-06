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
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;

namespace XenAdmin.Dialogs
{
    public partial class PvsCacheConfigurationDialog : VerticallyTabbedDialog
    {
        public PvsCacheConfigurationDialog(IXenConnection connection)
            :base(connection)
        {
            InitializeComponent();

            System.Diagnostics.Debug.Assert(connection != null);

            Text = string.Format(Messages.PVS_CACHE_CONFIG_DIALOG_TITLE, connection.Name);

            Rebuild();
            this.connection.Cache.RegisterCollectionChanged<PVS_site>(PvsSiteCollectionChanged);
        }

        protected override string GetTabTitle(VerticalTabs.IVerticalTab verticalTab)
        {
            PvsCacheConfigurationPage page = verticalTab as PvsCacheConfigurationPage;
            if (page != null)
            {
                return page.Text;
            }
            return base.GetTabTitle(verticalTab);
        }

        private void Rebuild()
        {
            ContentPanel.SuspendLayout();
            verticalTabs.BeginUpdate();

            try
            {
                verticalTabs.Items.Clear();

                var pvsSites = connection.Cache.PVS_sites.ToList();
                pvsSites.Sort();

                foreach (var pvsSite in pvsSites)
                {
                    NewPage(pvsSite);
                }
            }
            finally
            {
                ContentPanel.ResumeLayout();
                verticalTabs.EndUpdate();
            }

            if (verticalTabs.Items.Count > 0) 
                verticalTabs.SelectedIndex = 0;
            ResizeVerticalTabs(verticalTabs.Items.Count);
            verticalTabs.AdjustItemTextBounds = GetItemTextBounds;
        }

        protected Rectangle GetItemTextBounds(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.X, itemBounds.Y, itemBounds.Width - 20, itemBounds.Height);
        }
        
        private PvsCacheConfigurationPage NewPage(PVS_site pvsSite)
        {
            var existingTabNames = (from PvsCacheConfigurationPage page in verticalTabs.Items select page.Text).ToList();
            PvsCacheConfigurationPage editPage = new PvsCacheConfigurationPage(connection, existingTabNames);
            var pvsSiteCopy = pvsSite != null ? pvsSite.Clone() : null;
            editPage.SetXenObjects(pvsSite, pvsSiteCopy);
            editPage.Changed += SomethingChangedOnPage;
            editPage.DeleteButtonClicked += DeleteButtonClickedOnPage;
            ShowTab(editPage);
            RefreshButtons();
            return editPage;
        }

        private void ShowTab(IEditPage editPage)
        {
            var pageAsControl = (Control)editPage;
            ContentPanel.Controls.Add(pageAsControl);
            pageAsControl.BackColor = Color.Transparent;
            pageAsControl.Dock = DockStyle.Fill;

            verticalTabs.Items.Add(editPage);
        }
        
        void DeletePage(PvsCacheConfigurationPage page)
        {
            // try to delete the site (asks user for confirmation), also passing the site name, in case it has changed 
            if (!DeleteSite(page.PvsSite, page.Text)) 
                return;
            int selectedIndex = verticalTabs.SelectedIndex;
            verticalTabs.Items.Remove(page);
            verticalTabs.SelectedIndex = selectedIndex < verticalTabs.Items.Count - 1 ? selectedIndex : verticalTabs.Items.Count - 1;
            page.Changed -= SomethingChangedOnPage;
            page.DeleteButtonClicked -= DeleteButtonClickedOnPage;
            ContentPanel.Controls.Remove(page);
            RefreshButtons();
            ResizeVerticalTabs(verticalTabs.Items.Count);
        }

        private bool DeleteSite(PVS_site site, string siteName)
        {
            // We cannot delete the site if there are running proxies
            if (site != null)
            {    
                var pvsProxies = connection.Cache.PVS_proxies.Where(s => s.site.opaque_ref == site.opaque_ref).ToList();
                if (pvsProxies.Count > 0)
                {
                    using (var dlg = 
                        new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.PVS_SITE_CANNOT_BE_REMOVED, Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(Parent);
                    }
                    return false;
                }
            }

            // show confirmation dialog
            var message = site != null && !string.IsNullOrEmpty(site.PVS_uuid)
                ? string.Format(Messages.CONFIRM_DELETE_PVS_SITE_IN_USE, siteName)
                : string.Format(Messages.CONFIRM_DELETE_PVS_SITE, siteName);
            DialogResult dialogResult;
            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, message, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonOK,
                    ThreeButtonDialog.ButtonCancel))
            {
                dialogResult = dlg.ShowDialog(Parent);
            }
            if (dialogResult != DialogResult.OK)
                return false;

            // if it is a newly added site, then there's noting we need to do here (it does not exist on the server yet)
            if (site == null)
                return true;

            // delete existing site
            var action = new DeletePvsSiteAction(site);
            new ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog(this);
            return action.Succeeded;
        }

        private void ResizeVerticalTabs(int itemCount)
        {
            int maxHeight = splitContainer.Panel1.Height - AddButton.Height;
            verticalTabs.Height = Math.Min(maxHeight, itemCount * verticalTabs.ItemHeight);
            AddButton.Top = verticalTabs.Top + verticalTabs.Height;
        }

        private void SomethingChangedOnPage(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void DeleteButtonClickedOnPage(object sender, EventArgs e)
        {
            var page = sender as PvsCacheConfigurationPage;
            if (page != null)
            {
                DeletePage(page);
            }
        }

        void RefreshButtons()
        {
            okButton.Enabled = AllPagesValid();
            addSiteButton.Visible = verticalTabs.Items.Count == 0;
        }

        private bool AllPagesValid()
        {
            return verticalTabs.Items.Cast<PvsCacheConfigurationPage>().All(page => page.ValidToSave);
        }
        
        private void splitContainer_Panel1_Resize(object sender, EventArgs e)
        {
            ResizeVerticalTabs(verticalTabs.Items.Count);
        }

        private void verticalTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= verticalTabs.Items.Count)
                return;

            PvsCacheConfigurationPage page = verticalTabs.Items[e.Index] as PvsCacheConfigurationPage;
            if (page == null)
                return;

            Graphics g = e.Graphics;
            Rectangle b = e.Bounds;

            // draw Delete icon
            Image deleteIcon = Properties.Resources._000_Abort_h32bit_16;
            if (deleteIcon != null)
            {
                page.DeleteIconBounds = new Rectangle(b.Right - deleteIcon.Width - ((32 - deleteIcon.Width) / 2),
                    b.Y + ((32 - deleteIcon.Height) / 2), deleteIcon.Width, deleteIcon.Height);
                g.DrawImage(deleteIcon, page.DeleteIconBounds);

            }
        }

        private bool MouseIsOnDeleteIcon(Point mouseLocation)
        {
            int pageIndex = verticalTabs.IndexFromPoint(mouseLocation);
            if (pageIndex < 0)
                return false;

            PvsCacheConfigurationPage page = verticalTabs.Items[pageIndex] as PvsCacheConfigurationPage;
            if (page == null)
                return false;

            var bounds = page.DeleteIconBounds;
            return bounds.Contains(mouseLocation);
        }
        
        private void verticalTabs_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseIsOnDeleteIcon(e.Location))
                ShowTooltip(e.Location);
            else
                HideTooltip();
        }

        private void verticalTabs_MouseClick(object sender, MouseEventArgs e)
        {

            int pageIndex = verticalTabs.IndexFromPoint(e.Location);
            if (pageIndex < 0 || !MouseIsOnDeleteIcon(e.Location))
                return;

            PvsCacheConfigurationPage page = verticalTabs.Items[pageIndex] as PvsCacheConfigurationPage;
            if (page != null)
            {
                DeletePage(page);
                HideTooltip();
            }
        }

        private readonly ToolTip toolTipRemove = new ToolTip();
        private bool tooltipVisible;

        private void ShowTooltip(Point location)
        {
            if (!tooltipVisible)
            {
                toolTipRemove.Show(Messages.REMOVE, verticalTabs, location.X, location.Y + 20);
                tooltipVisible = true;
                Cursor = Cursors.Hand;
            }
        }

        private void HideTooltip()
        {
            toolTipRemove.Hide(verticalTabs);
            tooltipVisible = false;
            Cursor = Cursors.Default;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ResizeVerticalTabs(verticalTabs.Items.Count + 1);
            verticalTabs.SelectedItem = NewPage(null);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            List<AsyncAction> actions = GetActions();
            
            if (actions.Count == 0) 
                return;

            var objName = Helpers.GetName(connection).Ellipsise(50);
            var multipleAction = new MultipleAction(
                connection,
                string.Format(Messages.UPDATE_PROPERTIES, objName),
                Messages.UPDATING_PROPERTIES,
                 string.Format(Messages.UPDATED_PROPERTIES, objName),
                actions, true);
            
            multipleAction.RunAsync();
        }

        private List<AsyncAction> GetActions()
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            foreach (IEditPage editPage in verticalTabs.Items)
            {
                if (!editPage.HasChanged)
                    continue;

                AsyncAction action = editPage.SaveSettings();
                if (action != null)
                    actions.Add(action);
            }

            return actions;
        }

        private void PvsSiteCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                PVS_site addedSite = e.Element as PVS_site;

                Program.Invoke(this, () =>
                {
                    ResizeVerticalTabs(verticalTabs.Items.Count + 1);
                    NewPage(addedSite);
                });
            }
            
        }

        private void PvsCacheConfigurationDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.connection.Cache.DeregisterCollectionChanged<PVS_site>(PvsSiteCollectionChanged);
        }
    }
}
