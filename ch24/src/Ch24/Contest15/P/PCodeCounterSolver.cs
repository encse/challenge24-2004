using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest15.P
{
    class PCodeCounterSolver : Solver
    {
        public override void Solve()
        {
            var rgsample = Wavu.Rgsample16bit(FpatIn);
          //  rgsample = rgsample.Select(s => Math.Abs()).ToArray();
            var blocks = Wavu.RgsampleBlock(rgsample[0], 1400, 20).Where(b => b.Length > 500).ToArray();
            var tUnit = 1000;

            Wavu.Block<short> blockPrev = null;
          
            var st = "";
            using (Output)
            {
                foreach (var block in blocks)
                {
                    
                    if (blockPrev != null)
                    {
                        var dist = block.IsampleStart - blockPrev.IsampleStart - blockPrev.Length;
                     //  Console.WriteLine(dist / tUnit);
                        if (dist > 15*tUnit)
                        {
                            var decoded = Decode(st);
                            Write(decoded);
                            Console.Write(decoded);
                            st = "";
                        }

                        if (dist > 25*tUnit)
                        {
                            var decoded = " ";
                            Write(decoded);
                            Console.Write(decoded);
                        }
                        
                    }

                    if (block.Length < 2*tUnit)
                        st += ".";
                    else
                        st += "-";
                    blockPrev = block;
                }

                {
                    var decoded = Decode(st);
                    Write(decoded);
                    Console.Write(decoded);
                }
            }

            Console.WriteLine();
        }

        Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                {"a", ".-"},
                {"b", "-..."},
                {"c", "-.-."},
                {"d", "-.."},
                {"e", "."},
                {"f", "..-."},
                {"g", "--."},
                {"h", "...."},
                {"i", ".."},
                {"j", ".---"},
                {"k", "-.-"},
                {"l", ".-.."},
                {"m", "--"},
                {"n", "-."},
                {"o", "---"},
                {"p", ".--."},
                {"q", "--.-"},
                {"r", ".-."},
                {"s", "..."},
                {"t", "-"},
                {"u", "..-"},
                {"v", "...-"},
                {"w", ".--"},
                {"x", "-..-"},
                {"y", "-.--"},
                {"z", "--.."},
                {"0", "-----"},
                {"1", ".----"},
                {"2", "..---"},
                {"3", "...--"},
                {"4", "....-"},
                {"5", "....."},
                {"6", "-...."},
                {"7", "--..."},
                {"8", "---.."},
                {"9", "----."},
                {".", ".-.-.-"},
                {",", "--..--"},
                {":", "---..."},
                {"?", "..--.."},
                {"'", ".----."},
                {"-", "-....-"},
                {"/", "-..-."},
            };

        private string Decode(string st)
        {
            foreach (var kvp in alphabet)
            {
                if (kvp.Value == st)
                    return kvp.Key;
            }
            return "$";
        }
    }
}

