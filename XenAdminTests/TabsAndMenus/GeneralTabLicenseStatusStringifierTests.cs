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
using Moq;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Dialogs;
using XenAdmin.TabPages;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    public class GeneralTabLicenseStatusStringifierTests : UnitTester_SingleConnectionTestFixture
    {
        #region Test Data and Objects

        public class TestData
        {
            public bool IsFree { get; set; }
            public DateTime? Expiry { get; set; }
            public DateTime Ref { get; set; }
            public string Expected { get; set; }
            public LicenseStatus.HostState State { get; set; }
        }

        private static IEnumerable<TestData> StatusTestData
        {
            get
            {
                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2002, 10, 25),
                    Ref = new DateTime(2012, 10, 10),
                    Expected = Messages.LICENSE_UNSUPPORTED,
                    State = LicenseStatus.HostState.Expired
                };

                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2012, 10, 25, 1, 2, 0, 0),
                    Ref = new DateTime(2012, 10, 25, 1, 1, 0),
                    Expected = Messages.LICENSE_EXPIRES_ONE_MIN
                };

                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2012, 10, 25, 2, 0, 0),
                    Ref = new DateTime(2012, 10, 25, 1, 30, 0),
                    Expected = String.Format(Messages.LICENSE_EXPIRES_MINUTES, "30")
                };

                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2012, 10, 26, 2, 0, 0),
                    Ref = new DateTime(2012, 10, 25, 8, 0, 0),
                    Expected = String.Format(Messages.LICENSE_EXPIRES_HOURS, "18")
                };

                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2012, 10, 12, 1, 0, 0),
                    Ref = new DateTime(2012, 9, 25, 1, 0, 0),
                    Expected = String.Format(Messages.LICENSE_EXPIRES_DAYS, "17")
                };

                yield return new TestData
                {
                    IsFree = false,
                    Expiry = new DateTime(2012, 10, 12),
                    Ref = new DateTime(2012, 6, 25),
                    Expected = Messages.LICENSE_LICENSED
                };
            }
        } 
        #endregion

        [Test, TestCaseSource("StatusTestData")]
        public void StatusToNonTimeStringConversion(TestData data)
        {
            Mock<ILicenseStatus> mockStatus = new Mock<ILicenseStatus>();
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.IsFreeLicense()).Returns(data.IsFree);
            mockStatus.Setup(s => s.LicencedHost).Returns(host.Object);
            mockStatus.Setup(l => l.ExpiryDate).Returns(data.Expiry);
            mockStatus.Setup(s => s.Updated).Returns(true);
            mockStatus.Setup(s => s.LicenseExpiresExactlyIn).Returns(data.Expiry.Value.Subtract(data.Ref));
            mockStatus.Setup(s => s.CurrentState).Returns(data.State);
            GeneralTabLicenseStatusStringifier fs = new GeneralTabLicenseStatusStringifier(mockStatus.Object);
            Assert.That(fs.ExpiryStatus, Is.EqualTo(data.Expected));
        }

        [Test]
        public void NullStatusProvided()
        {
            GeneralTabLicenseStatusStringifier fs = new GeneralTabLicenseStatusStringifier(null);
            Assert.That(fs.ExpiryStatus, Is.EqualTo(Messages.GENERAL_UNKNOWN));
        }

        [Test]
        public void StatusNotUpdatedProvided()
        {
            Mock<ILicenseStatus> mockStatus = new Mock<ILicenseStatus>();
            mockStatus.Setup(s => s.Updated).Returns(false);
            GeneralTabLicenseStatusStringifier fs = new GeneralTabLicenseStatusStringifier(mockStatus.Object);
            Assert.That(fs.ExpiryStatus, Is.EqualTo(Messages.GENERAL_UNKNOWN));
        }

    }
}
