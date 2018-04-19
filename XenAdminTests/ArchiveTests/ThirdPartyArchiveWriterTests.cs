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
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class TarArchiveWriterTests : ThirdPartyArchiveWriterTest
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Writer = new SharpZipTarArchiveWriter();
            Reader = new SharpZipTarArchiveIterator();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class ZipArchiveWriterTests : ThirdPartyArchiveWriterTest
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Writer = new DotNetZipZipWriter();
            Reader = null;
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            Writer.Dispose();
        }
    }

    public abstract class ThirdPartyArchiveWriterTest
    {
        private ArchiveWriter writer;
        private ArchiveIterator reader;

        protected ArchiveWriter Writer
        {
            set { writer = value; }
            get { return writer;  }
        }

        protected ArchiveIterator Reader
        {
            set { reader = value; }
            get { return reader;  }
        }

        [Test]
        public void CreateAnArchiveAndRereadWhenPossible()
        {
            string tempPath = CreateTempFolder();

            string tarFileName = Path.Combine( tempPath, Path.GetRandomFileName() );

            WriteAnArchive(tarFileName);

            if( reader != null )
                RereadAnArchiveAndTest(tarFileName);
            else
                Trace.WriteLine(String.Format("A reader for the writer class {0} was not found so the contents are unvalidated by this test", writer.GetType()));
            
            Directory.Delete(tempPath, true);
        }

        // This test knows the contents of the file written in the archive 
        private void RereadAnArchiveAndTest(string archiveName)
        {
            List<string> expectedDirs = new List<string>()
                                            {
                                                "adir/",
                                                "adir2/a/"                                          
                                            };

            List<string> expectedFiles = new List<string>()
                                            {
                                                "tf1",
                                                "adir2/a/tf2"                                          
                                            };

            using (FileStream fs = File.OpenRead(archiveName))
            {
                reader.SetBaseStream(fs);
                while (reader.HasNext())
                {
                    if (reader.IsDirectory() && expectedDirs.Contains(reader.CurrentFileName()))
                        expectedDirs.Remove(reader.CurrentFileName());

                    if (!reader.IsDirectory() && expectedFiles.Contains(reader.CurrentFileName()))
                    {
                        expectedFiles.Remove(reader.CurrentFileName());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            reader.ExtractCurrentFile(ms);
                            Assert.IsTrue( ms.Length > 0, "Extracted file contents have data");
                        }
                    }

                }

                Assert.AreEqual(0, expectedFiles.Count, "Expected files count");
                Assert.AreEqual(0, expectedDirs.Count, "Expected dir count");

            }

            Reader.Dispose();
        }

        private string CreateTempFolder()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        private void WriteAnArchive(string tarFileName)
        {
            using (FileStream ms = File.OpenWrite(tarFileName))
            {
                Assert.AreEqual(0, ms.Length);
                writer.SetBaseStream(ms);

                using (MemoryStream ms1 = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
                {
                    writer.Add(ms1, "tf1");
                }

                using (MemoryStream ms2 = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
                {
                    writer.Add(ms2, "adir2/a/tf2", DateTime.Now);
                }

                writer.AddDirectory("adir/");
                writer.AddDirectory("adir2/a", DateTime.Now);
                writer.Dispose();
            }

            Assert.IsTrue(File.Exists(tarFileName), "archive exists");
            Assert.IsTrue(File.ReadAllBytes(tarFileName).Length > 0, "archive has contents");
        }
    }
}
