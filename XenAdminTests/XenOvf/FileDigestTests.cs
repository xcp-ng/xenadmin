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

using System.Collections.Generic;
using NUnit.Framework;
using XenOvf;

namespace XenAdminTests.XenOvf
{
    public class FileDigestTests : UnitTester_SingleConnectionTestFixture
    {

        public class TestCase
        {
            public string ToParse { get; set; }
            public string Name { get; set; }
            public string AlgorithmString { get; set; }
            public string DigestString { get; set; }
        }

        private class AccessibleFileDigest : FileDigest
        {
            public AccessibleFileDigest(string line) : base(line)
            {
            }

            public string DigestString
            {
                get { return DigestAsString; }
            }
        }

        private static IEnumerable<TestCase> TestCases
        {
            get
            {
                yield return new TestCase
                 {
                     ToParse = "SHA1(certname.mf)=b51121)",
                     Name = "certname.mf",
                     AlgorithmString = "SHA1",
                     DigestString = ""
                 };
                yield return new TestCase //CA-89555 file names with parenthesis
                {
                    ToParse = "SHA2(certname (32-bit).mf)=b51121)",
                    Name = "certname (32-bit).mf",
                    AlgorithmString = "SHA2",
                    DigestString = ""
                };
                yield return new TestCase //CA-89555 file names with parenthesis
                {
                    ToParse = "SHA2(certname (32-bit).mf)=b51121",
                    Name = "certname (32-bit).mf",
                    AlgorithmString = "SHA2",
                    DigestString = "b51121"
                };
                yield return new TestCase //CA-89555 file names with parenthesis
                {
                    ToParse = "SHA2(cert-56-Name (((((3 2-bit ))))).m(ui))=b51121)",
                    Name = "cert-56-Name (((((3 2-bit ))))).m(ui)",
                    AlgorithmString = "SHA2",
                    DigestString = ""
                };
                yield return new TestCase
                {
                    ToParse = "TIMTOM(bob)",
                    Name = "bob",
                    AlgorithmString = "TIMTOM",
                    DigestString = ""
                };
                yield return new TestCase //This is a real example
                {
                    ToParse = "SHA1(Database server.mf)= 1c274a2139a50eae131342b4a11ce3d12121a46321394301fff3fcab68dbc424cb164c87984b5206a5a41e56ac8b96e5b93806362062dbca1ad29d79b6e0581408525f9192e8154a8eae2cd45e3cae16deebceee1ae32c1190b87f036b1724efb909e636a5ad552bae654bc614d7f391368ff9071cf018275fa7bea472ebf96f9400adaf26f5dfe3c3769ad6317ccb4cca0e0b6829e8f4621e767e75a0eaa06000925001da152e205809fc644caa66fca6796f012c9084aa94a505249f900c0c8b32c871649bfd0ef323c1c94523ca5f17a8e13502cac26d25e6f1f504d81a4b5551828c9fdf60c8b155d6919184d1a83d022091955cda56797e20c00e9341f8",
                    Name = "Database server.mf",
                    AlgorithmString = "SHA1",
                    DigestString = "1c274a2139a50eae131342b4a11ce3d12121a46321394301fff3fcab68dbc424cb164c87984b5206a5a41e56ac8b96e5b93806362062dbca1ad29d79b6e0581408525f9192e8154a8eae2cd45e3cae16deebceee1ae32c1190b87f036b1724efb909e636a5ad552bae654bc614d7f391368ff9071cf018275fa7bea472ebf96f9400adaf26f5dfe3c3769ad6317ccb4cca0e0b6829e8f4621e767e75a0eaa06000925001da152e205809fc644caa66fca6796f012c9084aa94a505249f900c0c8b32c871649bfd0ef323c1c94523ca5f17a8e13502cac26d25e6f1f504d81a4b5551828c9fdf60c8b155d6919184d1a83d022091955cda56797e20c00e9341f8"
                };
                yield return new TestCase
                {
                    ToParse = "SHA1(certname.mf)= b51121",
                    Name = "certname.mf",
                    AlgorithmString = "SHA1",
                    DigestString = "b51121"
                };
            }
        }

        [Test, TestCaseSource("TestCases")]
        public void FileDigestNameExtraction(TestCase tc)
        {
            FileDigest fd = new FileDigest(tc.ToParse);
            Assert.That(fd.Name, Is.EqualTo(tc.Name));
        }

        [Test, TestCaseSource("TestCases")]
        public void FileDigestAlgorithmExtraction(TestCase tc)
        {
            FileDigest fd = new FileDigest(tc.ToParse);
            Assert.That(fd.AlgorithmName, Is.EqualTo(tc.AlgorithmString));
        }

        [Test, TestCaseSource("TestCases")]
        public void FileDigestDigestExtraction(TestCase tc)
        {
            AccessibleFileDigest fd = new AccessibleFileDigest(tc.ToParse);
            Assert.That(fd.DigestString, Is.EqualTo(tc.DigestString));
        }
    }
}
