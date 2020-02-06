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
using System.Text;
using NUnit.Framework;
using XenAdmin.Controls;

namespace XenAdminTests.Controls
{
    [TestFixture, Category(TestCategories.Unit)]
    public class LongStringComboBoxTest
    {
        private class LongStringComboBoxWrapper : LongStringComboBox
        {
            public LongStringComboBoxWrapper()
            {
                Items.AddRange(new object[]{"This", "is", "some", "base", "data"});
                Width = 200;
            }
            public void TriggerDropDown()
            {
                OnDropDown(new EventArgs());
            }
        }
        
        [Test]
        public void CheckValidDropDownWidthSetUnderSize()
        {
            LongStringComboBoxWrapper cb = new LongStringComboBoxWrapper();
            cb.Items.Add("Word");
            cb.TriggerDropDown();
            Assert.That(cb.Items.Count, Is.EqualTo(6));
            Assert.That(cb.Width, Is.EqualTo(200));
            Assert.That(cb.DropDownWidth, Is.EqualTo(200));
        }

        [Test]
        public void CheckValidDropDownWidthSetOverSize()
        {
            LongStringComboBoxWrapper cb = new LongStringComboBoxWrapper();
            cb.Items.Add(ALongWord);
            cb.TriggerDropDown();
            Assert.That(cb.Items.Count, Is.EqualTo(6));
            Assert.That(cb.Width, Is.EqualTo(200));
            Assert.That(cb.DropDownWidth, Is.Not.EqualTo(200));
            Assert.That(cb.DropDownWidth, Is.GreaterThan(200));
        }

        private string ALongWord
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 2000; i++)
                {
                    sb.Append("Word");
                }
                return sb.ToString();
            }
        }
    }
}
