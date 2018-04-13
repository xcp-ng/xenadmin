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
using System.Text.RegularExpressions;
using NUnit.Framework;
using XenAdmin.Wizards.DRWizards;

namespace XenAdminTests.WizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DRFailoverWizardSummaryReportTests
    {

        private SummaryReport summaryReport;

        [SetUp]
        public void PerTestSetUp()
        {
            summaryReport = new SummaryReport();
        }


        [Test]
        public void AddingValidRows()
        {
            List<string> linesToAdd = new List<string>()
                                          {
                                             "line 1",
                                             "line 2",
                                             "line 3",
                                             "line 4",
                                             "line 5",
                                          };

            summaryReport.AddLine(linesToAdd[0]);
            summaryReport.AddLine(linesToAdd[1], 1);
            summaryReport.AddLine(linesToAdd[2], 0);
            summaryReport.AddLine(linesToAdd[3], 1, true);
            summaryReport.AddLine(linesToAdd[4], 0, true);

            string outputString = summaryReport.ToString();
            CheckForStringDecorations(outputString);

            foreach (string line in linesToAdd)
            {
                Assert.IsTrue(outputString.Contains(line), "Line: \"" + line + "\" missing");
            }
        }

        [Test]
        public void AddingNoLinesFailsGracefully()
        {
            Assert.AreEqual(string.Empty, 
                            summaryReport.ToString(), 
                            "Adding no rows did not produce an empty string");
        }

        [Test, SetCulture("en-EN")]
        public void AddingNullStringsAddsJustDecorations()
        {
            summaryReport.AddLine(null);
            summaryReport.AddLine(null, 1);
            summaryReport.AddLine(null, 0);
            summaryReport.AddLine(null, 1, true);
            summaryReport.AddLine(null, 0, true);

            string outputString = summaryReport.ToString();
            CheckForStringDecorations(outputString);
            
        }

        private void CheckForStringDecorations(string outputString)
        {
            Assert.AreEqual(2, Regex.Matches(outputString, "  [\u2022]").Count, "2 spaced bullet points not found in " + outputString);
            Assert.AreEqual(5, Regex.Matches(outputString, "\r\n").Count, "5 new lines not found in " + outputString);
            Assert.AreEqual(2, Regex.Matches(outputString,
                                             "[a-zA-Z]{3} ([0-9]|[0-9][0-9]), [0-9]{4} ([0-9]|[0-9][0-9]):[0-9]{2}:[0-9]{2} [AP]M -").Count,
                                             "2 date stamps not found in " + outputString);
        }
    }
}
