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


using System.Linq;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Model;
using XenAdminTests.LicensingTests;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class MainMenuGeorge : TabsAndMenus
    {
        public MainMenuGeorge()
            : base("state1.xml")
        {
        }

        protected MainMenuGeorge(string db)
            : base(db)
        {
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            DisableAllPlugins();
        }

        [Test]
        public void MainMenu_XenCenterNode()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New Pool...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Add Server", false, false  ),
	            new ExpectedTextMenuItem("Re&move Server", false, false  ),
	            new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
	            new ExpectedTextMenuItem("Dis&connect", false, false  ),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Manage &vApps...", false, false ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&High Availability...", false, false  ),
				new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", false, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", false, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", false, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
	            new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", false, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
                new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New VM...", false, false  ),
	            new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                new ExpectedTextMenuItem("&Move VM...", false, false  ),
	            new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	            new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	            new ExpectedTextMenuItem("&Export...", false, false  ),
                new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", false),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            VerifyMainMenu(null, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
        }

        [Test]
        public void MainMenu_Pool()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New Pool...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	            new ExpectedTextMenuItem("Re&move Server", false, false  ),
	            new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
	            new ExpectedTextMenuItem("Dis&connect", true, false  ),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("Manage &vApps...", true, false ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&High Availability...", true, false  ),
				new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)}),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", false, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", false, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", false, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", true, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", true, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
                new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New VM...", true, false  ),
	            new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                new ExpectedTextMenuItem("&Move VM...", false, false  ),
	            new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	            new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	            new ExpectedTextMenuItem("&Export...", true, false  ),
                new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", false),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
                new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            VerifyMainMenu(GetAnyPool(), poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
        }

        [Test]
        public void MainMenu_Host()
        {
            foreach (Host host in GetAllXenObjects<Host>())
            {
                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	                new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
	                new ExpectedTextMenuItem("Dis&connect", true, false  ),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Manage &vApps...", true, false ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&High Availability...", true, false  ),
					new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Add...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Reb&oot", true, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
	                new ExpectedTextMenuItem("S&hut Down", true, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", true, false),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", host.IsMaster()),new ExpectedTextMenuItem("Reconnec&t As...", host.IsMaster()),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Back Up...", true, false  ),
	                new ExpectedTextMenuItem("Restore From Back&up...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Enter &Maintenance Mode...", true, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                    new ExpectedTextMenuItem("Pass&word", true, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", true, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("D&estroy", false, false  ),
                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, host.IsMaster(), false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New VM...", true, false  ),
	                new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	                new ExpectedSeparator(),
					new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                    new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                new ExpectedTextMenuItem("&Export...", true, false  ),
                    new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                    new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                    new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New SR...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Virtual Disks", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", false),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	                new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Detach...", false, false  ),
	                new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                new ExpectedTextMenuItem("&Forget", false, false  ),
	                new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                        new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                    new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                new ExpectedTextMenuItem("&Copy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                VerifyMainMenu(host, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
        }

        [Test]
        public void MainMenu_DefaultTemplate()
        {
            EnsureDefaultTemplatesShown();

            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New Pool...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	            new ExpectedTextMenuItem("Re&move Server", false, false  ),
	            new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
	            new ExpectedTextMenuItem("Dis&connect", true, false  ),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&High Availability...", true, false  ),
				new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
	            new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", false, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", false, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", false, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
	            new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New VM...", true, false  ),
	            new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
                new ExpectedSeparator(),
				new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                new ExpectedTextMenuItem("&Move VM...", false, false  ),
	            new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	            new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	            new ExpectedTextMenuItem("&Export...", false, false  ),
                new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", true)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", true, false, new ExpectedMenuItem[]{
                        new ExpectedTextMenuItem("&New VM wizard...", true, false  ),
	                    new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", true, false  ),
	            new ExpectedTextMenuItem("&Copy...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            VerifyMainMenu(GetAnyDefaultTemplate(t => t.name_label == "CentOS 4.5"), poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
        }

        [Test]
        public void MainMenu_UserTemplate()
        {
            EnsureDefaultTemplatesShown();

            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate() && !v.is_a_snapshot))
            {
                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	                new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
	                new ExpectedTextMenuItem("Dis&connect", true, false  ),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Manage &vApps...", true, false ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&High Availability...", true, false  ),
					new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Add...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Reb&oot", false, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
	                new ExpectedTextMenuItem("S&hut Down", false, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Enter &Maintenance Mode...", vm.Home() != null, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
	                new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("D&estroy", false, false  ),
	                new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", vm.Home() != null, false  )
                };

                ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New VM...", true, false  ),
	                new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", true),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	                new ExpectedSeparator(),
					new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                    new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                new ExpectedTextMenuItem("&Export...", false, false  ),
                    new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                    new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                    new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New SR...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", true)}  ),
	               // new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Detach...", false, false  ),
	                new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                new ExpectedTextMenuItem("&Forget", false, false  ),
	                new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("Create &VM From Selection", true, false, new ExpectedMenuItem[]{
                        new ExpectedTextMenuItem("&New VM wizard...", true, false  ),
	                    new ExpectedTextMenuItem("&Quick Create", vm.InstantTemplate(), false  )}  ),
                    new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Export to File...", true, false  ),
	                new ExpectedTextMenuItem("&Copy...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete Template...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                VerifyMainMenu(vm, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
        }

        [Test]
        public void MainMenu_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);

            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR()))
            {
                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	                new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
	                new ExpectedTextMenuItem("Dis&connect", true, false  ),
	                new ExpectedSeparator(),
					new ExpectedTextMenuItem("Manage &vApps...", true, false ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&High Availability...", true, false  ),
					new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Add...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Reb&oot", false, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
	                new ExpectedTextMenuItem("S&hut Down", false, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Enter &Maintenance Mode...", sr.Home() != null, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                    new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("D&estroy", false, false  ),
	                new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", sr.Home() != null, false  )
                };

                ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New VM...", true, false  ),
	                new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	                new ExpectedSeparator(),
					new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                    new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                new ExpectedTextMenuItem("&Export...", false, false  ),
                    new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                    new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                    new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New SR...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                new ExpectedTextMenuItem("Set as Defaul&t", CanSetAsDefault(sr), false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	               // new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Detach...", CanDetach(sr), false  ),
	                new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                new ExpectedTextMenuItem("&Forget", CanForget(sr), false  ),
	                new ExpectedTextMenuItem("Destr&oy...", CanDestroy(sr), false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                        new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                    new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                new ExpectedTextMenuItem("&Copy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                VerifyMainMenu(sr, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
        }

        [Test]
        public void MainMenu_Snapshot()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Add Server", false, false),
	                    new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                    new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
	                    new ExpectedTextMenuItem("Dis&connect", false, false  ),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Manage &vApps...", false, false ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&High Availability...", false, false  ),
						new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                        new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                        new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                        new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Add...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Reb&oot", false, false  ),
                        new ExpectedTextMenuItem("Power O&n", false, false  ),
	                    new ExpectedTextMenuItem("S&hut Down", false, false  ),
                        new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                    new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                    new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	                    new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                    new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                        new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                        new ExpectedTextMenuItem("&Change...", false, false  ),
                            new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("D&estroy", false, false  ),
	                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New VM...", false, false  ),
	                    new ExpectedTextMenuItem("&Start/Shut down", false, false),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                        new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                    new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                    new ExpectedTextMenuItem("&Export...", false, false  ),
                        new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                        new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                        new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                    new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New SR...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                    new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Virtual Disks", false, false),
	                //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                    new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                        new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Detach...", false, false  ),
	                    new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                    new ExpectedTextMenuItem("&Forget", false, false  ),
	                    new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                    new ExpectedTextMenuItem("&Copy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    VerifyMainMenu(snapshot, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void MainMenu_VDI()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Add Server", false, false),
                    new ExpectedTextMenuItem("Re&move Server", false, false  ),
                    new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
                    new ExpectedTextMenuItem("Dis&connect", false, false  ),
                    new ExpectedSeparator(),
					new ExpectedTextMenuItem("Manage &vApps...", false, false ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&High Availability...", false, false  ),
					new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&Add...", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Reb&oot", false, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
                    new ExpectedTextMenuItem("S&hut Down", false, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
                    new ExpectedTextMenuItem("Add to &Pool", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Back Up...", false, false  ),
                    new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                    new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("D&estroy", false, false  ),
                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New VM...", false, false  ),
                    new ExpectedTextMenuItem("&Start/Shut down", false, false),
                    new ExpectedSeparator(),
					new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                    new ExpectedTextMenuItem("&Move VM...", false, false  ),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
                    new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
                    new ExpectedTextMenuItem("&Export...", false, false  ),
                    new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                    new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                    new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
                    new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete VM...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New SR...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Re&pair...", false, false  ),
                    new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Virtual Disks", false, false),
  	                //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
                    new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Detach...", false, false  ),
                    new ExpectedTextMenuItem("R&eattach...", false, false  ),
                    new ExpectedTextMenuItem("&Forget", false, false  ),
                    new ExpectedTextMenuItem("Destr&oy...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", false, false  ),
                    new ExpectedTextMenuItem("&Copy...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                VerifyMainMenu(GetAnyVDI(v => v.name_label != "base copy"), poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void MainMenu_Network()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Guest installer network"))
                {
                    ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Add Server", false, false),
	                    new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                    new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
	                    new ExpectedTextMenuItem("Dis&connect", false, false  ),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Manage &vApps...", false, false ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&High Availability...", false, false  ),
						new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                        new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                        new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                        new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Add...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Reb&oot", false, false  ),
                        new ExpectedTextMenuItem("Power O&n", false, false  ),
	                    new ExpectedTextMenuItem("S&hut Down", false, false  ),
                        new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                    new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                    new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	                    new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                    new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                        new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                        new ExpectedTextMenuItem("&Change...", false, false  ),
                            new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("D&estroy", false, false  ),
	                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New VM...", false, false  ),
	                    new ExpectedTextMenuItem("&Start/Shut down", false, false),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                        new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                    new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                    new ExpectedTextMenuItem("&Export...", false, false  ),
                        new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                        new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                        new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                    new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New SR...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                    new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Virtual Disks", false, false),
	                   // new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                    new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                        new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Detach...", false, false  ),
	                    new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                    new ExpectedTextMenuItem("&Forget", false, false  ),
	                    new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                    new ExpectedTextMenuItem("&Copy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    VerifyMainMenu(network, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void MainMenu_GroupingTag()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Tags);
            try
            {
                VirtualTreeNode n = GetAllTreeNodes().Find(v => v.Tag is GroupingTag);

                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Add Server", false, false),
                    new ExpectedTextMenuItem("Re&move Server", false, false  ),
                    new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
                    new ExpectedTextMenuItem("Dis&connect", false, false  ),
                    new ExpectedSeparator(),
					new ExpectedTextMenuItem("Manage &vApps...", false, false ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&High Availability...", false, false  ),
					new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&Add...", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Reb&oot", false, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
                    new ExpectedTextMenuItem("S&hut Down", false, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
                    new ExpectedTextMenuItem("Add to &Pool", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Back Up...", false, false  ),
                    new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                    new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("D&estroy", false, false  ),
                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New VM...", false, false  ),
                    new ExpectedTextMenuItem("&Start/Shut down", false, false),
                    new ExpectedSeparator(),
					new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                    new ExpectedTextMenuItem("&Move VM...", false, false  ),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
                    new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
                    new ExpectedTextMenuItem("&Export...", false, false  ),
                    new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                    new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                    new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
                    new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete VM...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New SR...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Re&pair...", false, false  ),
                    new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Virtual Disks", false, false),
 	                //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
                    new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Detach...", false, false  ),
                    new ExpectedTextMenuItem("R&eattach...", false, false  ),
                    new ExpectedTextMenuItem("&Forget", false, false  ),
                    new ExpectedTextMenuItem("Destr&oy...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", false, false  ),
                    new ExpectedTextMenuItem("&Copy...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                VerifyMainMenu((GroupingTag)n.Tag, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void MainMenu_Folder()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Folders);
            try
            {
                var folders = GetAllXenObjects<Folder>().Where(f => !(string.IsNullOrEmpty(f.ToString())));

                foreach (Folder folder in folders)
                {
                    ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Add Server", false, false),
	                    new ExpectedTextMenuItem("Re&move Server", false, false  ),
	                    new ExpectedTextMenuItem("Reconnec&t As...", false, false  ),
	                    new ExpectedTextMenuItem("Dis&connect", false, false  ),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Manage &vApps...", false, false ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&High Availability...", false, false  ),
						new ExpectedTextMenuItem("Di&saster Recovery", false, false ),
                        new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                        new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                        new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", false, false  ),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Add...", true, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Reb&oot", false, false  ),
                        new ExpectedTextMenuItem("Power O&n", false, false  ),
	                    new ExpectedTextMenuItem("S&hut Down", false, false  ),
                        new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                    new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                    new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	                    new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                    new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                        new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                        new ExpectedTextMenuItem("&Change...", false, false  ),
                            new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                    new ExpectedSeparator(),
                        new ExpectedTextMenuItem("D&estroy", false, false  ),
	                    new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New VM...", false, false  ),
	                    new ExpectedTextMenuItem("&Start/Shut down", false, false),
	                    new ExpectedSeparator(),
						new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                        new ExpectedTextMenuItem("&Move VM...", false, false  ),
	                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	                    new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	                    new ExpectedTextMenuItem("&Export...", false, false  ),
                        new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                        new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                        new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	                    new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&New SR...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                    new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Virtual Disks", false, false),
	                    //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                    new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                        new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Detach...", false, false  ),
	                    new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                    new ExpectedTextMenuItem("&Forget", false, false  ),
	                    new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                   new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                    new ExpectedTextMenuItem("&Copy...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                    new ExpectedSeparator(),
	                    new ExpectedTextMenuItem("P&roperties", false, false  )
                    };

                    VerifyMainMenu(folder, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }


        [Test]
        public void MainMenu_Master()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                new ExpectedTextMenuItem("&New Pool...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
                new ExpectedTextMenuItem("Re&move Server", false, false  ),
                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
                new ExpectedTextMenuItem("Dis&connect", true, false  ),
                new ExpectedSeparator(),
				new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&High Availability...", true, false  ),
				new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", true, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", true, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", true, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", true),new ExpectedTextMenuItem("Reconnec&t As...", true),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", true, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", true, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", true, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", true, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
	            new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New VM...", true, false  ),
	            new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                new ExpectedTextMenuItem("&Move VM...", false, false  ),
	            new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	            new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	            new ExpectedTextMenuItem("&Export...", true, false  ),
                new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", false),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            VerifyMainMenu(GetAnyHost(IsMaster), poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
        }

        [Test]
        public void MainMenu_Slave()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                new ExpectedTextMenuItem("&New Pool...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
                new ExpectedTextMenuItem("Re&move Server", false, false  ),
                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
                new ExpectedTextMenuItem("Dis&connect", true, false  ),
                new ExpectedSeparator(),
				new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&High Availability...", true, false  ),
				new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", true, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", true, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", true, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", true, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", true, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", true, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", true, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
	            new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New VM...", true, false  ),
	            new ExpectedTextMenuItem("&Start/Shut down", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Start", false),new ExpectedTextMenuItem("S&uspend", false),new ExpectedTextMenuItem("Reb&oot", false),new ExpectedTextMenuItem("Start in Reco&very Mode", false),new ExpectedSeparator(),new ExpectedTextMenuItem("Force Shut&down", false),new ExpectedTextMenuItem("Force Re&boot", false  )}),
	            new ExpectedSeparator(),
				new ExpectedTextMenuItem("Assign to vA&pp", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Copy VM...", false, false  ),
                new ExpectedTextMenuItem("&Move VM...", false, false  ),
	            new ExpectedTextMenuItem("Ta&ke a Snapshot...", false, false  ),
	            new ExpectedTextMenuItem("Convert to &Template...", false, false  ),
	            new ExpectedTextMenuItem("&Export...", true, false  ),
                new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false  ),
	            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete VM...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", false),new ExpectedTextMenuItem("&Attach Virtual Disk...", false)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            VerifyMainMenu(GetAnyHost(t => t.name_label == "incubus" && !t.IsMaster()), poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
        }

        [Test]
        [Ignore("Ignore, as it currently fails because some additional members are added to the VMTooltripMenuItem on a separate thread (MigrateVMToolStripMenuItem.AddAdditionalMenuItems)." +
                " Same problem as with ContextMenu_VMWithTools,see CA-123967.")]
        public void MainMenu_VMWithTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(HasTools))
            {
                ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                    new ExpectedTextMenuItem("&New Pool...", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	                new ExpectedTextMenuItem("Re&move Server", false, false  ),
                    new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
                    new ExpectedTextMenuItem("Dis&connect", true, false  ),
                    new ExpectedSeparator(),
				    new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&High Availability...", true, false  ),
				    new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                    new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                    new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                    new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Add...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Reb&oot", false, false  ),
                    new ExpectedTextMenuItem("Power O&n", false, false  ),
	                new ExpectedTextMenuItem("S&hut Down", false, false  ),
                    new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	                new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Back Up...", false, false  ),
	                new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Enter &Maintenance Mode...", true, false  ),
	                new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	                new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                    new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                    new ExpectedTextMenuItem("&Change...", false, false  ),
                        new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	                new ExpectedSeparator(),
                    new ExpectedTextMenuItem("D&estroy", false, false  ),
	                new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", true, false  )
                };

                bool residentOnInflames = vm.Connection.Resolve<Host>(vm.resident_on).name_label == "inflames";

                ExpectedMenuItem[] VMToolStripMenuItem =
                    new ExpectedMenuItem[]
                        {
                            new ExpectedTextMenuItem("&New VM...", true, false),
                            new ExpectedTextMenuItem("&Start/Shut down", true, false,
                                                     new ExpectedMenuItem[]
                                                         {
                                                             new ExpectedTextMenuItem("S&hut Down", true),
                                                             new ExpectedTextMenuItem("S&uspend", true),
                                                             new ExpectedTextMenuItem("Reb&oot", true),
                                                             new ExpectedTextMenuItem("Start in Reco&very Mode", false),
                                                             new ExpectedSeparator(),
                                                             new ExpectedTextMenuItem("Force Shut &Down", true),
                                                             new ExpectedTextMenuItem("Force Re&boot", true),
                                                             new ExpectedSeparator(),
                                                             new ExpectedTextMenuItem("S&tart vApp", false),
                                                             new ExpectedTextMenuItem("Shut Dow&n vApp", false)
                                                         }),
                            new ExpectedTextMenuItem("M&igrate to Server", true, false,
                                 new ExpectedMenuItem[]
                                     {
                                         new ExpectedTextMenuItem("&Home Server (Current server)", false),
                                         residentOnInflames
                                             ? new ExpectedTextMenuItem("inflames (Current server)", false, false, true)
                                             : new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                         residentOnInflames
                                             ? new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                                             : new ExpectedTextMenuItem("incubus (Current server)", false, false, true),
                                         new ExpectedSeparator(),
                                         new ExpectedTextMenuItem("&Migrate VM wizard...", false, false)
                                     }),
                            new ExpectedSeparator(),
                            new ExpectedTextMenuItem("Assign to vA&pp", true, false, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }),
                            new ExpectedSeparator(),
                            new ExpectedTextMenuItem("&Copy VM...", false, false),
                            new ExpectedTextMenuItem("&Move VM...", false, false),
                            new ExpectedTextMenuItem("Ta&ke a Snapshot...", true, false),
                            new ExpectedTextMenuItem("Convert to &Template...", false, false),
                            new ExpectedTextMenuItem("&Export...", false, false),
                            new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                            new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                            new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
                            new ExpectedSeparator(),
                            new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false),
                            new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false),
                            new ExpectedSeparator(),
                            new ExpectedTextMenuItem("&Delete VM...", false, false),
                            new ExpectedSeparator(),
                            new ExpectedTextMenuItem("P&roperties", true, false)
                        };

                ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&New SR...", true, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Re&pair...", false, false  ),
	                new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", true)}  ),
	                //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	                new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                    new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Detach...", false, false  ),
	                new ExpectedTextMenuItem("R&eattach...", false, false  ),
	                new ExpectedTextMenuItem("&Forget", false, false  ),
	                new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Export to File...", false, false  ),
	                new ExpectedTextMenuItem("&Copy...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("P&roperties", false, false  )
                };

                VerifyMainMenu(vm, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }

        }

        [Test]
        public void MainMenu_VMWithoutTools()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
                new ExpectedTextMenuItem("&New Pool...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
                new ExpectedTextMenuItem("Re&move Server", false, false  ),
                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
                new ExpectedTextMenuItem("Dis&connect", true, false  ),
                new ExpectedSeparator(),
			    new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&High Availability...", true, false  ),
			    new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", false, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", false, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", false, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", true, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", false, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
	            new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem =
                new ExpectedMenuItem[]
                    {
                        new ExpectedTextMenuItem("&New VM...", true, false),
                        new ExpectedTextMenuItem("&Start/Shut down", true, false,
                                                 new ExpectedMenuItem[]
                                                     {
                                                         new ExpectedTextMenuItem("&Start", false),
                                                         new ExpectedTextMenuItem("S&uspend", false), new ExpectedTextMenuItem("Reb&oot", false),
                                                         new ExpectedTextMenuItem("Start in Reco&very Mode", false), new ExpectedSeparator(),
                                                         new ExpectedTextMenuItem("Force Shut &Down", true),
                                                         new ExpectedTextMenuItem("Force Re&boot", true),
                                                         new ExpectedSeparator(),
                                                         new ExpectedTextMenuItem("S&tart vApp", false),
                                                         new ExpectedTextMenuItem("Shut Dow&n vApp", false)
                                                     }),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Assign to vA&pp", true, false, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("&Copy VM...", false, false),
                        new ExpectedTextMenuItem("&Move VM...", false, false),
                        new ExpectedTextMenuItem("Ta&ke a Snapshot...", true, false),
                        new ExpectedTextMenuItem("Convert to &Template...", false, false),
                        new ExpectedTextMenuItem("&Export...", false, false),
                        new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                        new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                        new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", true, false),
                        new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("&Delete VM...", false, false),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("P&roperties", true, false)
                    };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", true)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            foreach (VM vm in GetAllXenObjects<VM>(NoTools))
            {
                VerifyMainMenu(vm, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
        }

        [Test]
        public void MainMenu_VMShutdown()
        {
            ExpectedMenuItem[] poolToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New Pool...", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Add Server", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Add New Server...", true)}     ),
	            new ExpectedTextMenuItem("Re&move Server", false, false  ),
                new ExpectedTextMenuItem("Reconnec&t As...", true, false  ),
                new ExpectedTextMenuItem("Dis&connect", true, false  ),
                new ExpectedSeparator(),
			    new ExpectedTextMenuItem("Manage &vApps...", true, false ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&High Availability...", true, false  ),
			    new ExpectedTextMenuItem("Di&saster Recovery", true, false, new ExpectedMenuItem[]{ new ExpectedTextMenuItem("&Configure...", true), new ExpectedTextMenuItem("&Disaster Recovery Wizard...", true)} ),
                new ExpectedTextMenuItem("E&xport Resource Data...", false, false  ),
                new ExpectedTextMenuItem("View Wor&kload Reports...", false, false  ),
                new ExpectedTextMenuItem("Disconnect Workload &Balancing Server", true, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Change Server Pass&word...", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Mak&e into standalone server", false, false  ),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true, false  )
            };

            ExpectedMenuItem[] HostMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&Add...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Reb&oot", false, false  ),
                new ExpectedTextMenuItem("Power O&n", false, false  ),
	            new ExpectedTextMenuItem("S&hut Down", false, false  ),
                new ExpectedTextMenuItem("Restart Toolstac&k", false, false),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Connect/Disconnect", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Connect", false),new ExpectedTextMenuItem("Dis&connect", false),new ExpectedTextMenuItem("Reconnec&t As...", false),new ExpectedSeparator(),new ExpectedTextMenuItem("C&onnect All", false), new ExpectedTextMenuItem("Di&sconnect All", true)}  ),
	            new ExpectedTextMenuItem("Add to &Pool", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Back Up...", false, false  ),
	            new ExpectedTextMenuItem("Restore From Back&up...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Enter &Maintenance Mode...", false, false  ),
	            new ExpectedTextMenuItem("Control &Domain Memory...", false, false  ),
	            new ExpectedTextMenuItem("Remove Crash Dump &Files", false, false  ),
                new ExpectedTextMenuItem("Pass&word", false, false, new ExpectedMenuItem[]{
	                new ExpectedTextMenuItem("&Change...", false, false  ),
                    new ExpectedTextMenuItem("&Forget Password", false, false  )}),
	            new ExpectedSeparator(),
                new ExpectedTextMenuItem("D&estroy", false, false  ),
	            new ExpectedTextMenuItem("Remo&ve from " + Branding.BRAND_CONSOLE, false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] VMToolStripMenuItem =
                new ExpectedMenuItem[]
                    {
                        new ExpectedTextMenuItem("&New VM...", true, false),
                        new ExpectedTextMenuItem("&Start/Shut down", true, false,
                                                 new ExpectedMenuItem[]
                                                     {
                                                         new ExpectedTextMenuItem("&Start", true),
                                                         new ExpectedTextMenuItem("S&uspend", false),
                                                         new ExpectedTextMenuItem("Reb&oot", false),
                                                         new ExpectedTextMenuItem("Start in Reco&very Mode", true),
                                                         new ExpectedSeparator(),
                                                         new ExpectedTextMenuItem("Force Shut &Down", false),
                                                         new ExpectedTextMenuItem("Force Re&boot", false),
                                                         new ExpectedSeparator(),
                                                         new ExpectedTextMenuItem("S&tart vApp", false),
                                                         new ExpectedTextMenuItem("Shut Dow&n vApp", false)
                                                     }),
                        new ExpectedTextMenuItem("Start on Ser&ver", true, false,
                                                 new ExpectedMenuItem[]
                                                     {
                                                         new ExpectedTextMenuItem("&Home Server (Home Server is not set)", false),
                                                         new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                                         new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                                                     }),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Assign to vA&pp", true, false, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New vApp...", true) }),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("&Copy VM...", true, false),
                        new ExpectedTextMenuItem("&Move VM...", false, false),
                        new ExpectedTextMenuItem("Ta&ke a Snapshot...", true, false),
                        new ExpectedTextMenuItem("Convert to &Template...", true, false),
                        new ExpectedTextMenuItem("&Export...", true, false),
                        new ExpectedTextMenuItem("D&isable Changed Block Tracking", false, false  ),
                        new ExpectedTextMenuItem("Ena&ble PVS-Accelerator...", false, false  ),
                        new ExpectedTextMenuItem("Disable P&VS-Accelerator", false, false  ),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("Inst&all " + Branding.PRODUCT_BRAND + " Tools...", false, false),
                        new ExpectedTextMenuItem("Send Ctrl+&Alt+Del", false, false),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("&Delete VM...", true, false),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("P&roperties", true, false)
                    };

            ExpectedMenuItem[] StorageToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("&New SR...", true, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Re&pair...", false, false  ),
	            new ExpectedTextMenuItem("Set as Defaul&t", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Virtual Disks", true, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("New &Virtual Disk...", true),new ExpectedTextMenuItem("&Attach Virtual Disk...", true)}  ),
	            //new ExpectedTextMenuItem("Storage&Link", false, false, new ExpectedMenuItem[]{new ExpectedTextMenuItem("&Change Server Password...", false),new ExpectedTextMenuItem("&Remove Servers...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Add Storage System...", false), new ExpectedTextMenuItem("R&emove Storage System...", false), new ExpectedSeparator(), new ExpectedTextMenuItem("&Destroy Storage Volume...", false)}  ),
	            new ExpectedTextMenuItem("Re&claim freed space", false, false  ),
                new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Detach...", false, false  ),
	            new ExpectedTextMenuItem("R&eattach...", false, false  ),
	            new ExpectedTextMenuItem("&Forget", false, false  ),
	            new ExpectedTextMenuItem("Destr&oy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            ExpectedMenuItem[] templatesToolStripMenuItem = new ExpectedMenuItem[]{
	            new ExpectedTextMenuItem("Create &VM From Selection", false, false, new ExpectedMenuItem[]{
                            new ExpectedTextMenuItem("&New VM wizard...", false, false  ),
	                        new ExpectedTextMenuItem("&Quick Create", false, false  )}  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Export to File...", false, false  ),
	            new ExpectedTextMenuItem("&Copy...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("&Delete Template...", false, false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("P&roperties", false, false  )
            };

            foreach (VM vm in GetAllXenObjects<VM>(IsShutdown))
            {
                VerifyMainMenu(vm, poolToolStripMenuItem, HostMenuItem, VMToolStripMenuItem, StorageToolStripMenuItem, templatesToolStripMenuItem);
            }
        }
    }
}
