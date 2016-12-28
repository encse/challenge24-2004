using System;
using System.Collections.Generic;
using Cmn.Util;
using Google.OrTools.ConstraintSolver;
using Solver = Ch24.Contest.Solver;

namespace Ch24.Contest03.C
{
    public class CDNA3Solver : Solver
    {
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int n, l;
            pparser.Fetch(out n, out l);
            var rgnode = pparser.FetchN<string>(n);
            rgnode.Insert(0, "X");
            var model = new RoutingModel(n, 1);
            model.SetFirstSolutionStrategy(RoutingModel.ROUTING_GLOBAL_CHEAPEST_ARC );
            //model.SetMetaheuristic(RoutingModel.ROUTING_TABU_SEARCH);
            model.SetCost( new NodeEval(rgnode.ToArray()));
            model.SetDepot(0);
            for (int i = 0; i < n;i++ )
            {
                var varI = model.NextVar(i);
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;
                    if (!NodeEval.FMatch(rgnode[i], rgnode[j]))
                    varI.RemoveValue(j);
                }
            }
            Console.WriteLine("solving");

            Assignment solution = model.Solve();
            model.UpdateTimeLimit(1000*60*3);
            if (solution != null)
            {
                // Solution cost.
                Console.WriteLine("Cost = {0}", solution.ObjectiveValue());
                for (var inode = (int)model.Start(0); !model.IsEnd(inode); inode = (int)solution.Value(model.NextVar(inode)))
                {
                    Console.WriteLine(rgnode[inode]);
                }
                Console.WriteLine("0");
            }
        }

        public class NodeEval : NodeEvaluator2
        {
            private string[] rgnode;
            private Dictionary<int, int> mpcnodeOutByinode = new Dictionary<int, int>();
            private Dictionary<int, int> mpcnodeInByinode = new Dictionary<int, int>();
            public NodeEval(string[] rgnode)
            {
                this.rgnode = rgnode;
                for (int i = 0; i < rgnode.Length; i++)
                {
                    mpcnodeInByinode[i] = 0;
                    mpcnodeOutByinode[i] = 0;
                }

                for (int i = 0; i < rgnode.Length; i++)
                for (int j = 0; j < rgnode.Length; j++)
                {
                    if (FMatch(rgnode[i], rgnode[j]))
                    {
                        mpcnodeInByinode[j]++;
                        mpcnodeOutByinode[i]++;
                    }
                }
            }

            public static bool FMatch(string nodeA, string nodeB)
            {
                if(nodeA == "X" || nodeB == "X")
                    return true;
                return nodeA.Substring(nodeA.Length - 5) == nodeB.Substring(0, 5);
            }

            public override long Run(int i, int j)
            {
                if (FMatch(rgnode[i], rgnode[j]))
                   // return mpcnodeInByinode[j];
                    //return mpcnodeOutByinode[i];
                    return mpcnodeOutByinode[i]- mpcnodeInByinode[j];
                return long.MaxValue - 10;
            }
        }
    }
}


