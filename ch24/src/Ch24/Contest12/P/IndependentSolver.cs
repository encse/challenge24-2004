using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ch24.Contest;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Ch24.Contest12.P
{
    public class IndependentSolver : Solver
    {

        private class Graph : UndirectedGraph<int, Edge<int>>
        {
            
        }

        public override void Solve()
        {
            var pp = Pparser;

            int cvertex, cedge;
            pp.Fetch(out cvertex, out cedge);

            var g = new Graph();
            var mpkinvByvertex = new Dictionary<int, Kinv>();

            for (var ivertex = 0; ivertex < cvertex; ivertex++)
            {
                g.AddVertex(ivertex);
                mpkinvByvertex.Add(ivertex, Kinv.Unknown);
            }

            for (var iedge = 0; iedge < cedge; iedge++)
                g.AddEdge(pp.Fetch<Edge<int>>());


            var mpicomponentByvertex = new Dictionary<int, int>();
            var cComponent = g.ConnectedComponents(mpicomponentByvertex);

            var cInvitedMax = 0;
            for (int icomponent = 0; icomponent < cComponent; icomponent++)
            {
                var vertex = g.Vertices.First(vertexT => mpicomponentByvertex[vertexT] == icomponent);
                cInvitedMax += Math.Max(CIfInvited(vertex, mpkinvByvertex, g), CIfNotInvited(vertex, mpkinvByvertex, g));
            }

            using(Output)
                WriteLine(cInvitedMax);
        }

        private enum Kinv
        {
            Unknown,
            Invite,
            NotInvite,
        }

        
        private int CIfNotInvited(int vertex, Dictionary<int, Kinv> mpkinvByvertex, Graph g)
        {
            Debug.Assert(mpkinvByvertex[vertex] == Kinv.Unknown);
            mpkinvByvertex[vertex] = Kinv.NotInvite;

            var c = 0;
            foreach (var vertexChild in g.AdjacentEdges(vertex).Select(edge => edge.GetOtherVertex(vertex)))
            {
                if (mpkinvByvertex[vertexChild] == Kinv.Unknown)
                    c += Math.Max(CIfInvited(vertexChild, mpkinvByvertex, g), CIfNotInvited(vertexChild, mpkinvByvertex, g));
            }
            mpkinvByvertex[vertex] = Kinv.Unknown;
            return c;
        }

        private int CIfInvited(int vertex, Dictionary<int, Kinv> mpkinvByvertex, Graph g)
        {
            Debug.Assert(mpkinvByvertex[vertex] == Kinv.Unknown);
            mpkinvByvertex[vertex] = Kinv.Invite;

            var c = 1;
            foreach (var vertexChild in g.AdjacentEdges(vertex).Select(edge => edge.GetOtherVertex(vertex)))
            {
                if (mpkinvByvertex[vertexChild] == Kinv.Unknown)
                    c += CIfNotInvited(vertexChild, mpkinvByvertex, g);
            }

            mpkinvByvertex[vertex] = Kinv.Unknown;
            return c;
        }

    }

  
}
