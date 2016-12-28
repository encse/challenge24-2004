using System;
using System.Collections.Generic;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest05.F
{
    internal class Features3Solver : Solver
    {

        public override void Solve()
        {
            int cfec;
            int ifec;
            List<int> rgcost;
            Pparser.Fetch(out cfec, out ifec, out rgcost);

            using (Output)
            {
                while (cfec != 0)
                {
                    rgcost.Sort();
                    rgcost.Reverse();
                    // Sanity(rgcost);
                    var cost = CostByIfec(ifec, rgcost.ToArray());
                    log.Info(cost);
                    Solwrt.WriteLine(cost);
                    Pparser.Fetch(out cfec, out ifec, out rgcost);
                }
            }
        }

        private object CostByIfec(int ifec, int[] rgcost)
        {
            var cache = new Dictionary<Tuple<int, int>, int>();
            return Bsrc.Find(1, cost => CfecFromCost(cost, rgcost, 0, 0, cache) >= ifec);
        }


        int CfecFromCost(int costMax, int[] rgcost, int fem, int ifepFirst, Dictionary<Tuple<int,int>, int> cache)
        {
            int cfec;
            if (cache.TryGetValue(new Tuple<int, int>(ifepFirst, costMax), out  cfec))
                return cfec;

            cfec = 0;

            for (int ifep = ifepFirst; ifep < rgcost.Length; ifep++)
            {
                int mask = 1 << ifep;
                if (costMax >= rgcost[ifep] && (fem & mask) == 0 )
                {
                    fem ^= mask;
                    int cfecT = CfecFromCost(costMax - rgcost[ifep], rgcost, fem, ifep + 1, cache);
                   
                    cfec += cfecT+1;
                    fem ^= mask;
                }
            }

            cache[new Tuple<int, int>(ifepFirst, costMax)] = cfec;

            return cfec;
        }
    }
}
