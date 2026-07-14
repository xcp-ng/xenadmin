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
using System.IO;
using System.Net;
#if (NET8_0_OR_GREATER)
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
#endif
using System.Net.Security;
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
            Id = id;
            Method = method;
            Parameters = parameters;
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

        public abstract string JsonRPC { get;}

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

        [JsonIgnore]
        public override string JsonRPC => "1.0";
    }

    internal class JsonRequestV2 : JsonRequest
    {
        public JsonRequestV2(int id, string method, JToken parameters)
            : base(id, method, parameters)
        {
        }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public override string JsonRPC => "2.0";
    }


    internal abstract class JsonResponse<T>
    {
        [JsonProperty("id", Required = Required.AllowNull)] public int Id { get; set; }

        [JsonProperty("result", Required = Required.Default)] public T Result { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    internal class JsonResponseV1<T> : JsonResponse<T>
    {
        [JsonProperty("error", Required = Required.AllowNull)] public JToken Error { get; set; }
    }

    internal class JsonResponseV2<T> : JsonResponse<T>
    {
        [JsonProperty("error", Required = Required.DisallowNull)] public JsonResponseV2Error Error { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)] public string JsonRpc { get; set; }
    }

    internal class JsonResponseV2Error
    {
        [JsonProperty("code", Required = Required.Always)] public int Code { get; set; }

        [JsonProperty("message", Required = Required.Always)] public string Message { get; set; }

        [JsonProperty("data", Required = Required.Default)] public JToken Data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }


    public partial class JsonRpcClient
    {
        private int _globalId;
        private string _userAgent;

#if (NET8_0_OR_GREATER)
        private static readonly Type ClassType = typeof(JsonRpcClient);
        private static readonly System.Reflection.AssemblyName ClassAssemblyName = ClassType?.Assembly?.GetName();
        private static readonly ActivitySource source = new ActivitySource(ClassAssemblyName.Name + "." + ClassType?.FullName, ClassAssemblyName.Version?.ToString());

        // Follow naming conventions from OpenTelemetry.SemanticConventions
        // Not yet on NuGet though:
        // dotnet add package OpenTelemetry.SemanticConventions
        private static class RpcAttributes
        {
            public const string AttributeRpcMethod = "rpc.method";
            public const string AttributeRpcSystem = "rpc.system";
            public const string AttributeRpcService = "rpc.service";
            public const string AttributeRpcJsonrpcErrorCode = "rpc.jsonrpc.error_code";
            public const string AttributeRpcJsonrpcErrorMessage = "rpc.jsonrpc.error_message";
            public const string AttributeRpcJsonrpcRequestId = "rpc.jsonrpc.request_id";
            public const string AttributeRpcJsonrpcVersion = "rpc.jsonrpc.version";
            public const string AttributeRpcMessageType = "rpc.message.type";
            
            public static class RpcMessageTypeValues
            {
                public const string Sent = "SENT";
                public const string Received = "RECEIVED";
            }
        }

        private static class ServerAttributes
        {
            public const string AttributeServerAddress = "server.address";
        }

        // not part of the SemanticConventions package
        private const string ValueJsonRpc = "jsonrpc";
        private const string EventRpcMessage = "rpc.message";
#endif

        public JsonRpcClient(string baseUrl)
        {
            Url = baseUrl;
            JsonRpcUrl = new Uri(new Uri(baseUrl), "/jsonrpc").ToString();
            JsonRpcVersion = JsonRpcVersion.v1;
            Timeout = Session.STANDARD_TIMEOUT;
            UserAgent = Session.DefaultUserAgent;
            KeepAlive = true;
            AllowAutoRedirect = true;
        }

        /// <summary>
        /// Fired when a request has been serialized taking as a parameter the call name in RELEASE and the
        /// Json string in DEBUG mode.
        /// IMPORTANT: the latter may contain sensitive data, so handle it carefully.
        /// </summary>
        public event Action<string> RequestEvent;

        public JsonRpcVersion JsonRpcVersion { get; set; }

        public string UserAgent
        {
            get => _userAgent;
            set => _userAgent = string.IsNullOrEmpty(value) ? Session.DefaultUserAgent : value;
        }

        public bool KeepAlive { get; set; }
        public IWebProxy WebProxy { get; set; }
        public int Timeout { get; set; }
        public string ConnectionGroupName { get; set; }
        public Version ProtocolVersion { get; set; }
        public bool Expect100Continue { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public bool PreAuthenticate { get; set; }
        public CookieContainer Cookies { get; set; }

#if (NET8_0_OR_GREATER)
        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateValidationCallback { get; set; }
#else
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
#endif

        public Dictionary<string, string> RequestHeaders { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }

        public string Url { get; private set; }

        public string JsonRpcUrl { get; private set; }

        protected void Rpc(string callName, JToken parameters, JsonSerializer serializer)
        {
            Rpc<object>(callName, parameters, serializer);
        }

        protected virtual T Rpc<T>(string callName, JToken parameters, JsonSerializer serializer)
        {
            // Note that the following method handles an overflow condition by wrapping.
            // If the _globalId reaches Int32.MaxValue, _globalId + 1 starts over from
            // Int32.MinValue and no exception is thrown.
            var id = Interlocked.Increment(ref _globalId);

            JsonRequest request = JsonRequest.Create(JsonRpcVersion, id, callName, parameters);

            // for performance reasons it's preferable to deserialize directly
            // from the Stream rather than allocating strings inbetween
            // therefore the latter will be done only in DEBUG mode
            using (var postStream = new MemoryStream())
            {
#if (NET8_0_OR_GREATER)
                // the semantic convention is $package.$service/$method
                using (Activity activity = source.CreateActivity("XenAPI/" + callName, ActivityKind.Client))
                {
                    activity?.Start();
                    // Set the fields described in the OpenTelemetry Semantic Conventions:
                    // https://opentelemetry.io/docs/specs/semconv/rpc/json-rpc/
                    // https://opentelemetry.io/docs/specs/semconv/rpc/rpc-spans/
                    activity?.SetTag(RpcAttributes.AttributeRpcSystem, ValueJsonRpc);
                    activity?.SetTag(ServerAttributes.AttributeServerAddress, new Uri(Url).Host);
                    activity?.SetTag(RpcAttributes.AttributeRpcMethod, callName);
                    activity?.SetTag(RpcAttributes.AttributeRpcJsonrpcRequestId, id.ToString());
#endif
                    using (var sw = new StreamWriter(postStream))
                    {
#if DEBUG
                        var settings = CreateSettings(serializer.Converters);
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
                        postStream.Seek(0, SeekOrigin.Begin);

                        using (var responseStream = new MemoryStream())
                        {
                            PerformPostRequest(postStream, responseStream);
                            responseStream.Position = 0;

                            using (var responseReader = new StreamReader(responseStream))
                            {
#if (NET8_0_OR_GREATER)
                                activity?.SetTag(RpcAttributes.AttributeRpcJsonrpcVersion, request.JsonRPC);
#endif
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
                                            var descr = new List<string> { res2.Error.Message };
                                            descr.AddRange(res2.Error.Data.ToObject<string[]>());
#if (NET8_0_OR_GREATER)
                                            activity?.SetTag(RpcAttributes.AttributeRpcJsonrpcErrorCode, res2.Error.Code);
                                            activity?.SetTag(RpcAttributes.AttributeRpcJsonrpcErrorMessage, descr);
                                            activity?.SetStatus(ActivityStatusCode.Error);
#endif
                                            throw new Failure(descr);
                                        }
#if (NET8_0_OR_GREATER)
                                        activity?.SetStatus(ActivityStatusCode.Ok);
#endif
                                        return res2.Result;
                                    default:
#if DEBUG
                                        string json1 = responseReader.ReadToEnd();
                                        var res1 = JsonConvert.DeserializeObject<JsonResponseV1<T>>(json1, settings);
#else
                                        var res1 = (JsonResponseV1<T>)serializer.Deserialize(responseReader, typeof(JsonResponseV1<T>));
#endif
                                        var errorArray = res1.Error?.ToObject<string[]>();
                                        if (errorArray != null)
                                        {
#if (NET8_0_OR_GREATER)
                                            activity?.SetStatus(ActivityStatusCode.Error);
                                            // we can't be sure whether we'll have a Code here
                                            // the exact format of an error object is not specified in JSONRPC v1
                                            activity?.SetTag(RpcAttributes.AttributeRpcJsonrpcErrorMessage, errorArray.ToString());
#endif
                                            throw new Failure(errorArray);
                                        }
#if (NET8_0_OR_GREATER)
                                        activity?.SetStatus(ActivityStatusCode.Ok);
#endif
                                        return res1.Result;
                                }
                            }
                        }
                    }
#if (NET8_0_OR_GREATER)
                }
#endif
            }
        }

        protected virtual void PerformPostRequest(Stream postStream, Stream responseStream)
        {
#if (NET8_0_OR_GREATER)
            HttpClient httpClient = null;
            HttpClientHandler httpHandler = null;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage = null;

            try
            {
                httpHandler = new HttpClientHandler
                {
                    AllowAutoRedirect = AllowAutoRedirect,
                    PreAuthenticate = PreAuthenticate,
                    CookieContainer = Cookies ?? new CookieContainer(),
                    Proxy = WebProxy
                };

                Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> callBack = null;
                if (ServicePointManager.ServerCertificateValidationCallback != null)
                    callBack = ServicePointManager.ServerCertificateValidationCallback.Invoke;

                httpHandler.ServerCertificateCustomValidationCallback = ServerCertificateValidationCallback ?? callBack;

                httpClient = new HttpClient(httpHandler) { Timeout = TimeSpan.FromMilliseconds(Timeout) };

                requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(JsonRpcUrl));
                if (ProtocolVersion != null)
                    requestMessage.Version = ProtocolVersion;
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Headers.UserAgent.ParseAdd(UserAgent);
                requestMessage.Headers.ConnectionClose = !KeepAlive;
                requestMessage.Headers.ExpectContinue = Expect100Continue;
                requestMessage.Content = new StreamContent(postStream);

                if (RequestHeaders != null)
                {
                    foreach (var header in RequestHeaders)
                        requestMessage.Headers.Add(header.Key, header.Value);
                }

                // propagate W3C traceparent and tracestate
                // HttpClient would do this automatically on .NET 5,
                // and .NET 6 would provide even more control over this: https://blog.ladeak.net/posts/opentelemetry-net6-httpclient
                // the caller must ensure that the activity is in W3C format (by inheritance or direct setting)
                var activity = Activity.Current;
                if (activity != null)
                {
                    if (activity.IdFormat == ActivityIdFormat.W3C)
                    {
                        requestMessage.Headers.Add("traceparent", activity.Id);
                        var state = activity.TraceStateString;

                        if (state?.Length > 0)
                            requestMessage.Headers.Add("tracestate", state);
                    }

                    var tags = new ActivityTagsCollection { { RpcAttributes.AttributeRpcMessageType, RpcAttributes.RpcMessageTypeValues.Sent } };
                    activity.AddEvent(new ActivityEvent(EventRpcMessage, DateTimeOffset.Now, tags));
                }

                responseMessage = httpClient.SendAsync(requestMessage).Result;
                responseMessage.EnsureSuccessStatusCode();

                var str = responseMessage.Content.ReadAsStream();
                str.CopyTo(responseStream);
                responseStream.Flush();

                ResponseHeaders = responseMessage.Headers.ToDictionary(header => header.Key, header => string.Join(",", header.Value));

                if (activity != null)
                {
                    var tags = new ActivityTagsCollection { { RpcAttributes.AttributeRpcMessageType, RpcAttributes.RpcMessageTypeValues.Received } };
                    activity.AddEvent(new ActivityEvent(EventRpcMessage, DateTimeOffset.Now, tags));
                }
            }
            finally
            {
                RequestHeaders = null;
                responseMessage?.Dispose();
                requestMessage?.Dispose();
                httpClient?.Dispose();
                httpHandler?.Dispose();
            }
#else
            var webRequest = (HttpWebRequest)WebRequest.Create(JsonRpcUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Accept = "application/json";
            webRequest.Timeout = Timeout;
            webRequest.Proxy = WebProxy;
            webRequest.KeepAlive = KeepAlive;
            webRequest.UserAgent = UserAgent;
            webRequest.ConnectionGroupName = ConnectionGroupName;
            webRequest.ProtocolVersion = ProtocolVersion ?? webRequest.ProtocolVersion;
            webRequest.ServicePoint.Expect100Continue = Expect100Continue;
            webRequest.AllowAutoRedirect = AllowAutoRedirect;
            webRequest.PreAuthenticate = PreAuthenticate;
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.CookieContainer = Cookies ?? webRequest.CookieContainer ?? new CookieContainer();
            webRequest.ServerCertificateValidationCallback = ServerCertificateValidationCallback ?? ServicePointManager.ServerCertificateValidationCallback;

            if (RequestHeaders != null)
            {
                foreach (var header in RequestHeaders)
                    webRequest.Headers.Add(header.Key, header.Value);
            }

            using (var str = webRequest.GetRequestStream())
            {
                postStream.CopyTo(str);
                str.Flush();
            }

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                var newResponseHeaders = new Dictionary<string, string>();

                if (webResponse.Headers != null)
                {
                    var keys = webResponse.Headers.AllKeys;
                    foreach (var key in keys)
                        newResponseHeaders.Add(key, string.Join(",", webResponse.Headers.Get(key)));
                }

                ResponseHeaders = newResponseHeaders;

                if (webResponse.StatusCode != HttpStatusCode.OK)
                    throw new WebException(webResponse.StatusCode.ToString());

                using (var str = webResponse.GetResponseStream())
                {
                    if (str == null)
                        throw new WebException();

                    str.CopyTo(responseStream);
                    responseStream.Flush();
                }
            }
            finally
            {
                RequestHeaders = null;
                webResponse?.Dispose();
            }
#endif
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

        protected JsonSerializer CreateSerializer(IList<JsonConverter> converters)
        {
            var settings = CreateSettings(converters);
            return JsonSerializer.Create(settings);
        }
    }
}
