using System.Collections.Generic;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.R1A.A
{
	public class TheLastWordSolver : IConcurrentSolver
	{
		public int CCaseGet(Pparser pparser)
		{
			return pparser.Fetch<int>();
		}

		public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
		{
			var n = pparser.Fetch<string>();
			return () => Solve(n);
		}

		private IEnumerable<object> Solve(string st)
		{
			var res = "";
			foreach(var ch in st)
			{
				if (res == "" || ch < res[0])
					res += ch;
				else
					res = ch + res;
			}
			yield return res;
		}
	}
}