using System;
using System.IO;
using System.Reflection;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{       

  [TestFixture]
  public class RemotingServerTest
  {
    [Test]
    public void Method1()
    {
      ITest proxy = (ITest)XmlRpcProxyGen.Create(typeof(ITest));
      XmlRpcClientProtocol cp = (XmlRpcClientProtocol)proxy;
    }
  }
}