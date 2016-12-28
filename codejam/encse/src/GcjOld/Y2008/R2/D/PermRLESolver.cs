using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2008.R2.D
{
    public class PermRLESolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            var k = pparser.Fetch<int>();
            var st = pparser.StLineNext();
            
            return () => Solve(k,st);
        }

        private IEnumerable<object> Solve(int k, string st)
        {
            var w = new uint[k,k];
            for(int i=0;i<k;i++)
            for(int j=0;j<k;j++)
            {
                if(j == i)
                    continue;
                w[i,j] = WijGet(k, st, i, j);
            }

            var msk = (1 << k) - 1;

            var mpwPathByiFromAndmsk = new Dictionary<Tuple<int, int, int>, uint>();
            var wMin = uint.MaxValue;
            for (var iFrom = 0; iFrom < k; iFrom++)
            {

                var mskT = Remove(msk, iFrom);
                var wiFromiTo = Foo(iFrom, st, k, mskT, iFrom, w, mpwPathByiFromAndmsk);
                if (wMin > wiFromiTo)
                    wMin = wiFromiTo;
                mpwPathByiFromAndmsk.Clear();
            }
           
            Console.Write(".");
            yield return wMin+1;
        }

        
        uint Foo(int iStart, string st, int k, int msk, int iFrom, uint[,] w, Dictionary<Tuple<int, int,int>, uint> mpwPathByiFromAndmsk)
        {
            
            var key = new Tuple<int, int, int>(iStart, iFrom, msk);
            if (mpwPathByiFromAndmsk.ContainsKey(key))
                return mpwPathByiFromAndmsk[key];
            uint wMin;
            if (msk == 0)
            {
                wMin = WLoopback(st, k, iFrom, iStart);
                
            }
            else
            {
                wMin = uint.MaxValue;
               
                for (var iTo = 0; iTo < k; iTo++)
                {
                    if (((1 << iTo) & msk) == 0)
                        continue;

                    var mskT = Remove(msk, iTo);
                    var wiFromiTo = w[iFrom, iTo] + Foo(iStart, st, k, mskT, iTo, w, mpwPathByiFromAndmsk);
                    if (wMin > wiFromiTo)
                    {
                        wMin = wiFromiTo;
                    }
                }
            }
          //  if(mpwPathByiFromAndmsk.Count<1000000)
                mpwPathByiFromAndmsk[key] = wMin;
            return wMin;
        }

     
        int Remove(int msk, int i)
        {
            return msk & (~(1 << i));
        }

        private uint WijGet(int k, string st, int i, int j)
        {
            uint w = 0;
            var cgroup = st.Length/k;
            for(int igroup=0;igroup<cgroup;igroup++)
            {
                var chi = st[igroup*k + i];
                var chj = st[igroup*k + j];
                if (chi != chj)
                    w++;
            }
            return w;
        }

        private uint WLoopback(string st, int k, int i, int j)
        {
            uint w = 0;
            var cgroup = st.Length / k;
            for (int igroup = 0; igroup < cgroup-1; igroup++)
            {
                var chi = st[igroup * k + i];
                var chj = st[(igroup+1) * k + j];
                if (chi != chj)
                    w++;
            }
            return w;
        }

    }
}
