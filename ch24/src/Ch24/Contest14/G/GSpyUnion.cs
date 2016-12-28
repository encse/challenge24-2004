using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Google.OrTools.LinearSolver;
using Solver = Google.OrTools.LinearSolver.Solver;

namespace Ch24.Contest14.G
{
    public class GSpyUnionSolver : Ch24.Contest.Solver
    {
        public override void Solve()
        {
            var pp = new Pparser(FpatIn);

            var n = pp.Fetch<int>();
            var rgnode = new Node[n];

            var solver = Google.OrTools.LinearSolver.Solver.CreateSolver("IntegerProgramming",
                                                                        "CBC_MIXED_INTEGER_PROGRAMMING");
            
            for (int i = 0; i < n; i++)
            {
                var node = new Node(i);
                node.v = solver.MakeIntVar(0, 1, "x" + i);
                rgnode[i] = node;
            }

            int inodeWRoot = -1;
            int inodeURoot = -1;
            for(int i=0;i<n;i++)
            {
                int bw, bu, rw, ru;
                pp.Fetch(out bw, out bu, out rw, out ru);

                rgnode[i].rw = rw;
                rgnode[i].ru = ru;
                if (bw != i)
                    rgnode[bw].rgnodeOutW.Add(rgnode[i]);
                else
                {
                    if (inodeWRoot != -1)
                        throw new Exception("coki");
                    inodeWRoot = i;
                }

                if (bu != i)
                    rgnode[bu].rgnodeOutU.Add(rgnode[i]);
                else
                {
                    if (inodeURoot != -1)
                        throw new Exception("coki");
                    inodeURoot = i;
                }
            }

            AddConstraints(solver, rgnode[inodeWRoot], node => node.rgnodeOutW, node => node.rw);
            AddConstraints(solver, rgnode[inodeURoot], node => node.rgnodeOutU, node => node.ru);

            //v==1: ha megtartjuk 
            solver.Minimize(new SumVarArray(rgnode.Select(node => node.v).ToArray()));

            var resultStatus = solver.Solve();

            double min, max;
            if (resultStatus == Google.OrTools.LinearSolver.Solver.OPTIMAL)
                min = max = solver.Objective().Value();
            else
            {
                min = solver.Objective().Value();
                max = solver.Objective().BestBound();
            }

            using(Output)
            {
                Output.WriteLine(n - min);
                foreach (var node in rgnode)
                {
                    //Console.WriteLine(node.v.SolutionValue());
                    if (node.v.SolutionValue() == 0)
                        Output.Write(node.inode + " ");
                }
            }
            

        }

        private IEnumerable<Variable> AddConstraints(Solver solver, Node node, Func<Node, IEnumerable<Node>> dgrgnodeOut, Func<Node, int> dgMinInDept )
        {
            var rgv = new List<Variable> {node.v};
            foreach (var nodeD in dgrgnodeOut(node))
                rgv.AddRange(AddConstraints(solver, nodeD, dgrgnodeOut, dgMinInDept));

            solver.Add(new SumVarArray(rgv.ToArray()) >= dgMinInDept(node));
            return rgv;
        }

        class Node
        {
            public List<Node> rgnodeOutW = new List<Node>();
            public List<Node> rgnodeOutU = new List<Node>();
            public int rw;
            public int ru;
            
            public Variable v;
            public int inode;

            public Node(int inode)
            {
                this.inode = inode;
            }
        }


    }
}
