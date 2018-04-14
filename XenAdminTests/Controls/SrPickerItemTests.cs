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
using Moq;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdminTests.Controls
{
    [SetCulture("en-EN")]
    public class SrPickerItemTests : DatabaseTester_TestFixture
    {
        private const string dbName = "TampaTwoHostPoolSelectioniSCSI.xml";
        private const string slaveHost = "dt16";
        private const string masterHost = "dt23";
        private const string largeISCSI = "large iSCSI";
        private const string mediumISCSI = "medium iSCSI";
        private const string smallISCSI = "small iSCSI";

        public SrPickerItemTests() : base(dbName) { }

        private class TestData
        {
            public long DiskSize;
            public SR Sr;
            public VDI[] VdisToMove;
            public Host Affinity;
            public string ExpectedFailureDescription;
            public override string ToString()
            {
                return String.Format("SR: {0}, Affinity {1}", Sr.name_label, Affinity.Name());
            }
        }

        #region Test Data
        private IEnumerable<TestData> CannotMigrateTestData
        {
            get
            {
                yield return new TestData //iscsi shared -> local slave with restricted free space
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetLocalSr(slaveHost),
                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => !v.name_label.Contains("OnMaster")).ToArray(),
                    DiskSize = 240518168576,
                    ExpectedFailureDescription = "224 GB required when only 220.28 GB available"
                };
                yield return new TestData //iscsi shared -> local slave with restricted SR size
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetLocalSr(slaveHost),
                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => !v.name_label.Contains("OnMaster")).ToArray(),
                    DiskSize = 8000000000000,
                    ExpectedFailureDescription = "Disk size (7450.58 GB) exceeds SR size (224.8 GB)"
                };
                yield return new TestData //Local master to slave local
                {
                    Affinity = GetAffinity(masterHost),
                    Sr = GetLocalSr(slaveHost),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.LOCAL_TO_LOCAL_MOVE
                };
                yield return new TestData //Master and slave VDIs selected together from a iSCSI -> master local SR
                {
                    Affinity = GetAffinity(slaveHost),
                    VdisToMove = GetVDIsOn(largeISCSI),
                    Sr = GetLocalSr(masterHost),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS
                };
                yield return new TestData //Master VDI to a slave move
                {
                    Affinity = GetAffinity(masterHost),
                    Sr = GetLocalSr(slaveHost),
                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => v.name_label.Contains("OnMaster")).ToArray(),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS
                };
                yield return new TestData //Slave VDI to a master move
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetLocalSr(masterHost),
                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => !v.name_label.Contains("OnMaster")).ToArray(),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS
                };
                yield return new TestData //Local master to shared restricted free space
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetSr(smallISCSI),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 1010 * 1024 * 1024,
                    ExpectedFailureDescription = "1010 MB required when only 1008 MB available"
                };
                yield return new TestData //Local master to shared restricted total space
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetSr(smallISCSI),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 2024 * 1024* 1024,
                    ExpectedFailureDescription = "Disk size (1.98 GB) exceeds SR size (1012 MB)"
                };
                yield return new TestData //Move the vdis to the same place
                {
                    Affinity = GetAffinity(masterHost),
                    Sr = GetLocalSr(masterHost),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.CURRENT_LOCATION
                };
            }
        }

        private IEnumerable<TestData> CanMigrateTestData
        {
            get
            {
                //VDIs from master local storage -> shared storage
                yield return new TestData //Local master to shared
                                 {
                                     Affinity = GetAffinity(masterHost),
                                     VdisToMove = GetVDIsFromLocal(masterHost),
                                     Sr = GetSr(largeISCSI),
                                     DiskSize = 1024 * 1024
                                 };
                yield return new TestData //Local master to shared
                                {
                                    Affinity = GetAffinity(masterHost),
                                    VdisToMove = GetVDIsFromLocal(masterHost),
                                    Sr = GetSr(mediumISCSI),
                                    DiskSize = 1024 * 1024
                                };
                yield return new TestData //Local master to shared
                                {
                                    Affinity = GetAffinity(masterHost),
                                    VdisToMove = GetVDIsFromLocal(masterHost),
                                    Sr = GetSr(smallISCSI),
                                    DiskSize = 1024 * 1024
                                };
                //VDIs from the shared storage -> local storage
                yield return new TestData //Master VDIs to a master
                                {
                                    Affinity = GetAffinity(masterHost),
                                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => v.name_label.Contains("OnMaster")).ToArray(),
                                    Sr = GetLocalSr(masterHost),
                                    DiskSize = 1024 * 1024
                                };
                yield return new TestData //Slave VDIs to a slave 
                                {
                                    Affinity = GetAffinity(slaveHost),
                                    VdisToMove = GetVDIsOn(largeISCSI).Where(v => !v.name_label.Contains("OnMaster")).ToArray(),
                                    Sr = GetLocalSr(slaveHost),
                                    DiskSize = 1024 * 1024
                                };
                yield return new TestData //Shared to shared
                                {
                                    Affinity = GetAffinity(masterHost),
                                    VdisToMove = GetVDIsOn(largeISCSI),
                                    Sr = GetSr(mediumISCSI),
                                    DiskSize = 1024 * 1024
                                };
            }
        }

        private IEnumerable<TestData> CanMoveTestData
        {
            get
            {
                yield return new TestData //Shared to local with different affinity 
                {
                    Affinity = GetAffinity(slaveHost),
                    VdisToMove = GetVDIsOn(largeISCSI),
                    Sr = GetLocalSr(masterHost),
                    DiskSize = 1024 * 1024
                };
                yield return new TestData //Local master to shared
                {
                    Affinity = GetAffinity(masterHost),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    Sr = GetSr(largeISCSI),
                    DiskSize = 1024 * 1024
                };
                yield return new TestData //Local to local
                {
                    Affinity = GetAffinity(masterHost),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    Sr = GetLocalSr(slaveHost),
                    DiskSize = 1024 * 1024
                };
                yield return new TestData //Shared to local
                {
                    Affinity = GetAffinity(masterHost),
                    VdisToMove = GetVDIsOn(largeISCSI),
                    Sr = GetLocalSr(masterHost),
                    DiskSize = 1024 * 1024
                };
                yield return new TestData //Shared to shared
                {
                    Affinity = GetAffinity(masterHost),
                    VdisToMove = GetVDIsOn(largeISCSI),
                    Sr = GetSr(mediumISCSI),
                    DiskSize = 1024 * 1024
                };
            }
        }

        private IEnumerable<TestData> CannotMoveTestData
        {
            get
            {
                yield return new TestData //Move the vdis to the same place
                {
                    Affinity = GetAffinity(masterHost),
                    Sr = GetLocalSr(masterHost),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 1024 * 1024,
                    ExpectedFailureDescription = Messages.CURRENT_LOCATION
                };
                yield return new TestData //Local master to shared restricted free space
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetSr(smallISCSI),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 1010 * 1024 * 1024,
                    ExpectedFailureDescription = "1010 MB required when only 1008 MB available"
                };
                yield return new TestData //Local master to shared restricted total space
                {
                    Affinity = GetAffinity(slaveHost),
                    Sr = GetSr(smallISCSI),
                    VdisToMove = GetVDIsFromLocal(masterHost),
                    DiskSize = 2024 * 1024 * 1024,
                    ExpectedFailureDescription = "Disk size (1.98 GB) exceeds SR size (1012 MB)"
                };
            }
        }

        private TestData IsoSrData
        {
            get
            {
                return new TestData //ISO not supporting VDI create
                           {
                               Affinity = GetAffinity(slaveHost),
                               VdisToMove = GetVDIsFromLocal(masterHost),
                               Sr = GetLocalISOSr(slaveHost),
                               DiskSize = 1024*1024
                           };
            }
        }

        #endregion

        [Test]
        public void IsoSRMigrateScenario()
        {

            SrPickerItem item = new SrPickerMigrateItem(IsoSrData.Sr, IsoSrData.Affinity,
                                                 IsoSrData.DiskSize, IsoSrData.VdisToMove);

            Assert.That(item.Show, Is.False, "Item Shown: Item: ISOSr migrate");
            Assert.That(item.Enabled, Is.True, "Item Enabled: Item: ISOSr migrate");
        }

        [Test]
        public void IsoSRMigrateScenarioNullStorageHost() //From CA-92099
        {
            TestData td = new TestData //Shared VDIs to local storage - this data can migrate - used above
                                    {
                                        Affinity = GetAffinity(masterHost),
                                        VdisToMove = GetVDIsOn(largeISCSI).Where(v => v.name_label.Contains("OnMaster")).ToArray(),
                                        Sr = GetLocalSr(masterHost),
                                        DiskSize = 1024 * 1024
                                    };

            //Setup the data so that PBD count > 1 in order that GetStorageHost returns null
            Assert.That(td.Sr.PBDs.Count, Is.EqualTo(1), "PBD count before alteration");
            td.Sr.PBDs.Add(new XenRef<PBD>("dummy")); //Additional PBD should make GetStorageHost return null
            Assert.That(td.Sr.PBDs.Count, Is.EqualTo(2), "PBD count after alteration");
            Assert.IsNull(td.Sr.GetStorageHost(), "SR.GetStorageHost == null");
            
            //Setup the item and test this
            SrPickerItem item = new SrPickerMigrateItem(td.Sr, td.Affinity,
                                                        td.DiskSize, td.VdisToMove);

            Assert.That(item.Show, Is.True, "Item Shown: Item: Null StorageHost");
            Assert.That(item.Enabled, Is.True, "Item Enabled: Item: Null StorageHost");
            
            td.Sr.PBDs.Remove(new XenRef<PBD>("dummy"));
            Assert.That(td.Sr.PBDs.Count, Is.EqualTo(1), "PBD count after test"); 
        }

        [Test]
        public void IsoSRMoveScenario()
        {

            SrPickerItem item = new SrPickerMoveCopyItem(IsoSrData.Sr, IsoSrData.Affinity,
                                                 IsoSrData.DiskSize, IsoSrData.VdisToMove);

            Assert.That(item.Show, Is.False, "Item Shown: Item: ISOSr migrate");
            Assert.That(item.Enabled, Is.True, "Item Enabled: Item: ISOSr migrate");
        }

        [Test, Description("See PR-1255 for details")]
        public void TestCanMigrateScenarios()
        {
            CanDoScenario(SrPicker.SRPickerType.Migrate, CanMigrateTestData);
        }

        [Test, Description("See PR-1255 for details")]
        public void TestCannotMigrateScenario()
        {
            CannotDoScenario(SrPicker.SRPickerType.Migrate, CannotMigrateTestData);
        }

        [Test]
        public void TestCanMoveScenario()
        {
            CanDoScenario(SrPicker.SRPickerType.MoveOrCopy, CanMoveTestData);
        }

        [Test]
        public void TestCannotMoveScenario()
        {
            CannotDoScenario(SrPicker.SRPickerType.MoveOrCopy, CannotMoveTestData);
        }

        #region Helper methods for XenObjects
        private readonly SrPickerItemFactory factory = new SrPickerItemFactory();
        private void CannotDoScenario(SrPicker.SRPickerType type, IEnumerable<TestData> testData)
        {
            int count = 0;
            foreach (TestData data in testData)
            {
                SrPickerItem item = factory.PickerItem(data.Sr, type, data.Affinity,
                                                     data.DiskSize, data.VdisToMove);
                Assert.That(item.Show, Is.True, "Item Shown: Item " + count);
                Assert.That(item.Enabled, Is.False, "Item Enabled: Item " + count);
                Assert.That(item.Description, Is.EqualTo(data.ExpectedFailureDescription), "Item Reason: Item " + count);
                count++;
            }
        }


        private void CanDoScenario(SrPicker.SRPickerType type, IEnumerable<TestData> testData)
        {
            int count = 0;
            foreach (TestData data in testData)
            {
                SrPickerItem item = factory.PickerItem(data.Sr, type, data.Affinity,
                                                     data.DiskSize, data.VdisToMove);
                Assert.That(item.Show, Is.True, "Item Shown: Item " + count);
                Assert.That(item.Enabled, Is.True, "Item Enabled: Item " + count);
                count++;
            }
        }

        private Host GetAffinity(string hostName)
        {
            return DatabaseManager.ConnectionFor(dbName).Cache.Hosts.FirstOrDefault(h => h.name_label == hostName);
        }

        private VDI[] GetVDIsOn(string srName)
        {
            SR sr = GetSr(srName);
            return DatabaseManager.ConnectionFor(dbName).ResolveAll(sr.VDIs).ToArray();
        }

        private SR GetLocalSr(string hostName)
        {
            List<SR> local = DatabaseManager.ConnectionFor(dbName).Cache.SRs.Where(s => s.IsLocalSR()).ToList();
            return local.FirstOrDefault(s => s.GetStorageHost().name_label == hostName && s.name_label == "Local storage");
        }

        private SR GetLocalISOSr(string hostName)
        {
            List<SR> local = DatabaseManager.ConnectionFor(dbName).Cache.SRs.Where(s => s.IsLocalSR()).ToList();
            SR sr = local.FirstOrDefault(s => s.GetStorageHost().name_label == hostName && s.name_label.Contains("DVD"));
            return sr;
        }

        private VDI[] GetVDIsFromLocal(string hostName)
        {
            SR srLocal = GetLocalSr(hostName);
            return DatabaseManager.ConnectionFor(dbName).ResolveAll(srLocal.VDIs).ToArray();
        }

        private SR GetSr(string srName)
        {
            return DatabaseManager.ConnectionFor(dbName).Cache.SRs.FirstOrDefault(h => h.name_label == srName);
        } 
        #endregion
    }

    public class SrPickerItemUnitTests : UnitTester_TestFixture
    {
        private const string id = "test";
        public SrPickerItemUnitTests() : base(id){}

        private class SrPickerItemTest : SrPickerItem
        {
            public SrPickerItemTest(SR sr) : base(sr, null, 0 , null) {}
            protected override void SetImage(){} //Not needed here
            protected override bool CanBeEnabled { get { throw new NotImplementedException(); }}
        }

        [Test]
        public void LunPerVDIDisablesSrPickerItem()
        {
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            sr.Setup(i => i.HBALunPerVDI()).Returns(true);
            SrPickerItem spi = new SrPickerItemTest(sr.Object);
            Assert.That(spi.Show, Is.False);
            sr.Verify(a=>a.HBALunPerVDI(), Times.Once());
        }
    }
}
