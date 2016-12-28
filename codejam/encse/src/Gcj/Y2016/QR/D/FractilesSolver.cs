using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.QR.D
{
	public class FractilesSolver : IConcurrentSolver
	{
		//small only
		public int CCaseGet(Pparser pparser)
		{
			return pparser.Fetch<int>();
		}

		public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
		{
			int k,c,s;
			pparser.Fetch(out k, out c, out s);
          
			return () => Solve(k,c,s);
		}

		private IEnumerable<object> Solve(int k, int c, int s)
		{
			if (c > 1 && s >= (k + 1)/2)
			{
				var ctipp = (k + 1)/2;
				for (int iblock = 1; iblock <= ctipp; iblock++)
				{
					var tipp = k*iblock - iblock + 1;
					yield return tipp ;
				}
			}
			else if (s >= k)
			{
				for (int i = 1; i <= k; i++)
					yield return i;
			}
			else
				yield return "IMPOSSIBLE";
		}

	}
}