using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.B
{
    internal class NewLotteryGameSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            BigInteger afirst;
            BigInteger bfirst;
            BigInteger kfirst;
            Fetch(out afirst, out bfirst, out kfirst);

            var rga = new List<int>();
            var rgb = new List<int>();
            var rgk = new List<int>();

            for(;;)
            {
                if(afirst == 0 && bfirst == 0 && kfirst == 0)
                    break;

                rga.Add((int) (afirst % 2));
                rgb.Add((int) (bfirst % 2));
                rgk.Add((int) (kfirst % 2));
                
                afirst /= 2;
                bfirst /= 2;
                kfirst /= 2;
            }
            rga.Reverse();
            rgb.Reverse();
            rgk.Reverse();


            var l = rga.Count;

            var rgpra = RgprGet(rga);
            var rgprb = RgprGet(rgb);
            var rgprk = RgprGet(rgk);

            Func<int, int, int, int> q = (abit, bbit, kbit) => (abit & bbit) == kbit ? 1 : 0;

            yield return (
                from pra in rgpra
                from prb in rgprb
                from prk in rgprk
                select l.Eni().Select(i =>
                {
                    if(i < prk.Count)
                        if(i < pra.Count)
                        {
                            if(i < prb.Count)
                                return q(pra[i], prb[i], prk[i]);
                            else
                                return q(pra[i], 0, prk[i]) + q(pra[i], 1, prk[i]);

                        }
                        else
                        {
                            if(i < prb.Count)
                                return q(0, prb[i], prk[i]) + q(1, prb[i], prk[i]);
                            else
                                return prk[i] == 1 ? 1 : 3;
                        }
                    else
                    {
                        if(i < pra.Count)
                        {
                            if(i < prb.Count)
                                return 1;
                            else
                                return 2;

                        }
                        else
                        {
                            if(i < prb.Count)
                                return 2;
                            else
                                return 4;
                        }
                    }
                }).Select(x => (BigInteger)x).Mul())
                .Sum();
        }

        private static List<List<int>> RgprGet(List<int> rga)
        {
            return rga
                .Select((v, i) => new {v, i})
                .Where(vi => vi.v == 1)
                .Select(vi => rga.Take(vi.i).Concat(new[] {0}).ToList())
                .ToList();
        }
    }
}
