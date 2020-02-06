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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Plugins;


namespace XenAdmin.Dialogs.OptionsPages
{
    internal partial class PluginOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PluginManager _pluginManager;
        public PluginManager PluginManager
        {
            set
            {
                _pluginManager = value;

                if (_pluginManager.Plugins.Count > 0)
                {
                    label2.Visible = m_gridPlugins.Visible = label3.Visible = m_gridFeatures.Visible = true;
                    LoadPluginList();
                }
                else
                {
                    label2.Visible = m_gridPlugins.Visible = label3.Visible = m_gridFeatures.Visible = false;
                    labelNoPlugins.Visible = true;
                }
                Refresh();
            }
        }

        public PluginOptionsPage()
        {
            InitializeComponent();
            m_tlpScanning.Visible = false;
            labelNoPlugins.Visible = false;
            this.linkLabel1.Visible = !XenAdmin.Core.HiddenFeatures.LinkLabelHidden;
        }

        public static void Log()
        {
            log.InfoFormat("=== DisabledPlugins: {0}",
                Properties.Settings.Default.DisabledPlugins.Length == 0
                    ? "<None>"
                    : string.Join(",", Properties.Settings.Default.DisabledPlugins));
        }

        #region Private methods

        private void LoadPluginList()
        {
            if (_pluginManager.Plugins.Count > 0)
            {
                try
                {
                    m_gridPlugins.SuspendLayout();
                    m_gridPlugins.Rows.Clear();

                    foreach (var plugin in _pluginManager.Plugins)
                    {
                        var row = new PluginRow(plugin);
                        if (row.HasErrors)
                            row.DefaultCellStyle = new DataGridViewCellStyle
                                                       {
                                                           ForeColor = SystemColors.GrayText,
                                                           SelectionForeColor = SystemColors.GrayText,
                                                           SelectionBackColor = SystemColors.ControlLight
                                                       };
                        m_gridPlugins.Rows.Add(row);
                    }
                }
                finally
                {
                    m_gridPlugins.ResumeLayout();
                }

                label2.Visible = m_gridPlugins.Visible = label3.Visible = m_gridFeatures.Visible = true;
            }
            else
            {
                label2.Visible = m_gridPlugins.Visible = label3.Visible = m_gridFeatures.Visible = false;
                labelNoPlugins.Visible = true;
            }
            Refresh();
        }

        #endregion

        #region Control event handlers

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refreshButton.Enabled = false;
            labelNoPlugins.Visible = label2.Visible = m_gridPlugins.Visible = label3.Visible = m_gridFeatures.Visible = false;
            m_tlpScanning.Visible = true;
            Refresh();

            _pluginManager.ReloadPlugins();
            m_tlpScanning.Visible = false;
            LoadPluginList();
            refreshButton.Enabled = true;
            Refresh();
        }


        private void m_gridPlugins_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                m_gridFeatures.SuspendLayout();
                m_gridFeatures.Rows.Clear();

                if (m_gridPlugins.SelectedRows.Count < 1)
                    return;

                var pluginRow = m_gridPlugins.SelectedRows[0] as PluginRow;
                if (pluginRow == null)
                    return;

                var plugin = pluginRow.Plugin;

                //plugin version
                var version = string.IsNullOrEmpty(plugin.Version.ToString()) ? Messages.NONE : plugin.Version.ToString();
                m_gridFeatures.Rows.Add(new object[] {Messages.PLUGIN_VERSION, version});

                //plugin copyright
                var copyright = string.IsNullOrEmpty(plugin.Copyright) ? Messages.NONE : plugin.Copyright;
                m_gridFeatures.Rows.Add(new object[] {Messages.PLUGIN_COPYRIGHT, copyright});

                //plugin link
                var linkRow = new DataGridViewRow();
                linkRow.Cells.Add(new DataGridViewTextBoxCell { Value = Messages.PLUGIN_LINK });
                
                if (string.IsNullOrEmpty(plugin.Link))
                    linkRow.Cells.Add(new DataGridViewTextBoxCell { Value = Messages.NONE });
                else
                    linkRow.Cells.Add(new DataGridViewLinkCell { Value = plugin.Link });
                
                m_gridFeatures.Rows.Add(linkRow);

                //feattures
                if (plugin.Features.Count > 0)
                {
                    var row = new DataGridViewRow();
                    row.Cells.AddRange(new DataGridViewCell[]
                                           {
                                               new DataGridViewTextBoxCell
                                                   {
                                                       Value = Messages.PLUGIN_FEATURES,
                                                       Style = new DataGridViewCellStyle {Font = new Font(m_gridFeatures.Font, FontStyle.Bold)}
                                                   },
                                               new DataGridViewLinkCell {Value = ""}
                                           });
                    m_gridFeatures.Rows.Add(row);

                    foreach (var feature in plugin.Features)
                    {
                        var label = string.IsNullOrEmpty(feature.Label) ? feature.Name : feature.Label;
                        m_gridFeatures.Rows.Add(new object[] {label, feature.Description});
                    }
                }
            }
            finally
            {
                m_gridFeatures.ResumeLayout();
            }
        }

        private void m_gridPlugins_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= m_gridPlugins.RowCount)
                return;

            var row = m_gridPlugins.Rows[e.RowIndex] as PluginRow;
            if (row == null)
                return;

            if (!row.HasErrors)
                row.Cells[0].Value = !(bool)row.Cells[0].Value;
        }

        private void m_gridPlugins_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex > m_gridPlugins.ColumnCount || e.RowIndex < 0 || e.RowIndex >= m_gridPlugins.RowCount)
                return;
            
            var row = m_gridPlugins.Rows[e.RowIndex] as PluginRow;
            if (row == null)
                return;

            if (row.HasErrors)
                row.Cells[e.ColumnIndex].ToolTipText = row.Error;
        }


        private void m_gridFeatures_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1 || e.RowIndex < 0 || e.RowIndex >= m_gridPlugins.RowCount)
                return;

            Program.OpenURL((string)m_gridFeatures.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        private void m_gridFeatures_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if ((e.Row.State & DataGridViewElementStates.Selected) != 0)
                e.Row.Selected = false;
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.OpenURL(InvisibleMessages.PLUGINS_URL);
        }

        #endregion

        #region Implementation of IVerticalTab

        public override string Text { get { return Messages.PLUGINS; } }

        public string SubText
        {
            get
            {
                return _pluginManager.EnabledPluginsCount == 1
                           ? Messages.PLUGIN_ENABLED_COUNT_ONE
                           : string.Format(Messages.PLUGIN_ENABLED_COUNT, _pluginManager.EnabledPluginsCount);
            }
        }

        public Image Image { get { return Properties.Resources._000_Module_h32bit_16; } }

        #endregion

        #region Implementation of IOptionsPage

        public void Save()
        {
            var disabled = new List<string>();

            foreach (DataGridViewRow row in m_gridPlugins.Rows)
            {
                var pluginRow = row as PluginRow;
                if (pluginRow == null)
                    continue;

                bool isChecked = (bool)pluginRow.Cells[0].Value;
                pluginRow.Plugin.Enabled = isChecked;

                if (!isChecked)
                    disabled.Add(string.Format("{0}::{1}", pluginRow.Plugin.Organization, pluginRow.Plugin.Name));
            }

            Settings.UpdateDisabledPluginsList(disabled);
            _pluginManager.OnPluginsChanged();
        }

        #endregion

        #region Nested Items

        private class PluginRow : DataGridViewRow
        {
            public PluginRow(PluginDescriptor plugin)
            {
                Plugin = plugin;
                Error = plugin.Error;
                HasErrors = !string.IsNullOrEmpty(plugin.Error);
                Cells.AddRange(new DataGridViewCell[]
                                   {
                                       new DataGridViewCheckBoxCell {Value = plugin.Enabled && !HasErrors},
                                       new DataGridViewTextBoxCell {Value = string.IsNullOrEmpty(plugin.Label) ? plugin.Name : plugin.Label},
                                       new DataGridViewTextBoxCell {Value = plugin.Organization},
                                       new DataGridViewTextBoxCell {Value = plugin.Description}
                                   });
            }

            public PluginDescriptor Plugin { get; private set; }

            public string Error { get; private set; }

            public bool HasErrors { get; private set; }
        }

        #endregion
    }
}