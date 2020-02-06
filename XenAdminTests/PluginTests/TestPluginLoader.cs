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
using System.IO;
using NUnit.Framework;
using XenAdmin.Plugins;


namespace XenAdminTests.PluginTests
{
    public abstract class TestPluginLoader
    {
        protected string _allPluginsFolder;
        protected PluginManager _pluginManager;

        protected abstract string pluginName { get; }

        [TearDown]
        public void TearDown()
        {
            _pluginManager.Dispose();

            try
            {
                Directory.Delete(_allPluginsFolder, true);
            }
            catch
            {
                //ignore
            }
        }

        [SetUp]
        public virtual void Setup()
        {
            Assert.That(pluginName, Is.Not.Null.And.Not.Empty, "Either set pluginName or override this Setup");

           _pluginManager = new PluginManager();

            _allPluginsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var pluginFolder = Path.Combine(_allPluginsFolder, "plugin-vendor", pluginName);
            var pluginFile = Path.Combine(pluginFolder, $"{pluginName}.xcplugin.xml");

            Directory.CreateDirectory(pluginFolder);

            var pluginSource = TestUtils.GetTestResource($"PluginResources\\{pluginName}.xcplugin.xml");

            using (var fs = new FileStream(pluginSource, FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                var content = sr.ReadToEnd();
                File.WriteAllText(pluginFile, content);
            }

            _pluginManager.LoadPlugins(_allPluginsFolder);
        }
    }
}
