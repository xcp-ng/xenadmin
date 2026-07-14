/*
 * Copyright (c) Cloud Software Group, Inc.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace XenAPI
{
    public partial class XenRef<T> where T : XenObject<T>
    {
        private readonly string opaqueRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opaqueRef">May not be null.</param>
        public XenRef(string opaqueRef)
        {
            System.Diagnostics.Debug.Assert(opaqueRef != null, "'opaqueRef' parameter must not be null");
            this.opaqueRef = opaqueRef;
        }

        /// <summary>
        /// Create a XenRef from the opaque_ref of the given XenObject.
        /// </summary>
        /// <param name="obj"></param>
        public XenRef(T obj) : this(obj.opaque_ref)
        {
        }

        /// <param name="opaqueRef">Must be a string.</param>
        public static XenRef<T> Create(object opaqueRef)
        {
            return Create((string)opaqueRef);
        }

        public static XenRef<T> Create(string opaqueRef)
        {
            return new XenRef<T>(opaqueRef);
        }

        public static List<XenRef<T>> Create(string[] opaqueRefs)
        {
            List<XenRef<T>> objs = new List<XenRef<T>>(opaqueRefs.Length);
            foreach (string opaqueRef in opaqueRefs)
            {
                objs.Add(new XenRef<T>(opaqueRef));
            }
            return objs;
        }

        /// <summary>
        /// Creates a List of XenRefs from an array of objects by first calling ToString() on each array element.
        /// </summary>
        /// <param name="opaqueRefs">Must not be null. Must not contain null. May have Length zero. Each element must
        /// return an opaqueRef from its ToString() method.</param>
        /// <returns></returns>
        public static List<XenRef<T>> Create(object[] opaqueRefs)
        {
            List<XenRef<T>> objs = new List<XenRef<T>>(opaqueRefs.Length);
            foreach (object opaqueRef in opaqueRefs)
            {
                objs.Add(new XenRef<T>(opaqueRef.ToString()));
            }
            return objs;
        }

        /// <summary>
        /// Takes a hashmap of string -> hashmap (ie XenAPI ref -> record)
        /// 
        /// Internally convert record to an S (ie a Proxy_VM etc) then uses
        /// that to create a new T (ie a VM) via UpdateFrom
        /// 
        /// returns ie a dict ((ref VM) -> VM)
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="o">May not be null.</param>
        /// <returns></returns>
        public static Dictionary<XenRef<T>,T> Create<S>(Object o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            Hashtable dict = (Hashtable) o;

            Dictionary<XenRef<T>, T> result = new Dictionary<XenRef<T>,T>();

            foreach (Object key in dict.Keys)
            {
                XenRef<T> t_ref = new XenRef<T>((String) key);
                Hashtable t_record = (Hashtable)dict[key];
                result[t_ref] = (T)Marshalling.convertStruct(typeof(T), t_record);
            }

            return result;
        }

        public string opaque_ref
        {
            get 
            {
               return opaqueRef;
            }
        }

        public override bool Equals(object obj)
        {
            XenRef<T> other = obj as XenRef<T>;
            if (other == null)
                return false;
          
            return opaqueRef.Equals(other.opaqueRef);
        }

        public override int GetHashCode()
        {
            return opaqueRef.GetHashCode();
        }

        // convert to an opaqueRef
        public static implicit operator string(XenRef<T> xenRef)
        {
            return xenRef == null ? null : xenRef.opaque_ref;
        }
    }
}
