/* 
XML-RPC.NET library
Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CookComputing.XmlRpc
{
  public class XmlRpcClientProtocol : Component, IXmlRpcProxy
  {
    #region Instance Variables
    private bool _allowAutoRedirect = true;
#if (!COMPACT_FRAMEWORK)
    private string _connectionGroupName = null;
#endif
#if (!COMPACT_FRAMEWORK && !FX1_0)
    private bool _expect100Continue = false;
    private bool _enableCompression = false;
#endif
    private ICredentials _credentials = null;
    private WebHeaderCollection _headers = new WebHeaderCollection();
    private int _indentation = 2;
    private bool _keepAlive = true;
    private XmlRpcNonStandard _nonStandard = XmlRpcNonStandard.None;
    private bool _preAuthenticate = false;
    private Version _protocolVersion = HttpVersion.Version11;
    private IWebProxy _proxy = null;
#if (!COMPACT_FRAMEWORK)
    private CookieCollection _responseCookies;
    private WebHeaderCollection _responseHeaders;
#endif
    private int _timeout = 100000;
    private string _url = null;
    private string _userAgent = "XML-RPC.NET";
    private bool _useEmptyParamsTag = true;
    private bool _useIndentation = true;
    private bool _useIntTag = false;
    private bool _useStringTag = true;
    private Encoding _xmlEncoding = null;
    private string _xmlRpcMethod = null;

#if (!COMPACT_FRAMEWORK)
    private X509CertificateCollection _clientCertificates
      = new X509CertificateCollection();
    private CookieContainer _cookies = new CookieContainer();
#endif
    #endregion
    private Guid _id = Util.NewGuid();


#if (!COMPACT_FRAMEWORK)
    public XmlRpcClientProtocol(System.ComponentModel.IContainer container)
    {
      container.Add(this);
      InitializeComponent();
    }
#endif
    public XmlRpcClientProtocol()
    {
      InitializeComponent();
    }

    public object Invoke(
      MethodBase mb,
      params object[] Parameters)
    {
      return Invoke(this, mb as MethodInfo, Parameters);
    }

    public object Invoke(
      MethodInfo mi,
      params object[] Parameters)
    {
      return Invoke(this, mi, Parameters);
    }

    public object Invoke(
      string MethodName,
      params object[] Parameters)
    {
      return Invoke(this, MethodName, Parameters);
    }

    public object Invoke(
      Object clientObj,
      string methodName,
      params object[] parameters)
    {
      MethodInfo mi = GetMethodInfoFromName(clientObj, methodName, parameters);
      return Invoke(this, mi, parameters);
    }

    public object Invoke(
      Object clientObj,
      MethodInfo mi,
      params object[] parameters)
    {
#if (!COMPACT_FRAMEWORK)
      _responseHeaders = null;
      _responseCookies = null;
#endif
      WebRequest webReq = null;
      object reto = null;
      try
      {
        string useUrl = GetEffectiveUrl(clientObj);
        webReq = GetWebRequest(new Uri(useUrl));
        XmlRpcRequest req = MakeXmlRpcRequest(webReq, mi, parameters,
          clientObj, _xmlRpcMethod, _id);
        SetProperties(webReq);
        SetRequestHeaders(_headers, webReq);
#if (!COMPACT_FRAMEWORK)
        SetClientCertificates(_clientCertificates, webReq);
#endif
        Stream serStream = null;
        Stream reqStream = null;
        bool logging = (RequestEvent != null);
        if (!logging)
          serStream = reqStream = webReq.GetRequestStream();
        else
          serStream = new MemoryStream(2000);
        try
        {
          XmlRpcSerializer serializer = new XmlRpcSerializer();
          if (_xmlEncoding != null)
            serializer.XmlEncoding = _xmlEncoding;
          serializer.UseIndentation = _useIndentation;
          serializer.Indentation = _indentation;
          serializer.NonStandard = _nonStandard;
          serializer.UseStringTag = _useStringTag;
          serializer.UseIntTag = _useIntTag;
          serializer.UseEmptyParamsTag = _useEmptyParamsTag;
          serializer.SerializeRequest(serStream, req);
          if (logging)
          {
            reqStream = webReq.GetRequestStream();
            serStream.Position = 0;
            Util.CopyStream(serStream, reqStream);
            reqStream.Flush();
            serStream.Position = 0;
            OnRequest(new XmlRpcRequestEventArgs(req.proxyId, req.number,
              serStream));
          }
        }
        finally
        {
          if (reqStream != null)
            reqStream.Close();
        }
        HttpWebResponse webResp = GetWebResponse(webReq) as HttpWebResponse;
#if (!COMPACT_FRAMEWORK)
        _responseCookies = webResp.Cookies;
        _responseHeaders = webResp.Headers;
#endif
        Stream respStm = null;
        Stream deserStream;
        logging = (ResponseEvent != null);
        try
        {
          respStm = webResp.GetResponseStream();
          if (!logging)
          {
            deserStream = respStm;
          }
          else
          {
            deserStream = new MemoryStream(2000);
            Util.CopyStream(respStm, deserStream);
            deserStream.Flush();
            deserStream.Position = 0;
          }
#if (!COMPACT_FRAMEWORK && !FX1_0)
          deserStream = MaybeDecompressStream((HttpWebResponse)webResp, 
            deserStream);          
#endif
          try
          {
            XmlRpcResponse resp = ReadResponse(req, webResp, deserStream, null);
            reto = resp.retVal;
          }
          finally
          {
            if (logging)
            {
              deserStream.Position = 0;
              OnResponse(new XmlRpcResponseEventArgs(req.proxyId, req.number,
                deserStream));
            }
          }
        }
        finally
        {
          if (respStm != null)
            respStm.Close();
        }
      }
      finally
      {
        if (webReq != null)
          webReq = null;
      }
      return reto;
    }

    #region Properties

    public bool AllowAutoRedirect
    {
      get { return _allowAutoRedirect; }
      set { _allowAutoRedirect = value; }
    }

#if (!COMPACT_FRAMEWORK)
    [Browsable(false)]
    public X509CertificateCollection ClientCertificates
    {
      get { return _clientCertificates; }
    }
#endif

#if (!COMPACT_FRAMEWORK)
    public string ConnectionGroupName
    {
      get { return _connectionGroupName; }
      set { _connectionGroupName = value; }
    }
#endif

    [Browsable(false)]
    public ICredentials Credentials
    {
      get { return _credentials; }
      set { _credentials = value; }
    }

#if (!COMPACT_FRAMEWORK && !FX1_0)
    public bool EnableCompression
    {
      get { return _enableCompression; }
      set { _enableCompression = value; }
    }
#endif

    [Browsable(false)]
    public WebHeaderCollection Headers
    {
      get { return _headers; }
    }

#if (!COMPACT_FRAMEWORK && !FX1_0)
    public bool Expect100Continue
    {
      get { return _expect100Continue; }
      set { _expect100Continue = value; }
    }
#endif

#if (!COMPACT_FRAMEWORK)
    public CookieContainer CookieContainer
    {
      get { return _cookies; }
    }
#endif

    public Guid Id
    {
      get { return _id; }
    }

    public int Indentation
    {
      get { return _indentation; }
      set { _indentation = value; }
    }

    public bool KeepAlive
    {
      get { return _keepAlive; }
      set { _keepAlive = value; }
    }

    public XmlRpcNonStandard NonStandard
    {
      get { return _nonStandard; }
      set { _nonStandard = value; }
    }

    public bool PreAuthenticate
    {
      get { return _preAuthenticate; }
      set { _preAuthenticate = value; }
    }

    [Browsable(false)]
    public System.Version ProtocolVersion
    {
      get { return _protocolVersion; }
      set { _protocolVersion = value; }
    }

    [Browsable(false)]
    public IWebProxy Proxy
    {
      get { return _proxy; }
      set { _proxy = value; }
    }

#if (!COMPACT_FRAMEWORK)
    public CookieCollection ResponseCookies
    {
      get { return _responseCookies; }
    }
#endif

#if (!COMPACT_FRAMEWORK)
    public WebHeaderCollection ResponseHeaders
    {
      get { return _responseHeaders; }
    }
#endif

    public int Timeout
    {
      get { return _timeout; }
      set { _timeout = value; }
    }

    public string Url
    {
      get { return _url; }
      set { _url = value; }
    }

    public bool UseEmptyParamsTag
    {
      get { return _useEmptyParamsTag; }
      set { _useEmptyParamsTag = value; }
    }

    public bool UseIndentation
    {
      get { return _useIndentation; }
      set { _useIndentation = value; }
    }

    public bool UseIntTag
    {
      get { return _useIntTag; }
      set { _useIntTag = value; }
    }

    public string UserAgent
    {
      get { return _userAgent; }
      set { _userAgent = value; }
    }

    public bool UseStringTag
    {
      get { return _useStringTag; }
      set { _useStringTag = value; }
    }

    [Browsable(false)]
    public Encoding XmlEncoding
    {
      get { return _xmlEncoding; }
      set { _xmlEncoding = value; }
    }

    public string XmlRpcMethod
    {
      get { return _xmlRpcMethod; }
      set { _xmlRpcMethod = value; }
    }

    #endregion

    public void SetProperties(WebRequest webReq)
    {
      if (_proxy != null)
        webReq.Proxy = _proxy;
      HttpWebRequest httpReq = (HttpWebRequest)webReq;
      httpReq.UserAgent = _userAgent;
      httpReq.ProtocolVersion = _protocolVersion;
      httpReq.KeepAlive = _keepAlive;
#if (!COMPACT_FRAMEWORK)
      httpReq.CookieContainer = _cookies;
#endif
#if (!COMPACT_FRAMEWORK && !FX1_0)
      httpReq.ServicePoint.Expect100Continue = _expect100Continue;
#endif
      httpReq.AllowAutoRedirect = _allowAutoRedirect;
      webReq.Timeout = Timeout;
#if (!COMPACT_FRAMEWORK)
      webReq.ConnectionGroupName = this._connectionGroupName;
#endif
      webReq.Credentials = Credentials;
      webReq.PreAuthenticate = PreAuthenticate;
      // Compact Framework sets this to false by default
      (webReq as HttpWebRequest).AllowWriteStreamBuffering = true;
#if (!COMPACT_FRAMEWORK && !FX1_0)
      if (_enableCompression)
        webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
#endif
    }

    private void SetRequestHeaders(
      WebHeaderCollection headers,
      WebRequest webReq)
    {
      foreach (string key in headers)
      {
        webReq.Headers.Add(key, headers[key]);
      }
    }
#if (!COMPACT_FRAMEWORK)
    private void SetClientCertificates(
      X509CertificateCollection certificates,
      WebRequest webReq)
    {
      foreach (X509Certificate certificate in certificates)
      {
        HttpWebRequest httpReq = (HttpWebRequest)webReq;
        httpReq.ClientCertificates.Add(certificate);
      }
    }
#endif
    XmlRpcRequest MakeXmlRpcRequest(WebRequest webReq, MethodInfo mi,
      object[] parameters, object clientObj, string xmlRpcMethod,
      Guid proxyId)
    {
      webReq.Method = "POST";
      webReq.ContentType = "text/xml";
      string rpcMethodName = GetRpcMethodName(clientObj, mi);
      XmlRpcRequest req = new XmlRpcRequest(rpcMethodName, parameters, mi,
        xmlRpcMethod, proxyId);
      return req;
    }

    XmlRpcResponse ReadResponse(
      XmlRpcRequest req,
      WebResponse webResp,
      Stream respStm,
      Type returnType)
    {
      HttpWebResponse httpResp = (HttpWebResponse)webResp;
      if (httpResp.StatusCode != HttpStatusCode.OK)
      {
        // status 400 is used for errors caused by the client
        // status 500 is used for server errors (not server application
        // errors which are returned as fault responses)
        if (httpResp.StatusCode == HttpStatusCode.BadRequest)
          throw new XmlRpcException(httpResp.StatusDescription);
        else
          throw new XmlRpcServerException(httpResp.StatusDescription);
      }
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = _nonStandard;
      Type retType = returnType;
      if (retType == null)
        retType = req.mi.ReturnType;
      XmlRpcResponse xmlRpcResp
        = serializer.DeserializeResponse(respStm, retType);
      return xmlRpcResp;
    }

    MethodInfo GetMethodInfoFromName(object clientObj, string methodName, 
      object[] parameters)
    {
      Type[] paramTypes = new Type[0];
      if (parameters != null)
      {
        paramTypes = new Type[parameters.Length];
        for (int i = 0; i < paramTypes.Length; i++)
        {
          if (parameters[i] == null)
            throw new XmlRpcNullParameterException("Null parameters are invalid");
          paramTypes[i] = parameters[i].GetType();
        }
      }
      Type type = clientObj.GetType();
      MethodInfo mi = type.GetMethod(methodName, paramTypes);
      if (mi == null)
      {
        try
        {
          mi = type.GetMethod(methodName);
        }
        catch (System.Reflection.AmbiguousMatchException)
        {
          throw new XmlRpcInvalidParametersException("Method parameters match "
            + "the signature of more than one method");
        }
        if (mi == null)
          throw new Exception(
            "Invoke on non-existent or non-public proxy method");
        else
          throw new XmlRpcInvalidParametersException("Method parameters do "
            + "not match signature of any method called " + methodName);
      }
      return mi;
    }

    string GetRpcMethodName(object clientObj, MethodInfo mi)
    {
      string rpcMethod;
      string MethodName = mi.Name;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcBeginAttribute));
      if (attr != null)
      {
        rpcMethod = ((XmlRpcBeginAttribute)attr).Method;
        if (rpcMethod == "")
        {
          if (!MethodName.StartsWith("Begin") || MethodName.Length <= 5)
            throw new Exception(String.Format(
              "method {0} has invalid signature for begin method",
              MethodName));
          rpcMethod = MethodName.Substring(5);
        }
        return rpcMethod;
      }
      // if no XmlRpcBegin attribute, must have XmlRpcMethod attribute   
      attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
      if (attr == null)
      {
        throw new Exception("missing method attribute");
      }
      XmlRpcMethodAttribute xrmAttr = attr as XmlRpcMethodAttribute;
      rpcMethod = xrmAttr.Method;
      if (rpcMethod == "")
      {
        rpcMethod = mi.Name;
      }
      return rpcMethod;
    }

    public IAsyncResult BeginInvoke(
      MethodBase mb,
      object[] parameters,
      AsyncCallback callback,
      object outerAsyncState)
    {
      return BeginInvoke(mb as MethodInfo, parameters, this, callback,
        outerAsyncState);
    }

    public IAsyncResult BeginInvoke(
      MethodInfo mi,
      object[] parameters,
      AsyncCallback callback,
      object outerAsyncState)
    {
      return BeginInvoke(mi, parameters, this, callback,
        outerAsyncState);
    }

    public IAsyncResult BeginInvoke(
      string methodName,
      object[] parameters,
      object clientObj,
      AsyncCallback callback,
      object outerAsyncState)
    {
      MethodInfo mi = GetMethodInfoFromName(clientObj, methodName, parameters);
      return BeginInvoke(mi, parameters, this, callback,
        outerAsyncState);
    }

    public IAsyncResult BeginInvoke(
      MethodInfo mi,
      object[] parameters,
      object clientObj,
      AsyncCallback callback,
      object outerAsyncState)
    {
      string useUrl = GetEffectiveUrl(clientObj);
      WebRequest webReq = GetWebRequest(new Uri(useUrl));
      XmlRpcRequest xmlRpcReq = MakeXmlRpcRequest(webReq, mi,
        parameters, clientObj, _xmlRpcMethod, _id);
      SetProperties(webReq);
      SetRequestHeaders(_headers, webReq);
#if (!COMPACT_FRAMEWORK)
      SetClientCertificates(_clientCertificates, webReq);
#endif
      Encoding useEncoding = null;
      if (_xmlEncoding != null)
        useEncoding = _xmlEncoding;
      XmlRpcAsyncResult asr = new XmlRpcAsyncResult(this, xmlRpcReq,
        useEncoding, _useEmptyParamsTag, _useIndentation, _indentation, 
        _useIntTag, _useStringTag, webReq, callback, outerAsyncState, 0);
      webReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback),
        asr);
      if (!asr.IsCompleted)
        asr.CompletedSynchronously = false;
      return asr;
    }

    static void GetRequestStreamCallback(IAsyncResult asyncResult)
    {
      XmlRpcAsyncResult clientResult
        = (XmlRpcAsyncResult)asyncResult.AsyncState;
      clientResult.CompletedSynchronously = asyncResult.CompletedSynchronously;
      try
      {
        Stream serStream = null;
        Stream reqStream = null;
        bool logging = (clientResult.ClientProtocol.RequestEvent != null);
        if (!logging)
        {
          serStream = reqStream
            = clientResult.Request.EndGetRequestStream(asyncResult);
        }
        else
          serStream = new MemoryStream(2000);
        try
        {
          XmlRpcRequest req = clientResult.XmlRpcRequest;
          XmlRpcSerializer serializer = new XmlRpcSerializer();
          if (clientResult.XmlEncoding != null)
            serializer.XmlEncoding = clientResult.XmlEncoding;
          serializer.UseEmptyParamsTag = clientResult.UseEmptyParamsTag;
          serializer.UseIndentation = clientResult.UseIndentation;
          serializer.Indentation = clientResult.Indentation;
          serializer.UseIntTag = clientResult.UseIntTag;
          serializer.UseStringTag = clientResult.UseStringTag;
          serializer.SerializeRequest(serStream, req);
          if (logging)
          {
            reqStream = clientResult.Request.EndGetRequestStream(asyncResult);
            serStream.Position = 0;
            Util.CopyStream(serStream, reqStream);
            reqStream.Flush();
            serStream.Position = 0;
            clientResult.ClientProtocol.OnRequest(
              new XmlRpcRequestEventArgs(req.proxyId, req.number, serStream));
          }
        }
        finally
        {
          if (reqStream != null)
            reqStream.Close();
        }
        clientResult.Request.BeginGetResponse(
          new AsyncCallback(GetResponseCallback), clientResult);
      }
      catch (Exception ex)
      {
        ProcessAsyncException(clientResult, ex);
      }
    }

    static void GetResponseCallback(IAsyncResult asyncResult)
    {
      XmlRpcAsyncResult result = (XmlRpcAsyncResult)asyncResult.AsyncState;
      result.CompletedSynchronously = asyncResult.CompletedSynchronously;
      try
      {
        result.Response = result.ClientProtocol.GetWebResponse(result.Request,
          asyncResult);
      }
      catch (Exception ex)
      {
        ProcessAsyncException(result, ex);
        if (result.Response == null)
          return;
      }
      ReadAsyncResponse(result);
    }

    static void ReadAsyncResponse(XmlRpcAsyncResult result)
    {
      if (result.Response.ContentLength == 0)
      {
        result.Complete();
        return;
      }
      try
      {
        result.ResponseStream = result.Response.GetResponseStream();
        ReadAsyncResponseStream(result);
      }
      catch (Exception ex)
      {
        ProcessAsyncException(result, ex);
      }
    }

    static void ReadAsyncResponseStream(XmlRpcAsyncResult result)
    {
      IAsyncResult asyncResult;
      do
      {
        byte[] buff = result.Buffer;
        long contLen = result.Response.ContentLength;
        if (buff == null)
        {
          if (contLen == -1)
            result.Buffer = new Byte[1024];
          else
            result.Buffer = new Byte[contLen];
        }
        else
        {
          if (contLen != -1 && contLen > result.Buffer.Length)
            result.Buffer = new Byte[contLen];
        }
        buff = result.Buffer;
        asyncResult = result.ResponseStream.BeginRead(buff, 0, buff.Length,
          new AsyncCallback(ReadResponseCallback), result);
        if (!asyncResult.CompletedSynchronously)
          return;
      }
      while (!(ProcessAsyncResponseStreamResult(result, asyncResult)));
    }

    static bool ProcessAsyncResponseStreamResult(XmlRpcAsyncResult result,
      IAsyncResult asyncResult)
    {
      int endReadLen = result.ResponseStream.EndRead(asyncResult);
      long contLen = result.Response.ContentLength;
      bool completed;
      if (endReadLen == 0)
        completed = true;
      else if (contLen > 0 && endReadLen == contLen)
      {
        result.ResponseBufferedStream = new MemoryStream(result.Buffer);
        completed = true;
      }
      else
      {
        if (result.ResponseBufferedStream == null)
        {
          result.ResponseBufferedStream = new MemoryStream(result.Buffer.Length);
        }
        result.ResponseBufferedStream.Write(result.Buffer, 0, endReadLen);
        completed = false;
      }
      if (completed)
        result.Complete();
      return completed;
    }


    static void ReadResponseCallback(IAsyncResult asyncResult)
    {
      XmlRpcAsyncResult result = (XmlRpcAsyncResult)asyncResult.AsyncState;
      result.CompletedSynchronously = asyncResult.CompletedSynchronously;
      if (asyncResult.CompletedSynchronously)
        return;
      try
      {
        bool completed = ProcessAsyncResponseStreamResult(result, asyncResult);
        if (!completed)
          ReadAsyncResponseStream(result);
      }
      catch (Exception ex)
      {
        ProcessAsyncException(result, ex);
      }
    }

    static void ProcessAsyncException(XmlRpcAsyncResult clientResult,
      Exception ex)
    {
      WebException webex = ex as WebException;
      if (webex != null && webex.Response != null)
      {
        clientResult.Response = webex.Response;
        return;
      }
      if (clientResult.IsCompleted)
        throw new Exception("error during async processing");
      clientResult.Complete(ex);
    }

    public object EndInvoke(
      IAsyncResult asr)
    {
      return EndInvoke(asr, null);
    }

    public object EndInvoke(
      IAsyncResult asr,
      Type returnType)
    {
      object reto = null;
      Stream responseStream = null;
      try
      {
        XmlRpcAsyncResult clientResult = (XmlRpcAsyncResult)asr;
        if (clientResult.Exception != null)
          throw clientResult.Exception;
        if (clientResult.EndSendCalled)
          throw new Exception("dup call to EndSend");
        clientResult.EndSendCalled = true;
        HttpWebResponse webResp = (HttpWebResponse)clientResult.WaitForResponse();
#if (!COMPACT_FRAMEWORK)
        clientResult._responseCookies = webResp.Cookies;
        clientResult._responseHeaders = webResp.Headers;
#endif
        responseStream = clientResult.ResponseBufferedStream;
        if (ResponseEvent != null)
        {
          OnResponse(new XmlRpcResponseEventArgs(
            clientResult.XmlRpcRequest.proxyId,
            clientResult.XmlRpcRequest.number,
            responseStream));
          responseStream.Position = 0;
        }
#if (!COMPACT_FRAMEWORK && !FX1_0)
        responseStream = MaybeDecompressStream((HttpWebResponse)webResp, 
          responseStream);
#endif
        XmlRpcResponse resp = ReadResponse(clientResult.XmlRpcRequest,
          webResp, responseStream, returnType);
        reto = resp.retVal;
      }
      finally
      {
        if (responseStream != null)
          responseStream.Close();
      }
      return reto;
    }

    string GetEffectiveUrl(object clientObj)
    {
      Type type = clientObj.GetType();
      // client can either have define URI in attribute or have set it
      // via proxy's ServiceURI property - but must exist by now
      string useUrl = "";
      if (Url == "" || Url == null)
      {
        Attribute urlAttr = Attribute.GetCustomAttribute(type,
          typeof(XmlRpcUrlAttribute));
        if (urlAttr != null)
        {
          XmlRpcUrlAttribute xrsAttr = urlAttr as XmlRpcUrlAttribute;
          useUrl = xrsAttr.Uri;
        }
      }
      else
      {
        useUrl = Url;
      }
      if (useUrl == "")
      {
        throw new XmlRpcMissingUrl("Proxy XmlRpcUrl attribute or Url "
          + "property not set.");
      }
      return useUrl;
    }

    #region Introspection Methods
    [XmlRpcMethod("system.listMethods")]
    public string[] SystemListMethods()
    {
      return (string[])Invoke("SystemListMethods", new Object[0]);
    }

    [XmlRpcMethod("system.listMethods")]
    public IAsyncResult BeginSystemListMethods(
      AsyncCallback Callback,
      object State)
    {
      return BeginInvoke("SystemListMethods", new object[0], this, Callback,
        State);
    }

    public string[] EndSystemListMethods(IAsyncResult AsyncResult)
    {
      return (string[])EndInvoke(AsyncResult);
    }

    [XmlRpcMethod("system.methodSignature")]
    public object[] SystemMethodSignature(string MethodName)
    {
      return (object[])Invoke("SystemMethodSignature",
        new Object[] { MethodName });
    }

    [XmlRpcMethod("system.methodSignature")]
    public IAsyncResult BeginSystemMethodSignature(
      string MethodName,
      AsyncCallback Callback,
      object State)
    {
      return BeginInvoke("SystemMethodSignature",
        new Object[] { MethodName }, this, Callback, State);
    }

    public Array EndSystemMethodSignature(IAsyncResult AsyncResult)
    {
      return (Array)EndInvoke(AsyncResult);
    }

    [XmlRpcMethod("system.methodHelp")]
    public string SystemMethodHelp(string MethodName)
    {
      return (string)Invoke("SystemMethodHelp",
        new Object[] { MethodName });
    }

    [XmlRpcMethod("system.methodHelp")]
    public IAsyncResult BeginSystemMethodHelp(
      string MethodName,
      AsyncCallback Callback,
      object State)
    {
      return BeginInvoke("SystemMethodHelp",
        new Object[] { MethodName }, this, Callback, State);
    }

    public string EndSystemMethodHelp(IAsyncResult AsyncResult)
    {
      return (string)EndInvoke(AsyncResult);
    }
    #endregion

    #region Component Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }
    #endregion

    protected virtual WebRequest GetWebRequest(Uri uri)
    {
      WebRequest req = WebRequest.Create(uri);
      return req;
    }

    protected virtual WebResponse GetWebResponse(WebRequest request)
    {
      WebResponse ret = null;
      try
      {
        ret = request.GetResponse();
      }
      catch (WebException ex)
      {
        if (ex.Response == null)
          throw;
        ret = ex.Response;
      }
      return ret;
    }

#if (!COMPACT_FRAMEWORK && !FX1_0)
    // support for gzip and deflate
    protected Stream MaybeDecompressStream(HttpWebResponse httpWebResp, 
      Stream respStream)
    {
      Stream decodedStream;
      string contentEncoding = httpWebResp.ContentEncoding.ToLower();
      string coen = httpWebResp.Headers["Content-Encoding"];
      if (contentEncoding.Contains("gzip"))
      {
        decodedStream = new System.IO.Compression.GZipStream(respStream,
          System.IO.Compression.CompressionMode.Decompress);
      }
      else if (contentEncoding.Contains("deflate"))
      {
        decodedStream = new System.IO.Compression.DeflateStream(respStream,
          System.IO.Compression.CompressionMode.Decompress);
      }
      else
        decodedStream = respStream;
      return decodedStream;
    }
#endif

    protected virtual WebResponse GetWebResponse(WebRequest request,
      IAsyncResult result)
    {
      return request.EndGetResponse(result);
    }

    public event XmlRpcRequestEventHandler RequestEvent;
    public event XmlRpcResponseEventHandler ResponseEvent;


    protected virtual void OnRequest(XmlRpcRequestEventArgs e)
    {
      if (RequestEvent != null)
      {
        RequestEvent(this, e);
      }
    }

    internal bool LogResponse
    {
      get { return ResponseEvent != null; }
    }

    protected virtual void OnResponse(XmlRpcResponseEventArgs e)
    {
      if (ResponseEvent != null)
      {
        ResponseEvent(this, e);
      }
    }

    internal void InternalOnResponse(XmlRpcResponseEventArgs e)
    {
      OnResponse(e);
    }
  }

#if (COMPACT_FRAMEWORK)
  // dummy attribute because System.ComponentModel.Browsable is not
  // support in the compact framework
  [AttributeUsage(AttributeTargets.Property)]
  public class BrowsableAttribute : Attribute
  {
    public BrowsableAttribute(bool dummy)
    {
    }
  }
#endif

  public delegate void XmlRpcRequestEventHandler(object sender,
    XmlRpcRequestEventArgs args);

  public delegate void XmlRpcResponseEventHandler(object sender,
    XmlRpcResponseEventArgs args);
}


