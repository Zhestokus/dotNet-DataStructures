using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataStructuresConsoleApp.Terany
{
    public class TeranyTrieDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly TeranyTrie<TKey, TValue> terany;

        public TeranyTrieDictionary()
            : this(new BinaryFormatter())
        {
        }

        public TeranyTrieDictionary(IFormatter keySerializer)
        {
            terany = new TeranyTrie<TKey, TValue>(keySerializer);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in terany)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            terany.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            terany.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var node = terany.Search(item.Key);
            return (node != null);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in terany)
            {
                if (arrayIndex >= array.Length)
                {
                    return;
                }

                array[arrayIndex++] = pair;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return terany.Remove(item.Key);
        }

        public int Count
        {
            get { return terany.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(TKey key)
        {
            var node = terany.Search(key);
            return (node != null);
        }

        public void Add(TKey key, TValue value)
        {
            terany.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return terany.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            var node = terany.Search(key);
            if (node == null)
            {
                return false;
            }

            value = node.Value;
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                var node = terany.Search(key);
                if (node == null)
                {
                    throw new Exception("Key not found");
                }

                return node.Value;
            }
            set
            {
                terany.AddOrUpdate(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var list = new List<TKey>();

                foreach (var pair in terany)
                {
                    list.Add(pair.Key);
                }

                return list;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var list = new List<TValue>();

                foreach (var pair in terany)
                {
                    list.Add(pair.Value);
                }

                return list;
            }
        }
    }

}
