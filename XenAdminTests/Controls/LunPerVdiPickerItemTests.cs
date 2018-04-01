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
using Moq;
using NUnit.Framework;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdminTests.Controls
{
    public class LunPerVdiPickerItemTests : UnitTester_SingleConnectionTestFixture
    {

        [Test]
        public void NullSrIsValidForMapping()
        {
            Mock<VDI> vdi = ObjectManager.NewXenObject<VDI>(id);
            LunPerVdiPickerItem item = new LunPerVdiPickerItem(null, vdi.Object);
            Assert.That(item.IsValidForMapping, Is.False);
        }

        [Test]
        [TestCase(true, ExpectedResult = true)]
        [TestCase(false, ExpectedResult = false)]
        public bool IsValidForMapping(bool lunPerVDI)
        {
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            sr.Setup(s => s.HBALunPerVDI()).Returns(lunPerVDI);
            Mock<VDI> vdi = ObjectManager.NewXenObject<VDI>(id);
            LunPerVdiPickerItem item = new LunPerVdiPickerItem(sr.Object, vdi.Object);
            return item.IsValidForMapping;
        }

        private class LunPerVdiPickerItemWrapper : LunPerVdiPickerItem
        {
            public LunPerVdiPickerItemWrapper(SR LUNsourceSr, VDI vdi) : base(LUNsourceSr, vdi)
            {
            }

            public string VdiColumn
            {
                get { return VdiColumnText; }
            }

            public string SrColumn
            {
                get { return SrColumnText; }
            }
        }

        [Test]
        public void VdiColumnTextWithNull()
        {
            LunPerVdiPickerItemWrapper item = new LunPerVdiPickerItemWrapper(null, null);
            Assert.That(item.VdiColumn, Is.EqualTo(String.Empty));
        }

        [Test]
        public void SrColumnTextWithNull()
        {
            LunPerVdiPickerItemWrapper item = new LunPerVdiPickerItemWrapper(null, null);
            Assert.That(item.SrColumn, Is.EqualTo(String.Empty));
        }

        [Test]
        public void VdiColumnText()
        {
            const string dummy = "DUMMY_STRING";
            Mock<VDI> vdi = ObjectManager.NewXenObject<VDI>(id);
            vdi.Setup(v => v.Name()).Returns(dummy);
            LunPerVdiPickerItemWrapper item = new LunPerVdiPickerItemWrapper(null, vdi.Object);
            Assert.That(item.VdiColumn, Is.EqualTo(dummy));
            vdi.VerifyAll();
        }

        [Test]
        public void SrColumnText()
        {
            const string dummy = "DUMMY_STRING";
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            sr.Setup(v => v.Name()).Returns(dummy);
            LunPerVdiPickerItemWrapper item = new LunPerVdiPickerItemWrapper(sr.Object, null);
            Assert.That(item.SrColumn, Is.EqualTo(dummy));
            sr.VerifyAll();
        }

        [Test]
        public void NullEqualsReturnsFalse()
        {
            LunPerVdiPickerItem item = new LunPerVdiPickerItem(null, null);
            Assert.That(item.Equals(null), Is.False);
        }

        [Test]
        public void NullVdiInComparisonObjectEquals()
        {
            LunPerVdiPickerItem nullItem = new LunPerVdiPickerItem(null, null);
            Mock<VDI> vdi = ObjectManager.NewXenObject<VDI>(id);
            vdi.Setup(v => v.Name()).Returns("NAME");
            LunPerVdiPickerItem item = new LunPerVdiPickerItem(null, vdi.Object);
            Assert.That(item.Equals(nullItem), Is.False);
            vdi.Verify(v=>v.Name());
        }

        [Test]
        public void NullVdiInBaseObjectEquals()
        {
            LunPerVdiPickerItem nullItem = new LunPerVdiPickerItem(null, null);
            Mock<VDI> vdiA = ObjectManager.NewXenObject<VDI>(id);
            LunPerVdiPickerItem item = new LunPerVdiPickerItem(null, vdiA.Object);
            Assert.That(nullItem.Equals(item), Is.False);
        }

        [Test]
        public void VdiBaseEqualsIsCalled()
        {
            Mock<VDI> vdiA = ObjectManager.NewXenObject<VDI>(id);
            vdiA.Object.opaque_ref = "OREF1";
            LunPerVdiPickerItem itemA = new LunPerVdiPickerItem(null, vdiA.Object);

            Mock<VDI> vdiB = ObjectManager.NewXenObject<VDI>(id);
            vdiB.Object.opaque_ref = "OREF2";
            LunPerVdiPickerItem itemB = new LunPerVdiPickerItem(null, vdiB.Object);

            Assert.That(itemB.Equals(itemA), Is.False);

        }

    }
}
