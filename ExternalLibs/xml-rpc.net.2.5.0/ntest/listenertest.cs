using System;
using System.IO;
using System.Net;
using CookComputing.XmlRpc;
using NUnit.Framework;

#if !FX1_0

namespace ntest
{
  [TestFixture]
  public class ListenerTest
  {
    Listener _listener = new Listener(new StateNameListnerService(),
      "http://127.0.0.1:11000/");
    Listener _listenerDerived = new Listener(new StateNameListnerDerivedService(),
      "http://127.0.0.1:11001/");

    [TestFixtureSetUp]
    public void Setup()
    {
      _listener.Start();
      _listenerDerived.Start();
    }

    [TestFixtureTearDown]
    public void TearDown()
    {
      _listener.Stop();
      _listenerDerived.Stop();
    }


    [Test]
    public void MakeCall()
    {
      IStateName proxy = XmlRpcProxyGen.Create < IStateName>();
      proxy.Url = "http://127.0.0.1:11000/";
      proxy.AllowAutoRedirect = false;
      string name = proxy.GetStateName(1);
    }

    [Test]
    public void MakeSystemListMethodsCall()
    {
      IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
      proxy.Url = "http://127.0.0.1:11000/";
      string[] ret = proxy.SystemListMethods();
      Assert.AreEqual(1, ret.Length);
      Assert.AreEqual(ret[0], "examples.getStateName");
    }

    [Test]
    public void GetCookie()
    {
      IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
      proxy.Url = "http://127.0.0.1:11000/";
      string name = proxy.GetStateName(1);
      CookieCollection cookies = proxy.ResponseCookies;
      string value = cookies["FooCookie"].Value;
      Assert.AreEqual("FooValue", value);
    }

    [Test]
    public void GetHeader()
    {
      IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
      proxy.Url = "http://127.0.0.1:11000/";
      string name = proxy.GetStateName(1);
      WebHeaderCollection headers = proxy.ResponseHeaders;
      string value = headers["BarHeader"];
      Assert.AreEqual("BarValue", value);
    }

    [Test]
    public void GetAutoDocumentation()
    {
      WebRequest req = WebRequest.Create("http://127.0.0.1:11000/");
      Stream stm = req.GetResponse().GetResponseStream();
      string doc = new StreamReader(stm).ReadToEnd();
      Assert.IsNotNull(doc);
      Assert.IsTrue(doc.StartsWith("<html>"));
    }

    [Test]
    public void GetAutoDocumentationDerived()
    {
      WebRequest req = WebRequest.Create("http://127.0.0.1:11001/");
      Stream stm = req.GetResponse().GetResponseStream();
      string doc = new StreamReader(stm).ReadToEnd();
      Assert.IsNotNull(doc);
      Assert.IsTrue(doc.StartsWith("<html>"));
    }
  }
}

#endif