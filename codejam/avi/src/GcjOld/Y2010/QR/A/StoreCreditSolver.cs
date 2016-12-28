using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.QR.A
{
    internal class StoreCreditSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cred = Fetch<int>();
            var car = Fetch<int>();
            var rgar = Fetch<int[]>();

            var mpiarByAr = new Dictionary<int, int>();
            foreach(var xar in rgar.Select((ar, iar) => new {ar, iar = iar + 1}))
            {
                var arOther = cred - xar.ar;
                if(mpiarByAr.ContainsKey(arOther))
                {
                    return new object[] {mpiarByAr[arOther], xar.iar};
                }
                if(mpiarByAr.ContainsKey(xar.ar))
                    continue;
                mpiarByAr[xar.ar] = xar.iar;
            }
            throw new Exception();
        }
    }
}
