using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Gcj.Util;

namespace Gcj.Y2013.R1B.B
{
    class FallingDiamondsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cOrig;
            int x;
            int y;
            Fetch(out cOrig, out x, out y);

            if(cOrig==0 || ((x+y)%2) == 1)
            {
                yield return 0.0;
                yield break;
            }

            int w = 1;
            int cLeft = cOrig - 1;
            int cPiramid = 1;

            for(;;)
            {
                int cNext = w + 1 + w + 2;
                if(cLeft < cNext)
                    break;

                cLeft -= cNext;
                w += 2;
                cPiramid += cNext;
            }

            int cNeeded = y + 1;

            int cMax = w + 1;

            var coki = new Action<decimal>(qqq =>
            {
                return;
                Random rnd = new Random(0);
                decimal cgAll = 0;
                int c = 100000;

                for (int _ = 0; _ < c; _++)
                {
                    decimal c1 = 0;
                    decimal c2 = 0;
                    for (int i = 0; i < cLeft; i++)
                    {
                        if(c1 == cMax)
                        {
                            Debug.Assert(c2 < cMax);
                            c2++;
                        }
                        else if(c2 == cMax)
                        {
                            c1++;
                        }
                        else if(rnd.Next(2) != 0)
                        {
                            c1++;
                        }
                        else
                        {
                            c2++;
                        }
                    }

                    if(c1 >= cNeeded)
                        cgAll++;
                }

                if (Math.Abs((decimal)((cgAll / c) - qqq)) > ((decimal)0.01))
                    Console.WriteLine("Coki");
            });


            if (Math.Abs(x) + Math.Abs(y)  < w)
            {
                yield return 1.0;
                yield break;
            }

            if (Math.Abs(x) + Math.Abs(y) > w + 2 /*|| x == 0*/)
            {
                yield return 0.0;
                yield break;
            }

            Debug.Assert(Math.Abs(x) + Math.Abs(y) == w + 1);

            if(cNeeded > cLeft)
            {
                coki(0);
                yield return 0.0;
                yield break;
            }

            var iStart = Math.Max(0, cLeft - cMax);

            if(cNeeded <= iStart)
            {
                coki(1);
                yield return 1.0;
                yield break;
            }

            BigInteger all = 0;
            BigInteger good = 0;
            for (int i = iStart; i <= Math.Min(cLeft, cMax); i++)
            {
                BigInteger al = alatt(cLeft, i);

                all += al;
                if(cNeeded <= i)
                    good += al;
            }


            var r = ((decimal)(100000000 * good / all)) / 100000000;
            coki(r);
            yield return r;


        }

        private BigInteger alatt(BigInteger a, BigInteger b)
        {
            BigInteger r = 1;
            for (BigInteger i = 0; i < b; i++)
                r *= (a - i);
            for (BigInteger i = 0; i < b; i++)
                r /= (1 + i);
            return r;
        }

    }
}
