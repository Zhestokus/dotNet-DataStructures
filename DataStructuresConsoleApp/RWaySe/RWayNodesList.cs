using System.Collections;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.RWaySe
{
    public class RWayNodesList<TKey, TValue> : IEnumerable<RWayNode<TKey, TValue>>
    {
        private const int Size = 16;

        private RWayNodesPage<TKey, TValue>[] _pages;

        public RWayNode<TKey, TValue> this[int index]
        {
            get { return ReadNode(index); }
            set { WriteNode(value, index); }
        }

        public IEnumerator<RWayNode<TKey, TValue>> GetEnumerator()
        {
            const int count = Size * Size;

            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private RWayNode<TKey, TValue> ReadNode(int index)
        {
            var page = InitPage(index);
            if (page == null)
                return null;

            var itemIndex = index % Size;

            return page[itemIndex];
        }

        private void WriteNode(RWayNode<TKey, TValue> node, int index)
        {
            var page = InitPage(index);
            var itemIndex = index % Size;

            page[itemIndex] = node;
        }

        private RWayNodesPage<TKey, TValue> InitPage(int index)
        {
            if (_pages == null)
                _pages = new RWayNodesPage<TKey, TValue>[Size];

            var pageIndex = index / Size;
            var page = _pages[pageIndex];

            if (page == null)
            {
                page = new RWayNodesPage<TKey, TValue>();
                _pages[pageIndex] = page;
            }

            return page;
        }
    }
}