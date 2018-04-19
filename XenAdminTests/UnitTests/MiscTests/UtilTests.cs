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
using System.Linq;
using System.Collections.Generic;
using System;

namespace XenAdminTests.UnitTests.MiscTests
{
    [TestFixture, Category(TestCategories.Unit), SetCulture("en-EN")]
    internal class UtilTests
    {
        #region Test Data

        private Dictionary<double, string[]> memoryMBpairs = new Dictionary<double, string[]>
            {
                {          0, new[] { "0", "MB" } },
                {          1, new[] { "0", "MB" } },
                {       1024, new[] { "0", "MB" } },
                {     100000, new[] { "0", "MB" } },
                {    1048576, new[] { "1", "MB" } }, //1024*1024
                {    2100000, new[] { "2", "MB" } },
                {    2900000, new[] { "3", "MB" } },
                { 1073741824, new[] { "1024", "MB" } }, //1024*1024*1024
                { 2100000000, new[] { "2003", "MB" } },
            };

        private Dictionary<double, string[]> memoryVariousPairs = new Dictionary<double, string[]>
                {
                    {          0, new[] { "0", "B" } },
                    {       1000, new[] { "1000", "B" } },
                    {       1024, new[] { "1", "kB" } },
                    {     100000, new[] { "98", "kB" } },
                    {    1000000, new[] { "977", "kB" } },
                    {    1048576, new[] { "1", "MB" } }, //1024*1024
                    {    2100000, new[] { "2", "MB" } },
                    {    2900000, new[] { "3", "MB" } },
                    { 1073741824, new[] { "1", "GB" } }, //1024*1024*1024
                    { 2100000000, new[] { "2", "GB" } },
                    { 2900000000, new[] { "3", "GB" } }
                };

        private Dictionary<double, string[]> dataRatePairs = new Dictionary<double, string[]>
                {
                    {          0, new[] { "0", "Bps" } },
                    {       1000, new[] { "1000", "Bps" } },
                    {       1024, new[] { "1", "kBps" } },
                    {     100000, new[] { "97.7", "kBps" } },
                    {    1000000, new[] { "976.6", "kBps" } },
                    {    1048576, new[] { "1", "MBps" } }, //1024*1024
                    {    2100000, new[] { "2", "MBps" } },
                    {    2900000, new[] { "2.8", "MBps" } },
                    { 1073741824, new[] { "1", "GBps" } }, //1024*1024*1024
                    { 2100000000, new[] { "2", "GBps" } },
                    { 2900000000, new[] { "2.7", "GBps" } }
                };

        private Dictionary<double, string[]> nanoSecPairs = new Dictionary<double, string[]>
                {
                    {          0, new[] { "0", "ns" } },
                    {          1, new[] { "1", "ns" } },
                    {        1.1, new[] { "1", "ns" } },
                    {        1.5, new[] { "2", "ns" } },
                    {        1.9, new[] { "2", "ns" } },
                    {         12, new[] { "12", "ns" } },
                    {       1100, new[] { "1", '\u03bc'+"s" } },//greek mi
                    {       1500, new[] { "2", '\u03bc'+"s"  } },
                    {       1900, new[] { "2", '\u03bc'+"s"  } },
                    {    1100000, new[] { "1", "ms" } },
                    {    1500000, new[] { "2", "ms" } },
                    {    1900000, new[] { "2", "ms" } },
                    { 1100000000, new[] { "1", "s" } },
                    { 2100000000, new[] { "2", "s" } },
                    { 2900000000, new[] { "3", "s" } }
                };

        private Dictionary<long, string[]> diskSizeOneDpPairs = new Dictionary<long, string[]>
                {
                    {          0, new[] { "0", "B" } },
                    {       1000, new[] { "1000", "B" } },
                    {       1024, new[] { "1", "kB" } },
                    {     100000, new[] { "97.7", "kB" } },
                    {    1000000, new[] { "976.6", "kB" } },
                    {    1048576, new[] { "1", "MB" } }, //1024*1024
                    {    2100000, new[] { "2", "MB" } },
                    {    2900000, new[] { "2.8", "MB" } },
                    { 1073741824, new[] { "1", "GB" } }, //1024*1024*1024
                    { 2100000000, new[] { "2", "GB" } },
                    { 2900000000, new[] { "2.7", "GB" } }
                };

        #endregion

        [Test]
        public void TestMemorySizeStringSuitableUnits()
        {            
            var pairs = new Dictionary<string[], string>
            {
                { new [] {"1072693248", "false"}, "1023 MB"},
                { new [] {"1073741824", "false"}, "1 GB"},
                { new [] {"1073741824", "true"}, "1.0 GB"},
                { new [] {"1825361100.8", "false"}, "1.7 GB"},                
                { new [] {"1825361100.8", "true"}, "1.7 GB"},
                { new [] {"536870912", "true"}, "512 MB"},
                { new [] {"537290342.4", "true"}, "512 MB"}
            };
            foreach(var pair in pairs)
                Assert.AreEqual(pair.Value, Util.MemorySizeStringSuitableUnits(Convert.ToDouble(pair.Key[0]),Convert.ToBoolean(pair.Key[1])));
        }

        [Test]
        public void TestMemorySizeStringVariousUnits()
        {
            foreach (var pair in memoryVariousPairs)
            {
                string expected = string.Format("{0} {1}", pair.Value[0], pair.Value[1]);
                Assert.AreEqual(expected, Util.MemorySizeStringVariousUnits(pair.Key));
            }
        }

        [Test]
        public void TestMemorySizeValueVariousUnits()
        {
            foreach (var pair in memoryVariousPairs)
            {
                string unit;
                var value = Util.MemorySizeValueVariousUnits(pair.Key, out unit);
                Assert.AreEqual(pair.Value[0], value);
                Assert.AreEqual(pair.Value[1], unit);
            }
        }


        [Test]
        public void TestDataRateString()
        {
            foreach (var pair in dataRatePairs)
            {
                string expected = string.Format("{0} {1}", pair.Value[0], pair.Value[1]);
                Assert.AreEqual(expected, Util.DataRateString(pair.Key));
            }
        }

        [Test]
        public void TestDataRateValue()
        {
            foreach (var pair in dataRatePairs)
            {
                string unit;
                var value = Util.DataRateValue(pair.Key, out unit);
                Assert.AreEqual(pair.Value[0], value);
                Assert.AreEqual(pair.Value[1], unit);
            }
        }


        [Test]
        public void TestDiskSizeStringUlong()
        {
            Assert.AreEqual("17179869184 GB", Util.DiskSizeString(ulong.MaxValue));
        }

        [Test]
        public void TestDiskSizeString()
        {
            foreach (var pair in diskSizeOneDpPairs)
            {
                string expected = string.Format("{0} {1}", pair.Value[0], pair.Value[1]);
                Assert.AreEqual(expected, Util.DiskSizeString(pair.Key));
            }
        }

        [Test]
        public void TestDiskSizeStringWithoutUnits()
        {
            foreach (var pair in diskSizeOneDpPairs)
                Assert.AreEqual(pair.Value[0], Util.DiskSizeStringWithoutUnits(pair.Key));
        }

        [Test]
        public void TestDiskSizeStringVariousDp()
        {
            var pairs = new Dictionary<long[], string>
                {
                    { new[] {          0, 1L }, "0 B" },
                    { new[] {       1000, 2L }, "1000 B" },
                    { new[] {       1024, 3L }, "1 kB" },
                    { new[] {     100000, 2L }, "97.66 kB" },
                    { new[] {    1000000, 1L }, "976.6 kB" },
                    { new[] {    1048576, 3L }, "1 MB" }, //1024*1024
                    { new[] {    2100000, 3L }, "2.003 MB" },
                    { new[] {    2900000, 2L }, "2.77 MB" },
                    { new[] { 1073741824, 3L }, "1 GB" }, //1024*1024*1024
                    { new[] { 2100000000, 3L }, "1.956 GB" }
                };

            foreach (var pair in pairs)
                Assert.AreEqual(pair.Value, Util.DiskSizeString(pair.Key[0], (int)pair.Key[1]));
        }

        [Test]
        public void TestNanoSecondsString()
        {
            foreach (var pair in nanoSecPairs)
            {
                string expected = string.Format("{0}{1}", pair.Value[0], pair.Value[1]);
                Assert.AreEqual(expected, Util.NanoSecondsString(pair.Key));
            }
        }

        [Test]
        public void TestNanoSecondsValue()
        {
            foreach (var pair in nanoSecPairs)
            {
                string unit;
                var value = Util.NanoSecondsValue(pair.Key, out unit);
                Assert.AreEqual(pair.Value[0], value);
                Assert.AreEqual(pair.Value[1], unit);
            }
        }


        [Test]
        public void TestCountsPerSecondString()
        {
            var pairs = new Dictionary<double, string>
                {
                    {0,"0 /sec"},
                    {0.12,"0.12 /sec"},
                    {1234.56,"1234.56 /sec"}
                };

            foreach (var pair in pairs)
                Assert.AreEqual(pair.Value, Util.CountsPerSecondString(pair.Key));
        }

        [Test]
        public void TestPercentageString()
        {
            var pairs = new Dictionary<double, string>
                {
                    { 1, "100.0%" },
                    { 0.12, "12.0%" },
                    { 0.0121, "1.2%" },
                    { 0.0125, "1.3%" },
                    { 0.0129, "1.3%" }
                };

            foreach (var pair in pairs)
                Assert.AreEqual(pair.Value, Util.PercentageString(pair.Key));
        }
    }
}