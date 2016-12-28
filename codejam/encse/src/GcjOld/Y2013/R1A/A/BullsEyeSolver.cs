using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1A.A
{
    public class BullsEyeSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            //The first line of the input gives the number of test cases, T. T test cases follow. 
            //Each test case consists of a line containing two space separated integers: r and t.

            BigInteger r, t;
            pparser.Fetch(out r, out t);
            
            return () => Solve(r,t);
        }

        private IEnumerable<object> Solve(BigInteger r, BigInteger t)
        {
            
            BigInteger mLow = 1;
            BigInteger mHi = 2;
            while (FEnough(mHi, r, t))
            {
                mLow = mHi;
                mHi = 2*mHi;
            }

            //mLow-t még ki tud festeni, mHi-t már nem
            while(mHi-1>mLow)
            {
                var m = (mHi + mLow)/2;
                if (FEnough(m, r, t))
                    mLow = m;
                else
                    mHi = m;
            }
            yield return mLow;
        }

        private bool FEnough(BigInteger m, BigInteger r, BigInteger t)
        {
            //m db gyűrűhöz kell ennyi festék:
            //-1/6 (m+1)*(2*m^2+6*m*r+m+6r^2)
            m *= 2;

            
            return   (m+r-1)*(m+r)-(r-1)*r<= 2*t;

        }
    }
}
