using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.R2.B
{
    public class WorldCupSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            /*
             The first line of the input gives the number of test cases, T. T test cases follow. 
             * Each case starts with a line containing a single integer P. 
             * The next line contains 2P integers -- the constraints M[0], ..., M[2P-1].

                The following block of P lines contains the ticket prices for all matches: the first line of the block contains 2P-1 integers --
             * ticket prices for first round matches, the second line of the block contains 2P-2 integers -- ticket prices for second round matches, etc.
             * The last of the P lines contains a single integer -- ticket price for the final match of the World Cup.
             * The prices are listed in the order the matches are played.
             */
            int p = pparser.Fetch<int>();
            var rgm = pparser.Fetch<int[]>();

            var rgteam = new Team[rgm.Length];
            for(int i=0;i<rgm.Length;i++)
                rgteam[i] = new Team {m = rgm[i]};


            Node[] rgnodePrev = rgteam;
            for(int i=0;i<p;i++)
            {
                var rgprice = pparser.Fetch<int[]>();
                var rgnode = new Node[rgprice.Length];
                for (int inode = 0; inode < rgnode.Length; inode++)
                {
                    rgnode[inode] = new Game
                                        {
                                            NodeLeft = rgnodePrev[inode*2],
                                            NodeRight = rgnodePrev[inode*2 + 1],
                                            Price = rgprice[inode]
                                        };
                }

                rgnodePrev = rgnode;
            }


            return () => Solve(rgnodePrev.Single());
        }

        private IEnumerable<object> Solve(Node node)
        {
            yield return node.Min(0);
        }


        abstract class Node
        {
            public abstract long Min(int cSeen);

        }

        class Game : Node
        {
            public Node NodeLeft;
            public Node NodeRight;
            public long Price;
            
            private readonly Dictionary<int, long> mpMinBycNotSeen = new Dictionary<int, long>();

            public override long Min(int cNotSeen)
            {
                if (!mpMinBycNotSeen.ContainsKey(cNotSeen))
                    mpMinBycNotSeen[cNotSeen] = 
                        Math.Min(
                            Sum(Price, NodeLeft.Min(cNotSeen), NodeRight.Min(cNotSeen)),
                            Sum(NodeLeft.Min(cNotSeen + 1), NodeRight.Min(cNotSeen + 1)));

                return mpMinBycNotSeen[cNotSeen];
            }
            long Sum(params long[] rgv)
            {
                if (rgv.Any(v => v == long.MaxValue))
                    return long.MaxValue;
                return rgv.Sum();
            }
        }

        class Team : Node
        {
            public int m;

            public override long Min(int cNotSeen)
            {
                return cNotSeen <= m ? 0L : long.MaxValue;
            }
        }

        
    }

    
}
