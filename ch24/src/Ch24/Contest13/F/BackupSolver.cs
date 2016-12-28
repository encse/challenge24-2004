using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.F
{
    class BackupSolver : Solver
    {
        private class Coord
        {
            public readonly double X;
            public readonly double Y;

            public Coord(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            var ccoord = pparser.Fetch<int>();
            var rgcoord = pparser.FetchN<Coord>(ccoord);
            var xAvg = rgcoord.Select(coord => coord.X).Average();
            var yAvg = rgcoord.Select(coord => coord.Y).Average();

            
            var coordCannon = new Coord(xAvg, yAvg);
            
            Coord coordPrev = null;
            do
            {
                coordPrev = coordCannon;
                coordCannon = new Coord(Foo(rgcoord, coordCannon, coord => coord.X),
                                        Foo(rgcoord, coordCannon, coord => coord.Y));
                log.InfoFormat("{0} {1}", coordCannon.X, coordCannon.Y);
            } while (Dist(coordCannon, coordPrev) > 0.000000001);
                
            using (var solwrt = new Solwrt(FpatOut))
            {
                solwrt.WriteLine("{0} {1}", coordCannon.X, coordCannon.Y);
            }
        }

        private double Foo(IEnumerable<Coord> rgcoord,  Coord coordCannon, Func<Coord, double> dg)
        {
            double sum = 0;
            double sumOf1PerD = 0;
            foreach (var coord in rgcoord)
            {
                var d = Dist(coordCannon, coord);
                sum += dg(coord) / d;
                sumOf1PerD += (1 / d);
            }
            return sum / sumOf1PerD;
        }

        private static double Dist(Coord coordCannon, Coord coord)
        {
            return Math.Sqrt((coordCannon.X - coord.X) * (coordCannon.X - coord.X) + (coordCannon.Y - coord.Y) * (coordCannon.Y - coord.Y));
        }
    }
}
