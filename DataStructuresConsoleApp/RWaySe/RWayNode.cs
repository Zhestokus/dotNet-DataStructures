using System.Collections;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.RWaySe
{
    public class RWayNode<TKey, TValue> : IEnumerable<RWayNode<TKey, TValue>>
    {
        public const int Size = 256;

        private bool _leaf;

        private TKey _key;
        private TValue _value;

        private readonly RWayNodesList<TKey, TValue> _nodesLoader;

        public RWayNode()
            : this(false, default(TKey), default(TValue))
        {
        }

        public RWayNode(bool leaf, TKey key, TValue value)
        {
            _leaf = leaf;

            _key = key;
            _value = value;

            _nodesLoader = new RWayNodesList<TKey, TValue>();
        }

        public bool Leaf
        {
            get { return _leaf; }
            set { _leaf = value; }
        }

        public TKey Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public RWayNode<TKey, TValue> this[int index]
        {
            get { return _nodesLoader[index]; }
            set { _nodesLoader[index] = value; }
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
    }
}