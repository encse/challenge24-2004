using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R2.A
{
    internal class AirportWalkwaysSolver : GcjSolver
    {
        private class Ww
        {
            public decimal l;
            public decimal v;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            decimal lAll;
            decimal vs;
            decimal vr;
            decimal tr;
            int cww;
            Fetch(out lAll, out vs, out vr, out tr, out cww);

            var rgww = new List<Ww>();
            var ls = lAll;
            for(var iww=0;iww<cww;iww++)
            {
                var rg = Fetch<decimal[]>();
                var l = rg[1] - rg[0];
                rgww.Add(new Ww{l = l, v = rg[2]+vs});
                ls -= l;
            }

            vr -= vs;

            rgww.Add(new Ww{l = ls, v = vs});

            var tAll = rgww.Sum(ww => ww.l / ww.v);

            foreach(var ww in rgww.OrderBy(ww => ww.v))
            {
                Debug.Assert(tr>=0);
                if(tr==0)
                    break;

                var trMax = ww.l / (ww.v + vr);

                var trww = Math.Min(tr, trMax);

                tr -= trww;

                tAll -= vr * trww / ww.v;
            }
            Debug.Assert(tr>=0);

            yield return tAll;
        }
    }
}
