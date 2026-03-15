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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using XenCenterLib;

using Message = System.Windows.Forms.Message;
using System.Runtime.InteropServices;

namespace XenAdmin.Controls
{
    public partial class SnapshotTreeView : ListView
    {
        private const int straightLineLength = 8;

        private SnapshotIcon root;


        //We need this fake thing to make the scrollbars work with the customdrawdate.
        private ListViewItem whiteIcon = new ListViewItem();


        private Color linkLineColor = SystemColors.ControlDark;
        private float linkLineWidth = 2.0f;

        private int hGap = 50;
        private int vGap = 20;
        private readonly CustomLineCap linkLineArrow = new AdjustableArrowCap(4f, 4f, true);


        #region Properties

        [Browsable(true), Category("Appearance"), Description("Color used to draw connecting lines")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color LinkLineColor
        {
            get { return linkLineColor; }
            set
            {
                linkLineColor = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance"), Description("Width of connecting lines")]
        [DefaultValue(2.0f)]
        public float LinkLineWidth
        {
            get { return linkLineWidth; }
            set
            {
                linkLineWidth = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance"), Description("Horizontal gap between icons")]
        [DefaultValue(50)]
        public int HGap
        {
            get { return hGap; }
            set
            {
                if (value < 4 * straightLineLength)
                    value = 4 * straightLineLength;
                hGap = value;
                PerformLayout(this, "HGap");
            }
        }

        [Browsable(true), Category("Appearance"), Description("Vertical gap between icons")]
        [DefaultValue(20)]
        public int VGap
        {
            get { return vGap; }
            set
            {
                if (value < 0)
                    value = 0;
                vGap = value;
                PerformLayout(this, "VGap");
            }
        }

        #endregion

        private ImageList imageList = new ImageList();

        public SnapshotTreeView(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            container.Add(this);

            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.Items.Add(whiteIcon);

            //Init image list
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize=new Size(32,32);
            imageList.Images.Add(Properties.Resources._000_HighLightVM_h32bit_32);
            imageList.Images.Add(Properties.Resources.VMTemplate_h32bit_32);
            imageList.Images.Add(Properties.Resources.VMTemplate_h32bit_32);
            imageList.Images.Add(Properties.Resources._000_VMSnapShotDiskOnly_h32bit_32);
            imageList.Images.Add(Properties.Resources._000_VMSnapshotDiskMemory_h32bit_32);
            imageList.Images.Add(Properties.Resources._000_ScheduledVMsnapshotDiskOnly_h32bit_32);
            imageList.Images.Add(Properties.Resources._000_ScheduledVMSnapshotDiskMemory_h32bit_32);
            imageList.Images.Add(Properties.Resources.SpinningFrame0);
            imageList.Images.Add(Properties.Resources.SpinningFrame1);
            imageList.Images.Add(Properties.Resources.SpinningFrame2);
            imageList.Images.Add(Properties.Resources.SpinningFrame3);
            imageList.Images.Add(Properties.Resources.SpinningFrame4);
            imageList.Images.Add(Properties.Resources.SpinningFrame5);
            imageList.Images.Add(Properties.Resources.SpinningFrame6);
            imageList.Images.Add(Properties.Resources.SpinningFrame7);

            this.LargeImageList = imageList;

        }

        internal ListViewItem AddSnapshot(SnapshotIcon snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException("snapshot");
            if (snapshot.Parent != null)
            {
                snapshot.Parent.AddChild(snapshot);
                snapshot.Parent.Invalidate();
            }
            else if (root != null)
            {
                throw new InvalidOperationException("Adding a new root!");
            }
            else
            {
                root = snapshot;
            }
            if (snapshot.ImageIndex == SnapshotIcon.VMImageIndex)
            {
                //Sort all the parents of the VM to make the path to the VM the first one.
                SnapshotIcon current = snapshot;
                while (current.Parent != null)
                {
                    if (current.Parent.Children.Count > 1)
                    {
                        int indexCurrent = current.Parent.Children.IndexOf(current);
                        SnapshotIcon temp = current.Parent.Children[0];
                        current.Parent.Children[0] = current;
                        current.Parent.Children[indexCurrent] = temp;

                    }
                    current.IsInVMBranch = true;
                    current = current.Parent;
                }

            }
            ListViewItem item = Items.Add(snapshot);
            if (Items.Count == 1)
                Items.Add(whiteIcon);
            return item;
        }



        public new void Clear()
        {
            base.Clear();
            RemoveSnapshot(root);
        }



        internal void RemoveSnapshot(SnapshotIcon snapshot)
        {
            if (snapshot != null && snapshot.Parent != null)
            {
                IList<SnapshotIcon> siblings = snapshot.Parent.Children;
                int pos = siblings.IndexOf(snapshot);
                siblings.Remove(snapshot);
                // add our children in our place
                foreach (SnapshotIcon child in snapshot.Children)
                {
                    siblings.Insert(pos++, child);
                    child.Parent = snapshot.Parent;
                }
                snapshot.Parent.Invalidate();
                snapshot.Parent = null;
            }
            else
            {
                root = null;
            }

        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedItems.Count == 1)
            {
                SnapshotIcon item = SelectedItems[0] as SnapshotIcon;
                if (item != null && !item.Selectable)
                {
                    item.Selected = false;
                    item.Focused = false;
                }


            }
            else if (this.SelectedItems.Count > 1)
            {
                foreach (ListViewItem item in SelectedItems)
                {
                    SnapshotIcon snapItem = item as SnapshotIcon;
                    if (snapItem != null && !snapItem.Selectable)
                    {
                        item.Selected = false;
                        item.Focused = false;
                    }
                }
            }

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ListViewItem item = this.HitTest(e.X, e.Y).Item;
            if (item == null)
            {
                ListViewItem item2 = this.HitTest(e.X, e.Y - 23).Item;
                if (item2 != null)
                {
                    item2.Selected = true;
                    item2.Focused = true;
                }
                else
                {
                    base.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y - 23, e.Delta));
                    return;
                }
            }

            base.OnMouseUp(e);

        }

        #region Layout

        protected override void OnLayout(LayoutEventArgs levent)
        {

            if (root != null && this.Parent != null)
            {
                //This is needed to maximize and minimize properly, there is some issue in the ListView Control
                Win32.POINT pt = new Win32.POINT();
                
                if (Type.GetType("Mono.Runtime") == null)
                {
                    IntPtr hResult = SendMessage(Handle, LVM_GETORIGIN, IntPtr.Zero, ref pt);
                }
                else
                {
                    // TODO: There is an exception under Mono

                    // System.DllNotFoundException: user32.dll assembly:<unknown assembly> type:<unknown type> member:(null)
                    // at (wrapper managed-to-native) XenAdmin.Controls.SnapshotTreeView.SendMessage(intptr,int,intptr,XenCenterLib.Win32/POINT&)
                    // at XenAdmin.Controls.SnapshotTreeView.OnLayout (System.Windows.Forms.LayoutEventArgs levent)
                }

                origin = pt;
                root.InvalidateAll();
                int x = Math.Max(this.HGap, this.Size.Width / 2 - root.SubtreeWidth / 2);
                int y = Math.Max(this.VGap, this.Size.Height / 2 - root.SubtreeHeight / 2);
                PositionSnapshots(root, x, y);
                Invalidate();

                whiteIcon.Position = new Point(x + origin.X, y + root.SubtreeHeight - 20 + origin.Y);
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            //We need to cancel the parent changes when we change to the other view
        }

        private void PositionSnapshots(SnapshotIcon icon, int x, int y)
        {
            try
            {
                Size iconSize = icon.DefaultSize;


                Point newPoint = new Point(x, y + icon.CentreHeight - iconSize.Height / 2);
                icon.Position = new Point(newPoint.X + origin.X, newPoint.Y + origin.Y);
                x += iconSize.Width + HGap;
                for (int i = 0; i < icon.Children.Count; i++)
                {
                    SnapshotIcon child = icon.Children[i];
                    PositionSnapshots(child, x, y);
                    y += child.SubtreeHeight;
                }
            }
            catch (Exception)
            {
                // Debugger.Break();
            }
        }
        public const int LVM_GETORIGIN = 0x1000 + 41;

        private Win32.POINT origin = new Win32.POINT();

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref Win32.POINT pt);

        #endregion

        #region Drawing

        private bool m_empty = false;
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            const int WM_HSCROLL = 0x0114;
            const int WM_VSCROLL = 0x0115;
            const int WM_MOUSEWHEEL = 0x020A;
            const int WM_PAINT = 0x000F;

            base.WndProc(ref m);

            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL || (m.Msg == WM_MOUSEWHEEL && (IsVerticalScrollBarVisible(this) || IsHorizontalScrollBarVisible(this))))
            {
                Invalidate();
            }
            if (m.Msg == WM_PAINT)
            {
                Graphics gg = CreateGraphics();

                if (this.Items.Count == 0)
                {
                    m_empty = true;
                    Graphics g = CreateGraphics();
                    string text = Messages.SNAPSHOTS_EMPTY;
                    SizeF proposedSize = g.MeasureString(text, Font, 275);
                    float x = this.Width / 2 - proposedSize.Width / 2;
                    float y = this.Height / 2 - proposedSize.Height / 2;
                    RectangleF rect = new RectangleF(x, y, proposedSize.Width, proposedSize.Height);
                    using (var brush = new SolidBrush(BackColor))
                        g.FillRectangle(brush, rect);
                    g.DrawString(text, Font, Brushes.Black, rect);
                }
            }
            else if (m_empty && this.Items.Count > 0)
            {
                m_empty = false;
                this.Invalidate();
            }
        }




        private const int WS_HSCROLL = 0x100000;
        private const int WS_VSCROLL = 0x200000;
        private const int GWL_STYLE = (-16);

        [System.Runtime.InteropServices.DllImport("user32",
            CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        internal static bool IsVerticalScrollBarVisible(Control ctrl)
        {
            if (!ctrl.IsHandleCreated)
                return false;

            return (GetWindowLong(ctrl.Handle, GWL_STYLE) & WS_VSCROLL) != 0;
        }

        internal static bool IsHorizontalScrollBarVisible(Control ctrl)
        {
            if (!ctrl.IsHandleCreated)
                return false;

            return (GetWindowLong(ctrl.Handle, GWL_STYLE) & WS_HSCROLL) != 0;
        }


        private void SnapshotTreeView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

            if (this.Parent != null)
            {
                e.DrawDefault = true;
                SnapshotIcon icon = e.Item as SnapshotIcon;
                if (icon == null)
                    return;
                
                DrawDate(e, icon, false);

                if (icon.Parent != null)
                    PaintLine(e.Graphics, icon.Parent, icon, icon.IsInVMBranch);
                for (int i = 0; i < icon.Children.Count; i++)
                {
                    SnapshotIcon child = icon.Children[i];
                    PaintLine(e.Graphics, icon, child, child.IsInVMBranch);

                }

            }

        }

        public void DrawRoundRect(Graphics g, Brush b, float x, float y, float width, float height, float radius)
        {

            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line

            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner

            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line

            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line

            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner

            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line

            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner

            gp.CloseFigure();



            g.FillPath(b, gp);

        }

        private void DrawDate(DrawListViewItemEventArgs e, SnapshotIcon icon, bool background)
        {

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            //Time
            int timeX = e.Bounds.X;
            int timeY = e.Bounds.Y + e.Bounds.Height;
            Size proposedSizeName = new Size(e.Bounds.Width,
                                            Int32.MaxValue);
            Size timeSize = TextRenderer.MeasureText(e.Graphics, icon.LabelCreationTime,
                                                     new Font(this.Font.FontFamily, this.Font.Size - 1), proposedSizeName, TextFormatFlags.WordBreak);
            timeSize = new Size(e.Bounds.Width, timeSize.Height);
            Rectangle timeRect = new Rectangle(new Point(timeX, timeY), timeSize);
            if (background)
            {
                e.Graphics.FillRectangle(Brushes.GreenYellow, timeRect);
            }
            e.Graphics.DrawString(icon.LabelCreationTime, new Font(this.Font.FontFamily, this.Font.Size - 1), Brushes.Black, timeRect, stringFormat);

        }

        private void PaintLine(Graphics g, SnapshotIcon icon, SnapshotIcon child, bool highlight)
        {
            if (child.Index == -1)
                return;

            try
            {
                Rectangle leftItemBounds = icon.GetBounds(ItemBoundsPortion.Entire);
                Rectangle rightItemBounds = child.GetBounds(ItemBoundsPortion.Entire);
                leftItemBounds.Size = icon.DefaultSize;
                rightItemBounds.Size = child.DefaultSize;


                int left = leftItemBounds.Right + 6;
                int right = rightItemBounds.Left;
                int mid = (left + right) / 2;
                Point start = new Point(left, (leftItemBounds.Bottom + leftItemBounds.Top) / 2);
                Point end = new Point(right, (rightItemBounds.Top + rightItemBounds.Bottom) / 2);
                Point curveStart = start;
                curveStart.Offset(straightLineLength, 0);
                Point curveEnd = end;
                curveEnd.Offset(-straightLineLength, 0);
                Point control1 = new Point(mid + straightLineLength, start.Y);
                Point control2 = new Point(mid - straightLineLength, end.Y);

                Color lineColor = LinkLineColor;
                float lineWidth = LinkLineWidth;
                if (highlight)
                {
                    lineColor = Color.ForestGreen;
                    lineWidth = 2.5f;
                }
                using (Pen p = new Pen(lineColor, lineWidth))
                {
                    p.SetLineCap(LineCap.Round, LineCap.Custom, DashCap.Flat);
                    p.CustomEndCap = linkLineArrow;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    GraphicsPath path = new GraphicsPath();
                    path.AddLine(start, curveStart);
                    path.AddBezier(curveStart, control1, control2, curveEnd);
                    path.AddLine(curveEnd, end);
                    g.DrawPath(p, path);
                }
            }
            catch (Exception)
            {
                //Debugger.Break();
            }
        }



        #endregion

        private string _spinningMessage = "";
        public string SpinningMessage { get { return _spinningMessage; } }

        internal void ChangeVMToSpinning(bool p, string message)
        {
            _spinningMessage = message;
            foreach (var item in Items)
            {
                if (item is SnapshotIcon snapshotIcon && (snapshotIcon.ImageIndex == SnapshotIcon.VMImageIndex || snapshotIcon.ImageIndex > SnapshotIcon.UnknownImage))
                {
                    snapshotIcon.ChangeSpinningIcon(p, _spinningMessage);
                    return;
                }
            }
        }
    }


    internal class SnapshotIcon : ListViewItem
    {
        public const int VMImageIndex = 0;
        public const int Template = 1;
        public const int CustomTemplate = 2;
        public const int DiskSnapshot = 3;
        public const int DiskAndMemorySnapshot = 4;
        public const int ScheduledDiskSnapshot = 5;
        public const int ScheduledDiskMemorySnapshot = 6;
        public const int UnknownImage = 6;

        private SnapshotIcon parent;
        private readonly SnapshotTreeView treeView;
        private readonly List<SnapshotIcon> children = new List<SnapshotIcon>();
        private readonly string _name;
        private readonly string _creationTime;

        public Size DefaultSize = new Size(70, 64);

        private Timer spinningTimer = new Timer();



        #region Cached dimensions

        private int subtreeWidth;
        private int subtreeHeight;
        private int subtreeWeight;
        private int centreHeight;

        #endregion

        #region Properties

        public string LabelCreationTime
        {
            get { return _creationTime; }
        }

        public string LabelName
        {
            get { return _name; }
        }



        internal SnapshotIcon Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        internal IList<SnapshotIcon> Children
        {
            get { return children; }
        }

        internal bool Selectable
        {
            get
            {
                // It's selectable if it's a snapshot; otherwise not
                return ImageIndex == DiskSnapshot || ImageIndex == DiskAndMemorySnapshot || ImageIndex == ScheduledDiskSnapshot || ImageIndex == ScheduledDiskMemorySnapshot;
            }
        }

        #endregion

        public SnapshotIcon(string name, string createTime, SnapshotIcon parent, SnapshotTreeView treeView, int imageIndex)
            : base(name.Ellipsise(35))
        {
            this._name = name.Ellipsise(35);
            this._creationTime = createTime;
            this.parent = parent;
            this.treeView = treeView;
            this.UseItemStyleForSubItems = false;
            this.ToolTipText = String.Format("{0} {1}", name, createTime);
            this.ImageIndex = imageIndex;
            if (imageIndex == SnapshotIcon.VMImageIndex)
            {
                spinningTimer.Tick += new EventHandler(timer_Tick);
                spinningTimer.Interval = 150;
            }

        }

        private int currentSpinningFrame = 7;
        private void timer_Tick(object sender, EventArgs e)
        {
            this.ImageIndex = currentSpinningFrame <= 14 ? currentSpinningFrame++ : currentSpinningFrame = 7;
        }

        internal void ChangeSpinningIcon(bool enabled, string message)
        {
            if (this.ImageIndex > UnknownImage || this.ImageIndex == VMImageIndex)
            {
                this.ImageIndex = enabled ? 7 : VMImageIndex;
                this.Text = enabled ? message : Messages.NOW;

                if (enabled)
                    spinningTimer.Start();
                else
                    spinningTimer.Stop();
            }
        }



        private bool _isInVMBranch = false;
        public bool IsInVMBranch
        {
            get { return _isInVMBranch; }
            set { _isInVMBranch = value; }
        }

        internal void AddChild(SnapshotIcon icon)
        {
            children.Add(icon);
            Invalidate();
        }

        public override void Remove()
        {
            SnapshotTreeView view = (SnapshotTreeView)ListView;
            view.RemoveSnapshot(this);
            base.Remove();
            view.PerformLayout();
        }

        /// <summary>
        /// Causes the item and its ancestors to forget their cached dimensions.
        /// </summary>
        public void Invalidate()
        {
            subtreeWeight = 0;
            subtreeHeight = 0;
            centreHeight = 0;
            subtreeWidth = 0;
            if (parent != null)
                parent.Invalidate();
        }

        /// <summary>
        /// Causes the item and all its descendents to forget their cached dimensions.
        /// </summary>
        public void InvalidateAll()
        {
            subtreeWeight = 0;
            subtreeHeight = 0;
            centreHeight = 0;
            subtreeWidth = 0;
            foreach (SnapshotIcon icon in children)
            {
                icon.InvalidateAll();
            }
        }





        #region Layout/Dimensions

        public int SubtreeWidth
        {
            get
            {
                if (subtreeWidth == 0)
                {
                    int currentWidth = this.DefaultSize.Width + treeView.HGap;
                    if (children.Count > 0)
                    {
                        int maxWidth = 0;
                        foreach (SnapshotIcon icon in children)
                        {
                            int childSubtree = icon.SubtreeWidth;
                            if (currentWidth + childSubtree > maxWidth)
                                maxWidth = currentWidth + childSubtree;
                        }
                        if (maxWidth > currentWidth)
                            subtreeWidth = maxWidth;
                        else
                            subtreeWidth = currentWidth;
                    }
                }
                return subtreeWidth;
            }
        }

        /// <summary>
        /// Height of the subtree rooted at this node, including the margin above and below.
        /// </summary>
        public int SubtreeHeight
        {
            get
            {
                if (subtreeHeight == 0)
                {
                    subtreeHeight = this.DefaultSize.Height + treeView.VGap;
                    if (children.Count > 0)
                    {
                        int height = 0;
                        foreach (SnapshotIcon icon in children)
                        {
                            height += icon.SubtreeHeight; // recurse
                        }
                        if (height > subtreeHeight)
                            subtreeHeight = height;
                    }
                }
                return subtreeHeight;
            }
        }

        /// <summary>
        /// The number of items rooted at this node (including the node itself)
        /// </summary>
        public int SubtreeWeight
        {
            get
            {
                if (subtreeWeight == 0)
                {
                    int weight = 1; // this
                    foreach (SnapshotIcon icon in children)
                    {
                        weight += icon.SubtreeWeight;
                    }
                    subtreeWeight = weight;
                }
                return subtreeWeight;
            }
        }

        /// <summary>
        /// The weighted mean centre height for this node, within the range 0 - SubtreeHeight
        /// </summary>
        public int CentreHeight
        {
            get
            {
                if (centreHeight == 0)
                {
                    int top = 0;
                    int totalWeight = 0;
                    int weightedCentre = 0;
                    foreach (SnapshotIcon icon in children)
                    {
                        int iconWeight = icon.SubtreeWeight;
                        totalWeight += iconWeight;
                        weightedCentre += iconWeight * (top + icon.CentreHeight); // recurse
                        top += icon.SubtreeHeight;
                    }
                    if (totalWeight > 0)
                        centreHeight = weightedCentre / totalWeight;
                    else
                        centreHeight = (top + SubtreeHeight) / 2;
                }
                return centreHeight;
            }
        }

        #endregion
    }
}