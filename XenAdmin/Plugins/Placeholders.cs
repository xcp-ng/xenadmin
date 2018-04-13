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
using System.Text.RegularExpressions;
using XenCenterLib;
using XenAdmin.XenSearch;
using XenAPI;
using System.Net.Sockets;

namespace XenAdmin.Plugins
{
    /// <summary>
    /// A plugin can specify various URLs, file paths, or command arguments, any of which
    /// may contain placeholders such as {$session_id} or {$ip_address}.  These are
    /// replaced at time of use with the appropriate value.
    /// 
    /// Most values come from the parameters on the (first) selected object, but
    /// {$session_id} is handled specially, and is replaced with the OpaqueRef of a
    /// logged-in session.
    /// </summary>
    internal class Placeholders
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Regex PlaceholderRegex = new Regex(@"{\$([\w:]+)}");
        private const string PlaceholderFormat = @"{{${0}}}";

        private const string NULL_PLACEHOLDER_KEY = "null";
        private const string MULTI_TARGET_PLACEHOLDER_KEY = "multi_target";

        /// <summary>
        /// Gets the replacement for the specified placeholder for the specified object.
        /// </summary>
        /// <param name="placeholder">The placeholder to be replaced.</param>
        /// <param name="objs">The objects that the placeholder replacements are for.</param>
        /// <param name="match">A predicate indicating whether the specified placeholder should be replaced.</param>
        /// <returns>The replacement for the specified placeholder.</returns>
        private static string GetPlaceholderReplacement(string placeholder, IList<IXenObject> objs, Predicate<string> match)
        {
            if (match(placeholder))
            {
                if (objs == null || objs.Count == 0)
                    return NULL_PLACEHOLDER_KEY;

                if (objs.Count > 1)
                    return MULTI_TARGET_PLACEHOLDER_KEY;

                if (placeholder == "session_id")
                {
                    if (objs[0].Connection == null || objs[0].Connection.Session == null)
                    {
                        return NULL_PLACEHOLDER_KEY;
                    }
                    return objs[0].Connection.Session.opaque_ref;
                }
                else
                {
                    // otherwise update url with the latest info
                    PropertyNames property = (PropertyNames)Enum.Parse(typeof(PropertyNames), placeholder);

                    object val = PropertyAccessors.Get(property)(objs[0]);
                    return val != null ? val.ToString() : NULL_PLACEHOLDER_KEY;
                }
            }

            return null;
        }

        /// <summary>
        /// Replaces all placeholders (e.g. {$ip_address}, {$session_id} in the specified text for the specified object array.
        /// Note that if there is more than one object in the array current behaviour is to sub in MULTI_TARGET_PLACEHOLDER_KEY for all situations
        /// </summary>
        /// <param name="text">The text that contains the placeholders to be replaced.</param>
        /// <param name="objs">The objects that the placeholder replacements are for. Can be null.</param>
        /// <param name="match">A predicate indicating whether the specified placeholder should be replaced.</param>
        /// <returns>The processed text.</returns>
        private static string Substitute(string text, IList<IXenObject> objs, Predicate<string> match)
        {
            return PlaceholderRegex.Replace(text, m =>
            {
                try
                {
                    return GetPlaceholderReplacement(m.Groups[1].Value, objs, match) ?? m.Value;
                }
                catch (Exception e)
                {
                    // Leave the placeholder in; nothing we can do.
                    log.Warn(string.Format("Bad placeholder '{0}' in '{1}'", m.Value, text), e);
                    return m.Value;
                }
            });
        }

        /// <summary>
        /// Replaces all placeholders (e.g. {$ip_address}, {$session_id} in the specified text for the specified obj.
        /// </summary>
        /// <param name="text">The text that contains the placeholders to be replaced.</param>
        /// <param name="obj">The object that the placeholder replacements are for. Can be null.</param>
        /// <param name="match">A predicate indicating whether the specified placeholder should be replaced.</param>
        /// <returns>The processed text.</returns>
        private static string Substitute(string text, IXenObject obj, Predicate<string> match)
        {
            return Substitute(
                text, 
                obj == null ? null : new List<IXenObject>(new IXenObject[] { obj }), 
                match);
        }

        /// <summary>
        /// Replaces all placeholders (e.g. {$ip_address}, {$session_id} in the specified text for the specified object array.
        /// Note that if there is more than one object in the array current behaviour is to sub in MULTI_TARGET_PLACEHOLDER_KEY for all situations
        /// </summary>
        /// <param name="text">The text that contains the placeholders to be replaced.</param>
        /// <param name="objs">The objects that the placeholder replacements are for. Can be null.</param>
        /// <returns>The processed text.</returns>
        public static string Substitute(string text, IList<IXenObject> objs)
        {
            Util.ThrowIfParameterNull(text, "text");

            return Substitute(text, objs, s => true);
        }

        /// <summary>
        /// Replaces all placeholders (e.g. {$ip_address}, {$session_id} in the specified text for the specified obj.
        /// </summary>
        /// <param name="text">The text that contains the placeholders to be replaced.</param>
        /// <param name="obj">The object that the placeholder replacements are for. Can be null.</param>
        /// <returns>The processed text.</returns>
        public static string Substitute(string text, IXenObject obj)
        {
            return Substitute(
                text, 
                obj == null ? null : new List<IXenObject>(new IXenObject[] { obj }), 
                s => true);
        }

        /// <summary>
        /// Replaces all placeholders (e.g. {$ip_address}, {$session_id} in the specified text for the specified obj.
        /// Since ip_address can take several values over different Networks, this method returns a list of Uri for
        /// each of the different IP addresses.
        /// </summary>
        /// <param name="uri">The text that contains the placeholders to be replaced.</param>
        /// <param name="obj">The object that the placeholder replacements are for.</param>
        /// <returns>A List of Uris.</returns>
        public static List<Uri> SubstituteUri(string uri, IXenObject obj)
        {
            Util.ThrowIfParameterNull(uri, "uri");
            string ipAddressName = Enum.GetName(typeof(PropertyNames), PropertyNames.ip_address);

            try
            {
                if (!uri.Contains(string.Format(PlaceholderFormat, ipAddressName)))
                {
                    return new List<Uri> { new Uri(Substitute(uri, obj)) };
                }
                
                string u = Substitute(uri, obj, s => s != ipAddressName);
                var ips = (List<ComparableAddress>)PropertyAccessors.Get(PropertyNames.ip_address)(obj);

                if (ips == null || ips.Count == 0)
                {
                    return new List<Uri> { new Uri(u) };
                }

                return ips.ConvertAll(ip =>
                {
                    var ipstring = ip.AddressIP != null && ip.AddressIP.AddressFamily == AddressFamily.InterNetworkV6
                        ? string.Format("[{0}]", ip)
                        : ip.ToString();

                    return new Uri(u.Replace(string.Format(PlaceholderFormat, ipAddressName), ipstring));
                });
            }
            catch (UriFormatException)
            {
                log.Warn(string.Format("Failed to parse url {0}", uri));
                return new List<Uri> { new Uri("about:blank") };
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified object can supply values for all placeholders
        /// in the specified uri.
        /// </summary>
        /// <param name="uri">The uri string with placeholders to be read.</param>
        /// <param name="obj">The IXenObject to be used for the placeholder substitution.</param>
        /// <returns>A value indicating whether all place-holders could be replaced for this uri.</returns>
        public static bool UriValid(string uri, IXenObject obj)
        {
            return SubstituteUri(uri, obj).TrueForAll(u => u.AbsoluteUri != "about:blank");
        }
    }
}
