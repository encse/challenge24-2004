using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R1.C
{

    public class CWinningAtSportsSolver : IConcurrentSolver
    {
        const int mod = 1000000007;

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var points = pparser.StLineNext().Split('-').Select(int.Parse).ToArray();

            return () => Solve(points[0], points[1]);
        }

        private IEnumerable<object> Solve(int myPoints, int otherPoints)
        {
            yield return Compute(myPoints, otherPoints, true, new Dictionary<Tuple<int, int>, int>());
            yield return Compute(Math.Min(myPoints, otherPoints), otherPoints, false, new Dictionary<Tuple<int, int>, int>());
        }

        
        private int Compute(int myPoints, int otherPoints, bool fStressFree, Dictionary<Tuple<int, int>,int> cache)
        {
            if (fStressFree && myPoints <= otherPoints)
                return 0;

            if (!fStressFree && myPoints > otherPoints)
                return 0;

            if (myPoints == 0 || otherPoints == 0)
                return 1;

            var key = new Tuple<int, int>(myPoints, otherPoints);
            if (!cache.ContainsKey(key))
            {
                cache[key] = (Compute(myPoints - 1, otherPoints, fStressFree, cache) +
                              Compute(myPoints, otherPoints - 1, fStressFree, cache)) % mod;
            }

            return cache[key];
        }

    }
}
