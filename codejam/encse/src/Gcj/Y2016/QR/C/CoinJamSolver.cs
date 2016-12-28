using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.QR.C
{
    public class CoinJamSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n, j;
            pparser.Fetch(out n, out j);
          
            return () => Solve(new Random(01), n, j);
        }

		private IEnumerable<object> Solve(Random r, int n, int j)
		{
			var lim = 1 << (n - 2);
			var results = new HashSet<int>();

			int i = 0;
			while(i < j)
			{
				var x = r.Next(lim);
				var c = (1 << (n-1)) + (x << 1) + 1;
				if (results.Contains(c))
					continue;
				results.Add(c);

				var divisors = Divisors(c);
				if (divisors != null)
				{
					i++;
					Console.Write(".");
					yield return Solwrt.NewLine;
					yield return Convert.ToString(c, 2);
					foreach (var divisor in divisors)
						yield return divisor;
				}
			}
        }

		static BigInteger FromBase(string st, int b)
		{
			return st.Aggregate(BigInteger.Zero, (current, t) => b * current + t - '0');
		}

	    private List<int> Divisors(int candidate)
	    {
		    int p = 0;
		    var divisors = new List<int>();
		    var st = Convert.ToString(candidate, 2);
		    for (var b = 2; b <= 10; b++)
		    {
			  
			    var n = FromBase(st, b);

			    var prime = true;
			    for (int divisor = 2; divisor*divisor <= n; divisor++)
			    {
					p++;
					if (p > 100000)
						return null;

					if (n % divisor == 0)
				    {
					    divisors.Add(divisor);
					    prime = false;
					    break;
				    }
			    }

			    if (prime)
				    return null;
		    }
		    return divisors;
	    }

    }
}