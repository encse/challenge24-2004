using System.Collections.Generic;
using System.Numerics;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest15.A
{
    class ATropesSolver : Solver
    {
        public override void Solve()
        {
            var cTest = Fetch<int>();

            using (Output)
            {
                for (var iTest = 0; iTest < cTest; iTest++)
                {
                    var cSeries = Fetch<int>();
                    var series = new List<int[]>();
                    for (var i = 0; i < cSeries; i++)
                    {
                        var l = Fetch<int>();
                        series.Add(Fetch<int[]>());
                    }

                    var res = SolveI(series);
                    Output.WriteLine(res);
                }
            }
            
        }

        private int SolveI(List<int[]> series)
        {

            var hlmLetters = new HashSet<int>();
            foreach (var s in series)
                foreach (var ch in s)
                    hlmLetters.Add(ch);
            var letters = new List<int>(hlmLetters);
            var pos = new int[series.Count];
            var Q = MinMaxKer.WMax(series, s => s.Length)+2;
            return SolveR(series, letters, pos, new Cache(), Q);
        }

        private int SolveR(List<int[]> series, List<int> letters, int[] pos, Cache cache, int Q)
        {
            for(var i = 0;i<pos.Length;i++)
                if (series[i].Length == pos[i])
                    return 0;

            var key = GetKey(pos, series, Q);
            if (cache.ContainsKey(key))
                return cache[key];
            var maxLen = 0;
            foreach(var letter in letters)
            {
                var posNew = PosNew(series, letter, pos);
                if(posNew == null)
                    continue;

                var len = 1 + SolveR(series, letters, posNew, cache, Q);
                if (len > maxLen)
                    maxLen = len;
            }

            cache[key] = maxLen;
            return maxLen;
        }

        private BigInteger GetKey(int[] pos, List<int[]> series, int Q)
        {
            BigInteger key = 0;
            for(var i = 0;i<series.Count;i++)
            {
                if (i > 0)
                    key = key*(series[i - 1].Length + 2);
                key += pos[i];
            }
            return key;
        }

        private int[] PosNew(List<int[]> series, int letter, int[] pos)
        {
            var posNew = new int[series.Count];
            for (var iser = 0; iser < series.Count; iser++)
            {
                var nextPos = FindPos(series[iser], pos[iser], letter);
                if (nextPos == -1)
                    return null;
                posNew[iser] = nextPos;
            }
            return posNew;
        }

        private int FindPos(int[] ser, int pos, int letter)
        {
            for (var ich = pos; ich < ser.Length; ich++)
            {
                if (ser[ich] == letter)
                    return ich+1;
            }
            return -1;
        }
    }


    internal class Cache : Dictionary<BigInteger, int>
    {
    }
}
