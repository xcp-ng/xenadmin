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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using XenAdmin.Core;
using XenAPI;
using System.Threading;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class ImportVmAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int HTTP_PUT_TIMEOUT = 30*60*1000; //30 minutes

        private string m_filename;
        private readonly Host m_affinity;
        public const string IMPORT_TASK = "import_task";

		private bool m_startAutomatically;
    	private List<VIF> m_VIFs;

    	private object monitor = new object();
		private bool m_wizardDone;

    	/// <summary>
        /// These calls are always required to import a VM, as opposed to the set which are only called if needed set in the constructor of this action.
        /// </summary>
        public readonly static RbacMethodList ConstantRBACRequirements = new RbacMethodList(            
            "vm.set_name_label",
            "network.destroy",
            "vif.create",
            "vif.destroy",
            "http/put_import"
        );

		public ImportVmAction(IXenConnection connection, Host affinity, string filename, SR sr)
			: base(connection, string.Format(Messages.IMPORTVM_TITLE, filename, Helpers.GetName(connection)), Messages.IMPORTVM_PREP)
		{
			Pool = Helpers.GetPoolOfOne(connection);
			m_affinity = affinity;
			Host = affinity ?? connection.Resolve(Pool.master);
			SR = sr;
			VM = null;
			m_filename = filename;

			#region RBAC Dependencies

			ApiMethodsToRoleCheck.AddRange(ConstantRBACRequirements);

			if (affinity != null)
				ApiMethodsToRoleCheck.Add("vm.set_affinity");

			//??
			//if (startAutomatically)
			//	ApiMethodsToRoleCheck.Add("vm.start");

			ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
			ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

			#endregion
		}

    	private string GetVmRef(string result)
        {
            if (string.IsNullOrEmpty(result))
                return null;

            string head = "<value><array><data><value>";
            string tail = "</value></data></array></value>";

            if (!result.StartsWith(head) || !result.EndsWith(tail))
                return null;

            int start = head.Length;
            int length = result.IndexOf(tail) - start;

            return result.Substring(start, length);
        }

        protected override void Run()
        {
            SafeToExit = false;
        	bool isTemplate;

        	try
            {
                string vmRef;

				if (m_filename.EndsWith("ova.xml"))//importing version 1 from of VM
				{
					m_filename = m_filename.Replace("ova.xml", "");
					vmRef = GetVmRef(applyVersionOneFiles());
				}
				else//importing current format of VM
					vmRef = GetVmRef(applyFile());

            	if (Cancelling)
                    throw new CancelledException();

                // Now lets try and set the affinity and start the VM

                if (string.IsNullOrEmpty(vmRef))
                    return;

                while (!Cancelling && (VM = Connection.Resolve(new XenRef<VM>(vmRef))) == null)
                    Thread.Sleep(100);

                if (Cancelling)
                    throw new CancelledException();

                isTemplate = VM.get_is_a_template(Session, vmRef);
                if (isTemplate && Helpers.FalconOrGreater(Connection) && VM.get_is_default_template(Session, vmRef))
                {
                    var otherConfig = VM.get_other_config(Session, vmRef);
                    if (!otherConfig.ContainsKey(IMPORT_TASK) || otherConfig[IMPORT_TASK] != RelatedTask.opaque_ref)
                    {
                        throw new Exception(Messages.IMPORT_TEMPLATE_ALREADY_EXISTS);
                    }
                }

                Description = isTemplate ? Messages.IMPORT_TEMPLATE_UPDATING_TEMPLATE : Messages.IMPORTVM_UPDATING_VM;
                VM.set_name_label(Session, vmRef, DefaultVMName(VM.get_name_label(Session, vmRef)));

				if (!isTemplate && m_affinity != null)
					VM.set_affinity(Session, vmRef, m_affinity.opaque_ref);

                // Wait here for the wizard to finish
                Description = isTemplate ? Messages.IMPORT_TEMPLATE_WAITING_FOR_WIZARD : Messages.IMPORTVM_WAITING_FOR_WIZARD;
				lock (monitor)
				{
					while (!(m_wizardDone || Cancelling))
						Monitor.Wait(monitor);
				}

                if (Cancelling)
                    throw new CancelledException();

                if (m_VIFs != null)
                {
                    Description = isTemplate ? Messages.IMPORT_TEMPLATE_UPDATING_NETWORKS : Messages.IMPORTVM_UPDATING_NETWORKS;

                    // For ElyOrGreater hosts, we can move the VIFs to another network, 
                    // but for older hosts we need to destroy all vifs and recreate them

                    List<XenRef<VIF>> vifs = VM.get_VIFs(Session, vmRef);
                    List<XenAPI.Network> networks = new List<XenAPI.Network>();

                    bool canMoveVifs = Helpers.ElyOrGreater(Connection);

                    foreach (XenRef<VIF> vif in vifs)
                    {
                        // Save the network as we may have to delete it later
                        XenAPI.Network network = Connection.Resolve(VIF.get_network(Session, vif));
                        if (network != null)
                            networks.Add(network);

                        string oldmac = VIF.get_MAC(Session, vif);

                        if (canMoveVifs)
                        {
                            var vifObj = Connection.Resolve(vif);
                            if (vifObj == null)
                                continue;
                            // try to find a matching VIF in the m_proxyVIFs list, based on the device field
                            var matchingProxyVif = m_VIFs.FirstOrDefault(proxyVIF => proxyVIF.device == vifObj.device);
                            if (matchingProxyVif != null && matchingProxyVif.MAC == oldmac)
                            {
                                // move the VIF to the desired network
                                VIF.move(Session, vif, matchingProxyVif.network);
                                // remove matchingProxyVif from the list, so we don't create the VIF again later
                                m_VIFs.Remove(matchingProxyVif); 
                                continue;
                            }
                        }
                        // destroy the VIF, if we haven't managed to move it
                        VIF.destroy(Session, vif);
                    }

                    // recreate VIFs if needed (m_proxyVIFs can be empty, if we moved all the VIFs in the previous step)
                    foreach (VIF vif in m_VIFs)
                    {
                        vif.VM = new XenRef<VM>(vmRef);
                        VIF.create(Session, vif);
                    }

                    // now delete any Networks associated with this task if they have no VIFs

                    foreach (XenAPI.Network network in networks)
                    {
                        if (!network.other_config.ContainsKey(IMPORT_TASK))
                            continue;

                        if (network.other_config[IMPORT_TASK] != RelatedTask.opaque_ref)
                            continue;

                        try
                        {
                            if (XenAPI.Network.get_VIFs(Session, network.opaque_ref).Count > 0)
                                continue;

                            if (XenAPI.Network.get_PIFs(Session, network.opaque_ref).Count > 0)
                                continue;

                            XenAPI.Network.destroy(Session, network.opaque_ref);
                        }
                        catch (Exception e)
                        {
                            log.ErrorFormat("Exception while deleting network {0}. Squashing.", network.Name());
                            log.Error(e, e);
                        }
                    }
                }

                if (!VM.get_is_a_template(Session, vmRef))
                {
                    if (m_startAutomatically)
                    {
                        Description = Messages.IMPORTVM_STARTING;
                        VM.start(Session, vmRef, false, false);
                    }
                }
            }
            catch (CancelledException)
            {
                Description = Messages.CANCELLED_BY_USER;
                throw;
            }

            Description = isTemplate ? Messages.IMPORT_TEMPLATE_IMPORTCOMPLETE : Messages.IMPORTVM_IMPORTCOMPLETE;
        }

        private int VMsWithName(string name)
        {
            int i = 0;

            foreach (VM v in Connection.Cache.VMs)
                if (v.name_label == name)
                    i++;

            return i;
        }

        private string DefaultVMName(string vmName)
        {
            string name = vmName;
            int i = 0;

            if (VMsWithName(vmName) > 1)
                while (VMsWithName(name) > 0)
                {
                    i++;
                    name = string.Format(Messages.NEWVM_DEFAULTNAME, vmName, i);
                }

            return name;
        }

        private List<string> TaskErrorInfo()
        {
            return new List<string>(Task.get_error_info(Session, RelatedTask));
        }

        private long getSize(DirectoryInfo dir, long count)
        {
            long s = count;
            foreach (FileInfo f in dir.GetFiles("*.gz"))
                s += f.Length;

            foreach (DirectoryInfo d in dir.GetDirectories())
                s += getSize(d, s);

            return s;
        }

        private string applyVersionOneFiles()
        {
            RelatedTask = Task.create(Session, "importTask", Messages.IMPORTING);

            try
            {
				long totalSize = getSize(new DirectoryInfo(m_filename), 0);
                long bytesWritten = 0;

                if (totalSize == 0)
                {
                    // We didn't find any .gz files, just bail out here
                    throw new Exception(Messages.IMPORT_INCOMPLETE_FILES);
                }

            	CommandLib.Config config = new CommandLib.Config
            	                           	{
            	                           		hostname = Connection.Hostname,
            	                           		username = Connection.Username,
            	                           		password = Connection.Password
            	                           	};
                
                CommandLib.thinCLIProtocol tCLIprotocol = null;
                int exitCode = 0;
            	tCLIprotocol = new CommandLib.thinCLIProtocol(delegate(string s) { throw new Exception(s); },
            	                                              delegate { throw new Exception(Messages.EXPORTVM_NOT_HAPPEN); },
            	                                              delegate(string s, CommandLib.thinCLIProtocol t) { log.Debug(s); },
            	                                              delegate(string s) { log.Debug(s); },
            	                                              delegate(string s) { log.Debug(s); },
            	                                              delegate { throw new Exception(Messages.EXPORTVM_NOT_HAPPEN); },
            	                                              delegate(int i)
            	                                              	{
            	                                              		exitCode = i;
            	                                              		tCLIprotocol.dropOut = true;
            	                                              	},
            	                                              delegate(int i)
            	                                              	{
            	                                              		bytesWritten += i;
            	                                              		PercentComplete = (int)(100.0*bytesWritten/totalSize);
            	                                              	},
            	                                              config);

                string body = string.Format("vm-import\nsr-uuid={0}\nfilename={1}\ntask_id={2}\n",
											SR.uuid, m_filename, RelatedTask.opaque_ref);
				log.DebugFormat("Importing Geneva-style XVA from {0} to SR {1} using {2}", m_filename, SR.Name(), body);
                CommandLib.Messages.performCommand(body, tCLIprotocol);

                // Check the task status -- Geneva-style XVAs don't raise an error, so we need to check manually.
                List<string> excep = TaskErrorInfo();
                if (excep.Count > 0)
                    throw new Failure(excep);
                
                // CA-33665: We found a situation before were the task handling had been messed up, we should check the exit code as a failsafe
				if (exitCode != 0)
					throw new Failure(new[] {Messages.IMPORT_GENERIC_FAIL});

                return Task.get_result(Session, RelatedTask);
            }
            catch (Exception exn)
            {
                List<string> excep = TaskErrorInfo();
                if (excep.Count > 0)
                    throw new Failure(excep);
                else
                    throw exn;
            }
            finally
            {
                Task.destroy(Session, RelatedTask);
            }
        }

        private string applyFile()
        {
            log.DebugFormat("Importing Rio-style XVA from {0} to SR {1}", m_filename, SR.Name());

            Host host = SR.GetStorageHost();

            string hostURL;
            if (host == null)
            {
                Uri uri = new Uri(Session.Url);
                hostURL = uri.Host;
            }
            else
            {
                log.DebugFormat("SR is not shared -- redirecting to {0}", host.address);
                hostURL = host.address;
            }

            log.DebugFormat("Using {0} for import", hostURL);

            return HTTPHelper.Put(this, HTTP_PUT_TIMEOUT, m_filename, hostURL,
                                  (HTTP_actions.put_ssbbs)HTTP_actions.put_import,
                                  Session.opaque_ref, false, false, SR.opaque_ref);
        }

		public override void RecomputeCanCancel()
		{
		    CanCancel = m_wizardDone;
		}

        protected override void CancelRelatedTask()
        {
            log.InfoFormat("VM/Template import from '{0}' cancelled", m_filename);

			lock (monitor)
			{
			    Monitor.PulseAll(monitor);
			}

            base.CancelRelatedTask();
        }

		public void EndWizard(bool start, List<VIF> vifs)
		{
			m_startAutomatically = start;
			m_VIFs = vifs;

			lock (monitor)
			{
				m_wizardDone = true;
				Monitor.PulseAll(monitor);
			}
		}
    }
}
