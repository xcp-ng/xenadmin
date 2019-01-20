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

namespace XenAdmin
{
    public enum Icons
    {
        XenCenter,

        HostConnecting,
        HostConnected,
        HostDisconnected,
        HostEvacuate,
        HostUnpatched,
        HostOlderThanMaster,
        HostHasCrashDumps,

        VmRunning,
        VmRunningDisabled,
        VmStopped,
        VmStoppedDisabled,
        VmSuspended,
        VmSuspendedDisabled,
        VmPaused,
        VmPausedDisabled,
        VmStarting,
        VmStartingDisabled,

        Template,
        TemplateDisabled,
        TemplateUser,
        Snapshot,
        SnapshotDisksOnly,
        SnapshotWithMem,
        ScheduledSnapshotDiskOnly,
        ScheduledSnapshotDiskMemory,

        PoolConnected,
        PoolNotFullyUpgraded,

        PoolUnlicensed,
        ServerUnlicensed,

        Storage,
        StorageBroken,
        StorageDefault,
        StorageDisabled,

        PowerStateHalted,
        PowerStateSuspended,
        PowerStatePaused,
        PowerStateRunning,
        PowerStateUnknown,

        Network,

        DefaultSearch,
        Search,

        #region Server message icons
        
        // Icons for server messages
        VmStart,
        VmShutdown,
        VmReboot,
        VmSuspend,
        VmResumed,
        VmCloned,

        MessageUnknown,
        MessagePriority1,
        MessagePriority2,
        MessagePriority3,
        MessagePriority4,
        MessagePriority5,

        #endregion

        #region OS Icons

        CentOS,
        Debian,
        Linx,
        Oracle,
        RHEL,
        SUSE,
        Ubuntu,
        YinheKylin,
        SciLinux,
        NeoKylin,
        Asianux,
        Turbo,       
        Windows,
        CoreOS,

        #endregion

        #region tools icons

        ToolInstalled,
        ToolsNotInstalled,
        ToolsOutOfDate,

        #endregion

        #region Blank Icons

        VM,
        Host,
        Pool,

        #endregion

        VDI,
        VDISnapshot,

        Folder,
        FolderGrey,

        Tag,
        CustomField,
        HA,

        VmAppliance,

        VmCrossPoolMigrate,
        VmCrossPoolMigrateStopped,
        VmCrossPoolMigrateSuspended,

        PifPrimary,
        PifSecondary,

        Home,
        Objects,

        PvsSite,

        #region DockerContainer Icons

        DCRunning,
        DCStopped,
        DCPaused,

        #endregion

        #region Status Icons
        Ok,
        Info,
        Warning,
        Error
        #endregion
    }
}
