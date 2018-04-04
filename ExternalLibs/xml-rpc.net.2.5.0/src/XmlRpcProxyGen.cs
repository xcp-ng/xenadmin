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
  using System.Reflection;
  using System.Reflection.Emit;

  public class XmlRpcProxyGen
  {
    static Hashtable _types = new Hashtable();

#if (!FX1_0)
    public static T Create<T>()
    {
      return (T)Create(typeof(T));
    }
#endif

    public static object Create(Type itf)
    {
      // create transient assembly
      Type proxyType;
      lock (typeof(XmlRpcProxyGen))
      {
        proxyType = (Type)_types[itf];
        if (proxyType == null)
        {
          Guid guid = Guid.NewGuid();
          string assemblyName = "XmlRpcProxy" + guid.ToString();
          string moduleName = "XmlRpcProxy" + guid.ToString() + ".dll";
          string typeName = "XmlRpcProxy" + guid.ToString();
          AssemblyBuilder assBldr = BuildAssembly(itf, assemblyName,
            moduleName, typeName, AssemblyBuilderAccess.Run);
          proxyType = assBldr.GetType(typeName);
          _types.Add(itf, proxyType);
        }
      }
      object ret = Activator.CreateInstance(proxyType);
      return ret;
    }

    public static object CreateAssembly(
      Type itf,
      string typeName,
      string assemblyName
      )
    {
      // create persistable assembly
      if (assemblyName.IndexOf(".dll") == (assemblyName.Length - 4))
        assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
      string moduleName = assemblyName + ".dll";
      AssemblyBuilder assBldr = BuildAssembly(itf, assemblyName,
        moduleName, typeName, AssemblyBuilderAccess.RunAndSave);
      Type proxyType = assBldr.GetType(typeName);
      object ret = Activator.CreateInstance(proxyType);
      assBldr.Save(moduleName);
      return ret;
    }

    static AssemblyBuilder BuildAssembly(
      Type itf,
      string assemblyName,
      string moduleName,
      string typeName,
      AssemblyBuilderAccess access)
    {
      string urlString = GetXmlRpcUrl(itf);
      ArrayList methods = GetXmlRpcMethods(itf);
      ArrayList beginMethods = GetXmlRpcBeginMethods(itf);
      ArrayList endMethods = GetXmlRpcEndMethods(itf);
      AssemblyName assName = new AssemblyName();
      assName.Name = assemblyName;
      if (access == AssemblyBuilderAccess.RunAndSave)
        assName.Version = itf.Assembly.GetName().Version;
      AssemblyBuilder assBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(
        assName, access);
      ModuleBuilder modBldr = (access == AssemblyBuilderAccess.Run
        ? assBldr.DefineDynamicModule(assName.Name)
        : assBldr.DefineDynamicModule(assName.Name, moduleName));
      TypeBuilder typeBldr = modBldr.DefineType(
        typeName,
        TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public,
        typeof(XmlRpcClientProtocol),
        new Type[] { itf });
      BuildConstructor(typeBldr, typeof(XmlRpcClientProtocol), urlString);
      BuildMethods(typeBldr, methods);
      BuildBeginMethods(typeBldr, beginMethods);
      BuildEndMethods(typeBldr, endMethods);
      typeBldr.CreateType();
      return assBldr;
    }

    static void BuildMethods(TypeBuilder tb, ArrayList methods)
    {
      foreach (MethodData mthdData in methods)
      {
        MethodInfo mi = mthdData.mi;
        Type[] argTypes = new Type[mi.GetParameters().Length];
        string[] paramNames = new string[mi.GetParameters().Length];
        for (int i = 0; i < mi.GetParameters().Length; i++)
        {
          argTypes[i] = mi.GetParameters()[i].ParameterType;
          paramNames[i] = mi.GetParameters()[i].Name;
        }
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)
          Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
        BuildMethod(tb, mi.Name, mthdData.xmlRpcName, paramNames, argTypes,
          mthdData.paramsMethod, mi.ReturnType, mattr.StructParams);
      }
    }

    static void BuildMethod(
      TypeBuilder tb,
      string methodName,
      string rpcMethodName,
      string[] paramNames,
      Type[] argTypes,
      bool paramsMethod,
      Type returnType,
      bool structParams)
    {
      MethodBuilder mthdBldr = tb.DefineMethod(
        methodName,
        MethodAttributes.Public | MethodAttributes.Virtual,
        returnType, argTypes);
      // add attribute to method
      Type[] oneString = new Type[1] { typeof(string) };
      Type methodAttr = typeof(XmlRpcMethodAttribute);
      ConstructorInfo ci = methodAttr.GetConstructor(oneString);
      PropertyInfo[] pis 
        = new PropertyInfo[] { methodAttr.GetProperty("StructParams") };
      object[] structParam = new object[] { structParams };
      CustomAttributeBuilder cab =
        new CustomAttributeBuilder(ci, new object[] { rpcMethodName },
          pis, structParam);
      mthdBldr.SetCustomAttribute(cab);
      for (int i = 0; i < paramNames.Length; i++)
      {
        ParameterBuilder paramBldr = mthdBldr.DefineParameter(i + 1, 
          ParameterAttributes.In, paramNames[i]);
        // possibly add ParamArrayAttribute to final parameter
        if (i == paramNames.Length - 1 && paramsMethod)
        {
          ConstructorInfo ctorInfo = typeof(ParamArrayAttribute).GetConstructor(
            new Type[0]);
          CustomAttributeBuilder attrBldr =
            new CustomAttributeBuilder(ctorInfo, new object[0]);
          paramBldr.SetCustomAttribute(attrBldr);
        }
      }
      // generate IL
      ILGenerator ilgen = mthdBldr.GetILGenerator();
      // if non-void return, declared locals for processing return value
      LocalBuilder retVal = null;
      LocalBuilder tempRetVal = null;
      if (typeof(void) != returnType)
      {
        tempRetVal = ilgen.DeclareLocal(typeof(System.Object));
        retVal = ilgen.DeclareLocal(returnType);
      }
      // declare variable to store method args and emit code to populate ut
      LocalBuilder argValues = ilgen.DeclareLocal(typeof(System.Object[]));
      ilgen.Emit(OpCodes.Ldc_I4, argTypes.Length);
      ilgen.Emit(OpCodes.Newarr, typeof(System.Object));
      ilgen.Emit(OpCodes.Stloc, argValues);
      for (int argLoad = 0; argLoad < argTypes.Length; argLoad++)
      {
        ilgen.Emit(OpCodes.Ldloc, argValues);
        ilgen.Emit(OpCodes.Ldc_I4, argLoad);
        ilgen.Emit(OpCodes.Ldarg, argLoad + 1);
        if (argTypes[argLoad].IsValueType)
        {
          ilgen.Emit(OpCodes.Box, argTypes[argLoad]);
        }
        ilgen.Emit(OpCodes.Stelem_Ref);
      }
      // call Invoke on base class
      Type[] invokeTypes = new Type[] { typeof(MethodInfo), typeof(object[]) };
      MethodInfo invokeMethod
        = typeof(XmlRpcClientProtocol).GetMethod("Invoke", invokeTypes);
      ilgen.Emit(OpCodes.Ldarg_0);
      ilgen.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
      ilgen.Emit(OpCodes.Castclass, typeof(System.Reflection.MethodInfo));
      ilgen.Emit(OpCodes.Ldloc, argValues);
      ilgen.Emit(OpCodes.Call, invokeMethod);
      //  if non-void return prepare return value, otherwise pop to discard 
      if (typeof(void) != returnType)
      {
        // if return value is null, don't cast it to required type
        Label retIsNull = ilgen.DefineLabel();
        ilgen.Emit(OpCodes.Stloc, tempRetVal);
        ilgen.Emit(OpCodes.Ldloc, tempRetVal);
        ilgen.Emit(OpCodes.Brfalse, retIsNull);
        ilgen.Emit(OpCodes.Ldloc, tempRetVal);
        if (true == returnType.IsValueType)
        {
          ilgen.Emit(OpCodes.Unbox, returnType);
          ilgen.Emit(OpCodes.Ldobj, returnType);
        }
        else
        {
          ilgen.Emit(OpCodes.Castclass, returnType);
        }
        ilgen.Emit(OpCodes.Stloc, retVal);
        ilgen.MarkLabel(retIsNull);
        ilgen.Emit(OpCodes.Ldloc, retVal);
      }
      else
      {
        ilgen.Emit(OpCodes.Pop);
      }
      ilgen.Emit(OpCodes.Ret);
    }

    static void BuildBeginMethods(TypeBuilder tb, ArrayList methods)
    {
      foreach (MethodData mthdData in methods)
      {
        MethodInfo mi = mthdData.mi;
        // assume method has already been validated for required signature   
        int paramCount = mi.GetParameters().Length;
        // argCount counts of params before optional AsyncCallback param
        int argCount = paramCount;
        Type[] argTypes = new Type[paramCount];
        for (int i = 0; i < mi.GetParameters().Length; i++)
        {
          argTypes[i] = mi.GetParameters()[i].ParameterType;
          if (argTypes[i] == typeof(System.AsyncCallback))
            argCount = i;
        }
        MethodBuilder mthdBldr = tb.DefineMethod(
          mi.Name,
          MethodAttributes.Public | MethodAttributes.Virtual,
          mi.ReturnType,
          argTypes);
        // add attribute to method
        Type[] oneString = new Type[1] { typeof(string) };
        Type methodAttr = typeof(XmlRpcBeginAttribute);
        ConstructorInfo ci = methodAttr.GetConstructor(oneString);
        CustomAttributeBuilder cab =
          new CustomAttributeBuilder(ci, new object[] { mthdData.xmlRpcName });
        mthdBldr.SetCustomAttribute(cab);
        // start generating IL
        ILGenerator ilgen = mthdBldr.GetILGenerator();
        // declare variable to store method args and emit code to populate it
        LocalBuilder argValues = ilgen.DeclareLocal(typeof(System.Object[]));
        ilgen.Emit(OpCodes.Ldc_I4, argCount);
        ilgen.Emit(OpCodes.Newarr, typeof(System.Object));
        ilgen.Emit(OpCodes.Stloc, argValues);
        for (int argLoad = 0; argLoad < argCount; argLoad++)
        {
          ilgen.Emit(OpCodes.Ldloc, argValues);
          ilgen.Emit(OpCodes.Ldc_I4, argLoad);
          ilgen.Emit(OpCodes.Ldarg, argLoad + 1);
          ParameterInfo pi = mi.GetParameters()[argLoad];
          string paramTypeName = pi.ParameterType.AssemblyQualifiedName;
          paramTypeName = paramTypeName.Replace("&", "");
          Type paramType = Type.GetType(paramTypeName);
          if (paramType.IsValueType)
          {
            ilgen.Emit(OpCodes.Box, paramType);
          }
          ilgen.Emit(OpCodes.Stelem_Ref);
        }
        // emit code to store AsyncCallback parameter, defaulting to null 
        // if not in method signature
        LocalBuilder acbValue = ilgen.DeclareLocal(typeof(System.AsyncCallback));
        if (argCount < paramCount)
        {
          ilgen.Emit(OpCodes.Ldarg, argCount + 1);
          ilgen.Emit(OpCodes.Stloc, acbValue);
        }
        // emit code to store async state parameter, defaulting to null 
        // if not in method signature
        LocalBuilder objValue = ilgen.DeclareLocal(typeof(System.Object));
        if (argCount < (paramCount - 1))
        {
          ilgen.Emit(OpCodes.Ldarg, argCount + 2);
          ilgen.Emit(OpCodes.Stloc, objValue);
        }
        // emit code to call BeginInvoke on base class
        Type[] invokeTypes = new Type[] 
      { 
        typeof(MethodInfo), 
        typeof(object[]), 
        typeof(System.Object),
        typeof(System.AsyncCallback),
        typeof(System.Object)
      };
        MethodInfo invokeMethod
          = typeof(XmlRpcClientProtocol).GetMethod("BeginInvoke", invokeTypes);
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
        ilgen.Emit(OpCodes.Castclass, typeof(System.Reflection.MethodInfo));
        ilgen.Emit(OpCodes.Ldloc, argValues);
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Ldloc, acbValue);
        ilgen.Emit(OpCodes.Ldloc, objValue);
        ilgen.Emit(OpCodes.Call, invokeMethod);
        // BeginInvoke will leave IAsyncResult on stack - leave it there
        // for return value from method being built
        ilgen.Emit(OpCodes.Ret);
      }
    }

    static void BuildEndMethods(TypeBuilder tb, ArrayList methods)
    {
      LocalBuilder retVal = null;
      LocalBuilder tempRetVal = null;
      foreach (MethodData mthdData in methods)
      {
        MethodInfo mi = mthdData.mi;
        Type[] argTypes = new Type[] { typeof(System.IAsyncResult) };
        MethodBuilder mthdBldr = tb.DefineMethod(mi.Name,
          MethodAttributes.Public | MethodAttributes.Virtual,
          mi.ReturnType, argTypes);
        // start generating IL
        ILGenerator ilgen = mthdBldr.GetILGenerator();
        // if non-void return, declared locals for processing return value
        if (typeof(void) != mi.ReturnType)
        {
          tempRetVal = ilgen.DeclareLocal(typeof(System.Object));
          retVal = ilgen.DeclareLocal(mi.ReturnType);
        }
        // call EndInvoke on base class
        Type[] invokeTypes
          = new Type[] { typeof(System.IAsyncResult), typeof(System.Type) };
        MethodInfo invokeMethod
          = typeof(XmlRpcClientProtocol).GetMethod("EndInvoke", invokeTypes);
        Type[] GetTypeTypes
          = new Type[] { typeof(System.String) };
        MethodInfo GetTypeMethod
          = typeof(System.Type).GetMethod("GetType", GetTypeTypes);
        ilgen.Emit(OpCodes.Ldarg_0);  // "this"
        ilgen.Emit(OpCodes.Ldarg_1);  // IAsyncResult parameter
        ilgen.Emit(OpCodes.Ldstr, mi.ReturnType.AssemblyQualifiedName);
        ilgen.Emit(OpCodes.Call, GetTypeMethod);
        ilgen.Emit(OpCodes.Call, invokeMethod);
        //  if non-void return prepare return value otherwise pop to discard 
        if (typeof(void) != mi.ReturnType)
        {
          // if return value is null, don't cast it to required type
          Label retIsNull = ilgen.DefineLabel();
          ilgen.Emit(OpCodes.Stloc, tempRetVal);
          ilgen.Emit(OpCodes.Ldloc, tempRetVal);
          ilgen.Emit(OpCodes.Brfalse, retIsNull);
          ilgen.Emit(OpCodes.Ldloc, tempRetVal);
          if (true == mi.ReturnType.IsValueType)
          {
            ilgen.Emit(OpCodes.Unbox, mi.ReturnType);
            ilgen.Emit(OpCodes.Ldobj, mi.ReturnType);
          }
          else
          {
            ilgen.Emit(OpCodes.Castclass, mi.ReturnType);
          }
          ilgen.Emit(OpCodes.Stloc, retVal);
          ilgen.MarkLabel(retIsNull);
          ilgen.Emit(OpCodes.Ldloc, retVal);
        }
        else
        {
          // void method so throw away result from EndInvoke
          ilgen.Emit(OpCodes.Pop);
        }
        ilgen.Emit(OpCodes.Ret);
      }
    }

    private static void BuildConstructor(
      TypeBuilder typeBldr,
      Type baseType,
      string urlStr)
    {
      ConstructorBuilder ctorBldr = typeBldr.DefineConstructor(
        MethodAttributes.Public | MethodAttributes.SpecialName |
        MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
        CallingConventions.Standard,
        Type.EmptyTypes);
      if (urlStr != null && urlStr.Length > 0)
      {
        Type urlAttr = typeof(XmlRpcUrlAttribute);
        Type[] oneString = new Type[1] { typeof(string) };
        ConstructorInfo ci = urlAttr.GetConstructor(oneString);
        CustomAttributeBuilder cab =
          new CustomAttributeBuilder(ci, new object[] { urlStr });
        typeBldr.SetCustomAttribute(cab);
      }
      ILGenerator ilgen = ctorBldr.GetILGenerator();
      //  Call the base constructor.
      ilgen.Emit(OpCodes.Ldarg_0);
      ConstructorInfo ctorInfo = baseType.GetConstructor(System.Type.EmptyTypes);
      ilgen.Emit(OpCodes.Call, ctorInfo);
      ilgen.Emit(OpCodes.Ret);
    }

    private static string GetXmlRpcUrl(Type itf)
    {
      Attribute attr = Attribute.GetCustomAttribute(itf,
        typeof(XmlRpcUrlAttribute));
      if (attr == null)
        return null;
      XmlRpcUrlAttribute xruAttr = attr as XmlRpcUrlAttribute;
      string url = xruAttr.Uri;
      return url;
    }

    /// <summary>
    /// Type.GetMethods() does not return methods that a derived interface
    /// inherits from its base interfaces; this method does.
    /// </summary>
    private static MethodInfo[] GetMethods(Type type)
    {
      MethodInfo[] methods = type.GetMethods();
      if (!type.IsInterface)
      {
        return methods;
      }

      Type[] interfaces = type.GetInterfaces();
      if (interfaces.Length == 0)
      {
        return methods;
      }

      ArrayList result = new ArrayList(methods);
      foreach (Type itf in type.GetInterfaces())
      {
        result.AddRange(itf.GetMethods());
      }
      return (MethodInfo[])result.ToArray(typeof(MethodInfo));
    }

    private static ArrayList GetXmlRpcMethods(Type itf)
    {
      ArrayList ret = new ArrayList();
      if (!itf.IsInterface)
        throw new Exception("type not interface");
      foreach (MethodInfo mi in GetMethods(itf))
      {
        string xmlRpcName = GetXmlRpcMethodName(mi);
        if (xmlRpcName == null)
          continue;
        ParameterInfo[] pis = mi.GetParameters();
        bool paramsMethod = pis.Length > 0 && Attribute.IsDefined(
          pis[pis.Length - 1], typeof(ParamArrayAttribute));
        ret.Add(new MethodData(mi, xmlRpcName, paramsMethod));
      }
      return ret;
    }

    private static string GetXmlRpcMethodName(MethodInfo mi)
    {
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr == null)
        return null;
      XmlRpcMethodAttribute xrmAttr = attr as XmlRpcMethodAttribute;
      string rpcMethod = xrmAttr.Method;
      if (rpcMethod == "")
      {
        rpcMethod = mi.Name;
      }
      return rpcMethod;
    }

    class MethodData
    {
      public MethodData(MethodInfo Mi, string XmlRpcName, bool ParamsMethod)
      {
        mi = Mi;
        xmlRpcName = XmlRpcName;
        paramsMethod = ParamsMethod;
        returnType = null;
      }
      public MethodData(MethodInfo Mi, string XmlRpcName, bool ParamsMethod,
        Type ReturnType)
      {
        mi = Mi;
        xmlRpcName = XmlRpcName;
        paramsMethod = ParamsMethod;
        returnType = ReturnType;
      }
      public MethodInfo mi;
      public string xmlRpcName;
      public Type returnType;
      public bool paramsMethod;
    }

    private static ArrayList GetXmlRpcBeginMethods(Type itf)
    {
      ArrayList ret = new ArrayList();
      if (!itf.IsInterface)
        throw new Exception("type not interface");
      foreach (MethodInfo mi in itf.GetMethods())
      {
        Attribute attr = Attribute.GetCustomAttribute(mi,
          typeof(XmlRpcBeginAttribute));
        if (attr == null)
          continue;
        string rpcMethod = ((XmlRpcBeginAttribute)attr).Method;
        if (rpcMethod == "")
        {
          if (!mi.Name.StartsWith("Begin") || mi.Name.Length <= 5)
            throw new Exception(String.Format(
              "method {0} has invalid signature for begin method",
              mi.Name));
          rpcMethod = mi.Name.Substring(5);
        }
        int paramCount = mi.GetParameters().Length;
        int i;
        for (i = 0; i < paramCount; i++)
        {
          Type paramType = mi.GetParameters()[0].ParameterType;
          if (paramType == typeof(System.AsyncCallback))
            break;
        }
        if (paramCount > 1)
        {
          if (i < paramCount - 2)
            throw new Exception(String.Format(
              "method {0} has invalid signature for begin method", mi.Name));
          if (i == (paramCount - 2))
          {
            Type paramType = mi.GetParameters()[i + 1].ParameterType;
            if (paramType != typeof(System.Object))
              throw new Exception(String.Format(
                "method {0} has invalid signature for begin method",
                mi.Name));
          }
        }
        ret.Add(new MethodData(mi, rpcMethod, false, null));
      }
      return ret;
    }

    private static ArrayList GetXmlRpcEndMethods(Type itf)
    {
      ArrayList ret = new ArrayList();
      if (!itf.IsInterface)
        throw new Exception("type not interface");
      foreach (MethodInfo mi in itf.GetMethods())
      {
        Attribute attr = Attribute.GetCustomAttribute(mi,
          typeof(XmlRpcEndAttribute));
        if (attr == null)
          continue;
        ParameterInfo[] pis = mi.GetParameters();
        if (pis.Length != 1)
          throw new Exception(String.Format(
            "method {0} has invalid signature for end method", mi.Name));
        Type paramType = pis[0].ParameterType;
        if (paramType != typeof(System.IAsyncResult))
          throw new Exception(String.Format(
            "method {0} has invalid signature for end method", mi.Name));
        ret.Add(new MethodData(mi, "", false));
      }
      return ret;
    }
  }
}