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
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.CustomFields;
using XenAdmin.Dialogs;
using XenAdmin.XenSearch;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class CustomFieldsDisplayPage : UserControl, IEditPage
    {
        private readonly Dictionary<CustomFieldDefinition, KeyValuePair<Label, Control>> controls = new Dictionary<CustomFieldDefinition, KeyValuePair<Label, Control>>();

        public CustomFieldsDisplayPage()
        {
            InitializeComponent();

            Text = Messages.CUSTOM_FIELDS;
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            List<CustomField> customFields = new List<CustomField>();

            foreach (KeyValuePair<CustomFieldDefinition, KeyValuePair<Label, Control>> kvp in controls)
            {
                object currentValue = CustomFieldsManager.GetCustomFieldValue(xenObject, kvp.Key);
                object newValue = GetValue(kvp.Key, kvp.Value.Value);

                if (currentValue == null && newValue == null)
                    continue;

                customFields.Add(new CustomField(kvp.Key, newValue));
            }

            return new SaveCustomFieldsAction(xenObject, customFields, true);
        }

        private IXenObject xenObject;
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            CustomFieldsManager.CustomFieldsChanged -= CustomFields_CustomFieldsChanged;
            xenObject = clone;

            if (xenObject != null)
            {
                CustomFieldsManager.CustomFieldsChanged += CustomFields_CustomFieldsChanged;
                Rebuild(true);
            }
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                foreach (KeyValuePair<CustomFieldDefinition, KeyValuePair<Label, Control>> kvp in controls)
                {
                    Object currentValue = CustomFieldsManager.GetCustomFieldValue(xenObject, kvp.Key);
                    Object newValue = GetValue(kvp.Key, kvp.Value.Value);

                    if (currentValue == null && newValue == null)
                        continue;

                    if (currentValue == null || newValue == null)
                        return true;

                    if (newValue.Equals(currentValue))
                        continue;

                    return true;
                }

                return false;
            }
        }

        #endregion

        #region IVerticalTab Members

        public String SubText
        {
            get
            {
                List<String> fields = new List<String>();

                foreach (KeyValuePair<CustomFieldDefinition, KeyValuePair<Label, Control>> kvp in controls)
                {
                    Object newValue = GetValue(kvp.Key, kvp.Value.Value);

                    if (newValue == null || newValue.ToString() == String.Empty)
                        continue;

                    fields.Add(kvp.Key.Name + Messages.GENERAL_PAGE_KVP_SEPARATOR + newValue);
                }

                if (fields.Count == 0)
                    return Messages.NONE;

                return String.Join(Messages.LIST_SEPARATOR, fields.ToArray());
            }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_Fields_h32bit_16;
            }
        }

        #endregion

        private void CustomFields_CustomFieldsChanged()
        {
            Rebuild(false);
        }

        private Object GetValue(CustomFieldDefinition definition, Control control)
        {
            switch (definition.Type)
            {
                case CustomFieldDefinition.Types.Date:
                    {
                        DateTimePicker dateControl = (DateTimePicker)control;
                        if (!dateControl.Checked)
                            return null;
                        DateTimePicker timeControl = (DateTimePicker)dateControl.Tag;
                        DateTime date = dateControl.Value;
                        DateTime time = timeControl.Value;

                        return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                    }

                case CustomFieldDefinition.Types.String:
                    TextBox textBox = control as TextBox;
                    if (textBox == null)
                        return null;

                    string text = textBox.Text;
                    return (text == "" ? null : text);

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void SetValue(CustomFieldDefinition definition, Control control, Object value)
        {
            switch (definition.Type)
            {
                case CustomFieldDefinition.Types.Date:
                    {
                        DateTimePicker dateControl = (DateTimePicker)control;
                        DateTimePicker timeControl = (DateTimePicker)dateControl.Tag;

                        if (value != null)
                        {
                            dateControl.Checked = true;
                            dateControl.Value = (DateTime)value;
                            timeControl.Value = (DateTime)value;
                        }
                        else
                            dateControl.Checked = false;
                    }
                    break;

                case CustomFieldDefinition.Types.String:
                    TextBox textBox = control as TextBox;
                    if (textBox == null)
                        return;

                    textBox.Text = (String)value;
                    break;
            }
        }

        private void Rebuild(bool revertValues)
        {
            CustomFieldDefinition[] customFieldDefinitions = CustomFieldsManager.GetCustomFields(xenObject.Connection).ToArray();

            tableLayoutPanel.SuspendLayout();

            // Add new custom fields
            foreach (CustomFieldDefinition customFieldDefinition in customFieldDefinitions)
            {
                Object value = CustomFieldsManager.GetCustomFieldValue(xenObject, customFieldDefinition);

                if (!controls.ContainsKey(customFieldDefinition))
                {
                    // Create the display label
                    Label lblKey = new Label();
                    lblKey.Text = customFieldDefinition.Name.EscapeAmpersands();
                    lblKey.Margin = new Padding(3, 7, 3, 3);
                    lblKey.Font = Program.DefaultFont;
                    lblKey.Width = (int)tableLayoutPanel.ColumnStyles[0].Width;
                    lblKey.AutoEllipsis = true;
                    lblKey.AutoSize = false;

                    tableLayoutPanel.Controls.Add(lblKey);

                    // Create value field
                    Control control;

                    switch (customFieldDefinition.Type)
                    {
                        case CustomFieldDefinition.Types.String:
                            TextBox textBox = new TextBox();
                            textBox.Text = (String)value;

                            tableLayoutPanel.Controls.Add(textBox);
                            tableLayoutPanel.SetColumnSpan(textBox, 2);
                            textBox.Dock = DockStyle.Fill;
                            control = textBox;
                            break;

                        case CustomFieldDefinition.Types.Date:
                            DateTimePicker date = new DateTimePicker();
                            date.MinDate = DateTime.MinValue;
                            date.MaxDate = DateTime.MaxValue;
                            date.Dock = DockStyle.Fill;
                            date.MinimumSize = new Size(0, 24);
                            date.ShowCheckBox = true;
                            date.Format = DateTimePickerFormat.Long;
                            if (value != null)
                            {
                                date.Value = (DateTime)value;
                                date.Checked = true;
                            }
                            else
                                date.Checked = false;
                            tableLayoutPanel.Controls.Add(date);

                            DateTimePicker time = new DateTimePicker();
                            time.MinDate = DateTime.MinValue;
                            time.MaxDate = DateTime.MaxValue;
                            time.Dock = DockStyle.Fill;
                            time.MinimumSize = new Size(0, 24);
                            time.Format = DateTimePickerFormat.Time;
                            time.ShowUpDown = true;
                            if (value != null)
                            {
                                time.Value = (DateTime)value;
                                time.Enabled = true;
                            }
                            else
                                time.Enabled = false;
                            tableLayoutPanel.Controls.Add(time);
                            // Tag so we can remove this control later
                            date.Tag = time;
                            date.ValueChanged += delegate(Object sender, EventArgs e)
                            {
                                time.Enabled = date.Checked;
                            };

                            control = date;
                            break;

                        default:
                            throw new InvalidEnumArgumentException();
                    }

                    controls[customFieldDefinition] = new KeyValuePair<Label, Control>(lblKey, control);
                }
                else if (revertValues)
                {
                    KeyValuePair<Label, Control> kvp = controls[customFieldDefinition];

                    SetValue(customFieldDefinition, kvp.Value, value);
                }
            }

            // Remove old ones
            CustomFieldDefinition[] definitions = new CustomFieldDefinition[controls.Keys.Count];
            controls.Keys.CopyTo(definitions, 0);

            foreach (CustomFieldDefinition definition in definitions)
            {
                if (Array.IndexOf<CustomFieldDefinition>(customFieldDefinitions, definition) > -1)
                    continue;

                KeyValuePair<Label, Control> kvp = controls[definition];

                tableLayoutPanel.Controls.Remove(kvp.Value);
                tableLayoutPanel.Controls.Remove(kvp.Key);

                DateTimePicker timeControl = kvp.Value.Tag as DateTimePicker;
                if (timeControl != null)
                {
                    tableLayoutPanel.Controls.Remove(timeControl);
                }

                controls.Remove(definition);

                kvp.Key.Dispose();
                kvp.Value.Dispose();
            }

            tableLayoutPanel.ResumeLayout();
        }

        private void buttonEditCustomFields_Click(object sender, EventArgs e)
        {
            new CustomFieldsDialog(xenObject.Connection).ShowDialog(this);
        }
    }
}
