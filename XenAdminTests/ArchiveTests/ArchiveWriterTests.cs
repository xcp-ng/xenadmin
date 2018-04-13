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
using System.Linq;
using System.Text;
using NUnit.Framework;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    class ArchiveWriterTests
    {
        private class FakeArchiveWriter : ArchiveWriter
        {
            private List<Stream> streamAdded;
            private List<string> fileNameAdded;
            private List<DateTime> dateAdded;
            private bool disposed;

            public List<Stream> AddedStreamData
            {
                get { return streamAdded; }
            }

            public List<string> AddedFileNameData
            {
                get { return fileNameAdded; }
            }

            public List<DateTime> AddedDates
            {
                get { return dateAdded; }
            }

            public FakeArchiveWriter()
            {
                Reset();
            }

            public void Reset()
            {
                DisposeStreamList();
                streamAdded = new List<Stream>();
                fileNameAdded = new List<string>();
                dateAdded = new List<DateTime>();
            }

            private void DisposeStreamList()
            {
                if (streamAdded != null)
                {
                   foreach (Stream stream in streamAdded)
                    {
                        if( stream != null )
                            stream.Dispose();
                    } 
                }
                
            }

            public override void Add(Stream filetoAdd, string fileName, DateTime modificationTime)
            {
                disposed = false;
                streamAdded.Add(filetoAdd);
                fileNameAdded.Add(fileName);
                dateAdded.Add(modificationTime);
            }

            public override void AddDirectory(string directoryName, DateTime modificationTime)
            {
                fileNameAdded.Add(directoryName);
                dateAdded.Add(modificationTime);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if(disposing)
                {
                    if( !disposed )
                    {
                        DisposeStreamList();
                    }
                    disposed = true;
                }
            }
        }

        private FakeArchiveWriter fakeWriter;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            fakeWriter = new FakeArchiveWriter();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            fakeWriter.Dispose();
        }

        [SetUp]
        public void TestSetup()
        {
            fakeWriter.Reset();
        }

        [Test]
        public void DatelessAddCallsImplementation()
        {
            const string fileName = "test.file";
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
            {
                fakeWriter.Add(ms, fileName);
                Assert.AreEqual(1, fakeWriter.AddedFileNameData.Count);
                Assert.AreEqual(1, fakeWriter.AddedStreamData.Count);
                Assert.AreEqual(1, fakeWriter.AddedDates.Count);
                Assert.AreEqual(fileName, fakeWriter.AddedFileNameData[0], "File name");
                Assert.IsTrue(fakeWriter.AddedStreamData[0].Length == 14, "Stream has data");
                AssertCurrentDateIsPlausible(fakeWriter.AddedDates[0]);
            }
        }

        private void AssertCurrentDateIsPlausible(DateTime currentDate)
        {
            //If this is failing check that the number of seconds is enough
            const double seconds = 5.0;
            DateTime maxDate = DateTime.Now.AddSeconds(seconds);
            DateTime minDate = DateTime.Now.AddSeconds(-1.0 * seconds);
            Assert.IsTrue(currentDate > minDate, "Date is > minimum");
            Assert.IsTrue(currentDate < maxDate, "Date is < maximum");
        }

        [Test]
        public void DatelessAddDirectoryCallsImplementation()
        {
            const string dirName = "test.file";
            const int totalAdded = 3;
            for (int i = 0; i < totalAdded; i++)
            {
                fakeWriter.AddDirectory(dirName);
            }

            Assert.AreEqual(totalAdded, fakeWriter.AddedFileNameData.Count);
            Assert.AreEqual(0, fakeWriter.AddedStreamData.Count);
            Assert.AreEqual(totalAdded, fakeWriter.AddedDates.Count);
            Assert.AreEqual(dirName, fakeWriter.AddedFileNameData[0], "File name");
            AssertCurrentDateIsPlausible(fakeWriter.AddedDates[0]);
        }

        [Test]
        public void CreateArchiveThrowsWithBadPath()
        {
            Assert.That(()=> fakeWriter.CreateArchive("Yellow brick road - not a path!"), Throws.ArgumentException);
        }

        [Test]
        public void CreateArchiveWorksWithValidDirectoryStructure()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            CreateFiles(tempPath, 2);

            for (int i = 0; i < 4; i++)
            {
                string subfolder = Path.Combine(tempPath, Path.GetRandomFileName());
                Directory.CreateDirectory(subfolder);
                CreateFiles( subfolder, i);
            }

            fakeWriter.CreateArchive(tempPath);

            Assert.AreEqual(12, fakeWriter.AddedDates.Count );
            Assert.AreEqual(12, fakeWriter.AddedFileNameData.Count);
            Assert.AreEqual(8, fakeWriter.AddedStreamData.Count);
            
            foreach( DateTime date in fakeWriter.AddedDates )
                AssertCurrentDateIsPlausible(date);

            foreach (string name in fakeWriter.AddedFileNameData)
                Assert.AreEqual(-1, name.IndexOfAny(@":\".ToArray()), "Unwanted chars found in path");

            Directory.Delete(tempPath, true);
        }

        private void CreateFiles(string tempPath, int numberOfFiles)
        {
            for (int i = 0; i < numberOfFiles; i++)
            {
                using( FileStream fs = File.OpenWrite(Path.Combine(tempPath, Path.GetRandomFileName())))
                {
                    fs.Write(Encoding.ASCII.GetBytes("This is a test"), 0, 14);
                    fs.Flush();
                }
            }
        }
    }
}
