using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using CookComputing.XmlRpc;

public struct TestStruct
{
  public int x;
  public int y;
}

[XmlRpcUrl("http://localhost/test/")]
class Foo : XmlRpcClientProtocol
{
  [XmlRpcMethod]
  public int Send_Param(object[] toSend)
  {
    return (int)Invoke("Send_Param", toSend);
  }

  [XmlRpcMethod]
  public int SendTwoParams(int param1, int param2)
  {
    return (int)Invoke("SendTwoParams", new object[] { param1 } );
  }

  [XmlRpcMethod]
  public string Send(string str)
  {
    return (string)Invoke("Send", new object[] { str });
  }

  [XmlRpcMethod]
  public string Send(TestStruct strct)
  {
    return (string)Invoke("Send", new object[] { strct });
  }
}

[XmlRpcUrl("http://localhost:8005/statename.rem")]
class StateName : XmlRpcClientProtocol
{
  [XmlRpcMethod("examples.getStateName")]
  public string GetStateNameUsingMethodName(int stateNumber)
  {
    return (string)Invoke("GetStateNameUsingMethodName", 
      new object[] { stateNumber });
  }

  [XmlRpcMethod("examples.getStateNameFromString")]
  public string GetStateNameUsingMethodName(string stateNumber)
  {
    return (string)Invoke("GetStateNameUsingMethodName",
      new object[] { stateNumber });
  }

  [XmlRpcMethod("examples.getStateName")]
  public string GetStateNameUsingMethodInfo(int stateNumber)
  {
    return (string)Invoke(MethodBase.GetCurrentMethod(), 
      new object[] { stateNumber });
  }

  [XmlRpcMethod("examples.getStateNameFromString")]
  public string GetStateNameUsingMethodInfo(string stateNumber)
  {
    return (string)Invoke(MethodBase.GetCurrentMethod(),
      new object[] { stateNumber });
  }

  [XmlRpcMethod("examples.getStateName")]
  public IAsyncResult BeginGetStateName(int stateNumber, AsyncCallback callback,
    object asyncState)
  {
    return BeginInvoke(MethodBase.GetCurrentMethod(), 
      new object[] { stateNumber }, callback, asyncState);
  }

  [XmlRpcMethod("examples.getStateName")]
  public IAsyncResult BeginGetStateName(int stateNumber)
  {
    return BeginInvoke(MethodBase.GetCurrentMethod(),
      new object[] { stateNumber }, null, null);
  }

  public string EndGetStateName(IAsyncResult asr)
  {
    return (string)EndInvoke(asr);
  }
}

namespace ntest
{
  [TestFixture]
  public class InvokeTest
  {
    [TestFixtureSetUp]
    public void Setup()
    {
      StateNameService.Start(8005);
    }

    [TestFixtureTearDown]
    public void TearDown()
    {
      StateNameService.Stop();
    }

    [Test]
    public void MakeSynchronousCalls()
    {
      StateName proxy = new StateName();
      string ret1 = proxy.GetStateNameUsingMethodName(1);
      Assert.AreEqual("Alabama", ret1);
      string ret2 = proxy.GetStateNameUsingMethodInfo(1);
      Assert.AreEqual("Alabama", ret2);
      string ret3 = proxy.GetStateNameUsingMethodName("1");
      Assert.AreEqual("Alabama", ret3);
      string ret4 = proxy.GetStateNameUsingMethodInfo("1");
      Assert.AreEqual("Alabama", ret4);
    }

    [Test]
    public void MakeSystemListMethodsCall()
    {
      StateName proxy = new StateName();
      string[] ret = proxy.SystemListMethods();
      Assert.AreEqual(3, ret.Length);
      Assert.AreEqual(ret[0], "examples.getStateName");
      Assert.AreEqual(ret[1], "examples.getStateNameFromString");
      Assert.AreEqual(ret[2], "examples.getStateStruct");
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
      StateName proxy = (StateName)clientResult.ClientProtocol;
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
      StateName proxy = new StateName() ;
      IAsyncResult asr1 = proxy.BeginGetStateName(1);
      while (asr1.IsCompleted == false)
        System.Threading.Thread.Sleep(10);
      string ret1 = proxy.EndGetStateName(asr1);
      Assert.AreEqual("Alabama", ret1);
    }

    [Test]
    public void MakeAsynchronousCallWait()
    {
      StateName proxy = new StateName();
      IAsyncResult asr2 = proxy.BeginGetStateName(1);
      asr2.AsyncWaitHandle.WaitOne();
      string ret2 = proxy.EndGetStateName(asr2);
      Assert.AreEqual("Alabama", ret2);
    }

    [Test]
    public void MakeAsynchronousCallCallBack()
    {
      StateName proxy = new StateName();
      ManualResetEvent evt = new ManualResetEvent(false);
      CBInfo info = new CBInfo(evt);
      IAsyncResult asr3 = proxy.BeginGetStateName(1, StateNameCallback, info);
      evt.WaitOne();
      Assert.AreEqual(null, info._excep, "Async call threw exception");
      Assert.AreEqual("Alabama", info._ret);
    }

    // TODO: add sync fault exception
    // TODO: add async fault exception

    [Test]
    [ExpectedException(typeof(XmlRpcInvalidParametersException))]
    public void Massimo()
    {
      object[] parms = new object[12] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
      Foo foo = new Foo();
      foo.Send_Param(parms);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcNullParameterException))]
    public void NullArg()
    {
      Foo foo = new Foo();
      foo.Send(null);
    }
  }
}