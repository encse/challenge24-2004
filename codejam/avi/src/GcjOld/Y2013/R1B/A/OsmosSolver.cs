using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2013.R1B.A
{
    class OsmosSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int moteA;
            int c;
            Fetch(out moteA, out c);

            var cInsert = 0;
            var min = c;
            var rg = Fetch<int[]>();
            foreach(var vimote in rg.OrderBy(mote => mote).Select((v,i)=>new{v,i}))
            {
                for (; vimote.v >= moteA; )
                {
                    if(moteA == 1)
                    {
                        yield return min;
                        yield break;
                    }
                    cInsert++;
                    moteA += moteA - 1;
                }

                moteA += vimote.v;
                min = Math.Min(c - vimote.i - 1 + cInsert, min);
            }
            yield return min;
        }

    }
}
