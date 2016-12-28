using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1A.C
{
    public class CProperShuffleSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
          //  Calibrate();

            return pparser.Fetch<int>();
        }

        private void Calibrate()
        {
            var r = new Random();

            var minta = 1000;
            var rgok = new int[minta][];
            var rgbad = new int[minta][];
            var diffMax = 0.0;

            for (int i = 0; i < minta; i++)
            {
                rgok[i] = Ok(r);
                rgbad[i] = Bad(r);
            }

            Console.WriteLine("x");
            var step = 100;

            for (int lo = 0; lo < 1000; lo += step)
            {
                for (int hi = 1000; hi >= lo + step; hi -= step)
                {
                    for (int z = 320; z < 430; z += 1)
                    {
                        var sOk = 0.0;
                        var sBad = 0.0;
                        for (int i = 0; i < minta; i++)
                        {
                            sOk += S(rgok[i], lo, hi, z);
                            sBad += S(rgbad[i], lo, hi, z);
                        }
                        sOk /= 1000;
                        sBad /= 1000;

                        var diff = Math.Abs(sOk - sBad);
                        Console.Write("\rdiff:{0}, sok: {1}, sbad: {2}, lo:{3}, hi:{4}, z:{5}   ", diff, sOk, sBad, lo, hi, z);
                        if (diff > diffMax)
                        {
                            diffMax = diff;
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private int[] Ok(Random r)
        {
            var N = 1000;
            var a = Enumerable.Range(0, 1000).ToArray();
            for (int k = 0; k < 1000; k++)
            {
                var p = r.Next(N - k) + k;
                var z = a[k];
                a[k] = a[p];
                a[p] = z;
            }
            return a;
        }

        private int[] Bad(Random r)
        {
            var N = 1000;
            var a = Enumerable.Range(0, 1000).ToArray();
            for (int k = 0; k < 1000; k++)
            {
                var p = r.Next(N);
                var z = a[k];
                a[k] = a[p];
                a[p] = z;
            }
            return a;
        }
    

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var n = pparser.Fetch<int>();
            var rgn = pparser.Fetch<int[]>().ToList();
            return () => Solve(rgn);
        }

        private IEnumerable<object> Solve(List<int> rgn)
        {
            var s = S(rgn.ToArray(), 0, 1000, 403);
            yield return s < (322.099 + 367.935) / 2 ? "GOOD" : "BAD";

        }

        private int S(int[] rgn, int lo, int hi, int z)
        {
            int s = 0;
            for (int i = lo; i < hi; i++)
            {
                var ii = Array.IndexOf(rgn, i);
                if (ii <= i  && ii >= i - z)
                    s++;
            }
            return s;
        }
    }
}
