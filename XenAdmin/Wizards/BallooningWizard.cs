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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Controls;
using XenAdmin.Wizards.BallooningWizard_Pages;
using XenAPI;

namespace XenAdmin.Wizards
{
    public partial class BallooningWizard : XenWizardBase
    {
        private readonly ChooseVMs xenTabPageVMs;
        private readonly MemorySettings xenTabPageSettings;
        
        private long origStaticMax;
        private bool has_ballooning;

        public BallooningWizard(List<VM> vms)
            : base(vms[0].Connection)
        {
            InitializeComponent();

            xenTabPageVMs = new ChooseVMs();
            xenTabPageSettings = new MemorySettings();

            xenTabPageVMs.VMs = vms;
            AddPage(xenTabPageVMs);
            AddPage(xenTabPageSettings);

            if (vms.Count == 1)  // if there is only one VM, don't offer a choice
            {
                xenTabPageVMs.DisableStep = true;
                NextStep();
            }

            origStaticMax = vms[0].memory_static_max;
            has_ballooning = vms[0].has_ballooning();
        }

        protected override void OnShown(EventArgs e)
        {
            UpdateWizard();
            base.OnShown(e);
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(ChooseVMs))
                xenTabPageSettings.VMs = xenTabPageVMs.CheckedVMs;
        }

        protected override void FinishWizard()
        {
            xenTabPageSettings.UnfocusSpinners();
            bool canCloseWizard = BallooningDialog.ConfirmAndChange(this, xenTabPageVMs.CheckedVMs,
                has_ballooning ? (long)xenTabPageSettings.dynamic_min : (long)xenTabPageSettings.static_max,
                // dynamic_min and _max should stay equal to static_max for VMs without ballooning
                has_ballooning ? (long)xenTabPageSettings.dynamic_max : (long)xenTabPageSettings.static_max,
                (long)xenTabPageSettings.static_max, origStaticMax, xenTabPageSettings.AdvancedMode);
            if (canCloseWizard)
                base.FinishWizard();
            else
                FinishCanceled();
        }
        
        private void xenTabPageSettings_InstallTools(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

