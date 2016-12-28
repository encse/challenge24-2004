using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gcj.Util;
using System.Linq;
using Cmn.Util;

namespace Gcj.Y2013.QR.D
{
    internal class TreasureNotSolver : IConcurrentSolver
    {
        private class Chest
        {
            public int id;
            public int k;
            public Dictionary<int, int> mpcByK;
        }

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int ckey;
            int cchest;
            pparser.Fetch(out ckey, out cchest);

            var mpcByK = pparser.Fetch<int[]>().GroupBy(k => k).ToDictionary(grp => grp.Key, grp => grp.Count());

            var rgchest = new LinkedList<Chest>();

            for(var ichest=0;ichest<cchest;ichest++)
            {
                var rgl = pparser.Fetch<int[]>();
                rgchest.AddLast(new Chest
                {
                    id = ichest + 1,
                    k=rgl[0],
                    mpcByK = rgl.Skip(2).GroupBy(k => k).ToDictionary(grp => grp.Key, grp => grp.Count())
                });
            }

            return ()=>Run(mpcByK, rgchest);
        }

        private IEnumerable<object> Run(Dictionary<int, int> mpcByK, LinkedList<Chest> rgchest)
        {
            var rgic = RgicGet(mpcByK, rgchest);

            return rgic == null ? new object[] {"IMPOSSIBLE"} : rgic.Cast<object>().ToArray();
        }

        private List<int> RgicGet(Dictionary<int, int> mpcByK, LinkedList<Chest> rgchest)
        {
            if(rgchest.Count==0)
                return new List<int>();

            if(mpcByK.Count == 0)
                return null;

            for(var ndchest=rgchest.First;ndchest!=null;ndchest=ndchest.Next)
            {
                var k = ndchest.Value.k;

                if(!mpcByK.ContainsKey(k))
                    continue;

                var ndbefore = ndchest.Previous;
                rgchest.Remove(ndchest);

                foreach(var kvpcByKChest in ndchest.Value.mpcByK)
                {
                    mpcByK[kvpcByKChest.Key] = mpcByK.GetOrDefault(kvpcByKChest.Key, 0) + kvpcByKChest.Value;
                }

                mpcByK[k] -= 1;

                if(mpcByK[k] == 0)
                    mpcByK.Remove(k);

                var rgic = RgicGet(mpcByK, rgchest);

                if(rgic != null)
                {
                    rgic.Insert(0, ndchest.Value.id);
                    return rgic;
                }

                mpcByK[k] = mpcByK.GetOrDefault(k, 0) + 1;

                foreach(var kvpcByKChest in ndchest.Value.mpcByK)
                {
                    var v = mpcByK[kvpcByKChest.Key] - kvpcByKChest.Value;
                    if(v == 0)
                        mpcByK.Remove(kvpcByKChest.Key);
                    else
                        mpcByK[kvpcByKChest.Key] = v;
                }

                if(ndbefore == null)
                    rgchest.AddFirst(ndchest);
                else
                    rgchest.AddAfter(ndbefore, ndchest);
            }

            return null;

        }
    }
}
