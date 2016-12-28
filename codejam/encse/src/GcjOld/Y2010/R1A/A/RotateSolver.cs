using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.R1A.A
{
    public class RotateSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var tbl = Pparser.Fetch<Tbl>();
            for(var irow=0;irow<tbl.size;irow++)
                tbl.ParseRow(Pparser.StLineNext(), irow);
            tbl.Rotate();
            yield return tbl.KwinnerGet().ToString();
        }

        public enum Kcell
        {
            Nil=0, Red=1, Blue=2
        }

        [Flags]
        public enum Kwinner
        {
            Neither=0,Red=1,Blue=2,Both=3
        }
        public class Tbl
        {
            public readonly int size;
            private readonly int cmatchWin;
            Kcell[,] field;
            public Tbl(int size, int cmatchWin)
            {
                this.size = size;
                this.cmatchWin = cmatchWin;
                field = new Kcell[size, size];
            }

            public Kcell this[int row, int col]
            {
                get { return field[row, col]; }
            }

            public void ParseRow(string strow, int irow)
            {
                for(int icol=0;icol<size;icol++)
                {
                    switch (strow[icol])
                    {
                        case '.': field[irow, icol] = Kcell.Nil; break;
                        case 'R': field[irow, icol] = Kcell.Red; break;
                        case 'B': field[irow, icol] = Kcell.Blue; break;
                    }
                }
            }
            public void Rotate()
            {
                var fieldNew = new Kcell[size,size];
                for(var irowSrc=0;irowSrc<size;irowSrc++)
                {
                    var icolDst = size - irowSrc-1;
                    
                    var irowDst = size - 1;
                    for(var icolSrc = size - 1;icolSrc>=0;icolSrc--)
                    {
                        if(this[irowSrc, icolSrc] != Kcell.Nil)
                        {
                            fieldNew[irowDst, icolDst] = this[irowSrc, icolSrc];
                            irowDst--;
                        }
                    }
                }

                field = fieldNew;
            }

            public Kwinner KwinnerGet()
            {
                var rgdx = new[] {1, 0, 1, 1};
                var rgdy = new[] {0, 1, 1, -1};

                var kwinner = Kwinner.Neither;

                for(var irow=0;irow<size;irow++)
                for(var icol=0;icol<size;icol++)
                {
                    var kcellBase = this[irow, icol];
                    for(int i=0;i<4;i++)
                    {
                        var cmatch = 0;

                        var dx = rgdx[i];
                        var dy = rgdy[i];
                        for (var j = 0; j < cmatchWin; j++)
                        {
                            var irowT = irow + j*dx;
                            var icolT = icol + j*dy;
                            if (irowT >=size || icolT>=size || icolT<0 || irowT<0)
                                break;
                            if (this[irowT, icolT] != kcellBase)
                                break;
                            cmatch++;

                        }
                        if(cmatch>=cmatchWin)
                            kwinner |= (Kwinner)(int)kcellBase;
                    }
                }
                return kwinner;
            }
        }
    }
}
