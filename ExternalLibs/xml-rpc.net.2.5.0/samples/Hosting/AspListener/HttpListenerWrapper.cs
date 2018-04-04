using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Threading;
using System.Diagnostics;

public class HttpListenerWrapper : MarshalByRefObject
{
  private HttpListener _listener;
  private string _virtualDir;
  private string _physicalDir;

  public void Configure(string[] prefixes, string vdir, string pdir)
  {
    _virtualDir = vdir;
    _physicalDir = pdir;
    _listener = new HttpListener();

    foreach (string prefix in prefixes)
      _listener.Prefixes.Add(prefix);
  }
  public void Start()
  {
    _listener.Start();
  }
  public void Stop()
  {
    _listener.Stop();
  }
  public void ProcessRequest()
  {
    HttpListenerContext ctx = _listener.GetContext();
    HttpListenerWorkerRequest workerRequest =
        new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
    HttpRuntime.ProcessRequest(workerRequest);
  }
}



