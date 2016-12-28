using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1C.B
{
    internal class OutOfGasConcurrentSolver2 : IConcurrentSolver
    {
        private class Pos
        {
            public decimal t;
            public decimal x;
        }

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            decimal xHome;
            int cpos;
            int ca;
            pparser.Fetch(out xHome, out cpos, out ca);

            var rgpos = new byte[cpos].Select(_ => pparser.Fetch<Pos>()).ToList();
            var rga = pparser.Fetch<decimal[]>();

            return () => Run(rga, rgpos, cpos, xHome);
        }

        private static IEnumerable<object> Run(decimal[] rga, List<Pos> rgpos, int cpos, decimal xHome)
        {
            for(var ipos = 0; ipos < cpos; ipos++)
            {
                var pos = rgpos[ipos];
                if(pos.x == xHome)
                {
                    rgpos = rgpos.GetRange(0, ipos + 1);
                    break;
                }
                if(pos.x <= xHome)
                    continue;

                if(ipos == 0)
                {
                    rgpos = new List<Pos>();
                    break;
                }

                var posPrev = rgpos[ipos - 1];

                pos.t = (pos.t - posPrev.t) / (pos.x - posPrev.x) * (xHome - posPrev.x) + posPrev.t;

                pos.x = xHome;

                rgpos = rgpos.GetRange(0, ipos + 1);
                break;
            }

            Debug.Assert(!rgpos.Any() || rgpos.Last().x == xHome);

            foreach(var a in rga)
            {
                var fCheck = new Func<decimal, bool>(tWait => !(
                    from pos in rgpos
                    let dt = pos.t - tWait
                    where dt > 0
                    let xCar = (decimal) 0.5 * a * dt * dt
                    where xCar > pos.x
                    select pos)
                    .Any());

                decimal tStart = 0;

                if(!fCheck(tStart))
                    tStart = U.BinsearchDecimal(fCheck, tStart, 1, (decimal) 0.0000001);

                yield return Solwrt.NewLine;
                yield return tStart + U.BinsearchDecimal(tTotal => (decimal) 0.5 * a * tTotal * tTotal < xHome, 0, 1, (decimal) 0.0000001);
            }
        }
    }
}
