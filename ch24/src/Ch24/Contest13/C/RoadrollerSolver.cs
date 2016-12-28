using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.C
{
    public partial class RoadrollerSolver : Solver
    {
        private class Pont : IComparable<Pont>
        {
            public int x;
            public int y;

            public int CompareTo(Pont other)
            {
                int d;
                d = x - other.x;
                if(d!=0)
                    return d;

                return y - other.y;
            }

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}", x, y);
            }
        }

        private Func<int, int, int>[] rgdiscGet = new[]
        {
            new Func<int, int, int>((x, y) => x),
            (x, y) => y,
            (x, y) => x-y,
            (x, y) => x+y,
        };

        public override void Solve()
        {
            var cpont = Fetch<int>();

            var rgpont = new SortedSet<Pont>();
            for(int ipont=0;ipont<cpont;ipont++)
            {
                var rgk = Fetch<int[]>();
                rgpont.Add(new Pont{x=rgk[0],y=rgk[1]});
            }

            var rgpontSol = new List<Pont>();

            Pont pontLast = null;
            
            var rgmprgpontByDisc = rgdiscGet.Select(_ => new Dictionary<int, SortedSet<Pont>>()).ToArray();

            foreach(var pont in rgpont)
            {
                for(int i = 0; i < rgmprgpontByDisc.Length; i++)
                {
                    var rgpontT = rgmprgpontByDisc[i].EnsureGet(rgdiscGet[i](pont.x, pont.y));
                    rgpontT.Add(pont);

                }
            }
            
            for(;rgpont.Any();)
            {
                Info(rgpont.Count);
                var rgpontMax = RgpontMaxGet(rgmprgpontByDisc);

                var pontFirst = rgpontMax.Min;
                rgpontSol.AddRange(rgpontGetWay(pontLast, pontFirst));

                pontLast = rgpontMax.Max;
                rgpontSol.Add(pontFirst);
                if(pontLast!=pontFirst)
                    rgpontSol.Add(pontLast);


                rgpont.ExceptWith(rgpontMax);
            }
            Score = rgpontSol.Count - 1;
            using(Output)
            {
                foreach(var pont in rgpontSol)
                {
                    WriteLine(new []{pont.x,pont.y});
                }
            }

        }

        private SortedSet<Pont> RgpontMaxGet(Dictionary<int, SortedSet<Pont>>[] rgmprgpontByDisc)
        {
            SortedSet<Pont> rgpontMax = null;
            foreach(var mprgpontByDisc in rgmprgpontByDisc)
            {
                foreach(var rgpont in mprgpontByDisc.Values)
                {
                    if(rgpontMax == null || rgpont.Count > rgpontMax.Count)
                        rgpontMax = rgpont;
                }
            }
            rgpontMax = new SortedSet<Pont>(rgpontMax);
            foreach(var mprgpontByDisc in rgmprgpontByDisc)
            {
                foreach(var kvrgpontByDisc in mprgpontByDisc.ToArray())
                {
                    kvrgpontByDisc.Value.ExceptWith(rgpontMax);
                    if(!kvrgpontByDisc.Value.Any())
                        mprgpontByDisc.Remove(kvrgpontByDisc.Key);
                }
            }
            return rgpontMax;
        }

        private List<Pont> rgpontGetWay(Pont pontFrom, Pont pontTo)
        {
            var rgpont = new List<Pont>();
            if(pontFrom != null && rgdiscGet.All( discGet => discGet(pontFrom.x,pontFrom.y) != discGet(pontTo.x,pontTo.y)))
                rgpont.Add(new Pont{x=pontTo.x,y=pontFrom.y});
            return rgpont;
        }

    }
}
