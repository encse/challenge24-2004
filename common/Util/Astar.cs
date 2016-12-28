using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cmn.Util
{
    public class Astar<TNode, TDistance> where TDistance : IComparable<TDistance>
    {
        private readonly HashSet<TNode> hlmDone = new HashSet<TNode>();
        private readonly SortedDictionary<TDistance, ISet<TNode>> mpActive = new SortedDictionary<TDistance, ISet<TNode>>();
        private readonly Dictionary<TNode, TDistance> mpttotalByN = new Dictionary<TNode, TDistance>();
        private readonly Dictionary<TNode, TDistance> mptByN = new Dictionary<TNode, TDistance>();
        private readonly Func<TNode,bool> fEnd;
        private readonly Func<TNode, TDistance, IEnumerable<TNode>> enNextGet;
        private readonly Func<TNode, TDistance, TNode, TDistance> tGet;
        private readonly Func<TNode, TDistance, TDistance> tGetMinTotal;

        //public Astar(IEnumerable<Tuple<N, T>> enprntStart, IEnumerable<N> ennEnd, Func<N, T, IEnumerable<N>> dgenNextGet, Func<N, T, N, T> tGet, Func<N, T, T> tGetMinTotal = null)
        //    :this(enprntStart, n => )
        //{
            
        //}

        public Astar(IEnumerable<Tuple<TNode, TDistance>> enprntStart, Func<TNode, bool> fEnd, Func<TNode, TDistance, IEnumerable<TNode>> enNextGet, Func<TNode, TDistance, TNode, TDistance> tGet, Func<TNode, TDistance, TDistance> tGetMinTotal = null)
        {
            this.enNextGet = enNextGet;
            this.tGet = tGet;
            this.tGetMinTotal = tGetMinTotal ?? ((n, t) => t) ;

            foreach(var prnt in enprntStart)
            {
                AddActive(prnt.Item1, this.tGetMinTotal(prnt.Item1,prnt.Item2), prnt.Item2);
            }

            this.fEnd = fEnd;
        }

        private void AddActive(TNode n, TDistance tTotal, TDistance t)
        {
            TDistance tTotalOld;
            if(mpttotalByN.TryGetValue(n, out tTotalOld))
            {
                if(tTotalOld.CompareTo(tTotal) <= 0)
                    return;

                var hlmn = mpActive[tTotalOld];
                hlmn.Remove(n);
                if(hlmn.Count == 0)
                    mpActive.Remove(tTotalOld);
            }
            mpttotalByN[n] = tTotal;
            mptByN[n] = t;
            mpActive.EnsureGet(tTotal, () => n is IComparable<TNode> ? (ISet<TNode>)new SortedSet<TNode>() : new HashSet<TNode>()).Add(n);
        }

        public Tuple<TNode, TDistance> Find()
        {
            for(;;)
            {
                if(mpActive.Count == 0)
                    return null;

                var kvpFirst = mpActive.First();
                var ttotalFrom = kvpFirst.Key;
                var nFrom = kvpFirst.Value.First();
                var tFrom = mptByN[nFrom];

                mptByN.Remove(nFrom);
                mpttotalByN.Remove(nFrom);
                kvpFirst.Value.Remove(nFrom);

                if(kvpFirst.Value.Count == 0)
                    mpActive.Remove(ttotalFrom);

                Debug.Assert(!hlmDone.Contains(nFrom));

                if(fEnd(nFrom))
                {
                    Debug.Assert(tFrom.CompareTo(ttotalFrom)==0);
                    return new Tuple<TNode, TDistance>(nFrom, tFrom);
                }

                hlmDone.Add(nFrom);

                foreach(var nTo in enNextGet(nFrom, tFrom))
                {
                    if(hlmDone.Contains(nTo))
                        continue;

                    var t = tGet(nFrom, tFrom, nTo);
                    AddActive(nTo, tGetMinTotal(nTo,t), t);
                }
            }
        }
    }
}