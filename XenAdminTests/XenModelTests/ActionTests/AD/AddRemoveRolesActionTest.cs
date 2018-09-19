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
using System.Linq;
using Moq;
using NUnit.Framework;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdminTests.XenModelTests.ActionTests.AD
{
    [TestFixture]
    public class AddRemoveRolesActionTest : ActionUnitTest<AddRemoveRolesAction>
    {

        protected override AddRemoveRolesAction NewAction()
        {
            Mock<Subject> subject = ObjectManager.NewXenObject<Subject>(id);

            List<Role> rolesToAdd = new List<Role>
                                        {
                                            ObjectManager.NewXenObject<Role>(id).Object,
                                            ObjectManager.NewXenObject<Role>(id).Object
                                        };
            List<Role> rolesToRemove = new List<Role> { ObjectManager.NewXenObject<Role>(id).Object };

            subject.Setup(s => s.other_config).Returns(new Dictionary<string, string>
                                                                  {{Subject.SUBJECT_DISPLAYNAME_KEY, "myRole"}});
            
            ObjectManager.MockProxyFor(id).Setup(
                x => x.subject_add_to_roles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(ValidResponse);

            ObjectManager.MockProxyFor(id).Setup(
                x => x.subject_remove_from_roles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(ValidResponse);

            return new AddRemoveRolesAction(ObjectManager.MockConnectionFor(id).Object, subject.Object, rolesToAdd, rolesToRemove);
        }

        protected override bool VerifyResult(AddRemoveRolesAction action)
        {
            ObjectManager.MockProxyFor(id).Verify(x => x.subject_add_to_roles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            ObjectManager.MockProxyFor(id).Verify(x => x.subject_remove_from_roles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            return ObjectManager.ConnectionFor(id).Cache.Roles.Count() == 3; //Nothing is actually added or deleted here
        }
    }
}
