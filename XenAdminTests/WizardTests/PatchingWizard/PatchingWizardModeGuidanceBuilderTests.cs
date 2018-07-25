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
using System.Text;
using Moq;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    class PatchingWizardModeGuidanceBuilderTests : UnitTester_SingleConnectionTestFixture
    {
        [Test]
        public void CheckRestartXapiText()
        {
            string msg;
            Mock<Host> host;
            Mock<Pool_patch> patch = SetupBuilder(after_apply_guidance.restartXAPI, out msg, out host);
            Assert.That(msg, Is.EqualTo("Restart toolstack in this order (master always first):\r\n\tMyHost (Master)\r\n"));
            patch.VerifyAll();
            host.VerifyAll();
        }

        [Test]
        public void CheckRestartHostText()
        {
            string msg;
            Mock<Host> host;
            Mock<Pool_patch> patch = SetupBuilder(after_apply_guidance.restartHost, out msg, out host);
            Assert.That(msg, Is.EqualTo("Restart these servers in this order (master always first):\r\n\tMyHost (Master)\r\n"));
            patch.VerifyAll();
            host.VerifyAll();
        }


        private Mock<Pool_patch> SetupBuilder(after_apply_guidance guidance, out string msg, out Mock<Host> host)
        {
            Mock<Pool_patch> patch = ObjectManager.NewXenObject<Pool_patch>(id);
            
            host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.IsMaster()).Returns(true);
            host.Setup(h => h.Name()).Returns("MyHost");

            if (guidance == after_apply_guidance.restartHost)
                host.Setup(h => h.uuid).Returns("MyHostUUID");
            
            host.Setup(h => h.patches).Returns(new List<XenRef<Host_patch>>());
            patch.Setup(p => p.after_apply_guidance).Returns(new List<after_apply_guidance> { guidance });
            bool outBool;
            msg = PatchingWizardModeGuidanceBuilder.ModeRetailPatch(new List<Host> { host.Object }, patch.Object, out outBool);
            return patch;
        }
    }
}
