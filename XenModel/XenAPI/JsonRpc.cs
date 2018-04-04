/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace XenAPI
{
    public enum JsonRpcVersion
    {
        v1,
        v2
    }

    internal abstract class JsonRequest
    {
        protected JsonRequest(int id, string method, JToken parameters)
        {
            this.Id = id;
            this.Method = method;
            this.Parameters = parameters;
        }

        public static JsonRequest Create(JsonRpcVersion jsonRpcVersion, int id, string method, JToken parameters)
        {
            switch (jsonRpcVersion)
            {
                case JsonRpcVersion.v2:
                    return new JsonRequestV2(id, method, parameters);
                default:
                    return new JsonRequestV1(id, method, parameters);
            }
        }

        /// <summary>
        /// Unique call id. Can be null in JSON_RPC v2.0, but xapi disallows it.
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public int Id { get; private set; }

        /// <summary>
        /// The API function to call.
        /// </summary>
        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; private set; }

        /// <summary>
        /// Any parameters, optional in JSON-RPC v2.0.
        /// </summary>
        [JsonProperty("params", Required = Required.Always)]
        public JToken Parameters { get; private set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    internal class JsonRequestV1 : JsonRequest
    {
        public JsonRequestV1(int id, string method, JToken parameters)
            : base(id, method, parameters)
        {
        }
    }

    internal class JsonRequestV2 : JsonRequest
    {
        public JsonRequestV2(int id, string method, JToken parameters)
            : base(id, method, parameters)
        {
        }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string JsonRPC
        {
            get { return "2.0"; }
        }
    }


    internal abstract class JsonResponse<T>
    {
        [JsonProperty("id", Required = Required.AllowNull)] public int Id = 0;

        [JsonProperty("result", Required = Required.Default)] public T Result = default(T);

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    internal class JsonResponseV1<T> : JsonResponse<T>
    {
        [JsonProperty("error", Required = Required.AllowNull)] public object Error = null;
    }

    internal class JsonResponseV2<T> : JsonResponse<T>
    {
        [JsonProperty("error", Required = Required.DisallowNull)] public JsonResponseV2Error Error = null;

        [JsonProperty("jsonrpc", Required = Required.Always)] public string JsonRpc = null;
    }

    internal class JsonResponseV2Error
    {
        [JsonProperty("code", Required = Required.Always)] public int Code = 0;

        [JsonProperty("message", Required = Required.Always)] public string Message = null;

        [JsonProperty("data", Required = Required.Default)] public JToken Data = null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }


    public partial class JsonRpcClient
    {
        private int _globalId;

        public JsonRpcClient(string baseUrl)
        {
            Url = baseUrl;
            JsonRpcUrl = new Uri(new Uri(baseUrl), "/jsonrpc").ToString();
            JsonRpcVersion = JsonRpcVersion.v1;
        }

        /// <summary>
        /// Fired when a request has been serialized taking as a parameter the call name in RELEASE and the
        /// Json string in DEBUG mode.
        /// IMPORTANT: the latter may contain sensitive data, so handle it carefully.
        /// </summary>
        public event Action<string> RequestEvent;

        public JsonRpcVersion JsonRpcVersion { get; set; }
        public string UserAgent { get; set; }
        public bool KeepAlive { get; set; }
        public IWebProxy WebProxy { get; set; }
        public int Timeout { get; set; }
        public string ConnectionGroupName { get; set; }

        public string Url { get; private set; }

        public string JsonRpcUrl { get; private set; }

        private void Rpc(string callName, JToken parameters, JsonSerializer serializer)
        {
            Rpc<object>(callName, parameters, serializer);
        }

        private T Rpc<T>(string callName, JToken parameters, JsonSerializer serializer)
        {
            // Note that the following method handles an overflow condition by wrapping;
            // if the _globalId reaches Int32.MaxValue, _globalId + 1 starts over from
            //  Int32.MinValue and no exception is thrown.
            var id = Interlocked.Increment(ref _globalId);

            JsonRequest request = JsonRequest.Create(JsonRpcVersion, id, callName, parameters);
            var webRequest = (HttpWebRequest)WebRequest.Create(JsonRpcUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Accept = "application/json";
            if (Timeout > 0)
                webRequest.Timeout = Timeout;
            if (WebProxy != null)
                webRequest.Proxy = WebProxy;
            if (KeepAlive)
                webRequest.KeepAlive = KeepAlive;
            if (!string.IsNullOrEmpty(UserAgent))
                webRequest.UserAgent = UserAgent;
            if (!string.IsNullOrEmpty(ConnectionGroupName))
                webRequest.ConnectionGroupName = ConnectionGroupName;

            // for performance reasons it's preferable to deserialize directly
            // from the Stream rather than allocating strings inbetween
            // therefore the latter will be done only in DEBUG mode

#if DEBUG
            var settings = CreateSettings(serializer.Converters);
#endif

            using (var str = webRequest.GetRequestStream())
            using (var sw = new StreamWriter(str))
            {
#if DEBUG
                string jsonReq = JsonConvert.SerializeObject(request, settings);
                if (RequestEvent != null)
                    RequestEvent(jsonReq);
                sw.Write(jsonReq);
#else
                if (RequestEvent != null)
                    RequestEvent(callName);
                serializer.Serialize(sw, request);
#endif
                sw.Flush();
            }

            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode != HttpStatusCode.OK)
                    throw new WebException(webResponse.StatusCode.ToString());

                using (var str = webResponse.GetResponseStream())
                {
                    if (str == null)
                        throw new WebException();

                    using (var responseReader = new StreamReader(str))
                    {
                        switch (JsonRpcVersion)
                        {
                            case JsonRpcVersion.v2:
#if DEBUG
                                string json2 = responseReader.ReadToEnd();
                                var res2 = JsonConvert.DeserializeObject<JsonResponseV2<T>>(json2, settings);
#else
                                var res2 = (JsonResponseV2<T>)serializer.Deserialize(responseReader, typeof(JsonResponseV2<T>));
#endif
                                if (res2.Error != null)
                                {
                                    var descr = new List<string> {res2.Error.Message};
                                    descr.AddRange(res2.Error.Data.ToObject<string[]>());
                                    throw new Failure(descr);
                                }
                                return res2.Result;
                            default:
#if DEBUG
                                string json1 = responseReader.ReadToEnd();
                                var res1 = JsonConvert.DeserializeObject<JsonResponseV1<T>>(json1, settings);
#else
                                var res1 = (JsonResponseV1<T>)serializer.Deserialize(responseReader, typeof(JsonResponseV1<T>));
#endif
                                if (res1.Error != null)
                                    throw new Failure(res1.Error as string[]);
                                return res1.Result;
                        }
                    }
                }
            }
        }

        private JsonSerializerSettings CreateSettings(IList<JsonConverter> converters)
        {
            return new JsonSerializerSettings
            {
#if DEBUG
                Formatting = Formatting.Indented,
#endif
                Converters = converters,
                DateParseHandling = DateParseHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private JsonSerializer CreateSerializer(IList<JsonConverter> converters)
        {
            var settings = CreateSettings(converters);
            return JsonSerializer.Create(settings);
        }
    }
}
