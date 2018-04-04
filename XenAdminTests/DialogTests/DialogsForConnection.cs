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
using NUnit.Framework;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests.DialogTests.state1_xml.DialogsForConnection
{
    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class AddServerDialogTest_true : DialogTest<AddServerDialog>
    {
        protected override AddServerDialog NewDialog()
        {
            return new AddServerDialog(GetAnyConnection(), true);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class AddServerDialogTest_false : DialogTest<AddServerDialog>
    {
        protected override AddServerDialog NewDialog()
        {
            return new AddServerDialog(GetAnyConnection(), false);
        }
    }


    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class ConnectingToServerDialogTest : DialogTest<ConnectingToServerDialog>
    {
        IXenConnection connection = null;

        [TestFixtureSetUp]
        public void ChooseConnection()
        {
            connection = GetAnyConnection();
        }

        protected override ConnectingToServerDialog NewDialog()
        {
            return new ConnectingToServerDialog(connection);
        }

        protected override void RunAfter()
        {
            dialog.SetText("Connecting to " + connection.Hostname);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class CustomFieldsDialogTest : DialogTest<CustomFieldsDialog>
    {
        protected override CustomFieldsDialog NewDialog()
        {
            return new CustomFieldsDialog(GetAnyConnection());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DuplicateTemplateNameDialogTest : DialogTest<DuplicateTemplateNameDialog>
    {
        protected override DuplicateTemplateNameDialog NewDialog()
        {
            return new DuplicateTemplateNameDialog(GetAnyConnection());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class InstallToolsWarningDialogTest : DialogTest<InstallToolsWarningDialog>
    {
        protected override InstallToolsWarningDialog NewDialog()
        {
            return new InstallToolsWarningDialog(GetAnyConnection());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class IscsiChoicesDialogTest_SRInfo : DialogTest<IscsiChoicesDialog>
    {
        protected override IscsiChoicesDialog NewDialog()
        {
            XenAPI.SR.SRInfo srinfo = new XenAPI.SR.SRInfo("1234-5678-9012-3456", 200000000000);
            return new IscsiChoicesDialog(GetAnyConnection(), srinfo, SR.SRTypes.lvmoiscsi, SR.SRTypes.lvmoiscsi);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class IscsiChoicesDialogTest_FibreChannelDevice : DialogTest<IscsiChoicesDialog>
    {
        protected override IscsiChoicesDialog NewDialog()
        {
            var dev = new FibreChannelDevice(
                "1234-5678-9012-3456", "path", "vendor", 200000000000, null, "adapter", "1", "ID", "lun");
            return new IscsiChoicesDialog(GetAnyConnection(), dev);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NewCustomFieldDialogTest : DialogTest<NewCustomFieldDialog>
    {
        protected override NewCustomFieldDialog NewDialog()
        {
            return new NewCustomFieldDialog(GetAnyConnection());
        }
    }
}
