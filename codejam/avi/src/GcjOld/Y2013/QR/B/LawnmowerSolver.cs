using System;
using System.Collections.Generic;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.QR.B
{
    internal class LawnmowerSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int ymax;
            int xmax;
            Fetch(out ymax, out xmax);

            var mh = new int[xmax,ymax];
            for(var y=0;y<ymax;y++)
            {
                var rgh = Fetch<int[]>();
                for(var x=0;x<xmax;x++)
                {
                    mh[x, y] = rgh[x];
                }
            }

            var rghmaxY = new int[ymax];
            var rghmaxX = new int[xmax];
            
            for(var y=0;y<ymax;y++)
            {
                for(var x=0;x<xmax;x++)
                {
                    var h = mh[x, y];
                    rghmaxY[y] = Math.Max(rghmaxY[y], h);
                    rghmaxX[x] = Math.Max(rghmaxX[x], h);
                }
            }

            for(var y=0;y<ymax;y++)
            {
                for(var x=0;x<xmax;x++)
                {
                    var h = mh[x, y];

                    if(h<Math.Min(rghmaxY[y],rghmaxX[x]))
                    {
                        yield return "NO";
                        yield break;;
                    }
                }
            }

            yield return "YES";

        }

    }
}
