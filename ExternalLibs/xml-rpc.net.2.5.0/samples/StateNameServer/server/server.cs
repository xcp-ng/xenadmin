using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

using CookComputing.XmlRpc;

public class StateNameServer : MarshalByRefObject, IStateName
{
  public string GetStateName(int stateNumber)
  {
    if (stateNumber < 1 || stateNumber > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "Invalid state number");
    return m_stateNames[stateNumber-1]; 
  }

  public string GetStateNames(StateStructRequest request)
  {
    if (request.state1 < 1 || request.state1 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    if (request.state2 < 1 || request.state2 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    if (request.state3 < 1 || request.state3 > m_stateNames.Length)
      throw new XmlRpcFaultException(1, "State number 1 invalid");
    string ret = m_stateNames[request.state1-1] + " "
      + m_stateNames[request.state2-1] + " " 
      + m_stateNames[request.state3-1];
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

class _
{
  static void Main(string[] args)
  {
    IDictionary props = new Hashtable();
    props["name"] = "MyHttpChannel";
    props["port"] = 5678;
    HttpChannel channel = new HttpChannel(
       props,
       null,
       new XmlRpcServerFormatterSinkProvider()
    );
    ChannelServices.RegisterChannel(channel, false);

    //RemotingConfiguration.Configure("StateNameServer.exe.config", false);
    RemotingConfiguration.RegisterWellKnownServiceType(
      typeof(StateNameServer),
      "statename.rem",
      WellKnownObjectMode.Singleton);
    Console.WriteLine("Press <ENTER> to shutdown");
    Console.ReadLine();
  }
}


