using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Threading;
using System.Diagnostics;

public class HttpListenerController
{
  private Thread _pump;
  private bool _listening = false;
  private string _virtualDir;
  private string _physicalDir;
  private string[] _prefixes;
  private HttpListenerWrapper _listener;

  public HttpListenerController(string[] prefixes, string vdir, string pdir)
  {
    _prefixes = prefixes;
    _virtualDir = vdir;
    _physicalDir = pdir;
  }

  public void Start()
  {
    _listening = true;
    _pump = new Thread(new ThreadStart(Pump));
    _pump.Start();
  }

  public void Stop()
  {
    _listening = false;
    _pump.Abort();
    _pump.Join();
  }

  private void Pump()
  {
    try
    {
      _listener = (HttpListenerWrapper)ApplicationHost.CreateApplicationHost(
          typeof(HttpListenerWrapper), _virtualDir, _physicalDir);
      _listener.Configure(_prefixes, _virtualDir, _physicalDir);
      _listener.Start();

      while (_listening)
        _listener.ProcessRequest();
    }
    catch (Exception ex)
    {
      EventLog myLog = new EventLog();
      myLog.Source = "HttpListenerController";
      if (null != ex.InnerException)
        myLog.WriteEntry(ex.InnerException.ToString(), EventLogEntryType.Error);
      else
        myLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
    }
  }
}