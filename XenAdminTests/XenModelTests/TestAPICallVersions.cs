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
using System.Reflection;
using Moq;
using NUnit.Framework;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    /// <summary>
    /// This unit test relies on a list of call versions. For ease, they are included
    /// as a csv file
    /// 
    /// To update the csv call list:
    /// 1. Build your own version on the api bindings.
    /// 2. Look in the generated code output directory (not the code that's been packaged) eg. build.hg/output/api-bindings
    /// 3. Copy out the "callVersions.csv" and replace the one in TestResources
    /// 
    /// To Add New Actions:
    /// 1. Go down to the container of actions called "Actions"
    /// 2. Add enough unit test code to get your action to run - this is failrly minimal
    /// 
    /// To Add a new version:
    /// 1. Add a const string containing the correct name as extracted into the csv file
    /// 2. Add this to the test ctor
    /// 3. Add a DatabaseInfo member
    /// 4. Check the unsupportedVersions are up to date
    /// </summary>
    [TestFixture]
    internal class TestAPICallVersions : UnitTester_TestFixture
    {
        public TestAPICallVersions()
            : base(rio, miami, symc, orlando, george, midnight, cowley, 
                   boston, sanibel, tampa, tallahassee, clearwater, sarasota, thefuture)
        {
        }

        private void SetupConnection(DatabaseInfo id)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id.name);
            host.Setup(h => h.ProductVersion()).Returns(id.version);
            ObjectManager.MockConnectionFor(id.name).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(host.Object);
        }

        [Test]
        public void CompareCalls()
        {
            ReadCallList();
            foreach (DatabaseInfo info in versionData.Where( v=> !unsupportedVersions.Contains(v.name)))
            {
                SetupConnection(info);
                foreach (AsyncAction myAction in Actions(info.name))
                {
                    List<string> calls = ExtractApiCallList(myAction);
                    VerifyCalls(calls, info);
                }
            }

        }

        #region Data

        private const string tampa = "tampa";
        private const string boston = "boston";
        private const string rio = "rio";
        private const string sanibel = "sanibel";
        private const string cowley = "cowley";
        private const string george = "george";
        private const string miami = "miami";
        private const string midnight = "midnight-ride";
        private const string orlando = "orlando";
        private const string symc = "symc";
        private const string tallahassee = "tallahassee";
        private const string clearwater = "clearwater";
        private const string sarasota = "sarasota";
        private const string thefuture = "Oct 21, 2015";

        /// <summary>
        /// Complete lookup of version numbers
        /// </summary>
        private readonly List<DatabaseInfo> versionData = new List<DatabaseInfo>
                                                              {
                                                                  new DatabaseInfo {name = rio, version = "4.0.0"},
                                                                  new DatabaseInfo {name = miami, version = "4.1.0"},
                                                                  new DatabaseInfo {name = symc, version = "4.1.0"},
                                                                  new DatabaseInfo {name = orlando, version = "5.0.0"},
                                                                  new DatabaseInfo {name = george, version = "5.5.0"},
                                                                  new DatabaseInfo {name = midnight, version = "5.6.0"},
                                                                  new DatabaseInfo {name = cowley, version = "5.6.100"},
                                                                  new DatabaseInfo {name = boston, version = "6.0.0"},
                                                                  new DatabaseInfo {name = sanibel, version = "6.0.2"},
                                                                  new DatabaseInfo {name = tampa, version = "6.1.0"},
                                                                  new DatabaseInfo {name = tallahassee, version = "6.1.1"},
                                                                  new DatabaseInfo {name = clearwater, version = "6.1.2"},
                                                                  new DatabaseInfo {name = sarasota, version = "6.5.0"},
                                                                  new DatabaseInfo {name = thefuture, version = "9999.9999.9999"},
                                                              };

        /// <summary>
        /// These versions are not checked for compatability
        /// </summary>
        private readonly List<string> unsupportedVersions = new List<string>
                                                              {
                                                                  rio, miami, symc, orlando, george
                                                              };

        /// <summary>
        /// Additional calls that are added to the call list or overloads for the list
        /// </summary>
        private readonly Dictionary<string, string> additionalCalls = new Dictionary<string, string>
                                                                          {
                                                                              {"vif_destroy", "rio"}
                                                                          };

        /// <summary>
        /// List of actions to test - you need to add one per test of interest
        /// TODO: Probably best to add this to a factory at some point
        /// TODO: Add more actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IEnumerable<AsyncAction> Actions(string id)
        {
            yield return ActionFactory.MockActionFor(id, typeof (DeleteVIFAction));
            yield return ActionFactory.MockActionFor(id, typeof(AddRemoveRolesAction));
        }

        private Dictionary<string, string> CallsInProductSince { get; set; }
        #endregion

        #region Helper Classes And Methods
        private class DatabaseInfo
        {
            public string name;
            public string version;
        }

        private void ReadCallList()
        {
            CallsInProductSince = additionalCalls;
            using (StreamReader sr = new StreamReader(TestResource("callVersions.csv")))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (String.IsNullOrEmpty(line))
                        continue;

                    string[] words = line.Split(',');
                    if (!CallsInProductSince.ContainsKey(words[0]))
                        CallsInProductSince.Add(words[0], words[1]);
                }
            }
        }

        private void VerifyCalls(List<string> callsMade, DatabaseInfo dbInfo)
        {
            Assert.That(callsMade.Count, Is.GreaterThan(0), "The action made no api calls");

            foreach (string call in callsMade)
            {
                
                if(call.Contains("_get_allowed_operations"))
                    continue;

                if(!CallsInProductSince.ContainsKey(call))
                {
                    string msg = String.Format("Call missing '{0}'- suggest an update of the call list", call);
                    Assert.Fail(msg);
                }
                    
                string closureCall = call;
                string callVersionNumber = versionData.Find(d => d.name == CallsInProductSince[closureCall]).version;

                Assert.IsTrue(Helpers.productVersionCompare(callVersionNumber, dbInfo.version) <= 0,
                              String.Format("Call '{0}' made out of version. It was added in version '{1}' but called in version '{2}'", call, callVersionNumber, dbInfo.version));
            }
        }

        private List<string> ExtractApiCallList(AsyncAction myAction)
        {
            RbacMethodList rbacMethods = new RbacMethodList();
            RbacCollectorProxy.GetProxy(rbacMethods);
            Session session = new Session(RbacCollectorProxy.GetProxy(rbacMethods), myAction.Connection);
            myAction.RunExternal(session);
            return rbacMethods.ConvertAll(c => c.Method.ToLower().Replace('.', '_').Replace("async_", ""));
        }

        private string TestResource(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), @"XenAdminTests\TestResources", name);
        }
        #endregion
    }
}
