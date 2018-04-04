using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Threading;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{
  [TestFixture]
  public class SerializeResponseTest
  {
    [Test]
    public void PaoloLiveraniProblem()
    {
      try
      {
        XmlRpcResponse resp = new XmlRpcResponse(new DataSet());
        Stream responseStream = new MemoryStream();
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        serializer.SerializeResponse(responseStream, resp);
      }
      catch(XmlRpcInvalidReturnType ex)
      {
        string s = ex.Message;
      }
    }

    class FooClass
    {
      public void Foo() 
      {
        Console.WriteLine("Foo called");
      }
    }

//    [Test]
//    public void VoidReturn()
//    {
//      MethodInfo mi = typeof(FooClass).GetMethod("Foo");
//      FooClass fooInst = new FooClass();
//      object ret = mi.Invoke(fooInst, new object[0]);
//      XmlRpcResponse xmlRpcResp = new XmlRpcResponse(ret);
//      Stream responseStream = new MemoryStream();
//      XmlRpcSerializer serializer = new XmlRpcSerializer();
//      serializer.SerializeResponse(responseStream, xmlRpcResp);
//      responseStream.Seek(0, SeekOrigin.Begin);
//      TextReader trdr = new StreamReader(responseStream);
//      String s = trdr.ReadLine();
//      while (s != null)
//      {
//        Console.WriteLine(s);
//        s = trdr.ReadLine();
//      }
//      responseStream.Seek(0, SeekOrigin.Begin);
//      xmlRpcResp = serializer.DeserializeResponse(responseStream, 
//        typeof(string));
//
//    }
  }
}