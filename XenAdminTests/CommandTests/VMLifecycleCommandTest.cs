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
using XenAPI;
using XenAdmin;
using NUnit.Framework;
using XenAdmin.Core;
using System.Windows.Forms;


namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VMLifecycleCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public VMLifecycleCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            VMLifecycleCommandTest tester = new VMLifecycleCommandTest();
            tester.TestSingleSelect();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VMLifecycleCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        private VMLifecycleCommandTest tester = new VMLifecycleCommandTest();

        public VMLifecycleCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void TestSingleSelect()
        {
            tester.TestSingleSelect();
        }

        [Test]
        public void TestMultipleSelect()
        {
            tester.TestMultipleSelect();
        }
        
        [Test]
        public void TestCommandErrorDialog()
        {
            tester.TestCommandErrorDialog();
        }
        
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VMLifecycleCommandRbacTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public VMLifecycleCommandRbacTestMidnightRide()
            : base(true, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        [MaxTime(100 * 1000)]
        public void Run()
        {
            VMLifecycleCommandTest tester = new VMLifecycleCommandTest();
            tester.TestRbacMidnightRide();
        }

    }

    public class VMLifecycleCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new ForceVMShutDownCommand();
        }

        public void TestSingleSelect()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                MW(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ForceShutdownToolStripMenuItem.PerformClick);
                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => vm.power_state == vm_power_state.Halted, "VM " + vm + " didn't shutdown.");
                }

                MW(MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.PerformClick);
                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => vm.power_state == vm_power_state.Running, "VM " + vm + " didn't start.");
                }

                MW(MainWindowWrapper.MainToolStripItems.SuspendToolbarButton.PerformClick);
                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    if (vm.allowed_operations.Contains(vm_operations.suspend))
                    {
                        MWWaitFor(() => vm.power_state == vm_power_state.Suspended, "VM " + vm + " didn't suspend.");
                    }
                }

                MW(MainWindowWrapper.MainToolStripItems.ResumeToolStripButton.PerformClick);
                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => vm.power_state == vm_power_state.Running, "VM " + vm + " didn't resume.");
                }
            }
        }

        public void TestMultipleSelect()
        {
            foreach (SelectedItemCollection selection in RunTest(GetMultipleSelections()))
            {
                MW(MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ForceShutdownToolStripMenuItem.PerformClick);
                MWWaitFor(() => selection.AsXenObjects<VM>().TrueForAll(v => v.power_state == vm_power_state.Halted), "VMs didn't shut down.");

                MW(MainWindowWrapper.MainToolStripItems.StartVMToolStripButton.PerformClick);
                MWWaitFor(() => selection.AsXenObjects<VM>().TrueForAll(v => v.power_state == vm_power_state.Running), "VMs didn't start.");
            }
        }

        public void TestCommandErrorDialog()
        {
            foreach (SelectedItemCollection selection in RunTest(GetMultipleSelections()))
            {
                var curSelection = selection;//closure
                HandleModalDialog("Error Suspending VM", MainWindowWrapper.MainToolStripItems.SuspendToolbarButton.PerformClick,
                  (CommandErrorDialogWrapper d) =>
                      {
                          var dialog = d.Item;
                          var dataGridView = TestUtils.GetDataGridView(dialog, "m_dataGridView");
                          Assert.Greater(dataGridView.RowCount, 0, "Command dialog shown with no reasons.");

                          foreach (DataGridViewRow row in dataGridView.Rows)
                          {
                              var curRow = row;//closure
                              VM vm = curSelection.AsXenObjects<VM>().Find(v => v.Name() == curRow.Cells[1].Value.ToString());
                              Assert.IsFalse(vm.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED), "PV drivers installed on " + vm + " but it couldn't suspend.");
                          }
                          TestUtils.GetButton(dialog, "btnClose").PerformClick();
                      });
            }
        }

        public void TestRbacMidnightRide()
        {
            foreach (SelectedItemCollection selection in RunTest(GetRbacSelections()))
            {
                HandleModalDialog(Messages.XENCENTER,
                    MainWindowWrapper.VMMenuItems.StartShutdownMenuItems.ForceShutdownToolStripMenuItem.PerformClick,
                    (RoleElevationDialogWrapper d) => TestUtils.GetButton(d.Item, "buttonCancel").PerformClick());
            }
        }

        private IEnumerable<SelectedItemCollection> GetRbacSelections()
        {
            yield return new SelectedItemCollection(new SelectedItem(GetAnyVM(v => v.allowed_operations.Contains(vm_operations.hard_shutdown))));
        }

        private IEnumerable<SelectedItemCollection> GetMultipleSelections()
        {
            yield return new SelectedItemCollection(GetAllXenObjects<VM>(v => v.allowed_operations.Contains(vm_operations.hard_shutdown)).ConvertAll(v => new SelectedItem(v)));
        }
    }
}
