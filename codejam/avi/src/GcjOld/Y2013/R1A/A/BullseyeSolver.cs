using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.R1A.A
{
    internal class BullseyeSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            BigInteger rStart;
            BigInteger tMax;
            Fetch(out rStart, out tMax);

            yield return U.BinsearchBiginteger(cCircle => (2 * rStart + 2*(cCircle - 1) + 1) * cCircle <= tMax, 1, 1, 2);
        }

    }
}
