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
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Wizards;
using XenCenterLib;

namespace XenAdminTests.WizardTests
{
    public abstract class WizardTest<T> : MainWindowTester where T : XenWizardBase
    {
        protected T wizard;
        private readonly string[] pageNames;
        private readonly bool canFinish;
        private readonly bool doFinish;

        protected Button btnNext;
        protected Button btnPrevious;
        protected Button btnCancel;

        protected WizardTest(string[] pageNames, bool canFinish = true, bool doFinish = true)
        {
            this.pageNames = pageNames;
            this.canFinish = canFinish;
            this.doFinish = doFinish;
        }


        [Test(Description = "Test that the Enter key takes us forward through the wizard")]
        public void RunWizardKeyboardTests()
        {
            SetUp();

            RunWizardTests(() =>
            {
                    //ensure the focus is not on Previous or Cancel, otherwise pressing Enter will result in clicking those
                    for (int j = 0; j < wizard.Controls.Count; j++)
                {
                        if (!btnCancel.Focused && !btnPrevious.Focused)
                            break;
                        wizard.Controls[j].Focus();
                    }
                    Win32.PostMessage(wizard.Handle, Win32.WM_KEYDOWN, new IntPtr((int)Keys.Enter), IntPtr.Zero);
                },
                btnPrevious.PerformClick,
                () =>
                    {
                    var key = doFinish ? Keys.Enter : Keys.Escape;
                    Win32.PostMessage(wizard.Handle, Win32.WM_KEYDOWN, new IntPtr((int)key), IntPtr.Zero);
                    });
        }

        [Test(Description = "Test that Next/Previous buttons take us forwards/backwards through the wizard")]
        [Timeout(100 * 1000)]
        public void RunWizardButtonTests()
                {
            SetUp();

            RunWizardTests(
                btnNext.PerformClick,
                btnPrevious.PerformClick,
                () =>
                {
                    if (doFinish)
                        btnNext.PerformClick();
                    else if (btnCancel.Enabled)
                        btnCancel.PerformClick();
                    else
                        wizard.Close();
                });
        }

<<<<<<< .mine
        [Test, MaxTime(100 * 1000)]
        public void RunWizardTests()
=======
        private void SetUp()

>>>>>>> .theirs
        {
            RunBefore();

            wizard = MW(NewWizard);
            MW(wizard.Show);

            btnNext = TestUtils.GetButton(wizard, "buttonNext");
            btnPrevious = TestUtils.GetButton(wizard, "buttonPrevious");
            btnCancel = TestUtils.GetButton(wizard, "buttonCancel");
        }

        private void RunWizardTests(Action goForwards, Action goBackwards, Action finish)
        {
            for (int i = 0; i < pageNames.Length; ++i)
            {
                bool lastPage = i == pageNames.Length - 1;

                Assert.AreEqual(pageNames[i], CurrentPageName(wizard), "Wrong page name");
                Assert.IsTrue(wizard.HasHelp(), $"Help missing for page {pageNames[i]}");

                TestPage(pageNames[i]);

                Assert.AreEqual(!lastPage || canFinish, btnNext.Enabled,
                    $"Next button enabled state wrong on page {pageNames[i]}");
                Assert.AreEqual(lastPage && !IsCancelButtonEnabledOnLastPage, !btnCancel.Enabled,
                    $"Cancel button enabled state wrong on page {pageNames[i]}");

                if (!lastPage)
                {
                    MW(goForwards);

                    // wait for any progress dialog to close
                    MWWaitFor(() => wizard.Visible && wizard.CanFocus);
                    
                    Assert.AreEqual(pageNames[i + 1], CurrentPageName(wizard),
                        $"Next button didn't get from page {pageNames[i]} to page {pageNames[i + 1]}");

                    if (btnPrevious.Enabled)
                    {
                        MW(goBackwards);

                        while (!btnNext.Enabled)
                            Thread.Sleep(1000);

                        Assert.AreEqual(pageNames[i], CurrentPageName(wizard),
                            $"Next then Previous didn't get back to page {pageNames[i]}");

                        MW(goForwards);

                        // wait for any progress dialog to close
                        MWWaitFor(() => wizard.Visible && wizard.CanFocus);

                        while (!btnPrevious.Enabled)
                            Thread.Sleep(1000);

                        Assert.AreEqual(pageNames[i + 1], CurrentPageName(wizard),
                            $"Next button didn't get from page {pageNames[i]} to page {pageNames[i + 1]}");
                    }
                }
                else
                {
                    MW(finish);
                }
            }

            RunAfter();
            MW(() => wizard.Dispose());
        }

        private string CurrentPageName(T wiz)
        {
            return wiz.CurrentStepTabPage.Text;
        }

        protected abstract T NewWizard();


        protected virtual void RunBefore()
        {
        }

        protected virtual void RunAfter()
        {
        }

        protected virtual void TestPage(string pageName)
        {
        }

        protected virtual bool IsCancelButtonEnabledOnLastPage
        {
            get { return true; }
        }
    }
}

namespace XenAdminTests.WizardTests.state1_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("TampaTwoHostPoolSelectioniSCSI.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.state4_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("state4.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.cowley1_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("TampaTwoHostPoolSelectioniSCSI.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.updatesState_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("TampaTwoHostPoolSelectioniSCSI.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.cowleyPolicies_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("TampaTwoHostPoolSelectioniSCSI.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.state5_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("state5.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.small_vms
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("small_vms.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.xapidb_app
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("TampaTwoHostPoolSelectioniSCSI.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.boston_db
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("boston-db.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.tampa_cpm_one_and_two_host_pools
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("tampa_poolofone_40.xml", "tampa-poolof16and23-xapi-db.xml")
        { }
    }
}