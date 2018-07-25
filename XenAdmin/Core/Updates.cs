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
using System.ComponentModel;
using System.Linq;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Network;
using System.Diagnostics;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Text;
using XenCenterLib;

namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static event Action<bool, string> CheckForUpdatesCompleted;
        public static event Action CheckForUpdatesStarted;
        public static event Action RestoreDismissedUpdatesStarted;

        private static readonly object downloadedUpdatesLock = new object();
        private static List<XenServerVersion> XenServerVersionsForAutoCheck = new List<XenServerVersion>();
        private static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();
        private static List<XenCenterVersion> XenCenterVersions = new List<XenCenterVersion>();
        public static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();

        private static readonly object updateAlertsLock = new object();
        private static readonly ChangeableList<Alert> updateAlerts = new ChangeableList<Alert>();
        
        public static IEnumerable<Alert> UpdateAlerts
        {
            get { return updateAlerts; }
        }

        public static int UpdateAlertsCount
        {
            get { return updateAlerts.Count; }
        }

        private static void AddUpate(Alert update)
        {
            try
            {
                lock (updateAlertsLock)
                {
                    if(!updateAlerts.Contains(update))
                    {
                        updateAlerts.Add(update);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Failed to add update", e);
            }
        }

        public static void RemoveUpdate(Alert update)
        {
            try
            {
                lock (updateAlertsLock)
                {
                    if(updateAlerts.Contains(update))
                    {
                        updateAlerts.Remove(update);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Failed to remove update", e);
            }
        }

        /// <summary>
        /// Dismisses the updates in the given list i.e. they are added in the 
        /// other_config list of each pool and removed from the Updates.UpdateAlerts list.
        /// </summary>
        /// <param name="toBeDismissed"></param>
        public static void DismissUpdates(List<Alert> toBeDismissed)
        {
            if (toBeDismissed.Count == 0)
                return;

            foreach(IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!Alert.AllowedToDismiss(connection))
                    continue;

                XenAPI.Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool == null)
                    continue;

                Dictionary<string, string> other_config = pool.other_config;

                foreach (Alert alert in toBeDismissed)
                {
                  
                    if (alert is XenServerPatchAlert)
                    {

                        if (other_config.ContainsKey(IgnorePatchAction.IgnorePatchKey))
                        {
                            List<string> current = new List<string>(other_config[IgnorePatchAction.IgnorePatchKey].Split(','));
                            if (current.Contains(((XenServerPatchAlert)alert).Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                                continue;
                            current.Add(((XenServerPatchAlert)alert).Patch.Uuid);
                            other_config[IgnorePatchAction.IgnorePatchKey] = string.Join(",", current.ToArray());
                        }
                        else
                        {
                            other_config.Add(IgnorePatchAction.IgnorePatchKey, ((XenServerPatchAlert)alert).Patch.Uuid);
                        }
                    }
                    if (alert is XenServerVersionAlert)
                    {

                        if (other_config.ContainsKey(IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY))
                        {
                            List<string> current = new List<string>(other_config[IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                            if (current.Contains(((XenServerVersionAlert)alert).Version.VersionAndOEM))
                                continue;
                            current.Add(((XenServerVersionAlert)alert).Version.VersionAndOEM);
                            other_config[IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY] = string.Join(",", current.ToArray());
                        }
                        else
                        {
                            other_config.Add(IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY, ((XenServerVersionAlert)alert).Version.VersionAndOEM);
                        }                       
                    }
                    Updates.RemoveUpdate(alert);
                }

                XenAPI.Pool.set_other_config(connection.Session, pool.opaque_ref, other_config);
            }
        }

        private static Alert FindUpdate(Alert alert)
        {
            lock (updateAlertsLock)
                return FindUpdate(a => a.Equals(alert));
        }

        private static Alert FindUpdate(Predicate<Alert> predicate)
        {
            lock (updateAlertsLock)
                return updateAlerts.Find(predicate);
        }


        /// <summary>
        /// If AutomaticCheck is enabled it checks for updates regardless the
        /// value of the parameter force. If AutomaticCheck is disabled it 
        /// checks for all update types if force is true; forceRefresh causes 
        /// the check for update action to run and refresh the Updates page
        /// </summary>
        public static void CheckForUpdates(bool force, bool forceRefresh = false)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            if (Properties.Settings.Default.AllowXenCenterUpdates ||
                Properties.Settings.Default.AllowXenServerUpdates ||
                Properties.Settings.Default.AllowPatchesUpdates || force || forceRefresh)
            {
                var action = CreateDownloadUpdatesXmlAction(
                    CheckForUpdatesUrl,
                    Properties.Settings.Default.AllowXenCenterUpdates || force,
                    Properties.Settings.Default.AllowXenServerUpdates || force,
                    Properties.Settings.Default.AllowPatchesUpdates || force);

                action.Completed += actionCompleted;

                if (CheckForUpdatesStarted != null)
                    CheckForUpdatesStarted();

                action.RunAsync();
            }
        }

        public static DownloadUpdatesXmlAction CreateDownloadUpdatesXmlAction(string checkForUpdatesUrl, bool checkForXenCenter = false, bool checkForServerVersion = false, bool checkForPatches = false)
        {
            string userAgent = string.Format("{0}/{1}.{2} ({3}-bit)", Branding.BRAND_CONSOLE, Branding.XENCENTER_VERSION, Program.Version.Revision.ToString(), IntPtr.Size * 8);
            string userAgentId = GetUniqueIdHash();

            return new DownloadUpdatesXmlAction(checkForXenCenter, checkForServerVersion, checkForPatches, userAgent, userAgentId, checkForUpdatesUrl);
        }

        internal static string GetUniqueIdHash()
        {
            string uniqueIdHash = "nil";

            try
            {
                var managementObj = new System.Management.ManagementObject("Win32_OperatingSystem=@");
                string serialNumber = (string)managementObj["SerialNumber"];

                if (!string.IsNullOrWhiteSpace(serialNumber))
                {
                    var serialBytes = Encoding.ASCII.GetBytes(serialNumber);

                    using (var md = new System.Security.Cryptography.MD5CryptoServiceProvider()) // MD5 to keep it short enough as this hash is not used for security in any way
                    {
                        var hash = md.ComputeHash(serialBytes);
                        uniqueIdHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return uniqueIdHash;
        }

        /// <summary>
        /// It does exactly what CheckForUpdates(true) does, but this is sync and shows an ActionProgressDialog while running
        /// </summary>
        /// <returns>true if the action has succeeded</returns>
        public static bool CheckForUpdatesSync(Control parentForProgressDialog)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return false;

            var action = CreateDownloadUpdatesXmlAction(CheckForUpdatesUrl, true, true, true);
            action.Completed += actionCompleted;

            if (CheckForUpdatesStarted != null)
                CheckForUpdatesStarted();

            if (parentForProgressDialog != null)
            {
                using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    dialog.ShowDialog(parentForProgressDialog);
            }
            else
            {
                action.RunExternal(action.Session);
            }

            return action.Succeeded;
        }

        public static string CheckForUpdatesUrl
        {
            get
            {
                return Registry.CustomUpdatesXmlLocation ?? Branding.CheckForUpdatesUrl;
            }
        }

        private static void actionCompleted(ActionBase sender)
        {
            Program.AssertOffEventThread();

            DownloadUpdatesXmlAction action = sender as DownloadUpdatesXmlAction;
            if (action == null)
                return;

            bool succeeded = action.Succeeded;
            string errorMessage = string.Empty;

            lock (updateAlertsLock)
                updateAlerts.Clear();

            if (succeeded)
            {
                lock (downloadedUpdatesLock)
                {
                    XenCenterVersions = action.XenCenterVersions;

                    XenServerVersionsForAutoCheck = action.XenServerVersionsForAutoCheck;

                    XenServerVersions = action.XenServerVersions;

                    XenServerPatches = action.XenServerPatches;
                }

                var xenCenterAlerts = NewXenCenterUpdateAlerts(XenCenterVersions, Program.Version);
                if (xenCenterAlerts != null)
                    updateAlerts.AddRange(xenCenterAlerts.Where(a=>!a.IsDismissed()));

                var xenServerUpdateAlerts = NewXenServerVersionAlerts(XenServerVersionsForAutoCheck);
                if (xenServerUpdateAlerts != null)
                    updateAlerts.AddRange(xenServerUpdateAlerts.Where(a=>!a.CanIgnore));

                var xenServerPatchAlerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
                if (xenServerPatchAlerts != null)
                    updateAlerts.AddRange(xenServerPatchAlerts.Where(alert => !alert.CanIgnore));
            }
            else
            {
                if (action.Exception != null)
                {
                    if (action.Exception is System.Net.Sockets.SocketException)
                    {
                        errorMessage = Messages.AVAILABLE_UPDATES_NETWORK_ERROR;
                    }
                    else
                    {
                        // Clean up and remove excess newlines, carriage returns, trailing nonsense
                        string errorText = action.Exception.Message.Trim();
                        errorText = System.Text.RegularExpressions.Regex.Replace(errorText, @"\r\n+", "");
                        errorMessage = string.Format(Messages.AVAILABLE_UPDATES_ERROR, errorText);
                    }
                }

                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = Messages.AVAILABLE_UPDATES_INTERNAL_ERROR;
            }

            if (CheckForUpdatesCompleted != null)
                CheckForUpdatesCompleted(succeeded, errorMessage);
        }


        public static void RegisterCollectionChanged(CollectionChangeEventHandler handler)
        {
            updateAlerts.CollectionChanged += handler;
        }

        public static void DeregisterCollectionChanged(CollectionChangeEventHandler handler)
        {
            updateAlerts.CollectionChanged -= handler;
        }


        public static List<XenCenterUpdateAlert> NewXenCenterUpdateAlerts(List<XenCenterVersion> xenCenterVersions,
            Version currentProgramVersion)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;
            var alerts = new List<XenCenterUpdateAlert>();
            XenCenterVersion latest = null, latestCr = null;
            if (xenCenterVersions.Count != 0 && currentProgramVersion != new Version(0, 0, 0, 0))
            {
                var latestVersions = from v in xenCenterVersions where v.Latest select v;
                latest = latestVersions.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                         latestVersions.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));

                if (IsSuitableForXenCenterAlert(latest, currentProgramVersion))
                    alerts.Add(new XenCenterUpdateAlert(latest));

                var latestCrVersions = from v in xenCenterVersions where v.LatestCr select v;
                latestCr = latestCrVersions.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                           latestCrVersions.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));

                if (latestCr != latest && IsSuitableForXenCenterAlert(latestCr, currentProgramVersion))
                    alerts.Add(new XenCenterUpdateAlert(latestCr));
            }

            if (alerts.Count == 0)
            {
                log.Info(string.Format("Not alerting XenCenter update - latest = {0},  latestcr = {1}, detected = {2}", 
                    latest != null ? latest.VersionAndLang : "", latestCr != null ? latestCr.VersionAndLang : "", Program.VersionAndLanguage));
            }

            return alerts;
        }

        private static bool IsSuitableForXenCenterAlert(XenCenterVersion toUse, Version currentProgramVersion)
        {
            if (toUse == null)
                return false;

            return toUse.Version > currentProgramVersion ||
                   (toUse.Version == currentProgramVersion && toUse.Lang == Program.CurrentLanguage &&
                    !PropertyManager.IsCultureLoaded(Program.CurrentCulture));
        }

        public static List<XenServerPatchAlert> NewXenServerPatchAlerts(List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var alerts = new List<XenServerPatchAlert>();

            var xenServerVersionsAsUpdates = xenServerVersions.Where(v => v.IsVersionAvailableAsAnUpdate);

            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                Host master = Helpers.GetMaster(xenConnection);
                Pool pool = Helpers.GetPoolOfOne(xenConnection);
                List<Host> hosts = xenConnection.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                var serverVersions = new List<XenServerVersion>();
                foreach (Host host in hosts)
                {
                    var serverVersion = GetServerVersions(host, xenServerVersions);
                    serverVersions.AddRange(serverVersion);
                }
                serverVersions = serverVersions.Distinct().ToList();

                if (serverVersions.Count == 0)
                    continue;

                foreach (XenServerVersion xenServerVersion in serverVersions)
                {
                    XenServerVersion version = xenServerVersion;
                    List<XenServerPatch> patches = xenServerPatches.FindAll(patch => version.Patches.Contains(patch));

                    if (patches.Count == 0)
                        continue;

                    foreach (XenServerPatch xenServerPatch in patches)
                    {
                        XenServerVersion newServerVersion = xenServerVersionsAsUpdates.FirstOrDefault(newVersion => newVersion.PatchUuid.Equals(xenServerPatch.Uuid, StringComparison.OrdinalIgnoreCase));

                        var alert = new XenServerPatchAlert(xenServerPatch, newServerVersion);
                        var existingAlert = alerts.Find(al => al.Equals(alert));

                        if (existingAlert != null)
                            alert = existingAlert;
                        else
                            alerts.Add(alert);

                        if (!xenConnection.IsConnected)
                            continue;

                        XenServerPatch serverPatch = xenServerPatch;

                        var noPatchHosts = hosts.Where(host => PatchCanBeInstalledOnHost(serverPatch, host, version));
        
                        if (noPatchHosts.Count() == hosts.Count)
                            alert.IncludeConnection(xenConnection);
                        else
                            alert.IncludeHosts(noPatchHosts);
                    }
                }
            }

            return alerts;
        }

        private static bool PatchCanBeInstalledOnHost(XenServerPatch serverPatch, Host host, XenServerVersion patchApplicableVersion)
        {
            Debug.Assert(serverPatch != null);
            Debug.Assert(host != null);

            if (Helpers.productVersionCompare(patchApplicableVersion.Version.ToString(), host.ProductVersion()) != 0)
                return false;

            // A patch can be installed on a host if:
            // 1. it is not already installed and
            // 2. the host has all the required patches installed and
            // 3. the host doesn't have any of the conflicting patches installed

            bool elyOrGreater = Helpers.ElyOrGreater(host);
            var appliedUpdates = host.AppliedUpdates();
            var appliedPatches = host.AppliedPatches();

            // 1. patch is not already installed 
            if (elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (!elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
                return false;

            // 2. the host has all the required patches installed
            if (serverPatch.RequiredPatches != null && serverPatch.RequiredPatches.Count > 0 &&
                !serverPatch.RequiredPatches
                .All(requiredPatchUuid =>
                    elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, requiredPatchUuid, StringComparison.OrdinalIgnoreCase))
                    || !elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, requiredPatchUuid, StringComparison.OrdinalIgnoreCase))
                    )
                )
                return false;

            // 3. the host doesn't have any of the conflicting patches installed
            if (serverPatch.ConflictingPatches != null && serverPatch.ConflictingPatches.Count > 0 &&
                serverPatch.ConflictingPatches
                .Any(conflictingPatchUuid =>
                    elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, conflictingPatchUuid, StringComparison.OrdinalIgnoreCase))
                    || !elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, conflictingPatchUuid, StringComparison.OrdinalIgnoreCase))
                    )
                )
                return false;

            return true;
        }


        /// <summary>
        /// If parameter is null, it returns latestcr XenCenter version if it is greater than current XC version,
        /// or null, if the current XC version is latestcr.
        /// If parameter is not null, it returns the minimum XenCenter version if it is greater than the current XC version,
        /// or null, if the minimum XC version couldn't be found or the current XC version is enough.
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>
        public static XenCenterVersion GetRequiredXenCenterVersion(XenServerVersion serverVersion)
        {
            if (XenCenterVersions.Count == 0)
                return null;

            var currentProgramVersion = Program.Version;
            if (currentProgramVersion == new Version(0, 0, 0, 0))
                return null;

            if (serverVersion == null)
                return XenCenterVersions.FirstOrDefault(xcv => xcv.LatestCr && xcv.Version > currentProgramVersion);

            var minXcVersion = serverVersion.MinimumXcVersion;
            if (minXcVersion == null)
                return null;

            var minimumXcVersion = XenCenterVersions.FirstOrDefault(xcv => xcv.Version == minXcVersion);
            return minimumXcVersion != null && minimumXcVersion.Version > currentProgramVersion
                ? minimumXcVersion
                : null;
        }
        
        /// <summary>
        /// This method returns the minimal set of patches for a host if this class already has information about them. Otherwise it returns empty list.
        /// Calling this function will not initiate a download or update.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static List<XenServerPatch> RecommendedPatchesForHost(Host host)
        {
            var recommendedPatches = new List<XenServerPatch>();

            if (XenServerVersions == null)
                return null;

            var serverVersions = GetServerVersions(host, XenServerVersions);

            if (serverVersions.Count != 0)
            {
                var minimumPatches = serverVersions[0].MinimalPatches;

                if (minimumPatches == null) //unknown
                    return null;

                bool elyOrGreater = Helpers.ElyOrGreater(host);

                var appliedPatches = host.AppliedPatches();
                var appliedUpdates = host.AppliedUpdates();

                if (elyOrGreater)
                {
                    recommendedPatches = minimumPatches.FindAll(p => !appliedUpdates.Any(au => string.Equals(au.uuid, p.Uuid, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    recommendedPatches = minimumPatches.FindAll(p => !appliedPatches.Any(ap => string.Equals(ap.uuid, p.Uuid, StringComparison.OrdinalIgnoreCase)));
                }

            }

            return recommendedPatches;
        }

        public static List<XenServerPatch> GetMinimalPatches(IXenConnection conn)
        {
            var version = GetCommonServerVersionOfHostsInAConnection(conn, XenServerVersions);
            return GetMinimalPatches(version);
        }

        public static List<XenServerPatch> GetMinimalPatches(Host host)
        {
            if (host == null || host.Connection == null || XenServerVersions== null)
                return null;
            var hostVersions = GetServerVersions(host, XenServerVersions);
            return GetMinimalPatches(hostVersions.FirstOrDefault());
        }

        private static List<XenServerPatch> GetMinimalPatches(XenServerVersion version)
        {
            if (version == null || version.MinimalPatches == null)
                return null;

            var minimalPatches = new List<XenServerPatch>(version.MinimalPatches);

            // if there is a "new version" update in the update sequence, also add the minimal patches of this new version
            if (minimalPatches.Count > 0)
            {
                // assuming that the new version update (if there is one) is the last one in the minimal patches list
                var lastUpdate = minimalPatches[minimalPatches.Count - 1];
                    
                var newServerVersion = XenServerVersions.FirstOrDefault(
                    v => v.IsVersionAvailableAsAnUpdate && v.PatchUuid.Equals(lastUpdate.Uuid, StringComparison.OrdinalIgnoreCase));

                if (newServerVersion != null && newServerVersion.MinimalPatches != null)
                    minimalPatches.AddRange(newServerVersion.MinimalPatches);
            }

            return minimalPatches;
        }

        /// <summary>
        /// Gets an upgrade sequence that contains a version upgrade, optionally followed by the minimal patches for the new version
        /// </summary>
        /// <param name="conn">Connection for the pool</param>
        /// <param name="alert">The alert that refers the version-update</param>
        /// <param name="updateTheNewVersion">Also add the minimum patches for the new version (true) or not (false).</param>
        public static List<XenServerPatch> GetMinimalPatches(IXenConnection conn, XenServerPatchAlert alert, bool updateTheNewVersion)
        {
            Debug.Assert(conn != null);
            Debug.Assert(alert != null);

            var minimalPatches = new List<XenServerPatch> {alert.Patch};

            // if it's a version updgrade the min sequence will be this patch (the upgrade) and the min patches for the new version
            if (updateTheNewVersion && alert.NewServerVersion != null && alert.NewServerVersion.MinimalPatches != null)
            {
                minimalPatches.AddRange(alert.NewServerVersion.MinimalPatches);
            }

            return minimalPatches;
        }


        /// <summary>
        /// Returns a XenServerVersion if all hosts of the pool have the same version
        /// Returns null if it is unknown or they don't match
        /// </summary>
        /// <returns></returns>
        private static XenServerVersion GetCommonServerVersionOfHostsInAConnection(IXenConnection connection, List<XenServerVersion> xsVersions)
        {
            if (connection == null || xsVersions == null)
                return null;

            XenServerVersion commonXenServerVersion = null;
            List<Host> hosts = connection.Cache.Hosts.ToList();

            foreach (Host host in hosts)
            {
                var hostVersions = GetServerVersions(host, xsVersions);

                var foundVersion = hostVersions.FirstOrDefault();

                if (foundVersion == null)
                {
                    return null;
                }
                else
                {
                    if (commonXenServerVersion == null)
                    {
                        commonXenServerVersion = foundVersion;
                    }
                    else
                    {
                        if (commonXenServerVersion != foundVersion)
                            return null;
                    }
                }
            }

            return commonXenServerVersion;
        }

        public static List<XenServerPatch> GetPatchSequenceForHost(Host h, List<XenServerPatch> minimalPatches)
        {
            if (minimalPatches == null)
                return null;

            var appliedUpdateUuids = Helpers.ElyOrGreater(h)
                ? h.AppliedUpdates().Select(u => u.uuid).ToList()
                : h.AppliedPatches().Select(p => p.uuid).ToList();

            var neededPatches = new List<XenServerPatch>(minimalPatches);
            var sequence = new List<XenServerPatch>();

            //Iterate through minimalPatches once; in each iteration, move the first item from L0
            //that has its dependencies met to the end of the update schedule
            for (int i = 0; i < neededPatches.Count; i++)
            {
                var p = neededPatches[i];

                if (appliedUpdateUuids.Any(apu => string.Equals(apu, p.Uuid, StringComparison.OrdinalIgnoreCase)))
                    continue; //the patch has been applied

                if (p.RequiredPatches == null || p.RequiredPatches.Count == 0 // no requirements
                    || p.RequiredPatches.All(rp => //all the required patches are already in the sequence or have already been applied
                            sequence.Any(useqp => string.Equals(useqp.Uuid, rp, StringComparison.OrdinalIgnoreCase))
                            || appliedUpdateUuids.Any(apu => string.Equals(apu, rp, StringComparison.OrdinalIgnoreCase))
                    )
                )
                {
                    // this patch can be added to the upgrade sequence now
                    sequence.Add(p);

                    // by now the patch has either been added to the upgrade sequence or something already contains it among the installed patches
                    neededPatches.RemoveAt(i);

                    //resetting position - the loop will start on 0. item
                    i = -1;
                }
            }

            return sequence;
        }

        public static List<XenServerVersionAlert> NewXenServerVersionAlerts(List<XenServerVersion> xenServerVersions)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var latestVersion = xenServerVersions.FindAll(item => item.Latest).OrderByDescending(v => v.Version).FirstOrDefault();
            var latestCrVersion = xenServerVersions.FindAll(item => item.LatestCr).OrderByDescending(v => v.Version).FirstOrDefault();

            List<XenServerVersionAlert> alerts = new List<XenServerVersionAlert>();

            if (latestVersion != null)
                alerts.Add(CreateAlertForXenServerVersion(latestVersion));

            if (latestCrVersion != null && latestCrVersion != latestVersion)
                alerts.Add(CreateAlertForXenServerVersion(latestCrVersion));

            return alerts;
        }

        private static XenServerVersionAlert CreateAlertForXenServerVersion(XenServerVersion version)
        {
            var alert = new XenServerVersionAlert(version);

            // the patch that installs this version, if any
            var patch  = XenServerPatches.FirstOrDefault(p => p.Uuid.Equals(version.PatchUuid, StringComparison.OrdinalIgnoreCase));

            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                if (!xc.IsConnected)
                    continue;

                Host master = Helpers.GetMaster(xc);
                Pool pool = Helpers.GetPoolOfOne(xc);
                List<Host> hosts = xc.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                // Show the Upgrade alert for a host if:
                // - the host version is older than this version AND
                // - there is no patch (amongst the current version patches) that can update to this version OR, if there is a patch, the patch cannot be installed
                var patchApplicable = patch != null && GetServerVersions(master, XenServerVersions).Any(v => v.Patches.Contains(patch));
                var outOfDateHosts = hosts.Where(host => new Version(Helpers.HostProductVersion(host)) < version.Version
                    && (!patchApplicable || !PatchCanBeInstalledOnHost(patch, host, version)));

                if (outOfDateHosts.Count() == hosts.Count)
                    alert.IncludeConnection(xc);
                else
                    alert.IncludeHosts(outOfDateHosts);
            }

            return alert;
        }

        public static List<XenServerVersion> GetServerVersions(Host host, List<XenServerVersion> xenServerVersions)
        {
            var serverVersions = xenServerVersions.FindAll(version =>
            {
                if (version.BuildNumber != string.Empty)
                    return (host.BuildNumberRaw() == version.BuildNumber);

                return Helpers.HostProductVersionWithOEM(host) == version.VersionAndOEM
                       || (version.Oem != null && Helpers.OEMName(host).StartsWith(version.Oem)
                           && Helpers.HostProductVersion(host) == version.Version.ToString());
            });
            return serverVersions;
        }

        public static void CheckServerVersion()
        {
            var alerts = NewXenServerVersionAlerts(XenServerVersionsForAutoCheck);
            if (alerts == null || alerts.Count == 0)
                return;

            alerts.ForEach(a => CheckUpdate(a));
        }

        public static void CheckServerPatches()
        {
            var alerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
            if (alerts == null)
                return;

            alerts.ForEach(a => CheckUpdate(a));
        }

        private static void CheckUpdate(XenServerUpdateAlert alert)
        {
            var existingAlert = FindUpdate(alert);

            if (existingAlert != null && alert.CanIgnore)
                RemoveUpdate(existingAlert);
            else if (existingAlert != null)
                ((XenServerUpdateAlert)existingAlert).CopyConnectionsAndHosts(alert);
            else if (!alert.CanIgnore)
                AddUpate(alert);
        }

        public static void RestoreDismissedUpdates()
        {
            var actions = new List<AsyncAction>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                actions.Add(new RestoreDismissedUpdatesAction(connection));

            var action = new ParallelAction(Messages.RESTORE_DISMISSED_UPDATES, Messages.RESTORING, Messages.COMPLETED, actions, true, false);
            action.Completed += action_Completed;

            if (RestoreDismissedUpdatesStarted != null)
                RestoreDismissedUpdatesStarted();

            action.RunAsync();
        }

        private static void action_Completed(ActionBase action)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                Properties.Settings.Default.LatestXenCenterSeen = "";
                Settings.TrySaveSettings();

                CheckForUpdates(true);
            });
        }

        private static XenServerPatchAlert FindPatchAlert(Predicate<XenServerPatch> predicate)
        {
            var existingAlert = FindUpdate(a => a is XenServerPatchAlert && predicate(((XenServerPatchAlert)a).Patch));
            if (existingAlert != null)
                return existingAlert as XenServerPatchAlert;

            if (XenServerPatches.Count == 0)
                return null;

            var xenServerPatch = XenServerPatches.FirstOrDefault(p => predicate(p));
            if (xenServerPatch == null)
                return null;

            var newServerVersion = XenServerVersions.FirstOrDefault(v => v.IsVersionAvailableAsAnUpdate &&
                    v.PatchUuid.Equals(xenServerPatch.Uuid, StringComparison.OrdinalIgnoreCase));

            return new XenServerPatchAlert(xenServerPatch, newServerVersion);
        }

        public static XenServerPatchAlert FindPatchAlertByUuid(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
                return null;
            return FindPatchAlert(p => p.Uuid.Equals(uuid, StringComparison.OrdinalIgnoreCase));
        }

        public static XenServerPatchAlert FindPatchAlertByName(string patchName)
        {
            if (string.IsNullOrEmpty(patchName))
                return null;
            return FindPatchAlert(p => p.Name.Equals(patchName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
