using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;

namespace Ch24.Contest10.Q
{
    class QAbsurdistanSolver : Solver
    {
        const int freqSample = 18000;
        public override void Solve()
        {
            var mpchbymorse = new Dictionary<string, string>
            {
                {".-", "A"}, {"-...", "B"}, {"-.-.", "C"}, {"-..", "D"}, {".", "E"},
                {"..-.", "F"}, {"--.", "G"}, {"....", "H"}, {"..", "I"}, {".---", "J"},
                {"-.-", "K"}, {".-..", "L"}, {"--", "M"}, {"-.", "N"}, {"---", "O"},
                {".--.", "P"}, {"--.-", "Q"}, {".-.", "R"}, {"...", "S"}, {"-", "T"},
                {"..-", "U"}, {"...-", "V"}, {".--", "W"}, {"-..-", "X"}, {"-.--", "Y"},
                {"--..", "Z"}, {"--..--", ","}, {"-----", "0"}, {".----", "1"}, {"..---", "2"},
                {"...--", "3"}, {"....-", "4"}, {".....", "5"}, {"-....", "6"}, {"--...", "7"},
                {"---..", "8"}, {"----.", "9"}, {" ", " "}
            };


            var rgsample = Wavu.Rgsample8bit(FpatIn, freqSample);
            rgsample = rgsample.SkipWhile(b => b == 128).ToArray();
            using (Output)
            {
                foreach (var morse in Enmorse(rgsample))
                    Solwrt.Write(mpchbymorse[morse]);
                Solwrt.WriteLine("");
            }
        }

        private IEnumerable<string> Enmorse(byte[] rgsample)
        {
            var lShort = int.MaxValue;
            var lLong = int.MinValue;

            Dictionary<int, int> cdt = new Dictionary<int, int>();

            var isampleEndPrevBlock = -1;

            foreach(var block in Wavu.RgsampleBlock(rgsample, 0, 50))
            {
                var l = block.Length;
                lLong = Math.Max(lLong, l);
                lShort = Math.Min(lShort, l);

                if (isampleEndPrevBlock != -1)
                {
                    var dt = block.IsampleStart - isampleEndPrevBlock;
                    if (!cdt.ContainsKey(dt))
                        cdt[dt] = 1;
                    else
                        cdt[dt]++;
                }
                isampleEndPrevBlock = block.IsampleStart + block.Length;
            }

            var rgdt = cdt.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).Take(3).ToArray();
            var dtShort = rgdt[0];
            var dtMiddle = rgdt[1];
            var dtLong = rgdt[2];

            isampleEndPrevBlock = -1;
            string morse = "";
            foreach (var block in Wavu.RgsampleBlock(rgsample, 0, 50))
            {
                if (isampleEndPrevBlock != -1)
                {
                    var dt = block.IsampleStart - isampleEndPrevBlock;
                    var dtShortA = Math.Abs(dt - dtShort);
                    var dtMiddleA = Math.Abs(dt - dtMiddle);
                    var dtLongA = Math.Abs(dt - dtLong);

                    if (dt > dtMiddle * 1.5)
                    {
                        yield return morse;
                        yield return " ";
                        morse = "";
                    }
                    else if (dtMiddleA < dtLongA && dtMiddleA < dtShortA)
                    {
                        yield return morse;
                        morse = "";
                    }
                }

                morse += block.Length < (lShort + lLong) / 2 ? "." : "-";
                isampleEndPrevBlock = block.IsampleStart + block.Length;
            }

            if (morse != "")
                yield return morse;
        }

       

    }
}
