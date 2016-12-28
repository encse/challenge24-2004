using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1B.B
{
    internal class TideGoesInTideGoesOutSolver : GcjSolver
    {
        private class Cave
        {
            public int x;
            public int y;
            public int plafon;
            public int padlo;
        }


        protected override IEnumerable<object> EnobjSolveCase()
        {
            decimal hStart;
            int ymax;
            int xmax;
            Fetch(out hStart, out ymax, out xmax);

            var mcave = new Cave[xmax,ymax];

            for(var y = 0; y < ymax; y++)
            {
                var rgplafon = Fetch<int[]>();
                for(var x = 0; x < xmax; x++)
                {
                    mcave[x, y] = new Cave {x = x, y = y, plafon = rgplafon[x]};
                }
            }

            for(var y = 0; y < ymax; y++)
            {
                var rgpadlo = Fetch<int[]>();
                for(var x = 0; x < xmax; x++)
                {
                    mcave[x, y].padlo = rgpadlo[x];
                }
            }

            var caveStart = mcave[0, 0];
            var caveEnd = mcave[xmax - 1, ymax - 1];

            yield return new Astar<Cave, decimal>(
                new[] {new Tuple<Cave, decimal>(caveStart, 0)},
                cave => cave == caveEnd,
                (caveFrom, t) =>
                    from d in new[]
                    {
                        new {x = 1, y = 0},
                        new {x = -1, y = 0},
                        new {x = 0, y = 1},
                        new {x = 0, y = -1},
                    }
                    let x = caveFrom.x + d.x
                    where 0 <= x && x < xmax
                    let y = caveFrom.y + d.y
                    where 0 <= y && y < ymax
                    select mcave[x, y],
                (caveFrom, t, caveTo) =>
                {
                    if(caveTo.plafon - caveTo.padlo < 50 
                        || caveFrom.plafon - caveTo.padlo < 50 
                        || caveTo.plafon - caveFrom.padlo < 50)
                        return decimal.MaxValue;

                    var h = Math.Max(0, hStart - t * 10);
                    Debug.Assert(caveFrom.plafon - h >= 50);
                    if(caveTo.plafon - h < 50)
                    {
                        var hOk = caveTo.plafon - 50;
                        t += (h - hOk) / 10;
                        h = hOk;
                    }
                    Debug.Assert(caveTo.plafon - h >= 50);

                    if(t == 0)
                    {
                        Debug.Assert(h == hStart);
                        t += 0;
                    }
                    else if(h - caveFrom.padlo < 20)
                        t += 10;
                    else
                        t += 1;
                    return t;
                }
                ).Find().Item2;
        }
    }
}
