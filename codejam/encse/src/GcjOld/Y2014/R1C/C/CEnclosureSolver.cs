using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1C.C
{
    public class CEnclosureSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int w, h, k;
            pparser.Fetch(out w, out h, out k);
            return () => Solve(w,h,k);
        }

        private IEnumerable<object> Solve(int W, int H, int k)
        {
            
            if (k == 1)
                yield return 1;
            else if (k == 2)
                yield return 2;
            else if (k == 3)
                yield return 3;
            else if (k == 4)
                yield return 4;
            else if (W < 3 || H < 3)
                yield return k;
            else
            {

                var Z = SolveI(new[] { new Pt(1, 0), new Pt(2, 1), new Pt(1, 2), new Pt(0, 1)}.ToList(), Math.Max(W,H), Math.Min(W,H), 5, k);
                yield return Z;

            }
        }

        private int SolveI(List<Pt> rgpt, int xLim, int yLim, int t, int k)
        {
            //Console.WriteLine("T: "+t);
            //
            if (t >= k)
            {
                Console.WriteLine("{0}x{1} k={2} needs {3} stone(s)", xLim, yLim, k, rgpt.Count);
                Console.WriteLine(Tsto(rgpt, xLim, yLim));
                return rgpt.Count;
            }
            var iptTop = IptTop(rgpt);
            var iptRight = IptRight(rgpt);
            var iptLeft = IptLeft(rgpt);
            var iptBottom = IptBottom(rgpt);
            
            List<Pt> rgptNew = null;
            var dtMax = 0;
            foreach (var qqq in new[]
                {
                    new {ipt = iptTop + 1, pt = rgpt[iptTop].PtRight()},
                    new {ipt = iptRight + 1, pt = rgpt[iptRight].PtBelow()},
                    new {ipt = iptBottom + 1, pt = rgpt[iptBottom].PtLeft()},
                    new {ipt = iptLeft + 1, pt = rgpt[iptLeft].PtAbove()},
                })

            {
                var ipt = qqq.ipt;
                var pt = qqq.pt;

                if (FInBounds(pt, xLim, yLim))
                {
                    int dtT;

                    var rgptT = Insert(rgpt, ipt, pt, xLim, yLim, out dtT);
                    if (dtT > dtMax)
                    {
                        rgptNew = rgptT;
                        dtMax = dtT;
                    }
                }
            }

            return SolveI(rgptNew, xLim, yLim, t + dtMax, k);
        }

        private string Tsto(List<Pt> rgpt, int xLim, int yLim)
        {
            var xMin = rgpt.Min(pt => pt.x);
            var xMax = rgpt.Max(pt => pt.x);
            var yMin = rgpt.Min(pt => pt.y);
            var yMax = rgpt.Max(pt => pt.y);

            var mx = new int[xLim, yLim];
            foreach (var pt in rgpt)
                mx[pt.x - xMin, pt.y - yMin] = 1;

            string st = "";
            for (int y = 0; y < yLim; y++)
            {
                for (int x = 0; x < xLim; x++)
                {
                    st += mx[x, y] == 1 ? "o" : ".";
                }
                st += "\n";
            }

            return st;
        }


        private List<Pt> Insert(List<Pt> rgpt, int ipt, Pt pt, int xLim, int yLim, out int dtT)
        {
            rgpt = new List<Pt>(rgpt);
            rgpt.Insert(ipt, pt);
            
            dtT = 1;
            Simit(rgpt, xLim, yLim, ref dtT);

            return rgpt;
        }

        private void Simit(List<Pt> rgpt, int xLim, int yLim, ref int dtT)
        {
            var hMax = yLim;
            var wMax = xLim;
        lStart:
            var xMin = rgpt.Min(pt => pt.x);
            var xMax = rgpt.Max(pt => pt.x);
            var yMin = rgpt.Min(pt => pt.y);
            var yMax = rgpt.Max(pt => pt.y);

            var midX = (xMin + xMax) / 2.0;
            var midY = (yMin + yMax) / 2.0;

            var w = xMax - xMin + 1;
            var h = yMax - yMin + 1;

            for (int i = 0; i < rgpt.Count; i++)
            {
                var iPrev = (i - 1 + rgpt.Count)%rgpt.Count;
                var iNext = (i + 1)%rgpt.Count;

                var ptPrev = rgpt[iPrev];
                var pt = rgpt[i];
                var ptNext = rgpt[iNext];
                if ((h < hMax || pt.y > yMin) && midY > pt.y && ptPrev.y.FIn(pt.y, pt.y - 1) && (ptNext.y == pt.y || ptNext.y == pt.y - 1)) //fel
                {

                    rgpt[i] = pt.PtAbove();
                    if (!FInBounds(rgpt[i], xLim, yLim))
                        Relocate(rgpt, 0, 1);
                    dtT++;
                    goto lStart;
                }
                
                if ( (h < hMax ||  pt.y < yMax ) && midY < pt.y && ptPrev.y.FIn(pt.y, pt.y + 1) && (ptNext.y == pt.y || ptNext.y == pt.y + 1)) // le
                {

                    rgpt[i] = pt.PtBelow();
                    if (!FInBounds(rgpt[i], xLim, yLim))
                        Relocate(rgpt, 0, -1);
                    dtT++;
                    goto lStart;
                }

                if ((w < wMax || pt.x > xMin) && midX > pt.x && ptPrev.x.FIn(pt.x, pt.x - 1) && (ptNext.x == pt.x || ptNext.x == pt.x - 1)) //balra
                {

                    rgpt[i] = pt.PtLeft();
                    if (!FInBounds(rgpt[i], xLim, yLim))
                        Relocate(rgpt, 1, 0);
                    dtT++;
                    goto lStart;
                }

                if ((w < wMax || pt.x < xMax) && midX < pt.x && ptPrev.x.FIn(pt.x, pt.x + 1) && (ptNext.x == pt.x || ptNext.x == pt.x + 1)) //jobbra
                {

                    rgpt[i] = pt.PtRight();
                    if (!FInBounds(rgpt[i], xLim, yLim))
                        Relocate(rgpt, -1, 0);
                    dtT++;
                    goto lStart;
                }
            }
        }

        private void Relocate(List<Pt> rgpt, int dx, int dy)
        {
            for (int i = 0; i < rgpt.Count; i++)
            {
                rgpt[i] = new Pt(rgpt[i].x + dx, rgpt[i].y + dy);
            }
        }

        private int IptTop(List<Pt> rgpt)
        {
            var yMin = rgpt.Min(pt => pt.y); 
            var iptMaxX = -1;
            for (int ipt = 0; ipt < rgpt.Count; ipt++)
            {
                if (rgpt[ipt].y == yMin && (iptMaxX == -1 || rgpt[ipt].x > rgpt[iptMaxX].x))
                    iptMaxX = ipt;
            }
            return iptMaxX;
        }
        private int IptLeft(List<Pt> rgpt)
        {
            var xMin = rgpt.Min(pt => pt.x);
            var iptMinY = -1;
            for (int ipt = 0; ipt < rgpt.Count; ipt++)
            {
                if (rgpt[ipt].x == xMin && (iptMinY == -1 || rgpt[ipt].y < rgpt[iptMinY].y))
                    iptMinY = ipt;
            }
            return iptMinY;
        }
        private int IptRight(List<Pt> rgpt)
        {
            var xMax = rgpt.Max(pt => pt.x);
            var iptMaxY = -1;
            for (int ipt = 0; ipt < rgpt.Count; ipt++)
            {
                if (rgpt[ipt].x == xMax && (iptMaxY == -1 || rgpt[ipt].y > rgpt[iptMaxY].y))
                    iptMaxY = ipt;
            }
            return iptMaxY;
        }
        private int IptBottom(List<Pt> rgpt)
        {
            var yMax = rgpt.Max(pt => pt.y);
            var iptMinX = -1;
            for (int ipt = 0; ipt < rgpt.Count; ipt++)
            {
                if (rgpt[ipt].y == yMax && (iptMinX == -1 || rgpt[ipt].x < rgpt[iptMinX].x))
                    iptMinX = ipt;
            }
            return iptMinX;
        }

        private bool FInBounds(Pt pt, int xLim, int yLim)
        {
            return pt.x >= 0 && pt.x < xLim && pt.y >= 0 && pt.y < yLim;
        }

        class Pt
        {
            public readonly int x, y;

            public Pt(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public Pt PtRight()
            {
                return new Pt(x + 1, y);
            }
            public Pt PtLeft()
            {
                return new Pt(x - 1, y);
            }
            public Pt PtAbove()
            {
                return new Pt(x, y-1);
            }
            public Pt PtBelow()
            {
                return new Pt(x, y+1);
            }
        }

    }
}
