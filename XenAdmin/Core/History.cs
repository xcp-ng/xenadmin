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
using XenAdmin.XenSearch;
using System.Drawing;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Core
{
    class History
    {
        private static readonly LimitedStack<HistoryItem> backwardHistory = new LimitedStack<HistoryItem>(15);
        private static readonly LimitedStack<HistoryItem> forwardHistory = new LimitedStack<HistoryItem>(15);
        private static HistoryItem currentHistoryItem;
        private static bool InHistoryNavigation = false;

        // Use this when modifying a search
        public static void ReplaceHistoryItem(HistoryItem historyItem)
        {
            if (InHistoryNavigation)
                return;

            currentHistoryItem = historyItem;

            EnableHistoryButtons();
        }

        public static void NewHistoryItem(HistoryItem historyItem)
        {
            if (InHistoryNavigation)
                return;

            if (historyItem.Equals(currentHistoryItem))
                return;

            if (currentHistoryItem != null)
                backwardHistory.Push(currentHistoryItem);

            forwardHistory.Clear();

            currentHistoryItem = historyItem;

            EnableHistoryButtons();
        }

        public static void Back(int i)
        {
            while (i > 0)
            {
                forwardHistory.Push(currentHistoryItem);
                currentHistoryItem = backwardHistory.Pop();
                i--;
            }

            DoHistoryItem(currentHistoryItem);
        }

        public static void Forward(int i)
        {
            while (i > 0)
            {
                backwardHistory.Push(currentHistoryItem);
                currentHistoryItem = forwardHistory.Pop();
                i--;
            }
            
            DoHistoryItem(currentHistoryItem);
        }

        private static void DoHistoryItem(HistoryItem item)
        {
            InHistoryNavigation = true;

            try
            {
                item.Go();
            }
            finally
            {
                InHistoryNavigation = false;

                EnableHistoryButtons();
            }
        }

        public static void EnableHistoryButtons()
        {
            Program.MainWindow.forwardButton.Enabled = forwardHistory.Peek() != null;
            Program.MainWindow.backButton.Enabled = backwardHistory.Peek() != null;
        }

        private delegate void HistoryNavigationDelegate(int i);

        public static void PopulateForwardDropDown(ToolStripSplitButton button)
        {
            PopulateMenuWith(button, forwardHistory, Forward);
        }

        public static void PopulateBackDropDown(ToolStripSplitButton button)
        {
            PopulateMenuWith(button, backwardHistory, Back);
        }

        private static void PopulateMenuWith(ToolStripSplitButton button, LimitedStack<HistoryItem> history, 
            HistoryNavigationDelegate historyNaviagtionDelegate)
        {
            button.DropDownItems.Clear();

            int i = 0;
            foreach (HistoryItem item in history)
            {
                int j = ++i;
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = item.Name.EscapeAmpersands();
                menuItem.Image = item.Image;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Click += delegate(Object sender, EventArgs e)
                {
                    historyNaviagtionDelegate(j);
                };

                button.DropDownItems.Add(menuItem);
            }
        }
    }

    public interface HistoryItem
    {
        bool Go();
        String Name { get; }
        Image Image { get; }
    }

    public class XenModelObjectHistoryItem : HistoryItem
    {
        private readonly IXenObject o;
        private readonly TabPage tab;

        public XenModelObjectHistoryItem(IXenObject o, TabPage tab)
        {
            this.o = o;
            this.tab = tab;
        }

        public bool Go()
        {
            if (Program.MainWindow.SelectObjectInTree(o))
            {
                Program.MainWindow.TheTabControl.SelectedTab = tab;
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            XenModelObjectHistoryItem other = obj as XenModelObjectHistoryItem;
            if (other == null)
                return false;

            if (other.tab != tab)
                return false;

            if (other.o == null && o == null)
                return true;

            if (other.o == null || o == null)
                return false;

            return other.o.opaque_ref == o.opaque_ref;
        }

        public override int GetHashCode()
        {
            return o.opaque_ref.GetHashCode();
        }

        public String Name
        {
            get
            {
                return String.Format("{0}, ({1})", 
                    o == null ? Messages.XENCENTER : Helpers.GetName(o), tab.Text);
            }
        }

        public Image Image
        {
            get
            {
                return o == null ? Images.GetImage16For(Icons.XenCenter) : Images.GetImage16For(o);
            }
        }
    }

    public class SearchHistoryItem : HistoryItem
    {
        private readonly Search search;

        public SearchHistoryItem(Search search)
        {
            this.search = search;
        }

        public bool Go()
        {
            Program.MainWindow.DoSearch(search);
            return true;
        }

        public override bool Equals(object obj)
        {
            SearchHistoryItem other = obj as SearchHistoryItem;
            if (other == null)
                return false;

            return other.search.Equals(search);
        }

        public override int GetHashCode()
        {
            return search.GetHashCode();
        }

        public String Name
        {
            get
            {
                return search.Name;
            }
        }

        public Image Image
        {
            get
            {
                return Images.GetImage16For(search);
            }
        }
    }

    public class ModifiedSearchHistoryItem : HistoryItem
    {
        private readonly IXenObject o;
        private readonly Search search;

        public ModifiedSearchHistoryItem(IXenObject o, Search search)
        {
            this.o = o;
            this.search = search;
        }

        public bool Go()
        {
            if (Program.MainWindow.SelectObjectInTree(o))
            {
                Program.MainWindow.TheTabControl.SelectedTab = Program.MainWindow.TabPageSearch;
                Program.MainWindow.SearchPage.Search = search;
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            ModifiedSearchHistoryItem other = obj as ModifiedSearchHistoryItem;
            if (other == null)
                return false;

            if (!other.search.Equals(search))
                return false;

            if (other.o == null && o == null)
                return true;

            if (other.o == null || o == null)
                return false;

            return other.o.opaque_ref == o.opaque_ref;
        }

        public override int GetHashCode()
        {
            return o.opaque_ref.GetHashCode();
        }

        public String Name
        {
            get
            {
                return String.Format("{0}, ({1})",
                    o == null ? Messages.XENCENTER : Helpers.GetName(o), Program.MainWindow.TabPageSearch.Text);
            }
        }

        public Image Image
        {
            get
            {
                return o == null ? Images.GetImage16For(Icons.XenCenter) : Images.GetImage16For(o);
            }
        }
    }
}
