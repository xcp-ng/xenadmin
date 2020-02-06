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
using CookComputing.XmlRpc;
using XenAdmin.Actions;
using XenCenterLib;


namespace XenAdmin.XCM
{
    public struct ServiceCredentials
    {
        public string Username;
        public string Password;
    }

    public struct Conversion : IStatus
    {
        #region Constructors

        public Conversion(string id)
        {
            Configuration = new ConversionConfig();

            Id = id;
            Name = "";
            Description = "";

            XenServerName = "";
            SRName = "";

            CreatedTime = DateTime.Now;
            StartTime = DateTime.MaxValue;
            CompletedTime = DateTime.MinValue;

            PercentComplete = 0;
            Status = (int)ConversionStatus.Created;
            CompressedBytesRead = UncompressedBytesWritten = 0;
            StatusDetail = "";
            Error = "";
            ClientIpEndPoint = "";
            XenServerVMUuid = "";
        }

        public Conversion(ConversionConfig config, long id, string conversionName, string conversionDescription)
            : this(id.ToString())
        {
            Configuration = config;
            Name = conversionName;
            Description = conversionDescription;
        }

        #endregion

        public string Id;

        [XmlRpcMember("JobName")]
        public string Name;

        [XmlRpcMember("JobDesc")]
        public string Description;

        public string XenServerName;
        public string SRName;
        public DateTime CreatedTime;
        public DateTime StartTime;
        public DateTime CompletedTime;

        public long CompressedBytesRead;
        public long UncompressedBytesWritten;
        public long PercentComplete;

        [XmlRpcMember("State")]
        public int Status;

        [XmlRpcMember("StateDesc")]
        public string StatusDetail;

        [XmlRpcMember("ErrorString")]
        public string Error;

        /// <summary>
        /// Set by the server.
        /// </summary>
        public string ClientIpEndPoint;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string XenServerVMUuid;

        [XmlRpcMember("JobInfo")]
        public ConversionConfig Configuration;


        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool InProgress => Status == (int)ConversionStatus.Running;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool IsCompleted =>
            !(Status == (int)ConversionStatus.Created || Status == (int)ConversionStatus.Queued || Status == (int)ConversionStatus.Running);

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool Succeeded => Status == (int)ConversionStatus.Successful;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool IsCancelled => Status == (int)ConversionStatus.Cancelled;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool IsError => Status == (int)ConversionStatus.Failed;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool IsIncomplete => Status == (int)ConversionStatus.Incomplete;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool IsQueued => Status == (int)ConversionStatus.Created || Status == (int)ConversionStatus.Queued;


        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool CanCancel => Status == (int)ConversionStatus.Queued || Status == (int)ConversionStatus.Running;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool CanRetry => Status == (int)ConversionStatus.Cancelled || Status == (int)ConversionStatus.Failed;


        public string GetStatusString()
        {
            switch (Status)
            {
                case (int)ConversionStatus.Created:
                    return Messages.CONVERSION_STATUS_CREATED;
                case (int)ConversionStatus.Queued:
                    return Messages.CONVERSION_STATUS_QUEUED;
                case (int)ConversionStatus.Running:
                    return Messages.CONVERSION_STATUS_RUNNING;
                case (int)ConversionStatus.Successful:
                    return Messages.CONVERSION_STATUS_SUCCESSFUL;
                case (int)ConversionStatus.Failed:
                    return Messages.CONVERSION_STATUS_FAILED;
                case (int)ConversionStatus.Cancelled:
                    return Messages.CONVERSION_STATUS_CANCELLED;
                case (int)ConversionStatus.Incomplete:
                    return Messages.CONVERSION_STATUS_INCOMPLETE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Status), "Unknown Conversion Status");
            }
        }

        #region Sorting methods

        public static int CompareOnVm(Conversion conv1, Conversion conv2)
        {
            var result = StringUtility.NaturalCompare(conv1.Configuration.SourceVmName, conv2.Configuration.SourceVmName);
            return result == 0 ? CompareOnId(conv1, conv2) : result;
        }

        public static int CompareOnServer(Conversion conv1, Conversion conv2)
        {
            var result = StringUtility.NaturalCompare(conv1.Configuration.SourceServer.Hostname, conv2.Configuration.SourceServer.Hostname);
            return result == 0 ? CompareOnId(conv1, conv2) : result;
        }

        public static int CompareOnStatus(Conversion conv1, Conversion conv2)
        {
            var result = conv1.Status.CompareTo(conv2.Status);
            return result == 0 ? CompareOnId(conv1, conv2) : result;
        }

        public static int CompareOnId(Conversion conv1, Conversion conv2)
        {
            return StringUtility.NaturalCompare(conv1.Id, conv2.Id);
        }

        public static int CompareOnStartTime(Conversion conv1, Conversion conv2)
        {
            var result = DateTime.Compare(conv1.StartTime, conv2.StartTime);
            return result == 0 ? CompareOnId(conv1, conv2) : result;
        }

        public static int CompareOnCompletedTime(Conversion conv1, Conversion conv2)
        {
            var result = DateTime.Compare(conv1.CompletedTime, conv2.CompletedTime);
            return result == 0 ? CompareOnId(conv1, conv2) : result;
        }

        #endregion
    }

    public struct NetworkInstance
    {
        public string Name;
        public string Id;
    }

    public struct VmInstance
    {
        public string UUID;
        public string Name;
        public int PowerState;
        public string OSType;
        public long CommittedStorage;
        public long UncommittedStorage;
        public bool Template;
    }

    public struct ServerInfo
    {
        public int ServerType;
        public string Hostname;
        public string Username;
        public string Password;
    }


    public enum VmPowerState
    {
        Off,
        Running,
        Suspended
    }

    public enum ServerType
    {
        XenServer,
        ESXServer,
        VirtualCenter,
        HyperVServer
    }

    public enum ConversionStatus
    {
        Created = 0,
        Queued = 1,
        Running = 2,
        Successful = 3,
        Failed = 4,
        Cancelled = 5,
        Incomplete = 6,
    }


    public struct StorageMapping
    {
        /// <summary>
        /// The target SR where the converted VM's disks will be placed
        /// </summary>
        public string SRuuid;
    }

    public struct ConversionConfig
    {
        /// <summary>
        /// ESX/vCenter server with the source VMs
        /// </summary>
        [XmlRpcMember("Source")]
        public ServerInfo SourceServer;

        /// <summary>
        /// ESX/vCenter VM to convert
        /// </summary>
        public string SourceVmUUID;
        public string SourceVmName;

        [XmlRpcMember("ImportInfo")]
        public StorageMapping StorageMapping;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public object NetworkMappings;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool? PreserveMAC;

        public static object DictionaryToStruct(Dictionary<string, string> dict)
        {
            var theStruct = new XmlRpcStruct();
            if (dict == null)
                return theStruct;

            foreach (var kvp in dict)
                theStruct.Add(kvp.Key, kvp.Value);

            return theStruct;
        }
    }

    public struct ConversionProgressData
    {
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public long BytesRead;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public long BytesWritten;

        public ConversionProgressData(long bytesRead, long bytesWritten)
        {
            BytesRead = bytesRead;
            BytesWritten = bytesWritten;
        }
    }
}
