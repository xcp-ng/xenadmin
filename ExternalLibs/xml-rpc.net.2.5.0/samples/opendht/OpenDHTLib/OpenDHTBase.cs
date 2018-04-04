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
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using CookComputing.XmlRpc;

namespace OpenDHTLib
{
    public abstract class OpenDHTBase : IOpenDHT
    {
        #region Put
        public int Put(string key, string data, int ttl)
        {
            byte[] keyBytes = GetBytes(key);
            byte[] dataBytes = GetBytes(data);
            return Put(keyBytes, dataBytes, ttl);
        }

        public int Put(string key, byte[] data, int ttl)
        {
            byte[] keyBytes = GetBytes(key);
            return Put(keyBytes, data, ttl);
        }

        public int Put(IOpenDHTKeyValue keyValue)
        {
            return Put(keyValue.Key, keyValue.Value, keyValue.TTL);
        }
        public int Put(byte[] key, byte[] data, int ttl)
        {
            return Put(key, data, ttl, ApplicationName);
        }

        public int Put(byte[] key, byte[] data, int ttl, string application)
        {
            key = HashAlgorithm.ComputeHash(key);
            return Proxy.Put(key, data, ttl, application);
        }
        #endregion
        #region PutRemovable
        public int PutRemovable(string key, string data, string secret, int ttl)
        {
            byte[] keyBytes = GetBytes(key);
            byte[] dataBytes = GetBytes(data);
            return PutRemovable(keyBytes, dataBytes, secret, ttl);
        }
        public int PutRemovable(string key, byte[] data, string secret, int ttl)
        {
            byte[] keyBytes = GetBytes(key);
            return PutRemovable(keyBytes, data, secret, ttl);
        }
        public int PutRemovable(IOpenDHTKeyValue keyValue)
        {
            if (keyValue.Secret == null)
                return PutRemovable(keyValue.Key, keyValue.Value, ApplicationName, keyValue.TTL);
            return PutRemovable(keyValue.Key, keyValue.Value, keyValue.Secret, keyValue.TTL);
        }
        public int PutRemovable(byte[] key, byte[] data, string secret, int ttl)
        {
            byte[] hash = HashAlgorithm.ComputeHash(GetBytes(secret));

            return PutRemovable(key, data, HashAlgo, hash, ttl, ApplicationName);
        }

        public int PutRemovable(byte[] key, byte[] data, string algo, byte[] hash, int ttl, string application)
        {
            key = HashAlgorithm.ComputeHash(key);
            return Proxy.PutRemovable(key, data, algo, hash, ttl, application);
        }

        #endregion

        #region Get
        public object[] Get(string key, int maxvals, byte[] pm)
        {
            byte[] keyBytes = GetBytes(key);
            return Get(keyBytes, maxvals, pm);
        }

        public object[] Get(byte[] key, int maxvals, byte[] pm)
        {
            return Get(key, maxvals, pm, ApplicationName);
        }

        public object[] Get(byte[] key, int maxvals, byte[] pm, string application)
        {
            key = HashAlgorithm.ComputeHash(key);
            return Proxy.Get(key, maxvals, pm, application);
        }

        #endregion
        #region GetDetails
        public object[] GetDetails(string key, int maxvals, byte[] pm)
        {
            byte[] keyBytes = GetBytes(key);
            return GetDetails(keyBytes, maxvals, pm);
        }

        public object[] GetDetails(byte[] key, int maxvals, byte[] pm)
        {
            return GetDetails(key, maxvals, pm, ApplicationName);
        }

        public object[] GetDetails(byte[] key, int maxvals, byte[] pm, string application)
        {
            key = HashAlgorithm.ComputeHash(key);
            return Proxy.GetDetails(key, maxvals, pm, application);
        }

        #endregion

        #region Rm
        public int Rm(string key, string data, string secret, int ttl)
        {
            byte[] dataBytes = GetBytes(data);
            return Rm(key, dataBytes, secret, ttl);

        }
        public int Rm(IOpenDHTKeyValue keyValue)
        {
            if (keyValue.Secret == null)
                return Rm(keyValue.Key, keyValue.Value, ApplicationName, keyValue.TTL);
            return Rm(keyValue.Key, keyValue.Value, keyValue.Secret, keyValue.TTL);
        }
        public int Rm(byte[] key, byte[] data, string secret, int ttl)
        {
            byte[] secretBytes = GetBytes(secret);
            return Rm(key, data, secretBytes, ttl);
        }

        public int Rm(string key, byte[] data, string secret, int ttl)
        {
            byte[] keyBytes = GetBytes(key);
            byte[] secretBytes = GetBytes(secret);
            return Rm(keyBytes, data, secretBytes, ttl);
        }

        public int Rm(byte[] key, byte[] data, byte[] secret, int ttl)
        {
            return Rm(key, data, HashAlgo, secret, ttl, ApplicationName);
        }

        public int Rm(byte[] key, byte[] data, string algo, byte[] secret, int ttl, string application)
        {
            key = HashAlgorithm.ComputeHash(key);
            byte[] dataHash = HashAlgorithm.ComputeHash(data);
            return Proxy.Rm(key, dataHash, algo, secret, ttl, application);
        }

        #endregion

        #region Overridable
        public string ApplicationName { get { return "OpenDht.Net v0.1"; } }

        protected string HashAlgo { get { return "SHA"; } }
        protected HashAlgorithm HashAlgorithm
        {
            get
            {
                HashAlgorithm hashAlgorithm = null;
                if (hashAlgorithm == null)
                {
                    hashAlgorithm = SHA1.Create();
                    hashAlgorithm.Initialize();
                }
                return hashAlgorithm;
            }
        }

        public int Maxvals { get { return 50; } }

        #endregion

        #region Helper
        public delegate object ProcessDelegate(byte[] data);
        public abstract object GetValue(byte[] data);

        protected object[] GetValues(string key)
        {
            ArrayList ret = new ArrayList();
            byte[] pm = new byte[0];
            do
            {
                object[] get = Get(key, Maxvals, pm);

                pm = (byte[])get[1];
                get = (object[])get[0];
                foreach (object obj in get)
                {
                    object val = GetValue((byte[])obj);
                    if (val != null)
                        ret.Add(val);
                }
            }
            while (pm.Length != 0);

            return ret.ToArray();
        }

        protected object[] GetDetailsValues(string key)
        {
            ArrayList ret = new ArrayList();
            byte[] pm = new byte[0];
            do
            {
                object[] get = GetDetails(key, Maxvals, pm);

                pm = (byte[])get[1];
                get = (object[])get[0];
                foreach (object[] obj in get)
                {
                    OpenDHTMessageDetails details = new OpenDHTMessageDetails();
                    details.Data = (byte[])obj[0];
                    details.TTL = (int)obj[1];
                    details.Algo = (string)obj[2];
                    details.Hash = (byte[])obj[3];
                    object val = GetValue(details.Data);
                    if (val != null)
                        ret.Add(details);
                }
            }
            while (pm.Length != 0);

            return ret.ToArray();
        }


        protected IOpenDHT proxy = null;
        public IOpenDHT Proxy
        {
            get
            {
                if (proxy == null)
                {
                    proxy = XmlRpcProxyGen.Create(typeof(IOpenDHT)) as IOpenDHT;
                    (proxy as XmlRpcClientProtocol).UseIndentation = false;
                }
                return proxy;
            }
        }

        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(byte[] data)
        {
            string ret = string.Empty;
            try
            {
                ret = Encoding.UTF8.GetString(data);
            }
            catch { ret = string.Empty; }
            return ret;
        }

        #endregion
    }
}
