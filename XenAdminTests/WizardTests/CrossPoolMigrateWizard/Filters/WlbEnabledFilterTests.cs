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

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    public class WlbEnabledFilterTests : UnitTester_TestFixture
    {
        private const string id = "id";
        private const string id2 = "id2";
        private static readonly string[] ids = new[] {id, id2};
        private const string hostFailureReason = "WLB is enabled on the host";
        private const string vmFailureReason = "WLB is enabled on the source";

        public WlbEnabledFilterTests() : base(ids)
        {
        }

        [TearDown]
        public void TestTearDown()
        {
            foreach (string currentid in ids)
            {
                ObjectManager.ClearXenObjects(currentid);
                ObjectManager.RefreshCache(currentid);
            }
        }

        [Test]
        [TestCase(true, true, Description = "Wlb enabled both", ExpectedResult = true)]
        [TestCase(false, false, Description = "Wlb disabled both", ExpectedResult = false)]
        [TestCase(true, false, Description = "Wlb enabled VM", ExpectedResult = true)]
        [TestCase(false, true, Description = "Wlb enabled target", ExpectedResult = true)]
        public bool WlbEnabledFilterResults(bool WlbEnabledVM, bool WlbEnabledTarget)
        {
            Mock<Pool> pool = ObjectManager.NewXenObject<Pool>(id);
            Mock<Pool> targetPool = ObjectManager.NewXenObject<Pool>(id2);
            WlbEnabledFilter filter = Setup(WlbEnabledVM, WlbEnabledTarget, pool, targetPool);
            bool filterResult = filter.FailureFound;

            pool.Verify(p => p.wlb_enabled, Times.AtLeastOnce());
            targetPool.Verify(tp => tp.wlb_enabled, Times.AtLeastOnce());
            return filterResult;
        }

        [Test]
        [TestCase(true, true, Description = "Wlb enabled both", ExpectedResult = hostFailureReason)]
        [TestCase(false, false, Description = "Wlb disabled both", ExpectedResult = vmFailureReason)]
        [TestCase(true, false, Description = "Wlb enabled VM", ExpectedResult = vmFailureReason)]
        [TestCase(false, true, Description = "Wlb enabled target", ExpectedResult = hostFailureReason)]
        public string WlbEnabledFailureReasons(bool WlbEnabledVM, bool WlbEnabledTarget)
        {
            Mock<Pool> pool = ObjectManager.NewXenObject<Pool>(id);
            Mock<Pool> targetPool = ObjectManager.NewXenObject<Pool>(id2);
            WlbEnabledFilter filter = Setup(WlbEnabledVM, WlbEnabledTarget, pool, targetPool);
            bool filterResult = filter.FailureFound;
            string reason = filter.Reason;

            pool.Verify(p => p.wlb_enabled, Times.AtLeastOnce());
            targetPool.Verify(tp => tp.wlb_enabled, Times.AtLeastOnce());
            return reason;
        }

        private WlbEnabledFilter Setup(bool WlbEnabledVM, bool WlbEnabledTarget, Mock<Pool> pool, Mock<Pool> targetPool)
        {
            //First connection
            Mock<VM> vm = ObjectFactory.BuiltObject<VM>(ObjectBuilderType.VmWithHomeServerHost, id);
            vm.Setup(v => v.allowed_operations).Returns(new List<vm_operations> { vm_operations.migrate_send });
            pool.Setup(p => p.wlb_enabled).Returns(WlbEnabledVM);
            pool.Setup(p => p.wlb_url).Returns("wlburl"); //Configured == true

            Mock<Host> master = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.TampaHost, id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(master.Object);

            //Second connection
            Mock<Host> targetHost = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.TampaHost, id2);
            ObjectManager.MockConnectionFor(id2).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(targetHost.Object);
            targetPool.Setup(p => p.wlb_enabled).Returns(WlbEnabledTarget);
            targetPool.Setup(p => p.wlb_url).Returns("wlburl"); //Configured == true

            return new WlbEnabledFilter(targetHost.Object, new List<VM> { vm.Object });
        }
    }
}
