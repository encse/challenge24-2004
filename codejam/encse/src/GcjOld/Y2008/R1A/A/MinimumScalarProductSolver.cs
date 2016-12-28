using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2008.R1A.A
{
    public class MinimumScalarProductSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var n = Pparser.Fetch<int>();
            var rgA = Pparser.Fetch<long[]>().ToList();
            var rgB = Pparser.Fetch<long[]>().ToList();
            rgA.Sort();
            rgB.Sort();

            var m = 0L;
            for (int i = 0; i < n;i++ )
                m += rgA[i]*rgB[n-i-1];
            yield return m;
        }
    }
}
