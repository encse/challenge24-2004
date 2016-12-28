using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2009.R2.C
{
    public class StockChartsSolver : IConcurrentSolver
    {
        public class Stock
        {
            public readonly int[] rgsample;

            public Stock(int[] rgsample)
            {
                this.rgsample = rgsample;
            }
        }
       
        class Set
        {
            private static BigInteger[] msk = new BigInteger[1000];

            static Set()
            {
                var x = new BigInteger(1);
                for(int i=0;i<1000;i++)
                {
                    msk[i] = x;
                    x *= 2;
                }
            }

            public readonly BigInteger members;
            private readonly int count;
            public int Count()
            {
              
                return count;
            }

            private Set(BigInteger members, int count)
            {
                this.members = members;
                this.count = count;
            }

            public Set Add(int i)
            {
                return new Set(members | msk[i], ((members&msk[i]) == 0) ? count+1 : count);
            }

            public Set Intersect(Set set)
            {
                var membersT = members & set.members;
                return new Set(membersT, CountI(membersT));
            }

            private int CountI(BigInteger membersT)
            {
                int c = 0;
                    
                while (membersT != 0)
                {
                    if (membersT % 2 == 1)
                        c++;
                    membersT >>= 1;
                }
                return c;
            }

            public bool FEmpty()
            {
                return members == 0;
            }

            public bool FContains(int i)
            {
                return (members | msk[i]) == members;
            }

            public static Set Create(int item)
            {
                return new Set(msk[item], 1);
            }

            public static Set Empty()
            {
                return new Set(0, 0);
            }

            public Set Sub(Set setA)
            {
                var membersT = members - (members & setA.members);
                return new Set(membersT, CountI(membersT));
            }

            public IEnumerable<int> Enitem()
            {
                int i = 0;
                var membersT = members;
                while(membersT != 0)
                {
                    if (membersT % 2 == 1)
                        yield return i;
                    i++;
                    membersT >>= 1;
                }
            }

            public bool FIntersect(Set set)
            {
                return (members & set.members) != 0;
            }
        }

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            int cstock, csample;
            pparser.Fetch(out cstock, out csample);

            var rgstock = new List<Stock>();
            for (int i = 0; i < cstock;i++)
                rgstock.Add(new Stock(pparser.Fetch<int[]>()));


            var mpSetUnlikesByItem = new Set[cstock];

            for (int i = 0; i < cstock; i++)
            {
                mpSetUnlikesByItem[i] = Set.Empty();
                for (int j = 0; j < cstock; j++)
                {
                    if (i == j) 
                        continue;
                    if (FUnlikes(rgstock[i], rgstock[j]))
                        mpSetUnlikesByItem[i] = mpSetUnlikesByItem[i].Add(j);
                }
            }
            return () =>
                       {
                          // Console.Write(".");
                           
                           var f = Solve(mpSetUnlikesByItem).ToList();
                           Console.Write("#");
                           return f;
                       };

        }

        private bool FUnlikes(Stock stockA, Stock stockB)
        {

            var stockLo = stockA.rgsample[0] <= stockB.rgsample[0] ? stockA : stockB;
            var stockHi = stockLo == stockA ? stockB : stockA;

            for (int i = 0; i < stockLo.rgsample.Length;i++ )
            {
                if (stockLo.rgsample[i] >= stockHi.rgsample[i])
                    return true;
            }
            return false;
        }

        private IEnumerable<object> Solve(Set[] mpSetUnlikesByItem)
        {
            //int minCur = mpSetUnlikesByItem.Length/10;
            int[] rgitem = new int[mpSetUnlikesByItem.Length];
            for (int i = 0; i < rgitem.Length;i++ )
            {
                rgitem[i] = i;
            }

            Array.Sort(rgitem, (itemA, itemB) => -mpSetUnlikesByItem[itemA].Count() + mpSetUnlikesByItem[itemB].Count());

            //yield return FooB(rgitem, mpSetUnlikesByItem);
            yield return BSearch(0, mpSetUnlikesByItem.Length, minDst => FooA(rgitem, mpSetUnlikesByItem, minDst));
           //var x = Bar(rgitem, mpSetUnlikesByItem);
           // yield return -1;
            //yield return BSearch(Math.Min(0, x - ((int)Math.Log(rgitem.Length) + 2)), x, minDst => FooA(rgitem, mpSetUnlikesByItem, minDst));

        }
        private int LSearch(int a, int b, Func<int, bool> p)
        {
            for(int i=b-1;i>=a;i--)
            {
                //if (p(i))
                //    return i;
                if (!p(i))
                    return i + 1;
              //  Console.Write("@");
            }
            return b;
            throw new Exception("wtf");
        }
        private int BSearch(int a, int b, Func<int, bool> p)
        {
        /*    if (p(a))
                return a;
            if (!p(b))
                return -1;*/

            while(b-a>1)
            {
                var m = (a + b)/2;
                if (p(m))
                    b = m;
                else
                    a = m;
            }
            return b;
        }

        private bool FooA(int[] rgitem, Set[] mpSetUnlikesByItem, int minDst)
        {
            var cut = minDst + 1;
            return Foo2(new List<Set>(), rgitem, mpSetUnlikesByItem, 0, minDst,ref cut);
        }

        private int FooB(int[] rgitem, Set[] mpSetUnlikesByItem)
        {
            var cut = rgitem.Length;
            Foo2(new List<Set>(), rgitem, mpSetUnlikesByItem, 0, null, ref cut);
            return cut;
        }

        private Random r = new Random();
        private bool Foo2(List<Set> rgset, int[] rgitem, Set[] mpSetUnlikesByItem, int pitemNext, int? minDst, ref int cut)
        {
            if (pitemNext == rgitem.Length)
            {

                cut = Math.Min(cut, rgset.Count);
                if(minDst.HasValue)
                    return rgset.Count <= minDst;
                else
                    return false;
            }

            if (rgset.Count > cut || (minDst.HasValue && rgset.Count > minDst))
                return false ;
            
            var setUnlikes = mpSetUnlikesByItem[rgitem[pitemNext]];
          

            try
            {
                rgset.Add(Set.Create(rgitem[pitemNext]));
                if (Foo2(rgset, rgitem, mpSetUnlikesByItem, pitemNext + 1, minDst, ref cut))
                    return true;

            }
            finally
            {
                rgset.RemoveAt(rgset.Count - 1);
            }

            var rgsetT = rgset;

            //rgsetT.Sort((setA, setB) => setA.Count() - setB.Count());
          //  rgsetT.Reverse();
            var ibase = r.Next(rgsetT.Count);
            for (int iX = 0; iX < rgsetT.Count; iX++)
            {
                var iset = (ibase + iX)%rgsetT.Count;
                if (!setUnlikes.FIntersect(rgsetT[iset]))
                {
                    var setOld = rgsetT[iset];
                    rgsetT[iset] = setOld.Add(rgitem[pitemNext]);

                    if (Foo2(rgsetT, rgitem, mpSetUnlikesByItem, pitemNext + 1, minDst, ref cut))
                        return true;

                    rgsetT[iset] = setOld;
                }
            }

            return false;
        }

        private int Bar(int[] rgitem, Set[] mpSetUnlikesByItem)
        {
           // Console.Write("[");
            int c = 0;
            var hlmItem = new Hlm_Chewbacca<int>(rgitem);

            var rgsetBig = new SortedDictionary<BigInteger, Set>();
            Set setMax = null;
            var cMax = 0;
            foreach (var item in hlmItem)
            {

                if (rgsetBig.Count == 0)
                {
                    setMax = Set.Create(item);
                    cMax = 1;
                    rgsetBig.Add(setMax.members, setMax);
                    continue;

                }
                var setUnlikes = mpSetUnlikesByItem[item];
                var rgsetNew = new SortedDictionary<BigInteger, Set>();
                foreach (var set in rgsetBig.Values)
                {
                    //var set = rgsetBig[i];
                    if (!setUnlikes.FIntersect(set))
                    {
                        var setT = set.Add(item);
                        rgsetNew.Add(setT.members, setT);
                        var cT = setT.Count();
                        if (cT > cMax)
                        {
                            setMax = setT;
                            cMax = cT;
                        }
                    }
                    else
                    {
                        var setNew = set.Sub(setUnlikes.Intersect(set)).Add(item);
                        rgsetNew[setNew.members] = setNew;
                        rgsetNew[set.members] = set;
                    }
                }
                rgsetBig = rgsetNew;


            }

            while (hlmItem.Any())
            {
                var setRemoved = setMax;
                foreach (var item in setMax.Enitem())
                    hlmItem.Remove(item);
                c++;

                setMax = null;
                cMax = 0;
                var rgsetNew = new SortedDictionary<BigInteger, Set>();
                foreach (var set in rgsetBig.Values)
                {
                    var setNew = set.Sub(setRemoved);
                    
                    rgsetNew[setNew.members] = setNew;
                    var cT = setNew.Count();
                    if (cT > cMax)
                    {
                        setMax = setNew;
                        cMax = cT;
                    }
                }
                rgsetBig = rgsetNew;

            }
          //  Console.Write("]");
            return c;
        }
    }
}