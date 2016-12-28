using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.R2.B
{
    internal class ManyPrizes2Solver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cRound;
            BigInteger cPrize;
            Fetch(out cRound, out cPrize);

            var cTeam = BigInteger.Pow(2, cRound);

            if(cTeam <= cPrize)
            {
                yield return cTeam - 1;
                yield return cTeam - 1;
                yield break;
            }


            var icantLoose = iCantLoose(cRound, cPrize).ToString();

            var cPrize2 = cTeam - cPrize;
            var icantLoose2 = iCantLoose(cRound, cPrize2);
            var icanWin = (cTeam - 1 - icantLoose2 - 1).ToString();

            yield return icantLoose;
            yield return icanWin;
        }

        private BigInteger iCantLoose(int cRound, BigInteger cPrize)
        {
            BigInteger iTeam = 0;
            BigInteger pos = 0;
            var i = 0;
            for(;;)
            {
                Set(ref pos, cRound - 1 - i);
                iTeam = 2 * iTeam + 1;
                i++;
                if(pos >= cPrize)
                    break;
                if(i >= cRound)
                {
                    Debug.Assert(false);
                    return iTeam;
                }
            }
            Debug.Assert(BigInteger.Pow(2, cRound) - 1 >= iTeam);
            return iTeam - 1;
        }

        private void Set(ref BigInteger num, int ibit)
        {
            num = num | BigInteger.Pow(2, ibit);
        }

        private bool FSet(BigInteger num, int ibit)
        {
            return (num & BigInteger.Pow(2,ibit)) > 0;
        }

    }
}
