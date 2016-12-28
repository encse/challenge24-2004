using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;
using Microsoft.SqlServer.Server;

namespace Gcj.Y2014.QR.D
{
    public class DeceitfulWarSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var n = pparser.Fetch<int>();
            var rgwA = pparser.Fetch<decimal[]>();
            var rgwB = pparser.Fetch<decimal[]>();

            return () => Solve(n,rgwA, rgwB);
        }

        private IEnumerable<object> Solve(int n, decimal[] rgwA, decimal[] rgwB)
        {
            yield return DeceitfulWar(rgwA, rgwB.ToList());
            yield return War(rgwA, rgwB.ToList());
        }

        private object War(decimal[] rgwA, List<decimal> rgwB)
        {
            rgwA = rgwA.OrderBy(w => w).Reverse().ToArray();
            rgwB = rgwB.ToList();

            var pointA = 0;
            for (int iA = 0; iA < rgwA.Length; iA++)
            {
                var iBBest = -1;
                for (int iB = 0; iB < rgwB.Count; iB++)
                {
                    if (rgwB[iB] > rgwA[iA] && (iBBest == -1 || rgwB[iB] < rgwB[iBBest]))
                        iBBest = iB;
                }

                if (iBBest == -1)
                {
                    for (int iB = 0; iB < rgwB.Count; iB++)
                    {
                        if (iBBest == -1 || rgwB[iB] < rgwB[iBBest])
                            iBBest = iB;
                    }
                }

                if (rgwB[iBBest] < rgwA[iA])
                    pointA++; 

                rgwB.RemoveAt(iBBest);
            }
            return pointA;
        }

        private object DeceitfulWar(decimal[] rgwA, List<decimal> rgwB)
        {
            rgwA = rgwA.OrderBy(w => w).ToArray();
            rgwB = rgwB.OrderBy(w => w).ToList();

            var pointA = 0;
            for (int iA = 0; iA < rgwA.Length; iA++)
            {
                var iBBest = -1;
                
                if (rgwA[iA] < rgwB[0])
                {
                    //az én legkisebb súlyommal kiütöm az ő legnagyobb súlyát 
                    //azt hazudom hogy az én súlyom csak egy picit könnyebb mint az ő legnagyobb súlya
                    iBBest = rgwB.Count - 1;
                }
                else
                {
                    //az én legkisebb súlyom most nagyobb mint az ő legkisebb súlya
                    //azt hazudom hogy az én súlyom még az ő legnehezebb súlyánál is nehezebb -> bedobja a legkisebbet
                    iBBest = 0;
                }
                
                if (rgwB[iBBest] < rgwA[iA])
                    pointA++;

                rgwB.RemoveAt(iBBest);
            }
            return pointA;
        }
    }
}
