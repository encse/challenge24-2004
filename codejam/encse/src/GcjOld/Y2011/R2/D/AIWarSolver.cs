using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2011.R2.D
{
    public class AIWarSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             * The first line of the input gives the number of test cases, T. T test cases follow. 
             * Each test case starts with a single line containing two space-separated integers: 
             * P, the number of planets, and W, the number of wormholes. Your home planet is planet 0, and the A.I.'s home planet is planet 1.
                The second line of each test case will contain W space-separated pairs of comma-separated integers xi,yi. 
             * Each of these indicates that there is a two-way wormhole connecting planets xi and yi.
             */
            int cplanet, chole;
            pparser.Fetch(out cplanet, out chole);
            var map = new List<int>[cplanet];
            for (int i = 0; i < cplanet; i++)
                map[i] = new List<int>();
            foreach (var stA in pparser.StLineNext().Split(' '))
            {
                var rgst = stA.Split(',');

                var iFrom = int.Parse(rgst[0]);
                var iTo = int.Parse(rgst[1]);
                map[iFrom].Add(iTo);
                map[iTo].Add(iFrom);
            }
            return () => Solve(new Map(map));
        }

        private IEnumerable<object> Solve(Map map)
        {
            var rgpath = new Iddf<int>(0, map.EnIplanetTo, iplanet => map.FThreatens(iplanet,1)).EnpathFind().ToList();
            var chop = rgpath.First().Count()-1;
            var x = MinMaxKer.WAndRgtMax(rgpath, map.CThreaten);
            var cthreaten = x.Item1;
            Console.Write(".");
            yield return "{0} {1}".StFormat(chop, cthreaten);
        }

    }

    internal class Map
    {
        private readonly int[][] map;
        private readonly int cplanet;

        public Map(List<int>[] mapT)
        {
            cplanet = mapT.Length;
            map = new int[cplanet][];
            for(int i=0;i<cplanet;i++)
            {
                mapT[i].Sort();
                map[i] = mapT[i].ToArray();
            }
        }

        public bool FThreatens(int iplanetA, int iplanetB)
        {
            return Array.BinarySearch(map[iplanetA], iplanetB) >= 0;
        }

        public List<int> EnIplanetTo(Hlm_Chewbacca<int> hlmplanetOwned, int iplanetFrom)
        {
            var rgiplanetNext = new List<int>();
            foreach(var iplanetTo in map[iplanetFrom])
            {
               if(!hlmplanetOwned.Contains(iplanetTo))
                    rgiplanetNext.Add(iplanetTo);
            }

            rgiplanetNext.Sort((iplanetA, iplanetB) => CThreaten(hlmplanetOwned, iplanetB) - CThreaten(hlmplanetOwned, iplanetA));
            return rgiplanetNext;
        }
        
        public int CThreaten(Hlm_Chewbacca<int> hlmplanetOwned, int iplanetFrom)
        {
            return map[iplanetFrom].Count(iplanetTo => !hlmplanetOwned.Contains(iplanetTo));
        }

        public int CThreaten(IEnumerable<int> path)
        {
            var hlmVisited = new Hlm_Chewbacca<int>(path);
            var hlmThreaten = new Hlm_Chewbacca<int>();
            foreach (var iplanetFrom in path)
            {
                foreach (var iplanetTo in map[iplanetFrom])
                {
                    if (!hlmVisited.Contains(iplanetTo))
                        hlmThreaten.Add(iplanetTo);
                }

            }
            return hlmThreaten.Count;
        }

     
    }
}
