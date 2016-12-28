using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.QR.A
{
    class StandingOvationSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int n;
            string st;
            Fetch(out n, out st);

            var cStanding = 0;
            var cPlus = 0;
            foreach(var vi in st.Select(ch => int.Parse(ch.ToString(CultureInfo.InvariantCulture))).Select((v,i) => new {v,i}))
            {
                if (cStanding < vi.i)
                {
                    cPlus += vi.i - cStanding;
                    cStanding = vi.i;
                }
                cStanding += vi.v;
            }

            yield return cPlus;
        }

    }
}
