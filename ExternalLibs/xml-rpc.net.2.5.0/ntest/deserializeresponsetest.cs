using System;
using System.IO;
using System.Reflection;
using System.Text;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{       
  [TestFixture]
  public class DeserializeResponseTest 
  {

    // test return integer
    [Test]
    public void I4NullType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    public void I4WithType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(int));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    public void IntegerNullType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    public void IntegerWithType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(int));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    public void IntegerIncorrectType()
    {
      try
      {
        string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";
        StringReader sr = new StringReader(xml);
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(string));
        Assert.Fail("Should throw XmlRpcTypeMismatchException");
      }
      catch (XmlRpcTypeMismatchException)
      {
      }
    }
  
    // test return double

    // test return boolean

    // test return string
    [Test]
    public void StringNullType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is string, "retval is string");
      Assert.AreEqual((string)o, "test string", "retval is 'test string'");
    }

    [Test]
    public void String2NullType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is string, "retval is string");
      Assert.AreEqual((string)o, "test string", "retval is 'test string'");
    }

    [Test]
    public void String1WithType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(string));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is string, "retval is string");
      Assert.AreEqual((string)o, "test string", "retval is 'test string'");
    }

    [Test]
    public void String2WithType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(string));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is string, "retval is string");
      Assert.AreEqual((string)o, "test string", "retval is 'test string'");
    }

    [Test]
    public void String1IncorrectType()
    {
      try
      {
        string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";
        StringReader sr = new StringReader(xml);
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(int));
        Assert.Fail("Should throw XmlRpcTypeMismatchException");
      }
      catch(XmlRpcTypeMismatchException)
      {
      }
    }

    [Test]
    public void StringEmptyValue()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(string));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is string, "retval is string");
      Assert.AreEqual((string)o, "", "retval is empty string");
    }



    // test return dateTime

    [Test]
    public void MinDateTime1NotStrict()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(DateTime));
      Object o = response.retVal;
      Assert.IsTrue(o is DateTime, "retval is string");
      Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
    }

    [Test]
    public void MinDateTime2NotStrict()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00Z</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(DateTime));

      Object o = response.retVal;
      Assert.IsTrue(o is DateTime, "retval is string");
      Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
    }

    [Test]
    public void MinDateTime3NotStrict()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>0000-00-00T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(DateTime));

      Object o = response.retVal;
      Assert.IsTrue(o is DateTime, "retval is string");
      Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
    }

    [Test]
    public void MinDateTime4NotStrict()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>0000-00-00T00:00:00Z</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(DateTime));

      Object o = response.retVal;
      Assert.IsTrue(o is DateTime, "retval is string");
      Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
    }

    [Test]
    public void MinDateTimeStrict()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
      try
      {
        XmlRpcResponse response = serializer.DeserializeResponse(sr, 
          typeof(DateTime));
        Assert.Fail("dateTime 00000000T00:00:00 invalid when strict");
      }
      catch (XmlRpcInvalidXmlRpcException)
      {
      }
    }

    // test return base64


    // test return array


    // test return struct


    [Test]
    public void ReturnStructAsObject()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response 
        = serializer.DeserializeResponse(sr, typeof(object));
      
      Object o = response.retVal;
      string ret = (string)((XmlRpcStruct)o)["key3"];
    }


    [Test]
    public void ReturnStructAsXmlRpcStruct()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response 
        = serializer.DeserializeResponse(sr, typeof(XmlRpcStruct));
      
      Object o = response.retVal;
      string ret = (string)((XmlRpcStruct)o)["key3"];
    }


   
    [Test]
    public void ArrayInStruct()
    {
      // reproduce problem reported by Alexander Agustsson
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <array>
                <data>
                  <value>New Milk</value>
                  <value>Old Milk</value>
                </data>
              </array>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);
      
      Object o = response.retVal;
      Assert.IsTrue(o is XmlRpcStruct, "retval is XmlRpcStruct");
      XmlRpcStruct xrs = (XmlRpcStruct)o;
      Assert.IsTrue(xrs.Count == 1, "retval contains one entry");
      object elem = xrs["key3"];
      Assert.IsTrue(elem != null, "element has correct key");
      Assert.IsTrue(elem is Array, "element is an array");
      object[] array = (object[])elem;
      Assert.IsTrue(array.Length == 2, "array has 2 members");
      Assert.IsTrue(array[0] is string && (string)array[0] == "New Milk"
        && array[1] is string && (string)array[1] == "Old Milk",
        "values of array members");
    }


    [Test]
    public void StringAndStructInArray()
    {
      // reproduce problem reported by Eric Brittain
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <array>
          <data>
            <value>
              <string>test string</string>
            </value>
            <value>
              <struct>
                <member>
                  <name>fred</name>
                  <value><string>test string 2</string></value>
                </member>
              </struct>
            </value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodResponse>";      

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);
      Object o = response.retVal;

    }



    public struct InternalStruct
    {
      public string firstName;
      public string lastName;
    }

    public struct MyStruct
    {
      public string version;
      public InternalStruct record;
    }

    [Test]
    public void ReturnNestedStruct()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>version</name>
            <value><string>1.6</string></value>
          </member>
          <member>
            <name>record</name>
            <value>
              <struct>
                <member>
                  <name>firstName</name>
                  <value>Joe</value></member>
                <member>
                  <name>lastName</name>
                  <value>Test</value>
                </member>
              </struct>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response 
        = serializer.DeserializeResponse(sr, typeof(MyStruct));
      
      Object o = response.retVal;
      Assert.IsTrue(o is MyStruct, "retval is MyStruct");
      MyStruct mystr = (MyStruct)o;
      Assert.AreEqual(mystr.version, "1.6", "version is 1.6");
      Assert.IsTrue(mystr.record.firstName == "Joe", "firstname is Joe");
      Assert.IsTrue(mystr.record.lastName == "Test", "lastname is Test");
    }

    [Test]
    public void JoseProblem()
    {

      string xml = @"<?xml version='1.0'?> 
<methodResponse> 
<params> 
<param> 
<value><int>12</int></value> 
</param> 
</params> 

</methodResponse>"; 
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response 
        = serializer.DeserializeResponse(sr, typeof(int));
      
      Object o = response.retVal;
      Assert.IsTrue(o is int, "retval is int");
      int myint = (int)o;
      Assert.AreEqual(myint, 12, "int is 12");
    }

    struct BillStruct
    {
      public int x;
      public string s;
    }

    [Test]
    public void MissingStructMember()
    {
      string xml = @"<?xml version='1.0'?> 
<methodResponse> 
  <params> 
    <param> 
      <value>
        <struct>
          <member>
            <name>x</name>
            <value>
              <i4>123</i4>
            </value>
          </member>
        </struct>
      </value> 
    </param> 
  </params> 
</methodResponse>"; 
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try
      {
        XmlRpcResponse response 
          = serializer.DeserializeResponse(sr, typeof(BillStruct));
        Assert.Fail("Should detect missing struct member");
      } 
      catch(AssertionException)
      {
        throw;
      }
      catch(Exception)
      {
      }    
    }

    [Test]
    public void BillKeenanProblem()
    {
      string xml = @"<?xml version='1.0'?> 
<methodResponse> 
  <params> 
    <param> 
      <value>
        <struct>
          <member>
            <name>x</name>
            <value>
              <i4>123</i4>
            </value>
          </member>
          <member>
            <name>s</name>
            <value>
              <string>ABD~~DEF</string>
            </value>
          </member>
          <member>
            <name>unexpected</name>
            <value>
              <string>this is unexpected</string>
            </value>
          </member>
        </struct>
      </value> 
    </param> 
  </params> 
</methodResponse>"; 
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response 
        = serializer.DeserializeResponse(sr, typeof(BillStruct));
      
      Object o = response.retVal;
      Assert.IsTrue(o is BillStruct, "retval is BillStruct");
      BillStruct bs = (BillStruct)o;
      Assert.IsTrue(bs.x == 123 && bs.s == "ABD~~DEF", "struct members");
    }

    [Test]
    public void AdvogatoProblem()
    {
      string xml = @"<?xml version='1.0'?> 
<methodResponse>
<params>
<param>
<array>
<data>
<value>
<dateTime.iso8601>20020707T11:25:37</dateTime.iso8601>
</value>
<value>
<dateTime.iso8601>20020707T11:37:12</dateTime.iso8601>
</value>
</data>
</array>
</param>
</params>
</methodResponse>"; 
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try 
      {
        XmlRpcResponse response 
          = serializer.DeserializeResponse(sr, null);
        Object o = response.retVal;
        Assert.Fail("should have thrown XmlRpcInvalidXmlRpcException");
      }    
      catch(XmlRpcInvalidXmlRpcException)
      {
      }
    }

    [Test]
    public void VoidReturnType()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value></value>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(void));
      Assert.IsTrue(response.retVal == null, "retval is null");
    }

    [Test]
    public void EmptyValueReturn()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(string));
      string s = (string)response.retVal;
      Assert.IsTrue(s == "", "retval is empty string");
    }


    public struct XmlRpcClassifyRequest
    {
      public int q_id;  
      public string docid;
      public string query;
      public string [] cattypes;
      public int topscores;
      public int timeout;
    }

    public struct user_info
    {
      public string username;
      public string password;
      public string hostname;
      public string ip;
    }

    public struct XmlRpcClassifyResult
    {
      public XmlRpcCatData [] categories;
      public string error_msg;
      public int error_code;
      public double exec_time;
      public int q_id;
      public string cattype;
    }

    public struct XmlRpcCatData
    {
      public int rank;
      public int cat_id;
      public string cat_title;
      public double composite_score;
      public string meta_info;
      public double [] component_scores;
    }

    [Test]
    public void ISO_8869_1()
    {
      using(Stream stm = new FileStream("../iso-8859-1_response.xml",
               FileMode.Open, FileAccess.Read))
      {
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        XmlRpcResponse response 
          = serializer.DeserializeResponse(stm, typeof(String));
        String ret = (String)response.retVal;
        int nnn  = ret.Length;
        Assert.IsTrue(ret == "hæ hvað segirðu þá", 
          "retVal is 'hæ hvað segirðu þá'");
      }
    }

    [Test]
    public void FaultResponse()
    {
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>faultCode</name>
          <value><int>4</int></value>
        </member>
        <member>
          <name>faultString</name>
          <value><string>Too many parameters.</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try
      {
        XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(void));
      }
      catch (XmlRpcFaultException fex)
      {
        Assert.AreEqual(fex.FaultCode, 4);
        Assert.AreEqual(fex.FaultString, "Too many parameters.");
      }
    }

    [Test]
    public void FaultStringCode()
    {
      // Alex Hung reported that some servers, e.g. WordPress, return fault code
      // as a string
      string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>faultCode</name>
          <value><string>4</string></value>
        </member>
        <member>
          <name>faultString</name>
          <value><string>Too many parameters.</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try
      {
        XmlRpcResponse response = serializer.DeserializeResponse(sr, typeof(void));
      }
      catch (XmlRpcFaultException fex)
      {
        Assert.AreEqual(fex.FaultCode, 4);
        Assert.AreEqual(fex.FaultString, "Too many parameters.");
      }
    }
 
    [Test]
    public void Yolanda()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><methodResponse><params><param><value><array><data><value>addressbook</value><value>system</value></data></array></value></param></params></methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, null);

      Object o = response.retVal;
    }
 
    [Test]
    public void Gabe()
    {
      string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><methodResponse><params><param><value><struct><member><name>response</name><value><struct><member><name>result</name><value><array><data><value><struct><member><name>state</name><value><string>CO</string></value></member><member><name>latitude</name><value><double>39.74147878</double></value></member><member><name>add1</name><value><string>110 16th St.</string></value></member><member><name>add2</name><value><string /></value></member><member><name>image_map</name><value><array><data><value><string>rect</string></value><value><int>290</int></value><value><int>190</int></value><value><int>309</int></value><value><int>209</int></value></data></array></value></member><member><name>city</name><value><string>Denver</string></value></member><member><name>fax</name><value><string>303-623-1111</string></value></member><member><name>name</name><value><boolean>0"
+ "</boolean></value></member><member><name>longitude</name><value><double>-104.9874159</double></value></member><member><name>georesult</name><value><string>10 W2GIADDRESS</string></value></member><member><name>zip</name><value><string>80202</string></value></member><member><name>hours</name><value><string>Mon-Sun 10am-6pm</string></value></member><member><name>dealerid</name><value><string>545</string></value></member><member><name>phone</name><value><string>303-623-5050</string></value></member></struct></value></data></array></value></member><member><name>map_id</name><value><string>a5955239d080dfbb7002fd063aa7b47e0d</string></value></member><member><name>map</name><value><struct><member><name>zoom_level</name><value><int>3</int></value></member><member><name>image_type</name><value><string>image/png</string></value></member><member><name>miles</name><value><double>1.75181004463519</double></value></member><member><name>kilometers</name><value><double>2.81926498447338"
+ "</double></value></member><member><name>scalebar</name><value><int>1</int></value></member><member><name>content</name><value><string>http://mapserv.where2getit.net/maptools/mapserv.cgi/a5955239d080dfbb7002fd063aa7b47e0d.png</string></value></member><member><name>scale</name><value><int>26000</int></value></member><member><name>map_style</name><value><string>default</string></value></member><member><name>size</name><value><array><data><value><int>600</int></value><value><int>400</int></value></data></array></value></member><member><name>content_type</name><value><string>text/uri-list</string></value></member><member><name>buffer</name><value><double>0.01</double></value></member><member><name>center</name><value><struct><member><name>georesult</name><value><string>AUTOBBOX</string></value></member><member><name>latitude</name><value><double>39.74147878</double></value></member><member><name>longitude</name><value><double>-104.9874159</double></value></member></struct></value></member></struct></value></member><member><name>result_count</name><value><int>1</int></value></member><member><name>image_map</name><value><boolean>1</boolean></value></member><member><name>result_total_count</name><value><int>1</int></value></member></struct></value></member><member><name>times</name><value><struct><member><name>csys</name><value><int>0</int></value></member><member><name>cusr</name><value><int>0</int></value></member><member><name>sys</name><value><int>0</int></value></member><member><name>usr</name><value><double>0.0200000000000005"
        + "</double></value></member><member><name>wallclock</name><value><double>2.547471</double></value></member></struct></value></member><member><name>request</name><value><struct><member><name>state</name><value><string>CO</string></value></member><member><name>%sort</name><value><array><data /></array></value></member><member><name>%id</name><value><string>4669b341d87be7f450b4bf0dc4cd0a1e</string></value></member><member><name>city</name><value><string>denver</string></value></member><member><name>%limit</name><value><int>10</int></value></member><member><name>%offset</name><value><int>0</int></value></member></struct></value></member></struct></value></param></params></methodResponse>";
      StringReader sr = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr, 
        typeof(XmlRpcStruct));

      XmlRpcStruct response_struct = (XmlRpcStruct)response.retVal;
      XmlRpcStruct _response = (XmlRpcStruct)response_struct["response"];
      Array results = (Array)_response["result"];
      Assert.AreEqual(results.Length, 1);
    }

    struct DupMem
    {
      public string foo;
    }

    [Test]
    public void StructDuplicateMember()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>foo</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
          <member>
            <name>foo</name>
            <value>
              <string>duplicate this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try
      {
        XmlRpcResponse response1 = serializer.DeserializeResponse(sr1, typeof(DupMem));
        Assert.Fail("Ignored duplicate member");
      }
      catch (XmlRpcInvalidXmlRpcException)
      {
      }
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      StringReader sr2 = new StringReader(xml);
      XmlRpcResponse response2 = serializer.DeserializeResponse(sr2, typeof(DupMem));
      DupMem dupMem = (DupMem)response2.retVal;
      Assert.AreEqual(dupMem.foo, "this is a test");
    }

    [Test]
    public void XmlRpcStructDuplicateMember()
    {
      string xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>foo</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
          <member>
            <name>foo</name>
            <value>
              <string>duplicate this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      try
      {
        XmlRpcResponse response1 = serializer.DeserializeResponse(sr1, typeof(XmlRpcStruct));
        Assert.Fail("Ignored duplicate member");
      }
      catch (XmlRpcInvalidXmlRpcException)
      {

      }
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      StringReader sr2 = new StringReader(xml);
      XmlRpcResponse response2 = serializer.DeserializeResponse(sr2, typeof(XmlRpcStruct));
      XmlRpcStruct dupMem = (XmlRpcStruct)response2.retVal;
      Assert.IsTrue((string)dupMem["foo"] == "this is a test");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void InvalidHTTPContentLeadingWhiteSpace()
    {
      string xml = @"
 
   
<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0; 
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));
    }

    [Test]
    public void AllowInvalidHTTPContentLeadingWhiteSpace()
    {
      string xml = @"
 
   
<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";
      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0; 
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    public void AllowInvalidHTTPContentTrailingWhiteSpace()
    {
      string xml = @"


<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0;

      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));

      Object o = response.retVal;
      Assert.IsTrue(o != null, "retval not null");
      Assert.IsTrue(o is int, "retval is int");
      Assert.AreEqual((int)o, 12345, "retval is 12345");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void InvalidXML()
    {
      string xml = @"response>";
      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0; 
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void InvalidXMLWithAllowInvalidHTTPContent()
    {
      string xml = @"response>";
      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0; 
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void OneByteContentAllowInvalidHTTPContent()
    {
      string xml = @"<";
      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0;
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));
    }

    [Test]
    [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
    public void ZeroByteContentAllowInvalidHTTPContent()
    {
      string xml = @"";
      Stream stm = new MemoryStream();
      StreamWriter wrtr = new StreamWriter(stm, Encoding.ASCII);
      wrtr.Write(xml);
      wrtr.Flush();
      stm.Position = 0;
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
      XmlRpcResponse response = serializer.DeserializeResponse(stm, typeof(int));
    }


    [Test]
    public void Donhrobjartz_XmlRpcStructNonMemberStructChild()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<foo>
This should be ignored.
</foo>
<member>
<name>period</name>
<value><string>1w</string></value>
</member>
<bar>
This should be ignored.
</bar>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(XmlRpcStruct));
      XmlRpcStruct ret = (XmlRpcStruct)response.retVal;
      Assert.AreEqual(ret.Count,1);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void Donhrobjartz_XmlRpcStructMemberDupName()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<name>price</name>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1, 
        typeof(XmlRpcStruct));
      XmlRpcStruct ret = (XmlRpcStruct)response.retVal;
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void Donhrobjartz_XmlRpcStructMemberDupValue()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<value><string>284</string></value>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1, 
        typeof(XmlRpcStruct));
      XmlRpcStruct ret = (XmlRpcStruct)response.retVal;
    }

    struct Donhrobjartz
    {
      public string period;
    }

    [Test]
    public void Donhrobjartz_StructNonMemberStructChild()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<foo>
This should be ignored.
</foo>
<member>
<name>period</name>
<value><string>1w</string></value>
</member>
<bar>
This should be ignored.
</bar>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(Donhrobjartz));
      Donhrobjartz ret = (Donhrobjartz)response.retVal;
      Assert.AreEqual(ret.period, "1w");
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void Donhrobjartz_StructMemberDupName()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<name>price</name>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(Donhrobjartz));
      Donhrobjartz ret = (Donhrobjartz)response.retVal;
    }

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
    public void Donhrobjartz_StructMemberDupValue()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<value><string>284</string></value>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(Donhrobjartz));
      Donhrobjartz ret = (Donhrobjartz)response.retVal;
    }

    public struct Category
    {
      public string Title;
      public int id;
    }

    [Test]
    [ExpectedException(typeof(XmlRpcTypeMismatchException))]
    public void StructContainingArrayError()
    {
      string xml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
 <methodResponse>
 <params>
 <param>
 <value>
 <struct>
 <member>
 <name>Categories</name>
 <value>
 <array>
 <data>
 <value>
 <struct>
 <member>
 <name>id</name>
 <value>
 <int>0</int>
 </value>
 </member>
 <member>
 <name>Title</name>
 <value>
 <string>Other</string>
 </value>
 </member>
 </struct>
 </value>
 <value>
 <struct>
 <member>
 <name>id</name>
 <value>
 <int>41</int>
 </value>
 </member>
 <member>
 <name>Title</name>
 <value>
 <string>Airplanes</string>
 </value>
 </member>
 </struct>
 </value>
 </data>
 </array>
 </value>
 </member>
 </struct>
 </value>
 </param>
 </params>
 </methodResponse>";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      serializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(Category[]));
    }

    [Test]
    public void EmptyParamsVoidReturn()
    {
      string xml = @"<?xml version=""1.0""?>
<methodResponse>
  <params/>
</methodResponse>
";
      StringReader sr1 = new StringReader(xml);
      XmlRpcSerializer serializer = new XmlRpcSerializer();
      XmlRpcResponse response = serializer.DeserializeResponse(sr1,
        typeof(void));
      Assert.IsNull(response.retVal);
    }
  }
}