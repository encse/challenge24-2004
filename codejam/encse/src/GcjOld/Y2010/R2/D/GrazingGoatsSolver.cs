using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.R2.D
{
    public class GrazingGoatsSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The first line of the input gives the number of test cases, T. T test cases follow. Each test case begins with a line containing the integers N and M.

             The next N lines contain the positions P1, P2, ..., PN, one per line. This is followed by M lines, containing the positions Q1, Q2, ..., QM, one per line.

             Each of these N + M lines contains the corresponding position's x and y coordinates, separated by a single space.
             */
            int n, m;
            pparser.Fetch(out n, out m);


            if (n != 2)
                throw new ArgumentException();
            var rgptPole = pparser.FetchN<Pt>(n);
            var rgptBucket = pparser.FetchN<Pt>(m);

            return () => Solve(rgptPole, rgptBucket);
        }

        class Pt
        {
            public decimal x;
            public decimal y;

            public Pt(decimal x, decimal y)
            {
                this.x = x;
                this.y = y;
            }
            public decimal Dist(Pt pt)
            {
                var dx = x - pt.x;
                var dy = y - pt.y;
                return (decimal)Math.Sqrt((double)(dx*dx + dy*dy));
            }
        }

        private IEnumerable<object> Solve(List<Pt> rgptPole, List<Pt> rgptBucket)
        {
            foreach (var ptBucket in rgptBucket)
            {
                var pt1 = rgptPole[0];
                var pt2 = rgptPole[1];
                var r =(double) pt1.Dist(ptBucket);
                var R = (double)pt2.Dist(ptBucket);

                var d =(double) pt1.Dist(pt2);

                var tKeresett = r*r*Math.Acos((d*d + r*r - R*R)/(2*d*r)) +
                                R*R*Math.Acos((d*d + R*R - r*r)/(2*d*R)) -
                                0.5*Math.Sqrt((-d + r + R)*(d + r - R)*(d - r + R)*(d + r + R));

                yield return tKeresett.ToString("0.#######", CultureInfo.InvariantCulture);
            }
        }

     
    }
}
