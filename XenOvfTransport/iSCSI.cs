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
using System.IO;
using System.Threading;
using DiscUtils.Iscsi;
using XenAPI;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;


namespace XenOvfTransport
{
    public class iSCSI
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);
        private DiscUtils.Iscsi.Session _iscsisession;
        private ulong _bytescopied;
        private ulong _bytestotal;
        private bool _newscsi;
        private string _pluginrecord = "";
        private Disk iDisk;
        private string _hashAlgorithmName = "SHA1";
        private byte[] _copyHash;
        private byte[] _buffer = new byte[2 * MB];

        private Dictionary<string, string> m_networkArgs = new Dictionary<string, string>();

        #region PUBLIC

		public bool Cancel { get; set; }

		public Action<XenOvfTransportEventArgs> UpdateHandler { get; set; }

        public Disk ScsiDisk
        {
            get
            {
                return iDisk;
            }
        }

        public ulong Position
        {
            get
            {
                return _bytescopied;
            }
        }
        
        public ulong Length
        {
            get
            {
                return _bytestotal;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        public DiskStream Connect(XenAPI.Session xenSession, string vdiuuid, bool read_only)
        {
            StartiScsiTarget(xenSession, vdiuuid, read_only);

            string ipaddress, port, targetGroupTag, username, password;

            if (!TryParsePluginRecordFor(_pluginrecord, "ip", out ipaddress))
                throw new Exception(Messages.ISCSI_ERROR_NO_IPADDRESS);

            TryParsePluginRecordFor(_pluginrecord, "port", out port);
            int ipport = Convert.ToInt32(port);
            TryParsePluginRecordFor(_pluginrecord, "isci_lun", out targetGroupTag);
            TryParsePluginRecordFor(_pluginrecord, "username", out username);
            TryParsePluginRecordFor(_pluginrecord, "password", out password);

            Initiator initiator = new Initiator();
            if (username != null && password != null)
                initiator.SetCredentials(username, password);

            int iSCSIConnectRetry = Properties.Settings.Default.iSCSIConnectRetry;
            bool iSCSIConnected = false;
            
            while (!iSCSIConnected && iSCSIConnectRetry > 0)
            {
				if (Cancel)
					throw new OperationCanceledException();

                try
                {
                    log.DebugFormat("Connecting virtual disk {0}... ", vdiuuid);
                    TargetAddress ta = new TargetAddress(ipaddress, ipport, targetGroupTag);
                    TargetInfo[] targets = initiator.GetTargets(ta);
                    log.InfoFormat("iSCSI.Connect found {0} targets, connecting to: {1}", targets.Length, targets[0].Name);
                    _iscsisession = initiator.ConnectTo(targets[0]);
                    iSCSIConnected = true;
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to connect to VDI {vdiuuid} over iSCSI.", ex);
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                    iSCSIConnectRetry--;
                }
            }

            if (!iSCSIConnected)
                throw new Exception(Messages.ISCSI_ERROR);
            
            long lun = 0;
            try
            {
                LunInfo[] luns = _iscsisession.GetLuns();
                if (_newscsi)
                {
                    string idx;
                    TryParsePluginRecordFor(_pluginrecord, "iscsi_lun", out idx);
                    long lunIdx = Convert.ToInt32(idx);
                    lun = luns[lunIdx].Lun;
                }
                
                log.InfoFormat("iSCSI.Connect found {0} luns, looking for block storage.", luns.Length);
                
                foreach (LunInfo iLun in luns)
                {
                    if (iLun.DeviceType == LunClass.BlockStorage)
                    {
                        if (_newscsi && iLun.Lun == lun)
                            break;

                        lun = iLun.Lun;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Could not determine LUN", e);
                throw;
            }

            log.InfoFormat("iSCSI.Connect, found on lun: {0}", lun);

            try
            {
                iDisk = _iscsisession.OpenDisk(lun);
                // Use our own DiskStream class to workaround a bug in DiscUtils.DiskStream.
                return new DiskStream(_iscsisession, lun, (read_only ? FileAccess.Read : FileAccess.ReadWrite));
            }
            catch (Exception ex)
            {   
                log.Error("Failed to open virtual disk.", ex);
                throw new Exception(Messages.ISCSI_ERROR_CANNOT_OPEN_DISK, ex);
            }
        }
        
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        public void Disconnect(XenAPI.Session xenSession)
        {
            try
            {
                if (iDisk != null)
                    iDisk.Dispose();
                iDisk = null;
            }
            catch (Exception exn)
            {
                log.Debug("Failed to dispose iDisk. Continuing.", exn);
            }

            try
            {
                if (_iscsisession != null)
                    _iscsisession.Dispose();
                _iscsisession = null;
            }
            catch (Exception exn)
            {
                log.Debug("Failed to dispose iScsiSession. Continuing.", exn);
            }

            StopiScsiTarget(xenSession);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        public void Copy(Stream source, Stream destination, string filename, bool shouldHash)
        {
            log.InfoFormat("Started copying {0} bytes to {1} via iSCSI.", source.Length, filename);

            int bytesRead = 0;
            long offset = 0;
            long limit = source.Length;
            _bytestotal = (ulong)source.Length;

            string updatemsg = string.Format(Messages.ISCSI_COPY_PROGRESS, filename);
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, 0, _bytestotal));

            // Create a hash algorithm to compute the hash from separate blocks during the copy.
            using (var hashAlgorithm = System.Security.Cryptography.HashAlgorithm.Create(_hashAlgorithmName))
            {
                while (offset < limit)
                {
                    if (Cancel)
                    {
                        log.InfoFormat("Canceled transfer of virtual disk {0}.", filename);
                        throw new OperationCanceledException(string.Format(Messages.ISCSI_COPY_CANCELLED, filename));
                    }

                    try
                    {
                        bytesRead = source.Read(_buffer, 0, _buffer.Length);

                        if (bytesRead <= 0)
                            break;

                        if (!IsZeros(_buffer))
                        {
                            // This block has content.
                            // Seek the same position in the destination.
                            destination.Seek(offset, SeekOrigin.Begin);

                            destination.Write(_buffer, 0, bytesRead);

                            if ((offset + bytesRead) < limit)
                            {
                                // This is not the last block.
                                // Compute the partial hash.
                                if (shouldHash)
                                    hashAlgorithm.TransformBlock(_buffer, 0, bytesRead, _buffer, 0);
                            }
                        }

                        offset += bytesRead;

                        _bytescopied = (ulong)offset;

                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, _bytescopied, _bytestotal));
                    }
                    catch (Exception ex)
                    {
                        log.Warn("Failed to transfer virtual disk.", ex);
                        var message = string.Format(Messages.ISCSI_COPY_ERROR, filename);
                        throw new Exception(message, ex);
                    }
                }

                if (shouldHash)
                {
                    // It is necessary to call TransformBlock at least once and TransformFinalBlock only once before getting the hash.
                    // If only the last buffer had content, then TransformBlock would not have been called at least once.
                    // So, split the last buffer and hash it even if it is empty.
                    // Note: TransformBlock will accept an "inputCount" that is zero.
                    hashAlgorithm.TransformBlock(_buffer, 0, bytesRead / 2, _buffer, 0);

                    // Compute the final hash.
                    hashAlgorithm.TransformFinalBlock(_buffer, bytesRead / 2, bytesRead / 2 + bytesRead % 2);

                    _copyHash = hashAlgorithm.Hash;
                }
            }

            destination.Flush();
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, _bytescopied, _bytestotal));

            log.InfoFormat("Finished copying {0} bytes to {1} via iSCSI.", source.Length, filename);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        public void Verify(Stream target, string filename)
        {
            log.InfoFormat("Started verifying {0} bytes in {1} after copy via iSCSI.", _bytescopied, filename);

            int bytesRead = 0;
            long offset = 0;
            long limit = (long)_bytescopied;

            string updatemsg = string.Format(Messages.ISCSI_VERIFY_PROGRESS, filename);
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, 0, (ulong)limit));

            // Create a hash algorithm to compute the hash from separate blocks in the same way as Copy().
            using (var hashAlgorithm = System.Security.Cryptography.HashAlgorithm.Create(_hashAlgorithmName))
            {
                while (offset < limit)
                {
                    if (Cancel)
                    {
                        log.Info("Canceled virtual disk verification after export.");
                        throw new OperationCanceledException(Messages.ISCSI_VERIFY_CANCELLED);
                    }

                    try
                    {
                        bytesRead = target.Read(_buffer, 0, _buffer.Length);

                        if (bytesRead <= 0)
                            break;

                        if (!IsZeros(_buffer))
                        {
                            if ((offset + bytesRead) < limit)
                            {
                                // This is not the last block.
                                // Compute the partial hash.
                                hashAlgorithm.TransformBlock(_buffer, 0, bytesRead, _buffer, 0);
                            }
                        }

                        offset += bytesRead;

                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, (ulong)offset, (ulong)limit));
                    }
                    catch (Exception ex)
                    {
                        log.Warn($"Failed to verify virtual disk {filename}.", ex);
                        var message = string.Format(Messages.ISCSI_VERIFY_ERROR, filename);
                        throw new Exception(message, ex);
                    }
                }

                // It is necessary to call TransformBlock at least once and TransformFinalBlock only once before getting the hash.
                // If only the last buffer had content, then TransformBlock would not have been called at least once.
                // So, split the last buffer and hash it even if it is empty.
                // Note: TransformBlock will accept an "inputCount" that is zero.
                hashAlgorithm.TransformBlock(_buffer, 0, bytesRead / 2, _buffer, 0);

                // Compute the final hash.
                hashAlgorithm.TransformFinalBlock(_buffer, bytesRead / 2, bytesRead / 2 + bytesRead % 2);

                // Compare targetHash with copyHash.
                if (!System.Linq.Enumerable.SequenceEqual(_copyHash, hashAlgorithm.Hash))
                {
                    log.Error("The exported virtual disk does not match the source.");
                    throw new Exception(Messages.ISCSI_VERIFY_INVALID);
                }
            }

            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, (ulong)offset, (ulong)limit));

            log.InfoFormat("Finished verifying {0} bytes in {1} after copy via iSCSI.", target.Length, filename);
        }

        public void WimCopy(Stream source, Stream destination, string filename, bool close, ulong fileindex, ulong filecount)
        {
            log.InfoFormat("iSCSI.Copy copying {0} bytes.", source.Length);
            _bytestotal = (ulong)source.Length;
            ulong zerosskipped = 0;
            byte[] block = new byte[2 * MB];
            ulong p = 0;
			
            string updatemsg = string.Format(Messages.ISCSI_WIM_PROGRESS_FORMAT, fileindex, filecount, filename);
			OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, 0, _bytestotal));
            
			while (true)
            {
                try
                {
                    int n = source.Read(block, 0, block.Length);
                    if (n <= 0)
                        break;
                    if (!IsZeros(block))
                    {
                        destination.Seek((long)p, SeekOrigin.Begin);
                        destination.Write(block, 0, n);
                    }
                    else
                    {
                        zerosskipped += (ulong)n;
                    }
                    if (Cancel)
                    {
                        log.WarnFormat("Cancelled transfer of virtual disk {0}.", filename);
                        throw new OperationCanceledException(string.Format(Messages.ISCSI_COPY_CANCELLED, filename));
                    }
                    p += (ulong)n;
                    _bytescopied = p;
                    if (p >= (ulong)source.Length)
                        break;
                    OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, _bytescopied, _bytestotal));
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Warn($"Failed to transfer virtual disk {filename}.", ex);
                    var message = string.Format(Messages.ISCSI_COPY_ERROR, filename);
                    throw new Exception(message, ex);
                }
            }
            destination.Flush();
            if (close)
            {
                if (source != null) source.Close();
                if (destination != null) destination.Close();
            }
			OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, updatemsg, _bytescopied, _bytestotal));
            log.Info("iSCSI.Copy done with copy.");
        }

        /// <summary>
        /// Write a master boot record to the iSCSI device.
        /// </summary>
        /// <param name="mbrstream">a stream containing the MBR</param>
        public void WriteMBR(Stream mbrstream)
        {
            mbrstream.Position = 0;
            byte[] mbr = new byte[mbrstream.Length];
            mbrstream.Read(mbr, 0, (int)mbrstream.Length);
            iDisk.SetMasterBootRecord(mbr);
        }

		/// <summary>
		/// Configure the network settings for the transfer VM
		/// </summary>
		/// <param name="isIpStatic">True if a static IP address is to be used, false if we get IP address through DHCP</param>
		/// <param name="ip">The static IP address to use</param>
		/// <param name="mask">The subnet mask</param>
		/// <param name="gateway">The network gateway</param>
		public void ConfigureTvmNetwork(string networkUuid, bool isIpStatic, string ip, string mask, string gateway)
		{
			m_networkArgs = new Dictionary<string, string>();

			//network_config is "auto", therefore no related arguments need to be added
			//if we set it to "manual", then we should also add:
			//m_networkArgs.Add("network_port", <portValue>);
			//m_networkArgs.Add("network_mac", <macValue>);

			m_networkArgs.Add("network_uuid", networkUuid);

			if (isIpStatic)
			{
				m_networkArgs.Add("network_mode", "manual");
				m_networkArgs.Add("network_ip", ip);
				m_networkArgs.Add("network_mask", mask);
				m_networkArgs.Add("network_gateway", gateway);
			}
			else
			{
				m_networkArgs.Add("network_mode", "dhcp");
			}
		}

        #endregion

        #region PRIVATE

		private void OnUpdate(XenOvfTransportEventArgs e)
		{
			if (UpdateHandler != null)
				UpdateHandler.Invoke(e);
		}

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        private void StartiScsiTarget(XenAPI.Session xenSession, string vdiuuid, bool read_only)
        {
            try
            {
                string host = XenAPI.Session.get_this_host(xenSession, xenSession.opaque_ref);

                // Transfer VM for VDI <uuid>
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("vdi_uuid", vdiuuid);
                args.Add("transfer_mode", "ISCSI");
                args.Add("read_only", read_only ? "true" : "false");
                args.Add("timeout_minutes", "1");

                //Transfer VM IP settings
                foreach (var kvp in m_networkArgs)
                    args.Add(kvp.Key, kvp.Value);

                string record_handle = Host.call_plugin(xenSession, host, "transfer", "expose", args);
                Dictionary<string, string> get_record_args = new Dictionary<string, string>();
                get_record_args["record_handle"] = record_handle;
                _pluginrecord = Host.call_plugin(xenSession, host, "transfer", "get_record", get_record_args);
                _newscsi = true;
            }
            catch (Exception ex)
            {
                log.Error("Failed to start Transfer VM.", ex);
                throw new Exception(Messages.ISCSI_START_ERROR, ex);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging mechanism")]
        private void StopiScsiTarget(XenAPI.Session xenSession)
        {
            try
            {
                string host = XenAPI.Session.get_this_host(xenSession, xenSession.opaque_ref);
                Dictionary<string, string> args = new Dictionary<string, string>();

                string handle;
                if (!TryParsePluginRecordFor(_pluginrecord, "record_handle", out handle))
                {
                    log.Debug("Transfer VM was not started. Will not attempt to shut it down.");
                    return;
                }
                
                args["record_handle"] = handle;
                Host.call_plugin(xenSession, host, "transfer", "unexpose", args);
                log.Debug("iSCSI.StopScsiTarget: iSCSI Target Destroyed.");
            }
            catch (Exception ex)
            {
                log.Warn("Failed to shutdown Transfer VM.", ex);
                throw new Exception(Messages.ISCSI_SHUTDOWN_ERROR, ex);
            }
        }

        private static bool IsZeros(byte[] buff)
        {
            foreach (byte b in buff)
            {
                if (b != 0x0)
                    return false;
            }
            return true;
        }

        private static bool TryParsePluginRecordFor(string rec, string name, out string outValue)
        {
            outValue = null;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            try
            {
                doc.LoadXml(rec);
                foreach (System.Xml.XmlElement n in doc.GetElementsByTagName("transfer_record"))
                {
                    outValue = n.GetAttribute(name);
                    return true;
                }
            }
            catch (System.Xml.XmlException ex)
            {
                log.Debug("Failed to parse the transfer plugin record.", ex);
            }
            return false;
        }

        #endregion
    }
}
