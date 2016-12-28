using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2013.QR.B
{
    /* https://www.facebook.com/hackercup/problems.php?pid=494433657264959&round=185564241586420
     * Your friend John uses a lot of emoticons when you talk to him on Messenger. In addition to being a person who likes to express 
     * himself through emoticons, he hates unbalanced parenthesis so much that it makes him go :(
     *
     * Sometimes he puts emoticons within parentheses, and you find it hard to tell if a parenthesis really is a parenthesis or part of an emoticon.
     *
     * A message has balanced parentheses if it consists of one of the following:
     *
     * - An empty string ""
     * - One or more of the following characters: 'a' to 'z', ' ' (a space) or ':' (a colon)
     * - An open parenthesis '(', followed by a message with balanced parentheses, followed by a close parenthesis ')'.
     * - A message with balanced parentheses followed by another message with balanced parentheses.
     * - A smiley face ":)" or a frowny face ":("
     * Write a program that determines if there is a way to interpret his message while leaving the parentheses balanced.
     *
     * Input
     * The first line of the input contains a number T (1 ≤ T ≤ 50), the number of test cases. 
     * The following T lines each contain a message of length s that you got from John.
     *
     * Output
     * For each of the test cases numbered in order from 1 to T, output "Case #i: " followed by a string stating whether or not it is possible
     * that the message had balanced parentheses. If it is, the string should be "YES", else it should be "NO" (all quotes for clarity only)
     *
     * Constraints
     * 1 ≤ length of s ≤ 100
    */
    public class BBalancedSmileysSolver : IConcurrentSolver
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
            if (st.Any(ch => ch != ' ' && ch != ':' && ch != '(' && ch != ')' && !(ch >= 'a' && ch <= 'z')))
            {
                var cht = st.First(ch => ch != ' ' && ch != ':' && !(ch >= 'a' && ch <= 'z'));
                yield return "NO";
            }
            else
                yield return SolveRecursive(st, 0, 0) ? "YES" : "NO";
        }

        private static bool SolveRecursive(string st, int ich, int depth)
        {
            if (ich == st.Length)
                return depth == 0;
            if (depth < 0)
                return false;
            var ch = st[ich];
            var chNext  = ich < st.Length - 1 ? st[ich+1]:'\0';

            if (ch == ')')
                return SolveRecursive(st, ich+1, depth - 1);
            if (ch == '(')
                return SolveRecursive(st, ich+1, depth + 1);
            if (ch == ':' && chNext == ')')
                return SolveRecursive(st, ich + 2, depth) || SolveRecursive(st, ich + 2, depth - 1);
            if (ch == ':' && chNext == '(')
                return SolveRecursive(st, ich + 2, depth) || SolveRecursive(st, ich + 2, depth + 1);
            
            return SolveRecursive(st, ich + 1, depth);
        }
    }
}
