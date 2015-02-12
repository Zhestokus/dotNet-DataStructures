using System;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.Heaps
{
    public class IndexMaxHeap<TItem> : IIndexHeap<TItem>
    {
        private readonly IComparer<TItem> _comparer;

        private readonly TItem[] _items;      // keys[i] = priority of i

        private readonly int[] _binaryIndices;        // binary heap using 1-based indexing
        private readonly int[] _inversIndices;        // inverse of pq - qp[pq[i]] = pq[qp[i]] = i

        private readonly int _capacity;        // maximum number of elements on PQ

        private int _count;           // number of elements on PQ

        public IndexMaxHeap(int capacity)
            : this(capacity, Comparer<TItem>.Default)
        {
        }

        public IndexMaxHeap(IComparer<TItem> comparer)
            : this(1, comparer)
        {
        }

        public IndexMaxHeap(int capacity, IComparer<TItem> comparer)
        {
            if (capacity < 0)
                throw new ArgumentException();

            _comparer = comparer;
            _capacity = capacity;

            _items = new TItem[capacity + 1];    // make this of length NMAX??
            _binaryIndices = new int[capacity + 1];
            _inversIndices = new int[capacity + 1];         // make this of length NMAX??

            for (int i = 0; i <= capacity; i++)
                _inversIndices[i] = -1;
        }

        public int Count
        {
            get { return _count; }
        }

        public int TopIndex
        {
            get
            {
                if (_count == 0)
                    throw new Exception("Priority queue underflow");

                return _binaryIndices[1];
            }
        }

        public TItem TopItem
        {
            get
            {
                if (_count == 0)
                    throw new Exception("Priority queue underflow");

                return _items[_binaryIndices[1]];
            }
        }

        public TItem this[int index]
        {
            get
            {
                if (index < 0 || index >= _capacity)
                    throw new IndexOutOfRangeException();

                if (!Contains(index))
                    throw new Exception("index is not in the priority queue");

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _capacity)
                    throw new IndexOutOfRangeException();

                if (!Contains(index))
                    throw new Exception("index is not in the priority queue");

                _items[index] = value;

                Swim(_inversIndices[index]);
                Sink(_inversIndices[index]);
            }
        }

        public int Pop()
        {
            if (_count == 0)
                throw new Exception("Priority queue underflow");

            var min = _binaryIndices[1];

            Exch(1, _count--);
            Sink(1);

            _inversIndices[min] = -1;            // delete
            _items[_binaryIndices[_count + 1]] = default(TItem);    // to help with garbage collection
            _binaryIndices[_count + 1] = -1;            // not needed

            return min;
        }

        public void Push(int index, TItem item)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException();

            if (Contains(index))
                throw new ArgumentException("index is already in the priority queue");

            _count++;

            _inversIndices[index] = _count;
            _binaryIndices[_count] = index;
            _items[index] = item;

            Swim(_count);
        }

        public void Increase(int index, TItem item)
        {
            if (!Contains(index))
                throw new Exception("index is not in the priority queue");

            if (_comparer.Compare(_items[index], item) >= 0)
                throw new ArgumentException("Calling increaseKey() with given argument would not strictly increase the key");


            _items[index] = item;
            Swim(_inversIndices[index]);
        }

        public void Decrease(int index, TItem item)
        {
            if (!Contains(index))
                throw new Exception("index is not in the priority queue");

            if (_comparer.Compare(_items[index], item) <= 0)
                throw new ArgumentException("Calling decreaseKey() with given argument would not strictly decrease the key");

            _items[index] = item;
            Sink(_inversIndices[index]);
        }

        public void Delete(int index)
        {
            if (!Contains(index))
                throw new Exception("index is not in the priority queue");

            var i = _inversIndices[index];

            Exch(i, _count--);
            Swim(i);
            Sink(i);

            _items[i] = default(TItem);
            _inversIndices[i] = -1;
        }

        public bool Contains(int index)
        {
            return _inversIndices[index] != -1;
        }

        private bool Less(int x, int y)
        {
            return _comparer.Compare(_items[_binaryIndices[x]], _items[_binaryIndices[y]]) < 0;
        }

        private void Exch(int x, int y)
        {
            var swap = _binaryIndices[x];

            _binaryIndices[x] = _binaryIndices[y];
            _binaryIndices[y] = swap;

            _inversIndices[_binaryIndices[x]] = x;
            _inversIndices[_binaryIndices[y]] = y;
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
                int j = 2 * index;
                if (j < _count && Less(j, j + 1)) j++;
                if (!Less(index, j)) break;
                Exch(index, j);
                index = j;
            }
        }
    }
}