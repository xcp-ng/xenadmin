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
using System.Xml;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Controls;
using XenAPI;


namespace XenAdmin.TabPages
{
    internal partial class DockerProcessPage : BaseTabPage
    {
        private const int REFRESH_INTERVAL = 20000;

        private DockerContainer container;
        private VM parentVM;
        private Host host;
        
        private readonly ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

        public DockerProcessPage()
        {
            InitializeComponent();
            listView1.ListViewItemSorter = lvwColumnSorter;
            base.Text = Messages.DOCKER_PROCESS_TAB_TITLE;
            RefreshTimer.Interval = REFRESH_INTERVAL;
        }

        public override string HelpID => "TabPageDockerProcess";

        public DockerContainer DockerContainer
        {
            get
            {
                Program.AssertOnEventThread();
                return container;
            }
            set
            {
                Program.AssertOnEventThread();
                RefreshButton.Enabled = true;

                if (value == null) return;

                if (container == null || !container.Equals(value))
                {
                    container = value;
                    if (container.Connection == null || container.Connection.Session == null)
                        return;

                    parentVM = container.Parent;

                    if (parentVM == null)
                        return;

                    host = container.Connection.Resolve(parentVM.resident_on);

                    listView1.Items.Clear();
                    labelRefresh.Text = Messages.LAST_REFRESH_IN_PROGRESS;
                    StartUpdating();
                }
            }
        }

        private void StartUpdating()
        {
            var args = new Dictionary<string, string>();
            args["vmuuid"] = parentVM.uuid;
            args["object"] = container.Name();

            var action = new ExecuteContainerPluginAction(container, host,
                        "xscontainer", "get_top", args, true); 

            action.Completed += action_Completed;
            action.RunAsync();
        }

        private void action_Completed(ActionBase sender)
        {
            var action = sender as ExecuteContainerPluginAction;
            if (action == null || action.Container != container)
                return;
            Program.Invoke(Program.MainWindow, () =>
            {
                if (action.Succeeded)
                    UpdateList(action.Result);
                else
                    ShowInvalidInfo();
                RefreshButton.Enabled = true;
            });
        }

        private void UpdateList(string xmlResult)
        {
            // Parse the XML result
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlResult);

                string pid = "";
                string command = "";
                string cpuTime = "";
                string[] row = {"", "", ""};

                listView1.SuspendLayout();
                listView1.Items.Clear();
                XmlNodeList processList = xmlDoc.GetElementsByTagName("Process");
                foreach (XmlNode process in processList)
                {
                    if (process.HasChildNodes)
                    {
                        foreach (XmlNode child in process.ChildNodes)
                        {
                            XmlNode v = child.FirstChild;
                            if (child.Name.Equals("PID"))
                                pid = v.Value;
                            else if (child.Name.Equals("CMD"))
                                command = v.Value;
                            else if (child.Name.Equals("TIME"))
                                cpuTime = v.Value;
                        }

                        row[0] = pid;
                        row[1] = command;
                        row[2] = cpuTime;
                    }
                    listView1.Items.Add(new ListViewItem(row));
                }
                labelRefresh.Text = string.Format(Messages.LAST_REFRESH_SUCCESS,
                                                  HelpersGUI.DateTimeToString(DateTime.Now, Messages.DATEFORMAT_HMS, true));
            }
            catch (Exception)
            {
                ShowInvalidInfo();
            }
            finally
            {
                listView1.ResumeLayout();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            labelRefresh.Text = Messages.LAST_REFRESH_IN_PROGRESS;
            RefreshButton.Enabled = false; 
            StartUpdating();
        }

        private void ShowInvalidInfo()
        {
            labelRefresh.Text = Messages.LAST_REFRESH_FAIL;
            listView1.Items.Clear();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            StartUpdating();
        }

        // Sort by column. Refer to the implementation of PhysicalStoragePage.
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            listView1.Sort();
        }

        public void PauseRefresh()
        {
            RefreshTimer.Enabled = false;
        }

        public void ResumeRefresh()
        {
            RefreshTimer.Enabled = true;
        }
    }
}
