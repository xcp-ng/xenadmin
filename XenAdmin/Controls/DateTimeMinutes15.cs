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
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public class DateTimeMinutes15 : DateTimePicker
    {
        public bool AutoCorrecting;

        public DateTimeMinutes15()
        {
            ValueChanged += DateTimeMinutes15_ValueChanged;
            Value = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        private void DateTimeMinutes15_ValueChanged(object sender, EventArgs e)
        {
            if (Value.Minute == 1)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 15, 0);
                return;
            }

            if (Value.Minute == 14)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 0, 0);
                return;
            }

            if (Value.Minute == 16)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 30, 0);
                return;
            }

            if (Value.Minute == 29)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 15, 0);
                return;
            }

            if (Value.Minute == 31)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 45, 0);
                return;
            }

            if (Value.Minute == 44)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 30, 0);
                return;
            }


            if (Value.Minute == 46)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 0, 0);
                Value = Value.AddHours(1);
                return;
            }


            if (Value.Minute == 59)
            {
                Value = new DateTime(1970, 1, 1, Value.Hour, 45, 0);
                Value = Value.AddHours(-1);
                return;
            }

            if (Value.Minute > 0 && Value.Minute < 15)
            {
                Value = Value.AddMinutes(15 - Value.Minute);
                return;
            }
            if (Value.Minute > 15 && Value.Minute < 30)
            {
                Value = Value.AddMinutes(30 - Value.Minute);
                return;
            }
            if (Value.Minute > 30 && Value.Minute < 45)
            {
                Value = Value.AddMinutes(45 - Value.Minute);
                return;
            }
            if (Value.Minute > 45 && Value.Minute < 60)
            {
                Value = Value.AddMinutes(60 - Value.Minute);
                return;
            }
        }

        public new DateTime Value
        {
            get { return base.Value; }
            set
            {
                try
                {
                    AutoCorrecting = true;
                    base.Value = value;
                }
                finally
                {
                    AutoCorrecting = false;
                }
            }
        }
    }
}
