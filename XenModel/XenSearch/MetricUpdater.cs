/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Xml;

using XenCenterLib;
using XenAPI;

using XenAdmin.Core;
using System.Threading.Tasks;
using System.Linq;

namespace XenAdmin.XenSearch
{
    public class MetricUpdater
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int _sleep = 30000;

        private bool _run;
        private readonly object _pauseMonitor = new object();
        private bool _pause;
        private readonly object _sleepMonitor = new object();
        private bool _skip_sleep;

        private readonly Thread _metricUpdaterThread;

        private readonly object _hostsLock = new object();
        private ConcurrentDictionary<Host, HostMetric> _hosts = new ConcurrentDictionary<Host, HostMetric>();

        public event EventHandler MetricsUpdated;

        public MetricUpdater()
        {
            _metricUpdaterThread = new Thread(UpdateMetrics) {IsBackground = true, Name = "MetricUpdater"};
        }

        /// <summary>
        /// start or resume thread
        /// </summary>
        public void Start()
        {
            _run = true;
            _pause = false;
            _skip_sleep = true;
            if ((_metricUpdaterThread.ThreadState & ThreadState.Unstarted) > 0)
            {
                log.Info("Starting MetricUpdater thread");
                _metricUpdaterThread.Start();
            }
            else
            {
                lock (_pauseMonitor)
                    Monitor.PulseAll(_pauseMonitor);
            }
        }

        public void Pause()
        {
            _pause = true;
            lock (_sleepMonitor)
                Monitor.PulseAll(_sleepMonitor);
        }

        public void Kill()
        {
            log.Info("Killing MetricUpdater thread");
            _run = false;
            _pause = false;
            lock (_pauseMonitor)
                Monitor.PulseAll(_pauseMonitor);
            lock (_sleepMonitor)
                Monitor.PulseAll(_sleepMonitor);
        }

        private void UpdateMetrics()
        {
            while (_run)
            {
                // We work on the current list, at the time each update pulse begins.
                // Sometimes _hosts can be changed by SetXenModelObjects while we're updating,
                // but that's OK: we've done an unnecessary update of a data structure that's
                // about to go away, but we'll populate the new structure soon.
                ConcurrentDictionary<Host, HostMetric> hosts;

                lock (_hostsLock)
                {
                    hosts = _hosts;
                }

                Parallel.ForEach(hosts.Keys.Where(h => h.Connection.IsConnected),
                    host => 
                    {
                        HostMetric hm;
                        //Intentionally using TryGetValue (instead of indexer's getter), because there is a slight chance 'host' is not in 'hosts.Keys' anymore.
                        //This means that metrics of such 'host' can be ignored safely (no else implemented below).
                        if (hosts.TryGetValue(host, out hm)) 
                        {
                            var values = ValuesFor(host);
                            DistributeValues(values, hm);
                        }
                    });
                
                OnMetricsUpdate(EventArgs.Empty);

                if (_pause)
                {
                    lock (_pauseMonitor)
                        Monitor.Wait(_pauseMonitor);
                }
                else if (!_skip_sleep)
                {
                    lock (_sleepMonitor)
                        Monitor.Wait(_sleepMonitor, _sleep);
                }
                _skip_sleep = false;
            }
        }

        private static void DistributeValues(Dictionary<string, double> value, HostMetric host)
        {
            foreach (KeyValuePair<string, double> kvp in value)
            {
                string[] bits = kvp.Key.Split(':');
                if (bits.Length < 4)
                    continue;

                double currentValue = kvp.Value;
                string key = GetSetName(kvp.Key);

                if (bits[1].ToLowerInvariant() == "host")
                {
                    host.Values[key] = currentValue;
                }
                else
                {
                    VmMetric vm = host.GetVmByUuid(bits[2]);
                    if (vm == null)
                        continue;
                    vm.Values[key] = currentValue;
                }
            }
        }

        private static Dictionary<string, double> ValuesFor(Host host)
        {
            if (host.address == null)
                return new Dictionary<string, double>();

            try
            {
                Session session = host.Connection.Session;
                if (session != null)
                    return AllValues(HTTPHelper.GET(GetUri(session, host), host.Connection, true));
            }
            catch (Exception e)
            {
                log.Warn(string.Format("Exception getting metrics for {0}", Helpers.GetName(host)), e);
            }
            return new Dictionary<string, double>();
        }

        private static Dictionary<string, double> AllValues(Stream httpstream)
        {
            var result = new Dictionary<string, double>();
            var keys = new List<string>();

            using (var reader = XmlReader.Create(httpstream))
            {
                string lastnode = "";
                int lastkey = 0;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        lastnode = reader.Name;
                        if (lastnode == "row")
                            lastkey = 0;
                    }

                    if (reader.NodeType != XmlNodeType.Text)
                        continue;

                    if (lastnode == "entry")
                        keys.Add(reader.ReadContentAsString());

                    if (lastnode == "v")
                    {
                        result[keys[lastkey]] = Helpers.StringToDouble(reader.ReadContentAsString());
                        lastkey++;
                    }
                }
            }
            return result;
        }

        public void SetXenObjects(IXenObject[] objects)
        {
            InvokeHelper.AssertOnEventThread();

            // The QueryPanel can trigger this too often (sometimes more than once when we switch to it).
            // So we do a preliminary check of whether any changes will be needed to the data structures.
            // Without this, we wipe out the stats, and can sometimes then display them before they've been
            // repopulated. This check used to be at the end of this function, just before _hosts = hosts,
            // but it's cheaper to do it up front, at the expense of some code duplication.

            if (!AnyNewObjects(objects))
                return;

            var hosts = new ConcurrentDictionary<Host, HostMetric>();

            // Create HostMetric's for all the hosts
            foreach (IXenObject obj in objects)
            {
                Host host = obj as Host;
                if (host != null)
                    hosts[host] = new HostMetric();
            }

            // Create VmMetric's for all the VMs, and put them under their hosts, indexed by uuid
            foreach (IXenObject obj in objects)
            {
                VM vm = obj as VM;
                if (vm != null)
                {
                    Host host = GetHost(vm);
                    if (host != null)
                    {
                        var hm = hosts.GetOrAdd(host, new HostMetric());
                        string uuid = Helpers.GetUuid(vm);
                        hm.VMs[uuid] = new VmMetric();
                    }
                }
            }

            lock (_hostsLock)
            {
                _hosts = hosts;
            }

            _skip_sleep = true;
            lock (_sleepMonitor)
                Monitor.PulseAll(_sleepMonitor);
        }

        private bool AnyNewObjects(IXenObject[] objects)
        {
            foreach (IXenObject obj in objects)
            {
                Host host = obj as Host;
                if (host != null)
                {
                    if (!_hosts.ContainsKey(host))
                        return true;
                    continue;
                }

                VM vm = obj as VM;
                if (vm != null)
                {
                    Host vmHost = GetHost(vm);

                    if (vmHost != null)
                    {
                        HostMetric hm;
                        if (!_hosts.TryGetValue(vmHost, out hm))
                            return true;
                        string uuid = Helpers.GetUuid(vm);
                        if (!hm.VMs.ContainsKey(uuid))
                            return true;
                    }
                }
            }

            return false;
        }

        public double GetValue(IXenObject obj, string property)
        {
            Host host = GetHost(obj);
            if (host == null)
                return 0d;

            HostMetric host_resident;
            if (!_hosts.TryGetValue(host, out host_resident))
                return 0d;

            if (obj is Host)
            {
                double result;
                if (host_resident.Values.TryGetValue(property, out result))
                    return result;
                return 0d;
            }

            string uuid = Helpers.GetUuid(obj);

            VmMetric vm = host_resident.GetVmByUuid(uuid);

            if (vm != null)
            {
                double result;
                if (vm.Values.TryGetValue(property, out result))
                    return result;
            }

            return 0d;
       }

        private const string RrdUpdatesPath = "rrd_updates";
        private const string RrdHostAndVmUpdatesQuery = "session_id={0}&start={1}&cf={2}&interval={3}&host=true";
        private const long TicksInTenSeconds = 100000000;
        private const string RrdCFAverage = "AVERAGE";

        private static Uri GetUri(Session session, Host host)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = host.Connection.UriScheme;
            builder.Host = host.address;
            builder.Port = host.Connection.Port;
            builder.Path = RrdUpdatesPath;
            builder.Query = string.Format(RrdHostAndVmUpdatesQuery, Uri.EscapeDataString(session.opaque_ref), TimeUtil.TicksToSecondsSince1970(DateTime.UtcNow.Ticks - (host.Connection.ServerTimeOffset.Ticks + TicksInTenSeconds)), RrdCFAverage, 5);
            return builder.Uri;
        }

        private static Host GetHost(IXenObject obj)
        {
            Host host = obj as Host;
            if (host == null && obj is VM)
            {
                host = obj.Connection.Resolve<Host>(((VM)obj).resident_on);
                if (host == null)
                {
                    host = Helpers.GetMaster(obj.Connection);
                }
            }
            return host;
        }

        private static string GetSetName(string p)
        {
            string[] bits = p.Split(':');
            return bits[bits.Length - 1];
        }

        public void Prod()
        {
            _skip_sleep = true;
            lock (_sleepMonitor)
                Monitor.PulseAll(_sleepMonitor);
        }

        protected virtual void OnMetricsUpdate(EventArgs e)
        {
            EventHandler handler = MetricsUpdated;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private class HostMetric
        {
            public readonly ConcurrentDictionary<string, double> Values = new ConcurrentDictionary<string, double>();
            public readonly ConcurrentDictionary<string, VmMetric> VMs = new ConcurrentDictionary<string, VmMetric>();  // VMs under this host, indexed by uuid

            public VmMetric GetVmByUuid(string uuid)
            {
                VmMetric ans;
                if (VMs.TryGetValue(uuid, out ans))
                    return ans;
                return null;
            }
        }

        private class VmMetric
        {
            public readonly ConcurrentDictionary<string, double> Values = new ConcurrentDictionary<string, double>();
        }

        /// <summary>
        /// This function is used for generate resource report only.
        /// </summary>
        public void UpdateMetricsOnce()
        {
            Parallel.ForEach(_hosts.Keys.Where(h => h.Connection.IsConnected),
                host =>
                {
                    HostMetric hm;
                    if (_hosts.TryGetValue(host, out hm))
                    {
                        var values = ValuesFor(host);
                        DistributeValues(values, hm);
                    }
                });
        }
    }
}
