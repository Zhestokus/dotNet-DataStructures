using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructuresConsoleApp.RedBlack
{
    public class RedBlackTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private enum TreeRotation
        {
            LeftRotation = 1,
            RightRotation = 2,
            RightLeftRotation = 3,
            LeftRightRotation = 4,
        }

        private readonly IComparer<TKey> _comparer;

        private RedBlackNode<TKey, TValue> _root;
        private int _count;

        public RedBlackTree()
            : this(Comparer<TKey>.Default)
        {
        }

        public RedBlackTree(IComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        public int Count
        {
            get { return _count; }
        }

        public TKey Min
        {
            get
            {
                var prev = _root;
                var node = _root;

                while (node != null)
                {
                    prev = node;
                    node = node.Left;
                }

                if (prev != null)
                    return prev.Key;

                return default(TKey);
            }
        }

        public TKey Max
        {
            get
            {
                var prev = _root;
                var node = _root;

                while (node != null)
                {
                    prev = node;
                    node = node.Right;
                }

                if (prev != null)
                    return prev.Key;

                return default(TKey);
            }
        }

        public IComparer<TKey> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (_root == null)
            {   // empty tree
                _root = new RedBlackNode<TKey, TValue>(key, value);
                _count = 1;

                return;
            }

            var current = _root;
            var parent = (RedBlackNode<TKey, TValue>)null;

            var grandParent = (RedBlackNode<TKey, TValue>)null;
            var greatGrandParent = (RedBlackNode<TKey, TValue>)null;

            var order = 0;

            while (current != null)
            {
                order = _comparer.Compare(key, current.Key);

                if (order == 0)
                {
                    _root.Color = RedBlackNode<TKey, TValue>.BLACK;

                    throw new Exception("Key exists");
                }

                // split a 4-node into two 2-nodes
                if (Is4Node(current))
                {
                    Split4Node(current);

                    // We could have introduced two consecutive red nodes after split. Fix that by rotation.
                    if (IsRed(parent))
                    {
                        parent = InsertionBalance(current, parent, grandParent, greatGrandParent);
                    }
                }

                greatGrandParent = grandParent;
                grandParent = parent;
                parent = current;

                current = (order < 0) ? current.Left : current.Right;
            }

            var node = new RedBlackNode<TKey, TValue>(key, value);
            if (order > 0)
            {
                parent.Right = node;
            }
            else
            {
                parent.Left = node;
            }

            // the new node will be red, so we will need to adjust the colors if parent node is also red
            if (parent.Color == RedBlackNode<TKey, TValue>.RED)
            {
                parent = InsertionBalance(node, parent, grandParent, greatGrandParent);
            }

            // Root node is always black
            _root.Color = RedBlackNode<TKey, TValue>.BLACK;

            _count++;
        }

        public bool Remove(TKey key)
        {
            if (_root == null)
            {
                return false;
            }

            var current = _root;

            var parent = (RedBlackNode<TKey, TValue>)null;
            var grandParent = (RedBlackNode<TKey, TValue>)null;

            var match = (RedBlackNode<TKey, TValue>)null;
            var parentOfMatch = (RedBlackNode<TKey, TValue>)null;

            var foundMatch = false;

            while (current != null)
            {
                if (Is2Node(current))
                {
                    if (parent == null)
                    {
                        current.Color = RedBlackNode<TKey, TValue>.RED;
                    }
                    else
                    {
                        var sibling = GetSibling(current, parent);
                        if (sibling.Color == RedBlackNode<TKey, TValue>.RED)
                        {
                            if (parent.Right == sibling)
                                RotateLeft(parent);
                            else
                                RotateRight(parent);

                            parent.Color = RedBlackNode<TKey, TValue>.RED;
                            sibling.Color = RedBlackNode<TKey, TValue>.BLACK;

                            ReplaceChildOfNodeOrRoot(grandParent, parent, sibling);

                            grandParent = sibling;
                            if (parent == match)
                            {
                                parentOfMatch = sibling;
                            }

                            // update sibling, this is necessary for following processing
                            sibling = (parent.Left == current) ? parent.Right : parent.Left;
                        }

                        if (Is2Node(sibling))
                        {
                            Merge2Nodes(parent, current, sibling);
                        }
                        else
                        {
                            var rotation = RotationNeeded(parent, current, sibling);

                            var newGrandParent = (RedBlackNode<TKey, TValue>)null;
                            switch (rotation)
                            {
                                case TreeRotation.RightRotation:
                                    sibling.Left.Color = RedBlackNode<TKey, TValue>.BLACK;
                                    newGrandParent = RotateRight(parent);
                                    break;
                                case TreeRotation.LeftRotation:
                                    sibling.Right.Color = RedBlackNode<TKey, TValue>.BLACK;
                                    newGrandParent = RotateLeft(parent);
                                    break;

                                case TreeRotation.RightLeftRotation:
                                    newGrandParent = RotateRightLeft(parent);
                                    break;
                                case TreeRotation.LeftRightRotation:
                                    newGrandParent = RotateLeftRight(parent);
                                    break;
                            }

                            newGrandParent.Color = parent.Color;

                            parent.Color = RedBlackNode<TKey, TValue>.BLACK;
                            current.Color = RedBlackNode<TKey, TValue>.RED;

                            ReplaceChildOfNodeOrRoot(grandParent, parent, newGrandParent);

                            if (parent == match)
                                parentOfMatch = newGrandParent;

                            grandParent = newGrandParent;
                        }
                    }
                }

                var order = foundMatch ? -1 : _comparer.Compare(key, current.Key);
                if (order == 0)
                {
                    // save the matching node
                    foundMatch = true;
                    match = current;
                    parentOfMatch = parent;
                }

                grandParent = parent;
                parent = current;

                current = (order < 0 ? current.Left : current.Right);
            }

            // move successor to the matching node position and replace links
            if (match != null)
            {
                ReplaceNode(match, parentOfMatch, parent, grandParent);
                _count--;
            }

            if (_root != null)
            {
                _root.Color = RedBlackNode<TKey, TValue>.BLACK;
            }

            return foundMatch;
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public bool Contains(TKey item)
        {
            return FindNode(item) != null;
        }

        public RedBlackNode<TKey, TValue> FindNode(TKey key)
        {
            var current = _root;

            while (current != null)
            {
                var order = _comparer.Compare(key, current.Key);
                if (order == 0)
                    return current;

                current = (order < 0) ? current.Left : current.Right;
            }

            return null;
        }
        public RedBlackNode<TKey, TValue> FindRange(TKey from, TKey to)
        {
            return FindRange(from, to, true, true);
        }
        public RedBlackNode<TKey, TValue> FindRange(TKey from, TKey to, bool lowerBoundActive, bool upperBoundActive)
        {
            var current = _root;

            while (current != null)
            {
                if (lowerBoundActive && _comparer.Compare(from, current.Key) > 0)
                {
                    current = current.Right;
                }
                else
                {
                    if (upperBoundActive && _comparer.Compare(to, current.Key) < 0)
                        current = current.Left;
                    else
                        return current;
                }
            }

            return null;
        }

        private RedBlackNode<TKey, TValue> GetSibling(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> parent)
        {
            return (parent.Left == node ? parent.Right : parent.Left);
        }
        private RedBlackNode<TKey, TValue> InsertionBalance(RedBlackNode<TKey, TValue> current, RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> grandParent, RedBlackNode<TKey, TValue> greatGrandParent)
        {
            var parentIsOnRight = (grandParent.Right == parent);
            var currentIsOnRight = (parent.Right == current);

            RedBlackNode<TKey, TValue> newChildOfGreatGrandParent;

            if (parentIsOnRight == currentIsOnRight)
            {
                newChildOfGreatGrandParent = currentIsOnRight ? RotateLeft(grandParent) : RotateRight(grandParent);
            }
            else
            {
                newChildOfGreatGrandParent = currentIsOnRight ? RotateLeftRight(grandParent) : RotateRightLeft(grandParent);
                parent = greatGrandParent;
            }

            grandParent.Color = RedBlackNode<TKey, TValue>.RED;
            newChildOfGreatGrandParent.Color = RedBlackNode<TKey, TValue>.BLACK;

            ReplaceChildOfNodeOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);

            return parent;
        }

        private bool Is2Node(RedBlackNode<TKey, TValue> node)
        {
            return IsBlack(node) && IsNullOrBlack(node.Left) && IsNullOrBlack(node.Right);
        }
        private bool Is4Node(RedBlackNode<TKey, TValue> node)
        {
            return IsRed(node.Left) && IsRed(node.Right);
        }

        private bool IsRed(RedBlackNode<TKey, TValue> node)
        {
            return (node != null && node.Color == RedBlackNode<TKey, TValue>.RED);
        }
        private bool IsBlack(RedBlackNode<TKey, TValue> node)
        {
            return (node != null && node.Color == RedBlackNode<TKey, TValue>.BLACK);
        }
        private bool IsNullOrBlack(RedBlackNode<TKey, TValue> node)
        {
            return (node == null || node.Color == RedBlackNode<TKey, TValue>.BLACK);
        }

        private void Split4Node(RedBlackNode<TKey, TValue> node)
        {
            node.Color = RedBlackNode<TKey, TValue>.RED;
            node.Left.Color = RedBlackNode<TKey, TValue>.BLACK;
            node.Right.Color = RedBlackNode<TKey, TValue>.BLACK;
        }

        private void Merge2Nodes(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> child1, RedBlackNode<TKey, TValue> child2)
        {
            parent.Color = RedBlackNode<TKey, TValue>.BLACK;

            child1.Color = RedBlackNode<TKey, TValue>.RED;
            child2.Color = RedBlackNode<TKey, TValue>.RED;
        }
        private void ReplaceNode(RedBlackNode<TKey, TValue> match, RedBlackNode<TKey, TValue> parentOfMatch, RedBlackNode<TKey, TValue> succesor, RedBlackNode<TKey, TValue> parentOfSuccesor)
        {
            if (succesor == match)
            {
                succesor = match.Left;
            }
            else
            {
                if (succesor.Right != null)
                    succesor.Right.Color = RedBlackNode<TKey, TValue>.BLACK;

                if (parentOfSuccesor != match)
                {
                    parentOfSuccesor.Left = succesor.Right;
                    succesor.Right = match.Right;
                }

                succesor.Left = match.Left;
            }

            if (succesor != null)
                succesor.Color = match.Color;

            ReplaceChildOfNodeOrRoot(parentOfMatch, match, succesor);
        }

        private void ReplaceChildOfNodeOrRoot(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> child, RedBlackNode<TKey, TValue> newChild)
        {
            if (parent != null)
            {
                if (parent.Left == child)
                {
                    parent.Left = newChild;
                }
                else
                {
                    parent.Right = newChild;
                }
            }
            else
            {
                _root = newChild;
            }
        }

        private RedBlackNode<TKey, TValue> RotateLeft(RedBlackNode<TKey, TValue> node)
        {
            var x = node.Right;

            node.Right = x.Left;
            x.Left = node;

            return x;
        }
        private RedBlackNode<TKey, TValue> RotateRight(RedBlackNode<TKey, TValue> node)
        {
            var x = node.Left;

            node.Left = x.Right;
            x.Right = node;

            return x;
        }

        private RedBlackNode<TKey, TValue> RotateLeftRight(RedBlackNode<TKey, TValue> node)
        {
            var child = node.Left;
            var grandChild = child.Right;

            node.Left = grandChild.Right;
            grandChild.Right = node;
            child.Right = grandChild.Left;
            grandChild.Left = child;
            return grandChild;
        }
        private RedBlackNode<TKey, TValue> RotateRightLeft(RedBlackNode<TKey, TValue> node)
        {
            var child = node.Right;
            var grandChild = child.Left;

            node.Right = grandChild.Left;
            grandChild.Left = node;
            child.Left = grandChild.Right;
            grandChild.Right = child;
            return grandChild;
        }

        private TreeRotation RotationNeeded(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> current, RedBlackNode<TKey, TValue> sibling)
        {
            if (IsRed(sibling.Left))
            {
                if (parent.Left == current)
                    return TreeRotation.RightLeftRotation;

                return TreeRotation.RightRotation;
            }

            if (parent.Left == current)
                return TreeRotation.LeftRotation;

            return TreeRotation.LeftRightRotation;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var collection = InOrderTraversal();
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
        {
            foreach (var node in PreOrderTraversal(_root))
            {
                if (node != null)
                    yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }
        private IEnumerable<RedBlackNode<TKey, TValue>> PreOrderTraversal(RedBlackNode<TKey, TValue> node)
        {
            var stack = new Stack<RedBlackNode<TKey, TValue>>();
            while (stack.Count > 0 || node != null)
            {
                if (node != null)
                {
                    yield return node;

                    if (node.Right != null)
                        stack.Push(node.Right);

                    node = node.Left;
                }
                else
                    node = stack.Pop();
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
        {
            /*
             iterativeInorder(node)
                  parentStack = empty stack
                  while (not parentStack.isEmpty() or node ≠ null)
                    if (node ≠ null)
                      parentStack.push(node)
                      node = node.left
                    else
                      node = parentStack.pop()
                      visit(node)
                      node = node.right
             
             */

            foreach (var node in InOrderTraversal(_root))
            {
                if (node != null)
                    yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }
        private IEnumerable<RedBlackNode<TKey, TValue>> InOrderTraversal(RedBlackNode<TKey, TValue> node)
        {
            var stack = new Stack<RedBlackNode<TKey, TValue>>();
            while (stack.Count > 0 || node != null)
            {
                if (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                else
                {
                    node = stack.Pop();

                    yield return node;

                    node = node.Left;
                }
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
        {
            foreach (var node in PostOrderTraversal(_root))
            {
                if (node != null)
                    yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }
        private IEnumerable<RedBlackNode<TKey, TValue>> PostOrderTraversal(RedBlackNode<TKey, TValue> node)
        {
            var stack = new Stack<RedBlackNode<TKey, TValue>>();
            RedBlackNode<TKey, TValue> lastVisited = null;

            while (stack.Count > 0 || node != null)
            {
                if (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                else
                {
                    var p = stack.Peek();
                    if (p.Right != null && lastVisited != p.Right)
                        node = p.Right;
                    else
                    {
                        yield return p;
                        lastVisited = stack.Pop();
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal()
        {
            foreach (var node in LevelOrderTraversal(_root))
            {
                if (node != null)
                    yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }
        private IEnumerable<RedBlackNode<TKey, TValue>> LevelOrderTraversal(RedBlackNode<TKey, TValue> node)
        {
            var stack = new Stack<RedBlackNode<TKey, TValue>>();
            stack.Push(node);

            while (stack.Count > 0 || node != null)
            {
                node = stack.Pop();
                yield return node;

                if (node.Left != null)
                    stack.Push(node.Left);

                if (node.Right != null)
                    stack.Push(node.Right);
            }
        }
    }
}
