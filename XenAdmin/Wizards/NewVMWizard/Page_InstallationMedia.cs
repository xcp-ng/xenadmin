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
using System.Linq;
using XenAdmin.Actions.VMActions;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Core;
using System.Diagnostics;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenCenterLib;
using System.Windows.Forms;
using System.Drawing;
using BootMode = XenAdmin.Actions.VMActions.BootMode;

namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_InstallationMedia : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private VM m_template;

        public Page_InstallationMedia()
        {
            InitializeComponent();
        }

        bool defaultTemplate, userTemplate, hvm, eli, installMethods, installCd, installUrl, cds, installed;

        public Host Affinity { private get; set; }

        public bool ShowInstallationMedia { private get; set; }

        public bool IsSelectedTemplateHVM { get { return SelectedTemplate.IsHVM(); } }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            VM oldTemplate = m_template;
            m_template = SelectedTemplate;

            // We only want to do all these updates if the template has changed (or has gone from null -> selected)
            if (oldTemplate == m_template)
                return;

            /* This method is supposed to do the following:
             * 
             *  PV      -> Install from DVD drive (no empty)
             *             Url
             * 
             *  HVM     -> Install from DVD drive (no empty)
             *             Boot from network
             * 
             *  Custom  -> DVD drive (inc empty)
             * 
             *  Disable Installation method section if the custom template has no DVD drive (except debian etch template)
             */

            PvBootBox.Visible = !IsSelectedTemplateHVM;
            PvBootTextBox.Text = m_template.PV_args;

            bootModesControl1.Visible = IsSelectedTemplateHVM &&
                                        BootModesControl.ShowBootModeOptions(SelectedTemplate.Connection);
            ;
            bootModesControl1.TemplateVM = m_template;

            if (!ShowInstallationMedia)
            {
                CdRadioButton.Checked = UrlRadioButton.Checked = false;
                CdDropDownBox.Items.Clear();
                UrlTextBox.Text = string.Empty;
                panelInstallationMethod.Enabled = false;
                return;
            }
            panelInstallationMethod.Enabled = true;

            defaultTemplate = m_template.DefaultTemplate();
            userTemplate = !defaultTemplate;
            hvm = m_template.IsHVM();
            eli = !hvm && m_template.PV_bootloader == "eliloader";

            var tmplMethods = m_template.InstallMethods();
            installMethods = !string.IsNullOrEmpty(tmplMethods);
            installCd = installMethods && tmplMethods.Contains("cdrom") && (hvm || eli);
            installUrl = installMethods &&
                         (tmplMethods.Contains("http") || tmplMethods.Contains("ftp") || tmplMethods.Contains("nfs")) &&
                         eli;
            cds = Helpers.CDsExist(Connection);
            installed = userTemplate || !installMethods;

            CdRadioButton.Text = installed ? Messages.NEWVMWIZARD_INSTALLMEDIA_DVD : Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLDVD;
            CdRadioButton.Enabled = (installed || installCd) && cds;
            CdDropDownBox.Empty = installed;

            UrlRadioButton.Text = hvm ? Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLPXE : Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLURL;
            UrlRadioButton.Enabled = (!installed && ((eli && installUrl) || hvm));

            UrlTextBox.Visible = !hvm;
            UrlTextBox.Text = !installed ? m_template.InstallRepository() ?? "" : "";

            if (installed || (installCd && cds)) // if installed we will always have the empty cd
            {
                CdRadioButton.Checked = true;
                CdDropDownBox.Select();
            }
            else if (eli && installUrl)
            {
                UrlRadioButton.Checked = true;
                UrlTextBox.Select();
            }
            else
            {
                // oh dear, select anything that is enabled
                if (CdRadioButton.Enabled)
                    CdRadioButton.Checked = true;
                else if (UrlRadioButton.Enabled)
                    UrlRadioButton.Checked = true;
                else
                    Trace.Assert(false, string.Format("No install options were enabled, something is wrong with the template '{0}'", m_template.Name()));
            }
            if(IsBootFromNetworkCustomTemplate(userTemplate))
            {
                UrlRadioButton.Enabled = true;
                UrlRadioButton.Checked = true;
                CdRadioButton.Enabled = false;
            }

            LoadCdBox();

            UpdateEnablement();
        }

        public BootMode SelectedBootMode { get { return IsSelectedTemplateHVM ? bootModesControl1.SelectedOption : BootMode.NOT_AVAILABLE; } }

        private bool IsBootFromNetworkCustomTemplate(bool userTemplate)
        {
            return (userTemplate && m_template.GetBootOrder().StartsWith("N"));
        }

        private void LoadCdBox()
        {
            CdDropDownBox.vm = m_template;

            CdDropDownBox.refreshAll();

            RegisterBespokeEventsAgainstCdDropDownBox();

            VBD cddrive = m_template.FindVMCDROM();

            VDI cd = cddrive != null && !cddrive.empty 
                         ? Connection.Resolve<VDI>(cddrive.VDI) 
                         : (CdDropDownBox.Empty ? null : GetAffinityDvdDrive());

            if (cd == null)
                return; // select default whatever that is

            SR sr = Connection.Resolve<SR>(cd.SR);
            if (sr == null)
                return; // select default whatever that is

            CdDropDownBox.SelectedCD = cd;
            CdDropDownBox.SelectCD();
        }

        public VDI GetAffinityDvdDrive()
        {
            if (Affinity == null || Affinity.Connection == null)
                return null;

            List<SR> dvdSRs = Affinity.Connection.Cache.SRs.Where(sr => sr.content_type == SR.Content_Type_ISO && sr.Physical() && sr.GetStorageHost() == Affinity).ToList();

            if (dvdSRs.Count > 0 && dvdSRs[0].VDIs.Count > 0)
                return Affinity.Connection.Resolve(dvdSRs[0].VDIs[0]);

            return null;
        }

        public InstallMethod SelectedInstallMethod
        {
            get
            {
                if (!ShowInstallationMedia || m_template.DefaultTemplate() && String.IsNullOrEmpty(m_template.InstallMethods()))
                    return InstallMethod.None;
                if (CdRadioButton.Checked)
                    return InstallMethod.CD;

                if (UrlRadioButton.Checked)
                    return InstallMethod.Network;

                return InstallMethod.None;
            }
        }

        public string SelectedPvArgs
        {
            get
            {
                return !IsSelectedTemplateHVM ? PvBootTextBox.Text : string.Empty;
            }
        }

        public VDI SelectedCD
        {
            get
            {
                return ShowInstallationMedia && CdRadioButton.Checked ? CdDropDownBox.SelectedCD : null;
            }
        }

        public string SelectedUrl
        {
            get
            {
                return ShowInstallationMedia && UrlRadioButton.Checked ? UrlTextBox.Text : string.Empty;
            }
        }

        public VM SelectedTemplate { private get; set; }

        public override bool EnableNext()
        {
            return ShowInstallationMedia ? CdRadioButton.Checked || UrlRadioButton.Checked : true;
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "InstallationMedia"; }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                switch (SelectedInstallMethod)
                {
                    case InstallMethod.CD:
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_INSTALLMETHOD, Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_CD));
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CDMEDIAPAGE_INSTALLATIONSOURCE, SelectedCD != null ? SelectedCD.Name() : Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NONE));
                        break;
                    case InstallMethod.Network:
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_INSTALLMETHOD, Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NETWORK));
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_NETWORKMEDIAPAGE_INSTALLATIONURL, SelectedUrl));
                        break;
                }
                if (Helpers.NaplesOrGreater(SelectedTemplate.Connection) && IsSelectedTemplateHVM)
                    sum.Add(new KeyValuePair<string, string>(Messages.BOOT_MODE, SelectedBootMode.StringOf()));
                return sum;
            }
        }

        private void UpdateEnablement()
        {
            CdDropDownBox.Enabled = CdRadioButton.Checked;
            UrlTextBox.Enabled = UrlRadioButton.Checked;

            OnPageUpdated();
        }

        private void PhysicalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UrlRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NewSRWizard wizard = new NewSRWizard(Connection);
            wizard.CheckNFSISORadioButton();

            if (wizard.ShowDialog() == DialogResult.Cancel)
                return;

            // Return if we lost connection
            if (Connection == null || Helpers.GetPoolOfOne(Connection) == null)
            {
                log.Error("Page_InstallationMedia: connection to the server was lost");
                return;
            }

            // There's a chance the cd radio button was disabled because we didnt have any isos to select from, so refresh that bool
            // and the enablement of that button.
            cds = Helpers.CDsExist(Connection);
            CdRadioButton.Enabled = (installed || installCd) && cds;

            // We can get a lot of refresh flickering in the ISO box as all the VDIs are discovered
            // Possibly slightly rude, but were going to have a pretend action here which gives it some breathing space before we look for VDIs
            DelegatedAsyncAction waitAction = new DelegatedAsyncAction(Connection, Messages.SR_REFRESH_ACTION_DESC, Messages.SR_REFRESH_ACTION_DESC, Messages.COMPLETED,
                delegate
                {
                    System.Threading.Thread.Sleep(10000);
                }, true);
            using (var dlg = new ActionProgressDialog(waitAction, System.Windows.Forms.ProgressBarStyle.Marquee))
                dlg.ShowDialog(this);

            // Set the connection on the drop down iso box. This causes a complete refresh rather than a mini one - otherwise we miss out on
            // getting event handlers for the new SR
            CdDropDownBox.connection = Connection;
        }

        private void RegisterBespokeEventsAgainstCdDropDownBox()
        {
            CdDropDownBox.DrawMode = DrawMode.OwnerDrawFixed;
            CdDropDownBox.DrawItem += AddToolTipToCdDropDownBox_DrawItemEvent;
            CdDropDownBox.DropDownClosed += HideToolTipOnCdDropDownBox_DropDownClosedEvent;
        }

        private void HideToolTipOnCdDropDownBox_DropDownClosedEvent(object sender, EventArgs e)
        {
            comboBoxToolTip.Hide(CdDropDownBox);
        }

        private void AddToolTipToCdDropDownBox_DrawItemEvent(object sender, DrawItemEventArgs e)
        {
            string selectedText = "";
            if (e.Index != -1)
                selectedText = CdDropDownBox.GetItemText(CdDropDownBox.Items[e.Index]);

            Font font = (e.Index != -1 && CdDropDownBox.Items[e.Index] is ToStringWrapper<SR>) ? Program.DefaultFontBold : Program.DefaultFont;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected &&
                CdDropDownBox.DroppedDown &&
                TextRenderer.MeasureText(selectedText, font).Width > CdDropDownBox.DropDownWidth)
            {
                comboBoxToolTip.Show(selectedText, CdDropDownBox, e.Bounds.Right, e.Bounds.Bottom);
            }
            else
            {
                HideToolTipOnCdDropDownBox_DropDownClosedEvent(sender, e);
            }
        }

    }
}
