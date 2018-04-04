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
using System.Threading;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
    public interface IEnableableXenObjectComboBoxItem : IEnableableComboBoxItem
    {
        IXenObject Item { get; }
    }

    public class DelayLoadingOptionComboBoxItem : IEnableableXenObjectComboBoxItem
    {
        public delegate void ReasonUpdatedEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Event raised when the reason is updated
        /// </summary>
        public event ReasonUpdatedEventHandler ReasonUpdated;
        private string failureReason;
        private IXenObject xenObject;
        private const int defaultRetries = 10;
        private const int defaultTimeOut = 200;
        private readonly List<ReasoningFilter> _filters;

        /// <summary>
        /// Creates a new class instance and starts a thread to load data
        /// </summary>
        public DelayLoadingOptionComboBoxItem(IXenObject xenObject, List<ReasoningFilter> filters)
        {
            this.xenObject = xenObject;
            _filters = filters;
        }

        public void CopyFrom(DelayLoadingOptionComboBoxItem toCopy)
        {
            xenObject = toCopy.xenObject;
            failureReason = toCopy.FailureReason;
            Enabled = toCopy.Enabled;
            PreferAsSelectedItem = toCopy.PreferAsSelectedItem;
        }

        /// <summary>
        /// Underlying Xen Object
        /// </summary>
        public IXenObject Item
        {
            get { return xenObject; }
        }

        /// <summary>
        /// You would prefer this item to be the one that is selected
        /// As the items are threaded they may exist and be disabled but required
        /// as a selected item if they load successfully.
        /// 
        /// Use this in the event handler for the ReasonUpdated flag to find out
        /// which item should be the selected one and thus which to 
        /// set in the combo box
        /// </summary>
        public bool PreferAsSelectedItem { get; set; }

        /// <summary>
        /// Create a thread and fetch the reason
        /// </summary>
        public void Load()
        {
            //Set default reason without going through setter so not to trigger the event
            failureReason = Messages.DELAY_LOADING_COMBO_BOX_WAITING;

            //Thread will trigger event when setting the reason
            FetchReasonAsnyc();

        }

        //Fetch the reason, no thread
        public void LoadAndWait()
        {
            FetchFailureReasonWithRetry(defaultRetries, defaultTimeOut);
        }
 
        private void FetchReasonAsnyc()
        {
            ThreadPool.QueueUserWorkItem(delegate { FetchFailureReasonWithRetry(defaultRetries, defaultTimeOut); });
        }

        public void CancelFilters()
        {
            foreach (ReasoningFilter filter in _filters)
                filter.Cancel();
        }

        private void FetchFailureReasonWithRetry(int retries, int timeOut)
        {
            string threadFailureReason = Messages.DELAY_LOADED_COMBO_BOX_ITEM_FAILURE_UNKOWN;

            do
            {
                try
                {
                    threadFailureReason = FetchFailureReason();
                    break;
                }
                catch
                {
                    if (retries <= 0)
                    {
                        FailureReason = Messages.DELAY_LOADED_COMBO_BOX_ITEM_FAILURE_UNKOWN;
                        return;
                    }
                    Thread.Sleep(timeOut);
                }
            } while (retries-- > 0);

            FailureReason = threadFailureReason;
        }

        /// <summary>
        /// Trigger event
        /// </summary>
        /// <param name="e"></param>
        private void OnReasonChanged(EventArgs e)
        {
            if (ReasonUpdated != null)
                ReasonUpdated(this, e);
        }

        public bool Enabled { get; private set; }

        /// <summary>
        /// Setter will trigger reason updated event
        /// If no failure result is found on setting set the enabled
        /// </summary>
        private string FailureReason
        {
            get { return failureReason; }
            set
            {
                if (failureReason == value)
                    return;
                failureReason = value;

                Enabled = String.IsNullOrEmpty(failureReason);

                OnReasonChanged(new EventArgs());
            }
        }

        /// <summary>
        /// This will format the xen object and reason together
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(FailureReason))
                return Item.Name();

            return String.Format(Messages.DELAY_LOADED_COMBO_BOX_ITEM_FAILURE_REASON, Item.Name(), FailureReason);
        }

        /// <summary>
        /// Fetch the reason from somewhere that may take some time
        /// Called in a separate thread by the constructor
        /// Returning String.Empty or null will mean no failure has been found
        /// </summary>
        /// <returns></returns>
        protected virtual string FetchFailureReason()
        {
            foreach (ReasoningFilter filter in _filters)
            {
                if (filter.FailureFoundFor(Item))
                {
                    return filter.Reason;
                }
            }

            return string.Empty;
        }
    }
}
