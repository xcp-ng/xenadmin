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
    public class CPUMaskingTest
    {
        const string cpu1 = "040ce33d-bfebfbff-00000001-20100800";
        const string cpu2 = "000ce3bd-bfebfbff-00000001-20100800";
        const string cpu3 = "040ce33d-bfebfbff-00000001-20100801";
        const string mask = "ffffff7f-ffffffff-ffffffff-ffffffff";

        [OneTimeSetUp]
        public void SetupResults()
        {
            // Can always mask a CPU to itself
            Expect("base", cpu1, cpu1, null, true);
            Expect("base", cpu1, cpu1, mask, true);
            Expect("base", cpu2, cpu2, null, true);
            Expect("base", cpu2, cpu2, mask, true);
            Expect("base", cpu3, cpu3, null, true);
            Expect("base", cpu3, cpu3, mask, true);
            Expect("full", cpu1, cpu1, null, true);
            Expect("full", cpu1, cpu1, mask, true);
            Expect("full", cpu2, cpu2, null, true);
            Expect("full", cpu2, cpu2, mask, true);
            Expect("full", cpu3, cpu3, null, true);
            Expect("full", cpu3, cpu3, mask, true);

            // Masking CPUs to other CPUs: base masking only
            Expect("base", cpu1, cpu2, null, false);  // cpu1 can only become cpu2 in the presence of the mask
            Expect("base", cpu1, cpu2, mask, true);
            Expect("base", cpu1, cpu3, null, false);  // cpu1 is less than cpu3
            Expect("base", cpu1, cpu3, mask, false);
            Expect("base", cpu2, cpu1, null, false);  // cpu2 is less than cpu1 and cpu3
            Expect("base", cpu2, cpu1, mask, false);
            Expect("base", cpu2, cpu3, null, false);
            Expect("base", cpu2, cpu3, mask, false);
            Expect("base", cpu3, cpu1, null, false);  // cpu3 is greater than cpu1 and cpu2 in the extended bits, so can't be masked with only base masking
            Expect("base", cpu3, cpu1, mask, false);
            Expect("base", cpu3, cpu2, null, false);
            Expect("base", cpu3, cpu2, mask, false);

            // Masking CPUs to other CPUs: full masking
            Expect("full", cpu1, cpu2, null, false);  // cpu1 can only become cpu2 in the presence of the mask
            Expect("full", cpu1, cpu2, mask, true);
            Expect("full", cpu1, cpu3, null, false);  // cpu1 is less than cpu3
            Expect("full", cpu1, cpu3, mask, false);
            Expect("full", cpu2, cpu1, null, false);  // cpu2 is less than cpu1 and cpu3
            Expect("full", cpu2, cpu1, mask, false);
            Expect("full", cpu2, cpu3, null, false);
            Expect("full", cpu2, cpu3, mask, false);
            Expect("full", cpu3, cpu1, null, true);   // cpu3 is greater than cpu1
            Expect("full", cpu3, cpu1, mask, true);
            Expect("full", cpu3, cpu2, null, false);  // cpu3 can only become cpu2 in the presence of the mask
            Expect("full", cpu3, cpu2, mask, true);

            // Masking always fails with mask_type "no", even masking a CPU to itself.
            // (In the real program, masking a CPU to itself never gets to the "is it maskable?" code).
            Expect("no", cpu1, cpu1, null, false);
            Expect("no", cpu1, cpu1, mask, false);
            Expect("no", cpu2, cpu2, null, false);
            Expect("no", cpu2, cpu2, mask, false);
            Expect("no", cpu3, cpu3, null, false);
            Expect("no", cpu3, cpu3, mask, false);
            Expect("no", cpu1, cpu2, null, false);
            Expect("no", cpu1, cpu2, mask, false);
            Expect("no", cpu1, cpu3, null, false);
            Expect("no", cpu1, cpu3, mask, false);
            Expect("no", cpu2, cpu1, null, false);
            Expect("no", cpu2, cpu1, mask, false);
            Expect("no", cpu2, cpu3, null, false);
            Expect("no", cpu2, cpu3, mask, false);
            Expect("no", cpu3, cpu1, null, false);
            Expect("no", cpu3, cpu1, mask, false);
            Expect("no", cpu3, cpu2, null, false);
            Expect("no", cpu3, cpu2, mask, false);
        }

        Dictionary<Config, bool> expectedResults = new Dictionary<Config, bool>();
        private class Config
        {
            string mask_type, slave_cpu, master_cpu, mask_bits;
            public Config(string mask_type, string slave_cpu, string master_cpu, string mask_bits)
            {
                this.mask_type = mask_type;
                this.slave_cpu = slave_cpu;
                this.master_cpu = master_cpu;
                this.mask_bits = mask_bits;
            }

            public override bool Equals(object obj)
            {
                Config other = obj as Config;
                if (other == null)
                    return false;

                return
                    this.mask_type == other.mask_type &&
                    this.slave_cpu == other.slave_cpu &&
                    this.master_cpu == other.master_cpu &&
                    this.mask_bits == other.mask_bits;
            }

            public override int GetHashCode()
            {
                return mask_type.GetHashCode() * slave_cpu.GetHashCode() *
                    master_cpu.GetHashCode() * (mask_bits == null ? 1 : mask_bits.GetHashCode());
            }
        }

        private void Expect(string mask_type, string slave_cpu, string master_cpu, string mask_bits, bool expected_result)
        {
            expectedResults[new Config(mask_type, slave_cpu, master_cpu, mask_bits)] = expected_result;
        }

        private bool Expected(string mask_type, string slave_cpu, string master_cpu, string mask_bits)
        {
            return expectedResults[new Config(mask_type, slave_cpu, master_cpu, mask_bits)];
        }

        [Test]
        public void Run(
            [Values("no", "base", "full")] string mask_type,
            [Values(cpu1, cpu2, cpu3)] string slave_cpu,
            [Values(cpu1, cpu2, cpu3)] string master_cpu,
            [Values(mask, null)] string mask_bits
            )
        {
            Assert.AreEqual(
                Expected(mask_type, slave_cpu, master_cpu, mask_bits),
                PoolJoinRules.MaskableTo(mask_type, slave_cpu, master_cpu, mask_bits));
        }
    }
}
