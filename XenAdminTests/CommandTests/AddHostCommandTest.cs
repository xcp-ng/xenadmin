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
using System.Threading;
using XenAdmin.Commands;
using NUnit.Framework;
using XenAPI;
using XenAdmin;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddHostCommandTestSetup : MainWindowLauncher_TestFixture
    {
        public AddHostCommandTestSetup()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test, MaxTime( 100 * 1000 )]
        public void Run()
        {
            AddHostCommandTest tester = new AddHostCommandTest();
            tester.Test();
        }
    }

    
    public class AddHostCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new AddHostCommand(Program.MainWindow);
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest(() => (IXenObject)null))
            {
                MW(delegate
                {
                    Command.Execute();
                    AddServerDialogWrapper addServerDialogWrapper = new AddServerDialogWrapper(WaitForWindowToAppear("Add New Server"));
                    Thread.Sleep(500);
                    
                    Assert.IsNotNull(addServerDialogWrapper, "AddServerDialogWrapper was null");
                    Assert.IsNotNull(addServerDialogWrapper.ServerNameComboBox, "ServerNameComboBox was null");
                    Assert.IsNotNull(addServerDialogWrapper.AddButton, "AddButton was null");
                    
                    addServerDialogWrapper.ServerNameComboBox.Text = GetTestResource(CommandTestsDatabase.SingleHost);
                    Thread.Sleep(200);
                    addServerDialogWrapper.AddButton.PerformClick();
                    Thread.Sleep(200);
                });

                Func<bool> finished = delegate
                  {
                      Assert.IsNotNull(MainWindowWrapper.TreeView, "Tree view is null");
                      Assert.IsNotNull(MainWindowWrapper.TreeView.Nodes, "Tree view nodes are null");

                      bool nodeCount = MainWindowWrapper.TreeView.Nodes.Count > 0;
                      bool subnodeCount = MainWindowWrapper.TreeView.Nodes[0].Nodes.Count > 1;

                      return nodeCount & subnodeCount;
                  };

                bool treeHasNodes = MWWaitFor(finished);
                Thread.Sleep(1000);
                MW(delegate
                {
                    Assert.IsTrue(treeHasNodes, "Waiting for tree to have nodes has failed");
                    Assert.IsNotNull(MainWindowWrapper.TreeView.Nodes[0], "Node 0 is null");
                    Assert.IsNotNull(MainWindowWrapper.TreeView.Nodes[0].Nodes[1], "Subnode 1 is null");

                    //Test name is correct
                    string subnodeOneText = MainWindowWrapper.TreeView.Nodes[0].Nodes[1].Text;
                    Assert.IsNotNull(subnodeOneText, "Subnode 1's text is null: The host may not have been added");
                    Assert.IsNotEmpty(subnodeOneText, "Subnode 1's text is empty: The host may not have been added");
                    Assert.AreEqual("krakout", subnodeOneText,
                                    "Subnode1's text containing the host name was incorrect");

                    //Test the node is expanded
                    bool subnodeOneIsExpanded = MainWindowWrapper.TreeView.Nodes[0].Nodes[1].IsExpanded;
                    Assert.IsTrue(subnodeOneIsExpanded, "Subnode 1 wasn't expanded.");
                });
            }
        }
    }
}
