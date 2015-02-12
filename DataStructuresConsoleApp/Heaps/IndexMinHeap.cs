using System;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.Heaps
{
    public class IndexMinHeap<TItem> : IIndexHeap<TItem>
    {
        private readonly IComparer<TItem> _comparer;

        private readonly TItem[] _items;   

        private readonly int[] _binaryIndices;        // binary heap using 1-based indexing
        private readonly int[] _inversIndices;        // inverse of pq - qp[pq[i]] = pq[qp[i]] = i

        private readonly int _capacity;

        private int _count;           

        public IndexMinHeap(int capacity)
            : this(capacity, Comparer<TItem>.Default)
        {
        }

        public IndexMinHeap(IComparer<TItem> comparer)
            : this(1, comparer)
        {
        }

        public IndexMinHeap(int capacity, IComparer<TItem> comparer)
        {
            if (capacity < 0)
                throw new ArgumentException();

            _comparer = comparer;
            _capacity = capacity;

            _items = new TItem[capacity + 1];

            _binaryIndices = new int[capacity + 1];
            _inversIndices = new int[capacity + 1];         

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

        public void Decrease(int index, TItem item)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException();

            if (!Contains(index))
                throw new Exception("index is not in the priority queue");

            if (Compare(_items[index], item) <= 0)
                throw new ArgumentException("Calling decreaseKey() with given argument would not strictly decrease the key");

            _items[index] = item;

            Swim(_inversIndices[index]);
        }

        public void Increase(int index, TItem item)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException();

            if (!Contains(index))
                throw new Exception("index is not in the priority queue");

            if (Compare(_items[index], item) >= 0)
                throw new ArgumentException("Calling increaseKey() with given argument would not strictly increase the key");

            _items[index] = item;

            Sink(_inversIndices[index]);
        }

        public void Delete(int index)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException();

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
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException();

            return _inversIndices[index] != -1;
        }

        private bool Greater(int x, int y)
        {
            return Compare(_items[_binaryIndices[x]], _items[_binaryIndices[y]]) > 0;
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
                int j = 2 * index;
                if (j < _count && Greater(j, j + 1))
                    j++;

                if (!Greater(index, j))
                    break;

                Exch(index, j);
                index = j;
            }
        }

        private int Compare(TItem x, TItem y)
        {
            return _comparer.Compare(x, y);
        }
    }
}