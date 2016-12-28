using System.IO;
using Ch24.Util;

namespace Ch24.Contest11.R
{
    public class RWavSolver : Contest.Solver
    {
        public override void Solve()
        {
            var n = Pparser.Fetch<int>();
            var rgsample = new int[n];
            for (int i = 0; i < n;i++ )
            {
                var d = Fetch<double>();
                rgsample[i] = (int)((d +1) * (1<<30));
            }
            using (var bw = new BinaryWriter(File.Create(FpatOut)))
                Wavu.Wav(bw, 8000, rgsample);
        }
    }
}
