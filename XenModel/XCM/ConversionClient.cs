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
using System.Net;
using CookComputing.XmlRpc;
using XenAdmin.Network;


namespace XenAdmin.XCM
{
    public class ConversionClient
    {
        private readonly IConversionProxy _conversionProxy;
        private readonly NetworkCredential _credential;

        public ConversionClient(IXenConnection connection, string vpxIp, bool useSsl)
        {
            Connection = connection;
            _credential = connection.NetworkCredential;
            var session = connection.Session;

            _conversionProxy = XmlRpcProxyGen.Create<IConversionProxy>();
            _conversionProxy.Url = string.Format(useSsl ? "https://{0}" : "http://{0}", vpxIp);

            if (session.JsonRpcClient != null)
            {
                _conversionProxy.Timeout = session.JsonRpcClient.Timeout;
                _conversionProxy.NonStandard = XmlRpcNonStandard.All;
                _conversionProxy.UseIndentation = false;
                _conversionProxy.UserAgent = session.JsonRpcClient.UserAgent;
                _conversionProxy.KeepAlive = session.JsonRpcClient.KeepAlive;
                _conversionProxy.Proxy = session.JsonRpcClient.WebProxy;
            }
            else
            {
                _conversionProxy.Timeout = session.proxy.Timeout;
                _conversionProxy.NonStandard = session.proxy.NonStandard;
                _conversionProxy.UseIndentation = session.proxy.UseIndentation;
                _conversionProxy.UserAgent = session.proxy.UserAgent;
                _conversionProxy.KeepAlive = session.proxy.KeepAlive;
                _conversionProxy.Proxy = session.proxy.Proxy;
            }
        }

        private ServiceCredentials ServiceCredentials => new ServiceCredentials {Username = _credential.UserName, Password = _credential.Password};

        public IXenConnection Connection { get; }

        public string GetVpxVersion()
        {
            return _conversionProxy.GetVpxVersion();
        }

        public VmInstance[] GetSourceVMs(ServerInfo vmWareServer)
        {
            return _conversionProxy.GetSourceVMs(ServiceCredentials, vmWareServer);
        }

        public Conversion CreateConversion(ConversionConfig config)
        {
            return _conversionProxy.CreateConversion(ServiceCredentials, config);
        }

        public Conversion RetryConversion(Conversion conversion)
        {
            return _conversionProxy.RetryConversion(ServiceCredentials, conversion.Id);
        }

        public void CancelConversion(Conversion conversion)
        {
            _conversionProxy.CancelConversion(ServiceCredentials, conversion.Id);
        }

        public void ClearConversionHistory()
        {
            _conversionProxy.ClearConversionHistory(ServiceCredentials);
        }

        public Conversion[] GetConversionHistory()
        {
            return _conversionProxy.GetConversionHistory(ServiceCredentials);
        }

        public Conversion GetConversionDetails(Conversion conversion)
        {
            return _conversionProxy.GetConversionDetails(ServiceCredentials, conversion.Id);
        }

        public long GetReservedDiskSpace(string srUuid)
        {
            return _conversionProxy.GetReservedDiskSpace(ServiceCredentials, srUuid);
        }

        public string GetVpxLogs()
        {
            return _conversionProxy.GetVpxLogs(ServiceCredentials);
        }

        public string GetConversionLog(Conversion conversion)
        {
            return _conversionProxy.GetConversionLog(ServiceCredentials, conversion.Id);
        }

        public void LogMessage(int level, string msg)
        {
            _conversionProxy.LogMessage(ServiceCredentials, level, msg);
        }

        public NetworkInstance[] GetNetworks(ServerInfo targetServer)
        {
            return _conversionProxy.GetSourceNetworks(ServiceCredentials, targetServer);
        }

        public void ApplyVmFixups(string vmUuid)
        {
            _conversionProxy.ApplyVmFixups(ServiceCredentials, vmUuid);
        }

        public void UpdateConversionProgress(Conversion conversion, ConversionProgressData progressData)
        {
            _conversionProxy.UpdateConversionProgress(conversion.Id, progressData);
        }
    }
}
