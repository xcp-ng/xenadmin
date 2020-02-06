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

using NUnit.Framework;
using XenAdmin;

namespace XenAdminTests.StringExtensionsTests
{
    [Category(TestCategories.Unit)]
    class TruncateStringTest
    {
        [TestCase(0, ExpectedResult = "")]
        [TestCase(1, ExpectedResult = "a")]
        [TestCase(2, ExpectedResult = "ab")]
        [TestCase(3, ExpectedResult = "abc")]
        [TestCase(4, ExpectedResult = "abcd")]
        [TestCase(5, ExpectedResult = "abcde")]
        [Test]
        public string LatinCharactersTest(int size)
        {
            return "abcde".Truncate(size);
        }

        [TestCase(0, ExpectedResult = "")]
        [TestCase(1, ExpectedResult = "短")]
        [TestCase(2, ExpectedResult = "短")]
        [TestCase(3, ExpectedResult = "短𠀁")]
        [TestCase(4, ExpectedResult = "短𠀁")]
        [TestCase(5, ExpectedResult = "短𠀁𪛕")]
        [Test]
        public string Utf16SurrogatePairsTest(int size)
        {
            return "短𠀁𪛕".Truncate(size); // The first character is 2 bytes, but the other two are 4-byte characters (UTF-16 surrogate pairs).
        }
    }
}
