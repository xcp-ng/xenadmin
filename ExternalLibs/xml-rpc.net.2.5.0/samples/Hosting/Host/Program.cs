using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Threading;
using System.Diagnostics;
using CookComputing.XmlRpc;

class Program
{
  static void Main(string[] args)
  {
    HttpListenerController _controller = null;

    string[] prefixes = new string[] {
                "http://localhost:8081/", 
                "http://127.0.0.1:8081/"
        };
    string curDir = System.Environment.CurrentDirectory;
    string vdir = "/";
    string pdir = curDir; ;

    _controller = new HttpListenerController(prefixes, vdir, pdir);
    _controller.Start();


    IStateName proxy = XmlRpcProxyGen.Create<IStateName>();
    (proxy as XmlRpcClientProtocol).Url = "http://127.0.0.1:8081/statename.rem";
    string name = proxy.GetStateName(1);
    Console.WriteLine("State 1 is {0}", name);
    _controller.Stop();
  }
}