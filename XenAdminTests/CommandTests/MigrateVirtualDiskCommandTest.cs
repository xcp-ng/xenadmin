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
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Commands;
using XenAPI;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class MigrateVirtualDiskCommandTest : DatabaseTester_TestFixture
    {
        private readonly IMainWindow mw = new MockMainWindow();
        private const string dbName = "tampa_livevdimove_xapi-db.xml";

        public MigrateVirtualDiskCommandTest() : base(dbName){}
        
        [Test]
        public void CheckContextMenuText()
        {
            var selectedVdi1 = new SelectedItem(new VDI());
            var selectedVdi2 = new SelectedItem(new VDI());

            var cmd = new MigrateVirtualDiskCommand(mw, new[] { selectedVdi1 });
            Assert.AreEqual(Messages.MOVE_VDI_CONTEXT_MENU, cmd.ContextMenuText);

            cmd = new MigrateVirtualDiskCommand(mw, new[] { selectedVdi1, selectedVdi2 });
            Assert.AreEqual(Messages.MAINWINDOW_MOVE_OBJECTS, cmd.ContextMenuText);
        }

        [Test]
        public void CommandCanExecuteWithSelectionOfSrAndVdiOnRunningVms()
        {
            foreach (SrTuple srTuple in runningVmCanMigrateVdiData)
            {
                SR sr; VDI vdi;
                ResolveSrAndVdiNames(srTuple.SrName, srTuple.VdiName, out sr, out vdi);
                var cmd = new MigrateVirtualDiskCommand(mw, new[] {new SelectedItem(vdi)});
                Assert.AreEqual(srTuple.ExpectedCanMigrate, cmd.CanExecute(),
                                String.Format("VDI '{0}' on SR '{1}' can migrate", srTuple.VdiName, srTuple.SrName));
            }
        }

        [Test]
        public void CommandCannotExecuteNullVdi()
        {
            Assert.Throws<ArgumentNullException>(() => new MigrateVirtualDiskCommand(mw, null), "Null vdi selection");
        }

        [Test]
        public void CommandCanExecuteWithSelectionOfSrAndVdiOnShutdownVms()
        {
            ShutdownAllVMs();

            foreach (SrTuple srTuple in runningVmCanMigrateVdiData)
            {
                SR sr; VDI vdi;
                ResolveSrAndVdiNames(srTuple.SrName, srTuple.VdiName, out sr, out vdi);
                var cmd = new MigrateVirtualDiskCommand(mw, new[] { new SelectedItem(vdi) });
                Assert.IsFalse(cmd.CanExecute(), 
                               String.Format("VDI '{0}' on SR '{1}' can migrate", srTuple.VdiName, srTuple.SrName));
            }
            StartAllVMs();
        }

        [Test]
        public void TestMultipleSelectionsAllValid()
        {
            List<SelectedItem> selection = CreateVdiSelection(runningVmCanMigrateVdiData, t=>t.ExpectedCanMigrate);
            Assert.IsNotEmpty(selection);
            MigrateVirtualDiskCommand cmd = new MigrateVirtualDiskCommand(mw, selection);
            Assert.IsTrue(cmd.CanExecute(), "All valid selection can migrate");
        }

        [Test]
        public void TestMultipleSelectionsMixed()
        {
            List<SelectedItem> selection = CreateVdiSelection(runningVmCanMigrateVdiData, t=>true);
            Assert.IsNotEmpty(selection);
            MigrateVirtualDiskCommand cmd = new MigrateVirtualDiskCommand(mw, selection);
            Assert.IsFalse(cmd.CanExecute(), "Mixture in/valid selection can migrate");
        }

        [Test]
        public void CommandCannotExecuteAndReasonMessage()
        {
            //Note: Order of logic implicitly checked here
            SR sr; VDI vdi;
            ResolveSrAndVdiNames("iSCSIa", "vdi1", out sr, out vdi);
            ExecuteCommandCheckingReason("Unknown", vdi);
            vdi.type = vdi_type.metadata;
            ExecuteCommandCheckingReason(Messages.CANNOT_MOVE_DR_VD, vdi);
            vdi.type = vdi_type.ha_statefile;
            ExecuteCommandCheckingReason(Messages.CANNOT_MOVE_HA_VD, vdi);
            vdi.Locked = true;
            ExecuteCommandCheckingReason(Messages.CANNOT_MOVE_VDI_IN_USE, vdi);
            vdi.is_a_snapshot = true;
            ExecuteCommandCheckingReason(Messages.CANNOT_MOVE_VDI_IS_SNAPSHOT, vdi);
        }

        #region Private Helper Methods
        private void StartAllVMs()
        {
            DatabaseManager.ConnectionFor(dbName).Cache.VMs.ToList().ForEach(vm => VM.start(DatabaseManager.ConnectionFor(dbName).Session, vm.opaque_ref, false, false));
            DatabaseManager.ConnectionFor(dbName).LoadCache(DatabaseManager.ConnectionFor(dbName).Session);
            Assert.IsTrue(DatabaseManager.ConnectionFor(dbName).Cache.VMs.All(vm => vm.IsRunning()), "VMs are in a running state");
        }

        private void ShutdownAllVMs()
        {
            DatabaseManager.ConnectionFor(dbName).Cache.VMs.ToList().ForEach(vm => VM.hard_shutdown(DatabaseManager.ConnectionFor(dbName).Session, vm.opaque_ref));
            DatabaseManager.ConnectionFor(dbName).LoadCache(DatabaseManager.ConnectionFor(dbName).Session);
            Assert.IsFalse(DatabaseManager.ConnectionFor(dbName).Cache.VMs.ToList().All(vm => vm.IsRunning()), "VMs are in a shutdown state");
        }

        private void ExecuteCommandCheckingReason(string expectedReason, VDI vdi)
        {
            MigrateVirtualDiskCommand cmd = new MigrateVirtualDiskCommand(mw, new List<SelectedItem> { new SelectedItem(vdi) });
            Dictionary<SelectedItem, string> reasons = cmd.GetCantExecuteReasons();
            Assert.IsNotEmpty(reasons, "Reasons found for " + vdi.name_label);
            Assert.IsFalse(cmd.CanExecute(), "Command can execute for " + vdi.name_label);
            Assert.AreEqual(expectedReason, reasons.First().Value, "Reason as expected for " + vdi.name_label);
        }

        private void ResolveSrAndVdiNames(string srName, string vdiName, out SR sr, out VDI vdi)
        {
            sr = DatabaseManager.ConnectionFor(dbName).Cache.SRs.First(s => s.name_label == srName);
            Assert.NotNull(sr, "Couldn't resolve SR: " + srName);
            List<VDI> vdis = DatabaseManager.ConnectionFor(dbName).ResolveAll(sr.VDIs);
            vdi = vdis.First(v => v.name_label == vdiName);
            Assert.NotNull(vdi, "Couldn't resolve VDI: " + vdiName);
        }

        private List<SelectedItem> CreateVdiSelection(List<SrTuple> sourceList, Predicate<SrTuple> condition)
        {
            List<SelectedItem> selection = new List<SelectedItem>();
            foreach (SrTuple srTuple in sourceList)
            {
                if(condition(srTuple))
                {
                    SR sr; VDI vdi;
                    ResolveSrAndVdiNames(srTuple.SrName, srTuple.VdiName, out sr, out vdi);
                    selection.Add(new SelectedItem(vdi));
                }
            }
            return selection;
        }
        #endregion

        #region Test Data
        private struct SrTuple
        {
            public SrTuple(string SrName, string VdiName, bool ExpectedCanMigrate)
            {
                this.SrName = SrName;
                this.VdiName = VdiName;
                this.ExpectedCanMigrate = ExpectedCanMigrate;
            }

            public readonly string SrName;
            public readonly string VdiName;
            public readonly bool ExpectedCanMigrate;
        }

        private readonly List<SrTuple> runningVmCanMigrateVdiData = new List<SrTuple>
                                                      {
                                                          new SrTuple("iSCSIa", "shared 1", true),
                                                          new SrTuple("iSCSIa", "shared 0", true),
                                                          new SrTuple("iSCSIa", "vdi1", false), //Not connected to a VM
                                                          new SrTuple("Local storage", "local 0", true),
                                                          new SrTuple("Local storage", "local 1", true)
                                                      };
        #endregion
    }
}
