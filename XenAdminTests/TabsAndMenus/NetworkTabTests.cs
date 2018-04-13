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

using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NetworkTabBoston : MainWindowLauncher_TestFixture
    {
        public NetworkTabBoston()
            : base("boston-db.xml")
        { }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
        }

        [Test, MaxTime(120000)]
        [Ignore("Ignore this test, because it takes too long.")]
        public void AddALotOfVIFsThenRemoveThemShouldNotThrowAnException()
        {
            const int MAX_VIFS_TO_ADD = 100;

            log.DebugFormat("Adding {0} VIFS and then removing them", MAX_VIFS_TO_ADD);
            
            //Add a lot of VIFs - use the default selection e.g. External network
            log.DebugFormat("Start adding VIFs");
            MW(
                delegate()
                    {
                        Host host = GetAnyHost(h => h.name_label == "inflames");
                        SelectInTree(host);
                        GoToTabPage(MainWindowWrapper.TabPageNetwork);
                        for (int i = 0; i < MAX_VIFS_TO_ADD; i++)
                        {
                            HandleModelessDialog<NewNetworkWizardWrapper>("NewNetworkWizard",
                                                                          MainWindowWrapper.NetworkPage.AddNetworkButton.PerformClick,
                                                                          w =>
                                                                              {
                                                                                  w.SSPNButton.PerformClick();
                                                                                  w.NextButton.PerformClick();
                                                                                  w.NextButton.PerformClick();
                                                                                  w.NextButton.PerformClick();
                                                                              });
                        }
                    }
                );
            
            //Remove all rows that can be removed i.e. all those expect original 2
            log.DebugFormat("Start removing VIFs");
            MW(
                delegate()
                    {
                        foreach (DataGridViewRow row in MainWindowWrapper.NetworkPage.NetworksGridView.Rows)
                        {
                            row.Selected = true;
                            Assert.IsTrue(row.Selected, "Row has been selected");
                            Thread.Sleep(100);
                            MainWindowWrapper.NetworkPage.RemoveNetworkButton.PerformClick();
                        }
                    }
                );
            log.DebugFormat("VIFs removed");
        }
    }
}
