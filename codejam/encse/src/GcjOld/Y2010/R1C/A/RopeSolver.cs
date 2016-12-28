using System.Collections.Generic;
using Gcj.Util;

namespace Gcj.Y2010.R1C.A
{
    public class RopeSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cwire = Pparser.Fetch<int>();
            var rgwpr = Pparser.FetchN<Wpr>(cwire);

            int cintersection = 0;
            for(int i=0;i<cwire;i++)
            {
                for(int j=0;j<i;j++)
                {
                    if (FIntersects(rgwpr[i], rgwpr[j]))
                        cintersection++;
                }
            }
            yield return cintersection;
        }

        bool FIntersects(Wpr wprA, Wpr wprB)
        {
            if (wprA.HeightLeft < wprB.HeightLeft && wprA.HeightRight > wprB.HeightRight)
                return true;
            if (wprA.HeightLeft > wprB.HeightLeft && wprA.HeightRight < wprB.HeightRight)
                return true;
            return false;
        }

        public class Wpr
        {
            public readonly int HeightLeft;
            public readonly int HeightRight;

            public Wpr(int heightLeft, int heightRight)
            {
                HeightLeft = heightLeft;
                HeightRight = heightRight;
            }
        }
    }
}
