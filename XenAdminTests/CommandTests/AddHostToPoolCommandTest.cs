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
using XenAdmin.Commands;
using XenAdmin;
using XenAPI;
using NUnit.Framework;
using XenAdmin.Core;
using XenAdmin.Controls;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddHostToPoolCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public AddHostToPoolCommandTestGeorge()
            : base(CommandTestsDatabase.George, CommandTestsDatabase.SingleHost)
        { }

        [Test]
        [MaxTime(100 * 1000)]
        public void Run()
        {
            AddHostToPoolCommandTest tester = new AddHostToPoolCommandTest();
            tester.TestAddGeorgeHostToGeorgePool();
        }
    }

    [Ignore("This test currently fails, because the two databases are at the same version, so the slave *can* be added to the pool, whereas the test thinks it can't")]
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddHostToPoolCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public AddHostToPoolCommandTestMidnightRide()
            : base(CommandTestsDatabase.MidnightRide, CommandTestsDatabase.SingleHost)
        { }

        [Test]
        public void Run()
        {
            AddHostToPoolCommandTest tester = new AddHostToPoolCommandTest();
            tester.TestAddGeorgeHostToMidnightRidePool();
        }
    }

    public class AddHostToPoolCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new AddHostToPoolCommand(Program.MainWindow, new List<Host> { GetAnyHost(h => h.name_label == "krakout") }, GetAnyPool(), false);
        }

        private bool Finished()
        {
            VirtualTreeView.VirtualTreeNodeCollection poolNodes = MainWindowWrapper.TreeView.Nodes[0].Nodes[0].Nodes;
            return poolNodes.Count > 2 && poolNodes[0].Text == "inflames" && poolNodes[1].Text == "incubus" && poolNodes[2].Text == "krakout";
        }

        public void TestAddGeorgeHostToGeorgePool()
        {
            foreach (Pool pool in RunTest(GetAnyPool))
            {
                MW(Command.Execute);
                MWWaitFor(Finished, "AddHostToPoolCommandTest.TestRbacGeorge() didn't finish.");
            }
        }

        public void TestAddGeorgeHostToMidnightRidePool()
        {
            foreach (Pool pool in RunTest(GetAnyPool))
            {
                HandleModalDialog<CommandErrorDialogWrapper>("Error Adding Server to Pool", Command.Execute, d => d.CloseButton.PerformClick());
            }
        }
    }
}
