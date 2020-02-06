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
using System.Drawing;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class LicenseDataGridViewRow : CheckableDataGridViewRow
    {
        public enum Status
        {
            Warning,
            Information,
            Ok,
            Updating
        }

        private readonly ILicenseStatus licenseStatus;

        public LicenseDataGridViewRow(IXenObject xenObject = null)
            : this(xenObject, new LicenseStatus(xenObject))
        {
        }

        public LicenseDataGridViewRow(IXenObject xenObject, ILicenseStatus status)
            : base(xenObject)
        {
            licenseStatus = status;
            licenseStatus.ItemUpdated += licenseStatus_ItemUpdated;
        }

        public override void BeginCellUpdate()
        {
            licenseStatus.BeginUpdate();
        }

        private void licenseStatus_ItemUpdated()
        {
            Program.Invoke(DataGridView?.Parent, TriggerCellTextUpdatedEvent);
        }

        public override Queue<object> CellText
        {
            get
            {
                bool connectionUp = XenObject != null && XenObject.Connection.IsConnected;
                if(!connectionUp)
                    Disabled = true;

                Queue<object> cellDetails = new Queue<object>();
                cellDetails.Enqueue(XenObject.Name());
                cellDetails.Enqueue(LicenseName);
                cellDetails.Enqueue(new Bitmap(1,1));
                cellDetails.Enqueue(LicenseStatus);
                return cellDetails;
            }
        }

        public bool IsUsingLicenseServer
        {
            get { return !String.IsNullOrEmpty(LicenseServer); }
        }

        private Host XenObjectHost
        {
            get
            {
                var host = XenObject as Host;
                if (host != null)
                    return host;

                var pool = XenObject as Pool;
                if (pool != null)
                    return pool.Connection.Resolve(pool.master);

                return null;
            }
        }

        public override bool CellDataLoaded
        {
            get
            {
                return licenseStatus.Updated;
            }
        }

        public virtual string LicenseName
        {
            get
            {
                if (XenObject == null || XenObject.Connection == null || !XenObject.Connection.IsConnected)
                    return Messages.UNKNOWN;

                // for a pool, get the lowest license
                Pool pool = Helpers.GetPool(XenObject.Connection);
                if (pool != null)
                    return Helpers.GetFriendlyLicenseName(pool);

                if (XenObjectHost != null)
                    return Helpers.GetFriendlyLicenseName(XenObjectHost);

                return Messages.UNKNOWN;
            }
        }

        public override bool WarningRequired
        {
            get
            {
                if (!licenseStatus.Updated)
                    return false;

                if (CurrentLicenseState == Dialogs.LicenseStatus.HostState.Unknown)
                    return false;

                return true;
            }
        }

        public override string WarningText
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.Free:
                        {
                            Pool pool = Helpers.GetPool(XenObjectHost.Connection);
                            if (Dialogs.LicenseStatus.PoolIsMixedFreeAndExpiring(pool))
                                return Messages.POOL_IS_PARTIALLY_LICENSED;
                            return licenseStatus.LicenseEntitlements;
                        }
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                        return Messages.POOL_IS_PARTIALLY_LICENSED;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        {
                            Pool pool = Helpers.GetPool(XenObjectHost.Connection);
                            if (Dialogs.LicenseStatus.PoolHasMixedLicenses(pool))
                                return Messages.POOL_HAS_MIXED_LICENSES;

                            if (Dialogs.LicenseStatus.PoolIsPartiallyLicensed(pool))
                                return Messages.POOL_IS_PARTIALLY_LICENSED;

                            return licenseStatus.LicenseEntitlements;
                        }
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                        return Messages.LICENSE_EXPIRED_NO_LICENSES_AVAILABLE;
                    case Dialogs.LicenseStatus.HostState.Expired:
                        return Messages.LICENSE_YOUR_LICENCE_HAS_EXPIRED;
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        return string.Format(Messages.LICENSE_YOUR_LICENCE_EXPIRES_IN, licenseStatus.LicenseExpiresIn.FuzzyTime());
                    default:
                        return Messages.UNKNOWN;
                }
            }
        }

        public bool HelperUrlRequired
        {
            get { return XenObject != null; }  
        }

        public Status RowStatus
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                    case Dialogs.LicenseStatus.HostState.Expired:
                    case Dialogs.LicenseStatus.HostState.Free:
                        return Status.Warning;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        return Status.Ok;
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        return Status.Information;
                    case Dialogs.LicenseStatus.HostState.Unknown:
                        if (!licenseStatus.Updated)
                            return Status.Updating;
                        return Status.Information;
                    default:
                        return Status.Information;
                }
            }
        }

        public string LicenseServer
        {
            get
            {
                if (licenseStatus.LicenseEdition != Host.Edition.Free)
                {
                    string address = LicenseServerAddress;
                    string port = LicenseServerPort;

                    if(!String.IsNullOrEmpty(port))
                    {
                        return String.Format(Messages.LICENSE_SERVER_PORT_FORMAT, address, port);
                    }

                    return address;
                }
                return string.Empty;
            }
        }

        public string LicenseServerAddress
        {
            get { return LicenseServerDetailKeyedOn("address"); }
        }

        public string LicenseServerPort
        {
            get { return LicenseServerDetailKeyedOn("port"); }
        }

        private string LicenseServerDetailKeyedOn(string key)
        {
            if (licenseStatus.LicenseEdition != Host.Edition.Free)
            {
                if (XenObjectHost.license_server.ContainsKey(key) && !string.IsNullOrEmpty(XenObjectHost.license_server[key]))
                {
                    return XenObjectHost.license_server[key];
                }
            }
            return string.Empty;
        }

        public bool CanUseLicenseServer
        {
            get { return !string.IsNullOrEmpty(XenObjectHost.edition); }
        }

        public bool HasLicenseServer
        {
            get { return !String.IsNullOrEmpty(LicenseServer); }
        }

        public virtual string LicenseProductVersion
        {
            get
            {
                if (XenObject != null && !XenObject.Connection.IsConnected)
                    return String.Empty;

                return XenObjectHost.ProductVersionTextShort();
            }
        }

        public DateTime? LicenseExpires
        {
            get { return licenseStatus.ExpiryDate; }
        }

        public TimeSpan LicenseExpiresIn
        {
            get { return licenseStatus.LicenseExpiresIn; }
        }


        public int NumberOfSockets
        {
            get
            {
                var h = XenObject as Host;
                if (h != null)
                    return h.CpuSockets();

                var p = XenObject as Pool;
                if (p != null)
                    return p.CpuSockets();
                return 0;
            }
        }

        public LicenseStatus.HostState CurrentLicenseState
        {
            get
            {
                if (XenObject != null && !XenObject.Connection.IsConnected)
                    return Dialogs.LicenseStatus.HostState.Unknown;
                
                return licenseStatus.CurrentState;
            }
        }

        public Host.Edition LicenseEdition
        {
            get { return licenseStatus.LicenseEdition; }
        }

        private string LicenseStatus
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                        return Messages.PARTIALLY_LICENSED;
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                    case Dialogs.LicenseStatus.HostState.Expired:
                        return Messages.LICENSE_UNLICENSED;
                    case Dialogs.LicenseStatus.HostState.Free:
                        return Messages.LICENSE_UNLICENSED;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        return Messages.LICENSE_LICENSED;
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        return licenseStatus.LicenseExpiresIn.FuzzyTime();
                    case Dialogs.LicenseStatus.HostState.Unknown:
                        if(!licenseStatus.Updated)
                            return Messages.LICENSE_UPDATING;
                        return Messages.UNKNOWN;
                    default:
                        return Messages.UNKNOWN;
                }
            }
        }

        public List<Host> RepresentedHosts
        {
            get
            {
                List<Host> hosts = new List<Host>();
                if (XenObject is Pool)
                    hosts.AddRange(XenObject.Connection.Cache.Hosts);
                if (XenObject is Host)
                    hosts.Add(XenObject as Host);
                return hosts;
            }
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if(licenseStatus != null)
                        licenseStatus.Dispose();
                }

                disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
