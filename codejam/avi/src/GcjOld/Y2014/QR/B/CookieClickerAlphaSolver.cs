using System;
using System.Collections.Generic;
using Gcj.Util;

namespace Gcj.Y2014.QR.B
{
    class CookieClickerAlphaSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            decimal costFarm;
            decimal cpsExtra;
            decimal costTarget;
            Fetch(out costFarm, out cpsExtra, out costTarget);

            decimal cps = 2;
            decimal sMin = costTarget / cps;

            decimal sFactory = 0;

            for(;;)
            {
                sFactory += costFarm / cps;

                if(sFactory >= sMin)
                    break;

                cps += cpsExtra;

                sMin = Math.Min(sMin, sFactory + costTarget / cps);
            }

            yield return sMin;
        }

    }
}
