using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R2.B
{
    public class BAllCriticalSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var p = pparser.Fetch<decimal>();

            return () => Solve(p);
        }

        private IEnumerable<object> Solve(decimal p)
        {
            yield return E(p, 20).ToString("0.00000");
        }

        private decimal E(decimal p, int s)
        {
            if (s == 0)
                return 0;

            //E(5) = (choose(5,0)) * p^0 * (1-p)^5 * (1+ E(5)) + choose(5,1)p*(1-p)^4*E(4) + ... choose(5,5) * p^5 * E(0)
            //E(5) = (1-p)^5 + (1-p)^5 * E(5) + choose(5,1)p*(1-p)^4*E(4) + ... choose(5,5) * p^5 * E(0)
            //E(5) = (choose(5,1)p*(1-p)^4*E(4) + ... choose(5,5) * p^5 * E(0)) / (1-2*(1-p)^5)

            decimal res = 0;
            for (var i = 1; i <= s; i++)
                res +=  (1+E(p, s - i)) * Choose(s, i) *  Pow(p, i) * Pow(1 - p, s - i);
            res += Pow(1 - p, s);

            return res / (1 - Pow(1 - p, s));

        }

        private decimal Pow(decimal p, long i)
        {
            return (decimal) Math.Pow((double) p, i);
        }

        private long Choose(long n, long k)
        {
            if (k == 0 || k == n)
                return 1;

            return Choose(n - 1, k - 1) * n / k;
        }
    }
}
