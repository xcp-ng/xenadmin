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
using CookComputing.XmlRpc;

namespace OpenDHTLib
{
    [XmlRpcUrl("http://opendht.nyuld.net:5851")]
    public interface IOpenDHT
    {
        [XmlRpcMethod("put")]
        int Put(byte[] key, byte[] data, int ttl, string application);

        [XmlRpcMethod("put_removable")]
        int PutRemovable(byte[] key, byte[] data, string algo, byte[] hash, int ttl, string application);

        [XmlRpcMethod("get")]
        object[] Get(byte[] key, int count, byte[] pm, string application);
        [XmlRpcMethod("get_details")]
        object[] GetDetails(byte[] key, int count, byte[] pm, string application);

        [XmlRpcMethod("rm")]
        int Rm(byte[] key, byte[] dataHash, string algo, byte[] secret, int ttl, string application);
    }
}
