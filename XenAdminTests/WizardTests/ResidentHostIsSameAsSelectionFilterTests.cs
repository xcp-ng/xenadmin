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
using XenAdmin;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    class ResidentHostIsSameAsSelectionFilterTests : DatabaseTester_TestFixture
    {
        private const string oneHostPool = "tampa_livevdimove_xapi-db.xml";
        private const string twoHostPool = "tampa-poolof16and23-xapi-db.xml";
        private Host singlePoolHost;
        private Pool singlePool;

        //Don't put this in a fixture setup as the test data is built before the fixture is setup
        public ResidentHostIsSameAsSelectionFilterTests()
            : base(oneHostPool, twoHostPool)
        {

        }

        private void Init()
        {
            singlePool = DatabaseManager.ConnectionFor(oneHostPool).Cache.Pools.First(p => p.name_label == "40");
            Assert.IsNotNull(singlePool, "Resolved pool");

            singlePoolHost = DatabaseManager.ConnectionFor(oneHostPool).Cache.Hosts.First(h => h.name_label == "dt40");
            Assert.IsNotNull(singlePoolHost, "Resolved host");
        }

        private class TestCase
        {
            public TestCase(IXenObject ixo, List<VM> vms, bool failureExpected)
            {
                this.ixo = ixo;
                this.vms = vms;
                this.failureExpected = failureExpected;
            }

            public IXenObject ixo { get; private set; }
            public List<VM> vms { get; private set; }
            public bool failureExpected { get; private set; }
        }

        //Collection of test data for the main calls to the class
        private IEnumerable TestCases
        {
            get
            {
                //Use host/pool with VMs from a same pool
                yield return new TestCase ( singlePoolHost, CreateSingleVmListFromOneHostPool(), true );
                yield return new TestCase ( singlePool, CreateSingleVmListFromOneHostPool(), true );
                yield return new TestCase ( singlePoolHost, CreateMultipleVmListFromOneHostPool(), true );
                yield return new TestCase ( singlePool, CreateMultipleVmListFromOneHostPool(), true );

                //Use host/pool with VMs from a different pool
                yield return new TestCase ( singlePoolHost, CreateSingleVmListFromTwoHostPool(), false );
                yield return new TestCase ( singlePool, CreateSingleVmListFromTwoHostPool(), false );
                yield return new TestCase ( singlePoolHost, CreateMultipleVmListFromTwoHostPool(), false );
                yield return new TestCase ( singlePool, CreateMultipleVmListFromTwoHostPool(), false );
            }
        }

        [Test]
        public void VerifyExceptionThrownForNullConstructedCommand()
        {
            Assert.That(() => new ResidentHostIsSameAsSelectionFilter(null, new List<VM>()), Throws.ArgumentException);
        }

        [Test]
        public void VerifyExceptionThrownForDoubleNullConstructedCommand()
        {
            Assert.That(() => new ResidentHostIsSameAsSelectionFilter(null, null), Throws.ArgumentException);
        }

        [Test]
        public void VerifyExceptionThrownForNullIXenObjectAsTarget()
        {
            Assert.That(() => new ResidentHostIsSameAsSelectionFilter(null, CreateSingleVmListFromOneHostPool()), Throws.ArgumentException);
        }

        [Test]
        public void VerifyExceptionThrownForUnsupportedTargetObject()
        {
            Assert.That(()=> new ResidentHostIsSameAsSelectionFilter(CreateSingleVmListFromOneHostPool().First(), new List<VM>()), Throws.ArgumentException);
        }

        [Test]
        public void VerifyFailureDeterminedAndMessageForObject()
        {
            Init();
            foreach (TestCase testCase in TestCases)
            {
                Assert.IsNotNull(testCase.ixo, "Resolved IXenObject");
                ResidentHostIsSameAsSelectionFilter cmd = new ResidentHostIsSameAsSelectionFilter(testCase.ixo, testCase.vms);
                Assert.That(cmd.FailureFound, Is.EqualTo(testCase.failureExpected));
                VerifyFailureFoundMessage(cmd);
            }
            
        }

        #region Helper methods (private)
        private void VerifyFailureFoundMessage(ResidentHostIsSameAsSelectionFilter cmd)
        {
            Assert.AreEqual(Messages.HOST_MENU_CURRENT_SERVER, cmd.Reason);
        }

        private List<VM> CreateMultipleVmListFromOneHostPool()
        {
            List<VM> vms = CreateVmList(oneHostPool, vm => vm.name_label == "local" || vm.name_label == "shared");
            Assert.AreEqual(2, vms.Count, "Extracted VMs count");
            return vms;
        }

        private List<VM> CreateMultipleVmListFromTwoHostPool()
        {
            List<VM> vms = CreateVmList(twoHostPool, vm => new List<string>
                                                               {
                                                                   "16local", "16shared",
                                                                   "23local", "23shared"                                                                   
                                                               }.Contains(vm.name_label));
            Assert.AreEqual(4, vms.Count, "Extracted VMs count");
            return vms;
        }

        private List<VM> CreateSingleVmListFromOneHostPool()
        {
            return CreateVmList(oneHostPool, vm => vm.name_label == "local");
        }

        private List<VM> CreateSingleVmListFromTwoHostPool()
        {
            return CreateVmList(twoHostPool, vm => vm.name_label == "16local");
        }

        private List<VM> CreateVmList(string db, Predicate<VM> vmPredicate)
        {
            List<VM> vms = DatabaseManager.ConnectionFor(db).Cache.VMs.Where(vm => vmPredicate(vm)).ToList();
            Assert.IsNotEmpty(vms, "Selecting VMs for a test");
            return vms;
        } 
        #endregion
    }
}
