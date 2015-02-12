namespace DataStructuresConsoleApp.Terany
{
    public class TeranyNode<TKey, TValue>
    {
        private byte _byte;
        private bool _leaf;

        private TKey _key;
        private TValue _value;

        private TeranyNode<TKey, TValue> _left;
        private TeranyNode<TKey, TValue> _middle;
        private TeranyNode<TKey, TValue> _right;

        public TeranyNode()
            : this(0)
        {
        }

        public TeranyNode(byte @byte)
            : this(@byte, false, default(TKey), default(TValue))
        {
        }

        public TeranyNode(byte @byte, bool leaf, TKey key, TValue value)
        {
            _byte = @byte;
            _leaf = leaf;

            _key = key;
            _value = value;
        }

        public byte Byte
        {
            get { return _byte; }
            set { _byte = value; }
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

        public TeranyNode<TKey, TValue> Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public TeranyNode<TKey, TValue> Middle
        {
            get { return _middle; }
            set { _middle = value; }
        }

        public TeranyNode<TKey, TValue> Right
        {
            get { return _right; }
            set { _right = value; }
        }
    }

}
