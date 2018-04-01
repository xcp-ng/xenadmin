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
using System.Threading;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAdmin.ServerDBs;
using XenAPI;

namespace XenAdminTests
{
    // IMPORTANT: Do not derive directly from this class. Either derive your TestFixture from
    // MainWindowLauncher_TestFixture, or your SetUpFixture from MainWindowLauncher_SetUpFixture and
    // your TestFixture from MainWindowTester.

    public abstract class MainWindowLauncher : MainWindowTester
    {
        private static log4net.ILog log = null;

        private string[] databases;
        private bool readOnly = false;
        private static Thread oldMainWindowThread;

        public MainWindowLauncher(params string[] databases)
        {
            this.databases = databases;
        }

        public MainWindowLauncher( bool readOnly, params string[] databases)
        {
            this.databases = databases;
            this.readOnly = readOnly;
        }

        public MainWindowLauncher()
            : this(new string[0])
        {
        }

        private Thread mainWindowThread;

        private void CreateMainWindow()
        {
            object oldMainWindow = Program.MainWindow;

            if (oldMainWindowThread != null && oldMainWindowThread.IsAlive)
            {
                log.DebugFormat("Cleaning leftover MainWindowThread from previous test");
                RemoveStateDBs();
                KillMainWindow();
                oldMainWindowThread.Join();
            }

            mainWindowThread = new Thread(() => Program.Main(new string[0]));
            mainWindowThread.Name = "MainWindowThread";
            mainWindowThread.SetApartmentState(ApartmentState.STA);
            mainWindowThread.Start();
            oldMainWindowThread = mainWindowThread;

            while (Program.MainWindow == null || !Program.MainWindow.IsHandleCreated || Program.MainWindow == oldMainWindow)
                Thread.Sleep(500);
        }

        protected void ConnectToStateDBs(params string[] names)
        {
            ConnectToStateDBs(readOnly, names);
        }

        protected void ConnectToStateDBs(bool readOnly, params string[] names)
        {
            if (names.Length > 0)
            {
                List<IXenConnection> connections = new List<IXenConnection>();

                foreach (string name in names)
                {
                    connections.Add(MW(() => LoadDB(name, readOnly ? "readonly" : "root")));
                }

                MWWaitFor(() => TreeViewContainsConnections(connections), "Couldn't connect to db.");
            }
        }

        protected bool TreeViewContainsConnections(IEnumerable<IXenConnection> connections)
        {
            List<IXenConnection> connectionsList = new List<IXenConnection>(connections);
            foreach (VirtualTreeNode n in MainWindowWrapper.TreeView.AllNodes)
            {
                IXenObject x = n.Tag as IXenObject;
                if (x != null && !(x is Host))
                {
                    connectionsList.RemoveAll(c => c == x.Connection);
                }
            }
            return connectionsList.Count == 0;
        }

        protected void RemoveStateDBs()
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                MW(() => c.Cache.Clear());
                c.EndConnect(false);
                c.Dispose();
            }
            lock (ConnectionsManager.ConnectionsLock)
            {
                ConnectionsManager.XenConnections.Clear();
            }

            DbProxy.RemoveAll();
        }

        private IXenConnection LoadDB(string name)
        {
            return LoadDB(name, "root");
        }

        private IXenConnection LoadDB(string name, string username)
        {
            var connection = new XenConnection
            {
                Username = username,
                Password = "",
                Hostname = TestResource(name),
                FriendlyName = name,
                Port = 443,
                SaveDisconnected = true
            };

            lock (ConnectionsManager.ConnectionsLock)
            {
                ConnectionsManager.XenConnections.Add(connection);
            }
            XenConnectionUI.BeginConnect(connection, false, null, false);
            return connection;
        }

        private void KillMainWindow()
        {
            if (Program.MainWindow != null)
            {
                MW(Program.MainWindow.Hide);
                MW(Program.MainWindow.Close);
                MW(Program.MainWindow.Dispose);

            }
            Thread.Sleep(3000);  // We need to give it a short time to clean up before launching another one
        }

        protected void _SetUp()
        {
            XenAdminConfigManager.Provider = new WinformsXenAdminConfigProvider();
            Program.RunInAutomatedTestMode = true;
            if (log == null)
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.DebugFormat("Starting test {0}...", GetType().Name);
            CreateMainWindow();
            log.DebugFormat("MainWindow created for test {0}.  Connecting to databases...", GetType().Name);

            ConnectToStateDBs(readOnly, databases);
        }

        protected void _TearDown()
        {
            log.DebugFormat("Cleaning up after test {0}...", GetType().Name);
            RemoveStateDBs();
            KillMainWindow();
            
            mainWindowThread.Join();
            log.DebugFormat("Clean up complete after test {0}.", GetType().Name);
        }
    }

    public abstract class MainWindowLauncher_TestFixture : MainWindowLauncher
    {
        public MainWindowLauncher_TestFixture(params string[] databases)
            : base(databases)
        { }

        public MainWindowLauncher_TestFixture(bool readOnly, params string[] databases)
            : base(readOnly, databases)
        { }

        [OneTimeSetUp]
        public void SetUp()
        {
            base._SetUp();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            base._TearDown();
        }
    }

    public abstract class MainWindowLauncher_SetUpFixture : MainWindowLauncher
    {
        public MainWindowLauncher_SetUpFixture(params string[] databases)
            : base(databases)
        { }

        [OneTimeSetUp]
        public void SetUp()
        {
            base._SetUp();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            base._TearDown();
        }
    }
}
