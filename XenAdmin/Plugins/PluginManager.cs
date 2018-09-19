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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using XenAPI;
using XenCenterLib;
using XenAdmin.Core;

namespace XenAdmin.Plugins
{
    public class PluginManager : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<PluginDescriptor> _plugins = new List<PluginDescriptor>();
        private static readonly string PLUGIN_FOLDER = Path.Combine(Program.AssemblyDir, "Plugins");

        private const string ROOT_ELEMENT_NAME = "XenCenterPlugin";

        public event Action PluginsChanged;

        /// <summary>
        /// Gets the plugins loaded from the Plugin directory.
        /// </summary>
        /// <value>The plugins.</value>
        public ReadOnlyCollection<PluginDescriptor> Plugins
        {
            get
            {
                return new ReadOnlyCollection<PluginDescriptor>(_plugins);
            }
        }

        /// <summary>
        /// Gets a value indicating whether plugins functionality is enabled. This is driven by whether the Plugins folder exists.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return !Registry.DisablePlugins; }
        }

        /// <summary>
        /// Gets a value indicating the number of plugins the user has enabled (ticked on the GUI)
        /// </summary>
        public int EnabledPluginsCount
        {
            get { return (from PluginDescriptor plugin in _plugins where plugin.Enabled select plugin).Count(); }
        }

        public void OnPluginsChanged()
        {
            if (PluginsChanged != null)
                PluginsChanged();
        }

        /// <summary>
        /// Loads the plugins from the default Plugins folder.
        /// </summary>
        public void LoadPlugins()
        {
            LoadPlugins(PLUGIN_FOLDER);
        }

        /// <summary>
        /// Loads the plugins from the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public void LoadPlugins(string folder)
        {
            if (Enabled)
            {
                // catch all exceptions, the most likely cause of errors is user mistakes with plugin creation.
                // we must not let this kill XenCenter
                try
                {
                    //CA-71469: check whether the plugins folder exists
                    if (!Directory.Exists(folder))
                    {
                        log.InfoFormat("Plugin directory {0} was not found.", folder);
                        return;
                    }

                    // Look for plugins in {Application Working Dir}\Plugins
                    foreach (string vendor in Directory.GetDirectories(folder))
                    {
                        foreach (string pluginDir in Directory.GetDirectories(vendor))
                        {
                            try
                            {
                                string plugin = Path.GetFileName(pluginDir);
                                string org = Path.GetFileName(vendor);

                                ProcessPlugin(Path.Combine(pluginDir, string.Format("{0}.xcplugin.xml", plugin)), plugin, org);
                            }
                            catch (Exception e)
                            {
                                log.Error(string.Format("Failed to load plugin xml for file {0}.xcplugin.xml", Path.GetFileName(pluginDir)), e);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error loading plugins.", ex);
                }
                
                OnPluginsChanged();
            }
        }

        private void ProcessPlugin(string file, string plugin, string org)
        {
            XmlDocument pluginXml = new XmlDocument();
            pluginXml.XmlResolver = new BasicXMLResolver();

            using (StreamReader sr = new StreamReader(file))
            {
                pluginXml.LoadXml(sr.ReadToEnd());
            }

            foreach (XmlNode node in pluginXml.GetElementsByTagName(ROOT_ELEMENT_NAME))
            {
                _plugins.Add(new PluginDescriptor(node, plugin, org));
            }
        }

        private void ClearPlugins()
        {
            foreach (PluginDescriptor plugin in _plugins)
            {
                plugin.Dispose();
            }
            _plugins.Clear();

            OnPluginsChanged();
        }

        public void ReloadPlugins()
        {
            ClearPlugins();
            LoadPlugins();
        }

        public void DisposeURLs(IXenObject xenObject)
        {
            foreach (PluginDescriptor plugin in _plugins)
            {
                plugin.DisposeURLs(xenObject);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPlugins();
            }
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
