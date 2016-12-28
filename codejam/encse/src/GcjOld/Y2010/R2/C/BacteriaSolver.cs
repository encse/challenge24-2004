using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.R2.C
{
    public class BacteriaSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The input consists of:

                One line containing C, the number of test cases.
                Then for each test case:
                One line containing R, the number of rectangles of cells that initially contain bacteria.
                R lines containing four space-separated integers X1 Y1 X2 Y2. This indicates that all the cells with X 
             *  coordinate between X1 and X2, inclusive, and Y coordinate between Y1 and Y2, inclusive, contain bacteria.
                The rectangles may overlap.
                North is in the direction of decreasing Y coordinate. 
                West is in the direction of decreasing X coordinate. 
             */
            int crect = pparser.Fetch<int>();

            var mtx = new int[101,101];
            for(int i=0;i<crect;i++)
            {
                int x1, x2, y1, y2;
                pparser.Fetch(out x1, out y1, out x2, out y2);
                for (int x = x1; x <= x2; x++)
                    for (int y = y1; y <= y2; y++)
                        mtx[x, y] = 1;
            }


            return () => Solve(mtx);
        }

        private IEnumerable<object> Solve(int[,] mtx)
        {
            int cround = 0;
            var fAny = true;
            while (fAny)
            {
                int[,] mtxNew = new int[101, 101]; 
                fAny = false;
                for (int x = 1; x < 101; x++)
                    for (int y = 1; y < 101; y++)
                    {
                        if (mtx[x, y] == 1)
                            mtxNew[x, y] = mtx[x - 1, y] != 0 || mtx[x, y - 1] != 0 ? 1 : 0;
                        else
                            mtxNew[x, y] = mtx[x - 1, y] != 0 && mtx[x, y - 1] != 0 ? 1 : 0;
                        fAny |= mtxNew[x, y] == 1;
                    }
                mtx = mtxNew;
                cround++;
            }
            yield return cround;
        }
     
    }
}
