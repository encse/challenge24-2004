using System;
using System.Collections.Generic;

namespace Cmn.Util
{
    public static class Seq
    {
        public static IEnumerable<int> Ints(int iStart, int c, int step = 1)
        {
            int iToBeReturned = iStart;
            for(int i=0;i<c;i++)
            {
                yield return iToBeReturned;
                iToBeReturned += step;
            }

        }

        public static T[] RangeGet<T>(this T[] rgt, int iStart, int c)
        {
            var rgtResult = new T[c];
            Array.Copy(rgt, iStart, rgtResult, 0, c);
            return rgtResult;
        }
    }
}
