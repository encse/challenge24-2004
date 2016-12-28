using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.A
{
    public class ARepeaterSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var n = pparser.Fetch<int>();
            var rgst = pparser.FetchN<string>(n);

            return () => Solve(rgst.ToArray());
        }

        private IEnumerable<object> Solve(string[] rgst)
        {

            var cmoveAll = 0;
            while (true)
            {
                //elfogyott
                if (!rgst.Any(st => st.Length > 0))
                    break;

                //valamelyik elfogyott
                if (rgst.Any(st => st.Length == 0))
                {
                    yield return "Fegla Won";
                    yield break;

                }

                //mindegyikben van még
                var rgstT = new string[rgst.Length];

                for (int i = 0; i < rgst.Length; i++)
                {
                    Cut(rgst, rgstT, i);
                }

                //valamelyik nem úgy kezdődik mint a többi:

                var hlmchFirst = new HashSet<char>(rgstT.Select(st => st[0]));
                if (hlmchFirst.Count > 1)
                {
                    yield return "Fegla Won";
                    yield break;
                }


                var cchMin = rgstT.Min(st => st.Length);
                var cchMax = rgstT.Max(st => st.Length);

                var cmoveMin = int.MaxValue;
                for (int cchCommon = cchMin; cchCommon <= cchMax; cchCommon++)
                {

                    var cmove = CMove(rgstT, cchCommon);
                    if (cmove < cmoveMin)
                        cmoveMin = cmove;
                }
                cmoveAll += cmoveMin;
            }

            yield return cmoveAll;
        }

        private int CMove(string[] rgstT, int cchCommon)
        {
            int cmove = 0;
            foreach (var st in rgstT)
            {
                var cch = st.Length;
                cmove += Math.Abs(cchCommon - cch);
            }
            return cmove;
        }

        private void Cut(string[] rgstToCut, string[] rgstOut, int istCut)
        {
            var stToCut = rgstToCut[istCut];
            var ch = stToCut[0];
            int cch = 0;
           
            for (int i = 0; i < stToCut.Length; i++)
            {
                if (stToCut[i] == ch)
                    cch++;
                else break;
            }

            rgstOut[istCut] = stToCut.Substring(0, cch);
            rgstToCut[istCut] = stToCut.Substring(cch);

        }
    }
}
