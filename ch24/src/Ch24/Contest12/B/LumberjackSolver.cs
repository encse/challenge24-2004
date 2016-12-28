using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest12.B
{
    
    public class BMines : Solver
    {
        public override void Solve()
        {
            var n = Fetch<int>();
            var rgmine = FetchN<Mine>(n);
            for (int imine = 0; imine < n; imine++)
                rgmine[imine].imine = imine;

            Console.Write(".");
            var mm = MMGet(rgmine);
            Console.Write(".");
            return;
            ;
            var rgfDetonated = new bool[n];
            var cDetonated = 0;
            int i = 0;
            while(cDetonated < n)
            {
                i++;
                Mine mineBest = null;
                var cBest = -1;

                var rgfDetonatedT = (bool[])rgfDetonated.Clone();
                foreach(var mine in rgmine)
                {
                    if (rgfDetonatedT[mine.imine])
                        continue;

                    var rgfDetonatedTT = (bool[]) rgfDetonated.Clone();
                    var c = Detonate(mm, mine, rgfDetonatedTT);
                    for (int j = 0; j < rgfDetonatedTT.Length;j++ )
                    {
                        rgfDetonatedT[j] |= rgfDetonatedTT[j];
                    }
                    if (c >= cBest)
                    {
                        mineBest = mine;
                        cBest = c;
                    }
                }

                cDetonated += Detonate(mm, mineBest, rgfDetonated);
               // Console.WriteLine(mineBest.imine);
            }
            Console.WriteLine(i);

        }
        class MM
        {
            private Dictionary<Mine, List<Mine>> mprgmineBymine = new Dictionary<Mine, List<Mine>>();

            public void AddVertex(Mine mine)
            {
                if(!mprgmineBymine.ContainsKey(mine))
                    mprgmineBymine[mine] = new List<Mine>();
            }

            public void AddEdge(Mine mineA, Mine mineB)
            {
                mprgmineBymine[mineA].Add(mineB);
            }

            public IEnumerable<Mine> OutEdges(Mine mine)
            {
                return mprgmineBymine[mine];
            }
        }

        private MM MMGet(List<Mine> rgmine)
        {
            var mm = new MM();
            foreach (var mine in rgmine)
                mm.AddVertex(mine);

            var foo = from mine in rgmine group mine by mine.X into g select new {X=g.Key, Mines = g};
            var bar = foo.OrderBy(fooT => fooT.X).ToArray();

            foreach (var mineA in rgmine)
            {
                var iMin = Bsrc.Find(0, bar.Length - 1, j => bar[j].X >= mineA.X || Math.Abs(mineA.X - bar[j].X) <= mineA.R).Value;
                iMin = Math.Max(0, iMin - 1);

                for (int i = iMin; i < bar.Length; i++)
                {
                    if (Math.Abs(mineA.X - bar[i].X) > mineA.R && i != iMin)
                        break;


                    foreach (var mineB in bar[i].Mines)
                    {
                        if (mineA == mineB)
                            continue;


                        var dist = Math.Sqrt((mineA.X - mineB.X)*(mineA.X - mineB.X) + (mineA.Y - mineB.Y)*(mineA.Y - mineB.Y));

                        if (dist <= mineA.R)
                        {
                            mm.AddEdge(mineA, mineB);


                        }
                    }
                }
            }

            return mm;
        }

        private Dictionary<XXX, int> cache = new Dictionary<XXX, int>();
        class XXX
        {
            private bool[] rgf;

            public XXX(bool[] rgf)
            {
                this.rgf = rgf;
            }

            public bool Equals(XXX other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                return rgf.SequenceEqual(other.rgf);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (XXX)) return false;
                return Equals((XXX) obj);
            }

            public override int GetHashCode()
            {
                return rgf.OrderBy(x => x).Aggregate(17, (current, val) => current*23 + val.GetHashCode());
            }
        }

        private int Detonate(MM mm, Mine mine, bool[] rgfDetonated)
        {
            if (rgfDetonated[mine.imine])
                return 0;
              
            rgfDetonated[mine.imine] = true;
            var c = 1;
            foreach (var mineB in mm.OutEdges(mine))
                c += Detonate(mm, mineB, rgfDetonated);
            return c;
        }

        public class Mine
        {
            public long X, Y, R;
            public int imine;
            public Mine(int x, int y, int r)
            {
                X = x;
                Y = y;
                R = r;
            }
        }
    }

   
}
