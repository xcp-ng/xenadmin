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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Alerts;
using XenAdmin.Model;
using System.Drawing;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.XenSearch;
using System.IO;
using System.Linq;

namespace XenAdmin
{
    static class Images
    {
        public static readonly ImageList ImageList16 = new ImageList();
        /// <summary>
        /// This is the same images as in ImageList16, but in an array.
        /// Reading from the ImageList is strangely slow.
        /// </summary>
        private static readonly Image[] ImageArray16 = new Image[Enum.GetValues(typeof(Icons)).Length];

        static Images()
        {
            ImageList16.ColorDepth = ColorDepth.Depth32Bit;
            ImageList16.TransparentColor = Color.Transparent;

            // Initialize our ImageList. We do this programatically (as opposed to using the designer) because
            // of a known bug in ImageList with transparency. 
            // See http://www.codeproject.com/cs/miscctrl/AlphaImageImagelist.asp?df=100&forumid=137678&exp=0&select=1392020#xx1392020xx
            // To workaround the bug, we just add the images from the resource file.
            // Note that this list is in the same order as the enum in Icons.cs.
            ImageList16.Images.Add("Logo.png", XenAdmin.Properties.Resources.Logo);
            ImageList16.Images.Add("000_ServerInProgress_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerInProgress_h32bit_16);
            ImageList16.Images.Add("000_TreeConnected_h32bit_16.png", XenAdmin.Properties.Resources._000_TreeConnected_h32bit_16);
            ImageList16.Images.Add("000_ServerDisconnected_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerDisconnected_h32bit_16);
            ImageList16.Images.Add("000_ServerMaintenance_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerMaintenance_h32bit_16);
            ImageList16.Images.Add("000_HostUnpatched_h32bit_16.png", XenAdmin.Properties.Resources._000_HostUnpatched_h32bit_16);
            ImageList16.Images.Add("server_up_16.png", XenAdmin.Properties.Resources.server_up_16);
            ImageList16.Images.Add("000_ServerErrorFile_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerErrorFile_h32bit_16);
            ImageList16.Images.Add("000_StartVM_h32bit_16.png", XenAdmin.Properties.Resources._000_StartVM_h32bit_16);
            ImageList16.Images.Add("000_VMDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMDisabled_h32bit_16);
            ImageList16.Images.Add("000_StoppedVM_h32bit_16.png", XenAdmin.Properties.Resources._000_StoppedVM_h32bit_16);
            ImageList16.Images.Add("000_VMStoppedDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStoppedDisabled_h32bit_16);
            ImageList16.Images.Add("000_SuspendVM_h32bit_16.png", XenAdmin.Properties.Resources._000_SuspendVM_h32bit_16);
            ImageList16.Images.Add("000_VMPausedDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMPausedDisabled_h32bit_16);
            ImageList16.Images.Add("000_VMStarting_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStarting_h32bit_16);
            ImageList16.Images.Add("000_VMStartingDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStartingDisabled_h32bit_16);
            ImageList16.Images.Add("000_VMTemplate_h32bit_16.png", XenAdmin.Properties.Resources._000_VMTemplate_h32bit_16);
            ImageList16.Images.Add("000_TemplateDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_TemplateDisabled_h32bit_16);
            ImageList16.Images.Add("000_UserTemplate_h32bit_16.png", XenAdmin.Properties.Resources._000_UserTemplate_h32bit_16);
            ImageList16.Images.Add("000_VMSession_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSession_h32bit_16);
            ImageList16.Images.Add("000_VMSnapShotDiskOnly_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSnapShotDiskOnly_h32bit_16);
            ImageList16.Images.Add("000_VMSnapshotDiskMemory_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSnapshotDiskMemory_h32bit_16);
            ImageList16.Images.Add("_000_ScheduledVMsnapshotDiskOnly_h32bit_16.png", XenAdmin.Properties.Resources._000_ScheduledVMsnapshotDiskOnly_h32bit_16);
            ImageList16.Images.Add("_000_ScheduledVMsnapshotDiskMemory_h32bit_16.png", XenAdmin.Properties.Resources._000_ScheduledVMsnapshotDiskMemory_h32bit_16);
            ImageList16.Images.Add("000_PoolConnected_h32bit_16.png", XenAdmin.Properties.Resources._000_PoolConnected_h32bit_16);
            ImageList16.Images.Add("pool_up_16.png", XenAdmin.Properties.Resources.pool_up_16);

            ImageList16.Images.Add("000_Pool_h32bit_16-w-alert.png", Properties.Resources._000_Pool_h32bit_16_w_alert);
            ImageList16.Images.Add("000_Server_h32bit_16-w-alert.png", Properties.Resources._000_Server_h32bit_16_w_alert);

            ImageList16.Images.Add("000_Storage_h32bit_16.png", XenAdmin.Properties.Resources._000_Storage_h32bit_16);
            ImageList16.Images.Add("000_StorageBroken_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageBroken_h32bit_16);
            ImageList16.Images.Add("000_StorageDefault_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageDefault_h32bit_16);
            ImageList16.Images.Add("000_StorageDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageDisabled_h32bit_16);

            ImageList16.Images.Add("001_ShutDown_h32bit_16.png", Properties.Resources._001_ShutDown_h32bit_16);
            ImageList16.Images.Add("000_paused_h32bit_16.png", Properties.Resources._000_paused_h32bit_16);
            ImageList16.Images.Add("001_PowerOn_h32bit_16.png", Properties.Resources._001_PowerOn_h32bit_16);
            ImageList16.Images.Add("000_HelpIM_h32bit_16.png", Properties.Resources._000_HelpIM_h32bit_16);

            ImageList16.Images.Add("000_Network_h32bit_16.png", Properties.Resources._000_Network_h32bit_16);
            ImageList16.Images.Add("000_defaultSpyglass_h32bit_16.png", Properties.Resources._000_defaultSpyglass_h32bit_16);
            ImageList16.Images.Add("000_Search_h32bit_16.png", Properties.Resources._000_Search_h32bit_16);

            #region Server message images

            ImageList16.Images.Add("001_PowerOn_h32bit_16.png", Properties.Resources._001_PowerOn_h32bit_16);
            ImageList16.Images.Add("001_ShutDown_h32bit_16.png", Properties.Resources._001_ShutDown_h32bit_16);
            ImageList16.Images.Add("001_Reboot_h32bit_16.png", Properties.Resources._001_Reboot_h32bit_16);
            ImageList16.Images.Add("000_paused_h32bit_16.png", Properties.Resources._000_paused_h32bit_16);
            ImageList16.Images.Add("000_Resumed_h32bit_16.png", Properties.Resources._000_Resumed_h32bit_16);
            ImageList16.Images.Add("clonevm_16.png", Properties.Resources.clonevm_16);

            ImageList16.Images.Add("Logo.png", Properties.Resources.Logo);

            ImageList16.Images.Add("alert1_16.png", Properties.Resources.alert1_16);
            ImageList16.Images.Add("alert2_16.png", Properties.Resources.alert2_16);
            ImageList16.Images.Add("alert3_16.png", Properties.Resources.alert3_16);
            ImageList16.Images.Add("alert4_16.png", Properties.Resources.alert4_16);
            ImageList16.Images.Add("alert5_16.png", Properties.Resources.alert5_16);

            #endregion

            ImageList16.Images.Add("centos_16x.png", Properties.Resources.centos_16x);
            ImageList16.Images.Add("debian_16x.png", Properties.Resources.debian_16x);
            ImageList16.Images.Add("linx_16x.png", Properties.Resources.linx_16x);
            ImageList16.Images.Add("oracle_16x.png", Properties.Resources.oracle_16x);
            ImageList16.Images.Add("redhat_16x.png", Properties.Resources.redhat_16x);
            ImageList16.Images.Add("suse_16x.png", Properties.Resources.suse_16x);
            ImageList16.Images.Add("ubuntu_16x.png", Properties.Resources.ubuntu_16x);
            ImageList16.Images.Add("yinhekylin_16x.png", Properties.Resources.yinhekylin_16x);
            ImageList16.Images.Add("scilinux_16x.png", Properties.Resources.scilinux_16x);
            ImageList16.Images.Add("neokylin_16x.png", Properties.Resources.neokylin_16x);
            ImageList16.Images.Add("asianux_16x.png", Properties.Resources.asianux_16x);
            ImageList16.Images.Add("turbo_16x.png", Properties.Resources.turbo_16x);
            ImageList16.Images.Add("windows_h32bit_16.png", Properties.Resources.windows_h32bit_16);
            ImageList16.Images.Add("coreos-globe-icon.png", Properties.Resources.coreos_globe_icon);

            ImageList16.Images.Add("tools_uptodate_16x.png", Properties.Resources.tools_uptodate_16x);
            ImageList16.Images.Add("tools_notinstalled_16x.png", Properties.Resources.tools_notinstalled_16x);
            ImageList16.Images.Add("tools_outofdate_16x.png", Properties.Resources.tools_outofdate_16x);

            ImageList16.Images.Add("000_VM_h32bit_16.png", Properties.Resources._000_VM_h32bit_16);
            ImageList16.Images.Add("000_Server_h32bit_16.png", Properties.Resources._000_Server_h32bit_16);
            ImageList16.Images.Add("000_Pool_h32bit_16.png", Properties.Resources._000_Pool_h32bit_16);

            ImageList16.Images.Add("000_VirtualStorage_h32bit_16.png", Properties.Resources._000_VirtualStorage_h32bit_16);
            ImageList16.Images.Add("virtualstorage_snapshot_16.png", Properties.Resources.virtualstorage_snapshot_16);

            ImageList16.Images.Add("000_Folder_open_h32bit_16.png", Properties.Resources._000_Folder_open_h32bit_16);
            ImageList16.Images.Add("folder_grey.png", Properties.Resources.folder_grey);

            ImageList16.Images.Add("000_Tag_h32bit_16.png", Properties.Resources._000_Tag_h32bit_16);
            ImageList16.Images.Add("000_Fields_h32bit_16.png", Properties.Resources._000_Fields_h32bit_16);
            ImageList16.Images.Add("ha_16.png", Properties.Resources.ha_16);

            ImageList16.Images.Add("virtualappliance_16.png", Properties.Resources._000_VirtualAppliance_h32bit_16);

            ImageList16.Images.Add("000_MigrateVM_h32bit_16.png", Properties.Resources._000_MigrateVM_h32bit_16);
            ImageList16.Images.Add("000_MigrateStoppedVM_h32bit_16.png", Properties.Resources._000_MigrateStoppedVM_h32bit_16);
            ImageList16.Images.Add("000_MigrateSuspendedVM_h32bit_16.png", Properties.Resources._000_MigrateSuspendedVM_h32bit_16);

            ImageList16.Images.Add("_000_ManagementInterface_h32bit_16.png", Properties.Resources._000_ManagementInterface_h32bit_16);
            ImageList16.Images.Add("000_TCP_IPGroup_h32bit_16.png", Properties.Resources._000_TCP_IPGroup_h32bit_16);
            ImageList16.Images.Add("infra_view_16_textured.png", Properties.Resources.infra_view_16_textured);
            ImageList16.Images.Add("objects_16_textured.png", Properties.Resources.objects_16_textured);

            ImageList16.Images.Add("000_Sites_h32bit_16.png", Properties.Resources._000_Sites_h32bit_16);

            ImageList16.Images.Add("RunningDC_16.png", Properties.Resources.RunningDC_16);
            ImageList16.Images.Add("StoppedDC_16.png", Properties.Resources.StoppedDC_16);
            ImageList16.Images.Add("PausedDC_16.png", Properties.Resources.PausedDC_16);

            #region Status Icons
            ImageList16.Images.Add("000_Tick_h32bit_16", Properties.Resources._000_Tick_h32bit_16); //Ok
            ImageList16.Images.Add("000_Info3_h32bit_16.png", Properties.Resources._000_Info3_h32bit_16); //Info
            ImageList16.Images.Add("000_Alert2_h32bit_16.png", Properties.Resources._000_Alert2_h32bit_16); //Warning
            ImageList16.Images.Add("000_Abort_h32bit_16.png", Properties.Resources._000_Abort_h32bit_16); //Error
            #endregion

            System.Diagnostics.Trace.Assert(ImageList16.Images.Count == Enum.GetValues(typeof(Icons)).Length,
                "Programmer error - you must add an entry to the image list when you add a new icon to the enum");

            int i = 0;
            foreach (Image im in ImageList16.Images)
            {
                ImageArray16[i++] = im;
            }
        }

        public static Image GetImage16For(IXenConnection connection)
        {
            Icons icon = GetIconFor(connection);
            return ImageArray16[(int)icon];
        }



        public static Image GetImage16For(IXenObject o)
        {
            Icons icon = GetIconFor(o);
            return ImageArray16[(int)icon];
        }

        public static Image GetImage16For(Search search)
        {
            return GetImage16For(GetIconFor(search));
        }

        public static Image GetImage16For(XenAPI.Message.MessageType type)
        {
            return GetImage16For(GetIconFor(type));
        }

        public static Image GetImage16For(AlertPriority priority)
        {
            var icon = GetIconFor(priority);
            return icon == Icons.MessageUnknown ? null : GetImage16For(icon);
        }

        public static Image GetImage32For(IXenObject o)
        {
            return new Bitmap(GetImage16For(o), 32, 32);
        }

        public static Image GetImage16For(Icons icon)
        {
            return ImageArray16[(int)icon];
        }



        public static Icons GetIconFor(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null)
                return GetIconFor(vm);

            VM_appliance appl = o as VM_appliance;
            if (appl != null)
                return GetIconFor(appl);

            SR sr = o as SR;
            if (sr != null)
                return GetIconFor(sr);

            Host host = o as Host;
            if (host != null)
                return GetIconFor(host);

            Pool pool = o as Pool;
            if (pool != null)
                return GetIconFor(pool);

            XenAPI.Network network = o as XenAPI.Network;
            if (network != null)
                return GetIconFor(network);

            VDI vdi = o as VDI;
            if (vdi != null)
                return GetIconFor(vdi);

            VBD vbd = o as VBD;
            if (vbd != null)
                return GetIconFor(vbd);

            Folder folder = o as Folder;
            if (folder != null)
                return GetIconFor(folder);

            PIF pif = o as PIF;
            if (pif != null)
                return GetIconFor(pif);

            DockerContainer dockerContainer = o as DockerContainer;
            if (dockerContainer != null)
                return GetIconFor(dockerContainer);

            System.Diagnostics.Trace.Assert(false,
                "You asked for an icon for a type I don't recognise!");

            return Icons.XenCenter;
        }

        public static Icons GetIconFor(Search search)
        {
            return search.DefaultSearch ? Icons.DefaultSearch : Icons.Search;
        }

        public static Icons GetIconFor(IXenConnection connection)
        {
            Pool pool = Helpers.GetPool(connection);
            if (pool != null)
                return GetIconFor(pool);

            Host host = Helpers.GetMaster(connection);
            if (host != null)
                return GetIconFor(host);

            if (connection.InProgress)
                // Yellow connection in progress icon
                return Icons.HostConnecting;
            else
                // Red disconnected icon
                return Icons.HostDisconnected;
        }

        public static Icons GetIconFor(Folder folder)
        {
            return folder.Grey ? Icons.FolderGrey : Icons.Folder;
        }

        public static Icons GetIconFor(VM_appliance appl)
        {
            return Icons.VmAppliance;
        }

        public static Icons GetIconFor(VM vm)
        {
            bool disabled = vm.IsHidden();

            if (vm.is_a_snapshot)
            {
                if (vm.is_snapshot_from_vmpp || vm.is_vmss_snapshot)
                {
                    if (vm.power_state == vm_power_state.Suspended)
                        return Icons.ScheduledSnapshotDiskMemory;
                    else
                        return Icons.ScheduledSnapshotDiskOnly;
                }
                else
                {
                    if (vm.power_state == vm_power_state.Suspended)
                        return Icons.SnapshotWithMem;
                    else
                        return Icons.SnapshotDisksOnly;
                }
            }

            if (vm.is_a_template && vm.DefaultTemplate())
                return disabled ? Icons.TemplateDisabled : Icons.Template;

            if (vm.is_a_template && !vm.DefaultTemplate())
                return disabled ? Icons.TemplateDisabled : Icons.TemplateUser;

            if (!vm.ExistsOnServer())
                return disabled ? Icons.VmStoppedDisabled : Icons.VmStopped;

            if (vm.current_operations.ContainsValue(vm_operations.migrate_send))
            {
                switch (vm.power_state)
                {
                    case vm_power_state.Halted:
                        return Icons.VmCrossPoolMigrateStopped;
                    case vm_power_state.Suspended:
                    case vm_power_state.Paused:
                        return Icons.VmCrossPoolMigrateSuspended;
                    default:
                        return Icons.VmCrossPoolMigrate;
                }
            }

            // If a VM lifecycle operation is in progress, show the orange "starting" icon
            foreach (vm_operations op in vm.current_operations.Values)
            {
                if (VM.is_lifecycle_operation(op))
                    return disabled ? Icons.VmStartingDisabled : Icons.VmStarting;
            }

            switch (vm.power_state)
            {
                case XenAPI.vm_power_state.Suspended:
                    return disabled ? Icons.VmSuspendedDisabled : Icons.VmSuspended;
                case XenAPI.vm_power_state.Running:
                    return disabled ? Icons.VmRunningDisabled : Icons.VmRunning;
                case XenAPI.vm_power_state.Paused:
                    return disabled ? Icons.VmSuspendedDisabled : Icons.VmSuspended;
                default:
                    return disabled ? Icons.VmStoppedDisabled : Icons.VmStopped;
            }
        }

        public static Icons GetIconFor(SR sr)
        {
            if (!sr.HasPBDs() || sr.IsHidden())
            {
                return Icons.StorageDisabled;
            }
            else if (sr.IsDetached() || sr.IsBroken() || !sr.MultipathAOK())
            {
                return Icons.StorageBroken;
            }
            else if (SR.IsDefaultSr(sr))
            {
                return Icons.StorageDefault;
            }
            else
            {
                return Icons.Storage;
            }
        }

        public static Icons GetIconFor(Host host)
        {

            Host_metrics metrics = host.Connection.Resolve<Host_metrics>(host.metrics);
            bool host_is_live = metrics != null && metrics.live;

            if (host_is_live)
            {
                if (host.IsExpired())
                {
                    return Icons.ServerUnlicensed;
                }
                if (host.HasCrashDumps())
                {
                    return Icons.HostHasCrashDumps;
                }
                if (host.current_operations.ContainsValue(host_allowed_operations.evacuate)
                    || !host.enabled)
                {
                    return Icons.HostEvacuate;
                }
                else if (Helpers.IsOlderThanMaster(host))
                {
                    return Icons.HostOlderThanMaster;
                }
                else
                {
                    return Icons.HostConnected;
                }
            }
            else
            {
                // XenAdmin.XenSearch.Group puts a fake host in the treeview for disconnected
                // XenConnections. So here we give the yellow 'connection in progress' icon which formerly
                // applied only to XenConnections.
                if (host.Connection.InProgress && !host.Connection.IsConnected)
                    return Icons.HostConnecting;
                else
                    return Icons.HostDisconnected;
            }
        }


        public static Icons GetIconFor(Pool pool)
        {
            if (!pool.Connection.IsConnected)
                return Icons.HostDisconnected;
            if (pool.Connection.Cache.Hosts.Any(h => h.IsExpired()))
                return Icons.PoolUnlicensed;
            if (pool.IsPoolFullyUpgraded())
                return Icons.PoolConnected;
            return Icons.PoolNotFullyUpgraded;
        }

        public static Icons GetIconFor(XenAPI.Network network)
        {
            return Icons.Network;
        }

        public static Icons GetIconFor(VDI vdi)
        {
            if (vdi.is_a_snapshot)
                return Icons.VDISnapshot;
            else
                return Icons.VDI;
        }

        public static Icons GetIconFor(VBD vbd)
        {
            return Icons.VDI;
        }


        public static Icons GetIconFor(XenAPI.Message.MessageType type)
        {
            switch (type)
            {
                case XenAPI.Message.MessageType.VM_STARTED:
                    return Icons.VmStart;

                case XenAPI.Message.MessageType.VM_SHUTDOWN:
                    return Icons.VmShutdown;

                case XenAPI.Message.MessageType.VM_REBOOTED:
                    return Icons.VmReboot;

                case XenAPI.Message.MessageType.VM_SUSPENDED:
                    return Icons.VmSuspend;

                case XenAPI.Message.MessageType.VM_RESUMED:
                    return Icons.VmResumed;

                case XenAPI.Message.MessageType.VM_CLONED:
                    return Icons.VmCloned;

                default:
                    return Icons.MessageUnknown;
            }
        }

        public static Icons GetIconFor(AlertPriority priority)
        {
            switch (priority)
            {
                case AlertPriority.Priority1:
                    return Icons.MessagePriority1;
                case AlertPriority.Priority2:
                    return Icons.MessagePriority2;
                case AlertPriority.Priority3:
                    return Icons.MessagePriority3;
                case AlertPriority.Priority4:
                    return Icons.MessagePriority4;
                case AlertPriority.Priority5:
                    return Icons.MessagePriority5;
                default:
                    return Icons.MessageUnknown;
            }
        }

        public static void GenImageHTML()
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter("images.htm");
            tw.WriteLine("<html><head><title>XenCenter Images</title></head>");

            tw.WriteLine("<body><table>");

            foreach (Icons icon in Enum.GetValues(typeof(Icons)))
            {
                tw.WriteLine("<tr><td>{0}</td><td><image src='{1}'></td></tr>", icon, ImageList16.Images.Keys[(int)icon]);
            }

            tw.WriteLine("</table></body></html>");

            // close the stream
            tw.Close();
        }

        public static Icons GetIconFor(PIF pif)
        {
            return pif.IsPrimaryManagementInterface() ? Icons.PifPrimary : Icons.PifSecondary;
        }

        public static Icons GetIconFor(DockerContainer dockerContainer)
        {
            switch (dockerContainer.power_state)
            {
                case vm_power_state.Paused:
                    return Icons.DCPaused;
                case vm_power_state.Running:
                    return Icons.DCRunning;
                default:
                    return Icons.DCStopped;
            }
        }

        public static class StaticImages
        {
            // the followings are generated from Resources using:
            // cat Resources.Designer.cs | grep 'internal static System.Drawing.Bitmap' | sed "s/ {//g" | awk -F"Bitmap " '{print"public static Image " $2 " = Properties.Resources." $2 ";"}'
            public static Image _000_Abort_h32bit_16 = Properties.Resources._000_Abort_h32bit_16;
            public static Image _000_AddApplicationServer_h32bit_16 = Properties.Resources._000_AddApplicationServer_h32bit_16;
            public static Image _000_AddApplicationServer_h32bit_24 = Properties.Resources._000_AddApplicationServer_h32bit_24;
            public static Image _000_AddIPAddress_h32bit_16 = Properties.Resources._000_AddIPAddress_h32bit_16;
            public static Image _000_AddSite_h32bit_16 = Properties.Resources._000_AddSite_h32bit_16;
            public static Image _000_Alert2_h32bit_16 = Properties.Resources._000_Alert2_h32bit_16;
            public static Image _000_BackupMetadata_h32bit_16 = Properties.Resources._000_BackupMetadata_h32bit_16;
            public static Image _000_BackupMetadata_h32bit_32 = Properties.Resources._000_BackupMetadata_h32bit_32;
            public static Image _000_ConfigureIPAddresses_h32bit_16 = Properties.Resources._000_ConfigureIPAddresses_h32bit_16;
            public static Image _000_CPU_h32bit_16 = Properties.Resources._000_CPU_h32bit_16;
            public static Image _000_CreateVirtualStorage_h32bit_32 = Properties.Resources._000_CreateVirtualStorage_h32bit_32;
            public static Image _000_CreateVM_h32bit_24 = Properties.Resources._000_CreateVM_h32bit_24;
            public static Image _000_CreateVM_h32bit_32 = Properties.Resources._000_CreateVM_h32bit_32;
            public static Image _000_defaultSpyglass_h32bit_16 = Properties.Resources._000_defaultSpyglass_h32bit_16;
            public static Image _000_DeleteAllMessages_h32bit_16 = Properties.Resources._000_DeleteAllMessages_h32bit_16;
            public static Image _000_DeleteMessage_h32bit_16 = Properties.Resources._000_DeleteMessage_h32bit_16;
            public static Image _000_DeleteVirtualAppliance_h32bit_16 = Properties.Resources._000_DeleteVirtualAppliance_h32bit_16;
            public static Image _000_DisasterRecovery_h32bit_32 = Properties.Resources._000_DisasterRecovery_h32bit_32;
            public static Image _000_Email_h32bit_16 = Properties.Resources._000_Email_h32bit_16;
            public static Image _000_EnablePowerControl_h32bit_16 = Properties.Resources._000_EnablePowerControl_h32bit_16;
            public static Image _000_error_h32bit_16 = Properties.Resources._000_error_h32bit_16;
            public static Image _000_error_h32bit_32 = Properties.Resources._000_error_h32bit_32;
            public static Image _000_ExcludeHost_h32bit_16 = Properties.Resources._000_ExcludeHost_h32bit_16;
            public static Image _000_ExportMessages_h32bit_16 = Properties.Resources._000_ExportMessages_h32bit_16;
            public static Image _000_ExportVirtualAppliance_h32bit_16 = Properties.Resources._000_ExportVirtualAppliance_h32bit_16;
            public static Image _000_ExportVirtualAppliance_h32bit_32 = Properties.Resources._000_ExportVirtualAppliance_h32bit_32;
            public static Image _000_Failback_h32bit_32 = Properties.Resources._000_Failback_h32bit_32;
            public static Image _000_Failover_h32bit_32 = Properties.Resources._000_Failover_h32bit_32;
            public static Image _000_Fields_h32bit_16 = Properties.Resources._000_Fields_h32bit_16;
            public static Image _000_FilterDates_h32bit_16 = Properties.Resources._000_FilterDates_h32bit_16;
            public static Image _000_FilterServer_h32bit_16 = Properties.Resources._000_FilterServer_h32bit_16;
            public static Image _000_FilterSeverity_h32bit_16 = Properties.Resources._000_FilterSeverity_h32bit_16;
            public static Image _000_Folder_open_h32bit_16 = Properties.Resources._000_Folder_open_h32bit_16;
            public static Image _000_GetMemoryInfo_h32bit_16 = Properties.Resources._000_GetMemoryInfo_h32bit_16;
            public static Image _000_GetMemoryInfo_h32bit_32 = Properties.Resources._000_GetMemoryInfo_h32bit_32;
            public static Image _000_GetServerReport_h32bit_16 = Properties.Resources._000_GetServerReport_h32bit_16;
            public static Image _000_GetServerReport_h32bit_32 = Properties.Resources._000_GetServerReport_h32bit_32;
            public static Image _000_HAServer_h32bit_32 = Properties.Resources._000_HAServer_h32bit_32;
            public static Image _000_HelpIM_h32bit_16 = Properties.Resources._000_HelpIM_h32bit_16;
            public static Image _000_HelpIM_h32bit_32 = Properties.Resources._000_HelpIM_h32bit_32;
            public static Image _000_HighlightVM_h32bit_24 = Properties.Resources._000_HighlightVM_h32bit_24;
            public static Image _000_HighLightVM_h32bit_32 = Properties.Resources._000_HighLightVM_h32bit_32;
            public static Image _000_host_0_star = Properties.Resources._000_host_0_star;
            public static Image _000_host_1_star = Properties.Resources._000_host_1_star;
            public static Image _000_host_10_star = Properties.Resources._000_host_10_star;
            public static Image _000_host_2_star = Properties.Resources._000_host_2_star;
            public static Image _000_host_3_star = Properties.Resources._000_host_3_star;
            public static Image _000_host_4_star = Properties.Resources._000_host_4_star;
            public static Image _000_host_5_star = Properties.Resources._000_host_5_star;
            public static Image _000_host_6_star = Properties.Resources._000_host_6_star;
            public static Image _000_host_7_star = Properties.Resources._000_host_7_star;
            public static Image _000_host_8_star = Properties.Resources._000_host_8_star;
            public static Image _000_host_9_star = Properties.Resources._000_host_9_star;
            public static Image _000_HostUnpatched_h32bit_16 = Properties.Resources._000_HostUnpatched_h32bit_16;
            public static Image _000_ImportVirtualAppliance_h32bit_16 = Properties.Resources._000_ImportVirtualAppliance_h32bit_16;
            public static Image _000_ImportVirtualAppliance_h32bit_32 = Properties.Resources._000_ImportVirtualAppliance_h32bit_32;
            public static Image _000_ImportVM_h32bit_32 = Properties.Resources._000_ImportVM_h32bit_32;
            public static Image _000_Info3_h32bit_16 = Properties.Resources._000_Info3_h32bit_16;
            public static Image _000_ManagementInterface_h32bit_16 = Properties.Resources._000_ManagementInterface_h32bit_16;
            public static Image _000_MigrateStoppedVM_h32bit_16 = Properties.Resources._000_MigrateStoppedVM_h32bit_16;
            public static Image _000_MigrateSuspendedVM_h32bit_16 = Properties.Resources._000_MigrateSuspendedVM_h32bit_16;
            public static Image _000_MigrateVM_h32bit_16 = Properties.Resources._000_MigrateVM_h32bit_16;
            public static Image _000_MigrateVM_h32bit_32 = Properties.Resources._000_MigrateVM_h32bit_32;
            public static Image _000_Module_h32bit_16 = Properties.Resources._000_Module_h32bit_16;
            public static Image _000_Network_h32bit_16 = Properties.Resources._000_Network_h32bit_16;
            public static Image _000_NewNetwork_h32bit_32 = Properties.Resources._000_NewNetwork_h32bit_32;
            public static Image _000_NewStorage_h32bit_16 = Properties.Resources._000_NewStorage_h32bit_16;
            public static Image _000_NewStorage_h32bit_24 = Properties.Resources._000_NewStorage_h32bit_24;
            public static Image _000_NewStorage_h32bit_32 = Properties.Resources._000_NewStorage_h32bit_32;
            public static Image _000_NewVirtualAppliance_h32bit_16 = Properties.Resources._000_NewVirtualAppliance_h32bit_16;
            public static Image _000_NewVirtualAppliance_h32bit_32 = Properties.Resources._000_NewVirtualAppliance_h32bit_32;
            public static Image _000_Optimize_h32bit_16 = Properties.Resources._000_Optimize_h32bit_16;
            public static Image _000_Patch_h32bit_16 = Properties.Resources._000_Patch_h32bit_16;
            public static Image _000_Patch_h32bit_32 = Properties.Resources._000_Patch_h32bit_32;
            public static Image _000_paused_h32bit_16 = Properties.Resources._000_paused_h32bit_16;
            public static Image _000_Paused_h32bit_24 = Properties.Resources._000_Paused_h32bit_24;
            public static Image _000_Pool_h32bit_16 = Properties.Resources._000_Pool_h32bit_16;
            public static Image _000_Pool_h32bit_16_w_alert = Properties.Resources._000_Pool_h32bit_16_w_alert;
            public static Image _000_PoolConnected_h32bit_16 = Properties.Resources._000_PoolConnected_h32bit_16;
            public static Image _000_PoolNew_h32bit_16 = Properties.Resources._000_PoolNew_h32bit_16;
            public static Image _000_PoolNew_h32bit_24 = Properties.Resources._000_PoolNew_h32bit_24;
            public static Image _000_RebootVM_h32bit_16 = Properties.Resources._000_RebootVM_h32bit_16;
            public static Image _000_RemoveIPAddress_h32bit_16 = Properties.Resources._000_RemoveIPAddress_h32bit_16;
            public static Image _000_RemoveSite_h32bit_16 = Properties.Resources._000_RemoveSite_h32bit_16;
            public static Image _000_Resumed_h32bit_16 = Properties.Resources._000_Resumed_h32bit_16;
            public static Image _000_Resumed_h32bit_24 = Properties.Resources._000_Resumed_h32bit_24;
            public static Image _000_ScheduledVMsnapshotDiskMemory_h32bit_16 = Properties.Resources._000_ScheduledVMsnapshotDiskMemory_h32bit_16;
            public static Image _000_ScheduledVMSnapshotDiskMemory_h32bit_32 = Properties.Resources._000_ScheduledVMSnapshotDiskMemory_h32bit_32;
            public static Image _000_ScheduledVMsnapshotDiskOnly_h32bit_16 = Properties.Resources._000_ScheduledVMsnapshotDiskOnly_h32bit_16;
            public static Image _000_ScheduledVMsnapshotDiskOnly_h32bit_32 = Properties.Resources._000_ScheduledVMsnapshotDiskOnly_h32bit_32;
            public static Image _000_Search_h32bit_16 = Properties.Resources._000_Search_h32bit_16;
            public static Image _000_Server_h32bit_16 = Properties.Resources._000_Server_h32bit_16;
            public static Image _000_Server_h32bit_16_w_alert = Properties.Resources._000_Server_h32bit_16_w_alert;
            public static Image _000_ServerDisconnected_h32bit_16 = Properties.Resources._000_ServerDisconnected_h32bit_16;
            public static Image _000_ServerErrorFile_h32bit_16 = Properties.Resources._000_ServerErrorFile_h32bit_16;
            public static Image _000_ServerHome_h32bit_16 = Properties.Resources._000_ServerHome_h32bit_16;
            public static Image _000_ServerInProgress_h32bit_16 = Properties.Resources._000_ServerInProgress_h32bit_16;
            public static Image _000_ServerMaintenance_h32bit_16 = Properties.Resources._000_ServerMaintenance_h32bit_16;
            public static Image _000_ServerMaintenance_h32bit_32 = Properties.Resources._000_ServerMaintenance_h32bit_32;
            public static Image _000_ServerWlb_h32bit_16 = Properties.Resources._000_ServerWlb_h32bit_16;
            public static Image _000_Sites_h32bit_16 = Properties.Resources._000_Sites_h32bit_16;
            public static Image _000_SliderTexture = Properties.Resources._000_SliderTexture;
            public static Image _000_StartVM_h32bit_16 = Properties.Resources._000_StartVM_h32bit_16;
            public static Image _000_StoppedVM_h32bit_16 = Properties.Resources._000_StoppedVM_h32bit_16;
            public static Image _000_Storage_h32bit_16 = Properties.Resources._000_Storage_h32bit_16;
            public static Image _000_StorageBroken_h32bit_16 = Properties.Resources._000_StorageBroken_h32bit_16;
            public static Image _000_StorageDefault_h32bit_16 = Properties.Resources._000_StorageDefault_h32bit_16;
            public static Image _000_StorageDisabled_h32bit_16 = Properties.Resources._000_StorageDisabled_h32bit_16;
            public static Image _000_SuspendVM_h32bit_16 = Properties.Resources._000_SuspendVM_h32bit_16;
            public static Image _000_SwitcherBackground = Properties.Resources._000_SwitcherBackground;
            public static Image _000_Tag_h32bit_16 = Properties.Resources._000_Tag_h32bit_16;
            public static Image _000_TCP_IPGroup_h32bit_16 = Properties.Resources._000_TCP_IPGroup_h32bit_16;
            public static Image _000_TemplateDisabled_h32bit_16 = Properties.Resources._000_TemplateDisabled_h32bit_16;
            public static Image _000_TestFailover_h32bit_32 = Properties.Resources._000_TestFailover_h32bit_32;
            public static Image _000_Tick_h32bit_16 = Properties.Resources._000_Tick_h32bit_16;
            public static Image _000_ToolBar_Pref_Icon_dis = Properties.Resources._000_ToolBar_Pref_Icon_dis;
            public static Image _000_ToolBar_Pref_Icon_ovr = Properties.Resources._000_ToolBar_Pref_Icon_ovr;
            public static Image _000_ToolBar_Pref_Icon_up = Properties.Resources._000_ToolBar_Pref_Icon_up;
            public static Image _000_ToolBar_USB_Icon_dis = Properties.Resources._000_ToolBar_USB_Icon_dis;
            public static Image _000_ToolBar_USB_Icon_ovr = Properties.Resources._000_ToolBar_USB_Icon_ovr;
            public static Image _000_ToolBar_USB_Icon_up = Properties.Resources._000_ToolBar_USB_Icon_up;
            public static Image _000_TreeConnected_h32bit_16 = Properties.Resources._000_TreeConnected_h32bit_16;
            public static Image _000_UpgradePool_h32bit_32 = Properties.Resources._000_UpgradePool_h32bit_32;
            public static Image _000_User_h32bit_16 = Properties.Resources._000_User_h32bit_16;
            public static Image _000_UserAndGroup_h32bit_32 = Properties.Resources._000_UserAndGroup_h32bit_32;
            public static Image _000_UserTemplate_h32bit_16 = Properties.Resources._000_UserTemplate_h32bit_16;
            public static Image _000_ViewModeList_h32bit_16 = Properties.Resources._000_ViewModeList_h32bit_16;
            public static Image _000_ViewModeTree_h32bit_16 = Properties.Resources._000_ViewModeTree_h32bit_16;
            public static Image _000_VirtualAppliance_h32bit_16 = Properties.Resources._000_VirtualAppliance_h32bit_16;
            public static Image _000_VirtualStorage_h32bit_16 = Properties.Resources._000_VirtualStorage_h32bit_16;
            public static Image _000_VM_h32bit_16 = Properties.Resources._000_VM_h32bit_16;
            public static Image _000_VM_h32bit_24 = Properties.Resources._000_VM_h32bit_24;
            public static Image _000_VMDisabled_h32bit_16 = Properties.Resources._000_VMDisabled_h32bit_16;
            public static Image _000_VMPausedDisabled_h32bit_16 = Properties.Resources._000_VMPausedDisabled_h32bit_16;
            public static Image _000_VMSession_h32bit_16 = Properties.Resources._000_VMSession_h32bit_16;
            public static Image _000_VMSnapshotDiskMemory_h32bit_16 = Properties.Resources._000_VMSnapshotDiskMemory_h32bit_16;
            public static Image _000_VMSnapshotDiskMemory_h32bit_32 = Properties.Resources._000_VMSnapshotDiskMemory_h32bit_32;
            public static Image _000_VMSnapShotDiskOnly_h32bit_16 = Properties.Resources._000_VMSnapShotDiskOnly_h32bit_16;
            public static Image _000_VMSnapShotDiskOnly_h32bit_32 = Properties.Resources._000_VMSnapShotDiskOnly_h32bit_32;
            public static Image _000_VMStarting_h32bit_16 = Properties.Resources._000_VMStarting_h32bit_16;
            public static Image _000_VMStartingDisabled_h32bit_16 = Properties.Resources._000_VMStartingDisabled_h32bit_16;
            public static Image _000_VMStoppedDisabled_h32bit_16 = Properties.Resources._000_VMStoppedDisabled_h32bit_16;
            public static Image _000_VMTemplate_h32bit_16 = Properties.Resources._000_VMTemplate_h32bit_16;
            public static Image _000_WarningAlert_h32bit_32 = Properties.Resources._000_WarningAlert_h32bit_32;
            public static Image _000_weighting_h32bit_16 = Properties.Resources._000_weighting_h32bit_16;
            public static Image _000_XenCenterAlerts_h32bit_24 = Properties.Resources._000_XenCenterAlerts_h32bit_24;
            public static Image _001_Back_h32bit_24 = Properties.Resources._001_Back_h32bit_24;
            public static Image _001_CreateVM_h32bit_16 = Properties.Resources._001_CreateVM_h32bit_16;
            public static Image _001_ForceReboot_h32bit_16 = Properties.Resources._001_ForceReboot_h32bit_16;
            public static Image _001_ForceReboot_h32bit_24 = Properties.Resources._001_ForceReboot_h32bit_24;
            public static Image _001_ForceShutDown_h32bit_16 = Properties.Resources._001_ForceShutDown_h32bit_16;
            public static Image _001_ForceShutDown_h32bit_24 = Properties.Resources._001_ForceShutDown_h32bit_24;
            public static Image _001_Forward_h32bit_24 = Properties.Resources._001_Forward_h32bit_24;
            public static Image _001_LifeCycle_h32bit_24 = Properties.Resources._001_LifeCycle_h32bit_24;
            public static Image _001_PerformanceGraph_h32bit_16 = Properties.Resources._001_PerformanceGraph_h32bit_16;
            public static Image _001_Pin_h32bit_16 = Properties.Resources._001_Pin_h32bit_16;
            public static Image _001_PowerOn_h32bit_16 = Properties.Resources._001_PowerOn_h32bit_16;
            public static Image _001_PowerOn_h32bit_24 = Properties.Resources._001_PowerOn_h32bit_24;
            public static Image _001_Reboot_h32bit_16 = Properties.Resources._001_Reboot_h32bit_16;
            public static Image _001_Reboot_h32bit_24 = Properties.Resources._001_Reboot_h32bit_24;
            public static Image _001_ShutDown_h32bit_16 = Properties.Resources._001_ShutDown_h32bit_16;
            public static Image _001_ShutDown_h32bit_24 = Properties.Resources._001_ShutDown_h32bit_24;
            public static Image _001_Tools_h32bit_16 = Properties.Resources._001_Tools_h32bit_16;
            public static Image _001_WindowView_h32bit_16 = Properties.Resources._001_WindowView_h32bit_16;
            public static Image _002_Configure_h32bit_16 = Properties.Resources._002_Configure_h32bit_16;
            public static Image _015_Download_h32bit_32 = Properties.Resources._015_Download_h32bit_32;
            public static Image _075_TickRound_h32bit_16 = Properties.Resources._075_TickRound_h32bit_16;
            public static Image _075_WarningRound_h32bit_16 = Properties.Resources._075_WarningRound_h32bit_16;
            public static Image _112_LeftArrowLong_Blue_24x24_72 = Properties.Resources._112_LeftArrowLong_Blue_24x24_72;
            public static Image _112_RightArrowLong_Blue_24x24_72 = Properties.Resources._112_RightArrowLong_Blue_24x24_72;
            public static Image about_box_graphic_423x79 = Properties.Resources.about_box_graphic_423x79;
            public static Image ajax_loader = Properties.Resources.ajax_loader;
            public static Image alert1_16 = Properties.Resources.alert1_16;
            public static Image alert2_16 = Properties.Resources.alert2_16;
            public static Image alert3_16 = Properties.Resources.alert3_16;
            public static Image alert4_16 = Properties.Resources.alert4_16;
            public static Image alert5_16 = Properties.Resources.alert5_16;
            public static Image alert6_16 = Properties.Resources.alert6_16;
            public static Image alerts_32 = Properties.Resources.alerts_32;
            public static Image ascending_triangle = Properties.Resources.ascending_triangle;
            public static Image asterisk = Properties.Resources.asterisk;
            public static Image attach_24 = Properties.Resources.attach_24;
            public static Image attach_virtualstorage_32 = Properties.Resources.attach_virtualstorage_32;
            public static Image backup_restore_32 = Properties.Resources.backup_restore_32;
            public static Image cancelled_action_16 = Properties.Resources.cancelled_action_16;
            public static Image centos_16x = Properties.Resources.centos_16x;
            public static Image change_password_16 = Properties.Resources.change_password_16;
            public static Image change_password_32 = Properties.Resources.change_password_32;
            public static Image clonevm_16 = Properties.Resources.clonevm_16;
            public static Image close_16 = Properties.Resources.close_16;
            public static Image commands_16 = Properties.Resources.commands_16;
            public static Image console_16 = Properties.Resources.console_16;
            public static Image contracted_triangle = Properties.Resources.contracted_triangle;
            public static Image copy_16 = Properties.Resources.copy_16;
            public static Image coreos_16 = Properties.Resources.coreos_16;
            public static Image coreos_globe_icon = Properties.Resources.coreos_globe_icon;
            public static Image cross = Properties.Resources.cross;
            public static Image DateTime16 = Properties.Resources.DateTime16;
            public static Image DC_16 = Properties.Resources.DC_16;
            public static Image debian_16x = Properties.Resources.debian_16x;
            public static Image descending_triangle = Properties.Resources.descending_triangle;
            public static Image desktop = Properties.Resources.desktop;
            public static Image detach_24 = Properties.Resources.detach_24;
            public static Image edit_16 = Properties.Resources.edit_16;
            public static Image expanded_triangle = Properties.Resources.expanded_triangle;
            public static Image export_32 = Properties.Resources.export_32;
            public static Image folder_grey = Properties.Resources.folder_grey;
            public static Image folder_separator = Properties.Resources.folder_separator;
            public static Image grab = Properties.Resources.grab;
            public static Image grapharea = Properties.Resources.grapharea;
            public static Image graphline = Properties.Resources.graphline;
            public static Image gripper = Properties.Resources.gripper;
            public static Image ha_16 = Properties.Resources.ha_16;
            public static Image help_16_hover = Properties.Resources.help_16_hover;
            public static Image help_24 = Properties.Resources.help_24;
            public static Image help_24_hover = Properties.Resources.help_24_hover;
            public static Image help_32_hover = Properties.Resources.help_32_hover;
            public static Image homepage_bullet = Properties.Resources.homepage_bullet;
            public static Image import_32 = Properties.Resources.import_32;
            public static Image infra_view_16 = Properties.Resources.infra_view_16;
            public static Image infra_view_16_textured = Properties.Resources.infra_view_16_textured;
            public static Image infra_view_24 = Properties.Resources.infra_view_24;
            public static Image licensekey_32 = Properties.Resources.licensekey_32;
            public static Image lifecycle_hot = Properties.Resources.lifecycle_hot;
            public static Image lifecycle_pressed = Properties.Resources.lifecycle_pressed;
            public static Image log_destination_16 = Properties.Resources.log_destination_16;
            public static Image Logo = Properties.Resources.Logo;
            public static Image memory_dynmax_slider = Properties.Resources.memory_dynmax_slider;
            public static Image memory_dynmax_slider_dark = Properties.Resources.memory_dynmax_slider_dark;
            public static Image memory_dynmax_slider_light = Properties.Resources.memory_dynmax_slider_light;
            public static Image memory_dynmax_slider_noedit = Properties.Resources.memory_dynmax_slider_noedit;
            public static Image memory_dynmax_slider_noedit_small = Properties.Resources.memory_dynmax_slider_noedit_small;
            public static Image memory_dynmax_slider_small = Properties.Resources.memory_dynmax_slider_small;
            public static Image memory_dynmin_slider = Properties.Resources.memory_dynmin_slider;
            public static Image memory_dynmin_slider_dark = Properties.Resources.memory_dynmin_slider_dark;
            public static Image memory_dynmin_slider_light = Properties.Resources.memory_dynmin_slider_light;
            public static Image memory_dynmin_slider_noedit = Properties.Resources.memory_dynmin_slider_noedit;
            public static Image memory_dynmin_slider_noedit_small = Properties.Resources.memory_dynmin_slider_noedit_small;
            public static Image memory_dynmin_slider_small = Properties.Resources.memory_dynmin_slider_small;
            public static Image minus = Properties.Resources.minus;
            public static Image more_16 = Properties.Resources.more_16;
            public static Image neokylin_16x = Properties.Resources.neokylin_16x;
            public static Image notif_alerts_16 = Properties.Resources.notif_alerts_16;
            public static Image notif_events_16 = Properties.Resources.notif_events_16;
            public static Image notif_events_errors_16 = Properties.Resources.notif_events_errors_16;
            public static Image notif_none_16 = Properties.Resources.notif_none_16;
            public static Image notif_none_24 = Properties.Resources.notif_none_24;
            public static Image notif_updates_16 = Properties.Resources.notif_updates_16;
            public static Image objects_16 = Properties.Resources.objects_16;
            public static Image objects_16_textured = Properties.Resources.objects_16_textured;
            public static Image objects_24 = Properties.Resources.objects_24;
            public static Image oracle_16x = Properties.Resources.oracle_16x;
            public static Image org_view_16 = Properties.Resources.org_view_16;
            public static Image org_view_24 = Properties.Resources.org_view_24;
            public static Image padlock = Properties.Resources.padlock;
            public static Image paste_16 = Properties.Resources.paste_16;
            public static Image PausedDC_16 = Properties.Resources.PausedDC_16;
            public static Image PDChevronDown = Properties.Resources.PDChevronDown;
            public static Image PDChevronDownOver = Properties.Resources.PDChevronDownOver;
            public static Image PDChevronLeft = Properties.Resources.PDChevronLeft;
            public static Image PDChevronRight = Properties.Resources.PDChevronRight;
            public static Image PDChevronUp = Properties.Resources.PDChevronUp;
            public static Image PDChevronUpOver = Properties.Resources.PDChevronUpOver;
            public static Image pool_up_16 = Properties.Resources.pool_up_16;
            public static Image redhat_16x = Properties.Resources.redhat_16x;
            public static Image Refresh16 = Properties.Resources.Refresh16;
            public static Image RunningDC_16 = Properties.Resources.RunningDC_16;
            public static Image saved_searches_16 = Properties.Resources.saved_searches_16;
            public static Image saved_searches_24 = Properties.Resources.saved_searches_24;
            public static Image scilinux_16x = Properties.Resources.scilinux_16x;
            public static Image server_up_16 = Properties.Resources.server_up_16;
            public static Image sl_16 = Properties.Resources.sl_16;
            public static Image sl_add_storage_system_16 = Properties.Resources.sl_add_storage_system_16;
            public static Image sl_add_storage_system_32 = Properties.Resources.sl_add_storage_system_32;
            public static Image sl_add_storage_system_small_16 = Properties.Resources.sl_add_storage_system_small_16;
            public static Image sl_connected_16 = Properties.Resources.sl_connected_16;
            public static Image sl_connecting_16 = Properties.Resources.sl_connecting_16;
            public static Image sl_disconnected_16 = Properties.Resources.sl_disconnected_16;
            public static Image sl_lun_16 = Properties.Resources.sl_lun_16;
            public static Image sl_luns_16 = Properties.Resources.sl_luns_16;
            public static Image sl_pool_16 = Properties.Resources.sl_pool_16;
            public static Image sl_pools_16 = Properties.Resources.sl_pools_16;
            public static Image sl_system_16 = Properties.Resources.sl_system_16;
            public static Image SpinningFrame0 = Properties.Resources.SpinningFrame0;
            public static Image SpinningFrame1 = Properties.Resources.SpinningFrame1;
            public static Image SpinningFrame2 = Properties.Resources.SpinningFrame2;
            public static Image SpinningFrame3 = Properties.Resources.SpinningFrame3;
            public static Image SpinningFrame4 = Properties.Resources.SpinningFrame4;
            public static Image SpinningFrame5 = Properties.Resources.SpinningFrame5;
            public static Image SpinningFrame6 = Properties.Resources.SpinningFrame6;
            public static Image SpinningFrame7 = Properties.Resources.SpinningFrame7;
            public static Image StoppedDC_16 = Properties.Resources.StoppedDC_16;
            public static Image subscribe = Properties.Resources.subscribe;
            public static Image suse_16x = Properties.Resources.suse_16x;
            public static Image tools_notinstalled_16x = Properties.Resources.tools_notinstalled_16x;
            public static Image tools_outofdate_16x = Properties.Resources.tools_outofdate_16x;
            public static Image tools_uptodate_16x = Properties.Resources.tools_uptodate_16x;
            public static Image tree_minus = Properties.Resources.tree_minus;
            public static Image tree_plus = Properties.Resources.tree_plus;
            public static Image tshadowdown = Properties.Resources.tshadowdown;
            public static Image tshadowdownleft = Properties.Resources.tshadowdownleft;
            public static Image tshadowdownright = Properties.Resources.tshadowdownright;
            public static Image tshadowright = Properties.Resources.tshadowright;
            public static Image tshadowtopright = Properties.Resources.tshadowtopright;
            public static Image ubuntu_16x = Properties.Resources.ubuntu_16x;
            public static Image upsell_16 = Properties.Resources.upsell_16;
            public static Image usagebar_0 = Properties.Resources.usagebar_0;
            public static Image usagebar_1 = Properties.Resources.usagebar_1;
            public static Image usagebar_10 = Properties.Resources.usagebar_10;
            public static Image usagebar_2 = Properties.Resources.usagebar_2;
            public static Image usagebar_3 = Properties.Resources.usagebar_3;
            public static Image usagebar_4 = Properties.Resources.usagebar_4;
            public static Image usagebar_5 = Properties.Resources.usagebar_5;
            public static Image usagebar_6 = Properties.Resources.usagebar_6;
            public static Image usagebar_7 = Properties.Resources.usagebar_7;
            public static Image usagebar_8 = Properties.Resources.usagebar_8;
            public static Image usagebar_9 = Properties.Resources.usagebar_9;
            public static Image virtualstorage_snapshot_16 = Properties.Resources.virtualstorage_snapshot_16;
            public static Image vmBackground = Properties.Resources.vmBackground;
            public static Image vmBackgroundCurrent = Properties.Resources.vmBackgroundCurrent;
            public static Image VMTemplate_h32bit_32 = Properties.Resources.VMTemplate_h32bit_32;
            public static Image vnc_local_cursor = Properties.Resources.vnc_local_cursor;
            public static Image windows_h32bit_16 = Properties.Resources.windows_h32bit_16;
            public static Image wizard_background = Properties.Resources.wizard_background;
            public static Image WLB = Properties.Resources.WLB;
            public static Image XS = Properties.Resources.XS;
        }
    }
}