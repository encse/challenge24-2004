using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R2.B
{
    internal class AerobicsSolver : GcjSolver
    {
        private class Qn
        {
            public decimal x;
            public decimal y;
            public decimal w;
            public decimal h;
        }

        private class C
        {
            public decimal r;
            public decimal x;
            public decimal y;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cCircle;
            var qt = new Qn {x = 0, y = 0};

            Fetch(out cCircle, out qt.w, out qt.h);
            var rgcOrig = Fetch<List<decimal>>().Select(r => new C {r = r}).ToList();
            var rgc = rgcOrig.OrderByDescending(c => c.r).ToList();

            var iCircle=0;
            for(;iCircle<rgc.Count;iCircle++)
            {
                var c = rgc[iCircle];
                var d = 2 * c.r;
                

                if(d > qt.w)
                {
                    Debug.Assert(d < qt.h);
                    c.x = qt.x + qt.w / 2;
                    c.y = qt.y + qt.h - d / 2;
                    qt.h -= d;
                    continue;
                }

                if(d > qt.h)
                {
                    c.x = qt.x + qt.w - d / 2;
                    c.y = qt.y + qt.h / 2;
                    qt.w -= d;
                    continue;
                }

                break;
            }

            var rgqn = new LinkedList<Qn>();
            rgqn.AddFirst(qt);

            for(;iCircle<rgc.Count;iCircle++)
            {
                var c = rgc[iCircle];
                var d = 2 * c.r;

                for(;;)
                {
                    var nqn = rgqn.First;
                    var qn = nqn.Value;
                    if(d > Math.Min(qn.w, qn.h))
                    {
                        rgqn.RemoveFirst();
                        continue;
                    }

                    c.x = qn.x + d / 2;
                    c.y= qn.y + d / 2;

                    rgqn.RemoveFirst();
                    if(qn.w < qn.h)
                    {
                        rgqn.AddFirst(new Qn {x = qn.x, y = qn.y + d, w = qn.w, h = qn.h - d});
                        rgqn.AddFirst(new Qn {x = qn.x + d, y = qn.y, w = qn.w - d, h = d});
                    }
                    else
                    {
                        rgqn.AddFirst(new Qn {x = qn.x + d, y = qn.y, w = qn.w - d, h = qn.h});
                        rgqn.AddFirst(new Qn {x = qn.x, y = qn.y + d, w = d, h = qn.h - d});
                    }

                    break;
                }
            }

            foreach(var c in rgcOrig)
            {
                yield return c.x;
                yield return c.y;
            }

            for(var i1 = 0;i1<rgc.Count;i1++)
            {
                for(var i2 = i1 + 2;i2<rgc.Count;i2++)
                {
                    var dx = rgc[i1].x - rgc[i2].x;
                    var dy = rgc[i1].y - rgc[i2].y;
                    var d2 = dx * dx + dy * dy;
                    Debug.Assert(d2 >= (rgc[i1].r+rgc[i2].r)*(rgc[i1].r+rgc[i2].r));
                }
            }
        }

    }
}
