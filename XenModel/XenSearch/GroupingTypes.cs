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
using XenAdmin.CustomFields;
using XenAdmin.Model;
using XenAPI;
using System.Xml;
using XenAdmin.Core;
using System.Diagnostics;
using XenCenterLib;

namespace XenAdmin.XenSearch
{
    public abstract class Grouping
    {
        public readonly Grouping subgrouping;

        protected Grouping(Grouping subgrouping)
        {
            this.subgrouping = subgrouping;
        }

        public abstract String GroupingName { get; }

        public virtual String GetGroupName(object group)
        {
            return group.ToString();
        }

        public virtual Icons GetGroupIcon(Object group)
        {
            return Icons.XenCenter;
        }

        public abstract Object GetGroup(IXenObject o);

        public virtual bool BelongsAsGroupNotMember(IXenObject o)
        {
            return false;
        }

        public virtual QueryFilter GetRelevantGroupQuery(Search search)
        {
            return null;
        }

        /// <param name="parent">May be null.</param>
        /// <param name="val">May not be null.</param>
        /// <returns>A QueryFilter that returns the objects with the given value (wrt the given parent, if given).
        /// In other words, the QueryFilter that we need when selecting val in the Folder View.  May return null if all objects
        /// are to be shown (such as when selecting the Types node).</returns>
        public virtual QueryFilter GetSubquery(object parent, object val)
        {
            return null;
        }

        /// <param name="val">May not be null.</param>
        /// <returns>The Grouping that we need when selecting val in the Folder View, or null if no grouping is required.</returns>
        public virtual Grouping GetSubgrouping(object val)
        {
            return null;
        }

        #region Marshalling

        protected virtual void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
        }

        public virtual XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(SearchMarshalling.GetClassName(this));

            AddXmlAttributes(doc, node);

            if (subgrouping != null)
                node.AppendChild(subgrouping.ToXmlNode(doc));

            return node;
        }

        #endregion

        #region Unmarshalling

        protected Grouping(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == CustomFieldDefinition.TAG_NAME)
                    continue;

                subgrouping = (Grouping)SearchMarshalling.FromXmlNode(childNode);
                if (subgrouping != null)
                    break;
            }
        }

        #endregion
    }

    public class FolderGrouping : Grouping
    {
        public FolderGrouping(Grouping subgrouping)
            : base(subgrouping)
        {
        }

        public FolderGrouping(XmlNode node)
            : base(node)
        {
        }

        public override string GroupingName
        {
            get { return Messages.FOLDER; }
        }

        public override object GetGroup(IXenObject o)
        {
            ComparableList<Folder> folders = Folders.GetAncestorFolders(o);
            if (folders.Count == 0)
                return null;
            else
            {
                List<Folder[]> ans = new List<Folder[]>(1);  // See comment in NodeGroup.AddGrouped()
                folders.Reverse();
                ans.Add(folders.ToArray());
                return ans;
            }
        }

        public override bool BelongsAsGroupNotMember(IXenObject o)
        {
            Folder folder = o as Folder;
            return (folder != null && folder.Parent != null && folder.Parent.IsRootFolder);  // i.e., top level folders
        }

        public override bool Equals(object obj)
        {
            FolderGrouping other = obj as FolderGrouping;
            return other != null; // All folder groupings are the same!
        }

        /// <summary>
        /// http://xkcd.com/221/
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 4; // chosen by fair dice roll.
                      // guaranteed to be random.
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The expected type of the property to group by</typeparam>
    public class PropertyGrouping<T> : Grouping
    {
        protected readonly Dictionary<T, String> i18ns;
        protected readonly String i18n;
        protected readonly ImageDelegate<T> images;
        protected readonly PropertyAccessor propertyAccessor;
        public readonly PropertyNames property;

        public PropertyGrouping(PropertyNames property, Grouping subgrouping)
            : base(subgrouping)
        {
            this.property = property;
            this.propertyAccessor = PropertyAccessors.Get(property);

            this.i18n = PropertyAccessors.PropertyNames_i18n[property];
            this.i18ns = Invert((Dictionary<String, T>)PropertyAccessors.Geti18nFor(property));
            this.images = (ImageDelegate<T>)PropertyAccessors.GetImagesFor(property);
        }

        public PropertyGrouping(XmlNode node)
            : base(node)
        {
            this.property = (PropertyNames)Enum.Parse(typeof(PropertyNames), Helpers.GetXmlAttribute(node, "property"));
            this.propertyAccessor = PropertyAccessors.Get(property);

            this.i18n = PropertyAccessors.PropertyNames_i18n[property];
            this.i18ns = Invert((Dictionary<String, T>)PropertyAccessors.Geti18nFor(property));
            this.images = (ImageDelegate<T>)PropertyAccessors.GetImagesFor(property);
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "property", property.ToString());
        }

        private static Dictionary<R, S> Invert<R, S>(Dictionary<S, R> dict)
        {
            if (dict == null)
                return null;

            Dictionary<R, S> result = new Dictionary<R, S>();

            foreach (KeyValuePair<S, R> kvp in dict)
            {
                result[kvp.Value] = kvp.Key;
            }

            return result;
        }

        public override Object GetGroup(IXenObject o)
        {
            return propertyAccessor(o);
        }

        public override QueryFilter GetRelevantGroupQuery(Search search)
        {
            return search.Query.GetSubQueryFor(property);
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            if (typeof(T).IsEnum)
                return new EnumPropertyQuery<T>(property, (T)val, true);
            if (typeof(T) == typeof(string) && property == PropertyNames.tags)
            {
                // !!! Tags come through here.  Soon, tags will be proper
                // objects, so this will be replaced.
                return new StringListContainsQuery(property, (string)val, true);
            }
            if (typeof(T) == typeof(string))
            {
                return new ValuePropertyQuery(property, (string)val, true);
            }
            return null;
        }

        public override String GroupingName
        {
            get
            {
                return i18n;
            }
        }

        public override String GetGroupName(Object group)
        {
            Debug.Assert(i18ns == null || i18ns.ContainsKey((T)group)); //if i18ns is specified, it should contain an item for the group

            if (!(group is T) || i18ns == null || !i18ns.ContainsKey((T)group))
                return base.GetGroupName(group);

            return i18ns[(T)group];
        }

        public override Icons GetGroupIcon(Object group)
        {
            if (images == null || !(group is T))
                return base.GetGroupIcon(group);

            return images((T)group);
        }

        public override bool Equals(object obj)
        {
            PropertyGrouping<T> other = obj as PropertyGrouping<T>;
            if (other == null)
                return false;

            return property == other.property;
        }

        public override int GetHashCode()
        {
            return (int)property;
        }
    }

    // Special-case Boolean groups in order to make the group name more readable: CA-165366
    public class BoolGrouping : PropertyGrouping<bool>
    {
        protected readonly string i18nfalse;

        public BoolGrouping(PropertyNames property, Grouping subgrouping)
            : base(property, subgrouping)
        {
            this.i18nfalse = PropertyAccessors.PropertyNames_i18n_false[property];
        }

        public BoolGrouping(XmlNode node)
            : base(node)
        {
            this.i18nfalse = PropertyAccessors.PropertyNames_i18n_false[property];
        }

        public string GroupingName2(object group)
        {
            return (!(group is bool) /* shouldn't happen */ || (bool)group) ? i18n : i18nfalse;
        }

        public override string GetGroupName(object group)
        {
            return string.Empty;
        }
    }

    public class XenModelObjectPropertyGrouping<T> 
        : PropertyGrouping<T> where T : XenObject<T>
    {
        public XenModelObjectPropertyGrouping(PropertyNames property, Grouping subgrouping)
            : base(property, subgrouping)
        {
        }

        public XenModelObjectPropertyGrouping(XmlNode node)
            : base(node)
        {
        }

        public override bool BelongsAsGroupNotMember(IXenObject o)
        {
            // If the object is not of the group type, it's not a group
            if (!(o is T))
                return false;

            // Special case: if the type is VM, and the object is not a real VM,
            // it's not a group. This is because snapshots are XMO<VM>s internally,
            // but we want to group them by the real VM they came from.
            VM vm = o as VM;
            if (vm != null && vm.not_a_real_vm())
                return false;

            // Otherwise this is a group
            return true;
        }
    }

    public class CustomFieldGrouping : Grouping
    {
        public readonly CustomFieldDefinition definition;

        public CustomFieldGrouping(CustomFieldDefinition definition, Grouping subgrouping)
            : base(subgrouping)
        {
            this.definition = definition;
        }

        public CustomFieldGrouping(XmlNode node)
            : base(node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == CustomFieldDefinition.TAG_NAME)
                {
                    definition = new CustomFieldDefinition(childNode);
                    return;
                }
            }

            throw new Exception(string.Format("Custom field declaration with no {0}", CustomFieldDefinition.TAG_NAME));
        }

        public override string GroupingName
        {
            get { return definition.Name; }
        }

        public override object GetGroup(IXenObject o)
        {
            return CustomFieldsManager.GetCustomFieldValue(o, definition);
        }

        public override XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = base.ToXmlNode(doc);

            node.AppendChild(definition.ToXmlNode(doc));

            return node;
        }

        public override Icons GetGroupIcon(Object group)
        {
            return Icons.CustomField;
        }

        public override bool Equals(object obj)
        {
            CustomFieldGrouping other = obj as CustomFieldGrouping;
            if (other == null)
                return false;

            return definition.Equals(other.definition);
        }

        public override int GetHashCode()
        {
            return definition.GetHashCode();
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            System.Diagnostics.Trace.Assert(val != null);

            CustomFieldDefinition defn =
                    CustomFieldsManager.GetCustomFieldDefinition(definition.Name);
            System.Diagnostics.Trace.Assert(defn != null);

            if (defn.Type == CustomFieldDefinition.Types.String)
                return new CustomFieldQuery(defn, (string)val, StringPropertyQuery.PropertyQueryType.exactmatch);
            else if (defn.Type == CustomFieldDefinition.Types.Date)
                return new CustomFieldDateQuery(defn, (DateTime)val, DatePropertyQuery.PropertyQueryType.exact);
            else return base.GetSubquery(parent, val);
        }

        public override Grouping GetSubgrouping(object val)
        {
            CustomFieldDefinition defn =
                CustomFieldsManager.GetCustomFieldDefinition(val.ToString());
            return defn == null ? null : new CustomFieldGrouping(defn, null);
        }
    }

    public class AllCustomFieldsGrouping : Grouping
    {
        public AllCustomFieldsGrouping(Grouping subgrouping)
            : base(subgrouping)
        {
        }

        public AllCustomFieldsGrouping(XmlNode node)
            : base(node)
        {
        }

        public override string GroupingName
        {
            get { return Messages.CUSTOM_FIELDS; }
        }

        public override object GetGroup(IXenObject o)
        {
            return CustomFieldsManager.CustomFieldArrays(o);
        }

        public override QueryFilter GetSubquery(object parent, object val)
        {
            System.Diagnostics.Trace.Assert(val != null);

            // Clicked on a key in the tree (which is now in val).
            if (parent == null)
            {
                CustomFieldDefinition defn =
                    CustomFieldsManager.GetCustomFieldDefinition(val.ToString());
                System.Diagnostics.Trace.Assert(defn != null);
                
                if (defn.Type == CustomFieldDefinition.Types.String)
                    return new CustomFieldQuery(defn, String.Empty, StringPropertyQuery.PropertyQueryType.exactmatch);
                else
                    return new CustomFieldDateQuery(defn, DateTime.Now, DatePropertyQuery.PropertyQueryType.exact);
            }

            // Clicked on a value in the tree (key is in parent, value is in val).
            else
            {
                CustomFieldDefinition defn =
                    CustomFieldsManager.GetCustomFieldDefinition(parent.ToString());
                System.Diagnostics.Trace.Assert(defn != null);

                if (defn.Type == CustomFieldDefinition.Types.String)
                    return new CustomFieldQuery(defn, val.ToString(), StringPropertyQuery.PropertyQueryType.exactmatch);
                else
                    return new CustomFieldDateQuery(defn, (DateTime)val, DatePropertyQuery.PropertyQueryType.exact);
            }
        }

        public override Grouping GetSubgrouping(object val)
        {
            CustomFieldDefinition defn =
                CustomFieldsManager.GetCustomFieldDefinition(val.ToString());
            return defn == null ? null : new CustomFieldGrouping(defn, null);
        }

        public override Icons GetGroupIcon(Object group)
        {
            return Icons.CustomField;
        }

        public override bool Equals(object obj)
        {
            return (obj is AllCustomFieldsGrouping);  // All AllCustomFieldsGrouping's are the same
        }

        public override int GetHashCode()
        {
            return 17;
        }
    }
}
