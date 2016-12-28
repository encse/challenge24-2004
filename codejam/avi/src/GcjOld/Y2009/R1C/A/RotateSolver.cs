using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2009.R1C.A
{
    internal class AllYourBaseSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var radix = 0;
            var mpnByCh = new Dictionary<char, int>();
            var st = Fetch<string>();
            foreach(var ch in st)
            {
                if(mpnByCh.ContainsKey(ch))
                    continue;
                int n;
                if(radix==0)
                    n = 1;
                else if(radix==1)
                    n = 0;
                else
                    n = radix;
                radix++;
                mpnByCh[ch] = n;
            }
            if(radix==1)
                radix = 2;

            yield return st.Select(ch => mpnByCh[ch]).Aggregate<int, long>(0, (current, n) => current * radix + n);
        }
    }
}
