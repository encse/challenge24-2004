using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest05.B
{
    class BarbarianInvasionSolver : Solver
    {
      
        public override void Solve()
        {
        
            using(Output)
            {
                var pparser = new Pparser(DpatIn + "B" + IdProblem + ".in");
                for (var cCity = pparser.Fetch<int>(); cCity != 0; cCity = pparser.Fetch<int>())
                {
                    var rgcoord = pparser.FetchN<Coord>(cCity);

                    var keruletMin = double.MaxValue;
                    for (int iCity = 0; iCity < cCity; iCity++)
                    {
                        var kerulet = Kerulet(Convexhull.convexhull(RgpointFromRgcoordSkipICity(rgcoord, iCity)));
                        if (kerulet < keruletMin)
                            keruletMin = kerulet;
                    }

                    Output.WriteLine((int)keruletMin);
                }
            }
        }

        private Point[] RgpointFromRgcoordSkipICity(IEnumerable<Coord> rgcoord, int iCitySkip)
        {
            return rgcoord.Where((t, iCity) => iCity != iCitySkip).Select(t => new Point(t.X, t.Y)).ToArray();
        }

        private double Kerulet(Point[] rgpoint)
        {
            double k = 0;
            for(int i=0;i<rgpoint.Length;i++)
            {
                var v = rgpoint[i];
                var w = rgpoint[(i + 1)%rgpoint.Length];
                var d = new Point(v.x - w.x, v.y - w.y);
                k += Math.Sqrt(d.x*d.x + d.y*d.y);
            }
            return k;
        }
    }

    public class Coord
    {
        public readonly int X;
        public readonly int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
