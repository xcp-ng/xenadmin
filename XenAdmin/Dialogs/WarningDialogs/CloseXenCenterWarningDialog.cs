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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs.WarningDialogs
{
    public partial class CloseXenCenterWarningDialog : XenDialogBase
    {
        public CloseXenCenterWarningDialog(IXenConnection connection = null)
            :base(connection)
        {
            InitializeComponent();

            if (connection != null)
            {
                label2.Text = String.Format(Messages.DISCONNECT_WARNING, Helpers.GetName(connection).Ellipsise(50));
                ExitButton.Text = Messages.DISCONNECT_ANYWAY;
                DontExitButton.Text = Messages.DISCONNECT_CANCEL;
            }

            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            BuildList();
        }

        internal override string HelpName => connection == null ? Name : "DisconnectServerWarningDialog";

        private void BuildList()
        {
            try
            {
                dataGridViewActions.SuspendLayout();
                dataGridViewActions.Rows.Clear();

                var actions = ConnectionsManager.History.FindAll(CanAddRowAction);
                SortActions(actions);

                var rows = new List<DataGridViewActionRow>();
                foreach (var action in actions)
                {
                    var row = new DataGridViewActionRow(action);
                    rows.Add(row);
                }
                dataGridViewActions.Rows.AddRange(rows.ToArray());

                foreach (DataGridViewActionRow row in dataGridViewActions.Rows)
                    RegisterActionEvents(row.Action);
            }
            finally
            {
                dataGridViewActions.ResumeLayout();
            }
        }

        private bool CanAddRowAction(ActionBase action)
        {
            if (action.IsCompleted || action is MeddlingAction)
                return false;

            AsyncAction a = action as AsyncAction;
            if (a != null && a.Cancelling)
                return false;

            if (connection == null)
                return true;

            var xo = action.Pool ?? action.Host ?? action.VM ?? action.SR as IXenObject;
            if (xo == null || xo.Connection != connection)
                return false;

            return true;
        }

        private void RemoveActionRow(ActionBase action)
        {
            var row = FindRowFromAction(action);
            if (row != null)
            {
                action.Changed -= action_Changed;
                action.Completed -= action_Changed;
                dataGridViewActions.Rows.Remove(row);

                if (dataGridViewActions.RowCount == 0)
                    Close();
            }
        }

        private DataGridViewActionRow FindRowFromAction(ActionBase action)
        {
            foreach (DataGridViewRow row in dataGridViewActions.Rows)
            {
                var actionRow = row as DataGridViewActionRow;
                if (actionRow == null)
                    continue;

                if (actionRow.Action == action)
                    return actionRow;
            }

            return null;
        }

        private void RegisterActionEvents(ActionBase action)
        {
            action.Completed -= action_Completed;
            action.Changed -= action_Changed;
            action.Completed += action_Completed;
            action.Changed += action_Changed;
        }

        private void SortActions(List<ActionBase> actions)
        {
            if (dataGridViewActions.SortedColumn != null)
            {
                if (dataGridViewActions.SortedColumn.Index == columnStatus.Index)
                    actions.Sort(ActionBaseExtensions.CompareOnStatus);
                else if (dataGridViewActions.SortedColumn.Index == columnMessage.Index)
                    actions.Sort(ActionBaseExtensions.CompareOnTitle);
                else if (dataGridViewActions.SortedColumn.Index == columnLocation.Index)
                    actions.Sort(ActionBaseExtensions.CompareOnLocation);
                else if (dataGridViewActions.SortedColumn.Index == columnDate.Index)
                    actions.Sort(ActionBaseExtensions.CompareOnDateStarted);

                if (dataGridViewActions.SortOrder == SortOrder.Descending)
                    actions.Reverse();
            }
        }

        private void ToggleExpandedState(int rowIndex)
        {
            var row = dataGridViewActions.Rows[rowIndex] as DataGridViewActionRow;
            if (row == null)
                return;

            if (row.Expanded)
            {
                row.Cells[columnExpander.Index].Value = Properties.Resources.contracted_triangle;
                row.Cells[columnMessage.Index].Value = row.Action.GetTitle();
            }
            else
            {
                row.Cells[columnExpander.Index].Value = Properties.Resources.expanded_triangle;
                row.Cells[columnMessage.Index].Value = row.Action.GetDetails();
            }
            row.Expanded = !row.Expanded;
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, () =>
            {
                ActionBase action = e.Element as ActionBase;
                switch (e.Action)
                {
                    case CollectionChangeAction.Add:
                        var actions = ConnectionsManager.History.FindAll(CanAddRowAction);
                        SortActions(actions);
                        var index = actions.IndexOf(action);
                        if (index > -1)
                        {
                            var row = new DataGridViewActionRow(action);
                            dataGridViewActions.Rows.Insert(0, row);
                            RegisterActionEvents(action);
                        }
                        break;
                    case CollectionChangeAction.Remove:
                        if (action != null)
                        {
                            RemoveActionRow(action);
                        }
                        else
                        {
                            var range = e.Element as List<ActionBase>;
                            if (range != null)
                                BuildList();
                        }
                        break;
                    case CollectionChangeAction.Refresh:
                        BuildList();
                        break;
                }
            });
        }
        
        private void action_Changed(ActionBase action)
        {
            if (!action.IsCompleted)
                return;

            Program.Invoke(this, () => RemoveActionRow(action));
        }

        private void action_Completed(ActionBase action)
        {
            Program.Invoke(this, () => RemoveActionRow(action));
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != columnExpander.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }
        
        private void datagridViewActions_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewActions.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
                BuildList();
        }

        #region Nested classes

        private class DataGridViewActionRow : DataGridViewRow
        {
            private DataGridViewImageCell expanderCell = new DataGridViewImageCell();
            private DataGridViewImageCell statusCell = new DataGridViewImageCell();
            private DataGridViewTextBoxCell messageCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell dateCell = new DataGridViewTextBoxCell();

            public ActionBase Action { get; private set; }
            public bool Expanded { get; set; }

            public DataGridViewActionRow(ActionBase action)
            {
                Action = action;
                Cells.AddRange(expanderCell, statusCell, messageCell, locationCell, dateCell);
                RefreshSelf();
            }

            public void RefreshSelf()
            {
                statusCell.Value = Action.GetImage();

                if (Expanded)
                {
                    expanderCell.Value = Properties.Resources.expanded_triangle;
                    messageCell.Value = Action.GetDetails();
                }
                else
                {
                    expanderCell.Value = Properties.Resources.contracted_triangle;
                    messageCell.Value = Action.GetTitle();
                }
                locationCell.Value = Action.GetLocation();
                dateCell.Value = HelpersGUI.DateTimeToString(Action.Started.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
            }
        }

        #endregion
    }
}