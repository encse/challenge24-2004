using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.QR.C
{
    public class FairAndSquareSolver : IConcurrentSolver
    {
         
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        private static int ccore = 8;
        private static BigInteger[] rgfairRoot;

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            
            if (rgfairRoot == null)
            {
                rgfairRoot = RgfairRoot().ToArray();
                File.WriteAllLines("prl", rgfairRoot.Select(x => x.ToString()));
            }

            BigInteger A;
            BigInteger B;
            pparser.Fetch(out A, out B);
            return () => Solve(A, B);
        }

        IEnumerable<BigInteger> RgfairRoot()
        {
            
            var rgdg = new Action[ccore];
            var rgresult = new Hlm_Chewbacca<BigInteger>[ccore];

            for (var i = 0; i < ccore; i++)
            {
                var iT = i;
                rgdg[iT] = () =>
                               {
                                   rgresult[iT] = new Qux().RgfairRoot(iT+1);
                               };
            }
                
            Parallel.Invoke(rgdg);

            var hlmP = new Qux().XX();
            foreach (var hlm in rgresult)
                hlmP.Union(hlm);

            return hlmP.Elements.OrderBy(b => b);
        }

        class Qux
        {
            public Hlm_Chewbacca<BigInteger> RgfairRoot(int i)
            {
                var hlmP =  new Hlm_Chewbacca<BigInteger>();
                foreach (var x in EnpalindromeForFairRoot(i).Where(x => FPalindrome(x*x)))
                {
                    hlmP.Add(x);
                    if (hlmP.Count % 1000 == 0)
                        Console.Write(".");
                }
                return hlmP;
            }

            private IEnumerable<BigInteger> EnpalindromeForFairRoot(int y)
            {
                Console.WriteLine("#{0}#".StFormat(y));

                var lPrev = -1;
                BigInteger shift = 10;
                for (int i = y; ; i += ccore)
                {

                    var st = Convert.ToString(i, 2);
                    var stRev = Reverse(st);

                    int l = st.Length;
                    if (l != lPrev)
                    {
                        shift = BigInteger.Pow(10, l);
                        Console.Write(" " + l + " ");
                        lPrev = l;
                    }

                    var biHi = BigInteger.Parse(st);
                    var biLo = BigInteger.Parse(stRev);

                    yield return (biHi) * shift + biLo;
                    biHi *= 10;
                    yield return (biHi) * shift + biLo;
                    yield return (biHi + 1) * shift + biLo;
                    yield return (biHi + 2) * shift + biLo;


                    if (st.Length >= 26)
                        break;

                }
            }


            public string Reverse(string s)
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }


            private bool FPalindrome(BigInteger p)
            {
                string st = p.ToString();
                for (int i = 0; i < st.Length; i++)
                    if (st[i] != st[st.Length - i - 1])
                        return false;
                return true;
            }

            public Hlm_Chewbacca<BigInteger> XX()
            {
                var hlmP = new Hlm_Chewbacca<BigInteger>();

                hlmP.AddRange(
                    new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 22, 33, 44, 55, 66, 77, 88, 99}.Select(x => new BigInteger(x)));

                for (int i = 0; i < 50; i++)
                {
                    var st = 2 + new string('0', i);
                    hlmP.Add(BigInteger.Parse(st + Reverse(st)));
                    hlmP.Add(BigInteger.Parse(st + "0" + Reverse(st)));
                    hlmP.Add(BigInteger.Parse(st + "1" + Reverse(st)));
                    hlmP.Add(BigInteger.Parse(st + "2" + Reverse(st)));
                }
                return hlmP;
            }
        }
        
        private IEnumerable<object> Solve(BigInteger A, BigInteger B)
        {
            
            var iStart = Sqrt(A);
            if (iStart * iStart < A)
                iStart++;

            var iLim = Sqrt(B);
            if (iLim * iLim <= B)
                iLim++;
            
            long cFairAndSquare = 0;
            foreach (var palindrome in Enpalindrome2())
            {
                if(palindrome < iStart)
                    continue;
                
                if(palindrome >= iLim)
                    break;
                
                var sq = palindrome*palindrome;

                if (FPalindrome(sq))
                    cFairAndSquare++;
            }
            Console.Write(".");

            yield return cFairAndSquare;
        }

        private bool FPalindrome(BigInteger p)
        {
            string st = p.ToString();
            for (int i = 0; i < st.Length; i++)
                if (st[i] != st[st.Length - i - 1])
                    return false;
            return true;
        }

        private IEnumerable<BigInteger> Enpalindrome2()
        {
            return rgfairRoot;
        }
        //private IEnumerable<BigInteger> Enpalindrome()
        //{
        //    foreach (var x in new []{1,2,3,4,5,6,7,8,9})
        //    {
        //        yield return new BigInteger(x);
        //    }

        //    int iStart = 1;

        //    while(true)
        //    {
        //        int iLim = iStart * 10;

        //        for (int i = iStart; i < iLim; i++)
        //        {
        //            var v = i.ToString();
        //            yield return BigInteger.Parse(v + Reverse(v));
        //        }

        //        for (int i = iStart; i < iLim; i++)
        //        {
        //            for (int k = 0; k < 10; k++)
        //            {
        //                var v = i.ToString();
        //                yield return BigInteger.Parse(v + k + Reverse(v));
        //            }
        //        }
        //        iStart = iLim;
        //    }
        //}

        

        public BigInteger Sqrt(BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
    }
}
