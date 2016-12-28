using System;
using System.Collections.Generic;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1C.A
{
    public class ConsonantsSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
               The first line of the input gives the number of test cases, T. T test cases follow. 
             * The first line of each test case gives the name of a member as a string of length L, and an integer n.
             * Each name consists of one or more lower-case English letters.
             */
            string stName;
            int n;
            pparser.Fetch(out stName, out n);
            return () => Solve(stName, n);
        }

        private IEnumerable<object> Solve(string stName, int n)
        {
            long c = 0;
            long dc = 0;
            int lmatch = 0;
            for (int ichFirst = stName.Length-1; ichFirst >= 0; ichFirst--)
            {
                if(FConsonant(stName[ichFirst]))
                    lmatch++;
                else
                    lmatch = 0;

                if(lmatch >= n)
                    dc = stName.Length - (ichFirst + n) + 1;
                c += dc;
            }
            Console.Write(".");
            yield return c;
        }
     
        private bool FConsonant(char ch)
        {
            return ch != 'a' && ch != 'e' && ch != 'i' && ch != 'o' && ch != 'u';
        }
    }

}
