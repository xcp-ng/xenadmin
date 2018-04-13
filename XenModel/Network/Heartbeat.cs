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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Network
{
    /// <summary>
    /// A per-connection thread that periodically calls host.get_servertime.  This is used to tell us the skew between the clocks on the client
    /// and the server, but more importantly, to act as a connection heartbeat.
    /// xapi will return from event.next every so often even if there are no new events, to act as the connection heartbeat in the other direction.
    /// </summary>
    public class Heartbeat
    {
        private const int HeartbeatInterval = 15000;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IXenConnection connection;
        private Session session = null;

        // Once set to false, the worker thread will eventually exit.
        private volatile bool _run = true;

        private bool running = false;

        /// <summary>
        /// If true, the heartbeat has already failed once, and we are giving the server a final second chance.
        /// </summary>
        private bool retrying = false;

        private readonly int connectionTimeout;

        public Heartbeat(IXenConnection connection, int connectionTimeout)
        {
            this.connection = connection;
            this.connectionTimeout = connectionTimeout;
        }

        /// <summary>
        /// Should only be called once.
        /// </summary>
        public void Start()
        {
            // We shouldn't need the running flag -- this is just a safety catch.
            if (!running)
            {
                running = true;
                Thread t = new Thread(HeartbeatThread);
                t.Name = "Heartbeat for " + connection.Hostname;
                t.IsBackground = true;
                t.Start();
            }
        }

        public void Stop()
        {
            _run = false;
        }

        /// <param name="o">Unused</param>
        private void HeartbeatThread(object o)
        {
            log.DebugFormat("Heartbeat thread for connection to {0} started", connection.Hostname);

            try
            {
                while (_run)
                {
                    DoHeartbeat();
                    System.Threading.Thread.Sleep(HeartbeatInterval);
                }
            }
            finally
            {
                log.DebugFormat("Heartbeat thread for connection to {0} terminated", connection.Hostname);
            }
        }

        private readonly string heartbeatConnectionGroupName = Guid.NewGuid().ToString(); 

        private void DoHeartbeat()
        {
            if (!connection.IsConnected)
                return;

            try
            {
                if (session == null)
                {
                    // Try to get a new session, but only give the server one chance (otherwise we get the default 3x timeout)
                    session = connection.DuplicateSession(connectionTimeout < 5000 ? 5000 : connectionTimeout);
                    session.ConnectionGroupName = heartbeatConnectionGroupName; // this will force the Heartbeat session onto its own set of TCP streams (see CA-108676)
                }

                GetServerTime();

                // Now that we've successfully received a heartbeat, reset our 'second chance' for the server to timeout
                if (retrying)
                    log.DebugFormat("Heartbeat for {0} has come back", session.Url);
                retrying = false;
            }
            catch (TargetInvocationException exn)
            {
                if (exn.InnerException is SocketException ||
                    exn.InnerException is WebException)
                {
                    log.Debug(exn.Message);
                }
                else
                {
                    log.Error(exn);
                }
                HandleConnectionLoss();
            }
            catch (WebException exn)
            {
                log.Error(exn);
                var webResponse = (HttpWebResponse)exn.Response;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired) // work-around for CA-214653
                {
                    if (session == null)
                        log.Debug("Heartbeat has failed due to null session; closing the main connection");
                    else if (session.Credentials == null)
                        log.DebugFormat("Heartbeat for {0} has failed due to missing credentials; closing the main connection", session.Url);
                    else
                        log.DebugFormat("Heartbeat for {0} has failed due to incorrect credentials; closing the main connection", session.Url);

                    connection.Interrupt();
                    DropSession();
                }
                else
                {
                    HandleConnectionLoss();
                }
            }
            catch (Exception exn)
            {
                log.Error(exn);
                HandleConnectionLoss();
            }
        }

        private void GetServerTime()
        {
            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return;
            DateTime t = Host.get_servertime(session, master.opaque_ref);
            connection.ServerTimeOffset = DateTime.UtcNow - t;
        }

        private void HandleConnectionLoss()
        {
            // We retry once, as the server is entitled to drop persistent connections from time to time.
            // After that, we assume that the server's gone away.
            // Note that this doubles the effective timeout before we decide the server has died.
            if (retrying && !connection.ExpectDisruption)
            {
                log.DebugFormat("Heartbeat for {0} has failed for the second time; closing the main connection",
                                session == null ? "null" : session.Url);
                connection.Interrupt();
                DropSession();
            }
            else
            {
                log.DebugFormat("Heartbeat for {0} has failed; retrying",
                                session == null ? "null" : session.Url);
                retrying = true;
            }
        }

        /// Drop the session so that we'll get a new one the next time.
        /// This is used when handling exceptions.
        private void DropSession()
        {
            Session s = session;
            session = null;
            // Do this on a new thread: if the master has died, then the session logout may block
            // up to the timeout. Doing this on the calling thread would mean doubling the actual time elapsed
            // before we decide the server is dead.
            connection.Logout(s);
        }
    }
}
