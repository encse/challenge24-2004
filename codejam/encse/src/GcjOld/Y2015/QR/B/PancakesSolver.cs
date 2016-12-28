using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.QR.B
{
    public class PancakesSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var d = pparser.Fetch<int>();
            var rgp = pparser.Fetch<int[]>();

          
            return () => Solve(rgp);
        }

        private IEnumerable<object> Solve(int[] rgp)
        {
            int[] v = new int[rgp.Max()+1];
            for (var i = 0; i < rgp.Length; i++)
                v[rgp[i]]++;
            var min = Solve2(v);

            yield return min;
        }

        private int[] Cut()
        {
            var s = new int[1001];
            var cut = new int[1001];
            var x = new int[1001];
            s[0] = 0;
            s[1] = 1;
            s[2] = 2;
            s[3] = 3;
            for(var p =4;p<cut.Length;p++)
            {
                cut[p] = 0;
                s[p] = p;
                x[p] = 0;
                  
                for (int cutT = 1; cutT < p/2+1; cutT++)
                {
                    int xT = 1 + x[cutT] + x[p - cutT];
                    int sT = 1 + Math.Max(s[cutT], s[p - cutT]);
                    if (sT < s[p] || (sT == s[p] && xT< x[p]))
                    {
                        x[p] = xT;
                        s[p] = sT;
                        cut[p] = cutT;
                    }
                }
            }

            return cut;
        }

        private class Cache2 : Dictionary<Tuple<int, int>, int>
        {
        }

        private int Solve2(int[] v)
        {
            var cache = new Cache2();

            var c = new int[v.Length];
            for (var p = 1; p < c.Length; p++)
            {
                if (v[p] == 0)
                {
                    c[p] = c[p - 1];
                    continue;
                }

                if (p < 4)
                {
                    c[p] = p;
                    continue;
                }

                c[p] = p;
                var d = v[p];
                for (var i = 1; i < p/2+1; i++)
                {
                    //ha az osszes d*p palacsintat szetosztanank egy d*i es d*(p-i) kupacra, akkor ennyi lenne c:
                    var cPI = Math.Max(c[p - 1], Opti(d, i, cache) + Opti(d, p - i, cache));
                    c[p] = Math.Min(cPI, c[p]);
                }
            }

            return c[v.Length - 1];
        }

        private int Opti(int d, int p, Cache2 cache)
        {
            var key = new Tuple<int, int>(d, p);
            if (cache.ContainsKey(key))
                return cache[key];

            var res = 0;
            return res;
        }

        private int SolveI(int imax, int[] v, Cache cache, int[] cut, int level)
        {
            if (imax < 0) 
                return 0;

            var ores = cache.TryGet(v);
            if (ores.HasValue)
                return ores.Value;
            var res = 0;

            for (var i = imax; i > 0; i--)
            {
                var p = i; //palacsintak szama a tanyeron
                var d = v[i]; //ennyi embernek van p palacsintaja
                if (d == 0)
                    continue;

                if (p <= 3)
                {
                    res = p; //mindenki megeszi p lepesben
                }
                else
                {
                    res = p;
                    var optCut = 0;
                    for (var pT = 1; pT < p/2+1; pT++)
                 //  var pT = cut[p];
                    {
                        var w = v.ToArray();
                        w[p]=0;
                        w[pT]+=d;
                        w[p-pT]+=d;
                        var resB = d + SolveI(p, w, cache, cut, level+1);

                        if (resB < res)
                        {
                            optCut = pT;
                        }
                        res = Math.Min(resB, res);
                    }

                    if(optCut != 0 && optCut != cut[p])
                    {
                        Console.Write("x");
                    }

                }
                break;
            }
           
            cache.Add(v, res);
            return res;
        }

        private class Cache
        {
            Node root = new Node();

            public Cache()
            {
                root.children = new Dictionary<int, Node>();
            }

            public int? TryGet(int[] v)
            {
                var node = root;
                for (var i = 0; i < v.Length; i++)
                {
                    if(!node.children.ContainsKey(v[i]))
                        return null;

                    node = node.children[v[i]];
                }
                return node.v;
            }

            public void Add(int[] v, int res)
            {
                var node = root;
                for (var i = 0; i < v.Length; i++)
                {
                    if (!node.children.ContainsKey(v[i]))
                    {
                        node.children[v[i]] = new Node();
                        if(i<v.Length-1)
                            node.children[v[i]].children = new Dictionary<int, Node>();
                    }

                    node = node.children[v[i]];
                }
                node.children = null;
                node.v = res;
            }
        }

        private class Node
        {
            public int v;
            public Dictionary<int, Node> children;
        }

    }
}
