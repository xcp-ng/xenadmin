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
using XenAdmin.Dialogs;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.LicensingTests
{
    public class LicenceStatusTests : UnitTester_SingleConnectionTestFixture
    {
        [TearDown]
        public void TestTearDown()
        {
            ObjectManager.ClearXenObjects(id);
        }

        //CA-103925 - in a pool situation the licence soonest to expire provides the pools expiry
        [Test]
        public void MinimumPoolLicenseValueOnSlave()
        {
            Mock<Pool> pool = ObjectFactory.BuiltObject<Pool>(ObjectBuilderType.PoolOfTwoClearwaterHosts, id);
            List<Mock<Host>> hosts = ObjectManager.GeneratedXenObjectsMocks<Host>(id);
            Mock<Host> master = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("master"));
            SetupMockHostWithExpiry(master, new DateTime(2013, 4, 1));
            Mock<Host> slave = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("slave"));
            SetupMockHostWithExpiry(slave, new DateTime(2012, 1, 12));
            
            using(LicenseStatus ls = new LicenseStatus(pool.Object))
            {
                Assert.True(ls.ExpiryDate.HasValue, "Expiry date doesn't have a value");
                Assert.That(ls.ExpiryDate.Value.ToShortDateString(), Is.EqualTo(new DateTime(2012, 1, 12).ToShortDateString()), "Expiry dates");
        
            }
        }

        [Test]
        public void EqualLicencesOnSlaveAndMasterInPool()
        {
            Mock<Pool> pool = ObjectFactory.BuiltObject<Pool>(ObjectBuilderType.PoolOfTwoClearwaterHosts, id);
            List<Mock<Host>> hosts = ObjectManager.GeneratedXenObjectsMocks<Host>(id);

            Mock<Host> master = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("master"));
            SetupMockHostWithExpiry(master, new DateTime(2013, 4, 1));
            Mock<Host> slave = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("slave"));
            SetupMockHostWithExpiry(slave, new DateTime(2013, 4, 1));

            using (LicenseStatus ls = new LicenseStatus(pool.Object))
            {
                Assert.True(ls.ExpiryDate.HasValue, "Expiry date doesn't have a value");
                Assert.That(ls.ExpiryDate.Value.ToShortDateString(), Is.EqualTo(new DateTime(2013, 4, 1).ToShortDateString()), "Expiry dates");
            }
        }

        [Test]
        public void MinimumPoolLicenseValueOnMaster()
        {
            Mock<Pool> pool = ObjectFactory.BuiltObject<Pool>(ObjectBuilderType.PoolOfTwoClearwaterHosts, id);
            List<Mock<Host>> hosts = ObjectManager.GeneratedXenObjectsMocks<Host>(id);

            Mock<Host> slave = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("slave"));
            SetupMockHostWithExpiry(slave, new DateTime(2013, 4, 1));

            Mock<Host> master = hosts.FirstOrDefault(h => h.Object.opaque_ref.Contains("master"));
            SetupMockHostWithExpiry(master, new DateTime(2012, 1, 12));

            using (LicenseStatus ls = new LicenseStatus(pool.Object))
            {
                Assert.True(ls.ExpiryDate.HasValue, "Expiry date doesn't have a value");
                Assert.That(ls.ExpiryDate.Value.ToShortDateString(),
                            Is.EqualTo(new DateTime(2012, 1, 12).ToShortDateString()), "Expiry dates");
            }
        }

        [Test]
        public void TestMinimumHostLicenseValue()
        {
            Mock<Host> saHost = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.ClearwaterHost, id);
            SetupMockHostWithExpiry(saHost, new DateTime(2015, 10, 25));

            using (LicenseStatus ls = new LicenseStatus(saHost.Object))
            {
                Assert.True(ls.ExpiryDate.HasValue, "Expiry date doesn't have a value");
                Assert.That(ls.ExpiryDate.Value.ToShortDateString(),
                            Is.EqualTo(new DateTime(2015, 10, 25).ToShortDateString()), "Expiry dates");
            }
        }

        [Test, TestCaseSource("CoarseExpiryDateTestCases")]
        public void LicenseExpiresIn(ExipryDateTestCase tc)
        {
            Mock<Host> host = ObjectFactory.BuiltObject<Host>(ObjectBuilderType.ClearwaterHost, id);
            SetupMockHostWithExpiry(host, new DateTime());
            using (OverriddenExpiresIn ls = new OverriddenExpiresIn(host.Object))
            {
                ls.DummyExpiresIn = tc.Expiry;
                ls.CalculateState();
                Assert.That(ls.LicenseExpiresIn, Is.EqualTo(tc.ExpectedExpiry));
            }
        }

        #region Private Helper Methods
        private static IEnumerable<ExipryDateTestCase> CoarseExpiryDateTestCases
        {
            get
            {
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(20, 2, 2, 2, 0),
                    ExpectedExpiry = new TimeSpan(20, 2, 2, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(1, 0, 0, 0, 0),
                    ExpectedExpiry = new TimeSpan(1, 0, 0, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(0, 2, 2, 2, 0),
                    ExpectedExpiry = new TimeSpan(0, 2, 2, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(0, 0, 2, 2, 0),
                    ExpectedExpiry = new TimeSpan(0, 0, 2, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(0, 0, 0, 2, 0),
                    ExpectedExpiry = new TimeSpan(0, 0, 0, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(0, 0, 0, 0, 5),
                    ExpectedExpiry = new TimeSpan(0, 0, 0, 0, 0)
                };
                yield return new ExipryDateTestCase
                {
                    Expiry = new TimeSpan(0, 0, 0, 0, 0),
                    ExpectedExpiry = new TimeSpan(0, 0, 0, 0, 0)
                };
            }
        }

        public class OverriddenExpiresIn : LicenseStatus
        {
            public OverriddenExpiresIn(IXenObject xo) : base(xo) { }
            public TimeSpan DummyExpiresIn { set; private get; }
            public void CalculateState() { CalculateLicenseState(); }
            protected override TimeSpan CalculateLicenceExpiresIn()
            {
                return DummyExpiresIn;
            }
        }

        public class ExipryDateTestCase
        {
            public TimeSpan Expiry { get; set; }
            public TimeSpan ExpectedExpiry { get; set; }
        }

        private void SetupMockHostWithExpiry(Mock<Host> host, DateTime expiry)
        {
            Assert.IsNotNull(host.Object, "Host object");
            host.Setup(h => h.LicenseExpiryUTC()).Returns(expiry);
            host.Setup(h => h.license_params).Returns(new Dictionary<string, string> { { "expiry", "expiry" } });
        }
        #endregion
    }
}
