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
using System.Text;
using NUnit.Framework;
using XenAPI;
using System.Collections;
using System.Reflection;
using XenAdmin.Model;
using System.Diagnostics;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class XenObjectEqualsTests
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Gets all Types that derive from IXenObject except Folder and DockerContainer
        /// </summary>
        public static IEnumerable<Type> AllXenObjectTypesExceptFolder
        {
            get
            {
                foreach (Type t in typeof(IXenObject).Assembly.GetTypes())
                {
                    if (!t.IsAbstract && typeof(IXenObject).IsAssignableFrom(t) && t.GetConstructor(new Type[0]) != null && !typeof(Folder).IsAssignableFrom(t) && !typeof(DockerContainer).IsAssignableFrom(t))
                    {
                        yield return t;
                    }
                }
            }
        }

        /// <summary>
        /// Tests object.Equals uses opaque_ref for all types of IXenObject except folders.
        /// </summary>
        [Test]
        public void TestObjectEquals([ValueSource("AllXenObjectTypesExceptFolder")] Type xenObjectType)
        {
            IXenObject a = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(a);
            a.opaque_ref = "hello";

            IXenObject b = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(b);
            b.opaque_ref = "hello";

            IXenObject c = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(c);
            c.opaque_ref = "goodbye";

            Assert.IsTrue(a.Equals(b), xenObjectType.Name + " Equals failed");
            Assert.IsFalse(a.Equals(c), xenObjectType.Name + " Equals failed");
        }

        /// <summary>
        /// Tests that folder uses _name_label for object.Equals instead of opaque_ref.
        /// </summary>
        [Test]
        public void TestFolderObjectEquals()
        {
            Folder a = new Folder(null, "hello") { opaque_ref = "a" };
            Folder b = new Folder(null, "hello") { opaque_ref = "b" };
            Folder c = new Folder(null, "goodbye") { opaque_ref = "c" };
            Assert.AreEqual(a, b, "Folder Equals failed");
            Assert.AreNotEqual(a, c, "Folder Equals failed");
        }

        /// <summary>
        /// Tests that IEquatable has the same result as object.Equals for all types that derive from IXenObject except Folder.
        /// </summary>
        [Test]
        public void TestIEquatableUsage([ValueSource("AllXenObjectTypesExceptFolder")] Type xenObjectType)
        {
            IXenObject a = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(a);
            a.opaque_ref = "hello";

            IXenObject b = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(b);
            b.opaque_ref = "hello";

            IXenObject c = (IXenObject)Activator.CreateInstance(xenObjectType);
            SetRandomStuffToFields(c);
            c.opaque_ref = "goodbye";

            Assert.IsTrue(IEquatableEquals(a, b), xenObjectType.Name + " Equals failed");
            Assert.IsFalse(IEquatableEquals(a, c), xenObjectType.Name + " Equals failed");
        }

        /// <summary>
        /// Iterate through all implemented IEquatable&lt;T&gt; interfaces and call Equals on each, if any of them
        /// return false, then this method returns false, otherwise it returns true.
        /// 
        /// Both o and oo should be of the same type.
        /// </summary>
        private bool IEquatableEquals(IXenObject o, IXenObject oo)
        {
            Assert.AreEqual(o.GetType(), oo.GetType());

            foreach (Type iface in o.GetType().GetInterfaces())
            {
                if (iface.Name.StartsWith("IEquatable"))
                {
                    MethodInfo mi = iface.GetMethod("Equals");

                    if (!(bool)mi.Invoke(o, new object[] { oo }))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [Test]
        public void TestFolderIEquatableUsage()
        {
            Folder a = new Folder(null, "hello") { opaque_ref = "a" };
            Folder b = new Folder(null, "hello") { opaque_ref = "b" };
            Folder c = new Folder(null, "goodbye") { opaque_ref = "c" };

            IEquatable<IXenObject> aa = (IEquatable<IXenObject>)a;
            IEquatable<IXenObject> bb = (IEquatable<IXenObject>)b;
            IEquatable<IXenObject> cc = (IEquatable<IXenObject>)c;

            Assert.IsTrue(aa.Equals(bb), "Folder Equals failed");
            Assert.IsFalse(aa.Equals(cc), "Folder Equals failed");
        }

        /// <summary>
        /// Sets the random stuff to the string, int, long, double fields of the specified object.
        /// </summary>
        private void SetRandomStuffToFields(IXenObject xenObject)
        {
            foreach (FieldInfo fi in xenObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!fi.IsInitOnly && !fi.IsLiteral) //check field isn't const or readonly
                {
                    if (fi.FieldType == typeof(string))
                    {
                        fi.SetValue(xenObject, Guid.NewGuid().ToString());
                    }
                    else if (fi.FieldType == typeof(int) || fi.FieldType == typeof(long) || fi.FieldType == typeof(double))
                    {
                        fi.SetValue(xenObject, _random.Next());
                    }
                }
            }
        }

        [Test]
        public void TestCanDeriveHostAndOverrideEquals()
        {
            DerivedFromHost h1 = new DerivedFromHost { opaque_ref = "hello" };
            DerivedFromHost h2 = new DerivedFromHost { opaque_ref = "hello" };

            Assert.IsFalse(h1.Equals(h2));

            object hh1 = h1;
            object hh2 = h2;

            Assert.IsFalse(hh1.Equals(hh2));

            IEquatable<Host> hhh1 = h1;
            IEquatable<Host> hhh2 = h2;

            Assert.IsFalse(hhh1.Equals(hhh2));
        }

        public class DerivedFromHost : Host
        {
            private readonly object _o = new object();
            
            public override bool Equals(object other)
            {
                return ReferenceEquals(this, other);
            }
            public override int GetHashCode()
            {
                return _o.GetHashCode();
            }
        }
    }
}
