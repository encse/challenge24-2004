using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1B.C
{
    internal class EqualSumsSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var rgnum = Fetch<long[]>().Skip(1).ToArray();

            yield return Solwrt.NewLine;

            var mpsumprevBySum = new SortedDictionary<long, long> {{0, 0}};

            foreach(var vinum in rgnum.Select((v,i)=>new{v,i}))
            {
                var num = vinum.v;
                Info(string.Format("{0} / {1}", vinum.i+1, rgnum.Length));
                foreach(var sumT in mpsumprevBySum.Keys.Take(1000000).ToArray())
                {
                    var sum = sumT + num;


                    if(mpsumprevBySum.ContainsKey(sum))
                    {
                        for(var sumcur = sum; sumcur != 0;)
                        {
                            var sumprev = mpsumprevBySum[sumcur];
                            yield return sumcur - sumprev;

                            sumcur = sumprev;
                        }
                        yield return Solwrt.NewLine;

                        yield return num;
                        for(var sumcur = sumT; sumcur != 0;)
                        {
                            var sumprev = mpsumprevBySum[sumcur];
                            yield return sumcur - sumprev;

                            sumcur = sumprev;
                        }

                        yield break;
                    }

                    mpsumprevBySum[sum] = sumT;
                }
            }
            yield return "Impossible";

        }
    }
}
