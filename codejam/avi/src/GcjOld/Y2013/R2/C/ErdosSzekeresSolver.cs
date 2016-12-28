using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;
using Cmn.Util;

namespace Gcj.Y2013.R2.C
{
    internal class ErdosSzekeresSolver : GcjSolver
    {
        private class Vi
        {
            public int v;
            public int i;

            public override string ToString()
            {
                return string.Format("V: {0}, I: {1}", v, i);
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var c = Fetch<int>();
            var rginc = Fetch<List<int>>();
            var rgdec = Fetch<List<int>>();
            var rgiEmpty = new LinkedList<int>(new int[c].Select((v, i) => i));
            var rgn = new int[c].ToList();
            var rgviForInc = new LinkedList<Vi>();
            rgviForInc.AddFirst(new Vi {i = -1, v = 1});
            var rgviForDec = new LinkedList<Vi>();
            rgviForDec.AddFirst(new Vi {i = c, v = 1});

            for(var n = 1;n<=c;n++)
            {
                var nviForInc = rgviForInc.First;
                var nviForDec = rgviForDec.First;

                for(var ni = rgiEmpty.First;;ni=ni.Next)
                {
                    if(ni==null)
                    {
                        return rgn.Cast<object>();
                    }
                    var i = ni.Value;
                    
                    for(;nviForInc.Next != null && (nviForInc.Next.Value.i<i);nviForInc=nviForInc.Next)
                        ;
                    
                    for(;!(i<nviForDec.Value.i);nviForDec=nviForDec.Next)
                        ;

                    if(nviForInc.Value.v != rginc[i] || nviForDec.Value.v != rgdec[i])
                        continue;
                    
                    ni.List.Remove(ni);
                    rgn[i] = n;

                    if(nviForInc.Next != null && nviForInc.Next.Value.v == rginc[i]+1)
                    {
                        nviForInc.Next.Value.i = i;
                    }
                    else if(nviForInc.Next == null || nviForInc.Next.Value.v > rginc[i]+1)
                    {
                        nviForInc.List.AddAfter(nviForInc, new Vi {i = i, v = rginc[i] + 1});
                    }
                    else
                    {
                        Debug.Assert(false);
                    }

                    if(nviForDec.Previous != null && nviForDec.Previous.Value.v == rgdec[i]+1)
                    {
                        nviForDec.Previous.Value.i = i;
                    }
                    else if(nviForDec.Previous == null || nviForDec.Previous.Value.v > rgdec[i]+1)
                    {
                        nviForDec.List.AddBefore(nviForDec, new Vi {i = i, v = rgdec[i] + 1});
                    }
                    else
                    {
                        Debug.Assert(false);
                    }

                    break;
                }
            }

            return rgn.Cast<object>();
        }
    }
}
