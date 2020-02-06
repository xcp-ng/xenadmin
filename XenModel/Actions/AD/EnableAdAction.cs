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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;


namespace XenAdmin.Actions
{
    public class EnableAdAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string domain;
        private readonly string user;
        private readonly string password;

        public EnableAdAction(IXenConnection connection, string domain, string user, string password)
            : base(connection, string.Format(Messages.ENABLING_AD_ON, Helpers.GetName(connection).Ellipsise(50)), Messages.ENABLING_AD, false)
        {
            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("domain");
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException("user");
            if (password == null)
                throw new ArgumentNullException("password");

            var pool = Helpers.GetPool(connection);
            if (pool != null)
                Pool = pool;
            else
                Host = Helpers.GetMaster(connection);

            this.domain = domain;
            this.user = user;
            this.password = password;
        }

        private static Regex AuthFailedReg = new Regex(@"^([1-9]+) \(0x.*\).*");

        protected override void Run()
        {
            log.DebugFormat("Enabling AD on pool '{0}'", Helpers.GetName(Connection));

            Dictionary<string, string> config = new Dictionary<string, string>();
            config["domain"] = domain; // NB this line is now redundant, it is here to support the old now-superseded way of passing in the domain
            config["user"] = user;
            config["pass"] = password;
            try
            {
                var pool = Helpers.GetPoolOfOne(Connection);
                try
                {
                    //CA-48122: Call disable just in case it was not disabled properly
                    Pool.disable_external_auth(Session, pool.opaque_ref, new Dictionary<string, string>());
                }
                catch (Exception ex)
                {
                    log.Debug("Tried to disable AD before enabling it, but it has failed. Ignoring it, because in this case we are executing disable on best effort basis only.", ex);
                }

                Pool.enable_external_auth(Session, pool.opaque_ref, config, domain, Auth.AUTH_TYPE_AD);
            }
            catch (Failure f)
            {
                // CA-37255 CA-38369 CA-39485
                // We can get errors from likewise that correspond to an error in WinError.h
                // By and large they are useless to the user so we log the details for support and show something more friendly.
                if (f.ErrorDescription[0] == Failure.AUTH_ENABLE_FAILED && f.ErrorDescription.Count > 2)
                {

                    Match m = AuthFailedReg.Match(f.ErrorDescription[2]);
                    if (!m.Success)
                        throw f;

                    int errorId;
                    if (!int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out errorId))
                        throw f;

                    log.Error($"Received errorId {errorId} from likewise when attempting to join domain.", f);
                }
                XenRef<Host> hostref = new XenRef<Host>(f.ErrorDescription[1]);
                Host host = Connection.Resolve(hostref);
                if (host == null)
                    throw;
                if (f.ErrorDescription[0] == Failure.POOL_AUTH_ENABLE_FAILED_WRONG_CREDENTIALS)
                    throw new CredentialsFailure(f.ErrorDescription);
                
                throw new Exception(string.Format(Messages.AD_FAILURE_WITH_HOST, f.Message, host.Name()));
            }
            Description = Messages.COMPLETED;
        }

        /// <summary>
        /// Exception thrown when enabling AD authentication fails due to wrong supplied credentials
        /// </summary>
        public class CredentialsFailure : Failure
        {
            public CredentialsFailure(List<string> err)
                : base(err)
            {
            }
        }
    }
}
