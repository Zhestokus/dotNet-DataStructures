using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataStructuresConsoleApp.RWaySe
{
    public class RWayTrieSe<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly IFormatter _keySerializer;

        private readonly RWayNode<TKey, TValue> _root;

        private int _count;

        public RWayTrieSe()
            : this(new BinaryFormatter())
        {
        }

        public RWayTrieSe(IFormatter keySerializer)
        {
            _keySerializer = keySerializer;
            _root = new RWayNode<TKey, TValue>();
        }

        public int Count
        {
            get { return _count; }
        }

        public void Add(TKey key, TValue value)
        {
            var keyBytes = Serialize(key);
            Insert(key, value, keyBytes, false);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            var keyBytes = Serialize(key);
            Insert(key, value, keyBytes, true);
        }

        private void Insert(TKey key, TValue value, byte[] keyBytes, bool update)
        {
            var node = _root;

            for (int i = 0; i < keyBytes.Length; i++)
            {
                var @byte = keyBytes[i];

                var n = node[@byte];
                if (n == null)
                {
                    n = new RWayNode<TKey, TValue>();

                    node[@byte] = n;
                }

                node = n;
            }

            if (!node.Leaf)
            {
                node.Leaf = true;
                node.Key = key;
                node.Value = value;

                _count++;

                return;
            }

            if (update)
            {
                node.Key = key;
                node.Value = value;

                return;
            }

            throw new Exception("Key already exists");
        }

        public bool Remove(TKey key)
        {
            return Remove(_root, key);
        }
        private bool Remove(RWayNode<TKey, TValue> node, TKey key)
        {
            var n = Search(node, key);
            if (n == null || !n.Leaf)
                return false;

            n.Leaf = false;
            _count--;

            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < RWayNode<TKey, TValue>.Size; i++)
            {
                _root[i] = null;
            }

            _count = 0;
        }

        public RWayNode<TKey, TValue> Search(TKey key)
        {
            return Search(_root, key);
        }
        private RWayNode<TKey, TValue> Search(RWayNode<TKey, TValue> node, TKey key)
        {
            var keyBytes = Serialize(key);
            return Search(node, keyBytes);
        }
        private RWayNode<TKey, TValue> Search(RWayNode<TKey, TValue> node, byte[] keyBytes)
        {
            if (node == null)
            {
                return null;
            }

            for (int i = 0; i < keyBytes.Length; i++)
            {
                var @byte = keyBytes[i];
                node = node[@byte];

                if (node == null)
                {
                    return null;
                }
            }

            if (!node.Leaf)
                return null;

            return node;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Travers(_root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<KeyValuePair<TKey, TValue>> Travers(RWayNode<TKey, TValue> node)
        {
            var stack = new Stack<RWayNode<TKey, TValue>>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var n = stack.Pop();

                foreach (var child in n)
                {
                    if (child != null)
                        stack.Push(child);
                }

                if (n.Leaf)
                {
                    yield return new KeyValuePair<TKey, TValue>(n.Key, n.Value);
                }
            }
        }

        private byte[] Serialize(TKey key)
        {
            using (var stream = new MemoryStream())
            {
                _keySerializer.Serialize(stream, key);

                return stream.ToArray();
            }
        }
    }
}