using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using CookComputing.XmlRpc;
using NUnit.Framework;

namespace ntest
{
  [TestFixture]
  public class ServerProtocol
  {
    [Test]
    public void ISO8859Foo()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>Foo</methodName> 
  <params>
    <param>
      <value><string>1234</string></value>
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new ISO8859Service();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
 <params>
  <param>
   <value>1234</value>
  </param>
 </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void ISO8859Bar()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>Bar</methodName> 
  <params>
    <param>
      <value><string>1234</string></value>
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new ISO8859Service();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
 <params>
  <param>
   <value>
    <int>1234</int>
   </value>
  </param>
 </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void DefaultFoo()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>Foo</methodName> 
  <params>
    <param>
      <value><string>1234</string></value>
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new DefaultService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>1234</string>
      </value>
    </param>
  </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void DefaultBar()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>Bar</methodName> 
  <params>
    <param>
      <value><string>1234</string></value>
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new DefaultService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <i4>1234</i4>
      </value>
    </param>
  </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void NoIndentationFoo()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>Foo</methodName> 
  <params>
    <param>
      <value><string>1234</string></value>
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new NoIndentationService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?><methodResponse><params><param><value><string>1234</string></value></param></params></methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void SystemListMethods()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>system.listMethods</methodName> 
  <params>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new DefaultService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <array>
          <data>
            <value>
              <string>Bar</string>
            </value>
            <value>
              <string>Foo</string>
            </value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void SystemMethodSignature()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>system.methodSignature</methodName> 
  <params>
    <param>
      <value>
        <string>Foo</string>
      </value>    
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new DefaultService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <array>
          <data>
            <value>
              <array>
                <data>
                  <value>
                    <string>string</string>
                  </value>
                  <value>
                    <string>string</string>
                  </value>
                </data>
              </array>
            </value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }

    [Test]
    public void SystemMethodHelp()
    {
      string reqXml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>system.methodHelp</methodName> 
  <params>
    <param>
      <value>
        <string>Foo</string>
      </value>    
    </param>
  </params>
</methodCall>";
      Stream reqStm = new MemoryStream(ASCIIEncoding.Default.GetBytes(reqXml));
      XmlRpcServerProtocol svrProt = new DefaultService();
      Stream respStm = svrProt.Invoke(reqStm);
      StreamReader rdr = new StreamReader(respStm);
      string response = rdr.ReadToEnd();
      string respXml = @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>Method Foo</string>
      </value>
    </param>
  </params>
</methodResponse>";
      Assert.AreEqual(respXml, response);
    }


    [XmlRpcService(XmlEncoding="ISO-8859-1", Indentation=1, UseStringTag=false,
      UseIntTag=true)]
    public class ISO8859Service : XmlRpcServerProtocol
    {
      [XmlRpcMethod]
      public string Foo(string x)
      {
        return x;
      }

      [XmlRpcMethod]
      public int Bar(string x)
      {
        return 1234;
      }
    }

    public class DefaultService : XmlRpcServerProtocol
    {
      [XmlRpcMethod(Description="Method Foo")]
      public string Foo(string x)
      {
        return x;
      }

      [XmlRpcMethod]
      public int Bar(string x)
      {
        return 1234;
      }
    }

    [XmlRpcService(UseIndentation=false)]
    public class NoIndentationService : XmlRpcServerProtocol
    {
      [XmlRpcMethod]
      public string Foo(string x)
      {
        return x;
      }

      [XmlRpcMethod]
      public int Bar(string x)
      {
        return 1234;
      }
    }
  }
}

