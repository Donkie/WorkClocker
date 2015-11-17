using System;
using System.Collections;

namespace WorkClocker.Helpers
{
    public static class ListExtension
    {
        public static void BubbleSort(this IList o)
        {
            for (var i = o.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = o[j - 1];
                    var o2 = o[j];

                    if (((IComparable)o1).CompareTo(o2) <= 0)
                        continue;

                    o.Remove(o1);
                    o.Insert(j, o1);
                }
            }
        }
    }
}
