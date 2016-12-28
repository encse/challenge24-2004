using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;
using Cmn.Util;

namespace Gcj.Y2013.R2.C
{
    internal class ErdosSzekeres2Solver : GcjSolver
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
            var rgiEmptyInc = new LinkedList<int>(new int[c].Select((v, i) => i).OrderBy(i =>c*rginc[i] -i));
            var rgiEmptyDec = new LinkedList<int>(new int[c].Select((v, i) => i).OrderBy(i =>c*rgdec[i] +i));
            var rgn = new int[c].ToList();

            var niEmptyInc = rgiEmptyInc.First;
            var niEmptyDec = rgiEmptyDec.First;

            for(var n=1;n<=c;n++)
            {
                for(;rgn[niEmptyInc.Value]!=0;)
                {
                    Debug.Assert(
                        rginc[niEmptyInc.Value] < rginc[niEmptyInc.Next.Value] || 
                        (rginc[niEmptyInc.Value] == rginc[niEmptyInc.Next.Value] && 
                        niEmptyInc.Value > niEmptyInc.Next.Value));
                    niEmptyInc = niEmptyInc.Next;
                }

                for(;rgn[niEmptyDec.Value]!=0;)
                    niEmptyDec = niEmptyDec.Next;

                Debug.Assert(niEmptyInc.Value <= niEmptyDec.Value);
                rgn[niEmptyInc.Value] = n;
            }

            Check(rgn, rginc, rgdec);

            return rgn.Cast<object>();
        }

        private void Check(List<int> rgn, List<int> rginc, List<int> rgdec)
        {
            for(var i=0;i<rgn.Count;i++)
            {
                var inc = rginc.Take(i).Where((v, iinc) => rgn[iinc] < rgn[i]).Concat(new[]{0}).Max() + 1;
                if(rginc[i]!=inc)
                {
                    throw new Exception();
                }
                var dec = rgdec.Skip(i + 1).Where((v, idec) => rgn[idec+i+1] < rgn[i]).Concat(new[]{0}).Max() + 1;
                if(rgdec[i]!=dec)
                {
                    throw new Exception();
                }
            }
        }
    }
}
