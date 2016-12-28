using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R2.C
{
    internal class MountainViewSolver : GcjSolver
    {
        private class Mou
        {
            public int imou;
            public decimal h;

        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            Fetch<int>();
            var rgmou = Fetch<List<int>>().Select(x => new Mou {imou = x - 1}).ToList();

            var qudg = new Queue<Action>();

            Action<int, int> calc = (_1, _2) => { throw new Exception(); };

            calc = (iStart, iEnd) =>
            {
                if(iStart>=iEnd)
                    return;

                var hmax = rgmou[iEnd].h;
                decimal hmin = 1;
                for(;;)
                {
                    if(decimal.Remainder((hmax - hmin) , (iEnd - iStart)) == 0)
                        break;
                    hmin++;
                }
                rgmou[iStart].h = hmin;

                var imou = iStart;
                for(;;)
                {
                    if(imou==iEnd)
                        break;

                    Debug.Assert(rgmou[imou].h != 0);

                    var imouNext = rgmou[imou].imou;

                    if(imouNext>iEnd)
                    {
                        rgmou = null;
                        break;
                    }

                    var mouNext = rgmou[imouNext];
                    var hNext = hmin + (hmax - hmin) * (imouNext - iStart) / (iEnd - iStart);

                    Debug.Assert(!(imouNext < iEnd) || mouNext.h == 0 );
                    Debug.Assert(!(imouNext == iEnd) || mouNext.h == hNext );
                    Debug.Assert(!(imouNext == iEnd) || hmax == hNext );
                    
                    mouNext.h = hNext;

                    var imouT = imou;
                    qudg.Enqueue(() => calc(imouT + 1, imouNext));
                    if(rgmou == null)
                        break;

                    imou = imouNext;
                }
            };

            rgmou.Add(new Mou{imou = -1, h = 999999999});
            var rgmouQ = new List<Mou>(rgmou);

            qudg.Enqueue(() => calc(0, rgmou.Count - 1));

            for(;rgmou != null && qudg.Count > 0;)
            {
                qudg.Dequeue()();
            }

            if(rgmou!=null)
                foreach(var vimou in rgmou.Select((v,i)=>new{v,i}))
                {
                    var imouNext = -1;
                    var max = decimal.MinValue;

                    foreach(var vimou2 in rgmou.Select((v,i)=>new{v,i}).Skip(vimou.i+1))
                    {
                        var t = (vimou2.v.h - vimou.v.h) / (vimou2.i - vimou.i);
                        if(t > max)
                        {
                            max = t;
                            imouNext = vimou2.i;
                        }
                    }

                    Debug.Assert(vimou.v.imou == imouNext);
                }

            return rgmou == null ? 
                new object[] {"Impossible"}
                //.Concat(new object[]{ Solwrt.NewLine})
                //.Concat(rgmouQ.Select((v,i)=>i+1).Cast<object>())
                //.Concat(new object[]{Solwrt.NewLine})
                //.Concat(rgmouQ.Take(rgmouQ.Count-1).Select(mou => mou.imou+1).Cast<object>())
                : rgmou.Select(mou => mou.h).Cast<object>();
        }
    }
}
