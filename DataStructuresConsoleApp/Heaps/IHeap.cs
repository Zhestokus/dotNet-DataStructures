using System.Collections.Generic;

namespace DataStructuresConsoleApp.Heaps
{
    public interface IHeap<TItem> : IEnumerable<TItem>
    {
        int Count { get; }

        TItem Pop();

        TItem Peek();

        void Push(TItem item);
    }
}