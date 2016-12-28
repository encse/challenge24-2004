using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.QR.A
{
    public class ACookingSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            return () => Solve(pparser.Fetch<string>());
        }

        private static IEnumerable<string> Solve(string stN)
        {
            var max = int.MinValue;
            var min = int.MaxValue;
            for(var i =0; i < stN.Length;i++)
            {
                for (var j = i; j < stN.Length; j++)
                {
                    var chI = stN[i];
                    var chJ = stN[j];
                    
                    if(i == 0 && i != j && chJ == '0') continue;
                    
                    var chars = stN.ToCharArray();
                    chars[i] = chJ;
                    chars[j] = chI;
                    
                    var n = int.Parse(new string(chars));
                    max = Math.Max(max, n);
                    min = Math.Min(min, n);
                }
            }
            yield return "{0} {1}".StFormat(min, max);
        }
    }
}
