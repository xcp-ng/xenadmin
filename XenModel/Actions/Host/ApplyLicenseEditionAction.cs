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
using System.Linq;
using System.Collections.Generic;
using XenAPI;
using XenAdmin.Alerts;
using System.ComponentModel;
using System.Threading;

namespace XenAdmin.Actions
{
    /// <summary>
    /// This Action is for v6 (Midnight Ride) licensing. It sets the license server details and sets the edition to advanced, enterprise, enterprise-xd, platinum or free.
    /// </summary>
    public class ApplyLicenseEditionAction : AsyncAction
    {
        private readonly IEnumerable<IXenObject> xos;
        private readonly Host.Edition _edition;
        private readonly string _licenseServerAddress;
        private readonly string _licenseServerPort;

        public List<LicenseFailure> LicenseFailures { get; private set; }

        protected readonly Action<List<LicenseFailure>, string> DoOnLicensingFailure;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyLicenseEditionAction"/> class. 
        /// </summary>
        /// <param name="xos">The hosts for which the license should be set.</param>
        /// <param name="edition">The edition the hosts should be set to.</param>
        /// <param name="licenseServerAddress">The license server address.</param>
        /// <param name="licenseServerPort">The license server port.</param>
        /// <param name="DoOnLicensingFailure">The method to call when the licensing fails. This method will show failure details in a dialog</param>
        public ApplyLicenseEditionAction(IEnumerable<IXenObject> xos, Host.Edition edition, string licenseServerAddress, string licenseServerPort,
            Action<List<LicenseFailure>, string> DoOnLicensingFailure)
            : base(null, Messages.LICENSE_UPDATING_LICENSES)
        {
            LicenseFailures = new List<LicenseFailure>();
            Util.ThrowIfEnumerableParameterNullOrEmpty(xos, "xenobjects");

            _edition = edition;
            this.xos = xos;
            _licenseServerAddress = licenseServerAddress;
            _licenseServerPort = licenseServerPort;

            this.DoOnLicensingFailure = DoOnLicensingFailure;

            if (xos != null &&  Host == null && Pool == null && xos.Count() == 1)
            {
                var xo = xos.FirstOrDefault();
                if (xo is Host)
                    Host = xo as Host;
                else if (xo is Pool)
                    Pool = xo as Pool;
            }
        }

        private static void SetLicenseServer(Host host, string licenseServerAddress, string licenseServerPort)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(licenseServerAddress) && !string.IsNullOrEmpty(licenseServerPort))
            {
                d.Add("address", licenseServerAddress);
                d.Add("port", licenseServerPort);
            }

            Host.set_license_server(host.Connection.Session, host.opaque_ref, d);
        }

        protected override void Run()
        {
            Description = Messages.LICENSE_UPDATING_LICENSES;
            foreach (IXenObject xo in xos)
            {
                Connection = xo.Connection;

                if (!Connection.IsConnected)
                {
                    continue;
                }

                Host host = null;
                Pool pool = null;

                if (xo is Host)
                    host = xo as Host;
                if (xo is Pool)
                {
                    pool = xo as Pool;
                    host = xo.Connection.Resolve(pool.master);
                }

                string previousLicenseServerAddress = null;
                string previousLicenseServerPort = null;
                CollectionChangeEventHandler alertsChangeHandler = null;
                string alertText = null;
                object lck = new object();

                if (host != null && host.license_server.ContainsKey("address"))
                {
                    previousLicenseServerAddress = host.license_server["address"];
                }

                if (host != null &&  host.license_server.ContainsKey("port"))
                {
                    previousLicenseServerPort = host.license_server["port"];
                }

                try
                {
                    if (pool != null)
                        pool.Connection.Cache.Hosts.ToList().ForEach(h=>SetLicenseServer(h, _licenseServerAddress, _licenseServerPort));
                    else
                        SetLicenseServer(host, _licenseServerAddress, _licenseServerPort);

                    IXenObject xoClosure = xo;
                    alertsChangeHandler = delegate(object sender, CollectionChangeEventArgs e)
                    {
                        if (e.Action == CollectionChangeAction.Add)
                        {
                            lock (lck)
                            {
                                Alert alert = (Alert)e.Element;
                                Message.MessageType messageType;
                                // if this is a message alert, its Name property will contain the MessageType
                                if (host != null && host.uuid == alert.HostUuid && Enum.TryParse(alert.Name, out messageType))
                                {
                                    switch (messageType)
                                    {
                                        case Message.MessageType.LICENSE_NOT_AVAILABLE:
                                        case Message.MessageType.LICENSE_SERVER_UNREACHABLE:
                                        case Message.MessageType.LICENSE_SERVER_VERSION_OBSOLETE:
                                            alertText = string.Format(Message.FriendlyBody(alert.Name), xoClosure.Name());
                                            break;
                                        case Message.MessageType.GRACE_LICENSE:
                                            alertText = string.Empty;
                                            break;
                                    }
                                }
                            }
                        }
                    };

                    Alert.RegisterAlertCollectionChanged(alertsChangeHandler);

                    if (xo is Host && host != null)
                    {
                        Host.apply_edition(host.Connection.Session, host.opaque_ref, host.GetEditionText(_edition), false);
                    }

                    if (xo is Pool)
                    {
                        Pool.apply_edition(xo.Connection.Session, pool.opaque_ref, xo.Connection.Cache.Hosts.First().GetEditionText(_edition));
                    }

                    Description = Messages.APPLYLICENSE_UPDATED;
                }
                catch (Failure e)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Thread.Sleep(100);

                        lock (lck)
                        {
                            if (alertText != null)
                                break;
                        }
                    }

                    LicenseFailures.Add(new LicenseFailure(host, alertText ?? e.Message));
                    
                    if (pool != null)
                        pool.Connection.Cache.Hosts.ToList().ForEach(h => SetLicenseServer(h, previousLicenseServerAddress, previousLicenseServerPort));
                    else
                        SetLicenseServer(host, previousLicenseServerAddress, previousLicenseServerPort);
                }
                finally
                {
                    Alert.DeregisterAlertCollectionChanged(alertsChangeHandler);
                }
            }

            if (LicenseFailures.Count > 0)
            {
                string exceptionText = LicenseFailures.Count == 1 ? string.Format(Messages.LICENSE_ERROR_1, LicenseFailures[0].Host.Name()) : string.Format(Messages.LICENSE_ERROR_MANY, LicenseFailures.Count, new List<IXenObject>(xos).Count);

                DoOnLicensingFailure?.Invoke(LicenseFailures, exceptionText);
                throw new InvalidOperationException(exceptionText);
            }
        }
    }

    public class LicenseFailure
    {
        public Host Host;
        public string AlertText;

        public LicenseFailure(Host host, string alertText)
        {
            Host = host;
            AlertText = alertText;
        }
    }
}
