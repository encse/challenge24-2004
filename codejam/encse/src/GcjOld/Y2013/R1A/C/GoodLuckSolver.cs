using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1A.C
{
    public class GoodLuckSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
            The first line of the input gives the number of test cases, T, which is always equal to 1. 
             * The second line of the input file contains four space-separated integers R, N, M and K, in that order. 
             * The next R lines describe one set of K products each. Each of those lines contains K space-separated integers —
             * the products that Maryam passes to Peiling. It is guaranteed that all sets in the input are generated independently
             * randomly according to the procedure from the problem statement.
             */


            int R, N, M, K;
            pparser.Fetch(out R, out N, out M, out K);
            var rgtc = new List<int[]>();
            for(int i=0;i<R;i++)
            {
               rgtc.Add(pparser.Fetch<int[]>());
            }
            
            
            return () => Solve(rgtc);
        }

        
        private IEnumerable<object> Solve(List<int[]> rgtc)
        {
            var r = new Random();
            foreach (var tc in rgtc)
            {
                var rgcandidate = new List<string>();
                for(int i=2;i<=5;i++)
                for(int j=2;j<=i;j++)
                for(int k=2;k<=j;k++)
                {
                    bool fOK = true;
                    foreach(var mul in tc)
                    {
                        if( mul == 1)
                            continue;
                        if(mul == i || mul == j || mul == k)
                            continue;
                        if (mul == i*j || mul == i*k || mul == j*k)
                            continue;
                        if (mul == i*j*k)
                            continue;
                        fOK = false;
                        break;
                    }

                    if (fOK)
                        rgcandidate.Add(k.ToString() + j.ToString() + i.ToString());
                }
                //foreach (var candidate in rgcandidate)
                //{
                //    Console.WriteLine(candidate);
                //}
                yield return Solwrt.NewLine;
                yield return rgcandidate[r.Next(rgcandidate.Count)];

            }
        }

       
    }
}
