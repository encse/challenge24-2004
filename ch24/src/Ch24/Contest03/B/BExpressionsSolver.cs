using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest03.B
{
    public class BExpressionsSolver : Solver
    {

        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            var l = pparser.Fetch<int>();

            var mp = new Dictionary<int, SortedDictionary<string, string>>();
            Foo(l, mp);
            using (Output)
            {
                var rgst = Enst(mp, l).ToList();
                rgst.Sort(StringComparer.Ordinal);
                foreach (var st in rgst)
                {
                  //  Console.WriteLine(st);
                    Solwrt.WriteLine(st);
                }    
            }
            

        }

        void Foo(int l, Dictionary<int, SortedDictionary<string, string>> mp)
        {
            if (l > 1)
            {
                Foo(l - 1, mp);
            }

            if (l == 1)
            {
                Add(mp, "x");
                Add(mp, "y");
            }

            foreach (var st in Enst(mp, l-2))
                Add(mp, "(" + st + ")");
            
            for(int i=1;i<=l-2;i++)
            {
                foreach (var stA in Enst(mp, i))
                {
                    foreach (var stB in Enst(mp, l - i - 1))
                    {
                        Add(mp, stA + "+" + stB);
                        Add(mp, stA + "-" + stB);
                        Add(mp, stA + "*" + stB);
                        Add(mp, stA + "/" + stB);
                    }
                }
            }

            foreach (var st in Enst(mp, l - 5))
            {
                Add(mp, "sin(" + st + ")");
                Add(mp, "cos(" + st + ")");
            }
        }

        private IEnumerable<string> Enst(Dictionary<int, SortedDictionary<string, string>> mp, int i)
        {
            if (!mp.ContainsKey(i))
                return Enumerable.Empty<string>();
            return mp[i].Values;
        }

        void Add(Dictionary<int, SortedDictionary<string, string>> mp, string st)
        {
            if (!mp.ContainsKey(st.Length))
                mp[st.Length] = new SortedDictionary<string, string>();
           if (!mp[st.Length].ContainsKey(st))
                mp[st.Length].Add(st, st);
        }
    }
}


