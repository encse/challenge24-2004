using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1C.B
{
    public class PogoSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             The first line of the input gives the number of test cases, T. T test cases follow, one per line. 
             * Each line consists of 2 integers separated by a single space, X and Y, the coordinates of the point you want to reach.
             */
            int x, y;
            pparser.Fetch(out x, out y);
            return () => Solve(x, y);
        }
        private IEnumerable<object> Solve(int xD, int yD)
        {
            int x = 0;
            int y = 0;
            int c = 1;

            int d = x < xD ? -1 : 1;

            var sb = new StringBuilder();
            while (x != xD)
            {
                sb.Append(d == 1 ? "E" : "W");
                x += d*c;
                c++;
                d = -d;
            }

            d = y < yD ? -1 : 1;

            while (y != yD)
            {
                sb.Append(d == 1 ? "N" : "S");
                y += d*c;
                c++;
                d = -d;
            }
            yield return sb.ToString();
        }
    }

}
