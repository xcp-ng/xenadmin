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
using XenAPI;
using System.Collections.Generic;
using System.Net;


namespace XenAdmin.Network
{
    public interface IXenConnection : IComparable<IXenConnection>, IDisposable
    {
        string Hostname { get; set; }
        ICache Cache { get; }
        string Username { get; set; }
        string Password { get; set; }
        bool ExpectPasswordIsCorrect { get; set; }
        bool ExpectDisruption { get; set; }
        int Port { get; set; }
        string FriendlyName { get; set; }
        bool CacheIsPopulated { get; }
        event Action<IXenConnection> CachePopulated;
        event Action<IXenConnection> ClearingCache;
        event Action<IXenConnection> BeforeConnectionEnd;
        event Action<IXenConnection> ConnectionClosed;
        event Action<IXenConnection> ConnectionLost;
        event Action<IXenConnection> ConnectionReconnecting;
        event EventHandler<ConnectionResultEventArgs> ConnectionResult;
        event Action<IXenConnection> ConnectionStateChanged;
        event Action<IXenConnection, string> ConnectionMessageChanged;
        event Action<IXenConnection, bool> BeforeMajorChange;
        event Action<IXenConnection, bool> AfterMajorChange;
        Session DuplicateSession();
        Session DuplicateSession(int timeout);
        void EndConnect(bool resetState = true, bool exiting = false);
        void Interrupt();
        Session Connect(string user, string password);
        List<string> PoolMembers { get; set; }
        void LoadCache(Session session);
        bool SuppressErrors { get; set; }
        bool MasterMayChange { get; set; }
        bool SaveDisconnected { get; set; }
        string HostnameWithPort { get; }
        bool InProgress { get; }
        bool IsConnected { get; }
        string Name { get; }
        Session ElevatedSession(string username, string password);
        T TryResolveWithTimeout<T>(XenRef<T> t) where T : XenObject<T>;
        T Resolve<T>(XenRef<T> xenRef) where T : XenObject<T>;
        List<T> ResolveAll<T>(IEnumerable<XenRef<T>> xenRefs) where T : XenObject<T>;
        List<VDI> ResolveAllShownXenModelObjects(List<XenRef<VDI>> xenRefs, bool showHiddenObjects);
        T WaitForCache<T>(XenRef<T> xenref) where T : XenObject<T>;
        T WaitForCache<T>(XenRef<T> xenref, Func<bool> cancelling) where T : XenObject<T>;
        void WaitFor(Func<bool> predicate, Func<bool> cancelling);
        TimeSpan ServerTimeOffset { get; set; }
        Session Session { get; }
        event EventHandler<EventArgs> TimeSkewUpdated;
        string UriScheme { get; }
        string Version { get; set; }
        event EventHandler<EventArgs> XenObjectsUpdated;
        NetworkCredential NetworkCredential { get; set; }

        /// <summary>
        /// Try to logout the given session. This will cause any threads blocking on Event.next() to get
        /// a XenAPI.Failure (which is better than them hanging around forever).
        /// Do on a background thread - otherwise, if the master has died, then this will block
        /// until the timeout is reached (default 20s).
        /// </summary>
        /// <param name="session">May be null, in which case nothing happens.</param>
        void Logout();

        /// <summary>
        /// Try to logout the given session. This will cause any threads blocking on Event.next() to get
        /// a XenAPI.Failure (which is better than them hanging around forever).
        /// Do on a background thread - otherwise, if the master has died, then this will block
        /// until the timeout is reached (default 20s).
        /// </summary>
        /// <param name="session">May be null, in which case nothing happens.</param>
        void Logout(Session session, bool exiting = false);
    }

    public class ConnectionResultEventArgs : EventArgs
    {
        public bool Connected;
        public string Reason;
        public Exception Error;

        public ConnectionResultEventArgs(bool connected, string reason, Exception error)
        {
            this.Connected = connected;
            this.Reason = reason;
            this.Error = error;
        }
    }
}
