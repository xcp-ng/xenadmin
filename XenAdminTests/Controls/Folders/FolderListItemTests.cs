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


using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Controls;

namespace XenAdminTests.Controls.Folders
{
    [TestFixture, Category(TestCategories.Unit)]
    public class FolderListItemTests
    {
        private Control parent;

        [SetUp]
        public void TestSetup()
        {
            parent = new Control { Font = new Font(FontFamily.GenericMonospace, 10) };
        }

        [TearDown]
        public void TestTearDown()
        {
            parent.Dispose();
        }

        [Test]
        [TestCase("", ExpectedResult = 43)]
        [TestCase("mypath", ExpectedResult = 44)]
        [TestCase("mypathmypathmypathmypathmypathmypathmypathmypathmypathmypathmypath", ExpectedResult = 454)]
        public int CalculatePreferedSizes(string path)
        {
            var item = new FolderListItem(path, FolderListItem.AllowSearch.None, false)
            {
                MaxWidth = 50,
                Parent = parent
            };

            Assert.AreEqual(path, item.Path);
            return item.PreferredSize.Width;
        }
    }
}
