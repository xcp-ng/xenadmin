using System;
using System.IO;
using System.Threading;
using CookComputing.XmlRpc;

public struct StateStructRequest
{
  public int state1;
  public int state2;
  public int state3;
}

[XmlRpcUrl("http://www.cookcomputing.com/xmlrpcsamples/RPC2.ashx")]
public interface IStateName : IXmlRpcProxy
{
  [XmlRpcMethod("examples.getStateName")]
  string GetStateName(int stateNumber);

  [XmlRpcBegin("examples.getStateName")]
  IAsyncResult BeginGetStateName(int stateNumber);

  [XmlRpcEnd]
  string EndGetStateName(IAsyncResult asr);

  [XmlRpcMethod("examples.getStateStruct")]
  string GetStateNames(StateStructRequest request);
}

class LoggingExample
{
  static void Main(string[] args)
  {
    try
    {
      IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
      RequestResponseLogger dumper = new RequestResponseLogger();
      dumper.Directory = "C:/temp";
      dumper.Attach(proxy);
      Console.WriteLine("Synchronous call");
      string ret = proxy.GetStateName(45);
      Console.WriteLine("state #45 is {0}", ret);
      Console.WriteLine("Asynchronous call");
      IAsyncResult asr = proxy.BeginGetStateName(46);
      asr.AsyncWaitHandle.WaitOne();
      string aret = proxy.EndGetStateName(asr);
      Console.WriteLine("state #46 is {0}", aret);
    }
    catch (XmlRpcFaultException fex)
    {
      Console.WriteLine(fex.FaultString);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}


