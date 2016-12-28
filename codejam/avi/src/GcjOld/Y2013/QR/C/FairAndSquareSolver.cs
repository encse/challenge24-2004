using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.QR.C
{
    internal class FairAndSquareSolver : GcjSolver
    {
        private static List<BigInteger> Rgfnsq;
        private static BigInteger Max;

        private IEnumerable<BigInteger> enfair()
        {
            var l = 1;
            var fparos = false;
            BigInteger i = 1;

            for(;;)
            {
                var sti = i.ToString();

                if(sti.Length > l)
                {
                    if(fparos)
                    {
                        l = sti.Length;
                        fparos = false;
                    }
                    else
                    {
                        fparos = true;
                        i = BigInteger.Parse("1" + new string('0', l - 1));
                        continue;
                    }
                }

                var stfair = sti + new string(sti.Reverse().Skip(fparos ? 0 : 1).ToArray());
                yield return BigInteger.Parse(stfair);

                i++;
            }
        }

        private static bool fFair(BigInteger num)
        {
            if(num < 1)
                throw new Exception();

            var rgch = num.ToString().ToCharArray();
            for(int i = 0, i2 = rgch.Length - 1; i < i2; i++,i2--)
            {
                if(rgch[i] != rgch[i2])
                    return false;
            }
            return true;
        }

        private IEnumerable<Tuple<BigInteger, BigInteger>> enfnsq()
        {
            var bigIntegers = enfair();
            foreach(BigInteger fair in bigIntegers)
            {
                var tp = new Tuple<BigInteger, BigInteger>(fair * fair, fair);
                if(fFair(tp.Item1))
                    yield return tp;
            }
        }

        private static IEnumerable<BigInteger> enfnsq2(BigInteger max)
        {

            var l = 1;
            var fparos = false;
            BigInteger i = 1;

            for(;;)
            {
                var sti = i.ToString();

                if(sti.Length > l)
                {
                    if(fparos)
                    {
                        l = sti.Length;
                        fparos = false;
                    }
                    else
                    {
                        fparos = true;
                        i = BigInteger.Parse("1" + new string('0', l - 1));
                        continue;
                    }
                }

                var stfair = sti + new string(sti.Reverse().Skip(fparos ? 0 : 1).ToArray());

                var fair = BigInteger.Parse(stfair);

                var fnsq = fair * fair;

                if(fnsq > max)
                    yield break;

                if(fFair(fnsq))
                {
                    yield return fnsq;
                    i++;
                }
                else
                {
                    BigInteger d = 1;
                    var inul = sti.Length;
                    for(;;)
                    {
                        inul--;
                        if(sti[inul] != '0')
                        {
                            i += d;
                            break;
                        }
                        d *= 10;
                    }
                }
            }
        }

        static FairAndSquareSolver()
        {
            Max = 10000000000;
            Max = Max * Max * Max * Max * Max * Max * Max * Max * Max * Max;

            Rgfnsq = enfnsq2(Max).ToList();

        }


        protected override IEnumerable<object> EnobjSolveCase()
        {
            BigInteger nummin;
            BigInteger nummax;
            Fetch(out nummin, out nummax);

            Debug.Assert(nummax <= Max);

            var imin = Rgfnsq.BinarySearch(nummin);
            if(imin < 0)
            {
                imin = ~imin;
            }

            var imax = Rgfnsq.BinarySearch(nummax);
            if(imax < 0)
            {
                imax = ~imax - 1;
            }

            yield return imax - imin + 1;
        }
    }
}
