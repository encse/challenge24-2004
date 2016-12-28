using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.R1A.B
{
    internal class ManageYourEnergySolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            BigInteger eMax;
            BigInteger eBack;
            BigInteger ctask;
            Fetch(out eMax, out eBack, out ctask);
            var rgvalue = Fetch<int[]>().ToList();

            var e = eMax;
            BigInteger score = 0;
            for(var itask = 0;itask<rgvalue.Count;itask++)
            {
                var efree = (e + eBack) - eMax;
                if(efree >= e)
                {
                    efree = e;
                }
                else
                {
                    var c = 1;
                    for(;;)
                    {
                        var itask2 = itask + c;

                        if(itask2 >= rgvalue.Count)
                        {
                            efree = e;
                            break;
                        }

                        if(rgvalue[itask2] >= rgvalue[itask])
                            break;

                        efree = (e + eBack + c * eBack) - eMax;

                        if(efree >= e)
                        {
                            efree = e;
                            break;
                        }

                        c++;
                    }
                }
                if(efree < 0)
                    efree = 0;
                score += rgvalue[itask] * efree;
                e = e - efree + eBack;
                if(e > eMax)
                    e = eMax;
            }
            yield return score;
        }

    }
}
