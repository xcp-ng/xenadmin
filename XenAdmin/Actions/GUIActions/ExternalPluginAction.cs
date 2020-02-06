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
using System.Diagnostics;
using System.Windows.Forms;
using XenAdmin.Network;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Plugins;
using System.Xml;
using System.Collections.ObjectModel;
using System.Text;
using XenAdmin.Dialogs;


namespace XenAdmin.Actions
{
    internal class ExternalPluginAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string EmptyParameter = "null";
        private const string BlankParamter = "blank";

        private static readonly string SnapInTrustedCertXml =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"WindowsPowerShell\XenServer_Known_Certificates.xml");

        private static readonly string SnapInTrustedCertXmlDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WindowsPowerShell");

        private readonly ReadOnlyCollection<IXenObject> _targets = new ReadOnlyCollection<IXenObject>(new List<IXenObject>());
        private readonly bool XenCenterNodeTarget = false;
        private readonly MenuItemFeature _menuItemFeature;
        private Process _extAppProcess;

        public ExternalPluginAction(MenuItemFeature menuItemFeature, IEnumerable<IXenObject> targets, bool XenCenterNodeSelected)
            : base(null,
            string.Format(Messages.EXTERNAL_PLUGIN_TITLE, menuItemFeature.ToString(), menuItemFeature.PluginName),
            string.Format(Messages.EXTERNAL_PLUGIN_RUNNING, menuItemFeature.ToString()), false)
        {
            if (targets != null)
            {
                _targets = new ReadOnlyCollection<IXenObject>(new List<IXenObject>(targets));
                if (_targets.Count == 1)
                {
                    Connection = this._targets[0].Connection;
                    SetAppliesTo(this._targets[0]);
                }
            }
            XenCenterNodeTarget = XenCenterNodeSelected;
            ShowProgress = false;
            _menuItemFeature = menuItemFeature;
        }

        protected override void Run()
        {
            try
            {
                // build up a list of params to pass to the process, checking each connection for permissions as we go
                Dictionary<IXenConnection, bool> connectionsChecked = new Dictionary<IXenConnection, bool>();
                List<string> procParams = new List<string>();
                if (XenCenterNodeTarget)
                {
                    foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
                    {
                        if (c.IsConnected)
                        {
                            if (!connectionsChecked.ContainsKey(c))
                            {
                                CheckPermission(c);
                                connectionsChecked.Add(c, true);
                            }
                            procParams.AddRange(RetrieveParams(c));
                        }
                    }
                }
                if (_targets != null)
                {
                    foreach (IXenObject o in _targets)
                    {
                        if (!connectionsChecked.ContainsKey(o.Connection))
                        {
                            CheckPermission(o.Connection);
                            connectionsChecked.Add(o.Connection, true);
                        }
                        procParams.AddRange(RetrieveParams(o));
                    }
                }
                _extAppProcess = _menuItemFeature.ShellCmd.CreateProcess(procParams, _targets);

                _extAppProcess.OutputDataReceived += _extAppProcess_OutputDataReceived;  

                log.InfoFormat("Starting plugin process for {0}.", _extAppProcess.StartInfo.FileName);
                _extAppProcess.Start();

                if (_extAppProcess.StartInfo.RedirectStandardError)
                    _extAppProcess.BeginOutputReadLine();

                RecomputeCanCancel();

                try
                {
                    while (!_extAppProcess.HasExited)
                    {
                        if (Cancelling || Cancelled)
                            throw new CancelledException();
                        Thread.Sleep(500);
                    }

                    RecomputeCanCancel();

                    if (_extAppProcess.HasExited && _extAppProcess.ExitCode != 0)
                    {
                        log.ErrorFormat("Plugin process for {0} exited with exit code {1}", _extAppProcess.StartInfo.FileName, _extAppProcess.ExitCode);
                        throw new Exception(String.Format(Messages.EXTERNAL_PLUGIN_BAD_EXIT, _extAppProcess.ExitCode));
                    }
                }
                catch (InvalidOperationException)
                {
                    if (Cancelling || Cancelled)
                        throw new CancelledException();
                }

                Description = Messages.EXTERNAL_PLUGIN_FINISHED;
            }
            catch (Win32Exception ex)
            {
                log.Error("Error running plugin executable", ex);
                throw new Win32Exception(Messages.EXTERNAL_PLUGIN_WIN32, ex);
            }
            catch (CancelledException)
            {
                Description = Messages.CANCELING;
                log.Error("User pressed cancel, sending close request and waiting");
                if (_extAppProcess != null && !_extAppProcess.HasExited)
                {
                    _extAppProcess.CloseMainWindow();
                    WatchForClose(_extAppProcess);
                }
                throw;
            }
            finally
            {
                if (_extAppProcess != null)
                {
                    _extAppProcess.Close();
                    _extAppProcess.Dispose();
                    _extAppProcess = null;
                }
            }
        }

        private void CheckPermission(IXenConnection xenConnection)
        {
            ShellCmd cmd = _menuItemFeature.ShellCmd;
            RbacMethodList methodsToCheck = cmd.RequiredMethods.Count == 0 ? _menuItemFeature.GetMethodList(cmd.RequiredMethodList) : cmd.RequiredMethods;
            if (methodsToCheck == null || xenConnection.Session == null || xenConnection.Session.IsLocalSuperuser)
            {
                return;
            }
            log.DebugFormat("Checking Plugin can run against connection {0}", xenConnection.Name);
            List<Role> rolesAbleToCompleteAction;
            bool ableToCompleteAction = Role.CanPerform(methodsToCheck, xenConnection, out rolesAbleToCompleteAction);
            
            log.DebugFormat("Roles able to complete action: {0}", Role.FriendlyCSVRoleList(rolesAbleToCompleteAction));
            log.DebugFormat("Subject {0} has roles: {1}", xenConnection.Session.UserLogName(), Role.FriendlyCSVRoleList(xenConnection.Session.Roles));

            if (ableToCompleteAction)
            {
                log.Debug("Subject authorized to complete action");
                return;
            }

            // Can't run on this connection, bail out
            string desc = string.Format(FriendlyErrorNames.RBAC_PERMISSION_DENIED_FRIENDLY_CONNECTION,
                xenConnection.Session.FriendlyRoleDescription(),
                Role.FriendlyCSVRoleList(rolesAbleToCompleteAction),
                xenConnection.Name);
            throw new Exception(desc);
        }

        private void WatchForClose(Process _extAppProcess)
        {
            TimeSpan gracePeriod = new TimeSpan(0, 0, (int)_menuItemFeature.ShellCmd.DisposeTime);
            DateTime start = DateTime.Now;
            while (DateTime.Now - start < gracePeriod)
            {
                if (_extAppProcess.HasExited)
                    return;

                Thread.Sleep(1000);
            }
            if (!_extAppProcess.HasExited)
            {
                Program.Invoke(Program.MainWindow, delegate
                {
                    using (var d = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(System.Drawing.SystemIcons.Warning, string.Format(Messages.FORCE_CLOSE_PLUGIN_PROMPT, _menuItemFeature.ToString())),
                        "ProcessForceClosePrompt",
                        new ThreeButtonDialog.TBDButton(Messages.FORCE_CLOSE, DialogResult.Yes),
                        new ThreeButtonDialog.TBDButton(Messages.ALLOW_TO_CONTINUE, DialogResult.No)))
                    {
                        if (d.ShowDialog(Program.MainWindow) == DialogResult.Yes && !_extAppProcess.HasExited)
                            _extAppProcess.Kill();
                    }
                });
            }
            
        }

        void _extAppProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            log.Debug(e.Data);
        }

        // Returns a set of params which relate to the object you have selected in the treeview
        private List<string> RetrieveParams(IXenObject obj)
        {
            IXenConnection connection = obj.Connection;
            Host master = connection != null ? Helpers.GetMaster(connection) : null; // get master asserts connection is not null
            string masterAddress = EmptyParameter;

            if (master != null)
            {
                masterAddress = Helpers.GetUrl(master.Connection);
                WriteTrustedCertificates(master.Connection);
            }

            string sessionRef = connection.Session != null ? connection.Session.opaque_ref : EmptyParameter;
            string objCls = obj != null ? obj.GetType().Name : EmptyParameter;
            string objUuid = obj != null && connection.Session != null ? Helpers.GetUuid(obj) : EmptyParameter;
            return new List<string>(new string[] { masterAddress, sessionRef, objCls, objUuid });
        }

        // Returns a set of params which relate to the connection in general, with no obj information
        private List<string> RetrieveParams(IXenConnection connection)
        {
            Host master = connection != null ? Helpers.GetMaster(connection) : null; // get master asserts connection is not null
            string masterAddress = EmptyParameter;

            if (master != null)
            {
                masterAddress = Helpers.GetUrl(master.Connection);
                WriteTrustedCertificates(master.Connection);
            }

            string sessionRef = connection.Session != null ? connection.Session.opaque_ref : EmptyParameter;
            string objCls = BlankParamter;
            string objUuid = BlankParamter;
            return new List<string>(new string[] { masterAddress, sessionRef, objCls, objUuid });
        }

        private void WriteTrustedCertificates(IXenConnection connection)
        {
            if (!Directory.Exists(SnapInTrustedCertXmlDir))
                Directory.CreateDirectory(SnapInTrustedCertXmlDir);  // CA-42052

            Dictionary<string, string> trusted = Settings.KnownServers;

            if (!trusted.ContainsKey(connection.Hostname))
                return;

            XmlDocument doc = new XmlDocument();

            XmlNode cert = doc.CreateElement("certificate");
            XmlAttribute hostname = doc.CreateAttribute("hostname");
            XmlAttribute fingerprint = doc.CreateAttribute("fingerprint");

            hostname.Value = connection.Hostname;
            fingerprint.Value = Helpers.PrettyFingerprint(trusted[connection.Hostname]);

            cert.Attributes.Append(hostname);
            cert.Attributes.Append(fingerprint);

            XmlNode certs;

            if (File.Exists(SnapInTrustedCertXml))
            {
                doc.Load(SnapInTrustedCertXml);

                certs = doc.SelectSingleNode("certificates");

                foreach (XmlNode node in certs.ChildNodes)
                {
                    if (node.Name == "certificate" && node.Attributes["hostname"] != null && node.Attributes["hostname"].Value == hostname.Value)
                        certs.RemoveChild(node);
                }
            }
            else
            {
                XmlNode decl = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                certs = doc.CreateElement("certificates");

                doc.AppendChild(decl);
                doc.AppendChild(certs);
            }

            certs.AppendChild(cert);

            doc.Save(SnapInTrustedCertXml);
        }

        public override void RecomputeCanCancel()
        {
            try
            {
                CanCancel = _extAppProcess != null && !_extAppProcess.HasExited;
            }
            catch (InvalidOperationException)
            {
                CanCancel = false;
            }
        }

        protected override string AuditDescription()
        {
            return string.Format("{0}: {1} {2} {3}", this.GetType().Name, DescribeTargets(), _menuItemFeature.ShellCmd.Filename, Description);
        }

        protected string DescribeTargets()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IXenObject o in _targets)
            {
                if (o is VM)
                {
                    sb.Append(DescribeVM(o as VM));
                }
                else if (o is Pool)
                {
                    sb.Append(DescribePool(o as Pool));
                }
                else if (o is Host)
                {
                    sb.Append(DescribeHost(o as Host));
                }
                else
                {
                    sb.Append(o.GetType().Name);
                    sb.Append(" ");
                    sb.Append(o.opaque_ref);
                    sb.Append(" : ");
                }
            }
            return sb.ToString();
        }
    }
}
