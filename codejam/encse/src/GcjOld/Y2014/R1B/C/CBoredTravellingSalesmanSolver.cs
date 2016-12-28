using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.C
{
    public class CBoredTravellingSalesmanSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int ccity, cpath;
            pparser.Fetch(out ccity, out cpath);

            var rgcity = new City[ccity];
            for (int i = 0; i < ccity; i++)
                rgcity[i] = new City {rgcityNeighbour = new List<City>(), zip = pparser.Fetch<string>()};

            for(int i=0;i<cpath;i++)
            {
                int icityA, icityB;
                pparser.Fetch(out icityA, out icityB);
                icityA--;
                icityB--;


                rgcity[icityA].rgcityNeighbour.Add(rgcity[icityB]);
                rgcity[icityB].rgcityNeighbour.Add(rgcity[icityA]);
            }

            return () => Solve(rgcity);
        }

        private IEnumerable<object> Solve(City[] rgcity)
        {
            foreach (var city in rgcity.OrderBy(x => x.zip))
            {
                var zip = ZipBejar(new M(city), rgcity);
                if (zip != null)
                {
                    yield return zip;
                    yield break;
                }
            }
        }

        class City
        {
            public List<City> rgcityNeighbour;
            public string zip;
        }

        class M
        {
            public Stack<City> vrmcityAbove = new Stack<City>();
            public HashSet<City> hlmcityVisited = new HashSet<City>();
            public string zip="";

            public M(City cityStart)
            {
                vrmcityAbove.Push(cityStart);
                hlmcityVisited.Add(cityStart);
                zip = cityStart.zip;
            }
            private M() { }
            public M MRecurse(City city)
            {
                var m = new M {vrmcityAbove = new Stack<City>(vrmcityAbove.Reverse()), hlmcityVisited = new HashSet<City>(hlmcityVisited), zip = zip};
                
                while (!m.vrmcityAbove.Peek().rgcityNeighbour.Contains(city))
                    m.vrmcityAbove.Pop();

                m.hlmcityVisited.Add(city);
                m.vrmcityAbove.Push(city);
                m.zip += city.zip;
                return m;
            }
        }

        private string ZipBejar(M m, City[] rgcity)
        {
            if (m.hlmcityVisited.Count == rgcity.Length)
                return m.zip;

            foreach (var city in RgcityNext(m))
            {
                var mChild = m.MRecurse(city);
                if (FDeadend(mChild, rgcity.Length))
                    continue;
       
                var zip = ZipBejar(mChild, rgcity);
                if (zip != null)
                    return zip;
            }

            return null;
        }

        private IEnumerable<City> RgcityNext(M m)
        {
            var hlmNext = new HashSet<City>();
            foreach (var city in m.vrmcityAbove)
                foreach (var cityT in city.rgcityNeighbour.Where(cityT => !m.hlmcityVisited.Contains(cityT)))
                    hlmNext.Add(cityT);

            return hlmNext.OrderBy(city => city.zip);
        }

        private bool FDeadend(M m, int ccity)
        {
            var qcity = new Queue<City>(m.vrmcityAbove);

            var hlmCitySeen = new HashSet<City>(m.vrmcityAbove);
            
            while (qcity.Any())
            {
                var city = qcity.Dequeue();

                foreach (var cityN in city.rgcityNeighbour.Where(cityN => !m.hlmcityVisited.Contains(cityN)))
                {
                    if (!hlmCitySeen.Contains(cityN))
                    {
                        qcity.Enqueue(cityN);
                        hlmCitySeen.Add(cityN);
                    }
                }
            }

            hlmCitySeen.UnionWith(m.hlmcityVisited);
            return hlmCitySeen.Count != ccity;
        }

    }
}
