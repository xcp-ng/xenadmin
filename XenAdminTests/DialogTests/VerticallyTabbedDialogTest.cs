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
using System.Text;
using System.Linq;
using NUnit.Framework;
using XenAdmin.Controls;
using XenAdmin.Dialogs;

namespace XenAdminTests.DialogTests
{
    public abstract class VerticallyTabbedDialogTest<T> : DialogTest<T> where T : VerticallyTabbedDialog
    {
        private string[] tabNames;

        public VerticallyTabbedDialogTest(string[] tabNames)
        {
            this.tabNames = tabNames;
        }

        protected override void RunAfter()
        {
            // Check that stepping through the left-hand column
            // visits each of the tabs in order, and that they all have help
            Assert.AreEqual(tabNames.Length, dialog.Tabs.Length,
                string.Format("Wrong number of tabs: {0} != {1}", string.Join(", ", tabNames), GetNames(dialog.Tabs)));
            
            for (int i = 0; i < tabNames.Length; ++i)
            {
                int index = i;
                MW(() =>
                    {
                        var verticalTabs = TestUtils.GetFieldDeep<VerticalTabs>(dialog, "verticalTabs");
                        verticalTabs.SelectedIndex = index;
                        var vtab = dialog.SelectedTab;
                        Assert.IsNotNull(vtab, "Failed to select tab number " + index);
                        Assert.AreEqual(tabNames[index], vtab.Text, "Wrong tabs found at position " + index);
                        Assert.IsTrue(dialog.HasHelp(), "Help missing on tab: " + tabNames[index]);
                    });

                // Extra tests for each tab, defined in derived class
                TestTab(tabNames[index]);
            }
        }

        private string GetNames(VerticalTabs.IVerticalTab[] items)
        {
            return string.Join(", ", (from t in items select t.Text).ToArray());
        }

        protected virtual void TestTab(string name)
        {
        }

        protected void AddTab(string name)
        {
            Array.Resize(ref tabNames, tabNames.Length + 1);
            tabNames[tabNames.Length - 1] = name;
        }
    }
}
