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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using XenAdmin.Network;

using XenAPI;
using System.Globalization;
using System.Reflection;
using System.Xml;
using XenCenterLib;


namespace XenAdmin.Core
{
    public static class Helpers
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const long XLVHD_DEF_ALLOCATION_QUANTUM_DIVISOR = 10000;
        public const long XLVHD_MIN_ALLOCATION_QUANTUM_DIVISOR = 50000;
        public const long XLVHD_MAX_ALLOCATION_QUANTUM_DIVISOR = 4000;
        public const long XLVHD_MIN_ALLOCATION_QUANTUM = 16777216; // 16 MB

        public const int DEFAULT_NAME_TRIM_LENGTH = 50;
        
        public const string GuiTempObjectPrefix = "__gui__";

        public const string PRODUCT_BRAND_XCP_NG = "XCP-ng";

        public static NumberFormatInfo _nfi = new CultureInfo("en-US", false).NumberFormat;

        public static readonly Regex SessionRefRegex = new Regex(@"OpaqueRef:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");


        /// <summary>
        /// Return the given host's product version, or the pool master's product version if
        /// the host does not have one, or null if none can be found.
        /// </summary>
        /// <param name="Host">May be null.</param>
        public static string HostProductVersion(Host host)
        {
            return FromHostOrMaster(host, h => h.ProductVersion());
        }

        public static string HostProductVersionText(Host host)
        {
            return FromHostOrMaster(host, h => h.ProductVersionText());
        }

        public static string HostProductVersionTextShort(Host host)
        {
            return FromHostOrMaster(host, h => h.ProductVersionTextShort());
        }

        public static string HostPlatformVersion(Host host)
        {
            if (host == null)
                return null;

            return host.PlatformVersion();
        }

        private delegate string HostToStr(Host host);
        private static string FromHostOrMaster(Host host, HostToStr fn)
        {
            if (host == null)
                return null;

            string output = fn(host);

            if (output == null)
            {
                Host master = GetMaster(host.Connection);
                return master == null ? null : fn(master);
            }

            return output;
        }

        /// <summary>
        /// Only log the unrecognised version message once (CA-11201).
        /// </summary>
        private static bool _unrecognisedVersionWarned = false;
        /// <summary>
        /// Numbers should have three parts, i.e. be in the form a.b.c, otherwise they won't be parsed.
        /// </summary>
        /// <param name="version1">May be null.</param>
        /// <param name="version2">May be null.</param>
        /// <returns></returns>
        public static int productVersionCompare(string version1, string version2)
        {
            // Assume version numbers are of form 'a.b.c'
            int a1 = 99, b1 = 99, c1 = 99, a2 = 99, b2 = 99, c2 = 99;


            string[] tokens = null;
            if (version1 != null)
                tokens = version1.Split('.');
            if (tokens != null && tokens.Length == 3)
            {
                a1 = int.Parse(tokens[0]);
                b1 = int.Parse(tokens[1]);
                c1 = int.Parse(tokens[2]);
            }
            else
            {
                if (!_unrecognisedVersionWarned)
                {
                    log.DebugFormat("Unrecognised version format {0} - treating as developer version", version1);
                    _unrecognisedVersionWarned = true;
                }
            }
            tokens = null;
            if (version2 != null)
                tokens = version2.Split('.');
            if (tokens != null && tokens.Length == 3)
            {
                a2 = int.Parse(tokens[0]);
                b2 = int.Parse(tokens[1]);
                c2 = int.Parse(tokens[2]);
            }
            else
            {
                if (!_unrecognisedVersionWarned)
                {
                    log.DebugFormat("Unrecognised version format {0} - treating as developer version", version2);
                    _unrecognisedVersionWarned = true;
                }
            }

            if (a2 > a1)
            {
                return -1;
            }
            else if (a2 == a1)
            {
                if (b2 > b1)
                {
                    return -1;
                }
                else if (b2 == b1)
                {
                    if (c2 > c1)
                    {
                        return -1;
                    }
                    else if (c1 == c2)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// Gets the pool for the provided connection. Returns null if we have
        /// a standalone host (or the provided connection is null).
        /// </summary>
        /// <remarks> To obtain the pool object for the case of a standalone host
        /// use GetPoolOfOne.</remarks>
        public static Pool GetPool(IXenConnection connection)
        {
            if (connection == null)
                return null;

            foreach (Pool thePool in connection.Cache.Pools)
            {
                if (thePool != null && thePool.IsVisible())
                    return thePool;
            }
            return null;
        }

        /// <summary>
        /// Get the unique Pool object corresponding to the given connection.
        /// Returns the pool object even in the case of a standalone host. May
        /// return null if the cache is still being populated or the given
        /// connection is null.
        /// </summary>
        public static Pool GetPoolOfOne(IXenConnection connection)
        {
            if (connection == null)
                return null;

            foreach (Pool pool in connection.Cache.Pools)
                return pool;
            
            return null;
        }

        /// <summary>
        /// Return the host object representing the master of the given connection, or null if the
        /// cache is being populated.
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        public static Host GetMaster(IXenConnection connection)
        {
            Pool pool = GetPoolOfOne(connection);
            return pool == null ? null : connection.Resolve(pool.master);
        }

        /// <summary>
        /// Return the host object representing the master of the given pool.
        /// (If pool is null, returns null).
        /// </summary>
        public static Host GetMaster(Pool pool)
        {
            return pool == null ? null : pool.Connection.Resolve(pool.master);
        }

        public static bool HostIsMaster(Host host)
        {
            Pool pool = Helpers.GetPoolOfOne(host.Connection);
            if (pool == null) //Cache is being populated...  what do we do?
                return false;
            return host.opaque_ref == pool.master.opaque_ref;
        }

        public static bool IsPool(IXenConnection connection)
        {
            return (GetPool(connection) != null);
        }

        /// <param name="pool">May be null, in which case the empty string is returned.</param>
        public static string GetName(Pool pool)
        {
            return pool == null ? "" : pool.Name();
        }

        /// <param name="connection">May be null, in which case the empty string is returned.</param>
        public static string GetName(IXenConnection connection)
        {
            return connection == null ? "" : connection.Name;
        }

        /// <param name="o">May be null, in which case the empty string is returned.</param>
        public static string GetName(IXenObject o)
        {
            return o == null ? "" : o.Name();
        }

        public static bool IsConnected(IXenConnection connection)
        {
            return (connection != null && connection.IsConnected);
        }

        public static bool IsConnected(Pool pool)
        {
            return (pool != null && IsConnected(pool.Connection));
        }

        public static bool IsConnected(Host host)
        {
            return (host != null && IsConnected(host.Connection));
        }

        public static bool HasFullyConnectedSharedStorage(IXenConnection connection)
        {
            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.content_type != XenAPI.SR.Content_Type_ISO && sr.shared && sr.CanCreateVmOn())
                    return true;
            }
            return false;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool DundeeOrGreater(IXenConnection conn)
        {
            return conn == null || DundeeOrGreater(Helpers.GetMaster(conn));
        }

        /// Dundee is ver. 2.0.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool DundeeOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.0.0") >= 0;
        }

        public static bool DundeePlusOrGreater(IXenConnection conn)
        {
            return conn == null || conn.Session == null || conn.Session.APIVersion >= API_Version.API_2_6;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool ElyOrGreater(IXenConnection conn)
        {
            return conn == null || ElyOrGreater(Helpers.GetMaster(conn));
        }

        /// Ely is ver. 2.1.1
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool ElyOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.1.1") >= 0;
        }

        public static bool HavanaOrGreater(IXenConnection conn)
        {
            return conn == null || HavanaOrGreater(Helpers.GetMaster(conn));
        }

        /// As Havana platform version is same with Ely and Honolulu, so use product version here
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool HavanaOrGreater(Host host)
        {
            if (host == null)
                return true;
             string product_version = HostProductVersion(host);
            return
                product_version != null &&
                ElyOrGreater(host) && 
                !FalconOrGreater(host) && 
                productVersionCompare(product_version, "[BRANDING_VERSION_7_1_2]") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool FalconOrGreater(IXenConnection conn)
        {
            return conn == null || FalconOrGreater(Helpers.GetMaster(conn));
        }

        /// Falcon is ver. 2.3.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool FalconOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.2.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool InvernessOrGreater(IXenConnection conn)
        {
            return conn == null || InvernessOrGreater(Helpers.GetMaster(conn));
        }

        /// Inverness is ver. 2.4.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool InvernessOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.3.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
         public static bool JuraOrGreater(IXenConnection conn)
        {
            return conn == null || JuraOrGreater(Helpers.GetMaster(conn));
        }

        /// Jura is ver. 2.5.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool JuraOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.4.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool KolkataOrGreater(IXenConnection conn)
        {
            return conn == null || KolkataOrGreater(Helpers.GetMaster(conn));
        }

        /// Kolkata platform version is 2.6.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool KolkataOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.5.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool LimaOrGreater(IXenConnection conn)
        {
            return conn == null || LimaOrGreater(Helpers.GetMaster(conn));
        }

        /// Lima platform version is 2.7.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool LimaOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platform_version = HostPlatformVersion(host);
            return platform_version != null && productVersionCompare(platform_version, "2.6.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool NaplesOrGreater(IXenConnection conn)
        {
            return conn == null || NaplesOrGreater(GetMaster(conn));
        }

        /// Naples is ver. 3.0.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool NaplesOrGreater(Host host)
        {
            return host == null || NaplesOrGreater(HostPlatformVersion(host));
        }

        public static bool NaplesOrGreater(string platformVersion)
        {
            return platformVersion != null && productVersionCompare(platformVersion, "2.9.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool QuebecOrGreater(IXenConnection conn)
        {
            return conn == null || QuebecOrGreater(GetMaster(conn));
        }

        /// Quebec platform version is 3.1.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool QuebecOrGreater(Host host)
        {
            return host == null || QuebecOrGreater(HostPlatformVersion(host));
        }

        public static bool QuebecOrGreater(string platformVersion)
        {
            return platformVersion != null && productVersionCompare(platformVersion, "3.0.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool StockholmOrGreater(IXenConnection conn)
        {
            return conn == null || StockholmOrGreater(Helpers.GetMaster(conn));
        }

        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool StockholmOrGreater(Host host)
        {
            return host == null || StockholmOrGreater(HostPlatformVersion(host));
        }

        /// Stockholm is ver. 3.2.0
        public static bool StockholmOrGreater(string platformVersion)
        {
            return platformVersion != null && productVersionCompare(platformVersion, "3.1.50") >= 0;
        }

        // CP-3435: Disable Check for Updates in Common Criteria Certification project
        public static bool CommonCriteriaCertificationRelease
        {
            get { return false; }
        }

        public static bool WlbEnabled(IXenConnection connection)
        {
            Pool pool = GetPoolOfOne(connection);
            return pool != null && pool.wlb_enabled;
        }

        public static bool WlbEnabledAndConfigured(IXenConnection conn)
        {
            return WlbEnabled(conn)&& WlbConfigured(conn);
        }

        public static bool WlbConfigured(IXenConnection conn)
        {
            Pool p = GetPoolOfOne(conn);
            return p != null && !string.IsNullOrEmpty(p.wlb_url);
        }

        public static bool CrossPoolMigrationRestrictedWithWlb(IXenConnection conn)
        {
            return WlbEnabledAndConfigured(conn) && !DundeeOrGreater(conn);
        }


        /// <summary>
        /// Determines whether two lists contain the same elements (but not necessarily in the same order).
        /// Compares list elements using reference equality.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst1">Must not be null.</param>
        /// <param name="lst2">Must not be null.</param>
        /// <returns></returns>
        public static bool ListsContentsEqual<T>(List<T> lst1, List<T> lst2)
        {
            if (lst1.Count != lst2.Count)
                return false;
            foreach (T item1 in lst1)
                if (!lst2.Contains(item1))
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst1">Must not be null.</param>
        /// <param name="lst2">Must not be null.</param>
        /// <returns></returns>
        public static List<T> ListsCommonItems<T>(List<T> lst1, List<T> lst2)
        {
            List<T> common = new List<T>();
            foreach (T item1 in lst1)
                if (lst2.Contains(item1) && !common.Contains(item1))
                    common.Add(item1);
            foreach (T item2 in lst2)
                if (lst1.Contains(item2) && !common.Contains(item2))
                    common.Add(item2);
            return common;
        }

        /// <summary>
        /// Determines if two arrays have the same elements in the same order.
        /// Elements may be null.
        /// Uses Object.Equals() when comparing (pairs of non-null) elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">Must not be null.</param>
        /// <param name="b">Must not be null.</param>
        /// <returns></returns>
        public static bool ArrayElementsEqual<T>(T[] a, T[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == null && b[i] == null)
                    continue;
                if (a[i] == null)
                    return false;
                if (!a[i].Equals(b[i]))
                    return false;
            }

            return true;
        }

        internal static bool OrderOfMagnitudeDifference(long MinSize, long MaxSize)
        {
            if (MinSize < Util.BINARY_KILO && MaxSize < Util.BINARY_KILO)
                return false;
            else if (MinSize < Util.BINARY_KILO && MaxSize > Util.BINARY_KILO)
                return true;
            else if (MinSize > Util.BINARY_KILO && MaxSize < Util.BINARY_KILO)
                return true;

            if (MinSize < Util.BINARY_MEGA && MaxSize < Util.BINARY_MEGA)
                return false;
            else if (MinSize < Util.BINARY_MEGA && MaxSize > Util.BINARY_MEGA)
                return true;
            else if (MinSize > Util.BINARY_MEGA && MaxSize < Util.BINARY_MEGA)
                return true;

            if (MinSize < Util.BINARY_GIGA && MaxSize < Util.BINARY_GIGA)
                return false;
            else if (MinSize < Util.BINARY_GIGA && MaxSize > Util.BINARY_GIGA)
                return true;
            else if (MinSize > Util.BINARY_GIGA && MaxSize < Util.BINARY_GIGA)
                return true;

            return false;
        }

        public static string StringFromMaxMinSize(long min, long max)
        {
            if (min == -1 && max == -1)
                return Messages.SIZE_NEGLIGIBLE;
            else if (min == -1)
                return Util.LThanSize(max);
            else if (max == -1)
                return Util.GThanSize(min);
            else if (min == max)
                return Util.DiskSizeString(max);
            else if (Helpers.OrderOfMagnitudeDifference(min, max))
                return string.Format("{0} - {1}", Util.DiskSizeString(min), Util.DiskSizeString(max));
            else
                return string.Format("{0} - {1}", Util.DiskSizeStringWithoutUnits(min), Util.DiskSizeString(max));
        }

        public static string StringFromMaxMinSizeList(List<long> minList, List<long> maxList)
        {
            bool lessFlag = false;
            bool moreFlag = false;
            bool negligFlag = false;

            long minSum = 0;
            long maxSum = 0;

            for (int i = 0; i < minList.Count; i++)
            {
                if (minList[i] < 0 && maxList[i] < 0)
                {
                    negligFlag = true;
                }
                else if (minList[i] < 0)
                {
                    maxSum += maxList[i];
                    lessFlag = true;
                }
                else if (maxList[i] < 0)
                {
                    minSum += minList[i];
                    moreFlag = true;
                }
                else
                {
                    minSum += minList[i];
                    maxSum += maxList[i];
                }
            }

            if (moreFlag)
                return Util.GThanSize(minSum);
            if (lessFlag)
                return Util.LThanSize(maxSum);
            if (negligFlag && maxSum <= 0)
                return Util.DiskSizeString(maxSum);
            return StringFromMaxMinSize(minSum, maxSum);
        }


        public static string GetCPUProperties(Host_cpu cpu)
        {
            return string.Format("{0}\n{1}\n{2}",
                string.Format(Messages.GENERAL_CPU_VENDOR, cpu.vendor),
                string.Format(Messages.GENERAL_CPU_MODEL, cpu.modelname),
                string.Format(Messages.GENERAL_CPU_SPEED, cpu.speed));
        }

        public static string GetAllocationProperties(string initial_allocation, string quantum_allocation)
        {
            return string.Format(Messages.SR_DISK_SPACE_ALLOCATION,
                   Util.MemorySizeStringSuitableUnits(Convert.ToDouble(initial_allocation), true, Messages.VAL_MB),
                   Util.MemorySizeStringSuitableUnits(Convert.ToDouble(quantum_allocation), true, Messages.VAL_MB));
        }

        public static string GetHostRestrictions(Host host)
        {
            string output = "";

            List<string> restrictions = new List<string>();

            // Build license details info box
            foreach (String key in host.license_params.Keys)
            {
                if (host.license_params[key] == "true")
                {
                    restrictions.Add(key);
                }
            }

            bool first = true;
            restrictions.Sort();
            foreach (String restriction in restrictions)
            {
                string restrictionText = Messages.ResourceManager.GetString(restriction);
                if (restrictionText == null)
                    continue;

                if (first)
                {
                    output += restrictionText;
                    first = false;
                    continue;
                }

                output += "\n" + restrictionText;
            }
            return output;
        }

        public static string BoolToString(bool b)
        {
            return b ? Messages.YES : Messages.NO;
        }

        private static readonly Regex IqnRegex = new Regex(@"^iqn\.\d{4}-\d{2}\.([a-zA-Z0-9][-_a-zA-Z0-9]*(\.[a-zA-Z0-9][-_a-zA-Z0-9]*)*)(:.+)?$", RegexOptions.ECMAScript);

        public static bool ValidateIscsiIQN(string iqn)
        {
            return IqnRegex.IsMatch(iqn);
        }

        public static bool IsOlderThanMaster(Host host)
        {
            Host master = Helpers.GetMaster(host.Connection);
            if (master == null || master.opaque_ref == host.opaque_ref)
                return false;
            else if (Helpers.productVersionCompare(Helpers.HostProductVersion(host), Helpers.HostProductVersion(master)) >= 0)
                return false;
            else
                return true;
        }


        private static readonly Regex MacRegex = new Regex(@"^([0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2}$");

        public static bool IsValidMAC(string macString)
        {
            return MacRegex.IsMatch(macString);
        }

        /// <summary>
        /// Recursively sums the total size of all files in the directory tree rooted at the given dir.
        /// </summary>
        public static long GetDirSize(DirectoryInfo dir)
        {
            long size = 0;
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }

            DirectoryInfo[] subdirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                size += GetDirSize(subdir);
            }
            return size;
        }

        public static string ToWindowsLineEndings(string input)
        {
            return Regex.Replace(input, "\r?\n", "\r\n");
        }

        public static bool PoolHasEnabledHosts(Pool pool)
        {
            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                if (host.enabled)
                {
                    return true;
                }
            }
            return false;
        }


        public static string MakeUniqueName(string stub, List<string> compareAgainst)
        {
            string pre = stub;
            int i = 1;
            while (compareAgainst.Contains(stub))
            {
                stub = string.Format(Messages.NEWVM_DEFAULTNAME, pre, i);
                i++;
            }
            return stub;
        }

        public static string MakeUniqueNameFromPattern(string pattern, List<string> compareAgainst, int startAt)
        {
            int i = startAt;
            string val;
            do
            {
                val = string.Format(pattern, i++);
            } while (compareAgainst.Contains(val));

            return val;
        }

        public static string MakeHiddenName(string name)
        {
            return string.Format("{0}{1}", GuiTempObjectPrefix, name);
        }

        public static string GetFriendlyLicenseName(Pool pool)
        {
            var hosts = new List<Host>(pool.Connection.Cache.Hosts);

            if (hosts.Count > 0)
            {
                var editions = Enum.GetValues(typeof(Host.Edition));
                foreach (Host.Edition edition in editions)
                {
                    Host.Edition edition1 = edition;
                    Host host = hosts.Find(h => Host.GetEdition(h.edition) == edition1);

                    if (host != null)
                        return GetFriendlyLicenseName(host);
                }
            }

            return Messages.UNKNOWN;
        }

        public static string GetFriendlyLicenseName(Host host)
        {
			if (string.IsNullOrEmpty(host.edition))
				return Messages.UNKNOWN;

            string legacy = NaplesOrGreater(host) ? "" : "legacy-";
            string name = FriendlyNameManager.GetFriendlyName("Label-host.edition-" + legacy + host.edition);
            return name ?? Messages.UNKNOWN;
        }

        /// <summary>
        /// Used for determining which features are available on the current license.
        /// Note that all the features are per-pool: e.g., even if iXenObject is a Host,
        /// we return whether *any* the hosts in that pool are forbidden that feature.
        /// </summary>
        public static bool FeatureForbidden(IXenObject iXenObject, Predicate<Host> restrictionTest)
        {
            IXenConnection connection = (iXenObject == null ? null : iXenObject.Connection);
            return FeatureForbidden(connection, restrictionTest);
        }

        public static bool FeatureForbidden(IXenConnection xenConnection, Predicate<Host> restrictionTest)
        {
            if (xenConnection == null)
                return false;

            foreach (Host host in xenConnection.Cache.Hosts)
            {
                if (restrictionTest(host))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Parse string represented double to a double with en-US number format
        /// </summary>
        /// <param name="toParse">String represented double</param>
        /// <param name="defaultValue">Default value to use if the string can't be parsed</param>
        /// <returns>The parsed double.</returns>
        public static double ParseStringToDouble(string toParse, double defaultValue)
        {
            if (double.TryParse(toParse, NumberStyles.Any, _nfi, out var doubleValue))
                return doubleValue;

            return defaultValue;
        }

        /// <summary>
        /// Return the UUID of the given XenObject, or the empty string if that can't be found.
        /// </summary>
        public static string GetUuid(IXenObject iXenObject)
        {
            if (iXenObject == null)
                return "";
            Type t = iXenObject.GetType();
            PropertyInfo p = t.GetProperty("uuid", BindingFlags.Public | BindingFlags.Instance);
            if (p == null)
                return "";
            return (string)p.GetValue(iXenObject, null);
        }

        #region Reflexive other_config and gui_config functions

        public static Dictionary<String, String> GetOtherConfig(IXenObject o)
        {
            return o.Get("other_config") as Dictionary<String, String>;
        }

        public static void SetOtherConfig(Session session, IXenObject o, String key, String value)
        {
            //Program.AssertOffEventThread();

            o.Do("remove_from_other_config", session, o.opaque_ref, key);
            o.Do("add_to_other_config", session, o.opaque_ref, key, value);
        }

        public static void RemoveFromOtherConfig(Session session, IXenObject o, string key)
        {
            //Program.AssertOffEventThread();

            o.Do("remove_from_other_config", session, o.opaque_ref, key);
        }

        public static Dictionary<String, String> GetGuiConfig(IXenObject o)
        {
            return o.Get("gui_config") as Dictionary<String, String>;
        }

        public static void SetGuiConfig(Session session, IXenObject o, String key, String value)
        {
            //Program.AssertOffEventThread();

            o.Do("remove_from_gui_config", session, o.opaque_ref, key);
            o.Do("add_to_gui_config", session, o.opaque_ref, key, value);
        }

        public static void RemoveFromGuiConfig(Session session, IXenObject o, string key)
        {
            //Program.AssertOffEventThread();

            o.Do("remove_from_gui_config", session, o.opaque_ref, key);
        }

        #endregion

        public static Regex CpuRegex = new Regex("^cpu([0-9]+)$");
        public static Regex CpuAvgFreqRegex = new Regex("^CPU([0-9]+)-avg-freq$");
        public static Regex CpuStateRegex = new Regex("^cpu([0-9]+)-(C|P)([0-9]+)$");
        static Regex VifRegex = new Regex("^vif_([0-9]+)_(tx|rx)((_errors)?)$");
        static Regex PifEthRegex = new Regex("^pif_eth([0-9]+)_(tx|rx)((_errors)?)$");
		static Regex PifVlanRegex = new Regex("^pif_eth([0-9]+).([0-9]+)_(tx|rx)((_errors)?)$");
        static Regex PifBrRegex = new Regex("^pif_xenbr([0-9]+)_(tx|rx)((_errors)?)$");
		static Regex PifXapiRegex = new Regex("^pif_xapi([0-9]+)_(tx|rx)((_errors)?)$");
        static Regex PifTapRegex = new Regex("^pif_tap([0-9]+)_(tx|rx)((_errors)?)$");
        static Regex PifLoRegex = new Regex("^pif_lo_(tx|rx)((_errors)?)$");
        static Regex PifBondRegex = new Regex("^pif_(bond[0-9]+)_(tx|rx)((_errors)?)$");
        static Regex DiskRegex = new Regex("^vbd_((xvd|hd)[a-z]+)_(read|write)((_latency)?)$");
        static Regex DiskIopsRegex = new Regex("^vbd_((xvd|hd)[a-z]+)_iops_(read|write|total)$");
        static Regex DiskThroughputRegex = new Regex("^vbd_((xvd|hd)[a-z]+)_io_throughput_(read|write|total)$");
        static Regex DiskOtherRegex = new Regex("^vbd_((xvd|hd)[a-z]+)_(avgqu_sz|inflight|iowait)$");
        static Regex NetworkLatencyRegex = new Regex("^network/latency$");
        static Regex XapiLatencyRegex = new Regex("^xapi_healthcheck/latency$");
        static Regex StatefileLatencyRegex = new Regex("^statefile/[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}/latency$");
        static Regex LoadAvgRegex = new Regex("loadavg");
    	static Regex SrRegex = new Regex("^sr_[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}_cache_(size|hits|misses)");
        static Regex SrIORegex = new Regex("^(io_throughput|iops)_(read|write|total)_([a-f0-9]{8})$");
        static Regex SrOtherRegex = new Regex("^(latency|avgqu_sz|inflight|iowait)_([a-f0-9]{8})$");
        static Regex SrReadWriteRegex = new Regex("^((read|write)(_latency)?)_([a-f0-9]{8})$");
        static Regex GpuRegex = new Regex(@"^gpu_((memory_(free|used))|power_usage|temperature|(utilisation_(compute|memory_io)))_((([a-fA-F0-9]{4}\/)|([a-fA-F0-9]{8}\/))?[a-fA-F0-9]{2}\/[0-1][a-fA-F0-9].[0-7])$");

        public static string GetFriendlyDataSourceName(string name, IXenObject iXenObject)
        {
            if (iXenObject == null)
                return name;
            string s = GetFriendlyDataSourceName_(name, iXenObject);
            return string.IsNullOrEmpty(s) ? name : s;
        }

        private static string GetFriendlyDataSourceName_(string name, IXenObject iXenObject)
        {
            Match m;

            m = CpuRegex.Match(name);
            if (m.Success)
                return FormatFriendly("Label-performance.cpu", m.Groups[1].Value);

            m = CpuAvgFreqRegex.Match(name);
            if (m.Success)
                return FormatFriendly("Label-performance.cpu-avg-freq", m.Groups[1].Value);

            m = VifRegex.Match(name);
            if (m.Success)
            {
                string device = m.Groups[1].Value;
                XenAPI.Network network = FindNetworkOfVIF(iXenObject, device);
                return network == null
                           ? null //don't try to retrieve it in the FriendlyNames.
                           : FormatFriendly(string.Format("Label-performance.vif_{0}{1}",
                               m.Groups[2].Value, m.Groups[3].Value), network.Name());
            }

            m = PifEthRegex.Match(name);
            if (m.Success)
                return FormatFriendly(string.Format("Label-performance.nic_{0}{1}", m.Groups[2].Value, m.Groups[3].Value), m.Groups[1].Value);

        	m = PifVlanRegex.Match(name);
			if (m.Success)
			{
				string device = string.Format("eth{0}", m.Groups[1].Value);
				XenAPI.Network network = FindVlan(iXenObject, device, m.Groups[2].Value);
			    return network == null
			               ? null //don't try to retrieve it in the FriendlyNames.
			               : FormatFriendly(string.Format("Label-performance.vlan_{0}{1}",
			                   m.Groups[3].Value, m.Groups[4].Value), network.Name());
			}

            m = PifBrRegex.Match(name);
            if (m.Success)
            {
                string device = string.Format("eth{0}", m.Groups[1].Value);
                XenAPI.Network network = FindNetworkOfPIF(iXenObject, device);
            	return network == null
            	       	? null //don't try to retrieve it in the FriendlyNames.
            	       	: FormatFriendly(string.Format("Label-performance.xenbr_{0}{1}", m.Groups[2].Value, m.Groups[3].Value), network.Name());
            }

        	m = PifXapiRegex.Match(name);
			if (m.Success)
				return FormatFriendly(string.Format("Label-performance.xapi_{0}{1}", m.Groups[2].Value, m.Groups[3].Value), m.Groups[1].Value);

            m = PifBondRegex.Match(name);
            if (m.Success)
            {
            	PIF pif = FindPIF(iXenObject, m.Groups[1].Value, false);
            	return pif == null
            	       	? null //pif doesn't exist anymore so don't try to retrieve it in the FriendlyNames.
            	       	: FormatFriendly(string.Format("Label-performance.bond_{0}{1}", m.Groups[2].Value, m.Groups[3].Value), pif.Name());
            }

        	m = PifLoRegex.Match(name);
            if (m.Success)
                return FormatFriendly(string.Format("Label-performance.lo_{0}{1}", m.Groups[1].Value, m.Groups[2].Value));

            m = PifTapRegex.Match(name);
            if (m.Success)
                return FormatFriendly(string.Format("Label-performance.tap_{0}{1}", m.Groups[2].Value, m.Groups[3].Value), m.Groups[1].Value);

            m = DiskRegex.Match(name);
            if (m.Success)
            {
                VBD vbd = FindVBD(iXenObject, m.Groups[1].Value);
                return vbd == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.vbd_{0}{1}", m.Groups[3].Value, m.Groups[4].Value), vbd.userdevice);
            }

            m = DiskIopsRegex.Match(name);
            if (m.Success)
            {
                VBD vbd = FindVBD(iXenObject, m.Groups[1].Value);
                return vbd == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.vbd_iops_{0}", m.Groups[3].Value),
                               vbd.userdevice);
            }

            m = DiskThroughputRegex.Match(name);
            if (m.Success)
            {
                VBD vbd = FindVBD(iXenObject, m.Groups[1].Value);
                return vbd == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.vbd_io_throughput_{0}", m.Groups[3].Value),
                               vbd.userdevice);
            }

            m = DiskOtherRegex.Match(name);
            if (m.Success)
            {
                VBD vbd = FindVBD(iXenObject, m.Groups[1].Value);
                return vbd == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.vbd_{0}", m.Groups[3].Value),
                               vbd.userdevice);
            }

			m = SrRegex.Match(name);
			if (m.Success)
				return FormatFriendly(string.Format("Label-performance.sr_cache_{0}", m.Groups[1].Value));

            m = CpuStateRegex.Match(name);
            if (m.Success)
                return FormatFriendly("Label-performance.cpu-state", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);

            m = SrIORegex.Match(name);
            if (m.Success)
            {
                SR sr = FindSr(iXenObject, m.Groups[3].Value);
                return sr == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.sr_{0}_{1}",
                               m.Groups[1].Value, m.Groups[2].Value),
                               sr.Name().Ellipsise(30));
            }

            m = SrOtherRegex.Match(name);
            if (m.Success)
            {
                SR sr = FindSr(iXenObject, m.Groups[2].Value);
                return sr == null
                           ? null
                           : FormatFriendly(string.Format("Label-performance.sr_{0}", m.Groups[1].Value),
                               sr.Name().Ellipsise(30));
            }

            m = SrReadWriteRegex.Match(name);
            if (m.Success)
            {
                SR sr = FindSr(iXenObject, m.Groups[4].Value);
                return sr == null
                    ? null
                    : FormatFriendly(string.Format("Label-performance.sr_rw_{0}", m.Groups[1].Value),
                        sr.Name().Ellipsise(30));
            }

            m = GpuRegex.Match(name);
            if (m.Success)
            {
                string pciId = m.Groups[6].Value.Replace(@"/", ":");
                PGPU gpu = FindGpu(iXenObject, pciId);

                if (gpu == null && string.IsNullOrEmpty(m.Groups[8].Value))
                {
                    pciId = pciId.Substring(4);
                    gpu = FindGpu(iXenObject, pciId);
                }
                return gpu == null
                    ? null
                    : FormatFriendly(string.Format("Label-performance.gpu_{0}", m.Groups[1].Value),
                        gpu.Name(), pciId);
            }

            if (NetworkLatencyRegex.IsMatch(name))
                return FriendlyNameManager.GetFriendlyName("Label-performance.network_latency");

            if (XapiLatencyRegex.IsMatch(name))
                return FriendlyNameManager.GetFriendlyName("Label-performance.xapi_latency");

            if (StatefileLatencyRegex.IsMatch(name))
                return FriendlyNameManager.GetFriendlyName("Label-performance.statefile_latency");

            if (LoadAvgRegex.IsMatch(name))
                return FriendlyNameManager.GetFriendlyName("Label-performance.loadavg");

            return FriendlyNameManager.GetFriendlyName(string.Format("Label-performance.{0}", name));
        }

        /// <summary>
        /// Lookup key using PropertyManager.GetFriendlyName, and then apply that as a format pattern to the given args.
        /// </summary>
        private static string FormatFriendly(string key, params string[] args)
        {
            return string.Format(FriendlyNameManager.GetFriendlyName(key), args);
        }

        private static PGPU FindGpu(IXenObject iXenObject, string pciId)
        {
            foreach (PCI pci in iXenObject.Connection.Cache.PCIs)
            {
                if (pci.pci_id != pciId)
                    continue;

                foreach (PGPU gpu in iXenObject.Connection.Cache.PGPUs)
                {
                    if (gpu.PCI.opaque_ref == pci.opaque_ref)
                        return gpu;
                }
            }
            return null;
        }

        private static XenAPI.Network FindNetworkOfVIF(IXenObject iXenObject, string device)
        {
            foreach (VIF vif in iXenObject.Connection.Cache.VIFs)
            {
                if (vif.device == device && (iXenObject is Host && ((Host)iXenObject).resident_VMs.Contains(vif.VM) || iXenObject is VM && vif.VM.opaque_ref == iXenObject.opaque_ref))
                {
                    XenAPI.Network network = iXenObject.Connection.Resolve(vif.network);
                    if (network != null)
                        return network;
                }
            }
            return null;
        }

        private static XenAPI.Network FindNetworkOfPIF(IXenObject iXenObject, string device)
        {
            PIF pif = FindPIF(iXenObject, device, true);
            if (pif != null)
            {
                XenAPI.Network network = iXenObject.Connection.Resolve(pif.network);
                if (network != null)
                    return network;
            }
            return null;
        }

		private static XenAPI.Network FindVlan(IXenObject iXenObject, string device, string tag)
		{
			foreach (PIF pif in iXenObject.Connection.Cache.PIFs)
			{
				if (pif.device == device && (iXenObject is Host && pif.host.opaque_ref == iXenObject.opaque_ref || iXenObject is VM && pif.host.opaque_ref == ((VM)iXenObject).resident_on.opaque_ref) && pif.VLAN == long.Parse(tag))
				{
					return iXenObject.Connection.Resolve(pif.network);
				}
			}
			return null;
		}

        private static PIF FindPIF(IXenObject iXenObject, string device, bool physical)
        {
            foreach (PIF pif in iXenObject.Connection.Cache.PIFs)
            {
                if ((!physical || pif.IsPhysical()) && pif.device == device && (iXenObject is Host && pif.host.opaque_ref == iXenObject.opaque_ref || iXenObject is VM && pif.host.opaque_ref == ((VM)iXenObject).resident_on.opaque_ref))
                    return pif;
            }
            return null;
        }

        private static VBD FindVBD(IXenObject iXenObject, string device)
        {
            if (iXenObject is VM)
            {
                VM vm = (VM)iXenObject;
                foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                {
                    if (vbd.device == device)
                        return vbd;
                }
            }
            return null;
        }

        private static SR FindSr(IXenObject iXenObject, string srUuid)
        {
            foreach (var sr in iXenObject.Connection.Cache.SRs)
            {
                if (sr.uuid.StartsWith(srUuid))
                    return sr;
            }
            return null;
        }

        /// <summary>
        /// Returns the first line of the given string, or the empty string if null was passed in. Will take the first occurence of \r or \n
        /// as the end of a line.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstLine(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = s.Split('\n')[0];
            return s.Split('\r')[0];
        }

        public static double StringToDouble(string str)
        {
            if (str == "NaN")
                return Double.NaN;
            else if (str == "Infinity")
                return Double.PositiveInfinity;
            else if (str == "-Infinity")
                return Double.NegativeInfinity;
            else
                return Convert.ToDouble(str, CultureInfo.InvariantCulture);
        }
      
        public static bool HAEnabled(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            return pool.ha_enabled;
        }

        /// <summary>
        /// Returns "type 'name'" for the specified object, e.g. "pool 'foo'".
        /// </summary>
        /// <param name="XenObject"></param>
        /// <returns></returns>
        public static string GetNameAndObject(IXenObject XenObject)
        {
            if (XenObject is Pool)
                return string.Format(Messages.POOL_X, GetName(XenObject));

            if (XenObject is Host)
                return string.Format(Messages.SERVER_X, GetName(XenObject));

            VM vm = XenObject as VM;
            if (vm != null)
            {
                if (vm.IsControlDomainZero())
                    return string.Format(Messages.SERVER_X, GetName(XenObject.Connection.Resolve(vm.resident_on)));
                else
                    return string.Format(Messages.VM_X, GetName(XenObject));
            }

            if (XenObject is SR)
                return string.Format(Messages.STORAGE_REPOSITORY_X, GetName(XenObject));

            return Messages.UNKNOWN_OBJECT;
        }

        /// <summary>
        /// Gets the i18n'd name for a HA restart priority according to FriendlyNames.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static string RestartPriorityI18n(VM.HA_Restart_Priority? priority)
        {
            if (priority == null)
            {
                return "";
            }
            else
            {
                return FriendlyNameManager.GetFriendlyName("Label-VM.ha_restart_priority." + priority.ToString()) ?? priority.ToString();
            }
        }

        public static string RestartPriorityDescription(VM.HA_Restart_Priority? priority)
        {
            if (priority == null)
            {
                return "";
            }
            else
            {
                return FriendlyNameManager.GetFriendlyName("Description-VM.ha_restart_priority." + priority.ToString()) ?? priority.ToString();
            }
        }


        /// <summary>
        /// Builds up a dictionary of the current restart priorities for all the VMs in the given IXenConnection.
        /// </summary>
        /// <param name="connection">Must not be null.</param>
        /// <returns></returns>
        public static Dictionary<VM, VM.HA_Restart_Priority> GetVmHaRestartPriorities(IXenConnection connection,bool showHiddenVMs)
        {
            Dictionary<VM, VM.HA_Restart_Priority> result = new Dictionary<VM, VM.HA_Restart_Priority>();
            foreach (VM vm in connection.Cache.VMs)
            {
                if (vm.HaCanProtect(showHiddenVMs))
                {
                    result[vm] = vm.HARestartPriority();
                }
            }
            return result;
        }

        /// <summary>
        /// Converts Dictionary<XenObject<VM>, VM.HA_Restart_Priority> -> Dictionary<XenRef<VM>, string>.
        /// The former is useful in the GUI, the latter is suitable for sending into compute_hypothetical_max.
        /// </summary>
        /// <param name="settings">Must not be null.</param>
        /// <returns></returns>
        public static Dictionary<XenRef<VM>, string> GetVmHaRestartPrioritiesForApi(Dictionary<VM, VM.HA_Restart_Priority> settings)
        {
            Dictionary<XenRef<VM>, string> result = new Dictionary<XenRef<VM>, string>();
            foreach (VM vm in settings.Keys)
            {
                if (settings[vm] == VM.HA_Restart_Priority.BestEffort || settings[vm] == VM.HA_Restart_Priority.DoNotRestart)
                {
                    // The server doesn't want to know about best-effort/do not restart VMs.
                    // (They don't influence the plan, and sending in the dictionary gives an error).
                    continue;
                }
                result[new XenRef<VM>(vm.opaque_ref)] = VM.PriorityToString(settings[vm]);
            }
            return result;
        }

        /// <summary>
        /// Builds up a dictionary of the current startup options for all the VMs in the given IXenConnection.
        /// </summary>
        /// <param name="connection">Must not be null.</param>
        /// <returns></returns>
        public static Dictionary<VM, VMStartupOptions> GetVmStartupOptions(IXenConnection connection, bool showHiddenVMs)
        {
            Dictionary<VM, VMStartupOptions> result = new Dictionary<VM, VMStartupOptions>();
            foreach (VM vm in connection.Cache.VMs)
            {
                if (vm.HaCanProtect(showHiddenVMs))
                {
                    result[vm] = new VMStartupOptions(vm.order, vm.start_delay, vm.HARestartPriority());
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves a IXenObject from a message. May return null if type not recognised.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IXenObject XenObjectFromMessage(XenAPI.Message message)
        {
            switch (message.cls)
            {
                case cls.Pool:
                    Pool pool = message.Connection.Cache.Find_By_Uuid<Pool>(message.obj_uuid);
                    if (pool != null)
                        return pool;
                    break;
                case cls.Host:
                    Host host = message.Connection.Cache.Find_By_Uuid<Host>(message.obj_uuid);
                    if (host != null)
                        return host;
                    break;
                case cls.VM:
                    VM vm = message.Connection.Cache.Find_By_Uuid<VM>(message.obj_uuid);
                    if (vm != null)
                        return vm;
                    break;
                case cls.SR:
                    SR sr = message.Connection.Cache.Find_By_Uuid<SR>(message.obj_uuid);
                    if (sr != null)
                        return sr;
                    break;
                case cls.VMSS:
                    VMSS vmss = message.Connection.Cache.Find_By_Uuid<VMSS>(message.obj_uuid);
                    if (vmss != null)
                        return vmss;
					 break;					 
                case cls.PVS_proxy:
                    PVS_proxy proxy = message.Connection.Cache.Find_By_Uuid<PVS_proxy>(message.obj_uuid);
                    if (proxy != null)
                        return proxy;
					break;
            }
            return null;
        }

        /// <summary>
        /// Load an xml stream and ignore comments and whitespace
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDocument(Stream xmlStream)
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;

            doc.Load(XmlReader.Create(xmlStream, settings));

            return doc;
        }

        public static Regex HostnameOrIpRegex = new Regex(@"[\w.]+");

        public static string HostnameFromLocation(string p)
        {
            foreach (Match m in HostnameOrIpRegex.Matches(p))
            {
                return m.Value; // we only want the hostname or ip which should be the first match
            }
            return "";
        }

        /// <summary>
        /// Retrieves a float value from an XML attribute.
        /// Returns defaultValue if the attribute doesn't exist.
        /// </summary>
        public static string GetStringXmlAttribute(XmlNode node, string attributeName, string defaultValue = null)
        {
            if (node == null || node.Attributes == null || node.Attributes[attributeName] == null)
                return defaultValue;

            return node.Attributes[attributeName].Value;
        }

        /// <summary>
        /// Retrieves a true of false value from an XML attribute.
        /// Returns defaultValue if the attribute doesn't exist or the value is malformed.
        /// </summary>
        public static bool GetBoolXmlAttribute(XmlNode node, string attributeName, bool defaultValue = false)
        {
            if (node == null || node.Attributes == null || node.Attributes[attributeName] == null)
                return defaultValue;

            if (bool.TryParse(node.Attributes[attributeName].Value, out bool b))
                return b;

            return defaultValue;
        }

        /// <summary>
        /// Retrieves a float value from an XML attribute.
        /// Returns defaultValue if the attribute doesn't exist or the value is malformed.
        /// </summary>
        public static float GetFloatXmlAttribute(XmlNode node, string attributeName, float defaultValue)
        {
            if (node == null || node.Attributes == null || node.Attributes[attributeName] == null)
                return defaultValue;

            if (float.TryParse(node.Attributes[attributeName].Value, out float f))
                return f;

            return defaultValue;
        }

        /// <summary>
        /// Retrieves an int value from an XML attribute.
        /// Returns defaultValue if the attribute doesn't exist or the value is malformed.
        /// </summary>
        public static int GetIntXmlAttribute(XmlNode node, string attributeName, int defaultValue)
        {
            if (node == null || node.Attributes == null || node.Attributes[attributeName] == null)
                return defaultValue;

            if (int.TryParse(node.Attributes[attributeName].Value, out int i))
                return i;

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the string content of an XmlNode attribute.
        /// </summary>
        /// <exception cref="I18NException">Thrown if the attribute is missing</exception>
        public static string GetXmlAttribute(XmlNode node, string attributeName)
        {
            if (node == null)
                throw new I18NException(I18NExceptionType.XmlAttributeMissing, attributeName);

            if (node.Attributes == null || node.Attributes[attributeName] == null)
                throw new I18NException(I18NExceptionType.XmlAttributeMissing, attributeName, node.Name);

            return node.Attributes[attributeName].Value;
        }

        /// <summary>
        /// Retrieves the enum content of an XmlNode attribute or defaultValue if it is missing. 
        /// </summary>
        public static T GetEnumXmlAttribute<T>(XmlNode node, string attributeName, T defaultValue)
        {
            if (node == null || node.Attributes == null || node.Attributes[attributeName] == null)
                return defaultValue;

            if (!typeof(T).IsEnum)
                return defaultValue;

            try
            {
                return (T)Enum.Parse(typeof(T), node.Attributes[attributeName].Value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string PrettyFingerprint(string p)
        {
            List<string> pairs = new List<string>();
            for (int i = 0; i < p.Length; i += 2)
            {
                pairs.Add(p.Substring(i, 2));
            }
            return string.Join(":", pairs.ToArray());
        }

        public static string GetUrl(IXenConnection connection)
        {
            UriBuilder uriBuilder = new UriBuilder(connection.UriScheme, connection.Hostname);
            uriBuilder.Port = connection.Port;
            return uriBuilder.ToString();
        }

        public static string DefaultVMName(string p, IXenConnection connection)
        {
            for (int i = 1; true; i++)
            {
                bool willDo = true;
                string name = string.Format(Messages.NEWVM_DEFAULTNAME, p, i);
                string hiddenName = Helpers.GuiTempObjectPrefix + name;
                // Check to see if name is in use
                foreach (VM v in connection.Cache.VMs)
                {
                    if (v.name_label == name || v.name_label == hiddenName)
                    {
                        willDo = false;
                        break;
                    }
                }
                if (willDo) return name;
            }
        }

        public static bool CDsExist(IXenConnection connection)
        {
            if (connection == null)
                return false;

            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.content_type != SR.Content_Type_ISO)
                    continue;

                if (sr.VDIs.Count > 0)
                    return true;
            }

            return false;
        }

        public static bool CompareLists<T>(List<T> l1, List<T> l2)
        {
            if (l1 == l2)
                return true;
            else if (l1 == null || l2 == null)
                return false;

            bool same = l1.Count == l2.Count;
            foreach (T item in l1)
            {
                if (!l2.Contains(item))
                {
                    same = false;
                    break;
                }
            }

            return same;
        }

        public static bool ListsIntersect<T>(List<T> l1, List<T> l2)
        {
            foreach (T item in l1)
            {
                if (l2.Contains(item))
                    return true;
            }
            return false;
        }

        public static bool CustomWithNoDVD(VM template)
        {
            return template != null && !template.DefaultTemplate() && template.FindVMCDROM() == null;
        }

        public static string GetMacString(string mac)
        {
            return mac == "" ? Messages.MAC_AUTOGENERATE : mac;
        }

        public static PIF FindPIF(XenAPI.Network network, Host owner)
        {
            foreach (PIF pif in network.Connection.ResolveAll(network.PIFs))
            {
                if (owner == null ||
                     pif.host == owner.opaque_ref)
                    return pif;
            }
            return null;
        }

        public static string VlanString(PIF pif)
        {
            if (pif == null || pif.VLAN == -1)
                return Messages.SPACED_HYPHEN;

            return pif.VLAN.ToString();
        }

        /// <summary>
        /// Return a string version of a list, in the form "L1, L2, L3 and L4"
        /// </summary>
        public static string StringifyList<T>(List<T> list)
        {
            if (list == null)
                return String.Empty;

            StringBuilder ans = new StringBuilder();
            for (int i = 0; i < list.Count; ++i)
            {
                ans.Append(list[i].ToString());
                if (i < list.Count - 2)
                    ans.Append(Messages.STRINGIFY_LIST_INNERSEP);
                else if (i == list.Count - 2)
                    ans.Append(Messages.STRINGIFY_LIST_LASTSEP);
            }
            return ans.ToString();
        }

       /// <summary>
       /// Does the connection support Link aggregation (LACP) bonds (i.e. on the vSwitch backend)?
       /// </summary>
       public static bool SupportsLinkAggregationBond(IXenConnection connection)
       {
           Host master = GetMaster(connection);
           return master != null && master.vSwitchNetworkBackend();
       }

       /// <summary>
       /// Number of alloowed NICs per bond
       /// </summary>
       public static int BondSizeLimit(IXenConnection connection)
       {
           Host master = GetMaster(connection);
           // For hosts on the vSwitch backend, we allow 4 NICs per bond; otherwise, 2
           return master != null && master.vSwitchNetworkBackend() ? 4 : 2;
       }

       public static Host GetHostAncestor(IXenObject xenObject)
       {
           if (xenObject == null || xenObject.Connection == null)
               return null;

           var h = xenObject as Host;
           if (h != null)
               return h;

           var sr = xenObject as SR;
           if (sr != null)
               return sr.Home();

           var vm = xenObject as VM;
           if (vm != null)
               return vm.Home();

           return null;
       }

       public static bool SameServerVersion(Host host, string longProductVersion)
       {
           return host != null && host.LongProductVersion() == longProductVersion;
       }

       public static bool EnabledTargetExists(Host host, IXenConnection connection)
       {
           if (host != null)
               return host.enabled;

           return connection.Cache.Hosts.Any(h => h.enabled);
       }

       public static bool GpuCapability(IXenConnection connection)
       {
           if (FeatureForbidden(connection, Host.RestrictGpu))
               return false;
           var pool = GetPoolOfOne(connection);
           return pool != null && pool.HasGpu();
       }

        public static bool VGpuCapability(IXenConnection connection)
        {
            if (FeatureForbidden(connection, Host.RestrictVgpu))
                return false;
            var pool = GetPoolOfOne(connection);
            return pool != null && pool.HasVGpu();
        }

        /// <summary>
       /// Whether creation of VLAN 0 is allowed.
       /// </summary>
       public static bool VLAN0Allowed(IXenConnection connection)
       {
           Host master = GetMaster(connection);
           // For Creedence or later on the vSwitch backend, we allow creation of VLAN 0
           return master != null && master.vSwitchNetworkBackend();
       }

       public static bool ContainerCapability(IXenConnection connection)
       {
           var master = GetMaster(connection);
           if (master == null)
               return false;
           if (master.ProductBrand() == PRODUCT_BRAND_XCP_NG)
               return true;
           if (ElyOrGreater(connection))
               return master.AppliedUpdates().Any(update => update.Name().ToLower().StartsWith("xscontainer")); 
           return master.SuppPacks().Any(suppPack => suppPack.Name.ToLower().StartsWith("xscontainer")); 
       }

       public static bool PvsCacheCapability(IXenConnection connection)
       {
           var master = GetMaster(connection);
           return master != null && master.AppliedUpdates().Any(update => update.Name().ToLower().StartsWith("pvsaccelerator"));
       }

        public static string UrlEncode(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return System.Net.WebUtility.UrlEncode(str);
        }

        public static string UpdatesFriendlyName(string propertyName)
        {
            return FriendlyNameManager.FriendlyNames.GetString(string.Format("Label-{0}", propertyName)) ?? propertyName;
        }

        public static string UpdatesFriendlyNameAndVersion(Pool_update update)
        {
            var friendlyName = UpdatesFriendlyName(update.Name());
            if (string.IsNullOrEmpty(update.version))
                return friendlyName;
            return string.Format(Messages.SUPP_PACK_DESCRIPTION, friendlyName, update.version);
        }

        public static List<string> HostAppliedPatchesList(Host host)
        {
            List<string> result = new List<string>();

            if (ElyOrGreater(host))
            {
                foreach (var update in host.AppliedUpdates())
                    result.Add(UpdatesFriendlyNameAndVersion(update));
            }
            else
            {
                foreach (Pool_patch patch in host.AppliedPatches())
                    result.Add(patch.Name());
            }

            result.Sort(StringUtility.NaturalCompare);

            return result;
        }

        public static List<string> FindIpAddresses(Dictionary<string, string> networks, string device)
        {
            if (networks == null || string.IsNullOrWhiteSpace(device))
                return new List<string>();

            // PR-1373 - VM_guest_metrics.networks is a dictionary of IP addresses in the format:
            // [["0/ip", <IPv4 address>], 
            //  ["0/ipv4/0", <IPv4 address>], ["0/ipv4/1", <IPv4 address>],
            //  ["0/ipv6/0", <IPv6 address>], ["0/ipv6/1", <IPv6 address>]]

            return
                (from network in networks
                    where network.Key.StartsWith(string.Format("{0}/ip", device))
                    orderby network.Key
                    select network.Value.Split(new[] { "\n", "%n" }, StringSplitOptions.RemoveEmptyEntries)).SelectMany(x => x).Distinct().ToList();
        }
    }
}
