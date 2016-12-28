using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.QR.A
{
	public class CountingSheepSolver : IConcurrentSolver
	{
		public int CCaseGet(Pparser pparser)
		{
			return pparser.Fetch<int>();
		}

		private int qqq = 1000000;
		public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
		{
			var n = pparser.Fetch<int>();
			return () => Solve(n);
		}

		private IEnumerable<object> Solve(int n)
		{
			if (n == 0)
			{
				yield return "INSOMNIA";
				yield break;
			}

			var digits = new bool[10];
			var lastN = new BigInteger(n);
			while (true)
			{
				foreach (var ch in (lastN).ToString())
				{
					var d = ch - '0';
					digits[d] = true;
				}
				if (digits.All())
				{
					yield return lastN;
					yield break;
				}
				lastN +=n;
			}
		}
	}
}