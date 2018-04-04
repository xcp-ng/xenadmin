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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    public class VerticalTabs : FlickerFreeListBox
    {
        public interface IVerticalTab
        {
            string Text { get; }
            string SubText { get; }
            Image Image { get; }
        }

        public VerticalTabs()
        {
            base.ItemHeight = 40;
            IntegralHeight = false; 
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        public override int ItemHeight
        {
            get
            {
                return base.ItemHeight;
            }
            set
            {
            }
        }

        public Func<Rectangle, Rectangle> AdjustItemTextBounds;

        private readonly Color TOP_COLOR = Color.FromArgb(101, 140, 244);
        private readonly Color BOTTOM_COLOR = Color.FromArgb(31, 102, 241);

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            IVerticalTab editPage = Items[e.Index] as IVerticalTab;
            if (editPage == null)
                return;

            Graphics g = e.Graphics;
            Rectangle b = e.Bounds;

            if ((e.State & DrawItemState.Selected) > 0)
            {
                if (Application.RenderWithVisualStyles)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(b, TOP_COLOR,
                        BOTTOM_COLOR, LinearGradientMode.Vertical))
                    {
                        //http://www.dotnetmonster.com/Uwe/Forum.aspx/dotnet-drawing/350/LinearGradientBrush-bug
                        brush.WrapMode = WrapMode.TileFlipX;
                        g.FillRectangle(brush, b);
                    }
                }
                else
                {
                    g.FillRectangle(SystemBrushes.Highlight, b);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(e.BackColor))
                {
                    g.FillRectangle(brush, b);
                }
            }

            Image icon = editPage.Image;
            if (icon != null)
            {
                g.DrawImage(icon, b.X + ((32 - icon.Width) / 2), 
                    b.Y + ((32 - icon.Height) / 2),
                    icon.Width, icon.Height);
            }

            if (AdjustItemTextBounds != null)
                b = AdjustItemTextBounds(b);

            int h = b.Height / 2;
            Rectangle topRect = new Rectangle(b.X + 32, b.Y, b.Width - 34, h - 1);
            Rectangle bottomRect = new Rectangle(b.X + 40, b.Y + h + 1, b.Width - 42, h - 1);

            Drawing.DrawText(g, editPage.Text, Program.DefaultFont, topRect,
                e.ForeColor, Color.Transparent, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.Bottom);

            Color gray = (e.State & DrawItemState.Selected) > 0 ? e.ForeColor : Color.Gray;

            try
            {
                Drawing.DrawText(g, editPage.SubText, Program.DefaultFont, bottomRect,
                    gray, Color.Transparent, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.Top);
            }
            catch
            {
                if (!DesignMode)
                    throw;
            }

            base.OnDrawItem(e);
        }
    }
}
