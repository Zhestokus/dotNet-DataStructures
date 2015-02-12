using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.Heaps
{
    public class MaxHeap<TItem> : IHeap<TItem>
    {
        private readonly IComparer<TItem> _comparer;

        private TItem[] _items;
        private int _count;

        public MaxHeap(int initCapacity)
        {
            _items = new TItem[initCapacity + 1];
            _count = 0;
        }

        public MaxHeap()
            : this(1)
        {
        }

        public MaxHeap(IComparer<TItem> comparer)
            : this(1, comparer)
        {
        }

        public MaxHeap(int capacity, IComparer<TItem> comparer)
        {
            _comparer = comparer;
            _items = new TItem[capacity + 1];
            _count = 0;
        }

        public MaxHeap(IEnumerable<TItem> items)
            : this(items, Comparer<TItem>.Default)
        {
        }

        public MaxHeap(IEnumerable<TItem> items, IComparer<TItem> comparer)
        {
            _comparer = comparer;

            var list = new List<TItem>(items);

            _count = list.Count;
            _items = new TItem[list.Count + 1];

            for (int i = 0; i < _count; i++)
                _items[i + 1] = list[i];

            for (int k = _count / 2; k >= 1; k--)
                Sink(k);
        }

        private MaxHeap(MaxHeap<TItem> other)
        {
            _count = other._count;
            _comparer = other._comparer;

            var len = other._items.Length;

            _items = new TItem[len];
            Array.Copy(other._items, _items, len);
        }

        public int Count
        {
            get { return _count; }
        }

        public TItem Peek()
        {
            if (_count == 0)
                throw new KeyNotFoundException("Priority queue underflow");

            return _items[1];
        }

        public void Push(TItem item)
        {

            // double size of array if necessary
            if (_count >= _items.Length - 1)
                Resize(2 * _items.Length);

            // add x, and percolate it up to maintain heap invariant
            _items[++_count] = item;
            Swim(_count);
        }

        public TItem Pop()
        {
            if (_count == 0)
                throw new KeyNotFoundException("Priority queue underflow");

            var max = _items[1];

            Exch(1, _count--);
            Sink(1);

            _items[_count + 1] = default(TItem);     // to avoid loiterig and help with garbage collection

            if ((_count > 0) && (_count == (_items.Length - 1) / 4))
                Resize(_items.Length / 2);

            return max;
        }

        private void Swim(int index)
        {
            while (index > 1 && Less(index / 2, index))
            {
                Exch(index, index / 2);
                index = index / 2;
            }
        }

        private void Sink(int index)
        {
            while (2 * index <= _count)
            {
                var j = 2 * index;
                if (j < _count && Less(j, j + 1))
                    j++;

                if (!Less(index, j))
                    break;

                Exch(index, j);
                index = j;
            }
        }

        private bool Less(int x, int y)
        {
            return _comparer.Compare(_items[x], _items[y]) < 0;
        }

        private void Exch(int x, int y)
        {
            var swap = _items[x];
            _items[x] = _items[y];
            _items[y] = swap;
        }

        private void Resize(int capacity)
        {
            var temp = new TItem[capacity];

            for (var i = 1; i <= _count; i++)
                temp[i] = _items[i];

            _items = temp;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            var heap = new MaxHeap<TItem>(this);
            while (heap.Count > 0)
            {
                yield return heap.Pop();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}