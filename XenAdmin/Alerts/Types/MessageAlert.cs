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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Help;
using System.Text.RegularExpressions;
using XenAdmin.Commands;
using XenAdmin.Network;

namespace XenAdmin.Alerts
{
    public class MessageAlert : Alert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public XenAPI.Message Message;
        public IXenObject XenObject;

        private const int DEFAULT_PRIORITY = 0;

        public MessageAlert(XenAPI.Message m)
        {
            Message = m;
            uuid = m.uuid;
            _timestamp = m.timestamp;
            try
            {
                _priority = (int)m.priority;
            }
            catch (OverflowException)
            {
                _priority = DEFAULT_PRIORITY;
            }
            Connection = m.Connection;
            XenObject = Helpers.XenObjectFromMessage(m);

            // TODO: This would be better if there was some way of getting the actual host that the XenObject belongs to
            // Currently if the applies to object is not a host or pool and belongs to a slave it is filtered under the master. 

            Host h = XenObject as Host;
            if (h == null)
                h = Helpers.GetMaster(m.Connection);

            if (h != null)
                HostUuid = h.uuid;
        }

        public override AlertPriority Priority
        {
            get
            {
                if (Enum.IsDefined(typeof(AlertPriority), _priority))
                    return (AlertPriority)_priority;

                return AlertPriority.Unknown;
            }
        }

        public override string AppliesTo
        {
            get
            {
                string name = Helpers.GetName(Helpers.GetPoolOfOne(Connection));
                return !string.IsNullOrEmpty(name) ? name : Message.obj_uuid;
            }
        }

        public override string Description
        {
            get
            {
                // If you add something to this switch statement, be sure to add a corresponding entry to FriendlyNames.
                var typ = Message.Type;
                switch (typ)
                {
                    case XenAPI.Message.MessageType.HA_POOL_DROP_IN_PLAN_EXISTS_FOR:
                    case Message.MessageType.HA_POOL_OVERCOMMITTED:
                        int pef;
                        if (XenObject != null && int.TryParse(Message.body, out pef))
                        {
                            string f = Message.FriendlyBody("ha_pool_drop_in_plan_exists_for-" + (pef == 0 ? "0" : pef == 1 ? "1" : "n"));
                            return string.Format(f, Helpers.GetName(XenObject), pef);
                        }
                        break;

                    // applies to is hosts, vms and pools where only the name is required
                    case Message.MessageType.HA_HEARTBEAT_APPROACHING_TIMEOUT:
                    case Message.MessageType.HA_HOST_FAILED:
                    case Message.MessageType.HA_HOST_WAS_FENCED:
                    case Message.MessageType.HA_PROTECTED_VM_RESTART_FAILED:
                    case Message.MessageType.HA_STATEFILE_APPROACHING_TIMEOUT:
                    case Message.MessageType.HA_STATEFILE_LOST:
                    case Message.MessageType.HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT:
                    case Message.MessageType.LICENSE_DOES_NOT_SUPPORT_POOLING:
                    case Message.MessageType.PBD_PLUG_FAILED_ON_SERVER_START:
                    case Message.MessageType.VM_CLONED:
                    case Message.MessageType.VM_CRASHED:
                    case Message.MessageType.VM_REBOOTED:
                    case Message.MessageType.VM_RESUMED:
                    case Message.MessageType.VM_SHUTDOWN:
                    case Message.MessageType.VM_STARTED:
                    case Message.MessageType.VM_SUSPENDED:
                    case Message.MessageType.METADATA_LUN_BROKEN:
                    case Message.MessageType.METADATA_LUN_HEALTHY:
                    case Message.MessageType.LICENSE_SERVER_UNREACHABLE:
                    case Message.MessageType.LICENSE_SERVER_VERSION_OBSOLETE:
                    case Message.MessageType.GRACE_LICENSE:
                    case Message.MessageType.LICENSE_NOT_AVAILABLE:
                    case Message.MessageType.LICENSE_EXPIRED:
                    case Message.MessageType.LICENSE_SERVER_CONNECTED:
                    case Message.MessageType.LICENSE_SERVER_UNAVAILABLE:
                    case Message.MessageType.HOST_CLOCK_WENT_BACKWARDS:
                    case Message.MessageType.POOL_CPU_FEATURES_UP:
                    case Message.MessageType.POOL_CPU_FEATURES_DOWN:
                    case Message.MessageType.HOST_CPU_FEATURES_UP:
                    case Message.MessageType.HOST_CPU_FEATURES_DOWN:
                    case Message.MessageType.VDI_CBT_RESIZE_FAILED:
                    case Message.MessageType.VDI_CBT_SNAPSHOT_FAILED:
                    case Message.MessageType.VDI_CBT_METADATA_INCONSISTENT:
                    case Message.MessageType.CLUSTER_HOST_FENCING:
                    case Message.MessageType.CLUSTER_HOST_ENABLE_FAILED:
                        if (XenObject != null)
                            return string.Format(FriendlyFormat(), Helpers.GetName(XenObject));
                        break;

                    // object then pool
                    case Message.MessageType.HOST_CLOCK_SKEW_DETECTED:
                    case Message.MessageType.POOL_MASTER_TRANSITION:
                        if (XenObject != null)
                        {
                            Pool pool = Helpers.GetPoolOfOne(XenObject.Connection);
                            if (pool != null)
                                return string.Format(FriendlyFormat(), Helpers.GetName(XenObject), pool.Name());
                        }
                        break;
                    
                    case Message.MessageType.HA_NETWORK_BONDING_ERROR:
                        if (XenObject != null)
                            return string.Format(FriendlyFormat(), GetManagementBondName(), Helpers.GetName(XenObject));
                        break;

                    case Message.MessageType.LICENSE_EXPIRES_SOON:
                        if (XenObject != null)
                        {
                            Host host = XenObject as Host ?? Helpers.GetMaster(Connection);
                            return string.Format(FriendlyFormat(), Helpers.GetName(XenObject));
                        }
                        break;

                    case Message.MessageType.VBD_QOS_FAILED:
                    case Message.MessageType.VCPU_QOS_FAILED:
                    case Message.MessageType.VIF_QOS_FAILED:
                        if (XenObject != null)
                            return string.Format(FriendlyFormat(), "", Helpers.GetName(XenObject));
                        break;
                    
                    case Message.MessageType.EXTAUTH_INIT_IN_HOST_FAILED:
                        if (XenObject != null)
                        {
                            Match m = extAuthRegex.Match(Message.body);
                            return m.Success ? string.Format(FriendlyFormat(), Helpers.GetName(XenObject), m.Groups[1].Value) : "";
                        }
                        break;

                    case Message.MessageType.EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS:
                        if (XenObject != null)
                            return string.Format(FriendlyFormat(), Helpers.GetName(Helpers.GetPoolOfOne(XenObject.Connection)));
                        break;

                    case Message.MessageType.MULTIPATH_PERIODIC_ALERT:
                        if (XenObject != null)
                        {
                            log.InfoFormat("{0} - {1}", Title, Message.body);
                            return extractMultipathCurrentState(Message.body, FriendlyFormat());
                        }
                        break;

                    case Message.MessageType.WLB_CONSULTATION_FAILED:
                        if (XenObject != null)
                        {
                            Pool p = Helpers.GetPoolOfOne(XenObject.Connection);
                            return string.Format(FriendlyFormat(), Helpers.GetName(p), Helpers.GetName(XenObject));
                        }
                        break;

                    case Message.MessageType.WLB_OPTIMIZATION_ALERT:
                        if (XenObject != null)
                        {
                            Match match = wlbOptAlertRegex.Match(Message.body);
                            return match.Success
                                       ? string.Format(FriendlyFormat(), Helpers.GetName(Helpers.GetPoolOfOne(XenObject.Connection)),
                                                       match.Groups[2], match.Groups[1])
                                       : "";
                        }
                        break;

                    case Message.MessageType.PVS_PROXY_NO_CACHE_SR_AVAILABLE:
                        var proxy = XenObject as PVS_proxy;
                        if (proxy != null)
                        {
                            return string.Format(FriendlyFormat(), proxy.VM(), proxy.Connection.Resolve(proxy.site));
                        }
                        break;

                    case Message.MessageType.VMSS_SNAPSHOT_MISSED_EVENT:
                    case Message.MessageType.VMSS_XAPI_LOGON_FAILURE:
                    case Message.MessageType.VMSS_LICENSE_ERROR:
                    case Message.MessageType.VMSS_SNAPSHOT_FAILED:
                    case Message.MessageType.VMSS_SNAPSHOT_SUCCEEDED:
                    case Message.MessageType.VMSS_SNAPSHOT_LOCK_FAILED:
                        VMSS vmss = Helpers.XenObjectFromMessage(Message) as VMSS;
                        var policyAlertVMSS = new PolicyAlert(Message.priority, Message.name, Message.timestamp, Message.body, (vmss == null) ? "" : vmss.Name());
                        return policyAlertVMSS.Text;
                }

                return Message.body;
            }
        }

        private string FriendlyFormat()
        {
            return Message.FriendlyBody(Message.MessageTypeString());
        }

        private static readonly Regex extAuthRegex = new Regex(@"error=(.*)");
        private static readonly Regex multipathRegex = new Regex(@"^.*host=(.*); host-name=.*; current=(\d+); max=(\d+)$");
        private static readonly Regex wlbOptAlertRegex = new Regex(@"severity:(.*) mode:(.*)");

        private string extractMultipathCurrentState(string body, string format)
        {
            /* message body format - if this changes you need to alter this method
            * 
            * Unhealthy paths:
            * [20090511T16:29:22Z] host=foo; host-name=bar; pbd=whiz; scsi_id=pop; current=1; max=2
            * [20090511T16:29:22Z] host=foo; host-name=bar; pbd=whiz; scsi_id=pop; current=1; max=2
            * ....
            * Events received during the last 120 seconds:
            * [20090511T16:29:22Z] host=foo; host-name=bar; pbd=whiz; scsi_id=pop; current=1; max=2
            * [20090511T16:29:22Z] host=foo; host-name=bar; pbd=whiz; scsi_id=pop; current=1; max=2
            * ...
            */
            string[] lines = Message.body.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> currentState = new List<string>();
            if (lines[0] == "Events received during the last 120 seconds:")
            {
                // current state is healthy, past errors have been resolved.
                if (Helpers.IsPool(Message.Connection))
                {
                    return string.Format(PropertyManager.GetFriendlyName("Message.body-multipath_periodic_alert_healthy"),
                        Helpers.GetName(XenObject));
                }
                else
                {
                    return string.Format(PropertyManager.GetFriendlyName("Message.body-multipath_periodic_alert_healthy_standalone"),
                        Helpers.GetName(XenObject));
                }
            }
            // Skip "unhealthy paths" line
            int lineIndex = 1;
            while (lineIndex < lines.Length && lines[lineIndex].StartsWith("["))
            {
                //record all lines that describe the current state
                currentState.Add(lines[lineIndex]);
                lineIndex++;
            }
            if (currentState.Count == 1)
            {
                // Only one host currently unhealthy, describe it's specific min/max paths
                Match m = multipathRegex.Match(currentState[0]);
                if (m.Success)
                {
                    return string.Format(format, Message.Connection.Cache.Find_By_Uuid<Host>(m.Groups[1].Value),
                        m.Groups[2].Value,
                        m.Groups[3].Value);
                }
                return "";
            }
            else
            {
                // Several hosts in pool unhealthy, list their names as a summary
                string output = "";
                foreach (string s in currentState)
                {
                    Match m = multipathRegex.Match(s);
                    if (m.Success)
                    {
                        output = string.Format("{0}, '{1}'", output, Message.Connection.Cache.Find_By_Uuid<Host>(m.Groups[1].Value));
                    }
                }
                return string.Format(PropertyManager.GetFriendlyName("Message.body-multipath_periodic_alert_summary"),
                    Helpers.GetName(XenObject),
                    output);
            }
        }

        private string GetManagementBondName()
        {
            Bond bond = NetworkingHelper.GetMasterManagementBond(Connection);
            return bond == null ? Messages.UNKNOWN : bond.Name();
        }

        public override Action FixLinkAction
        {
			get
			{
				if (XenObject == null)
					return null;

			    var typ = Message.Type;
				switch (typ)
				{
					case Message.MessageType.HA_HEARTBEAT_APPROACHING_TIMEOUT:
					case Message.MessageType.HA_HOST_FAILED:
					case Message.MessageType.HA_HOST_WAS_FENCED:
					case Message.MessageType.HA_NETWORK_BONDING_ERROR:
					case Message.MessageType.HA_POOL_DROP_IN_PLAN_EXISTS_FOR:
					case Message.MessageType.HA_POOL_OVERCOMMITTED:
					case Message.MessageType.HA_PROTECTED_VM_RESTART_FAILED:
					case Message.MessageType.HA_STATEFILE_APPROACHING_TIMEOUT:
					case Message.MessageType.HA_STATEFILE_LOST:
					case Message.MessageType.HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT:
						return () => new HACommand(Program.MainWindow, XenObject.Connection).Execute();

					case Message.MessageType.LICENSE_EXPIRES_SOON:
					case Message.MessageType.LICENSE_DOES_NOT_SUPPORT_POOLING:
                        return () => Program.OpenURL(HiddenFeatures.LinkLabelHidden ? null : InvisibleMessages.LICENSE_EXPIRY_WEBPAGE);
					case Message.MessageType.VBD_QOS_FAILED:
					case Message.MessageType.VCPU_QOS_FAILED:
					case Message.MessageType.VIF_QOS_FAILED:
						return () => Program.MainWindow.LaunchLicensePicker("");

					case Message.MessageType.MULTIPATH_PERIODIC_ALERT:
						return Program.ViewLogFiles;

					case Message.MessageType.PBD_PLUG_FAILED_ON_SERVER_START:
						var repairSrCommand = new RepairSRCommand(Program.MainWindow, XenObject.Connection.Cache.SRs);
						if (repairSrCommand.CanExecute())
							return () => repairSrCommand.Execute();
						return null;
					default:
						return null;
				}
			}
        }

        public override string FixLinkText
        {
			get
			{
				if (XenObject == null)
					return "";
				if (FixLinkAction == null)
					return null;

				return Message.FriendlyAction(Message.MessageTypeString()) ?? Messages.DETAILS;
			}
        }

        public override string HelpID
        {
            get
            {
                string pageref = "MessageAlert_" + Message.Type.ToString();
                return HelpManager.GetID(pageref) == null ? null : pageref;
            }
        }

        public override string HelpLinkText
        {
            get
            {
                return Message.FriendlyHelp(Message.MessageTypeString());
            }
        }

        public override string Title
        {
            get
            {
                string title = Message.FriendlyName(Message.MessageTypeString());
                if (string.IsNullOrEmpty(title))
                    title = Message.name;

                if (Message.cls != cls.Pool)
                {
                    Host host = XenObject as Host;
                    if (host == null || Helpers.IsPool(host.Connection))
                    {
                        string name = Helpers.GetName(XenObject);
                        if (!string.IsNullOrEmpty(name))
                            title = string.Format(Messages.MESSAGE_ALERT_TITLE, name, title);
                    }
                }

                return title;
            }
        }

        public override string Name
        {
            get { return Message.MessageTypeString(); }
        }

        public override void Dismiss()
        {
            new DestroyMessageAction(Message.Connection, Message.opaque_ref).RunAsync();
            base.Dismiss();
        }

        public override void DismissSingle(Session s)
        {
            Message.destroy(s, Message.opaque_ref);
            base.Dismiss();
        }

        /// <summary>
        /// Find the MessageAlert corresponding to the given Message, or null if none exists.
        /// </summary>
        /// <param name="m"></param>
        public static Alert FindAlert(Message m)
        {
            return FindAlert(a => a is MessageAlert &&
                                  ((MessageAlert)a).Message.opaque_ref == m.opaque_ref &&
                                  m.Connection == a.Connection);
        }

        public static void RemoveAlert(Message m)
        {
            Alert a = FindAlert(m);
            if (a != null)
                RemoveAlert(a);
        }

        /// <summary>
        /// Parses a XenAPI.Message into an Alert object.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Alert ParseMessage(Message msg)
        {
            if (msg.IsPerfmonAlarm())
            {
                return new AlarmMessageAlert(msg);
            }

            // For all other kinds of alert
            return new MessageAlert(msg);
        }
    }
}
