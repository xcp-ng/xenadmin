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
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class ResolvingSubjectsDialog : XenDialogBase
    {
        private AddRemoveSubjectsAction resolveAction;
        private IXenConnection _connection;

        private ResolvingSubjectsDialog()
        {
            InitializeComponent();
        }

        public ResolvingSubjectsDialog(IXenConnection connection)
        {
            InitializeComponent();
            _connection = connection;
            labelTopBlurb.Text = string.Format(labelTopBlurb.Text, Helpers.GetName(connection).Ellipsise(80));
        }

        private void BeginResolve()
        {
            if (textBoxUserNames.Text.Trim().Length == 0)
                return;

            entryListView.Visible = true;
            progressBar1.Visible = true;
            textBoxUserNames.Enabled = false;
            buttonGrantAccess.Enabled = false;
            LabelStatus.Visible = true;
            textBoxUserNames.Dock = DockStyle.Fill;

            List<string> lookup = new List<string>();
            string[] firstSplit = textBoxUserNames.Text.Split(',');

            foreach (string s in firstSplit)
            {
                lookup.AddRange(s.Split(';'));
            }

            Dictionary<string, object> nameDict = new Dictionary<string, object>();
            foreach (string name in lookup)
            {
                string cleanName = name.Trim();
                if (cleanName.Length == 0)
                    continue;
                if (!nameDict.ContainsKey(cleanName))
                    nameDict.Add(cleanName, null);
            }

            List<String> nameList = new List<string>();
            foreach (string s in nameDict.Keys)
                nameList.Add(s);

            // start the resolve
            foreach (string name in nameList)
            {
                ListViewItemSubjectWrapper i = new ListViewItemSubjectWrapper(name);
                entryListView.Items.Add(i);
            }
            resolveAction = new AddRemoveSubjectsAction(_connection, nameList, new List<Subject>());
            resolveAction.NameResolveComplete += resolveAction_NameResolveComplete;
            resolveAction.AllResolveComplete += resolveAction_AllResolveComplete;
            resolveAction.SubjectAddComplete += resolveAction_SubjectAddComplete;
            resolveAction.Completed += addAction_Completed;
            resolveAction.RunAsync();
        }

        private void updateProgress()
        {
            progressBar1.Value = resolveAction.PercentComplete;
        }

        void resolveAction_NameResolveComplete(object sender, string enteredName, string resolvedName, string sid, Exception exception)
        {
            Program.Invoke(this, delegate
            {
                foreach (ListViewItemSubjectWrapper i in entryListView.Items)
                {
                    if (i.EnteredName == enteredName)
                    {
                        i.ResolveException = exception;
                        i.ResolvedName = resolvedName;
                        i.sid = sid;
                        i.Update();
                        break;
                    }
                }
                updateProgress();
            });
        }

        void resolveAction_AllResolveComplete()
        {
            Program.Invoke(this, delegate
           {
               LabelStatus.Text = Messages.ADDING_RESOLVED_TO_ACCESS_LIST;
           });
        }

        void resolveAction_SubjectAddComplete(object sender, Subject subject, Exception exception)
        {
            Program.Invoke(this, delegate
            {
                foreach (ListViewItemSubjectWrapper i in entryListView.Items)
                {
                    if (i.sid == subject.subject_identifier)
                    {
                        i.AddException = exception;
                        i.Subject = subject;
                        i.Update();
                        break;
                    }
                }
                updateProgress();
            });
        }

        private void addAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, delegate
            {
                if (resolveAction.Cancelled)
                {
                    LabelStatus.Text = Messages.CANCELLED_BY_USER;
                    foreach (ListViewItemSubjectWrapper i in entryListView.Items)
                        i.Cancel();
                }
                else
                {
                    LabelStatus.Text = anyFailures() ? Messages.COMPLETED_WITH_ERRORS : Messages.COMPLETED;
                } 
                updateProgress();
                SwitchCloseToCancel();

                progressBar1.Value = progressBar1.Maximum;
            });
        }

        private bool anyFailures()
        {
            foreach (ListViewItemSubjectWrapper i in entryListView.Items)
            {
                if (i.Failed)
                    return true;
            }
            return false;
        }

        private void SwitchCloseToCancel()
        {
            Program.AssertOnEventThread();
            AcceptButton = ButtonCancel;
            CancelButton = ButtonCancel;
            ButtonCancel.Text = Messages.CLOSE;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (resolveAction != null && (!resolveAction.Cancelled || !resolveAction.Cancelling))
                resolveAction.Cancel();

            base.OnClosing(e);
        }

        private class ListViewItemSubjectWrapper : ListViewItem
        {
            public Subject Subject;
            public string sid;
            public string ResolvedName;

            private string enteredName;
            public string EnteredName
            {
                get { return enteredName; }
            }

            private Exception resolveException;
            public Exception ResolveException
            {
                get { return resolveException; }
                set { resolveException = value; }
            }

            private Exception addException;
            public Exception AddException
            {
                get { return addException; }
                set { addException = value; }
            }

            private bool IsResolved
            {
                get { return !String.IsNullOrEmpty(sid); }
            }

            public bool Failed
            {
                get { return addException != null || resolveException != null; }
            }

            public ListViewItemSubjectWrapper(string EnteredName)
                : base(EnteredName)
            {
                enteredName = EnteredName;
                // Resolve status column
                SubItems.Add(resolveStatus());
                // Grant status column
                SubItems.Add("");
            }

            private string resolveStatus()
            {
                if (IsResolved)
                {
                    // Resolved
                    return String.Format(Messages.RESOLVED_AS, ResolvedName ?? Messages.UNKNOWN_AD_USER);
                }
                else if (resolveException != null)
                {
                    // Resolve Failed
                    return Messages.AD_COULD_NOT_RESOLVE_SUFFIX;
                }
                // Resolving
                return Messages.AD_RESOLVING_SUFFIX;
            }

            private string grantStatus()
            {
                if (addException != null || resolveException != null)
                    return Messages.FAILED_TO_ADD_TO_ACCESS_LIST;

                // If we haven't resolved yet and there are no exceptions we show a blank status - hasn't reached grant stage yet
                if (!IsResolved)
                    return "";

                return Subject == null ? Messages.ADDING_TO_ACCESS_LIST : Messages.ADDED_TO_ACCESS_LIST;
            }

            public void Update()
            {
                SubItems[1].Text = resolveStatus();
                SubItems[2].Text = grantStatus();
            }

            public void Cancel()
            {
                if (!IsResolved && resolveException == null)
                    resolveException = new CancelledException();

                if (Subject == null && addException == null)
                    addException = new CancelledException();

                Update();
            }
        }

        private void buttonGrantAccess_Click(object sender, EventArgs e)
        {
            BeginResolve();
        }

        private void setResolveEnable()
        {
            buttonGrantAccess.Enabled = textBoxUserNames.Text != "";
        }


        private void textBoxUserNames_TextChanged(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            setResolveEnable();
        }

        private void textBoxUserNames_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter && buttonGrantAccess.Enabled)
                //buttonGrantAccess_Click(null, null);
        }



    }
}