using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1A.B
{
    public class BFullBinaryTreeSolver : IConcurrentSolver
    {
        public class Node
        {
            public int inode;
            public BigInteger key;
            public HashSet<Node> hlmNodeChildren = new HashSet<Node>();
        }
   
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n = pparser.Fetch<int>();
            var rgnode = new Node[n];
            for (int i = 0; i < n; i++)
                rgnode[i] = new Node {inode = i, key = BigInteger.Pow(2, i)};
            for (int i = 0; i < n - 1; i++)
            {
                int inodeFrom, inodeTo;
                pparser.Fetch(out inodeFrom, out inodeTo);
                inodeFrom--;
                inodeTo--;
                rgnode[inodeFrom].hlmNodeChildren.Add(rgnode[inodeTo]);
                rgnode[inodeTo].hlmNodeChildren.Add(rgnode[inodeFrom]);
            }

            return () => Solve(rgnode);
        }

        private IEnumerable<object> Solve(Node[] rgnode)
        {
            long lKeepMin = long.MaxValue;

            var cache = new Dictionary<BigInteger, Tuple<long, long>>();
            for (int inode = 0; inode < rgnode.Length; inode++)
            {
                var rgnodeSeen = new BigInteger(0);

                long lFull, lKeep;
                SolveFromRoot(rgnode, rgnode[inode], ref rgnodeSeen, out lFull, out lKeep, cache);
                if (lKeep < lKeepMin)
                    lKeepMin = lKeep;
            }
            Console.Write(".");
            yield return lKeepMin;
        }
        

        private void SolveFromRoot(Node[] rgnode, Node node, ref BigInteger rgnodeSeen, out long lFull, out long lKeep, Dictionary<BigInteger, Tuple<long, long>> cache)
        {
            rgnodeSeen += node.key;

            var sChildren = new List<Tuple<long, long>>();

            lFull = 1;
            foreach (var nodeChild in node.hlmNodeChildren)
            {
                if ((rgnodeSeen & nodeChild.key) != 0)
                    continue;

                long lFullT, lKeepT;
                SolveFromRoot(rgnode, nodeChild, ref rgnodeSeen, out lFullT, out lKeepT, cache);
                sChildren.Add(new Tuple<long, long>(lFullT, lKeepT));
                lFull += lFullT;
            }

            if (sChildren.Count == 0)
            {
                lKeep = 0;
            }
            else if (sChildren.Count == 2)
            {
                lKeep = sChildren.Sum(x => x.Item2);
            }
            else if (sChildren.Count == 1)
            {
                lKeep = sChildren.Single().Item1;
            }
            else
            {
                lKeep = long.MaxValue;
                var lFullChildren = lFull - 1;

                for (int i = 0; i < sChildren.Count; i++)
                {
                    for (int j = i + 1; j < sChildren.Count; j++)
                    {
                        var lfullA = sChildren[i].Item1;
                        var lkeepA = sChildren[i].Item2;

                        var lfullB = sChildren[j].Item1;
                        var lkeepB = sChildren[j].Item2;


                        //ezeket tartjuk meg
                        var lkeepAB = lkeepA + lkeepB + lFullChildren - lfullA - lfullB;

                        if (lkeepAB < lKeep)
                            lKeep = lkeepAB;
                    }
                }

            }
            rgnodeSeen -= node.key;
        }
    }
}
