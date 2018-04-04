using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading;
using CookComputing.XmlRpc;

namespace ntest
{
  
  class StateNameService
  {
    static HttpChannel _channel;

    public static void Start(int port)
    {
      try
      {
        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = port;
        _channel = new HttpChannel(
           props,
           null,
           new XmlRpcServerFormatterSinkProvider()
        );
        ChannelServices.RegisterChannel(_channel, false);
        RemotingConfiguration.RegisterWellKnownServiceType(
          typeof(StateNameServer),
          "statename.rem",
          WellKnownObjectMode.Singleton);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }
    }

    public static void Stop()
    {
      ChannelServices.UnregisterChannel(_channel);
      Thread.Sleep(100);
    }
  }
}

public struct StateStructRequest
{
  public int state1;
  public int state2;
  public int state3;
}

[XmlRpcUrl("http://localhost:5678/statename.rem")]
public interface IStateName :IXmlRpcProxy
{
  [XmlRpcMethod("examples.getStateName")]
  string GetStateName(int stateNumber);

  [XmlRpcMethod("examples.getStateNameFromString")]
  string GetStateName(string stateNumber);

  [XmlRpcBegin("examples.getStateName")]
  IAsyncResult BeginGetStateName(int stateNumber);

  [XmlRpcBegin("examples.getStateName")]
  IAsyncResult BeginGetStateName(int stateNumber, AsyncCallback callback);

  [XmlRpcBegin("examples.getStateName")]
  IAsyncResult BeginGetStateName(int stateNumber, AsyncCallback callback,
    object asyncState);

  [XmlRpcEnd]
  string EndGetStateName(IAsyncResult iasr);

  [XmlRpcMethod("examples.getStateStruct")]
  string GetStateNames(StateStructRequest request);
}

public class StateNameServer : SystemMethodsBase
{
  [XmlRpcMethod("examples.getStateName")]
  public string GetStateName(int stateNumber)
  {
    if (stateNumber < 1 || stateNumber > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "Invalid state number");
    return m_stateNames[stateNumber - 1];
  }

  [XmlRpcMethod("examples.getStateNameFromString")]
  public string GetStateNameFromString(string stateNumber)
  {
    int number = Convert.ToInt32(stateNumber);
    if (number < 1 || number > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "Invalid state number");
    return m_stateNames[number - 1];
  }

  [XmlRpcMethod("examples.getStateStruct")]
  public string GetStateNames(StateStructRequest request)
  {
    if (request.state1 < 1 || request.state1 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    if (request.state2 < 1 || request.state2 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    if (request.state3 < 1 || request.state3 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    string ret = m_stateNames[request.state1 - 1] + " "
      + m_stateNames[request.state2 - 1] + " "
      + m_stateNames[request.state3 - 1];
    return ret;
  }

  string[] m_stateNames
    = { "Alabama", "Alaska", "Arizona", "Arkansas",
        "California", "Colorado", "Connecticut", "Delaware", "Florida",
        "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", 
        "Kansas", "Kentucky", "Lousiana", "Maine", "Maryland", "Massachusetts",
        "Michigan", "Minnesota", "Mississipi", "Missouri", "Montana",
        "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", 
        "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma",
        "Oregon", "Pennsylviania", "Rhose Island", "South Carolina", 
        "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", 
        "Washington", "West Virginia", "Wisconsin", "Wyoming" };
}