using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Solver = Ch24.Contest.Solver;

namespace Ch24.Contest03.C
{
    public class CDNA2Solver : Solver
    {
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int n, l;
            pparser.Fetch(out n, out l);
            var rgnode = pparser.FetchN<Node2>(n);
            rgnode.Add(new Node2("X"));
            n++;
            double inf = n*n*n;
            var tsp = new TSP();
            tsp.n = n;
            tsp.rgst = rgnode.Select(node => node.st).ToArray();
            tsp.cost = new double[n,n];
            for(int i=0;i<n;i++)
            {
                for(int j=0;j<n;j++)
                {
                    if (i == j || !Node2.FMatch(rgnode[i], rgnode[j]))
                        tsp.cost[i, j] = inf;
                    else
                        tsp.cost[i, j] = 1;
                }
            }
            tsp.solve();


        }

        private class Node2
        {
            public static bool FMatch(Node2 nodeA, Node2 nodeB)
            {
                if (nodeA.st == "X") return true;
                if (nodeB.st == "X") return true;

                return nodeA.st.Substring(nodeA.st.Length - 5) == nodeB.st.Substring(0, 5);
            }

            public string st;
     

            public Node2(string st)
            {
                this.st = st;
            }
        }


        public class TSP
        {
            // number of cities
            public int n;
            // city locations
            public string[] rgst;
            // cost matrix
            public double[,] cost;
            // matrix of adjusted costs
            private double[,] costWithPi;
            private Node bestNode = new Node();


            class PriorityQueue<T> 
            {
                SortedSet<Tuple<T, T>> set;

                class Comparer : IComparer<Tuple<T, T>> {
                    private Func<T,T,int> dgcompare;

                    public Comparer(Func<T,T,int> dgcompare)
                    {
                        this.dgcompare = dgcompare;
                    }

                    public int Compare(Tuple<T, T> x, Tuple<T, T> y)
                    {
                        return dgcompare(x.Item2, y.Item2);
                    }
                }

                public PriorityQueue( Func<T,T,int> dgcompare) { set = new SortedSet<Tuple<T, T>>(new Comparer(dgcompare)); }
                public bool Empty { get { return set.Count == 0;  } }
                public void add(T x) { set.Add(Tuple.Create(x, x)); }
                public void Dequeue() { set.Remove(set.Max); }
                public T Top { get { return set.Max.Item1; } }

                public T poll()
                {
                    T t = Top;
                    Dequeue();
                    return t;
                }

                public void addAll(PriorityQueue<T> children)
                {
                    foreach(var t in children.set)
                        this.set.Add(Tuple.Create(t.Item1, t.Item2));
                }
            }


            public void solve()
            {
                bestNode.lowerBound = Double.MaxValue;
                var currentNode = new Node();
                currentNode.excluded = new bool[n, n];
                costWithPi = new double[n, n];
                computeHeldKarp(currentNode);
                var pq = new PriorityQueue<Node>((n1, n2) => n1.lowerBound.CompareTo(n2.lowerBound));
                do
                {
                    do
                    {
                        bool isTour = true;
                        int i = -1;
                        for (int j = 0; j < n; j++)
                        {
                            if (currentNode.degree[j] > 2 && (i < 0 || currentNode.degree[j] < currentNode.degree[i])) i = j;
                        }
                        if (i < 0)
                        {
                            if (currentNode.lowerBound < bestNode.lowerBound)
                            {
                                bestNode = currentNode;
                                Console.Error.WriteLine("{0}", bestNode.lowerBound);
                            }
                            break;
                        }
                        Console.Error.Write(".");
                        var children = new PriorityQueue<Node>((n1, n2) => n1.lowerBound.CompareTo(n2.lowerBound));
                        children.add(exclude(currentNode, i, currentNode.parent[i]));
                        for (int j = 0; j < n; j++)
                        {
                            if (currentNode.parent[j] == i) children.add(exclude(currentNode, i, j));
                        }
                        currentNode = children.poll();
                        pq.addAll(children);
                    } while (currentNode.lowerBound < bestNode.lowerBound);
                    Console.WriteLine();
                    currentNode = pq.poll();
                } while (currentNode != null && currentNode.lowerBound < bestNode.lowerBound);
                // output suitable for gnuplot
                // set style data vector
                Console.WriteLine("# {0}", bestNode.lowerBound);
                {
                    int j = 0;
                    do
                    {
                        int i = bestNode.parent[j];
                        Console.WriteLine(rgst[i]);
                        j = i;
                    } while (j != 0);
                }
            }

            private Node exclude(Node node, int i, int j)
            {
                var child = new Node();
               
                child.excluded = (bool[,])node.excluded.Clone();
                //child.excluded[i] = node.excluded[i].clone();
                //child.excluded[j] = node.excluded[j].clone();
                child.excluded[i, j] = true;
                child.excluded[j, i] = true;
                computeHeldKarp(child);
                return child;
            }

            private void computeHeldKarp(Node node)
            {
                node.pi = new double[n];
                node.lowerBound = Double.MinValue;
                node.degree = new int[n];
                node.parent = new int[n];
                double lambda = 0.1;
                while (lambda > 1e-06)
                {
                    double previousLowerBound = node.lowerBound;
                    computeOneTree(node);
                    if (!(node.lowerBound < bestNode.lowerBound)) return;
                    if (!(node.lowerBound < previousLowerBound)) lambda *= 0.9;
                    int denom = 0;
                    for (int i = 1; i < n; i++)
                    {
                        int d = node.degree[i] - 2;
                        denom += d*d;
                    }
                    if (denom == 0) return;
                    double t = lambda*node.lowerBound/denom;
                    for (int i = 1; i < n; i++) node.pi[i] += t*(node.degree[i] - 2);
                }
            }

            private void computeOneTree(Node node)
            {
                // compute adjusted costs
                node.lowerBound = 0.0;
                fill(node.degree, 0);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++) costWithPi[i, j] = node.excluded[i, j] ? Double.MaxValue : cost[i, j] + node.pi[i] + node.pi[j];
                }
                int firstNeighbor;
                int secondNeighbor;
                // find the two cheapest edges from 0
                if (costWithPi[0, 2] < costWithPi[0, 1])
                {
                    firstNeighbor = 2;
                    secondNeighbor = 1;
                }
                else
                {
                    firstNeighbor = 1;
                    secondNeighbor = 2;
                }
                for (int j = 3; j < n; j++)
                {
                    if (costWithPi[0, j] < costWithPi[0, secondNeighbor])
                    {
                        if (costWithPi[0, j] < costWithPi[0, firstNeighbor])
                        {
                            secondNeighbor = firstNeighbor;
                            firstNeighbor = j;
                        }
                        else
                        {
                            secondNeighbor = j;
                        }
                    }
                }
                addEdge(node, 0, firstNeighbor);
                fill(node.parent, firstNeighbor);
                node.parent[firstNeighbor] = 0;
                // compute the minimum spanning tree on nodes 1..n-1
                double[] minCost = new double[costWithPi.GetLength(1)];
                for(int i=0;i<costWithPi.GetLength(1); i++)
                    minCost[i] =  costWithPi[firstNeighbor, i];
                for (int k = 2; k < n; k++)
                {
                    int i;
                    for (i = 1; i < n; i++)
                    {
                        if (node.degree[i] == 0) break;
                    }
                    for (int j = i + 1; j < n; j++)
                    {
                        if (node.degree[j] == 0 && minCost[j] < minCost[i]) i = j;
                    }
                    addEdge(node, node.parent[i], i);
                    for (int j = 1; j < n; j++)
                    {
                        if (node.degree[j] == 0 && costWithPi[i, j] < minCost[j])
                        {
                            minCost[j] = costWithPi[i, j];
                            node.parent[j] = i;
                        }
                    }
                }
                addEdge(node, 0, secondNeighbor);
                node.parent[0] = secondNeighbor;
                node.lowerBound = Math.Round(node.lowerBound);
            }

            private void fill<T>(T[] rgt, T t)
            {
                for (int i = 0; i < rgt.Length; i++)
                    rgt[i] = t;
            }

            private void addEdge(Node node, int i, int j)
            {
                double q = node.lowerBound;
                node.lowerBound += costWithPi[i, j];
                node.degree[i]++;
                node.degree[j]++;
            }
        }

        private class Node
        {
            public bool[,] excluded;
            // Held--Karp solution
            public double[] pi;
            public double lowerBound;
            public int[] degree;
            public int[] parent;

         
        }


    }
}


