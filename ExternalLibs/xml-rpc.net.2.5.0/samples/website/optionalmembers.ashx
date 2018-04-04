<%@ WebHandler Language="C#" Class="OptionalMembers" %> <%@ Assembly Name="CookComputing.XmlRpcV2" %>
 
using System;
using System.Collections;
using System.Web;
using CookComputing.XmlRpc;

public struct NameValuePair
{
  public string name;  
  public object value;
  public NameValuePair(string Name, object Value)
  {
    name = Name;
    value = Value;
  }
}

[XmlRpcService(Description=
"This sample service can be used to test optional struct members.")]
public class OptionalMembers : XmlRpcService
{
  [XmlRpcMethod]
  [return:XmlRpcReturnValue(Description="Struct members passed in request.")]
  public NameValuePair[] GetNameValuePairs(
    [XmlRpcParameter(Description="Struct containing zero or more members")]
    XmlRpcStruct reqStruct)
  {
    NameValuePair[] ret = new NameValuePair[reqStruct.Count];
    int idx = 0;
    foreach (DictionaryEntry de in reqStruct)
    {
        ret[idx++] = new NameValuePair((string)de.Key, de.Value);
    }
    return ret;
  }

}
