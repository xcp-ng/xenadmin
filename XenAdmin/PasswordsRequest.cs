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
using System.Configuration;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using XenCenterLib;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using System.Xml;
using System.IO;
using System.Security.Principal;
using System.Drawing;


namespace XenAdmin
{
    internal class PasswordsRequest
    {
        private const string NamespaceURI = "http://citrix.com/XenCenter/ConnectionExport";
        private const string TokenIdentifier = "XenCenterPasswordToken";
        private const string RootElement = "XenCenterConnectionExport";
        private const string TokenElement = "token";
        private const string RequestFilename = "XenCenterConnectionRequest.xml";
        private const string ResultFilename = "XenCenterConnectionExport.xml";
        private const char Separator = '\x202f'; // narrow non-breaking space.

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static void HandleRequest(string destdir)
        {
            log.DebugFormat("Handling password request");

            Process thisProcess = Process.GetCurrentProcess();
            Process parentProcess = Processes.FindParent(thisProcess);

            if (parentProcess == null)
            {
                log.Warn("Cannot find parent process.  Ignoring request.");
                return;
            }

            WindowsIdentity parentUser = Processes.GetWindowsIdentity(parentProcess);
            WindowsIdentity thisUser = Processes.GetWindowsIdentity(thisProcess);

            if (parentUser == null || thisUser == null)
            {
                log.Warn("Cannot find user details.  Ignoring request.");
                return;
            }

            if (parentUser.User != thisUser.User)
            {
                log.Warn("Passwords requested from user different to us.  Ignoring request.");
                return;
            }

            if (!Registry.AllowCredentialSave || !Properties.Settings.Default.SaveSession)
            {
                WriteXML(destdir, null, "nosession");
                return;
            }

            string exePath = Processes.GetExePath(parentProcess);

            if (ParseToken(destdir, out var token, out var tokenExePath, out var userSid))
            {
                // Valid token.
                if (tokenExePath == exePath)
                {
                    if (userSid == thisUser.User.ToString())
                    {
                        CompleteRequest(destdir, token);
                        return;
                    }

                    log.WarnFormat("Valid token, but for the wrong user. Will re-prompt");
                }
                else
                {
                    log.WarnFormat("Valid token, but for the wrong app. Will re-prompt");
                }
            }
            else
            {
                log.Warn("Missing or invalid token.");
            }

            using (var d = new PasswordsRequestDialog { Application = exePath })
                switch (d.ShowDialog())
                {
                    case DialogResult.OK:
                        // Give passwords this time.
                        CompleteRequest(destdir, null);
                        break;

                    case DialogResult.Yes:
                        // Give passwords always.
                        CompleteRequest(destdir, GenerateToken(exePath, thisUser.User.ToString()));
                        break;

                    case DialogResult.Cancel:
                        WriteXML(destdir, null, "cancelled");
                        break;

                    default:
                        log.Error("Unexpected result from PasswordsRequestDialog!");
                        return;
                }
        }

        private static bool ParseToken(string destdir, out string token, out string token_exepath, out string user_sid)
        {
            string enc = GetToken(destdir);
            if (enc != null)
            {
                string plain = EncryptionUtils.Unprotect(enc);
                if (plain != null)
                {
                    string[] bits = plain.Split(Separator);
                    long ticks;
                    if (bits.Length == 4 && bits[0] == TokenIdentifier && long.TryParse(bits[1], out ticks))
                    {
                        token = enc;
                        token_exepath = bits[2];
                        user_sid = bits[3];
                        return true;
                    }
                }
            }

            token = null;
            token_exepath = null;
            user_sid = null;
            return false;
        }

        private static string GetToken(string destdir)
        {
            string path = Path.Combine(destdir, RequestFilename);
            if (!File.Exists(path))
                return null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("n", NamespaceURI);

                XmlNode n = doc.SelectSingleNode("/n:" + RootElement + "/n:" + TokenElement, nsmgr);
                return n == null ? null : n.InnerText;
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                return null;
            }
        }

        private static string GenerateToken(string exepath, string user_sid)
        {
            string plain = string.Format("{0}{1}{2}{3}{4}{5}{6}", TokenIdentifier, Separator, DateTime.Now.Ticks, Separator, exepath,
                Separator, user_sid);
            return EncryptionUtils.Protect(plain);
        }

        private static void CompleteRequest(string destdir, string token)
        {
            bool restored;
            try
            {
                Settings.RestoreSession();
                restored = Properties.Settings.Default.SaveSession;
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error("Could not load settings.", ex);
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()),
                       Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
                Application.Exit();
                return;
            }
            try
            {
                if (restored)
                {
                    WriteXML(destdir, token, null);
                }
                else
                {
                    // The user has cancelled the restore.
                    WriteXML(destdir, null, "cancelled");
                }
            }
            catch (Exception exn)
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_PASSWORD_WRITE_FAILED, exn.Message),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
                Application.Exit();
            }
        }

        private static void WriteXML(string destdir, string token, string error)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            XmlElement root = doc.CreateElement(RootElement, NamespaceURI);
            root.SetAttribute("version", "1.0");
            doc.AppendChild(root);

            if (error != null)
            {
                XmlElement error_node = doc.CreateElement("error", NamespaceURI);
                root.AppendChild(error_node);

                error_node.SetAttribute("code", error);
            }
            else
            {
                if (token != null)
                {
                    XmlElement token_node = doc.CreateElement(TokenElement, NamespaceURI);
                    root.AppendChild(token_node);

                    token_node.AppendChild(doc.CreateTextNode(token));
                }

                foreach (IXenConnection conn in ConnectionsManager.XenConnectionsCopy)
                {
                    XenConnection connection = conn as XenConnection;
                    XmlElement pool_node = doc.CreateElement("pool", NamespaceURI);
                    root.AppendChild(pool_node);

                    pool_node.SetAttribute("name_label", conn.FriendlyName);
                    pool_node.SetAttribute("password", conn.Password);
                    pool_node.SetAttribute("is_connected", conn.SaveDisconnected ? "false" : "true");

                    lock (connection.PoolMembersLock)
                    {
                        pool_node.AppendChild(ServerNode(doc, conn.Hostname, conn.Port, true));

                        foreach (string member in conn.PoolMembers)
                        {
                            if (member != conn.Hostname)
                                pool_node.AppendChild(ServerNode(doc, member, conn.Port, false));
                        }
                    }
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            string dest = Path.Combine(destdir, ResultFilename);
            XmlWriter writer = XmlWriter.Create(dest, settings);
            try
            {
                doc.WriteContentTo(writer);
            }
            finally
            {
                writer.Close();
            }
        }

        private static XmlElement ServerNode(XmlDocument doc, string address, int port, bool is_master)
        {
            XmlElement server_node = doc.CreateElement("server", NamespaceURI);

            server_node.SetAttribute("address", address);
            server_node.SetAttribute("port", port.ToString());
            server_node.SetAttribute("is_master", is_master ? "true" : "false");

            return server_node;
        }
    }
}
