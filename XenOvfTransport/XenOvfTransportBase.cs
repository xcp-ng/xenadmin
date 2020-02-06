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
using XenAPI;


namespace XenOvfTransport
{
    public class XenOvfTransportBase
    {
        public Action<XenOvfTransportEventArgs> UpdateHandler { private get; set; }

		protected string m_networkUuid;
		protected bool m_isTvmIpStatic;
		protected string m_tvmIpAddress;
		protected string m_tvmSubnetMask;
		protected string m_tvmGateway;
        protected readonly Session XenSession;
        protected readonly Uri _uri;
		protected iSCSI m_iscsi;
		private bool m_cancel;

        protected XenOvfTransportBase(Uri uri, Session session)
        {
            _uri = uri;
        	XenSession = session;
        }

        protected void OnUpdate(XenOvfTransportEventArgs e)
        {
            UpdateHandler?.Invoke(e);
        }

		public bool Cancel
		{
			protected get { return m_cancel; }
			set
			{
				m_cancel = value;
				if (m_iscsi != null)
					m_iscsi.Cancel = value;
			}
		}

		public void SetTvmNetwork(string networkUuid, bool isTvmIpStatic, string tvmIpAddress, string tvmSubnetMask, string tvmGateway)
		{
			m_networkUuid = networkUuid;
			m_isTvmIpStatic = isTvmIpStatic;
			m_tvmIpAddress = tvmIpAddress;
			m_tvmSubnetMask = tvmSubnetMask;
			m_tvmGateway = tvmGateway;
		}
    }
}
