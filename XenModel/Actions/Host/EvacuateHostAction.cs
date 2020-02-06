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
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;

using XenAdmin.Actions.HostActions;

namespace XenAdmin.Actions
{
    public class EvacuateHostAction:HostAbstractAction
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<XenRef<VM>, string[]> _hostRecommendations;
        private readonly Host _newMaster;

        /// <summary>
        /// 
        /// NOTE: when creating new HostActions, add Program.MainWindow.action_Completed to the completed event,
        /// and call Program.MainWindow.UpdateToolbars() after starting the action. This ensures the toolbar
        /// buttons are disabled while the action is in progress.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="kind"></param>
        /// <param name="host">Must not be null.</param>
        /// <param name="acceptNTolChanges"></param>
        /// <param name="acceptNTolChangesOnEnable"></param>
        public EvacuateHostAction(Host host, Host newMaster, Dictionary<XenRef<VM>, String[]> hostRecommendations, Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges, Func<Pool, Host, long, long, bool> acceptNTolChangesOnEnable) 
            : base(host.Connection, null, Messages.HOST_EVACUATE, acceptNTolChanges, acceptNTolChangesOnEnable)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            Host = host;
            _newMaster = newMaster;
            _hostRecommendations = hostRecommendations;
        }

        protected override void Run()
        {
            bool isMaster = Host.IsMaster();

            try
            {
                this.Description = String.Format(Messages.HOSTACTION_EVACUATING, Helpers.GetName(Host));

                // call "MaybeReduceNtolBeforeOp", 
                // if currentNtol > targetNtol, asks users whether to decrease ntol (since disable will fail if it would cause HA overcommit),
                // if users don't cancel, puts MAINTENANCE_MODE=true into the host's other_config, 
                // then does a Host.disable using XenAPI.Host.async_disable.
                // Parameters 0 and 20 are for scaling low and high values for progress bar
                Disable(0, 20);


                bool tryAgain = false;

                // WLB: use non-wlb evcaute when wlb is not enabled
                if (Helpers.WlbEnabled(Host.Connection))
                {
                    //  WLB: get wlb evacuate recommendations
                    //Dictionary<XenRef<VM>, String[]> hostRecommendations = XenAPI.Host.retrieve_wlb_evacuate_recommendations(Session, Host.opaque_ref);

                    if (_hostRecommendations != null && _hostRecommendations.Count > 0)
                    {
                        List<string> error = new List<string>();

                        // WLB: continue only if there are no errors in wlb evacuate recommendations
                        if (NoRecommendationError(_hostRecommendations, out error))
                        {
                            int start = 20;
                            int each = (isMaster ? 80 : 90) / _hostRecommendations.Count;

                            IEnumerable<WlbHostEvacuationRecommendation> sortedRecommendations = SortedHostRecommendations(_hostRecommendations);

                            foreach (WlbHostEvacuationRecommendation rec in sortedRecommendations)
                            {
                                if (string.Compare(rec.Label, "wlb", true) == 0)
                                {
                                    Host toHost = Host.Connection.Cache.Find_By_Uuid<Host>(rec.HostUuid);
                                    if ((Session.Connection.Resolve(rec.Vm)).is_control_domain)
                                    {
                                        if (!toHost.IsLive())
                                        {
                                            try
                                            {
                                                new HostPowerOnAction(toHost).RunExternal(Session);
                                            }
                                            catch (Exception)
                                            {
                                                Description = string.Format(Messages.ACTION_HOST_START_FAILED, Helpers.GetName(toHost));
                                            }
                                            if (!toHost.enabled)
                                            {
                                                RelatedTask = XenAPI.Host.async_enable(Session, toHost.opaque_ref);
                                                PollToCompletion(start, start);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // sometimes, the SR is not available after host power-on, so give three try                                              
                                        int retry = 3;
                                        while (retry > 0)
                                        {
                                            try
                                            {
                                                RelatedTask = XenAPI.VM.async_live_migrate(Session, rec.Vm.opaque_ref, toHost.opaque_ref);
                                                PollToCompletion(start, start + each);
                                                start += each;
                                                break;
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex.Message, ex);

                                                // sleep for 10s, then try again
                                                System.Threading.Thread.Sleep(10 * 1000);
                                                retry--;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // WLB: don't evacuate when there is errors in wlb evacuate recommendations
                            throw new XenAPI.Failure(error);
                        }
                    }
                    else
                    {
                        // WLB: when there is no wlb evacuate recommendations, fall through to use the non-WLB evacuate.
                        tryAgain = true;
                    }
                }

                if (!Helpers.WlbEnabled(Host.Connection) || tryAgain)
                {
                    RelatedTask = XenAPI.Host.async_evacuate(Session, Host.opaque_ref);
                    PollToCompletion(20, isMaster ? 80 : 90);
                }

                this.Description = String.Format(Messages.HOSTACTION_EVACUATED, Helpers.GetName(Host));

                if (isMaster && _newMaster != null)
                {
                    // Signal to the connection that the master is going to change underneath us.
                    Connection.MasterMayChange = true;

                    //Transition to new master
                    this.Description = String.Format(Messages.HOSTACTION_TRANSITIONING_NEW_MASTER, Helpers.GetName(_newMaster));

                    try
                    {
                        RelatedTask = XenAPI.Pool.async_designate_new_master(Session, _newMaster.opaque_ref);
                        PollToCompletion(80, 90);
                    }
                    catch
                    {
                        // If theres an error during designate new master, clear flag to prevent leak.
                        Connection.MasterMayChange = false;
                        throw;
                    }

                    this.Description = String.Format(Messages.HOSTACTION_TRANSITIONED_NEW_MASTER, Helpers.GetName(_newMaster));
                }

                this.PercentComplete = 100;
            }
            catch (Exception e)
            {
                log.Error($"There was an exception putting the host {Host.opaque_ref} into maintenance mode. Removing other_config key.", e);
                Enable(isMaster ? 80 : 90, 100, false);
                throw;
            }
        }
        /// <summary>
        /// Check whether there is  error in wlb evacaute recommendations, return true if there is no errors
        /// </summary>
        /// <param name="hostRecommendations">evcaute recommendations</param>
        /// <param name="error">output error if there is at least one</param>
        /// <returns>false if there is at least one error in wlbevacute  recommendations</returns>
        private static bool NoRecommendationError(Dictionary<XenRef<VM>, String[]> hostRecommendations, out List<string> error)
        {
            bool noError = true;
            error = new List<string>();

            foreach (KeyValuePair<XenRef<VM>, string[]> rec in hostRecommendations)
            {
                if (string.Compare(rec.Value[0].Trim(), "wlb", true) != 0)
                {
                    error.Add(rec.Value[0]);
                    error.Add(rec.Value[1]);
                    noError = false;
                    break;
                }
            }
            return noError;
        }

        /// <summary>
        /// Puts MAINTENANCE_MODE=true into the host's other_config, then does a Host.disable.
        /// If appropriate, first asks the user if they want to decrease ntol (since disable will fail if it would cause HA overcommit).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        private void Disable(int start, int finish)
        {
            // ask users if they want to decrease ntol, may throw CancelledException if the user says no.
            MaybeReduceNtolBeforeOp(HostActionKind.Evacuate);

            RelatedTask = XenAPI.Host.async_disable(Session, Host.opaque_ref);

            PollToCompletion(start, finish);

            XenAPI.Host.remove_from_other_config(Session, Host.opaque_ref, XenAPI.Host.MAINTENANCE_MODE);
            XenAPI.Host.add_to_other_config(Session, Host.opaque_ref, XenAPI.Host.MAINTENANCE_MODE, "true");
        }

        /// <summary>
        /// Sort the WlbHostEvacuationRecommendation, so host powerOn always on the top, vmMoves is after host powerOn.
        /// </summary>
        /// <param name="hostRecommendations">A instance of raw WlbHostEvacuationRecommendation dictionary.</param>
        /// <returns>A list of WlbHostEvacuationRecommendation.</returns>
        private IEnumerable<WlbHostEvacuationRecommendation> SortedHostRecommendations(Dictionary<XenRef<VM>, String[]> hostRecommendations)
        {
            List<WlbHostEvacuationRecommendation> hostPowerOnRecs = new List<WlbHostEvacuationRecommendation>();
            List<WlbHostEvacuationRecommendation> vmMoveRecs = new List<WlbHostEvacuationRecommendation>();
            List<WlbHostEvacuationRecommendation> sortedRecs = new List<WlbHostEvacuationRecommendation>();

            foreach (KeyValuePair<XenRef<VM>, string[]> rec in hostRecommendations)
            {
                Host toHost = Host.Connection.Cache.Find_By_Uuid<Host>(rec.Value[(int)RecProperties.ToHost]);
                if (string.Compare(rec.Value[(int)RecProperties.WLB], "wlb", true) == 0)
                {
                    WlbHostEvacuationRecommendation hostEvacuatRec = new WlbHostEvacuationRecommendation();
                    hostEvacuatRec.Label = rec.Value[(int)RecProperties.WLB];
                    hostEvacuatRec.Vm = rec.Key;
                    hostEvacuatRec.HostUuid = rec.Value[(int)RecProperties.ToHost];
                    if ((Session.Connection.Resolve(rec.Key)).is_control_domain && !toHost.IsLive())
                    {
                        hostPowerOnRecs.Add(hostEvacuatRec);
                    }
                    else
                    {
                        vmMoveRecs.Add(hostEvacuatRec);
                    }
                }
            }

            foreach (WlbHostEvacuationRecommendation hpoRec in hostPowerOnRecs)
            {
                sortedRecs.Add(hpoRec);
            }

            foreach (WlbHostEvacuationRecommendation vmRec in vmMoveRecs)
            {
                sortedRecs.Add(vmRec);
            }

            return sortedRecs;
        }

    }
}
