﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Xml;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2014.QR.B
{
    /* https://www.facebook.com/hackercup/problems.php?round=598486203541358
     *  A group of N high school students wants to play a basketball game. To divide themselves 
     *  into two teams they first rank all the players in the following way:
     *
     *  Players with a higher shot percentage are rated higher than players with a lower shot percentage.
     *  If two players have the same shot percentage, the taller player is rated higher.
     *  Luckily there are no two players with both the same shot percentage and height so they are able 
     *  to order themselves in an unambiguous way. Based on that ordering each player is assigned a draft
     *  number from the range [1..N], where the highest-rated player gets the number 1, the second 
     *  highest-rated gets the number 2, and so on. Now the first team contains all the players with the 
     *  odd draft numbers and the second team all the players with the even draft numbers.
     *
     *  Each team can only have P players playing at a time, so to ensure that everyone gets similar time 
     *  on the court both teams will rotate their players according to the following algorithm:
     *
     *  Each team starts the game with the P players who have the lowest draft numbers.
     *  If there are more than P players on a team after each minute of the game the player with the highest 
     *  total time played leaves the playing field. Ties are broken by the player with the higher draft number
     *  leaving first.
     *  To replace her the player on the bench with the lowest total time played joins the game. Ties are
     *  broken by the player with the lower draft number entering first.
     *  The game has been going on for M minutes now. Your task is to print out the names of all the players 
     *  currently on the field, (that is after M rotations).
     *
     *  Input
     *  The first line of the input consists of a single number T, the number of test cases.
     *
     *  Each test case starts with a line containing three space separated integers N M P
     *
     *  The subsequent N lines are in the format "<name> <shot_percentage> <height>". See the example for clarification.
     *
     *  Constraints
     *  1 ≤ T ≤ 50
     *  2 * P ≤ N ≤ 30
     *  1 ≤ M ≤ 120
     *  1 ≤ P ≤ 5
     *  Each name starts with an uppercase English letter, followed by 0 to 20 lowercase English letters. There can be
     *  players sharing the same name. Each shot percentage is an integer from the range [0..100]. Each height is an 
     *  integer from the range [100..240]
     *
     *  Output
     *  For each test case i numbered from 1 to T, output "Case #i: ", followed by 2 * P space separated names of the
     *  players playing after M rotations. The names should be printed in lexicographical order.
     *
     */
    public class BBasketballGameSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n, m, p;
            pparser.Fetch(out n, out m, out p);
            var rgplayer = pparser.FetchN<Player>(n);

            return () => Solve(m, p, rgplayer);
        }

        private IEnumerable<object> Solve(int M, int P, List<Player> rgplayer)
        {
            rgplayer = rgplayer
                .OrderByDescending(player => player.ShotPercentage)
                .ThenByDescending(player => player.Height)
                .ToList();
            
            for (var i = 0; i < rgplayer.Count; i++)
                rgplayer[i].DraftNumber = i;
            
            var teamA = rgplayer.Where(player => player.DraftNumber % 2 == 0).ToList();
            var teamB = rgplayer.Where(player => player.DraftNumber % 2 == 1).ToList();

            var playingA = new HashSet<Player>(teamA.Take(P));
            var playingB = new HashSet<Player>(teamB.Take(P));
            var benchA = new HashSet<Player>(teamA.Skip(P));
            var benchB = new HashSet<Player>(teamB.Skip(P));

            for (var m = 0; m < M; m++)
            {
                PlayAndSwap(benchA, playingA);
                PlayAndSwap(benchB, playingB);
            }

            yield return playingA.Concat(playingB)
                .Select(player => player.Name)
                .OrderBy(name => name, StringComparer.InvariantCulture)
                .StJoin(" ");
        }

        private void PlayAndSwap(HashSet<Player> hlmPlayerBench, HashSet<Player> hlmPlayerPlaying)
        {
            foreach (var player in hlmPlayerPlaying)
                player.PlayTime++;

            if (hlmPlayerBench.Count == 0)
                return;

            var longestPlayTime = MinMaxKer.WAndRgtMax(hlmPlayerPlaying, player => player.PlayTime).Item2;
            var playerLeaving = MinMaxKer.WAndRgtMax(longestPlayTime, player => player.DraftNumber).Item2.Single();

            var shortestPlayTime = MinMaxKer.WAndRgtMin(hlmPlayerBench, player => player.PlayTime).Item2;
            var playerJoining = MinMaxKer.WAndRgtMin(shortestPlayTime, player => player.DraftNumber).Item2.Single();

            hlmPlayerBench.Remove(playerJoining);
            hlmPlayerPlaying.Add(playerJoining);

            hlmPlayerPlaying.Remove(playerLeaving);
            hlmPlayerBench.Add(playerLeaving);
        }

        public class Player
        {
            public readonly string Name;
            public readonly int ShotPercentage;
            public readonly int Height;
            public int DraftNumber;
            public int PlayTime;

            public Player(string name, int shotPercentage, int height)
            {
                this.Name = name;
                this.ShotPercentage = shotPercentage;
                this.Height = height;
            }
        }

    }
}
