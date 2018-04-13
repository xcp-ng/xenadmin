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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenCenterLib;


namespace XenAdmin.Controls.CustomDataGraph
{
    public partial class DataKey : FlickerFreeListBox
    {
        private ArchiveMaintainer _archivemaintainer;
        public List<string> DataSourceUUIDsToShow = new List<string>();

        public ArchiveMaintainer ArchiveMaintainer
        {
            get { return _archivemaintainer; }
            set { _archivemaintainer = value; }
        }

        public DataPlot DataPlot;

        public List<DataSetCollectionWrapper> CurrentKeys = new List<DataSetCollectionWrapper>();

        public event Action SetsChanged;

        public DataKey()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            DrawItem += new DrawItemEventHandler(DataKey_DrawItem);
            MouseDown += new MouseEventHandler(DataKey_MouseDown);
        }

        void DataKey_MouseDown(object sender, MouseEventArgs e)
        {
            int index = IndexFromPoint(e.Location);
            if (index == -1 || index >= Items.Count) return;
            SelectedIndex = index;
        }

        void DataKey_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1 || (ArchiveMaintainer != null && ArchiveMaintainer.LoadingInitialData))
                return;
            DataSetCollectionWrapper item = (DataSetCollectionWrapper)Items[e.Index];
            
            lock (Palette.PaletteLock)
            {
                using (var thickPen = Palette.CreatePen(item.Sets[ArchiveInterval.FiveSecond].Uuid, Palette.PEN_THICKNESS_THICK))
                {
                    e.Graphics.DrawLine(thickPen,
                        new Point(e.Bounds.Left + 2, e.Bounds.Top + e.Bounds.Height / 2),
                        new Point(e.Bounds.Left + e.Bounds.Height - 4, e.Bounds.Top + e.Bounds.Height / 2));
                }
            }
            
            int lineWidth = e.Bounds.Height;
            int width = Drawing.MeasureText(e.Graphics, Items[e.Index].ToString(), Font).Width;
            Rectangle textRect = new Rectangle(e.Bounds.Left + lineWidth, e.Bounds.Top, width, e.Bounds.Height);
            Drawing.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, textRect, e.ForeColor, e.BackColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            
            width += lineWidth;
            if (HorizontalExtent < width)
                HorizontalExtent = width;
        }

        /// <summary>
        /// Uses Sets (which is used for drawing) so must be on the event thread)
        /// </summary>
        public void UpdateItems()
        {
            Program.AssertOnEventThread();

            if (DataPlot == null || DataPlot.DataPlotNav == null)
                return;

            var wrappers = new List<DataSetCollectionWrapper>();
            var archives = ArchiveMaintainer.Archives;
            var fiveSecSets = archives[ArchiveInterval.FiveSecond].Sets;

            foreach (DataSet fivesecond in fiveSecSets)
            {
                var wrapper = new DataSetCollectionWrapper();

                if (!DataSourceUUIDsToShow.Contains(fivesecond.Uuid))
                    continue;

                wrapper.Sets.Add(ArchiveInterval.FiveSecond, fivesecond);
                var intervals = new[] {ArchiveInterval.OneMinute, ArchiveInterval.OneHour, ArchiveInterval.OneDay};

                foreach (var interval in intervals)
                {
                    var found = archives[interval].Sets.FirstOrDefault(s => s.Uuid == fivesecond.Uuid);
                    if (found != null)
                        wrapper.Sets.Add(interval, found);
                }

                wrappers.Add(wrapper);
            }

            bool anynew = false;

            foreach (DataSetCollectionWrapper wrapper in wrappers)
            {
                if (!CurrentKeys.Contains(wrapper))
                {
                    CurrentKeys.Add(wrapper);
                    anynew = true;
                }
                else if (CurrentKeys[CurrentKeys.IndexOf(wrapper)].Sets.Count < wrapper.Sets.Count)
                {
                    CurrentKeys.Remove(wrapper);
                    CurrentKeys.Add(wrapper);
                    anynew = true;
                }
            }

            CurrentKeys.RemoveAll(item =>
            {
                if (!wrappers.Contains(item))
                {
                    anynew = true;
                    return true;
                }
                return false;
            });

            if (!anynew)
            {
                Refresh(); // is this necessary
                return;
            }

            BeginUpdate();
            try
            {
                Items.Clear();
                foreach (DataSetCollectionWrapper key in CurrentKeys)
                {
                    if (key.Show)
                        Items.Add(key);
                }
                Sort();
            }
            finally
            {
                EndUpdate();
                Refresh();
            }
        }

        protected override void Sort()
        {
            List<DataSetCollectionWrapper> list = new List<DataSetCollectionWrapper>();
            foreach (DataSetCollectionWrapper item in Items)
            {
                list.Add(item);
            }

            list.Sort();
            Items.Clear();
            Items.AddRange(list.ToArray());
        }

        // may return null
        public DataSet SelectedDataSet
        {
            get
            {
                if (SelectedIndex == -1 || DataPlot == null || DataPlot.DataPlotNav == null)
                    return null;
                DataSetCollectionWrapper dscw = (Items[SelectedIndex] as DataSetCollectionWrapper);
                ArchiveInterval current = DataPlot.DataPlotNav.GetCurrentLeftArchiveInterval();
                if(!dscw.Sets.ContainsKey(current))
                    return null;

                return (Items[SelectedIndex] as DataSetCollectionWrapper).Sets[current];
            }
        }

        public DataSetCollectionWrapper SelectedSet = null;

        private void DataKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedSet != null)
                SelectedSet.Selected = false;
            SelectedSet = (DataSetCollectionWrapper)SelectedItem;
            if (SelectedSet != null)
                SelectedSet.Selected = true;
        }

        private void DataKey_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (SelectedSet != null && DataPlot.PlotNav != null)
            //    DataPlot.PlotNav.ZoomToFit(SelectedSet);
        }

        internal void Clear()
        {
            Items.Clear();
            CurrentKeys.Clear();
            HorizontalExtent = 0;
            SetsChanged();
        }

        int LastIndexSelected = -1;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (LastIndexSelected == SelectedIndex)
                return;
            if (DataPlot.SelectedPoint != null)
            {
                DataPlot.SelectedPoint.Show = false;
                DataPlot.SelectedPoint = null;
            }
            LastIndexSelected = SelectedIndex;
            base.OnSelectedIndexChanged(e);
        }

        DataSetCollectionWrapper LastValueSelected;

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            if (LastValueSelected == (DataSetCollectionWrapper)SelectedItem) return;
            LastValueSelected = (DataSetCollectionWrapper)SelectedItem;
            base.OnSelectedValueChanged(e);
        }

        public void SelectDataSet(DataSet set)
        {
            SelectedItem = SelectWrapperFromUuid(set.Uuid);
        }

        public DataSetCollectionWrapper SelectWrapperFromUuid(string uuid)
        {
            return CurrentKeys.Find(new Predicate<DataSetCollectionWrapper>(delegate(DataSetCollectionWrapper item)
            {
                return item.Uuid == uuid;
            }));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ArchiveMaintainer != null && ArchiveMaintainer.LoadingInitialData)
            {
                e.Graphics.DrawString(Messages.GRAPH_LOADING, Palette.LabelFont, Palette.LabelBrush, 10, 10);
                return;
            }
            base.OnPaint(e);
        }
    }

    public class DataSetCollectionWrapper : IEquatable<DataSetCollectionWrapper>, IComparable<DataSetCollectionWrapper>
    {
        public Dictionary<ArchiveInterval, DataSet> Sets = new Dictionary<ArchiveInterval, DataSet>();

        private bool _selected;
        private bool _hide;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                foreach (DataSet set in Sets.Values)
                {
                    set.Selected = value;
                }
            }
        }

        public bool Hide
        {
            get { return _hide; }
            set
            {
                _hide = value;
                foreach (DataSet set in Sets.Values)
                {
                    set.Deselected = value;
                }
            }
        }

        public DataRange YRange
        {
            get
            {
                foreach (DataSet s in Sets.Values)
                {
                    return s.CustomYRange;
                }
                return null;
            }
        }

        public bool Show
        {
            get
            {
                if (!Sets.ContainsKey(ArchiveInterval.FiveSecond))
                    return false;

                return Sets[ArchiveInterval.FiveSecond].Show;
            }
        }

        public DataSetCollectionWrapper()
        {

        }

        public string Uuid
        {
            get
            {
                if (!Sets.ContainsKey(ArchiveInterval.FiveSecond))
                    return base.ToString();

                return Sets[ArchiveInterval.FiveSecond].Uuid;
            }
        }

        public override string ToString()
        {
            if (!Sets.ContainsKey(ArchiveInterval.FiveSecond))
                return base.ToString();

            return Sets[ArchiveInterval.FiveSecond].ToString();
        }

        public bool Equals(DataSetCollectionWrapper other)
        {
            return Uuid.Equals(other.Uuid);
        }

        public int CompareTo(DataSetCollectionWrapper other)
        {
            return StringUtility.NaturalCompare(this.ToString(), other.ToString());
        }
    }
}
