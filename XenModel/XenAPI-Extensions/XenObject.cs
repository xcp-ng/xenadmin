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
using System.Reflection;
using Newtonsoft.Json;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin;
using XenCenterLib;

namespace XenAPI
{
    public abstract partial class XenObject<S> : IComparable<S> where S : XenObject<S>
    {
        protected const String HIDE_FROM_XENCENTER = "HideFromXenCenter";

        private IXenConnection m_Connection = null;

        private bool _locked;

        [JsonIgnore]
        public IXenConnection Connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }


        public virtual string Description()
        {
            return string.Empty;
        }

        /// <summary>
        /// True if the Xen server knows about this object; false otherwise, e.g., when the user has created an object
        /// but not yet saved it to the server.
        /// </summary>
        public bool ExistsOnServer()
        {
            return this.opaque_ref != null;
        }

        /// <summary>
        /// True if a server request is in progress.
        /// </summary>
        [JsonIgnore]
        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    this.NotifyPropertyChanged("Locked");
                }
            }
        }

        protected static T Get<T>(Dictionary<string, T> d, string k) where T : class
        {
            return d != null && d.ContainsKey(k) ? d[k] : null;
        }

        private String path = String.Empty;

        [JsonIgnore]
        public String Path
        {
            get { return path; }
            set
            {
                if (!Helper.AreEqual(value, path))
                {
                    path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }

        public IXenObject Clone()
        {
            IXenObject result = (IXenObject)MemberwiseClone();
            result.ClearEventListeners();
            result.Locked = false;
            return result;
        }

        public virtual bool Show(bool showHiddenVMs)
        {
             return true; 
        }

        public virtual bool IsHidden()
        {
            return false;
        }

        public string SaveChanges(Session session)
        {
            return SaveChanges(session, null);
        }

        public string SaveChanges(Session session, IXenObject beforeObject)
        {
            S server =
                opaque_ref == null ?
                    null :
                    session.Connection.Resolve(new XenRef<S>(opaque_ref));

            if (server == null && opaque_ref != null)
                return null;

            if (opaque_ref != null && !server.Locked)
                throw new InvalidOperationException("Instance must be locked before calling SaveChanges()");

            if (beforeObject == null)
                beforeObject = server;
            return SaveChanges(session, opaque_ref, (S)beforeObject);
        }

        public virtual int CompareTo(S other)
        {
            if (other == null)
                return -1;

            int result = StringUtility.NaturalCompare(Name(), other.Name());
            if (result != 0)
                return result;

            return this.opaque_ref.CompareTo(other.opaque_ref);
        }

        public int CompareTo(object obj)
        {
            S other = obj as S;
            if (other != null)
                return CompareTo(other);

            IXenObject o = obj as IXenObject;
            if (o == null)
                return -1;

            return StringUtility.NaturalCompare(Name(), o.Name());
        }

        /// <summary>
        /// This method can be overridden by the derived classes. This is why the implementation is here and the typed 
        /// equals calls this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            IXenObject otherIXenObject = other as IXenObject;
            if (otherIXenObject != null)
                return opaque_ref == otherIXenObject.opaque_ref;
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance. This is required if you want to use this type as a key in a hashtable.
        /// </summary>
        public override int GetHashCode()
        {
            return opaque_ref.GetHashCode();
        }

        #region IEquatable<IXenObject> Members

        /// <summary>
        /// This is the implementation of IEquatable of IXenObject which is defined in IXenObject. This calls the virtual
        /// Equals so that any derived classes can override Equals and it will used the derived class implementation in 
        /// all circumstances.
        /// </summary>
        public bool Equals(IXenObject other)
        {
            return Equals((object)other);
        }

        #endregion

        public object Get(String property)
        {
            PropertyInfo pi = GetType().GetProperty(property);
            if (pi == null)
                return null;

            return pi.GetValue(this, null);
        }

        public void Set(String property, object val)
        {
            PropertyInfo pi = GetType().GetProperty(property);
            if (pi != null)
                pi.SetValue(this, val, null);
        }

        public void Do(String method, params Object[] methodParams)
        {
            MethodInfo mi = GetType().GetMethod(method, BindingFlags.Public | BindingFlags.Static);
            if (mi == null)
                return;

            try
            {
                mi.Invoke(this, methodParams);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public static Dictionary<string, string> SetDictionaryKey(Dictionary<string, string> dict, string key, string value)
        {
            return SetDictionaryKeys(dict, new KeyValuePair<string, string>(key, value));
        }

        public static Dictionary<string, string> SetDictionaryKeys(Dictionary<string, string> dict, params KeyValuePair<string, string>[] kvps)
        {
            var newDict = dict == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(dict);

            foreach (var kvp in kvps)
            {
                string key = kvp.Key;
                string value = kvp.Value;

                if (value == null)
                    newDict.Remove(key);
                else
                    newDict[key] = value;
            }

            return newDict;
        }

        /// <summary>
        /// If d[k] == "true", then return true.  Anything else is false.
        /// Handles all the cases with d being null or not containing k.
        /// </summary>
        protected static bool BoolKey(Dictionary<string, string> d, string k)
        {
            string v = Get(d, k);
            return v != null && v == "true";
        }

        /// <summary>
        /// Converts dictionary pair to a bool.
        /// Only return false if it's looked up otherwise return true
        /// 
        /// This is similar to BoolKey but BoolKey prefers to return false,
        /// whereas BoolKeyPreferTrue prefers true;
        /// </summary>
        protected static bool BoolKeyPreferTrue(Dictionary<string, string> d, string k)
        {
            string v = Get(d, k);
            return String.IsNullOrEmpty(v) || v != "false";
        }

        /// <summary>
        /// If d[k] is parseable as an integer, then return it, otherwise return def.
        /// Handles all the cases with d being null or not containing k.
        /// </summary>
        protected int IntKey(Dictionary<string, string> d, string k, int def)
        {
            int result;
            string s = Get(d, k);
            return s != null && int.TryParse(s, out result) ? result : def;
        }

        public virtual string Name()
        {
            return "";
        }

        public virtual string NameWithLocation()
        {
            return string.Format(Messages.NAME_WITH_LOCATION, Name(), LocationString());
        }

        internal virtual string LocationString()
        {
            if (Connection == null || string.IsNullOrEmpty(Connection.Name))
                return string.Empty;

            if (Helpers.IsPool(Connection))
                return string.Format(Messages.IN_POOL, Connection.Name);

            return string.Format(Messages.ON_SERVER, Connection.Name);
        }
    }
}
