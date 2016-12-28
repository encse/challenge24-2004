using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest03.C
{
    public class CDNA4Solver : Solver
    {
        public override void Solve()
        {
            int cpart, cch;
            Pparser.Fetch(out cpart, out cch);
            var rgst = Pparser.FetchN<string>(cpart).ToArray();
            var mpnodeByst= new Dictionary<string, Node>();

            foreach (var st in rgst)
            {
                var stA = st.Substring(0, 5);
                var stB = st.Substring(st.Length - 5);
                if(!mpnodeByst.ContainsKey(stA))
                    mpnodeByst[stA] = new Node(stA);
                
                if (!mpnodeByst.ContainsKey(stB))
                    mpnodeByst[stB] = new Node(stB);

                var edge = new Edge(st, mpnodeByst[stA], mpnodeByst[stB]);
                mpnodeByst[stA].rgedgeOut.Add(edge);
                mpnodeByst[stB].rgedgeIn.Add(edge);
            }


            var rgedge = Euler(new HashSet<Node>(mpnodeByst.Values)).ToArray();

            Sanity(rgedge, rgst);
            
            string stResult = "";
            stResult = rgedge[0].st;
            for (int i = 1; i < rgedge.Length; i++)
            {
                stResult += rgedge[i].st.Substring(5);
            }
            using (Output)
            {
                Solwrt.Write(stResult);

            }
        }

        private void Sanity(Edge[] rgedge, string[] rgst)
        {
            List<string> rgstNotSeen = rgst.ToList();
            Edge edgePrev = null;
            foreach (var edge in rgedge)
            {
                if(edgePrev != null && !FMatch(edgePrev, edge))
                    throw new Exception("coki");

                if(!rgstNotSeen.Contains(edge.st))
                    throw new Exception("coki");

                rgstNotSeen.Remove(edge.st);
                edgePrev = edge;
            }

            if (rgstNotSeen.Any())
                throw new Exception("coki");
        }

        private bool FMatch(Edge edgePrev, Edge edge)
        {
            return edgePrev.st.EndsWith(edge.st.Substring(0,5));
        }

        IEnumerable<Edge> Euler(HashSet<Node> rgnode)
        {
            var rgedge = new List<Edge>();

            Node nodeStart;
            Node nodeEnd;
            
            var rgnodePtlFok = rgnode.Where(node => node.Fok() % 2 == 1).ToList();
            if (rgnodePtlFok.Count != 0 && rgnodePtlFok.Count != 2)
                throw new Exception("coki");

            var rgnodePs = rgnode.Where(node => node.rgedgeIn.Count != node.rgedgeOut.Count).ToList();

            if (rgnodePtlFok.Any())
            {

                nodeStart = rgnodePtlFok[0].rgedgeIn.Count() < rgnodePtlFok[0].rgedgeOut.Count() ? rgnodePtlFok[0] : rgnodePtlFok[1];
                nodeEnd = rgnodePtlFok[0].rgedgeIn.Count() < rgnodePtlFok[0].rgedgeOut.Count() ? rgnodePtlFok[1] : rgnodePtlFok[0];

            }
            else
            {
                nodeStart = nodeEnd = rgnode.First();
            } 
            
            var rgedgeAfter = new List<Edge>();
            while (rgnode.Any())
            {
                var nodeCur = nodeStart;
                var fFirst = true;
                while (fFirst || nodeCur != nodeEnd)
                {
                    fFirst = false;

                    var edge = nodeCur.rgedgeOut.First(edgeOut => edgeOut.nodeB.rgedgeOut.Any() || edgeOut.nodeB == nodeEnd);
                    rgedge.Add(edge);

                    edge.nodeA.rgedgeOut.Remove(edge);
                    edge.nodeB.rgedgeIn.Remove(edge);

                    if (edge.nodeA.Fok() == 0)
                        rgnode.Remove(edge.nodeA);
                    if (edge.nodeB.Fok() == 0)
                        rgnode.Remove(edge.nodeB);
                    nodeCur = edge.nodeB;
                }

                rgedge = rgedge.Concat(rgedgeAfter).ToList();

                if (!rgnode.Any())
                    break;

                nodeStart = nodeEnd = null;

                if (rgnode.Any(node => node.Fok() % 2 == 1))
                    throw new Exception("coki");

                for (int iedge = 0; iedge < rgedge.Count;iedge++ )
                {
                    //if (rgnode.Contains(rgedge[iedge].nodeA))
                    //{
                    //    nodeStart = nodeEnd = rgedge[iedge].nodeA;
                    //    rgedgeAfter = rgedge.Skip(iedge-1).ToList();
                    //    rgedge = rgedge.Take(iedge-1).ToList();
                    //    break;
                    //}

                    if (rgnode.Contains(rgedge[iedge].nodeB))
                    {
                        nodeStart = nodeEnd = rgedge[iedge].nodeB;
                        rgedgeAfter = rgedge.Skip(iedge+1).ToList();
                        rgedge = rgedge.Take(iedge+1).ToList();
                        break;
                    }
                    
                }
                if (nodeStart == null)
                    throw new Exception("coki");
            }

            return rgedge;
        }


        class Node
        {
            private string st;

            public List<Edge> rgedgeOut = new List<Edge>();
            public List<Edge> rgedgeIn = new List<Edge>();

            public int Fok()
            {
                return rgedgeOut.Count + rgedgeIn.Count;
            }

            public Node(string st)
            {
                this.st = st;
            }
        }


        class Edge
        {
            public string st;
            public Node nodeA;
            public Node nodeB;

            public Edge(string st, Node nodeA, Node nodeB)
            {
                this.st = st;
                this.nodeA = nodeA;
                this.nodeB = nodeB;
            }
        }
        
    }
}


