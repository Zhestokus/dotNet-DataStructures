using System.Collections;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.RWaySe
{
    public class RWayNodesPage<TKey, TValue> : IEnumerable<RWayNode<TKey, TValue>>
    {
        private const int Size = 16;

        private bool _inited;

        private RWayNode<TKey, TValue>[] _children;

        public RWayNode<TKey, TValue> this[int index]
        {
            get { return ReadNode(index); }
            set { WriteNode(value, index); }
        }

        public IEnumerator<RWayNode<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < Size; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Init()
        {
            if (_inited)
                return;

            _inited = true;

            _children = new RWayNode<TKey, TValue>[Size];
        }

        private RWayNode<TKey, TValue> ReadNode(int index)
        {
            Init();

            if (_children == null)
                return null;

            var child = _children[index];
            return child;
        }

        private void WriteNode(RWayNode<TKey, TValue> node, int index)
        {
            Init();

            _children[index] = node;
        }
    }
}