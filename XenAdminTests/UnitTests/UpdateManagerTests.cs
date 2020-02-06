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
using System.Diagnostics;
using System.Threading;


namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class UpdateManagerTests
    {
        private const int Delay = 1000;
        private const int Leeway = 500;
        private UpdateManager _um;

        [SetUp]
        public void SetUp()
        {
            _um = new UpdateManager();
        }

        [TearDown]
        public void TearDown()
        {
            _um.Dispose();
            _um = null;
        }

        private static void AccurateSleep(int milliseconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds)
            {
                Thread.Sleep(1);
            }
        }

        [Test]
        public void TestDispose()
        {
            _um.Update += delegate
            {
                Assert.Fail("Dispose didn't work");
            };
            _um.Dispose();
            _um.RequestUpdate();
        }

        [Test]
        public void TestUpdateIsOnDifferentThread()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            bool finished = false;

            _um.Update += delegate
            {
                Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId, "Update wasn't on different thread.");
                finished = true;
            };
            _um.RequestUpdate();

            AccurateSleep(Leeway);
            Assert.IsTrue(finished, "test never finished");
        }

        [Test]
        public void TestFirstUpdateIsNearlyImmediate()
        {
            Stopwatch sw = null;
            bool finished = false;

            _um.Update += delegate
            {
                sw.Stop();
                Assert.Less(sw.ElapsedMilliseconds, 50, "Update didn't happen immediately.");
                // it won't be completely immediate because it's on another thread and a small delay will occur.
                finished = true;
            };
            sw = Stopwatch.StartNew();
            _um.RequestUpdate();

            AccurateSleep(Leeway);
            Assert.IsTrue(finished, "Test never finished.");
        }

        [Test]
        public void TestThatTwoSlowUpdateRequestsCausesTwoUpdates()
        {
            bool secondUpdateStarted = false;
            int updateCount = 0;
            const int updateTime = 100;

            _um.Update += delegate
            {
                AccurateSleep(updateTime);

                updateCount++;

                if (updateCount == 1)
                {
                    Assert.IsFalse(secondUpdateStarted);
                }
                else
                {
                    Assert.IsTrue(secondUpdateStarted);
                }
            };

            _um.RequestUpdate();
            AccurateSleep(updateTime + Leeway);

            secondUpdateStarted = true;
            _um.RequestUpdate();
        }
    }
}
