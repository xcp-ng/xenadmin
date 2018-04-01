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
using XenAdmin.Controls;

namespace XenAdminTests.Controls
{
    public class DecentGroupBoxTests : UnitTester_TestFixture
    {
        private class DecentGroupBoxWrapper : DecentGroupBox
        {
            public string ManipulatedText
            {
                get { return EscapedText; }
            }
        }

        [Test, Description("Default case")]
        [TestCaseSource("TestCasesNoMnemonic")]
        public void TestTextManipulationNoMnemonic(TestCase testCase)
        {
            DecentGroupBoxWrapper gb = new DecentGroupBoxWrapper { Width = testCase.Width };
            gb.Text = testCase.Text;
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.That(gb.Text, Is.EqualTo(testCase.Text), "Raw text TestCase: " + testCase.Text);
            Assert.That(gb.ManipulatedText, Is.EqualTo(testCase.Expected), "Manipulated text TestCase: " + testCase.Text);
        }

        [Test]
        [TestCaseSource("TestCasesWithMnemonic")]
        public void TestTextManipulationWithMnemonic(TestCase testCase)
        {
            DecentGroupBoxWrapper gb = new DecentGroupBoxWrapper { Width = testCase.Width, UseMnemonic = true};
            gb.Text = testCase.Text;
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsTrue(gb.UseMnemonic, "use mnemonic setting");
            Assert.That(gb.Text, Is.EqualTo(testCase.Text), "Raw text TestCase: " + testCase.Text);
            Assert.That(gb.ManipulatedText, Is.EqualTo(testCase.Expected), "Manipulated text TestCase: " + testCase.Text);
        }

        [Test]
        [TestCaseSource("TestCasesWithMnemonicNoEllipsing")]
        public void TestTextManipulationWithMnemonicNoEllipsing(TestCase testCase)
        {
            DecentGroupBoxWrapper gb = new DecentGroupBoxWrapper { Width = testCase.Width, UseMnemonic = true, AutoEllipsis = false};
            gb.Text = testCase.Text;
            Assert.IsFalse(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsTrue(gb.UseMnemonic, "use mnemonic setting");
            Assert.That(gb.Text, Is.EqualTo(testCase.Text), "Raw text TestCase: " + testCase.Text);
            Assert.That(gb.ManipulatedText, Is.EqualTo(testCase.Expected), "Manipulated text TestCase: " + testCase.Text);
        }

        [Test]
        [TestCaseSource("TestCasesNoMnemonicNoEllipsing")]
        public void TestTextManipulationNoMnemonicNoAutoEllipse(TestCase testCase)
        {
            DecentGroupBoxWrapper gb = new DecentGroupBoxWrapper { Width = testCase.Width, UseMnemonic = false, AutoEllipsis = false};
            gb.Text = testCase.Text;
            Assert.IsFalse(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.That(gb.Text, Is.EqualTo(testCase.Text), "Raw text TestCase: " + testCase.Text);
            Assert.That(gb.ManipulatedText, Is.EqualTo(testCase.Expected), "Manipulated text TestCase: " + testCase.Text);
        }

        [Test]
        public void TextBoxWidthNearFudgeFactor([Values(10, 14, 15, 16, 18)] int width)
        {
            const string text = "SomeTextOrOther";
            DecentGroupBoxWrapper gb = new DecentGroupBoxWrapper { Width = width };
            gb.Text = text;
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.That(gb.Text, Is.EqualTo(text), "Raw text TestCase width: " + width);
            Assert.That(gb.ManipulatedText, Is.EqualTo(String.Empty), "Manipulated text TestCase width: " + width);
        }

        public static IEnumerable<TestCase> TestCasesNoMnemonicNoEllipsing
        {
            get
            {
                yield return new TestCase { Text = "junk", Expected = "junk", Width = 100 };
                yield return new TestCase { Text = "junk&", Expected = "junk&&", Width = 50 };
                yield return new TestCase { Text = "junk&", Expected = "junk&&", Width = 100 };
                yield return new TestCase { Text = "junk&&", Expected = "junk&&&&", Width = 100 };
                yield return new TestCase { Text = "junk&&junk", Expected = "junk&&&&junk", Width = 100 };
                yield return new TestCase { Text = "junk&junk", Expected = "junk&&junk", Width = 100 };
                yield return new TestCase
                                 {
                                     Text = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 
                                     Expected = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 
                                     Width = 100
                                 };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Width = 50
                };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Width = 100
                };
            }
        }

        public static IEnumerable<TestCase> TestCasesWithMnemonicNoEllipsing
        {
            get
            {
                yield return new TestCase { Text = "junk", Expected = "junk", Width = 100 };
                yield return new TestCase { Text = "junk&", Expected = "junk&", Width = 50 };
                yield return new TestCase { Text = "junk&", Expected = "junk&", Width = 100 };
                yield return new TestCase { Text = "junk&&", Expected = "junk&&", Width = 100 };
                yield return new TestCase { Text = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Expected = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Width = 100 };
                yield return new TestCase { Text = "junk&&junk", Expected = "junk&&junk", Width = 100 };
                yield return new TestCase { Text = "junk&junk", Expected = "junk&junk", Width = 100 };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Width = 50
                };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Width = 100
                };
            }
        }

        public static IEnumerable<TestCase> TestCasesWithMnemonic
        {
            get
            {
                yield return new TestCase { Text = "junk", Expected = "junk", Width = 100 };
                yield return new TestCase { Text = "junk&", Expected = "junk&", Width = 50 };
                yield return new TestCase { Text = "junk&", Expected = "junk&", Width = 100 };
                yield return new TestCase { Text = "junk&&", Expected = "junk&&", Width = 100 };
                yield return new TestCase { Text = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Expected = "junk&&&&&&&&&&&&&&&&&...", Width = 100 };
                yield return new TestCase { Text = "jnk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Expected = "jnk&&&&&&&&&&&&&&&&&&&...", Width = 100 };
                yield return new TestCase { Text = "junk&&junk", Expected = "junk&&junk", Width = 100 };
                yield return new TestCase { Text = "junk&junk", Expected = "junk&junk", Width = 100 };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "All...",
                    Width = 50
                };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndN...",
                    Width = 100
                };
            }
        }

        public static IEnumerable<TestCase> TestCasesNoMnemonic
        {
            get
            {
                yield return new TestCase { Text = "junk", Expected = "junk", Width = 100 };
                yield return new TestCase { Text = "junk&", Expected = "junk&&", Width = 50 };
                yield return new TestCase { Text = "junk&", Expected = "junk&&", Width = 100 };
                yield return new TestCase { Text = "junk&&", Expected = "junk&&&&", Width = 100 };
                yield return new TestCase { Text = "junk&&junk", Expected = "junk&&&&junk", Width = 100 };
                yield return new TestCase { Text = "junk&junk", Expected = "junk&&junk", Width = 100 };
                yield return new TestCase { Text = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Expected = "junk&&&&&&&&&&&&&&&&...", Width = 100 };
                yield return new TestCase { Text = "jnk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", Expected = "jnk&&&&&&&&&&&&&&&&&&...", Width = 100 };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "All...",
                    Width = 50
                };
                yield return new TestCase
                {
                    Text = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
                    Expected = "AllWorkAndN...",
                    Width = 100
                };
            }
        }

        public class TestCase
        {
            public string Text { get; set; }
            public string Expected { get; set; }
            public int Width { get; set; }
        }
    }
}
