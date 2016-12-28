using System;
using System.Diagnostics;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest10.P
{
    public class NormalizeSolver : Solver
    {

        public override void Solve()
        {
            var n = Pparser.Fetch<int>();
            var rgdVal = new double[n];

            var dSumSquares = 0.0;
            for (int i = 0; i < n; i++)
            {
                var dVal = Pparser.Fetch<double>();
                rgdVal[i] = dVal;
                dSumSquares += dVal * dVal;
            }

            var dNormalizer = Math.Sqrt(dSumSquares);
            using (Output)
                foreach (var dVal in rgdVal)
                    WriteLine(dVal / dNormalizer);

            Check(FpatOut);
        }

        private void Check(string fpatOut)
        {
            var pp = new Pparser(fpatOut);
            double d= 0;
            while(!pp.FEof())
                d += Math.Pow(pp.Fetch<double>(), 2);

            Debug.Assert(Math.Abs(d - 1) <= 0.001);

        }

    }
}
