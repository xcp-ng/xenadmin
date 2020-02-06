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
using System.ComponentModel;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin
{
    public class OtherConfigAndTagsWatcher
    {
        public static event Action OtherConfigChanged;
        public static event Action TagsChanged;
        public static event Action GuiConfigChanged;
        private static bool FireOtherConfigEvent;
        private static bool FireTagsEvent;
        private static bool FireGuiConfigEvent;

        static OtherConfigAndTagsWatcher()
        {
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            MarkEventsReadyToFire(true);
        }

        public static void DeregisterEventHandlers()
        {
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                connection.Cache.DeregisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                connection.Cache.DeregisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                connection.Cache.DeregisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                connection.Cache.DeregisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                connection.Cache.DeregisterCollectionChanged<VDI>(VDICollectionChangedWithInvoke);
                connection.Cache.DeregisterCollectionChanged<XenAPI.Network>(NetworkCollectionChangedWithInvoke);

                connection.XenObjectsUpdated -= connection_XenObjectsUpdated;
                connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
            }
        }

        public static void InitEventHandlers()
        {
            PoolCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<Pool>);
            VMCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<VM>);
            HostCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<Host>);
            SRCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<SR>);
            VDICollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<VDI>);
            NetworkCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<XenAPI.Network>);

            MarkEventsReadyToFire(true);
        }

        private static CollectionChangeEventHandler PoolCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler VMCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler HostCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler SRCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler VDICollectionChangedWithInvoke;
        private static CollectionChangeEventHandler NetworkCollectionChangedWithInvoke;

        private static void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e == null)
                return;

            IXenConnection connection = e.Element as IXenConnection;

            if (e.Action == CollectionChangeAction.Add)
            {
                if (connection == null)
                    return;

                connection.Cache.RegisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<VDI>(VDICollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<XenAPI.Network>(NetworkCollectionChangedWithInvoke);

                connection.XenObjectsUpdated -= connection_XenObjectsUpdated;
                connection.XenObjectsUpdated += connection_XenObjectsUpdated;
                connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                connection.ConnectionStateChanged += connection_ConnectionStateChanged;
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                var range = new List<IXenConnection>();
                if (connection != null)
                {
                    range.Add(connection);
                }
                else
                {
                    var r = e.Element as List<IXenConnection>;
                    if (r != null)
                        range = r;
                    else
                        return;
                }

                foreach (var con in range)
                {
                    con.Cache.DeregisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                    con.Cache.DeregisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                    con.Cache.DeregisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                    con.Cache.DeregisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                    con.Cache.DeregisterCollectionChanged<VDI>(VDICollectionChangedWithInvoke);
                    con.Cache.DeregisterCollectionChanged<XenAPI.Network>(NetworkCollectionChangedWithInvoke);

                    con.XenObjectsUpdated -= connection_XenObjectsUpdated;
                    con.ConnectionStateChanged -= connection_ConnectionStateChanged;
                }
            }

            MarkEventsReadyToFire(true);
        }

        private static void CollectionChanged<T>(object sender, CollectionChangeEventArgs e) where T : XenObject<T>
        {
            T xmo = e.Element as T;

            if (xmo == null)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    xmo.PropertyChanged += PropertyChanged<T>;
                    break;
                case CollectionChangeAction.Remove:
                    xmo.PropertyChanged -= PropertyChanged<T>;
                    break;
                case CollectionChangeAction.Refresh:
                    // As of writing, ChangeableDictionary never fires a Refresh event.
                    // If this changes, we need to take it into account here.
                    throw new NotImplementedException("CollectionChangeAction.Refresh is unhandled!");
            }

            MarkEventsReadyToFire(true);
        }

        private static void PropertyChanged<T>(object sender1, PropertyChangedEventArgs e) where T : XenObject<T>
        {
            if (e.PropertyName == "other_config")
                FireOtherConfigEvent = true;

            if (e.PropertyName == "tags")
                FireTagsEvent = true;

            if (e.PropertyName == "gui_config")
                FireGuiConfigEvent = true;
        }

        private static void connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (FireOtherConfigEvent)
                OnOtherConfigChanged();

            if (FireTagsEvent)
                OnTagsChanged();

            if (FireGuiConfigEvent)
                OnGuiConfigChanged();

            MarkEventsReadyToFire(false);
        }

        private static void connection_ConnectionStateChanged(IXenConnection conn)
        {
            InvokeHelper.Invoke(delegate
            {
                OnOtherConfigChanged();
                OnTagsChanged();
                OnGuiConfigChanged();

                MarkEventsReadyToFire(false);
            });
        }

        private static void MarkEventsReadyToFire(bool fire)
        {
            FireOtherConfigEvent = fire;
            FireTagsEvent = fire;
            FireGuiConfigEvent = fire;
        }

        private static void OnOtherConfigChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (OtherConfigChanged != null)
                OtherConfigChanged();
        }

        private static void OnTagsChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (TagsChanged != null)
                TagsChanged();
        }

        private static void OnGuiConfigChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (GuiConfigChanged != null)
                GuiConfigChanged();
        }
    }
}
