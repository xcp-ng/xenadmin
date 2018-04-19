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
using NUnit.Framework;
using XenAdmin.Controls;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.Controls
{
    public class LunComboBoxItemTests : UnitTester_SingleConnectionTestFixture
    {

        [TearDown]
        public void PerTestTearDown()
        {
            ObjectManager.ClearXenObjects(id);
            ObjectManager.RefreshCache(id);
        }

        [Test]
        [TestCase(0, ExpectedResult = true)]
        [TestCase(1, ExpectedResult = false)]
        [TestCase(22, ExpectedResult = false)]
        public bool VBDCountDisablesItem(int vbdCount)
        {
            Mock<VDI> vdi = GetMockVdi(vbdCount);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object);
            return item.Enabled;
        }

        [Test, TestCaseSource("AdditionalConstraintCases")]
        public void AdditionalConstraintsDisableItem(List<Predicate<VDI>> constraints, bool expectedEnabled, string description )
        {
            Mock<VDI> vdi = GetMockVdi(0);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object) {AdditionalConstraints = constraints};
            Assert.That(item.Enabled, Is.EqualTo(expectedEnabled), description);
        }

        [Test]
        public void NullAdditionalConstraintsThrows()
        {
            Mock<VDI> vdi = GetMockVdi(0);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object) { AdditionalConstraints = null };
            Assert.That(()=> item.Enabled, Throws.ArgumentNullException);
        }

   
        [Test]
        public void ItemMatchingWithMatchingValues()
        {
            Mock<VDI> vdi = GetMockVdi(0);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object){AdditionalConstraints = new List<Predicate<VDI>>()};
            LunComboBoxItem secondItem = new LunComboBoxItem(vdi.Object) { AdditionalConstraints = new List<Predicate<VDI>>() };
            item.EnableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.True);
            item.DisableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.False);
            item.EnableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.True);
        }

        [Test]
        public void ItemMatchingWithMismatchingValues()
        {
            Mock<VDI> vdi = GetMockVdi(0);
            Mock<VDI> secondVdi = GetMockVdi(0);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object) { AdditionalConstraints = new List<Predicate<VDI>>() };
            LunComboBoxItem secondItem = new LunComboBoxItem(secondVdi.Object) { AdditionalConstraints = new List<Predicate<VDI>>() };
            item.EnableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.True);
            item.DisableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.True);
            item.EnableItemMatching(secondItem);
            Assert.That(item.Enabled, Is.True);
        }

        [Test]
        [TestCase("MySizeText", "NameLabel", "NameLabel - MySizeText")]
        [TestCase("", "NameLabel", "NameLabel - ")]
        [TestCase("MySizeText", "", " - MySizeText")]
        [TestCase("", "", " - ")]
        [TestCase(null, null, " - ")]
        public void ItemToStringIncludeSizeDetails(string sizeText, string nameLabel, string expectedResult)
        {
            Mock<VDI> vdi = GetMockVdi(0);
            vdi.Setup(v => v.SizeText()).Returns(sizeText);
            vdi.Setup(v => v.name_label).Returns(nameLabel);
            LunComboBoxItem item = new LunComboBoxItem(vdi.Object);
            Assert.That(item.ToString(), Is.EqualTo(expectedResult));
        }

        private Mock<VDI> GetMockVdi(int vbdsToAdd)
        {
            MockVdiWithVbds vdiBuilder = ObjectFactory.Builder(ObjectBuilderType.VdiWithVbds, id) as MockVdiWithVbds;
            if (vdiBuilder == null)
                throw new ApplicationException("Builder was null");
            vdiBuilder.NumberOfVbdsToAdd = vbdsToAdd;
            return vdiBuilder.BuildObject() as Mock<VDI>;
        }

        #region Data Sources
        private static object[] AdditionalConstraintCases =
            {
                new object[]{new List<Predicate<VDI>>(), true, "TC1"},
                new object[]{new List<Predicate<VDI>>{ v => true}, false, "TC2"},
                new object[]{new List<Predicate<VDI>>{ v => false}, true, "TC3"},
                new object[]{new List<Predicate<VDI>>{ v => true, v=> true, v=> true}, false, "TC4"},
                new object[]{new List<Predicate<VDI>>{ v => false, v=> false, v=> true}, false, "TC5"},
                new object[]{new List<Predicate<VDI>>{ v => false, v=> false, v=> false}, true, "TC6"},
                new object[]{new List<Predicate<VDI>>{ v => true, v=> false, v=> true}, false, "TC7"}
            }; 
        #endregion
    }
}
