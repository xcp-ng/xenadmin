/*
 * Copyright (c) Cloud Software Group, Inc.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

#if BUILD_FOR_TEST
[assembly: InternalsVisibleTo("XenServerTest")]
#endif

namespace XenAPI
{
    internal abstract class CustomJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }
    }


    internal class XenRefConverter<T> : CustomJsonConverter<XenRef<T>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var str = jToken.ToObject<string>();
            return string.IsNullOrEmpty(str) ? null : new XenRef<T>(str);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var reference = JObject.FromObject(value).GetValue("opaque_ref");
            writer.WriteValue(reference);
        }
    }


    internal class XenRefListConverter<T> : CustomJsonConverter<List<XenRef<T>>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var refList = new List<XenRef<T>>();
            foreach (JToken token in jToken.ToArray())
            {
                var str = token.ToObject<string>();
                refList.Add(new XenRef<T>(str));
            }
            return refList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<XenRef<T>>;
            writer.WriteStartArray();
            if (list != null)
                list.ForEach(v => writer.WriteValue(v.opaque_ref));
            writer.WriteEndArray();
        }
    }


    internal class XenRefXenObjectMapConverter<T> : CustomJsonConverter<Dictionary<XenRef<T>, T>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<T>, T>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<T>(property.Name), property.Value.ToObject<T>());

            return dict;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


    internal class XenRefLongMapConverter<T> : CustomJsonConverter<Dictionary<XenRef<T>, long>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<T>, long>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<T>(property.Name), property.Value.ToObject<long>());

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<XenRef<T>, long>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key.opaque_ref);
                    writer.WriteValue(kvp.Value);
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class XenRefStringMapConverter<T> : CustomJsonConverter<Dictionary<XenRef<T>, string>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<T>, string>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<T>(property.Name), property.Value.ToString());

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<XenRef<T>, string>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key.opaque_ref);
                    writer.WriteValue(kvp.Value);
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class XenRefStringStringMapMapConverter<T> : CustomJsonConverter<Dictionary<XenRef<T>, Dictionary<string, string>>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<T>, Dictionary<string, string>>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<T>(property.Name), property.Value.ToObject<Dictionary<string, string>>());

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<XenRef<T>, Dictionary<string, string>>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key.opaque_ref);
                    writer.WriteStartObject();
                    foreach (var valKvp in kvp.Value)
                    {
                        writer.WritePropertyName(valKvp.Key);
                        writer.WriteValue(valKvp.Value);
                    }
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class XenRefXenRefMapConverter<TK, TV> : CustomJsonConverter<Dictionary<XenRef<TK>, XenRef<TV>>> where TK : XenObject<TK> where TV : XenObject<TV>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<TK>, XenRef<TV>>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<TK>(property.Name), new XenRef<TV>(property.Value.ToString()));

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<XenRef<TK>, XenRef<TV>>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key.opaque_ref);
                    writer.WriteValue(kvp.Value.opaque_ref);
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class XenRefStringSetMapConverter<T> : CustomJsonConverter<Dictionary<XenRef<T>, string[]>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<XenRef<T>, string[]>();

            foreach (JProperty property in jToken)
                dict.Add(new XenRef<T>(property.Name), property.Value.ToObject<string[]>());

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<XenRef<T>, string[]>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key.opaque_ref);
                    writer.WriteStartArray();
                    foreach (var v in kvp.Value)
                        writer.WriteValue(v);
                    writer.WriteEndArray();

                }
            }
            writer.WriteEndObject();
        }
    }


    internal class StringXenRefMapConverter<T> : CustomJsonConverter<Dictionary<string, XenRef<T>>> where T : XenObject<T>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<string, XenRef<T>>();

            foreach (JProperty property in jToken)
                dict.Add(property.Name, new XenRef<T>(property.Value.ToString()));

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<string, XenRef<T>>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteValue(kvp.Value.opaque_ref);
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class StringStringMapConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var dict = new Dictionary<string, string>();

            foreach (JProperty property in jToken)
                dict.Add(property.Name, property.Value == null ? null : property.Value.ToString());

            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, string>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as Dictionary<string, string>;
            writer.WriteStartObject();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteValue(kvp.Value ?? "");
                }
            }
            writer.WriteEndObject();
        }
    }


    internal class XenEventConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Event).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);

            var id = jToken["id"];
            var timestamp = jToken["timestamp"];
            var operation = jToken["operation"];
            var opaqueRef = jToken["ref"];
            var class_ = jToken["class"];
            var snapshot = jToken["snapshot"];

            var newEvent = new Event
            {
                id = id == null ? 0 : id.ToObject<long>(),
                timestamp = timestamp == null ? null : timestamp.ToObject<string>(),
                operation = operation == null ? null : operation.ToObject<string>(),
                opaqueRef = opaqueRef == null ? null : opaqueRef.ToObject<string>(),
                class_ = class_ == null ? null : class_.ToObject<string>()
            };

            Type typ = Type.GetType(string.Format("XenAPI.{0}", newEvent.class_), false, true);
            newEvent.snapshot = snapshot == null ? null : snapshot.ToObject(typ, serializer);
            return newEvent;
        }
    }


    internal class XenDateTimeConverter : IsoDateTimeConverter
    {
        string [] DateFormatsUtc = {
            // dashes and colons
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",

            // no dashes, with colons
            "yyyyMMddTHH:mm:ssZ",
            "yyyyMMddTHH:mm:ss.fffZ",

            // no dashes
            "yyyyMMddTHHmmssZ",
            "yyyyMMddTHHmmss.fffZ",
        };

        string[] DateFormatsLocal =
        {
            // no dashes
            "yyyyMMddTHHmmss.fffzzzz",
            "yyyyMMddTHHmmss.fffzzz",
            "yyyyMMddTHHmmss.fffzz",
            "yyyyMMddTHHmmss.fff",

            "yyyyMMddTHHmmsszzzz",
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmss",

            // no dashes, with colons
            "yyyyMMddTHH:mm:ss.fffzzzz",
            "yyyyMMddTHH:mm:ss.fffzzz",
            "yyyyMMddTHH:mm:ss.fffzz",
            "yyyyMMddTHH:mm:ss.fff",

            "yyyyMMddTHH:mm:sszzzz",
            "yyyyMMddTHH:mm:sszzz",
            "yyyyMMddTHH:mm:sszz",
            "yyyyMMddTHH:mm:ss",

            // dashes and colons
            "yyyy-MM-ddTHH:mm:ss.fffzzzz",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "yyyy-MM-ddTHH:mm:ss.fffzz",
            "yyyy-MM-ddTHH:mm:ss.fff",

            "yyyy-MM-ddTHH:mm:sszzzz",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ss",
        };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // JsonReader may have already parsed the date for us
            if (reader.ValueType != null && reader.ValueType == typeof(DateTime))
            {
                return reader.Value;
            }

            var str = JToken.Load(reader).ToString();

            if (DateTime.TryParseExact(str, DateFormatsUtc, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
                return result;

            if (DateTime.TryParseExact(str, DateFormatsLocal, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
                return result;

            return DateTime.MinValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime)
            {
                dateTime = dateTime.ToUniversalTime();
                var text = dateTime.ToString(DateFormatsUtc[0], CultureInfo.InvariantCulture);
                writer.WriteValue(text);
                return;
            }

            base.WriteJson(writer, value, serializer);
        }
    }


    internal class XenEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            return Helper.EnumParseDefault(objectType, jToken.ToObject<string>());
        }
    }
}
