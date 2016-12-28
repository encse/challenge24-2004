using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using QuickGraph;

namespace Ch24.Contest12.P
{
    public class Independent2Solver : Contest.Solver
    {
        private class Gr : UndirectedGraph<int, Edge<int>>
        {
        }

        public override void Solve()
        {
            var gr = new Gr();
            // defhun:eg    edge
            int ceg;
            // defhun:vt    vertex
            int cvt;
            Pparser.Fetch(out cvt, out ceg);

            for (var iVertex = 0; iVertex < cvt; iVertex++)
                gr.AddVertex(iVertex);

            foreach (var egT in FetchN<Edge<int>>(ceg))
                gr.AddEdge(egT);

            var cvtInvited = 0;
            var cvtNotInvited = 0;
            while (gr.VertexCount > 0)
            {
                var vtLeaf = this.vtLeaf(gr);

                var egParent = gr.AdjacentEdges(vtLeaf).SingleOrDefault();
                if (egParent != null)
                {
                    var vtParent = egParent.GetOtherVertex(vtLeaf);
                    cvtNotInvited++;
                    gr.RemoveVertex(vtParent);
                }
                cvtInvited++;
                gr.RemoveVertex(vtLeaf);
            }

            Debug.Assert(cvtInvited + cvtNotInvited == cvt);

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
                solwrt.WriteLine(cvtInvited);
        }

        private int vtLeaf(Gr gr)
        {
            Debug.Assert(gr.VertexCount > 0);
            return gr.Vertices.First(vtT => gr.AdjacentEdges(vtT).Count() < 2);
        }
    }
}
