using System;
using System.Collections.Generic;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1B.A
{
    internal class SafetyInNumbersSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            Output.NufDouble = "0.######";
            var rgscr = Fetch<int[]>().Skip(1).Select((v,i)=>new{v,i}).OrderBy(viscr => viscr.v).ToArray();
            double votLeft = rgscr.Sum(viscr => viscr.v);
            var votSum = votLeft;

            var rgvot = new double[rgscr.Length];

            for(var iscr=0;iscr<rgscr.Length;iscr++)
            {
                var scr = rgscr[iscr].v;
                var votSafe = votLeft/(iscr+1);
                if(iscr + 1 < rgscr.Length)
                {
                    var scrNext = rgscr[iscr + 1].v;
                    if(scr == scrNext)
                        continue;

                    votSafe = Math.Min(votSafe, scrNext-scr);
                }

                for(var ivot=0;ivot<=iscr;ivot++)
                    rgvot[ivot] += votSafe;

                votLeft -= votSafe * (iscr + 1);

                if(votLeft < 1)
                    break;
            }

            return rgvot.Select((v,i) => new {v,i}).OrderBy(vivot => rgscr[vivot.i].i).Select(vivot => 100 * vivot.v / votSum).Cast<object>();
        }
    }
}
