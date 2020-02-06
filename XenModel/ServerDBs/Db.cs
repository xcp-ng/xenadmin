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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using XenAPI;
using XenCenterLib;
using XenAdmin.Network;

namespace XenAdmin.ServerDBs
{
    /// <summary>
    /// A class which represents the XenServer status report Xml document. For use by <see cref="DbProxy"/>.
    /// </summary>
    public partial class Db
    {
        private readonly TableDictionary _tables;
        private static readonly Dictionary<Type, Relation[]> AllRelations = Relation.GetRelations();

        public Db(IXenConnection connection, string url)
        {
            _tables = new TableDictionary(this);

            using (StreamReader stream = url.StartsWith("http")
                ? new StreamReader(HTTPHelper.GET(new Uri(url), connection, true))
                : new StreamReader(url))
            {
                StatusReportXmlDocReader reader = new StatusReportXmlDocReader();
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = new BasicXMLResolver();
                doc.Load(stream);
                reader.PopulateDbFromXml(this, doc);
            }

            UpdateRelations();

            _tables.Changed += TablesChanged;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool disposed;
        private void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    _tables.Changed -= TablesChanged;
                    _tables.Clear();
                    disposed = true;  
                }
            }
        }

        private void TablesChanged(object sender, EventArgs e)
        {
            _tables.Changed -= TablesChanged;
            UpdateRelations();
            _tables.Changed += TablesChanged;
        }

        public TableDictionary Tables
        {
            get
            {
                return _tables;
            }
        }

        public object GetValue(string clazz, string opaque_ref, string field)
        {
            return Tables[clazz].Rows[opaque_ref].Props[field].XapiObjectValue;
        }

        public string ObjectWithFieldValue(string clazz, string field, string value)
        {
            foreach (KeyValuePair<string, Db.Row> row in Tables[clazz].Rows)
            {
                if (row.Value.Props[field].XapiObjectValue.ToString() == value)
                {
                    return row.Key;
                }
            }

            return Helper.NullOpaqueRef;
        }

        private void ClearRelations()
        {
            foreach (Table table in Tables.Values)
            {
                foreach (Db.Row row in table.Rows.Values)
                {
                    Relation[] relations;

                    if (AllRelations.TryGetValue(table.XapiType, out relations))
                    {
                        foreach (Relation relation in relations)
                        {
                            row.Props[relation.field] = new Prop(row, relation.field, new string[0]);
                        }
                    }
                }
            }
        }

        private void UpdateRelations()
        {
            // clear all existing relation
            ClearRelations();

            foreach (PropInfo propInfo in GetAllPropsWithOpaqueRefValue())
            {
                foreach (KeyValuePair<Type, Relation[]> relationsPair in AllRelations)
                {
                    foreach (Relation relation in relationsPair.Value)
                    {
                        if (relation.manyField.ToLower() == propInfo.Prop.Name.ToLower() && propInfo.Table.Name.ToLower() == relation.manyType.ToLower())
                        {
                            // string of the Proxy_ from the front.
                            string relTypeName = relationsPair.Key.Name.Substring(6);

                            if (Tables[relTypeName].Rows.ContainsKey(propInfo.PropValue))
                            {
                                List<string> list = new List<string>((string[])Tables[relTypeName].Rows[propInfo.PropValue].Props[relation.field].XapiObjectValue);
                                list.Add(propInfo.RowKey);
                                Tables[relTypeName].Rows[propInfo.PropValue].Props[relation.field].XapiObjectValue = list.ToArray();
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<PropInfo> GetAllPropsWithOpaqueRefValue()
        {
            foreach (Table table in Tables.Values)
            {
                foreach (KeyValuePair<string, Db.Row> row in table.Rows)
                {
                    foreach (Prop prop in row.Value.Props.Values)
                    {
                        string propOpaqueRef = prop.XapiObjectValue as string;

                        if (propOpaqueRef != null && propOpaqueRef.StartsWith("OpaqueRef:") && propOpaqueRef != "OpaqueRef:NULL")
                        {
                            yield return new PropInfo(table, row.Key, row.Value, prop, propOpaqueRef);
                        }
                    }
                }
            }
        }

        private class PropInfo
        {
            public readonly Table Table;
            public readonly string RowKey;
            public readonly Row Row;
            public readonly Prop Prop;
            public readonly string PropValue;

            public PropInfo(Table table, string rowKey, Row row, Prop prop, string propValue)
            {
                Table = table;
                RowKey = rowKey;
                Row = row;
                Prop = prop;
                PropValue = propValue;
            }
        }

        /// <summary>
        /// A class which represents a table in the XenServer status report Xml document. For use by <see cref="DbProxy"/>.
        /// </summary>
        public class Table
        {
            private readonly string _name;
            private readonly Type _xapiType;
            private readonly RowDictionary _rows;
            private readonly Db _db;

            public event EventHandler Changed;

            internal virtual void OnChanged(EventArgs e)
            {
                EventHandler handler = Changed;

                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Table"/> class.
            /// </summary>
            /// <param name="name">The name of the table.</param>
            /// <param name="db">The <see cref="Db"/> to which this <see cref="Table"/> is attached.</param>
            public Table(string name, Db db)
            {
                Util.ThrowIfStringParameterNullOrEmpty(name, "name");
                Util.ThrowIfParameterNull(db, "db");

                _rows = new RowDictionary(this);
                _name = name.ToLower();
                _db = db;
                _xapiType = TypeCache.GetProxyType(name);

                _rows.Changed += delegate { OnChanged(EventArgs.Empty); };
            }

            public RowDictionary Rows
            {
                get
                {
                    return _rows;
                }
            }

            /// <summary>
            /// Gets the name of this <see cref="Table"/>.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// Gets the XAPI type that this <see cref="Table"/> represents.
            /// </summary>
            /// <value>The XAPI type that this <see cref="Table"/> represents.</value>
            public Type XapiType
            {
                get { return _xapiType; }
            }
        }

        /// <summary>
        /// Represents a row entity in the parent <see cref="Table"/>.
        /// </summary>
        public class Row
        {
            private readonly PropDictionary _props;
            private readonly Table _table;

            /// <summary>
            /// Initializes a new instance of the <see cref="Row"/> class.
            /// </summary>
            /// <param name="table">The <see cref="Table"/> to which this <see cref="Row"/> belongs.</param>
            public Row(Table table)
            {
                Util.ThrowIfParameterNull(table, "table");

                _table = table;
                _props = new PropDictionary(this);
                _props.Changed += delegate { _table.OnChanged(EventArgs.Empty); };
            }

            public PropDictionary Props
            {
                get
                {
                    return _props;
                }
            }

            /// <summary>
            /// Gets the <see cref="Table"/> which this <see cref="Row"/> belongs to.
            /// </summary>
            /// <value>The table.</value>
            public Table Table
            {
                get { return _table; }
            }

            public Row CopyOf()
            {
                Row r = new Row(_table);
                foreach (KeyValuePair<string, Prop> p in _props)
                {
                    r.Props[p.Key] = p.Value.CopyOf(this);
                }
                return r;
            }

            public void PopulateFrom(Hashtable h)
            {
                foreach (string key in h.Keys)
                {
                    this.Props[key] = new Prop(this, key, h[key]);
                }
            }
        }

        /// <summary>
        /// A class representing a property of a <see cref="Row"/> within the XenServer status report XML document.
        /// </summary>
        public class Prop
        {
            private readonly Row _row;
            private Type _xapiType;
            private readonly string _name;
            private object _xapiValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Prop"/> class.
            /// </summary>
            /// <param name="row">The row to which this <see cref="Prop"/> belongs.</param>
            /// <param name="name">The name of this <see cref="Prop"/>.</param>
            /// <param name="stringValue">The string value of this <see cref="Prop"/>.</param>
            public Prop(Row row, string name, string stringValue)
            {
                Util.ThrowIfParameterNull(row, "row");
                Util.ThrowIfStringParameterNullOrEmpty(name, "name");
                Util.ThrowIfParameterNull(stringValue, "stringValue");

                _row = row;
                _name = name;
                _xapiType = TypeCache.GetFieldType(_row.Table.Name, _name);

                _xapiValue = Parser.Parse(XapiType, stringValue);

            }

            public Prop(Row row, string name, object xapiValue)
            {
                Util.ThrowIfParameterNull(row, "row");
                Util.ThrowIfStringParameterNullOrEmpty(name, "name");

                _row = row;
                _name = name;
                _xapiType = TypeCache.GetFieldType(_row.Table.Name, _name);
                _xapiValue = xapiValue ?? Parser.Parse(XapiType, "");

            }

            /// <summary>
            /// Gets the name of this <see cref="Prop"/>.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// Gets the XAPI type which this <see cref="Prop"/> represents.
            /// </summary>
            /// <value>The XAPI type which this <see cref="Prop"/> represents.</value>
            public Type XapiType
            {
                get
                {
                    return _xapiType;
                }
            }

            /// <summary>
            /// Gets or sets the XAPI value which of the XAPI type which this <see cref="Prop"/> represents.
            /// </summary>
            /// <value>The xapi object value.</value>
            public object XapiObjectValue
            {
                get
                {
                    return _xapiValue;
                }
                set
                {
                    Util.ThrowIfParameterNull(value, "value");

                    _xapiType = value.GetType();
                    _xapiValue = value;

                    _row.Table.OnChanged(EventArgs.Empty);
                }
            }

            public Prop CopyOf(Row r)
            {
                return new Prop(r, _name, _xapiValue);
            }
        }
    }
}
