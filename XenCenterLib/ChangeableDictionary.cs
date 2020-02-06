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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace XenCenterLib
{
    /// <summary>
    /// This is a dictionary which fires events when it changes.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ChangeableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public event CollectionChangeEventHandler CollectionChanged;
        public event EventHandler BatchCollectionChanged;

        public void Add(TKey key, TValue value)
        {
            // will throw if aleady exists
            dictionary.Add(key, value);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }

        /// <summary>
        /// If this dictionary already contains a value for the given key, will fire a Remove event and then an Add event.
        /// Otherwise will just fire an Add event.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set
            {
                TValue v;

                if (dictionary.TryGetValue(key, out v))
                {
                    // There was already a value for the given key
                    // Remove old value and fire event
                    dictionary.Remove(key);
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, v));
                }

                dictionary[key] = value;
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
            }
        }

        public bool Remove(TKey key)
        {
            TValue v;
            if (!dictionary.TryGetValue(key, out v))
                return false;

            dictionary.Remove(key);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, v));
            return true;
        }

        public void OnBatchCollectionChanged()
        {
            if (BatchCollectionChanged != null)
            {
                try
                {
                    BatchCollectionChanged(this, null);
                }
                catch (Exception e)
                {
                    log.Debug("Exception firing batchCollectionChanged cache.", e);
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                        throw;
#endif
                }
            }
        }

        private void OnCollectionChanged(CollectionChangeEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }



        #region IDictionary<TKey,TValue> Members

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys => dictionary.Keys;

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => dictionary.Values;

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Caller beware: you won't get any CollectionChangedEvents from calling this method (e.g. a 'Remove' event for each element).
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDictionary Members

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return Contains((KeyValuePair<TKey, TValue>)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)GetEnumerator();
        }

        bool IDictionary.IsFixedSize => throw new NotImplementedException();

        ICollection IDictionary.Keys => dictionary.Keys;

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        ICollection IDictionary.Values => dictionary.Values;

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count => dictionary.Count;

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        #endregion
    }
}
