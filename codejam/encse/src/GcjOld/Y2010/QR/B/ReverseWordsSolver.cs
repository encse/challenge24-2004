using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gcj.Util;

namespace Gcj.Y2010.QR.B
{
    public class ReverseWordsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            return Pparser.StLineNext().Split(' ').Reverse();
        }
    }
}
