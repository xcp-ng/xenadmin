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

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CPUFeaturesTest
    {
        const string cpu1 = "000ce3bd-bfebfbff-00000001-20100800";
        const string cpu2 = "040ce3bd-bfebfbff-00000001-20100800";
        const string cpu3 = "040ce33d-bfebfbff-00000001-20100801";
        const string cpu4 = "000ce3bd-bfebfbff-00000001";

        [OneTimeSetUp]
        public void SetupResults()
        {
            // A cpu does not have less features than itself
            Expect(cpu1, cpu1, false);
            Expect(cpu2, cpu2, false);
            Expect(cpu3, cpu3, false);
            Expect(cpu4, cpu4, false);

            // cpu1 has less features than cpu2 and cpu3
            Expect(cpu1, cpu2, true);
            Expect(cpu1, cpu3, true);
            Expect(cpu1, cpu4, false);

            // cpu2 has less features than cpu3 (and some extra ones)
            Expect(cpu2, cpu1, false);
            Expect(cpu2, cpu3, true);
            Expect(cpu2, cpu4, false);

            // cpu3 is missing one feature (and has some extra ones)
            Expect(cpu3, cpu1, true);
            Expect(cpu3, cpu2, true);
            Expect(cpu3, cpu4, true);

            // cpu4 has less features than all because is missing the last set
            Expect(cpu4, cpu1, true);
            Expect(cpu4, cpu2, true);
            Expect(cpu4, cpu3, true);
        }

        Dictionary<Config, bool> expectedResults = new Dictionary<Config, bool>();
        private class Config
        {
            string featureSet1, featureSet2;
            public Config(string featureSet1, string featureSet2)
            {
                this.featureSet1 = featureSet1;
                this.featureSet2 = featureSet2;
            }

            public override bool Equals(object obj)
            {
                Config other = obj as Config;
                if (other == null)
                    return false;

                return
                    this.featureSet1 == other.featureSet1 &&
                    this.featureSet2 == other.featureSet2;
            }

            public override int GetHashCode()
            {
                return featureSet1.GetHashCode() * featureSet2.GetHashCode();
            }
        }

        private void Expect(string featureSet1, string featureSet2, bool expected_result)
        {
            expectedResults[new Config(featureSet1, featureSet2)] = expected_result;
        }

        private bool Expected(string featureSet1, string featureSet2)
        {
            return expectedResults[new Config(featureSet1, featureSet2)];
        }

        [Test]
        public void Run(
            [Values(cpu1, cpu2, cpu3, cpu4)] string featureSet1,
            [Values(cpu1, cpu2, cpu3, cpu4)] string featureSet2
            )
        {
            System.Console.WriteLine("Asserting {0} < {1}", featureSet1, featureSet2);
            Assert.AreEqual(
                Expected(featureSet1, featureSet2),
                PoolJoinRules.FewerFeatures(featureSet1, featureSet2), string.Format("Assertion failed on {0} < {1}", featureSet1, featureSet2));
        }
    }
}