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
using System.Collections.Generic;
using System.Text;

namespace OpenDHTLib
{
    [Serializable]
    public class OpenDHTMessageDetails
    {
        byte[] data = new byte[0];
        int ttl = 0;
        string algo = string.Empty;
        byte[] hash = new byte[0];

        public byte[] Data
        {
            get { return (byte[])data.Clone(); }
            set { data = (byte[])value.Clone(); }
        }

        public int TTL
        {
            get { return ttl; }
            set { ttl = value; }
        }

        public string Algo
        {
            get { return algo; }
            set { algo = value; }
        }

        public byte[] Hash
        {
            get { return (byte[])hash.Clone(); }
            set { hash = (byte[])value.Clone(); }
        }

        public static string ToHexa(byte[] data)
        {
            StringBuilder ret = new StringBuilder();
            foreach (byte b in data)
                ret.Append(string.Format("{0:x2}", b));
            return ret.ToString();
        }
        public override string ToString()
        {
            string str = OpenDHT.GetString(Data);
            if (str == string.Empty)
                str = ToHexa(Data);

            string hash = ToHexa(Hash).Substring(0, 8);
            return string.Format("{0} {1} {2} 0x{3}", str, TTL, Algo, hash);
        }
    }
}
