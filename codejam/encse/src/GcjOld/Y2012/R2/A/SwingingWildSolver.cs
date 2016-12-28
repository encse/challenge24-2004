using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2012.R2.A
{
    public class SwingingWildSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The first line of the input gives the number of test cases, T. T test cases follow. 
             * The first line of each test case contains the number N of vines.
             * N lines describing the vines follow, each with a pair of integers di and li - the distance of the vine from your ledge, 
             * and the length of the vine, respectively. The last line of the test case contains the distance D to the ledge with your one true love. 
             * You start by holding the first vine in hand.
             */
            var cvine = pparser.Fetch<int>();
            var rgvine = pparser.FetchN<int[]>(cvine);
            rgvine.Sort((vineA, vineB) => vineA[0] - vineB[0]);
            var dist = pparser.Fetch<int>();

            return () => Solve(rgvine, dist);
        }
        private IEnumerable<object> Solve(List<int[]> rgvine, int dist)
        {
            var rgminLenghtByIvine = new int[rgvine.Count];
            for (int i = 0; i < rgvine.Count; i++)
                rgminLenghtByIvine[i] = int.MaxValue;

            for (int ivine = rgvine.Count - 1; ivine >= 0; ivine--)
            {
                var vineFrom = rgvine[ivine];
                var dFrom = vineFrom[0];
                var lFrom = vineFrom[1];

                var lMin = dist - dFrom <= lFrom ? dist - dFrom : int.MaxValue;

                for (int jvine = ivine + 1; jvine < rgvine.Count; jvine++)
                {
                    var vineTo = rgvine[jvine];
                    var dTo = vineTo[0];

                    if (dFrom + lFrom >= dTo)
                    {
                        //elérjük
                        var lTo = Math.Min(vineTo[1], dTo - dFrom); //ennyivel tudunk róla továbblendülni

                        if (lTo >= rgminLenghtByIvine[jvine])
                        {
                            //elég hosszan rá tudunk kapaszkodni

                            if (dTo - dFrom < lMin)
                                lMin = dTo - dFrom;
                        }
                    }
                }
                rgminLenghtByIvine[ivine] = lMin;
            }

            var d0 = rgvine[0][0];
            var l0 = rgvine[0][1];
            var f = Math.Min(l0, d0) >= rgminLenghtByIvine[0];
            Console.Write(".");
            yield return f ? "YES" : "NO";
        }

        private IEnumerable<object> Solve2(List<int[]> rgvine, int dist)
        {

            var mpLengthAndfPossibleByIvine = new Dictionary<int, Tuple<int, bool>>();

            
            var vine = rgvine[0];
            var d = vine[0];
            var l = vine[1];
            var f = FPossible(0, Math.Min(l, d), rgvine, dist, mpLengthAndfPossibleByIvine);
            Console.Write(".");
            yield return f ?  "YES" : "NO";
        }

        private bool FPossible(int ivine, int length, List<int[]> rgvine, int dist, Dictionary<int, Tuple<int, bool>> mpLengthAndfPossibleByIvine)
        {
            if(mpLengthAndfPossibleByIvine.ContainsKey(ivine))
            {
                var lengthAndFPossible = mpLengthAndfPossibleByIvine[ivine];
                var lengthCached = lengthAndFPossible.Item1;
                var fCached = lengthAndFPossible.Item2;
                if (fCached && length >= lengthCached)
                    return true;
                if (!fCached && length <= lengthCached)
                    return false;
            }

            var vine = rgvine[ivine];
            var d = vine[0];

            if(d+length>=dist)
            {
                Update(mpLengthAndfPossibleByIvine, ivine, length, true);
                return true;
            }

            var rgivineToAndlTo = new List<Tuple<int, int>>();
            for (int ivineTo = ivine+1; ivineTo< rgvine.Count; ivineTo++)
            {
                var vineTo = rgvine[ivineTo];
                var dTo = vineTo[0];
                
                if(d+length >= dTo)
                {
                    //akkor elérjük
                    var lTo = Math.Min(vineTo[1], dTo - d);
                    rgivineToAndlTo.Add(new Tuple<int, int>(ivineTo, lTo));

                    //if (FPossible(ivineTo, lTo, rgvine, dist, mpLengthAndfPossibleByIvine))
                    //{
                    //    Update(mpLengthAndfPossibleByIvine, ivine, length, true);
                    //    return true;
                    //}
                }
            }

            rgivineToAndlTo.Sort((x, y) =>
                                     {
                                         var vineX = rgvine[x.Item1];
                                         var lX = x.Item2;
                                         var vineY = rgvine[y.Item1];
                                         var lY = y.Item2;

                                         var dX = vineX[0] + lX;
                                         var dY = vineY[0] + lY;
                                         return dY - dX;
                                     });

            if (rgivineToAndlTo.Any(x => FPossible(x.Item1, x.Item2, rgvine, dist, mpLengthAndfPossibleByIvine)))
            {
                Update(mpLengthAndfPossibleByIvine, ivine, length, true);
                return true;
            }
            Update(mpLengthAndfPossibleByIvine, ivine, length, false);
            return false;
        }

        private void Update(Dictionary<int, Tuple<int, bool>> mpLengthAndfPossibleByIvine, int ivine, int length, bool f)
        {
            if (!mpLengthAndfPossibleByIvine.ContainsKey(ivine))
                mpLengthAndfPossibleByIvine[ivine] = new Tuple<int, bool>(length, f);
            var lengthAndFPossible = mpLengthAndfPossibleByIvine[ivine];
            var lengthCached = lengthAndFPossible.Item1;
            var fCached = lengthAndFPossible.Item2;

            if(f && lengthCached > length)
                mpLengthAndfPossibleByIvine[ivine] = new Tuple<int, bool>(length, true);
            else if(!f && !fCached && length > lengthCached)
                mpLengthAndfPossibleByIvine[ivine] = new Tuple<int, bool>(length, false);
        }
    }
}
