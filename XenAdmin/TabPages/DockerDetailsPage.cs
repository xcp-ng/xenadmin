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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Model;
using System.Xml;
using System.Collections;

namespace XenAdmin.TabPages
{
    public partial class DockerDetailsPage : BaseTabPage
    {
        private const int REFRESH_INTERVAL = 20000;

        private DockerContainer container;
        private VM parentVM;
        private Host host;
        private string cachedResult;

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

                if (value == null)
                    return;

                if (container != value)
                {
                    container = value;
                    parentVM = container.Parent;
                    if (parentVM.resident_on == null || string.IsNullOrEmpty(parentVM.resident_on.opaque_ref) ||
                        (parentVM.resident_on.opaque_ref.ToLower().Contains("null")))
                        return;

                    host = container.Connection.Resolve(parentVM.resident_on);

                    DetailtreeView.Nodes.Clear();

                    RefreshTime.Text = Messages.LAST_REFRESH_IN_PROGRESS;
                    StartUpdating();
                }
            }
        }

        public override string HelpID => "TabPageDockerDetails";

        private void StartUpdating()
        {
            var args = new Dictionary<string, string>();
            args["vmuuid"] = parentVM.uuid;
            args["object"] = container.uuid;

            var action = new ExecuteContainerPluginAction(container, host,
                        "xscontainer", "get_inspect", args, true);

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
                    Rebuild(action.Result);
                else
                    ShowInvalidInfo();
                RefreshButton.Enabled = true;
            });
        }

        private void CreateTree(XmlNode node, TreeNode rootNode)
        {
            Program.AssertOnEventThread();

            if (node.NodeType == XmlNodeType.Text)
                rootNode.Text = node.Value;
            else
            {
                if (node.Name == "SPECIAL_XS_ENCODED_ELEMENT" && node.Attributes != null)
                {
                    rootNode.Text = node.Attributes["name"].Value;
                }
                else
                    rootNode.Text = node.Name;
            }
            IEnumerator ienum = node.GetEnumerator();
            while (ienum.MoveNext())
            {
                XmlNode current = (XmlNode)ienum.Current;
                TreeNode currentNode = new TreeNode();
                CreateTree(current, currentNode);
                rootNode.Nodes.Add(currentNode);
            }
        }

        public void Rebuild(string currentResult)
        {
            Program.AssertOnEventThread();
            RefreshTime.Text = string.Format(Messages.LAST_REFRESH_SUCCESS,
                                             HelpersGUI.DateTimeToString(DateTime.Now, Messages.DATEFORMAT_HMS, true));
            try
            {
                if (cachedResult == currentResult)
                    return;

                cachedResult = currentResult;
                DetailtreeView.Nodes.Clear();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(currentResult);
                
                IEnumerator firstEnum = doc.GetEnumerator();
                XmlNode node;
                while (firstEnum.MoveNext())
                {
                    node = (XmlNode)firstEnum.Current;

                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        //we are on the root element now (docker_inspect)
                        //using the following enumerator to iterate through the children nodes and to build related sub-trees
                        //note that we are intentionally not adding the root node to the tree (UX decision)
                        var secondEnum = node.GetEnumerator();
                        while (secondEnum.MoveNext())
                        {
                            //recursively building the tree
                            TreeNode rootNode = new TreeNode();
                            CreateTree((XmlNode)secondEnum.Current, rootNode);

                            //adding the current sub-tree to the TreeView
                            DetailtreeView.Nodes.Add(rootNode);
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowInvalidInfo();
            }
        }

        public DockerDetailsPage()
        {
            InitializeComponent();
            base.Text = Messages.DOCKER_DETAIL_TAB_TITLE;
            RefreshTimer.Interval = REFRESH_INTERVAL;
        }

        private void ShowInvalidInfo()
        {
            RefreshTime.Text = Messages.LAST_REFRESH_FAIL;
            DetailtreeView.Nodes.Clear();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshTime.Text = Messages.LAST_REFRESH_IN_PROGRESS;
            RefreshButton.Enabled = false;
            StartUpdating();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            StartUpdating();
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
