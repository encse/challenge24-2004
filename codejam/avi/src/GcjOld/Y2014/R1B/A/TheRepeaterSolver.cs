using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.A
{
    internal class TheRepeaterSolver : GcjSolver
    {

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var cword = Fetch<int>();
            var rgword = cword.Eni().Select(iword => Fetch<string>()+"#").ToList();
            var rgich = cword.Eni().Select(iword => 0).ToList();

            var cstep = 0;
            for(;;)
            {
                var ch = rgword[0][rgich[0]];
                if(rgword.Select((word,i)=>
                {
                    var c = word[rgich[i]];
                    return c != ch;
                }).Any(f => f))
                {
                    yield return "Fegla Won";
                    yield break;
                }

                if(ch=='#')
                {
                    yield return cstep;
                    yield break;
                }

                var rgcch = rgword.Select((word, i) =>
                {
                    var cch = 0;
                    for(; word[rgich[i]] == ch;)
                    {
                        cch++;
                        rgich[i]++;
                    }
                    return cch;
                }).ToList();
                rgcch.Sort();
                var med = rgcch[rgcch.Count / 2];
                var steps = rgcch.Sum(cch => Math.Abs(med - cch));
                cstep += steps;
            }
        }

    }
}
