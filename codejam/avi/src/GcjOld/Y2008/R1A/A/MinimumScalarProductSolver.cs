using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2008.R1A.A
{
    internal class MinimumScalarProductSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            Fetch<int>();
            yield return new LinkedList<long>(Fetch<long[]>().OrderBy(n => n)).Zip(new LinkedList<long>(Fetch<long[]>().OrderByDescending(n => n)), (n1, n2) => n1 * n2).Sum();
        }
    }
}
