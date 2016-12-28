using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2015.QR.B
{
    class InfiniteHouseOfPancakesSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var _ = Fetch<int>();
            var rgn = Fetch<int[]>().ToList();

            rgn.Sort();
            rgn.Reverse();

            var minSec = rgn.First();

            for(var target = rgn.First() - 2; target > 1; target-- )
            {
                var sec = target;

                foreach(var n in rgn)
                {
                    var nCut = (n - 1) / target;

                    if(nCut == 0)
                    {
                        break;
                    }

                    sec += nCut;
                    if(sec>=minSec)
                    {
                        break;
                    }
                }

                minSec = Math.Min(minSec, sec);
            }

            yield return minSec;
        }

    }
}
