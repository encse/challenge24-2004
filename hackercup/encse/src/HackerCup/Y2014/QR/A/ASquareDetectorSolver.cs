using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Xml;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2014.QR.A
{
    /* https://www.facebook.com/hackercup/problems.php?round=598486203541358
     * You want to write an image detection system that is able to recognize different geometric shapes. 
     * In the first version of the system you settled with just being able to detect filled squares on a grid.
     *
     * You are given a grid of N×N square cells. Each cell is either white or black. Your task is to detect 
     * whether all the black cells form a square shape.
     *
     * Input
     * The first line of the input consists of a single number T, the number of test cases.
     *
     * Each test case starts with a line containing a single integer N. Each of the subsequent N lines contain
     * N characters. Each character is either "." symbolizing a white cell, or "#" symbolizing a black cell. 
     * Every test case contains at least one black cell.
     *
     * Output
     * For each test case i numbered from 1 to T, output "Case #i: ", followed by YES or NO depending on whether
     * or not all the black cells form a completely filled square with edges parallel to the grid of cells.
     *
     * Constraints
     * 1 ≤ T ≤ 20
     * 1 ≤ N ≤ 20
     *
     */
    public class ASquareDetectorSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var rgst = pparser.FetchN<string>(pparser.Fetch<int>());
            return () => Solve(rgst);
        }

        private IEnumerable<object> Solve(List<string> rgst)
        {
            var crow = rgst.Count;
            var ccol = rgst[0].Length;

            for (var irow = 0; irow < crow; irow++)
            {
                for (var icol = 0; icol < ccol; icol++)
                {
                    if (rgst[irow][icol] == '#')
                    {
                        yield return 
                            Check(rgst, irow, icol, Right(rgst, irow, icol), Bottom(rgst, irow, icol)) ? "YES" : "NO";
                        yield break;
                    }   
                }
            }
        }

        private int Bottom(List<string> rgst, int irow, int icol)
        {
            var crow = rgst.Count;
            while (irow < crow && rgst[irow][icol] == '#')
                irow++;
            return irow - 1;
        }

        private int Right(List<string> rgst, int irow, int icol)
        {
            var ccol = rgst[0].Length;
            while (icol < ccol && rgst[irow][icol] == '#')
                icol++;
            return icol - 1;
        }

        private bool Check(List<string> rgst, int top, int left, int right, int bottom)
        {
            if (right - left != bottom - top)
                return false;

            var crow = rgst.Count;
            var ccol = rgst[0].Length;

            for (int irow = 0; irow < crow; irow++)
            {
                for (int icol = 0; icol < ccol; icol++)
                {
                    var fInside = irow >= top && irow <= bottom && icol >= left && icol <= right;
                    if (fInside && rgst[irow][icol] != '#')
                        return false;
                    if (!fInside && rgst[irow][icol] == '#')
                        return false;
                }
            }

            return true;
        }
    }
}
