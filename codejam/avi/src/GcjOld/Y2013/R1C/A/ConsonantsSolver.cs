using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1C.A
{
    class ConsonantsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            string st;
            decimal cbMin;
            Fetch(out st, out cbMin);

            decimal crng = 0;
            decimal crngGood = 0;
            decimal cb = 0;
            decimal cAll = 0;
            foreach(var ab in st.Select(ch => "aeiou".Contains(ch) ? 'a' : 'b'))
            {
                cb = ab == 'b' ? cb + 1 : 0;

                crng++;

                if (cb >= cbMin)
                    crngGood = crng - cbMin + 1;

                cAll += crngGood;
            }

            yield return cAll;
        }

    }
}
