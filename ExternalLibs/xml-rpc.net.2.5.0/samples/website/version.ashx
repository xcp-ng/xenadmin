<%@ WebHandler Language="C#" Class="VersionService" %> <%@ Assembly Name="CookComputing.XmlRpcV2" %>
 
using System;
using System.Collections;
using System.Web;
using CookComputing.XmlRpc;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Runtime.InteropServices;

[XmlRpcService(Description=
"Version Service")]
public class VersionService: XmlRpcService
{

  [XmlRpcMethod]
  public string GetRuntimeVersion()
  {
      return RuntimeEnvironment.GetSystemVersion();
  }

  [XmlRpcMethod]
  public bool IsFullTrust()
  {
    try
    {
        new PermissionSet(PermissionState.Unrestricted).Demand();
    }
    catch (SecurityException ex)
    {
        return false;
    }
    return true;
  }
}