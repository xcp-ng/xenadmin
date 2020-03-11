/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


namespace XenAPI
{
    public partial class Message : XenObject<Message>
    {
        public enum MessageType
        {
            CLUSTER_HOST_FENCING,
            CLUSTER_HOST_ENABLE_FAILED,
            POOL_CPU_FEATURES_UP,
            POOL_CPU_FEATURES_DOWN,
            HOST_CPU_FEATURES_UP,
            HOST_CPU_FEATURES_DOWN,
            BOND_STATUS_CHANGED,
            VDI_CBT_RESIZE_FAILED,
            VDI_CBT_SNAPSHOT_FAILED,
            VDI_CBT_METADATA_INCONSISTENT,
            VMSS_SNAPSHOT_MISSED_EVENT,
            VMSS_XAPI_LOGON_FAILURE,
            VMSS_LICENSE_ERROR,
            VMSS_SNAPSHOT_FAILED,
            VMSS_SNAPSHOT_SUCCEEDED,
            VMSS_SNAPSHOT_LOCK_FAILED,
            VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS,
            VMPP_ARCHIVE_MISSED_EVENT,
            VMPP_SNAPSHOT_MISSED_EVENT,
            VMPP_XAPI_LOGON_FAILURE,
            VMPP_LICENSE_ERROR,
            VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED,
            VMPP_ARCHIVE_TARGET_MOUNT_FAILED,
            VMPP_ARCHIVE_SUCCEEDED,
            VMPP_ARCHIVE_FAILED_0,
            VMPP_ARCHIVE_LOCK_FAILED,
            VMPP_SNAPSHOT_FAILED,
            VMPP_SNAPSHOT_SUCCEEDED,
            VMPP_SNAPSHOT_LOCK_FAILED,
            PVS_PROXY_SR_OUT_OF_SPACE,
            PVS_PROXY_NO_SERVER_AVAILABLE,
            PVS_PROXY_SETUP_FAILED,
            PVS_PROXY_NO_CACHE_SR_AVAILABLE,
            LICENSE_SERVER_VERSION_OBSOLETE,
            LICENSE_SERVER_UNREACHABLE,
            LICENSE_NOT_AVAILABLE,
            GRACE_LICENSE,
            LICENSE_SERVER_UNAVAILABLE,
            LICENSE_SERVER_CONNECTED,
            LICENSE_EXPIRED,
            LICENSE_EXPIRES_SOON,
            LICENSE_DOES_NOT_SUPPORT_POOLING,
            MULTIPATH_PERIODIC_ALERT,
            EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS,
            EXTAUTH_INIT_IN_HOST_FAILED,
            WLB_OPTIMIZATION_ALERT,
            WLB_CONSULTATION_FAILED,
            ALARM,
            PBD_PLUG_FAILED_ON_SERVER_START,
            POOL_MASTER_TRANSITION,
            HOST_CLOCK_WENT_BACKWARDS,
            HOST_CLOCK_SKEW_DETECTED,
            HOST_SYNC_DATA_FAILED,
            VM_SECURE_BOOT_FAILED,
            VM_CLONED,
            VM_CRASHED,
            VM_RESUMED,
            VM_SUSPENDED,
            VM_REBOOTED,
            VM_SHUTDOWN,
            VM_STARTED,
            VCPU_QOS_FAILED,
            VBD_QOS_FAILED,
            VIF_QOS_FAILED,
            IP_CONFIGURED_PIF_CAN_UNPLUG,
            METADATA_LUN_BROKEN,
            METADATA_LUN_HEALTHY,
            HA_HOST_WAS_FENCED,
            HA_HOST_FAILED,
            HA_PROTECTED_VM_RESTART_FAILED,
            HA_POOL_DROP_IN_PLAN_EXISTS_FOR,
            HA_POOL_OVERCOMMITTED,
            HA_NETWORK_BONDING_ERROR,
            HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT,
            HA_STATEFILE_APPROACHING_TIMEOUT,
            HA_HEARTBEAT_APPROACHING_TIMEOUT,
            HA_STATEFILE_LOST,
            unknown
        }

        public MessageType Type
        {
            get
            {
                switch (this.name)
                {
                    case "CLUSTER_HOST_FENCING":
                        return MessageType.CLUSTER_HOST_FENCING;
                    case "CLUSTER_HOST_ENABLE_FAILED":
                        return MessageType.CLUSTER_HOST_ENABLE_FAILED;
                    case "POOL_CPU_FEATURES_UP":
                        return MessageType.POOL_CPU_FEATURES_UP;
                    case "POOL_CPU_FEATURES_DOWN":
                        return MessageType.POOL_CPU_FEATURES_DOWN;
                    case "HOST_CPU_FEATURES_UP":
                        return MessageType.HOST_CPU_FEATURES_UP;
                    case "HOST_CPU_FEATURES_DOWN":
                        return MessageType.HOST_CPU_FEATURES_DOWN;
                    case "BOND_STATUS_CHANGED":
                        return MessageType.BOND_STATUS_CHANGED;
                    case "VDI_CBT_RESIZE_FAILED":
                        return MessageType.VDI_CBT_RESIZE_FAILED;
                    case "VDI_CBT_SNAPSHOT_FAILED":
                        return MessageType.VDI_CBT_SNAPSHOT_FAILED;
                    case "VDI_CBT_METADATA_INCONSISTENT":
                        return MessageType.VDI_CBT_METADATA_INCONSISTENT;
                    case "VMSS_SNAPSHOT_MISSED_EVENT":
                        return MessageType.VMSS_SNAPSHOT_MISSED_EVENT;
                    case "VMSS_XAPI_LOGON_FAILURE":
                        return MessageType.VMSS_XAPI_LOGON_FAILURE;
                    case "VMSS_LICENSE_ERROR":
                        return MessageType.VMSS_LICENSE_ERROR;
                    case "VMSS_SNAPSHOT_FAILED":
                        return MessageType.VMSS_SNAPSHOT_FAILED;
                    case "VMSS_SNAPSHOT_SUCCEEDED":
                        return MessageType.VMSS_SNAPSHOT_SUCCEEDED;
                    case "VMSS_SNAPSHOT_LOCK_FAILED":
                        return MessageType.VMSS_SNAPSHOT_LOCK_FAILED;
                    case "VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS":
                        return MessageType.VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS;
                    case "VMPP_ARCHIVE_MISSED_EVENT":
                        return MessageType.VMPP_ARCHIVE_MISSED_EVENT;
                    case "VMPP_SNAPSHOT_MISSED_EVENT":
                        return MessageType.VMPP_SNAPSHOT_MISSED_EVENT;
                    case "VMPP_XAPI_LOGON_FAILURE":
                        return MessageType.VMPP_XAPI_LOGON_FAILURE;
                    case "VMPP_LICENSE_ERROR":
                        return MessageType.VMPP_LICENSE_ERROR;
                    case "VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED":
                        return MessageType.VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED;
                    case "VMPP_ARCHIVE_TARGET_MOUNT_FAILED":
                        return MessageType.VMPP_ARCHIVE_TARGET_MOUNT_FAILED;
                    case "VMPP_ARCHIVE_SUCCEEDED":
                        return MessageType.VMPP_ARCHIVE_SUCCEEDED;
                    case "VMPP_ARCHIVE_FAILED_0":
                        return MessageType.VMPP_ARCHIVE_FAILED_0;
                    case "VMPP_ARCHIVE_LOCK_FAILED":
                        return MessageType.VMPP_ARCHIVE_LOCK_FAILED;
                    case "VMPP_SNAPSHOT_FAILED":
                        return MessageType.VMPP_SNAPSHOT_FAILED;
                    case "VMPP_SNAPSHOT_SUCCEEDED":
                        return MessageType.VMPP_SNAPSHOT_SUCCEEDED;
                    case "VMPP_SNAPSHOT_LOCK_FAILED":
                        return MessageType.VMPP_SNAPSHOT_LOCK_FAILED;
                    case "PVS_PROXY_SR_OUT_OF_SPACE":
                        return MessageType.PVS_PROXY_SR_OUT_OF_SPACE;
                    case "PVS_PROXY_NO_SERVER_AVAILABLE":
                        return MessageType.PVS_PROXY_NO_SERVER_AVAILABLE;
                    case "PVS_PROXY_SETUP_FAILED":
                        return MessageType.PVS_PROXY_SETUP_FAILED;
                    case "PVS_PROXY_NO_CACHE_SR_AVAILABLE":
                        return MessageType.PVS_PROXY_NO_CACHE_SR_AVAILABLE;
                    case "LICENSE_SERVER_VERSION_OBSOLETE":
                        return MessageType.LICENSE_SERVER_VERSION_OBSOLETE;
                    case "LICENSE_SERVER_UNREACHABLE":
                        return MessageType.LICENSE_SERVER_UNREACHABLE;
                    case "LICENSE_NOT_AVAILABLE":
                        return MessageType.LICENSE_NOT_AVAILABLE;
                    case "GRACE_LICENSE":
                        return MessageType.GRACE_LICENSE;
                    case "LICENSE_SERVER_UNAVAILABLE":
                        return MessageType.LICENSE_SERVER_UNAVAILABLE;
                    case "LICENSE_SERVER_CONNECTED":
                        return MessageType.LICENSE_SERVER_CONNECTED;
                    case "LICENSE_EXPIRED":
                        return MessageType.LICENSE_EXPIRED;
                    case "LICENSE_EXPIRES_SOON":
                        return MessageType.LICENSE_EXPIRES_SOON;
                    case "LICENSE_DOES_NOT_SUPPORT_POOLING":
                        return MessageType.LICENSE_DOES_NOT_SUPPORT_POOLING;
                    case "MULTIPATH_PERIODIC_ALERT":
                        return MessageType.MULTIPATH_PERIODIC_ALERT;
                    case "EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS":
                        return MessageType.EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS;
                    case "EXTAUTH_INIT_IN_HOST_FAILED":
                        return MessageType.EXTAUTH_INIT_IN_HOST_FAILED;
                    case "WLB_OPTIMIZATION_ALERT":
                        return MessageType.WLB_OPTIMIZATION_ALERT;
                    case "WLB_CONSULTATION_FAILED":
                        return MessageType.WLB_CONSULTATION_FAILED;
                    case "ALARM":
                        return MessageType.ALARM;
                    case "PBD_PLUG_FAILED_ON_SERVER_START":
                        return MessageType.PBD_PLUG_FAILED_ON_SERVER_START;
                    case "POOL_MASTER_TRANSITION":
                        return MessageType.POOL_MASTER_TRANSITION;
                    case "HOST_CLOCK_WENT_BACKWARDS":
                        return MessageType.HOST_CLOCK_WENT_BACKWARDS;
                    case "HOST_CLOCK_SKEW_DETECTED":
                        return MessageType.HOST_CLOCK_SKEW_DETECTED;
                    case "HOST_SYNC_DATA_FAILED":
                        return MessageType.HOST_SYNC_DATA_FAILED;
                    case "VM_SECURE_BOOT_FAILED":
                        return MessageType.VM_SECURE_BOOT_FAILED;
                    case "VM_CLONED":
                        return MessageType.VM_CLONED;
                    case "VM_CRASHED":
                        return MessageType.VM_CRASHED;
                    case "VM_RESUMED":
                        return MessageType.VM_RESUMED;
                    case "VM_SUSPENDED":
                        return MessageType.VM_SUSPENDED;
                    case "VM_REBOOTED":
                        return MessageType.VM_REBOOTED;
                    case "VM_SHUTDOWN":
                        return MessageType.VM_SHUTDOWN;
                    case "VM_STARTED":
                        return MessageType.VM_STARTED;
                    case "VCPU_QOS_FAILED":
                        return MessageType.VCPU_QOS_FAILED;
                    case "VBD_QOS_FAILED":
                        return MessageType.VBD_QOS_FAILED;
                    case "VIF_QOS_FAILED":
                        return MessageType.VIF_QOS_FAILED;
                    case "IP_CONFIGURED_PIF_CAN_UNPLUG":
                        return MessageType.IP_CONFIGURED_PIF_CAN_UNPLUG;
                    case "METADATA_LUN_BROKEN":
                        return MessageType.METADATA_LUN_BROKEN;
                    case "METADATA_LUN_HEALTHY":
                        return MessageType.METADATA_LUN_HEALTHY;
                    case "HA_HOST_WAS_FENCED":
                        return MessageType.HA_HOST_WAS_FENCED;
                    case "HA_HOST_FAILED":
                        return MessageType.HA_HOST_FAILED;
                    case "HA_PROTECTED_VM_RESTART_FAILED":
                        return MessageType.HA_PROTECTED_VM_RESTART_FAILED;
                    case "HA_POOL_DROP_IN_PLAN_EXISTS_FOR":
                        return MessageType.HA_POOL_DROP_IN_PLAN_EXISTS_FOR;
                    case "HA_POOL_OVERCOMMITTED":
                        return MessageType.HA_POOL_OVERCOMMITTED;
                    case "HA_NETWORK_BONDING_ERROR":
                        return MessageType.HA_NETWORK_BONDING_ERROR;
                    case "HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT":
                        return MessageType.HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT;
                    case "HA_STATEFILE_APPROACHING_TIMEOUT":
                        return MessageType.HA_STATEFILE_APPROACHING_TIMEOUT;
                    case "HA_HEARTBEAT_APPROACHING_TIMEOUT":
                        return MessageType.HA_HEARTBEAT_APPROACHING_TIMEOUT;
                    case "HA_STATEFILE_LOST":
                        return MessageType.HA_STATEFILE_LOST;
                    default:
                        return MessageType.unknown;
                }
            }
        }
    }
}