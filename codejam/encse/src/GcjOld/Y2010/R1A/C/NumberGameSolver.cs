using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.R1A.C
{
    public class NumberGameSolver : IConcurrentSolver
    {
        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int A1, A2, B1, B2;
            pparser.Fetch(out A1, out A2, out B1, out B2);
            return () => EnobjSolveCase(A1, A2, B1, B2);
        }

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }


        protected IEnumerable<object> EnobjSolveCase(int A1, int A2, int B1, int B2)
        {
            long c = 0;
            int i = 0;
            Console.Write("*");
            for (var A = A1; A <= A2; A++)
            {
                if(i%100000 == 0)
                    Console.Write(".");
                i++;
                
                int bFirstLose = BSignChange(A, 1, A);
                int bFirstWin = BSignChange(A, A, 2 * A);



                if(bFirstLose > B1)
                {
                    if (B2 < bFirstLose)
                        bFirstLose = B2 + 1;
                    c += bFirstLose - B1;
                    
                }
                if(bFirstWin <= B2)
                {
                    if (bFirstWin < B1)
                        bFirstWin = B1;
                    c += (B2 - bFirstWin) + 1;
                }
                //for (var B = B1; B <= B2; B++)
                //{
                //    if (FPlayerXWins(A, B))
                //    {
                //        c++;

                //    }
                //}
            }
            Console.Write("#");
            yield return c;
        }

        private int BSignChange(int A, int bLow, int bHi)
        {
            var sgn = FPlayerXWins(A, bLow);

            while (bHi - bLow > 1)
            {
                var b = (bLow + bHi)/2;

                var sgnT = FPlayerXWins(A, b);
                if (sgnT == sgn)
                {
                    bLow = b;
                }
                else
                {
                    bHi = b;
                }
            }
            return bHi;
        }

        bool FPlayerXWins(int A, int B)
        {
            bool fWins = true;
            foreach (var c in RgcGet(A,B))
            {
                if (c != 1)
                    break;

                fWins = !fWins;
            }
            return fWins;
        }

        IEnumerable<int>  RgcGet(int A, int B)
        {
            var bigger = Math.Max(A, B);
            var smaller = Math.Min(A, B);
            while (bigger != smaller)
            {
                yield return bigger/smaller;
                bigger = bigger%smaller;
                var swap = bigger;
                bigger = smaller;
                smaller = swap;
            }
            if (bigger != 0)
                yield return 1;

        }

     
    }

   
}
