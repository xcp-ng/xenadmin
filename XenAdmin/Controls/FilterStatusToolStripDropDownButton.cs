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
using System.ComponentModel;
using System.Windows.Forms;

using XenAdmin.Actions;

namespace XenAdmin.Controls
{
    public class FilterStatusToolStripDropDownButton : ToolStripDropDownButton
    {
        [Browsable(true)]
        public event Action FilterChanged;

        private bool internalUpdating;

        private ToolStripMenuItem toolStripMenuItemComplete;
        private ToolStripMenuItem toolStripMenuItemInProgress;
        private ToolStripMenuItem toolStripMenuItemError;
        private ToolStripMenuItem toolStripMenuItemCancelled;
        private ToolStripMenuItem toolStripMenuItemQueued;
        private ToolStripMenuItem toolStripMenuItemIncomplete;
        private ToolStripMenuItem toolStripMenuItemAll;

        public FilterStatusToolStripDropDownButton()
        {
            toolStripMenuItemQueued = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_QUEUED,
                Checked = true,
                CheckOnClick = true
            };
            toolStripMenuItemComplete = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_COMPLETE,
                Checked = true,
                CheckOnClick = true
            };
            toolStripMenuItemInProgress = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_IN_PROGRESS,
                Checked = true,
                CheckOnClick = true
            };
            toolStripMenuItemError = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_ERROR,
                Checked = true,
                CheckOnClick = true
            };
            toolStripMenuItemCancelled = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_CANCEL,
                Checked = true,
                CheckOnClick = true
            };

            toolStripMenuItemIncomplete = new ToolStripMenuItem
            {
                Text = Messages.STATUS_FILTER_INCOMPLETE,
                Checked = true,
                CheckOnClick = true
            };
            toolStripMenuItemAll = new ToolStripMenuItem
            {
                Text = Messages.FILTER_SHOW_ALL,
                Enabled = false
            };

            DropDownItems.AddRange(new ToolStripItem[]
            {
                toolStripMenuItemQueued,
                toolStripMenuItemComplete,
                toolStripMenuItemInProgress,
                toolStripMenuItemError,
                toolStripMenuItemCancelled,
                toolStripMenuItemIncomplete,
                new ToolStripSeparator(),
                toolStripMenuItemAll
            });

            toolStripMenuItemQueued.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItemComplete.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItemInProgress.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItemError.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItemCancelled.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItemIncomplete.CheckedChanged += Item_CheckedChanged;
        }

        public bool HideByStatus(IStatus iStatus)
        {
            return !(toolStripMenuItemComplete.Checked && iStatus.Succeeded
                || toolStripMenuItemError.Checked && iStatus.IsError
                || toolStripMenuItemInProgress.Checked && iStatus.InProgress
                || toolStripMenuItemCancelled.Checked && iStatus.IsCancelled
                || toolStripMenuItemIncomplete.Checked && iStatus.IsIncomplete
                || toolStripMenuItemQueued.Checked && iStatus.IsQueued);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ImplementsQueued
        {
            get => toolStripMenuItemQueued.Visible;
            set => toolStripMenuItemQueued.Visible = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ImplementsIncomplete
        {
            get => toolStripMenuItemIncomplete.Visible;
            set => toolStripMenuItemIncomplete.Visible = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FilterIsOn =>
            !toolStripMenuItemComplete.Checked
            || !toolStripMenuItemInProgress.Checked
            || !toolStripMenuItemError.Checked
            || !toolStripMenuItemCancelled.Checked
            || !toolStripMenuItemIncomplete.Checked
            || !toolStripMenuItemQueued.Checked;

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnDropDownItemClicked(e);

            if (e.ClickedItem == toolStripMenuItemAll)
            {
                internalUpdating = true;
                toolStripMenuItemComplete.Checked = true;
                toolStripMenuItemInProgress.Checked = true;
                toolStripMenuItemError.Checked = true;
                toolStripMenuItemCancelled.Checked = true;
                toolStripMenuItemIncomplete.Checked = true;
                toolStripMenuItemQueued.Checked = true;
                internalUpdating = false;

                Item_CheckedChanged(null, null);
            }
        }

        private void Item_CheckedChanged(object sender, EventArgs e)
        {
            if (!internalUpdating)
            {
                toolStripMenuItemAll.Enabled = FilterIsOn;

                if (FilterChanged != null)
                    FilterChanged();
            }
        }
    }
}