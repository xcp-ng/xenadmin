using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using CookComputing.XmlRpc;
using System.Security.Permissions;

namespace ntest
{
  [XmlRpcUrl("http://localhost/test/")]
  public interface ITest 
  {
    [XmlRpcMethod]
    string Method1(int x);
  }

  public interface ITest2 : IXmlRpcProxy
  {
    [XmlRpcMethod]
    string Method1(int x);
  }


  [TestFixture]
  public class ProxyGenTest
  {
    [TestFixtureSetUp]
    public void Setup()
    {
      StateNameService.Start(5678);
    }

    [TestFixtureTearDown]
    public void TearDown()
    {
      StateNameService.Stop();
    }

    [Test]
    public void Method1()
    {
      ITest proxy = (ITest)XmlRpcProxyGen.Create(typeof(ITest));
      XmlRpcClientProtocol cp = (XmlRpcClientProtocol)proxy;
      Assert.IsTrue(cp is ITest);
      Assert.IsTrue(cp is XmlRpcClientProtocol);
    }

#if !FX1_0
    [Test]
    public void Method1Generic()
    {
      ITest2 proxy = XmlRpcProxyGen.Create<ITest2>();
      XmlRpcClientProtocol cp = (XmlRpcClientProtocol)proxy;
      Assert.IsTrue(cp is ITest2);
      Assert.IsTrue(cp is IXmlRpcProxy);
      Assert.IsTrue(cp is XmlRpcClientProtocol);
    }
#endif

    public interface IParent : IXmlRpcProxy
    {
      [XmlRpcMethod]
      string Foo(int x);
    }

    public interface IChild : IParent
    {
      [XmlRpcMethod]
      string Bar(int x);
    }

    [Test]
    public void InheritedInterface()
    {
      // Test problem reported by Sean Rohead. This will throw an exception 
      // if method Foo in the base class Parent is not implemented
      IChild proxy = (IChild)XmlRpcProxyGen.Create(typeof(IChild));
    }

    [Test]
    public void ListMethods()
    {
      IChild proxy = (IChild)XmlRpcProxyGen.Create(typeof(IChild));
    }

    [Test]
    public void CheckProperties()
    {
      ITest2 proxy = (ITest2)XmlRpcProxyGen.Create(typeof(ITest2));
      X509CertificateCollection certs = proxy.ClientCertificates;
      string groupName = proxy.ConnectionGroupName;
#if (!FX1_0)
      bool expect100 = proxy.Expect100Continue;
#endif
      WebHeaderCollection header = proxy.Headers;
      int indentation = proxy.Indentation;
      bool keepAlive = proxy.KeepAlive;
      XmlRpcNonStandard nonStandard = proxy.NonStandard;
      bool preauth = proxy.PreAuthenticate;
      Version version = proxy.ProtocolVersion;
      IWebProxy webProxy = proxy.Proxy;
      CookieContainer container = proxy.CookieContainer;
      int timeout = proxy.Timeout;
      string url = proxy.Url;
      bool useIndent = proxy.UseIndentation;
      System.Text.Encoding encoding = proxy.XmlEncoding;
      string method = proxy.XmlRpcMethod;
      bool useIntTag = proxy.UseIntTag;

      // introspection methods
      try { proxy.SystemListMethods(); } catch (XmlRpcMissingUrl) { }
      try { proxy.SystemMethodSignature("Foo"); } catch (XmlRpcMissingUrl) { }
      try { proxy.SystemMethodHelp("Foo"); } catch (XmlRpcMissingUrl) { }
    }

    public interface IOverrides
    {
      [XmlRpcMethod("account.info")]
      string acct_info(int SITECODE, String username);
      [XmlRpcMethod("account.info")]
      string acct_info(int SITECODE, int account);
    }

    public interface IOverridesChild : IOverrides
    {
      [XmlRpcMethod("account.info")]
      new string acct_info(int SITECODE, int account);
    }

    [Test]
    public void Overrides()
    {
      IOverrides proxy = (IOverrides)XmlRpcProxyGen.Create(typeof(IOverrides));
    }

    [Test]
    public void OverridesChild()
    {
      IOverridesChild proxy = (IOverridesChild)XmlRpcProxyGen.Create(typeof(IOverridesChild));
    }

    [Test]
    public void MakeSynchronousCalls()
    {
        IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
        string ret1 = proxy.GetStateName(1);
        Assert.AreEqual("Alabama", ret1);
        string ret2 = proxy.GetStateName("1");
        Assert.AreEqual("Alabama", ret2);
    }

    [Test]
    public void SynchronousFaultException()
    {
      IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      try
      {
        string ret1 = proxy.GetStateName(100);
        Assert.Fail("exception not thrown on sync call");
      }
      catch (XmlRpcFaultException fex)
      {
        Assert.AreEqual(1, fex.FaultCode);
        Assert.AreEqual("Invalid state number", fex.FaultString);
      }
    }

    class CBInfo
    {
      public ManualResetEvent _evt;
      public Exception _excep;
      public string _ret;
      public CBInfo(ManualResetEvent evt)
      {
        _evt = evt;
      }
    }

    void StateNameCallback(IAsyncResult asr)
    {
      XmlRpcAsyncResult clientResult = (XmlRpcAsyncResult)asr;
      IStateName proxy = (IStateName)clientResult.ClientProtocol;
      CBInfo info = (CBInfo)asr.AsyncState;
      try
      {
        info._ret = proxy.EndGetStateName(asr);
      }
      catch (Exception ex)
      {
        info._excep = ex;
      }
      info._evt.Set();
    }

    [Test]
    public void MakeAsynchronousCallIsCompleted()
    {
        IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
        IAsyncResult asr1 = proxy.BeginGetStateName(1);
        while (asr1.IsCompleted == false)
          System.Threading.Thread.Sleep(10);
        string ret1 = proxy.EndGetStateName(asr1);
        Assert.AreEqual("Alabama", ret1);
    }

    [Test]
    public void MakeAsynchronousCallWait()
    {
      IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      IAsyncResult asr2 = proxy.BeginGetStateName(1);
      asr2.AsyncWaitHandle.WaitOne();
      string ret2 = proxy.EndGetStateName(asr2);
      Assert.AreEqual("Alabama", ret2);
    }

    [Test]
    public void MakeAsynchronousCallCallback()
    {
      IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      ManualResetEvent evt = new ManualResetEvent(false);
      CBInfo info = new CBInfo(evt);
      IAsyncResult asr3 = proxy.BeginGetStateName(1, StateNameCallback, info);
      evt.WaitOne();
      Assert.AreEqual(null, info._excep, "Async call threw exception");
      Assert.AreEqual("Alabama", info._ret);
    }

    void StateNameCallbackNoState(IAsyncResult asr)
    {
      XmlRpcAsyncResult clientResult = (XmlRpcAsyncResult)asr;
      IStateName proxy = (IStateName)clientResult.ClientProtocol;
      try
      {
        _ret = proxy.EndGetStateName(asr);
      }
      catch (Exception ex)
      {
        _excep = ex;
      }
      _evt.Set();
    }

    ManualResetEvent _evt;
    Exception _excep;
    string _ret;
    [Test]
    public void MakeAsynchronousCallCallbackNoState()
    {
      IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      _evt = new ManualResetEvent(false);
      IAsyncResult asr3 = proxy.BeginGetStateName(1, StateNameCallbackNoState);
      _evt.WaitOne();
      Assert.AreEqual(null, _excep, "Async call threw exception");
      Assert.AreEqual("Alabama", _ret);
    }

    [Test]
    public void AsynchronousFaultException()
    {
      IStateName proxy = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      IAsyncResult asr = proxy.BeginGetStateName(100);
      asr.AsyncWaitHandle.WaitOne();
      try
      {
        string ret = proxy.EndGetStateName(asr);
        Assert.Fail("exception not thrown on async call");
      }
      catch (XmlRpcFaultException fex)
      {
        Assert.AreEqual(1, fex.FaultCode);
        Assert.AreEqual("Invalid state number", fex.FaultString);
      }
    }

    [XmlRpcUrl("http://localhost:5678/statename.rem")]
    public interface IStateName2 : IXmlRpcProxy
    {
      [XmlRpcMethod("examples.getStateStruct", StructParams = true)]
      string GetStateNames(int state1, int state2, int state3);
    }

    [Test]
    public void MakeStructParamsCall()
    {
      IStateName2 proxy = (IStateName2)XmlRpcProxyGen.Create(typeof(IStateName2));
      string ret = proxy.GetStateNames(1, 2, 3);
      Assert.AreEqual("Alabama Alaska Arizona", ret);
    }

    [Test]
    public void FileIOPermission()
    {
      FileIOPermission f = new FileIOPermission(PermissionState.Unrestricted);
      f.Deny();
      try
      {
        IStateName2 proxy = (IStateName2)XmlRpcProxyGen.Create(typeof(IStateName2));
      }
      finally
      {
        CodeAccessPermission.RevertDeny();
      }
    }
  }
}


