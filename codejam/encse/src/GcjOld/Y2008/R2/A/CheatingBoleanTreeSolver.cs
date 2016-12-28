using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2008.R2.A
{
    internal class CheatingBoleanTreeSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public enum Kgate
        {
            Or = 0,
            And = 1,
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             * The first line of the input file contains the number of cases, N. N test cases follow.
             *
             * Each case begins with M and V. M represents the number of nodes in the tree and will be odd to ensure all nodes have 0 or 2 children. 
             * V is the desired value for the root node, 0 or 1.
             *
             * M lines follow describing each of the tree's nodes. The Xth line will describe node X, starting with node 1 on the first line.
             *
             * The first (M−1)/2 lines describe the interior nodes. Each line contains G and C, each being either 0 or 1. If G is 1 then the gate 
             * for this node is an AND gate, otherwise it is an OR gate. If C is 1 then the gate for this node is changeable, otherwise it is not. 
             * Interior node X has nodes 2X and 2X+1 as children.
             *
             * The next (M+1)/2 lines describe the leaf nodes. Each line contains one value I, 0 or 1, the value of the leaf node.
             */
            int cnode, v;
            pparser.Fetch(out cnode, out v);

            var rgnode = new List<Nd>();
            rgnode.AddRange(pparser.FetchN<Node>((cnode - 1)/2));
            rgnode.AddRange(pparser.FetchN<Leaf>((cnode + 1)/2));

            for(int i=0;i<(cnode - 1)/2 ;i++)
            {
                ((Node) rgnode[i]).nodeLeft =  rgnode[2*(i+1) - 1];
                ((Node) rgnode[i]).nodeRight = rgnode[2*(i+1)+1  -1];
            }

            return () =>
                       {
                           Console.Write(".");
                           return Solve(rgnode, v);
                       };

        }

        private IEnumerable<object> Solve(List<Nd> rgnode, int v)
        {
            int? c = Solve(rgnode[0], v);
            if(c.HasValue)
                yield return c.Value;
            else
                yield return "IMPOSSIBLE";
        }
        int? OSum(int? a, int? b)
        {
            if (!a.HasValue || !b.HasValue)
                return null;
            return a.Value + b.Value;
        }

        private int? OMin(int? a, int? b)
        {
            if (!a.HasValue && !b.HasValue)
                return null;
            if (a.HasValue && !b.HasValue)
                return a.Value;
            if (!a.HasValue && b.HasValue)
                return b.Value;
            return Math.Min(a.Value, b.Value);
        }

        int? Solve(Nd nd, int v)
        {
            if (nd.V() == v)
                return 0;
            if(!(nd is Node))
                return null;

            var node = nd as Node;
            var ndLeft = node.nodeLeft;
            var ndRight = node.nodeRight;

            if (!node.fChange)
            {
                if (node.kgate == Kgate.Or)
                {
                    if(v == 0)
                        return OSum(Solve(ndLeft, 0), Solve(ndRight, 0));
                    return OMin(Solve(ndLeft, 1), Solve(ndRight, 1));
                }
                else 
                {
                    if (v == 0)
                        return OMin(Solve(ndLeft, 0), Solve(ndRight, 0));
                    return OSum(Solve(ndLeft, 1), Solve(ndRight, 1));
                }
            }
            else
            {
                if (node.kgate == Kgate.Or)
                {
                    if (v == 0)
                        return OMin(
                            OSum(Solve(ndLeft, 0), Solve(ndRight, 0)),
                            1 + OMin(Solve(ndLeft, 0), Solve(ndRight, 0)));
                    return OMin(
                        OMin(Solve(ndLeft, 1), Solve(ndRight, 1)),
                        1 + OSum(Solve(ndLeft, 1), Solve(ndRight, 1)));
                }
                else
                {
                    if (v == 0)
                        return OMin(
                            OMin(Solve(ndLeft, 0), Solve(ndRight, 0)),
                            1 + OSum(Solve(ndLeft, 0), Solve(ndRight, 0)));

                    return OMin(
                        OSum(Solve(ndLeft, 1), Solve(ndRight, 1)),
                        1 + OMin(Solve(ndLeft, 1), Solve(ndRight, 1)));
                }
            }
        }

      

        public abstract class Nd
        {
            public abstract int V();
        }

        public class Node : Nd
        {
            public Kgate kgate;
            public bool fChange;
            public Nd nodeLeft;
            public Nd nodeRight;

            public Node(int g, int c)
            {
                kgate = (Kgate) g;
                fChange = c == 1;
            }

            public override int V()
            {
                return kgate == Kgate.And ? nodeLeft.V() & nodeRight.V() : nodeLeft.V() | nodeRight.V();
            }

        }

        public class Leaf : Nd
        {
            private readonly int v;
            public override int V()
            {
                return v;
            }

            public Leaf(int v) 
            {
                this.v = v;
            }
        }
    }
}
