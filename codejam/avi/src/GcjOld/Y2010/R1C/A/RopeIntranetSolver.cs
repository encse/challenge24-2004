using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.R1C.A
{
    internal class RopeIntranetSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var nrope = Fetch<int>();
            var rgrope = new List<int[]>();
            var ncross = 0;
            for(var irope = 0; irope < nrope; irope++)
            {
                var rope = Fetch<int[]>();
                ncross += rgrope.Count(ropeOther => (ropeOther[0] < rope[0]) != (ropeOther[1] < rope[1]));
                rgrope.Add(rope);
            }
            yield return ncross;
        }
    }
}
