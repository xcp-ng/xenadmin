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
using System.Text;
using System.Security.Principal;
using System.IO.Pipes;
using XenAdmin.Model;

namespace XenServerHealthCheck
{

    public class CredentialReceiver
    {
        private CredentialReceiver() { }
        public static readonly CredentialReceiver instance = new CredentialReceiver();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NamedPipeServerStream pipeServer;
        private static readonly Object pipeServerLock = new Object();
        private const int pipeBufferSize = 1024;

        public void UnInit()
        {
            lock (pipeServerLock)
            {
                if (pipeServer != null)
                {
                    pipeServer.Close();
                    pipeServer = null;
                }
            }
        }

        public void Init()
        {
            lock (pipeServerLock)
            {
                if (pipeServer != null)
                {
                    return;
                }
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var rule = new PipeAccessRule(sid, PipeAccessRights.ReadWrite,
                System.Security.AccessControl.AccessControlType.Allow);
                var sec = new PipeSecurity();
                sec.AddAccessRule(rule);
                pipeServer = new NamedPipeServerStream(HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 0, 0, sec);
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
        }

        private void reInitPipe()
        {
            if (pipeServer != null)
            {
                pipeServer.Close();
                pipeServer = null;
            }
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var rule = new PipeAccessRule(sid, PipeAccessRights.ReadWrite,
            System.Security.AccessControl.AccessControlType.Allow);
            var sec = new PipeSecurity();
            sec.AddAccessRule(rule);
            pipeServer = new NamedPipeServerStream(HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 0, 0, sec);
            pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                lock (pipeServerLock)
                {
                    NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;
                    pipeServer.EndWaitForConnection(iar);

                    do
                    {
                        try
                        {
                            int byteCount = 0;
                            byte[] msgBuff = new byte[pipeBufferSize];
                            StringBuilder mb = new StringBuilder();
                            do
                            {
                                byteCount = pipeServer.Read(msgBuff, 0, pipeBufferSize);
                                mb.Append(Encoding.UTF8.GetString(msgBuff, 0, byteCount));
                            } while (!(pipeServer.IsMessageComplete));
                            string message = mb.ToString();
                            if (message == HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE)
                                break;
                            if (message.StartsWith(HealthCheckSettings.PROXY_SETTINGS))
                            {
                                ServerListHelper.instance.UpdateProxy(message);
                            }
                            else if (message.StartsWith(HealthCheckSettings.XENCENTER_METADATA))
                            {
                                ServerListHelper.instance.UpdateXenCenterMetadata(message);
                            }    
                            else
                            {
                                ServerListHelper.instance.UpdateServerCredential(message);
                            }
                        }
                        catch (Exception exp)
                        {
                            log.Error("Receive credential with error:", exp);
                            break;
                        }
                    } while (pipeServer.IsConnected);
                    reInitPipe();
                }
            }
            catch (Exception exp)
            {
                log.Error(exp, exp);
            }
        }
    }
}