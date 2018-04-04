using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{       
  [TestFixture]
  public class DeserializeRequestTest
  {
    [Test]
    public void StringElement()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
  
      Assert.AreEqual(request.method, "TestString", "method is TestString");
      Assert.AreEqual(request.args[0].GetType(),  typeof(string),
		    "argument is string");
      Assert.AreEqual((string)request.args[0], "test string", 
        "argument is 'test string'");
    }

    [Test]
    public void StringNoStringElement()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
 
      Assert.AreEqual(request.method, "TestString", "method is TestString");
      Assert.AreEqual(request.args[0].GetType(),  typeof(string),
        "argument is string");
      Assert.AreEqual((string)request.args[0], "test string", 
        "argument is 'test string'");
    }

    [Test]
    public void StringEmptyValue1()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
 
      Assert.AreEqual(request.method, "TestString", "method is TestString");
      Assert.AreEqual(request.args[0].GetType(),  typeof(string),
        "argument is string");
      Assert.AreEqual((string)request.args[0], "", "argument is empty string"); 
    }

    [Test]
    public void StringEmptyValue2()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
 
      Assert.AreEqual(request.method, "TestString", "method is TestString");
      Assert.AreEqual(request.args[0].GetType(),  typeof(string),
        "argument is string");
      Assert.AreEqual((string)request.args[0], "", "argument is empty string"); 
    }

    [Test]
    public void FlatXml()
    {
      string xml = @"<?xml version=""1.0"" ?><methodCall><methodName>TestString</methodName><params><param><value>test string</value></param></params></methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.AreEqual(request.method, "TestString", "method is TestString");
      Assert.AreEqual(request.args[0].GetType(),  typeof(string),
        "argument is string");
      Assert.AreEqual((string)request.args[0], "test string", 
        "argument is 'test string'");
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRequestStream()
    {
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      Stream stm = null;
      XmlRpcRequest request = serializer.DeserializeRequest(stm, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void EmptyRequestStream()
    {
      StringReader sr = new StringReader("");
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }
    
    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void InvalidXml()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall> </duffMmethodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    // test handling of methodCall element
    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void MissingMethodCall()
    {
      string xml = @"<?xml version=""1.0"" ?> <elem/>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    // test handling of methodName element
    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void MissingMethodName()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void EmptyMethodName()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName/> 
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void ZeroLengthMethodName()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName></methodName> 
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    // test handling of params element
    [Test]
    public void MissingParams()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }


    [XmlRpcMethod]
    public string MethodNoArgs()
    {
      return "";
    }

    // test handling of params element
    [Test]    
    public void NoParam1()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>MethodNoArgs</methodName> 
<params/>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, this.GetType());
    }

    // test handling of param element
    [Test]
    public void NoParam2()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>MethodNoArgs</methodName> 
  <params>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, this.GetType());
      //Console.WriteLine("");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void EmptyParam1()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
<params>
  <param/>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void EmptyParam2()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
<params>
  <param>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    // test handling integer values
    [Test]
    public void Integer()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value>
        <int>666</int>
      </value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.method == "TestInt", "method is TestInt");
      Assert.AreEqual(request.args[0].GetType(), typeof(int),
        "argument is int");
      Assert.AreEqual((int)request.args[0], 666, "argument is 666");
    }

    [Test]
    public void I4Integer()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>666</i4></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.method == "TestInt", "method is TestInt");
      Assert.AreEqual(request.args[0].GetType(), typeof(int),
        "argument is int");
      Assert.AreEqual((int)request.args[0], 666, "argument is 666");
    }

    [Test]
    public void IntegerWithPlus()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>+666</i4></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.method == "TestInt", "method is TestInt");
      Assert.AreEqual(request.args[0].GetType(), typeof(int),
        "argument is int");
      Assert.AreEqual((int)request.args[0], 666, "argument is 666");
    }

    [Test]
    public void NegativeInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>-666</i4></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.method == "TestInt", "method is TestInt");
      Assert.AreEqual(request.args[0].GetType(), typeof(int),
        "argument is int");
      Assert.AreEqual((int)request.args[0], -666, "argument is -666");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void EmptyInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4></i4></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void InvalidInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>12kiol</i4></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
      Assert.Fail("Invalid integer should cause exception");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void OverflowInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>99999999999999999999</i4></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    public void ZeroInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>0</i4></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.method == "TestInt", "method is TestInt");
      Assert.AreEqual(request.args[0].GetType(), typeof(int),
        "argument is int");
      Assert.AreEqual((int)request.args[0], 0, "argument is 0");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void NegativeOverflowInteger()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>-99999999999999999999</i4></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    // test handling i8 values
    [Test]
    public void I8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value>
        <i8>123456789012</i8>
      </value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
      Assert.AreEqual(request.args[0].GetType(), typeof(long),
        "argument is long");
      Assert.AreEqual((long)request.args[0], 123456789012, "argument is 123456789012");
    }

    [Test]
    public void I8WithPlus()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>+123456789012</i8></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
      Assert.AreEqual(request.args[0].GetType(), typeof(long),
        "argument is long");
      Assert.AreEqual((long)request.args[0], 123456789012, "argument is 123456789012");
    }

    [Test]
    public void NegativeI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>-123456789012</i8></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
      Assert.AreEqual(request.args[0].GetType(), typeof(long),
        "argument is long");
      Assert.AreEqual((long)request.args[0], -123456789012, "argument is -123456789012");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void EmptyI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8></i8></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void InvalidI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>12kiol</i8></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void OverflowI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>9999999999999999999999999999999999999999999</i8></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }

    [Test]
    public void ZeroI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>0</i8></value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
      Assert.AreEqual(request.args[0].GetType(), typeof(long),
        "argument is long");
      Assert.AreEqual((long)request.args[0], 0, "argument is 0");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void NegativeOverflowI8()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>-9999999999999999999999999999999999999999999</i8></value>
  </param>
</params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    }


    [Test]
    public void ISO_8859_1()
    {
      using(Stream stm = new FileStream("../iso-8859-1_request.xml", 
              FileMode.Open, FileAccess.Read))
      {
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        XmlRpcRequest request = serializer.DeserializeRequest(stm, null);
        Assert.AreEqual(request.args[0].GetType(), typeof(string), 
          "argument is string");
        Assert.AreEqual((string)request.args[0], "hæ hvað segirðu þá",
          "argument is 'hæ hvað segirðu þá'");
      }
    }

    struct Struct3
    {
      int _member1;
      public int member1 { get { return _member1; } set { _member1 = value; } } 

      int _member2;
      public int member2 { get { return _member2; }  } 

      int _member3;
      [XmlRpcMember("member-3")]
      public int member3 { get { return _member3; } set { _member3 = value; } } 

      int _member4;
      [XmlRpcMember("member-4")]
      public int member4 { get { return _member4; }  } 
    }

    [Test]
    public void StructProperties()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>member1</name>
            <value>
              <i4>1</i4>
            </value>
          </member>
          <member>
            <name>member2</name>
            <value>
              <i4>2</i4>
            </value>
          </member>
          <member>
            <name>member-3</name>
            <value>
              <i4>3</i4>
            </value>
          </member>
          <member>
            <name>member-4</name>
            <value>
              <i4>4</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.AreEqual(request.args[0].GetType(), typeof(XmlRpcStruct),
        "argument is XmlRpcStruct");
      XmlRpcStruct xrs = (XmlRpcStruct)request.args[0];
      Assert.IsTrue(xrs.Count == 4, "XmlRpcStruct has 4 members");
      Assert.IsTrue(xrs.ContainsKey("member1") && (int)xrs["member1"] == 1, 
        "member1");
      Assert.IsTrue(xrs.ContainsKey("member2") && (int)xrs["member2"] == 2, 
        "member2");
      Assert.IsTrue(xrs.ContainsKey("member-3") && (int)xrs["member-3"] == 3,
        "member-3");
      Assert.IsTrue(xrs.ContainsKey("member-4") && (int)xrs["member-4"] == 4,
        "member-4");
      


    }

    // test handling dateTime values


    [Test]
    public void DateTimeFormats()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestDateTime</methodName> 
<params>
  <param>
    <value><dateTime.iso8601>20020707T11:25:37Z</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>20020707T11:25:37</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>2002-07-07T11:25:37Z</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>2002-07-07T11:25:37</dateTime.iso8601></value>
  </param>
</params>
</methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.IsTrue(request.args[0] is DateTime, "argument is DateTime");
      DateTime dt0 = (DateTime)request.args[0];
      DateTime dt1 = (DateTime)request.args[1];
      DateTime dt2 = (DateTime)request.args[2];
      DateTime dt3 = (DateTime)request.args[3];

      DateTime dt = new DateTime(2002, 7, 7, 11, 25, 37);
      Assert.AreEqual(dt0, dt, "DateTime WordPress");
      Assert.AreEqual(dt0, dt, "DateTime XML-RPC spec");
      Assert.AreEqual(dt0, dt, "DateTime TypePad");
      Assert.AreEqual(dt0, dt, "DateTime other");
    }

    [Test]
    public void DateTimeLocales()
    {
      CultureInfo oldci = Thread.CurrentThread.CurrentCulture;
      try
      {
        foreach (string locale in Utils.GetLocales())
        {
          try
          {
            CultureInfo ci = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = ci;
            if (ci.LCID == 0x401    // ar-SA  (Arabic - Saudi Arabia)
              || ci.LCID == 0x465   // div-MV (Dhivehi - Maldives)
              || ci.LCID == 0x41e)  // th-TH  (Thai - Thailand)
              break;

            DateTime dt = new DateTime(1900, 01, 02, 03, 04, 05);
            while (dt < DateTime.Now)
            {
              Stream stm = new MemoryStream();
              XmlRpcRequest req = new XmlRpcRequest();
              req.args = new Object[] { dt };
              req.method = "Foo";
              XmlRpcSerializer ser = new XmlRpcSerializer();
              ser.SerializeRequest(stm, req);
              stm.Position = 0;

              XmlRpcSerializer serializer = new XmlRpcSerializer();
              XmlRpcRequest request = serializer.DeserializeRequest(stm, null);

              Assert.IsTrue(request.args[0] is DateTime,
                "argument is DateTime");
              DateTime dt0 = (DateTime)request.args[0];
              Assert.AreEqual(dt0, dt, "DateTime argument 0");
              dt += new TimeSpan(100, 1, 1, 1);
            }
          }
          catch (Exception ex)
          {
              Assert.Fail(String.Format("unexpected exception {0}: {1}",
                locale, ex.Message));
          }
        }
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = oldci;
      }
    }



    //    // test handling double values
    //
    //
    //    // test handling string values
    //



    //
    //    // test handling base64 values
    [Test]
    public void Base64Empty()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64></base64>
      </value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
 
      Assert.AreEqual(request.args[0].GetType(),  typeof(byte[]),
        "argument is byte[]");
      Assert.AreEqual(request.args[0], new byte[0], 
        "argument is zero length byte[]"); 
    }

    [Test]
    public void Base64()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64>AQIDBAUGBwg=</base64>
      </value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.AreEqual(request.args[0].GetType(), typeof(byte[]),
        "argument is byte[]");
      byte[] ret = (byte[])request.args[0];
      Assert.AreEqual(8, ret.Length, "argument is byte[8]");
      for (int i = 0; i < ret.Length; i++)
        Assert.AreEqual(i+1, ret[i], "members are 1 to 8");
    }

    [Test]
    public void Base64MultiLine()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64>AQIDBAUGBwgJ
AQIDBAUGBwg=</base64>
      </value>
    </param>
  </params>
</methodCall>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);

      Assert.AreEqual(request.args[0].GetType(), typeof(byte[]),
        "argument is byte[]");
      byte[] ret = (byte[])request.args[0];
      Assert.AreEqual(17, ret.Length, "argument is byte[17]");
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(i + 1, ret[i], "first 9 members are 1 to 9");
      for (int i = 0; i < 8; i++)
        Assert.AreEqual(i + 1, ret[i + 9], "last 8 members are 1 to 9");
    }


    //    // test array handling
    //
    //
    //
    //    // tests of handling of structs
    //    public void testMissingMemberStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //
    //    public void testAdditonalMemberStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //
    //    public void testReversedMembersStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //    
    //    public void testWrongTypeMembersStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //
    //    public void testDuplicateMembersStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //
    //    public void testNonAsciiMemberNameStruct()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }
    //
    //    // test various invalid requests
    //    public void testIncorrectParamType()
    //    {
    //      string xml = @"<?xml version=""1.0"" ?> 
    //<methodCall>
    //  <methodName>TestStruct</methodName> 
    //  <params>
    //    <param>
    //    </param>
    //  </params>
    //</methodCall>";
    //      StringReader sr = new StringReader(xml);
    //      XmlRpcSerializer serializer = new XmlRpcSerializer();
    //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
    //    }



    public class TestClass
    {
      public int _int;
      public string _string;
    }

    [XmlRpcMethod]
    public void TestClassMethod(TestClass testClass)
    {
    }

    [Test]
    public void Class()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestClassMethod</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>_int</name>
            <value>
              <i4>456</i4>
            </value>
          </member>
          <member>
            <name>_string</name>
            <value>
              <string>Test Class</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(TestClass),
        "argument is TestClass");
//      XmlRpcStruct xrs = (XmlRpcStruct)request.args[0];
//      Assert.IsTrue(xrs.Count == 4, "XmlRpcStruct has 4 members");
//      Assert.IsTrue(xrs.ContainsKey("member1") && (int)xrs["member1"] == 1, 
//        "member1");
//      Assert.IsTrue(xrs.ContainsKey("member2") && (int)xrs["member2"] == 2, 
//        "member2");
//      Assert.IsTrue(xrs.ContainsKey("member-3") && (int)xrs["member-3"] == 3,
//        "member-3");
//      Assert.IsTrue(xrs.ContainsKey("member-4") && (int)xrs["member-4"] == 4,
//        "member-4");
      
    }


    public struct simple
    {
      public int number;
      public string detail;
    }

    [XmlRpcMethod("rtx.useArrayOfStruct")]
    public string UseArrayOfStruct(simple[] myarr)
    {
      return "";
    }

    [Test]
    public void Blakemore()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.useArrayOfStruct</methodName>
<params>
<param><value><array>
<data><value>
<struct><member><name>detail</name><value><string>elephant</string></value></member><member><name>number</name><value><int>76</int></value></member></struct>
</value></data>
<data><value>
<struct><member><name>detail</name><value><string>rhino</string></value></member><member><name>number</name><value><int>33</int></value></member></struct>
</value></data>
<data><value>
<struct><member><name>detail</name><value><string>porcupine</string></value></member><member><name>number</name><value><int>106</int></value></member></struct>
</value></data>
</array></value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(simple[]),
        "argument is simple[]");
      Assert.IsTrue((request.args[0] as simple[]).Length == 1,
        "argument is simple[] of length 1");

    }

    [XmlRpcMethod("rtx.EchoString")]
    public string EchoString(string str)
    {
      return str;
    }

    [Test]
    public void SingleSpaceString()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value> </value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(string),
        "argument is string");
      Assert.AreEqual(" ", request.args[0],
        "argument is string containing single space");
    }

    [Test]
    public void TwoSpaceString()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>  </value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(string),
        "argument is string");
      Assert.AreEqual("  ", request.args[0],
        "argument is string containing two spaces");
    }

    [Test]
    public void LeadingSpace()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value> ddd</value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(string),
        "argument is string");
      Assert.AreEqual(" ddd", request.args[0]);
    }

    [Test]
    public void TwoLeadingSpace()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>  ddd</value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(string),
        "argument is string");
      Assert.AreEqual("  ddd", request.args[0]);
    }

    [Test]
    public void EmptyLines()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>
</value></param>
</params></methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());

      Assert.AreEqual(request.args[0].GetType(), typeof(string),
        "argument is string");
      Assert.AreEqual("\r\n", request.args[0]);
    }

    [XmlRpcMethod("blogger.getUsersBlogs")]
    public void GetUsersBlogs(string username, string password)
    {

    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidParametersException))]
    public void TooManyParameters()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
<methodName>blogger.getUsersBlogs</methodName>
<params>
<param>
<value>
<string>ffffffabffffffce6dffffff93ffffffac29ffffffc9fffffff826ffffffdefffff
fc9ff\
ffffe43c0b763036ffffffa0fffffff3ffffffa963377716</string>
</value>
</param>
<param>
<value>
<string>myusername</string>
</value>
</param>
<param>
<value>
<string>mypassword</string>
</value>
</param>
</params>
</methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidParametersException))]
    public void TooFewParameters()
    {
      string xml = @"<?xml version=""1.0""?>
<methodCall>
<methodName>blogger.getUsersBlogs</methodName>
<params>
<param>
<value>
<string>myusername</string>
</value>
</param>
</params>
</methodCall>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcRequest request = serializer.DeserializeRequest(sr, GetType());
    }
  }
}



