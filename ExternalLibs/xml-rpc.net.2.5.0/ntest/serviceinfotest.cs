using System;
using System.IO;
using System.Reflection;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{       
  [TestFixture]
  public class ServiceInfoTest
  {
    [Test]
    public void Int32()
    {
      Type type = typeof(int);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInt32, rpcType,
        "Int32 doesn't map to XmlRpcType.tInt32");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "integer", "Int32 doesn't map to 'integer'");
    }

    [Test]
    public void Int64()
    {
      Type type = typeof(long);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInt64, rpcType,
        "Int64 doesn't map to XmlRpcType.tInt64");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "i8", "Int64 doesn't map to 'i8'");
    }
    
    [Test]
    public void XmlRpcInt()
    {
      Type type = typeof(XmlRpcInt);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInt32, rpcType, 
        "XmlRpcInt doesn't map to XmlRpcType.tInt32");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "integer", 
        "XmlRpcInt doesn't map to 'integer'");
    }

    [Test]
    public void Boolean()
    {
      Type type = typeof(bool);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tBoolean, rpcType,
        "Boolean doesn't map to XmlRpcType.tBoolean");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "boolean",
        "Boolean doesn't map to 'boolean'");
    }

    [Test]
    public void XmlRpcBoolean()
    {
      Type type = typeof(XmlRpcBoolean);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tBoolean, rpcType,
        "XmlRpcBoolean doesn't map to XmlRpcType.tBoolean");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "boolean", 
        "XmlRpcBoolean doesn't map to 'boolean'");
    }

    [Test]
    public void String()
    {
      Type type = typeof(string);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tString, rpcType, 
        "String doesn't map to XmlRpcType.tString");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "string", "String doesn't map to 'string'");
    }
    
    [Test]
    public void Double()
    {
      Type type = typeof(double);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDouble, rpcType, 
        "Double doesn't map to XmlRpcType.tDouble");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "double", "Double doesn't map to 'double'");
    }
    
    [Test]
    public void XmlRpcDouble()
    {
      Type type = typeof(XmlRpcDouble);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDouble, rpcType, 
        "XmlRpcDouble doesn't map to XmlRpcType.tDouble");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "double",
        "XmlRpcDouble doesn't map to 'double'");
    }

    [Test]
    public void DateTime()
    {
      Type type = typeof(DateTime);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDateTime, rpcType,
        "DateTime doesn't map to XmlRpcType.tDateTime");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "dateTime", 
        "DateTime doesn't map to 'dateTime'");
    }
    
    [Test]
    public void XmlRpcDateTime()
    {
      Type type = typeof(XmlRpcDateTime);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDateTime, rpcType,
        "XmlRpcDateTime doesn't map to XmlRpcType.tDateTime");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "dateTime",
        "XmlRpcDateTime doesn't map to 'dateTime'");
    }

    [Test]
    public void Base64()
    {
      Type type = typeof(byte[]);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tBase64, rpcType, 
        "Byte[] doesn't map to XmlRpcType.tBase64");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "base64", "Byte[] doesn't map to 'base64'");
    }
    
    [Test]
    public void XmlRpcStruct()
    {
      Type type = typeof(XmlRpcStruct);
        XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tHashtable, rpcType, 
        "XmlRpcStruct doesn't map to XmlRpcType.tHashtable");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "struct", 
        "XmlRpcStruct doesn't map to 'struct'");
    }
    
    [Test]
    public void Array()
    {
      Type type = typeof(Array);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tArray, rpcType, 
        "Array doesn't map to XmlRpcType.tArray");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "array", "Array doesn't map to 'array'");
    }

    [Test]
    public void IntArray()
    {
      Type type = typeof(Int32[]);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tArray, rpcType,
        "Int32[] doesn't map to XmlRpcType.tArray");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "array", "Int32[] doesn't map to 'array'");
    }
    
    [Test]
    public void MultiDimIntArray()
    {
      Type type = typeof(Int32[,]);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tMultiDimArray, rpcType,
        "Int32[] doesn't map to XmlRpcType.tMultiDimArray");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "array", "Int32['] doesn't map to 'array'");
    }


    [Test]
    public void JaggedIntArray()
    {
      Type type = typeof(Int32[][]);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tArray, rpcType,
        "Int32[] doesn't map to XmlRpcType.tArray");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "array", "Int32[] doesn't map to 'array'");
    }

    [Test]
    public void Void()
    {
      Type type = typeof(void);
        XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tVoid, rpcType, 
        "void doesn't map to XmlRpcType.tVoid");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "void", "void doesn't map to 'void'");
    }
    
    struct struct1
    {
      public int mi;
    }
    
    [Test]
    public void Struct()
    {
      Type type = typeof(struct1);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tStruct, rpcType, 
        "struct doesn't map to XmlRpcType.tStruct");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "struct", 
        "struct doesn't map to 'struct'");
    }

    enum Fooe
    {
      one, two
    }
    struct struct2
    {
      public int mi;
      public Fooe mf;
    }
    
    [Test]
    public void StructWithEnum()
    {
      Type type = typeof(struct2);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInvalid, rpcType, 
        "struct doesn't map to XmlRpcType.tInvalid");
    }


    public struct struct3
    {
      public int TestProperty { get { return 12345; } }
    }

    [Test]
    public void PropertyMember()
    {
      XmlRpcServiceInfo info = XmlRpcServiceInfo.CreateServiceInfo(
        typeof(struct3));
      
    }

    [Test]
    public void DBNull()
    {
      DBNull value = System.DBNull.Value;
      Type type = value.GetType();
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tStruct, rpcType,
        "GetXmlRpcType return DBNull as tStruct");
    }

#if !FX1_0
    [Test]
    public void NullableInt()
    {
      Type type = typeof(int?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInt32, rpcType,
        "int? doesn't map to XmlRpcType.tInt32");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "integer",
        "int? doesn't map to 'integer'");
    }

    [Test]
    public void NullableIn64()
    {
      Type type = typeof(long?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tInt64, rpcType,
        "long? doesn't map to XmlRpcType.tInt64");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "i8",
        "long? doesn't map to 'i8'");
    }

    [Test]
    public void NullableBool()
    {
      Type type = typeof(bool?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tBoolean, rpcType,
        "bool? doesn't map to XmlRpcType.tBoolean");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "boolean",
        "bool? doesn't map to 'boolean'");
    }

    [Test]
    public void NullableDouble()
    {
      Type type = typeof(double?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDouble, rpcType,
        "double? doesn't map to XmlRpcType.tDouble");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "double",
        "double? doesn't map to 'double'");
    }

    [Test]
    public void NullableDateTime()
    {
      Type type = typeof(DateTime?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tDateTime, rpcType,
        "DateTime? doesn't map to XmlRpcType.tDateTime");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "dateTime",
        "DateTime? doesn't map to 'dateTime'");
    }

    struct TestStruct
    {
      public int x;
    }

    [Test]
    public void NullableStruct()
    {
      Type type = typeof(TestStruct?);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tStruct, rpcType,
        "TestStruct? doesn't map to XmlRpcType.tStruct");
      string rpcString = XmlRpcServiceInfo.GetXmlRpcTypeString(type);
      Assert.AreEqual(rpcString, "struct",
        "TestStruct? doesn't map to 'struct'");
    }
#endif

    [Test]
    public void DerivedInterfaces()
    {
      XmlRpcServiceInfo svcinfo =  XmlRpcServiceInfo.CreateServiceInfo(
        typeof(FooBar));
      Assert.AreEqual(2, svcinfo.Methods.Length);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcDupXmlRpcMethodNames))]
    public void DupXmlRpcNames()
    {
      XmlRpcServiceInfo svcinfo = XmlRpcServiceInfo.CreateServiceInfo(
        typeof(IDupXmlRpcNames));
    }


    class Example
    {
      public Example childExample;
      public Example ChildExample { get; set; }
    }

    [Test]
    public void RecursiveClass()
    {
      Type type = typeof(Example);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tStruct, rpcType);
    }

    class ExampleWithArray
    {
      public ExampleWithArray[] childExamples;
      public ExampleWithArray[] ChildExamples { get; set; }
    }

    [Test]
    public void RecursiveArrayClass()
    {
      Type type = typeof(ExampleWithArray);
      XmlRpcType rpcType = XmlRpcServiceInfo.GetXmlRpcType(type);
      Assert.AreEqual(XmlRpcType.tStruct, rpcType);
    }
  }

interface IFoo
  {
    [XmlRpcMethod("IFoo.Foo", Description = "IFoo")]
    int Foo(int x);
  }

  interface IBar
  {
    [XmlRpcMethod("IBar.Foo", Description = "IFooBar")]
    int Foo(int x);
  }

  class FooBar : IFoo, IBar
  {
    int IFoo.Foo(int x)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    int IBar.Foo(int x)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }

  interface IDupXmlRpcNames
  {
    [XmlRpcMethod("bad.Foo")]
    int Foo1(int x);
    [XmlRpcMethod("bad.Foo")]
    int Foo2(int x);
  }

}