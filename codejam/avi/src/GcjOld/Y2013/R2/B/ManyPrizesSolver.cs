using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.R2.B
{
    internal class ManyPrizesSolver : GcjSolver
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

            BigInteger iworstNotLoose=0;
            for(var i=cRound-1;i>=0;i--)
            {
                iworstNotLoose = Set(iworstNotLoose, cRound - 1 - i);
                if(!FSet(cPrize-1,i))
                    break;
            }
            iworstNotLoose--;
            yield return iworstNotLoose;

            BigInteger iworstCouldWin = 0;
            BigInteger ipos = 0;
            for(var i=0;;i++)
            {
                ipos = Set(ipos, i);
                if(ipos>=cPrize)
                    break;
                iworstCouldWin = Set(iworstCouldWin, cRound - 1 - i);
            }
            yield return iworstCouldWin;
        }

        private void x(int cRound, BigInteger cPrize, out BigInteger iCantLoose, out BigInteger iCanWin)
        {
            throw new Exception();

            var rg = new List<BigInteger>();
            for(BigInteger num=0;!FSet(num,cRound);num++)
            {
                BigInteger rank = 0;
                for(var i=0;i<cRound;i++)
                {
                    if(FSet(num,i))
                        rank = Set(rank, cRound - 1 - i);
                }
                rg.Add(rank);
            }
            var rg2 = rg.Take((int) cPrize).OrderBy(num => num).ToList();

            iCanWin = rg2.Last();
            foreach(var vi in rg2.Select((v,i)=>new{v,i}))
            {
                if(vi.v!=vi.i)
                    break;
                iCantLoose = vi.i;
            }
        }

        private BigInteger Set(BigInteger num, int ibit)
        {
            return num | BigInteger.Pow(2, ibit);
        }

        private bool FSet(BigInteger num, int ibit)
        {
            return (num & BigInteger.Pow(2,ibit)) > 0;
        }

    }
}
