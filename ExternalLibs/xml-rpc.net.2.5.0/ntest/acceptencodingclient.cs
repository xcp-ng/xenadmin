using System;
using System.IO;
using System.Net;
using System.Threading;
using CookComputing.XmlRpc;
using NUnit.Framework;

#if !FX1_0

namespace ntest
{
  [TestFixture]
  public class AcceptEncodingClient
  {
    bool _running;
    HttpListener _lstner;
    string encoding;

    [TestFixtureSetUp]
    public void Setup()
    {
      _lstner = new HttpListener();
      _lstner.Prefixes.Add("http://127.0.0.1:11002/");
      Thread thrd = new Thread(new ThreadStart(Run));
      _running = true;
      _lstner.Start();
      thrd.Start();
    }

    public void Run()
    {
      try
      {
        while (_running)
        {
          string xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>Alabama</value>
    </param>
  </params>
</methodResponse>";
          HttpListenerContext context = _lstner.GetContext();
          switch (encoding)
          {
            case "gzip":
              context.Response.Headers.Add("Content-Encoding", "gzip");
              break;
            case "deflate":
              context.Response.Headers.Add("Content-Encoding", "deflate");
              break;
            default:
              break;
          }
          context.Response.ContentEncoding = System.Text.Encoding.UTF32;
          Stream respStm = context.Response.OutputStream;
          Stream compStm;
          switch (encoding)
          {
            case "gzip":
              compStm = new System.IO.Compression.GZipStream(respStm,
                System.IO.Compression.CompressionMode.Compress);
              break;
            case "deflate":
              compStm = new System.IO.Compression.DeflateStream(respStm,
                System.IO.Compression.CompressionMode.Compress);
              break;
            default:
              compStm = null;
              break;
          }
          StreamWriter wrtr = new StreamWriter(compStm);
          wrtr.Write(xml);
          wrtr.Close();
        }
      }
      catch (HttpListenerException)
      {
      }
    }

    [TestFixtureTearDown]
    public void TearDown()
    {
      _running = false;
      _lstner.Stop();
    }

    [Test]
    public void GZipCall()
    {
      encoding = "gzip";
      IStateName proxy = XmlRpcProxyGen.Create < IStateName>();
      proxy.Url = "http://127.0.0.1:11002/";
      proxy.EnableCompression = true;
      string name = proxy.GetStateName(1);
    }

    [Test]
    public void DeflateCall()
    {
      encoding = "deflate";
      IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
      proxy.Url = "http://127.0.0.1:11002/";
      proxy.EnableCompression = true;
      string name = proxy.GetStateName(1);
    }
  }
}

#endif