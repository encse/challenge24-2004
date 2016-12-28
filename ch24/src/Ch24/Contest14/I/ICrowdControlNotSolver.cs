using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest14.I
{
    public class ICrowdControlNotSolver : Contest.Solver
    {
        private class Poly
        {
            public int i;
            public List<Vert> rgvert=new List<Vert>();

            public Point[] rgpoint(double scale)
            {
                return rgvert.Select(vert => new Point((int) (vert.x*scale),(int) (vert.y*scale))).ToArray();
            }
        }

        private class Vert
        {
            public double x;
            public double y;
        }

        public override void Solve()
        {
            var rgobe = Fetch<int[]>();

            var rgpoly = (rgobe[0] + 1).Eni().Select(ipoly => new Poly
            {
                i = ipoly, 
                rgvert = Fetch<int>().Eni().Select(ivert =>
                {
                    var rgxy = Fetch<double[]>();
                    return new Vert {x = rgxy[0], y = rgxy[1]};
                }).ToList()
            }).ToList();

            Console.WriteLine(rgpoly.Count);
            Console.WriteLine(rgpoly.SelectMany(poly => poly.rgvert).Count());

            var w = rgpoly.First().rgvert.Select(vert => vert.x).Max();
            var h = rgpoly.First().rgvert.Select(vert => vert.y).Max();
            var scale = (800 / Math.Max(w, h));
            using(var bmp = new Bitmap(
                (int) (w*scale)+2,
                (int) (h*scale)+2))
            using(var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawPolygon(Pens.Black, rgpoly.First().rgpoint(scale));
                foreach(var poly in rgpoly.Skip(1))
                {
                    g.FillPolygon(Brushes.Blue, poly.rgpoint(scale));
                }
                bmp.Tsto();
            }
        }
    }
}
