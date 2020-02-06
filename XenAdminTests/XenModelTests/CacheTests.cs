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
using NUnit.Framework;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;
using CacheEntry = System.Collections.Generic.KeyValuePair<XenAdmin.Core.ObjectChange, System.Func<XenAdmin.Network.ICache, bool>>;


namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CacheTests
    {
        private static IEnumerable<CacheEntry> ObjectChanges
        {
            get
            {
                yield return new CacheEntry(new ObjectChange(typeof(VM), "1", new VM()), cache => cache.VMs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Folder), "1", new Folder(null, "folder")), cache => cache.Folders.Length == 0);
                yield return new CacheEntry(new ObjectChange(typeof(Host), "1", new Host()), cache => cache.Hosts.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Pool), "1", new Pool()), cache => cache.Pools.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(VMSS), "1", new VMSS()), cache => cache.VMSSs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Network), "1", new Network()), cache => cache.Networks.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(VBD), "1", new VBD()), cache => cache.VBDs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Bond), "1", new Bond()), cache => cache.Bonds.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(VDI), "1", new VDI()), cache => cache.VDIs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(PBD), "1", new PBD()), cache => cache.PBDs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Tunnel), "1", new Tunnel()), cache => cache.Tunnels.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(VIF), "1", new VIF()), cache => cache.VIFs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(SM), "1", new SM()), cache => cache.SMs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Pool_patch), "1", new Pool_patch()), cache => cache.Pool_patches.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(SR), "1", new SR()), cache => cache.SRs.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Message), "1", new Message()), cache => cache.Messages.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Host_cpu), "1", new Host_cpu()), cache => cache.Host_cpus.Length == 1);
                yield return new CacheEntry(new ObjectChange(typeof(Role), "1", new Role()), cache => cache.Roles.Length == 1);
            }
        }

        [Test]
        public void TestsCacheCollectionsAfterUpdateFrom([ValueSource(nameof(ObjectChanges))]  CacheEntry test)
        {
            var changes = test.Key;
            Cache cache = new Cache();
            if (changes.type == typeof(Folder))
                Assert.Throws<ArgumentException>(() => cache.UpdateFrom(new XenConnection(), new List<ObjectChange> { changes }));
            else
                cache.UpdateFrom(new XenConnection(), new List<ObjectChange> { changes });

            Assert.IsTrue(test.Value(cache));
        }

        [Test]
        public void FolderTest()
        {
            Cache cache = new Cache();
            Folder folder = new Folder(null, "folder") {opaque_ref = "1"};
            cache.AddFolder(new XenRef<Folder>(folder.opaque_ref), folder);
            Assert.IsTrue(cache.Folders.Length == 1);
            Assert.AreEqual(folder, cache.Resolve(new XenRef<Folder>(folder.opaque_ref)));
            Assert.AreEqual(folder, cache.Folders[0]);
            //Check tryresolve
            cache.TryResolve(new XenRef<Folder>(folder.opaque_ref), out var result);
            Assert.AreEqual(folder, result);
            //Check Clear
            cache.Clear();
            Assert.IsTrue(cache.Folders.Length == 0);
        }

    }
}
