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
using System.Reflection;
using System.Text;
using System.Xml;

using XenCenterLib;
using XenAdmin.Core;

namespace XenAdmin.XenSearch
{
    public class SearchMarshalling
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int CURRENT_SEARCH_MAJOR_VERSION = 2;
        private const int CURRENT_SEARCH_MINOR_VERSION = 0;

        private const String xmlns = "http://www.citrix.com/XenCenter/XenSearch/schema";

        private static Dictionary<String, Type> subclasses = new Dictionary<String, Type>();

        static SearchMarshalling()
        {
            subclasses["Query"] = typeof(Query);

            foreach (Type t in Assembly.GetAssembly(typeof(QueryFilter)).GetTypes())
            {
                if (t.IsSubclassOf(typeof(QueryFilter)) && !t.IsAbstract)
                    subclasses[GetClassName(t)] = t;
            }

            foreach (Type t in Assembly.GetAssembly(typeof(Grouping)).GetTypes())
            {
                if (t.IsSubclassOf(typeof(Grouping)) && !t.IsAbstract)
                    subclasses[GetClassName(t)] = t;
            }
        }

        internal static String GetClassName(object o)
        {
            return GetClassName(o.GetType());
        }

        private static String GetClassName(Type type)
        {
            return type.Name.Split(new char[] { '`' })[0];
        }

        private static Type GetType(XmlNode node)
        {
            Type type;

            if (subclasses.ContainsKey(node.Name))
                type = subclasses[node.Name];
            else
                type = Type.GetType(node.Name, true);

            if(!type.IsGenericType)
                return type;

            Type genericType = PropertyAccessors.GetType(GetPropertyNames(node));

            return type.MakeGenericType(genericType);
        }

        private static PropertyNames GetPropertyNames(XmlNode node)
        {
            return (PropertyNames)Enum.Parse(typeof(PropertyNames), node.Attributes["property"].Value);
        }

        public static Object FromXmlNode(XmlNode node)
        {
            try
            {
                Type type = GetType(node);

                if (type == null)
                    return null;

                ConstructorInfo ci = type.GetConstructor(new Type[] { typeof(XmlNode) });

                return ci.Invoke(new Object[] { node });
            }
            catch (TargetInvocationException e)
            {
                log.ErrorFormat("Error unmarshalling object. Xml was: {0}", node.OuterXml);
                log.Error(e, e);
                throw e.InnerException;
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error unmarshalling object. Xml was: {0}", node.OuterXml);
                log.Error(e, e);
                throw;
            }
        }

        internal static List<Search> LoadSearches(String xml)
        {
            try
            {
                List<Search> result = new List<Search>();

                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = new BasicXMLResolver();
                doc.LoadXml(xml);

                foreach (XmlNode search in doc.GetElementsByTagName("Search"))
                {
                    result.Add(LoadSearch(search));
                }

                // Just for backwards compat.
                foreach (XmlNode search in doc.GetElementsByTagName("search"))
                {
                    result.Add(LoadSearch(search));
                }

                return result;
            }
            catch (Exception e)
            {
                log.DebugFormat("Exception parsing xml '{0}'", xml.Substring(0, 10000));
                log.Debug(e, e);

                return null;
            }
        }

        public static Search LoadSearch(String xml)
        {
            // The string "null" is the default in Orlando (inadvertently, I think).
            if (xml == null || xml == "null")
                return null;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = new BasicXMLResolver();
                doc.LoadXml(xml);

                return LoadSearch(doc.FirstChild);
            }
            catch (Exception e)
            {
                log.DebugFormat("Exception parsing xml '{0}'", xml);
                log.Debug(e, e);

                return null;
            }
        }

        public class SearchVersionException : Exception
        {
            int majorVersion, currentMajor;

            public SearchVersionException(int majorVersion, int currentMajor)
            {
                this.majorVersion = majorVersion;
                this.currentMajor = currentMajor;
            }

            public override string Message
            {
                get
                {
                    return String.Format("SearchVersionException: a search version {0} was loaded.  Max allowed is {1}",
                        majorVersion, currentMajor);
                }
            }
        }

        public static Search LoadSearch(XmlNode node)
        {
            String uuid = Helpers.GetXmlAttribute(node, "uuid");
            String name = Helpers.GetXmlAttribute(node, "name");
            int major_version = int.Parse(Helpers.GetXmlAttribute(node, "major_version"));
            //int minor_version = int.Parse(Helpers.GetXmlAttribute(node, "minor_version"));
            
            if (major_version > CURRENT_SEARCH_MAJOR_VERSION)
                throw new SearchVersionException(major_version, CURRENT_SEARCH_MAJOR_VERSION);

            Query query;
            Object q = FromXmlNode(node.ChildNodes[0]);
            switch (major_version)
            {
                case 0:
                case 1:
                    QueryFilter filter = q as QueryFilter;
                    if (filter == null)
                        throw new I18NException(I18NExceptionType.XmlInvalid, node.OuterXml);
                    query = new Query(null, filter);
                    break;
                default:
                    query = q as Query;
                    if (query == null || query.QueryScope == null)  // query.QueryFilter == null is allowed
                        throw new I18NException(I18NExceptionType.XmlInvalid, node.OuterXml);
                    break;
            }


            Grouping grouping = null;
            Sort[] sorting = new Sort[] { };
            List<KeyValuePair<String, int>> columns = null;
            int i = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (i != 0)
                {
                    if (child.Name == "columns")
                    {
                        columns = LoadColumns(child);
                    }
                    else if (child.Name == "sorting")
                    {
                        sorting = LoadSorting(child);
                    }
                    else
                    {
                        grouping = (Grouping)FromXmlNode(child);
                    }
                }

                i++;
            }

            return new Search(query, grouping, name, uuid, columns, sorting);
        }

        private static List<KeyValuePair<string, int>> LoadColumns(XmlNode columnsNode)
        {
            List<KeyValuePair<string, int>> columns = new List<KeyValuePair<string, int>>();

            foreach (XmlNode node in columnsNode.ChildNodes)
            {
                String name = node.Attributes["name"].Value;
                if (name == "state")  // the "state" column no longer exists (CA-25116)
                    continue;

                int width;
                if (!Int32.TryParse(node.Attributes["width"].Value, out width))
                    continue;

                columns.Add(new KeyValuePair<String, int>(name, Math.Abs(width)));
            }

            return columns;
        }

        private static Sort[] LoadSorting(XmlNode sortingNode)
        {
            Sort[] sorting = new Sort[sortingNode.ChildNodes.Count];

            int i = 0;
            foreach (XmlNode childNode in sortingNode.ChildNodes)
                sorting[i++] = new Sort(childNode);

            return sorting;
        }

        internal static string SearchesToXML(List<Search> searches)
        {
            XmlDocument document = new XmlDocument();
            document.XmlResolver = new BasicXMLResolver();

            document.AppendChild(document.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null));
            document.AppendChild(document.CreateDocumentType(Search.BrandedSearchKey, "-//" + Search.BrandedSearchKey.ToUpper() + "//DTD " + Search.BrandedSearchKey.ToUpper() + " 1//EN", Search.BrandedSearchKey + "-1.dtd", null));

            XmlNode node = document.CreateElement("Searches");
            AddAttribute(document, node, "xmlns", xmlns);
            document.AppendChild(node);

            foreach (Search s in searches)
            {
                node.AppendChild(SearchToXMLNode(document, s));
            }

            StringWriter sw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(sw);
            try
            {
                w.Formatting = Formatting.Indented;
                w.Indentation = 4;
                w.IndentChar = ' ';

                document.WriteTo(w);
                w.Flush();

                return sw.ToString();
            }
            finally
            {
                w.Close();
                sw.Close();
            }
        }

        internal static string SearchToXML(Search search)
        {
            XmlDocument document = new XmlDocument();
            XmlNode node = SearchToXMLNode(document, search);
            return node.OuterXml;
        }

        private static XmlNode SearchToXMLNode(XmlDocument document, Search search)
        {
            XmlNode node = document.CreateElement("Search");
            AddAttribute(document, node, "uuid", search.UUID);
            AddAttribute(document, node, "name", search.Name);
            AddAttribute(document, node, "major_version", CURRENT_SEARCH_MAJOR_VERSION);
            AddAttribute(document, node, "minor_version", CURRENT_SEARCH_MINOR_VERSION);

            node.AppendChild(search.Query.ToXmlNode(document));
            if (search.Grouping != null)
                node.AppendChild(search.Grouping.ToXmlNode(document));

            XmlNode columnsNode = GetColumnXML(document, search);
            if (columnsNode != null)
                node.AppendChild(columnsNode);

            XmlNode sortingNode = GetSortingXML(document, search);
            if (sortingNode != null)
                node.AppendChild(sortingNode);

            return node;
        }

        private static XmlNode GetColumnXML(XmlDocument doc, Search search)
        {
            if (search.Columns == null)
                return null;

            XmlNode columnsNode = doc.CreateElement("columns");

            foreach (KeyValuePair<String, int> kvp in search.Columns)
            {
                XmlNode columnNode = doc.CreateElement("column");

                AddAttribute(doc, columnNode, "name", kvp.Key);
                AddAttribute(doc, columnNode, "width", kvp.Value.ToString());

                columnsNode.AppendChild(columnNode);
            }

            return columnsNode;
        }

        private static XmlNode GetSortingXML(XmlDocument doc, Search search)
        {
            XmlNode columnsNode = doc.CreateElement("sorting");

            foreach (Sort sort in search.Sorting)
                columnsNode.AppendChild(sort.ToXmlNode(doc));

            return columnsNode;
        }

        internal static void AddAttribute(XmlDocument document, XmlNode node, string n, string v)
        {
            XmlAttribute attr = document.CreateAttribute(n);
            attr.Value = v;
            node.Attributes.Append(attr);
        }

        internal static void AddAttribute(XmlDocument document, XmlNode node, string n, int v)
        {
            AddAttribute(document, node, n, v.ToString());
        }

        internal static void AddAttribute(XmlDocument document, XmlNode node, string n, bool v)
        {
            AddAttribute(document, node, n, v ? "yes" : "no");
        }

        internal static bool ParseBool(string v)
        {
            // "True" is just for backwards compat.
            return v == "yes" || v == "True" ? true : false;
        }
    }
}
