using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1A.A
{
    internal class ChargingChaosSolver : GcjSolver
    {
        private class Gnd : IRanked<int>, IAstarNd<int>
        {
            public int d;
            public List<Ndp> rgndp;

            public int Rank
            {
                get
                {
                    return -d;
                }
            }

            public int DistFromStart { get; set; }
        }

        private class Ndp
        {
            public Nd ndFlow;
            public Nd ndTarget;
        }

        private class Nd
        {
            public Dictionary<char,Nd> mpndByCh = new Dictionary<char, Nd>();
            public int c0;
            public int c1;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            Fetch<int[]>();

            var rgconfFlow = Fetch<string[]>();
            var ndFlowRoot = fill(rgconfFlow);

            var rgconfTarget = Fetch<string[]>();
            var ndTargetRoot = fill(rgconfTarget);
            Debug.Assert(rgconfFlow.Length == rgconfTarget.Length);
            var l = rgconfTarget.Select(st => st.Length).Distinct().Single();

            var nodiResult = Astar2.FindRanked<Gnd,int,int>(
                new Gnd {d = 0, rgndp = new List<Ndp> {new Ndp{ndFlow = ndFlowRoot, ndTarget = ndTargetRoot}}, DistFromStart = 0}.Encons(),
                gnd => gnd.d == l,
                nodi =>
                {
                    var rgnodiNext=new List<Gnd>();
                    if(nodi.rgndp.All(ndp => ndp.ndFlow.c0 == ndp.ndTarget.c0))
                    {
                        Debug.Assert(nodi.rgndp.All(ndp => ndp.ndFlow.c1 == ndp.ndTarget.c1));
                        rgnodiNext.Add(new Gnd
                        {
                            d = nodi.d + 1,
                            rgndp = qqq(nodi, false).ToList(),
                            DistFromStart = nodi.DistFromStart
                        });
                    }
                    if(nodi.rgndp.All(ndp => ndp.ndFlow.c0 == ndp.ndTarget.c1))
                    {
                        Debug.Assert(nodi.rgndp.All(ndp => ndp.ndFlow.c1 == ndp.ndTarget.c0));
                        rgnodiNext.Add(new Gnd
                        {
                            d = nodi.d + 1,
                            rgndp = qqq(nodi, true).ToList(),
                            DistFromStart = nodi.DistFromStart+1
                        });
                    }
                    return rgnodiNext;
                });
               
            if(nodiResult==null)
                yield return "NOT POSSIBLE";
            else
                yield return nodiResult.DistFromStart;
        }

        private static IEnumerable<Ndp> qqq(Gnd nodi, bool fSwitched)
        {
            foreach(var ndp in nodi.rgndp)
            {
                if(ndp.ndFlow.c0 > 0)
                    yield return new Ndp {ndFlow = ndp.ndFlow.mpndByCh['0'], ndTarget = ndp.ndTarget.mpndByCh[fSwitched ? '1' : '0']};
                if(ndp.ndFlow.c1 > 0)
                    yield return new Ndp {ndFlow = ndp.ndFlow.mpndByCh['1'], ndTarget = ndp.ndTarget.mpndByCh[fSwitched ? '0' : '1']};
            }
        }

        private static Nd fill(string[] rgconfFlow)
        {
            var ndRoot = new Nd();
            foreach(var conf in rgconfFlow)
            {
                var nd = ndRoot;
                foreach(var vich in conf.Select((v, i) => new {v, i}))
                {
                    if(vich.v == '1')
                        nd.c1++;
                    else
                        nd.c0++;
                    nd = nd.mpndByCh.EnsureGet(vich.v);
                }
            }
            return ndRoot;
        }
    }
}
