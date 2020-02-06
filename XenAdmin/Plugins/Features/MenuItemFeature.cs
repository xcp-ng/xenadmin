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
using System.Xml;
using XenAdmin.Core;
using System.Resources;
using XenAdmin.Commands;


namespace XenAdmin.Plugins
{
    /// <summary>
    /// Class to describe a menu item which is added to XenCenter via a plugin and is used to launch a plugin command.
    /// </summary>
    internal class MenuItemFeature : Feature
    {
        public readonly ParentMenuItemFeature ParentFeature;

        /// <summary>
        /// (Mandatory) "menu" attribute on the "MenuItem" tag
        /// </summary>
        public readonly PluginMenu Menu;

        /// <summary>
        /// (Optional) "serialized" attribute on the "MenuItem" tag
        /// </summary>
        public readonly PluginSerializationLevel Serialized;

        /// <summary>
        /// (Mandatory) A child tag specifying a command - either "Shell", "PowerShell" or "XenServerPowerShell"
        /// </summary>
        public readonly ShellCmd ShellCmd;

        /// <summary>
        /// (Optional) "contextmenu"; the context menu you would like this item to appear under
        /// </summary>
        public readonly PluginContextMenu ContextMenu;

        public const string ELEMENT_NAME = "MenuItem";
        public const string TYPE_SHELL = "Shell";
        public const string TYPE_POWERSHELL = "PowerShell";
        public const string TYPE_XENSERVER_POWERSHELL = "XenServerPowerShell";

        public const string ATT_MENU = "menu";
        public const string ATT_CONTEXT_MENU = "contextmenu";
        public const string ATT_SERIALIZED = "serialized";
        public const string ATT_PARAM = "param";


        public MenuItemFeature(ResourceManager resourceManager, XmlNode node, PluginDescriptor plugin, ParentMenuItemFeature parentFeature = null)
            : base(resourceManager, node, plugin)
        {
            ParentFeature = parentFeature;
            Menu = Helpers.GetEnumXmlAttribute(node, ATT_MENU, PluginMenu.none);
            ContextMenu = Helpers.GetEnumXmlAttribute(node, ATT_CONTEXT_MENU, GetContextMenuFromMenu(Menu));
            Serialized = Helpers.GetEnumXmlAttribute(node, ATT_SERIALIZED, PluginSerializationLevel.none);

            foreach (XmlNode child in node.ChildNodes)
            {   
                switch (child.Name)
                {
                    case TYPE_SHELL:
                        ShellCmd = new ShellCmd(child, paramsFromXML(child));
                        break;
                    case TYPE_POWERSHELL:
                        ShellCmd = new PowerShellCmd(child, paramsFromXML(child));
                        break;
                    case TYPE_XENSERVER_POWERSHELL:
                        ShellCmd = new XenServerPowershellCmd(child, paramsFromXML(child));
                        break;
                }
                return;
            }
        }

        public override string CheckForError()
        {
            if (Menu == PluginMenu.none)
                return string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, ATT_MENU);

            if (node.ChildNodes.Count != 1)
                return Messages.PLUGINS_MENU_ITEMS_ONLY_ONE_CHILD_ALLOWED;
            
            foreach (XmlNode child in node.ChildNodes)
            {
                if (!(child.Name == TYPE_SHELL 
                    || child.Name == TYPE_POWERSHELL
                    || child.Name == TYPE_XENSERVER_POWERSHELL))
                {
                    return string.Format(Messages.PLUGINS_UNRECOGNISED_XML_NODE, child.Name, node.Name);
                }
            }

            return ShellCmd.CheckForError() ?? base.CheckForError();
        } 

        private static List<string> paramsFromXML(XmlNode CmdNode)
        {
            string paramList = CmdNode.Attributes[ATT_PARAM] != null ? CmdNode.Attributes[ATT_PARAM].Value : "";
            List<string> p = new List<string>(paramList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < p.Count; i++)
            {
                p[i] = p[i].Trim();
            }

            return p;
        }

        public MenuItemFeatureCommand GetCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
        {
            return new MenuItemFeatureCommand(mainWindow, selection, this, Search, Serialized);
        }

        public static PluginContextMenu GetContextMenuFromMenu(PluginMenu menu)
        {
            switch (menu)
            {
                case PluginMenu.pool:
                    return PluginContextMenu.pool;
                case PluginMenu.server:
                    return PluginContextMenu.server;
                case PluginMenu.storage:
                    return PluginContextMenu.storage;
                case PluginMenu.templates:
                    return PluginContextMenu.template;
                case PluginMenu.vm:
                    return PluginContextMenu.vm;
                default:
                    return PluginContextMenu.none;
            }
        }

    }
}
