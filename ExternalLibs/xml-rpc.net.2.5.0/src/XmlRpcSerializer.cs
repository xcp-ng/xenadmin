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

// TODO: overriding default mapping action in a struct should not affect nested structs

namespace CookComputing.XmlRpc
{
  using System;
  using System.Collections;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;

  struct Fault
  {
    public int faultCode;
    public string faultString;
  }

  public class XmlRpcSerializer
  {
    // public properties

    public int Indentation
    {
      get { return m_indentation; }
      set { m_indentation = value; }
    }
    int m_indentation = 2;

    public XmlRpcNonStandard NonStandard
    {
      get { return m_nonStandard; }
      set { m_nonStandard = value; }
    }
    XmlRpcNonStandard m_nonStandard = XmlRpcNonStandard.None;

    public bool UseEmptyParamsTag
    {
      get { return m_bUseEmptyParamsTag; }
      set { m_bUseEmptyParamsTag = value; }
    }
    bool m_bUseEmptyParamsTag = true;

    public bool UseIndentation
    {
      get { return m_bUseIndentation; }
      set { m_bUseIndentation = value; }
    }
    bool m_bUseIndentation = true;

    public bool UseIntTag
    {
      get { return m_useIntTag; }
      set { m_useIntTag = value; }
    }
    bool m_useIntTag;

    public bool UseStringTag
    {
      get { return m_useStringTag; }
      set { m_useStringTag = value; }
    }
    bool m_useStringTag = true;

    public Encoding XmlEncoding
    {
      get { return m_encoding; }
      set { m_encoding = value; }
    }
    Encoding m_encoding = null;

    // private properties
    bool AllowInvalidHTTPContent
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowInvalidHTTPContent) != 0; }
    }

    bool AllowNonStandardDateTime
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowNonStandardDateTime) != 0; }
    }

    bool AllowStringFaultCode
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowStringFaultCode) != 0; }
    }

    bool IgnoreDuplicateMembers
    {
      get { return (m_nonStandard & XmlRpcNonStandard.IgnoreDuplicateMembers) != 0; }
    }

    bool MapEmptyDateTimeToMinValue
    {
      get { return (m_nonStandard & XmlRpcNonStandard.MapEmptyDateTimeToMinValue) != 0; }
    }

    bool MapZerosDateTimeToMinValue
    {
      get { return (m_nonStandard & XmlRpcNonStandard.MapZerosDateTimeToMinValue) != 0; }
    }

    public void SerializeRequest(Stream stm, XmlRpcRequest request) 
    {
      XmlTextWriter xtw = new XmlTextWriter(stm, m_encoding);
      ConfigureXmlFormat(xtw);
      xtw.WriteStartDocument();
      xtw.WriteStartElement("", "methodCall", "");
    {
      // TODO: use global action setting
      MappingAction mappingAction = MappingAction.Error; 
      if (request.xmlRpcMethod == null)
        xtw.WriteElementString("methodName", request.method);
      else
        xtw.WriteElementString("methodName", request.xmlRpcMethod);
      if (request.args.Length > 0 || UseEmptyParamsTag)
      {
        xtw.WriteStartElement("", "params", "");
        try
        {
          if (!IsStructParamsMethod(request.mi))
            SerializeParams(xtw, request, mappingAction);
          else
            SerializeStructParams(xtw, request, mappingAction);
        }
        catch (XmlRpcUnsupportedTypeException ex)
        {
          throw new XmlRpcUnsupportedTypeException(ex.UnsupportedType,
            String.Format("A parameter is of, or contains an instance of, "
            + "type {0} which cannot be mapped to an XML-RPC type",
            ex.UnsupportedType));
        }
        xtw.WriteEndElement();
      }
    }
      xtw.WriteEndElement();
      xtw.Flush();
    }

    void SerializeParams(XmlTextWriter xtw, XmlRpcRequest request,
      MappingAction mappingAction)
    {
      ParameterInfo[] pis = null;
      if (request.mi != null)
      {
        pis = request.mi.GetParameters();
      }
      for (int i = 0; i < request.args.Length; i++)
      {
        if (pis != null)
        {
          if (i >= pis.Length)
            throw new XmlRpcInvalidParametersException("Number of request "
              + "parameters greater than number of proxy method parameters.");
          if (i == pis.Length - 1 
            && Attribute.IsDefined(pis[i], typeof(ParamArrayAttribute)))
          {
            Array ary = (Array)request.args[i];
            foreach (object o in ary)
            {
              if (o == null)
                throw new XmlRpcNullParameterException(
                  "Null parameter in params array");
              xtw.WriteStartElement("", "param", "");
              Serialize(xtw, o, mappingAction);
              xtw.WriteEndElement();
            }
            break;
          }
        }
        if (request.args[i] == null)
        {
          throw new XmlRpcNullParameterException(String.Format(
            "Null method parameter #{0}", i + 1));
        }
        xtw.WriteStartElement("", "param", "");
        Serialize(xtw, request.args[i], mappingAction);
        xtw.WriteEndElement();
      }
    }

    void SerializeStructParams(XmlTextWriter xtw, XmlRpcRequest request,
      MappingAction mappingAction)
    {
      ParameterInfo[] pis = request.mi.GetParameters();
      if (request.args.Length > pis.Length)
        throw new XmlRpcInvalidParametersException("Number of request "
          + "parameters greater than number of proxy method parameters.");
      if (Attribute.IsDefined(pis[request.args.Length - 1],
        typeof(ParamArrayAttribute)))
      {
        throw new XmlRpcInvalidParametersException("params parameter cannot "
          + "be used with StructParams.");
      }
      xtw.WriteStartElement("", "param", "");
      xtw.WriteStartElement("", "value", "");
      xtw.WriteStartElement("", "struct", "");
      for (int i = 0; i < request.args.Length; i++)
      {
        if (request.args[i] == null)
        {
          throw new XmlRpcNullParameterException(String.Format(
            "Null method parameter #{0}", i + 1));
        }
        xtw.WriteStartElement("", "member", "");
        xtw.WriteElementString("name", pis[i].Name);
        Serialize(xtw, request.args[i], mappingAction);
        xtw.WriteEndElement();
      }
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.WriteEndElement();
    }

#if (!COMPACT_FRAMEWORK)
    public XmlRpcRequest DeserializeRequest(Stream stm, Type svcType)
    {
      if (stm == null)
        throw new ArgumentNullException("stm",
          "XmlRpcSerializer.DeserializeRequest");
      XmlDocument xdoc = new XmlDocument(); 
      xdoc.PreserveWhitespace = true;
      try
      {
        using (XmlTextReader xmlRdr = new XmlTextReader(stm))
        {
          xmlRdr.ProhibitDtd = true;
          xdoc.Load(xmlRdr);
        }
      }
      catch (Exception ex)
      {
        throw new XmlRpcIllFormedXmlException(
          "Request from client does not contain valid XML.", ex);
      }
      return DeserializeRequest(xdoc, svcType);
    }

    public XmlRpcRequest DeserializeRequest(TextReader txtrdr, Type svcType)
    {
      if (txtrdr == null)
        throw new ArgumentNullException("txtrdr",
          "XmlRpcSerializer.DeserializeRequest");
      XmlDocument xdoc = new XmlDocument();
      xdoc.PreserveWhitespace = true;
      try
      {
        using (XmlTextReader xmlRdr = new XmlTextReader(txtrdr))
        {
          xmlRdr.ProhibitDtd = true;
          xdoc.Load(xmlRdr);
        }
      }
      catch (Exception ex)
      {
        throw new XmlRpcIllFormedXmlException(
          "Request from client does not contain valid XML.", ex);
      }
      return DeserializeRequest(xdoc, svcType);
    }

    public XmlRpcRequest DeserializeRequest(XmlDocument xdoc, Type svcType)
    {
      XmlRpcRequest request = new XmlRpcRequest();
      XmlNode callNode = SelectSingleNode(xdoc, "methodCall");
      if (callNode == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Request XML not valid XML-RPC - missing methodCall element.");
      }
      XmlNode methodNode = SelectSingleNode(callNode, "methodName");
      if (methodNode == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Request XML not valid XML-RPC - missing methodName element.");
      }
      if (methodNode.FirstChild == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Request XML not valid XML-RPC - missing methodName element.");
      }
      request.method = methodNode.FirstChild.Value;
      if (request.method == "")
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Request XML not valid XML-RPC - empty methodName.");
      }
      request.mi = null;
      ParameterInfo[] pis = new ParameterInfo[0];
      if (svcType != null)
      {
        // retrieve info for the method which handles this XML-RPC method
        XmlRpcServiceInfo svcInfo
          = XmlRpcServiceInfo.CreateServiceInfo(svcType);
        request.mi = svcInfo.GetMethodInfo(request.method);
        // if a service type has been specified and we cannot find the requested
        // method then we must throw an exception
        if (request.mi == null)
        {
          string msg = String.Format("unsupported method called: {0}",
                                      request.method);
          throw new XmlRpcUnsupportedMethodException(msg);
        }
        // method must be marked with XmlRpcMethod attribute
        Attribute attr = Attribute.GetCustomAttribute(request.mi,
          typeof(XmlRpcMethodAttribute));
        if (attr == null)
        {
          throw new XmlRpcMethodAttributeException(
            "Method must be marked with the XmlRpcMethod attribute.");
        }
        pis = request.mi.GetParameters();
      }
      XmlNode paramsNode = SelectSingleNode(callNode, "params");
      if (paramsNode == null)
      {
        if (svcType != null)
        {
          if (pis.Length == 0)
          {
            request.args = new object[0];
            return request;
          }
          else
          {
            throw new XmlRpcInvalidParametersException(
              "Method takes parameters and params element is missing.");
          }
        }
        else
        {
          request.args = new object[0];
          return request;
        }
      }
      XmlNode[] paramNodes = SelectNodes(paramsNode, "param");
      int paramsPos = GetParamsPos(pis);
      int minParamCount = paramsPos == -1 ? pis.Length : paramsPos;
      if (svcType != null && paramNodes.Length < minParamCount)
      {
        throw new XmlRpcInvalidParametersException(
          "Request contains too few param elements based on method signature.");
      }
      if (svcType != null && paramsPos == -1 && paramNodes.Length > pis.Length)
      {
        throw new XmlRpcInvalidParametersException(
          "Request contains too many param elements based on method signature.");
      }
      ParseStack parseStack = new ParseStack("request");
      // TODO: use global action setting
      MappingAction mappingAction = MappingAction.Error;
      int paramObjCount = (paramsPos == -1 ? paramNodes.Length : paramsPos + 1);
      Object[] paramObjs = new Object[paramObjCount];
      // parse ordinary parameters
      int ordinaryParams = (paramsPos == -1 ? paramNodes.Length : paramsPos);
      for (int i = 0; i < ordinaryParams; i++)
      {
        XmlNode paramNode = paramNodes[i];
        XmlNode valueNode = SelectSingleNode(paramNode, "value");
        if (valueNode == null)
          throw new XmlRpcInvalidXmlRpcException("Missing value element.");
        XmlNode node = SelectValueNode(valueNode);
        if (svcType != null)
        {
          parseStack.Push(String.Format("parameter {0}", i + 1));
          // TODO: why following commented out?
          //          parseStack.Push(String.Format("parameter {0} mapped to type {1}", 
          //            i, pis[i].ParameterType.Name));
          paramObjs[i] = ParseValue(node, pis[i].ParameterType, parseStack,
            mappingAction);
        }
        else
        {
          parseStack.Push(String.Format("parameter {0}", i));
          paramObjs[i] = ParseValue(node, null, parseStack, mappingAction);
        }
        parseStack.Pop();
      }
      // parse params parameters
      if (paramsPos != -1)
      {
        Type paramsType = pis[paramsPos].ParameterType.GetElementType();
        Object[] args = new Object[1];
        args[0] = paramNodes.Length - paramsPos;
        Array varargs = (Array)CreateArrayInstance(pis[paramsPos].ParameterType,
          args);
        for (int i = 0; i < varargs.Length; i++)
        {
          XmlNode paramNode = paramNodes[i + paramsPos];
          XmlNode valueNode = SelectSingleNode(paramNode, "value");
          if (valueNode == null)
            throw new XmlRpcInvalidXmlRpcException("Missing value element.");
          XmlNode node = SelectValueNode(valueNode);
          parseStack.Push(String.Format("parameter {0}", i + 1 + paramsPos));
          varargs.SetValue(ParseValue(node, paramsType, parseStack,
            mappingAction), i);
          parseStack.Pop();
        }
        paramObjs[paramsPos] = varargs;
      }
      request.args = paramObjs;
      return request;
    }

    int GetParamsPos(ParameterInfo[] pis)
    {
      if (pis.Length == 0)
        return -1;
      if (Attribute.IsDefined(pis[pis.Length - 1], typeof(ParamArrayAttribute)))
      {
        return pis.Length - 1;
      }
      else
        return -1;
    }

    public void SerializeResponse(Stream stm, XmlRpcResponse response)
    {
      Object ret = response.retVal;
      if (ret is XmlRpcFaultException)
      {
        SerializeFaultResponse(stm, (XmlRpcFaultException)ret);
        return;
      }

      XmlTextWriter xtw = new XmlTextWriter(stm, m_encoding);
      ConfigureXmlFormat(xtw);
      xtw.WriteStartDocument();
      xtw.WriteStartElement("", "methodResponse", "");
      xtw.WriteStartElement("", "params", "");
      // "void" methods actually return an empty string value
      if (ret == null)
      {
        ret = "";
      }
      xtw.WriteStartElement("", "param", "");
      // TODO: use global action setting
      MappingAction mappingAction = MappingAction.Error;
      try
      {
        Serialize(xtw, ret, mappingAction);
      }
      catch (XmlRpcUnsupportedTypeException ex)
      {
        throw new XmlRpcInvalidReturnType(string.Format(
          "Return value is of, or contains an instance of, type {0} which "
          + "cannot be mapped to an XML-RPC type", ex.UnsupportedType));
      }
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.Flush();
    }
#endif

    public XmlRpcResponse DeserializeResponse(Stream stm, Type svcType)
    {
      if (stm == null)
        throw new ArgumentNullException("stm",
          "XmlRpcSerializer.DeserializeResponse");
      if (AllowInvalidHTTPContent)
      {
        Stream newStm = new MemoryStream();
        Util.CopyStream(stm, newStm);
        stm = newStm;
        stm.Position = 0;
        while (true)
        {
          // for now just strip off any leading CR-LF characters
          int byt = stm.ReadByte();
          if (byt == -1)
            throw new XmlRpcIllFormedXmlException(
              "Response from server does not contain valid XML.");
          if (byt != 0x0d && byt != 0x0a && byt != ' ' && byt != '\t')
          {
            stm.Position = stm.Position - 1;
            break;
          }
        }
      }
      XmlDocument xdoc = new XmlDocument();
      xdoc.PreserveWhitespace = true;
      try
      {
        using (XmlTextReader xmlRdr = new XmlTextReader(stm))
        {
#if (!COMPACT_FRAMEWORK)
          xmlRdr.ProhibitDtd = true;
#endif
          xdoc.Load(xmlRdr);
        }
      }
      catch (Exception ex)
      {
        throw new XmlRpcIllFormedXmlException(
          "Response from server does not contain valid XML.", ex);
      }
      return DeserializeResponse(xdoc, svcType);
    }

    public XmlRpcResponse DeserializeResponse(TextReader txtrdr, Type svcType)
    {
      if (txtrdr == null)
        throw new ArgumentNullException("txtrdr",
          "XmlRpcSerializer.DeserializeResponse");
      XmlDocument xdoc = new XmlDocument();
      xdoc.PreserveWhitespace = true;
      try
      {
        using (XmlTextReader xmlRdr = new XmlTextReader(txtrdr))
        {
#if (!COMPACT_FRAMEWORK)
          xmlRdr.ProhibitDtd = true;
#endif
          xdoc.Load(xmlRdr);
        }
      }
      catch (Exception ex)
      {
        throw new XmlRpcIllFormedXmlException(
          "Response from server does not contain valid XML.", ex);
      }
      return DeserializeResponse(xdoc, svcType);
    }

    public XmlRpcResponse DeserializeResponse(XmlDocument xdoc, Type returnType)
    {
      XmlRpcResponse response = new XmlRpcResponse();
      Object retObj = null;
      XmlNode methodResponseNode = SelectSingleNode(xdoc, "methodResponse");
      if (methodResponseNode == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Response XML not valid XML-RPC - missing methodResponse element.");
      }
      // check for fault response
      XmlNode faultNode = SelectSingleNode(methodResponseNode, "fault");
      if (faultNode != null)
      {
        ParseStack parseStack = new ParseStack("fault response");
        // TODO: use global action setting
        MappingAction mappingAction = MappingAction.Error;
        XmlRpcFaultException faultEx = ParseFault(faultNode, parseStack,
          mappingAction);
        throw faultEx;
      }
      XmlNode paramsNode = SelectSingleNode(methodResponseNode, "params");
      if (paramsNode == null && returnType != null)
      {
        if (returnType == typeof(void))
          return new XmlRpcResponse(null);
        else
          throw new XmlRpcInvalidXmlRpcException(
            "Response XML not valid XML-RPC - missing params element.");
      }
      XmlNode paramNode = SelectSingleNode(paramsNode, "param");
      if (paramNode == null && returnType != null)
      {
        if (returnType == typeof(void))
          return new XmlRpcResponse(null);
        else
          throw new XmlRpcInvalidXmlRpcException(
            "Response XML not valid XML-RPC - missing params element.");
      }
      XmlNode valueNode = SelectSingleNode(paramNode, "value");
      if (valueNode == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "Response XML not valid XML-RPC - missing value element.");
      }
      if (returnType == typeof(void))
      {
        retObj = null;
      }
      else
      {
        ParseStack parseStack = new ParseStack("response");
        // TODO: use global action setting
        MappingAction mappingAction = MappingAction.Error;
        XmlNode node = SelectValueNode(valueNode);
        retObj = ParseValue(node, returnType, parseStack, mappingAction);
      }
      response.retVal = retObj;
      return response;
    }

    //#if (DEBUG)
    public
      //#endif
    void Serialize(
      XmlTextWriter xtw,
      Object o,
      MappingAction mappingAction)
    {
      Serialize(xtw, o, mappingAction, new ArrayList(16));
    }

    //#if (DEBUG)
    public
      //#endif
    void Serialize(
      XmlTextWriter xtw,
      Object o,
      MappingAction mappingAction,
      ArrayList nestedObjs)
    {
      if (nestedObjs.Contains(o))
        throw new XmlRpcUnsupportedTypeException(nestedObjs[0].GetType(),
          "Cannot serialize recursive data structure");
      nestedObjs.Add(o);
      try
      {
        xtw.WriteStartElement("", "value", "");
        XmlRpcType xType = XmlRpcServiceInfo.GetXmlRpcType(o.GetType());
        if (xType == XmlRpcType.tArray)
        {
          xtw.WriteStartElement("", "array", "");
          xtw.WriteStartElement("", "data", "");
          Array a = (Array)o;
          foreach (Object aobj in a)
          {
            if (aobj == null)
              throw new XmlRpcMappingSerializeException(String.Format(
                "Items in array cannot be null ({0}[]).",
            o.GetType().GetElementType()));
            Serialize(xtw, aobj, mappingAction, nestedObjs);
          }
          xtw.WriteEndElement();
          xtw.WriteEndElement();
        }
        else if (xType == XmlRpcType.tMultiDimArray)
        {
          Array mda = (Array)o;
          int[] indices = new int[mda.Rank];
          BuildArrayXml(xtw, mda, 0, indices, mappingAction, nestedObjs);
        }
        else if (xType == XmlRpcType.tBase64)
        {
          byte[] buf = (byte[])o;
          xtw.WriteStartElement("", "base64", "");
          xtw.WriteBase64(buf, 0, buf.Length);
          xtw.WriteEndElement();
        }
        else if (xType == XmlRpcType.tBoolean)
        {
          bool boolVal;
          if (o is bool)
            boolVal = (bool)o;
          else
            boolVal = (bool)(XmlRpcBoolean)o;
          if (boolVal)
            xtw.WriteElementString("boolean", "1");
          else
            xtw.WriteElementString("boolean", "0");
        }
        else if (xType == XmlRpcType.tDateTime)
        {
          DateTime dt;
          if (o is DateTime)
            dt = (DateTime)o;
          else
            dt = (XmlRpcDateTime)o;
          string sdt = dt.ToString("yyyyMMdd'T'HH':'mm':'ss",
          DateTimeFormatInfo.InvariantInfo);
          xtw.WriteElementString("dateTime.iso8601", sdt);
        }
        else if (xType == XmlRpcType.tDouble)
        {
          double doubleVal;
          if (o is double)
            doubleVal = (double)o;
          else
            doubleVal = (XmlRpcDouble)o;
          xtw.WriteElementString("double", doubleVal.ToString(null,
          CultureInfo.InvariantCulture));
        }
        else if (xType == XmlRpcType.tHashtable)
        {
          xtw.WriteStartElement("", "struct", "");
          XmlRpcStruct xrs = o as XmlRpcStruct;
          foreach (object obj in xrs.Keys)
          {
            string skey = obj as string;
            xtw.WriteStartElement("", "member", "");
            xtw.WriteElementString("name", skey);
            Serialize(xtw, xrs[skey], mappingAction, nestedObjs);
            xtw.WriteEndElement();
          }
          xtw.WriteEndElement();
        }
        else if (xType == XmlRpcType.tInt32)
        {
          if (UseIntTag)
            xtw.WriteElementString("int", o.ToString());
          else
            xtw.WriteElementString("i4", o.ToString());
        }
        else if (xType == XmlRpcType.tInt64)
        {
          xtw.WriteElementString("i8", o.ToString());
        }
        else if (xType == XmlRpcType.tString)
        {
          if (UseStringTag)
            xtw.WriteElementString("string", (string)o);
          else
            xtw.WriteString((string)o);
        }
        else if (xType == XmlRpcType.tStruct)
        {
          MappingAction structAction
            = StructMappingAction(o.GetType(), mappingAction);
          xtw.WriteStartElement("", "struct", "");
          MemberInfo[] mis = o.GetType().GetMembers();
          foreach (MemberInfo mi in mis)
          {
            if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
              continue;
            if (mi.MemberType == MemberTypes.Field)
            {
              FieldInfo fi = (FieldInfo)mi;
              string member = fi.Name;
              Attribute attrchk = Attribute.GetCustomAttribute(fi,
              typeof(XmlRpcMemberAttribute));
              if (attrchk != null && attrchk is XmlRpcMemberAttribute)
              {
                string mmbr = ((XmlRpcMemberAttribute)attrchk).Member;
                if (mmbr != "")
                  member = mmbr;
              }
              if (fi.GetValue(o) == null)
              {
                MappingAction memberAction = MemberMappingAction(o.GetType(),
                  fi.Name, structAction);
                if (memberAction == MappingAction.Ignore)
                  continue;
                throw new XmlRpcMappingSerializeException(@"Member """ + member +
                  @""" of struct """ + o.GetType().Name + @""" cannot be null.");
              }
              xtw.WriteStartElement("", "member", "");
              xtw.WriteElementString("name", member);
              Serialize(xtw, fi.GetValue(o), mappingAction, nestedObjs);
              xtw.WriteEndElement();
            }
            else if (mi.MemberType == MemberTypes.Property)
            {
              PropertyInfo pi = (PropertyInfo)mi;
              string member = pi.Name;
              Attribute attrchk = Attribute.GetCustomAttribute(pi,
              typeof(XmlRpcMemberAttribute));
              if (attrchk != null && attrchk is XmlRpcMemberAttribute)
              {
                string mmbr = ((XmlRpcMemberAttribute)attrchk).Member;
                if (mmbr != "")
                  member = mmbr;
              }
              if (pi.GetValue(o, null) == null)
              {
                MappingAction memberAction = MemberMappingAction(o.GetType(),
                  pi.Name, structAction);
                if (memberAction == MappingAction.Ignore)
                  continue;
              }
              xtw.WriteStartElement("", "member", "");
              xtw.WriteElementString("name", member);
              Serialize(xtw, pi.GetValue(o, null), mappingAction, nestedObjs);
              xtw.WriteEndElement();
            }
          }
          xtw.WriteEndElement();
        }
        else if (xType == XmlRpcType.tVoid)
          xtw.WriteElementString("string", "");
        else
          throw new XmlRpcUnsupportedTypeException(o.GetType());
        xtw.WriteEndElement();
      }
      catch (System.NullReferenceException)
      {
        throw new XmlRpcNullReferenceException("Attempt to serialize data "
          + "containing null reference");
      }
      finally
      {
        nestedObjs.RemoveAt(nestedObjs.Count - 1);
      }
    }

    void BuildArrayXml(
      XmlTextWriter xtw,
      Array ary,
      int CurRank,
      int[] indices,
      MappingAction mappingAction,
      ArrayList nestedObjs)
    {
      xtw.WriteStartElement("", "array", "");
      xtw.WriteStartElement("", "data", "");
      if (CurRank < (ary.Rank - 1))
      {
        for (int i = 0; i < ary.GetLength(CurRank); i++)
        {
          indices[CurRank] = i;
          xtw.WriteStartElement("", "value", "");
          BuildArrayXml(xtw, ary, CurRank + 1, indices, mappingAction, nestedObjs);
          xtw.WriteEndElement();
        }
      }
      else
      {
        for (int i = 0; i < ary.GetLength(CurRank); i++)
        {
          indices[CurRank] = i;
          Serialize(xtw, ary.GetValue(indices), mappingAction, nestedObjs);
        }
      }
      xtw.WriteEndElement();
      xtw.WriteEndElement();
    }

    Object ParseValue(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      Type parsedType;
      Type parsedArrayType;
      return ParseValue(node, ValueType, parseStack, mappingAction,
        out parsedType, out parsedArrayType);
    }

    //#if (DEBUG)
    public
      //#endif
    Object ParseValue(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction,
      out Type ParsedType,
      out Type ParsedArrayType)
    {
      ParsedType = null;
      ParsedArrayType = null;
      // if suppplied type is System.Object then ignore it because
      // if doesn't provide any useful information (parsing methods
      // expect null in this case)
      Type valType = ValueType;
      if (valType != null && valType.BaseType == null)
        valType = null;

      Object retObj = null;
      if (node == null)
      {
        retObj = "";
      }
      else if (node is XmlText || node is XmlWhitespace)
      {
        if (valType != null && valType != typeof(string))
        {
          throw new XmlRpcTypeMismatchException(parseStack.ParseType
            + " contains implicit string value where "
            + XmlRpcServiceInfo.GetXmlRpcTypeString(valType)
            + " expected " + StackDump(parseStack));
        }
        retObj = node.Value;
      }
      else
      {
        if (node.Name == "array")
          retObj = ParseArray(node, valType, parseStack, mappingAction);
        else if (node.Name == "base64")
          retObj = ParseBase64(node, valType, parseStack, mappingAction);
        else if (node.Name == "struct")
        {
          // if we don't know the expected struct type then we must
          // parse the XML-RPC struct as an instance of XmlRpcStruct
          if (valType != null && valType != typeof(XmlRpcStruct)
            && !valType.IsSubclassOf(typeof(XmlRpcStruct)))
          {
            retObj = ParseStruct(node, valType, parseStack, mappingAction);
          }
          else
          {
            if (valType == null || valType == typeof(object))
              valType = typeof(XmlRpcStruct);
            // TODO: do we need to validate type here?
            retObj = ParseHashtable(node, valType, parseStack, mappingAction);
          }
        }
        else if (node.Name == "i4"  // integer has two representations in XML-RPC spec
          || node.Name == "int")
        {
          retObj = ParseInt(node, valType, parseStack, mappingAction);
          ParsedType = typeof(int);
          ParsedArrayType = typeof(int[]);
        }
        else if (node.Name == "i8")
        {
          retObj = ParseLong(node, valType, parseStack, mappingAction);
          ParsedType = typeof(long);
          ParsedArrayType = typeof(long[]);
        }
        else if (node.Name == "string")
        {
          retObj = ParseString(node, valType, parseStack, mappingAction);
          ParsedType = typeof(string);
          ParsedArrayType = typeof(string[]);
        }
        else if (node.Name == "boolean")
        {
          retObj = ParseBoolean(node, valType, parseStack, mappingAction);
          ParsedType = typeof(bool);
          ParsedArrayType = typeof(bool[]);
        }
        else if (node.Name == "double")
        {
          retObj = ParseDouble(node, valType, parseStack, mappingAction);
          ParsedType = typeof(double);
          ParsedArrayType = typeof(double[]);
        }
        else if (node.Name == "dateTime.iso8601")
        {
          retObj = ParseDateTime(node, valType, parseStack, mappingAction);
          ParsedType = typeof(DateTime);
          ParsedArrayType = typeof(DateTime[]);
        }
        else
          throw new XmlRpcInvalidXmlRpcException(
            "Invalid value element: <" + node.Name + ">");
      }
      return retObj;
    }

    Object ParseArray(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      // required type must be an array
      if (ValueType != null
        && !(ValueType.IsArray == true
            || ValueType == typeof(Array)
            || ValueType == typeof(object)))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains array value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      if (ValueType != null)
      {
        XmlRpcType xmlRpcType = XmlRpcServiceInfo.GetXmlRpcType(ValueType);
        if (xmlRpcType == XmlRpcType.tMultiDimArray)
        {
          parseStack.Push("array mapped to type " + ValueType.Name);
          Object ret = ParseMultiDimArray(node, ValueType, parseStack,
            mappingAction);
          return ret;
        }
        parseStack.Push("array mapped to type " + ValueType.Name);
      }
      else
        parseStack.Push("array");
      XmlNode dataNode = SelectSingleNode(node, "data");
      XmlNode[] childNodes = SelectNodes(dataNode, "value");
      int nodeCount = childNodes.Length;
      Object[] elements = new Object[nodeCount];
      // determine type of array elements
      Type elemType = null;
      if (ValueType != null
        && ValueType != typeof(Array)
        && ValueType != typeof(object))
      {
#if (!COMPACT_FRAMEWORK)
        elemType = ValueType.GetElementType();
#else
        string[] checkMultiDim = Regex.Split(ValueType.FullName, 
          "\\[\\]$");
        // determine assembly of array element type
        Assembly asmbly = ValueType.Assembly;
        string[] asmblyName = asmbly.FullName.Split(',');
        string elemTypeName = checkMultiDim[0] + ", " + asmblyName[0]; 
        elemType = Type.GetType(elemTypeName);
#endif
      }
      else
      {
        elemType = typeof(object);
      }
      bool bGotType = false;
      Type useType = null;
      int i = 0;
      foreach (XmlNode vNode in childNodes)
      {
        parseStack.Push(String.Format("element {0}", i));
        XmlNode vvNode = SelectValueNode(vNode);
        Type parsedType;
        Type parsedArrayType;
        elements[i++] = ParseValue(vvNode, elemType, parseStack, mappingAction,
                                    out parsedType, out parsedArrayType);
        if (bGotType == false)
        {
          useType = parsedArrayType;
          bGotType = true;
        }
        else
        {
          if (useType != parsedArrayType)
            useType = null;
        }
        parseStack.Pop();
      }
      Object[] args = new Object[1]; args[0] = nodeCount;
      Object retObj = null;
      if (ValueType != null
        && ValueType != typeof(Array)
        && ValueType != typeof(object))
      {
        retObj = CreateArrayInstance(ValueType, args);
      }
      else
      {
        if (useType == null)
          retObj = CreateArrayInstance(typeof(object[]), args);
        else
          retObj = CreateArrayInstance(useType, args);
      }
      for (int j = 0; j < elements.Length; j++)
      {
        ((Array)retObj).SetValue(elements[j], j);
      }
      parseStack.Pop();
      return retObj;
    }

    Object ParseMultiDimArray(XmlNode node, Type ValueType,
      ParseStack parseStack, MappingAction mappingAction)
    {
      // parse the type name to get element type and array rank
#if (!COMPACT_FRAMEWORK)
      Type elemType = ValueType.GetElementType();
      int rank = ValueType.GetArrayRank();
#else
      string[] checkMultiDim = Regex.Split(ValueType.FullName, 
        "\\[,[,]*\\]$");
      Type elemType = Type.GetType(checkMultiDim[0]);
      string commas = ValueType.FullName.Substring(checkMultiDim[0].Length+1, 
        ValueType.FullName.Length-checkMultiDim[0].Length-2);
      int rank = commas.Length+1;
#endif
      // elements will be stored sequentially as nested arrays are parsed
      ArrayList elements = new ArrayList();
      // create array to store length of each dimension - initialize to 
      // all zeroes so that when parsing we can determine if an array for 
      // that dimension has been parsed already
      int[] dimLengths = new int[rank];
      dimLengths.Initialize();
      ParseMultiDimElements(node, rank, 0, elemType, elements, dimLengths,
        parseStack, mappingAction);
      // build arguments to define array dimensions and create the array
      Object[] args = new Object[dimLengths.Length];
      for (int argi = 0; argi < dimLengths.Length; argi++)
      {
        args[argi] = dimLengths[argi];
      }
      Array ret = (Array)CreateArrayInstance(ValueType, args);
      // copy elements into new multi-dim array
      //!! make more efficient
      int length = ret.Length;
      for (int e = 0; e < length; e++)
      {
        int[] indices = new int[dimLengths.Length];
        int div = 1;
        for (int f = (indices.Length - 1); f >= 0; f--)
        {
          indices[f] = (e / div) % dimLengths[f];
          div *= dimLengths[f];
        }
        ret.SetValue(elements[e], indices);
      }
      return ret;
    }

    void ParseMultiDimElements(XmlNode node, int Rank, int CurRank,
      Type elemType, ArrayList elements, int[] dimLengths,
      ParseStack parseStack, MappingAction mappingAction)
    {
      if (node.Name != "array")
      {
        throw new XmlRpcTypeMismatchException(
          "param element does not contain array element.");
      }
      XmlNode dataNode = SelectSingleNode(node, "data");
      XmlNode[] childNodes = SelectNodes(dataNode, "value");
      int nodeCount = childNodes.Length;
      //!! check that multi dim array is not jagged
      if (dimLengths[CurRank] != 0 && nodeCount != dimLengths[CurRank])
      {
        throw new XmlRpcNonRegularArrayException(
          "Multi-dimensional array must not be jagged.");
      }
      dimLengths[CurRank] = nodeCount;  // in case first array at this rank
      if (CurRank < (Rank - 1))
      {
        foreach (XmlNode vNode in childNodes)
        {
          XmlNode arrayNode = SelectSingleNode(vNode, "array");
          ParseMultiDimElements(arrayNode, Rank, CurRank + 1, elemType,
            elements, dimLengths, parseStack, mappingAction);
        }
      }
      else
      {
        foreach (XmlNode vNode in childNodes)
        {
          XmlNode vvNode = SelectValueNode(vNode);
          elements.Add(ParseValue(vvNode, elemType, parseStack,
            mappingAction));
        }
      }
    }

    Object ParseStruct(
      XmlNode node,
      Type valueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (valueType.IsPrimitive)
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains struct value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(valueType)
          + " expected " + StackDump(parseStack));
      }
#if !FX1_0
      if (valueType.IsGenericType
        && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        valueType = valueType.GetGenericArguments()[0];
      }
#endif
      object retObj;
      try
      {
        retObj = Activator.CreateInstance(valueType);
      }
      catch (Exception)
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains struct value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(valueType)
          + " expected (as type " + valueType.Name + ") "
          + StackDump(parseStack));
      }
      // Note: mapping action on a struct is only applied locally - it 
      // does not override the global mapping action when members of the 
      // struct are parsed
      MappingAction localAction = mappingAction;
      if (valueType != null)
      {
        parseStack.Push("struct mapped to type " + valueType.Name);
        localAction = StructMappingAction(valueType, mappingAction);
      }
      else
      {
        parseStack.Push("struct");
      }
      // create map of field names and remove each name from it as 
      // processed so we can determine which fields are missing
      // TODO: replace HashTable with lighter collection
      Hashtable names = new Hashtable();
      foreach (FieldInfo fi in valueType.GetFields())
      {
        if (Attribute.IsDefined(fi, typeof(NonSerializedAttribute)))
          continue;
        names.Add(fi.Name, fi.Name);
      }
      foreach (PropertyInfo pi in valueType.GetProperties())
      {
        if (Attribute.IsDefined(pi, typeof(NonSerializedAttribute)))
          continue;
        names.Add(pi.Name, pi.Name);
      }
      XmlNode[] members = SelectNodes(node, "member");
      int fieldCount = 0;
      foreach (XmlNode member in members)
      {
        if (member.Name != "member")
          continue;
        XmlNode nameNode;
        bool dupName;
        XmlNode valueNode;
        bool dupValue;
        SelectTwoNodes(member, "name", out nameNode, out dupName, "value",
          out valueNode, out dupValue);
        if (nameNode == null || nameNode.FirstChild == null)
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains a member with missing name"
            + " " + StackDump(parseStack));
        if (dupName)
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains member with more than one name element"
            + " " + StackDump(parseStack));
        string name = nameNode.FirstChild.Value;
        if (valueNode == null)
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains struct member " + name + " with missing value "
            + " " + StackDump(parseStack));
        if (dupValue)
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains member with more than one value element"
            + " " + StackDump(parseStack));
        string structName = GetStructName(valueType, name);
        if (structName != null)
          name = structName;
        MemberInfo mi = valueType.GetField(name);
        if (mi == null)
          mi = valueType.GetProperty(name);
        if (mi == null)
          continue;
        if (names.Contains(name))
          names.Remove(name);
        else
        {
          if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
          {
            parseStack.Push(String.Format("member {0}", name));
            throw new XmlRpcNonSerializedMember("Cannot map XML-RPC struct "
              + "member onto member marked as [NonSerialized]: "
              + " " + StackDump(parseStack));
          }
          if (!IgnoreDuplicateMembers)
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains struct value with duplicate member "
              + nameNode.FirstChild.Value
              + " " + StackDump(parseStack));
          else
            continue;   // ignore duplicate member
        }
        Object valObj = null;
        switch (mi.MemberType)
        {
          case MemberTypes.Field:
            FieldInfo fi = (FieldInfo)mi;
            if (valueType == null)
              parseStack.Push(String.Format("member {0}", name));
            else
              parseStack.Push(String.Format("member {0} mapped to type {1}",
                name, fi.FieldType.Name));
            try
            {
              XmlNode vvvNode = SelectValueNode(valueNode);
              valObj = ParseValue(vvvNode, fi.FieldType,
                parseStack, mappingAction);
            }
            catch (XmlRpcInvalidXmlRpcException)
            {
              if (valueType != null && localAction == MappingAction.Error)
              {
                MappingAction memberAction = MemberMappingAction(valueType,
                  name, MappingAction.Error);
                if (memberAction == MappingAction.Error)
                  throw;
              }
            }
            finally
            {
              parseStack.Pop();
            }
            fi.SetValue(retObj, valObj);
            break;
          case MemberTypes.Property:
            PropertyInfo pi = (PropertyInfo)mi;
            if (valueType == null)
              parseStack.Push(String.Format("member {0}", name));
            else

              parseStack.Push(String.Format("member {0} mapped to type {1}",
                name, pi.PropertyType.Name));
            XmlNode vvNode = SelectValueNode(valueNode);
            valObj = ParseValue(vvNode, pi.PropertyType,
              parseStack, mappingAction);
            parseStack.Pop();

            pi.SetValue(retObj, valObj, null);
            break;
        }
        fieldCount++;
      }
      if (localAction == MappingAction.Error && names.Count > 0)
        ReportMissingMembers(valueType, names, parseStack);
      parseStack.Pop();
      return retObj;
    }

    void ReportMissingMembers(
      Type valueType,
      Hashtable names,
      ParseStack parseStack)
    {
      StringBuilder sb = new StringBuilder();
      int errorCount = 0;
      string sep = "";
      foreach (string s in names.Keys)
      {
        MappingAction memberAction = MemberMappingAction(valueType, s,
          MappingAction.Error);
        if (memberAction == MappingAction.Error)
        {
          sb.Append(sep);
          sb.Append(s);
          sep = " ";
          errorCount++;
        }
      }
      if (errorCount > 0)
      {
        string plural = "";
        if (errorCount > 1)
          plural = "s";
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains struct value with missing non-optional member"
          + plural + ": " + sb.ToString() + " " + StackDump(parseStack));
      }
    }

    string GetStructName(Type ValueType, string XmlRpcName)
    {
      // given a member name in an XML-RPC struct, check to see whether
      // a field has been associated with this XML-RPC member name, return
      // the field name if it has else return null
      if (ValueType == null)
        return null;
      foreach (FieldInfo fi in ValueType.GetFields())
      {
        Attribute attr = Attribute.GetCustomAttribute(fi,
          typeof(XmlRpcMemberAttribute));
        if (attr != null
          && attr is XmlRpcMemberAttribute
          && ((XmlRpcMemberAttribute)attr).Member == XmlRpcName)
        {
          string ret = fi.Name;
          return ret;
        }
      }
      foreach (PropertyInfo pi in ValueType.GetProperties())
      {
        Attribute attr = Attribute.GetCustomAttribute(pi,
          typeof(XmlRpcMemberAttribute));
        if (attr != null
          && attr is XmlRpcMemberAttribute
          && ((XmlRpcMemberAttribute)attr).Member == XmlRpcName)
        {
          string ret = pi.Name;
          return ret;
        }
      }
      return null;
    }

    MappingAction StructMappingAction(
      Type type,
      MappingAction currentAction)
    {
      // if struct member has mapping action attribute, override the current
      // mapping action else just return the current action
      if (type == null)
        return currentAction;
      Attribute attr = Attribute.GetCustomAttribute(type,
        typeof(XmlRpcMissingMappingAttribute));
      if (attr != null)
        return ((XmlRpcMissingMappingAttribute)attr).Action;
      return currentAction;
    }

    MappingAction MemberMappingAction(
      Type type,
      string memberName,
      MappingAction currentAction)
    {
      // if struct member has mapping action attribute, override the current
      // mapping action else just return the current action
      if (type == null)
        return currentAction;
      Attribute attr = null;
      FieldInfo fi = type.GetField(memberName);
      if (fi != null)
        attr = Attribute.GetCustomAttribute(fi,
          typeof(XmlRpcMissingMappingAttribute));
      else
      {
        PropertyInfo pi = type.GetProperty(memberName);
        attr = Attribute.GetCustomAttribute(pi,
          typeof(XmlRpcMissingMappingAttribute));
      }
      if (attr != null)
        return ((XmlRpcMissingMappingAttribute)attr).Action;
      return currentAction;
    }

    Object ParseHashtable(
      XmlNode node,
      Type valueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      XmlRpcStruct retObj = new XmlRpcStruct();
      parseStack.Push("struct mapped to XmlRpcStruct");
      try
      {
        XmlNode[] members = SelectNodes(node, "member");
        foreach (XmlNode member in members)
        {
          if (member.Name != "member")
            continue;
          XmlNode nameNode;
          bool dupName;
          XmlNode valueNode;
          bool dupValue;
          SelectTwoNodes(member, "name", out nameNode, out dupName, "value",
            out valueNode, out dupValue);
          if (nameNode == null || nameNode.FirstChild == null)
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains a member with missing name"
              + " " + StackDump(parseStack));
          if (dupName)
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains member with more than one name element"
              + " " + StackDump(parseStack));
          string rpcName = nameNode.FirstChild.Value;
          if (valueNode == null)
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains struct member " + rpcName + " with missing value "
              + " " + StackDump(parseStack));
          if (dupValue)
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains member with more than one value element"
              + " " + StackDump(parseStack));
          if (retObj.Contains(rpcName))
          {
            if (!IgnoreDuplicateMembers)
              throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
                + " contains struct value with duplicate member "
                + nameNode.FirstChild.Value
                + " " + StackDump(parseStack));
            else
              continue;
          }
          object valObj;
          parseStack.Push(String.Format("member {0}", rpcName));
          try
          {
            XmlNode vvNode = SelectValueNode(valueNode);
            valObj = ParseValue(vvNode, null, parseStack,
              mappingAction);
          }
          finally
          {
            parseStack.Pop();
          }
          retObj.Add(rpcName, valObj);
        }
      }
      finally
      {
        parseStack.Pop();
      }
      return retObj;
    }

    Object ParseInt(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(Object)
        && ValueType != typeof(System.Int32)
#if !FX1_0
 && ValueType != typeof(int?)
#endif
 && ValueType != typeof(XmlRpcInt))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType +
          " contains int value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      int retVal;
      parseStack.Push("integer");
      try
      {
        XmlNode valueNode = node.FirstChild;
        if (valueNode == null)
        {
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains invalid int element " + StackDump(parseStack));
        }
        try
        {
          String strValue = valueNode.Value;
          retVal = Int32.Parse(strValue);
        }
        catch (Exception)
        {
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains invalid int value " + StackDump(parseStack));
        }
      }
      finally
      {
        parseStack.Pop();
      }
      if (ValueType == typeof(XmlRpcInt))
        return new XmlRpcInt(retVal);
      else
        return retVal;
    }

    Object ParseLong(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(Object)
        && ValueType != typeof(System.Int64)
#if !FX1_0
 && ValueType != typeof(long?)
#endif
      )
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType +
          " contains i8 value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      long retVal;
      parseStack.Push("i8");
      try
      {
        XmlNode valueNode = node.FirstChild;
        if (valueNode == null)
        {
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains invalid i8 element " + StackDump(parseStack));
        }
        try
        {
          String strValue = valueNode.Value;
          retVal = Int64.Parse(strValue);
        }
        catch (Exception)
        {
          throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
            + " contains invalid i8 value " + StackDump(parseStack));
        }
      }
      finally
      {
        parseStack.Pop();
      }
      return retVal;
    }

    Object ParseString(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(System.String)
        && ValueType != typeof(Object))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains string value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      string ret;
      parseStack.Push("string");
      try
      {
        if (node.FirstChild == null)
          ret = "";
        else
          ret = node.FirstChild.Value;
      }
      finally
      {
        parseStack.Pop();
      }
      return ret;
    }

    Object ParseBoolean(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(Object)
        && ValueType != typeof(System.Boolean)
#if !FX1_0
 && ValueType != typeof(bool?)
#endif
 && ValueType != typeof(XmlRpcBoolean))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains boolean value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      bool retVal;
      parseStack.Push("boolean");
      try
      {
        string s = node.FirstChild.Value;
        if (s == "1")
        {
          retVal = true;
        }
        else if (s == "0")
        {
          retVal = false;
        }
        else
        {
          throw new XmlRpcInvalidXmlRpcException(
            "reponse contains invalid boolean value "
            + StackDump(parseStack));
        }
      }
      finally
      {
        parseStack.Pop();
      }
      if (ValueType == typeof(XmlRpcBoolean))
        return new XmlRpcBoolean(retVal);
      else
        return retVal;
    }

    Object ParseDouble(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(Object)
        && ValueType != typeof(System.Double)
#if !FX1_0
 && ValueType != typeof(double?)
#endif
 && ValueType != typeof(XmlRpcDouble))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains double value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      Double retVal;
      parseStack.Push("double");
      try
      {
        retVal = Double.Parse(node.FirstChild.Value,
          CultureInfo.InvariantCulture.NumberFormat);
      }
      catch (Exception)
      {
        throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
          + " contains invalid double value " + StackDump(parseStack));
      }
      finally
      {
        parseStack.Pop();
      }
      if (ValueType == typeof(XmlRpcDouble))
        return new XmlRpcDouble(retVal);
      else
        return retVal;
    }

    Object ParseDateTime(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(Object)
        && ValueType != typeof(System.DateTime)
#if !FX1_0
 && ValueType != typeof(DateTime?)
#endif
 && ValueType != typeof(XmlRpcDateTime))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains dateTime.iso8601 value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      DateTime retVal;
      parseStack.Push("dateTime");
      try
      {
        XmlNode child = node.FirstChild;
        if (child == null)
        {
          if (MapEmptyDateTimeToMinValue)
            return DateTime.MinValue;
          else
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains empty dateTime value "
              + StackDump(parseStack));
        }
        string s = child.Value;
        // Allow various iso8601 formats, e.g.
        //   XML-RPC spec yyyyMMddThh:mm:ss
        //   WordPress yyyyMMddThh:mm:ssZ
        //   TypePad yyyy-MM-ddThh:mm:ssZ
        //   other yyyy-MM-ddThh:mm:ss
        if (!DateTime8601.TryParseDateTime8601(s, out retVal))
        {
          if (MapZerosDateTimeToMinValue && s.StartsWith("0000")
            && (s == "00000000T00:00:00" || s == "0000-00-00T00:00:00Z"
            || s == "00000000T00:00:00Z" || s == "0000-00-00T00:00:00"))
            retVal = DateTime.MinValue;
          else 
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains invalid dateTime value "
              + StackDump(parseStack));
        }
      }
      finally
      {
        parseStack.Pop();
      }
      if (ValueType == typeof(XmlRpcDateTime))
        return new XmlRpcDateTime(retVal);
      else
        return retVal;
    }

    Object ParseBase64(
      XmlNode node,
      Type ValueType,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      if (ValueType != null && ValueType != typeof(byte[])
        && ValueType != typeof(Object))
      {
        throw new XmlRpcTypeMismatchException(parseStack.ParseType
          + " contains base64 value where "
          + XmlRpcServiceInfo.GetXmlRpcTypeString(ValueType)
          + " expected " + StackDump(parseStack));
      }
      byte[] ret;
      parseStack.Push("base64");
      try
      {
        if (node.FirstChild == null)
          ret = new byte[0];
        else
        {
          string s = node.FirstChild.Value;
          try
          {
            ret = Convert.FromBase64String(s);
          }
          catch (Exception)
          {
            throw new XmlRpcInvalidXmlRpcException(parseStack.ParseType
              + " contains invalid base64 value "
              + StackDump(parseStack));
          }
        }
      }
      finally
      {
        parseStack.Pop();
      }
      return ret;
    }

    XmlRpcFaultException ParseFault(
      XmlNode faultNode,
      ParseStack parseStack,
      MappingAction mappingAction)
    {
      XmlNode valueNode = SelectSingleNode(faultNode, "value");
      XmlNode structNode = SelectSingleNode(valueNode, "struct");
      if (structNode == null)
      {
        throw new XmlRpcInvalidXmlRpcException(
          "struct element missing from fault response.");
      }
      Fault fault;
      try
      {
        fault = (Fault)ParseValue(structNode, typeof(Fault), parseStack,
          mappingAction);
      }
      catch (Exception ex)
      {
        // some servers incorrectly return fault code in a string
        if (AllowStringFaultCode)
          throw;
        else
        {
          FaultStructStringCode faultStrCode;
          try
          {
            faultStrCode = (FaultStructStringCode)ParseValue(structNode,
              typeof(FaultStructStringCode), parseStack, mappingAction);
            fault.faultCode = Convert.ToInt32(faultStrCode.faultCode);
            fault.faultString = faultStrCode.faultString;
          }
          catch (Exception)
          {
            // use exception from when attempting to parse code as integer
            throw ex;
          }
        }
      }
      return new XmlRpcFaultException(fault.faultCode, fault.faultString);
    }

    struct FaultStruct
    {
      public int faultCode;
      public string faultString;
    }

    struct FaultStructStringCode
    {
      public string faultCode;
      public string faultString;
    }

    public void SerializeFaultResponse(
      Stream stm,
      XmlRpcFaultException faultEx)
    {
      FaultStruct fs;
      fs.faultCode = faultEx.FaultCode;
      fs.faultString = faultEx.FaultString;

      XmlTextWriter xtw = new XmlTextWriter(stm, m_encoding);
      ConfigureXmlFormat(xtw);
      xtw.WriteStartDocument();
      xtw.WriteStartElement("", "methodResponse", "");
      xtw.WriteStartElement("", "fault", "");
      Serialize(xtw, fs, MappingAction.Error);
      xtw.WriteEndElement();
      xtw.WriteEndElement();
      xtw.Flush();
    }

    void ConfigureXmlFormat(
      XmlTextWriter xtw)
    {
      if (m_bUseIndentation)
      {
        xtw.Formatting = Formatting.Indented;
        xtw.Indentation = m_indentation;
      }
      else
      {
        xtw.Formatting = Formatting.None;
      }
    }

    string StackDump(ParseStack parseStack)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string elem in parseStack)
      {
        sb.Insert(0, elem);
        sb.Insert(0, " : ");
      }
      sb.Insert(0, parseStack.ParseType);
      sb.Insert(0, "[");
      sb.Append("]");
      return sb.ToString();
    }

    XmlNode SelectSingleNode(XmlNode node, string name)
    {
#if (COMPACT_FRAMEWORK)
      foreach (XmlNode selnode in node.ChildNodes)
      {
        // For "*" element else return null
        if ((name == "*") && !(selnode.Name.StartsWith("#")))
          return selnode;
        if (selnode.Name == name)
          return selnode;
      }
      return null;
#else
      return node.SelectSingleNode(name);
#endif
    }

    XmlNode[] SelectNodes(XmlNode node, string name)
    {
      ArrayList list = new ArrayList();
      foreach (XmlNode selnode in node.ChildNodes)
      {
        if (selnode.Name == name)
          list.Add(selnode);
      }
      return (XmlNode[])list.ToArray(typeof(XmlNode));
    }

    XmlNode SelectValueNode(XmlNode valueNode)
    {
      // an XML-RPC value is either held as the child node of a <value> element
      // or is just the text of the value node as an implicit string value
      XmlNode vvNode = SelectSingleNode(valueNode, "*");
      if (vvNode == null)
        vvNode = valueNode.FirstChild;
      return vvNode;
    }

    void SelectTwoNodes(XmlNode node, string name1, out XmlNode node1,
      out bool dup1, string name2, out XmlNode node2, out bool dup2)
    {
      node1 = node2 = null;
      dup1 = dup2 = false;
      foreach (XmlNode selnode in node.ChildNodes)
      {
        if (selnode.Name == name1)
        {
          if (node1 == null)
            node1 = selnode;
          else
            dup1 = true;
        }
        else if (selnode.Name == name2)
        {
          if (node2 == null)
            node2 = selnode;
          else
            dup2 = true;
        }
      }
    }

    // TODO: following to return Array?
    object CreateArrayInstance(Type type, object[] args)
    {
#if (!COMPACT_FRAMEWORK)
      return Activator.CreateInstance(type, args);
#else
		Object Arr = Array.CreateInstance(type.GetElementType(), (int)args[0]);
		return Arr;
#endif
    }

    bool IsStructParamsMethod(MethodInfo mi)
    {
      if (mi == null)
        return false;
      bool ret = false;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null)
      {
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)attr;
        ret = mattr.StructParams;
      }
      return ret;
    }

    //#if (DEBUG)
    public
      //#endif
    class ParseStack : Stack
    {
      public ParseStack(string parseType)
      {
        m_parseType = parseType;
      }

      void Push(string str)
      {
        base.Push(str);
      }

      public string ParseType
      {
        get { return m_parseType; }
      }

      public string m_parseType = "";
    }
  }

}
