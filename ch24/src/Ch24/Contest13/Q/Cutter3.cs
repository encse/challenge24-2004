using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using log4net;

namespace Ch24.Contest13.Q
{
    public class Cutter3
    {
        static ILog log = LogManager.GetLogger(typeof(Program));
 
        private static readonly byte[] Rgcol = {255, 192, 128, 64, 0};
        private readonly BitmapWrapper Bmpw;

        public Cutter3(BitmapWrapper bmpw)
        {
            this.Bmpw = bmpw;
        }

        public class SqXYWComparer : IComparer<Sq>
        {
            public int Compare(Sq sq1, Sq sq2)
            {
                var d = sq2.W - sq1.W;
                if(d!=0)
                    return d;

                d = sq1.Y - sq2.Y;
                if(d!=0)
                    return d;

                d = sq1.X - sq2.X;
                return d;
            }
        }

        public class SqScoreComparer : IComparer<Sq>
        {
            public int Compare(Sq sq1, Sq sq2)
            {
                var d2 = sq2.Score - sq1.Score;
                if(d2!=0)
                    return Math.Sign(d2);

                var d = sq2.W - sq1.W;
                if(d!=0)
                    return d;

                d = sq1.Y - sq2.Y;
                if(d!=0)
                    return d;

                d = sq1.X - sq2.X;
                return d;
            }
        }

        public class Sq
        {
            public static BitmapWrapper Bmpw; 
            public static readonly Dictionary<Sq,Sq> Hlmsq=new Dictionary<Sq, Sq>(); 

            public static Sq SqGet(int x, int y, int w)
            {
                return Hlmsq.EnsureGet(new Sq {X = x, Y = y, W = w}, () => new Sq {X = x, Y = y, W = w});
            }

            public int X;
            public int Y;
            public int W;
            private long[] _Rgscore;

            private Sq()
            {
            }

            public long[] Rgscore
            {
                get
                {
                    if(_Rgscore==null)
                    {
                        _Rgscore=new long[Rgcol.Length];
                        if(W<=32)
                        {
                            for(int i = 0; i < _Rgscore.Length; i++)
                            {
                                _Rgscore[i] = ScoreGet(Rgcol[i]);
                            }
                        }
                        else
                        {
                            foreach(var sq in Ensq)
                            {
                                for(int i = 0; i < _Rgscore.Length; i++)
                                {
                                    _Rgscore[i] = _Rgscore[i] + sq.Rgscore[i];
                                }
                            }
                        }
                    }
                    return _Rgscore;
                }
            }

            private long _Score = -1;
            public long Score
            {
                get
                {
                    if(_Score==-1)
                    {
                        _Score = Rgscore.Min();
                    }
                    return _Score;
                }
            }

            private int _ICol = -1;
            public int ICol
            {
                get
                {
                    if(_ICol==-1)
                    {
                        for(int i = 0; i < Rgscore.Length; i++)
                        {
                            if(Rgscore[i]==Score)
                            {
                                _ICol = i;
                                break;
                            }
                        }
                        Debug.Assert(_ICol!=-1);
                    }
                    return _ICol;
                }
            }

            private long ScoreGet(int col)
            {
                long sum = 0;
                for(int y = Y; y < Math.Min(Y+W,16000); y++)
                    for(int x = X; x <Math.Min(X+W,16000); x++)
                    {
                        int q = Bmpw[x, y];
                        sum += (col - q) * (col - q);
                    }

                return sum;
            }

            private List<Sq> _Ensq;
            public IEnumerable<Sq> Ensq
            {
                get
                {
                    if(_Ensq==null)
                    {
                        _Ensq=new List<Sq>();
                        Debug.Assert(W % 2 == 0);
                        var nw = W / 2;
                        for(var ix = 0; ix < 2; ix++)
                            for(var iy = 0; iy < 2; iy++)
                                _Ensq.Add(SqGet(X + ix * nw, Y + iy * nw, nw));
                    }
                    return _Ensq;
                }
            }


            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}, W: {2}", X, Y, W);
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
                    var result = X;
                    result = (result * 397) ^ Y;
                    result = (result * 397) ^ W;
                    return result;
                }
            }

            public int CompareTo(Sq sq)
            {
                var d = sq.W - W;
                if(d!=0)
                    return d;

                d = Y - sq.Y;
                if(d!=0)
                    return d;

                d = X - sq.X;
                return d;
            }
        }

        public Prg Doit()
        {
            Sq.Bmpw = Bmpw;

            var rgsqByScore = new SortedSet<Sq>(new SqScoreComparer());
            var rgsqDraw = new SortedSet<Sq>(new SqXYWComparer());

            var addSq = new Action<Sq>(sq =>
            {
                rgsqByScore.Add(sq);
                if(sq.ICol!=0)
                    rgsqDraw.Add(sq);
            });

            var delSq = new Action<Sq>(sq =>
            {
                rgsqByScore.Remove(sq);
                if(sq.ICol!=0)
                    rgsqDraw.Remove(sq);
                Sq.Hlmsq.Remove(sq);
            });

            log.Info("Building cache");
            {
                var sqFirst = Sq.SqGet(0, 0, 16384);
                var qqq = sqFirst.Score;
                addSq(sqFirst);
            }
            log.Info("Starting");

            var rgstm = new List<Stm>();

            int wLast = 0;
            int cstmLast = 0;
            for(;;)
            {
                var sqSplit = rgsqByScore.Min;

                if(sqSplit.W==1)
                    break;

                var sqMinW = rgsqDraw.Max;
                if(sqMinW!=null && sqMinW.W!=wLast)
                {
                    wLast = sqMinW.W;
                    log.Info("W: "+wLast);
                }

                delSq(sqSplit);

                foreach(var sq in sqSplit.Ensq)
                {
                    addSq(sq);
                }

                if(rgsqDraw.Count * 5 < 20000)
                    continue;

                var rgstmNew = RgstmGet(rgsqDraw);

                if(rgstmNew.Count > 20000)
                    break;

                rgstm = rgstmNew;
                if(cstmLast+500<rgstm.Count)
                {
                    cstmLast = rgstm.Count;
                    log.Info("Cstm: "+cstmLast);
                }
            }
            log.Info("Cstm: "+rgstm.Count);
            Sq.Bmpw = null;
            Sq.Hlmsq.Clear();

            return new Prg(rgstm.ToArray());

        }

        private List<Stm> RgstmGet(SortedSet<Sq> rgsq)
        {
            const int regBrushX = 3;
            const int regBrushY = 4;
            const int regBrushW = 5;
            const int regReturn = 10;
            const int regAcc = 11;
            const int regFoo = 12;

            var rgstm = new List<Stm>
            {
                null, 
                new Brush(regFoo), //192
                new Brush(regFoo), //128
                new Brush(regFoo), //64
                new Brush(regFoo), //0
                new Movc(regAcc, 1),
                new Add(regReturn, regAcc),
                new Movr(R.regPC, regReturn),
              
            };

            const int lblBrush4 = 1;
            const int lblBrush3 = 2;
   
            rgstm[0] = new Movc(R.regPC, rgstm.Count);
            
            rgstm.Add(new Movc(regFoo, 3));

            var lastSqX = 0;
            var lastSqY = 0;
            var lastSqW = 0;

            foreach (var sq in rgsq)
            {
                    var csq = sq.ICol;

                    if(sq.X >= 16000 || sq.Y >= 16000)
                        continue;

                    if(csq==0)
                        break;

                    if(lastSqX != sq.X)
                    {
                        rgstm.Add(new Movc(regBrushX, sq.X));
                        lastSqX = sq.X;
                    }

                    if(lastSqY != sq.Y)
                    {
                        rgstm.Add(new Movc(regBrushY, sq.Y));
                        lastSqY = sq.Y;
                    }

                    if(lastSqW != sq.W)
                    {
                        rgstm.Add(new Movc(regBrushW, sq.W));
                        lastSqW = sq.W;
                    }

                    switch(csq)
                    {
                        case 4:
                        {
                            rgstm.Add(new Movr(regReturn, R.regPC));
                            rgstm.Add(new Movc(R.regPC, lblBrush4));
                            break;
                        }
                        case 3:
                        {
                            rgstm.Add(new Movr(regReturn, R.regPC));
                            rgstm.Add(new Movc(R.regPC, lblBrush3));
                            break;
                        }
                        case 2:
                        {
                            rgstm.Add(new Brush(regFoo));
                            rgstm.Add(new Brush(regFoo));
                            break;
                        }
                        case 1:
                        {
                            rgstm.Add(new Brush(regFoo));
                            break;
                        }
                        default:
                            throw new Exception("wtf, invalid cbrush: {0}".StFormat(csq));
                    }
                }
            rgstm.Add(new Exit());
            return rgstm;
        }

    }



}
