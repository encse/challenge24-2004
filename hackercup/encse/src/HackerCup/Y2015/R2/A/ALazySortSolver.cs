using System.Collections.Generic;
using System.Globalization;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R2.A
{
    public class ALazySortSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var n = pparser.Fetch<int>();
            var rgnum = pparser.Fetch<int[]>();
            return () => Solve(rgnum);
        }

        private IEnumerable<object> Solve(int[] rgnum)
        {
            if (Solve1(rgnum[0], rgnum, 1, rgnum.Length - 1) || Solve1(rgnum[rgnum.Length-1], rgnum, 0, rgnum.Length - 2))
                yield return "yes";
            else
                yield return "no";
        }

        private bool Solve1(int init, int[] rgnum, int i, int j)
        {
            var top = init;
            var bottom = init;

            while (i <= j)
            {
                if (rgnum[i] == top + 1)
                {
                    top++;
                    i++;
                }
                else if (rgnum[i] == bottom - 1)
                {
                    bottom--;
                    i++;
                }
                else if (rgnum[j] == top + 1)
                {
                    top++;
                    j--;
                }
                else if (rgnum[j] == bottom - 1)
                {
                    bottom--;
                    j--;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
