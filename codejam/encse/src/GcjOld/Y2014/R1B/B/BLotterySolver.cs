using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.B
{
    public class BLotterySolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int A, B, K;
            pparser.Fetch(out A, out B, out K);
            return () => Solve(A, B, K);
        }

        private IEnumerable<object> Solve(int A, int B, int K)
        {
            var mp = new Dictionary<Tuple<int, int, int>, long>();
            yield return SolveI(A - 1, B - 1, K - 1, mp);
        }


        private long SolveI(int a, int b, int k, Dictionary<Tuple<int, int, int>, long> mp)
        {
            var key = new Tuple<int, int, int>(a, b, k);
            if (mp.ContainsKey(key))
                return mp[key];

            long res = SolveII(a, b, k, mp);
            mp[key] = res;
            return res;
        }

        private long SolveII(int a, int b, int k, Dictionary<Tuple<int, int, int>, long> mp)
        {
            var a1 = (a%2 == 1 ? a : a - 1)/2;
            var b1 = (b%2 == 1 ? b : b - 1)/2;
            var k1 = (k%2 == 1 ? k : k - 1)/2;
            var a0 = a/2;
            var b0 = b/2;
            var k0 = k/2;

            if (a == 0 && b == 0)
                return 1;

            if (a == 0 && b > 0)
                return SolveI(a0, b0, k0, mp) + SolveI(a0, b1, k0, mp);

            if (a > 0 && b == 0)
                return SolveI(a0, b0, k0, mp) + SolveI(a1, b0, k0, mp);

            if (k == 0)
                return SolveI(a0, b0, k0, mp) + SolveI(a1, b0, k0, mp) + SolveI(a0, b1, k0, mp);

            return SolveI(a0, b0, k0, mp) + SolveI(a1, b0, k0, mp) + SolveI(a0, b1, k0, mp) + SolveI(a1, b1, k1, mp);
        }


        private static object Trivi(int A, int B, int K)
        {
            var cOk = 0;
            for (int i = 0; i < A; i++)
                for (int j = 0; j < B; j++)
                    if ((i & j) < K)
                        cOk++;

            return cOk;
        }
    }
}
