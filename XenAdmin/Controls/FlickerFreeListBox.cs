/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Runtime.InteropServices;
using XenCenterLib;

namespace XenAdmin.Controls
{
    public class FlickerFreeListBox : ListBox
    {
        /// <summary>
        /// These flicker free list boxes must be owner drawn.
        /// </summary>
        public FlickerFreeListBox()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.IntegralHeight = false;
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        /// <summary>
        /// The last seen value of the horizontal scrollbar position (offset in pixels from the lhs).
        /// </summary>
        private int _scrollLeft = 0;
        /// <summary>
        /// The handle of the horizontal scrollbar of this ListBox
        /// </summary>
        private IntPtr _hScrollbarHandle = IntPtr.Zero;

        public Func<Message, bool> CancelWndProc = msg => false;

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == Win32.WM_HSCROLL)
            {
                // Store the handle for use in Invalidate()
                _hScrollbarHandle = msg.HWnd;
                // Collect info about the position of the horizontal scrollbar
                Win32.ScrollInfo si = new Win32.ScrollInfo();
                si.fMask = (int)Win32.ScrollInfoMask.SIF_ALL;
                si.cbSize = (uint)Marshal.SizeOf(si);

                if (Type.GetType("Mono.Runtime") == null)
                {
                    Win32.GetScrollInfo(msg.HWnd, 0, ref si);
                }
                else
                {
                    //
                    // TODO: Exception under Mono
                    //

                    // System.DllNotFoundException: user32.dll assembly:<unknown assembly> type:<unknown type> member:(null)
                    // at (wrapper managed-to-native) XenCenterLib.Win32.GetScrollInfo(intptr,int,XenCenterLib.Win32/ScrollInfo&)
                    // at XenAdmin.Controls.FlickerFreeListBox.WndProc (System.Windows.Forms.Message& msg)
                }

                if ((msg.WParam.ToInt32() & 0xFF) == Win32.SB_THUMBTRACK)
                {
                    // If the user is in the middle of dragging the scrollbar, we're interested in
                    // the 'track' position
                    _scrollLeft = si.nTrackPos;
                }
                else
                {
                    // Otherwise just the regular scrollbar position
                    _scrollLeft = si.nPos;
                }
                // Force repaint
                base.Invalidate();
            }

            if (!CancelWndProc(msg))
                base.WndProc(ref msg);
        }

        public new void Invalidate()
        {
            if (_hScrollbarHandle != IntPtr.Zero)
            {
                // If we've been invalidated, update our drawing code to the new horizontal scrollbar
                // position (which is sometimes spontaneously reset to zero and can't be programmatically
                // set to its former value) - see CA-11405.
                Win32.ScrollInfo si = new Win32.ScrollInfo();
                si.fMask = (int)Win32.ScrollInfoMask.SIF_ALL;
                si.cbSize = (uint)Marshal.SizeOf(si);
                Win32.GetScrollInfo(_hScrollbarHandle, 0, ref si);
                _scrollLeft = si.nTrackPos; // often zero
            }
            base.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Invalidate();
            base.OnMouseWheel(e);
        }

        private ContextMenuStrip _contextMenuStrip;

        public override ContextMenuStrip ContextMenuStrip
        {
            get => _contextMenuStrip;
            set => _contextMenuStrip = value;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Apps && _contextMenuStrip != null)
            {
                if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
                    return;

                Rectangle r = GetItemRectangle(SelectedIndex);
                Point p = new Point(r.X + 1, r.Bottom + 1);
                ContextMenuStrip.Show(this, p);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right && _contextMenuStrip != null)
            {
                int i = IndexFromPoint(e.Location);
                if (i == ListBox.NoMatches)
                    return;

                SelectedIndex = i;

                ContextMenuStrip.Show(this, e.Location);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if(Enabled)
            {
                using (SolidBrush backBrush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(backBrush, g.ClipBounds);
                }
            }
            else
            {
                g.FillRectangle(SystemBrushes.Control, g.ClipBounds);
            }

            int top = 0;

            for (int i = 0; i < TopIndex; i++)
            {
                if (i < Items.Count)
                    top += base.GetItemHeight(i);
            }

            Rectangle t = new Rectangle(-_scrollLeft, -top, ClientSize.Width, ItemHeight);

            for (int i = 0; i < this.Items.Count; i++)
            {
                int itemHeight = GetItemHeight(i);
                Rectangle bounds = new Rectangle(t.Left, t.Top, t.Width, itemHeight);

                // if selected
                if (SelectedIndices.Contains(i))
                {
                    // Enabled & Focused = blue
                    // Enabled & !Focused = control
                    // !Enabled = dark grey
 
                    if(!Enabled)
                        OnDrawItem(new DrawItemEventArgs(g, Font, bounds, i,
                            DrawItemState.Disabled, SystemColors.HighlightText, 
                            SystemColors.ControlDark));
                    else if (!Focused)
                        OnDrawItem(new DrawItemEventArgs(g, Font, bounds, i, 
                            DrawItemState.Inactive, SystemColors.ControlText, 
                            SystemColors.Control));
                    else
                        OnDrawItem(new DrawItemEventArgs(g, Font, bounds, i, 
                            DrawItemState.Selected, SystemColors.HighlightText, 
                            SystemColors.Highlight));
                }
                else
                {
                    // If not selected, then we just care if the control is enabled

                    OnDrawItem(new DrawItemEventArgs(g, Font, bounds, i, 
                        DrawItemState.Default, SystemColors.ControlText, 
                        Enabled ? BackColor : SystemColors.Control));
                }

                t.Offset(0, itemHeight);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Refresh();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Refresh();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Refresh();
        }
    }
}
