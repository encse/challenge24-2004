using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1A.B
{
    internal class FullBinaryTreeSolver : GcjSolver
    {
        private class Nd
        {
            public readonly HashSet<Nd> Rgnd = new HashSet<Nd>();
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cnode = Fetch<int>();
            var mpnodeByI = cnode.Eni().ToDictionary(i => i+1, i => new Nd());
            foreach(var i in (cnode-1).Eni())
            {
                int ind1;
                int ind2;
                Fetch(out ind1, out ind2);
                mpnodeByI[ind1].Rgnd.Add(mpnodeByI[ind2]);
                mpnodeByI[ind2].Rgnd.Add(mpnodeByI[ind1]);
            }

            Func<Nd, Nd, int> dgcNode = null;
            dgcNode = (ndParent, nd) => 
                nd.Rgnd.Count == 1 
                    ? 1 
                    : 1 + nd.Rgnd.Where(ndT => ndT != ndParent).Sum(ndT => dgcNode(nd, ndT)
            );
            dgcNode = dgcNode.ToCached();

            Func<Nd, Nd, int> dgcCut = null;
            dgcCut = (ndParent, nd) => 
                nd.Rgnd.Count == 1 
                    ? (ndParent != null 
                        ? 0 
                        : dgcNode(nd, nd.Rgnd.Single())) 
                    : (nd.Rgnd.Count == 2 && ndParent != null 
                        ? dgcNode(nd, nd.Rgnd.Single(ndT => ndT != ndParent)) 
                        : nd.Rgnd
                            .Where(ndT => ndT != ndParent)
                            .Select(ndT => new { nd = ndT, cnode = dgcNode(nd, ndT), ccut = dgcCut(nd, ndT) })
                            .OrderByDescending(x => x.cnode - x.ccut)
                            .Select((v, i) => new { v, i })
                            .Sum(vix => vix.i < 2 ? vix.v.ccut : vix.v.cnode));
            dgcCut = dgcCut.ToCached();

            yield return mpnodeByI.Values.Min(ndRoot => dgcCut(null, ndRoot));
        }

    }
}
