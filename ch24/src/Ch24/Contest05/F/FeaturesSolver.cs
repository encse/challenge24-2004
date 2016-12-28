using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest05.F
{
    class FeaturesSolver : Solver
    {
        public override void Solve()
        {
            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                int cfec;
                int ifec;
                List<int> rgcost;
                Pparser.Fetch(out cfec, out ifec, out rgcost);

                while (cfec != 0)
                {
                    rgcost.Sort();
                   // Sanity(rgcost);
                    var cost = CostByIfec(ifec, rgcost);
                    log.Info(cost);
                    solwrt.WriteLine(cost);
                    Pparser.Fetch(out cfec, out ifec, out rgcost);
                }
            }
        }

        void Sanity(List<int> rgcost)
        {
            var rgfem = new List<int>();
            int costPrev = -1;
            foreach (var fec in Enfec(rgcost))
            {
                Debug.Assert(costPrev <= fec.Cost);
                rgfem.Add(fec.Fem);
                costPrev = fec.Cost;
            }
            rgfem.Sort();
            Debug.Assert(rgfem.Count == (1 << rgcost.Count));
            for(int i=0;i< (1 << rgcost.Count); i++)
            {
                Debug.Assert(rgfem[i] == i);
            }
        }

        private int CostByIfec(int ifec, List<int> rgcost)
        {
            var ifecCur = 0;
            foreach (var fec in Enfec(rgcost))
            {
                if (ifecCur == ifec)
                    return fec.Cost;
                ifecCur++;
            }
            throw new Exception();
        }

        private IEnumerable<Fec> Enfec(List<int> rgcost)
        {
            /*
             *  fe: feature
             *  fec: feature combination with cost
             *  fem: feature mask (a selected feature set)
             *  ife: i'th feature
             *  cfe: number of features
             */
            int cfe = rgcost.Count;

            var fecr = new Fecr();
            int femLim = 1 << cfe;

            fecr.Add(new Fec(0, 0));

            while (fecr.Any())
            {

                var fecCur = fecr.FecPick();
                yield return fecCur;

                var femNext = fecCur.Fem + 1;
                if (femNext < femLim)
                {
                    var costNext = CostFromFem(femNext, rgcost);
                    if (costNext >= fecCur.Cost)
                        fecr.Add(new Fec(femNext, costNext));
                    //otherwise we already seen that fem
                }

                if (fecCur.Fem > 0)
                {
                    var ifeCheapest = IfeCheapestFromFem(fecCur.Fem);
                    int ifeNext = ifeCheapest + 1;

                    if (!FSelected(fecCur.Fem, ifeNext))
                    {
                        var femToTheRight = fecCur.Fem + (1 << ifeCheapest);
                        if (femToTheRight < femLim)
                        {
                            var cost = CostFromFem(femToTheRight, rgcost);
                            if (cost >= fecCur.Cost && femNext != femToTheRight && rgcost[ifeNext] <= CostFromFem(FemEachIfeBefore(ifeNext), rgcost))
                                fecr.Add(new Fec(femToTheRight, cost));
                        }
                    }
                }
            }
        }


        private class Fecr
        {
            private readonly SortedDictionary<int, List<Fec>> mprgfecBycost = new SortedDictionary<int, List<Fec>>();

            public Fec FecPick()
            {
                var kvpRgfecAndCost = mprgfecBycost.First();
                var rgfec = kvpRgfecAndCost.Value;
                var cost = kvpRgfecAndCost.Key;

                var fec = rgfec.Last();

                rgfec.RemoveAt(rgfec.Count - 1);
                if (rgfec.Count == 0)
                    mprgfecBycost.Remove(cost);
                return fec;
            }

            public void Add(Fec fec)
            {
                if (!mprgfecBycost.ContainsKey(fec.Cost))
                    mprgfecBycost.Add(fec.Cost, new List<Fec> { fec });
                else
                    mprgfecBycost[fec.Cost].Add(fec);
            }

            public bool Any()
            {
                return mprgfecBycost.Any();
            }
        }

        private bool FSelected(int fem, int ife)
        {
            return (fem & (1 << ife)) != 0;
        }

        private int FemEachIfeBefore(int ife)
        {
            return (1 << (ife)) - 1;
        }

        private int IfeCheapestFromFem(int fem)
        {
            var ife = 0;
            while (fem > 0)
            {
                if (fem % 2 == 1)
                    return ife;
                ife++;
                fem >>= 1;
            }
            throw new Exception();
        }

        private class Fec
        {
            public readonly int Fem;
            public readonly int Cost;

            public Fec(int fem, int cost)
            {
                Fem = fem;
                Cost = cost;
            }
        }

        private int CostFromFem(int fem, List<int> rgcost)
        {
            var cost = 0;
            var ife = 0;
            while (fem > 0)
            {
                if (fem % 2 != 0)
                    cost += rgcost[ife];
                ife++;
                fem >>= 1;
            }

            return cost;
        }
    }
}
