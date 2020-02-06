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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Properties;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConfirmationOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConfirmationOptionsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var def = Properties.Settings.Default;

            checkBoxDontConfirmDismissAlerts.Checked = def.DoNotConfirmDismissAlerts;
            checkBoxDontConfirmDismissUpdates.Checked = def.DoNotConfirmDismissUpdates;
            checkBoxDontConfirmDismissEvents.Checked = def.DoNotConfirmDismissEvents;

            checkBoxIgnoreOvfWarnings.Checked = def.IgnoreOvfValidationWarnings;
        }

        public static void Log()
        {
            log.InfoFormat("=== DoNotConfirmDismissAlerts: {0}", Properties.Settings.Default.DoNotConfirmDismissAlerts);
            log.InfoFormat("=== DoNotConfirmDismissUpdates: {0}", Properties.Settings.Default.DoNotConfirmDismissUpdates);
            log.InfoFormat("=== DoNotConfirmDismissEvents: {0}", Properties.Settings.Default.DoNotConfirmDismissEvents);

            log.InfoFormat("=== IgnoreOvfValidationWarnings: {0}", Properties.Settings.Default.IgnoreOvfValidationWarnings);
        }

        #region IOptionsPage Members

        public void Save()
        {
            if (Properties.Settings.Default.DoNotConfirmDismissAlerts != checkBoxDontConfirmDismissAlerts.Checked)
                Properties.Settings.Default.DoNotConfirmDismissAlerts = checkBoxDontConfirmDismissAlerts.Checked;

            if (Properties.Settings.Default.DoNotConfirmDismissUpdates != checkBoxDontConfirmDismissUpdates.Checked)
                Properties.Settings.Default.DoNotConfirmDismissUpdates = checkBoxDontConfirmDismissUpdates.Checked;

            if (Properties.Settings.Default.DoNotConfirmDismissEvents != checkBoxDontConfirmDismissEvents.Checked)
                Properties.Settings.Default.DoNotConfirmDismissEvents = checkBoxDontConfirmDismissEvents.Checked;

            if (Properties.Settings.Default.IgnoreOvfValidationWarnings != checkBoxIgnoreOvfWarnings.Checked)
                Properties.Settings.Default.IgnoreOvfValidationWarnings = checkBoxIgnoreOvfWarnings.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.CONFIRMATIONS;

        public string SubText => Messages.CONFIRMATIONS_DETAIL;

        public Image Image => Resources._075_TickRound_h32bit_16;

        #endregion
    }
}
