using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using QuickGraph;

namespace Ch24.Contest13.R
{
    public class HistorySolver : Contest.Solver
    {
        class Vt
        {
            public double W;
            public double V;
            public int ivt;
        }

        public class Egt
        {
            public int ivtA;
            public int ivtB;
            public double P;

            public Egt(int ivtA, int ivtB, double p)
            {
                this.ivtA = ivtA;
                this.ivtB = ivtB;
                P = p;
            }
        }

        class Gr : BidirectionalGraph<Vt, TaggedEdge<Vt, Egt>>
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

            var rgvt = new List<Vt>();

            int ivt = 0;
            foreach (var vT in Pparser.Fetch<List<double>>())
            {
                var vt = new Vt {V = vT, ivt = ivt++, W = 0};
                rgvt.Add(vt);
                gr.AddVertex(vt);
            }

            foreach (var egtT in Pparser.FetchN<Egt>(ceg))
                gr.AddEdge(new TaggedEdge<Vt, Egt>(rgvt[egtT.ivtA-1], rgvt[egtT.ivtB-1], egtT));

            while(!gr.IsVerticesEmpty)
            {
                var vtLeaf = gr.Vertices.First(vt => gr.OutDegree(vt) == 0);
                vtLeaf.W += vtLeaf.V;
                foreach(var egIn in gr.InEdges(vtLeaf))
                {
                    var vtT = egIn.GetOtherVertex(vtLeaf);
                    vtT.W += egIn.Tag.P*vtLeaf.W;
                }
                gr.RemoveVertex(vtLeaf);
            }

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
                solwrt.WriteLine(rgvt.Select(vt=>vt.W));
        }
    }
}
