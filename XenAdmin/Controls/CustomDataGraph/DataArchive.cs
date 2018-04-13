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
using System.Linq;
using System.Text;
using XenAPI;

namespace XenAdmin.Controls.CustomDataGraph
{
    public class DataArchive
    {
        /// <summary>
        /// Used by the drawing code, must be updated on the event thread
        /// </summary>
        public List<DataSet> Sets = new List<DataSet>();
        private int maxPoints;
        public int MaxPoints
        {
            get { return maxPoints; }
            set
            {
                maxPoints = value;

                foreach (DataSet set in Sets)
                    set.TrimEnd(MaxPoints);
            }
        }

        public DataArchive(int maxpoints)
        {
            maxPoints = maxpoints;
        }

        /// <summary>
        /// Sets is used directly by the drawing code, so this method asserts it is on the event thread to avoid threading issues.
        /// </summary>
        /// <param name="set"></param>
        private void AddSet(DataSet set)
        {
            Program.AssertOnEventThread();

            set.Points.Sort();

            var other = Sets.FirstOrDefault(s => s.Uuid == set.Uuid);

            if (other == null)
            {
                Sets.Add(set);
                set.TrimEnd(MaxPoints);
            }
            else
            {
                other.InsertPointCollection(set.Points);
                other.TrimEnd(MaxPoints);
            }
        }

        /// <summary>
        /// Asserts off the event thread. Safely invokes to update the data sets.
        /// </summary>
        /// <param name="SetsAdded"></param>
        internal void Load(List<DataSet> SetsAdded)
        {
            if (SetsAdded == null)
                return;
            foreach (DataSet set in SetsAdded)
            {
                Palette.LoadSetColor(set);
                DataSet set1 = set;
                Program.Invoke(Program.MainWindow, () => AddSet(set1));
            }
        }

        /// <summary>
        /// Asserts off the event thread, for use when copying off the updater thread. Invokes onto event thread to update the sets safely.
        /// </summary>
        /// <param name="SetsAdded"></param>
        internal void CopyLoad(List<DataSet> SetsAdded)
        {
            Program.AssertOffEventThread();
            if (SetsAdded == null)
                return;
            foreach (DataSet set in SetsAdded)
            {
                Palette.LoadSetColor(set);
                DataSet copy = DataSet.Create(set.Uuid, set.XenObject, set.Show, set.TypeString);
                foreach (DataPoint p in set.Points)
                    copy.AddPoint(new DataPoint(p.X,p.Y));

                Program.Invoke(Program.MainWindow, delegate
                {
                    AddSet(copy);
                });
            }
        }

        internal void ClearSets()
        {
            Program.Invoke(Program.MainWindow, () => Sets.Clear());
        }
    }
}
