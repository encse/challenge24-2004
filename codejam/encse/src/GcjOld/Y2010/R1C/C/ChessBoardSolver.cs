using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gcj.Util;
using Gcj.Y2010.R1A.C;

namespace Gcj.Y2010.R1C.C
{
    public class ChessBoardSolver : GcjSolver
    {

        protected override void Solve()
        {
            var cCase = CCaseGet();
            using (Output)
            {
                for (var iCase = 1; iCase <= cCase; iCase++)
                {
                    SolveCase(iCase);
                }
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            throw new NotImplementedException();
        }

        class Cellid : Tuple<int, int>
        {
            public Cellid(int irow, int icol)
                : base(irow, icol)
            {
            }
        }

        class Cutmap
        {
            private Dictionary<Cellid, int> mpsizeBycellid = new Dictionary<Cellid, int>();

            private readonly SortedDictionary<int, SortedDictionary<int, Cellid>> mprgcellidBynsize = new SortedDictionary<int, SortedDictionary<int, Cellid>>();
            public void Add(Tbl tbl, int irow, int icol, int size)
            {
                if(size ==0)
                    return;
                

                var nsize = -size;
                if (!mprgcellidBynsize.ContainsKey(nsize))
                    mprgcellidBynsize[nsize] = new SortedDictionary<int, Cellid>();
                var cellid = new Cellid(irow, icol);
                mprgcellidBynsize[nsize].Add(tbl.ccol*irow + icol, cellid);
                mpsizeBycellid[cellid] = size;
            }
            public void Max(out int irow, out int icol, out int size)
            {
                if(mprgcellidBynsize.Count == 0)
                {
                    irow = icol = -1;
                    size = 0;
                    return;
                    
                }
                var kvpnsizeAndrgcellid = mprgcellidBynsize.First();
                var nsize = kvpnsizeAndrgcellid.Key;
                var rgcellid = kvpnsizeAndrgcellid.Value;
                size = -nsize;
                var cellid = rgcellid.First().Value;
                irow = cellid.Item1;
                icol = cellid.Item2;
            }

            public void Remove(Tbl tbl, int irow, int icol)
            {
                var cellid = new Cellid(irow, icol);
                var nsize = -mpsizeBycellid[cellid];

                var rgcellid = mprgcellidBynsize[nsize];
                rgcellid.Remove(tbl.ccol*irow + icol);
                
                if (rgcellid.Count == 0)
                    mprgcellidBynsize.Remove(nsize);
                mpsizeBycellid.Remove(cellid);
            }

            public void Update(Tbl tbl, int irow, int icol, int size)
            {
                var sizeOld = Size(irow, icol);


               if (sizeOld != size)
               {
                   Remove(tbl, irow, icol);

                   Add(tbl, irow, icol, size);
               }
            }

            public int Size(int irow, int icol)
            {
                var cellid = new Cellid(irow, icol);

                return mpsizeBycellid.ContainsKey(cellid) ? mpsizeBycellid[cellid] : 0;
            }

            public int Count(int size)
            {
                var nsize = -size;
                return mprgcellidBynsize[nsize].Count;
            }
        }

        protected void SolveCase(int iCase)
        {
            log.InfoFormat("Solving case {0}", iCase);
            var tbl = Pparser.Fetch<Tbl>();
            for(int irow=0;irow<tbl.crow;irow++)
                tbl.ParseRow(irow, Pparser.StLineNext());
          

            var cutmap = new Cutmap();
            InitCutmap(cutmap, tbl);
            var mpccutBysize = new Dictionary<int, int>();

            var sizeMaxPrev = -1;
            while(true)
            {
                int irowMax;
                int icolMax;
                int sizeMax;

                cutmap.Max(out irowMax, out icolMax, out sizeMax);

                if(sizeMax == 0)
                    break;
               
                if (sizeMaxPrev != sizeMax)
                    log.Info(sizeMax.ToString());

                if (sizeMax == 1)
                {
                    mpccutBysize[sizeMax] = cutmap.Count(1);
                    break;
                }
                sizeMaxPrev = sizeMax;
                Cut(tbl, irowMax, icolMax, sizeMax);

                UpdateCutmap(cutmap, tbl, irowMax, icolMax, sizeMax);
               // log.Info(tbl.Tsto());


                if (!mpccutBysize.ContainsKey(sizeMax))
                    mpccutBysize[sizeMax] = 0;
                mpccutBysize[sizeMax]++;
            }

            WriteLine(string.Format("Case #{0}: {1}", iCase, mpccutBysize.Keys.Count));

            var rgsize = mpccutBysize.Keys.ToList();
            rgsize.Sort();
            rgsize.Reverse();
            foreach(var size in rgsize)
                WriteLine("{0} {1}", size, mpccutBysize[size]);

        }



        private void InitCutmap(Cutmap cutmap, Tbl tbl)
        {
            for (int irow = 0; irow < tbl.crow; irow++)
                for (int icol = 0; icol < tbl.ccol; icol++)
                    cutmap.Add(tbl, irow, icol, SizeToCut(tbl, irow, icol));
        }

        private void UpdateCutmap(Cutmap cutmap, Tbl tbl, int irowCut, int icolCut, int sizeCut)
        {
            var irowMin = Math.Max(0, irowCut - sizeCut + 1);
            var irowLim = Math.Min(tbl.crow, irowCut + sizeCut);
            var icolMin = Math.Max(0, icolCut - sizeCut + 1);
            var icolLim = Math.Min(tbl.ccol, icolCut + sizeCut);
            for (int irow = irowMin; irow < irowLim; irow++)
            {
                
                for (int icol = icolMin; icol < icolLim; icol++)
                {
                    if (irow >= irowCut && irow < irowCut + sizeCut && icol >= icolCut && icol < icolCut + sizeCut)
                        cutmap.Remove(tbl, irow, icol);
                    else
                    {
                        var size = cutmap.Size(irow, icol);
                        if (irow + size >= irowCut && icol+size >= icolCut)
                        {
                            var sizeNew = Math.Max(irowCut - irow, icolCut - icol);
                            //if (SizeToCut(tbl, irow, icol) != sizeNew)
                            //    throw new Exception("size");
                            cutmap.Update(tbl, irow, icol, sizeNew);
                        }
                           
                    }
                        
                }
            }
        }

        private void Cut(Tbl tbl, int irowStart, int icolStart, int sizeMax)
        {
            for (var irow = irowStart; irow < irowStart+ sizeMax; irow++)
            for (var icol = icolStart; icol < icolStart+ sizeMax; icol++)
                tbl[irow, icol] = Kcell.Empty;
        }


       

        private int SizeToCut(Tbl tbl, int irowStart, int icolStart)
        {
            if (tbl[irowStart, icolStart] == Kcell.Empty)
                return 0;

            var size = 1;
            var irowNext = irowStart + 1;
            var icolNext = icolStart + 1;
            
            while(irowNext<tbl.crow && icolNext<tbl.ccol)
            {
                var kcell = tbl[irowStart, icolNext-1]; //első sor utolsó oszlopa
                var kcellNext = kcell == Kcell.Black ? Kcell.White : Kcell.Black;
                
                var fOK = true;
                for (int l = 0; l < size + 1; l++)
                {
                    if (tbl[irowNext, icolStart + l] != kcellNext)
                    {
                        fOK = false;
                        break;

                    }
                    if (tbl[irowStart + l, icolNext] != kcellNext)
                    {
                        fOK = false;
                        break;
                    }

                    kcellNext = kcellNext == Kcell.Black ? Kcell.White : Kcell.Black;
                    
                }
            
                if(!fOK)
                    break;
                size++;
                irowNext++;
                icolNext++;
            }

            return size;
        }
    }

    enum Kcell
    {
        Black,
        White,
        Empty,
    }

    internal class Tbl
    {
        public readonly int crow;
        public readonly int ccol;
        private Kcell[,] tbl;
        public Tbl(int crow, int ccol)
        {
            this.crow = crow;
            this.ccol = ccol;
            tbl = new Kcell[crow,ccol];
        }

        public void ParseRow(int irow, string st)
        {
            int icol = 0;
            foreach (var ch in st)
            {
                int i = Convert.ToInt32(ch.ToString(CultureInfo.InvariantCulture), 16);
                int j = 3;
                while(j>=0)
                {
                    tbl[irow, icol + j] = i%2 == 1 ? Kcell.White : Kcell.Black;
                    i /= 2;
                    j--;
                }
                icol += 4;
            }
        }

        public string Tsto()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            for(int irow=0;irow<crow;irow++)
            {

                for (int icol = 0; icol < ccol; icol++)
                {
                    switch (tbl[irow,icol])
                    {
                        case Kcell.Black:
                            sb.Append("\u2592\u2592");
                            break;
                        case Kcell.White:
                            
                            sb.Append("\u2588\u2588");
                            break;
                        case Kcell.Empty:
                            sb.Append("  ");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public Kcell this[int irow, int icol]
        {
            get { return tbl[irow, icol]; }
            set { tbl[irow, icol] = value; }
        }
    }

}
