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

using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class DrTaskCreateAction : AsyncAction
    {
        private readonly ScannedDeviceInfo _deviceInfo;

        /// <summary>
        /// RBAC dependencies needed to create DR_task.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies = new RbacMethodList("DR_task.async_create");

        public DrTaskCreateAction(IXenConnection connection, ScannedDeviceInfo deviceInfo)
            : base(connection, Messages.ACTION_DR_TASK_CREATE_TITLE) 
        {
            _deviceInfo = deviceInfo;
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);
            #endregion
        }

        protected override void Run()
        {
            Description = Messages.ACTION_DR_TASK_CREATE_STATUS; 
            
            // create DR_task
            RelatedTask = DR_task.async_create(Session, _deviceInfo.Type.ToString(),
                                               _deviceInfo.DeviceConfig, _deviceInfo.SrUuids);

            PollToCompletion();
            Description = Messages.ACTION_DR_TASK_CREATE_DONE; 
        }
    }

    public class ScannedDeviceInfo
    {
        public readonly SR.SRTypes Type;
        public readonly Dictionary<string, string> DeviceConfig;
        public List<SR.SRInfo> SRList;

        public string[] SrUuids
        {
            get
            {
                string[] result = new string[SRList.Count];
                int i = 0;
                foreach (SR.SRInfo srInfo in SRList)
                    result[i++] = srInfo.ToString();
                return result;
            }
        }

        public ScannedDeviceInfo(SR.SRTypes type, Dictionary<string, string> deviceConfig, List<SR.SRInfo> srList)
        {
            Type = type;
            DeviceConfig = deviceConfig;
            SRList = srList;
        }

        public ScannedDeviceInfo(SR.SRTypes type, Dictionary<string, string> deviceConfig, string srUuid)
            : this(type, deviceConfig, new List<SR.SRInfo> { new SR.SRInfo(srUuid) })
        {
        }
    }
}
