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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Web;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Dialogs;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Help;
using XenAdmin.Controls.Wlb;
// Report viewer control dependencies
using Microsoft.Reporting.WinForms;


namespace XenAdmin
{
    public delegate void CustomRefreshEventHandler(object sender, System.EventArgs e);

    public partial class WorkloadReports : XenDialogBase
    {
        #region Private Variables

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IEnumerable<Host> _hosts;
        private Pool _pool;
        private WlbReportSubscriptionCollection _subscriptionCollection;
        private string _reportFile;
        private bool _runReport;
        private bool _isCreedenceOrLater;

        private List<string> _midnightRideReports = new List<string>(new [] 
                                                                    { 
                                                                        Messages.WLBREPORT_POOL_AUDIT_HISTORY,
                                                                        Messages.WLBREPORT_POOL_OPTIMIZATION_HISTORY
                                                                    });

        #endregion


        #region Properties

        /// <summary>
        /// Object set to current  pool
        /// </summary>
        public Pool Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }

        /// <summary>
        /// Collection of hosts for the current pool
        /// </summary>
        public IEnumerable<Host> Hosts
        {
            get { return _hosts; }
            set { _hosts = value; }
        }


        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WorkloadReports() : this(string.Empty, false) {}

        /// <summary>
        /// Overloaded constructor provides access to a specific report upon load
        /// </summary>
        public WorkloadReports(string reportFile, bool run)
        {
            InitializeComponent();
            _reportFile = reportFile;
            _runReport = run;
        }

        #endregion

        internal override string HelpName
        {
            get { return "WLBReports"; }
        }

        #region Private Methods

        /// <summary>
        /// Populates the treeview with ReportInfo and SubscriptionInfo nodes
        /// </summary>
        private void SetTreeViewReportList()
        {
            bool errorLoading = false;

            // Prep treeview for population
            treeViewReportList.BeginUpdate();
            treeViewReportList.Nodes.Clear();

            // Set up the image list for the tree
            this.treeViewReportList.ImageList = CreateReportImageList();

            //_subscriptionCollection = null;

            try
            {
                // retrieve subscription
                SetSubscriptionCollection();

                this.wlbReportView1.btnSubscribe.Visible = false;
                this.wlbReportView1.btnLaterReport.Visible = false;
                this.wlbReportView1.IsCreedenceOrLater = _isCreedenceOrLater;
                PopulateTreeView();
            }
            catch (XenAdmin.CancelledException xc)
            {
                // User cancelled entering credentials when prompted by action
                log.Debug(xc);
                errorLoading = true;
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.WLBREPORT_REPORT_CONFIG_ERROR, Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
                errorLoading = true;
            }
            finally
            {
                if ((treeViewReportList != null) && (!errorLoading))
                    treeViewReportList.EndUpdate();
                else
                    this.Close();
            }
        }


        /// <summary>
        /// Populates the subscription list with items of type WlbReportSubscription
        /// </summary>
        private void SetTreeViewSubscriptionList()
        {
            // Prep treeview for population
            treeViewSubscriptionList.BeginUpdate();
            treeViewSubscriptionList.Nodes.Clear();

            treeViewSubscriptionList.ImageList = CreateReportImageList();

            if (_subscriptionCollection != null)
            {
                this.lblSubscriptions.Visible = true;
                this.treeViewSubscriptionList.Visible = true;
                foreach (string key in _subscriptionCollection.Keys)
                {
                    treeViewSubscriptionList.Nodes.Add(GetReportSubscriptionTreeNode(_subscriptionCollection[key]));
                }
            }
            else
            {
                this.lblSubscriptions.Visible = false;
                this.treeViewSubscriptionList.Visible = false;
            }
            treeViewSubscriptionList.EndUpdate();
        }


        /// <summary>
        /// Used by drilldown and back events from the reportviewer to maintain treeview state
        /// </summary>
        private void SynchTreeViewReportList(string reportName)
        {
            WlbReportInfo reportInfo;
            wlbReportView1.ResetReportViewer = false;

            string reportFile = String.Format("{0}.{1}", reportName, "rdlc");

            for (int i = 0; i < treeViewReportList.Nodes.Count; i++)
            {
                if ((treeViewReportList.Nodes[i].Tag != null) &&
                    (treeViewReportList.Nodes[i].Tag.GetType() == typeof(WlbReportInfo)))
                {
                    reportInfo = (WlbReportInfo)treeViewReportList.Nodes[i].Tag;

                    if (reportInfo.ReportFile == reportFile)
                    {
                        treeViewReportList.SelectedNode = treeViewReportList.Nodes[i];
                        treeViewReportList.Select();
                        wlbReportView1.ViewerReportInfo = reportInfo;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Update report treeView when edit/add/delete report subscriptions
        /// </summary>
        private void UpdateReportTreeView()
        {
            // Begin update report treeView
            treeViewReportList.BeginUpdate();

            // If there is a selected subscription node, need to save it before it got cleared
            // There is no need to save selected report node since it never got cleared
            WlbReportSubscription selectedSub = null;
            String selectedSubReportFile = String.Empty;
            if (this.treeViewReportList.SelectedNode != null && this.treeViewReportList.SelectedNode.Tag != null && this.treeViewReportList.SelectedNode.Tag.GetType() == typeof(WlbReportSubscription))
            {
                // Save selected subscription node for later
                selectedSub = (WlbReportSubscription)this.treeViewReportList.SelectedNode.Tag;

                // Save the report file of the selected subscription node for later
                selectedSubReportFile = ((WlbReportInfo)this.treeViewReportList.SelectedNode.Parent.Parent.Tag).ReportFile;
            }

            // Update subscription nodes
            foreach (TreeNode reportNode in this.treeViewReportList.Nodes)
            {
                // Get subscriptions under the report node
                Dictionary<string, WlbReportSubscription> subscriptionList = _subscriptionCollection.GetReportSubscriptionByReportName(((WlbReportInfo)reportNode.Tag).ReportFile);

                // If there's subscriptions for the current report, add them
                if (subscriptionList.Count > 0)
                {
                    // Add a subscriptions folder if there isn't one
                    if (reportNode.GetNodeCount(true) == 0)
                    {
                        TreeNode subscriptionFolderNode = new TreeNode(Messages.WLB_SUBSCRIPTIONS, 2, 2);
                        reportNode.Nodes.Add(subscriptionFolderNode);
                    }

                    // Delete all the nodes to build the updated list
                    reportNode.Nodes[0].Nodes.Clear();

                    // Add in the latest nodes
                    if (selectedSub != null && String.Compare(selectedSubReportFile, ((WlbReportInfo)reportNode.Tag).ReportFile, true) == 0)
                    {
                        // Add new subscription node and retrieve selected node if there is one
                        AddNewSubscriptionNode(reportNode.Nodes[0], subscriptionList, ((WlbReportInfo)reportNode.Tag).ReportName, selectedSub);
                    }
                    else
                    {
                        // Add new subscription node
                        AddNewSubscriptionNode(reportNode.Nodes[0], subscriptionList, ((WlbReportInfo)reportNode.Tag).ReportName, null);
                    }
                }
                else
                {
                    // Force to select reportNode before clear reportNode's children node
                    // only if the original selected subscription has been deleted 
                    // and there are no more subscriptions under the report
                    if (selectedSub != null && String.Compare(selectedSubReportFile, ((WlbReportInfo)reportNode.Tag).ReportFile, true) == 0)
                    {
                        this.treeViewReportList.SelectedNode = reportNode;
                    }

                    // Clear reportNode's children node (subscription folder node) 
                    // if there is no subscription under the reportNode
                    reportNode.Nodes.Clear();
                }
            }

            // End updating report treeView
            treeViewReportList.EndUpdate();
        }


        /// <summary>
        /// Update subscriptionTreeView when Add/Edit/Delete subscription
        /// </summary>
        private void UpdateSubscriptionTreeView()
        {
            // Begin update subscription treeView
            treeViewSubscriptionList.BeginUpdate();

            // Save selected node if there is one
            WlbReportSubscription selectedSub = null;
            if (treeViewSubscriptionList.SelectedNode != null)
            {
                selectedSub = (WlbReportSubscription)treeViewSubscriptionList.SelectedNode.Tag;
            }

            // Delete all nodes
            treeViewSubscriptionList.Nodes.Clear();

            // Add subscription nodes if there is any
            if (_subscriptionCollection.Count > 0)
            {
                foreach (string key in _subscriptionCollection.Keys)
                {
                    TreeNode subNode = GetReportSubscriptionTreeNode(_subscriptionCollection[key]);
                    treeViewSubscriptionList.Nodes.Add(subNode);

                    // Retrieve selected node if there is one
                    if (selectedSub != null && String.Compare(_subscriptionCollection[key].Id, selectedSub.Id, true) == 0)
                    {
                        treeViewSubscriptionList.SelectedNode = subNode;
                    }
                }

                // Force to set selected node to the first node 
                // only if treeViewSubscriptionList has selected subscription node before update
                // but the selected subscription doesn't exist anymore
                if (selectedSub != null && treeViewSubscriptionList.SelectedNode == null)
                {
                    treeViewSubscriptionList.SelectedNode = treeViewSubscriptionList.Nodes[0];
                }
            }
            else if (selectedSub != null)
            {
                // No nodes left in the tree so select the current report in the reportviewer tree
                foreach (TreeNode reportNode in this.treeViewReportList.Nodes)
                {
                    if (((WlbReportInfo)reportNode.Tag).ReportName == selectedSub.ReportDisplayName)
                    {
                        treeViewReportList.SelectedNode = reportNode;
                        break;
                    }
                }
            }

            // End update subscription treeView
            treeViewSubscriptionList.EndUpdate();
        }


        /// <summary>
        /// Adds an image list to the current treeview for use by each of the nodes types
        /// </summary>
        private ImageList CreateReportImageList()
        {
            ImageList reportImageList;

            reportImageList = new ImageList();

            reportImageList.Images.Add(XenAdmin.Properties.Resources._000_GetServerReport_h32bit_16);
            reportImageList.Images.Add(XenAdmin.Properties.Resources.subscribe);
            reportImageList.Images.Add(XenAdmin.Properties.Resources._000_Folder_open_h32bit_16);

            return reportImageList;
        }


        /// <summary>
        /// Retrieve subscriptions and set _subscriptionCollection
        /// </summary>
        private void SetSubscriptionCollection()
        {
            _subscriptionCollection = null;

            WlbPoolConfiguration poolConfiguration;
            RetrieveWlbConfigurationAction action = new RetrieveWlbConfigurationAction(_pool);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);    
            }

            if (action.Succeeded)
            {
                poolConfiguration = new WlbPoolConfiguration(action.WlbConfiguration);

                _isCreedenceOrLater = poolConfiguration.IsCreedenceOrLater;

                this.splitContainerLeftPane.Panel2Collapsed = true;
                this.wlbReportView1.btnSubscribe.Visible=false;
            }
            else
            {
                throw (action.Exception);
            }
        }


        /// <summary>
        /// Populate report treeView on initial load report window
        /// </summary>
        private void PopulateTreeView()
        {

            XmlNodeList currentNodes;
            XmlDocument xmlReportDoc = new XmlDocument();

            // Attempt to get the latest set of reports from the WLB server.  WLB version 2.0
            // and beyond will respond to this.
            xmlReportDoc = GetServerReportsConfig();

            // No report definitons on WLB server, obtain them locally (the old way)
            if ((!xmlReportDoc.HasChildNodes) || (!xmlReportDoc.DocumentElement.HasChildNodes))
                xmlReportDoc = GetLocalReportsConfig();

            // Get each of the reports from the config and load them into combo box
            currentNodes = xmlReportDoc.SelectNodes(@"Reports/Report");

            // pupulate treeNode
            for (int i = 0; i < currentNodes.Count; i++)
            {
                // Adds the report node
                TreeNode currentReportTreeNode = GetReportTreeNode(currentNodes[i]);
                treeViewReportList.Nodes.Add(currentReportTreeNode);

                // Force to highlight the proper report treeNode if WLBReportWindow is called from WLB tab
                if (!String.IsNullOrEmpty(_reportFile))
                {
                    string currentReportFile = ((WlbReportInfo)currentReportTreeNode.Tag).ReportFile.Split('.')[0];
                    if (String.Compare(currentReportFile, _reportFile, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        treeViewReportList.SelectedNode = currentReportTreeNode;
                        _reportFile = String.Empty;
                    }
                }

            }

            treeViewReportList.Sort();
        }


        /// <summary>
        /// Obtains report configuration from local report.xml definition
        /// This method is only utilized when WLB server is version 1.0 and 1.1.  
        /// Subsequent versions utilize GetReportConfig(XmlDocument)
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetLocalReportsConfig()
        {
            XmlDocument xmlReportsDoc = new XmlDocument();

            // Load up report info from XML file
            xmlReportsDoc.Load(String.Format(@"{0}\{1}", Application.StartupPath.ToString(), "reports.xml"));

            return xmlReportsDoc;

        }


        /// <summary>
        /// Invokes and executes a call to the Kirkwood database via Xapi 
        /// to obtain report configuration data including the actual
        /// rdlc report definitions
        /// </summary>
        /// <returns>Report definition list XML document</returns>
        private XmlDocument GetServerReportsConfig()
        {

            string returnValue;
            XmlDocument xmlReportsDoc = new XmlDocument();
            string reportName = "get_report_definitions";

            // Parameters
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("LocaleCode", Program.CurrentLanguage);
            if (_isCreedenceOrLater)
            {
                parms.Add("ReportVersion", "Creedence");
                parms.Add("PoolId", _pool.uuid);
            }

            AsyncAction action = new WlbReportAction(_pool.Connection,
                                                Helpers.GetMaster(_pool.Connection),
                                                reportName,
                                                Messages.WLB_REPORT_DEFINITIONS,
                                                true,
                                                parms);

            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog();

            returnValue = action.Result;

            if ((action.Succeeded) && (!String.IsNullOrEmpty(returnValue)))
            {
                try
                {
                    xmlReportsDoc.LoadXml(returnValue);

                    string rdlcText;

                    foreach (XmlNode currentRdlc in xmlReportsDoc.SelectNodes(@"Reports/Report/Rdlc"))
                    {
                        rdlcText = currentRdlc.InnerText;
                        currentRdlc.InnerText = String.Empty;
                        currentRdlc.InnerText = rdlcText;
                    }
                }
                catch (Exception)
                {
                    xmlReportsDoc = null;
                }
            }

            return xmlReportsDoc;
        }

        // To enhance pool audit trail report, WLB server updates RDLC and would send user and object lists.
        // These values are sent along with parameter keys "AuditUser", "AuditObject".
        // The GetCustomXmlElement functions are used to retrieve the parameter values from WLB server.
        private static XmlElement GetCustomXmlElement(XmlElement root)
        {
            XmlNodeList labelNodes = root.GetElementsByTagName("Custom");
            if (labelNodes != null && labelNodes.Count > 0)
            {
                string xmlCustom = labelNodes[0].InnerXml;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<root>" + xmlCustom + "</root>");
                return xmlDoc.DocumentElement;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                string searchStart = "Function Custom() As String";
                string searchEnd = "\nEnd Function";
                string xmlDocString = root.InnerXml;
                int startPos = xmlDocString.IndexOf(searchStart, 0);
                if (startPos > -1)
                {
                    int endPos = xmlDocString.IndexOf(searchEnd, startPos);
                    string customString = xmlDocString.Substring(startPos + searchStart.Length, endPos - startPos - searchStart.Length);
                    customString = customString.Trim().Substring("Return".Length).Trim().Trim("\"".ToCharArray());
                    customString = HttpUtility.HtmlDecode(customString);
                    xmlDoc.LoadXml(customString);
                    return xmlDoc.DocumentElement;
                }
                else
                {
                    return null;
                }
            }
        }

        private static XmlElement GetCustomXmlElement(XmlNode root)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root>" + root.InnerXml + "</root>");
            return GetCustomXmlElement(xmlDoc.DocumentElement);
        }

        private static XmlElement GetCustomXmlElement(XmlElement root, string version)
        {
            XmlElement customXml = GetCustomXmlElement(root);
            if (customXml != null)
            {
                XmlNodeList versionNodes = customXml.GetElementsByTagName("Version");
                foreach (XmlNode thisNode in versionNodes)
                {
                    if (thisNode.Attributes["value"].Value == version)
                    {
                        XmlDocument thisDoc = new XmlDocument();
                        thisDoc.LoadXml("<root>" + thisNode.InnerXml + "</root>");
                        return thisDoc.DocumentElement;
                    }
                }
            }
            return null;
        }

        private static XmlElement GetCustomXmlElement(XmlElement root, string version, string tagName)
        {
            XmlElement customXml = GetCustomXmlElement(root, version);
            if (customXml != null)
            {
                XmlNodeList tagNodes = customXml.GetElementsByTagName(tagName);
                if (tagNodes != null && tagNodes.Count > 0)
                {
                    XmlDocument thisDoc = new XmlDocument();
                    thisDoc.LoadXml("<root>" + tagNodes[0].InnerXml + "</root>");
                    return thisDoc.DocumentElement;
                }
            }
            return null;
        }

        private static XmlElement GetCustomXmlElement(XmlNode root, string version, string tagName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root>" + root.InnerXml + "</root>");
            return GetCustomXmlElement(xmlDoc.DocumentElement, version, tagName);
        }


        /// <summary>
        /// Creates a node of type ReportTreeNode based on information specified from the XML report 
        /// configuration file.
        /// </summary>
        /// <param name="currentNode">Current report XML node from config file</param>
        /// <returns></returns>
        private TreeNode GetReportTreeNode(XmlNode currentNode)
        {
            string nodeFileName;
            string nodeNameLabel;
            string nodeReportDefinition;
            bool nodeDisplayHosts;
            bool nodeDisplayFilter;
            bool nodeDisplayUsers;
            bool nodeDisplayAuditObjects;
            OrderedDictionary nodeParamDict;
            WlbReportInfo reportInfo;
            TreeNode reportTreeNode;

            nodeFileName = currentNode.Attributes["File"].Value;

            XmlElement queryParametersXmlElement = GetCustomXmlElement(currentNode, "Creedence", "QueryParameters");

            // If the report definition node doesn't exist (old WLB version), load the definition from the 
            // local file system.  Otherwise, the definition is present in the config from the WLB server
            if (currentNode.SelectSingleNode(@"Rdlc") == null)
            {
                XmlDocument xmlReportDefinition = new XmlDocument();
                xmlReportDefinition.Load(String.Format(@"{0}\{1}", Application.StartupPath.ToString(), nodeFileName));
                nodeReportDefinition = xmlReportDefinition.OuterXml.ToString();
            }
            else
            {
                nodeReportDefinition = currentNode.SelectSingleNode(@"Rdlc").InnerText;
            }


            // If the report definition was obtained from the WLB server, use the localized name provided.
            // Otherwise, get the label locally.  If all else fails, just use NameLabel attribute from 
            // xml config 
            if (currentNode.Attributes["Name"] != null)
            {
                nodeNameLabel = currentNode.Attributes["Name"].Value;
            }
            else if (Messages.ResourceManager.GetObject(currentNode.Attributes["NameLabel"].Value) != null)
            {
                nodeNameLabel = Messages.ResourceManager.GetObject(currentNode.Attributes["NameLabel"].Value).ToString();
            }
            else
            {
                nodeNameLabel = currentNode.Attributes["NameLabel"].Value;
            }


            // Boolean variuable to determine the display the Filter drop down menu?
            if (currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='Filter']") == null)
            {
                nodeDisplayFilter = false;
            }
            else
            {
                nodeDisplayFilter = true;
            }


            // Boolean variuable to determine the display the Host drop down menu?
            if (currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='HostID']") == null)
            {
                nodeDisplayHosts = false;
            }
            else
            {
                nodeDisplayHosts = true;
            }

            // Boolean variable to determine the display of the User drop down menu
            if (currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='AuditUser']") == null)
            {
                nodeDisplayUsers = false;
            }
            else
            {
                nodeDisplayUsers = true;
            }

            // Boolean variable to determine the display of the Object drop down menu
            if (currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='AuditObject']") == null)
            {
                nodeDisplayAuditObjects = false;
            }
            else
            {
                nodeDisplayAuditObjects = true;
            }

            // Get a list of query params
            nodeParamDict = GetSQLQueryParamNames(currentNode, queryParametersXmlElement);

            // Create a report node and add it to the treeview for the current report
            reportInfo = new WlbReportInfo(nodeNameLabel,
                                           nodeFileName,
                                           nodeReportDefinition,
                                           nodeDisplayHosts,
                                           nodeDisplayFilter,
                                           nodeDisplayUsers,
                                           nodeDisplayAuditObjects,
                                           nodeParamDict);

            reportTreeNode = new TreeNode();
            reportTreeNode.Tag = reportInfo;
            reportTreeNode.Text = nodeNameLabel;
            reportTreeNode.ImageIndex = 0;
            reportTreeNode.SelectedImageIndex = 0;

            return reportTreeNode;
        }


        /// <summary>
        /// Creates a treeNode for give report name and subscription object
        /// </summary>
        /// <param name="subscription">Instance of a subscription</param>
        /// <returns>Return a instance of TreeNode</returns>
        private TreeNode GetReportSubscriptionTreeNode(WlbReportSubscription subscription)
        {
            TreeNode subscriptionTreeNode = new TreeNode();

            subscriptionTreeNode.Text = subscription.Name;
            subscriptionTreeNode.Tag = subscription;
            subscriptionTreeNode.ImageIndex = 1;
            subscriptionTreeNode.SelectedImageIndex = 1;

            return subscriptionTreeNode;
        }


        /// <summary>
        /// Returns a list of parameters names whose values will be required for reports' SQL query
        /// to execute.  These names are specified in the Report XML configuration file.
        /// </summary>
        /// <param name="currentNode">Current report XML node from config file</param>
        /// <param name="queryParametersXmlElement">Current report XML node from config file</param>
        /// <returns>OrderedDictionary of parameter names and contents whose values are required by the report SQL query</returns>
        private OrderedDictionary GetSQLQueryParamNames(XmlNode currentNode, XmlElement queryParametersXmlElement)
        {
            OrderedDictionary paramNames = new OrderedDictionary();
            XmlNodeList paramNameNodes = currentNode.SelectNodes("QueryParameters/QueryParameter");

            foreach (XmlNode paramNode in paramNameNodes)
            {
                if (paramNode.Attributes["Name"] != null)
                {
                    string value = paramNode.Attributes["Name"].Value;
                    // To implement the AuditUser and AuditObject dropdowns, the values are set
                    // along with parameter "AuditUser' and "AuditObject" from WLB server.
                    // If the query parameters contain values, store them in dictionary.
                    // Else just store the parameter as key with "" value.
                    if(_isCreedenceOrLater && queryParametersXmlElement != null)
                    {
                        XmlNodeList valueTagNodes = queryParametersXmlElement.GetElementsByTagName(value);
                        if (valueTagNodes != null && valueTagNodes.Count > 0)
                        {
                            paramNames.Add(value, valueTagNodes[0].InnerText);
                        }
                        else
                        {
                            paramNames.Add(value, "");
                        }
                    }
                    else
                    {
                        paramNames.Add(value, "");
                    }
                }
            }

            return paramNames;
        }


        /// <summary>
        /// Add subscription nodes if a report has subscriptions
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <param name="reportName">Report display name that can be added to WlbReportSubscription instance</param>
        /// <param name="selectedSub">The original selected node before update</param>
        /// <param name="subscriptionFolder"></param>
        private void AddNewSubscriptionNode(TreeNode subscriptionFolder, Dictionary<string, WlbReportSubscription> subscriptions, string reportName, WlbReportSubscription selectedSub)
        {
            foreach (WlbReportSubscription sub in subscriptions.Values)
            {
                sub.ReportDisplayName = reportName;
                TreeNode subNode = GetReportSubscriptionTreeNode(sub);
                subscriptionFolder.Nodes.Add(subNode);

                // Retrieve selected subscription node if there is one
                if (selectedSub != null && String.Compare(selectedSub.Id, sub.Id, true) == 0)
                {
                    this.treeViewReportList.SelectedNode = subNode;
                }
            }

            // Force to set selected node to the first subscription node of the report 
            // only if the original selected subscription node of the report has been deleted.
            if (selectedSub != null && subscriptions.Count > 0 && (this.treeViewReportList.SelectedNode == null || this.treeViewReportList.SelectedNode.Tag == null))
            {
                this.treeViewReportList.SelectedNode = subscriptionFolder.Nodes[0];
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Load report form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportForm_Load(object sender, EventArgs e)
        {
            // Add event handlers for creating/editing/deleting subscription
            this.subscriptionView1.OnChangeOK += new CustomRefreshEventHandler(OnChangeOK_Refresh);
            this.wlbReportView1.OnChangeOK += new CustomRefreshEventHandler(OnChangeOK_Refresh);
        }


        /// <summary>
        /// When the report window is displayed, the current pool and a list of hosts that
        /// correlate to the pool need to be set in the viewer control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WlbReportWindow_Shown(object sender, EventArgs e)
        {
            wlbReportView1.Pool = _pool;
            subscriptionView1.Pool = _pool;
            wlbReportView1.Hosts = _hosts;

            // Populate report treeview with report and subscription on the top of left panel
            SetTreeViewReportList();

            // Populate subscription treeview with subscription on the bottom of the left panel
            SetTreeViewSubscriptionList();
        }


        /// <summary>
        /// Update report treeView and subscription treeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChangeOK_Refresh(object sender, EventArgs e)
        {
            try
            {
                // set _subscriptionCollection
                SetSubscriptionCollection();

                // Start update treeViews
                if (_subscriptionCollection != null)
                {
                    // Update subscription treeView must be before updating report treeView
                    this.UpdateSubscriptionTreeView();

                    // Update report treeView
                    this.UpdateReportTreeView();

                    // Rebuild panel if ReportSubscriptionView is visible
                    if (sender is WlbReportSubscriptionView)
                    {
                        this.subscriptionView1.BuildPanel();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.WLBREPORT_REPORT_CONFIG_ERROR, Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
                this.Close();
            }
        }


        /// <summary>
        /// Event handler addresses various UI nuances depending on the type the node selected in the tree:
        ///     - Hides/displays the ReportView control
        ///     - Hides/displays host dropdown menu in ReportView control (and it's label)
        ///     - Disables/enables Run Report button in ReportView control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewReportList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeViewReportList.SelectedNode.Tag != null)
            {
                // Display report view if a report node gets selected
                if (this.treeViewReportList.SelectedNode.Tag.GetType() == typeof(WlbReportInfo))
                {
                    // Reset reportView and disable subscriptionView
                    this.subscriptionView1.Visible = false;
                    this.wlbReportView1.SynchReportViewer((WlbReportInfo)treeViewReportList.SelectedNode.Tag);

                    // Run report if necessary
                    if (_runReport)
                    {
                        this.wlbReportView1.ExecuteReport();

                        // Reset _runReport flag back to false
                        _runReport = false;
                    }
                }
                else
                {
                    // Display the subscription view if it's that type of node
                    if (this.treeViewReportList.SelectedNode.Tag.GetType() == typeof(WlbReportSubscription))
                    {
                        // Reset subscriptionView and disable reportView
                        this.wlbReportView1.Visible = false;
                        this.subscriptionView1.ResetSubscriptionView((WlbReportSubscription)treeViewReportList.SelectedNode.Tag);
                    }
                }

                // Deselect treeViewsubscriptionList
                this.treeViewSubscriptionList.SelectedNode = null;
            }
        }


        /// <summary>
        /// Event handler for the the Subscriptions list box. Displays/Hides the Report View control 
        /// and the Subscription View control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewSubscriptionList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Display the subscription view if it's that type of node
            if (treeViewSubscriptionList.SelectedNode.Tag != null)
            {
                // Reset the reportview control and hide it (if it isn't already)
                this.wlbReportView1.Visible = false;
                this.subscriptionView1.ResetSubscriptionView((WlbReportSubscription)treeViewSubscriptionList.SelectedNode.Tag);

                // Deselect treeViewReportList
                this.treeViewReportList.SelectedNode = null;
            }
        }


        /// <summary>
        /// Event handler for when a report is drilled in to from the MS Viewer inside the ReportView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wlbReportView1_ReportDrilledThrough(object sender, DrillthroughEventArgs e)
        {
            SynchTreeViewReportList(e.ReportPath.ToString());
        }

        private void wlbReportView1_Close(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }


        /// <summary>
        /// Event handler for a lost pool connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wlbReportView1_PoolConnectionLost(object sender, EventArgs e)
        {
            using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Information, String.Format(Messages.WLB_REPORT_POOL_CONNECTION_LOST, _pool.Name()), Messages.WLBREPORT_POOL_CONNECTION_LOST_CAPTION)))
            {
                dlg.ShowDialog(this);
            }

            this.Close();
            this.Dispose();
        }

        #endregion
    }
}
