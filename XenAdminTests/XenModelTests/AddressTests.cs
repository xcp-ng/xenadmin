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
using NUnit.Framework;
using XenAdmin.Core;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AddressTests
    {
        private readonly Dictionary<string, string> _simpleNetworks = new Dictionary<string, string>
        {
            {"0/ip", "10.71.57.53"},
            {"0/ipv4/0", "10.71.57.53"},
            {"0/ipv6/0", "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"}
        };

        private readonly Dictionary<string, string> _compoundNetworks = new Dictionary<string, string>
        {
            {"0/ip", "192.168.0.1\n192.168.0.2"},
            {"1/ip", "192.168.0.3%n192.168.0.4"}
        };

        [Test]
        public void TestSimple()
        {
            var expected = new List<string>
            {
                "10.71.57.53",
                "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"
            };
            var actual = Helpers.FindIpAddresses(_simpleNetworks, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestUnusedDevice()
        {
            var actual = Helpers.FindIpAddresses(_simpleNetworks, "1");
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void TestCompoundNetworksNewlineSeparated()
        {
            var expected = new List<string>
            {
                "192.168.0.1",
                "192.168.0.2"
            };
            var actual = Helpers.FindIpAddresses(_compoundNetworks, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestCompoundNetworkPercentNSeparated()
        {
            var expected = new List<string>
            {
                "192.168.0.3",
                "192.168.0.4"
            };
            var actual = Helpers.FindIpAddresses(_compoundNetworks, "1");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestCompoundNetworkSkipsBlanks()
        {
            var given = new Dictionary<string, string>
            {
                {"0/ip", "10.0.0.24\n\n10.0.0.26"}
            };
            var expected = new List<string>
            {
                "10.0.0.24",
                "10.0.0.26"
            };
            var actual = Helpers.FindIpAddresses(given, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
