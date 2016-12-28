using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.QR.B
{
    public class CookieClickerSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            decimal c, f,x;
            pparser.Fetch(out c, out f, out x);
            return () => Solve(c,f,x);
        }

        private IEnumerable<object> Solve(decimal priceFarm, decimal cpsPerFarm, decimal cookieDst)
        {
            // ha nem veszünk farmot
            decimal cps = 2; 
            var tMin = cookieDst / cps;

            decimal tSpent = 0;
            for(int i=0;;i++)
            {
                //ha még egy farmot hozzáveszünk
                var cps1 = cps + cpsPerFarm;
                var dt = priceFarm/cps + cookieDst/cps1;

                //akkor összesen ennyi idő kéne hozzá
                var ttotal = tSpent + dt;
                if(ttotal > tMin)
                    break;

                //megéri még 1-et venni
                tMin = ttotal;
                //írjuk fel hogy a farmvásárlás részhez mennyi idő kell, és most mennyi a sebesség
                tSpent += priceFarm/cps;
                cps = cps1;
            }

            yield return tMin;
        }

    }
}
