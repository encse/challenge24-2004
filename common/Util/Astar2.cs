using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Cmn.Util
{
    public class Astar2
    {

        public static TAstarNd Find<TAstarNd, TDistance>(IEnumerable<TAstarNd> enstateStart, Func<TAstarNd, bool> fEnd, Func<TAstarNd, IEnumerable<TAstarNd>> enNextGet, Func<TAstarNd, TDistance> distGetMinToEnd = null) 
            where TAstarNd : IAstarNd<TDistance> 
            where TDistance : IComparable<TDistance>
        {
            return FindI(enstateStart, fEnd, enNextGet, distGetMinToEnd, () => new HashSet<TAstarNd>());
        }

        public static TAstarNd FindComparable<TAstarNd, TDistance>(IEnumerable<TAstarNd> enstateStart, Func<TAstarNd, bool> fEnd, Func<TAstarNd, IEnumerable<TAstarNd>> enNextGet, Func<TAstarNd, TDistance> distGetMinToEnd = null)
            where TAstarNd : IComparable<TAstarNd>, IAstarNd<TDistance>
            where TDistance : IComparable<TDistance>
        {
            return FindI(enstateStart, fEnd, enNextGet, distGetMinToEnd, () => new SortedSet<TAstarNd>());
        }

        public static TAstarNd FindRanked<TAstarNd, TRank, TDistance>(IEnumerable<TAstarNd> enstateStart, Func<TAstarNd, bool> fEnd, Func<TAstarNd, IEnumerable<TAstarNd>> enNextGet, Func<TAstarNd, TDistance> distGetMinToEnd = null)
            where TAstarNd : IAstarNd<TDistance>, IRanked<TRank>
            where TDistance : IComparable<TDistance>
            where TRank : IComparable<TRank>
        {
            return FindI(enstateStart, fEnd, enNextGet, distGetMinToEnd, () => new RankedSet<TAstarNd,TRank>());
        }

        public static TAstarNd FindI<TAstarNd, TDistance>(IEnumerable<TAstarNd> enstateStart, Func<TAstarNd, bool> fEnd, Func<TAstarNd, IEnumerable<TAstarNd>> enNextGet, Func<TAstarNd, TDistance> distGetMinToEnd, Func<ISet<TAstarNd>> dgNewSet) where TAstarNd : IAstarNd<TDistance> where TDistance : IComparable<TDistance>
        {
            distGetMinToEnd = distGetMinToEnd ?? (state => state.DistFromStart) ;

            var hlmDone = new HashSet<TAstarNd>();
            var mpActive = new SortedDictionary<TDistance, ISet<TAstarNd>>();
            var mpdisttoendByNode = new Dictionary<TAstarNd, TDistance>();

            for(var rgnodi = enstateStart;;)
            {
                foreach(var state in rgnodi)
                {
                    var distTotal = distGetMinToEnd(state);

                    TDistance distTotalOld;
                    if(mpdisttoendByNode.TryGetValue(state, out distTotalOld))
                    { 
                        if(distTotalOld.CompareTo(distTotal) <= 0)
                            continue;

                        var hlmn = mpActive[distTotalOld];
                        hlmn.Remove(state);
                        if (hlmn.Count == 0)
                            mpActive.Remove(distTotalOld);
                    }
                    mpdisttoendByNode[state] = distTotal;
                    mpActive.EnsureGet(distTotal, dgNewSet).Add(state);
                }

                if (mpActive.Count == 0)
                    return default(TAstarNd);

                var kvpFirst = mpActive.First();
                var distToEndCurrent = kvpFirst.Key;
                var nodeCurrent = kvpFirst.Value.First();
                var distFromStartCurrent = nodeCurrent.DistFromStart;

                mpdisttoendByNode.Remove(nodeCurrent);
                kvpFirst.Value.Remove(nodeCurrent);

                if(kvpFirst.Value.Count == 0)
                    mpActive.Remove(distToEndCurrent);

                Debug.Assert(!hlmDone.Contains(nodeCurrent));

                if(fEnd(nodeCurrent))
                {
                    Debug.Assert(distFromStartCurrent.CompareTo(distToEndCurrent)==0);
                    return nodeCurrent;
                }

                hlmDone.Add(nodeCurrent);

                rgnodi = enNextGet(nodeCurrent).Where(stateTo => !hlmDone.Contains(stateTo));
            }
        }
    }

    public interface IAstarNd<out TDistance> where TDistance : IComparable<TDistance>
    {
        TDistance DistFromStart { get; }
    }

}