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

using XenAdmin;
using NUnit.Framework;

namespace XenAdminTests.StringExtensionsTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class EllipsiseStringTests
    {
        [Test]
        [TestCase(5, ExpectedResult = "th...")]
        [TestCase(18, ExpectedResult = "this is a very ...")]
        [TestCase(3, ExpectedResult = "...")]
        public string TestEllipsisingToShortenAString(int size)
        {
            const string testString = "this is a very long string, well long-ish, actually it's of moderate length really";
            return testString.Ellipsise(size);
        }

        [Test]
        public void TestEllipsisingAShortEnoughString([Values(8, 9, 28, 10000)] int size)
        {
            const string testString = "short";
            Assert.AreEqual(testString.Ellipsise(size), testString);
        }

        [Test]
        public void TestEllipsisingANullString()
        {
            const string testString = null;
            Assert.AreEqual(testString.Ellipsise(10), "");
        }

        [Test]
        public void TestEllipsisingAnEmptyString()
        {
            Assert.AreEqual("".Ellipsise(10), "");
        }

        [Test]
        public void TestAppendingEllipsisingWithMaxLengthLessThanEllipse([Values(1,2)] int size)
        {
            Assert.AreEqual("short".Ellipsise(size), ".");
        }

        [Test]
        [TestCase("junk", ExpectedResult = "junk...")]
        [TestCase("", ExpectedResult = "...")]
        [TestCase(null, ExpectedResult = "...")]
        [TestCase("junk...", ExpectedResult = "junk......")]
        [TestCase("junk.", ExpectedResult = "junk....")]
        public string AdditionOfEllipsis(string testString)
        {
            return testString.AddEllipsis();
        }
    }

}
