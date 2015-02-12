namespace DataStructuresConsoleApp.RedBlack
{
    public class RedBlackNode<TKey, TValue>
    {
        public const bool RED = true;
        public const bool BLACK = false;

        public RedBlackNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Color = RED;
        }

        public RedBlackNode(TKey key, TValue value, bool color, int count)
        {
            Key = key;
            Value = value;
            Color = color;
            Count = count;
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public bool Color { get; set; }

        public int Count { get; set; }

        public RedBlackNode<TKey, TValue> Left { get; set; }

        public RedBlackNode<TKey, TValue> Right { get; set; }
    }

}
