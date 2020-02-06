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

using XenAdmin.Network;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Alerts
{
    public abstract class Alert : IEquatable<Alert>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object XenCenterAlertsLock = new object();
        private static readonly ChangeableList<Alert> XenCenterAlerts = new ChangeableList<Alert>();

        public bool Dismissing;

        public static void AddAlert(Alert a)
        {
            try
            {
                lock (XenCenterAlertsLock)
                    XenCenterAlerts.Add(a);
            }
            catch (Exception e)
            {
                log.Error("Failed to add incoming alert", e);
            }
        }

        public static void AddAlertRange(IEnumerable<Alert> collection)
        {
            try
            {
                lock (XenCenterAlertsLock)
                    XenCenterAlerts.AddRange(collection);
            }
            catch (Exception e)
            {
                log.Error("Failed to add incoming alerts", e);
            }
        }

        public static void RemoveAlert(Alert a)
        {
            try
            {
                lock (XenCenterAlertsLock)
                    XenCenterAlerts.Remove(a);
            }
            catch (Exception e)
            {
                log.Error("Failed to remove alert. ", e);
            }
        }

        public static void RemoveAlert(Predicate<Alert> predicate)
        {
            lock (XenCenterAlertsLock)
                XenCenterAlerts.RemoveAll(predicate);
        }

        /// <summary>
        /// Find the Alert in the alert collection, or null if none exists.
        /// </summary>
        public static Alert FindAlert(Alert alert)
        {
            lock (XenCenterAlertsLock)
                return FindAlert(a => a.Equals(alert));
        }

        public static Alert FindAlert(Predicate<Alert> predicate)
        {
            lock (XenCenterAlertsLock)
                return XenCenterAlerts.Find(predicate);
        }

        public static int FindAlertIndex(Predicate<Alert> predicate)
        {
            lock (XenCenterAlertsLock)
                return XenCenterAlerts.FindIndex(predicate);
        }

        public static void RefreshAlertAt(int index)
        {
            lock (XenCenterAlertsLock)
            {
                if (index >= 0 && index < XenCenterAlerts.Count)
                    XenCenterAlerts[index] = XenCenterAlerts[index]; // this will trigger the CollectionChanged event with CollectionChangeAction.Refresh
            }
        }

        /// <summary>
        /// locks the list of alerts before taking a total, and then returning that value
        /// </summary>
        public static int AlertCount
        {
            get
            {
                lock (XenCenterAlertsLock)
                {
                    return Alerts.Length;
                }
            }
        }

        public static int NonDismissingAlertCount
        {
            get
            {
                int count = 0;
                lock (XenCenterAlertsLock)
                {
                    foreach (Alert a in XenCenterAlerts)
                    {
                        if (!a.Dismissing)
                            count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Locks the list of alerts and returns a new array containing each alert which is not dismissing
        /// </summary>
        public static List<Alert> NonDismissingAlerts
        {
            get
            {
                List<Alert> alertList = new List<Alert>();
                lock (XenCenterAlertsLock)
                {
                    foreach (Alert a in XenCenterAlerts)
                    {
                        if (!a.Dismissing)
                            alertList.Add(a);
                    }
                }
                return alertList;
            }
        }

        /// <summary>
        /// Takes the alert lock and copies the alerts to a new array, which is then returned. Be careful where you use this.
        /// </summary>
        public static Alert[] Alerts
        {
            get
            {
                lock (XenCenterAlertsLock)
                    return XenCenterAlerts.ToArray();
            }
        }

        /// <summary>
        /// The host this alert is related to
        /// </summary>
        public string HostUuid;

        protected DateTime _timestamp = DateTime.MinValue;
        public string uuid;
        protected int _priority;

        public Alert()
        {
            uuid = Guid.NewGuid().ToString();
        }

        public static void RegisterAlertCollectionChanged(CollectionChangeEventHandler handler)
        {
            XenCenterAlerts.CollectionChanged += handler;
        }

        public static void DeregisterAlertCollectionChanged(CollectionChangeEventHandler handler)
        {
            XenCenterAlerts.CollectionChanged -= handler;
        }

        /// <summary>
        /// When the Alert was raised.
        /// </summary>
        public DateTime Timestamp => _timestamp;

        /// <summary>
        /// Dismisses the Alert: marks it as dealt with in some way. May only be called once.
        /// </summary>
        public virtual void Dismiss()
        {
            RemoveAlert(this);
        }

        /// <summary>
        /// Dismiss the alert, making a single API call if this is a remote message (Dismiss will launch a complete
        /// action).
        /// 
        /// The normal behaviour here is the same as Dismiss() -- it's only when overridden by MessageAlert that there
        /// is a difference.
        /// </summary>
        public virtual void DismissSingle(Session s)
        {
            Dismiss();
        }

        public virtual bool IsDismissed()
        {
            return false;
        }

        public virtual string Name => null;

        public virtual string WebPageLabel => null;

        public abstract string Title { get; }

        public abstract string Description { get; }

        /// <summary>
        /// Specifies the icon to use for the alert
        /// </summary>
        public abstract AlertPriority Priority { get; }

        public abstract string AppliesTo { get; }

        /// <summary>
        /// The text for the 'click here to fix...' link. A null return value
        /// indicates no such link should be displayed.
        /// </summary>
        public abstract string FixLinkText { get; }

        /// <summary>
        /// The delegate called when the 'click here to fix...' link is clicked. The calling
        /// Alert is passed in. May only return null if FixLinkText is null.
        /// </summary>
        public abstract Action FixLinkAction { get; }

        /// <summary>
        /// The text for the 'click here for help...' link.
        /// </summary>
        public virtual string HelpLinkText => Messages.ALERT_GENERIC_HELP;

        /// <summary>
        /// The helpid opened when the 'click here for help...' link is clicked.
        /// </summary>
        public abstract string HelpID { get; }

        public IXenConnection Connection;

        public virtual bool Equals(Alert other)
        {
            return base.Equals(other);
        }

        public static int CompareOnDate(Alert alert1, Alert alert2)
        {
            int sortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
            if (sortResult == 0)
                sortResult = CompareOnName(alert1, alert2);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }

        /// <summary>
        /// Sorts two alerts by priority based on their type, tie broken by uuid
        /// </summary>
        public static int CompareOnPriority(Alert alert1, Alert alert2)
        {
            //the Unknown priority is lowest of all
            //it's only given the value 0 because this is the default for integers
            if (alert1.Priority < alert2.Priority)
                return alert1.Priority == 0 ? 1 : -1;

            if (alert1.Priority > alert2.Priority)
                return alert2.Priority == 0 ? -1 : 1;

            return string.Compare(alert1.uuid, alert2.uuid);
        }

        public static int CompareOnTitle(Alert alert1, Alert alert2)
        {
            int sortResult = string.Compare(alert1.Title, alert2.Title);
            if (sortResult == 0)
                sortResult = CompareOnName(alert1, alert2);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }

        public static int CompareOnAppliesTo(Alert alert1, Alert alert2)
        {
            int sortResult = string.Compare(alert1.AppliesTo, alert2.AppliesTo);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.Name, alert2.Name);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }

        public static int CompareOnDescription(Alert alert1, Alert alert2)
        {
            int sortResult = string.Compare(alert1.Description, alert2.Description);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.Name, alert2.Name);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }

        public static int CompareOnName(Alert alert1, Alert alert2)
        {
            int sortResult = string.Compare(alert1.Name, alert2.Name);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }

        public static int CompareOnWebPage(Alert alert1, Alert alert2)
        {
            int sortResult = string.Compare(alert1.WebPageLabel, alert2.WebPageLabel);
            if (sortResult == 0)
                sortResult = CompareOnName(alert1, alert2);
            if (sortResult == 0)
                sortResult = string.Compare(alert1.uuid, alert2.uuid);
            return sortResult;
        }


        #region Update dismissal

        public static bool AllowedToDismiss(Alert alert)
        {
            return AllowedToDismiss(new[] { alert });
        }

        public static bool AllowedToDismiss(IEnumerable<Alert> alerts)
        {
            var alertConnections = (from Alert alert in alerts
                                    where alert != null && !alert.Dismissing
                                    let con = alert.Connection
                                    select con).Distinct();

            return alertConnections.Any(AllowedToDismiss);
        }

        /// <summary>
        /// Checks the user has sufficient RBAC privileges to clear updates on a given connection
        /// </summary>
        public static bool AllowedToDismiss(IXenConnection c)
        {
            // check if local alert
            if (c == null)
                return true;

            // have we disconnected? Alert will disappear soon, but for now block dismissal.
            if (c.Session == null)
                return false;

            if (c.Session.IsLocalSuperuser)
                return true;

            List<Role> rolesAbleToCompleteAction = Role.ValidRoleList("Message.destroy", c);
            foreach (Role possibleRole in rolesAbleToCompleteAction)
            {
                if (c.Session.Roles.Contains(possibleRole))
                    return true;
            }
            return false;
        }

        #endregion
    }
    public enum AlertPriority
    {
        /// <summary>
        /// Includes alerts on servers older than Clearwater for which no icon will be shown
        /// Note that this is given the value 0 only because this is the default for integers
        /// and Unknown is the default priority. In fact it is the lowest priority and comparers
        /// should explicitly handle this.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Data-loss imminent: take action now or your data may be permanently lost (e.g. corrupted)
        /// </summary>
        Priority1,

        /// <summary>
        /// Service-loss imminent: take action now or some services may fail (e.g. host/VM crash)
        /// </summary>
        Priority2,

        /// <summary>
        /// Service degraded: take action now or some service may suffer
        /// (e.g. NIC bond degraded without HA, management network used for storage traffic?)
        /// </summary>
        Priority3,

        /// <summary>
        /// Service recovered: notice that something just improved (e.g. NIC bond repaired)
        /// </summary>
        Priority4,

        /// <summary>
        /// Informational: more day-to-day stuff (e.g. VM started, suspended, shutdown, rebooted etc)
        /// </summary>
        Priority5
    }

    public static class AlertPriorityExtensions
    {
        public static string GetString(this AlertPriority priority)
        {
            switch (priority)
            {
                case AlertPriority.Priority1:
                    return 1.ToString();
                case AlertPriority.Priority2:
                    return 2.ToString();
                case AlertPriority.Priority3:
                    return 3.ToString();
                case AlertPriority.Priority4:
                    return 4.ToString();
                case AlertPriority.Priority5:
                    return 5.ToString();
                default:
                    return Messages.UNKNOWN;
            }
        }
    }
}
