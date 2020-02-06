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

using CookComputing.XmlRpc;

namespace XenAdmin.XCM
{
    public interface IConversionProxy : IXmlRpcProxy
    {
        [XmlRpcMethod("svc.get_version")]
        string GetVpxVersion();

        [XmlRpcMethod("svc.get_vmlist")]
        VmInstance[] GetSourceVMs(ServiceCredentials cred, ServerInfo source);

        [XmlRpcMethod("svc.get_networks")]
        NetworkInstance[] GetSourceNetworks(ServiceCredentials cred, ServerInfo source);

        [XmlRpcMethod("svc.vm_fixups")]
        void ApplyVmFixups(ServiceCredentials cred, string vmUuid);

        [XmlRpcMethod("svc.getlog")]
        string GetVpxLogs(ServiceCredentials cred);

        [XmlRpcMethod("svc.log")]
        void LogMessage(ServiceCredentials cred, int level, string msg);


        [XmlRpcMethod("job.create")]
        Conversion CreateConversion(ServiceCredentials cred, ConversionConfig configuration);

        [XmlRpcMethod("job.retry")]
        Conversion RetryConversion(ServiceCredentials cred, string conversionId);

        [XmlRpcMethod("job.delete")]
        void CancelConversion(ServiceCredentials cred, string conversionId);

        [XmlRpcMethod("job.clear")]
        void ClearConversionHistory(ServiceCredentials cred);

        [XmlRpcMethod("job.get_all")]
        Conversion[] GetConversionHistory(ServiceCredentials cred);

        [XmlRpcMethod("job.get")]
        Conversion GetConversionDetails(ServiceCredentials cred, string conversionId);

        [XmlRpcMethod("job.get_reserveddiskspace")]
        long GetReservedDiskSpace(ServiceCredentials cred, string srUuid);

        [XmlRpcMethod("job.getlog")]
        string GetConversionLog(ServiceCredentials cred, string conversionId);

        [XmlRpcMethod("job.update_progress")]
        void UpdateConversionProgress(string conversionId, ConversionProgressData progressData);
    }
}