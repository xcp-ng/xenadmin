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
using System.Linq;
using System.Text;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;

using XenAPI;

namespace XenAdmin.Core
{
    static class ActionBaseExtensions
    {
        internal static Image GetImage(this ActionBase action)
        {
            if (action.IsCompleted)
                return action.Succeeded
                           ? Images.StaticImages._000_Tick_h32bit_16
                           : action.Exception is CancelledException
                                 ? Images.StaticImages.cancelled_action_16
                                 : Images.StaticImages._000_error_h32bit_16;

            return Images.GetImageForPercentage(action.PercentComplete);
        }

        internal static string GetDetails(this ActionBase action)
        {
            var sb = new StringBuilder(GetTitle(action));
            sb.Append("\n").Append(GetDescription(action));

            string timeString = GetTimeElapsed(action);
            if (!string.IsNullOrEmpty(timeString))
                sb.Append("\n").Append(timeString);

            return sb.ToString();
        }

        internal static string GetTimeElapsed(this ActionBase action)
        {
            TimeSpan time = action.IsCompleted
                                ? action.Finished.Subtract(action.Started)
                                : DateTime.Now.Subtract(action.Started);

            time = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds, 0);
            return time.TotalSeconds > 0
                       ? string.Format("{0} {1}", Messages.HISTORYROW_TIME, time)
                       : string.Empty;
        }

        internal static string GetDescription(this ActionBase action)
        {
            if (action.Exception == null)
                return action.Description;

            if (action.Exception is CancelledException)
                return Messages.EXCEPTION_USER_CANCELLED;

            if (action.Exception is I18NException)
                return ((I18NException)action.Exception).I18NMessage;

            return action.Exception.Message;
        }

        internal static string GetTitle(this ActionBase action)
        {
            if (action.Title != null)
                return action.Title;

            AsyncAction asyncAction = action as AsyncAction;

            // Should occur rarely if ever
            IXenConnection conn = asyncAction == null ? null : asyncAction.Connection;

            string conn_name = conn == null ? null : Helpers.GetName(conn);
            string vm_name = action.VM == null ? null : action.VM.Name();
            string desc = action.Description;

            if (conn_name != null && vm_name != null && desc != null)
                return string.Format(Messages.HISTORYROW_ON_WITH, conn_name, vm_name, desc);

            if (vm_name != null && desc != null)
                return string.Format(Messages.HISTORYROW_WITH, vm_name, desc);

            if (conn_name != null && desc != null)
                return string.Format(Messages.HISTORYROW_ON, conn_name, desc);

            if (desc != null)
                return desc;

            return "";
        }

        internal static string GetLocation(this ActionBase action)
        {
            if (action.Host != null)
                return action.Host.Name();

            if (action.Pool != null)
                return action.Pool.Name();

            return string.Empty;
        }

        internal static IXenObject GetRelevantXenObject(this ActionBase action)
        {
            return action.SR ?? action.Template ?? action.VM ?? action.Host ??
                   (IXenObject)action.Pool;
        }

        internal static List<string> GetApplicableHosts(this ActionBase action)
        {
            var hostUuids = new List<string>();

            if (action.Host != null && action.Host.uuid != null)
                hostUuids.Add(action.Host.uuid);
            else if (action.Pool != null)
                hostUuids.AddRange(action.Pool.Connection.Cache.Hosts.Where(h => h.uuid != null).Select(h => h.uuid));

            return hostUuids;
        }

        internal static string GetStatusString(this ActionBase action)
        {
            if (action.IsCompleted)
                return action.Succeeded
                           ? Messages.ACTION_STATUS_SUCCEEDED
                           : action.Exception is CancelledException
                                 ? Messages.ACTION_SYSTEM_STATUS_CANCELLED
                                 : Messages.ACTION_STATUS_FAILED;

            return Messages.ACTION_STATUS_IN_PROGRESS;
        }

        #region Comparison (just static non-extension) methods

        /// <summary>
        /// Ascending order:
        /// 1. cancelled/failed
        /// 2. lower state of completeness (smaller percentage completed)
        /// 3. completed successfully
        /// </summary>
        /// <param name="action1"></param>
        /// <param name="action2"></param>
        /// <returns></returns>
        public static int CompareOnStatus(ActionBase action1, ActionBase action2)
        {
            if (action1.IsCompleted && action2.IsCompleted)
            {
                if (action1.Succeeded && action2.Succeeded)
                    return 0;
                if (!action1.Succeeded && !action2.Succeeded)
                    return 0;
                if (!action1.Succeeded && action2.Succeeded)
                    return -1;
                if (action1.Succeeded && !action2.Succeeded)
                    return 1;
            }

            if (!action1.IsCompleted && action2.IsCompleted)
                return -1;

            if (action1.IsCompleted && !action2.IsCompleted)
                return 1;

            return action1.PercentComplete.CompareTo(action2.PercentComplete);
        }

        public static int CompareOnTitle(ActionBase action1, ActionBase action2)
        {
            int result = string.Compare(action1.GetTitle(), action2.GetTitle());
            if (result == 0)
                result = CompareOnStatus(action1, action2);
            return result;
        }

        public static int CompareOnLocation(ActionBase action1, ActionBase action2)
        {
            string location1 = action1.GetLocation();
            string location2 = action2.GetLocation();

            int result = string.Compare(location1, location2);
            if (result == 0)
                result = CompareOnStatus(action1, action2);
            return result;
        }

        public static int CompareOnDateStarted(ActionBase action1, ActionBase action2)
        {
            int result = DateTime.Compare(action1.Started, action2.Started);
            if (result == 0)
                result = CompareOnStatus(action1, action2);
            return result;
        }

        #endregion
    }
}
