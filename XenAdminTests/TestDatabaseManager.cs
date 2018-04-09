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
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using XenAdmin; // For XenAdminConfigManager - model
using XenAdmin.Network; //For IXenConnection - model
using XenAdmin.ServerDBs;
using XenAPI; //For API - model
using XenAdminTests.XenModelTests;

namespace XenAdminTests
{
    /// <summary>
    /// Class that creates a connection to a xml xapi database
    /// Connections are stored and can be looked back up without recreation.
    /// Connections to the same DB can be regenerated
    /// Disposable will clear up all connections after use - if you call Dispose()
    /// </summary>
    public sealed class TestDatabaseManager : IObjectManager
    {
        private Dictionary<string,IXenConnection> connections = new Dictionary<string,IXenConnection>();
        private readonly object ConnectionListLock = new object();
        private bool disposed;
        private const string defaultUserName = "root";
        private const string defaultPasword = "";

        public TestDatabaseManager(){}

        /// <summary>
        /// Provide a list of all connections
        /// </summary>
        public List<IXenConnection> AllConnections
        {
            get { return connections.Values.ToList(); }
        }

        /// <summary>
        /// Get the connection for a given database file name
        /// </summary>
        /// <param name="dbFileName">just the file name including the extension</param>
        /// <returns></returns>
        public IXenConnection ConnectionFor(string dbFileName)
        {
            if (HasConnectionFor(dbFileName))
                return connections[dbFileName];
            
            throw new ArgumentException("There is no registered connection for " + dbFileName);
        }

        /// <summary>
        /// Has a connection for a given database file name
        /// </summary>
        /// <param name="dbFileName">just the file name including the extension</param>
        /// <returns></returns>
        public bool HasConnectionFor(string dbFileName)
        {
            return connections.ContainsKey(dbFileName);
        }

        /// <summary>
        /// Constructor that creates a new connection for the given file name
        /// Use LastConnectionCreated to get the connection
        /// </summary>
        /// <param name="dbFileName">just the file name including the extension</param>
        public TestDatabaseManager(params string[] dbFileName)
        {
            CreateNewConnection(dbFileName);
        }

        /// <summary>
        /// Create a new connection with root credentials. If connection already exists it'll be replaced
        /// </summary>
        /// <param name="dbFileName">just the file name including the extension</param>
        /// <returns></returns>
        public void CreateNewConnection(params string[] dbFileName)
        {
            CreateNewConnection(defaultUserName, defaultPasword, dbFileName);
        }

        /// <summary>
        /// Refreshes all stored caches. Consider calling refresh per connection
        /// before opting for this method as it'll be more efficient
        /// </summary>
        public void RefreshAllCaches()
        {
            connections.Keys.ToList().ForEach(RefreshCacheFor);
        }

        /// <summary>
        /// Following an action you may want to call this method if the state of a cached object
        /// has changed
        /// </summary>
        /// <param name="dbName"></param>
        public void RefreshCacheFor(string dbName)
        {
            if (HasConnectionFor(dbName))
            {
                ConnectionFor(dbName).LoadCache(ConnectionFor(dbName).Session);
                return;
            }

            throw new ArgumentException("Cannot refresh cach as no connection exists for " + dbName);
        }

        /// <summary>
        /// Create a new connection with provided credentials. If connection already exists it'll be replaced
        /// </summary>
        /// <param name="dbFileName">just the file name including the extension</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public void CreateNewConnection(string username, string password, params string[] dbFileName)
        {
            if(disposed)
                throw new ObjectDisposedException("This class has been disposed");

            foreach (string db in dbFileName)
            {
                CreateNewConnection(db, username, password);   
            }
            
        }

        private void CreateNewConnection(string dbFileName, string username, string password)
        {
            string fileName = TestResource(dbFileName);
            Assert.True(File.Exists(fileName),
                String.Format("Provided filename does not exist: '{0}'. " +
                "If this is a new file, maybe the 'Copy To Ouput Directory' property has not been set",
                fileName));

            XenAdminConfigManager.Provider = new TestXenAdminConfigProvider();
            var connection = new XenConnection
            {
                Hostname = fileName,
                FriendlyName = dbFileName,
                Port = 443,
                SaveDisconnected = true
            };
            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.ServerCertificateValidationCallback = SSL.ValidateServerCertificate;
            
            Session session = connection.Connect(username, password);
            Assert.NotNull(session);
            connection.LoadCache(session);
            Assert.True(connection.CacheIsPopulated);

            lock (ConnectionListLock)
            {
                if (connections.ContainsKey(dbFileName))
                    connections.Remove(dbFileName);
                connections.Add(dbFileName,connection);
            }
        }

        private string TestResource(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), @"XenAdminTests\TestResources", name);
        }

        /// <summary>
        /// Must be called - clears up all connections etc...
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

        private void Dispose(bool disposing)
        {
     
            if(!disposed)
            {
                if(disposing)
                {
                    foreach (IXenConnection c in connections.Values)
                    {
                        c.Cache.Clear();
                        c.EndConnect(false);
                        c.Dispose();
                        DbProxy.RemoveAll();
                    }
                    lock (ConnectionListLock)
                    {
                        connections.Clear();
                    }
                }
                //TODO: This is a fix as the XenAdminConfigManager is not thread safe - it should be th
                XenAdminConfigManager.Provider = new WinformsXenAdminConfigProvider();
                connections = null;
                disposed = true;
            }
        }
    }
}
