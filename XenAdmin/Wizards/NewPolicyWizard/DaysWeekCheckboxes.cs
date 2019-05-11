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
using System.Linq;
using System.Windows.Forms;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class DaysWeekCheckboxes : UserControl
    {
        public event EventHandler CheckBoxChanged;

        public DaysWeekCheckboxes()
        {
            InitializeComponent();
        }

        public bool AnySelected()
        {
            return checkBoxSunday.Checked ||
                   checkBoxMonday.Checked ||
                   checkBoxTuesday.Checked ||
                   checkBoxWednesday.Checked ||
                   checkBoxThursday.Checked ||
                   checkBoxFriday.Checked ||
                   checkBoxSaturday.Checked;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DayOfWeek[] SelectedDays
        {
            get
            {
                var days = new List<DayOfWeek>();

                if (checkBoxSunday.Checked)
                    days.Add(DayOfWeek.Sunday);
                if (checkBoxMonday.Checked)
                    days.Add(DayOfWeek.Monday);
                if (checkBoxTuesday.Checked)
                    days.Add(DayOfWeek.Tuesday);
                if (checkBoxWednesday.Checked)
                    days.Add(DayOfWeek.Wednesday);
                if (checkBoxThursday.Checked)
                    days.Add(DayOfWeek.Thursday);
                if (checkBoxFriday.Checked)
                    days.Add(DayOfWeek.Friday);
                if (checkBoxSaturday.Checked)
                    days.Add(DayOfWeek.Saturday);

                return days.ToArray();
            }
            set
            {
                if (value == null || value.Length <= 0)
                    return;

                checkBoxSunday.Checked = value.Contains(DayOfWeek.Sunday);
                checkBoxMonday.Checked = value.Contains(DayOfWeek.Monday);
                checkBoxTuesday.Checked = value.Contains(DayOfWeek.Tuesday);
                checkBoxWednesday.Checked = value.Contains(DayOfWeek.Wednesday);
                checkBoxThursday.Checked = value.Contains(DayOfWeek.Thursday);
                checkBoxFriday.Checked = value.Contains(DayOfWeek.Friday);
                checkBoxSaturday.Checked = value.Contains(DayOfWeek.Saturday);
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxChanged != null)
                CheckBoxChanged(sender, e);
        }
    }
}
