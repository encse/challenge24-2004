using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1C.A
{
    public class APartElfSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var st = pparser.StLineNext().Split('/');

            return () => Solve(BigInteger.Parse(st[0]), BigInteger.Parse(st[1]));
        }

        private IEnumerable<object> Solve(BigInteger a, BigInteger b)
        {
            long p = 1;
            long q = 1;

            var iFirst = -1;
            int i = 0;
            while (a > 0 && i <= 40)
            {
                if (b <= a * q)
                {
                    iFirst = iFirst == -1 ? i : iFirst;
                    // a/b - i/j  <=> (a*j - i*b) / b*j

                    var aT = a*q - p*b;
                    var bT = b*q;
                    var g = Gcd(aT, bT);
                    a = aT/g;
                    b = bT/g;
                }
                i++;

                q <<= 1;
            }
            if(iFirst == -1 || a != 0)
                yield return "impossible";
            else
                yield return iFirst;
        }

        private BigInteger Gcd(BigInteger a, BigInteger b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }
    }
}
