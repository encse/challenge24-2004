using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.QR.D
{
    class DeceitfulWarSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cwood = Fetch<int>();
            var rgwoodNaomiOrig = Fetch<List<decimal>>();
            var rgwoodKenOrig = Fetch<List<decimal>>();

            rgwoodNaomiOrig.Sort();
            rgwoodKenOrig.Sort();
            rgwoodKenOrig.Reverse();

            var cwinWarNaomi = 0;
            {
                var rgwoodKen = new List<decimal>(rgwoodKenOrig);
                foreach(var woodNaomi in rgwoodNaomiOrig)
                {
                    var viwoodKen = rgwoodKen.Select((v, i) => new {v, i}).LastOrDefault(viwood => woodNaomi < viwood.v);
                    rgwoodKen.RemoveAt(viwoodKen == null ? rgwoodKen.Count - 1 : viwoodKen.i);
                    cwinWarNaomi += viwoodKen == null ? 1 : 0;
                }
            }

            var cwinDWarNaomi = 0;
            {
                var rgwoodKen = new List<decimal>(rgwoodKenOrig);
                foreach(var woodNaomi in rgwoodNaomiOrig)
                {
                    if(woodNaomi <= rgwoodKen.Last())
                        continue;
                    rgwoodKen.RemoveAt(rgwoodKen.Count-1);
                    cwinDWarNaomi++;
                }
            }

            yield return cwinDWarNaomi;
            yield return cwinWarNaomi;
        }
    }
}
