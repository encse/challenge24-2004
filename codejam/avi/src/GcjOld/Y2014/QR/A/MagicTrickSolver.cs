using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.QR.A
{
    class MagicTrickSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var i1 = Fetch<int>()-1;
            var row = 4.Eni().Select(_ => Fetch<int[]>()).ToList()[i1];

            var i2 = Fetch<int>() - 1;
            var rg = 4.Eni().Select(_ => Fetch<int[]>()).ToList()[i2].Where(row.Contains).ToList();

            switch(rg.Count)
            {
                case 0:
                    yield return "Volunteer cheated!";
                    break;
                case 1:
                    yield return rg.Single();
                    break;
                default:
                    yield return "Bad magician!";
                    break;
            }
        }

    }
}
