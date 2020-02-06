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
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Help;
using XenAdmin.Network;

namespace XenAdmin.Wizards
{
    public partial class XenWizardBase : Form, IFormWithHelp
    {
        private IXenConnection connection; // connection to use

        protected IXenConnection xenConnection
        {
            get { return connection; }
            set
            {
                connection = value;
                foreach (XenTabPage p in wizardProgress.Steps)
                {
                    p.Connection = connection;
                }
            }
        }

        protected XenWizardBase()
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;
        }

        protected XenWizardBase(IXenConnection connection)
            : this()
        {
            this.connection = connection;
        }

        protected DeprecationBanner Banner
        {
            get { return deprecationBanner; }
        }

        /// <summary>
        /// Add a given page to the end of the wizard's page collection
        /// </summary>
        /// <param name="page">The page to add</param>
        protected void AddPage(XenTabPage page)
        {
            AddPage(page, -1);
        }

        /// <summary>
        /// Insert a page into the wizard's page collection at the specified index
        /// </summary>
        /// <param name="page">The page to add</param>
        /// <param name="index">The index at which the page will be inserted</param>
        protected void AddPage(XenTabPage page, int index)
        {
            xenTabControlBody.Controls.Add(page);
            page.Dock = DockStyle.Fill;

            page.WizardContentUpdater = UpdateWizardContent;
            page.NextPagePrecheck = RunNextPagePrecheck;
            page.Connection = connection;
            page.StatusChanged += page_StatusChanged;
            if (index == -1)
                wizardProgress.Steps.Add(page);
            else
                wizardProgress.Steps.Insert(index, page);
        }

        /// <summary>
        /// Add the given pages to the wizard one position after the supplied page
        /// </summary>
        /// <param name="existingPage">The existing page that the new page will be placed after</param>
        /// <param name="pages">The pages to add</param>
        protected void AddAfterPage(XenTabPage existingPage, params XenTabPage[] pages)
        {
            int index = wizardProgress.Steps.IndexOf(existingPage);

            for (int i = 0; i < pages.Length; i++)
            {
                var page = pages[i];
                AddPage(page, index + 1 + i);
            }
        }
        
        protected void AddPages(params XenTabPage[] pages)
		{
			foreach (var page in pages)
				AddPage(page);
		}

        protected void RemovePage(XenTabPage page)
        {
            xenTabControlBody.Controls.Remove(page);

            page.StatusChanged -= page_StatusChanged;
            page.WizardContentUpdater = null;
            page.NextPagePrecheck = null;
            wizardProgress.Steps.Remove(page);
        }

		protected void RemovePages(params XenTabPage[] pages)
		{
			foreach (var page in pages)
				RemovePage(page);
		}

        /// <summary>
        /// Removes pages at a certain index
        /// </summary>
        protected void RemovePageAt(int index)
        {
            if (wizardProgress.Steps.Count > index)
                wizardProgress.Steps.RemoveAt(index);
        }

        /// <summary>
        /// Removes pages from a certain index and above
        /// </summary>
        protected void RemovePagesFrom(int startIndex)
        {
            if (wizardProgress.Steps.Count > startIndex)
                wizardProgress.Steps.RemoveRange(startIndex, wizardProgress.Steps.Count - startIndex);
        }

        private void WizardProgress_EnteringStep(object sender, WizardProgressEventArgs e)
        {
            xenTabControlBody.SelectedTab = wizardProgress.CurrentStepTabPage;

            wizardProgress.CurrentStepTabPage.PageLoaded(e.IsForwardsTransition ? PageLoadedDirection.Forward : PageLoadedDirection.Back);
            wizardProgress.CurrentStepTabPage.SelectDefaultControl();
            UpdateWizard();

            if (wizardProgress.IsLastStep)
            {
                // Ensure the finish button always has focus when last page is showing.
                if (buttonNext.CanFocus)
                    buttonNext.Focus();
            }
        }

        private void WizardProgress_LeavingStep(object sender, WizardProgressEventArgs e)
        {
            wizardProgress.CurrentStepTabPage.PageLeave(e.IsForwardsTransition ? PageLoadedDirection.Forward : PageLoadedDirection.Back, ref e.Cancelled);
            UpdateWizard();
        }

        #region Button event handlers

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (wizardProgress.IsLastStep)
            {
                var args = new WizardProgressEventArgs(true);
                WizardProgress_LeavingStep(CurrentStepTabPage, args);
                if (args.Cancelled)
                    return;

                wizardFinished = true;
                FinishWizard();
            }
            else
            {
                NextStep();

                // update main text from selected tab in case it was changed between substeps
                SetTitle();
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            wizardProgress.PreviousStep();

            // update main text from selected tab in case it was changed between substeps
            SetTitle();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Updates the next wizard pages with content provided by the last visited page
        /// </summary>
        /// <param name="senderPage">The last visited page</param>
        protected virtual void UpdateWizardContent(XenTabPage senderPage)
        {

        }

        protected virtual bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            return true;
        }

        protected void NotifyNextPagesOfChange(params XenTabPage[] pages)
        {
            foreach (var p in pages)
                p.IsFirstLoad = true;
        }

        protected virtual void OnCancel(ref bool cancel)
        {
            wizardProgress.CurrentStepTabPage.PageCancelled(ref cancel);
            DialogResult = DialogResult.Cancel;
        }

        bool wizardFinished = false;

        protected virtual void FinishWizard()
        {
            DialogResult = DialogResult.OK;
            Close();
            Dispose();
        }

        protected void FinishCanceled()
        {
            wizardFinished = false;
            WizardProgress_EnteringStep(null, new WizardProgressEventArgs(false));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Owner != null)
                Owner.Focus();
        }

        private void XenWizardBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (wizardFinished)
                return;

            bool cancel = false;
            OnCancel(ref cancel);
            e.Cancel = cancel;
        }

        /// <summary>
        /// The help ID as sent to HelpManager when the user requests help on a given wizard page.
        /// The base implementation returns [typeName]_[CurrentStepTabPage.HelpID]Pane.
        /// Override this function or XenTabPage.HelpID if necessary (for example, some wizards have
        /// a single help ID for the whole wizard).
        /// </summary>
        protected virtual string WizardPaneHelpID()
        {
            return FormatHelpId(wizardProgress.CurrentStepTabPage.HelpID);
        }

        protected string FormatHelpId(string id)
        {
            return string.Format("{0}_{1}Pane", GetType().Name, id);
        }

        public bool HasHelp()
        {
            return HelpManager.TryGetTopicId(WizardPaneHelpID(), out _);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            Help.HelpManager.Launch(WizardPaneHelpID());
        }

        private void XenWizardBase_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.HelpManager.Launch(WizardPaneHelpID());
        }

        private void XenWizardBase_Load(object sender, EventArgs e)
        {
            if (Owner == null) 
                Owner = Program.MainWindow;
            CenterToParent();
            FormFontFixer.Fix(this);
            if (wizardProgress.Steps.Count == 0)
                return;

            xenTabControlBody.SelectedTab = wizardProgress.Steps[0];
            wizardProgress.CurrentStepTabPage.PageLoaded(PageLoadedDirection.Forward);
            UpdateWizard();
        }

        private void XenWizardBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            { 
                if (buttonNext.CanSelect)
                    buttonNext.Select();
                buttonNext.PerformClick();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XenTabPage CurrentStepTabPage
        {
            get
            {
                return wizardProgress.CurrentStepTabPage;
            }
        }

        public void NextStep()
        {
            wizardProgress.NextStep();
        }

        public void PreviousStep()
        {
            wizardProgress.PreviousStep();
        }

        public bool IsFirstPage()
        {
            return wizardProgress.IsFirstStep;
        }

        protected void RefreshProgress()
        {
            wizardProgress.Refresh();
        }

        private void SetTitle()
        {
            labelWizard.Text = wizardProgress.CurrentStepTabPage.PageTitle;
        }

        protected void UpdateWizard()
        {
            buttonPrevious.Enabled = !wizardProgress.IsFirstStep && wizardProgress.CurrentStepTabPage.EnablePrevious();

            buttonNext.Enabled = wizardProgress.IsLastStep
                                     ? !wizardFinished && wizardProgress.CurrentStepTabPage.EnableNext()
                                     : wizardProgress.CurrentStepTabPage.EnableNext();

            buttonCancel.Enabled = wizardProgress.CurrentStepTabPage.EnableCancel();

            SetTitle();
            buttonNext.Text = wizardProgress.CurrentStepTabPage.NextText(wizardProgress.IsLastStep);
        }

        private void page_StatusChanged(XenTabPage sender)
        {
            UpdateWizard();
        }

        internal void DisablePage(XenTabPage wizardTabPage, bool disable)
        {
            foreach (XenTabPage p in wizardProgress.Steps)
                if (p == wizardTabPage)
                {
                    p.DisableStep = disable;
                    return;
                }
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> GetSummary()
        {
            var entries = new List<KeyValuePair<string, string>>();

            foreach (XenTabPage page in wizardProgress.Steps)
                entries.AddRange(page.PageSummary);

            return entries;
        }

        protected virtual void ShowInformationMessage(string message)
        {
            labelGeneralInformationMessage.Text = message;
            panelGeneralInformationMessage.Visible = true;
        }

        protected virtual void HideInformationMessage()
        {
            panelGeneralInformationMessage.Visible = false;
        }
    }
}
