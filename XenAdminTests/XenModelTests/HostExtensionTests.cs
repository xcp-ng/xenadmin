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
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class HostExtensionTests
    {
        [Test]
        [TestCase("0", 0)]
        [TestCase("2", 2)]
        [TestCase("40004", 40004)]
        [TestCase("My pet fish", 0)]
        [TestCase("-10", -10)]
        [TestCase(null, 0)]
        public void SocketParsingScenarios(string arrayValue, int expectedSockets)
        {
            const string SocketKetCpuInfo = "socket_count";
            Host host = new Host
                            {
                                cpu_info = new Dictionary<string, string> {{SocketKetCpuInfo, arrayValue}}
                            };
            int actualSockets = host.CpuSockets();
            Assert.That(actualSockets, Is.EqualTo(expectedSockets));
        }

        [Test]
        public void MissingSocketInfoParsing()
        {
            Host host = new Host
                            {
                                cpu_info = new Dictionary<string, string>()
                            };
            int actualSockets = host.CpuSockets();
            Assert.That(actualSockets, Is.EqualTo(0));
        }

        [Test]
        public void SocketsGivenANullCpuInfoParsing()
        {
            Host host = new Host
                            {
                                cpu_info = null
                            };
            int actualSockets = host.CpuSockets();
            Assert.That(actualSockets, Is.EqualTo(0));
        }
    }
}
