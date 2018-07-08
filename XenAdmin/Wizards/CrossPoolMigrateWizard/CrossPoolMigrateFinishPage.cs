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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public partial class CrossPoolMigrateFinishPage : XenTabPage
    {
        private int selectionCount;
        private WizardMode wizardMode;
        private bool templatesOnly = false;

        public CrossPoolMigrateFinishPage(int selectionCount, WizardMode wizardMode, bool templatesOnly)
		{
            InitializeComponent();
            this.selectionCount = selectionCount;
            this.wizardMode = wizardMode;
            this.templatesOnly = templatesOnly;
		}

		/// <summary>
		/// The pages' label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text
		{
			get { return Messages.FINISH_PAGE_TEXT; }
		}

        public override string HelpID { get { return wizardMode == WizardMode.Copy ? "MigrationSummaryCopyMode" : "MigrationSummary"; } }

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_FINISH_PAGE_TITLE; } }

		public override void PopulatePage()
		{
            if (templatesOnly)
            {
                if (selectionCount > 1)
                    m_labelIntro.Text = Messages.CPM_WIZARD_FINISH_PAGE_INTRO_COPY_TEMPLATE;
                else
                    m_labelIntro.Text = Messages.CPM_WIZARD_FINISH_PAGE_INTRO_COPY_SINGLE_TEMPLATE;
            }
            else
            {
                if (selectionCount > 1)
                    m_labelIntro.Text = wizardMode == WizardMode.Copy ? Messages.CPM_WIZARD_FINISH_PAGE_INTRO_COPY : Messages.CPM_WIZARD_FINISH_PAGE_INTRO;
                else
                    m_labelIntro.Text = wizardMode == WizardMode.Copy ? Messages.CPM_WIZARD_FINISH_PAGE_INTRO_COPY_SINGLE : Messages.CPM_WIZARD_FINISH_PAGE_INTRO_SINGLE;
            }

            if (SummaryRetreiver == null)
				return;

			var entries = SummaryRetreiver.Invoke();
            m_dataGridView.Rows.Clear();
		    panelErrorsFound.Visible = false;

			foreach (var pair in entries)
			{
				var row = new DataGridViewRow();

                DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell() {Value = pair.Value};
                if (pair.Errors)
                {
                    valueCell.Style.Font = new Font(m_dataGridView.Font, FontStyle.Bold);
                    panelErrorsFound.Visible = true;
                }

                row.Cells.AddRange(new DataGridViewTextBoxCell {Value = pair.Key}, valueCell);
				m_dataGridView.Rows.Add(row);
			}

            HelpersGUI.ResizeGridViewColumnToAllCells(Column2);//set properly the width of the last column
		}

        public Func<IEnumerable<SummaryDetails>> SummaryRetreiver { private get; set; }

        public override bool EnableNext()
        {
            return !panelErrorsFound.Visible;
        }
	}
}
