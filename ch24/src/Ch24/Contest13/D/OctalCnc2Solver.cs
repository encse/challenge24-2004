using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Cmn.Util;

namespace Ch24.Contest13.D
{
    public class OctalCnc2Solver : Contest.Solver
    {
        class Sign
        {
            public Bound bound;
            public double[,] rgbri;

            public double DGet(Sign sign)
            {
                var maxx = rgbri.GetLength(0);
                var maxy = rgbri.GetLength(0);
                var d = 0.0;
                for(var x = 0; x < maxx; x++)
                {
                    for(var y = 0; y < maxy; y++)
                    {
                        d += Math.Abs(rgbri[x, y] - sign.rgbri[x, y]);
                    }
                }
                return d;
            }
        }

        class Bound
        {
            public int minx;
            public int maxx;
            public int miny;
            public int maxy;

            public Bound(Point p)
            {
                minx = maxx = p.X;
                miny = maxy = p.Y;
            }

            public void Add(Point p)
            {
                minmax(p.X, ref minx, ref maxx);
                minmax(p.Y, ref miny, ref maxy);
                _pointMiddle = null;
            }

            public void Add(Bound bound)
            {
                Add(new Point(bound.minx,bound.miny));
                Add(new Point(bound.maxx,bound.maxy));
            }

            private void minmax(int v, ref int min, ref int max)
            {
                min = Math.Min(min, v);
                max = Math.Max(max, v);
            }

            private Point? _pointMiddle;
            public Point PointMiddle
            {
                get
                {
                    return _pointMiddle ?? (_pointMiddle = new Point((minx + maxx) / 2, (miny + maxy) / 2)).Value;
                }
            }

            public int SizeGet()
            {
                return (maxy - miny) * (maxx - minx);
            }

            public int DGet(Bound bound)
            {
                int dx;
                if(Math.Max(minx,bound.minx)<Math.Min(maxx, bound.maxx))
                    dx = 0;
                else
                    dx = new[]
                    {
                        minx - bound.minx,
                        maxx - bound.minx,
                        minx - bound.maxx,
                        maxx - bound.maxx,
                    }.Select(Math.Abs).Min();
                
                int dy;
                if(Math.Max(miny,bound.miny)<Math.Min(maxy, bound.maxy))
                    dy = 0;
                else
                    dy = new[]
                    {
                        miny - bound.miny,
                        maxy - bound.miny,
                        miny - bound.maxy,
                        maxy - bound.maxy,
                    }.Select(Math.Abs).Min();
                return dx*dx+dy*dy;
            }
        }

        public override void Solve()
        {
            var fpatAbc = "data/2013/D/reference_digits.gray.png";
            var rgsignAbc = RgsignFromFile(fpatAbc, "out/2013/D/reference_digits.gray.png")[0];
            var rgrgsign = RgsignFromFile(FpatIn, FpatOut);

            Info("Match to ABC");
            var sb = new StringBuilder();
            foreach(var rgsign in rgrgsign)
            {
                var sum = 0;
                var isign = 0;
                foreach(var sign in rgsign)
                {
                    isign = IsignGet(rgsignAbc, sign);
                    sum += isign;
                    sb.Append(isign);
                }
                if((sum-isign) % 8 != isign)
                {
                    using(var bmp = Image.FromFile(FpatIn))
                    {
                        using(var bmpAbc = Image.FromFile(fpatAbc))
                        {
                            using(var g = Graphics.FromImage(bmp))
                            {
                                foreach(var sign in rgsign)
                                {
                                    isign = IsignGet(rgsignAbc, sign);
                                    var signAbc = rgsignAbc[isign];

                                    g.DrawImage(bmpAbc,
                                        new Rectangle(
                                            sign.bound.minx,
                                            sign.bound.miny - (sign.bound.maxy - sign.bound.miny),
                                            sign.bound.maxx - sign.bound.minx,
                                            sign.bound.maxy - sign.bound.miny
                                            ),
                                        new Rectangle(
                                            signAbc.bound.minx,
                                            signAbc.bound.miny,
                                            signAbc.bound.maxx - signAbc.bound.minx,
                                            signAbc.bound.maxy - signAbc.bound.miny
                                            ), GraphicsUnit.Pixel);
                                    g.DrawRectangle(Pens.Red,
                                        sign.bound.minx,
                                        sign.bound.miny,
                                        sign.bound.maxx - sign.bound.minx,
                                        sign.bound.maxy - sign.bound.miny
                                        );
                                    g.DrawRectangle(Pens.Green,
                                        sign.bound.minx,
                                        sign.bound.miny - (sign.bound.maxy - sign.bound.miny),
                                        sign.bound.maxx - sign.bound.minx,
                                        sign.bound.maxy - sign.bound.miny
                                        );
                                }
                            }

                            bmp.Save(FpatGet(FpatOut,"Trouble"), ImageFormat.Png);
                        }
                    }
                }
            }

            using(Output)
            {
                WriteLine(sb.ToString());
            }
        }

        private int IsignGet(Sign[] rgsignAbc, Sign sign)
        {
            var dMin = double.MaxValue;
            var isignMin = -1;
            for(var isign=0;isign<rgsignAbc.Length;isign++)
            {
                var signAbc = rgsignAbc[isign];
                var d = sign.DGet(signAbc);
                if(d<dMin)
                {
                    dMin = d;
                    isignMin = isign;
                }
            }
            return isignMin;
        }

        private Sign[][] RgsignFromFile(string fpatIn, string fpat)
        {
            Info(string.Format("Processing: {0}", fpat));
            byte[,] rgcol;
            if(!File.Exists(FpatGet(fpat, "Grow")))
            {
                Info("Read and convert to bw");
                rgcol = RgcolRead(fpatIn, 192);

                Save(rgcol, FpatGet(fpat, "BW"));

                Info("Grow");
                rgcol = RgcolGrow(rgcol, 10);

                Save(rgcol, FpatGet(fpat, "Grow"));
            }
            else
            {
                Info("Reading Grow");
                var bmpw = new BitmapWrapper(FpatGet(fpat, "Grow"));
                var maxx = bmpw.Width;
                var maxy = bmpw.Height;
                rgcol = new byte[maxx,maxy];

                for(var y = 0; y < maxy; y++)
                {
                    for(var x = 0; x < maxx; x++)
                    {
                        rgcol[x, y] = bmpw[x, y];
                    }
                }
                bmpw = null;
            }
            Info("Reading bounds");
            var rgbound = Rgbound(rgcol).ToList();

            Info("Merging bounds");
            rgbound = RgboundMerge(rgbound, 0.4);

            Info("Creating lines");
            var rgrgbound = RgrgboundLine(rgbound, rgcol);

            Info("Creating signs");
            var rgrgsign = Rgrgsign(rgcol, rgrgbound, 3, 3);
            return rgrgsign;
        }

        private List<Bound>[] RgrgboundLine(List<Bound> rgbound, byte[,] rgcol)
        {
            var rgrgbound = new List<List<Bound>>();
            foreach(var bound in rgbound.OrderBy(bound => bound.minx))
            {
                var dMin = int.MaxValue;
                List<Bound> rgboundLineMin = null;
                foreach(var rgboundLine in rgrgbound)
                {
                    var boundLast = rgboundLine.Last();
                    
                    if(!(Math.Max(bound.miny,boundLast.miny) <Math.Min(bound.maxy,boundLast.maxy)))
                        continue;

                    var dy = Math.Abs(boundLast.PointMiddle.Y - bound.PointMiddle.Y);
                    var dx = Math.Max(0, boundLast.maxx - bound.minx);
                    var d = dx * dx + dy * dy;
                    
                    if(d >= dMin)
                        continue;
                    
                    dMin = d;
                    rgboundLineMin = rgboundLine;
                }
                if(rgboundLineMin==null)
                {
                    rgrgbound.Add(new List<Bound>{bound});
                }
                else
                {
                    rgboundLineMin.Add(bound);
                }
            }
            return rgrgbound.OrderBy(rgboundLine => rgboundLine.First().miny).ToArray();

        }

        private Sign[][] Rgrgsign(byte[,] rgcol, List<Bound>[] rgrgbound, int cx, int cy)
        {
            var xGet = new Func<Bound, int, int>((bound, ix) => bound.minx + ix * (bound.maxx - bound.minx+1) / cx );
            var yGet = new Func<Bound, int, int>((bound, iy) => bound.miny + iy * (bound.maxy - bound.miny+1) / cy );
            return rgrgbound.Select(rgbound => rgbound.Select(bound =>
            {
                var rgbri = new double[cx,cy];
                for(var ix=0;ix<cx;ix++)
                {
                    for(var iy = 0; iy < cy; iy++)
                    {
                        var cWhite = 0.0;
                        var cAll = 0.0;
                        for(var x=xGet(bound,ix);x<xGet(bound,ix+1);x++)
                        {
                            for(var y=yGet(bound,iy);y<yGet(bound,iy+1);y++)
                            {
                                cAll++;
                                if(rgcol[x,y]==255)
                                    cWhite++;
                            }
                        }
                        rgbri[ix, iy] = cWhite/cAll;
                    }
                }
                return new Sign {bound = bound, rgbri = rgbri};
            }).ToArray()).ToArray();
        }

        private List<Bound> RgboundMerge(List<Bound> rgbound, double pctMin)
        {
            var sizeMin = rgbound.Average(bound => bound.SizeGet())*pctMin;

            var fBad = new Func<Bound, bool>(bound => bound.SizeGet() < sizeMin);

            var rgboundMerged = rgbound.Where(bound => !fBad(bound)).ToList();

            foreach(var boundBad in rgbound.Where(fBad))
            {
                var dMin = int.MaxValue;
                Bound boundDMin = null;
                foreach(var bound in rgboundMerged)
                {
                    var d = bound.DGet(boundBad);
                    
                    if(d >= dMin)
                        continue;
                    
                    dMin = d;
                    boundDMin = bound;
                }
                Debug.Assert(boundDMin != null, "boundDMin != null");
                boundDMin.Add(boundBad);
            }

            return rgboundMerged;
        }

        private void Save(byte[,] rgcol, string fpat)
        {
            Info("Saving "+fpat);
            var maxx = rgcol.GetLength(0);
            var maxy = rgcol.GetLength(1);
            var bmpw=new BitmapWrapper(maxx,maxy,PixelFormat.Format24bppRgb);
            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    bmpw[x, y] = rgcol[x,y];
                }
            }
            bmpw.Save(fpat);
        }

        private string FpatGet(string fpat, string st)
        {
            return string.Format("{0}{1}.png", fpat.Substring(0,fpat.Length-3), st);
        }

        private IEnumerable<Bound> Rgbound(byte[,] rgcol)
        {
            var maxx = rgcol.GetLength(0);
            var maxy = rgcol.GetLength(1);

            var rgcolFlood = (byte[,]) rgcol.Clone();

            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    var colStart = rgcolFlood[x, y];
                    Debug.Assert(colStart == 0 || colStart == 255);

                    if(colStart != 255)
                        continue;

                    var pointStart = new Point(x, y);

                    rgcolFlood[x, y] = 0;
                    var bound = new Bound(pointStart);
                    for(var queuePoint = new Queue<Point>(new[] {pointStart}); queuePoint.Any();)
                    {
                        var point = queuePoint.Dequeue();
                        foreach(var pointNext in 
                            from dxy in new[]
                            {
                                new {dx = -1, dy = -1}, new {dx = 0, dy = -1}, new {dx = 1, dy = -1},
                                new {dx = -1, dy = 0}, new {dx = 1, dy = 0},
                                new {dx = -1, dy = 1}, new {dx = 0, dy = 1}, new {dx = 1, dy = 1},
                            }
                            select new Point(point.X + dxy.dx, point.Y + dxy.dy)
                            into pointNext
                            where pointNext.X >= 0 && pointNext.X < maxx
                            where pointNext.Y >= 0 && pointNext.Y < maxy
                            where rgcolFlood[pointNext.X, pointNext.Y] == 255
                            select pointNext)
                        {
                            rgcolFlood[pointNext.X, pointNext.Y] = 0;
                            bound.Add(pointNext);
                            queuePoint.Enqueue(pointNext);
                        }
                    }
                    yield return bound;
                }
            }

            Info("Asserting flood");
            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    Debug.Assert(rgcolFlood[x, y] == 0);
                }
            }
        }

        private static byte[,] RgcolGrow(byte[,] rgcol, int rGrow)
        {
            var maxx = rgcol.GetLength(0);
            var maxy = rgcol.GetLength(1);

            var rgpoint = new List<Point>();
            for(var y = -rGrow; y <= rGrow; y++)
            {
                for(var x = -rGrow; x <= rGrow; x++)
                {
                    if(x * x + y * y <= rGrow * rGrow)
                        rgpoint.Add(new Point(x, y));
                }
            }

            var rgcolGrow = new byte[maxx,maxy];
            for(var y = 0; y < maxy; y++)
            {
                if(y%100==0)
                    Console.WriteLine(y);
                for(var x = 0; x < maxx; x++)
                {
                    if(rgcol[x, y] != 255)
                        continue;

                    foreach(var point in rgpoint)
                    {
                        var xT = x + point.X;
                        var yT = y + point.Y;

                        if(xT < 0 || xT >= maxx)
                            continue;

                        if(yT < 0 || yT >= maxy)
                            continue;

                        rgcolGrow[xT, yT] = 255;
                    }
                }
            }
            return rgcolGrow;
        }

        private byte[,] RgcolRead(string fpatIn, int thres)
        {
            var bmpw = new BitmapWrapper(fpatIn);
            var maxx = bmpw.Width;
            var maxy = bmpw.Height;
            var rgcol = new byte[maxx,maxy];

            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    rgcol[x, y] = bmpw[x, y] < thres ? (byte) 255 : (byte) 0;
                }
            }
            return rgcol;
        }
    }
}
