using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Gcj.Util;
using Cmn.Util;
using System.Linq;

namespace Gcj.Y2013.R1A.C
{
    internal class GoodLuckParallelSolver : GcjSolver
    {
        private class Riddle : Dictionary<int, int>
        {
            public Riddle(IDictionary<int, int> mp) : base(mp)
            {
            }

            [Pure]
            public override string ToString()
            {
                return string.Format("{0} ({1})"
                    , this.Select(kvpcBynum => new string(kvpcBynum.Key.ToString()[0], kvpcBynum.Value)).StJoin(string.Empty)
                    , this.Select(kvpcBynum => string.Format("N:{0} C:{1}", kvpcBynum.Key, kvpcBynum.Value)).StJoin(", ")
                    );
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(this);
            }

            public override bool Equals(object obj)
            {
                return Object.ReferenceEquals(this, obj);
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cRiddle;
            int cNumber;
            int max;
            int cProduct;

            Fetch(out cRiddle, out cNumber, out max, out cProduct);

            var mprgriddleByProd = new Dictionary<BigInteger, List<Riddle>>();
            var rgriddleAll = new List<Riddle>();
            for(var rgnum = new int[cNumber].Select(_ => 1).ToArray(); rgnum != null;)
            {
                var product = rgnum.Aggregate((BigInteger) 1, (prod, num) => prod * num);
                var riddle = new Riddle(rgnum.Where(num => num != 1).GroupBy(num => num).ToDictionary(grp => grp.Key, grp => grp.Count()));
                mprgriddleByProd.EnsureGet(product).Add(riddle);

                if(riddle.Values.Sum() == cNumber)
                    rgriddleAll.Add(riddle);

                for(var inum = rgnum.Length - 1; inum >= 0; inum--)
                {
                    var num = rgnum[inum];
                    if(num < max)
                    {
                        for(var inum2 = inum; inum2 < rgnum.Length; inum2++)
                        {
                            rgnum[inum2] = num + 1;
                        }
                        break;
                    }
                    else if(inum == 0)
                    {
                        rgnum = null;
                    }
                }
            }

            var alatt = new Func<int, int, int>((n, k) =>
            {
                var c = 1;
                for(var i = 0; i < k; i++)
                    c *= (n - i);
                for(var i = 0; i < k; i++)
                    c /= (1 + i);
                return c;
            });

            var wNextGetI = new Func<BigInteger, Riddle, BigInteger>((prod, riddle) =>
            {
                var wNext = (BigInteger) 0;

                foreach(var riddleProd in mprgriddleByProd[prod])
                {
                    var wProd = 1;
                    foreach(var kvpcBynumProd in riddleProd)
                    {
                        var c = riddle.GetOrDefault(kvpcBynumProd.Key, 0);
                        if(c < kvpcBynumProd.Value)
                        {
                            wProd = 0;
                            break;
                        }

                        wProd *= alatt(c, kvpcBynumProd.Value);
                    }
                    wNext += wProd;
                }

                return wNext;
            });

            Info("Caching " + mprgriddleByProd.Count);
            var cac = new Dictionary<BigInteger, List<Tuple<Riddle, BigInteger>>>();
            Parallel.ForEach(mprgriddleByProd.Keys, prod =>
            {
                var rg = new List<Tuple<Riddle, BigInteger>>();
                foreach(var riddle in rgriddleAll)
                {
                    var w = wNextGetI(prod, riddle);
                    if(w > 0)
                        rg.Add(new Tuple<Riddle, BigInteger>(riddle, w));
                }
                lock(cac)
                {
                    cac.Add(prod, rg);
                }
            });

            Info("Caching end");

            var fact = new Func<int, BigInteger>(n =>
            {
                var c = (BigInteger) 1;
                for(var i = 1; i < n; i++)
                    c *= (1 + i);
                return c;
            });

            var rgrgprog = new int[cRiddle].Select(_ => Fetch<BigInteger[]>().Distinct().OrderByDescending(prod => prod).ToArray()).ToArray();

            var mpstSolByi = new Dictionary<int, string>();

            var mpwByriddleFirst = rgriddleAll.ToDictionary(riddle => riddle, riddle =>
            {
                var p = fact(cNumber);
                foreach(var kvpcBynum in riddle)
                {
                    Debug.Assert(p % fact(kvpcBynum.Value) == 0);
                    p /= fact(kvpcBynum.Value);
                }
                return p;
            });

            Parallel.ForEach(rgrgprog.Select((v, i) => new {v, i}), virgprod =>
            {
                var rgprod = virgprod.v;

                var mpwByriddle = new Dictionary<Riddle, BigInteger>(mpwByriddleFirst);

                foreach(var prod in rgprod)
                {
                    var mpwByRiddleNext = new Dictionary<Riddle, BigInteger>();

                    foreach(var tpl in cac[prod])
                    {
                        var w = mpwByriddle.GetOrDefault(tpl.Item1, 0);
                        if(w > 0)
                            mpwByRiddleNext[tpl.Item1] = w * tpl.Item2;
                    }

                    mpwByriddle = mpwByRiddleNext;

                    if(mpwByriddle.Count == 1)
                        break;
                }

                var stsol = mpwByriddle.OrderByDescending(kvp => kvp.Value).First().Key.SelectMany(kvp => new int[kvp.Value].Select(_ => kvp.Key)).Select(num => num.ToString()).StJoin(string.Empty);
                lock(mpstSolByi)
                {
                    mpstSolByi[virgprod.i] = stsol;
                }
            });
            for(var iRiddle = 0; iRiddle < cRiddle; iRiddle++)
            {
                yield return Solwrt.NewLine;
                var stsol = mpstSolByi[iRiddle];
                yield return stsol;
            }
        }
    }
}
