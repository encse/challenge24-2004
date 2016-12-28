using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.Q
{
    public partial class PlotSolver : Solver
    {
        public override void Solve()
        {
            Info("reading through bitmap");
            var bmpwIn = new BitmapWrapper(FpatIn);
            Info("{0}x{1} {2}".StFormat(bmpwIn.Width, bmpwIn.Height, bmpwIn.Fmt));

            Debug.Assert(bmpwIn.Width == bmpwIn.Height);

            Info("Computing prg");
            Prg prg;
            {
                prg = new Cutter3(bmpwIn).Doit();
            }

            Info("Plotting");
            var bmpwResult = new BitmapWrapper(bmpwIn.Width, bmpwIn.Height, PixelFormat.Format24bppRgb);
            Plotter.Run(bmpwResult, false, prg);

            Info("Evaluating");
            Score = ScoreGet(bmpwIn, bmpwResult);
            log.Info("score: {0}".StFormat(Score));

            Info("Saving bitmap");
            bmpwResult.Save(FpatOut + ".png");

            using (Output)
            {
                foreach (var stm in prg.rgstm)
                    WriteLine(stm.ToString());
            }

            Info("done");
        }

        private Prg PrgFromBmpw(BitmapWrapper bmpwIn)
        {
            var rgsq = new Cutter(0, 0, bmpwIn).doit();
            var prg = PrgFromRgsq(rgsq);
            return prg;
        }

        public int ScoreGet(BitmapWrapper bmpwA, BitmapWrapper bmpwB)
        {
            long sum = 0;
            int w = bmpwA.Width;
            int h = bmpwB.Height;
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int p = bmpwA[x, y];
                int q = bmpwB[x, y];
                sum += (p - q) * (p - q);
            }

            return (int)Math.Round(Math.Sqrt(sum));
        }

#if false
        private byte[,] RgcolLoadBitmap(string fpat)
        {
            byte[,] rgcol;
            int w;
            int h;
            byte[] rgargb;
            int cstride;

            using (var bitmap = new Bitmap(fpat))
            {
                Console.WriteLine("{0}x{1} {2}".StFormat(bitmap.Width, bitmap.Height, bitmap.PixelFormat));
                w = bitmap.Width;
                h = bitmap.Height;

                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new Exception("tovabb");

                BitmapData bdt = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Debug.Assert(bdt.Stride > 0);

                IntPtr ptrScan0 = bdt.Scan0;
                cstride = bdt.Stride;

                rgargb = new byte[bdt.Stride*h];
                System.Runtime.InteropServices.Marshal.Copy(ptrScan0, rgargb, 0, Math.Abs(bdt.Stride)*bitmap.Height);
                bitmap.UnlockBits(bdt);

                int iargbStride = 0;
                rgcol = new byte[w,h];
                for (var y = 0; y < h; y++)
                {
                    if (y%1000 == 0)
                        log.Debug(y);

                    int iargb = iargbStride;
                    iargbStride += cstride;
                    for (var x = 0; x < w; x++)
                    {
                        rgcol[x, y] = rgargb[iargb + 1];
                        iargb += 4;
//                        if (rgcol[x,y] != bitmap.GetPixel(x, y).R)
//                            throw new Exception("{0} {1}".StFormat(x, y));
                    }
                }
            }
            return rgcol;
        }
#endif
        public Prg PrgFromRgsq(IEnumerable<Cutter.Sq> rgsq)
        {
            int regBrushX = 3;
            int regBrushY = 4;
            int regBrushW = 5;
            int regReturn = 10;
            int regAcc = 11;
            int regFoo = 12;

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

            int lblBrush4 = 1;
            int lblBrush3 = 2;
            int lblBrush2 = 3;
            int lblBrush1 = 4;
   
            rgstm[0] = new Movc(R.regPC, rgstm.Count);
            
            rgstm.Add(new Movc(regFoo, 3));

            int lastSqX = 0;
            int lastSqY = 0;
            int lastSqW = 0;

            int csqUsed = 0;
            foreach (var sq in rgsq)
            {

                if (sq.X >= 16000 || sq.Y >= 16000)
                    continue;

                if (rgstm.Count + 6 >= 20000)
                {
                    // try to compress
                    // check if we actually achieved anything
                    //if (rgstm.Count + 6 >= 20000)
                    break;
                }

                int cbrush = CbrushFomCol(sq.Col);
                if (cbrush == 0)
                    continue;

                csqUsed++;

                if (csqUsed % 500 == 0)
                {
                    log.Debug("sq used: {0}".StFormat(csqUsed));
                }
                if (lastSqX != sq.X)
                {
                    rgstm.Add(new Movc(regBrushX, sq.X));
                    lastSqX = sq.X;
                }

                if (lastSqY != sq.Y)
                {
                    rgstm.Add(new Movc(regBrushY, sq.Y));
                    lastSqY = sq.Y;
                }

                if (lastSqW != sq.W)
                {
                    Info(sq.W);
                    rgstm.Add(new Movc(regBrushW, sq.W));
                    lastSqW = sq.W;
                }

                int lblBrush;
                switch (cbrush)
                {
                    case 4: lblBrush = lblBrush4; break;
                    case 3: lblBrush = lblBrush3; break;
                    case 2: lblBrush = lblBrush2; break;
                    case 1: lblBrush = lblBrush1; break;
                    default:
                        throw new Exception("wtf, invalid cbrush: {0}".StFormat(cbrush));
                }
                rgstm.Add(new Movr(regReturn, R.regPC));
                rgstm.Add(new Movc(R.regPC, lblBrush));
            }
            rgstm.Add(new Exit());
            log.Info("sq used: {0}".StFormat(csqUsed));
            return new Prg(rgstm.ToArray());
        }

        private int CbrushFomCol(int col)
        {
            if (col < 32)
                return 4;
            if (col < 32 + 64)
                return 3;
            if (col < 32 + 2 * 64)
                return 2;
            if (col < 32 + 3 * 64)
                return 1;
            return 0;
        }

        private Cutter2 Cutter2Create(BitmapWrapper bmpw, int min)
        {
            return new Cutter2((x, y) => bmpw[x,y] < min);
        }

        private IEnumerable<SortedDictionary<Cutter2.Sq, int>> EnmpcsqBySq(BitmapWrapper bmpw)
        {
            //var rgenrsq = new[] {224, 160, 96, 32}
            var rgenrsq = new[] {255, 192, 128, 64}
                .Select(min => Cutter2Create(bmpw, min).Doit().GetEnumerator()).ToArray();

            var move = new Action<int>(i =>
            {
                if(!rgenrsq[i].MoveNext())
                    rgenrsq[i] = null;
            });

            for(var i=0;i<rgenrsq.Length;i++)
                move(i);

            for(;;)
            {
                var rgenrsqNotnull = rgenrsq.Where(enrsq => enrsq != null).ToArray();
                if(!rgenrsqNotnull.Any())
                    break;
                var maxw = rgenrsqNotnull.Max(enrsq => enrsq.Current.W);
                var mpcBySq = new SortedDictionary<Cutter2.Sq, int>();
                for(var i=0;i<rgenrsq.Length;i++)
                {
                    for(;;)
                    {
                        if(rgenrsq[i] == null)
                            break;
                        var sq = rgenrsq[i].Current;
                        if(sq.W != maxw)
                            break;
                        int c;
                        if(!mpcBySq.TryGetValue(sq, out c))
                            c = 0;
                        c++;
                        mpcBySq[sq] = c;
                        move(i);
                    }
                }
                yield return mpcBySq;
            }
        }
    
        private Prg PrgFromBmpw2(BitmapWrapper bmpw)
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

            var csqUsed = 0;
            foreach (var mpcsqBySq in EnmpcsqBySq(bmpw))
            {
                foreach(var kvcsqBySq in mpcsqBySq)
                {
                    var sq = kvcsqBySq.Key;
                    var csq = kvcsqBySq.Value;

                    if(sq.X >= 16000 || sq.Y >= 16000)
                        continue;

                    if(rgstm.Count + 6 >= 20000)
                        break;

                    csqUsed++;

                    if(csqUsed % 500 == 0)
                    {
                        log.Debug("sq used: {0}".StFormat(csqUsed));
                    }
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
                        Info(sq.W);
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
            }
            rgstm.Add(new Exit());
            log.Info("sq used: {0}".StFormat(csqUsed));
            return new Prg(rgstm.ToArray());
        }

    }
}
