using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R1A.A
{
    internal class FreeCellStatisticsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            long max;
            long pToday;
            long pAllTime;
            Fetch(out max, out pToday, out pAllTime);

            var cGames = 100L;
            var cWins = pToday;

            if(cWins==0)
            {
                cGames = 0;
            }
            else
            {
                var lnko = Lnko(cGames, cWins);
                cGames /= lnko;
                cWins /= lnko;
            }

            if(cGames<=max)
            {
                if(pAllTime == pToday || (pAllTime != 0 && pAllTime != 100))
                {
                    yield return "Possible";
                    yield break;
                }

            }

            yield return "Broken";
        }

        private long Lnko(long a, long b)
        {
            Debug.Assert(a>0 && b>0);

            if(a==b)
                return a;
            if(a>b)
            {
                U.Swap(ref a, ref b);
            }

            return Lnko(b - a, a);
        }

    }
}
