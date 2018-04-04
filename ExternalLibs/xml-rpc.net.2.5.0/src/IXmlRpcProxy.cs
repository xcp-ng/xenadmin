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
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CookComputing.XmlRpc
{
  public interface IXmlRpcProxy
  {
    bool AllowAutoRedirect { get; set; }

#if (!COMPACT_FRAMEWORK)
    X509CertificateCollection ClientCertificates { get; }
#endif

#if (!COMPACT_FRAMEWORK)
    string ConnectionGroupName { get; set; }
#endif

#if (!COMPACT_FRAMEWORK)
    CookieContainer CookieContainer { get; }
#endif

    [Browsable(false)]
    ICredentials Credentials { get; set; }

#if (!COMPACT_FRAMEWORK && !FX1_0)
    bool EnableCompression { get; set;}

    bool Expect100Continue { get; set; }
#endif

    [Browsable(false)]
    WebHeaderCollection Headers { get; }

    Guid Id { get; }

    int Indentation { get; set; }

    bool KeepAlive { get; set; }

    XmlRpcNonStandard NonStandard { get; set; }

    bool PreAuthenticate { get; set; }

    [Browsable(false)]
    System.Version ProtocolVersion { get; set; }

    [Browsable(false)]
    IWebProxy Proxy { get; set; }

#if (!COMPACT_FRAMEWORK)
    [Browsable(false)]
    CookieCollection ResponseCookies { get; }

    [Browsable(false)]
    WebHeaderCollection ResponseHeaders { get; }
#endif

    int Timeout { get; set; }

    string Url { get; set; }

    bool UseEmptyParamsTag { get; set; }

    bool UseIndentation { get; set; }

    bool UseIntTag { get; set; }

    bool UseStringTag { get; set; }

    string UserAgent { get; set; }

    [Browsable(false)]
    Encoding XmlEncoding { get; set; }

    string XmlRpcMethod { get; set; }

    // introspecton methods
    string[] SystemListMethods(); 
    object[] SystemMethodSignature(string MethodName);
    string SystemMethodHelp(string MethodName);

    // events
    event XmlRpcRequestEventHandler RequestEvent;
    event XmlRpcResponseEventHandler ResponseEvent;

  }
}
