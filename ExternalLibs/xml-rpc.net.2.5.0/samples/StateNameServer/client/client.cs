using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

using CookComputing.XmlRpc;

class _
{
  static void Main(string[] args)
  {
    bool bUseSoap = false;
    if (args.Length > 0 && args[0] == "SOAP")
      bUseSoap = true;
    HttpChannel chnl;
    if (bUseSoap)
      chnl = new HttpChannel();
    else
      chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
    ChannelServices.RegisterChannel(chnl, false);

    IStateName svr = (IStateName)Activator.GetObject(
      typeof(IStateName), "http://localhost:5678/statename.rem");
    // probably different URL for IIS
    //   IStateName svr = (IStateName)Activator.GetObject(
    //   typeof(IStateName), "http://localhost/statename/statename.rem");
    while (true)
    {
      Console.Write("Enter statenumber: ");
      string s = Console.ReadLine();
      if (s == "")
        break;
      int stateNumber;
      try
      {
        stateNumber = Convert.ToInt32(s);
      }
      catch(Exception)
      {
        Console.WriteLine("Invalid state number");
        continue;
      }
      try
      {
        string ret = svr.GetStateName(stateNumber);
        Console.WriteLine("State name is {0}", ret);
      }
      catch (XmlRpcFaultException fex)
      {
        Console.WriteLine("Fault response: {0} {1} {2}", 
          fex.FaultCode, fex.FaultString, fex.Message);
      }
    }
  }
}
