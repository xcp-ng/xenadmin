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
using NUnit.Framework;
using XenAdmin.Plugins;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class TabPageFeatureTests
    {
        private PluginManager _pluginManager;
        private TestPluginLoader _pluginLoader;

        [TearDown]
        public void TearDown()
        {
            _pluginLoader.Dispose();
            _pluginManager.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _pluginManager = new PluginManager();
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void TestLoadPlugin()
        {
            _pluginLoader = new TestPluginLoader("TabPageFeatureTestPlugin", _pluginManager);
            _pluginLoader.Load();

            Assert.AreEqual(1, _pluginManager.Plugins.Count, "No plugin loaded.");
            Assert.AreEqual(8, _pluginManager.Plugins[0].Features.Count, "No features loaded.");
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("pl"));
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("svr"));
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("vm"));
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("sr"));
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("ut"));
            Assert.IsNotNull(_pluginManager.Plugins[0].GetSearch("dt"));
        }


        //[Test]
        //public void Test_UrlReplacement_DefaultTemplate()
        //{
        //    MW(delegate()
        //    {
        //        SelectInTree(GetAnyDefaultTemplate());
        //        Helpers.ParseURL(GetAllProperties());
        //    });
        //}


        //private string GetAllProperties()
        //{
        //    StringBuilder builder = new StringBuilder("http://");
        //    foreach (string prop in Enum.GetNames(typeof(PropertyNames)))
        //    {
        //        builder.AppendFormat("{{${0}}}", prop);
        //    }
        //    return builder.ToString();
        //}
    }
}
