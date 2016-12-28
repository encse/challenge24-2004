using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest13.D
{
    public class OctalCncSolver : Contest.Solver
    {
        class Sign
        {
            public Bound bound;

            public List<Bound> rgbound=new List<Bound>();
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
        }

        public override void Solve()
        {
            byte[,] rgcol;
            if(!File.Exists(FpatGet("Shrink")))
            {
                Info("Read and convert to bw");
                rgcol = RgcolRead(192);

                Save(rgcol, "BW");

                Info("Grow");
                rgcol = RgcolGrow(rgcol, 13);

                Save(rgcol, "Grow");

                Info("Shrink");
                rgcol = RgcolShrink(rgcol, 18);

                Save(rgcol, "Shrink");
            }
            else
            {
                Info("Reading Shrink");
                var bmpw = new BitmapWrapper(FpatGet("Shrink"));
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
            Info("Reading signs");
            var rgsign = Rgsign(rgcol).ToArray();
            rgcol = null;

            Info("Reading lines");
            var rgrgsign = Rgrgsign(rgsign, 20);

            Info("Decoding lines");
            using(Output)
            {
                WriteLine(string.Concat(Rgchar(rgrgsign)));
            }
        }

        private IEnumerable<int> Rgchar(List<List<Sign>> rgrgsign)
        {
            var rgrgrgdot = new[]
            {
                new[]
                {
                    new[] {0,0,0},
                    new[] {1,1,1},
                    new[] {0,0,0},
                },
                new[]
                {
                    new[] {1,0},
                    new[] {1,0},
                    new[] {1,0},
                },
                new[]
                {
                    new[] {1,0,1},
                    new[] {0,0,0},
                    new[] {1,0,0},
                },
                new[]
                {
                    new[] {0,0,1},
                    new[] {0,0,0},
                    new[] {1,0,1},
                },
                new[]
                {
                    new[] {0,1,0},
                    new[] {0,0,0},
                    new[] {1,0,1},
                },
                new[]
                {
                    new[] {1,0,1},
                    new[] {0,0,0},
                    new[] {0,1,0},
                },
                new[]
                {
                    new[] {1,0,1},
                    new[] {0,2,0},
                    new[] {0,0,1},
                },
                new[]
                {
                    new[] {1,0,0},
                    new[] {0,2,0},
                    new[] {1,0,1},
                },
            };

                foreach(var rgsignLine in rgrgsign.Select(rgsignT => rgsignT.OrderBy(sign => sign.bound.PointMiddle.X)))
                {
                    foreach(var sign in rgsignLine)
                    {
                        var bound = sign.bound;

                        int? isign = null;
                        for(var i = 0;i<rgrgrgdot.Length;i++)
                        {
                            var rgrgDot = rgrgrgdot[i];
                            var pointGet = new Func<int, int, Point>((x, y) =>
                            {
                                var ry = y - bound.miny;
                                var h = bound.maxy - bound.miny;

                                Debug.Assert(ry >= 0);
                                Debug.Assert(ry <= h);

                                var iy = rgrgDot.Length * ry / h;
                                Debug.Assert(iy >= 0);
                                Debug.Assert(iy < rgrgDot.Length);

                                var rgDot = rgrgDot[iy];

                                var rx = x - bound.minx;
                                var w = bound.maxx - bound.minx;

                                Debug.Assert(rx >= 0);
                                Debug.Assert(rx <= w);

                                var ix = rgDot.Length * rx / w;
                                Debug.Assert(ix >= 0);
                                Debug.Assert(ix < rgDot.Length);

                                return new Point(ix, iy);
                            });
                            
                            var hlmpointAllDot=new HashSet<Point>(rgrgDot
                                .SelectMany((rgDot, y) => rgDot.Select( (c, x) =>  new {x = x, y = y, c = c}))
                                .Where( q => q.c==1)
                                .Select( q => new Point(q.x,q.y)));

                            bool fOk = true; 
                            foreach(var boundDot in sign.rgbound)
                            {
                                var point = pointGet(boundDot.PointMiddle.X, boundDot.PointMiddle.Y);
                                
                                hlmpointAllDot.Remove(point);
                                if(rgrgDot[point.Y][point.X]==0)
                                {
                                    fOk = false;
                                }
                            }

                            if(fOk)
                                fOk = !hlmpointAllDot.Any();

                            if(fOk)
                            {
                                Debug.Assert(!isign.HasValue);
                                isign = i;
                            }
                        }

                        Debug.Assert(isign.HasValue);
                        yield return isign.Value;
                    }
                }
        }

        private void Save(byte[,] rgcol, string st)
        {
            Info("Saving "+st);
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
            bmpw.Save(FpatGet(st));
        }

        private string FpatGet(string st)
        {
            return string.Format("{0}{1}.png", FpatOut.Substring(0,FpatOut.Length-3), st);
        }

        private static List<List<Sign>> Rgrgsign(IEnumerable<Sign> rgsign, int maxd)
        {
            var rgrgsign = new List<List<Sign>>();
            Sign signLast = null;
            foreach(var sign in rgsign.OrderBy(sign => sign.bound.PointMiddle.Y))
            {
                if(signLast == null || Math.Abs(signLast.bound.PointMiddle.Y - sign.bound.PointMiddle.Y) > maxd)
                {
                    rgrgsign.Add(new List<Sign>());
                }

                rgrgsign.Last().Add(sign);

                signLast = sign;
            }
            return rgrgsign;
        }

        private IEnumerable<Sign> Rgsign(byte[,] rgcol)
        {
            var maxx = rgcol.GetLength(0);
            var maxy = rgcol.GetLength(1);

            var rgcolFlood = (byte[,]) rgcol.Clone();

            var flood = new Func<Point, Func<Point, bool>, Bound>((pointStart, fIn) =>
            {
                rgcolFlood[pointStart.X, pointStart.Y] = 0;
                var bound = new Bound(pointStart);
                for(var queuePont = new Queue<Point>(new[] {pointStart}); queuePont.Any();)
                {
                    var point = queuePont.Dequeue();
                    foreach(var dxy in new[]
                    {
                        new {dx = -1, dy = -1}, new {dx = 0, dy = -1}, new {dx = 1, dy = -1}, 
                        new {dx = -1, dy = 0},new {dx = 1, dy = 0},
                        new {dx = -1, dy = 1}, new {dx = 0, dy = 1}, new {dx = 1, dy = 1}, 
                    })
                    {
                        var pointNext = new Point(point.X + dxy.dx, point.Y + dxy.dy);

                        if(pointNext.X < 0 || pointNext.X >= maxx)
                            continue;

                        if(pointNext.Y < 0 || pointNext.Y >= maxy)
                            continue;

                        if(!fIn(pointNext))
                            continue;
                        
                        rgcolFlood[pointNext.X, pointNext.Y] = 0;
                        bound.Add(pointNext);
                        queuePont.Enqueue(pointNext);
                    }
                }
                return bound;
            });

            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    var colStart = rgcolFlood[x, y];
                    Debug.Assert(colStart == 0 || colStart == 255);

                    if(colStart != 255)
                        continue;
                    
                    var pointStart = new Point(x, y);
                    var sign = new Sign();

                    sign.bound = flood(pointStart, point =>
                    {
                        var col1 = rgcolFlood[point.X, point.Y];
                        
                        if(col1 == 255)
                            return true;

                        if(col1 == 128)
                            sign.rgbound.Add(flood(point, pointT => rgcolFlood[pointT.X, pointT.Y] == 128));
                        
                        return false;
                    });
                    
                    yield return sign;
                }
            }

            Info("Asserting flood");
            for(var y = 0; y < maxy; y++)
            {
                for(var x = 0; x < maxx; x++)
                {
                    Debug.Assert(rgcolFlood[x,y] == 0);
                }
            }
        }

        private static byte[,] RgcolShrink(byte[,] rgcol, int rShrink)
        {
            var maxx = rgcol.GetLength(0);
            var maxy = rgcol.GetLength(1);

            var rgpoint = new List<Point>();
            for(var y = -rShrink; y <= rShrink; y++)
            {
                for(var x = -rShrink; x <= rShrink; x++)
                {
                    if(x * x + y * y <= rShrink * rShrink)
                        rgpoint.Add(new Point(x, y));
                }
            }

            var rgcolShrink = new byte[maxx,maxy];
            for(var y = 0; y < maxy; y++)
            {
                if(y%100==0)
                    Console.WriteLine(y);
                for(var x = 0; x < maxx; x++)
                {
                    if(rgcol[x, y] != 255)
                        continue;

                    var fHasZero = false;
                    foreach(var point in rgpoint)
                    {
                        var xT = x + point.X;
                        var yT = y + point.Y;

                        if(xT < 0 || xT >= maxx)
                            continue;

                        if(yT < 0 || yT >= maxy)
                            continue;

                        if(rgcol[xT, yT] != 0)
                            continue;
                        
                        fHasZero=true;
                        break;
                    }

                    rgcolShrink[x, y] = fHasZero ? rgcol[x, y] : (byte)128;
                }
            }
            return rgcolShrink;
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

        private byte[,] RgcolRead(int thres)
        {
            var bmpw = new BitmapWrapper(FpatIn);
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
