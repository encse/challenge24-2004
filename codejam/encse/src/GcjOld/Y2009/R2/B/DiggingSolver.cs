using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2009.R2.B
{
    public class DiggingSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public enum KCell
        {
            Rock,
            Hole,
            Nothing,
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The first line of input gives the number of cases, N. N test cases follow. The first line of each case is formatted as
             R C F
             where R is the number of rows in the cave, C is the number of columns in the cave, and F is the maximum distance you can fall without getting hurt.
             This is followed by R, rows each of which contains C characters. Each character can be one of two things:
             # for a solid rock
             . for an air hole
             The top-left cell will always be empty, and the cell below it will be a solid rock.
             */

            int crow, ccol, fall;
            pparser.Fetch(out crow, out ccol, out fall);
            var map = new ulong[crow];
            for(int irow =0;irow<crow;irow++)
            {
                string st = pparser.StLineNext();
                ulong maskRow = 0;
                for (int icol = ccol-1; icol >=0; icol--)
                {
                    maskRow <<= 1;
                    if(st[icol] == '#')
                        maskRow++;
                }
                map[irow] = maskRow;
            }
            var cave = new Cave(0,0,map, crow, ccol, fall, 0,0 );
            
            return () =>
                       {
                          // Console.Write(".");
                           
                           var f = Solve(cave).ToList();
                           Console.Write("#");
                           return f;
                       };

        }

       
      
        private IEnumerable<object> Solve(Cave cave)
        {

            var cDig = Find(cave);
            if (cDig == null)
            {
                yield return "No";
            }
            else
            {
                yield return "Yes " + cDig;
            }
        }

        private int? Find(Cave cave)
        {
            var astar = new Astar2<Cave, int>(
                new[] {cave},
                FEnd,
                EncaveGetNext,
                caveT => caveT.Cdig,
                null,
                caveT => -caveT.IrowPos); 
               ;
            var x = astar.Find();
            return x != null ? x.Item2 : (int?)null;
        }

        
        private int? Find2(Cave cave)
        {
            return BSearch(0, cave.Crow*2, minDst => FooA(cave, minDst));
        }

        private bool FooA(Cave cave, int minDst)
        {
            var cut = minDst + 1;
            return Foo2(cave, minDst, ref cut);
        }

        private int? BSearch(int a, int b, Func<int, bool> p)
        {
            if (!p(b))
                return null;
            
            /*    if (p(a))
                    return a;
                if (!p(b))
                    return -1;*/

            while (b - a > 1)
            {
                var m = (a + b) / 2;
                if (p(m))
                    b = m;
                else
                    a = m;
            }
            return b;
        }

        private bool Foo2(Cave cave, int? minDst, ref int cut)
        {
            int depth = cave.Cdig;

            if (FEnd(cave))
            {
                cut = Math.Min(cut, depth);
                if (minDst.HasValue)
                    return depth <= minDst;
                else
                    return false;
            }

            if (depth > cut || (minDst.HasValue && depth > minDst))
                return false;

            //rgsetT.Sort((setA, setB) => setA.Count() - setB.Count());
            //  rgsetT.Reverse();
            foreach (var caveNext in EncaveGetNext(cave, cave.Cdig)) 
            {
                if (Foo2(caveNext, minDst, ref cut))
                    return true;
            }

            return false;
        }
        private bool FEnd(Cave cave)
        {
            return cave.IrowPos == cave.Crow - 1;
        }
        
        private IEnumerable<Cave> EncaveGetNext(Cave cave, int distCur)
        {
            var caveNew = cave.Right();
            if(caveNew != null)
                yield return caveNew;
            caveNew = cave.Left();
            if(caveNew != null)
                yield return caveNew;

            caveNew = cave.DigLeft();
            if(caveNew != null)
                yield return caveNew;

            caveNew = cave.DigRight();
            if(caveNew != null)
                yield return caveNew;
        }

        public class Cave
        {
            public int IrowPos { get; private set; }
            public int IcolPos { get; private set; }
            private int cdigAbove;
            private int cdigCur;
            public int Cdig { get { return cdigAbove + cdigCur; } }

            public bool Equals(Cave other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (!(other.cdigAbove == cdigAbove && other.cdigCur == cdigCur && other.IrowPos == IrowPos && other.IcolPos == IcolPos))
                    return false;

                if (map[IrowPos] != other.map[IrowPos])
                    return false;
                if (IrowPos<Crow-1 && map[IrowPos+1] != other.map[IrowPos+1])
                    return false;
                
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Cave)) return false;
                return Equals((Cave) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    ulong result = (ulong)cdigAbove;
                    result = (result*397) ^ (ulong)cdigCur;
                    result = (result*397) ^ (ulong)IrowPos;
                    result = (result*397) ^ (ulong)IcolPos;
                    result = (result*397) ^ map[IrowPos];
                    if(IrowPos<Crow-1)
                        result = (result * 397) ^ map[IrowPos+1];
                    return (int)result;
                }
            }

            private readonly ulong[] map;
            private readonly int fallLimit;
            public readonly int Ccol;
            public readonly int Crow;
            public Cave(int irowPos, int icolPos, ulong[] map, int crow, int ccol, int fallLimit, int cdigAbove, int cdigCur)
            {
                this.IrowPos = irowPos;
                this.IcolPos = icolPos;
                this.map = map;
                this.fallLimit = fallLimit;
                this.cdigAbove = cdigAbove;
                this.cdigCur = cdigCur;
                this.Crow = crow;
                this.Ccol = ccol;
            }

            private Cave Clone()
            {
                var mapDst = new ulong[Crow];
                Array.Copy(map, 0, mapDst, 0, map.Length);
                return new Cave(IrowPos, IcolPos, mapDst, Crow, Ccol, fallLimit, cdigAbove, cdigCur);
            }

            public string Tsto()
            {
                var sb = new StringBuilder();
                for (int irow = 0; irow < Crow; irow++)
                {
                    for (int icol = 0; icol < Ccol; icol++)
                    {
                        sb.Append(IrowPos == irow && IcolPos == icol
                                      ? "%"
                                      : this[irow, icol] == KCell.Rock ? "#" : ".");

                    }
                    sb.AppendLine("");
                }
                return sb.ToString();
            }

            static readonly ulong[] bits;
            static Cave()
            {
                bits = new ulong[63];
                ulong b = 1;
                for(int i=0;i<63;i++)
                {
                    bits[i] = b;
                    b <<= 1;
                }
            }
            public KCell this[int irow, int icol]
            {
                get
                {
                    if(irow<0 || irow>=Crow || icol<0||icol>=Ccol )
                        return KCell.Nothing;
                    var bit = bits[icol];
                    if( (map[irow]&bit) != 0)
                        return KCell.Rock;
                    return KCell.Hole;
                }
                set
                {
                    var bit = bits[icol];
                    if(value == KCell.Rock)
                        map[irow] |= bit;
                    else if((map[irow] & bit) != 0)
                        map[irow] -= bit;
                }
            }

            private Cave DigSide(int irowDig, int icolDig)
            {
                if(this[irowDig, icolDig] != KCell.Rock)
                    return null;
                if(this[irowDig-1, icolDig] != KCell.Hole)
                    return null;
               
                var caveNew = Clone();
                caveNew[irowDig, icolDig] = KCell.Hole;
                caveNew.cdigCur++;
                return caveNew;
            }

            private Cave MoveSide(int irowPosNew, int icolPosNew)
            {
                if (this[irowPosNew, icolPosNew] != KCell.Hole)
                    return null;

                int irowPosAfterFall = Fall(irowPosNew, icolPosNew);
                if (irowPosAfterFall - irowPosNew > fallLimit)
                    return null;

                var caveNew = Clone();
                caveNew.IcolPos = icolPosNew;
                caveNew.IrowPos = irowPosAfterFall;
                if (irowPosNew != irowPosAfterFall)
                {
                    caveNew.cdigAbove += caveNew.cdigCur;
                    caveNew.cdigCur = 0;
                }

                return caveNew;
            }

            private int Fall(int irowT, int icolT)
            {
                while (this[irowT+1, icolT] == KCell.Hole)
                    irowT++;
                return irowT;
            }

            public Cave Left()
            {
                return MoveSide(IrowPos, IcolPos - 1);
            }

            public Cave Right()
            {
                return MoveSide(IrowPos, IcolPos + 1);

            }
         
            public Cave DigLeft()
            {
               return DigSide(IrowPos+1, IcolPos - 1);
            }


            public Cave DigRight()
            {
               return DigSide(IrowPos+1, IcolPos + 1);
            }
        }
    }

    class Heap<N>
    {
        private SortedDictionary<int, Queue<N>> items = new SortedDictionary<int, Queue<N>>();

        
        public int Count { get; private set; }
        public N RemoveMin()
        {
            Count--;
            var rgitem = items.First();
            var n = rgitem.Value.Dequeue();
            if (rgitem.Value.Count == 0)
                items.Remove(rgitem.Key);
            return n;
        }
        public void Add(int key, N n)
        {
            Count++;
            if(!items.ContainsKey(key))
                items[key] = new Queue<N>();
            items[key].Enqueue(n);
        }

        public void Remove(int key, N n)
        {
            if(items[key].Count == 1)
            {
                items.Remove(key);
                return;
                
            }
            var queueOld = items[key];
            items[key] = new Queue<N>();
            foreach (var n1 in queueOld)
            {
                if (!n1.Equals(n))
                    items[key].Enqueue(n1);
            }
            
        }
    }
    public class Astar2<N, T> where T : IComparable<T>
    {
        private readonly Hlm_Chewbacca<N> hlmChewbaccaDone = new Hlm_Chewbacca<N>();
        private readonly SortedDictionary<T, Heap<N>> mpActive2 = new SortedDictionary<T, Heap<N>>();
        private readonly Dictionary<N, T> mpttotalByN = new Dictionary<N, T>();
        private readonly Dictionary<N, T> mptByN = new Dictionary<N, T>();
        private readonly Func<N,bool> fEnd;
        private readonly Func<N, T, IEnumerable<N>> dgenNextGet;
        private readonly Func<N, T> tGet;
        private readonly Func<N,int> heur;
        private readonly Func<N, T, T> tGetMinTotal;


        public Astar2(IEnumerable<N> enprntStart, Func<N, bool> fEnd, Func<N, T, IEnumerable<N>> dgenNextGet, Func<N,T> tGet, Func<N, T, T> tGetMinTotal = null, Func<N,int> heur =null )
        {
            this.dgenNextGet = dgenNextGet;
            this.tGet = tGet;
            this.heur = heur;

            this.tGetMinTotal = tGetMinTotal ?? ((n, t) => t) ;

            foreach(var nStart in enprntStart)
            {
                var t = tGet(nStart);
                AddActive(nStart, this.tGetMinTotal(nStart,t), t);
            }

            this.fEnd = fEnd;
        }

        private void AddActive(N n, T tTotal, T t)
        {
            T tTotalOld;
            if(mpttotalByN.TryGetValue(n, out tTotalOld))
            {
                if(tTotalOld.CompareTo(tTotal) <= 0)
                    return;

                var hlmn = mpActive2[tTotalOld];
                hlmn.Remove(heur(n), n);
                if(hlmn.Count == 0)
                    mpActive2.Remove(tTotalOld);
            }
            mpttotalByN[n] = tTotal;
            mptByN[n] = t;
            mpActive2.EnsureGet(tTotal).Add(heur(n), n);
        }

        public Tuple<N, T> Find()
        {
            for(;;)
            {
                if(mpActive2.Count == 0)
                    return null;

                var kvpFirst = mpActive2.First();
                var ttotalFrom = kvpFirst.Key;
                var nFrom = kvpFirst.Value.RemoveMin();
                var tFrom = mptByN[nFrom];

                mptByN.Remove(nFrom);
                mpttotalByN.Remove(nFrom);

                if(kvpFirst.Value.Count == 0)
                    mpActive2.Remove(ttotalFrom);

                Debug.Assert(!hlmChewbaccaDone.Contains(nFrom));

                if(fEnd(nFrom))
                {
                    Debug.Assert(tFrom.CompareTo(ttotalFrom)==0);
                    return new Tuple<N, T>(nFrom, tFrom);
                }

 
                hlmChewbaccaDone.Add(nFrom);

                foreach(var nTo in dgenNextGet(nFrom, tFrom))
                {
                    if(hlmChewbaccaDone.Contains(nTo))
                        continue;

                    var t = tGet(nTo);
                    AddActive(nTo, tGetMinTotal(nTo,t), t);
                }
            }
        }

        private N MinGet(Hlm_Chewbacca<N> hlmChewbacca, Func<N,N,int> heur)
        {
            var nMin = hlmChewbacca.First();
            if (heur == null)
                return nMin;

            foreach (var nB in hlmChewbacca)
            {
                if(heur(nMin, nB)>0)
                    nMin = nB;
            }
            return nMin;


        }
    }

  
}