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
using NUnit.Framework;
using XenAdmin.Controls;


namespace XenAdminTests.TreeTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class MultiSelectTreeViewTests
    {
        private MultiSelectTreeView _tv;

        [SetUp]
        public void Setup()
        {
            _tv = new MultiSelectTreeView();

            // ensure handle is created. Some tests fail if you don't do this.
           Assert.NotNull(_tv.Handle);
        }

        [TearDown]
        public void TearDown()
        {
            if (_tv != null)
                _tv.Dispose();
        }

        /// <summary>
        /// Tests that the SetContents method fires only once even when a complex change takes place in the
        /// selected nodes collection.
        /// </summary>
        [Test]
        public void TestSetContents()
        {
            int count = 0;

            EventHandler handler = delegate
            {
                count++;
            };

            _tv.SelectionsChanged += handler;
            
            try
            {
                _tv.Nodes.Add(new MultiSelectTreeNode("0"));
                _tv.Nodes.Add(new MultiSelectTreeNode("1"));
                _tv.SelectedNodes.SetContents(new [] { _tv.Nodes[0], _tv.Nodes[1] });
                
                Assert.AreEqual(1, count, "SelectionsChanged should only have fired once");

                _tv.Nodes.Add(new MultiSelectTreeNode("2"));
                _tv.Nodes.Add(new MultiSelectTreeNode("3"));
                
                _tv.SelectedNodes.SetContents(new [] { _tv.Nodes[2], _tv.Nodes[3] });
                Assert.AreEqual(2, count, "SelectionsChanged should only have fired twice");
            }
            finally
            {
                _tv.SelectionsChanged -= handler;
            }
        }
    }
}
