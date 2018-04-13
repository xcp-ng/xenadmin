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
using System.IO;
using System.Text;
using NUnit.Framework;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    class ArchiveIteratorTests
    {
        private ArchiveIteratorFake fakeIterator;

        #region ArchiveIterator Fake 
        private class ArchiveIteratorFake : ArchiveIterator
        {
            private int numberOfCallsLeft;
            private Stream extractedFile;
            private string currentFileName;
            private long currentFileSize;
            private DateTime modTime;
            private bool isDirectory;
            private bool disposed;

            public int NumberOfCallsLeftReturn
            {
                set { numberOfCallsLeft = value; }
            }
            public Stream ExtractedFileReturn
            {
                set 
                { 
                    extractedFile = value;
                    disposed = false;
                }
            }
            public string CurrentFileNameReturn
            {
                set { currentFileName = value; }
            }
            public long CurrentFileSizeReturn
            {
                set { currentFileSize = value; }
            }
            public DateTime ModTimeReturn
            {
                set { modTime = value; }
            }
            public bool IsDirectoryReturn
            {
                set { isDirectory = value; }
            }

            public ArchiveIteratorFake()
            {
                Reset();
            }

            public void Reset()
            {
                CurrentFileNameReturn = "TestFileName.fake";
                CurrentFileSizeReturn = 100;
                ExtractedFileReturn = new MemoryStream(Encoding.ASCII.GetBytes("This is a test"));
                IsDirectoryReturn = false;
                ModTimeReturn = new DateTime(2011, 4, 1, 11, 04, 01);
                NumberOfCallsLeftReturn = 1;
            }

            public override bool HasNext()
            {
                if (numberOfCallsLeft < 1)
                    return false;

                numberOfCallsLeft--;
                return true;
            }

            public override void ExtractCurrentFile(Stream extractedFileContents)
            {
                byte[] buffer = new byte[2 * 1024 * 1024];
                int read;
                while ((read = extractedFile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    extractedFileContents.Write(buffer, 0, read);
                }
                extractedFile.Position = 0;
            }

            public override string CurrentFileName()
            {
                if (String.IsNullOrEmpty(currentFileName))
                    return currentFileName;

                return numberOfCallsLeft + currentFileName;
            }

            public override long CurrentFileSize()
            {
                return currentFileSize;
            }

            public override DateTime CurrentFileModificationTime()
            {
                return modTime;
            }

            public override bool IsDirectory()
            {
                return isDirectory;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if( !disposed )
                {
                    if( disposing )
                    {
                        if(extractedFile != null)
                            extractedFile.Dispose();
                    }
                }
            }
        } 
        #endregion

        [OneTimeSetUp]
        public void Setup()
        {
            fakeIterator = new ArchiveIteratorFake();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            fakeIterator.Dispose();
        }

        [SetUp]
        public void TestSetup()
        {
            fakeIterator.Reset();
        }


        [Test]
        public void AnExceptionIsThrownForNullArgumentWhenCallingExtractAllContents()
        {
            Assert.That(() => fakeIterator.ExtractAllContents(null), Throws.ArgumentException);
        }

        [Test]
        public void AnExceptionIsThrownForANullFileNameWhenCallingExtractAllContents()
        {
            fakeIterator.CurrentFileNameReturn = null;
            Assert.That(() => fakeIterator.ExtractAllContents(Path.GetTempPath()), Throws.ArgumentException);
        }
        
        [Test]
        public void VerifyAFileIsWrittenWhenCallingExtractAllContents()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            const int numberOfFiles = 3;
            fakeIterator.NumberOfCallsLeftReturn = numberOfFiles;
            fakeIterator.ExtractAllContents(tempPath);

            //Test file has been created
            string targetFile = Path.Combine(tempPath, fakeIterator.CurrentFileName());
            Assert.IsTrue(File.Exists(targetFile), "File Exists");

            Assert.IsTrue(File.ReadAllBytes(targetFile).Length > 1, "File length > 1");

            //Check recursively that there are only the correct number of files
            Assert.IsTrue(Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length == numberOfFiles, "File number is correct");

            Assert.IsFalse((File.GetAttributes(targetFile) & FileAttributes.Directory) == FileAttributes.Directory, "Is not a dir");

            Directory.Delete(tempPath,true);
        }

        [Test]
        public void VerifyADirectoryIsWrittenWhenCallingExtractAllContents()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            fakeIterator.IsDirectoryReturn = true;
            fakeIterator.CurrentFileNameReturn = "FakePath" + Path.DirectorySeparatorChar;
            fakeIterator.ExtractAllContents(tempPath);

            //Test file has been created
            string targetPath = Path.Combine(tempPath, fakeIterator.CurrentFileName());
            Assert.IsFalse(File.Exists(targetPath), "No files exist");
            Assert.IsTrue(Directory.Exists(targetPath), "Directories exist");

            //No files - just a directory
            Assert.IsTrue(Directory.GetFiles(tempPath).Length < 1, "No file in the directory" );

            //Check it's a directory
            Assert.IsTrue((File.GetAttributes(targetPath) & FileAttributes.Directory) == FileAttributes.Directory, "Has directory attributes");

            Directory.Delete(tempPath, true);
        }

    }
}
