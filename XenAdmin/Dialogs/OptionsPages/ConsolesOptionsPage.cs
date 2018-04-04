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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Properties;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConsolesOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string ConsoleTabSettingsHeader = "Console Tab Settings -";

        public ConsolesOptionsPage()
        {
            InitializeComponent();

            build();
        }

        private void build()
        {
            // Fullscreen shortcut keys
            buildKeyCodeListBox();

            // Dock-undock shortcut keys
            buildDockKeyCodeComboBox();

            // Uncapture keyboard and mouse shortcut keys
            buildUncaptureKeyCodeComboBox();

            // Windows Remote Desktop console
            WindowsKeyCheckBox.Checked = Properties.Settings.Default.WindowsShortcuts;
            SoundCheckBox.Checked = Properties.Settings.Default.ReceiveSoundFromRDP;
            AutoSwitchCheckBox.Checked = Properties.Settings.Default.AutoSwitchToRDP;
            ClipboardCheckBox.Checked = Properties.Settings.Default.ClipboardAndPrinterRedirection;
            ConnectToServerConsoleCheckBox.Checked = Properties.Settings.Default.ConnectToServerConsole;

            // Console scaling
            PreserveUndockedScaleCheckBox.Checked = Properties.Settings.Default.PreserveScaleWhenUndocked;
            PreserveVNCConsoleScalingCheckBox.Checked = Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC;
            checkBoxDisableRDPPolling.Visible = !HiddenFeatures.RDPPollingHidden;
            checkBoxDisableRDPPolling.Checked = Properties.Settings.Default.EnableRDPPolling;
        }

        private void buildKeyCodeListBox()
        {
            KeyComboListBox.Items.Clear();
            KeyComboListBox.Items.Add("Ctrl+Alt");
            KeyComboListBox.Items.Add("Ctrl+Alt+F");
            KeyComboListBox.Items.Add("F12");
            KeyComboListBox.Items.Add("Ctrl+Enter");
            selectKeyCombo();
        }

        private void buildDockKeyCodeComboBox()
        {
            DockKeyComboBox.Items.Clear();
            DockKeyComboBox.Items.Add(Messages.NONE);
            DockKeyComboBox.Items.Add("Alt+Shift+U");
            DockKeyComboBox.Items.Add("F11");
            selectDockKeyCombo();
        }

        private void selectDockKeyCombo()
        {
            DockKeyComboBox.SelectedIndex = Properties.Settings.Default.DockShortcutKey;
        }

        private void selectKeyCombo()
        {
            KeyComboListBox.SelectedIndex = Properties.Settings.Default.FullScreenShortcutKey;
        }

        private void buildUncaptureKeyCodeComboBox()
        {
            UncaptureKeyComboBox.Items.Clear();
            UncaptureKeyComboBox.Items.Add(Messages.RIGHT_CTRL);
            UncaptureKeyComboBox.Items.Add(Messages.LEFT_ALT);
            selectUncaptureKeyCombo();
        }

        private void selectUncaptureKeyCombo()
        {
            UncaptureKeyComboBox.SelectedIndex = Properties.Settings.Default.UncaptureShortcutKey;
        }

        public static void Log()
        {
            log.Info(ConsoleTabSettingsHeader);

            // Fullscreen shortcut keys
            log.Info("=== FullScreenShortcutKey: " + Properties.Settings.Default.FullScreenShortcutKey.ToString());
            // Dock-undock shortcut keys
            log.Info("=== DockShortcutKey: " + Properties.Settings.Default.DockShortcutKey.ToString());
            // Uncapture keyboard and mouse shortcut keys
            log.Info("=== UncaptureShortcutKey: " + Properties.Settings.Default.UncaptureShortcutKey.ToString());

            // Windows Remote Desktop console
            log.Info("=== ClipboardAndPrinterRedirection: " + Properties.Settings.Default.ClipboardAndPrinterRedirection.ToString());
            log.Info("=== WindowsShortcuts: " + Properties.Settings.Default.WindowsShortcuts.ToString());
            log.Info("=== ReceiveSoundFromRDP: " + Properties.Settings.Default.ReceiveSoundFromRDP.ToString());
            log.Info("=== AutoSwitchToRDP: " + Properties.Settings.Default.AutoSwitchToRDP.ToString());
            log.Info("=== ConnectToServerConsole: " + Properties.Settings.Default.ConnectToServerConsole.ToString());

            // Console scaling
            log.Info("=== PreserveScaleWhenUndocked: " + Properties.Settings.Default.PreserveScaleWhenUndocked.ToString());
            log.Info("=== PreserveScaleWhenSwitchBackToVNC: " + Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            // Fullscreen shortcut keys
            if (KeyComboListBox.SelectedIndex != Properties.Settings.Default.FullScreenShortcutKey)
                Properties.Settings.Default.FullScreenShortcutKey = KeyComboListBox.SelectedIndex;
            // Dock-undock shortcut keys
            if (DockKeyComboBox.SelectedIndex != Properties.Settings.Default.DockShortcutKey)
                Properties.Settings.Default.DockShortcutKey = DockKeyComboBox.SelectedIndex;
            // Uncapture keyboard and mouse shortcut keys
            if (UncaptureKeyComboBox.SelectedIndex != Properties.Settings.Default.UncaptureShortcutKey)
                Properties.Settings.Default.UncaptureShortcutKey = UncaptureKeyComboBox.SelectedIndex;

            // Windows Remote Desktop console
            if (WindowsKeyCheckBox.Checked != Properties.Settings.Default.WindowsShortcuts)
                Properties.Settings.Default.WindowsShortcuts = WindowsKeyCheckBox.Checked;
            if (SoundCheckBox.Checked != Properties.Settings.Default.ReceiveSoundFromRDP)
                Properties.Settings.Default.ReceiveSoundFromRDP = SoundCheckBox.Checked;
            if (AutoSwitchCheckBox.Checked != Properties.Settings.Default.AutoSwitchToRDP)
                Properties.Settings.Default.AutoSwitchToRDP = AutoSwitchCheckBox.Checked;
            if (ClipboardCheckBox.Checked != Properties.Settings.Default.ClipboardAndPrinterRedirection)
                Properties.Settings.Default.ClipboardAndPrinterRedirection = ClipboardCheckBox.Checked;
            if (ConnectToServerConsoleCheckBox.Checked != Properties.Settings.Default.ConnectToServerConsole)
                Properties.Settings.Default.ConnectToServerConsole = ConnectToServerConsoleCheckBox.Checked;

            // Console scaling
            if (PreserveUndockedScaleCheckBox.Checked != Properties.Settings.Default.PreserveScaleWhenUndocked)
                Properties.Settings.Default.PreserveScaleWhenUndocked = PreserveUndockedScaleCheckBox.Checked;
            if (PreserveVNCConsoleScalingCheckBox.Checked != Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC)
                Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC = PreserveVNCConsoleScalingCheckBox.Checked;
            if (checkBoxDisableRDPPolling.Checked != Properties.Settings.Default.EnableRDPPolling)
                Properties.Settings.Default.EnableRDPPolling = checkBoxDisableRDPPolling.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.CONSOLE; }
        }

        public string SubText
        {
            get { return Messages.CONSOLE_DESC; }
        }

        public Image Image
        {
            get { return Resources.console_16; }
        }

        #endregion

    }
}
