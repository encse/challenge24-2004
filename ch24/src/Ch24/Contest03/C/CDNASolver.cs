using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Google.OrTools.LinearSolver;
using Solver = Ch24.Contest.Solver;

namespace Ch24.Contest03.C
{
    public class CDNASolver : Solver
    {
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int n, l;
            pparser.Fetch(out n, out l);
            var rgnode = pparser.FetchN<Node>(n);
            rgnode.Add(new Node("X"));
            n++;
     
            var solver = Google.OrTools.LinearSolver.Solver.CreateSolver("IntegerProgramming",
                                                                         "CBC_MIXED_INTEGER_PROGRAMMING");
            for (int i = 0; i < n;i++ )
                rgnode[i].varU = solver.MakeIntVar(0,n, "U{0}".StFormat(i));
            var mpnodeByVarout = new Dictionary<Variable, Node>();
            var rgvarx = new List<Variable>();
            for(var i=0;i<n;i++)
            {
                for(var j=0;j<n;j++)
                {
                    if (i == j)
                        continue;

                    if (Node.FMatch(rgnode[i], rgnode[j]))
                    {
                        var varXij = solver.MakeIntVar(0, 1, "x{0}_{1}".StFormat(i,j));
                        rgvarx.Add(varXij);
                        rgnode[i].rgvarOut.Add(varXij);
                        rgnode[j].rgvarIn.Add(varXij);
                        mpnodeByVarout[varXij] = rgnode[j];

                        if (i > 0 && j > 0)
                            solver.Add(rgnode[i].varU - rgnode[j].varU + varXij * n <= n - 1);
                    }
                }
            }

            foreach(var node in rgnode)
            {
                solver.Add(new SumVarArray(node.rgvarOut.ToArray()) == 1);
                solver.Add(new SumVarArray(node.rgvarIn.ToArray()) == 1);
            }

            solver.Maximize(new SumVarArray(rgvarx.ToArray()));

            solver.SetTimeLimit(60 * 1000);

            var resultStatus = solver.Solve();
            double min, max;
            if (resultStatus == Google.OrTools.LinearSolver.Solver.OPTIMAL)
            {   
                min = max = solver.Objective().Value();

                var nodeCur = rgnode.Last();
                int s = 0;

                var rgst = new List<string>();
                while(nodeCur != null)
                {
                    Console.WriteLine("{0}\t{1}", ++s, nodeCur.st);
                    rgst.Add(nodeCur.st);
                    var varOut = nodeCur.rgvarOut.SingleOrDefault(var => var.SolutionValue() == 1);
                    if (varOut != null)
                        nodeCur = mpnodeByVarout[varOut];
                    else
                        nodeCur = null;
                    if (nodeCur == rgnode.Last())
                        break;
                }
                rgst = rgst.Skip(1).ToList();
                string stResult = "";
                stResult = rgst[0];
                for (int i = 1; i < rgst.Count; i++)
                {
                    stResult += rgst[i].Substring(5);
                }
                using (Output)
                {
                    Solwrt.Write(stResult);
                    
                }

            }
            else
            {
                min = solver.Objective().Value();
                max = solver.Objective().BestBound();
            }


            Console.WriteLine(min);
            Console.WriteLine(max);
        }

        class Node
        {
            public static bool FMatch(Node nodeA, Node nodeB)
            {
                if (nodeA.st == "X") return true;
                if (nodeB.st == "X") return true;

                return nodeA.st.Substring(nodeA.st.Length - 5) == nodeB.st.Substring(0, 5);
            }
            public string st;
            public List<Variable> rgvarIn = new List<Variable>();
            public List<Variable> rgvarOut = new List<Variable>();
            public Variable varU;
            public Node(string st)
            {
                this.st = st;
            }
        }
      
    }
}



//// simple exact TSP solver based on branch-and-bound/Held--Karp
//import java.io.*;
//import java.util.*;
//import java.util.regex.*;

//public class TSP {
//  // number of cities
//  private int n;
//  // city locations
//  private double[] x;
//  private double[] y;
//  // cost matrix
//  private double[][] cost;
//  // matrix of adjusted costs
//  private double[][] costWithPi;
//  Node bestNode = new Node();

//  public static void main(String[] args) throws IOException {
//    // read the input in TSPLIB format
//    // assume TYPE: TSP, EDGE_WEIGHT_TYPE: EUC_2D
//    // no error checking
//    TSP tsp = new TSP();
//    tsp.readInput(new InputStreamReader(System.in));
//    tsp.solve();
//  }

//  public void readInput(Reader r) throws IOException {
//    BufferedReader in = new BufferedReader(r);
//    Pattern specification = Pattern.compile("\\s*([A-Z_]+)\\s*(:\\s*([0-9]+))?\\s*");
//    Pattern data = Pattern.compile("\\s*([0-9]+)\\s+([-+.0-9Ee]+)\\s+([-+.0-9Ee]+)\\s*");
//    String line;
//    while ((line = in.readLine()) != null) {
//      Matcher m = specification.matcher(line);
//      if (!m.matches()) continue;
//      String keyword = m.group(1);
//      if (keyword.equals("DIMENSION")) {
//        n = Integer.parseInt(m.group(3));
//        cost = new double[n][n];
//      } else if (keyword.equals("NODE_COORD_SECTION")) {
//        x = new double[n];
//        y = new double[n];
//        for (int k = 0; k < n; k++) {
//          line = in.readLine();
//          m = data.matcher(line);
//          m.matches();
//          int i = Integer.parseInt(m.group(1)) - 1;
//          x[i] = Double.parseDouble(m.group(2));
//          y[i] = Double.parseDouble(m.group(3));
//        }
//        // TSPLIB distances are rounded to the nearest integer to avoid the sum of square roots problem
//        for (int i = 0; i < n; i++) {
//          for (int j = 0; j < n; j++) {
//            double dx = x[i] - x[j];
//            double dy = y[i] - y[j];
//            cost[i][j] = Math.rint(Math.sqrt(dx * dx + dy * dy));
//          }
//        }
//      }
//    }
//  }

//  public void solve() {
//    bestNode.lowerBound = Double.MAX_VALUE;
//    Node currentNode = new Node();
//    currentNode.excluded = new boolean[n][n];
//    costWithPi = new double[n][n];
//    computeHeldKarp(currentNode);
//    PriorityQueue<Node> pq = new PriorityQueue<Node>(11, new NodeComparator());
//    do {
//      do {
//        boolean isTour = true;
//        int i = -1;
//        for (int j = 0; j < n; j++) {
//          if (currentNode.degree[j] > 2 && (i < 0 || currentNode.degree[j] < currentNode.degree[i])) i = j;
//        }
//        if (i < 0) {
//          if (currentNode.lowerBound < bestNode.lowerBound) {
//            bestNode = currentNode;
//            System.err.printf("%.0f", bestNode.lowerBound);
//          }
//          break;
//        }
//        System.err.printf(".");
//        PriorityQueue<Node> children = new PriorityQueue<Node>(11, new NodeComparator());
//        children.add(exclude(currentNode, i, currentNode.parent[i]));
//        for (int j = 0; j < n; j++) {
//          if (currentNode.parent[j] == i) children.add(exclude(currentNode, i, j));
//        }
//        currentNode = children.poll();
//        pq.addAll(children);
//      } while (currentNode.lowerBound < bestNode.lowerBound);
//      System.err.printf("%n");
//      currentNode = pq.poll();
//    } while (currentNode != null && currentNode.lowerBound < bestNode.lowerBound);
//    // output suitable for gnuplot
//    // set style data vector
//    System.out.printf("# %.0f%n", bestNode.lowerBound);
//    int j = 0;
//    do {
//      int i = bestNode.parent[j];
//      System.out.printf("%f\t%f\t%f\t%f%n", x[j], y[j], x[i] - x[j], y[i] - y[j]);
//      j = i;
//    } while (j != 0);
//  }

//  private Node exclude(Node node, int i, int j) {
//    Node child = new Node();
//    child.excluded = node.excluded.clone();
//    child.excluded[i] = node.excluded[i].clone();
//    child.excluded[j] = node.excluded[j].clone();
//    child.excluded[i][j] = true;
//    child.excluded[j][i] = true;
//    computeHeldKarp(child);
//    return child;
//  }

//  private void computeHeldKarp(Node node) {
//    node.pi = new double[n];
//    node.lowerBound = Double.MIN_VALUE;
//    node.degree = new int[n];
//    node.parent = new int[n];
//    double lambda = 0.1;
//    while (lambda > 1e-06) {
//      double previousLowerBound = node.lowerBound;
//      computeOneTree(node);
//      if (!(node.lowerBound < bestNode.lowerBound)) return;
//      if (!(node.lowerBound < previousLowerBound)) lambda *= 0.9;
//      int denom = 0;
//      for (int i = 1; i < n; i++) {
//        int d = node.degree[i] - 2;
//        denom += d * d;
//      }
//      if (denom == 0) return;
//      double t = lambda * node.lowerBound / denom;
//      for (int i = 1; i < n; i++) node.pi[i] += t * (node.degree[i] - 2);
//    }
//  }

//  private void computeOneTree(Node node) {
//    // compute adjusted costs
//    node.lowerBound = 0.0;
//    Arrays.fill(node.degree, 0);
//    for (int i = 0; i < n; i++) {
//      for (int j = 0; j < n; j++) costWithPi[i][j] = node.excluded[i][j] ? Double.MAX_VALUE : cost[i][j] + node.pi[i] + node.pi[j];
//    }
//    int firstNeighbor;
//    int secondNeighbor;
//    // find the two cheapest edges from 0
//    if (costWithPi[0][2] < costWithPi[0][1]) {
//      firstNeighbor = 2;
//      secondNeighbor = 1;
//    } else {
//      firstNeighbor = 1;
//      secondNeighbor = 2;
//    }
//    for (int j = 3; j < n; j++) {
//      if (costWithPi[0][j] < costWithPi[0][secondNeighbor]) {
//        if (costWithPi[0][j] < costWithPi[0][firstNeighbor]) {
//          secondNeighbor = firstNeighbor;
//          firstNeighbor = j;
//        } else {
//          secondNeighbor = j;
//        }
//      }
//    }
//    addEdge(node, 0, firstNeighbor);
//    Arrays.fill(node.parent, firstNeighbor);
//    node.parent[firstNeighbor] = 0;
//    // compute the minimum spanning tree on nodes 1..n-1
//    double[] minCost = costWithPi[firstNeighbor].clone();
//    for (int k = 2; k < n; k++) {
//      int i;
//      for (i = 1; i < n; i++) {
//        if (node.degree[i] == 0) break;
//      }
//      for (int j = i + 1; j < n; j++) {
//        if (node.degree[j] == 0 && minCost[j] < minCost[i]) i = j;
//      }
//      addEdge(node, node.parent[i], i);
//      for (int j = 1; j < n; j++) {
//        if (node.degree[j] == 0 && costWithPi[i][j] < minCost[j]) {
//          minCost[j] = costWithPi[i][j];
//          node.parent[j] = i;
//        }
//      }
//    }
//    addEdge(node, 0, secondNeighbor);
//    node.parent[0] = secondNeighbor;
//    node.lowerBound = Math.rint(node.lowerBound);
//  }

//  private void addEdge(Node node, int i, int j) {
//    double q = node.lowerBound;
//    node.lowerBound += costWithPi[i][j];
//    node.degree[i]++;
//    node.degree[j]++;
//  }
//}

//class Node {
//  public boolean[][] excluded;
//  // Held--Karp solution
//  public double[] pi;
//  public double lowerBound;
//  public int[] degree;
//  public int[] parent;
//}

//class NodeComparator implements Comparator<Node> {
//  public int compare(Node a, Node b) {
//    return Double.compare(a.lowerBound, b.lowerBound);
//  }
//}