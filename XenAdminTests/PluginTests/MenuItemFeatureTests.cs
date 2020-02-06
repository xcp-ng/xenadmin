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

using System.Threading;
using NUnit.Framework;
using XenAdmin.Plugins;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class MenuItemFeatureTests : TestPluginLoader
    {
        protected override string pluginName => "MenuItemFeatureTestPlugin";

        [Test, Apartment(ApartmentState.STA)]
        public void TestLoadPlugin()
        {
            Assert.AreEqual(1, _pluginManager.Plugins.Count, "No plugins loaded.");
            Assert.AreEqual(pluginName, _pluginManager.Plugins[0].Name, "Plugin Name incorrect");
            Assert.AreEqual("plugin-vendor", _pluginManager.Plugins[0].Organization, "Plugin Vendor incorrect");
            Assert.IsTrue(_pluginManager.Enabled, "Plugin manager isn't enabled");
            Assert.IsNull(_pluginManager.Plugins[0].Error, "Error loading plugin manager");
        }

        [Test, Apartment(ApartmentState.STA)]
        public void TestFirstFeatureDetails()
        {
            _pluginManager.Plugins[0].Enabled = true;

            Assert.AreEqual(1, _pluginManager.Plugins.Count, "plugin wasn't loaded");
            Assert.AreEqual(10, _pluginManager.Plugins[0].Features.Count, "features weren't loaded");
            Assert.IsTrue(_pluginManager.Plugins[0].Enabled);

            MenuItemFeature menuItemFeature = (MenuItemFeature)_pluginManager.Plugins[0].Features[0];

            Assert.AreEqual(PluginMenu.file, menuItemFeature.Menu, "file_ShellTest1 plugin heading incorrect.");
            //Assert.IsTrue(menuItemFeature.Enabled, "file_ShellTest1 plugin not enabled.");
            Assert.AreEqual("file_ShellTest1", menuItemFeature.Name, "file_ShellTest1 plugin name incorrect.");
            Assert.AreEqual("the label", menuItemFeature.Label, "file_ShellTest1 plugin label incorrect.");
            Assert.AreEqual("the description", menuItemFeature.Description, "file_ShellTest1 plugin description incorrect.");
            Assert.AreEqual("the tooltip", menuItemFeature.Tooltip, "file_ShellTest1 plugin tooltip incorrect.");
            Assert.AreEqual(pluginName, menuItemFeature.PluginName, "file_ShellTest1 plugin name was incorrect.");
            Assert.IsNull(menuItemFeature.ParentFeature, "file_ShellTest1 shouldn't have parent feature");
            Assert.IsInstanceOf<ShellCmd>(menuItemFeature.ShellCmd, "file_ShellTest1 should have shell cmd.");
        }

        [Test, Apartment(ApartmentState.STA)]
        public void TestHeadings()
        {
            _pluginManager.Plugins[0].Enabled = true;

            Assert.AreEqual(PluginMenu.file, ((MenuItemFeature)_pluginManager.Plugins[0].Features[0]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.view, ((MenuItemFeature)_pluginManager.Plugins[0].Features[1]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.pool, ((MenuItemFeature)_pluginManager.Plugins[0].Features[2]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.server, ((MenuItemFeature)_pluginManager.Plugins[0].Features[3]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.vm, ((MenuItemFeature)_pluginManager.Plugins[0].Features[4]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.storage, ((MenuItemFeature)_pluginManager.Plugins[0].Features[5]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.templates, ((MenuItemFeature)_pluginManager.Plugins[0].Features[6]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.tools, ((MenuItemFeature)_pluginManager.Plugins[0].Features[7]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.window, ((MenuItemFeature)_pluginManager.Plugins[0].Features[8]).Menu, "file_ShellTest1 plugin heading incorrect.");
            Assert.AreEqual(PluginMenu.help, ((MenuItemFeature)_pluginManager.Plugins[0].Features[9]).Menu, "file_ShellTest1 plugin heading incorrect.");
        }
    }
}
