using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Cmn.Util;
using QuickGraph;

namespace Ch24.Contest13.A
{
    public class CeoSolver : Contest.Solver
    {
        public static IEnumerable<int> rgichSub(string st, string stSub)
        {
            int ich = 0;
            while(true)
            {
                ich = st.IndexOf(stSub, ich, StringComparison.InvariantCulture);
                if (ich == -1)
                    yield break;

                yield return ich;
                ich += stSub.Length;
            }
        }

        public static Dictionary<string, int> mpcoccByStSubGet(string st)
        {
            var mpcoccBySub = new Dictionary<string, int>();
            var hlmSub = new HashSet<string>(st.Select(ch=> new string(ch,1)));

            while(hlmSub.Count > 0)
            {
                var hlmSubNext = new HashSet<string>();
                foreach (var stSub in hlmSub)
                {
                    int cocc = rgichSub(st, stSub).Count();
                    if (cocc > 1)
                    {
                        mpcoccBySub.Add(stSub, cocc);
                        foreach (var ich in rgichSub(st, stSub))
                        {
                            if (ich + stSub.Length + 1 < st.Length)
                                hlmSubNext.Add(st.Substring(ich, stSub.Length + 1));
                        }
                    }
                }
                hlmSub = hlmSubNext;
            }

            return mpcoccBySub;
        }

        class Vt
        {
            public int cparaszt;
            public int ivt;
        }

        public class Vtp
        {
            public int cparaszt;
            public int cvt;
            public List<int> rgivtChild;

            public Vtp(int cparaszt, int cvt, List<int> rgivtChild)
            {
                this.cparaszt = cparaszt;
                this.cvt = cvt;
                this.rgivtChild = rgivtChild;
            }
        }

        class VtCompByCparaszt : IComparer<Vt>
        {
            public int Compare(Vt vt1, Vt vt2)
            {
                var d = vt1.cparaszt - vt2.cparaszt;
                if(d!=0)
                    return d;
                return vt1.ivt - vt2.ivt;
            }
        }


        class Gr : BidirectionalGraph<Vt, Edge<Vt>>
        {
        }

        public override void Solve()
        {
            var mpoccByStSub = mpcoccByStSubGet(File.ReadAllText(FpatIn, Encoding.ASCII));
            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                foreach (var kvpOccByStSub in mpoccByStSub)
                    solwrt.WriteLine("{0} {1}", kvpOccByStSub.Key, kvpOccByStSub.Value);
            }

            return;


            var gr = new Gr();

            // defhun:vt    vertex
            int cvt;
            int cmaxSo;
            Pparser.Fetch(out cvt, out cmaxSo);

            var rgvt = new List<Vt>();
            {
                for (int ivt = 0; ivt < cvt; ivt++)
                {
                    var vt = new Vt { ivt = ivt };
                    rgvt.Add(vt);
                    gr.AddVertex(vt);
                }
            }

            {
                var ivt = 0;
                foreach (var vtpT in Pparser.FetchN<List<int>>(cvt))
                {
                    rgvt[ivt].cparaszt = vtpT[0];
                    Debug.Assert(vtpT[1] == vtpT.Count - 2);
                    foreach (var ivtChild in vtpT.Skip(2))
                    {
                        gr.AddEdge(new Edge<Vt>(rgvt[ivt], rgvt[ivtChild]));
                    }
                    ivt++;
                }
            }

            var shlmLeaf = new SortedSet<Vt>(gr.Vertices.Where(vt => gr.OutDegree(vt) == 0), new VtCompByCparaszt());

            int cFire = 0;
            while (true)
            {
                var vtMin = shlmLeaf.Min;

                Debug.Assert(gr.InDegree(vtMin) == 1);
                Debug.Assert(gr.OutDegree(vtMin) == 0);

                var vtParent = vtParentGet(vtMin, gr);

                if (FCanFire(cmaxSo, vtMin, gr))
                {
                    vtParent.cparaszt += vtMin.cparaszt;
                    cFire++;
                }
                else
                {
                    vtParent.cparaszt += 1;
                }

                shlmLeaf.Remove(vtMin);
                gr.RemoveVertex(vtMin);

                if (gr.OutDegree(vtParent) == 0)
                {
                    if (gr.InDegree(vtParent) == 0)
                        break;

                    shlmLeaf.Add(vtParent);
                }

                int cleaf = gr.Vertices.Count(vt => gr.OutDegree(vt) == 0);
                Debug.Assert(shlmLeaf.Count == cleaf);
                Debug.Assert(shlmLeaf.Count > 0);



//                int csoMin = int.MaxValue;
//                Vt vtMin = null;
//                foreach(var vtT in gr.Vertices.Where(vtT => FCanFire(cmaxSo, vtT, gr)))
//                {
//                    int csoT = csoFromVt(vtT, gr);
//                    if (csoT < csoMin)
//                    {
//                        csoMin = csoT;
//                        vtMin = vtT;
//                    }
//                }
//
//                if (vtMin == null)
//                    break;
//
//                cFire++;
//                var vtParent = vtParentGet(vtMin, gr);
//                vtParent.cparaszt += vtMin.cparaszt;
//                foreach (var egChild in gr.OutEdges(vtMin))
//                    gr.AddEdge(new Edge<Vt>(vtParent, egChild.GetOtherVertex(vtMin)));
//                gr.RemoveVertex(vtMin);
            }

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
                solwrt.WriteLine(cFire);
        }

        private int csoFromVt(Vt vt, Gr gr)
        {
            return gr.OutDegree(vt) + vt.cparaszt;
        }

        private Vt vtParentGet(Vt vt, Gr gr)
        {
            Debug.Assert(gr.InDegree(vt) == 1);

            var egParent = gr.InEdges(vt).Single();
            return egParent.GetOtherVertex(vt);
        }

        private bool FCanFire(int cmaxSo, Vt vt, Gr gr)
        {
            if (gr.InDegree(vt) == 0) 
                return false;

            var vtParent = vtParentGet(vt, gr);
            return csoFromVt(vt, gr) - 1 + csoFromVt(vtParent, gr) <= cmaxSo;
        }
    }
}
