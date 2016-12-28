using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest13.Q
{
    public class Cutter
    {
        //public byte[,] Rgcol;
        public BitmapWrapper Bmpw;
        public int Dx;
        public int Dy;

        public class Sq
        {
            public int X;
            public int Y;
            public int W;
            public int Col=-1;

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}, W: {2}, Col: {3}", X, Y, W, Col);
            }

            public bool Equals(Sq other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return other.X == X && other.Y == Y && other.W == W;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != typeof(Sq))
                    return false;
                return Equals((Sq) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = X;
                    result = (result * 397) ^ Y;
                    result = (result * 397) ^ W;
                    return result;
                }
            }
        }

        public Cutter(int dx, int dy, BitmapWrapper bmpw)
        {
            Bmpw = bmpw;
            Dx = dx;
            Dy = dy;
        }

        public byte ColAt(int x, int y)
        {
            return Bmpw[x, y];
        }

        public IEnumerable<Sq> doit()
        {
            //Debug.Assert(Rgcol.Rank==2);
            Debug.Assert(Bmpw.Width == Bmpw.Height);

            var rgsqProcess=new LinkedList<Sq>();

            rgsqProcess.AddLast(new Sq {X = 0, Y = 0, W = 16384});

            var mprgcbycolBySq = new Dictionary<Sq, double[]>();

            Func<Sq, double[]> rgcbycolGet=null;
            rgcbycolGet = new Func<Sq, double[]>(sq =>
            {
                if(sq.X>=16000||sq.Y>=16000)
                    return new double[256];

                if(sq.W<=64)
                {
                    return RgcByColGet(sq);
                }

                return mprgcbycolBySq.EnsureGet(sq,()=>
                {
                    Debug.Assert(sq.W%2==0);
                    var rgcByCol = new double[256];
                    var nw = sq.W / 2;
                    for(var ix=0;ix<2;ix++)
                    for(var iy=0;iy<2;iy++)
                    {
                        var rgcByColT = rgcbycolGet(new Sq {X = sq.X + ix * nw, Y = sq.Y + iy * nw, W = nw});
                        for(var i=0;i<256;i++)
                            rgcByCol[i] = rgcByCol[i] + rgcByColT[i];
                    }
                    return rgcByCol;
                });
            });

            for(;;)
            {
                if(rgsqProcess.Count==0)
                    break;

                var sq = rgsqProcess.First.Value;
                rgsqProcess.RemoveFirst();

                Debug.Assert(sq.W>1);
                Debug.Assert(sq.Col==-1);
                if(sq.X >=16000 || sq.Y>=16000)
                    continue;
                
                var rgcByCol = rgcbycolGet(sq);

                double cpx = rgcByCol.Sum();
                double colSum = rgcByCol.Select((c, col) => c*col).Sum();
                
                double colAvg = Math.Round(colSum/ cpx);

                double dcolFromAvg = rgcByCol.Select((c,col) => Math.Abs((double) col - colAvg)*c).Sum();

                var fOk = dcolFromAvg/cpx < 15;

                if(fOk)
                {
                    yield return new Sq {X = Dx + sq.X, Y = Dy + sq.Y, W = sq.W, Col = (byte)colAvg};
                }
                else if(sq.W <= 32 && false)
                {
                    yield return new Sq { X = Dx + sq.X, Y = Dy + sq.Y, W = sq.W, Col = (byte)colAvg};

//                    for(var x=sq.X;x<sq.X+sq.W;x++)
//                        for (var y = sq.Y; y < sq.Y + sq.W; y++)
//                        {
//                            if (x >= 16000 || y >= 16000)
//                                continue;
//
//                            yield return new Sq { X = Dx + x, Y = Dy + y, W = 1, Col = Rgcol[x, y] };
//                        }
                }
                else
                {
                    Debug.Assert(sq.W%2==0);
                    var nw = sq.W / 2;
                    for(var ix=0;ix<2;ix++)
                    for(var iy=0;iy<2;iy++)
                        rgsqProcess.AddLast(new Sq {X = sq.X + ix * nw, Y = sq.Y + iy * nw, W = nw});
                }
            }
        }

        private double[] RgcByColGet(Sq sq)
        {
            var rgcByCol = new double[256];

            var cx = Math.Min(sq.X + sq.W, 16000);
            var cy = Math.Min(sq.Y + sq.W, 16000);
            ;

            for(var x = sq.X; x < cx; x++)
                for(var y = sq.Y; y < cy; y++)
                {
                    var colT = ColAt(x, y);
                    rgcByCol[colT] = rgcByCol[colT] + 1;
                }
            return rgcByCol;
        }
    }



}
