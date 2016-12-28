using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.QR.B
{
    internal class ReverseWordsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            return Fetch<string[]>().Reverse().ToArray();
        }
    }
}
