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
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Wizards.ExportWizard.ApplianceChecks;

namespace XenAdminTests.WizardTests.ExportWizard
{
    public class ApplianceExistsCheckTests : UnitTester_TestFixture
    {
        #region Test Helpers
        private class ApplianceExistsCheckStaticOverrides : ApplianceExistsCheck
        {
            public ApplianceExistsCheckStaticOverrides(string destinationDirectory, string fileName, FileExtension extension)
                : base(destinationDirectory, fileName, extension)
            {
                FileExistsCalls = DirectoryExistsCalls = 0;
            }

            private bool directoryExistsValue;
            public bool DirectoryExistsValue
            {
                set { directoryExistsValue = value; }
                private get
                {
                    DirectoryExistsCalls++;
                    return directoryExistsValue;
                }
            }

            private bool fileExistsValue;
            public bool FileExistsValue
            {
                set { fileExistsValue = value; }
                private get
                {
                    FileExistsCalls++;
                    return fileExistsValue;
                }
            }

            public int DirectoryExistsCalls { get; private set; }
            public int FileExistsCalls { get; private set; }

            protected override bool DirectoryExists(string directory)
            {
                return DirectoryExistsValue;
            }

            protected override bool FileExists(string directory)
            {
                return FileExistsValue;
            }
        }

        public class TestCase
        {
            public bool DirFound { get; set; }
            public bool FileFound { get; set; }
            public bool Valid { get; set; }
            public override string ToString()
            {
                return String.Format("DirFound: {0}; File found: {1}; Valid: {2}", DirFound, FileFound, Valid);
            }
        }
        #endregion

        #region Test Data
        private const string directoryName = @"C:\Some\Fake\Dir\Name";
        private const string fileName = "fileName";
        private readonly string errorMessage = Messages.EXPORT_APPLIANCE_PAGE_ERROR_APP_EXISTS;
        private readonly string noErrorMessage = string.Empty;

        private IEnumerable<TestCase> TestCases
        {
            get
            {
                yield return new TestCase
                 {
                     DirFound = true,
                     FileFound = true,
                     Valid = false
                 };

                yield return new TestCase
                {
                    DirFound = false,
                    FileFound = true,
                    Valid = false
                };

                yield return new TestCase
                {
                    DirFound = true,
                    FileFound = false,
                    Valid = true
                };

                yield return new TestCase
                {
                    DirFound = false,
                    FileFound = false,
                    Valid = true
                };
            }
        } 
        #endregion

        [Test, TestCaseSource("TestCases")]
        public void TestOvfOvaValidation(TestCase tc)
        {
            ApplianceExistsCheckStaticOverrides checker = new ApplianceExistsCheckStaticOverrides(directoryName, fileName, ApplianceCheck.FileExtension.ovaovf)
                                                              {
                                                                  DirectoryExistsValue = tc.DirFound,
                                                                  FileExistsValue = tc.FileFound
                                                              };
            checker.Validate();
            Assert.That(checker.IsValid, Is.EqualTo(tc.Valid), "Is valid test -> " + tc);
            Assert.That(checker.DirectoryExistsCalls, Is.EqualTo(1), "dir exists calls -> " + tc);
            Assert.That(checker.FileExistsCalls, Is.InRange(1,2), "file exists calls -> " + tc);
        }

        [Test, TestCaseSource("TestCases")]
        public void TestXvaValidation(TestCase tc)
        {
            ApplianceExistsCheckStaticOverrides checker = new ApplianceExistsCheckStaticOverrides(directoryName, fileName, ApplianceCheck.FileExtension.xva)
            {
                DirectoryExistsValue = tc.DirFound,
                FileExistsValue = tc.FileFound
            };
            checker.Validate();
            Assert.That(checker.IsValid, Is.EqualTo(tc.Valid), "Is valid test -> " + tc);
            Assert.That(checker.DirectoryExistsCalls, Is.EqualTo(0), "dir exists calls -> " + tc);
            Assert.That(checker.FileExistsCalls, Is.EqualTo(1), "file exists calls -> " + tc);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ErrorMessagesValidation(bool valid)
        {
            ApplianceExistsCheckStaticOverrides checker = new ApplianceExistsCheckStaticOverrides(directoryName,
                                                                                                  fileName,
                                                                                                  ApplianceCheck.FileExtension.xva)
                                                              {
                                                                  DirectoryExistsValue = !valid,
                                                                  FileExistsValue = !valid
                                                              };
            checker.Validate();
            Assert.That(checker.IsValid, Is.EqualTo(valid), "Is valid");
            string error = valid ? noErrorMessage : errorMessage;
            Assert.That(checker.ErrorReason, Is.EqualTo(error), "Error message");
        }

        [Test]
        public void DefaultCase()
        {
            ApplianceCheck checker = new ApplianceExistsCheck(directoryName, fileName,
                                                              ApplianceCheck.FileExtension.ovaovf);
            Assert.That(checker.IsValid, Is.False, "Is valid");
            Assert.That(checker.ErrorReason, Is.EqualTo(errorMessage), "error message");
        }

        [Test]
        public void NullPaths()
        {
            ApplianceCheck checker = new ApplianceExistsCheck(null, null,
                                                              ApplianceCheck.FileExtension.ovaovf);
            Assert.That(()=> checker.Validate(), Throws.ArgumentNullException);
        }

    }
}
