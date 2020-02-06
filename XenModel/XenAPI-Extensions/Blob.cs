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
using System.IO;
using XenAdmin;

namespace XenAPI
{
    public partial class Blob
    {
        private const String BLOB_URI = "/blob";

        public void Save(Stream inStream, Session session)
        {
            if (session == null)
                throw new IOException();
            UriBuilder uri = new UriBuilder();
            uri.Scheme = Connection.UriScheme;
            uri.Host = Connection.Hostname;
            uri.Port = Connection.Port;
            uri.Path = BLOB_URI;
            uri.Query = String.Format("ref={0}&session_id={1}",
                opaque_ref, Uri.EscapeDataString(session.opaque_ref));

            using (Stream outStream = HTTPHelper.PUT(uri.Uri, inStream.Length, true))
            {
                HTTP.CopyStream(inStream, outStream, null, () => XenAdminConfigManager.Provider.ForcedExiting);
            }
        }

        public Stream Load()
        {
            Session session = Connection.Session;
            if (session == null)
                throw new IOException();
            UriBuilder uri = new UriBuilder();
            uri.Scheme = Connection.UriScheme;
            uri.Host = Connection.Hostname;
            uri.Port = Connection.Port;
            uri.Path = BLOB_URI;
            uri.Query = String.Format("ref={0}&session_id={1}",
                opaque_ref, Uri.EscapeDataString(session.opaque_ref));

            return HTTPHelper.GET(uri.Uri, Connection, true);
        }
    }
}
