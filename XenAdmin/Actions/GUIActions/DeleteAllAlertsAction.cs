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
using System.Linq;
using XenAdmin.Alerts;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class DeleteAllAlertsAction : AsyncAction
    {
        private readonly IEnumerable<Alert> Alerts;

        /// <param name="connection">May be null; this is expected for client-side alerts.</param>
        /// <param name="alerts"></param>
        public DeleteAllAlertsAction(IXenConnection connection, IEnumerable<Alert> alerts)
            : base(connection,
                   GetActionTitle(connection, alerts.Count()), 
                   Messages.ACTION_REMOVE_ALERTS_DESCRIPTION)
        {
            this.Alerts = alerts;

            if (connection != null)
            {
                Pool = Helpers.GetPoolOfOne(connection);
                Host = Helpers.GetMaster(connection);
            }
        }

        private static string GetActionTitle(IXenConnection connection, int alertCount)
        {
            return connection == null
                ? alertCount == 1
                       ? Messages.ACTION_REMOVE_ALERTS_ON_CLIENT_TITLE_ONE
                       : string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CLIENT_TITLE, alertCount)
                : alertCount == 1
                       ? string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CONNECTION_TITLE_ONE, Helpers.GetName(connection))
                       : string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CONNECTION_TITLE, alertCount, Helpers.GetName(connection));
        }

        protected override void Run()
        {
            int i = 0;
            int max = Alerts.Count();
            Exception e = null;
            LogDescriptionChanges = false;
            List<Alert> toBeDismissed = new List<Alert>();

            try
            {
                var myList = new List<Alert>(Alerts);
                foreach (Alert a in myList)
                {
                    PercentComplete = (i * 100) / max;
                    i++;
                    Description = string.Format(Messages.ACTION_REMOVE_ALERTS_PROGRESS_DESCRIPTION, i, max);

                    Alert a1 = a;
                    BestEffort(ref e, delegate
                        {
                            try
                            {
                                if (a1 is XenServerPatchAlert || a1 is XenServerVersionAlert)
                                {
                                    toBeDismissed.Add(a1);
                                }
                                else if(a1 is XenCenterUpdateAlert)
                                {
                                    a1.Dismiss();
                                }
                                else
                                {
                                    a1.DismissSingle(Session);
                                }
                            }
                            catch (Failure exn)
                            {
                                if (exn.ErrorDescription[0] != Failure.HANDLE_INVALID)
                                    throw;
                                //remove alert from XenCenterAlerts; this will trigger the CollectionChanged event, on which we update the alert count
                                if (Alert.FindAlert(a1) != null)
                                    Alert.RemoveAlert(a1); 
                            }
                        });
                }

                Updates.DismissUpdates(toBeDismissed);
            }
            finally
            {
                LogDescriptionChanges = true;
            }

            Description = max == 1 ? Messages.ACTION_REMOVE_ALERTS_DONE_ONE : string.Format(Messages.ACTION_REMOVE_ALERTS_DONE, max);
        }
    }
}
