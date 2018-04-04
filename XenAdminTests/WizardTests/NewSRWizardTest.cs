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

using NUnit.Framework;
using XenAdmin.Wizards;
using XenAdmin.Wizards.NewSRWizard_Pages.Frontends;

namespace XenAdminTests.WizardTests.state1_xml.NewSRWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class NewSRWizardTest_NFS : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFS()
            : base(new string[] { "Type", "Name", "Location" }, true, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfs").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(VHDoNFS), TestUtils.GetXenTabPage(wizard, "xenTabPageVhdoNFS"), "Wrong page");
                MW(() => TestUtils.GetTextBox(wizard, "xenTabPageVhdoNFS.NfsServerPathTextBox").Text = "foo:/bar");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_NFSNoPath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFSNoPath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfs").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_NFSIncompletePath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFSIncompletePath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfs").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(VHDoNFS), TestUtils.GetXenTabPage(wizard, "xenTabPageVhdoNFS"), "Wrong page");
                MW(() => TestUtils.GetTextBox(wizard, "xenTabPageVhdoNFS.NfsServerPathTextBox").Text = "foo:");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class NewSRWizardTest_iSCSI : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_iSCSI()
            : base(new string[] { "Type", "Name", "Location" }, false, false)  // can't finish because we haven't implemented probes of iSCSI LUNs etc.
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonIscsi").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_HBA : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_HBA()
            : base(new string[] { "Type", "Name", "Location" }, true, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonFibreChannel").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(LVMoHBA), TestUtils.GetXenTabPage(wizard, "xenTabPageLvmoHba"), "Wrong page");
                MW(() => TestUtils.GetButton(wizard, "xenTabPageLvmoHba.buttonSelectAll").PerformClick());
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_HBANoSelection : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_HBANoSelection()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonFibreChannel").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class NewSRWizardTest_CIFS : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_CIFS()
            : base(new string[] { "Type", "Name", "Location" }, true, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonCifsIso").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(CIFS_ISO), TestUtils.GetXenTabPage(wizard, "xenTabPageCifsIso"), "Wrong page");
                MW(() => TestUtils.GetComboBox(wizard, "xenTabPageCifsIso.comboBoxCifsSharename").Text = @"\\foo");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_CIFSNoPath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_CIFSNoPath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonCifsIso").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_CIFSIncompletePath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_CIFSIncompletePath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonCifsIso").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(CIFS_ISO), TestUtils.GetXenTabPage(wizard, "xenTabPageCifsIso"), "Wrong page");
                MW(() => TestUtils.GetComboBox(wizard, "xenTabPageCifsIso.comboBoxCifsSharename").Text = @"\foo\bar");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_NFSISO : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFSISO()
            : base(new string[] { "Type", "Name", "Location" }, true, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfsIso").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(NFS_ISO), TestUtils.GetXenTabPage(wizard, "xenTabPageNfsIso"), "Wrong page");
                MW(() => TestUtils.GetComboBox(wizard, "xenTabPageNfsIso.NfsServerPathComboBox").Text = "foo:/bar");
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_NFSISONoPath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFSISONoPath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfsIso").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewSRWizardTest_NFSISOIncompletePath : WizardTest<NewSRWizard>
    {
        public NewSRWizardTest_NFSISOIncompletePath()
            : base(new string[] { "Type", "Name", "Location" }, false, false)
        { }

        protected override NewSRWizard NewWizard()
        {
            return new NewSRWizard(GetAnyConnection());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Type")
                MW(TestUtils.GetRadioButton(wizard, "xenTabPageChooseSrType.radioButtonNfsIso").Select);
            else if (pageName == "Location")
            {
                Assert.IsInstanceOf(typeof(NFS_ISO), TestUtils.GetXenTabPage(wizard, "xenTabPageNfsIso"), "Wrong page");
                MW(() => TestUtils.GetComboBox(wizard, "xenTabPageNfsIso.NfsServerPathComboBox").Text = "foo:");
            }
        }
    }
}
