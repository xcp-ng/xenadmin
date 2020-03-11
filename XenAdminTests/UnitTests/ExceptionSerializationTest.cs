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
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using XenAdmin.Core;
using XenAPI;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    class ExceptionSerializationTest
    {
        private List<string> errorDescriptions = new List<string> { "AN_ERROR_CODEE", "Error description", "Some text"};
        private List<string> friendlyErrorDescriptions = new List<string> { "CANNOT_ADD_VLAN_TO_BOND_SLAVE", "Cannot add..."};
        private string errorText = "An Error has occured";

        [Test]
        public void TestEmptyFailure()
        {
            var failure = new Failure();
            TestFailureSeralization(failure);
        }

        [Test]
        public void TestInnerExceptionFailure()
        {
            var failure = new Failure(errorText, new Exception(errorText));
            TestFailureSeralization(failure);
        }

        [Test]
        public void TestSimpleFailure()
        {
            var failure = new Failure(errorText);
            TestFailureSeralization(failure);
        }

        [Test]
        public void TestFailureWithErrorDescriptions()
        {
            var failure = new Failure(errorDescriptions);
            TestFailureSeralization(failure);
        }

        [Test]
        public void TestFailureWithFriendlyErrorNames()
        {
            var failure = new Failure(friendlyErrorDescriptions);
            TestFailureSeralization(failure);
        }

        private static void TestFailureSeralization(Failure failure)
        {
            Failure deserializedFailure;

            // Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, failure);
                ms.Seek(0, 0);
                deserializedFailure = (Failure)(bf.Deserialize(ms));
            }

            // Check that properties are preserved
            Assert.AreEqual(failure.Message, deserializedFailure.Message, "Message is different");
            Assert.AreEqual(failure.ShortMessage, deserializedFailure.ShortMessage, "ShortMessage is different");
            if (failure.ErrorDescription != null)
            {
                Assert.IsNotNull(deserializedFailure.ErrorDescription);
                Assert.AreEqual(failure.ErrorDescription.Count, deserializedFailure.ErrorDescription.Count,
                                "ErrorDescription count is different");
                for (int i = 0; i < failure.ErrorDescription.Count; i++)
                {
                    Assert.AreEqual(failure.ErrorDescription[i], deserializedFailure.ErrorDescription[i],
                                    string.Format("ErrorDescription[{0}] count is different", i));
                }
            }
            else
            {
                Assert.IsNull(deserializedFailure.ErrorDescription);
            }

            if (failure.InnerException != null)
            {
                Assert.IsNotNull(deserializedFailure.InnerException);
                Assert.AreEqual(failure.InnerException.Message, deserializedFailure.InnerException.Message, "Message is different");
            }
            else
            {
                Assert.IsNull(deserializedFailure.InnerException);
            }

        }

        [Test]
        public void TestTooManyRedirectsException()
        {
            var exception = new HTTP.TooManyRedirectsException(2, new Uri("http://www.someurl.com"));
            HTTP.TooManyRedirectsException deserializedException;

            // Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, exception);
                ms.Seek(0, 0);
                deserializedException = (HTTP.TooManyRedirectsException)(bf.Deserialize(ms));
            }

            // Check that properties are preserved
            Assert.AreEqual(exception.Message, deserializedException.Message, "Message is different");
        }

        [Test]
        public void TestBadServerResponseException()
        {
            var exception = new HTTP.BadServerResponseException();
            HTTP.BadServerResponseException deserializedException;

            // Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, exception);
                ms.Seek(0, 0);
                deserializedException = (HTTP.BadServerResponseException)(bf.Deserialize(ms));
            }

            // Check that properties are preserved
            Assert.AreEqual(exception.Message, deserializedException.Message, "Message is different");
        }

        [Test]
        public void TestCancelledException()
        {
            var exception = new HTTP.CancelledException();
            HTTP.CancelledException deserializedException;

            // Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, exception);
                ms.Seek(0, 0);
                deserializedException = (HTTP.CancelledException)(bf.Deserialize(ms));
            }

            // Check that properties are preserved
            Assert.AreEqual(exception.Message, deserializedException.Message, "Message is different");
        }

        [Test]
        public void TestEventNextBlockedException()
        {
            var exception = new EventFromBlockedException();
            EventFromBlockedException deserializedException;

            // Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, exception);
                ms.Seek(0, 0);
                deserializedException = (EventFromBlockedException)(bf.Deserialize(ms));
            }

            // Check that properties are preserved
            Assert.AreEqual(exception.Message, deserializedException.Message, "Message is different");
        }
    }
}
