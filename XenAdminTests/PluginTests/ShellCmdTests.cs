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
using NUnit.Framework;
using XenAdmin.Plugins;
using System.IO;
using System.Text;
using System.Threading;
using XenAdmin.Commands;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class ShellCmdTests : TestPluginLoader
    {
        private string _batchFile;
        private string _fileThatBatchFileWillWrite;

        protected override string pluginName => "ShellCmdTestPlugin";

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            string tempPath = Path.GetTempPath();
            _batchFile = Path.Combine(tempPath, $"{Guid.NewGuid().ToString()}.bat");
            _fileThatBatchFileWillWrite = Path.Combine(tempPath, Guid.NewGuid().ToString());

            string batchFileContents = $"echo %1 > \"{_fileThatBatchFileWillWrite}\"";
            File.WriteAllText(_batchFile, batchFileContents);
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            try
            {
                File.Delete(_batchFile);
                File.Delete(_fileThatBatchFileWillWrite);
            }
            catch
            {
                //ignore
            }
        }

        [SetUp]
        public override void Setup()
        {
            _pluginManager = new PluginManager();

            _allPluginsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var pluginFolder = Path.Combine(_allPluginsFolder, "plugin-vendor", pluginName);
            var pluginFile = Path.Combine(pluginFolder, $"{pluginName}.xcplugin.xml");

            Directory.CreateDirectory(pluginFolder);

            var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<XenCenterPlugin xmlns=\"http://www.citrix.com/XenCenter/Plugins/schema\" version=\"1\" plugin_name=\"TestPlugin\" plugin_version=\"1.0.0.0\">");
            sb.AppendLine("<MenuItem name=\"ShellCmdTestPlugin\" menu=\"file\" serialized=\"none\">");
            sb.AppendLine($"<Shell filename=\"{_batchFile}\" window=\"true\"/></MenuItem></XenCenterPlugin>");

            File.WriteAllText(pluginFile, sb.ToString());

            _pluginManager.LoadPlugins(_allPluginsFolder);
        }

        

        [Test, Apartment(ApartmentState.STA)]
        public void TestLoadPlugin()
        {
            Assert.IsTrue(File.Exists(_batchFile), "test batch file wasn't written");
            Assert.IsFalse(File.Exists(_fileThatBatchFileWillWrite), "test file shouldn't exist");

            Assert.AreEqual(1, _pluginManager.Plugins.Count, "No plugins loaded.");
            Assert.AreEqual("ShellCmdTestPlugin", _pluginManager.Plugins[0].Name, "Plugin Name incorrect");
            Assert.AreEqual("plugin-vendor", _pluginManager.Plugins[0].Organization, "Plugin Vendor incorrect");
            Assert.IsTrue(_pluginManager.Enabled, "Plugin manager isn't enabled");
            Assert.IsNull(_pluginManager.Plugins[0].Error, "Error when loading plugin manager");
            Assert.AreEqual(1, _pluginManager.Plugins[0].Features.Count, "MenuItem feature wasn't loaded");

            MenuItemFeature menuItemFeature = (MenuItemFeature)_pluginManager.Plugins[0].Features[0];

            Assert.IsNotNull(menuItemFeature.ShellCmd, "ShellCmd wasn't loaded");
            Assert.AreEqual(menuItemFeature.ShellCmd.Filename, _batchFile, "Batch file name wasn't read from plugin XML");

            // now execute the command and see if the test file is written by the batch file.
            menuItemFeature.GetCommand(new MockMainWindow(), new[] {new SelectedItem(null)}).Execute();

            int i = 0;
            bool completed = false;
            while (i < 300)
            {
                if (File.Exists(_fileThatBatchFileWillWrite))
                {
                    completed = true;
                    break;
                }

                Thread.Sleep(100);
                i++;
            }

            Assert.IsTrue(completed, "test didn't complete");
        }
    }
}
