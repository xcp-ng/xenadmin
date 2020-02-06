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

using NUnit.Framework;
using XenCenterLib;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    class SubnetworkMaskValidatorTest
    {
        [Test, Sequential]
        public void TestValidNetmasks(
            [Values(
                "255.255.0.0",
                "128.0.0.0",
                "255.240.0.0",
                "255.128.0.0",
                "255.255.128.0",
                "255.255.255.255",
                "0.0.0.0",
                "128.0.0.0",
                "240.0.0.0"
                )] string validValues)

        {
            Assert.IsTrue(StringUtility.IsValidNetmask(validValues));
        }

        [Test, Sequential]
        public void TestInvalidNetmasks(
            [Values(
                "Gabor was here",
                "",
                null,
                "255.255,255.255",
                "255.0.255.0",
                "240.128.0.0",
                "255.00.0.0",
                "128.128.0.0",
                "192.168.1.1",
                "....",
                ".255.255.0.0",
                "255.255.-0.0",
                "255.255.0.255",
                "255.128.0.1",
                "257.0.0.0",
                "888.0.0.0",
                "99999999999999999999999999999999999999999999999999999999999999999999999999",
                "0e0,0.0.0",
                "5e888",
                "255 .240.0.0",
                "255.                   128.0.0 ",
                " 255.255.128.0"
                )] string invalidValues)
        {
            Assert.IsFalse(StringUtility.IsValidNetmask(invalidValues));
        }
    }
}
