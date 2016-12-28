using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.B
{
    public class RequirementsSolver : Solver
    {
        private int mod = 1000000007;

        private class Mask
        {
            private readonly int creq;
            public readonly int mask;
            private readonly int cmasked;

            private bool this[int iman]
            {
                get { return (mask & iman) == 0; }
            }

          
            public Mask(int mask, int creq)
            {
                this.creq = creq;
                this.mask = mask;

                cmasked = 0;
                var creqT = creq-1;
                while(creqT>=0)
                {
                    if( (mask& (1<<creqT)) != 0)
                        cmasked++;
                    creqT --;
                }
                
            }

            public Mask Or(int w)
            {
                return new Mask(w | mask, creq);
            }

            public IEnumerable<int> EachNotMasked()
            {

                var iLim = 1 << (creq - cmasked);
                var maskLim = 1 << creq;
                for (int i = 0; i <= iLim -1; i++)
                {
                    var iman = maskLim - 1 - ImanGet(i, iLim, mask, maskLim);
                    //  Console.WriteLine(iman);
                    yield return iman;
                }
            }

            public int CNotMasked()
            {
                return 1 << (creq - cmasked);
            }

            public void ForEachNotMasked(Action<int> dg)
            {
             
                var iLim = 1 << (creq - cmasked);
                var maskLim = 1 << creq;
                for(int i=iLim-1;i>=0;i--)
                {
                    var iman = (maskLim - 1 - ImanGet(i, iLim, mask, maskLim));
                  //  Console.WriteLine(iman);
                    dg(iman);
                }
            }

            public int LargestNotMasked()
            {
                var iLim = 1 << (creq - cmasked);
                var maskLim = 1 << creq;

                return ImanGet(iLim - 1, iLim, mask, maskLim);
                
            }

            private static int ImanGet(int i, int iLim, int mask, int maskLim)
            {
                var iman = 0;

                var iptr = 1;
                var mptr = 1;

                while (mptr != maskLim)
                {
                    if ((mask & mptr) != 0)
                    {
                        iman+=mptr;
                    }
                    else
                    {
                        iman +=  mptr * ((i & iptr) == 0 ? 0 : 1);
                        iptr <<= 1;
                    }
                    mptr <<= 1;
                }

                
                return iman;
            }

            public void ForEachMasked2(Action<int> dg)
            {
                for (int iman = 0; iman < (1<<creq); iman++)
                {
                    if (this[iman])
                    {
                  //      Console.WriteLine(iman);
                        dg(iman);
                    }
                }
            }
        }

        private int cmpw;
        int foo(BigInteger by, int mod)
        {
            var c = 214562164;
            var z = (int)BigInteger.ModPow(c, by, mod);

            var by1 = (int)(by%(mod-1));

            //for (int i = by1; i < by1+10000; i++)
            {
                var z1 = (int)BigInteger.ModPow((c), by1, mod);
                if (z == z1)
                {
                    return by1;
                }
            }
            return -1;
        }

        public override void Solve()
        {
            //Console.WriteLine(foo(BigInteger.Pow(2, 999999), mod));
          //  var mod1 = 19;
          ////  var c = ;
          //  var by = BigInteger.Pow(2, 999999);
          //  var c = 214562164;
          //  var z = (int)BigInteger.ModPow((c), by, mod1);
          //  for(uint i=0;i< (1<<30) ;i++)
          //  {
          //      var by1 = i;
          //      var z1 = (int)BigInteger.ModPow((c), by1, mod1);
          //      if(z == z1)
          //      {
          //          Console.WriteLine(i);
          //      }
          //  }
            
            
          // by = BigInteger.Pow(2, 99);
           
            
            //2 123
            //87 65 43 21

        

            var rgfl = Fetch<int[]>();
            var creq = rgfl[0];
            var n = rgfl[1];

            var rgcman=Fetch<int[]>().ToArray();

            var bn = (BigInteger.Pow(2, n)%(mod - 1));

            var mask = new Mask(0, creq);
            var w = 1;

            var sum0 = 0L;
            var s0 = 0;
            for (; w < rgcman.Length; w <<= 1)
            {
                long sumT;
                int dummy;
                var p = Phi(rgcman, bn, mask.Or(w), 2 * w, out sumT, out dummy);
                sum0 += sumT;
                s0 = (s0 + p) % mod;
            }

            var sum1 = rgcman[rgcman.Length - 1] + sum0;
            var s1 = ModPow(sum1, bn);
            var result = (s1 - s0 + mod)%mod;

            using(var solwrt = new Solwrt(FpatOut, FpatRefout))
                solwrt.WriteLine(result);
            
        }

        private int Phi(int[] rgcman, BigInteger bn, Mask mask, int w, out long sum, out int csum)
        {
         
            var sum0 = 0L;
            var s0 = 0;
            var csum0 = 0;
            for (; w < rgcman.Length; w <<= 1)
            {
                long sumT;
                var maskT = mask.Or(w);
                int csumT;
                var p = Phi(rgcman, bn, maskT, 2 * w, out sumT, out csumT);
                csum0 += csumT;
                sum0 += sumT;
                s0 = (s0 + p) % mod;
            }



           // long sum1 = ModSumNotMasked(rgcman, mask);
            long sum1 =  sum0 + TopMostN(rgcman, mask, mask.CNotMasked() - csum0 );
            //if (sumTT != sum1)
            //    throw new ArgumentException();
            var s1 = ModPow(sum1, bn);
            var result = (s1 - s0 + mod)%mod;
            sum = sum1 - sum0;
            csum = mask.CNotMasked() - csum0;

            return result;
        }

        private long TopMostN(int[] rgcman, Mask mask, int n)
        {

            return mask.EachNotMasked().Take(n).Select(iman => rgcman[iman]).Sum() % mod;
            

        }

        private int ModSumNotMasked(int[] rgcman, Mask mask)
        {
            var sumT= 0;
            mask.ForEachNotMasked(iman => { sumT = (sumT + rgcman[iman]) % mod; });
            return sumT;
        }


        private int ModPow(long x, BigInteger by)
        {
          //  return (int)(x % mod);
            return (int)BigInteger.ModPow(x, by, mod);
        }
    }
}
