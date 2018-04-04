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
using System.Text.RegularExpressions;
using XenAdmin.Network;
using System.Linq;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public class SrWizardHelpers
    {
        public static bool ValidateNfsSharename(String sharename)
        {
            Regex re = new Regex(@"^[^/:*?<>|\\]+:/[^:*?<>|\\]+$"); // make sure we match server:/share/folder
            return re.IsMatch(sharename);
        }

        public static bool ValidateCifsSharename(String sharename)
        {
            Regex re = new Regex(@"^\\(\\[^\/:*?<>|\\]+\\{0,1})+$");

            return re.IsMatch(sharename);
        }

        /// <summary>
        /// Determines if an SR is already in use by checking all the SRs known to the GUI and comparing
        /// uuids. Obviously only best-effort, since an SR may be in use by a disconnected host.
        /// </summary>
        /// <param name="uuid">The uuid of the SR to compare against.</param>
        /// <returns>The XenObject SR taht corresponds to this uuid, null otherwise</returns>
        public static SR SrInUse(String uuid)
        {
            // SR can now exist on multiple connections
            // make sure you return the one that isn't detached first;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                foreach (SR sr in connection.Cache.SRs)
                    if (sr.uuid == uuid && sr.HasPBDs())
                        return sr;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                foreach (SR sr in connection.Cache.SRs)
                    if (sr.uuid == uuid)
                        return sr;

            return null;
        }

        private static bool NameExists(String potentialName, IEnumerable<string> names)
        {
            if (names != null)
                return names.Any(name => name.Equals(potentialName));

            return false;
        }

        public static String DefaultSRName(String potentialName, IEnumerable<string> names)
        {
            if (!NameExists(potentialName, names))
                return potentialName;

            int i = 0;

            while (true)
            {
                i++;

                String name = string.Format(Messages.NEWVM_DEFAULTNAME, potentialName, i);

                if (!NameExists(name, names))
                    return name;
            }
        }
        
        public static String DefaultSRName(String potentialName, IXenConnection connection)
        {
            return DefaultSRName(potentialName, connection.Cache.SRs.Select(sr => sr.Name()));
        }
    }
}
