using System;
using System.Collections.Generic;
using System.Globalization;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2011.R2.B
{
    public class SpinningBladeSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The first line of the input gives the number of test cases, T. T test cases follow. 
             * Each one starts with a line containing 3 integers: R, C and D — the dimensions of the grid and the mass you expected each cell to have. 
             * The next R lines each contain C digits wij each, giving the differences between the actual and the expected mass of the grid cells. 
             * Each cell has a uniform density, but could have an integer mass between D + 0 and D + 9, inclusive.
             */
            int crow, ccol, d;
            pparser.Fetch(out crow, out ccol, out d);
            var sheet = new int[crow,ccol];

            for (int irow = 0; irow < crow;irow++ )
            {
                string st = pparser.StLineNext();
                int icol = 0;
                foreach (var ch in st)
                {
                    sheet[irow, icol] = d + int.Parse(ch.ToString());
                    icol++;
                }
            }

            return () => Solve(sheet);
        }

       

        private IEnumerable<object> Solve(int[,] sheet)
        {
            int crow = sheet.GetLength(0);
            int ccol = sheet.GetLength(1);
            for(int k=Math.Min(ccol,crow);k>=3;k--)
            {
                for (int irow0 = 0; irow0+ k <= crow; irow0++)
                {
                    for (int icol0 = 0; icol0 + k <= ccol; icol0++)
                    {
                        var sm = (decimal)0;
                        var spxm = (decimal)0;
                        var spym = (decimal)0;
                        var py = (decimal)0.5;

                        for (int irow = 0; irow < k; irow++)
                        {
                            var px = (decimal) 0.5;

                            for (int icol = 0; icol < k; icol++)
                            {
                                if (!(icol == 0 && irow == 0 || icol == k - 1 && irow == 0 ||
                                    icol == 0 && irow == k - 1 ||
                                    icol == k - 1 && irow == k - 1))
                                {
                                    sm += sheet[irow + irow0, icol + icol0];
                                    spxm += sheet[irow + irow0, icol + icol0]*px;
                                    spym += sheet[irow + irow0, icol + icol0]*py;
                                }
                                px += 1;
                            }
                            py += 1;

                        }

                        var cx = spxm / sm;
                        var cy = spym / sm;
                        if (Math.Abs(cx * 2 - k) <= (decimal)0.00001 && Math.Abs(cy * 2 - k) <= (decimal)0.00001)
                        {
                            yield return k;
                            yield break;
                        }
                    }
                }

             
            }
            yield return "IMPOSSIBLE";
        }
    }
}
