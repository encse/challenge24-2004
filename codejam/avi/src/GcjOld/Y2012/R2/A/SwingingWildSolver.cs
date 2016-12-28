using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R2.A
{
    internal class SwingingWildSolver : GcjSolver
    {
        private class Vi
        {
            public decimal d;
            public decimal lMax;

            public decimal l = -1;

            public Vi(decimal d, decimal lMax)
            {
                this.d = d;
                this.lMax = lMax;
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cvi = Fetch<int>();
            var rgvi = new List<Vi>();
            for(var ivi = 0; ivi < cvi; ivi++)
                rgvi.Add(Fetch<Vi>());

            var fReach = new Func<Vi, decimal, bool>((vi, d) =>
            {
                Debug.Assert(vi.l != -1);
                return vi.d + vi.l >= d;
            });

            var dGoal = Fetch<decimal>();
            var fEnough = new Func<Vi, bool>(vi => fReach(vi, dGoal));

            var lGet = new Func<Vi, decimal, decimal>((vi, dPrev) =>
            {
                Debug.Assert(dPrev < vi.d);
                return Math.Min(vi.lMax, vi.d - dPrev);
            });

            rgvi.First().l = lGet(rgvi.First(), 0);

            if(fEnough(rgvi.First()))
            {
                yield return "YES";
                yield break;
            }

            var quvi = new Queue<Vi>();
            quvi.Enqueue(rgvi.First());

            foreach(var vi in rgvi.Skip(1))
            {
                for(;;)
                {
                    var viPrev = quvi.Peek();
                    if(fReach(viPrev, vi.d))
                    {
                        vi.l = lGet(vi, viPrev.d);

                        if(fEnough(vi))
                        {
                            yield return "YES";
                            yield break;
                        }

                        quvi.Enqueue(vi);
                        break;
                    }
                    quvi.Dequeue();
                    if(quvi.Count == 0)
                        break;
                }
                if(quvi.Count == 0)
                    break;
            }
            yield return "NO";
            yield break;
        }
    }
}
