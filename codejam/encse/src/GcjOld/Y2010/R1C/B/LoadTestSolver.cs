using System.Collections.Generic;
using Gcj.Util;

namespace Gcj.Y2010.R1C.B
{
    public class LoadTestSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int l, p, c;
            Pparser.Fetch(out l, out p, out c);
            var rgmp = RgmpGet(l, p, c);
            int impLow = 0;
            int impHi = rgmp.Count - 1;

            int meres = 0;
            while(rgmp[impLow]*c < rgmp[impHi])
            {
                var imp = (impHi + impLow)/2;
                meres++;
                //amerre több a munka
                if(imp-impLow > impHi-imp)
                    impHi = imp;
                else
                    impLow = imp;
            }
            yield return meres;

        }


        List<long> RgmpGet(long l, long p, int c)
        {
            var rgmp = new List<long> { l };
            if (l == 0)
            {
                l++;
                rgmp.Add(l);
            }
            while(l*c <p)
            {
                l *= c;
                rgmp.Add(l);
            }
            rgmp.Add(p);
            return rgmp;
        }
    }

}
