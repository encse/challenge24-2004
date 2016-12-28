using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2014.QR.C
{
    /* https://www.facebook.com/hackercup/problems.php?round=598486203541358
     * You may be familiar with the works of Alfred Lord Tennyson, the famous English poet. In this problem we will 
     * concern ourselves with Tennison, the less famous English tennis player. As you know, tennis is not so much a 
     * game of skill as a game of luck and weather patterns. The goal of tennis is to win K sets before the other player.
     * However, the chance of winning a set is largely dependent on whether or not there is weather.
     *
     * Tennison plays best when it's sunny, but sometimes of course, it rains. Tennison wins a set with probability
     * pWinIfSun when it's sunny, and with probability pWinIfRain when it's raining. The chance that there will be sun for the first 
     * set is pSun0. Luckily for Tennison, whenever he wins a set, the probability that there will be sun increases by pu 
     * with probability pw. Unfortunately, when Tennison loses a set, the probability of sun decreases by pd with probability
     * pl. What is the chance that Tennison will be successful in his match?
     *
     * Rain and sun are the only weather conditions, so P(rain) = 1 - P(sun) at all times. Also, probabilities always
     * stay in the range [0, 1]. If P(sun) would ever be less than 0, it is instead 0. If it would ever be greater than 1, 
     * it is instead 1.
     *
     * Input
     * Input begins with an integer T, the number of tennis matches that Tennison plays. For each match, there is a line 
     * containing an integer K, followed by the probabilities pWinIfSun, pWinIfRain, pSun0, pu, pw, pd, pl in that order. All of these values 
     * are given with exactly three places after the decimal point.
     *
     * Output
     * For each match, output "Case #i: " followed by the probability that Tennison wins the match, rounded to 6 decimal 
     * places (quotes for clarity only). It is guaranteed that the output is unaffected by deviations as large as 10-8.
     *
     * Constraints
     * 1 ≤ T ≤ 100
     * 1 ≤ K ≤ 100
     * 0 ≤ pWinIfSun, pWinIfRain, pSun0, pu, pw, pd, pl ≤ 1
     * pWinIfSun > pWinIfRain
     *
     */
    public class CTennisonSolver : IConcurrentSolver
    {
      

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var solver = pparser.Fetch<SolverI>();
            return () => solver.Solve();
        }

        public class SolverI { 
            private readonly int k;
            private readonly double pWinIfRain;
            private readonly double pWinIfSun;
            private readonly double pSun0;
            private readonly double dpSunIfWinsAndUpdate;
            private readonly double pUpdateIfWins;
            private readonly double pUpdateIfLoses;
            private readonly double dpSunIfLosesAndUpdate;

            public SolverI(int k, double pWinIfSun, double pWinIfRain, double pSun0, double dpSunIfWinsAndUpdate, double pUpdateIfWins, double dpSunIfLosesAndUpdate, double pUpdateIfLoses)
            {
                this.k = k;
                this.pWinIfSun = pWinIfSun;
                this.pWinIfRain = pWinIfRain;
                this.pSun0 = pSun0;
                this.dpSunIfWinsAndUpdate = dpSunIfWinsAndUpdate;
                this.pUpdateIfWins = pUpdateIfWins;
                this.dpSunIfLosesAndUpdate = dpSunIfLosesAndUpdate;
                this.pUpdateIfLoses = pUpdateIfLoses;
            }

            public IEnumerable<object> Solve()
            {
                yield return F(0, 0, pSun0, new Dictionary<Tuple<int,int,int>, double>());
            }

            private double F(int cWin, int cLose, double pSun, Dictionary<Tuple<int,int,int>, double> cache)
            {
                if (pSun < 0) pSun = 0;
                if (pSun > 1) pSun = 1;

                if (cWin == k)
                    return 1;
                if (cLose == k)
                    return 0;
                var key = new Tuple<int, int, int>(cWin, cLose, (int)Math.Truncate(pSun*10000));

                if (!cache.ContainsKey(key))
                {  

                    cache[key] = 
                              pSun *        pWinIfSun *        pUpdateIfWins * F(cWin + 1,     cLose,  pSun + dpSunIfWinsAndUpdate, cache ) +
                              pSun *        pWinIfSun *  (1 - pUpdateIfWins) * F(cWin + 1,     cLose,                         pSun, cache ) +
                              pSun *  (1 - pWinIfSun) *       pUpdateIfLoses * F(    cWin, cLose + 1, pSun - dpSunIfLosesAndUpdate, cache ) +
                              pSun *  (1 - pWinIfSun) * (1 - pUpdateIfLoses) * F(    cWin, cLose + 1,                         pSun, cache ) +
                        (1 - pSun) *       pWinIfRain *        pUpdateIfWins * F(cWin + 1,     cLose,  pSun + dpSunIfWinsAndUpdate, cache ) +
                        (1 - pSun) *       pWinIfRain *  (1 - pUpdateIfWins) * F(cWin + 1,     cLose,                         pSun, cache ) +
                        (1 - pSun) * (1 - pWinIfRain) *       pUpdateIfLoses * F(    cWin, cLose + 1, pSun - dpSunIfLosesAndUpdate, cache ) +
                        (1 - pSun) * (1 - pWinIfRain) * (1 - pUpdateIfLoses) * F(    cWin, cLose + 1,                         pSun, cache );

                }
                 
                return cache[key];
            }
        }
    }
}
