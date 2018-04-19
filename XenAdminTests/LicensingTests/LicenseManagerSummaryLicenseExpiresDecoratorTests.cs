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
using Moq;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Controls.SummaryPanel;
using XenAdmin.Dialogs;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.LicensingTests
{
    public class LicenseManagerSummaryLicenseExpiresDecoratorTests : UnitTester_SingleConnectionTestFixture
    {
        #region Test Data and helper classes
        public class TestCase
        {
            public TimeSpan ExpiresIn { get; set; }
            public DateTime? Expiry { get; set; }
            public LicenseStatus.HostState State { get; set; }
            public string Contains { get; set; }
        }

        private static IEnumerable<TestCase> TestData
        {
            get
            {
                yield return new TestCase
                 {
                     ExpiresIn = new TimeSpan(288, 0, 0, 0),
                     Expiry = new DateTime(2025, 10, 25),
                     Contains = "October 25, 2025"
                 };
                yield return new TestCase
                {
                    ExpiresIn = new TimeSpan(99288, 0, 0, 0),
                    Expiry = new DateTime(2025, 10, 25),
                    Contains = Messages.NEVER
                };
                yield return new TestCase
                {
                    ExpiresIn = new TimeSpan(99288, 0, 0, 0),
                    Expiry = null,
                    Contains = Messages.GENERAL_UNKNOWN
                };
            }
        }
        
        #endregion
        
        [TearDown]
        public void TearDownForTest()
        {
            ObjectManager.ClearXenObjects(id);
        }

        [Test, TestCaseSource("TestData")]
        public void StringBasedRepsonsesToRowState(TestCase tc)
        {
            Mock<SummaryTextComponent> baseComponent = new Mock<SummaryTextComponent>();
            StringBuilder sb = new StringBuilder();
            baseComponent.Setup(c => c.BuildSummary()).Returns(sb);

            Mock<Host> host = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.ClearwaterHost, id);
            Mock<ILicenseStatus> ls = new Mock<ILicenseStatus>();
            ls.Setup(l => l.CurrentState).Returns(tc.State);
            ls.Setup(l =>l.ExpiryDate).Returns(tc.Expiry);
            ls.Setup(l => l.LicenseExpiresIn).Returns(tc.ExpiresIn);

            CheckableDataGridViewRow row = new LicenseDataGridViewRow(host.Object, ls.Object);

            LicenseManagerSummaryLicenseExpiresDecorator decorator = new LicenseManagerSummaryLicenseExpiresDecorator(baseComponent.Object, row);
            decorator.BuildSummary();

            Assert.That(sb.ToString().Contains(Messages.LICENSE_MANAGER_SUMMARY_LICENSE_EXPIRES));
            Assert.That(sb.ToString().Contains(tc.Contains));

            baseComponent.Verify(c=>c.BuildSummary(), Times.Once());
        }

        [Test]
        public void TestFreeClearwater()
        {
            Mock<SummaryTextComponent> baseComponent = new Mock<SummaryTextComponent>();
            StringBuilder sb = new StringBuilder();
            baseComponent.Setup(c => c.BuildSummary()).Returns(sb);

            Mock<Host> host = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.ClearwaterHost, id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.IsConnected).Returns(true);
            Mock<ILicenseStatus> ls = new Mock<ILicenseStatus>();
            ls.Setup(l => l.CurrentState).Returns(LicenseStatus.HostState.Free);

            CheckableDataGridViewRow row = new LicenseDataGridViewRow(host.Object, ls.Object);

            LicenseManagerSummaryLicenseExpiresDecorator decorator = new LicenseManagerSummaryLicenseExpiresDecorator(baseComponent.Object, row);
            decorator.BuildSummary();

            Assert.That(sb.Length, Is.EqualTo(0));
            baseComponent.Verify(c => c.BuildSummary(), Times.Once());
        }
    }
}
