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

namespace XenAdmin.Core
{
    // A method for which we might need to run an RBAC test.
    public class RbacMethod
    {
        public const string KEY_SPLITTER = "/key:";
        private readonly string method;
        private readonly string key;

        /// <param name="method">The method to be checked. In the form "Object.Method" (e.g., "vm.start")
        /// or "http/get_host_backup"</param>
        public RbacMethod(string method)
        {
            this.method = method.ToLowerInvariant();
            this.method = this.method.Replace("async_", "");
        }

        /// <summary>
        /// An API call that edits a specific key in a hash table
        /// </summary>
        /// <param name="method">The method to be checked. In the form "Object.Method" (e.g., "vm.add_to_other_config").</param>
        /// <param name="key">The hash key to be altered by the call.</param>
        public RbacMethod(string method, string key)
            : this(method)
        {
            this.key = key.ToLowerInvariant();
        }

        public string Method
        {
            get { return method; }
        }

        public string Key
        {
            get { return key; }
        }

        public override string ToString()
        {
            if (key == null)
                return method;
            else
                return method + KEY_SPLITTER + key;
        }
    }

    // Syntactic sugar for making a list of RbacMethod's without doing "new RbacMethod" all the time
    public class RbacMethodList : List<RbacMethod>
    {
        public RbacMethodList()
        {
        }

        /// <summary>
        /// Note that this constructor takes simple API calls, or complex ones with hash table edits and keys deliminated by a RbacMethod.KEY_SPLITTER
        /// Silently ignores blank methods, blank keys and entries with too many RbacMethod.KEY_SPLITTER
        /// </summary>
        public RbacMethodList(params string[] methods)
        {
            foreach (string entry in methods)
            {
                if (entry == null)
                    continue;

                string[] entrySplit = entry.Split(new string[] { RbacMethod.KEY_SPLITTER }, StringSplitOptions.None);
                string method = entrySplit[0].Trim();
                if (method == "")
                    continue;

                switch (entrySplit.Length)
                {
                    case 1:
                        // no splitters, it's just a method
                        Add(method);
                        break;
                    case 2:
                        // we have a splitter so define it as a key
                        string key = entrySplit[1].Trim();
                        if (key == "")
                            continue;
                        Add(method, key);
                        break;
                    default:
                        // ignore lengths of longer than 2, too many splitters
                        continue;
                }
                
            }
        }

        public RbacMethodList(params RbacMethod[] methods)
        {
            AddRange(methods);
        }

        /// <summary>
        /// Add a simple API call
        /// </summary>
        /// <param name="method">The method to be checked. In the form "Object.Method" (e.g., "vm.start")
        /// or "http/get_host_backup"</param>
        public void Add(string method)
        {
            Add(new RbacMethod(method));
        }

        /// <summary>
        /// Add a hash table API call with key to be edited
        /// </summary>
        /// <param name="method">The method to be checked. In the form "Object.Method" (e.g., "vm.add_to_other_config").</param>
        /// <param name="key">The hash key to be altered by the call.</param>
        public void Add(string method, string key)
        {
            Add(new RbacMethod(method, key));
        }

        public void AddRange(params string[] methods)
        {
            foreach (var method in methods)
                Add(method);
        }

        public string[] ToStringArray()
        {
            string[] ans = new string[this.Count];
            int i = 0;
            foreach (RbacMethod method in this)
                ans[i++] = method.ToString();
            return ans;
        }
    }
}
