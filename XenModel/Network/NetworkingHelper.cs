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
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Network
{
    public class NetworkingHelper
    {
        /// <summary>
        /// Returns the bond that the master is using as its management interface, or null if
        /// such a thing cannot be found.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Bond GetMasterManagementBond(IXenConnection conn)
        {
            PIF pif = GetMasterManagementPIF(conn);
            return pif == null ? null : pif.BondMasterOf();
        }

        private static PIF GetMasterManagementPIF(IXenConnection conn)
        {
            Host host = Helpers.GetMaster(conn);
            return host == null ? null : GetManagementPIF(host);
        }

        public static PIF GetManagementPIF(Host host)
        {
            foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
            {
                if (pif.management)
                    return pif;
            }
            return null;
        }

        /// <summary>
        /// Returns all of the PIFs on the given host that represent NICs,
        /// i.e. those that are IsPhysical == true and IsBondNIC == false.
        /// The PIFs will be sorted by name too.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static List<PIF> GetAllPhysicalPIFs(Host host)
        {
            List<PIF> result = new List<PIF>();
            foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
            {
                if (!pif.IsBondNIC() && pif.IsPhysical())
                    result.Add(pif);
            }

            Sort(result);

            return result;
        }

        /// <summary>
        /// Returns all of the PIFs in the given pool that represent NICs,
        /// i.e. those that are IsPhysical == true and IsBondNIC == false.
        /// The PIFs will be sorted by name, and deduplicated by name -- i.e. there will be only
        /// one per device name, even if there are multiple hosts in the pool.
        /// 
        /// Note that this is not the same as GetAllPhysicalPIFs(pool.master) because the master
        /// may have fewer NICs than other members in the pool.
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static List<PIF> GetAllPhysicalPIFs(Pool pool)
        {
            List<PIF> result = new List<PIF>();

            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                List<PIF> pifs = GetAllPhysicalPIFs(host);

                foreach (PIF pif in pifs)
                {
                    if (null == result.Find((Predicate<PIF>)delegate(PIF p) { return PIFsMatch(p, pif); }))
                        result.Add(pif);
                }
            }

            Sort(result);

            return result;
        }
            
        /// <summary>
        /// Given a list of PIFs on the pool master, return a Dictionary mapping each host in the pool to a corresponding list of PIFs on that
        /// host.  The PIFs lists will be sorted by name too.
        /// </summary>
        /// <param name="PIFs_on_master"></param>
        /// <returns></returns>
        public static Dictionary<Host, List<PIF>> PIFsOnAllHosts(List<PIF> PIFs_on_master)
        {
            Dictionary<Host, List<PIF>> result = new Dictionary<Host, List<PIF>>();

            if (PIFs_on_master.Count == 0)
                return result;

            IXenConnection conn = PIFs_on_master[0].Connection;

            List<string> devices = GetDevices(PIFs_on_master);

            foreach (Host host in conn.Cache.Hosts)
            {
                List<PIF> pifs = new List<PIF>();
                foreach (PIF pif in conn.ResolveAll(host.PIFs))
                {
                    if (devices.Contains(pif.device))
                        pifs.Add(pif);
                }

                Sort(pifs);

                result[host] = pifs;
            }

            return result;
        }



        private static List<string> GetDevices(List<PIF> pifs)
        {
            List<string> result = new List<string>();
            foreach (PIF pif in pifs)
            {
                result.Add(pif.device);
            }
            return result;
        }

        /// <summary>
        /// Return the bond equivalent to the given one, but on the given host.  Returns if no
        /// such thing exists.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="bond"></param>
        /// <returns></returns>
        public static Bond FindBond(Host host, Bond bond)
        {
            foreach (Bond b in host.Connection.Cache.Bonds)
            {
                PIF master = host.Connection.Resolve(b.master);
                if (master != null && master.host.opaque_ref == host.opaque_ref && BondMatches(b, bond))
                    return b;
            }
            return null;
        }

        private static bool BondMatches(Bond b1, Bond b2)
        {
            List<PIF> s1 = b1.Connection.ResolveAll(b1.slaves);
            List<PIF> s2 = b2.Connection.ResolveAll(b2.slaves);
            if (s1.Count > s2.Count)
            {
                // Force s1.Count <= s2.Count.
                List<PIF> tmp = s1;
                s1 = s2;
                s2 = tmp;
            }

            foreach (PIF p1 in s1)
            {
                if (s2.RemoveAll((Predicate<PIF>)delegate(PIF p) { return PIFsMatch(p, p1); }) != 1)
                    return false;
            }

            // We can still have elements left in s2.  This is OK though -- bonds are considered to
            // match if one has a subset of the slaves of the other.  This is why we force
            // s1.Count <= s2.Count above.

            return true;
        }

        private static bool PIFsMatch(PIF pif1, PIF pif2)
        {
            return pif1.device == pif2.device;
        }

        /// <summary>
        /// Sort pifs according to pif.ToString().  This is the order that is used in the
        /// box on the Create Bond dialog, and so is a good choice to use as the order
        /// in all other places in the UI too.  PIFs have to be sorted because bonds are
        /// created with the MAC address taken from the first PIF in the bond.
        /// </summary>
        /// <param name="pifs"></param>
        public static void Sort(List<PIF> pifs)
        {
            pifs.Sort(ComparePIFs);
        }

        private static int ComparePIFs(PIF p1, PIF p2)
        {
            return p1.ToString().CompareTo(p2.ToString());
        }

      

        /// <summary>
        /// Copy the IP details from src to a clone of dest, and return the clone.
        /// </summary>
        public static PIF CopyIPConfig(PIF src, PIF dest)
        {
            PIF result = (PIF)dest.Clone();
            result.SetManagementPurspose(src.GetManagementPurpose());
            result.ip_configuration_mode = src.ip_configuration_mode;
            result.IP = src.IP;
            result.netmask = src.netmask;
            result.gateway = src.gateway;
            result.DNS = src.DNS;
            return result;
        }

        public static bool ContainsPrimaryManagement(List<PIF> PIFs)
        {
            return PIFs.Any(p => p.management);
        }

        public static bool ContainsSecondaryManagement(List<PIF> PIFs)
        {
            return PIFs.Any(p => p.IsSecondaryManagementInterface(true));
        }

        public static bool ContainsClusteringPif(List<PIF> PIFs)
        {
            return PIFs.Any(p => p.IsUsedByClustering());
        }
       
    }
}
