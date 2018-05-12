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

using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;

namespace XenAdminTests.SearchTests
{
    [SetUpFixture]
    public class SearchTestsSetup : MainWindowLauncher_SetUpFixture
    {
        public SearchTestsSetup()
            : this("state3.xml", "state.db", "xapidb_app.xml" , "tampa-db_inc_snapshots.xml")
        {}

        protected SearchTestsSetup(params string[] databases)
            : base(databases)
        { }

        [OneTimeSetUp]
        public void SwitchToSearchTab()
        {
            MainWindowWrapper mainWindowWrapper = new MainWindowWrapper(Program.MainWindow);

            // Select the XenCenter node
            VirtualTreeView treeView = mainWindowWrapper.TreeView;
            MW(delegate() { treeView.SelectedNode = treeView.Nodes[0]; });

            // Switch to the Search tab
            GoToTabPage(Program.MainWindow.TabPageSearch);
        }
    }
}
