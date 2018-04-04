using System;
using System.Collections;
using CookComputing.XmlRpc;

namespace CookComputing.XmlRpc
{
  public class SystemMethodsBase : MarshalByRefObject
  {
    [XmlRpcMethod("system.listMethods", IntrospectionMethod = true,
       Description =
       "Return an array of all available XML-RPC methods on this Service.")]
    public string[] System__List__Methods___()
    {
      XmlRpcServiceInfo svcInfo = XmlRpcServiceInfo.CreateServiceInfo(
                                    this.GetType());
      ArrayList alist = new ArrayList();
      foreach (XmlRpcMethodInfo mthdInfo in svcInfo.Methods)
      {
        if (!mthdInfo.IsHidden)
          alist.Add(mthdInfo.XmlRpcName);
      }
      return (String[])alist.ToArray(typeof(string));
    }

    [XmlRpcMethod("system.methodSignature", IntrospectionMethod = true,
       Description =
       "Given the name of a method, return an array of legal signatures. " +
       "Each signature is an array of strings. The first item of each " +
       "signature is the return type, and any others items are parameter " +
       "types.")]
    public Array System__Method__Signature___(string MethodName)
    {
      //TODO: support overloaded methods
      XmlRpcServiceInfo svcInfo = XmlRpcServiceInfo.CreateServiceInfo(
        this.GetType());
      XmlRpcMethodInfo mthdInfo = svcInfo.GetMethod(MethodName);
      if (mthdInfo == null)
      {
        throw new XmlRpcFaultException(880,
          "Request for information on unsupported method");
      }
      if (mthdInfo.IsHidden)
      {
        throw new XmlRpcFaultException(881,
          "Information not available on this method");
      }
      //XmlRpcTypes.CheckIsXmlRpcMethod(mi);
      ArrayList alist = new ArrayList();
      alist.Add(XmlRpcServiceInfo.GetXmlRpcTypeString(mthdInfo.ReturnType));
      foreach (XmlRpcParameterInfo paramInfo in mthdInfo.Parameters)
      {
        alist.Add(XmlRpcServiceInfo.GetXmlRpcTypeString(paramInfo.Type));
      }
      string[] types = (string[])alist.ToArray(typeof(string));
      ArrayList retalist = new ArrayList();
      retalist.Add(types);
      Array retarray = retalist.ToArray(typeof(string[]));
      return retarray;
    }

    [XmlRpcMethod("system.methodHelp", IntrospectionMethod = true,
       Description =
       "Given the name of a method, return a help string.")]
    public string System__Method__Help___(string MethodName)
    {
      //TODO: support overloaded methods?
      XmlRpcServiceInfo svcInfo = XmlRpcServiceInfo.CreateServiceInfo(
        this.GetType());
      XmlRpcMethodInfo mthdInfo = svcInfo.GetMethod(MethodName);
      if (mthdInfo == null)
      {
        throw new XmlRpcFaultException(880,
          "Request for information on unsupported method");
      }
      if (mthdInfo.IsHidden)
      {
        throw new XmlRpcFaultException(881,
          "Information not available for this method");
      }
      return mthdInfo.Doc;
    }
  }
}
