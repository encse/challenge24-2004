using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2013.QR.C
{
    /* https://www.facebook.com/hackercup/problems.php?pid=494433657264959&round=185564241586420
     * After sending smileys, John decided to play with arrays. Did you know that hackers enjoy playing with arrays? John has 
     * a zero-based index array, m, which contains n non-negative integers. However, only the first k values of the array are 
     * known to him, and he wants to figure out the rest.
     *
     * John knows the following: for each index i, where k <= i < n, m[i] is the minimum non-negative integer which is *not* 
     * contained in the previous *k* values of m.
     *
     * For example, if k = 3, n = 4 and the known values of m are [2, 3, 0], he can figure out that m[3] = 1.
     *
     * John is very busy making the world more open and connected, as such, he doesn't have time to figure out the rest of 
     * the array. It is your task to help him.
     *
     * Given the first k values of m, calculate the nth value of this array. (i.e. m[n - 1]).
     *
     * Because the values of n and k can be very large, we use a pseudo-random number generator to calculate the first k 
     * values of m. Given non-negative integers a, b, c and positive integer r, the known values of m can be calculated as follows:
     *
     * m[0] = a
     * m[i] = (b * m[i - 1] + c) % r, 0 < i < k
     * 
     * Input
     * The first line contains an integer T (T <= 20), the number of test cases.
     * This is followed by T test cases, consisting of 2 lines each.
     * The first line of each test case contains 2 space separated integers, n, k (1 <= k <= 105, k < n <= 109).
     * The second line of each test case contains 4 space separated integers a, b, c, r (0 <= a, b, c <= 109, 1 <= r <= 109).
     * 
     * Output
     * For each test case, output a single line containing the case number and the nth element of m.
     */
    public class CFindTheMinSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n, k;
            pparser.Fetch(out n, out k);
            int a, b, c, r;
            pparser.Fetch(out a, out b, out c, out r);
            return () => Solve(n,k,a,b,c,r);
        }

        private static IEnumerable<object> Solve(int n, int k, int a, BigInteger b, int c, int r)
        {
            var hlmNumbers = new Dictionary<int, int>();
            var numbers = new int[k+1];
            var m = -1;
            for (int i = 0; i < k; i++)
            {
                if (i == 0)
                    m = a;
                else
                    m = (int)((b * m + c) % r);
               
                numbers[i] = m;
               
                if (hlmNumbers.ContainsKey(m))
                    hlmNumbers[m]++;
                else
                    hlmNumbers[m] = 1;
            }

            var x = 0;
            var hint = -1;
            for (int i = 0; i < k + 1; i++)
            {
                m = numbers[(i%k)];
                int newNum;
                if (hint != -1)
                {
                    newNum = hint;
                    hint = -1;
                }
                else
                {
                    while (hlmNumbers.ContainsKey(x))
                        x++;
                    newNum = x;
                }
                
                numbers[i] = newNum;

                if (hlmNumbers[m] == 1)
                {
                    hlmNumbers.Remove(m);
                    if(m < x)
                        hint = m;
                }
                else
                {
                    hlmNumbers[m]--;
                }
                
                if (hlmNumbers.ContainsKey(newNum))
                    throw new Exception("wtf");
                hlmNumbers[newNum] = 1;
            }

            yield return numbers[(n)%(k+1)];
        }

    }
}
