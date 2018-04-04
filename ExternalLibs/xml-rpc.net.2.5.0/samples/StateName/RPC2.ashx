<%@ WebHandler Language="C#" Class="StateName" %> <%@ Assembly Name="CookComputing.XmlRpc" %>
 
using System;
using System.Collections;
using System.Web;
using CookComputing.XmlRpc;

public struct StateStructRequest
{
  public int state1;  
  public int state2;
  public int state3;
}

[XmlRpcService(Description=
"This service exactly reproduces the functionality of " +
"betty.userland.com/RPC2.")]
public class StateName: XmlRpcService
{
  private string[] _stateNames = 
  {
    "Alabama", "Alaska", "Arizona", 
    "Arkansas", "California", "Colorado", 
    "Connecticut", "Delaware", "Florida", 
    "Georgia", "Hawaii", "Idaho", 
    "Illinois", "Indiana", "Iowa", 
    "Kansas", "Kentucky", "Louisiana", 
    "Maine", "Maryland", "Massachusetts", 
    "Michigan", "Minnesota", "Mississippi", 
    "Missouri", "Montana", "Nebraska", 
    "Nevada", "New Hampshire", "New Jersey", 
    "New Mexico", "New York", "North Carolina", 
    "North Dakota", "Ohio", "Oklahoma", 
    "Oregon", "Pennsylvania", "Rhode Island", 
    "South Carolina", "South Dakota", "Tennessee", 
    "Texas", "Utah", "Vermont", 
    "Virginia", "Washington", "West Virginia", 
    "Wisconsin", "Wyoming" 
  };

  [XmlRpcMethod("examples.getStateName")]
  [return:XmlRpcReturnValue(Description="Name of state corresponding " +
    "to stateNumber parameter")]
  public string GetStateName(
    [XmlRpcParameter(Description="number of state (1-50)")]
    int stateNumber)
  {
    if (stateNumber < 1 || stateNumber > _stateNames.Length)
      throw new XmlRpcFaultException(1, 
       String.Format("{0} is not valid state number.", stateNumber));
    return _stateNames[stateNumber-1];
  }

  [XmlRpcMethod("examples.getStateStruct")]
  [return:XmlRpcReturnValue(Description="List of comma separated " +
    "state names. Note that the order of the names may not " +
    "correspond to the order of the numbers in the struct " +
    "input parameter. This is because XML-RPC struct members " +
    "are specified to be unordered (an array input parameter " +
    "would have been more appropriate).")]
  public string GetStateNames(
    [XmlRpcParameter(Description="struct containing any number of " +
      "integers representing state numbers")]XmlRpcStruct request)
  {
    string ret = "";
    foreach (DictionaryEntry de in request)
    {
      if (!(de.Value is int))
        throw new XmlRpcFaultException(2, 
          "struct parameter contains a non-integer member");
      if (ret != "")
        ret += ",";
      ret += GetStateName((int)de.Value);
    }
    return ret; 
  }
}
