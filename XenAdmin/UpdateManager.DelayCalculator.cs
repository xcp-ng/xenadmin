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
using System.Diagnostics;

namespace XenAdmin
{
    internal partial class UpdateManager
    {
        /// <summary>
        /// Calculates the delay required between updates for the <see cref="UpdateManager"/>.
        /// </summary>
        private class DelayCalculator
        {
            private Stopwatch _sw;

            private readonly List<double> _requestPeriods = new List<double>();
            private readonly List<double> _updateDurations = new List<double>();

            /// <summary>
            /// Call this to register an updates that took the specified length of time (in ms).
            /// </summary>
            /// <param name="duration">The length of time that the update took (in ms).</param>
            public void RegisterLatestUpdate(long duration)
            {
                if (duration > 0)
                {
                    AddToList(_updateDurations, duration);
                }
            }

            /// <summary>
            /// Call this to register that an update has been requested.
            /// </summary>
            public void RegisterLatestUpdateRequest()
            {
                lock (_requestPeriods)
                {
                    if (_sw != null)
                    {
                        AddToList(_requestPeriods, _sw.ElapsedMilliseconds);
                    }
                    _sw = Stopwatch.StartNew();
                }
            }

            private static double GetMean(List<double> list)
            {
                lock (list)
                {
                    double mean = 0;
                    foreach (double i in list)
                    {
                        mean += i;
                    }

                    return mean / list.Count;
                }
            }

            private static void AddToList(List<double> list, double value)
            {
                lock (list)
                {
                    list.Add(value);

                    if (list.Count > 10)
                    {
                        list.RemoveAt(0);
                    }
                }
            }

            /// <summary>
            /// Gets the delay that should be used between updates. I expect something more sophisticated
            /// may be required but let's give this a try for now.
            /// </summary>
            /// <returns>The delay that should be used between updates (in ms).</returns>
            public int GetDelay()
            {
                if(_updateDurations.Count > 0 && _requestPeriods.Count > 0)
                {
                    double meanDuration = GetMean(_updateDurations);
                    double meanRequestPeriod = GetMean(_requestPeriods);

                    if(meanRequestPeriod > 0.0)
                    {
                        double n = meanDuration / meanRequestPeriod;

                        // n represents the time an update takes / s or real-time
                        
                        // if n > 1 we have using more time than we've got

                        if (n > 1)
                        {
                            return 1000;
                        }
                    }
                }
                return 0;
            }
        }
    }
}
