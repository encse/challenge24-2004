using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.QR.A
{
    public class StandingOvationSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int l;
            string st;
            pparser.Fetch(out l, out st);
            return () => Solve(st.ToCharArray().Select(ch=>int.Parse(ch.ToString())).ToArray());
        }

        private IEnumerable<object> Solve(int[] rgs)
        {
            var res = 0;
            var standing = 0;
            for (var i = 0; i < rgs.Length; i++)
            {
                if (standing < i)
                {
                    res += i - standing;
                    standing = i;
                }
                standing += rgs[i];
            }
            yield return res;
        }
    }
}
