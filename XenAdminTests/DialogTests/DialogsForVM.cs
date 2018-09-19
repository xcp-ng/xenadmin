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
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdminTests.DialogTests.state1_xml.DialogsForVM
{
    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class AttachDiskDialogTest : DialogTest<AttachDiskDialog>
    {
        protected override AttachDiskDialog NewDialog()
        {
            return new AttachDiskDialog(GetAnyVM());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogTest : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyVM(),false);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogAdvancedTest : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyVM(), true);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogTest_DefaultTemplate : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyDefaultTemplate(), false);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogAdvancedTest_DefaultTemplate : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyDefaultTemplate(), true);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogTest_UserTemplate : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyUserTemplate(), false);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BallooningDialogAdvancedTest_UserTemplate : DialogTest<BallooningDialog>
    {
        protected override BallooningDialog NewDialog()
        {
            return new BallooningDialog(GetAnyUserTemplate(), true);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ConfirmVMDeleteDialogTest_VM : DialogTest<ConfirmVMDeleteDialog>
    {
        protected override ConfirmVMDeleteDialog NewDialog()
        {
            return new ConfirmVMDeleteDialog(GetAnyVM());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ConfirmVMDeleteDialogTest_DefaultTemplate : DialogTest<ConfirmVMDeleteDialog>
    {
        protected override ConfirmVMDeleteDialog NewDialog()
        {
            return new ConfirmVMDeleteDialog(GetAnyDefaultTemplate());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class CopyVMDialogTest_VM : DialogTest<CopyVMDialog>
    {
        protected override CopyVMDialog NewDialog()
        {
            return new CopyVMDialog(GetAnyVM());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class CopyVMDialogTest_DefaultTemplate : DialogTest<CopyVMDialog>
    {
        protected override CopyVMDialog NewDialog()
        {
            return new CopyVMDialog(GetAnyDefaultTemplate());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RevertDialogTest : DialogTest<RevertDialog>
    {
        protected override RevertDialog NewDialog()
        {
            return new RevertDialog(GetAnyVM(),"");
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VcpuWarningDialogTest : DialogTest<VcpuWarningDialog>
    {
        protected override VcpuWarningDialog NewDialog()
        {
            return new VcpuWarningDialog(GetAnyVM());
        }
    }


    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VmSnapshotDialogTest : DialogTest<VmSnapshotDialog>
    {
        protected override VmSnapshotDialog NewDialog()
        {
            return new VmSnapshotDialog(GetAnyVM());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class VNCPasswordDialogTest : DialogTest<VNCPasswordDialog>
    {
        protected override VNCPasswordDialog NewDialog()
        {
            return new VNCPasswordDialog(null, GetAnyVM());
        }
    }
}
