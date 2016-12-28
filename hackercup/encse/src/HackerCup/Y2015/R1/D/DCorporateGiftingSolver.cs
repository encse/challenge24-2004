using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R1.D
{

    public class DCorporateGiftingSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        private static int i = 0;
        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var cnode = pparser.Fetch<int>();
            var rgnode = new Node[cnode];
            for (var inode = 0; inode < cnode; inode++)
                rgnode[inode] = new Node(inode);

            var rginodeSuccParent = pparser.Fetch<int[]>();
            for (var inode = 0; inode < cnode; inode++)
            {
                var inodeParent = rginodeSuccParent[inode] - 1;
                if (inodeParent == -1)
                    continue;

                rgnode[inode].nodeParent = rgnode[inodeParent];
                rgnode[inodeParent].children.Add(rgnode[inode]);
            }

            return () => Solve(rgnode);
        }

        private IEnumerable<object> Solve(Node[] rgnode)
        {
            yield return ChoiceAndCostRecursive(-1, rgnode[0]).Item2;
        }

        public static Tuple<int, int> ChoiceAndCostRecursive(int parentChoice, Node node)
        {
            if (node.mpParentChoiceToChoiceAndCost.ContainsKey(parentChoice))
                return node.mpParentChoiceToChoiceAndCost[parentChoice];

            var choice = 1;
            var bestChoice = -1;
            var bestCost = int.MaxValue;

            if (parentChoice > node.maxChoiceInSubtree+1 && node.mpParentChoiceToChoiceAndCost.Any())
            {
                var cheapest = MinMaxKer.RgtMin(node.mpParentChoiceToChoiceAndCost.Values, choiceAndCost => choiceAndCost.Item2);
                var choiceAndCostStart = MinMaxKer.RgtMax(cheapest, choiceAndCost => choiceAndCost.Item1).First();
                bestChoice = choiceAndCostStart.Item1;
                bestCost = choiceAndCostStart.Item2;

                return new Tuple<int, int>(bestChoice, bestCost);
            }

            var choiceLast = Math.Max(1, parentChoice + 1);
            for (; choice <= choiceLast; choice++)
            {
                if (choice == parentChoice)
                    continue;

                var cost = choice;
                foreach (var child in node.children)
                {
                    var choiceAndCost = ChoiceAndCostRecursive(choice, child);
                    choiceLast = Math.Max(choiceLast, choiceAndCost.Item1 + 1);
                    cost += choiceAndCost.Item2;

                    node.maxChoiceInSubtree = Math.Max(node.maxChoiceInSubtree, child.maxChoiceInSubtree);
                }

                if (cost <= bestCost)
                {
                    bestCost = cost;
                    bestChoice = choice;
                }
            }

            var res = new Tuple<int, int>(bestChoice, bestCost);
            node.maxChoiceInSubtree = Math.Max(node.maxChoiceInSubtree, bestChoice);
            node.mpParentChoiceToChoiceAndCost[parentChoice] = res;
            return res;
        }

        public class Node
        {
            public List<Node> children;
            public int inode;
            public Node nodeParent;

            public Dictionary<int, Tuple<int, int>> mpParentChoiceToChoiceAndCost = new Dictionary<int, Tuple<int, int>>();
            public int maxChoiceInSubtree = -1;

            public Node(int inode)
            {
                this.inode = inode;
                children = new List<Node>();
            }
        }
    }
}
