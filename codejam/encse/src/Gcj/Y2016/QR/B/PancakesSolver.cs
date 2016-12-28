using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.QR.B
{
    public class PancakesSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
	        var pancakes = pparser.Fetch<string>();
          
            return () => Solve(pancakes);
        }

        private IEnumerable<object> Solve(string pancakes)
        {
            yield return Solve2(pancakes, pancakes.Length - 1, '+');
        }

	    private int Solve2(string pancakes, int ichLast, char chLast)
	    {
		    if (ichLast == -1)
			    return 0;
		    else if (pancakes[ichLast] == chLast)
			    return Solve2(pancakes, ichLast - 1, chLast);
		    else
				return 1 + Solve2(pancakes, ichLast - 1, chLast == '-' ? '+' : '-');
	    }
    }
}
