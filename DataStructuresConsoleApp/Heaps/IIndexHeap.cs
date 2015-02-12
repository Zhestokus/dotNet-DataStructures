namespace DataStructuresConsoleApp.Heaps
{
    public interface IIndexHeap<TItem>
    {
        int Count { get; }

        int TopIndex { get; }

        TItem TopItem { get; }

        TItem this[int index] { get; set; }

        int Pop();

        void Push(int index, TItem item);

        void Increase(int index, TItem item);

        void Decrease(int index, TItem item);

        void Delete(int index);

        bool Contains(int index);
    }
}