/* 
XML-RPC.NET library
Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

namespace CookComputing.XmlRpc
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public enum XmlRpcType
  {
    tInvalid,
    tInt32,
    tInt64,
    tBoolean,
    tString,
    tDouble,
    tDateTime,
    tBase64,
    tStruct,
    tHashtable,
    tArray,
    tMultiDimArray,
    tVoid
  }

  public class XmlRpcServiceInfo
  {
    public static XmlRpcServiceInfo CreateServiceInfo(Type type)
    {
      XmlRpcServiceInfo svcInfo = new XmlRpcServiceInfo();
      // extract service info
      XmlRpcServiceAttribute svcAttr = (XmlRpcServiceAttribute)
        Attribute.GetCustomAttribute(type, typeof(XmlRpcServiceAttribute));
      if (svcAttr != null && svcAttr.Description != "")
        svcInfo.doc = svcAttr.Description;
      if (svcAttr != null && svcAttr.Name != "")
        svcInfo.Name = svcAttr.Name;
      else
        svcInfo.Name = type.Name;
      // extract method info
      Hashtable methods = new Hashtable();

      foreach (Type itf in type.GetInterfaces())
      {
        XmlRpcServiceAttribute itfAttr = (XmlRpcServiceAttribute)
          Attribute.GetCustomAttribute(itf, typeof(XmlRpcServiceAttribute));
        if (itfAttr != null)
          svcInfo.doc = itfAttr.Description;
#if (!COMPACT_FRAMEWORK)
        InterfaceMapping imap = type.GetInterfaceMap(itf);
        foreach (MethodInfo mi in imap.InterfaceMethods)
        {
          ExtractMethodInfo(methods, mi, itf);
        }
#else
        foreach (MethodInfo mi in itf.GetMethods())
        {
          ExtractMethodInfo(methods, mi, itf);
        }
#endif
      }

      foreach (MethodInfo mi in type.GetMethods())
      {
        ArrayList mthds = new ArrayList();
        mthds.Add(mi);
        MethodInfo curMi = mi;
        while (true)
        {
          MethodInfo baseMi = curMi.GetBaseDefinition();
          if (baseMi.DeclaringType == curMi.DeclaringType)
            break;
          mthds.Insert(0, baseMi);
          curMi = baseMi;
        }
        foreach (MethodInfo mthd in mthds)
        {
          ExtractMethodInfo(methods, mthd, type);
        }
      }
      svcInfo.methodInfos = new XmlRpcMethodInfo[methods.Count];
      methods.Values.CopyTo(svcInfo.methodInfos, 0);
      Array.Sort(svcInfo.methodInfos);
      return svcInfo;
    }

    static void ExtractMethodInfo(Hashtable methods, MethodInfo mi, Type type)
    {
      XmlRpcMethodAttribute attr = (XmlRpcMethodAttribute)
        Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr == null)
        return;
      XmlRpcMethodInfo mthdInfo = new XmlRpcMethodInfo();
      mthdInfo.MethodInfo = mi;
      mthdInfo.XmlRpcName = GetXmlRpcMethodName(mi);
      mthdInfo.MiName = mi.Name;
      mthdInfo.Doc = attr.Description;
      mthdInfo.IsHidden = attr.IntrospectionMethod | attr.Hidden;
      // extract parameters information
      ArrayList parmList = new ArrayList();
      ParameterInfo[] parms = mi.GetParameters();
      foreach (ParameterInfo parm in parms)
      {
        XmlRpcParameterInfo parmInfo = new XmlRpcParameterInfo();
        parmInfo.Name = parm.Name;
        parmInfo.Type = parm.ParameterType;
        parmInfo.XmlRpcType = GetXmlRpcTypeString(parm.ParameterType);
        // retrieve optional attributed info
        parmInfo.Doc = "";
        XmlRpcParameterAttribute pattr = (XmlRpcParameterAttribute)
          Attribute.GetCustomAttribute(parm,
          typeof(XmlRpcParameterAttribute));
        if (pattr != null)
        {
          parmInfo.Doc = pattr.Description;
          parmInfo.XmlRpcName = pattr.Name;
        }
        parmInfo.IsParams = Attribute.IsDefined(parm,
          typeof(ParamArrayAttribute));
        parmList.Add(parmInfo);
      }
      mthdInfo.Parameters = (XmlRpcParameterInfo[])
        parmList.ToArray(typeof(XmlRpcParameterInfo));
      // extract return type information
      mthdInfo.ReturnType = mi.ReturnType;
      mthdInfo.ReturnXmlRpcType = GetXmlRpcTypeString(mi.ReturnType);
      object[] orattrs = mi.ReturnTypeCustomAttributes.GetCustomAttributes(
        typeof(XmlRpcReturnValueAttribute), false);
      if (orattrs.Length > 0)
      {
        mthdInfo.ReturnDoc = ((XmlRpcReturnValueAttribute)orattrs[0]).Description;
      }

      if (methods[mthdInfo.XmlRpcName] != null)
      {
        throw new XmlRpcDupXmlRpcMethodNames(String.Format("Method "
          + "{0} in type {1} has duplicate XmlRpc method name {2}",
          mi.Name, type.Name, mthdInfo.XmlRpcName));
      }
      else
        methods.Add(mthdInfo.XmlRpcName, mthdInfo);
    }

    public MethodInfo GetMethodInfo(string xmlRpcMethodName)
    {
      foreach (XmlRpcMethodInfo xmi in methodInfos)
      {
        if (xmlRpcMethodName == xmi.XmlRpcName)
        {
          return xmi.MethodInfo;
        }
      }
      return null;
    }

    static bool IsVisibleXmlRpcMethod(MethodInfo mi)
    {
      bool ret = false;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null)
      {
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)attr;
        ret = !(mattr.Hidden || mattr.IntrospectionMethod == true);
      }
      return ret;
    }

    public static string GetXmlRpcMethodName(MethodInfo mi)
    {
      XmlRpcMethodAttribute attr = (XmlRpcMethodAttribute)
        Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null
        && attr.Method != null
        && attr.Method != "")
      {
        return attr.Method;
      }
      else
      {
        return mi.Name;
      }
    }

    public string GetMethodName(string XmlRpcMethodName)
    {
      foreach (XmlRpcMethodInfo methodInfo in methodInfos)
      {
        if (methodInfo.XmlRpcName == XmlRpcMethodName)
          return methodInfo.MiName;
      }
      return null;
    }

    public String Doc
    {
      get { return doc; }
      set { doc = value; }
    }

    public String Name
    {
      get { return name; }
      set { name = value; }
    }

    public XmlRpcMethodInfo[] Methods
    {
      get { return methodInfos; }
    }

    public XmlRpcMethodInfo GetMethod(
      String methodName)
    {
      foreach (XmlRpcMethodInfo mthdInfo in methodInfos)
      {
        if (mthdInfo.XmlRpcName == methodName)
          return mthdInfo;
      }
      return null;
    }

    private XmlRpcServiceInfo()
    {
    }

    public static XmlRpcType GetXmlRpcType(Type t)
    {
      return GetXmlRpcType(t, new Stack());
    }

    private static XmlRpcType GetXmlRpcType(Type t, Stack typeStack)
    {
      XmlRpcType ret;
      if (t == typeof(Int32))
        ret = XmlRpcType.tInt32;
      else if (t == typeof(XmlRpcInt))
        ret = XmlRpcType.tInt32;
      else if (t == typeof(Int64))
        ret = XmlRpcType.tInt64;
      else if (t == typeof(Boolean))
        ret = XmlRpcType.tBoolean;
      else if (t == typeof(XmlRpcBoolean))
        ret = XmlRpcType.tBoolean;
      else if (t == typeof(String))
        ret = XmlRpcType.tString;
      else if (t == typeof(Double))
        ret = XmlRpcType.tDouble;
      else if (t == typeof(XmlRpcDouble))
        ret = XmlRpcType.tDouble;
      else if (t == typeof(DateTime))
        ret = XmlRpcType.tDateTime;
      else if (t == typeof(XmlRpcDateTime))
        ret = XmlRpcType.tDateTime;
      else if (t == typeof(byte[]))
        ret = XmlRpcType.tBase64;
      else if (t == typeof(XmlRpcStruct))
      {
        ret = XmlRpcType.tHashtable;
      }
      else if (t == typeof(Array))
        ret = XmlRpcType.tArray;
      else if (t.IsArray)
      {
#if (!COMPACT_FRAMEWORK)
        Type elemType = t.GetElementType();
        if (elemType != typeof(Object)
          && GetXmlRpcType(elemType, typeStack) == XmlRpcType.tInvalid)
        {
          ret = XmlRpcType.tInvalid;
        }
        else
        {
          if (t.GetArrayRank() == 1)  // single dim array
            ret = XmlRpcType.tArray;
          else
            ret = XmlRpcType.tMultiDimArray;
        }
#else
        //!! check types of array elements if not Object[]
        Type elemType = null;
        string[] checkSingleDim = Regex.Split(t.FullName, "\\[\\]$");
        if (checkSingleDim.Length > 1)  // single dim array
        {
          elemType = Type.GetType(checkSingleDim[0]);
          ret = XmlRpcType.tArray;
        }
        else
        {
          string[] checkMultiDim = Regex.Split(t.FullName, "\\[,[,]*\\]$");
          if (checkMultiDim.Length > 1)
          {
            elemType = Type.GetType(checkMultiDim[0]);
            ret = XmlRpcType.tMultiDimArray;
          }
          else
            ret = XmlRpcType.tInvalid;
        }
        if (elemType != null)
        {
          if (elemType != typeof(Object) 
            && GetXmlRpcType(elemType, typeStack) == XmlRpcType.tInvalid)
          {
            ret = XmlRpcType.tInvalid;
          }
        }
#endif

      }
#if !FX1_0
      else if (t == typeof(int?))
        ret = XmlRpcType.tInt32;
      else if (t == typeof(long?))
        ret = XmlRpcType.tInt64;
      else if (t == typeof(Boolean?))
        ret = XmlRpcType.tBoolean;
      else if (t == typeof(Double?))
        ret = XmlRpcType.tDouble;
      else if (t == typeof(DateTime?))
        ret = XmlRpcType.tDateTime;
#endif
      else if (t == typeof(void))
      {
        ret = XmlRpcType.tVoid;
      }
      else if ((t.IsValueType && !t.IsPrimitive && !t.IsEnum)
        || t.IsClass)
      {
        // if type is struct or class its only valid for XML-RPC mapping if all 
        // its members have a valid mapping or are of type object which
        // maps to any XML-RPC type
        MemberInfo[] mis = t.GetMembers();
        foreach (MemberInfo mi in mis)
        {
          if (mi.MemberType == MemberTypes.Field)
          {
            FieldInfo fi = (FieldInfo)mi;
            if (typeStack.Contains(fi.FieldType))
              continue;
            try
            {
              typeStack.Push(fi.FieldType);
              if ((fi.FieldType != typeof(Object)
                && GetXmlRpcType(fi.FieldType, typeStack) == XmlRpcType.tInvalid))
              {
                return XmlRpcType.tInvalid;
              }
            }
            finally
            {
              typeStack.Pop();
            }
          }
          else if (mi.MemberType == MemberTypes.Property)
          {
            PropertyInfo pi = (PropertyInfo)mi;
            if (typeStack.Contains(pi.PropertyType))
              continue;
            try
            {
              typeStack.Push(pi.PropertyType);
              if ((pi.PropertyType != typeof(Object)
                && GetXmlRpcType(pi.PropertyType, typeStack) == XmlRpcType.tInvalid))
              {
                return XmlRpcType.tInvalid;
              }
            }
            finally
            {
              typeStack.Pop();
            }
          }
        }
        ret = XmlRpcType.tStruct;
      }
      else
        ret = XmlRpcType.tInvalid;
      return ret;
    }

    static public string GetXmlRpcTypeString(Type t)
    {
      XmlRpcType rpcType = GetXmlRpcType(t);
      return GetXmlRpcTypeString(rpcType);
    }

    static public string GetXmlRpcTypeString(XmlRpcType t)
    {
      string ret = null;
      if (t == XmlRpcType.tInt32)
        ret = "integer";
      else if (t == XmlRpcType.tInt64)
        ret = "i8";
      else if (t == XmlRpcType.tBoolean)
        ret = "boolean";
      else if (t == XmlRpcType.tString)
        ret = "string";
      else if (t == XmlRpcType.tDouble)
        ret = "double";
      else if (t == XmlRpcType.tDateTime)
        ret = "dateTime";
      else if (t == XmlRpcType.tBase64)
        ret = "base64";
      else if (t == XmlRpcType.tStruct)
        ret = "struct";
      else if (t == XmlRpcType.tHashtable)
        ret = "struct";
      else if (t == XmlRpcType.tArray)
        ret = "array";
      else if (t == XmlRpcType.tMultiDimArray)
        ret = "array";
      else if (t == XmlRpcType.tVoid)
        ret = "void";
      else
        ret = null;
      return ret;
    }

    XmlRpcMethodInfo[] methodInfos;
    String doc;
    string name;
  }
}