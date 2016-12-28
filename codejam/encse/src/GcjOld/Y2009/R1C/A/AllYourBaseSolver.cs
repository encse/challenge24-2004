using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2009.R1C.A
{
    public class AllYourBaseSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var st = Pparser.StLineNext();
            var digits = new Dictionary<char, int>();
            foreach (var ch in st)
                digits[ch] = -1;

            var bse = digits.Count;
            if (bse == 1)
                bse++;
            long result = 0;

            var digitsNotUsed = new List<int>();
            for (int i = 0; i < bse;i++ )
                digitsNotUsed.Add(i);

            digitsNotUsed[0] = 1;
            digitsNotUsed[1] = 0;

            foreach (var ch in st)
            {
                var digit = digits[ch];
                if (digit == -1)
                {
                    digit = digitsNotUsed.First();
                    digits[ch] = digit;
                    digitsNotUsed.RemoveAt(0);
                }
                result = result * bse + digit;
            }
            yield return result;
        }

    }
}
