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
using XenAPI;


namespace XenAdmin.Actions
{
    public class EditMultipathAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const String DEFAULT_MULTIPATH_HANDLE = "dmp";

        private readonly Host host;
        private readonly bool multipath;

        public EditMultipathAction(Host host, bool multipath, bool suppressHistory)
            : base(host.Connection, Messages.ACTION_CHANGE_MULTIPATH, string.Format(Messages.ACTION_CHANGING_MULTIPATH_FOR, host), suppressHistory)
        {
            this.host = host;
            this.multipath = multipath;
        }

        protected override void Run()
        {
            Exception unplugException = null;

            // Only unplug and replug already plugged pbds
            List<String> pluggedPBDrefs = new List<String>();

            try
            {
                foreach (PBD pbd in host.Connection.ResolveAll(host.PBDs))
                {
                    if (!pbd.currently_attached)
                        continue;

                    pluggedPBDrefs.Add(pbd.opaque_ref);
                    PBD.unplug(Session, pbd.opaque_ref);
                }

                // CA-19392: Multipath enablement / disablement

                if (Helpers.KolkataOrGreater(host))
                {
                    Host.set_multipathing(Session, host.opaque_ref, multipath);
                }
                else
                {
                    Host.remove_from_other_config(Session, host.opaque_ref, Host.MULTIPATH);
                    Host.add_to_other_config(Session, host.opaque_ref, Host.MULTIPATH, multipath.ToString().ToLowerInvariant());
                }

                Host.remove_from_other_config(Session, host.opaque_ref, Host.MULTIPATH_HANDLE);
                
                if (multipath)
                {
                    Host.add_to_other_config(Session, host.opaque_ref, Host.MULTIPATH_HANDLE, DEFAULT_MULTIPATH_HANDLE);
                }

            }
            catch (Exception e)
            {
                unplugException = e;

                log.Debug("Error occurred unplugging pbds", e);

                throw;
            }
            finally
            {
                Exception plugException = null;

                foreach (String pbdRef in pluggedPBDrefs)
                {
                    try
                    {
                        PBD.plug(Session, pbdRef);
                    }
                    catch (Exception e)
                    {
                        if (unplugException == null && plugException == null)
                            plugException = e;

                        log.Debug("Error occurred replugging pbds", e);
                    }
                }

                // Only throw a plug exception if there we no unplug exceptions
                if (unplugException == null && plugException != null)
                    throw plugException;
            }
        }
    }
}
