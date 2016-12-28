using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.R1B.A
{
    public class FileFixitSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cdirExists;
            int cdirToCreate;
            Pparser.Fetch(out cdirExists, out cdirToCreate);
            var rgdpatExists = Pparser.FetchN<string>(cdirExists).ToArray();
            var rgdpatToCreate = Pparser.FetchN<string>(cdirToCreate).ToArray();
            var ndRoot = new Nd("<root>");
            AddRgdpat(ndRoot, rgdpatExists);
            var cndCreated = AddRgdpat(ndRoot, rgdpatToCreate);
            yield return cndCreated;
        }

        private int AddRgdpat(Nd ndRoot, IEnumerable<string> rgdpat)
        {
            int cndCreated = 0;
            foreach (var dpat in rgdpat)
            {
                var nd = ndRoot;
                foreach(var dirn in dpat.Split('/').Skip(1))
                {
                    if(!nd.FHasChild(dirn))
                    {
                        nd.AddNd(new Nd(dirn));
                        cndCreated++;
                    }
                    nd = nd.NdChild(dirn);
                }
            }
            return cndCreated;
        }

        public class Nd
        {
            private readonly Dictionary<string, Nd> mpndChildrenBydirn  = new Dictionary<string, Nd>();
            public readonly string Dirn;

            public Nd(string dirn)
            {
                Dirn = dirn;
            }
           
            public bool FHasChild(string dirn)
            {
                return mpndChildrenBydirn.ContainsKey(dirn);
            }

            public void AddNd(Nd nd)
            {
                mpndChildrenBydirn.Add(nd.Dirn, nd);
            }

            public Nd NdChild(string dirn)
            {
                return mpndChildrenBydirn[dirn];
            }
        }
    }
}
