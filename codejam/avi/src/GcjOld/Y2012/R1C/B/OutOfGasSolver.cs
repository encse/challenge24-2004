using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1C.B
{
    internal class OutOfGasSolver : GcjSolver
    {
        private class Pos
        {
            public decimal t;
            public decimal x;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            decimal xHome;
            int cpos;
            int ca;
            Fetch(out xHome, out cpos, out ca);

            var rgpos = new byte[cpos].Select(_ => Fetch<Pos>()).ToList();

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
                
                if(ipos==0)
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

            foreach(var a in Fetch<decimal[]>())
            {
                decimal vCar = 0;
                decimal xCar = 0;
                decimal t = 0;
                foreach(var vipos in rgpos.Select((v, i) => new {v, i}))
                {
                    var pos = vipos.v;
                    var dt = pos.t - t;
                    t = pos.t;

                    var xCarNew = xCar + vCar * dt + (decimal) 0.5 * a * dt * dt;
                    var vCarNew = vCar + dt * a;

                    if(xCarNew > pos.x)
                    {
                        xCarNew = pos.x;

                        dt = DtGet(vCar, a, xCarNew-xCar);
                        vCarNew = vCar + dt * a;
                    }

                    vCar = vCarNew;
                    xCar = xCarNew;
                }
                if(xCar < xHome)
                {
                    var dt = DtGet(vCar, a, xHome - xCar);

                    Debug.Assert(decimal.Round(xCar + vCar * dt + (decimal) 0.5 * a * dt * dt, 6) == xHome);
        
                    t += dt;
                }


                yield return Solwrt.NewLine;

                yield return t;
            }
        }

        private static decimal DtGet(decimal vCar, decimal a, decimal dx)
        {
            var aa = (decimal) 0.5 * a;
            var bb = vCar;
            var cc = -(dx);
            var sqrtpart = bb * bb - 4 * aa * cc;

            var x1 = (-bb + (decimal) Math.Sqrt((double) sqrtpart)) / (2 * aa);

            var x2 = (-bb - (decimal) Math.Sqrt((double) sqrtpart)) / (2 * aa);

            Debug.Assert(x1 == x2 || Math.Sign(x1) != Math.Sign(x2));

            var dt = Math.Max(x1, x2);

            //Debug.Assert(decimal.Round(vCar * dt + (decimal) 0.5 * a * dt * dt, 6) == dx);

            return dt;
        }
    }
}
