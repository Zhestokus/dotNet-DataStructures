using System;
using System.Collections;

namespace DataStructuresConsoleApp.Search
{
    public static class BinarySearch
    {
        public static int Find(IList list, Object item)
        {
            return Find(list, item, 0, list.Count - 1);
        }
        public static int Find(IList list, Object item, int index, int count)
        {
            return Find(list, item, Comparer.Default, index, count);
        }

        public static int Find(IList list, Object item, IComparer comparer)
        {
            return Find(list, item, comparer, 0, list.Count - 1);
        }
        public static int Find(IList list, Object item, IComparer comparer, int index, int count)
        {
            var start = index;
            var end = index + count;

            var left = start;
            var right = end;

            while (left <= right)
            {
                var mid = left + (right - left) / 2;

                var order = comparer.Compare(item, list[mid]);
                if (order < 0)
                {
                    right = mid - 1;
                }
                else if (order > 0)
                {
                    left = mid + 1;
                }
                else
                {
                    return mid;
                }
            }

            return ~left;
        }

        public static int FindFirst(IList list, Object item)
        {
            return FindFirst(list, item, 0, list.Count - 1);
        }
        public static int FindFirst(IList list, Object item, int index, int count)
        {
            return FindFirst(list, item, Comparer.Default, index, count);
        }

        public static int FindFirst(IList list, Object item, IComparer comparer)
        {
            return FindFirst(list, item, comparer, 0, list.Count - 1);
        }
        public static int FindFirst(IList list, Object item, IComparer comparer, int index, int count)
        {
            var start = index;
            var end = index + count;

            var left = start;
            var right = end;

            while (left <= right)
            {
                var mid = left + (right - left) / 2;

                var order = comparer.Compare(item, list[mid]);
                if (order < 0)
                {
                    right = mid - 1;
                }
                else if (order > 0)
                {
                    left = mid + 1;
                }
                else
                {
                    if (mid == start)
                    {
                        return mid;
                    }

                    order = comparer.Compare(item, list[mid - 1]);
                    if (order != 0)
                    {
                        return mid;
                    }

                    right = mid - 1;
                }
            }

            return ~left;
        }

        public static int FindLast(IList list, Object item)
        {
            return FindLast(list, item, 0, list.Count - 1);
        }
        public static int FindLast(IList list, Object item, int index, int count)
        {
            return FindLast(list, item, Comparer.Default, index, count);
        }

        public static int FindLast(IList list, Object item, IComparer comparer)
        {
            return FindLast(list, item, comparer, 0, list.Count - 1);
        }
        public static int FindLast(IList list, Object item, IComparer comparer, int index, int count)
        {
            var start = index;
            var end = index + count;

            var left = start;
            var right = end;

            while (left <= right)
            {
                var mid = left + (right - left) / 2;

                var order = comparer.Compare(item, list[mid]);
                if (order < 0)
                {
                    right = mid - 1;
                }
                else if (order > 0)
                {
                    left = mid + 1;
                }
                else
                {
                    if (mid == end)
                    {
                        return mid;
                    }

                    order = comparer.Compare(item, list[mid + 1]);
                    if (order != 0)
                    {
                        return mid;
                    }

                    left = mid + 1;
                }
            }

            return ~left;
        }
    }
}
