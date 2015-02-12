using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructuresConsoleApp.Heaps
{
    public class MinHeap<TItem> : IHeap<TItem>
    {
        private readonly IComparer<TItem> _comparer;

        private TItem[] _items;
        private int _count;

        public MinHeap(int initCapacity)
        {
            _items = new TItem[initCapacity + 1];
            _count = 0;
        }

        public MinHeap()
            : this(1)
        {
        }

        public MinHeap(IComparer<TItem> comparer)
            : this(1, comparer)
        {
        }
        public MinHeap(int capacity, IComparer<TItem> comparer)
        {
            _comparer = comparer;
            _items = new TItem[capacity + 1];
            _count = 0;
        }

        public MinHeap(IEnumerable<TItem> items)
            : this(items, Comparer<TItem>.Default)
        {
        }
        public MinHeap(IEnumerable<TItem> items, IComparer<TItem> comparer)
        {
            var list = new List<TItem>(items);

            _comparer = comparer;
            _count = list.Count;
            _items = new TItem[list.Count + 1];

            for (var i = 0; i < _count; i++)
                _items[i + 1] = list[i];

            for (var k = _count / 2; k >= 1; k--)
                Sink(k);
        }

        private MinHeap(MinHeap<TItem> other)
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

        private void Resize(int capacity)
        {
            var temp = new TItem[capacity];

            for (int i = 1; i <= _count; i++)
                temp[i] = _items[i];

            _items = temp;
        }

        public void Push(TItem item)
        {
            // double size of array if necessary
            if (_count == _items.Length - 1)
                Resize(2 * _items.Length);

            // add x, and percolate it up to maintain heap invariant
            _items[++_count] = item;
            Swim(_count);
        }

        public TItem Pop()
        {
            if (_count == 0)
                throw new KeyNotFoundException("Priority queue underflow");

            Exch(1, _count);

            var min = _items[_count--];

            Sink(1);

            _items[_count + 1] = default(TItem);         // avoid loitering and help with garbage collection

            if ((_count > 0) && (_count == (_items.Length - 1) / 4))
                Resize(_items.Length / 2);

            return min;
        }

        private void Swim(int index)
        {
            while (index > 1 && Greater(index / 2, index))
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

                if (j < _count && Greater(j, j + 1))
                    j++;

                if (!Greater(index, j))
                    break;

                Exch(index, j);
                index = j;
            }
        }

        private bool Greater(int x, int y)
        {
            return _comparer.Compare(_items[x], _items[y]) > 0;
        }

        private void Exch(int x, int y)
        {
            var swap = _items[x];
            _items[x] = _items[y];
            _items[y] = swap;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            var heap = new MinHeap<TItem>(this);
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
