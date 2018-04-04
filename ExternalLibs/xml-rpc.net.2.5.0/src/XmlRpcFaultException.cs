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

namespace CookComputing.XmlRpc
{
  using System;
#if (!COMPACT_FRAMEWORK)
  using System.Runtime.Serialization;
#endif

  // used to return server-side errors to client code - also can be 
  // thrown by Service implmentation code to return custom Fault Responses
#if (!COMPACT_FRAMEWORK)
  [Serializable]
#endif
  public class XmlRpcFaultException : ApplicationException
  { 
    // constructors
    //
    public XmlRpcFaultException(int TheCode, string TheString)
      : base("Server returned a fault exception: [" + TheCode.ToString() +  
              "] " + TheString)
    {
      m_faultCode = TheCode;
      m_faultString = TheString;
    }
#if (!COMPACT_FRAMEWORK)
    // deserialization constructor
    protected XmlRpcFaultException(
      SerializationInfo info, 
      StreamingContext context) 
      : base(info, context) 
    {
      m_faultCode = (int)info.GetValue("m_faultCode", typeof(int));
      m_faultString = (String)info.GetValue("m_faultString", typeof(string));
    }
#endif
    // properties
    //
    public int FaultCode 
    {
      get { return m_faultCode; } 
    }

    public string FaultString 
    {
      get { return m_faultString; } 
    }
#if (!COMPACT_FRAMEWORK)
    // public methods
    //
    public override void GetObjectData(
      SerializationInfo info, 
      StreamingContext context)
    {
      info.AddValue("m_faultCode", m_faultCode);
      info.AddValue("m_faultString", m_faultString);
      base.GetObjectData(info, context);
    }
#endif        
    // data
    //
    int m_faultCode;
    string m_faultString;
  }
}
