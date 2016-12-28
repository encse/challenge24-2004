using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;
using Cmn.Util;

namespace Gcj.Y2013.R2.C
{
    internal class ErdosSzekeresSmallSolver : GcjSolver
    {

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var c = Fetch<int>();
            var rginc = Fetch<List<int>>();
            var rgdec = Fetch<List<int>>();
            var rgn = new int[c].ToList();

            for(var n=c;n>0;n--)
            {
                for(var i = rgn.Count-1; i >= 0; i--)
                {
                    if(rgn[i]==0)
                    {

                        var inc = rginc.Take(i).Where((v, iinc) => rgn[iinc] < n).Concat(new[] {0}).Max() + 1;
                        if(rginc[i] == inc)
                        {

                            var dec = rgdec.Skip(i + 1).Where((v, idec) => rgn[idec + i + 1] < n).Concat(new[] {0}).Max() + 1;
                            if(rgdec[i] == dec)
                            {
                                rgn[i] = n;
                                break;
                            }
                        }
                    }
                    Debug.Assert(i>0);
                }
            }
            return rgn.Cast<object>();
        }
    }
}
