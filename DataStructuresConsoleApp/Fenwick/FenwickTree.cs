using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructuresConsoleApp.Fenwick
{
    public class FenwickTree
    {
        private readonly int[] _tree;

        public FenwickTree(int size)
        {
            _tree = new int[size];
        }

        public FenwickTree(int[] source)
        {
            _tree = new int[source.Length];

            for (int i = 0; i < _tree.Length; i++)
                Increase(i, source[i]);

        }

        public int Length
        {
            get { return _tree.Length; }
        }

        public void Increase(int index, int value)
        {
            for (; index < _tree.Length; index |= index + 1)
                _tree[index] += value;
        }

        public int GetSum(int left, int right)
        {
            return Query(right) - Query(left - 1); //when left equals 0 the function hopefully returns 0
        }

        public int Query(int index)
        {
            int res = 0;

            while (index >= 0)
            {
                res += _tree[index];
                index &= index + 1;
                index--;
            }

            return res;
        }
    }
}
