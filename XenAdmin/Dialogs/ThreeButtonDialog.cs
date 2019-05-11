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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


namespace XenAdmin.Dialogs
{
    public partial class ThreeButtonDialog : XenDialogBase
    {
        private string helpName = "DefaultHelpTopic";

        /// <summary>
        /// Gives you a dialog with a single OK button.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="helpName"></param>
        public ThreeButtonDialog(Details properties, string helpName)
            : this(properties, ButtonOK)
        {
            this.helpName = helpName;
            HelpButton = true;
        }

        /// <summary>
        /// Gives you a dialog with the specified buttons.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="helpName"></param>
        /// <param name="buttons">Must be between 1 and 3 buttons</param>
        public ThreeButtonDialog(Details properties, string helpName, params TBDButton[] buttons)
            : this(properties, buttons)
        {
            this.helpName = helpName;
            HelpButton = true;
        }

        /// <summary>
        /// Gives you a dialog with a single OK button.
        /// </summary>
        /// <param name="properties"></param>
        public ThreeButtonDialog(Details properties)
            : this(properties, ButtonOK)
        {
        }

        /// <summary>
        /// Gives you a dialog with the specified buttons.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="buttons">>Must be between 1 and 3 buttons</param>
        public ThreeButtonDialog(Details properties, params TBDButton[] buttons)
        {
            System.Diagnostics.Trace.Assert(buttons.Length > 0 && buttons.Length < 4, "Three button dialog can only have between 1 and 3 buttons.");
            InitializeComponent();

            if (properties.Icon == null)
                pictureBoxIcon.Visible = false;
            else
                pictureBoxIcon.Image = properties.Icon.ToBitmap();

            labelMessage.Text = properties.MainMessage;

            if (properties.WindowTitle != null)
                this.Text = properties.WindowTitle;

            button1.Visible = true;
            button1.Text = buttons[0].label;
            button1.DialogResult = buttons[0].result;
            if (buttons[0].defaultAction == ButtonType.ACCEPT)
            {
                AcceptButton = button1;
                if (buttons.Length == 1)
                    CancelButton = button1;
            }
            else if (buttons[0].defaultAction == ButtonType.CANCEL)
                CancelButton = button1;

            if (buttons[0].selected)
                button1.Select();

            if (buttons.Length > 1)
            {
                button2.Visible = true;
                button2.Text = buttons[1].label;
                button2.DialogResult = buttons[1].result;
                if (buttons[1].defaultAction == ButtonType.ACCEPT)
                    AcceptButton = button2;
                else if (buttons[1].defaultAction == ButtonType.CANCEL)
                    CancelButton = button2;

                if (buttons[1].selected)
                    button2.Select();
            }
            else
            {
                button2.Visible = false;
            }

            if (buttons.Length > 2)
            {
                button3.Visible = true;
                button3.Text = buttons[2].label;
                button3.DialogResult = buttons[2].result;
                if (buttons[2].defaultAction == ButtonType.ACCEPT)
                    AcceptButton = button3;
                else if (buttons[2].defaultAction == ButtonType.CANCEL)
                    CancelButton = button3;

                if (buttons[2].selected)
                    button3.Select();
            }
            else
            {
                button3.Visible = false;
            }
        }

        public bool ShowCheckbox
        {
            get { return checkBoxOption.Visible; }
            set { checkBoxOption.Visible = value; }
        }

        public string CheckboxCaption
        {
            get { return checkBoxOption.Text; }
            set { checkBoxOption.Text = value; }
        }

        public bool IsCheckBoxChecked
        {
            get { return checkBoxOption.Checked; }
            set { checkBoxOption.Checked = value; }
        }

        /// <summary>
        /// The message displayed on the dialog
        /// </summary>
        public String Message
        {
            get { return labelMessage.Text; }
        }

        /// <summary>
        /// A list of buttons on the page
        /// </summary>
        public List<Button> Buttons
        {
            get
            {
                return new List<Button>()
                {
                    button1,
                    button2,
                    button3
                };

            }
        }

        /// <summary>
        /// A list of buttons on the page that have been set visible
        /// </summary>
        public List<Button> VisibleButtons
        {
            get
            {
                List<Button> visibleButtonList = new List<Button>();
                Buttons.ForEach(button => AddButtonIfVisible(visibleButtonList, button));
                return visibleButtonList;
            }
        }

        private void AddButtonIfVisible( List<Button> buttonList, Button button )
        {
            if( button.Visible )
                buttonList.Add( button );
        }

        internal override string HelpName
        {
            get
            {
                return helpName;
            }
        }

        public class TBDButton
        {
            public string label;
            public DialogResult result;
            public ButtonType defaultAction = ButtonType.NONE;
            public bool selected = false;

            /// <summary>
            /// Describes a button for the three button dialog. This constructor infers the dialogs default button from
            /// the result type you give it. 
            /// 
            /// To override this behaviour use another constructor.
            /// </summary>
            /// <param name="label">The label for the button</param>
            /// <param name="result">The result to return on click. Setting result to be OK or Yes results in the dialog choosing this button as the DefaultAcceptButton, 
            /// and No or Cancel sets it as the DefaultCancelButton.</param>
            public TBDButton(string label, DialogResult result)
            {
                this.label = label;
                this.result = result;
                if (result == DialogResult.OK || result == DialogResult.Yes)
                    defaultAction = ButtonType.ACCEPT;
                if (result == DialogResult.No || result == DialogResult.Cancel)
                    defaultAction = ButtonType.CANCEL;
            }

            /// <summary>
            /// This constructor allows you to override how the threebuttondialog interprets the dialogresult of this button.
            /// </summary>
            /// <param name="label">The label for the button</param>
            /// <param name="result">The result to return on click.</param>
            /// <param name="isDefaultButton">The role the button plays in the dialog</param>
            public TBDButton(string label, DialogResult result, ButtonType isDefaultButton)
                : this(label, result)
            {
                defaultAction = isDefaultButton;
            }

            /// <summary>
            /// This constructor allows you to override how the threebuttondialog interprets the dialogresult of this button and specify if the button is selected by default.
            /// </summary>
            /// <param name="label">The label for the button</param>
            /// <param name="result">The result to return on click.</param>
            /// <param name="isDefaultButton">The role the button plays in the dialog</param>
            /// <param name="select"></param>
            public TBDButton(string label, DialogResult result, ButtonType isDefaultButton, bool select)
                : this(label, result, isDefaultButton)
            {
                selected = select;
            }

        }

        /// <summary>
        /// Using Accept results in this button becoming the DefaultAcceptButton, and Cancel sets it as the DefaultCancelButton
        /// </summary>
        public enum ButtonType { NONE, ACCEPT, CANCEL };

        /// <summary>
        /// Describes the main properties of a dialog
        /// </summary>
        public class Details
        {
            public readonly Icon Icon;
            public readonly string WindowTitle;
            public readonly string MainMessage;

            public Details(Icon icon, string mainMessage, string windowTitle = null)
            {
                Icon = icon;
                MainMessage = mainMessage ?? "";
                WindowTitle = string.IsNullOrEmpty(windowTitle) ? Messages.XENCENTER : windowTitle;
            }
        }

        /// <summary>
        /// Retrieves a button with label Messages.YES_BUTTON_CAPTION and result DialogResult.Yes
        /// </summary>
        public static TBDButton ButtonYes
        {
            get
            {
                return new TBDButton(Messages.YES_BUTTON_CAPTION, DialogResult.Yes);
            }
        }

        /// <summary>
        /// Retrieves a button with label Messages.NO_BUTTON_CAPTION and result DialogResult.No
        /// </summary>
        public static TBDButton ButtonNo
        {
            get
            {
                return new TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No);
            }
        }

        /// <summary>
        /// Retrieves a button with label Messages.OK and result DialogResult.OK)
        /// </summary>
        public static TBDButton ButtonOK
        {
            get
            {
                return new TBDButton(Messages.OK, DialogResult.OK);
            }
        }

        /// <summary>
        /// Retrieves a button with label Messages.CANCEL and result DialogResult.Cancel
        /// </summary>
        public static TBDButton ButtonCancel
        {
            get
            {
                return new TBDButton(Messages.CANCEL, DialogResult.Cancel);
            }
        }

        private void ThreeButtonDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!closedFromButton)
                {
                    // User has closed without pressing a button (e.g. the cross)

                    // In the following scenarios we can predict what they mean:

                    if (CancelButton != null)
                    {
                        // There's a cancel button, this most closely maps to the desire of someone telling the window to get lost
                        DialogResult = CancelButton.DialogResult;
                    }
                    else if (VisibleButtons.Find(delegate(Button b) { return b.DialogResult == DialogResult.Cancel; }) != null)
                    {
                        // There's a cancel button, this most closely maps to the desire of someone telling the window to get lost
                        DialogResult = DialogResult.Cancel;
                    }
                    else if (VisibleButtons.Count == 1)
                    {
                        // Single button, they only had one choice anyway. 99% of the time this an OK prompt
                        DialogResult = VisibleButtons[0].DialogResult;
                    }
                    else if (VisibleButtons.Count == 2 && VisibleButtons[0].DialogResult == DialogResult.Yes && VisibleButtons[1].DialogResult == DialogResult.No)
                    {
                        // Another common scenario, a yes/no prompt. Slightly more dubious this one, but they most likely mean no.
                        // The following issues have been considered:
                        // - If we are performing a dangerous/significant/unreversable action then this should be an OK Cancel dialog anyway
                        // - You've got the Yes/No buttons the wrong way round
                        //
                        // ...either way you should go back to UI school :)
                        DialogResult = DialogResult.No;
                    }
                    else
                    {
                        // We can't figure out what they mean, and since people almost always only assume that the dialog only returns the results on
                        // the buttons we are going to block the close

                        // Set a CancelButton if you want to stop this happening
                        e.Cancel = true;
                    }
                }
            }
        }

        private bool closedFromButton = false;
        private void button1_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void labelMessage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
            }
            catch { }  // Best effort
        }
    }

    public class NonModalThreeButtonDialog : ThreeButtonDialog
    {
        public NonModalThreeButtonDialog(Icon icon, string msg, string title, string button1Text, string button2Text)
            : base(new Details(icon, msg, title),
                new TBDButton(button1Text, DialogResult.OK),
                new TBDButton(button2Text, DialogResult.Cancel))
        {
            Buttons[0].Click += button1_Click;
            Buttons[1].Click += button2_Click;
        }

        void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
