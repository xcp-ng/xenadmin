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

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class GzipCompressionTests : ThirdPartyCompressionTests
    {
        [OneTimeSetUp]
        public void SetupFixture()
        {
            Compressor = new DotNetZipGZipOutputStream();
            Decompressor = new DotNetZipGZipInputStream();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class Bzip2CompressionTests : ThirdPartyCompressionTests
    {
        [OneTimeSetUp]
        public void SetupFixture()
        {
            Compressor = new DotNetZipBZip2OutputStream();
            Decompressor = new DotNetZipBZip2InputStream();
        }

    }

    public abstract class ThirdPartyCompressionTests
    {
        private CompressionStream compressor;
        private CompressionStream decompressor;

        protected CompressionStream Compressor
        {
            set { compressor = value; }
            get { return compressor;  }
        }

        protected CompressionStream Decompressor
        {
            set { decompressor = value; }
            get { return decompressor;  }
        }

        private const string loremIpsum =
            @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod
 tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris 
 nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore 
 eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit 
 anim id est laborum.";

        [Test]
        public void TestFileCompressionAndDecompression()
        {
            string basePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(basePath);

            //Write a file to compress
            string uncompressedFileName = Path.Combine(basePath, Path.GetRandomFileName());
            string compressedFileName = Path.Combine(basePath, Path.GetRandomFileName());
            string decompressedFileName = Path.Combine(basePath, Path.GetRandomFileName());

            CreateADummyFile(uncompressedFileName);
            CompressAFile(compressedFileName, uncompressedFileName);
            DecompressAFile(decompressedFileName, compressedFileName);

            int uncompressedFileLength = File.ReadAllBytes(uncompressedFileName).Length;
            int compressedFileLength = File.ReadAllBytes(compressedFileName).Length;
            int decompressedFileLength = File.ReadAllBytes(decompressedFileName).Length;

            Assert.IsTrue(uncompressedFileLength > 0);
            Assert.IsTrue(decompressedFileLength > 0);
            Assert.IsTrue(compressedFileLength > 0);

            Assert.AreNotEqual(uncompressedFileLength, compressedFileLength);
            Assert.AreNotEqual(decompressedFileLength, compressedFileLength);
            Assert.AreEqual(decompressedFileLength, uncompressedFileLength);

            Assert.IsTrue(File.ReadAllBytes(uncompressedFileName).SequenceEqual(File.ReadAllBytes(decompressedFileName)));

            Directory.Delete(basePath, true);

        }

        private void DecompressAFile( string targetFileName, string compressedFileName)
        {
            using (FileStream ifs = File.OpenRead(compressedFileName))
            {
                decompressor.SetBaseStream(ifs);
                using (FileStream fs = File.OpenWrite(targetFileName))
                {
                    decompressor.BufferedRead(fs);
                }

                decompressor.Dispose();
            }
        }


        private void CompressAFile(string targetFileName, string uncompressedFileName)
        {
            using (FileStream ifs = File.OpenWrite(targetFileName))
            {
                compressor.SetBaseStream(ifs);

                using (FileStream fs = File.OpenRead(uncompressedFileName))
                {
                    compressor.BufferedWrite(fs);
                }

                compressor.Dispose();
            }
            
        }

        private void CreateADummyFile(string uncompressedFileName)
        {
            using (FileStream ims = File.OpenWrite(uncompressedFileName))
            {
                ims.Write(Encoding.ASCII.GetBytes(loremIpsum), 0, loremIpsum.Length);
            }
        }
    }
}
