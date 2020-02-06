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
using NUnit.Framework;
using XenAdmin.Alerts;
using XenAdmin.Core;


namespace XenAdminTests.UnitTests.AlertTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class XenCenterUpdateAlertTests
    {
        [Test]
        public void VerifyStoredDataWithDefaultConstructor()
        {
            var version = new XenCenterVersion("6.0.2", "xc", true, false, "http://url",
                new DateTime(2011, 12, 09).ToString());

            ClassVerifiers.VerifyGetters(new XenCenterUpdateAlert(version),
                new AlertClassUnitTestData
                {
                    AppliesTo = XenAdmin.Branding.BRAND_CONSOLE,
                    FixLinkText = "Go to Web Page",
                    HelpID = "XenCenterUpdateAlert",
                    Description = "xc is now available. Download the new version from the "
                                  + XenAdmin.Branding.COMPANY_NAME_SHORT + " website.",
                    HelpLinkText = "Help",
                    Title = "xc is now available",
                    Priority = "Priority5"
                });
        }
    }
}