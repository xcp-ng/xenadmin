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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;


namespace XenAdminTests
{
    public abstract class MainWindowTester : TestObjectProvider
    {
        // Run on MainWindowThread
        private static object[] EMPTY = new object[0];
        protected void MW(MethodInvoker f)
        {
            MW(f, EMPTY);
        }

        protected T MW<T>(Func<T> func)
        {
            return (T)MW(func, EMPTY);
        }

        protected object MW(Delegate f, params object[] args)
        {
            Assert.IsNotNull(Program.MainWindow, "Program.MainWindow was found to be null");
            Assert.IsNotNull(f, "Delegate was found to be null");
            Assert.IsNotNull(args, "Params were found to be null");

            try
            {
                DoEvents();
                object o = Program.Invoke(Program.MainWindow, f, args);
                DoEvents();

                if (Program.TestExceptionString != null)
                    throw new Exception(Program.TestExceptionString);
                return o;
            }
            catch (Exception e)
            {
                String message = Program.TestExceptionString ?? e.GetBaseException().ToString();
                Program.TestExceptionString = null;
                Assert.Fail(message);
                return null;
            }
        }

        /// <summary>
        /// Waits for the specified action to return true. The action is run on the main program thread.
        /// The action is attempted 300 times, with a 100ms wait between tries. If the action doesn't
        /// succeed after being retried then the calling test is failed with the specified message.
        /// If assert message is null, then the test won't be failed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="assertMessage">The assert message.</param>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool MWWaitFor(Func<bool> action, string assertMessage)
        {
            bool success = false;
            for (int i = 0; i < 500 && !success; i++)
            {
                success = MW<bool>(action);
                if (!success)
                {
                    Thread.Sleep(300);
                }
            }

            if (assertMessage != null && !success)
            {
                Assert.Fail(assertMessage);
            }

            return success;
        }

        /// <summary>
        /// Waits for the specified action to return true. The action is run on the calling thread.
        /// The action is attempted 300 times, with a 100ms wait between tries. If the action doesn't
        /// succeed after being retried then the calling test is failed with the specified message.
        /// If assert message is null, then the test won't be failed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="assertMessage">The assert message.</param>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool WaitFor(Func<bool> action, string assertMessage)
        {
            bool success = false;
            for (int i = 0; i < 300 && !success; i++)
            {
                success = action();
                if (!success)
                {
                    Thread.Sleep(100);
                }
            }

            if (assertMessage != null && !success)
            {
                Assert.Fail(assertMessage);
            }
            return success;
        }

        internal Win32Window WaitForWindowToAppear(string windowText)
        {
            return WaitForWindowToAppear(windowText, w => true);
        }

        internal Win32Window WaitForWindowToAppear(string windowText, Predicate<Win32Window> match)
        {
            Win32Window window = null;
            Func<bool> func = delegate
            {
                window = Win32Window.GetWindowWithText(windowText, match);
                return window != null;
            };

            WaitFor(func, "Window with text " + windowText + " didn't appear.");
            return window;
        }

        /// <summary>
        /// Waits for the specified action to return true. The action is run on the main program thread.
        /// The action is attempted 200 times, with a 100ms wait between tries.
        /// </summary>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool MWWaitFor(Func<bool> action)
        {
            return MWWaitFor(action, null);
        }

        /// <summary>
        /// Waits for the specified action to return true. The action is run on the calling thread.
        /// The action is attempted 200 times, with a 100ms wait between tries.
        /// </summary>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool WaitFor(Func<bool> action)
        {
            return WaitFor(action, null);
        }

        /// <summary>
        /// <see cref="Application.DoEvents()"/> doesn't seems to work from the test framework. This is a replacement.
        /// </summary>
        private void DoEvents()
        {
            if (Program.MainWindow.InvokeRequired)
            {
                bool messagesPending = true;

                while (messagesPending)
                {
                    Program.Invoke(Program.MainWindow, delegate
                    {
                        foreach (Win32Window w in Win32Window.GetThreadWindows())
                        {
                            messagesPending = w.MessagesPending;

                            if (messagesPending)
                            {
                                break;
                            }
                        }
                    });
                    if (messagesPending)
                    {
                        Thread.Sleep(5);
                    }
                }
            }
        }

        // Put the TestResources directory on the front of a filename
        protected string TestResource(string name)
        {
            return GetTestResource(name);
        }

        public static string GetTestResource(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), @"XenAdminTests\TestResources", name);
        }


        protected List<VirtualTreeNode> GetAllTreeNodes()
        {
            return MW(() => new List<VirtualTreeNode>(MainWindowWrapper.TreeView.AllNodes));
        }

        protected void EnsureChecked(ToolStripMenuItem menuItem)
        {
            EnsureChecked(menuItem, CheckState.Checked);
        }

        protected void EnsureChecked(ToolStripMenuItem menuItem, CheckState checkState)
        {
            if (menuItem.CheckState != checkState)
                MW(menuItem.PerformClick);
        }

        protected void EnsureDefaultTemplatesShown()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);

            MWWaitFor(delegate
                {
                    var nodes = new List<VirtualTreeNode>(MainWindowWrapper.TreeView.AllNodes);
                    var defaultTemplates = nodes.Select(n => n.Tag is VM && ((VM)n.Tag).DefaultTemplate());
                    return (defaultTemplates.Count() > 0);
                });
        }

        protected void ClickMenuItem(string menuName, string itemName)
        {
            MW(delegate()
            {
                ToolStripItem item = GetMenuItem(menuName, itemName);
                Assert.NotNull(item, string.Format("Menu item '{0}' not found under menu '{1}'", itemName, menuName));
                item.PerformClick();
            });
        }

        protected void CheckMenuItemMissing(string menuName, string itemName)
        {
            MW(delegate()
            {
                ToolStripItem item = GetMenuItem(menuName, itemName);
                Assert.Null(item, string.Format("Did not expect to find menu item '{0}' under menu '{1}'", itemName, menuName));
            });
        }

        /// <summary>
        /// May return null
        /// </summary>
        private ToolStripItem GetMenuItem(string menu, string item)
        {
            ToolStripMenuItem menuItem = null;
            foreach (ToolStripMenuItem m in MainWindowWrapper.MainMenuBar.Items)
            {
                if (m.Text.Replace("&", "") == menu)
                {
                    menuItem = m;
                    break;
                }
            }

            if (menuItem == null)
                return null;

            foreach (ToolStripItem i in menuItem.DropDownItems)
            {
                if (i.Text.Replace("&", "") == item)
                    return i;
            }
            return null;
        }

        /// <summary>
        /// Selects the specified IXenObjects or GroupingTags in the tree.
        /// </summary>
        /// <param name="ixmos">The IXenObjects or GroupingTags to select.</param>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool SelectInTree(params object[] ixmos)
        {
            return SelectInTree(vm => true, ixmos);
        }

        /// <summary>
        /// Selects the specified IXenObjects or GroupingTags in the tree.
        /// </summary>
        /// <param name="match">The condition that the node must match.</param>
        /// <param name="ixmos">The IXenObjects or GroupingTags to select.</param>
        /// <returns>A value indicating whether the action was successful.</returns>
        protected bool SelectInTree(Predicate<VirtualTreeNode> match, params object[] ixmos)
        {
            return MWWaitFor(() =>
                {
                    List<VirtualTreeNode> nodes = new List<VirtualTreeNode>();
                    foreach (object x in ixmos)
                    {
                        VirtualTreeNode node = FindInTree(x, match);
                        if (node == null)
                            return false;
                        nodes.Add(node);
                    }

                    nodes.ForEach(v => v.EnsureVisible());
                    MainWindowWrapper.TreeView.SelectedNodes.SetContents(nodes);
                    return true;
                });
        }

        /// <summary>
        /// Finds the first (there maybe more than one) node that has specified IXenObject or GroupingTag as its tag. 
        /// Returns null if the object isn't found.
        /// </summary>
        /// <param name="ixmo">The IXenObject or GroupingTag to be found.</param>
        /// <returns>The node.</returns>
        protected VirtualTreeNode FindInTree(object ixmo)
        {
            return FindInTree(ixmo, v => true);
        }

        /// <summary>
        /// Finds the first (there maybe more than one) node that has specified IXenObject or GroupingTag as its tag
        /// that matches the specified condition. Returns null if the object isn't found.
        /// </summary>
        /// <param name="ixmo">The IXenObject or GroupingTag to be found.</param>
        /// <param name="node">The condition that the node must match.</param>
        /// <returns>The node.</returns>
        protected VirtualTreeNode FindInTree(object ixmo, Predicate<VirtualTreeNode> match)
        {
            foreach (VirtualTreeNode node in MainWindowWrapper.TreeView.AllNodes)
            {
                bool found = false;

                if (ixmo == null && node.Tag == null)
                {
                    found = true;
                }
                else
                {
                    Folder f = ixmo as Folder;
                    Folder ff = node.Tag as Folder;

                    // special case for folders as Folder.Equals is broken...

                    if (f != null && ff != null && f.opaque_ref == ff.opaque_ref)
                    {
                        found = true;
                    }
                    else if (ixmo != null && ixmo.Equals(node.Tag))
                    {
                        found = true;
                    }
                }

                if (found && match(node))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Puts the main treeview into Infrastructure, Objects or Organization
        /// view and waits until the tree has been repopulated.
        /// </summary>
        protected void PutInNavigationMode(NavigationPane.NavigationMode mode)
        {
            MW(() =>
                {
                    var originalMode = GetNavigationMode();

                    switch (mode)
                    {
                        case NavigationPane.NavigationMode.Infrastructure:
                            TestUtils.GetToolStripItem(MainWindowWrapper.Item, "navigationPane.buttonInfraBig").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.Objects:
                            TestUtils.GetToolStripItem(MainWindowWrapper.Item, "navigationPane.buttonObjectsBig").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.Tags:
                            TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item, "navigationPane.toolStripMenuItemTags").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.Folders:
                            TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item, "navigationPane.toolStripMenuItemFolders").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.CustomFields:
                            TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item, "navigationPane.toolStripMenuItemFields").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.vApps:
                            TestUtils.GetToolStripMenuItem(MainWindowWrapper.Item, "navigationPane.toolStripMenuItemVapps").PerformClick();
                            break;
                        case NavigationPane.NavigationMode.SavedSearch:
                            var item = (ToolStripDropDownButton)TestUtils.GetToolStripItem(MainWindowWrapper.Item, "navigationPane.buttonSearchesBig");
                            item.ShowDropDown();
                            WaitFor(() => item.DropDownItems.Count > 0);
                            item.DropDownItems[0].PerformClick();
                            break;
                        case NavigationPane.NavigationMode.Notifications:
                            break;
                    }

                    if (originalMode == mode)
                        MainWindowWrapper.Item.RequestRefreshTreeView();
                });

            MWWaitFor(() => GetNavigationMode() == mode);
        }

        private NavigationPane.NavigationMode GetNavigationMode()
        {
            switch (MainWindowWrapper.TreeView.Nodes[0].Text)
            {
                case "Objects by Type":
                    return NavigationPane.NavigationMode.Objects;
                case "Tags":
                    return NavigationPane.NavigationMode.Tags;
                case "Folders":
                    return NavigationPane.NavigationMode.Folders;
                case "Custom Fields":
                    return NavigationPane.NavigationMode.CustomFields;
                case "vApps":
                    return NavigationPane.NavigationMode.vApps;
                case Branding.BRAND_CONSOLE:
                    return NavigationPane.NavigationMode.Infrastructure;
                default:
                    return NavigationPane.NavigationMode.SavedSearch;
            }
        }

        protected void ApplyTreeSearch(string text)
        {
            MW(() =>
                {
                    var textbox = TestUtils.GetSearchTextBox(MainWindowWrapper.Item, "navigationPane.navigationView.searchTextBox");
                    textbox.Text = text;
                });
            MW(MainWindowWrapper.Item.RequestRefreshTreeView);
        }

        protected void GoToTabPage(TabPage tabPage)
        {
            MW(() => MainWindowWrapper.TheTabControl.SelectedTab = tabPage);
        }

        internal MainWindowWrapper MainWindowWrapper
        {
            get
            {
                return new MainWindowWrapper(Program.MainWindow);
            }
        }

        /// <summary>
        /// Handles a modal dialog. Opens the specified modal dialog using openDialog. A background thread is then used to wait
        /// for the window to appear. It is then closed using closeDialog.
        /// </summary>
        /// <typeparam name="TDialogWrapper">The type of the dialog wrapper.</typeparam>
        /// <param name="windowText">The window text used to detect when the window appears.</param>
        /// <param name="openDialog">Used to open the dialog.</param>
        /// <param name="closeDialog">Used to close the dialog.</param>
        public void HandleModalDialog<TDialogWrapper>(string windowText, MethodInvoker openDialog, Action<TDialogWrapper> closeDialog)
        {
            Util.ThrowIfParameterNull(openDialog, "openDialog");
            Util.ThrowIfParameterNull(closeDialog, "closeDialog");

            Type wrappedItemType = typeof(TDialogWrapper).GetProperty("Item").PropertyType;
            bool closed = false;

            Predicate<Win32Window> match = delegate(Win32Window w)
            {
                Control ctrl = Control.FromHandle(w.Handle);
                return ctrl != null && wrappedItemType.IsAssignableFrom(ctrl.GetType());
            };

            ThreadPool.QueueUserWorkItem(delegate
            {
                TDialogWrapper dialog = (TDialogWrapper)Activator.CreateInstance(typeof(TDialogWrapper), WaitForWindowToAppear(windowText, match));

                MW(() => closeDialog(dialog));
                closed = true;
            });

            MW(openDialog);
            MWWaitFor(() => closed, "Dialog \"" + windowText + "\" was not closed.");
        }

        /// <summary>
        /// Handles a modeless dialog. Opens a dialog of the specified wrapper type with the specified window text using the
        /// openDialog delegate. It then closes it with the closeDialog delegate.
        /// </summary>
        /// <typeparam name="TDialogWrapper">The type of the dialog to be opened.</typeparam>
        /// <param name="windowText">The window text.</param>
        /// <param name="openDialog">The delegate to open the dialog.</param>
        /// <param name="closeDialog">The delegate to close the dialog.</param>
        public void HandleModelessDialog<TDialogWrapper>(string windowText, MethodInvoker openDialog, Action<TDialogWrapper> closeDialog)
        {
            MW(openDialog);

            Type wrappedItemType = typeof(TDialogWrapper).GetProperty("Item").PropertyType;

            Predicate<Win32Window> match = w =>
            {
                var ctrl = Control.FromHandle(w.Handle);
                return ctrl != null && wrappedItemType.IsAssignableFrom(ctrl.GetType());
            };

            var dialog = (TDialogWrapper)Activator.CreateInstance(typeof(TDialogWrapper), WaitForWindowToAppear(windowText, match));

            MW(() => closeDialog(dialog));
        }

        public override List<IXenConnection> ConnectionManager
        {
            get { return ConnectionsManager.XenConnections; }
        }

        public override List<IXenConnection> ConnectionManagerCopy
        {
            get { return ConnectionsManager.XenConnectionsCopy; }
        }
    }
}
