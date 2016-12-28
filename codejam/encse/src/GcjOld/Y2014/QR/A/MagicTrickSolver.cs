using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.QR.A
{
    public class MagicTrickSolver : IConcurrentSolver
    {

        class Step
        {
            public int irow { get; set; }
            public List<int[]> rows { get; set; }

            public Step(int irow, List<int[]> rows)
            {
                this.irow = irow;
                this.rows = rows;
            }
        }
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var stepA = new Step(pparser.Fetch<int>(), pparser.FetchN<int[]>(4));
            var stepB = new Step(pparser.Fetch<int>(), pparser.FetchN<int[]>(4));

            return () => Solve(stepA, stepB);
        }

        private IEnumerable<object> Solve(Step stepA, Step stepB)
        {
            var rowA = stepA.rows[stepA.irow-1];
            var rowB = stepB.rows[stepB.irow-1];

            var rgc = rowA.Intersect(rowB).ToArray();
            if (rgc.Length == 1)
                yield return rgc[0];
            else if (rgc.Length == 0)
                yield return "Volunteer cheated!";
            else
                yield return "Bad magician!";
        }
    }
}
