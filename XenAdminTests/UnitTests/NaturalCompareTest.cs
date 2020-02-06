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
using XenCenterLib;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    class NaturalCompareTest
    {
        [Test, Sequential]
        public void Compare(
            [Values("a",  "", "a", "1",  "1",       "", "123abc", "abc123", "abc123ax", "foo bar",      "Windows 2000",  "h10",  "h9", "h 9", "VM-123",  "VM-334",  "VM-334",  "abc",  "zzz", "a", "B", "a",  "win95",  "Win95")] string a,
            [Values("b", "a",  "", "2", "10", "abc123", "abc123", "123abc", "abc123bx", "foo bar1", "Windows 2000 SP1", "h112", "h10", "h10", "VM-333", "VM-125x", "VM-1252", "abcd", "abcd", "B", "c", "A", "Win100", "win100")] string b,
            [Values( -1,  -1,  +1,  -1,   -1,       -1,       -1,       +1,         -1,         -1,                 -1,     -1,    -1,    -1,       -1,        +1,        -1,     -1,     +1,  -1,  -1,   0,       -1,       -1)] int result)
        {
            Assert.AreEqual(result, Clamp(StringUtility.NaturalCompare(a, b)));
            Assert.AreEqual(-result, Clamp(StringUtility.NaturalCompare(b, a)));
            Assert.AreEqual(0, StringUtility.NaturalCompare(a, a));
            Assert.AreEqual(0, StringUtility.NaturalCompare(b, b));
        }

        private int Clamp(int x)
        {
            return x < 0 ? -1 :
                x > 0 ? 1 :
                0;
        }
    }
}
