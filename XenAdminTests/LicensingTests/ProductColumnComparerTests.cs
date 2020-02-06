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
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.LicenseManagerLicenseRowComparers;

namespace XenAdminTests.UnitTests.LicenseManager
{
    [TestFixture, Category(TestCategories.Unit)]
    public class ProductColumnComparerTests
    {
        public class TestCase
        {
            public string ProductVersionA { get; set; }
            public string LicenseNameA { get; set; }
            public string ProductVersionB { get; set; }
            public string LicenseNameB { get; set; }
            public int Expected { get; set; }
            public bool NameChecked { get; set; }
            public bool VersionChecked { get; set; }
        }

        private static IEnumerable<TestCase> TestCases
        {
            get
            {
                yield return new TestCase
                {
                    ProductVersionA = "A",
                    ProductVersionB = "A",
                    LicenseNameA = "A",
                    LicenseNameB = "A",
                    Expected = 0,
                    NameChecked = true,
                    VersionChecked = true
                };
                yield return new TestCase
                {
                    LicenseNameA = "A",
                    LicenseNameB = "B",
                    Expected = -1,
                    NameChecked = true,
                    VersionChecked = false
                };
                yield return new TestCase
                {
                    LicenseNameA = "A",
                    LicenseNameB = "A",
                    ProductVersionA = "A",
                    ProductVersionB = "B",
                    Expected = -1,
                    NameChecked = true,
                    VersionChecked = true
                };
            }
        }

        [Test, TestCaseSource(typeof(ProductColumnComparerTests), nameof(TestCases))]
        public void ComparisonTests(TestCase tc)
        {
            var comparer = new ProductColumnComparer();
            var x = new Mock<LicenseDataGridViewRow>(null);
            var y = new Mock<LicenseDataGridViewRow>(null);
            x.Setup(l => l.LicenseProductVersion).Returns(tc.ProductVersionA);
            x.Setup(l => l.LicenseName).Returns(tc.LicenseNameA);
            y.Setup(l => l.LicenseProductVersion).Returns(tc.ProductVersionB);
            y.Setup(l => l.LicenseName).Returns(tc.LicenseNameB);
            
            Assert.That(comparer.Compare(x.Object, y.Object), Is.EqualTo(tc.Expected));
            
            if(tc.NameChecked)
            {
                x.Verify(s=>s.LicenseName, Times.Once());
                y.Verify(s=>s.LicenseName, Times.Once());
            }
            else
            {
                x.Verify(s => s.LicenseName, Times.Never());
                y.Verify(s => s.LicenseName, Times.Never());
            }
                
            if(tc.VersionChecked)
            {
                x.Verify(s => s.LicenseProductVersion, Times.Once());
                y.Verify(s => s.LicenseProductVersion, Times.Once());
            }
            else
            {
                x.Verify(s => s.LicenseProductVersion, Times.Never());
                y.Verify(s => s.LicenseProductVersion, Times.Never());
            }
        }
    }
}
