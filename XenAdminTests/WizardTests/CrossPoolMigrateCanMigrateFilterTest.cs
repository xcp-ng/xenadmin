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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAPI;
using TestData = System.Tuple<XenAPI.IXenObject, System.Collections.Generic.List<XenAPI.VM>>;

namespace XenAdminTests.WizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class CrossPoolMigrateCanMigrateFilterTest : DatabaseTester_TestFixture
    {
        private const string ONE_HOST_POOL = "tampa_livevdimove_xapi-db.xml";
        private const string TWO_HOST_POOL = "tampa-poolof16and23-xapi-db.xml";
        
        //pool (name 40), consisting of one host (name dt40) and two VMs,
        //one on local storage (VM name: local) and one on shared (VM name: shared)
        private Pool oneHostPool;
        private Host singleHost;
        private List<VM> vmLocal;
        private List<VM> vmShared;

        //two host pool (name 16And23), consisting of two hosts (master name: dt16, slave name dt23)
        //and 4 VMs, two on master on local and shared storage (name labels 16local and 16shared respectively)
        //and two on the slave on local and shared storage (name labels 23local and 23shared respectively)
        private Pool twoHostPool;
        private Host master;
        private Host slave;
        private List<VM> vmLocalOnMaster;
        private List<VM> vmSharedOnMaster;
        private List<VM> vmLocalOnSlave;
        private List<VM> vmSharedOnSlave;

        public CrossPoolMigrateCanMigrateFilterTest()
            : base(ONE_HOST_POOL, TWO_HOST_POOL)
        {

        }

        [OneTimeSetUp]
        public void TestSetup()
        {
            //one host pool

            var oneHostPoolCache = DatabaseManager.ConnectionFor(ONE_HOST_POOL).Cache;
            oneHostPool = oneHostPoolCache.Pools.First(p => p.name_label == "40");
            Assert.IsNotNull(oneHostPool, "Cannot resolve one-host pool");

            singleHost = oneHostPoolCache.Hosts.First(h => h.name_label == "dt40");
            Assert.IsNotNull(singleHost, "Cannot resolve host in one-host-pool");

            vmLocal = new List<VM> { oneHostPoolCache.VMs.First(h => h.name_label == "local") };
            Assert.IsNotEmpty(vmLocal, "Cannot resolve resident VM in one host pool");

            vmShared = new List<VM> { oneHostPoolCache.VMs.First(h => h.name_label == "shared") };
            Assert.IsNotEmpty(vmShared, "Cannot resolve non-resident VM in one host pool");

            //two host pool

            var twoHostPoolCache = DatabaseManager.ConnectionFor(TWO_HOST_POOL).Cache;
            twoHostPool = twoHostPoolCache.Pools.First(p => p.name_label == "16And23");
            Assert.IsNotNull(twoHostPool, "Cannot resolve two-host pool");

            master = twoHostPoolCache.Hosts.First(h => h.name_label == "dt16");
            Assert.IsNotNull(master, "Cannot resolve master in two-host-pool");

            vmLocalOnMaster = new List<VM> { twoHostPoolCache.VMs.First(h => h.name_label == "16local") };
            Assert.IsNotEmpty(vmLocalOnMaster, "Cannot resolve VM resident on master in two-host pool");

            vmSharedOnMaster = new List<VM> { twoHostPoolCache.VMs.First(h => h.name_label == "16shared") };
            Assert.IsNotEmpty(vmSharedOnMaster, "Cannot resolve non-resident VM running on master in two-host pool");

            slave = twoHostPoolCache.Hosts.First(h => h.name_label == "dt23");
            Assert.IsNotNull(slave, "Cannot resolve slave in two-host-pool");

            vmLocalOnSlave = new List<VM> { twoHostPoolCache.VMs.First(h => h.name_label == "23local") };
            Assert.IsNotEmpty(vmLocalOnSlave, "Cannot resolve VM resident on slave in two-host pool");

            vmSharedOnSlave = new List<VM> { twoHostPoolCache.VMs.First(h => h.name_label == "23shared") };
            Assert.IsNotEmpty(vmSharedOnSlave, "Cannot resolve non-resident VM runnign on slave in two-host pool");
        }

        #region Private methods

        private IEnumerable TestCasesMigrationAllowed
        {
            get
            {
                //other host in same pool
                yield return new TestData(master, vmLocalOnSlave);
                yield return new TestData(master, vmSharedOnSlave);
                yield return new TestData(slave, vmLocalOnMaster);
                yield return new TestData(slave, vmSharedOnMaster);

                //same pool of more than one hosts
                yield return new TestData(twoHostPool, vmLocalOnSlave);
                yield return new TestData(twoHostPool, vmSharedOnSlave);
                yield return new TestData(twoHostPool, vmLocalOnMaster);
                yield return new TestData(twoHostPool, vmSharedOnMaster);

                //other pool/host
                yield return new TestData(singleHost, vmLocalOnMaster);
                yield return new TestData(singleHost, vmSharedOnMaster);
                yield return new TestData(oneHostPool, vmLocalOnMaster);
                yield return new TestData(oneHostPool, vmSharedOnMaster);

                yield return new TestData(singleHost, vmLocalOnSlave);
                yield return new TestData(singleHost, vmSharedOnSlave);
                yield return new TestData(oneHostPool, vmLocalOnSlave);
                yield return new TestData(oneHostPool, vmSharedOnSlave);
            }
        }

        private IEnumerable TestCasesMigrationNotAllowedCurrentServer
        {
            get
            {
                //Use host/pool with VMs from same single host pool
                yield return new TestData(oneHostPool, vmLocal);
                yield return new TestData(oneHostPool, vmShared);
                yield return new TestData(master, vmLocalOnMaster);
                yield return new TestData(master, vmSharedOnMaster);
                yield return new TestData(slave, vmLocalOnSlave);
                yield return new TestData(slave, vmSharedOnSlave);

            }
        }

        private IEnumerable TestCasesArgumentException
        {
            get
            {
                //Unsupported IXenObjects
                yield return new TestData(vmLocalOnMaster.First(), vmLocalOnMaster);
                yield return new TestData(vmLocalOnMaster.First(), new List<VM>());

                //Null host/pool
                yield return new TestData(null, vmLocalOnMaster);
                yield return new TestData(null, vmLocal);
                yield return new TestData(null, new List<VM>());
                yield return new TestData(null, null);
            }
        }

        private IEnumerable TestCasesArgumentNullException
        {
            get
            {
                //null selection
                yield return new TestData(twoHostPool, null);
                yield return new TestData(master, null);
            }
        }

        #endregion


        [Test]
        public void VerifyMigrationAllowed()
        {
            foreach (TestData data in TestCasesMigrationAllowed)
            {
                var filter = new CrossPoolMigrateCanMigrateFilter(data.Item1, data.Item2, WizardMode.Migrate);
                Assert.False(filter.FailureFound, "Did not expect to find failure");
                Assert.That(filter.Reason, Is.Null.Or.Empty); //Did not expect failure reason
            }
        }

        [Test]
        public void VerifyThrowsArgumentException()
        {
            foreach (TestData data in TestCasesArgumentException)
            {
                Assert.Throws<ArgumentException>(() => { var filter = new CrossPoolMigrateCanMigrateFilter(data.Item1, data.Item2, WizardMode.Migrate); });
            }
        }

        [Test]
        public void VerifyThrowsArgumentNullException()
        {
            foreach (TestData data in TestCasesArgumentNullException)
            {
                Assert.Throws<ArgumentNullException>(() => { var filter = new CrossPoolMigrateCanMigrateFilter(data.Item1, data.Item2, WizardMode.Migrate); });
            }
        }

        [Test]
        public void VerifyMigrationNotAllowedCurrentServer()
        {
            foreach (TestData data in TestCasesMigrationNotAllowedCurrentServer)
            {
                var filter = new CrossPoolMigrateCanMigrateFilter(data.Item1, data.Item2, WizardMode.Migrate);
                Assert.True(filter.FailureFound, "Expected to find failure");
                Assert.That(filter.Reason, Is.Null.Or.Empty); //Did not expect failure reason
            }
        }
    }
}
