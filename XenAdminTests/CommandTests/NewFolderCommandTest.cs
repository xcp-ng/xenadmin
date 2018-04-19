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

using XenAdmin;
using XenAdmin.Commands;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Model;
using XenAdmin.Controls;
using NUnit.Framework;


namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NewFolderCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public NewFolderCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        [MaxTime(100 * 1000)]
        public void Run()
        {
            NewFolderCommandTest tester = new NewFolderCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NewFolderCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public NewFolderCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        [MaxTime(100 * 1000)]
        public void Run()
        {
            NewFolderCommandTest tester = new NewFolderCommandTest();
            tester.Test();
        }
    }

    public class NewFolderCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new NewFolderCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Folders; }
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                Folder folder = (Folder)selection[0].XenObject;

                // choose new folder name
                string newFolderName = Guid.NewGuid().ToString();

                if (folder == null)
                {
                    GroupingTag tag = selection[0].GroupingTag;

                    if (tag != null && tag.Grouping is OrganizationViewFolders)
                        HandleModalDialog("New Folder", Command.Execute, delegate(NameAndConnectionPromptWrapper p)
                            {
                                p.NameTextBox.Text = newFolderName;
                                p.OKButton.PerformClick();
                            });
                }
                else
                {
                    if (folder.IsRootFolder)
                    {
                        HandleModalDialog("New Folder", Command.Execute, delegate(NameAndConnectionPromptWrapper p)
                            {
                                p.NameTextBox.Text = newFolderName;
                                p.OKButton.PerformClick();
                            });
                    }
                    else
                    {
                        HandleModalDialog("New Folder", Command.Execute, delegate(InputPromptDialogWrapper p)
                            {
                                p.NameTextBox.Text = newFolderName;
                                p.OKButton.PerformClick();
                            });
                    }
                }

                // now check to see if new folder exists and is selected, and is visible.
                MWWaitFor(delegate
                {
                    VirtualTreeNode n = MainWindowWrapper.TreeView.SelectedNode;
                    Folder f = n == null ? null : n.Tag as Folder;
                    return f != null &&
                        n.Text == newFolderName &&
                        f.Name() == newFolderName &&
                        n.AllParentsAreExpanded;
                }, "Couldn't find new folder.");
            }
        }
    }
}
