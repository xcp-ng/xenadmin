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
using XenAdmin.Core;
using System.IO;
using System.Threading;
using System.Net;

namespace XenAdminTests.MiscTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
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
                File.WriteAllText(file, "<html>hello</html>");
                _testUris.Add(new Uri("file://" + file));
            }
        }

        [Test]
        public void TestMultipleUrisWithOneValid()
        {
            Uri uri = _testUris[0];
            Uri uri2 = new Uri("http://fgdfgd.dfgdfgd.dfg");
            bool navigating = false;
            bool navigated = false;

            _wb.Navigating += (s, e) =>
            {
                Assert.AreEqual(uri, e.Url, "Incorrect Uri in Navigation");
                Assert.IsFalse(navigating);
                navigating = true;
            };

            _wb.Navigated += (s, e) =>
            {
                Assert.AreEqual(uri, e.Url, "Incorrect Uri in Navigation");
                Assert.IsFalse(navigated);
                navigated = true;
            };

            _wb.NavigateError += (s, e) => Assert.Fail("Navigation failed.");

            MW(() => _wb.Navigate(new[] { uri, uri2 }));

            MWWaitFor(() => navigating && navigated, "Navigation didn't take place.");
        }

        [Test]
        public void TestMultipleUrisWithNoneValid()
        {
            Uri uri = new Uri("http://fgdfffgd.dfgdfgd.dfg");
            Uri uri2 = new Uri("http://fgdfgd.dfgdfgd.dfg");
            Uri navCancelUri = new Uri("res://ieframe.dll/navcancl.htm#http://fgdfgd.dfgdfgd.dfg/");
            bool navigating = false;
            bool navigated = false;
            bool navError = false;

            _wb.Navigating += (s, e) =>
            {
                if (e.Url != navCancelUri)
                {
                    Assert.IsFalse(navigating);
                }
                else
                {
                    Assert.IsTrue(navigating);
                }
                navigating = true;
            };

            _wb.Navigated += (s, e) =>
            {
                if (e.Url != navCancelUri)
                {
                    Assert.IsFalse(navigated);
                }
                else
                {
                    Assert.IsTrue(navigated);
                }
                navigated = true;
            };

            _wb.NavigateError += (s, e) =>
            {
                Assert.IsFalse(navError);
                navError = true;
            };

            MW(() => _wb.Navigate(new[] { uri, uri2 }));

            MWWaitFor(() => navigating && navigated && navError, "Navigation didn't take place.");
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

            MW(() => _wb.Navigate(_testUris));

            MWWaitFor(() => navigating && navigated, "Navigation didn't take place.");
        }
    }
}
