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
using System.IO;
using NUnit.Framework;
using XenAdmin;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    class ArchiveFactoryTests
    {
        [Test]
        public void TestValidReaderGeneration()
        {
            Dictionary<ArchiveFactory.Type, Type> validIterators = new Dictionary<ArchiveFactory.Type, Type>() 
            { 
                { ArchiveFactory.Type.Tar, typeof( SharpZipTarArchiveIterator )},
                { ArchiveFactory.Type.TarGz, typeof( SharpZipTarArchiveIterator )},
                { ArchiveFactory.Type.TarBz2, typeof( SharpZipTarArchiveIterator )}
            };

            foreach (KeyValuePair<ArchiveFactory.Type, Type> pair in validIterators)
            {
                string target = Path.Combine(Directory.GetCurrentDirectory(), @"XenAdminTests\TestResources\emptyfile.bz2");
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(target)))
                {
                    ArchiveIterator providedStream = ArchiveFactory.Reader(pair.Key, ms);
                    Assert.AreEqual(providedStream.GetType(), pair.Value);
                }
            }
        }

        [Test]
        public void TestValidWriterGeneration()
        {
            Dictionary<ArchiveFactory.Type, Type> validIterators = new Dictionary<ArchiveFactory.Type, Type>() 
            { 
                { ArchiveFactory.Type.Tar, typeof( SharpZipTarArchiveWriter )},
                { ArchiveFactory.Type.Zip, typeof( DotNetZipZipWriter )}
            };

            foreach (KeyValuePair<ArchiveFactory.Type, Type> pair in validIterators)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ArchiveWriter providedStream = ArchiveFactory.Writer(pair.Key, ms);
                    Assert.AreEqual(providedStream.GetType(), pair.Value);
                }
            }
        }

        [Test]
        public void TestInvalidTarGzWriterGeneration()
        {
            Assert.That(()=> CreateInvalidWriterType(ArchiveFactory.Type.TarGz), Throws.Exception.With.TypeOf(typeof(NotSupportedException)));
        }

        [Test]
        public void TestInvalidTarBz2WriterGeneration()
        {
            Assert.That(() => CreateInvalidWriterType(ArchiveFactory.Type.TarBz2), Throws.Exception.With.TypeOf(typeof(NotSupportedException))); 
        }

        private void CreateInvalidWriterType(ArchiveFactory.Type type)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ArchiveFactory.Writer(type, ms);
            }
        }
    }
}
