using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Google.OrTools.LinearSolver;

namespace Ch24.Contest14.E
{
    public class ETravellingSolver : Ch24.Contest.Solver
    {
        const double Rkm = 6371;

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
                double lat, lon;
                pp.Fetch(out lat, out lon);
                node.lat = lat;
                node.lon = lon;
                rgnode[i] = node;
                
                    node.vU = solver.MakeIntVar(0, n-1, "u" + i);
            }

            //n = 3;
            //rgnode = new Node[] { new Node(0) { lat = 0, lon = 0 }, new Node(1) { lat = 10, lon = 0 }, new Node(2) { lat = 20, lon = 0 } };

            var mpdist = new Dictionary<Variable, double>();
            var rgvXijNyugat = new List<Variable>();
            var rgvXij = new List<Variable>();

            var foo = new Variable[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var nodeI = rgnode[i];
                    var nodeJ = rgnode[j];
                    if(i == j)
                        continue;

                    var vXij = solver.MakeIntVar(0, 1, "x" + i + "_" + j);
                    rgvXij.Add(vXij);

                    nodeI.rgvarOut.Add(vXij);
                    nodeJ.rgvarIn.Add(vXij);

                    if (NyugatraMegy(nodeI, nodeJ))
                    {
                        rgvXijNyugat.Add(vXij);
                    }

                    mpdist[vXij] = dkm(nodeI, nodeJ);

                    if(i>0 && j>0)
                        solver.Add(nodeI.vU - nodeJ.vU + vXij * (n-1) <= n - 2);

                    foo[i, j] = vXij;
                }
            }

            foreach (var node in rgnode)
            {
                solver.Add(new SumVarArray(node.rgvarOut.ToArray()) == 1);
                solver.Add(new SumVarArray(node.rgvarIn.ToArray()) == 1);
            }

            //kétszer mehetünk nyugatra csak
            solver.Add(new SumVarArray(rgvXijNyugat.ToArray()) <= 2);

            solver.Minimize(new SumArray(rgvXij.Select(vXij => vXij*mpdist[vXij]).ToArray()));

//            solver.SetTimeLimit(60 * 1000);

            var resultStatus = solver.Solve();
            double min, max;
            if (resultStatus == Google.OrTools.LinearSolver.Solver.OPTIMAL)
            {
                min = max = solver.Objective().Value();
            }
            else
            {
                min = solver.Objective().Value();
                max = solver.Objective().BestBound();
            }

            var rgvSolution = new List<Variable>();


            foreach (var variable in rgvXij)
            {
                if (variable.SolutionValue() != 0)
                    rgvSolution.Add(variable);
            }

            var last = rgvSolution.First();
            var rg = last.Name().Substring(1).Split('_');
            using (Output)
            {
                NufDouble = "0.######";
                WriteLine(min);

                Write(rg[0] +" ");
                Write(rg[1] +" ");
                rgvSolution.Remove(last);

                while (rgvSolution.Any())
                {
                    last = rgvSolution.Single(v => v.Name().Substring(1).Split('_')[0] == rg[1]);
                    rg = last.Name().Substring(1).Split('_');
                    Write(rg[1] + " ");
                    rgvSolution.Remove(last);
                }
            }

        }

        private bool NyugatraMegy(Node nodeA, Node nodeB)
        {
            return nodeB.lon < nodeA.lon;
        }


        private double dkm(Node pont1, Node pont2)
        {
            //if(pont1.inode != 4 || pont2.inode != 1) // !NyugatraMegy(pont1,pont2))
            var fCoki = (pont2.lon - pont1.lon + 360)%360 > 180;
            if(!fCoki)
                return dkmI(pont1, pont2);


            var nodeNorth = new Node(-1) {lat = 90, lon = 0};
            var nodeSouth = new Node(-1) {lat = -90, lon = 0};
            var d1 = dkmI(pont1, nodeNorth) + dkmI(nodeNorth, pont2);
            var d2 = dkmI(pont1, nodeSouth) + dkmI(nodeSouth, pont2);

            return Math.Min(d1, d2);

        }
        private double dkmI(Node pont1, Node pont2)
        {
           
            var dlambda = Math.Abs(pont1.lon * Math.PI / 180 - pont2.lon * Math.PI / 180);
            var phi2 = pont2.lat * Math.PI / 180;
            var phi1 = pont1.lat * Math.PI / 180;
            var ca = Math.Abs(Math.Atan2(
                Math.Sqrt(
                    sq(Math.Cos(phi2) * Math.Sin(dlambda)) +
                    sq(
                        Math.Cos(phi1) * Math.Sin(phi2) -
                        Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(dlambda))
                ), (
                    Math.Sin(phi1) * Math.Sin(phi2) +
                    Math.Cos(phi1) * Math.Cos(phi2) * Math.Cos(dlambda)
                )
            ));
            //var ca2 = Math.Acos(Math.Sin(phi1) * Math.Sin(phi2) + Math.Cos(phi1) * Math.Cos(phi2) * Math.Cos(dlambda));
            return Rkm * ca;
        }

        public double sq(double a)
        {
            return a * a;
        }



        class Node
        {
            public double lat, lon;
            public Variable vU;
            public int inode;
            public List<Variable> rgvarOut = new List<Variable>();
            public List<Variable> rgvarIn = new List<Variable>();

            public Node(int inode)
            {
                this.inode = inode;
            }
        }


    }
}
