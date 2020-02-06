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
using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls.CheckableDataGridView
{
    public abstract class CheckableDataGridViewRow : DataGridViewRow
    {
        public virtual IXenObject XenObject { get; private set; }

        public event Action<CheckableDataGridViewRow> CellDataUpdated;

        protected CheckableDataGridViewRow(IXenObject xenObject = null)
        {
            XenObject = xenObject;
        }

        public abstract Queue<object> CellText { get; }

        public bool Checked { get; set; }

        public bool Highlighted { get; set; }

        private bool disabled;
        public bool Disabled
        {
            get { return disabled; }
            set
            {
                if(LockDisabledState)
                    return;
                disabled = value;
            }
        }

        private string disabledReason;
        public string DisabledReason
        {
            get { return disabledReason; }
            set
            {
                if(LockDisabledState)
                    return;
                disabledReason = value;
            }
        }

        public bool LockDisabledState { get; set; }

        public abstract bool WarningRequired { get; }

        public abstract string WarningText { get; }

        /// <summary>
        /// Override this if you cells data is loaded after it is first drawn
        /// </summary>
        public virtual bool CellDataLoaded => true;

        public virtual void BeginCellUpdate(){}

        public bool Equals(CheckableDataGridViewRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return XenObject.Equals(other.XenObject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            CheckableDataGridViewRow other = obj as CheckableDataGridViewRow;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return (XenObject != null ? XenObject.GetHashCode() : 0);
        }

        public static bool operator ==(CheckableDataGridViewRow left, CheckableDataGridViewRow right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CheckableDataGridViewRow left, CheckableDataGridViewRow right)
        {
            return !Equals(left, right);
        }

        protected void TriggerCellTextUpdatedEvent()
        {
            CellDataUpdated?.Invoke(this);
        }
    }
}
