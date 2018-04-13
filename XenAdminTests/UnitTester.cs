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

using NUnit.Framework;
using XenAdminTests.UnitTests.UnitTestHelper;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAdminTests.XenModelTests;

namespace XenAdminTests
{
    public class UnitTester : ManagedTester
    {
        public UnitTester(params string[] databaseNames)
            : base(new MockObjectManager(), databaseNames)
        {
        }

        protected MockObjectManager ObjectManager { get { return objectManager as MockObjectManager; } }

        /// <summary>
        /// Factory for mocked up types that are composite or require more complex setup
        /// </summary>
        protected MockObjectBuilderFactory ObjectFactory { get { return new MockObjectBuilderFactory(ObjectManager); } }

        /// <summary>
        /// Factory for mocked up actions
        /// </summary>
        protected MockActionFactory ActionFactory{ get { return new MockActionFactory(ObjectManager); }  }
    }

    [Category(TestCategories.Unit)]
    public abstract class UnitTester_TestFixture : UnitTester
    {
        protected UnitTester_TestFixture(params string[] databases)
            : base(databases)
        { }

        [OneTimeSetUp]
        public void SetUp()
        {
            Setup();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Convenience class so for single connection unit test fixtures
    /// you can simply use the default constructor. The connection id is set here
    /// </summary>
    public abstract class UnitTester_SingleConnectionTestFixture : UnitTester_TestFixture
    {
        protected const string id = "test";
        protected UnitTester_SingleConnectionTestFixture() : base(id){}
    }
}
