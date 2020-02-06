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
using System.Text;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;


namespace XenAdmin.Alerts
{
    public class PerfmonDefinition
    {
        private const string xmlns = ""; // no namespace for perfmon

        public const string PERFMON_KEY_NAME = "perfmon";
        private const string ROOT_ELEMENT_NAME = "config";
        private const string VARIABLE_ELEMENT_NAME = "variable";

        private const string ALARM_TRIGGER_LEVEL_ELEMENT_NAME = "alarm_trigger_level";
        private const string ALARM_TRIGGER_PERIOD_ELEMENT_NAME = "alarm_trigger_period";
        private const string ALARM_AUTO_INHIBIT_PERIOD_ELEMENT_NAME = "alarm_auto_inhibit_period";
        private const string ALARM_TYPE_ELEMENT_NAME = "name";
        private const string ALARM_COMMON_ATTR_NAME = "value";

        public const string ALARM_TYPE_CPU = "cpu_usage";
        public const string ALARM_TYPE_NETWORK = "network_usage";
        public const string ALARM_TYPE_DISK = "disk_usage";
        public const string ALARM_TYPE_FILESYSTEM = "fs_usage";
        public const string ALARM_TYPE_MEMORY_FREE = "memory_free_kib";
        public const string ALARM_TYPE_MEMORY_DOM0_USAGE = "mem_usage";
        public const string ALARM_TYPE_LOG_FILESYSTEM = "log_fs_usage";
        public const string ALARM_TYPE_SR_PHYSICAL_UTILISATION = "physical_utilisation";

        /// <summary>
        /// This is the name that will be stored in the SR's other-config:perfmon key
        /// </summary>
        public const string ALARM_TYPE_SR = "sr_io_throughput_total_per_host";
        /// <summary>
        /// This matches the name of the host alert
        /// </summary>
        public static readonly Regex SrRegex = new Regex("^sr_io_throughput_total_([a-f0-9]{8})$");

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string name;
        private readonly decimal alarmTriggerLevel;
        private readonly decimal alarmTriggerPeriod;
        private readonly decimal alarmAutoInhibitPeriod;

        public bool HasValueSet { private set; get; }

        public PerfmonDefinition(string name, decimal alarmTriggerLevel, decimal alarmTriggerPeriod, decimal alarmAutoInhibitPeriod)
        {
            this.name = name;
            this.alarmTriggerLevel = alarmTriggerLevel;
            this.alarmTriggerPeriod = alarmTriggerPeriod;
            this.alarmAutoInhibitPeriod = alarmAutoInhibitPeriod;
            HasValueSet = true;
        }

        public bool IsCPUUsage
        {
            get { return name.Equals(ALARM_TYPE_CPU); }
        }

        public bool IsNetworkUsage
        {
            get { return name.Equals(ALARM_TYPE_NETWORK); }
        }

        public bool IsDiskUsage
        {
            get { return name.Equals(ALARM_TYPE_DISK); }
        }

        public bool IsMemoryUsage
        {
            get { return name.Equals(ALARM_TYPE_MEMORY_FREE); }
        }

        public bool IsDom0MemoryUsage
        {
            get { return name.Equals(ALARM_TYPE_MEMORY_DOM0_USAGE); }
        }

        public bool IsSrUsage
        {
            get { return name.Equals(ALARM_TYPE_SR); }
        }

        public bool IsSrPhysicalUtilisation
        {
            get { return name.Equals(ALARM_TYPE_SR_PHYSICAL_UTILISATION); }
        }

        public decimal AlarmTriggerLevel
        {
            get { return alarmTriggerLevel; }
        }

        public decimal AlarmTriggerPeriod
        {
            get { return alarmTriggerPeriod; }
        }

        public decimal AlarmAutoInhibitPeriod
        {
            get { return alarmAutoInhibitPeriod; }
        }

        public override string ToString()
        {
            return String.Format("{0} (Value: {1} Trigger Period: {2} Auto Inhibit Period: {3})",
                                 name, alarmTriggerLevel, alarmTriggerPeriod, alarmAutoInhibitPeriod);
        }

        public override bool Equals(object obj)
        {
            PerfmonDefinition other = obj as PerfmonDefinition;
            if (other == null)
                return false;

            return other.name == name &&
                   other.alarmTriggerLevel == alarmTriggerLevel &&
                   other.alarmTriggerPeriod == alarmTriggerPeriod
                   && other.alarmAutoInhibitPeriod == alarmAutoInhibitPeriod;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        private PerfmonDefinition(XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                bool success = false;

                if (node.Attributes == null)
                {
                    //ignore
                }
                else if (node.Name.Equals(ALARM_TYPE_ELEMENT_NAME))
                {
                    name = node.Attributes[ALARM_COMMON_ATTR_NAME].Value;
                    success = true;
                }
                else if (node.Name.Equals(ALARM_TRIGGER_LEVEL_ELEMENT_NAME))
                {
                    success = Decimal.TryParse(
                        node.Attributes[ALARM_COMMON_ATTR_NAME].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out alarmTriggerLevel);
                    HasValueSet = HasValueSet || success;
                }
                else if (node.Name.Equals(ALARM_TRIGGER_PERIOD_ELEMENT_NAME))
                {
                    success = Decimal.TryParse(
                        node.Attributes[ALARM_COMMON_ATTR_NAME].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out alarmTriggerPeriod);
                    HasValueSet = HasValueSet || success;
                }
                else if (node.Name.Equals(ALARM_AUTO_INHIBIT_PERIOD_ELEMENT_NAME))
                {
                    success = Decimal.TryParse(
                        node.Attributes[ALARM_COMMON_ATTR_NAME].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out alarmAutoInhibitPeriod);
                    HasValueSet = HasValueSet || success;
                }

                if (!success)
                    log.DebugFormat("Failed to unmarshal perfmon definition '{0}'", node.OuterXml);
            }
        }

        /// <summary>
        /// Creates a "variable" element using the PerfmonDefinition.
        /// </summary>
        private XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode(XmlNodeType.Element, VARIABLE_ELEMENT_NAME, xmlns);

            XmlNode nameNode = doc.CreateNode(XmlNodeType.Element, ALARM_TYPE_ELEMENT_NAME, xmlns);
            XmlAttribute nameAttr = doc.CreateAttribute(ALARM_COMMON_ATTR_NAME);
            nameAttr.Value = name;
            nameNode.Attributes.Append(nameAttr);
            node.AppendChild(nameNode);

            XmlNode alarmTriggerLevelNode = doc.CreateNode(XmlNodeType.Element, ALARM_TRIGGER_LEVEL_ELEMENT_NAME, xmlns);
            XmlAttribute alarmTriggerLevelAttr = doc.CreateAttribute(ALARM_COMMON_ATTR_NAME);
            alarmTriggerLevelAttr.Value = alarmTriggerLevel.ToString(CultureInfo.InvariantCulture);
            alarmTriggerLevelNode.Attributes.Append(alarmTriggerLevelAttr);
            node.AppendChild(alarmTriggerLevelNode);

            XmlNode alarmTriggerPeriodNode = doc.CreateNode(XmlNodeType.Element, ALARM_TRIGGER_PERIOD_ELEMENT_NAME, xmlns);
            XmlAttribute alarmTriggerPeriodAttr = doc.CreateAttribute(ALARM_COMMON_ATTR_NAME);
            alarmTriggerPeriodAttr.Value = alarmTriggerPeriod.ToString(CultureInfo.InvariantCulture);
            alarmTriggerPeriodNode.Attributes.Append(alarmTriggerPeriodAttr);
            node.AppendChild(alarmTriggerPeriodNode);

            XmlNode alarmAutoInhibitPeriodNode = doc.CreateNode(XmlNodeType.Element, ALARM_AUTO_INHIBIT_PERIOD_ELEMENT_NAME, xmlns);
            XmlAttribute alarmAutoInhibitPeriodAttr = doc.CreateAttribute(ALARM_COMMON_ATTR_NAME);
            alarmAutoInhibitPeriodAttr.Value = alarmAutoInhibitPeriod.ToString(CultureInfo.InvariantCulture);
            alarmAutoInhibitPeriodNode.Attributes.Append(alarmAutoInhibitPeriodAttr);
            node.AppendChild(alarmAutoInhibitPeriodNode);

            return node;
        }

        /// <summary>
        /// Parse the entire perfmon xml config blob and build a collection of
        /// perfmon definitions.  Note the rootnode is a "config" element and the
        /// child elements are "variable".
        /// </summary>
        private static PerfmonDefinition[] GetPerfmonDefinitions(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode parentNode = doc.FirstChild;

                List<PerfmonDefinition> perfmonDefinitions = new List<PerfmonDefinition>();

                foreach (XmlNode node in parentNode.ChildNodes)
                {
                    try
                    {
                        var def = new PerfmonDefinition(node);
                        if (def.HasValueSet)
                            perfmonDefinitions.Add(def);
                    }
                    catch (Exception e)
                    {
                        log.Debug($"Exception unmarshalling perfmon definition '{node.OuterXml}'", e);
                    }
                }

                return perfmonDefinitions.ToArray();
            }
            catch (Exception e)
            {
                log.Debug(e, e);
                return new PerfmonDefinition[0];
            }
        }

        public static PerfmonDefinition[] GetPerfmonDefinitions(IXenObject xo)
        {
            if (!(xo is VM) && !(xo is Host) && !(xo is SR))
                return new PerfmonDefinition[0];

            Dictionary<string, string> other_config = Helpers.GetOtherConfig(xo);
            if (other_config == null)
                return new PerfmonDefinition[0];

            if (!other_config.ContainsKey(PERFMON_KEY_NAME))
                return new PerfmonDefinition[0];

            string perfmonConfigXML = other_config[PERFMON_KEY_NAME];
            if (perfmonConfigXML == null)
                return new PerfmonDefinition[0];

            perfmonConfigXML.Trim();
            if (String.IsNullOrEmpty(perfmonConfigXML))
                return new PerfmonDefinition[0];

            return GetPerfmonDefinitions(perfmonConfigXML);
        }

        /// <summary>
        /// Build an entire "perfmon" xml config blob using the collection of
        /// perfmon definitions.  Note the rootnode is a "config" element and the
        /// child elements are "variable".  This will be used to set the other_config
        /// of the host or vm objects.
        /// </summary>
        public static string GetPerfmonDefinitionXML(List<PerfmonDefinition> perfmonDefinitions)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode parentNode = doc.CreateNode(XmlNodeType.Element, ROOT_ELEMENT_NAME, xmlns);
            doc.AppendChild(parentNode);

            foreach (PerfmonDefinition perfmonDefinition in perfmonDefinitions)
                parentNode.AppendChild(perfmonDefinition.ToXmlNode(doc));

            return doc.OuterXml;
        }
    }
}
