using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2012.R2.B
{
    public class AerobicsSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
           The first line of the input gives the number of test cases, T. T test cases follow. 
             * Each test case consists of two lines. The first line contains three integers: N, W and L,
             * denoting the number of students, the width of the mat, and the length of the mat, respectively. 
             * The second line contains N integers ri, denoting the reach of the arms of the ith student.
             */
            int cstudent, width,height; 
            pparser.Fetch(out cstudent, out width, out height);
            var rgr = pparser.Fetch<int[]>();
            return () => Solve(rgr, width, height);
        }

        private IEnumerable<object> Solve(int[] rgr, int width, int height)
        {
            var r = new Random(42);
            var cstudent = rgr.Length;
            var posx = new int[cstudent];
            var posy = new int[cstudent];

            var perm = new List<int>();
            for (int i = 0; i < cstudent; i++)
                perm.Add(i);
            perm.Sort((i, j) => rgr[j] - rgr[i]);

            while (true)
            {
                var l = 0;
                var iperm = 0;
                while (iperm < cstudent && l < 10000)
                {
                    var i = perm[iperm];
                    posx[i] = r.Next(width);
                    posy[i] = r.Next(height);

                    if (FOk(iperm, perm, rgr, posx, posy))
                        iperm++;
                    else
                        l++;
                }
                if (iperm == cstudent)
                    break;
                Console.Write("#");
            }
            Console.Write(".");
            for (int i = 0; i < cstudent; i++)
            {
                yield return posx[i];
                yield return posy[i];
            }
        }

        private bool FOk(int iperm, List<int> perm, int[] rgr, int[] posx, int[] posy)
        {
            int i = perm[iperm];
            decimal x1 = posx[i];
            decimal y1 = posy[i];
            decimal r1 = rgr[i];
            for(int jperm=0;jperm<iperm;jperm++)
            {
                int j = perm[jperm];
                decimal x2 = posx[j];
                decimal y2 = posy[j];
                decimal r2 = rgr[j];

                var dx = x1 - x2;
                var dy = y1 - y2;
                var r = r1+r2;

                if (dx*dx+dy*dy <= r*r)
                    return false;
            }
            return true;
        }
    }
}
