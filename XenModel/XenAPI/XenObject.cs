/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace XenAPI
{
    public abstract partial class XenObject<S> : IXenObject where S : XenObject<S>
    {
        /// <summary>
        /// Copies properties from 'update' into this object. It uses property setters so that
        /// event handlers are triggered if the value is changing.
        /// </summary>
        public abstract void UpdateFrom(S update);

        /// <summary>
        /// Save any changed fields to the server.
        /// This method is usually invoked on a thread pool thread.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="serverOpaqueRef"/>
        /// <param name="serverObject">Changes are sent to the server if the field in "this"
        /// is different from serverObject. Can be the object in the cache, or another reference
        /// object that we want to save changes to.</param>
        public abstract string SaveChanges(Session session, string serverOpaqueRef, S serverObject);

        public string opaque_ref { get; set; }

        [JsonIgnore]
        [Obsolete("This property can no longer be used to deduce that a XenObject has changed: " +
                  "now it always returns true and new values assigned to it are ignored. " +
                  "To be notified when the XenObject has changed (i.e. its properties have new values)," +
                  "handle the event PropertyChanged instead.")]
        public bool Changed
        {
            get { return true; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void ClearEventListeners()
        {
            PropertyChanged = null;
        }

        public JObject ToJObject()
        {
            return JObject.FromObject(this);
        }
    }
}
