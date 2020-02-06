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
using XenAdmin.Actions;
using XenAPI;

namespace XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders
{
    public class MockActionFactory
    {
        private readonly MockObjectManager ObjectManager;
        public MockActionFactory(MockObjectManager manager)
        {
            ObjectManager = manager;
        }

        public AsyncAction MockActionFor(string connectionId, Type actionType)
        {
            if (actionType == typeof(DeleteVIFAction))
                return deleteVifAction(connectionId);
            if (actionType == typeof(AddRemoveRolesAction))
                return addRemoveRolesAction(connectionId);
            if (actionType == typeof(GpuAssignAction))
                return gpuAssignAction(connectionId);

            throw new ArgumentException("the type of action provided is not supported");

        }

        private AsyncAction deleteVifAction(string id)
        {
            Mock<VM> vm = ObjectManager.NewXenObject<VM>(id);
            vm.Setup(v => v.power_state).Returns(vm_power_state.Running);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<VM>>())).Returns(vm.Object);
            return new DeleteVIFAction(ObjectManager.NewXenObject<VIF>(id).Object);
        }

        private AsyncAction addRemoveRolesAction(string id)
        {
                Mock<Subject> subject = ObjectManager.NewXenObject<Subject>(id);
                subject.Setup(s => s.other_config).Returns(new Dictionary<string, string> { { Subject.SUBJECT_DISPLAYNAME_KEY, "myRole" } });
                return new AddRemoveRolesAction(ObjectManager.MockConnectionFor(id).Object, subject.Object, 
                                                new List<Role>{ObjectManager.NewXenObject<Role>(id).Object}, 
                                                new List<Role>{ObjectManager.NewXenObject<Role>(id).Object});
        }

        private AsyncAction gpuAssignAction(string id)
        {
            Mock<VGPU> vgpu = ObjectManager.NewXenObject<VGPU>(id);
            List<VGPU> vgpus = new List<VGPU>() { vgpu.Object };
            Mock<VM> vm = ObjectManager.NewXenObject<VM>(id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.ResolveAll(It.IsAny<List<XenRef<VGPU>>>())).Returns(new List<VGPU>());
            ObjectManager.MockProxyFor(id).Setup(
                p =>
                p.async_vgpu_create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "0",
                              It.IsAny<object>(), It.IsAny<string>())).Returns(new Response<string>("ok"));
            return new GpuAssignAction(vm.Object, vgpus);
        }
    }
}
