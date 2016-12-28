using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.B
{
    internal class NewLotteryGameSmallSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int a;
            int b;
            int k;
            Fetch(out a, out b, out k);

            yield return a.Eni().SelectMany(a1 => b.Eni(), (a1, b1) => new {a1, b1}).Count(ab => (ab.a1 & ab.b1) < k);
        }

    }
}
