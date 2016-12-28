using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using Cmn.Util;
using Gcj.Util;


namespace Gcj.Y2014.QR.C
{
    public class MinesweeperSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int r, c, m;
            pparser.Fetch(out r, out c, out m);

            return () => Solve2(r,c,m);
        }

        private IEnumerable<object> Solve2(int crow, int ccol, int cmine)
        {

            var tbl = new int[crow, ccol] ;

            //egy se hiányzik -> nincs megoldás
            if (cmine == crow*ccol)
                return Unsolvable(tbl, crow, ccol, cmine);
            
            //egy hiányzik, akkor a jobb alsó sarok kivételével mindenhova teszünk
            if (cmine == crow*ccol - 1)
            {
                for (int irow = 0; irow < crow; irow++)
                    for (int icol = 0; icol < ccol; icol++)
                        tbl[irow, icol] = 1;
                tbl[crow - 1, ccol - 1] = 0;
                return ToSolution(tbl, crow, ccol);
            }
            
            //egy sor esetén balról feltöltjük a sort
            if (crow == 1)
            {
                for (int icol = 0; icol < cmine; icol++)
                    tbl[0, icol] = 1;
                return ToSolution(tbl, crow, ccol);
            }

            //egy oszlop esetén felülről feltöltjük
            if (ccol == 1)
            {
                for (int irow = 0; irow < cmine; irow++)
                    tbl[irow, 0l] = 1;
                return ToSolution(tbl, crow, ccol);
            }

            if (ccol == 2)
            {
                if (cmine%2 == 1 || cmine == ccol*crow-2)
                    return Unsolvable(tbl, crow, ccol, cmine);
                for (int irow = 0; irow < cmine/2; irow++)
                {
                    tbl[irow, 0] = 1;
                    tbl[irow, 1] = 1;
                }

                return ToSolution(tbl, crow, ccol);
            }

            if (crow == 2)
            {
                if (cmine % 2 == 1 || cmine == ccol*crow-2)
                    return Unsolvable(tbl, crow, ccol, cmine);
                for (int icol = 0; icol < cmine / 2; icol++)
                {
                    tbl[0, icol] = 1;
                    tbl[1, icol] = 1;
                }

                return ToSolution(tbl, crow, ccol);
            }

            {
                tbl = new int[crow, ccol];
                int imine = 0;
                for (int icol = 0; icol < ccol - 3 && imine < cmine; icol++)
                {
                    for (int irow = 0; irow < crow -2 && imine < cmine; irow++)
                    {
                        tbl[irow, icol] = 1;
                        imine++;
                    }

                    if (imine + 2 <= cmine)
                    {
                        tbl[crow - 2, icol] = 1;
                        tbl[crow - 1, icol] = 1;
                        imine += 2;
                    }

                }

                for (int irow = 0; irow < crow - 2 && imine < cmine; irow++)
                {
                    tbl[irow, ccol - 3] = 1;
                    imine++;

                    if (imine + 2 <= cmine)
                    {
                        tbl[irow, ccol - 2] = 1;
                        tbl[irow, ccol - 1] = 1;
                        imine += 2;
                    }
                }

                if (imine <= cmine - 2)
                {
                    tbl[crow-2, ccol - 3] = 1;
                    tbl[crow-1, ccol - 3] = 1;
                    imine += 2;
                }

                if (imine == cmine && Check(crow - 1, ccol - 1, tbl, crow, ccol, (x, y, t) => t[x, y] == 1))
                    return ToSolution(tbl, crow, ccol);

            }


            {
                tbl = new int[crow, ccol];
                int imine = 0;
                for (int irow = 0; irow < crow - 3 && imine < cmine; irow++)
                {
                    for (int icol = 0; icol < ccol - 2 && imine < cmine; icol++)
                    {
                        tbl[irow, icol] = 1;
                        imine++;
                    }

                    if (imine + 2 <= cmine)
                    {
                        tbl[irow, ccol - 2] = 1;
                        tbl[irow, ccol - 1] = 1;
                        imine += 2;
                    }

                }

                for (int icol = 0; icol < ccol - 2 && imine < cmine; icol++)
                {
                    tbl[crow - 3, icol] = 1;
                    imine++;

                    if (imine + 2 <= cmine)
                    {
                        tbl[crow - 2, icol] = 1;
                        tbl[crow - 1, icol] = 1;
                        imine += 2;
                    }
                }

                if (imine <= cmine - 2)
                {
                    tbl[crow - 3, ccol - 2] = 1;
                    tbl[crow - 3, ccol - 1] = 1;
                    imine += 2;
                }

                if (imine == cmine && Check(crow - 1, ccol- 1, tbl, crow, ccol, (x, y, t) => t[x, y] == 1))
                    return ToSolution(tbl, crow, ccol);

            }


           


          
            return Unsolvable(tbl, crow, ccol, cmine);
        }

       
        private IEnumerable<object> ToSolution(int[,] tbl, int crow, int ccol)
        {
            var rg = new List<object>();
            for(int irow=0;irow<crow;irow++)
            {
                var st = "";
                for (int icol = 0; icol < ccol; icol++)
                {
                    if (icol == ccol - 1 && irow == crow - 1)
                        st += "c";
                    else
                        st += tbl[irow, icol] == 1 ? "*" : ".";
                        
                }
                rg.Add(Solwrt.NewLine);
                rg.Add(st);
            }

            return rg;
        }

        private IEnumerable<object> Unsolvable(int[,] tbl, int crow, int ccol, int cmine)
        {

            //if (FSolvableSlow(crow, ccol, cmine))
            //{
            //    Tsto(crow - 1, ccol - 1, tbl, crow, ccol); 
            //    throw new Exception();
            //}
            return new[] {Solwrt.NewLine, "Impossible"};
        }

        private bool FSolvableSlow(int crow, int ccol, int cmine)
        {
            int lLim = 1 << (crow * ccol);
            for (int tbl = 0; tbl < lLim; tbl++)
            {
                if (NumberOfSetBits(tbl) != cmine)
                    continue;

                for (int irow = 0; irow < crow; irow++)
                {
                    for (int icol = 0; icol < ccol; icol++)
                    {
                        if (Check(irow, icol, tbl, crow, ccol, (x, y, t) => FMine(x, y, t, crow, ccol)))
                        {
                            return true;
                        }

                    }
                }
         
            }
            return false;
        }

        private IEnumerable<object> Solve(int crow, int ccol, int cmine)
        {
            int lLim = 1 << (crow*ccol);
            for (int tbl = 0; tbl < lLim; tbl++)
            {
                if(NumberOfSetBits(tbl) != cmine)
                    continue;

                for (int irow = 0; irow < crow; irow++)
                {
                    for (int icol = 0; icol < ccol; icol++)
                    {
                        if (Check(irow, icol, tbl, crow, ccol, (x,y,t) => FMine(x,y,t, crow, ccol)))
                        {
                            foreach (var st in RgTsto(irow, icol, tbl, crow, ccol))
                            {
                             
                                yield return Solwrt.NewLine;
                                yield return st;
                            }

                            yield break;
                        }
                            
                    }
                }
            }
            yield return Solwrt.NewLine;
            yield return "Impossible";
        }

        private bool Check<T>(int irowStart, int icolStart, T tbl, int crow, int ccol, Func<int,int, T, bool> dgFMine)
        {
            //ha aknára lép az tutira rossz
            if (dgFMine(irowStart, icolStart, tbl))
                return false;

            //-1: felfedezetlen, -2 akna, egyébként annyi amennyi akna veszi körül
            var revealed = new int[crow, ccol];
            for (int irow = 0; irow < crow; irow++)
                for (int icol = 0; icol < ccol; icol++)
                    revealed[irow, icol] = -1;


            var q = new Queue<Tuple<int, int>>();
            q.Enqueue(new Tuple<int, int>(irowStart, icolStart));
            while (q.Any())
            {
                var t = q.Dequeue();
                var irow = t.Item1;
                var icol = t.Item2;

                if(!FInBounds(irow, icol, crow, ccol) || revealed[irow, icol] != -1)
                    continue;

                if (dgFMine(irow, icol, tbl))
                {
                    revealed[irow, icol] = -2;
                }
                else
                {
                    var cmineAround = 0;

                    Action<int,int> dg = (irowT, icolT) =>
                    {
                        if (FInBounds(irowT, icolT, crow, ccol) && dgFMine(irowT, icolT, tbl))
                        {
                            cmineAround++;
                            revealed[irowT, icolT] = -2;
                        }
                    };

                    dg(irow - 1, icol - 1);
                    dg(irow - 1, icol);
                    dg(irow - 1, icol + 1);
                    dg(irow, icol - 1);
                    dg(irow, icol + 1);
                    dg(irow + 1, icol - 1);
                    dg(irow + 1, icol);
                    dg(irow + 1, icol + 1);

                    Action<int, int> dgEnq = (irowT, icolT) =>
                    {
                        if (FInBounds(irowT, icolT, crow, ccol) && 
                            revealed[irowT, icolT] == -1)
                            q.Enqueue(new Tuple<int, int>(irowT, icolT));
                    };
                    revealed[irow, icol] = cmineAround;

                    if(cmineAround == 0)
                    {
                        dgEnq(irow - 1, icol - 1);
                        dgEnq(irow - 1, icol);
                        dgEnq(irow - 1, icol + 1);
                        dgEnq(irow, icol - 1);
                        dgEnq(irow, icol + 1);
                        dgEnq(irow+1, icol-1);
                        dgEnq(irow + 1, icol);
                        dgEnq(irow + 1, icol + 1);
                    }
                }
            }

            //maradt-e felfedezetlen
            for (int irow = 0; irow < crow; irow++)
                for (int icol = 0; icol < ccol; icol++)
                    if (revealed[irow, icol] == -1 && !dgFMine(irow, icol, tbl))
                        return false;
            return true;
        }

        private void Tsto(int irowStart, int icolStart, int tbl, int crow, int ccol)
        {
            Console.WriteLine(string.Join("\n", RgTsto(irowStart, icolStart, tbl, crow, ccol)));
        }

        private void Tsto(int irowStart, int icolStart, int[,] tbl, int crow, int ccol)
        {
            for (int irow = 0; irow < crow; irow++)
            {
                var st = "";
                for (int icol = 0; icol < ccol; icol++)
                {
                    if (icol == icolStart && irow == irowStart)
                        st += "c";
                    else
                        st += tbl[irow, icol] == 1 ? "*" : ".";

                }
                Console.WriteLine(st);
            }
        }
        private IEnumerable<object> RgTsto(int irowStart, int icolStart, int tbl, int crow, int ccol)
        {
            var rgstrow = new List<string>();
            for (int irow = 0; irow < crow; irow++)
            {
                string st = "";
                for (int icol = 0; icol < ccol; icol++)
                {
                    if (FMine(irow, icol, tbl, crow, ccol))
                        st += "*";
                    else if (irow == irowStart && icol == icolStart)
                        st += "c";
                    else
                        st += ".";
                }
                rgstrow.Add(st);
            }
            return rgstrow;
        }

        private bool FMine(int irow, int icol, int tbl, int crow, int ccol)
        {
            return (tbl & (1 << (irow*ccol + icol))) != 0 ;
        }

        private static bool FInBounds(int irow, int icol, int crow, int ccol)
        {
            return irow >= 0 && irow < crow && icol >= 0 && icol < ccol;
        }


        int NumberOfSetBits(int i)
        {
            var n = 0;
            while (i > 0)
            {
                if ((i & 1) == 1)
                    n++;
                i >>= 1;
         
            }
            return n;
        }
    }
}
