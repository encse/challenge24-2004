using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2013.QR.A
{
    /* https://www.facebook.com/hackercup/problems.php?pid=494433657264959&round=185564241586420
     * When John was a little kid he didn't have much to do. There was no internet, no Facebook, and no programs to hack on. 
     * So he did the only thing he could... he evaluated the beauty of strings in a quest to discover the most beautiful string in the world.
     * Given a string s, little Johnny defined the beauty of the string as the sum of the beauty of the letters in it.
     *
     * The beauty of each letter is an integer between 1 and 26, inclusive, and no two letters have the same beauty. 
     * Johnny doesn't care about whether letters are uppercase or lowercase, so that doesn't affect the beauty of a letter. 
     * (Uppercase 'F' is exactly as beautiful as lowercase 'f', for example.)
     *
     * You're a student writing a report on the youth of this famous hacker. You found the string that Johnny considered most beautiful.
     * What is the maximum possible beauty of this string?
     *
     * Input
     * The input file consists of a single integer m followed by m lines.
     * 
     * Output
     * Your output should consist of, for each test case, a line containing the string "Case #x: y" where x is the case number 
     * (with 1 being the first case in the input file, 2 being the second, etc.) and y is the maximum beauty for that test case.
     * 
     * Constraints
     * 5 ≤ m ≤ 50
     * 2 ≤ length of s ≤ 500
     */
    public class ABeautifulStringsSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            return () => Solve(pparser.Fetch<string>());
        }

        private static IEnumerable<object> Solve(string st)
        {
            var max = 0;
            var value = 26;
            foreach (var cch in from ch in st.ToLower() 
                                where ch >= 'a' && ch <= 'z' 
                                group ch by ch into g
                                orderby g.Count() descending 
                                select g.Count())
            {
                max += cch*value;
                value--;
            }
            yield return max;
        }
    }
}
