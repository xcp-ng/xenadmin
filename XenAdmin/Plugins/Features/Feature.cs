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
using System.Drawing;
using System.IO;
using XenAdmin.XenSearch;
using System.Xml;
using System.Resources;
using XenAdmin.Core;


namespace XenAdmin.Plugins
{
    /// <summary>
    /// Abstract base class for a ui addition added via a plugin
    /// </summary>
    public abstract class Feature : IDisposable
    {
        private readonly ResourceManager _resourceManager;
        protected readonly XmlNode node;

        protected PluginDescriptor PluginDescriptor { get; }

        /// <summary>
        /// (Mandatory) "name" attribute on the feature's tag
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// (Optional) "label" attribute, used as a display string instead of 'name' if set.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// (Optional) $feature.description in resources file
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// (Optional) $feature.tooltip in resources file
        /// </summary>
        public string Tooltip { get; }

        /// <summary>
        /// (Optional) $feature.icon in resources file
        /// </summary>
        public Image Icon { get; }

        /// <summary>
        /// (Optional) "search" attribute, specifies a search defined in the config file to set enablement
        /// </summary>
        protected Search Search { get; }

        private const string ATT_NAME = "name";
        private const string ATT_LABEL = "label";
        private const string ATT_DESCRIPTION = "description";
        private const string ATT_TOOLTIP = "tooltip";
        private const string ATT_ICON = "icon";
        private const string ATT_SEARCH = "search";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for localization</param>
        /// <param name="node">The XML node from the plugin config file</param>
        /// <param name="pluginDescriptor">The plugin descriptor.</param>
        protected Feature(ResourceManager resourceManager, XmlNode node, PluginDescriptor pluginDescriptor)
        {
            Util.ThrowIfParameterNull(node, "node");
            Util.ThrowIfParameterNull(pluginDescriptor, "pluginDescriptor");

            this.node = node;
            _resourceManager = resourceManager;
            PluginDescriptor = pluginDescriptor;

            Name = Helpers.GetStringXmlAttribute(node, ATT_NAME, "");
            Label = Helpers.GetStringXmlAttribute(node, ATT_LABEL, "");
            Description = Helpers.GetStringXmlAttribute(node, ATT_DESCRIPTION, "");
            Tooltip = Helpers.GetStringXmlAttribute(node, ATT_TOOLTIP, "");
            Icon = LoadIcon(Helpers.GetStringXmlAttribute(node, ATT_ICON, ""));
            Search = PluginDescriptor.GetSearch(Helpers.GetStringXmlAttribute(node, ATT_SEARCH, ""));

            // localize
            if (_resourceManager != null)
            {
                string label = _resourceManager.GetString($"{Name}.{ATT_LABEL}");
                if (!string.IsNullOrEmpty(label))
                    Label = label;

                string description = _resourceManager.GetString($"{Name}.{ATT_DESCRIPTION}");
                if (!string.IsNullOrEmpty(description))
                    Description = description;

                string tooltip = _resourceManager.GetString($"{Name}.{ATT_TOOLTIP}");
                if (!string.IsNullOrEmpty(tooltip))
                    Tooltip = tooltip;

                string icon = _resourceManager.GetString($"{Name}.{ATT_ICON}");
                if (!string.IsNullOrEmpty(icon))
                    Icon = LoadIcon(icon);
            }
        }

        public virtual string CheckForError()
        {
            return Name == "" ? string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, ATT_NAME) : null;
        }

        private static Image LoadIcon(string path)
        {
            return string.IsNullOrEmpty(path) ? null : Image.FromFile(Path.Combine(Program.AssemblyDir, path));
        }

        public virtual void Initialize()
        {
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Label) ? Name : Label;
        }

        public string PluginName => PluginDescriptor.Name;

        public RbacMethodList GetMethodList(string name)
        {
            return PluginDescriptor.GetMethodList(name);
        }

        public bool Enabled => PluginDescriptor.Enabled;

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
