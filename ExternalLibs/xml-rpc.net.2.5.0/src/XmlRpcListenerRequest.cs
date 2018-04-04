/* 
XML-RPC.NET library
Copyright (c) 2001-2007, Charles Cook <charlescook@cookcomputing.com>

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

#if !FX1_0

namespace CookComputing.XmlRpc
{
  using System;
  using System.IO;
  using System.Net;

  public class XmlRpcListenerRequest : CookComputing.XmlRpc.IHttpRequest
  {
    public XmlRpcListenerRequest(HttpListenerRequest request)
    {
      _request = request;
    }

    public Stream InputStream
    {
      get { return _request.InputStream; }
    }

    public string HttpMethod
    {
      get { return _request.HttpMethod; }
    }

    private HttpListenerRequest _request;
  }
}

#endif