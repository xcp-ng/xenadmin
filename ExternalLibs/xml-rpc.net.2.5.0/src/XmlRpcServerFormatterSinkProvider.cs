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
using System.Collections;
using System.Runtime.Remoting.Channels;

using CookComputing.XmlRpc;

namespace CookComputing.XmlRpc
{
  public class XmlRpcServerFormatterSinkProvider : IServerFormatterSinkProvider
  {
    // constructors
    public XmlRpcServerFormatterSinkProvider(
      IDictionary properties, 
      ICollection providerData) 
    {
      // can use properties to pass in custom attributes from the config
      // file which can then be passed to sink constructor as required
    }

    public XmlRpcServerFormatterSinkProvider()
    {
      // can use properties to pass in custom attributes from the config
      // file which can then be passed to sink constructor as required
    }

    // properties
    //
    public IServerChannelSinkProvider Next
    {
      get { return m_next; }
      set { m_next = value; }
    }

    // public methods
    //
    public IServerChannelSink CreateSink(
      IChannelReceiver channel)
    {
      IServerChannelSink scs = null;
      if (m_next != null)
      {
        scs = m_next.CreateSink(channel);
      }
      return new XmlRpcServerFormatterSink(scs);
    }

    public void GetChannelData(IChannelDataStore channelData)
    {
      // TODO: not required???
    }

    // data
    //
    IServerChannelSinkProvider m_next;
  }
}
