using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.QR.B
{
    class BNewYearsSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var dstFood = pparser.Fetch<int[]>();
            var cFood = pparser.Fetch<int>();
            var rgsrcFood = pparser.FetchN<int[]>(cFood);
            return () => Solve(dstFood, rgsrcFood);
        }

        private static IEnumerable<string> Solve(int[] dstFood, List<int[]> rgsrcFood)
        {
            yield return SolveRecursive(dstFood, rgsrcFood, 0) ? "yes" : "no";
        }

        private static bool SolveRecursive(int[] dstFood, List<int[]> rgsrcFood, int isrc)
        {
            if (Empty(dstFood))
                return true;
            if (isrc == rgsrcFood.Count)
                return false;

            if (CanEat(dstFood, rgsrcFood[isrc]) && SolveRecursive(Eat(dstFood, rgsrcFood[isrc]), rgsrcFood, isrc + 1))
                return true;

            return SolveRecursive(dstFood, rgsrcFood, isrc + 1);
        }

        private static bool Empty(int[] food)
        {
            return food.All(i => i == 0);
        }
        
        private static bool CanEat(int[] foodLimit, int[] food)
        {
            return foodLimit.Zip(food, (gA, gB) => gA >= gB).All(b => b);
        }

        private static int[] Eat(int[] foodLimit, int[] food)
        {
            return foodLimit.Zip(food, (gA, gB) => gA - gB).ToArray();
        }
    }
}
