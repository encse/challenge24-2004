using System.Collections.Generic;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.E
{
    public class ENumbersSolver : Solver
    {
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            var x = pparser.Fetch<List<int>>();
            var c = pparser.Fetch<List<int>>();

            var rgA = pparser.Fetch<int[]>();
            var rgB = pparser.Fetch<int[]>();

            var fAWins = FWins(x, rgA);
            var fBWins = FWins(x, rgB);
            
            var i = 7;
            for (; !fAWins && !fBWins; i++)
            {
                x.Add((x[7] * c[0] + x[6] * c[1] + x[5] * c[2] + x[4] * c[3] + x[3] * c[4] + x[2] * c[5] + x[1] * c[6] + x[0] * c[7]) % 1000);
                x.RemoveAt(0);

                fAWins = FWins(x, rgA);
                fBWins = FWins(x, rgB);
            }

            using (Output)
            {
                Solwrt.WriteLine(fAWins ? "A wins at {0}." : "B wins at {0}.", i);
            }
        }

        private bool FWins(IList<int> x, IList<int> rgB)
        {
            for (var i = 0; i < 8; i++)
                if (x[i] != rgB[i])
                    return false;
            return true;
        }
    }
}


