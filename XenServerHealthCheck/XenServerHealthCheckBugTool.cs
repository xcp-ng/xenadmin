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
using System.IO;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin;
using System.Linq;
using System.Globalization;
using System.Xml;

namespace XenServerHealthCheck
{
    public class XenServerHealthCheckBugTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> reportExcluded =
            new List<string>
            {
                "blobs",
                "vncterm",
                "xapi-debug"
            };

        private static readonly Dictionary<string, int> reportWithVerbosity =
            new Dictionary<string, int>
            {
                {"host-crashdump-logs", 2},
                {"system-logs", 2},
                {"tapdisk-logs", 2},
                {"xapi", 2},
                {"xcp-rrdd-plugins", 2},
                {"xenserver-install", 2},
                {"xenserver-logs", 2}
            };

        public readonly string outputFile;

        public XenServerHealthCheckBugTool()
        {
                string name = string.Format("{0}{1}.zip", Messages.BUGTOOL_FILE_PREFIX, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture));

                string folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (Directory.Exists(folder))
                    Directory.Delete(folder);
                Directory.CreateDirectory(folder);

                if (!name.EndsWith(".zip"))
                    name = string.Concat(name, ".zip");

                outputFile = string.Format(@"{0}\{1}", folder, name);
        }

        public void RunBugtool(IXenConnection connection, Session session)
        {
            if (connection == null || session == null)
                return;

            // Fetch the common capabilities of all hosts.
            Dictionary<Host, List<string>> hostCapabilities = new Dictionary<Host, List<string>>();
            foreach (Host host in connection.Cache.Hosts)
            {
                GetSystemStatusCapabilities action = new GetSystemStatusCapabilities(host);
                action.RunExternal(session);
                if (!action.Succeeded)
                    return;

                List<string> keys = new List<string>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(action.Result);
                foreach (XmlNode node in doc.GetElementsByTagName("capability"))
                {
                    foreach (XmlAttribute a in node.Attributes)
                    {
                        if (a.Name == "key")
                            keys.Add(a.Value);
                    }
                }
                hostCapabilities[host] = keys;
            }

            List<string> combination = null;
            foreach (List<string> capabilities in hostCapabilities.Values)
            {
                if (capabilities == null)
                    continue;

                if (combination == null)
                {
                    combination = capabilities;
                    continue;
                }

                combination = Helpers.ListsCommonItems<string>(combination, capabilities);
            }

            if (combination == null || combination.Count <= 0)
               return;

            // The list of the reports which are required in Health Check Report.
            List<string> reportIncluded = combination.Except(reportExcluded).ToList();

            // Verbosity works for xen-bugtool since Dundee.
            if (Helpers.DundeeOrGreater(connection))
            {
                List<string> verbReport = new List<string>(reportWithVerbosity.Keys);
                int idx = -1;
                for (int x = 0; x < verbReport.Count; x++)
                {
                    idx = reportIncluded.IndexOf(verbReport[x]);
                    if (idx >= 0)
                    {
                        reportIncluded[idx] = reportIncluded[idx] + ":" + reportWithVerbosity[verbReport[x]].ToString();
                    }
                }
            }

            // Ensure downloaded filenames are unique even for hosts with the same hostname: append a counter to the timestring
            string filepath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (Directory.Exists(filepath))
                Directory.Delete(filepath);
            Directory.CreateDirectory(filepath);

            string timestring = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            // Collect all master/slave information to output as a separate text file with the report
            List<string> mastersInfo = new List<string>();

            int i = 0;
            Pool p = Helpers.GetPool(connection);
            foreach (Host host in connection.Cache.Hosts)
            {
                // master/slave information
                if (p == null)
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a stand alone server",
                        host.Name()));
                }
                else
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a {1} of pool '{2}'",
                        host.Name(),
                        p.master.opaque_ref == host.opaque_ref ? "master" : "slave",
                        p.Name()));
                }

                var statAction = new SingleHostStatusAction(host, 0, reportIncluded, filepath, timestring + "-" + ++i);
                statAction.RunExternal(session);
            }

            // output the slave/master info
            string mastersDestination = string.Format("{0}\\{1}-Masters.txt", filepath, timestring);
            WriteExtraInfoToFile(mastersInfo, mastersDestination);
            
            // output the XenCenter metadata
            var metadata = XenAdminConfigManager.Provider.GetXenCenterMetadata(false);
            string metadataDestination = string.Format("{0}\\{1}-Metadata.json", filepath, timestring);
            WriteExtraInfoToFile(new List<string> {metadata}, metadataDestination);

            // Finish the collection of logs with bugtool.
            // Start to zip the files.
            ZipStatusReportAction zipAction = new ZipStatusReportAction(filepath, outputFile);
            zipAction.RunExternal(session);
            log.InfoFormat("Server Status Report is collected: {0}", outputFile);
        }

        private void WriteExtraInfoToFile(List<string> info, string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(fileName);
                foreach (string s in info)
                    sw.WriteLine(s);

                sw.Flush();
            }
            catch (Exception e)
            {
                log.Error($"Exception while writing {fileName} file:", e);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
    }
}
