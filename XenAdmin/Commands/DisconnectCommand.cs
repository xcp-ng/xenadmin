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
using System.Text;

using XenAdmin.Actions.GUIActions;
using XenAdmin.Network;
using System.Collections.ObjectModel;
using XenAdmin.Actions;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using System.Threading;
using XenAdmin.Dialogs.WarningDialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Disconnects the specified connection.
    /// </summary>
    internal class DisconnectCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _prompt;
        private readonly IXenConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="connection">The connection to be disconnected</param>
        /// <param name="prompt">if set to <c>true</c> a confirmation prompt is shown.</param>
        public DisconnectCommand(IMainWindow mainWindow, IXenConnection connection, bool prompt)
            : base(mainWindow)
        {
            _prompt = prompt;
            _connection = connection;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return _connection != null && (_connection.IsConnected || _connection.InProgress);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>false if the user cancels the disconnect.</returns>
        public new bool Execute()
        {
            return CanExecute() && Execute(_connection, _prompt);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(_connection, _prompt);
        }

        private bool Execute(IXenConnection connection, bool prompt)
        {
            if (prompt)
            {
                return PromptAndDisconnectServer(connection);
            }
            
            // no prompt. All tasks are cancelled and the server disconnected
            ConnectionsManager.CancelAllActions(connection);
            DoDisconnect(connection);
            return true;
        }

        /// <summary>
        /// First prompts the user if there are any actions running, then cancels and d/c if they give the OK.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>True if the user agreed to d/c and cancel their tasks, false if we are going to remain connected</returns>
        private bool PromptAndDisconnectServer(IXenConnection connection)
        {
            if (!AllActionsFinished(connection, true))
            {
                if (MainWindowCommandInterface.RunInAutomatedTestMode ||
                    new CloseXenCenterWarningDialog(connection).ShowDialog(Parent) == DialogResult.OK)
                {
                    ConnectionsManager.CancelAllActions(connection);

                    DelegatedAsyncAction waitForCancelAction = new DelegatedAsyncAction(connection,
                        Messages.CANCELING_TASKS, Messages.CANCELING, Messages.COMPLETED,
                        delegate
                        {
                            DateTime startTime = DateTime.Now;
                            while ((DateTime.Now - startTime).TotalSeconds < 6.0)
                            {
                                if (AllActionsFinished(connection, false))
                                    break;

                                Thread.Sleep(2000);
                            }
                        });

                    using (var pd = new ActionProgressDialog(waitForCancelAction, ProgressBarStyle.Marquee))
                    {
                        pd.ShowDialog(Parent);
                    }
                }
                else
                {
                    return false;
                }
            }

            DoDisconnect(connection);
            return true;
        }

        private void DoDisconnect(IXenConnection connection)
        {
            string msg = string.Format(Messages.CONNECTION_CLOSED_NOTICE_TEXT, connection.Hostname);
            ActionBase notice = new ActionBase(msg, msg, false, true);
            notice.Pool = Helpers.GetPoolOfOne(connection);
            notice.Host = Helpers.GetMaster(connection);
            log.Warn($"Connection to {connection.Hostname} closed.");

            MainWindowCommandInterface.CloseActiveWizards(connection);
            XenDialogBase.CloseAll(connection);
            connection.EndConnect();
            MainWindowCommandInterface.SaveServerList();
        }

        private bool AllActionsFinished(IXenConnection connection, bool treatCancelingAsFinished)
        {
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted || action is MeddlingAction)
                    continue;

                if (treatCancelingAsFinished)
                {
                    AsyncAction a = action as AsyncAction;
                    if (a != null && a.Cancelling)
                        continue;
                }

                IXenObject xo = action.Pool ?? action.Host ?? action.VM ?? action.SR as IXenObject;
                if (xo == null || xo.Connection != connection)
                    continue;

                return false;
            }
            return true;
        }
    }
}
