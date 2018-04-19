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
using System.IO;
using System.Xml;
using NUnit.Framework;
using XenAdmin.TestResources;
using XenAdmin.XenSearch;

namespace XenAdminTests.SearchTests
{
    // This test fixture tests that all the searches have the expected results, where the
    // expected results are pre-saved in the file XenAdmin\TestResources\results2.xml.
    // See comment at XenAdmin.TestResources.XenSearchQueryTest.SaveResults() for how to
    // create this file.
    [TestFixture, Category(TestCategories.UICategoryB)]
    class ExpectedResults : MainWindowTester
    {
        private readonly Dictionary<string, XmlNode> expectedResults = new Dictionary<string, XmlNode>();

        protected virtual string resultsFileName { get { return "searchresults.xml"; } }
        
        [OneTimeSetUp]
        public void LoadResultsFile()
        {
            //You can enable the below line to get the search results written out if new ones are required
            //XenSearchQueryTest.SaveResults("searchresults.xml");
            string fileName = TestResource(resultsFileName);
            
            XmlDocument doc;
            using(StreamReader stream = new StreamReader(fileName))
            {
                doc = new XmlDocument();
                doc.Load(stream);
            }
            
            foreach (XmlNode element in doc.FirstChild.ChildNodes)
            {
                if (element.Attributes != null) 
                    expectedResults[element.Attributes["uuid"].Value] = element;
                else
                    Assert.Fail("Element attributes were null while constructing data");
            }
                
        }

        public void RunTest()
        {
            Assert.AreEqual(Search.Searches.Length, expectedResults.Count, "Wrong number of searches in results file " + resultsFileName);
            foreach (Search search in Search.Searches)
            {
                string uuid = search.UUID;
                Assert.IsTrue(expectedResults.ContainsKey(uuid), "Couldn't find search with uuid " + uuid + " in results file " + resultsFileName);
                Search searcher = search;
                MW(() => ComparerAdapter.CompareResults(searcher, expectedResults[uuid]));
            }
        }
    }
}
