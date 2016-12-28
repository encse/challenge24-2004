using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.R1A.C
{
    class CSolver : GcjSolver
    {
        //static CSolver()
        //{
        //    var c = 3000;
        //    var r = new Random(0);
        //    using(var sw = new StreamWriter("x.txt"))
        //    {
        //        sw.WriteLine(c);
        //        for(int i = 0; i < c; i++)
        //        {
        //            sw.WriteLine("{0} {1}", r.Next(-1000000, 1000000), r.Next(-1000000, 1000000));
        //        }
        //        sw.Flush();
        //    }
        //    r.Next();
        //}

        class P
        {
            public int i;
            public double x;
            public double y;
        }

        class Pan
        {
            public P p;
            public double ang;
            public int i;
        }

        private IEnumerable<T> EnFrom<T>(List<T> rg, int iStart)
        {
            for(int i = iStart; i < rg.Count; i++)
            {
                yield return rg[i];
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cPoint = Fetch<int>();

            var rgpoint = Enumerable.Range(0, cPoint).Select(i =>
            {
                double x, y;
                Fetch(out x, out y);
                return new P {i = i, x = x, y = y};
            }).ToList();

            if(rgpoint.Count <= 3)
            {
                foreach (var pOrigin in rgpoint)
                {
                    yield return Solwrt.NewLine;
                    yield return 0;
                }
                yield break;    
            }

            foreach(var pOrigin in rgpoint)
            {
                var rgpan = rgpoint
                    .Where(p => p.i != pOrigin.i)
                    .Select(p => new Pan {p = p, ang = Math.Atan2(p.y - pOrigin.y, p.x - pOrigin.x)})
                    .ToList();

                Debug.Assert(rgpan.All(pan => -Math.PI < pan.ang && pan.ang <= Math.PI));

                rgpan.Sort((pan1T, pan2T) => pan1T.ang.CompareTo(pan2T.ang));
                var rgpan2 = rgpan.Select(pan => new Pan {p = pan.p, ang = 2 * Math.PI + pan.ang}).ToList();
                rgpan.AddRange(rgpan2);
                foreach(var vipan in rgpan.Select((v,i) => new {v,i}))
                {
                    vipan.v.i = vipan.i;
                }

                var pan1 = rgpan.First();
                var pan2 = rgpan.First(pan => pan.ang > pan1.ang + Math.PI);
                var maxtree = 1;

                for(;;)
                {
                    var ctree = pan2.i - pan1.i + 1;
                    Debug.Assert(ctree <= cPoint);
                    maxtree = Math.Max(maxtree, ctree);

                    pan1 = rgpan[pan1.i + 1];
                    if(pan1.i == cPoint - 1)
                        break;

                    pan2 = EnFrom(rgpan, pan2.i).First(pan => pan.ang > pan1.ang + Math.PI);
                }

                yield return Solwrt.NewLine;
                yield return cPoint - maxtree;
            }
        }

    }
}
