using System;
using System.Numerics;

namespace Wecomp.Util
{
    public class Bsrc
    {
        /// <summary>
        /// finds the first true index
        /// </summary>
        public static int Find(int lo, Func<int, bool> dgfGoal)
        {
            if (dgfGoal(lo))
                return lo;

            var hi = lo == 0 ? 1 : lo * 2;
            while (!dgfGoal(hi))
            {
                lo = hi;
                hi = 2 * hi;
            }
            return Find(lo, hi, dgfGoal).Value;
        }

        public static BigInteger FindBigInteger(BigInteger lo, Func<BigInteger, bool> dgfGoal)
        {
            if (dgfGoal(lo))
                return lo;

            var hi = lo == 0 ? 1 : lo * 2;
            while (!dgfGoal(hi))
            {
                lo = hi;
                hi = 2 * hi;
            }
            return FindBigInteger(lo, hi, dgfGoal).Value;
        }

        /// <summary>
        /// finds the first index between lo and hi inclusive where dgfGoal() is true, returns null if no such index exists
        /// </summary>
        public static int? Find(int lo, int hi, Func<int, bool> dgfGoal)
        {
            var fAny = false;
            while (hi - lo > 1)
            {
                int m = (lo + hi) / 2;
                if (!dgfGoal(m))
                    lo = m;
                else
                {
                    hi = m;
                    fAny = true;
                }
            }
            if (fAny)
                return hi;
            if (dgfGoal(hi))
                return hi;
            return null;
        }

        /// <summary>
        /// finds the first index between lo and hi inclusive where dgfGoal() is true, returns null if no such index exists
        /// </summary>
        public static BigInteger? FindBigInteger(BigInteger lo, BigInteger hi, Func<BigInteger, bool> dgfGoal)
        {
            var fAny = false;
            while (hi - lo > 1)
            {
                BigInteger m = (lo + hi) / 2;
                if (!dgfGoal(m))
                    lo = m;
                else
                {
                    hi = m;
                    fAny = true;
                }
            }
            if (fAny)
                return hi;
            if (dgfGoal(hi))
                return hi;
            return null;
        }
    }
}
