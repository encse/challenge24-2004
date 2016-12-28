using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gcj.Util;
using System.Linq;
using Cmn.Util;

namespace Gcj.Y2013.QR.D
{
    internal class TreasureOfficalSolver : GcjSolver
    {
        private class Chest
        {
            public int id;
            public int k;
            public Dictionary<int, int> mpcByK;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int ckey;
            int cchest;
            Fetch(out ckey, out cchest);

            var mpcByK = Fetch<int[]>().GroupBy(k => k).ToDictionary(grp => grp.Key, grp => grp.Count());

            var rgchest = new LinkedList<Chest>();

            for(var ichest = 0; ichest < cchest; ichest++)
            {
                var rgl = Fetch<int[]>();
                rgchest.AddLast(new Chest
                {
                    id = ichest + 1,
                    k = rgl[0],
                    mpcByK = rgl.Skip(2).GroupBy(k => k).ToDictionary(grp => grp.Key, grp => grp.Count())
                });
            }

            if(!fEnoughKey(mpcByK, rgchest) || !fConnected(mpcByK, rgchest))
            {
                yield return "IMPOSSIBLE";
                yield break;
            }

            for(; rgchest.Any();)
            {
                for(var ndchest = rgchest.First;; ndchest = ndchest.Next)
                {
                    if(mpcByK.GetOrDefault(ndchest.Value.k,0)<1)
                        continue;

                    var ndPrev = ndchest.Previous;
                    rgchest.Remove(ndchest);
                    var mpcByKNew = new Dictionary<int, int>(mpcByK);
                    foreach(var kvpcByK in ndchest.Value.mpcByK)
                        mpcByKNew[kvpcByK.Key] = mpcByKNew.GetOrDefault(kvpcByK.Key, 0) + kvpcByK.Value;

                    mpcByKNew[ndchest.Value.k] -= 1;

                    if(fConnected(mpcByKNew, rgchest))
                    {
                        mpcByK = mpcByKNew;
                        yield return ndchest.Value.id;
                        break;
                    }

                    if(ndPrev == null)
                        rgchest.AddFirst(ndchest);
                    else
                        rgchest.AddAfter(ndPrev, ndchest);
                }
            }
        }

        private bool fConnected(Dictionary<int, int> mpcByK, IEnumerable<Chest> rgchest)
        {
            var hlmClosedChest = new HashSet<Chest>(rgchest);
            var hlmMissingK = new SortedSet<int>(hlmClosedChest.Select(chest => chest.k).Distinct());
            var hlmReachedK = new SortedSet<int>(mpcByK.Where(kvpcByK => kvpcByK.Value > 0).Select(kvpcByK => kvpcByK.Key));
            hlmMissingK.ExceptWith(hlmReachedK);

            for(;;)
            {
                if(!hlmMissingK.Any())
                    return true;

                var ochest = hlmClosedChest.FirstOrDefault(chest => hlmReachedK.Contains(chest.k));
             
                if(ochest == null)
                    return false;

                hlmClosedChest.Remove(ochest);
                hlmMissingK.ExceptWith(ochest.mpcByK.Keys);
                hlmReachedK.UnionWith(ochest.mpcByK.Keys);
            }
        }

        private bool fEnoughKey(Dictionary<int, int> mpcByK, IEnumerable<Chest> rgchest)
        {
            var mpdifByK = new Dictionary<int, Wrp<int>>();
            foreach(var kvpcByK in mpcByK)
                mpdifByK.EnsureGet(kvpcByK.Key, () => new Wrp<int> {V = 0}).V += kvpcByK.Value;

            foreach(var chest in rgchest)
            {
                mpdifByK.EnsureGet(chest.k, () => new Wrp<int> {V = 0}).V -= 1;
                foreach(var kvpcByK in chest.mpcByK)
                    mpdifByK.EnsureGet(kvpcByK.Key, () => new Wrp<int> {V = 0}).V += kvpcByK.Value;
            }
            return mpdifByK.Values.All(wrpdif => wrpdif.V >= 0);
        }
    }
}
