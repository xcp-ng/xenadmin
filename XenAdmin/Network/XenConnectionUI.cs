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
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Network
{
    class XenConnectionUI
    {
        public static Dictionary<IXenConnection, ConnectingToServerDialog> connectionDialogs = new Dictionary<IXenConnection, ConnectingToServerDialog>();

        /// <summary>
        /// Start connecting to a server
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="interactive">Whether the user has initiated/is watching this connection attempt.</param>
        /// <param name="owner">The form that connecting dialogs will be displayed in front of.
        /// May be null, in which case Program.MainWindow is used.</param>
        /// <param name="initiateMasterSearch">If true, when connection to the master fails we will start trying to connect to
        /// each remembered slave in turn.</param>
        public static void BeginConnect(IXenConnection connection, bool interactive, Form owner, bool initiateMasterSearch)
        {
            Program.AssertOnEventThread();
            RegisterEventHandlers(connection);
            if (interactive)
            {
                // CA-214953 - Focus on this connection's dialog, if one exists, otherwise create one
                if (connectionDialogs.TryGetValue(connection, out ConnectingToServerDialog dlg))
                {
                    UnregisterEventHandlers(connection);
                    if (dlg.WindowState == FormWindowState.Minimized)
                        dlg.WindowState = FormWindowState.Normal;
                    dlg.Focus();
                    return;
                }

                dlg = new ConnectingToServerDialog(connection);
                connectionDialogs.Add(connection, dlg);

                if (!dlg.BeginConnect(owner, initiateMasterSearch) && connection != null)
                    connectionDialogs.Remove(connection);
            }
            else
                ((XenConnection)connection).BeginConnect(initiateMasterSearch, PromptForNewPassword);
        }

        private static bool PromptForNewPassword(IXenConnection connection, string oldPassword)
        {
            bool result = false;
            Program.Invoke(Program.MainWindow, () =>
            {
                // show an altered version of the add server dialog with the hostname greyed out
                using (var dialog = new AddServerDialog(connection, true))
                    result = dialog.ShowDialog(Program.MainWindow) == DialogResult.OK;
            });
            return result;
        }

        private static void RegisterEventHandlers(IXenConnection connection)
        {
            UnregisterEventHandlers(connection);
            connection.ConnectionResult += Connection_ConnectionResult;
            connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
        }

        private static void UnregisterEventHandlers(IXenConnection connection)
        {
            connection.ConnectionResult -= Connection_ConnectionResult;
            connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
        }

        private static void Connection_ConnectionResult(object sender, ConnectionResultEventArgs e)
        {
            if (!(sender is IXenConnection conn))
                return;

            Form ownerForm = Program.MainWindow;

            if (connectionDialogs.TryGetValue(conn, out var dlg))
            {
                var form = dlg.GetOwnerForm();
                if (form != null)
                    ownerForm = form;

                dlg.CloseConnectingDialog();
            }

            if (e.Connected || e.Error == null)
                return;

            Program.Invoke(Program.MainWindow, () => ShowConnectingDialogError(ownerForm, conn, e.Error));
        }

        private static void ShowConnectingDialogError(Form owner, IXenConnection connection, Exception error)
        {
            if (error is ExpressRestriction e)
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    new LicenseWarningDialog(e.HostName, e.ExistingHostName).ShowDialog(owner);
                });
                return;
            }

            if (error is Failure f)
            {
                if (f.ErrorDescription[0] == Failure.HOST_IS_SLAVE)
                {
                    string oldHost = connection.Name;
                    string poolMasterName = f.ErrorDescription[1];

                    string pool_name = XenConnection.ConnectedElsewhere(poolMasterName);
                    if (pool_name != null)
                    {
                        if (!Program.RunInAutomatedTestMode)
                        {
                            if (pool_name == oldHost)
                            {
                                using (var dlg = new ThreeButtonDialog(
                                    new ThreeButtonDialog.Details(
                                       SystemIcons.Information,
                                       string.Format(Messages.OLD_CONNECTION_ALREADY_CONNECTED, pool_name),
                                       Messages.ADD_NEW_CONNECT_TO)))
                                {
                                    dlg.ShowDialog(owner);
                                }
                            }
                            else
                            {

                                using (var dlg = new ThreeButtonDialog(
                                    new ThreeButtonDialog.Details(
                                       SystemIcons.Information,
                                        string.Format(Messages.SLAVE_ALREADY_CONNECTED, oldHost, pool_name),
                                       Messages.ADD_NEW_CONNECT_TO)))
                                {
                                    dlg.ShowDialog(owner);
                                }
                            }
                        }
                    }
                    else
                    {
                        DialogResult dialogResult;
                        using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(
                                    SystemIcons.Warning,
                                    String.Format(Messages.SLAVE_CONNECTION_ERROR, oldHost, poolMasterName),
                                    Messages.CONNECT_TO_SERVER),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                        {
                            dialogResult = dlg.ShowDialog(owner);
                        }
                        if (DialogResult.Yes == dialogResult)
                        {
                            ((XenConnection) connection).Hostname = poolMasterName;
                            BeginConnect(connection, true, owner, false);
                        }
                    }
                }
                else if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    AddError(owner, connection, Messages.ERROR_NO_PERMISSION, Messages.SOLUTION_NO_PERMISSION);
                }
                else if (f.ErrorDescription[0] == XenAPI.Failure.SESSION_AUTHENTICATION_FAILED)
                {
                    AddError(owner, connection, Messages.ERROR_AUTHENTICATION, Messages.SOLUTION_AUTHENTICATION);
                }
                else if (f.ErrorDescription[0] == Failure.HOST_STILL_BOOTING)
                {
                    AddError(owner, connection, Messages.ERROR_HOST_STILL_BOOTING, Messages.SOLUTION_HOST_STILL_BOOTING);
                }
                else
                {
                    AddError(owner, connection, string.IsNullOrEmpty(f.Message) ? Messages.ERROR_UNKNOWN : f.Message, string.Empty);
                }
            }
            else if (error is WebException w)
            {
                if (((XenConnection)connection).SuppressErrors)
                    return;

                var solutionCheckXenServer = 
                    Properties.Settings.Default.ProxySetting != (int)HTTPHelper.ProxyStyle.DirectConnection ? Messages.SOLUTION_CHECK_XENSERVER_WITH_PROXY : Messages.SOLUTION_CHECK_XENSERVER;

                switch (w.Status)
                {
                    case WebExceptionStatus.ConnectionClosed:
                        AddError(owner, connection, Messages.CONNECTION_CLOSED_BY_SERVER, string.Format(solutionCheckXenServer, ((XenConnection)connection).Hostname));
                        break;
                    case WebExceptionStatus.ConnectFailure:
                        AddError(owner, connection, Messages.CONNECTION_REFUSED, string.Format(solutionCheckXenServer, ((XenConnection)connection).Hostname));
                        break;
                    case WebExceptionStatus.ProtocolError:
                        if (w.Message != null && w.Message.Contains("(404)"))
                            AddError(owner, connection, string.Format(Messages.ERROR_NO_XENSERVER, ((XenConnection)connection).Hostname), string.Format(solutionCheckXenServer, ((XenConnection)connection).Hostname));
                        else if (w.Message != null && w.Message.Contains("(407)"))
                        {
                            string proxyAddress = Properties.Settings.Default.ProxyAddress;
                            AddError(owner, connection, string.Format(Messages.ERROR_PROXY_AUTHENTICATION, proxyAddress), string.Format(Messages.SOLUTION_CHECK_PROXY, proxyAddress));
                        }
                        else
                            AddError(owner, connection, Messages.ERROR_UNKNOWN, Messages.SOLUTION_UNKNOWN);
                        break;
                    case WebExceptionStatus.NameResolutionFailure:
                        AddError(owner, connection, string.Format(Messages.ERROR_NOT_FOUND, ((XenConnection)connection).Hostname), Messages.SOLUTION_NOT_FOUND);
                        break;
                    case WebExceptionStatus.ReceiveFailure:
                    case WebExceptionStatus.SendFailure:
                        AddError(owner, connection, string.Format(Messages.ERROR_NO_XENSERVER, ((XenConnection)connection).Hostname), string.Format(solutionCheckXenServer, ((XenConnection)connection).Hostname));
                        break;
                    case WebExceptionStatus.SecureChannelFailure:
                        AddError(owner, connection, string.Format(Messages.ERROR_SECURE_CHANNEL_FAILURE, ((XenConnection)connection).Hostname), Messages.SOLUTION_UNKNOWN);
                        break;
                    default:
                        AddError(owner, connection, Messages.ERROR_UNKNOWN, Messages.SOLUTION_UNKNOWN);
                        break;
                }
            }
            else if (error is UriFormatException)
            {
                AddError(owner, connection, string.Format(Messages.ERROR_INVALID_URI, connection.Name), Messages.SOLUTION_NOT_FOUND);
            }
            else if (error is FileNotFoundException)
            {
                // If you're using the DbProxy
                AddError(owner, connection, string.Format(string.Format(Messages.ERROR_FILE_NOT_FOUND, ((XenConnection)connection).Hostname), connection.Name), Messages.SOLUTION_UNKNOWN);
            }
            else if (error is ConnectionExists c)
            {
                ConnectionsManager.ClearCacheAndRemoveConnection(connection);

                if (!Program.RunInAutomatedTestMode)
                {
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Information,
                           c.GetDialogMessage(connection),
                           Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(owner);
                    }
                }
            }
            else if (error is ArgumentException)
            {
                // This happens if the server API is incompatible with our bindings.  This should
                // never happen in production, but will happen during development if a field
                // changes type, for example.
                AddError(owner, connection, Messages.SERVER_API_INCOMPATIBLE, Messages.SOLUTION_UNKNOWN);
            }
            else if (error is ServerNotSupported)
            {
                // Server version is too old for this version of XenCenter
                AddError(owner, connection, Messages.SERVER_TOO_OLD, Messages.SERVER_TOO_OLD_SOLUTION);
            }
            else
            {
                if (((XenConnection)connection).SuppressErrors)
                    return;

                AddError(owner, connection, string.Format(Messages.ERROR_UNKNOWN, ((XenConnection)connection).Hostname), Messages.SOLUTION_UNKNOWN);
            }
        }

        private static void AddError(Form owner, IXenConnection connection, string error, string solution)
        {
            string text = string.Format(Messages.MESSAGEBOX_ERRORTEXT, ((XenConnection)connection).Hostname, error, solution);

            // first check if there is already a dialog showing the same error (CA-97070, CA-88901)
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(ThreeButtonDialog))
                {
                    ThreeButtonDialog dlg = (ThreeButtonDialog) form;
                    if (dlg.Message == text && dlg.Owner == owner)
                    {
                        HelpersGUI.BringFormToFront(form);
                        return;
                    }
                }
            }

            if (((XenConnection)connection).fromDialog)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Error, text, Messages.CONNECT_TO_SERVER),
                    new ThreeButtonDialog.TBDButton(Messages.RETRY_BUTTON_LABEL, DialogResult.Retry, ThreeButtonDialog.ButtonType.ACCEPT, true),
                    ThreeButtonDialog.ButtonCancel))
                {
                    dialogResult = dlg.ShowDialog(owner);
                }
                if (DialogResult.Retry == dialogResult)
                {
                    new AddServerDialog(connection, false).Show(owner);
                }
            }
            else
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       text,
                       Messages.CONNECT_TO_SERVER)))
                {
                    dlg.ShowDialog(owner);
                }
            }
        }

        private static void Connection_ConnectionStateChanged(IXenConnection conn)
        {
            if (conn.IsConnected)
                return;

            // Close dialogs on termination
            Program.Invoke(Program.MainWindow, () => XenDialogBase.CloseAll(conn));
        }
    }
}
