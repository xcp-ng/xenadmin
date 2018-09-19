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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public abstract class PoolAbstractAction : AsyncAction
    {
        protected Func<Host, AdUserAndPassword> GetAdCredentials;
        protected Func<HostAbstractAction, Pool, long, long, bool> AcceptNTolChanges;
        protected Action<List<LicenseFailure>, string> DoOnLicensingFailure;

        protected PoolAbstractAction(IXenConnection connection, string title, Func<Host, AdUserAndPassword> getAdCredentials,
            Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges, Action<List<LicenseFailure>, string> doOnLicensingFailure)
            : base(connection, title)
        {
            this.GetAdCredentials = getAdCredentials;
            this.AcceptNTolChanges = acceptNTolChanges;
            this.DoOnLicensingFailure = doOnLicensingFailure;
        }

        protected void ClearAllDelegates()
        {
            GetAdCredentials = null;
            AcceptNTolChanges = null;
            DoOnLicensingFailure = null;
        }

        protected static void FixLicensing(Pool pool, List<Host> hostsToRelicense, Action<List<LicenseFailure>, string> doOnLicensingFailure)
        {
            if (hostsToRelicense.Count == 0)
                return;

            Host poolMaster = Helpers.GetMaster(pool);
            AsyncAction action = new ApplyLicenseEditionAction(hostsToRelicense.ConvertAll(h=>h as IXenObject), Host.GetEdition(poolMaster.edition), poolMaster.license_server["address"], poolMaster.license_server["port"], 
                doOnLicensingFailure);
            action.RunExternal(null);
        }

        /// <summary>
        /// Mask the CPUs of any slaves that need masking to join the pool
        /// </summary>
        /// <returns>Whether any CPUs were masked</returns>
        protected static bool FixCpus(Pool pool, List<Host> hostsToCpuMask, Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges)
        {
            if (hostsToCpuMask.Count == 0)
                return false;

            Host poolMaster = Helpers.GetMaster(pool);
            List<RebootHostAction> rebootActions = new List<RebootHostAction>();

            // Mask the CPUs, and reboot the hosts (simultaneously, as they must all be on separate connections)
            foreach (Host host in hostsToCpuMask)
            {
                // No CPU masking is needed for Dundee or greater hosts 
                if (Helpers.DundeeOrGreater(host))
                {
                    System.Diagnostics.Trace.Assert(false, "No CPU masking should be done for Dundee or greater hosts");
                    continue;
                }

                Host.set_cpu_features(host.Connection.Session, host.opaque_ref, poolMaster.cpu_info["features"]);
                RebootHostAction action = new RebootHostAction(host, acceptNTolChanges);
                rebootActions.Add(action);
                action.RunAsync();
            }

            // Wait for all the actions to finish, checking every ten seconds
            while (true)
            {
                bool done = true;
                foreach (RebootHostAction action in rebootActions)
                {
                    if (!action.IsCompleted)
                        done = false;
                }
                if (done)
                    break;
                System.Threading.Thread.Sleep(10000);
            }

            return true;
        }

        /// <summary>
        /// If we're joining a pool that has a non-shared default/crash/suspend SR, then clear that
        /// pool's default SRs, since a pool with default SRs set to local storage is a confusing
        /// configuration that we do not allow to be set through the GUI (only shared SRs can be set
        /// as the default in pools, even though xapi allows otherwise).
        /// </summary>
        /// <param name="pool"></param>
        protected void ClearNonSharedSrs(Pool pool)
        {
            SR defSR = pool.Connection.Resolve<SR>(pool.default_SR);

            if (defSR != null && !defSR.shared)
            {
                XenAPI.Pool poolCopy = (Pool)pool.Clone();
                poolCopy.default_SR = new XenRef<SR>(Helper.NullOpaqueRef);
                poolCopy.crash_dump_SR = new XenRef<SR>(Helper.NullOpaqueRef);
                poolCopy.suspend_image_SR = new XenRef<SR>(Helper.NullOpaqueRef);
                pool.Locked = true;
                try
                {
                    poolCopy.SaveChanges(Session);
                }
                finally
                {
                    pool.Locked = false;
                }
            }
        }


        protected static void FixAd(Pool pool, List<Host> hostsToAdConfigure, Func<Host, AdUserAndPassword> getAdCredentials)
        {
            if (hostsToAdConfigure.Count == 0)
                return;

            Host poolMaster = Helpers.GetMaster(pool);
            AsyncAction action;

            bool success = true;
            do
            {
                success = true;
                AdUserAndPassword adUserAndPassword = getAdCredentials(poolMaster);

                try
                {
                    foreach (Host h in hostsToAdConfigure)
                    {
                        action = new EnableAdAction(h.Connection, poolMaster.external_auth_service_name,
                                adUserAndPassword.Username, adUserAndPassword.Password)
                            {Host = h};
                        action.RunExternal(null);
                    }
                }
                catch (EnableAdAction.CredentialsFailure)
                {
                    success = false;
                }
            } while (!success);
        }

        
        public class AdUserAndPassword
        {
            public AdUserAndPassword(string username,string password)
            {
                Username = username;
                Password = password;

            }
            public readonly string Username;
            public readonly string Password;
        }
    }
}
