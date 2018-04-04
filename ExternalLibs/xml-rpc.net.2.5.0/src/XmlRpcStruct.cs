/* 
XML-RPC.NET library
Copyright (c) 2001-2009, Charles Cook <charlescook@cookcomputing.com>

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
using System.Collections;

namespace CookComputing.XmlRpc
{
  public class XmlRpcStruct : Hashtable
  {
    private ArrayList _keys = new ArrayList();
    private ArrayList _values = new ArrayList();

    public override void Add(object key, object value)
    {
      if (!(key is string))
      {
        throw new ArgumentException("XmlRpcStruct key must be a string.");
      }
      if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
          == XmlRpcType.tInvalid)
      {
        throw new ArgumentException(String.Format(
          "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
      }
      base.Add(key, value);
      _keys.Add(key);
      _values.Add(value);
    }

    public override object this[object key]
    {
      get
      {
        return base[key];
      }
      set
      {
        if (!(key is string))
        {
          throw new ArgumentException("XmlRpcStruct key must be a string.");
        }
        if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
            == XmlRpcType.tInvalid)
        {
          throw new ArgumentException(String.Format(
            "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
        }
        base[key] = value;
        _keys.Add(key);
        _values.Add(value);
      }
    }

    public override bool Equals(Object obj)
    {
      if (obj.GetType() != typeof(XmlRpcStruct))
        return false;
      XmlRpcStruct xmlRpcStruct = (XmlRpcStruct)obj;
      if (this.Keys.Count != xmlRpcStruct.Count)
        return false;
      foreach (String key in this.Keys)
      {
        if (!xmlRpcStruct.ContainsKey(key))
          return false;
        if (!this[key].Equals(xmlRpcStruct[key]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hash = 0;
      foreach (object obj in Values)
      {
        hash ^= obj.GetHashCode();
      }
      return hash;
    }

    public override void Clear()
    {
      base.Clear();
      _keys.Clear();
      _values.Clear();
    }

    public new IDictionaryEnumerator GetEnumerator()
    {
      return new XmlRpcStruct.Enumerator(_keys, _values);
    }

    public override ICollection Keys
    {
      get
      {
        return _keys;
      }
    }

    public override void Remove(object key)
    {
      base.Remove(key);
      int idx = _keys.IndexOf(key);
      if (idx >= 0)
      {
        _keys.RemoveAt(idx);
        _values.RemoveAt(idx);
      }
    }

    public override ICollection Values
    {
      get
      {
        return _values;
      }
    }

    private class Enumerator : IDictionaryEnumerator
    {
      private ArrayList _keys;
      private ArrayList _values;
      private int _index;

      public Enumerator(ArrayList keys, ArrayList values)
      {
        _keys = keys;
        _values = values;
        _index = -1;
      }

      public void Reset()
      {
        _index = -1;
      }

      public object Current
      {
        get
        {
          CheckIndex();
          return new DictionaryEntry(_keys[_index], _values[_index]);
        }
      }

      public bool MoveNext()
      {
        _index++;
        if (_index >= _keys.Count)
          return false;
        else
          return true;
      }

      public DictionaryEntry Entry
      {
        get
        {
          CheckIndex();
          return new DictionaryEntry(_keys[_index], _values[_index]);
        }
      }

      public object Key
      {
        get
        {
          CheckIndex();
          return _keys[_index];
        }
      }

      public object Value
      {
        get
        {
          CheckIndex();
          return _values[_index];
        }
      }

      private void CheckIndex()
      {
        if (_index < 0 || _index >= _keys.Count)
          throw new InvalidOperationException(
            "Enumeration has either not started or has already finished.");
      }
    }
  }
}