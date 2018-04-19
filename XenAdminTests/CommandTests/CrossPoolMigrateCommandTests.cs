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
using XenAdmin;
using XenAdmin.Commands;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.CommandTests
{
    public class CrossPoolMigrateCommandUnitTests : UnitTester_TestFixture
    {
        public const string id = "test";
        public const string id2 = "test2";
        private static readonly string[] ids = new []{id, id2};

        /* run these tests without a target host, so they're forced to go further
           than the CrossPoolCanMigrateFilter and perform the rest of the checks */

        public CrossPoolMigrateCommandUnitTests() : base(ids) { }

        [TearDown]
        public void TestTearDown()
        {
            foreach (string currentid in ids)
            {
                ObjectManager.ClearXenObjects(currentid);
                ObjectManager.RefreshCache(currentid);
            }
        }

        [Test, Category(TestCategories.SmokeTest)]
        [TestCase(true, Description = "Is LunPerVdi", ExpectedResult = false)]
        [TestCase(false, Description = "Is Not LunPerVdi", ExpectedResult = true)]
        public bool VerifyLunPerVdiBehaviour(bool IsLunPerVdi)
        {
            Mock<VM> vm = ObjectFactory.BuiltObject<VM>(ObjectBuilderType.VmWithHomeServerHost, id);
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            vm.Setup(v => v.SRs()).Returns(new List<SR> {sr.Object});
            vm.Setup(v => v.allowed_operations).Returns(new List<vm_operations> {vm_operations.migrate_send});
            sr.Setup(s => s.HBALunPerVDI()).Returns(IsLunPerVdi);

            IMainWindow mw = new MockMainWindow();

            var cmd = new CrossPoolMigrateCommand(mw, new List<SelectedItem> { new SelectedItem(vm.Object)}, null);
            return cmd.CanExecute();
        }

        [Test, Category(TestCategories.SmokeTest)]
        [TestCase(true, Description = "Wlb enabled", ExpectedResult = false)]
        [TestCase(false, Description = "Wlb disabled", ExpectedResult = true)]
        public bool IntrapoolWlbEnabledBehaviour(bool WlbEnabled)
        {
            Mock<VM> vm = ObjectFactory.BuiltObject<VM>(ObjectBuilderType.VmWithHomeServerHost, id);
            Mock<Host> targetHost = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.TampaHost, id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(targetHost.Object);
            vm.Setup(v => v.allowed_operations).Returns(new List<vm_operations> { vm_operations.migrate_send });
            Mock<Pool> pool = ObjectManager.NewXenObject<Pool>(id);
            pool.Setup(p => p.wlb_enabled).Returns(WlbEnabled);
            pool.Setup(p => p.wlb_url).Returns("wlburl"); //Configured == true
            
            IMainWindow mw = new MockMainWindow();
            var cmd = new CrossPoolMigrateCommand(mw, new List<SelectedItem> { new SelectedItem(vm.Object) }, null);
            bool canExecute = cmd.CanExecute();
            pool.Verify(p=>p.wlb_enabled, Times.AtLeastOnce());
            return canExecute;
        }


        [Test, Category(TestCategories.SmokeTest)]
        [TestCase(true, true, Description = "Wlb enabled both", ExpectedResult = false)]
        [TestCase(false, false, Description = "Wlb disabled both", ExpectedResult = true)]
        [TestCase(true, false, Description = "Wlb enabled VM", ExpectedResult = false)]
        [TestCase(false, true, Description = "Wlb enabled target", ExpectedResult = true)]
        public bool CrossPoolWlbEnabledBehaviour(bool WlbEnabledVM, bool WlbEnabledTarget)
        {
            //First connection
            Mock<VM> vm = ObjectFactory.BuiltObject<VM>(ObjectBuilderType.VmWithHomeServerHost, id);
            Mock<Host> master = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.TampaHost, id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(master.Object);
            vm.Setup(v => v.allowed_operations).Returns(new List<vm_operations> { vm_operations.migrate_send });
            Mock<Pool> pool = ObjectManager.NewXenObject<Pool>(id);
            pool.Setup(p => p.wlb_enabled).Returns(WlbEnabledVM);
            pool.Setup(p => p.wlb_url).Returns("wlburl"); //Configured == true

            //Second connection
            Mock<Host> targetHost = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.TampaHost, id2);
            Mock<Pool> targetPool = ObjectManager.NewXenObject<Pool>(id2);
            ObjectManager.MockConnectionFor(id2).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(targetHost.Object);
            targetPool.Setup(p => p.wlb_enabled).Returns(WlbEnabledTarget);
            targetPool.Setup(p => p.wlb_url).Returns("wlburl"); //Configured == true

            //Command
            IMainWindow mw = new MockMainWindow();
            var cmd = new CrossPoolMigrateCommand(mw, new List<SelectedItem> { new SelectedItem(vm.Object) }, null);
            bool canExecute = cmd.CanExecute();

            //As the command is launching the wizard it should only need to 
            //check the VMs are on Wlb as the target can be changed in the wizard anyhow
            pool.Verify(p => p.wlb_enabled, Times.AtLeastOnce());
            targetPool.Verify(tp => tp.wlb_enabled, Times.Never());
            return canExecute;
        }


    }
}
