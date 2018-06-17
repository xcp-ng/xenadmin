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
using System.Globalization;
using System.Text;
using System.Xml;
using XenCenterLib;
using XenAdmin.CustomFields;
using XenAPI;

namespace XenAdmin.XenSearch
{
    public class Sort
    {
        private readonly string column;
        private readonly bool ascending;

        private readonly PropertyAccessor property;

        public string Column
        {
            get { return column; }
        }

        public bool Ascending
        {
            get { return ascending; }
        }

        public Sort(string column, bool ascending)
        {
            this.column = column;
            this.ascending = ascending;
            this.property = CalcProperty();
        }

        private PropertyAccessor CalcProperty()
        {
            if (column.StartsWith(CustomFieldsManager.CUSTOM_FIELD))
            {
                string fieldName = column.Substring(CustomFieldsManager.CUSTOM_FIELD.Length);
                CustomFieldDefinition customFieldDefinition = CustomFieldsManager.GetCustomFieldDefinition(fieldName);
                if (customFieldDefinition == null)  // a custom field that existed at the time the search was created but no longer exists
                    return (o => null);

                if (customFieldDefinition.Type == CustomFieldDefinition.Types.Date)
                {
                    return delegate(IXenObject o)
                    {
                        object val = CustomFieldsManager.GetCustomFieldValue(o, customFieldDefinition);
                        return (DateTime?)(val is DateTime ? val : null);
                    };
                }
                else
                {
                    return delegate(IXenObject o)
                    {
                        object val = CustomFieldsManager.GetCustomFieldValue(o, customFieldDefinition);
                        return val == null ? null : val.ToString();
                    };
                }
            }

            ColumnNames c;
            try
            {
                c = (ColumnNames)Enum.Parse(typeof(ColumnNames), column);
            }  
            catch (ArgumentException)
            {
                return null;
            } 
            PropertyNames propertyName = PropertyAccessors.GetSortPropertyName(c);
            return PropertyAccessors.Get(propertyName);
        }

        public Sort(XmlNode node)
            : this(node.Attributes["column"].Value, SearchMarshalling.ParseBool(node.Attributes["ascending"].Value))
        {
        }

        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("sort");

            SearchMarshalling.AddAttribute(doc, node, "column", column);
            SearchMarshalling.AddAttribute(doc, node, "ascending", ascending);

            return node;
        }

        public int Compare(object one, object other)
        {
            // Comparison for the name column. If we need any other special cases,
            // it would be better to subclass Sort, but that involved too much
            // disruption especially in serializing and deserializing for this one case.
            bool nameSort = column == "name";

            IXenObject i1 = one as IXenObject;
            IXenObject i2 = other as IXenObject;

            IComparable c1, c2;

            if (i1 != null)
                c1 = property(i1);
            else if (nameSort)
                c1 = one is DateTime ? ((DateTime)one).ToString("u", CultureInfo.InvariantCulture) : one.ToString(); // CP-2036, CA-67113
            else
                c1 = null;

            if (i2 != null)
                c2 = property(i2);
            else if (nameSort)
                c2 = other is DateTime ? ((DateTime)other).ToString("u", CultureInfo.InvariantCulture) : other.ToString();
            else
                c2 = null;

            if (c1 == null && c2 == null)
                return 0;
            if (c1 == null)
                return ascending ? -1 : 1;
            if (c2 == null)
                return ascending ? 1 : -1;

            int r = nameSort ? StringUtility.NaturalCompare(c1.ToString(), c2.ToString()) : c1.CompareTo(c2);
            if (!ascending)
                r = -r;
            return r;
        }

        public override bool Equals(object obj)
        {
            Sort other = obj as Sort;
            if (other == null)
                return false;

            return column == other.column
                && ascending == other.ascending;
        }

        public override int GetHashCode()
        {
            return column.GetHashCode();
        }
    }
}
