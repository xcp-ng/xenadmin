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
using NUnit.Framework;
using XenAdmin.Core;
using System.IO;


namespace XenAdminTests.MiscTests
{
    [TestFixture, Apartment(System.Threading.ApartmentState.STA)]
    public class WebBrowser2Tests : MainWindowLauncher_TestFixture
    {
        private WebBrowser2 _wb;
        private readonly List<Uri> _testUris = new List<Uri>();

        [TearDown]
        public new void TearDown()
        {
            MW(_wb.Dispose);
            foreach (Uri uri in _testUris)
            {
                if (File.Exists(uri.LocalPath))
                {
                    try
                    {
                        File.Delete(uri.LocalPath);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        [SetUp]
        public new void SetUp()
        {
            _wb = new WebBrowser2();

            MW(() => _wb.Handle);

            _testUris.Clear();

            for (int i = 0; i < 2; i++)
            {
                string file = Path.GetTempFileName();
                File.Move(file, file + ".html");
                file += ".html";
                File.WriteAllText(file, "<html><body>hello</body></html>");
                _testUris.Add(new Uri("file://" + file));
            }
        }

        [Test]
        public void TestMultipleUrisWithOneValid()
        {
            Uri uri1 = _testUris[0];
            Uri uri2 = new Uri("http://fgdfgd.dfgdfgd.dfg");
            bool navigating = false;
            bool navigated = false;

            _wb.Navigating += (s, e) =>
            {
                Assert.AreEqual(uri1, e.Url, "Incorrect Uri in Navigation");
                Assert.IsFalse(navigating);
                navigating = true;
            };

            _wb.Navigated += (s, e) =>
            {
                Assert.AreEqual(uri1, e.Url, "Incorrect Uri in Navigation");
                Assert.IsFalse(navigated);
                navigated = true;
            };

            _wb.NavigateError += (s, e) => Assert.Fail("Navigation failed.");

            var uris = new[] {uri1, uri2};
            foreach (var uri in uris)
            {
                var curUri = uri;
                MW(() => _wb.Navigate(curUri));
                MWWaitFor(() => navigating && navigated, "Navigation didn't take place.");
            }
        }

        [Test]
        public void TestMultipleUrisWithNoneValid()
        {
            Uri curUri = null;
            bool navigating = false;
            bool navigated = false;
            bool navError = false;

            _wb.Navigating += (s, e) =>
            {
                if (e.Url == curUri)
                    navigating = true;
            };

            _wb.Navigated += (s, e) =>
            {
                if (e.Url == curUri)
                    navigated = true;
            };

            _wb.NavigateError += (s, e) =>
            {
                Assert.IsFalse(navError);
                navError = true;
            };

            //Use non-existing URLs with existing rather than completely fictional
            //domains, otherwise the NavigationError even is not fired

            var uris = new[]
            {
                new Uri("http://127.0.0.1/blah"),
                new Uri("http://127.0.0.1/blahblah")
            };

            foreach (var uri in uris)
            {
                curUri = uri;

                navigating = navigated = navError = false;
                MW(() => _wb.Navigate(curUri));
                MWWaitFor(() => navigating && navigated && navError, "Navigation didn't take place.");
            } 
        }

        [Test]
        public void TestMultipleUrisWithAllValid()
        {
            bool navigating = false;
            bool navigated = false;

            _wb.Navigating += (s, e) =>
            {
                Assert.IsFalse(navigating);
                navigating = true;
            };

            _wb.Navigated += (s, e) =>
            {
                Assert.IsFalse(navigated);
                navigated = true;
            };

            _wb.NavigateError += (s, e) => Assert.Fail("Navigation failed.");

            foreach (var uri in _testUris)
            {
                var curUri = uri;
                MW(() => _wb.Navigate(curUri));
                MWWaitFor(() => navigating && navigated, "Navigation didn't take place.");
            }
        }
    }
}
