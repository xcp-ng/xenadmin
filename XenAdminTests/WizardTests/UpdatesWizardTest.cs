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

using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Wizards.PatchingWizard;

namespace XenAdminTests.WizardTests.updatesState_xml
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class UpdatesAutomaticModeWizardTest : WizardTest<PatchingWizard>
    {
        //TODO: as a setup step move succeed.xsupdate to succeed.<BrandManager.ExtensionSearch>

        public UpdatesAutomaticModeWizardTest()
            : base(new[] {"Before You Start", "Select Update", "Select Servers", "Upload", "Prechecks", "Update Mode", "Install Update"})
        { }

        protected override PatchingWizard NewWizard()
        {
            return new PatchingWizard();
        }

        protected override bool IsCancelButtonEnabledOnLastPage => false;

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select Update")
            {
                DataGridView dataGrid = TestUtils.GetDataGridView(wizard, "PatchingWizard_SelectPatchPage.dataGridViewPatches");
                MW(dataGrid.Select);
                if (dataGrid.Rows.Count > 0) 
                    MW(() => dataGrid.Rows[dataGrid.Rows.Count - 1].Selected = true);
                else
                {
                    MW(TestUtils.GetRadioButton(wizard, "PatchingWizard_SelectPatchPage.selectFromDiskRadioButton").Select);
                    MW(() => TestUtils.GetTextBox(wizard, "PatchingWizard_SelectPatchPage.fileNameTextBox").Text =
                        TestResource("succeed." + XenAdmin.Branding.Update));
                }
            }
            else if (pageName == "Select Servers")
            {
                MW(TestUtils.GetButton(wizard, "PatchingWizard_SelectServers.buttonSelectAll").PerformClick);
            }
            else if (pageName == "Upload")
            {
                while (!btnNext.Enabled)
                    Thread.Sleep(1000);
            }
            else if (pageName == "Prechecks")
            {
                while (!btnNext.Enabled)
                    Thread.Sleep(1000);
            }
            else if (pageName == "Install Update")
            {
                while (!btnNext.Enabled)
                    Thread.Sleep(1000);
                Assert.IsTrue(TestUtils.GetTextBox(wizard, "PatchingWizard_PatchingPage.textBoxLog").Text != "");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class UpdatesManualModeWizardTest : UpdatesAutomaticModeWizardTest
    {
        protected override void TestPage(string pageName)
        {
            base.TestPage(pageName);

            if (pageName == "Update Mode")
            {
                MW(TestUtils.GetRadioButton(wizard, "PatchingWizard_ModePage.ManualRadioButton").PerformClick);
            }
        }
    }
}
