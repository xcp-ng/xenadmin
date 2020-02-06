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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Xml;

namespace XenAdmin.CustomFields
{
    internal class CustomFieldsCache
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<IXenConnection, List<CustomFieldDefinition>> _customFieldsPerConnection = new Dictionary<IXenConnection, List<CustomFieldDefinition>>();
        private readonly List<CustomFieldDefinition> _allCustomFields = new List<CustomFieldDefinition>();
        private readonly object _lock = new object();

        public void RecalculateCustomFields()
        {
            lock (_lock)
            {
                _customFieldsPerConnection.Clear();
                _allCustomFields.Clear();

                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    _customFieldsPerConnection[connection] = GetCustomFieldsFromGuiConfig(connection);
                    foreach (CustomFieldDefinition customField in _customFieldsPerConnection[connection])
                    {
                        if (!_allCustomFields.Contains(customField))
                        {
                            _allCustomFields.Add(customField);
                        }
                    }
                }
            }
        }

        private static List<CustomFieldDefinition> GetCustomFieldsFromGuiConfig(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
            {
                return new List<CustomFieldDefinition>();
            }

            Dictionary<String, String> other_config = Helpers.GetGuiConfig(pool);
            if (other_config == null)
            {
                return new List<CustomFieldDefinition>();
            }

            if (!other_config.ContainsKey(CustomFieldsManager.CUSTOM_FIELD_BASE_KEY))
            {
                return new List<CustomFieldDefinition>();
            }

            String customFields = other_config[CustomFieldsManager.CUSTOM_FIELD_BASE_KEY];
            if (customFields == null)
            {
                return new List<CustomFieldDefinition>();
            }

            customFields.Trim();
            if (String.IsNullOrEmpty(customFields))
            {
                return new List<CustomFieldDefinition>();
            }

            return GetCustomFieldDefinitions(customFields);
        }

        public List<CustomFieldDefinition> GetCustomFields()
        {
            lock (_lock)
            {
                return new List<CustomFieldDefinition>(_allCustomFields);
            }
        }

        public List<CustomFieldDefinition> GetCustomFields(IXenConnection connection)
        {
            // Note that we can't guarantee that customFieldsPerConnection[connection] exists.
            // It's possible for us to be calling GetCustomFields on a connection that is no longer in
            // Program.XenConnections, because we've looked it up from a stale XenObject.  This is
            // a transient condition, but real enough at the moment.
            // We return an empty CustomFieldDefinition[] in this case (callers are not expecting us to
            // return null).

            lock (_lock)
            {
                if (_customFieldsPerConnection.ContainsKey(connection))
                {
                    return new List<CustomFieldDefinition>(_customFieldsPerConnection[connection]);
                }
                else
                {
                    return new List<CustomFieldDefinition>();
                }
            }
        }

        private static List<CustomFieldDefinition> GetCustomFieldDefinitions(String xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode parentNode = doc.FirstChild;

                List<CustomFieldDefinition> customFieldDefinitions = new List<CustomFieldDefinition>();

                foreach (XmlNode node in parentNode.ChildNodes)
                {
                    try
                    {
                        customFieldDefinitions.Add(new CustomFieldDefinition(node));
                    }
                    catch (Exception e)
                    {
                        log.Debug($"Exception unmarshalling custom field definition '{node.OuterXml}'.", e);
                    }
                }

                return customFieldDefinitions;
            }
            catch (Exception e)
            {
                log.Debug(e, e);
                return new List<CustomFieldDefinition>();
            }
        }
    }
}