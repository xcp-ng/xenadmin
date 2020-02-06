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
using XenAdmin.Core;
using XenAdmin.Plugins;

namespace XenAdmin.Commands
{
    /// <summary>
    /// A List for toolstrip menu items. It contains useful methods for adding Commands
    /// and <see cref="CommandToolStripMenuItem"/>s.
    /// </summary>
    internal class ContextMenuItemCollection : List<ToolStripItem>
    {
        private readonly PluginManager _pluginManager;
        private readonly IMainWindow _mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuItemCollection"/> class.
        /// This constructor can not be used it this collection is required to contains plug-in menu items.
        /// </summary>
        public ContextMenuItemCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuItemCollection"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window required for adding plugin-menu items.</param>
        /// <param name="pluginManager">The plugin manager required for adding plugin-menu items.</param>
        public ContextMenuItemCollection(IMainWindow mainWindow, PluginManager pluginManager)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            _mainWindow = mainWindow;

            Util.ThrowIfParameterNull(mainWindow, "pluginManager");
            _pluginManager = pluginManager;
        }

        public void RemoveInvalidSeparators()
        {
            // remove leading separators
            while (Count > 0)
            {
                if (this[0] is ToolStripSeparator)
                {
                    RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            // remove trailing separators
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i] is ToolStripSeparator)
                {
                    RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }

        public void AddPluginItems(PluginContextMenu contextMenu, SelectedItemCollection selection)
        {
            if (_pluginManager == null)
            {
                throw new InvalidOperationException("No plugin manager was specified.");
            }
            
            bool addedSeparatorBefore = false;

            foreach (PluginDescriptor plugin in _pluginManager.Plugins)
            {
                if (plugin.Enabled)
                {
                    foreach (Feature feature in plugin.Features)
                    {
                        MenuItemFeature menuItemFeature = feature as MenuItemFeature;

                        if (menuItemFeature != null && menuItemFeature.ContextMenu == contextMenu)
                        {
                            if (!addedSeparatorBefore)
                            {
                                AddSeparator();
                                addedSeparatorBefore = true;
                            }

                            Add(menuItemFeature.GetCommand(_mainWindow, selection));
                        }

                        ParentMenuItemFeature parentMenuItemFeature = feature as ParentMenuItemFeature;

                        if (parentMenuItemFeature != null && parentMenuItemFeature.ContextMenu == contextMenu)
                        {
                            CommandToolStripMenuItem parentItem = new CommandToolStripMenuItem(parentMenuItemFeature.GetCommand(_mainWindow, selection));

                            foreach (MenuItemFeature f in parentMenuItemFeature.Features)
                            {
                                parentItem.DropDownItems.Add(new CommandToolStripMenuItem(f.GetCommand(_mainWindow, selection)));
                            }

                            if (!addedSeparatorBefore)
                            {
                                AddSeparator();
                                addedSeparatorBefore = true;
                            }

                            Add(parentItem);
                        }
                    }
                }
            }

            if (addedSeparatorBefore)
            {
                AddSeparator();
            }
        }

        public void AddIf(Command command, Func<bool> condition)
        {
            if (condition())
            {
                AddIfEnabled(command);
            }
        }

        public void AddIfEnabled(ToolStripMenuItem item)
        {
            if (item.Enabled)
            {
                Add(item);
            }
            else
            {
                item.Dispose();
            }
        }

        public void AddIfEnabled(Command command, bool bold = false)
        {
            if (command.CanExecute())
            {
                Add(command, bold);
            }
        }

        public void Add(Command command, bool bold = false)
        {
            CommandToolStripMenuItem item = new CommandToolStripMenuItem(command, true);

            if (bold)
            {
                item.Font = Program.DefaultFontBold;
            }

            Add(item);
        }

        public void AddSeparator()
        {
            if (Count > 0 && !(this[Count - 1] is ToolStripSeparator))
            {
                Add(new ToolStripSeparator());
            }
        }
    }
}
