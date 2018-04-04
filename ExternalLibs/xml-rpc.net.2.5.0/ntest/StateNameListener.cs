using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading;
using CookComputing.XmlRpc;

#if !FX1_0

namespace ntest
{
  public class StateNameListnerService : XmlRpcListenerService
  {
    [XmlRpcMethod("examples.getStateName")]
    public string GetStateName(int stateNumber)
    {
      if (stateNumber < 1 || stateNumber > m_stateNames.Length)
        throw new XmlRpcFaultException(1, "Invalid state number");
      return m_stateNames[stateNumber - 1];
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


  public interface IStateNameDerived 
  {
    [XmlRpcMethod("examples.getStateName")]
    string GetStateName(int stateNumber);
  }


  public class StateNameListnerDerivedService : XmlRpcListenerService,
    IStateNameDerived
  {
    public string GetStateName(int stateNumber)
    {
      if (stateNumber < 1 || stateNumber > m_stateNames.Length)
        throw new XmlRpcFaultException(1, "Invalid state number");
      return m_stateNames[stateNumber - 1];
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

}

#endif