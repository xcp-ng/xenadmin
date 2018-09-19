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
using System.Net;
using System.Text;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAPI;
using XenCenterLib;

namespace XenAdminTests.SearchTests
{
    // Test XenCenter's IP Address is never found on any XenObject.
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class IPAddressTest
    {
        [Test]
        public void Run()
        {
            IPHostEntry thisMachine = null;

            // Network on the GUI thread!!!
            try
            {
                thisMachine = Dns.GetHostEntry("");
            }
            catch
            {
                Assert.Fail("Couldn't resolve this machine's IP address");
            }

            PropertyAccessor ipAddress = PropertyAccessors.Get(PropertyNames.ip_address);

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                {
                    // We want this to error if the cast fails
                    ComparableList<ComparableAddress> addresses = (ComparableList<ComparableAddress>)ipAddress(o);
                    if (addresses == null)
                        continue;

                    foreach (ComparableAddress address in addresses)
                    {
                        foreach (IPAddress hostAddress in thisMachine.AddressList)
                        {
                            Assert.False(address.Equals(hostAddress),
                                String.Format("XenCenter address ({0}) appears on object '{1}'!", address, Helpers.GetName(o)));
                        }
                    }
                }
            }
        }
    }
}
