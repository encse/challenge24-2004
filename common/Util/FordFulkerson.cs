using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmn.Util
{
    //https://kunuk.wordpress.com/2010/11/09/graph-flow-ford-fulkerson-algorithm-example-with-c/
    //amikor eleket adunk a graphoz, hozza kell venni egy visszafele mutato elet is
    //nulla kapacitassal, vagy ha a graph nem iranyitott, akkor a kapacitassal.
    public class FordFulkerson
    {
        public readonly Dictionary<int, Edge> edges = new Dictionary<int, Edge>();

        public int FindFlow(Node nodeSource, Node nodeTerminal)
        {
            var flow = 0;

            var path = Bfs(nodeSource, nodeTerminal);

            while (path != null && path.Count > 0)
            {
                var minCapacity = int.MaxValue;
                foreach (var edge in path)
                {
                    if (edge.Capacity < minCapacity)
                        minCapacity = edge.Capacity;
                }

                if (minCapacity == int.MaxValue || minCapacity < 0)
                    throw new Exception("minCapacity " + minCapacity);

                AugmentPath(path, minCapacity);
                flow += minCapacity;

                path = Bfs(nodeSource, nodeTerminal);
            }
            return flow;
        }

        void AugmentPath(IEnumerable<Edge> path, int minCapacity)
        {
            foreach (var edge in path)
            {
                var keyResidual = GetKey(edge.NodeTo, edge.NodeFrom);
                var edgeResidual = edges[keyResidual];

                edge.Capacity -= minCapacity;
                edgeResidual.Capacity += minCapacity;
            }
        }

        List<Edge> Bfs(Node root, Node target)
        {
            root.TraverseParent = null;
            target.TraverseParent = null; //reset

            var queue = new Queue<Node>();
            var discovered = new HashSet<Node>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                discovered.Add(current);

                if (current == target)
                    return GetPath(current);

                var nodeEdges = current.NodeEdges;
                foreach (var edge in nodeEdges)
                {
                    var next = edge.NodeTo;
                    var c = GetCapacity(current, next);
                    if (c > 0 && !discovered.Contains(next))
                    {
                        next.TraverseParent = current;
                        queue.Enqueue(next);
                    }
                }
            }
            return null;
        }

        List<Edge> GetPath(Node node)
        {
            var path = new List<Edge>();
            var current = node;
            while (current.TraverseParent != null)
            {
                var key = GetKey(current.TraverseParent, current);
                var edge = edges[key];
                path.Add(edge);
                current = current.TraverseParent;
            }
            return path;
        }

        public int GetKey(Node nodeA, Node nodeB)
        {
            return nodeA.ifox * 1000 + nodeB.ifox;
        }

        public int GetCapacity(Node node1, Node node2)
        {
            return edges[GetKey(node1, node2)].Capacity;
        }

        public void AddEdge(Node nodeFrom, Node nodeTo, int capacity)
        {
            var key = GetKey(nodeFrom, nodeTo);
            var edge = new Edge { NodeFrom = nodeFrom, NodeTo = nodeTo, Capacity = capacity };
            edges.Add(key, edge);
            nodeFrom.NodeEdges.Add(edge);
        }

        public class Node
        {
            public List<Edge> NodeEdges { get; set; }
            public Node TraverseParent { get; set; }

            public int age;
            public int ifox;

            public Node()
            {
                NodeEdges = new List<Edge>();
            }
        }

        public class Edge
        {
            public Node NodeFrom { get; set; }
            public Node NodeTo { get; set; }
            public int Capacity { get; set; }
            public string Name { get; set; }
        }
    }
}
