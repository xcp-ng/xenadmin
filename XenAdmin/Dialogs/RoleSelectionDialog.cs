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
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class RoleSelectionDialog : XenDialogBase
    {
        private Subject[] subjects;

        private Dictionary<Role, List<Subject>> roleAssignment;
        // Used to stop 'changes made' checks during a batch select
        private bool batchChange;
        private IXenConnection _connection;

        public RoleSelectionDialog()
        {
            InitializeComponent();
        }

        public RoleSelectionDialog(IXenConnection connection, Subject[] subjects)
        {
            InitializeComponent();

            _connection = connection;

            this.subjects = subjects;
            roleAssignment = new Dictionary<Role,List<Subject>>();

            var allAreGroups = subjects.Length > 0 && subjects.All(s => s.IsGroup);
            var allAreUsers = subjects.Length > 0 && subjects.All(s => !s.IsGroup);

            pictureBoxSubjectType.Image = allAreUsers ? Properties.Resources._000_User_h32bit_32 : Properties.Resources._000_UserAndGroup_h32bit_32;

            if (subjects.Length == 1)
            {
                Subject subject = subjects[0];
                string sName = (subject.DisplayName ?? subject.SubjectName ?? Messages.UNKNOWN_AD_USER).Ellipsise(30);
                labelBlurb.Text = string.Format(allAreGroups ? Messages.AD_SELECT_ROLE_GROUP : Messages.AD_SELECT_ROLE_USER, sName);
            }
            else
            {
                if (allAreGroups)
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_GROUP_MANY;
                else if (allAreUsers)
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_USER_MANY;
                else
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_MIXED;
            }

            // Get the list of roles off the server and arrange them into rank
            List<Role> serverRoles = new List<Role>(_connection.Cache.Roles);
            //hide basic permissions, we only want the roles.
            serverRoles.RemoveAll(delegate(Role r){return r.subroles.Count < 1;});
            serverRoles.Sort();
            serverRoles.Reverse();
            foreach (Role r in serverRoles)
            {
                roleAssignment.Add(r, new List<Subject>());
            }
            foreach (Subject s in subjects)
            {
                List<Role> subjectRoles = _connection.ResolveAll(s.roles);
                foreach (Role r in subjectRoles)
                {
                    roleAssignment[r].Add(s);
                }
            }
            foreach (Role role in serverRoles)
            {
                DataGridViewRow r = new DataGridViewRow();
                DataGridViewCheckBoxCellEx c1 = new DataGridViewCheckBoxCellEx();
                c1.ThreeState = true;
                if (roleAssignment[role].Count == subjects.Length)
                {
                    c1.Value = CheckState.Checked;
                    c1.CheckState = CheckState.Checked;
                }
                else if (roleAssignment[role].Count == 0)
                {
                    c1.Value = CheckState.Unchecked;
                    c1.CheckState = CheckState.Unchecked;
                }
                else
                {
                    c1.Value = CheckState.Indeterminate;
                    c1.CheckState = CheckState.Indeterminate;
                }
                DataGridViewTextBoxCell c2 = new DataGridViewTextBoxCell();
                c2.Value = role.FriendlyName();
                r.Cells.Add(c1);
                r.Cells.Add(c2);
                r.Tag = role;
                gridRoles.Rows.Add(r);
            }
            setRoleDescription();
        }

        private void setRoleDescription()
        {
            if (gridRoles.SelectedRows.Count < 1)
                return;
            Role r = gridRoles.SelectedRows[0].Tag as Role;
            if (r == null)
                return;

            labelDescription.Text = r.FriendlyDescription();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            foreach (Subject s in subjects)
            {
                List<Role> rolesToAdd = new List<Role>();
                List<Role> rolesToRemove = new List<Role>();
                foreach (DataGridViewRow r in gridRoles.Rows)
                {
                    bool check = (bool)r.Cells[0].Value;
                    Role role = r.Tag as Role;
                    if (check && !(roleAssignment[role].Contains(s)))
                        rolesToAdd.Add(role);
                    else if (!check && (roleAssignment[role].Contains(s)))
                        rolesToRemove.Add(role);
                }

                new AddRemoveRolesAction(_connection, s, rolesToAdd, rolesToRemove).RunAsync();
            }
            Close();
        }

        private bool changesMade()
        {
            foreach (DataGridViewRow r in gridRoles.Rows)
            {
                Role role = r.Tag as Role;
                DataGridViewCheckBoxCellEx c = r.Cells[0] as DataGridViewCheckBoxCellEx;
                if (c.CheckState == CheckState.Checked && roleAssignment[role].Count < subjects.Length)
                {
                    return true;
                }
                else if (c.CheckState == CheckState.Unchecked && roleAssignment[role].Count > 0)
                {
                    return true;
                }
                // Don't care about intermediate values, it's not possible to set them in the dialog
            }
            return false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridRoles_SelectionChanged(object sender, EventArgs e)
        {
            setRoleDescription();
        }


        // We want to allow the user to select rows without changing the check boxes so they can browse the descriptions
        // so handle mouseclicks only on the check box column
        // Only allow them to select a single role, which also means all that tri state rubbish dissapears as soon as they make a selection
        private void gridRoles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || batchChange)
                return;

            selectIndex(e.RowIndex);
            buttonSave.Enabled = changesMade();
        }

        // We want to allow the user to select rows without changing the check boxes so they can browse the descriptions
        // but we want it to be keyboard accessible. Capture the select keypress and toggle the checkbox
        private void gridRoles_KeyPress(object sender, KeyPressEventArgs e)
        {
            // makes assumption about multiselect
            System.Diagnostics.Trace.Assert(!gridRoles.MultiSelect);

            if (batchChange || e.KeyChar != ' ') 
                return;

            selectIndex(gridRoles.SelectedRows[0].Index);
            buttonSave.Enabled = changesMade();
        }

        private void selectIndex(int i)
        {
            batchChange = true;
            foreach (DataGridViewRow r in gridRoles.Rows)
            {
                if (i == r.Index)
                {
                    r.Cells[0].Value = true;
                    ((DataGridViewCheckBoxCellEx)(r.Cells[0])).CheckState = CheckState.Checked;
                    continue;
                }
                r.Cells[0].Value = false;
                ((DataGridViewCheckBoxCellEx)(r.Cells[0])).CheckState = CheckState.Unchecked;
            }
            batchChange = false;
        }

        // Data grid view check box cells dont commit their value immeadiately, though the UI updates (edit mode).
        // To prevent messing around we handle the check state value ourselves.
        class DataGridViewCheckBoxCellEx : DataGridViewCheckBoxCell
        {
            public CheckState CheckState = CheckState.Unchecked;
        }
    }
}