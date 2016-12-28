using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2009.QR.A
{
    internal class AlienLanguageSolver : GcjSolver
    {
        private List<string> rgwordAll=new List<string>();
        private int Cch;

        protected override int CCaseGet()
        {
            var rgldn = Fetch<int[]>();
            Cch = rgldn[0];
            var d = rgldn[1];
            for(var iword = 0; iword < d; iword++)
                rgwordAll.Add(Fetch<string>());
            return rgldn[2];
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var stpat = Fetch<string>();
            var rgword = new List<string>(rgwordAll);
            for(var ich = 0;ich<Cch;ich++)
            {
                string strgch;
                if(stpat.First() == '(')
                {
                    strgch = stpat.Substring(1, stpat.IndexOf(')') - 1);
                    stpat = stpat.Substring(stpat.IndexOf(')') + 1);
                }
                else
                {
                    strgch = stpat.Substring(0,1);
                    stpat = stpat.Substring(1);
                }

                rgword = rgword.Where(word => strgch.Contains(word[ich])).ToList();
            }
            yield return rgword.Count;
        }
    }
}
