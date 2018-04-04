/* 
OpenDHT.NET library, based on XML-RPC.NET
Copyright (c) 2006, Michel Foucault <mmarsu@gmail.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

using System;

namespace OpenDHTLib
{
    [Serializable]
    public class OpenDHTMessage : IOpenDHTKeyValue
    {
        string key = string.Empty;
        byte[] data = new byte[0];
        string secret = string.Empty;
        int ttl;

        public OpenDHTMessage()
        {
        }
        public OpenDHTMessage(string key, byte[] data, int ttl)
        {
            Init(key, data, string.Empty, ttl);
        }

        public OpenDHTMessage(string key, string data, string secret, int ttl)
        {
            byte[] dataByte = OpenDHT.GetBytes(data);
            Init(key, dataByte, secret, ttl);
        }

        public OpenDHTMessage(string key, byte[] data, string secret, int ttl)
        {
            Init(key, data, secret, ttl);
        }

        private void Init(string key, byte[] data, string secret, int ttl)
        {
            this.key = key;
            this.data = (byte[])data.Clone();
            this.secret = secret;
            this.ttl = ttl;
        }
        #region IOpenDHTKeyValue Members

        public byte[] Key
        {
            get { return OpenDHT.GetBytes(key); }
            set { key = OpenDHT.GetString(value); }
        }

        public string KeyStr
        {
            get { return OpenDHT.GetString(Key); }
            set { Key = OpenDHT.GetBytes(value); }
        }

        public byte[] Value
        {
            get { return (byte[])data.Clone(); }
            set { data = (byte[])value.Clone(); }
        }
        public string Content
        {
            get { return OpenDHT.GetString(Value); }
            set { Value = OpenDHT.GetBytes(value); }
        }

        public string Secret
        {
            get { return secret; }
            set { secret = value; }
        }

        public int TTL
        {
            get { return ttl; }
            set { ttl = Math.Max(0, value); }
        }

        #endregion
    }
}
