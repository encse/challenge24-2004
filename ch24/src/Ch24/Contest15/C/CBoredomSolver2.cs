using System;
using System.Linq;
using System.Threading.Tasks;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest15.C
{
    //NCS
    class CBoredomSolver2 : Solver
    {
        public override void Solve()
        {
            var ctest = Fetch<int>();
            using (Output)
            {
                var rgdg = new Action[ctest];
                var rgout = new string[ctest];
                for (var itest = 0; itest < ctest; itest++)
                {
                    var i = itest;
                    var ccoord = Fetch<int>();
                    var rgx = Fetch<decimal[]>().OrderBy(x => x).ToArray();
                    var rgy = Fetch<decimal[]>().OrderBy(x => x).ToArray();
                    rgdg[i] =() =>
                    {
                        rgout[i] = Check(rgx, rgy, ccoord) ? "YES" : "NO";
                        Console.Write(".");
                    };
                }

                Parallel.Invoke(rgdg);

                foreach (var st in rgout)
                    WriteLine(st);
            }
        }


        private bool Check(decimal[] rgx, decimal[] rgy, int ccoord)
        {
            var x0 = rgx[0];
            var x1 = rgx[ccoord/2];
            var x2 = rgx[ccoord - 1];

            foreach (var rgicoord in Enumerable.Range(0, ccoord).EntChooseK(3).SelectMany(rgicoord => rgicoord.EntPermute()))
            {
                var y0 = rgy[rgicoord[0]];
                var y1 = rgy[rgicoord[1]];
                var y2 = rgy[rgicoord[2]];

                Pt ptCenter;
                decimal r2;
                if (Circle(new Pt(x0, y0), new Pt(x1, y1), new Pt(x2, y2), out ptCenter, out r2))
                {
                    if (FOnCircle(rgx, rgy, ptCenter, r2))
                        return true;
                }
            }

            return false;
        }

        private bool FOnCircle(decimal[] rgx, decimal[] rgy, Pt ptCenter, decimal r2)
        {
            var eps2 = new decimal(1.0e-8);
            var ccoord = rgx.Length;

            var oixU = Bsrc.Find(0, ccoord - 1, i => rgx[i] > ptCenter.X);
            var ixU = oixU ?? ccoord - 1;
            var ixL = ixU - 1;

            var iyL = 0;
            var iyU = ccoord -1;

            for (var i = 0; i < ccoord; i++)
            {
                //choose x an y so that
                //distance from pt.X is increasing
                //distance from pt.Y is decreasing
                decimal x;
                if (ixL == -1)
                {
                    x = rgx[ixU];
                    ixU++;
                }
                else if (ixU == ccoord)
                {
                    x = rgx[ixL];
                    ixL--;
                }
                else if ( (ptCenter.X - rgx[ixL]) < (rgx[ixU] - ptCenter.X))
                {
                    x = rgx[ixL];
                    ixL--;
                }
                else
                {
                    x = rgx[ixU];
                    ixU++;
                }

                decimal y;
                if (rgy[iyL] > ptCenter.Y)
                {
                    y = rgy[iyU];
                    iyU--;
                }
                else if (rgy[iyU] <= ptCenter.Y)
                {
                    y = rgy[iyL];
                    iyL++;
                }
                else if (ptCenter.Y - rgy[iyL] >= rgy[iyU] - ptCenter.Y)
                {
                    y = rgy[iyL];
                    iyL++;
                }
                else
                {
                    y = rgy[iyU];
                    iyU--;
                }

                if (Math.Abs((x - ptCenter.X) * (x - ptCenter.X) + (y - ptCenter.Y) * (y - ptCenter.Y) - r2) > eps2)
                    return false;
            }
            return true;
        }
  
        private readonly decimal eps = new decimal(1.0e-6) ;

        private bool Circle(Pt pt1, Pt pt2, Pt pt3, out Pt ptCenter, out decimal r2)
        {
            var det = (pt1.X - pt2.X)*(pt2.Y - pt3.Y) - (pt2.X - pt3.X) * (pt1.Y - pt2.Y);

            ptCenter = Pt.Nil;
            r2 = 0;

            if (Math.Abs(det) <= eps)
                return false;

            var t = pt2.X * pt2.X + pt2.Y * pt2.Y;
            var bc = (pt1.X * pt1.X + pt1.Y * pt1.Y - t) / 2;
            var cd = (t - pt3.X * pt3.X - pt3.Y * pt3.Y) / 2;

            var x = (bc*(pt2.Y - pt3.Y) - cd*(pt1.Y - pt2.Y))/det;
            var y = ((pt1.X - pt2.X)*cd - (pt2.X - pt3.X)*bc)/det;

            const long rLim = 1000000L; // r is less than 1000000

            var dx = Math.Abs(x - pt1.X);
            var dy = Math.Abs(y - pt1.Y);

            if (dx >= rLim || dy >= rLim) 
                return false;

            r2 = dx*dx + dy*dy;
            if (r2 > rLim*rLim)
                return false;

            ptCenter = new Pt(x, y);
            return true;
        }

        struct Pt
        {
            public readonly decimal X;
            public readonly decimal Y;
            public static readonly Pt Nil = new Pt(0,0);

            public Pt(decimal x, decimal y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
