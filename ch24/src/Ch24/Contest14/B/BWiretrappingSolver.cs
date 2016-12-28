using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest14.B
{
    public class BWiretrappingSolver : Contest.Solver
    {
        public class Node
        {
            public int i;
            public List<int> rgi;

        }

            List<int> rgi;

        public override void Solve()
        {
            var rgne = Fetch<int[]>();

            var mpnodeByI =rgne[0].Eni().ToDictionary(i => i, i => new Node {i = i, rgi = new List<int>()});

            var ia = -1;
            var ib = -1;
            foreach(var i in rgne[1].Eni())
            {
                var rgab = Fetch<int[]>();

                if(i == 0)
                {
                    ia = rgab[0];
                    ib = rgab[1];
                }

                mpnodeByI[rgab[0]].rgi.Add(rgab[1]);
                mpnodeByI[rgab[1]].rgi.Add(rgab[0]);
            }

            rgi = mpnodeByI.OrderBy(kvp => kvp.Value.rgi.Count).Select(kvp => kvp.Key).ToList();

            Check(mpnodeByI);

            var removed = removeE(mpnodeByI, ia, ib);
            var cremoved = Ctree(removed);

            var merged = merge(mpnodeByI, ia, ib);
            var cmerged = Ctree(merged);

            var p = ((double) cmerged) / (cmerged + cremoved);

            using(Output)
            {
                NufDouble = "0.######";

                WriteLine(p);
            }

        }

        public int Ctree(Dictionary<int, Node> mpnodeByIT)
        {
            var mpnodeByI = clone(mpnodeByIT);

            if(mpnodeByI.Count == 1)
                return 1;
            if(mpnodeByI.Count == 2)
                return mpnodeByI.First().Value.rgi.Count;

            int ia;
            Node a;
            for(;;)
            {
                ia = rgi.First();
                a = mpnodeByI.GetOrDefault(ia, null);
                if(a == null)
                {
                    rgi.RemoveAt(0);
                }
                else
                    break;
            }
            var ib = a.rgi.First();

            var ctree = 0;

            var c = mpnodeByI[ia].rgi.Count(i => i == ib);
            var removed = removeE(mpnodeByI, ia, ib);
            if(removed[ia].rgi.Count != 0 && removed[ib].rgi.Count != 0)
            {
                ctree += Ctree(removed);
            }

            removed = addE(removed, ia, ib, c);

            if(removed[ia].rgi.Count != 0 || removed[ib].rgi.Count != 0)
            {
                var merged = merge(mpnodeByI, ia, ib);
                if(merged[ia].rgi.Count != 0)
                    ctree += c * Ctree(merged);
            }
            return ctree;
        }

        public Dictionary<int, Node> clone(Dictionary<int, Node> mpnodeByI)
        {
            var dictionary = mpnodeByI.ToDictionary(kvp => kvp.Key, kvp => new Node {i = kvp.Value.i, rgi = new List<int>(kvp.Value.rgi)});
            Check(dictionary);
            return dictionary;
        }

        public Dictionary<int, Node> removeE(Dictionary<int, Node> mpnodeByIT, int ia, int ib)
        {
            //if(ia == ib)
            //    throw new Exception();
            //if(!mpnodeByIT[ia].rgi.Contains(ib))
            //    throw new Exception();
            //if(!mpnodeByIT[ib].rgi.Contains(ia))
            //    throw new Exception();

            var mpnodeByI = mpnodeByIT;
            mpnodeByI[ia].rgi = mpnodeByI[ia].rgi.Where(i => i != ib).ToList();
            mpnodeByI[ib].rgi = mpnodeByI[ib].rgi.Where(i => i != ia).ToList();
            Check(mpnodeByI);
            return mpnodeByI;
        }

        public Dictionary<int, Node> addE(Dictionary<int, Node> mpnodeByIT, int ia, int ib, int c)
        {
            //if(ia == ib)
            //    throw new Exception();
            //if(mpnodeByIT[ia].rgi.Contains(ib))
            //    throw new Exception();
            //if(mpnodeByIT[ib].rgi.Contains(ia))
            //    throw new Exception();

            var mpnodeByI = mpnodeByIT;
            for(int i = 0; i < c; i++)
            {
                mpnodeByI[ia].rgi.Add(ib);
                mpnodeByI[ib].rgi.Add(ia);
            }

            Check(mpnodeByI);
            return mpnodeByI;
        }

        public Dictionary<int, Node> merge(Dictionary<int, Node> mpnodeByIT, int ia, int ib)
        {
            //if(ia == ib)
            //    throw new Exception();
            //if(!mpnodeByIT[ia].rgi.Contains(ib))
            //    throw new Exception();
            //if(!mpnodeByIT[ib].rgi.Contains(ia))
            //    throw new Exception();


            var mpnodeByI = mpnodeByIT;
            var b = mpnodeByI[ib];
            mpnodeByI.Remove(ib);

            foreach(var ic in b.rgi.Distinct())
            {
                var c = mpnodeByI[ic];
                if(ic != ia)
                    c.rgi = c.rgi.Select(i => i == ib ? ia : i).ToList();
                else
                    c.rgi = c.rgi.Where(i => i != ib).Concat(b.rgi.Where(i => i != ia)).ToList();
            }

            Check(mpnodeByI);
            return mpnodeByI;
        }

        public void Check(Dictionary<int, Node> mpnodeByI)
        {
            return;
            foreach(var kvp in mpnodeByI)
            {
                if(kvp.Key != kvp.Value.i)
                    throw new Exception();
                foreach(var i in kvp.Value.rgi)
                {
                    if(i == kvp.Key)
                        throw new Exception();
                    if(!mpnodeByI.ContainsKey(i))
                        throw new Exception();
                    if(mpnodeByI[i].rgi.Count(i2 => i2 == kvp.Key) != kvp.Value.rgi.Count(i2 => i2 == i))
                        throw new Exception();
                }
            }
        }
    }
}
