using System;
using CookComputing.XmlRpc;

[XmlRpcUrl("http://www.cookcomputing.com/xmlrpcsamples/RPC2.ashx")]
public interface IStateName : IXmlRpcProxy
{
  [XmlRpcMethod("examples.getStateName")]
  string GetStateName(int stateNumber);

  [XmlRpcBegin("examples.getStateName")]
  IAsyncResult BeginGetStateName(int stateNumber, AsyncCallback acb, 
    object state);

  [XmlRpcEnd]
  string EndGetStateName(IAsyncResult asr);
}
