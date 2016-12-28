using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R1.A
{
    public class AHomeworkSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var cache = new Dictionary<int, int>();
            int a, b, k;
            pparser.Fetch(out a, out b, out k);
            return () => Solve(a, b, k, cache);
        }

        private IEnumerable<object> Solve(int a, int b, int k, Dictionary<int, int> cache)
        {
            var res = 0;
            for (var i = a; i <= b; i++)
            {
                if (Primacity(i, 2, cache) == k)
                    res ++;
            }
            yield return res;
        }

        private int Primacity(int num, int iStart, Dictionary<int, int> cache)
        {
            if (num == 1)
                return 0;

            if (cache.ContainsKey(num))
                return cache[num];

            var primacity = 0;
            if (iStart == 2 )
            {
                if (num % 2 == 0)
                {
                    var numT = num;
                    while (numT % 2 == 0)
                        numT /= 2;

                    primacity = 1 + Primacity(numT, 3, cache);
                }
                else
                    primacity = Primacity(num, 3, cache);
            }
            else
            {
                var i = iStart;
                
                while (true)
                {
                    if (num%i == 0)
                    {
                        var numT = num;
                        while (numT%i == 0)
                            numT /= i;

                        primacity = 1 + Primacity(numT, i + 2, cache);
                        break;
                    }

                    i += 2;
                    if (i*i > num)
                    {
                        primacity = 1;
                        break;
                    }
                }
            }
            if(num < 2000000)
                 cache[num] = primacity;
            return primacity;
        }

    }
}
