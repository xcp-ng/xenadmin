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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

using XenCenterLib;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    [Flags]
    public enum ArchiveInterval { None = 0, FiveSecond = 1, OneMinute = 2, OneHour = 4, OneDay = 8 }

    public class ArchiveMaintainer
    {
        private delegate void ReaderDelegate(XmlReader reader, IXenObject xmo);
        private delegate Uri URIDelegate(Session session, Host host, ArchiveInterval interval, IXenObject xmo);

        internal const string RrdUpdatesPath = "rrd_updates";
        private const string RrdHostPath = "host_rrds";
        private const string RrdVmPath = "vm_rrds";
        internal const string RrdCFAverage = "AVERAGE";

        private const string RrdHostUpdatesQuery = "session_id={0}&start={1}&cf={2}&interval={3}&host=true&vm_uuid=";
        internal const string RrdHostAndVmUpdatesQuery = "session_id={0}&start={1}&cf={2}&interval={3}&host=true";
        private const string RrdVmUpdatesQuery = "session_id={0}&start={1}&cf={2}&interval={3}&vm_uuid={4}";
        private const string RrdHostQuery = "session_id={0}";
        private const string RrdVmQuery = "session_id={0}&uuid={1}";

        private const long TicksInOneSecond = 10000000;
        private const long TicksInFiveSeconds = 50000000;
        internal const long TicksInTenSeconds = 100000000;
        private const long TicksInOneMinute = 600000000;
        internal const long TicksInTenMinutes = 6000000000;
        private const long TicksInOneHour = 36000000000;
        internal const long TicksInTwoHours = 72000000000;
        private const long TicksInOneDay = 864000000000;
        internal const long TicksInSevenDays = 6048000000000;
        internal const long TicksInOneYear = 316224000000000;

        private const int FiveSecondsInTenMinutes = 120;
        private const int MinutesInTwoHours = 120;
        private const int HoursInOneWeek = 168;
        private const int DaysInOneYear = 366;

        private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private const int SleepTime = 5000;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Fired (on a background thread) when new performance data are received from the server
        /// </summary>
        internal event Action ArchivesUpdated;

        internal readonly Dictionary<ArchiveInterval, DataArchive> Archives = new Dictionary<ArchiveInterval, DataArchive>();

        /// <summary>
        /// for pausing the retrieval of updates
        /// call Monitor.PulseAll(UpdateMonitor) on resume
        /// </summary>
        private readonly object UpdateMonitor = new object();
        /// <summary>
        /// for waiting between updates
        /// the Monitor has a timeout too so we either wait for 'SleepTime' or a pulseall on WaitUpdates
        /// </summary>
        private readonly object WaitUpdates = new object();

        private Thread UpdaterThread;

        /// <summary>
        ///  if true UpdaterThread will keep looping
        /// </summary>
        private bool RunThread;
        /// <summary>
        /// Whether the thread is started or not
        /// </summary>
        private bool ThreadRunning;

        /// <summary>
        /// collection for holding updates whil
        /// </summary>
        private List<DataSet> SetsAdded;

        private IXenObject _xenObject;

        /// <summary>
        /// Gui Thread
        /// </summary>
        public IXenObject XenObject
        {
            private get { return _xenObject; }
            set
            {
                Program.AssertOnEventThread();

                string oldref = _xenObject == null ? "" : _xenObject.opaque_ref;
                _xenObject = value;
                string newref = _xenObject == null ? "" : _xenObject.opaque_ref;
                FirstTime = FirstTime || newref != oldref;
            }
        }

        public DateTime LastFiveSecondCollection = DateTime.MinValue;
        public DateTime LastOneMinuteCollection = DateTime.MinValue;
        public DateTime LastOneHourCollection = DateTime.MinValue;
        public DateTime LastOneDayCollection = DateTime.MinValue;

        public bool FirstTime = true;

        public bool LoadingInitialData = false;

        public TimeSpan ClientServerOffset
        {
            get
            {
                IXenObject m = XenObject;
                return m == null ? TimeSpan.Zero : m.Connection.ServerTimeOffset;
            }
        }

        public ArchiveMaintainer()
        {
            Archives.Add(ArchiveInterval.FiveSecond, new DataArchive(FiveSecondsInTenMinutes + 4));
            Archives.Add(ArchiveInterval.OneMinute, new DataArchive(MinutesInTwoHours));
            Archives.Add(ArchiveInterval.OneHour, new DataArchive(HoursInOneWeek));
            Archives.Add(ArchiveInterval.OneDay, new DataArchive(DaysInOneYear));
            Archives.Add(ArchiveInterval.None, new DataArchive(0));

            UpdaterThread = new Thread(Update) {Name = "Archive Maintainer", IsBackground = true};
        }

        /// <summary>
        /// Call me, async update graph data set
        /// UpdaterThread Thread
        /// </summary>
        private void Update()
        {
            while (RunThread)
            {
                IXenObject xenObject = XenObject;
                Host Host = GetHost(xenObject);

                DateTime ServerWas = ServerNow(); // get time before updating so we don't miss any 5 second updates if getting the past data

                if (FirstTime)
                {
                    // Restrict to at most 24 hours data if necessary
                    if (Helpers.FeatureForbidden(_xenObject, XenAPI.Host.RestrictPerformanceGraphs))
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = 24;
                        Archives[ArchiveInterval.OneDay].MaxPoints = 0;
                    }
                    else
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = HoursInOneWeek;
                        Archives[ArchiveInterval.OneDay].MaxPoints = DaysInOneYear;
                    }

                    foreach (DataArchive a in Archives.Values)
                        a.ClearSets();

                    LoadingInitialData = true;
                    OnArchivesUpdated();
                    Get(ArchiveInterval.None, RrdsUri, RRD_Full_InspectCurrentNode, Host, xenObject);
                    LoadingInitialData = false;
                    OnArchivesUpdated();

                    LastFiveSecondCollection = ServerWas;
                    LastOneMinuteCollection = ServerWas;
                    LastOneHourCollection = ServerWas;
                    LastOneDayCollection = ServerWas;
                    FirstTime = false;
                }

                if (ServerWas - LastFiveSecondCollection > FiveSeconds)
                {
                    GetUpdate(ArchiveInterval.FiveSecond, Host, xenObject);
                    LastFiveSecondCollection = ServerWas;
                    Archives[ArchiveInterval.FiveSecond].Load(SetsAdded);
                }
                if (ServerWas - LastOneMinuteCollection > OneMinute)
                {
                    GetUpdate(ArchiveInterval.OneMinute, Host, xenObject);
                    LastOneMinuteCollection = ServerWas;
                    Archives[ArchiveInterval.OneMinute].Load(SetsAdded);
                }
                if (ServerWas - LastOneHourCollection > OneHour)
                {
                    GetUpdate(ArchiveInterval.OneHour, Host, xenObject);
                    LastOneHourCollection = ServerWas;
                    Archives[ArchiveInterval.OneHour].Load(SetsAdded);
                }
                if (ServerWas - LastOneDayCollection > OneDay)
                {
                    GetUpdate(ArchiveInterval.OneDay, Host, xenObject);
                    LastOneDayCollection = ServerWas;
                    Archives[ArchiveInterval.OneDay].Load(SetsAdded);
                }

                lock (WaitUpdates)
                {
                    Monitor.Wait(WaitUpdates, SleepTime);
                }
                lock (UpdateMonitor)
                {
                    if (!ThreadRunning)
                        Monitor.Wait(UpdateMonitor);
                }
            }
        }

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        public DateTime ServerNow()
        {
            return DateTime.UtcNow.Subtract(ClientServerOffset);
        }

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void GetUpdate(ArchiveInterval interval, Host host, IXenObject xo)
        {
            Get(interval, UpdateUri, RRD_Update_InspectCurrentNode, host, xo);
        }

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void Get(ArchiveInterval interval, URIDelegate URI, ReaderDelegate Reader,
            Host host, IXenObject xenObject)
        {
            if (host == null)
                return;

            try
            {
                Session session = xenObject.Connection.Session;
                if (session == null)
                    return;
                using (Stream httpstream = HTTPHelper.GET(URI(session, host, interval, xenObject), xenObject.Connection, true))
                {
                    using (XmlReader reader = XmlReader.Create(httpstream))
                    {
                        SetsAdded = new List<DataSet>();
                        while (reader.Read())
                        {
                            Reader(reader, xenObject);
                        }
                    }
                }
            }
            catch (WebException)
            {
            }
            catch (Exception e)
            {
                log.Debug(string.Format("ArchiveMaintainer: Get updates for {0}: {1} Failed.", xenObject is Host ? "Host" : "VM", xenObject != null ? xenObject.opaque_ref : Helper.NullOpaqueRef), e);
            }
        }

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private Uri UpdateUri(Session session, Host host, ArchiveInterval interval, IXenObject xo)
        {
            string query =
                xo is Host ?
                    string.Format(RrdHostUpdatesQuery, Uri.EscapeDataString(session.opaque_ref), TimeFromInterval(interval), RrdCFAverage, ToSeconds(interval)) :
                xo is VM ?
                    string.Format(RrdVmUpdatesQuery, Uri.EscapeDataString(session.opaque_ref), TimeFromInterval(interval), RrdCFAverage, ToSeconds(interval), Helpers.GetUuid(xo)) :
                    "";
            return BuildUri(host, RrdUpdatesPath, query);
        }

        private static Uri RrdsUri(Session session, Host host, ArchiveInterval interval, IXenObject xo)
        {
            string query =
                xo is Host ?
                    string.Format(RrdHostQuery, Uri.EscapeDataString(session.opaque_ref)) :
                xo is VM ?
                    string.Format(RrdVmQuery, Uri.EscapeDataString(session.opaque_ref), Helpers.GetUuid(xo)) :
                    "";
            return BuildUri(host, xo is Host ? RrdHostPath : RrdVmPath, query);
        }

        private static Uri BuildUri(Host host, string path, string query)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = host.Connection.UriScheme;
            builder.Host = host.address;
            builder.Port = host.Connection.Port;
            builder.Path = path;
            builder.Query = query;
            return builder.Uri;
        }

        public static long ToTicks(ArchiveInterval interval)
        {
            switch (interval)
            {
                case ArchiveInterval.FiveSecond:
                    return TicksInFiveSeconds;
                case ArchiveInterval.OneMinute:
                    return TicksInOneMinute;
                case ArchiveInterval.OneHour:
                    return TicksInOneHour;
                default:
                    return TicksInOneDay;
            }
        }

        private static long ToSeconds(ArchiveInterval interval)
        {
            return TimeUtil.TicksToSeconds(ToTicks(interval));
        }

        private long TimeFromInterval(ArchiveInterval interval)
        {
            switch (interval)
            {
                case ArchiveInterval.FiveSecond:
                    if (LastFiveSecondCollection != DateTime.MinValue)
                        return TimeUtil.TicksToSecondsSince1970(LastFiveSecondCollection.Ticks - TicksInFiveSeconds);
                    break;
                case ArchiveInterval.OneMinute:
                    if (LastOneMinuteCollection != DateTime.MinValue)
                        return TimeUtil.TicksToSecondsSince1970(LastOneMinuteCollection.Ticks - TicksInOneMinute);
                    break;
                case ArchiveInterval.OneHour:
                    if (LastOneHourCollection != DateTime.MinValue)
                        return TimeUtil.TicksToSecondsSince1970(LastOneHourCollection.Ticks - TicksInOneHour);
                    break;
                case ArchiveInterval.OneDay:
                    if (LastOneDayCollection != DateTime.MinValue)
                        return TimeUtil.TicksToSecondsSince1970(LastOneDayCollection.Ticks - TicksInOneDay);
                    break;
            }
            return 0;
        }

        private static ArchiveInterval GetArchiveIntervalFromFiveSecs(long v)
        {
            switch (v)
            {
                case 1:
                    return ArchiveInterval.FiveSecond;
                case 12:
                    return ArchiveInterval.OneMinute;
                case 720:
                    return ArchiveInterval.OneHour;
                case 17280:
                    return ArchiveInterval.OneDay;
                default:
                    return ArchiveInterval.None;
            }
        }


        private long EndTime = 0;
        private bool BailOut = false;
        private long CurrentInterval = 0;
        private long StepSize = 0;

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void RRD_Full_InspectCurrentNode(XmlReader reader, IXenObject xmo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                LastNode = reader.Name;
                if (LastNode == "row")
                {
                    CurrentTime += CurrentInterval * StepSize * TicksInOneSecond;
                    ValueCount = 0;
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                LastNode = reader.Name;
                if (LastNode == "rra")
                {
                    if (BailOut)
                    {
                        BailOut = false;
                        return;
                    }

                    ArchiveInterval i = GetArchiveIntervalFromFiveSecs(CurrentInterval);
                    if (i != ArchiveInterval.None)
                        Archives[i].CopyLoad(SetsAdded);

                    foreach (DataSet set in SetsAdded)
                        set.Points.Clear();
                    BailOut = false;
                }
            }

            if (reader.NodeType != XmlNodeType.Text)
                return;

            if (LastNode == "name")
            {
                string str = reader.ReadContentAsString();
                string id = string.Format("{0}:{1}:{2}", xmo is Host ? "host" : "vm", Helpers.GetUuid(xmo), str);
                SetsAdded.Add(DataSet.Create(id, xmo, true, str));
            }
            else if (LastNode == "step")
            {
                string str = reader.ReadContentAsString();
                StepSize = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (LastNode == "lastupdate")
            {
                string str = reader.ReadContentAsString();
                EndTime = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (LastNode == "pdp_per_row")
            {
                string str = reader.ReadContentAsString();
                CurrentInterval = long.Parse(str, CultureInfo.InvariantCulture);

                long modInterval = EndTime % (StepSize * CurrentInterval);
                long stepCount = CurrentInterval == 1 ? FiveSecondsInTenMinutes // 120 * 5 seconds in 10 minutes
                               : CurrentInterval == 12 ? MinutesInTwoHours   // 120 minutes in 2 hours
                               : CurrentInterval == 720 ? HoursInOneWeek     // 168 hours in a week
                               : DaysInOneYear;                              // 366 days in a year

                CurrentTime = new DateTime((((EndTime - modInterval) - (StepSize * CurrentInterval * stepCount)) * TimeSpan.TicksPerSecond) + TimeUtil.TicksBefore1970).ToLocalTime().Ticks;
            }
            else if (LastNode == "cf")
            {
                string str = reader.ReadContentAsString();
                if (str != RrdCFAverage)
                    BailOut = true;
            }
            else if (LastNode == "v")
            {
                if (BailOut || SetsAdded.Count <= ValueCount)
                    return;

                DataSet set = SetsAdded[ValueCount];
                string str = reader.ReadContentAsString();
                set.AddPoint(str, CurrentTime, SetsAdded);
                ValueCount++;
            }
        }

        private long CurrentTime = 0;
        private int ValueCount;

        private string LastNode = "";

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void RRD_Update_InspectCurrentNode(XmlReader reader, IXenObject xo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                LastNode = reader.Name;
                if (LastNode == "row")
                {
                    ValueCount = 0;
                }
            }
            if (reader.NodeType != XmlNodeType.Text) return;
            if (LastNode == "entry")
            {
                string str = reader.ReadContentAsString();
                SetsAdded.Add(DataSet.Create(str, xo));
            }
            else if (LastNode == "t")
            {
                string str = reader.ReadContentAsString();
                CurrentTime = new DateTime((Convert.ToInt64(str) * TimeSpan.TicksPerSecond) + TimeUtil.TicksBefore1970).ToLocalTime().Ticks;
            }
            else if (LastNode == "v")
            {
                if (SetsAdded.Count <= ValueCount) return;
                DataSet set = SetsAdded[ValueCount];
                string str = reader.ReadContentAsString();
                set.AddPoint(str, CurrentTime, SetsAdded);
                ValueCount++;
            }
        }

        /// <summary>
        /// run this to start or resume getting updates
        /// </summary>
        public void Start()
        {
            if (ThreadRunning)
                return;  // if we are already running dont start twice!
            ThreadRunning = true;
            RunThread = true; // keep looping
            if ((UpdaterThread.ThreadState & ThreadState.Unstarted) > 0)
                UpdaterThread.Start(); // if we have never been started
            else
            {
                lock (UpdateMonitor)
                    Monitor.PulseAll(UpdateMonitor);
                lock (WaitUpdates)
                    Monitor.PulseAll(WaitUpdates);
            }
        }

        /// <summary>
        /// for clean-up on exit only
        /// </summary>
        public void Stop()
        {
            ThreadRunning = false;
            RunThread = false; // exit loop
            // make sure we clear all Monitor.Waits so we can exit
            lock (WaitUpdates)
                Monitor.PulseAll(WaitUpdates);
            lock (UpdateMonitor)
                Monitor.PulseAll(UpdateMonitor);
        }

        /// <summary>
        /// for stoping getting updates when switching away from perfomance panel
        /// </summary>
        public void Pause()
        {
            ThreadRunning = false; // stop updating
            lock (WaitUpdates) // clear the first Monitor.Wait so we pause the thread instantly.
                Monitor.PulseAll(WaitUpdates);
        }

        public static ArchiveInterval NextArchiveDown(ArchiveInterval current)
        {
            switch (current)
            {
                case ArchiveInterval.FiveSecond:
                    return ArchiveInterval.None;
                case ArchiveInterval.OneMinute:
                    return ArchiveInterval.FiveSecond;
                case ArchiveInterval.OneHour:
                    return ArchiveInterval.OneMinute;
                case ArchiveInterval.OneDay:
                    return ArchiveInterval.OneHour;
                default:
                    return ArchiveInterval.None;
            }
        }

        public DateTime GraphNow
        {
            get
            {
                return DateTime.Now - (ClientServerOffset + TimeSpan.FromSeconds(15));
            }
        }

        private static Host GetHost(IXenObject xmo)
        {
            if (xmo is Host)
            {
                return (Host)xmo;
            }
            else if (xmo is VM)
            {
                VM vm = (VM)xmo;
                return xmo.Connection.Resolve(vm.resident_on) ?? Helpers.GetMaster(xmo.Connection);
            }
            else
            {
                System.Diagnostics.Trace.Assert(false);
                return null;
            }
        }


        private void OnArchivesUpdated()
        {
            if (ArchivesUpdated != null)
                ArchivesUpdated();
        }
    }
}
