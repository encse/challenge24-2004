using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.QR.A
{
    public class StoreCreditSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var credit = Pparser.Fetch<int>();
            var citem = Pparser.Fetch<int>();
            var rgprice = Pparser.Fetch<int[]>();
            for(int i=0;i<rgprice.Length;i++)
            {
                for(int j=0;j<i;j++)
                {
                    if(rgprice[i]+rgprice[j] == credit)
                    {
                        yield return j + 1;
                        yield return i + 1;
                    }
                }
            }
        }
    }
}
