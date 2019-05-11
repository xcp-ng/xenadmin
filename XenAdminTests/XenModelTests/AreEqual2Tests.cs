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

using System.Collections.Generic;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AreEqual2Tests
    {
        [Test]
        public void A_null_list_is_considered_equal_to_itself()
        {
            List<string> given = null;
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Separate_null_lists_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(Helper.AreEqual2((List<string>)null, (List<string>)null));
        }

        [Test]
        public void A_null_list_is_considered_equal_to_an_empty_one()
        {
            Assert.IsTrue(Helper.AreEqual2(null, new List<string>()));
        }

        [Test]
        public void An_empty_list_is_considered_equal_to_a_null_one()
        {
            Assert.IsTrue(Helper.AreEqual2(new List<string>(), null));
        }

        [Test]
        public void A_list_is_considered_equal_to_itself()
        {
            var given = new List<string> {
                "test",
                "other"
            };
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Different_lists_are_not_considered_equal_to_each_other()
        {
            Assert.IsFalse(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "something",
                        "else"
                    }));
        }

        [Test]
        public void Separate_though_equal_lists_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "test",
                        "other"
                    }));
        }

        [Test]
        public void A_null_dictionary_is_considered_equal_to_itself()
        {
            Dictionary<string, string> given = null;
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Separate_null_dictionary_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(Helper.AreEqual2((Dictionary<string, string>)null, (Dictionary<string, string>)null));
        }

        [Test]
        public void A_null_dictionary_is_considered_equal_to_an_empty_one()
        {
            Assert.IsTrue(Helper.AreEqual2(null, new Dictionary<string, string>()));
        }

        [Test]
        public void An_empty_dictionary_is_considered_equal_to_a_null_one()
        {
            Assert.IsTrue(Helper.AreEqual2(new Dictionary<string, string>(), null));
        }

        [Test]
        public void A_dictionary_is_considered_equal_to_itself()
        {
            var given = new Dictionary<string, string> {
                { "test", "a"},
                { "other", "b"}
            };
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Different_dictionaries_are_not_considered_equal_to_each_other()
        {
            Assert.IsFalse(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "something", "c" },
                        { "else", "d" }
                    }));
        }

        [Test]
        public void Separate_though_equal_dictionaries_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "test", "a" },
                        { "other", "b" }
                    }));
        }
    }
}
