using System;
using System.Collections;
using System.IO;
using System.Reflection;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{
  [TestFixture]
  public class XmlRpcStructTest
  {
    [Test]
    public void Set()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps["foo"] = "abcdef";
      Assert.AreEqual("abcdef", xps["foo"]);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void SetInvalidKey()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps[1] = "abcdef";
    }

    [Test]
    public void DoubleSet()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps["foo"] = "12345";
      xps["foo"] = "abcdef";
      Assert.AreEqual("abcdef", xps["foo"]);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void AddInvalidKey()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps.Add(1, "abcdef");
    }

    [Test]
    public void Add()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps.Add("foo", "abcdef");
      Assert.AreEqual("abcdef", xps["foo"]);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void DoubleAdd()
    {
      XmlRpcStruct xps = new XmlRpcStruct();
      xps.Add("foo", "123456");
      xps.Add("foo", "abcdef");
    }

    [Test]
    public void OrderingEnumerator()
    {
      var xps = new XmlRpcStruct();
      xps.Add("1", "a");
      xps.Add("3", "c");
      xps.Add("2", "b");
      IDictionaryEnumerator enumerator = xps.GetEnumerator();
      enumerator.MoveNext();
      Assert.AreEqual("1", enumerator.Key);
      Assert.AreEqual("a", enumerator.Value);
      Assert.IsInstanceOfType(typeof(DictionaryEntry), enumerator.Current);
      DictionaryEntry de = (DictionaryEntry)enumerator.Current;
      Assert.AreEqual("1", de.Key);
      Assert.AreEqual("a", de.Value);
      enumerator.MoveNext();
      Assert.AreEqual("3", enumerator.Key);
      Assert.AreEqual("c", enumerator.Value);
      Assert.IsInstanceOfType(typeof(DictionaryEntry), enumerator.Current);
      de = (DictionaryEntry)enumerator.Current;
      Assert.AreEqual("3", de.Key);
      Assert.AreEqual("c", de.Value);
      enumerator.MoveNext();
      Assert.AreEqual("2", enumerator.Key);
      Assert.AreEqual("b", enumerator.Value);
      Assert.IsInstanceOfType(typeof(DictionaryEntry), enumerator.Current);
      de = (DictionaryEntry)enumerator.Current;
      Assert.AreEqual("2", de.Key);
      Assert.AreEqual("b", de.Value);
    }

    [Test]
    public void OrderingKeys()
    {
      var xps = new XmlRpcStruct();
      xps.Add("1", "a");
      xps.Add("3", "c");
      xps.Add("2", "b");

      IEnumerator enumerator = xps.Keys.GetEnumerator();
      enumerator.MoveNext();
      Assert.AreEqual("1", enumerator.Current);
      enumerator.MoveNext();
      Assert.AreEqual("3", enumerator.Current);
      enumerator.MoveNext();
      Assert.AreEqual("2", enumerator.Current);
    }

    [Test]
    public void OrderingValues()
    {
      var xps = new XmlRpcStruct();
      xps.Add("1", "a");
      xps.Add("3", "c");
      xps.Add("2", "b");

      IEnumerator enumerator = xps.Values.GetEnumerator();
      enumerator.MoveNext();
      Assert.AreEqual("a", enumerator.Current);
      enumerator.MoveNext();
      Assert.AreEqual("c", enumerator.Current);
      enumerator.MoveNext();
      Assert.AreEqual("b", enumerator.Current);
    }
  }
}
