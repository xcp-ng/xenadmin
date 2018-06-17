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
using XenAPI;
using System.Collections.ObjectModel;
using XenAdmin.Core;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Performs the WLB recommendations for the Start-On, Resume-On and Migrate menu items.
    /// </summary>
    internal class WlbRecommendations
    {
        private readonly ReadOnlyCollection<VM> _vms;
        private readonly Dictionary<VM, Dictionary<XenRef<Host>, string[]>> _recommendations;
        private readonly bool _isError;

        /// <summary>
        /// Initializes a new instance of the <see cref="WlbRecommendations"/> class.
        /// </summary>
        /// <param name="vms">The VMs that the recommendations are required for.</param>
        /// <param name="recommendations"></param>
        public WlbRecommendations(List<VM> vms, Dictionary<VM, Dictionary<XenRef<Host>, string[]>> recommendations)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(vms, "vms");

            _vms = new ReadOnlyCollection<VM>(vms);
            _recommendations = recommendations;
            _isError = recommendations == null;
        }

        /// <summary>
        /// Gets a value indicating whether an exception was thrown when calling VM.retrieve_wlb_recommendations.
        /// </summary>
        /// <value><c>true</c> if an exception was thrown; otherwise, <c>false</c>.</value>
        public bool IsError
        {
            get
            {
                return _isError;
            }
        }

        private void Verify()
        {
            if (_isError)
            {
                throw new InvalidOperationException("There was an error getting the WLB recommendations.");
            }
        }

        public Host GetOptimalServer(VM vm)
        {
            Verify();

            double highStars = 0;
            Host recHost = null;
            foreach (KeyValuePair<XenRef<Host>, string[]> rec in _recommendations[vm])
            {
                if (rec.Value.Length > 0 && rec.Value[0].Trim().ToLower() == "wlb")
                {
                    // WLB: stars equal to highStars handles when there are only two hosts and vm resides on a host with high stars
                    double stars = Helpers.ParseStringToDouble(rec.Value[1].Trim(), 0);
                    if ((stars >= highStars) && (rec.Key.opaque_ref != vm.resident_on.opaque_ref))
                    {
                        highStars = stars;
                        recHost = vm.Connection.Resolve(rec.Key);
                    }
                }
            }

            return recHost;
        }

        public WlbRecommendation GetStarRating(Host host)
        {
            Verify();

            List<double> starRatings = new List<double>();
            Dictionary<VM, bool> canExecutes = new Dictionary<VM, bool>();
            Dictionary<VM, string> cantExecuteReasons = new Dictionary<VM, string>();

            foreach (VM vm in _vms)
            {
                Host residentHost = vm.Connection.Resolve(vm.resident_on);
                string[] rec = new string[0];

                if (residentHost != null && residentHost.opaque_ref == host.opaque_ref)
                {
                    cantExecuteReasons[vm] = Messages.HOST_MENU_CURRENT_SERVER;
                    canExecutes[vm] = false;
                }
                else if (_recommendations[vm].TryGetValue(new XenRef<Host>(host.opaque_ref), out rec))
                {
                    if (rec.Length > 0 && rec[0].Trim().ToLower() == "wlb")
                    {
                        double stars = 0;
                        ParseStarRating(rec, out stars);
                        canExecutes[vm] = true;
                        starRatings.Add(stars);
                    }
                    else
                    {
                        cantExecuteReasons[vm] = new Failure(rec).ShortMessage;
                        canExecutes[vm] = false;
                    }
                }
                else
                {
                    cantExecuteReasons[vm] = FriendlyErrorNames.HOST_NOT_LIVE_SHORT;
                    canExecutes[vm] = false;
                }
            }

            double averageStarRating = 0.0;

            foreach (double s in starRatings)
            {
                averageStarRating += s;
            }
            averageStarRating /= starRatings.Count;

            return new WlbRecommendation(canExecutes, averageStarRating, cantExecuteReasons);
        }

        private static bool ParseStarRating(string[] rec, out double starRating)
        {
            // recommedation string[] format examples: 
            //    WLB; 0.0; zero_score_reason
            //    WLB; 3.4
            //    xapiError(such as the assert can boot failsthe);detail;detail

            starRating = 0;
            if (rec != null && rec.Length > 1 && rec[0].Trim().ToLower() == "wlb")
            {
                starRating = Helpers.ParseStringToDouble(rec[1], 0);
                return true;
            }
            return false;
        }

        internal class WlbRecommendation
        {
            public readonly Dictionary<VM, bool> CanExecuteByVM;
            public readonly double StarRating;
            public readonly Dictionary<VM, string> CantExecuteReasons;

            public WlbRecommendation(Dictionary<VM, bool> canExecuteByVM, double starRating, Dictionary<VM, string> cantExecuteReasons)
            {
                CanExecuteByVM = canExecuteByVM;
                StarRating = starRating;
                CantExecuteReasons = cantExecuteReasons;
            }
        }
    }
}
