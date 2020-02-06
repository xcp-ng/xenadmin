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
using XenServerHealthCheck;
using XenCenterLib;
using System.IO.Pipes;
using XenAdmin.Model;
using NUnit.Framework;

namespace XenAdminTests.HealthCheckTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CredentialTests
    {
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            CredentialReceiver.instance.Init();
            ServerListHelper.instance.Init();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            CredentialReceiver.instance.UnInit();
        }

        [Test]
        public void CredentialOp()
        {
            string HostName = "Host1";
            string UserName = "User1";
            string Password = "password1";

            // Empty list
            ServerListHelper.instance.ClearServerList();
            int conSize = 0;
            List<ServerInfo> con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            //1. Empty credential 
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            string credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, null, null }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            //2. Send credential and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 1);

            //3. Send credential twice and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 1);

            //4. remove credential and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            //5. add long credential size
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            HostName = "Host01234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
            UserName = "User01234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
            Password = "password101234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 1);

            //6. remove long credential size
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);


            //7. send 2 credentials
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            HostName = "host3";
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            HostName = "host4";
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 2);

            //8. remove 2 credentials
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            HostName = "host3";
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            HostName = "host4";
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(ServerListHelper.SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);
        }

        string metadata = "{\"XenCenter\":{\"System\":{\"Version\":\"0.0.0.0\",\"DotNetVersion\":\"4.0.30319.42000\",\"Culture\":\"English (United States)\"," +
                           "\"OsVersion\":\"Microsoft Windows NT 6.1.7601 Service Pack 1\",\"OsCulture\":\"English (United States)\",\"IpAddress\":\"\"}," +
                           "\"Settings\":{\"CFU\":{\"AllowXenCenterUpdates\":true,\"AllowPatchesUpdates\":true,\"AllowXenServerUpdates\":true}," +
                           "\"Proxy\":{\"UseProxy\":true,\"UseIEProxy\":false,\"BypassProxyForServers\":false,\"ProxyAuthentication\":true,\"ProxyAuthenticationMethod\":\"Digest\"}," +
                           "\"SaveAndRestore\":{\"SaveSessionCredentials\":true,\"RequireMasterPassword\":false},\"HelpLastUsed\":\"2017-06-09T11:57:49.4046357Z\"}," +
                           "\"Infrastructure\":{\"TotalConnections\":22,\"Connected\":8},\"Plugins\":[],\"SourceOfData\":\"HealthCheck\",\"Created\":\"2017-07-03 14:24:14Z\"," + 
                           "\"Reported\":\"@HealthCheckReportTime@\"}}";

        [Test]
        public void TestMetadata()
        {
            // Empty server list
            ServerListHelper.instance.ClearServerList();
            int conSize = 0;
            List<ServerInfo> con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            // Send metadata, then check that it has been received and processed and that the server list remained unchanged
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            byte[] metadataBytes = Encoding.UTF8.GetBytes(String.Join(ServerListHelper.SEPARATOR.ToString(), HealthCheckSettings.XENCENTER_METADATA, EncryptionUtils.ProtectForLocalMachine(metadata)));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize); 
            Assert.IsTrue(ServerListHelper.instance.XenCenterMetadata == metadata, "\"Send metadata\" test failed"); 

            // Send metadata containing the separator
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            int separatorPosition = new Random().Next(0, metadata.Length);
            metadata = metadata.Insert(separatorPosition, ServerListHelper.SEPARATOR.ToString());
            metadataBytes = Encoding.UTF8.GetBytes(String.Join(ServerListHelper.SEPARATOR.ToString(), HealthCheckSettings.XENCENTER_METADATA, EncryptionUtils.ProtectForLocalMachine(metadata)));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);
            Assert.IsTrue(ServerListHelper.instance.XenCenterMetadata == metadata, "\"Send metadata containing the separator\" test failed (separator at position {0})", separatorPosition);

            // Send empty metadata
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            metadataBytes = Encoding.UTF8.GetBytes(String.Join(ServerListHelper.SEPARATOR.ToString(), HealthCheckSettings.XENCENTER_METADATA, EncryptionUtils.ProtectForLocalMachine("")));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);
            Assert.IsTrue(ServerListHelper.instance.XenCenterMetadata.Length == 0, "\"Send empty metadata\" test failed");

            // Send metadata unencrypted
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            metadataBytes = Encoding.UTF8.GetBytes(String.Join(ServerListHelper.SEPARATOR.ToString(), HealthCheckSettings.XENCENTER_METADATA, metadata));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);
            Assert.IsTrue(ServerListHelper.instance.XenCenterMetadata.Length == 0, "\"Send metadata unencrypted\" test failed");
            
            // Send something else
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            var someText = "Some random text";
            metadataBytes = Encoding.UTF8.GetBytes(String.Join(ServerListHelper.SEPARATOR.ToString(), "SomethingElse", EncryptionUtils.ProtectForLocalMachine(someText)));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);
            Assert.IsTrue(ServerListHelper.instance.XenCenterMetadata.Length == 0, "\"Send something else\" test failed");
        }
    }
}
