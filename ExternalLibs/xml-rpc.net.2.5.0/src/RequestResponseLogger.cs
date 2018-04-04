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
using System.IO;

namespace CookComputing.XmlRpc
{
  public class RequestResponseLogger : XmlRpcLogger
  {
    string _directory = ".";

    public string Directory
    {
      get { return _directory; }
      set { _directory = Path.GetDirectoryName(value + "/"); }
    }

    protected override void OnRequest(object sender, XmlRpcRequestEventArgs e)
    {
      string fname = string.Format("{0}/{1}-{2:0000}-request-{3}.xml",
        _directory, DateTime.Now.Ticks, e.RequestNum, e.ProxyID);
      FileStream fstm = new FileStream(fname, FileMode.Create);
      Util.CopyStream(e.RequestStream, fstm);
      fstm.Close();
    }

    protected override void OnResponse(object sender, XmlRpcResponseEventArgs e)
    {
      string fname = string.Format("{0}/{1}-{2:0000}-response-{3}.xml",
        _directory, DateTime.Now.Ticks, e.RequestNum, e.ProxyID);
      FileStream fstm = new FileStream(fname, FileMode.Create);
      Util.CopyStream(e.ResponseStream, fstm);
      fstm.Close();
    }
  }
}