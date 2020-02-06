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
using XenAPI;
using Newtonsoft.Json;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class ConverterTests
    {
        #region Private methods

        private object InvokeReadJson(JsonConverter converter, Type objType, string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
               return converter.ReadJson(reader, objType, null, new JsonSerializer());
            }
        }

        private string InvokeWriteJson(JsonConverter converter, object value)
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                converter.WriteJson(writer, value, new JsonSerializer());
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());

            }
        }

        #endregion

        [Test]
        public void Test_XenRefConverter()
        {
            var obj = new XenRef<VM>("OpaqueRef:123");
            var json = "\"OpaqueRef:123\"";

            var converter = new XenRefConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(XenRef<VM>), json) as XenRef<VM>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj, actualObj);
        }
        
        [Test]
        public void Test_XenRefListConverter()
        {
            var obj = new List<XenRef<VM>>
            {
                new XenRef<VM>("OpaqueRef:123"),
                new XenRef<VM>("OpaqueRef:456"),
                new XenRef<VM>("OpaqueRef:789")
            };
            var json = "[\"OpaqueRef:123\",\"OpaqueRef:456\",\"OpaqueRef:789\"]";

            var converter = new XenRefListConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(List<XenRef<VM>>), json) as List<XenRef<VM>>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj, actualObj);
        }

        [Test]
        public void Test_XenRefXenObjectMapConverter()
        {
            var obj = new Dictionary<XenRef<Blob>, Blob>
            {
                {
                    new XenRef<Blob>("OpaqueRef:123"),
                    new Blob
                    {
                        uuid = "abc",
                        name_label = "blob1",
                        name_description = "descr1",
                        size = 123,
                        pubblic = true,
                        last_updated = new DateTime(2018,02,14,10,55,00),
                        mime_type = "type1"
                    }
                },
                {
                    new XenRef<Blob>("OpaqueRef:456"),
                    new Blob
                    {
                        uuid = "def",
                        name_label = "blob2",
                        size = 456,
                        pubblic = false,
                        last_updated = new DateTime(2018,02,14,10,55,00)
                    }
                }
            };

            var json = "{\"OpaqueRef:123\":{\"uuid\":\"abc\",\"name_label\":\"blob1\",\"name_description\":\"descr1\"," +
                       "\"size\":123,\"pubblic\":true,\"last_updated\":\"20180214T10:55:00Z\",\"mime_type\":\"type1\"}," +
                       "\"OpaqueRef:456\":{\"uuid\":\"def\",\"name_label\":\"blob2\",\"name_description\":\"\"," +
                       "\"size\":456,\"pubblic\":false,\"last_updated\":\"20180214T10:55:00Z\",\"mime_type\":\"\"}}";

            var converter = new XenRefXenObjectMapConverter<Blob>();

            //WriteJson is not implemented for this

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<Blob>, Blob>), json)
                as Dictionary<XenRef<Blob>, Blob>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_XenRefLongMapConverter()
        {
            var obj = new Dictionary<XenRef<VM>, long>
            {
                {new XenRef<VM>("OpaqueRef:123"), 123},
                {new XenRef<VM>("OpaqueRef:456"), 456}
            };

            var json = "{\"OpaqueRef:123\":123," +
                       "\"OpaqueRef:456\":456}";

            var converter = new XenRefLongMapConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<VM>, long>), json)
                as Dictionary<XenRef<VM>, long>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_XenRefStringMapConverter()
        {
            var obj = new Dictionary<XenRef<VM>, string>
            {
                {new XenRef<VM>("OpaqueRef:123"), "string1"},
                {new XenRef<VM>("OpaqueRef:456"), "string2"}
            };

            var json = "{\"OpaqueRef:123\":\"string1\"," +
                       "\"OpaqueRef:456\":\"string2\"}";

            var converter = new XenRefStringMapConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<VM>, string>), json)
                as Dictionary<XenRef<VM>, string>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_XenRefStringStringMapMapConverter()
        {
            var obj = new Dictionary<XenRef<VM>, Dictionary<string, string>>
            {
                {
                    new XenRef<VM>("OpaqueRef:123"),
                    new Dictionary<string, string>
                    {
                        {"code1", "message1"},
                        {"code2", "message2"}
                    }
                },
                {
                    new XenRef<VM>("OpaqueRef:456"),
                    new Dictionary<string, string>
                    {
                        {"code3", "message3"},
                        {"code4", "message4"}
                    }
                }
            };

            var json = "{\"OpaqueRef:123\":{\"code1\":\"message1\",\"code2\":\"message2\"}," +
                       "\"OpaqueRef:456\":{\"code3\":\"message3\",\"code4\":\"message4\"}}";

            var converter = new XenRefStringStringMapMapConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<VM>, Dictionary<string, string>>), json)
                as Dictionary<XenRef<VM>, Dictionary<string, string>>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_XenRefXenRefMapConverter()
        {
            var obj = new Dictionary<XenRef<Host>, XenRef<VM>>
            {
                {new XenRef<Host>("OpaqueRef:111"), new XenRef<VM>("OpaqueRef:222")},
                {new XenRef<Host>("OpaqueRef:333"), new XenRef<VM>("OpaqueRef:444")}
            };

            var json = "{\"OpaqueRef:111\":\"OpaqueRef:222\"," +
                       "\"OpaqueRef:333\":\"OpaqueRef:444\"}";

            var converter = new XenRefXenRefMapConverter<Host, VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<Host>, XenRef<VM>>), json)
                as Dictionary<XenRef<Host>, XenRef<VM>>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_XenRefStringSetMapConverter()
        {
            var obj = new Dictionary<XenRef<VM>, string[]>
            {
                {
                    new XenRef<VM>("OpaqueRef:123"),
                    new[] {"string1", "string2"}
                },
                {
                    new XenRef<VM>("OpaqueRef:456"),
                    new[] {"string3", "string4"}
                }
            };

            var json = "{\"OpaqueRef:123\":[\"string1\",\"string2\"]," +
                       "\"OpaqueRef:456\":[\"string3\",\"string4\"]}";

            var converter = new XenRefStringSetMapConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<XenRef<VM>, string[]>), json)
                as Dictionary<XenRef<VM>, string[]>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }

        [Test]
        public void Test_StringXenRefMapConverter()
        {
            var obj = new Dictionary<string, XenRef<VM>>
            {
                {"string1", new XenRef<VM>("OpaqueRef:123")},
                {"string2", new XenRef<VM>("OpaqueRef:456")}
            };

            var json = "{\"string1\":\"OpaqueRef:123\"," +
                       "\"string2\":\"OpaqueRef:456\"}";

            var converter = new StringXenRefMapConverter<VM>();

            var actualJson = InvokeWriteJson(converter, obj);
            Assert.AreEqual(json, actualJson);

            var actualObj = InvokeReadJson(converter, typeof(Dictionary<string, XenRef<VM>>), json)
                as Dictionary<string, XenRef<VM>>;
            Assert.NotNull(actualObj);
            Assert.AreEqual(obj.Keys, actualObj.Keys);
            Assert.AreEqual(obj.Values, actualObj.Values);
        }
    }
}
