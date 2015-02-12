using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataStructuresConsoleApp.Terany
{
    public class TeranyTrie<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly IFormatter _keySerializer;

        private TeranyNode<TKey, TValue> _root;
        private int _count;

        public TeranyTrie()
            : this(new BinaryFormatter())
        {
        }

        public TeranyTrie(IFormatter keySerializer)
        {
            _keySerializer = keySerializer;
            _root = new TeranyNode<TKey, TValue>();
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
            var index = 0;
            var length = keyBytes.Length - 1;

            if (_root == null)
                _root = new TeranyNode<TKey, TValue>(keyBytes[0]);

            var node = _root;

            while (true)
            {
                var @byte = keyBytes[index];

                if (@byte < node.Byte)
                {
                    if (node.Left == null)
                        node.Left = new TeranyNode<TKey, TValue>(@byte);

                    node = node.Left;
                }
                else if (@byte > node.Byte)
                {
                    if (node.Right == null)
                        node.Right = new TeranyNode<TKey, TValue>(@byte);

                    node = node.Right;
                }
                else if (index < length)
                {
                    if (node.Middle == null)
                        node.Middle = new TeranyNode<TKey, TValue>(keyBytes[index + 1]);

                    node = node.Middle;
                    index++;
                }
                else
                {
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
            }
        }

        public bool Remove(TKey key)
        {
            var node = Search(key);
            if (node != null && node.Leaf)
            {
                node.Leaf = false;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public TeranyNode<TKey, TValue> Search(TKey key)
        {
            return Search(_root, key);
        }
        private TeranyNode<TKey, TValue> Search(TeranyNode<TKey, TValue> node, TKey key)
        {
            var keyBytes = Serialize(key);
            return Search(node, keyBytes);
        }
        private TeranyNode<TKey, TValue> Search(TeranyNode<TKey, TValue> node, byte[] keyBytes)
        {
            if (node == null)
            {
                return null;
            }

            var index = 0;
            var length = keyBytes.Length - 1;

            while (true)
            {
                if (node == null)
                    return null;

                var @byte = keyBytes[index];

                if (@byte < node.Byte)
                    node = node.Left;
                else if (@byte > node.Byte)
                    node = node.Right;
                else if (index < length)
                {
                    node = node.Middle;
                    index++;
                }
                else
                {
                    if (!node.Leaf)
                        return null;

                    return node;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Travers(_root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<KeyValuePair<TKey, TValue>> Travers(TeranyNode<TKey, TValue> node)
        {
            var stack = new Stack<TeranyNode<TKey, TValue>>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var n = stack.Pop();

                if (n.Left != null)
                    stack.Push(n.Left);

                if (n.Middle != null)
                    stack.Push(n.Middle);

                if (n.Right != null)
                    stack.Push(n.Right);

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
