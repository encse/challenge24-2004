using System;
using System.Linq;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.QR.A
{
    public class TicTacToeTomekSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var tbl = new Tbl(4,4);
            for (var irow = 0; irow < tbl.size; irow++)
                tbl.ParseRow(pparser.StLineNext(), irow);
            if(!pparser.FEof())
                pparser.StLineNext();
            return () => Solve(tbl);
        }

        private IEnumerable<object> Solve(Tbl tbl)
        {
            switch (tbl.KwinnerGet())
            {
                case Kwinner.Neither:
                    yield return "Game has not completed";
                    break;
                case Kwinner.X:
                    yield return "X won";
                    break;
                case Kwinner.O:
                    yield return "O won";
                    break;
                case Kwinner.Draw:
                    yield return "Draw";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum Kcell
        {
            Nil = 0, X = 1, O = 2, T = 3
        }

        [Flags]
        public enum Kwinner
        {
            Neither = 0, X = 1, O = 2, Draw = 3
        }
        public class Tbl
        {
            public readonly int size;
            private readonly int cmatchToWin;
            Kcell[,] field;
            public Tbl(int size, int cmatchToWin)
            {
                this.size = size;
                this.cmatchToWin = cmatchToWin;
                field = new Kcell[size, size];
            }

            public Kcell this[int row, int col]
            {
                get { return field[row, col]; }
            }

            public void ParseRow(string strow, int irow)
            {
                for (int icol = 0; icol < size; icol++)
                {
                    switch (strow[icol])
                    {
                        case '.': field[irow, icol] = Kcell.Nil; break;
                        case 'X': field[irow, icol] = Kcell.X; break;
                        case 'O': field[irow, icol] = Kcell.O; break;
                        case 'T': field[irow, icol] = Kcell.T; break;
                    }
                }
            }
         

            public Kwinner KwinnerGet()
            {
                var rgdx = new[] { 1, 0, 1, 1,  -1,  0, -1, -1};
                var rgdy = new[] { 0, 1, 1, -1,  0, -1, -1, 1 };

                for (var irow = 0; irow < size; irow++)
                {
                    for (var icol = 0; icol < size; icol++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            var cmatch = 0;

                            var dx = rgdx[i];
                            var dy = rgdy[i];

                            var kcellBase = this[irow, icol];
                            if(kcellBase == Kcell.T || kcellBase == Kcell.Nil)
                                continue; 
                            for (var j = 0; j < cmatchToWin; j++)
                            {
                                var irowT = irow + j * dy;
                                var icolT = icol + j * dx;

                                if (irowT >= size || icolT >= size || icolT < 0 || irowT < 0)
                                    break;

                                //if (kcellBase == Kcell.T)
                                //{
                                //    if (j != 0 && j != 1)
                                //        throw new Exception("wtf");
                                //    kcellBase = this[irowT, icolT];
                                //}

                                if (this[irowT, icolT] != kcellBase && this[irowT, icolT] != Kcell.T)
                                    break;
                                cmatch++;

                            }

                            if (cmatch >= cmatchToWin)
                            {
                                return (Kwinner)(int)kcellBase;
                            }
                                
                        }
                    }
                }

                for (var irow = 0; irow < size; irow++)
                    for (var icol = 0; icol < size; icol++)
                        if(this[irow, icol] == Kcell.Nil)
                            return Kwinner.Neither;
                return Kwinner.Draw;
            }
        }

     
    }
}
