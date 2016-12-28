using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2016.R1A.B
{
	public class RankAndFileSolver : IConcurrentSolver
	{
		public int CCaseGet(Pparser pparser)
		{
			return pparser.Fetch<int>();
		}

		public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
		{
			var n = pparser.Fetch<int>();
			var nums = pparser.FetchN<int[]>(2 * n -1);
			return () => Solve(nums);
		}

		private IEnumerable<object> Solve(List<int[]> nums)
		{
			var x = new Dictionary<int,int>();
			foreach (var row in nums)
			{
				foreach (var num in row)
				{
					if (!x.ContainsKey(num))
						x[num] = 0;
					x[num]++;
				}
			}

			var missing = new HashSet<int>();
			foreach (var num in x.Keys)
			{
				if (x[num]%2 == 1)
					missing.Add(num);
			}
			return missing.OrderBy(t => t).Cast<object>();
		}

		private IEnumerable<object> Solve2(List<int[]> nums)
		{
			var egyforma = 0;
			nums.Sort((rgx, rgy) =>
			{
				for (var i = 0; i < rgx.Length; i++)
				{
					if (rgx[i] < rgy[i])
						return -1;
					if (rgx[i] > rgy[i])
						return 1;
				}
				egyforma++;
				return 0;
			});


			var rows = new List<int[]> (){};
			var cols = new List<int[]> () {nums[0]};
			var skippedCol = -1;
			var skippedRow = -1;

			var crow = (nums.Count + 1) / 2;
			var ccol = crow;
			var diagonal = new int[ccol];

			var nums2 = new List<int[]>(nums);

			for (var icol = 0; icol < ccol; icol++)
			{
				var min = int.MaxValue;
				foreach (var row in nums2)
					min = Math.Min(min, row[icol]);

				diagonal[icol] = min;
				nums2 = nums2.Where(row => row[icol] != min).ToList();
			}

		//	Console.WriteLine($"egyforma {egyforma} crow {crow}");

			if (SolveI(nums, rows, cols, diagonal, 1, ref skippedRow, ref skippedCol))
			{
				if (skippedRow == -1)
				{
					for (int irow = 0; irow < crow; irow++)
						yield return rows[irow][skippedCol];
				}
				else
				{
					for (int icol = 0; icol < ccol; icol++)
						yield return cols[icol][skippedRow];
				}
			}
			else
			{
				Console.WriteLine("coki");
			}
		}
		private bool SolveI(List<int[]> nums, List<int[]> rows, List<int[]> cols, int[] diagonal, int i, ref int skippedRow, ref int skippedCol)
		{
			var crow = (nums.Count + 1)/2;
			var ccol = crow;
			if (i == nums.Count)
			{
				if (skippedRow == -1 && skippedCol == -1)
				{
					if (rows.Count < crow)
					{
						rows.Add(null);
						skippedRow = crow - 1;
					}
					else if (cols.Count < ccol)
					{
						cols.Add(null);
						skippedCol = ccol - 1;
					}
					else
					{
						Console.WriteLine("coki2");
					}
				}
				return true;
			}


			if (cols.Count < ccol && Match(rows, cols.Count, nums[i], diagonal))
			{
				cols.Add(nums[i]);
				if (SolveI(nums, rows, cols, diagonal, i + 1, ref skippedRow, ref skippedCol))
					return true;
				cols.RemoveAt(cols.Count - 1);
			}

			if (rows.Count < crow && Match(cols, rows.Count, nums[i], diagonal))
			{
				rows.Add(nums[i]);
				if (SolveI(nums, rows, cols, diagonal, i + 1, ref skippedRow, ref skippedCol))
					return true;
				rows.RemoveAt(rows.Count - 1);
			}

			if (skippedRow == -1 && skippedCol == -1)
			{

				if (cols.Count < ccol)
				{
					skippedCol = cols.Count;
					cols.Add(null);

					if (SolveI(nums, rows, cols, diagonal, i, ref skippedRow, ref skippedCol))
						return true;
					cols.RemoveAt(cols.Count - 1);
					skippedCol = -1;
				}

				if (rows.Count < crow)
				{
					skippedRow = rows.Count;
					rows.Add(null);
					if (SolveI(nums, rows, cols, diagonal, i, ref skippedRow, ref skippedCol))
						return true;
					skippedRow = -1;
					rows.RemoveAt(rows.Count - 1);
				}
			}

			return false;
		}

		private bool Match(List<int[]> cols, int irow, int[] row, int[] diagonal)
		{
			if (row[irow] > diagonal[irow])
				return false;

			int icol = 0;
			foreach(var col in cols)
			{
				if (col != null)
				{
					var numA = col[irow];
					var numB = row[icol];
					if (numA != numB)
						return false;
				}
				icol++;
			}
			return true;
		}
	}
}