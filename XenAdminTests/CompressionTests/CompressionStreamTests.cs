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

using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using XenCenterLib.Compression;

namespace XenAdminTests.CompressionTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CompressionStreamTest
    {
        private class CompressionStreamFake : CompressionStream
        {
            public CompressionStreamFake()
            {
                Reset();
            }

            public void Reset()
            {
                Dispose();
                zipStream = new MemoryStream();
            }
        }

        private CompressionStreamFake fakeCompressionStream;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            fakeCompressionStream = new CompressionStreamFake();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            fakeCompressionStream.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            fakeCompressionStream.Reset();
        }

        [Test]
        public void ReadAndWrite()
        {
            byte[] bytesToAdd = Encoding.ASCII.GetBytes("This is a test");
            fakeCompressionStream.Write(bytesToAdd, 0, bytesToAdd.Length);
            fakeCompressionStream.Position = 0;

            byte[] outBytes = new byte[fakeCompressionStream.Length];
            fakeCompressionStream.Read(outBytes, 0, 14);

            Assert.IsTrue(bytesToAdd.SequenceEqual(outBytes), "Read Write arrays are equal");
        }

        [Test]
        public void BufferedReadAndBufferedWrite()
        {
            using(MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
            {
                fakeCompressionStream.BufferedWrite(ms);
                fakeCompressionStream.Position = 0;
                
                using( MemoryStream oms = new MemoryStream() )
                {
                    fakeCompressionStream.BufferedRead(oms);
                    Assert.IsTrue( oms.Length > 1, "Stream is not empty" );
                    Assert.AreEqual(ms,oms, "Streams are equal");
                }
            }            
        }

        [Test]
        public void PropertiesAreEqualToUnderlyingStream()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                Assert.AreEqual(ms.CanRead, fakeCompressionStream.CanRead);
                Assert.AreEqual(ms.CanSeek, fakeCompressionStream.CanSeek);
                Assert.AreEqual(ms.CanTimeout, fakeCompressionStream.CanTimeout);
                Assert.AreEqual(ms.CanWrite, fakeCompressionStream.CanWrite);
            }
        }

        [Test]
        public void LengthAndPositionSetters()
        {
            fakeCompressionStream.SetLength(20);
            Assert.AreEqual(20, fakeCompressionStream.Length);

            fakeCompressionStream.Position = 2;
            Assert.AreEqual(2,fakeCompressionStream.Position);
        }

        [Test]
        public void Seeking()
        {
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
            {
                fakeCompressionStream.BufferedWrite(ms);
                fakeCompressionStream.Position = 0;

                fakeCompressionStream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual('T', fakeCompressionStream.ReadByte());

                fakeCompressionStream.Seek(5, SeekOrigin.Begin);
                Assert.AreEqual('i', fakeCompressionStream.ReadByte());
            }
        }
    }
}
