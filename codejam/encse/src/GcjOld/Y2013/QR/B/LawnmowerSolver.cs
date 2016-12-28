using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.QR.B
{
    public class LawnmowerSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             *The first line of the input gives the number of test cases, T. T test cases follow. 
             *Each test case begins with a line containing two numbers: N and M. Next follow N lines,
             *with the ith line containing M numbers ai,j each, the number ai,j describing the desired
             *height of the grass in the jth square of the ith row.
             */
            long crow;
            long ccol;
            pparser.Fetch(out crow, out ccol);
            var heightmap = new int[crow,ccol];
            for (int irow = 0; irow < crow;irow++ )
            {
                var rgxxx = pparser.Fetch<int[]>();
                for (int icol = 0; icol < ccol; icol++)
                    heightmap[irow, icol] = rgxxx[icol];
            }

            return () => Solve(heightmap);
        }


        private IEnumerable<object> Solve(int[,] heightmap)
        {
            int crow = heightmap.GetLength(0);
            int ccol = heightmap.GetLength(1);
            var heightmapNow = new int[crow,ccol];
            for (int irow = 0; irow < crow; irow++)
                for (int icol = 0; icol < ccol; icol++)
                    heightmapNow[irow, icol] = 100;
            

            for (int irow = 0; irow < crow; irow++)
            {
                int height = RowMin(heightmap, irow);
                for (int icol = 0; icol < ccol; icol++)
                    heightmapNow[irow, icol] = Math.Min(heightmapNow[irow, icol], height);
            }
            
            for (int icol = 0; icol < ccol; icol++)
            {
                int height = ColMin(heightmap, icol);
                for (int irow = 0; irow < crow; irow++)
                    heightmapNow[irow, icol] = Math.Min(heightmapNow[irow, icol], height);
            }



            for (int irow = 0; irow < crow; irow++)
            {
                for (int icol = 0; icol < ccol; icol++)
                {

                    if (heightmap[irow, icol] != heightmapNow[irow, icol])
                    {
                        yield return "NO";
                        yield break;
                    }
                }
            }

            yield return "YES";
        }

        private int ColMin(int[,] heightmap, int icol)
        {
            int crow = heightmap.GetLength(0);

            int height = 0;
            for(int irow = 0; irow<crow;irow++)
                height = Math.Max(height, heightmap[irow, icol]);
            return height;
        }

        private int RowMin(int[,] heightmap, int irow)
        {
            int ccol = heightmap.GetLength(1);

            int height = 0;
            for (int icol = 0; icol < ccol; icol++)
                height = Math.Max(height, heightmap[irow, icol]);
            return height;
        }


    }
}
