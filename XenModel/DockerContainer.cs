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
using System.Xml;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Model
{
    public class DockerContainer : XenObject<DockerContainer>, IComparable
    {
        public DockerContainer()
        {
        }

        public DockerContainer(VM parent, string uuid, string name, string description, string status, string container,
            string created, string image, string command, string ports)
            : this()
        {
            this.parent = parent;
            this.Connection = parent.Connection;
            this.uuid = uuid;
            // Containers don't have opaque_ref at server side; 
            // We want to have an opaque_ref that is unique per connection, so we use parent.opaque_ref + uuid (container's id is only unique per VM)
            this.opaque_ref = parent.opaque_ref + uuid;
            this.name_label = name;
            this.name_description = description;
            this.status = status;
            this.container = container;
            this.created = created;
            this.image = image;
            this.command = command;
            this.ports = ports;
        }

        public VM Parent
        {
            get { return parent; }
        }
        private VM parent;

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid;

        /// <summary>
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label;

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description;

        /// <summary>
        /// a notes field containing status description
        /// </summary>
        public virtual string status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private string _status;

        /// <summary>
        /// a notes field containing container (id)
        /// </summary>
        public virtual string container
        {
            get { return _container; }
            set
            {
                if (!Helper.AreEqual(value, _container))
                {
                    _container = value;
                    NotifyPropertyChanged("container");
                }
            }
        }
        private string _container;
        
        /// <summary>
        /// a notes field containing creation time
        /// </summary>
        public virtual string created
        {
            get { return _created; }
            set
            {
                if (!Helper.AreEqual(value, _created))
                {
                    _created = value;
                    NotifyPropertyChanged("created");
                }
            }
        }
        private string _created;
        
        /// <summary>
        /// a notes field containing image information
        /// </summary>
        public virtual string image
        {
            get { return _image; }
            set
            {
                if (!Helper.AreEqual(value, _image))
                {
                    _image = value;
                    NotifyPropertyChanged("image");
                }
            }
        }
        private string _image;

        public virtual string command
        {
            get { return _command; }
            set
            {
                if (!Helper.AreEqual(value, _command))
                {
                    _command = value;
                    NotifyPropertyChanged("command");
                }
            }
        }
        private string _command;

        public virtual string ports
        {
            get { return _ports; }
            set
            {
                if (!Helper.AreEqual(value, _ports))
                {
                    _ports = value;
                    NotifyPropertyChanged("ports");
                }
            }
        }
        private string _ports;

        public override string ToString()
        {
            return _name_label;
        }

        public override bool Equals(object obj)
        {
            DockerContainer other = obj as DockerContainer;

            return other != null &&
                uuid.Equals(other.uuid);
        }


        public override int GetHashCode()
        {
            return name_label.GetHashCode();
        }

        #region IComparable Members


        public override int CompareTo(DockerContainer other)
        {
            if (other == null)
                return 1;

            return StringUtility.NaturalCompare(_uuid, other.uuid);
        }

        #endregion

        public override void UpdateFrom(DockerContainer update)
        {
            parent = update.parent;
            Connection = update.parent.Connection;
            uuid = update.uuid;
            opaque_ref = parent.opaque_ref + uuid; //using parent.opaque_ref + uuid, because opaque_ref does not exist at server side
            name_label = update.name_label;
            name_description = update.name_description;
            status = update.status;
            container = update.container;
            created = update.created;
            image = update.image;
            command = update.command;
            ports = update.ports;
        }

        public override string SaveChanges(Session session, string _serverOpaqueRef, DockerContainer serverObject)
        {
            return String.Empty;
        }

        public override string Name()
        {
            return name_label;
        }

        public vm_power_state power_state
        {
            get {
                return status.Contains("Paused") 
                    ? vm_power_state.Paused
                    : status.StartsWith("Up") ? vm_power_state.Running : vm_power_state.Halted;
            }
        }

        public override string NameWithLocation()
        {
            if (parent == null)
                return base.NameWithLocation();
            return string.Format(Messages.CONTAINER_ON_VM_TITLE, Name(), parent.Name(), parent.LocationString());
        }

        public List<DockerContainerPort> PortList
        {
            get
            {
                var portList = new List<DockerContainerPort>();
                if (string.IsNullOrEmpty(ports))
                    return portList;

                var xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml("<items>" + ports + "</items>"); // wrap the ports into a root node
                    var items = xmlDoc.GetElementsByTagName("item");

                    foreach (XmlNode node in items)
                    {
                        var item = new DockerContainerPort();
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "IP":
                                    item.Address = child.InnerText;
                                    break;
                                case "PublicPort":
                                    item.PublicPort = child.InnerText;
                                    break;
                                case "PrivatePort":
                                    item.PrivatePort = child.InnerText;
                                    break;
                                case "Type":
                                    item.Protocol = child.InnerText;
                                    break;
                            }
                        }
                        portList.Add(item);
                    }
                }
                catch { }
                return portList;
            }
        }

        public struct DockerContainerPort
        {
            public string Address, PublicPort, PrivatePort, Protocol;

            public string Description
            {
                get
                {
                    var list = new List<string>();
                    if (!string.IsNullOrEmpty(Address))
                        list.Add(string.Format(Messages.CONTAINER_PORTS_ADDRESS, Address));
                    if (!string.IsNullOrEmpty(PublicPort))
                        list.Add(string.Format(Messages.CONTAINER_PORTS_PUBLIC_PORT, PublicPort));
                    if (!string.IsNullOrEmpty(PrivatePort))
                        list.Add(string.Format(Messages.CONTAINER_PORTS_PRIVATE_PORT, PrivatePort));
                    if (!string.IsNullOrEmpty(Protocol))
                        list.Add(string.Format(Messages.CONTAINER_PORTS_PROTOCOL, Protocol));
                    return string.Join("; ", list);
                }
            }
        }

    }
}
