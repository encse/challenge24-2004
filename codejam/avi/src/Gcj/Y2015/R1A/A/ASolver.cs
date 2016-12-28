using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2015.R1A.A
{
    class ASolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            Fetch<int>();
            var rg = Fetch<int[]>();

            var c1 = 0;
            var m = 0;
            for(var i = 0; i < rg.Length-1; i++)
            {
                var n1 = rg[i];
                var n2 = rg[i+1];
                c1 += Math.Max(0, n1 - n2);
                m = Math.Max(m, n1 - n2);
            }

            var c2 = 0;
            for (var i = 0; i < rg.Length - 1; i++)
            {
                var n1 = rg[i];
                c2 += Math.Min(m, n1);
            }

            yield return c1;
            yield return c2;
        }

    }
}
