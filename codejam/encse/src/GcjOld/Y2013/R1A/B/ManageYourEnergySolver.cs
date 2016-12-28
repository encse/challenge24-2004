using System;
using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1A.B
{
    public class ManageYourEnergySolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             The first line of the input gives the number of test cases, T. T test cases follow. 
             * Each test case is described by two lines. The first contains three integers:
             * E, the maximum (and initial) amount of energy, R, the amount you regain after each activity,
             * and N, the number of activities planned for the day. The second line contains N integers vi,
             * describing the values of the activities you have planned for today.
             */


            BigInteger E, R,N;
            pparser.Fetch(out E, out R, out N);
            var rgv = pparser.Fetch<BigInteger[]>();
            
            return () => Solve(E,R, rgv);
        }
        private IEnumerable<object> Solve(BigInteger eMax, BigInteger R, BigInteger[] rgv)
        {
            var rgemin = new BigInteger[rgv.Length];
            var rgsumNext = new BigInteger[rgv.Length];

            BigInteger esumPrev = eMax*rgv[rgv.Length-1];
            for (var i = rgv.Length - 2; i >= 0;i--)
            {
                var e0 = MaxHely(0, eMax, e => (eMax - e) * rgv[i] + EsumNext(eMax, R, rgv, rgemin, rgsumNext, i, e));
                rgemin[i] = e0;
                rgsumNext[i] = EsumNext(eMax, R, rgv, rgemin, rgsumNext, i, e0);
                esumPrev = (eMax - e0) * rgv[i] + rgsumNext[i];
            }
            Console.Write(".");
            yield return esumPrev;
        }

        private BigInteger EsumNext(BigInteger eMax, BigInteger R, BigInteger[] rgv, BigInteger[] rgemin,
                                      BigInteger[] rgsumNext, int i, BigInteger e0)
        {
            var e = e0;
            
            BigInteger esumNext = 0;
            for (int j = i + 1;; j++)
            {
                e = Tolt(e, eMax, R);
                if (e >= rgemin[j])
                {
                    esumNext += rgsumNext[j];
                    esumNext += (e - rgemin[j])*rgv[j];
                    break;
                }
            }
            return esumNext;
        }

        private BigInteger MaxHely(BigInteger lo, BigInteger hi, Func<BigInteger, BigInteger> f)
        {
            while (hi > lo + 1)
            {
                var m = (hi+lo)/2;
                var df = f(m ) - f(m+1);

                if (df > 0)
                    hi = m;
                else
                    lo = m;
            }

           return f(hi) > f(lo) ? hi : lo;
            
        }

        private BigInteger Tolt(BigInteger e, BigInteger eMax, BigInteger R)
        {
            return e+R <eMax ? e+R : eMax;
        }

    }
}
