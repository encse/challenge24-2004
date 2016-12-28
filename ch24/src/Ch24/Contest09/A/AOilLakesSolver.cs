using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;
using Cmn.Util;
using GMath = System.Math;

namespace Ch24.Contest09.A
{
    public class AOilLakesSolver : Solver
    {

        class Math
        {
            public static double PI =(double) GMath.PI;

            public static double Sqrt(double @double)
            {
                return (double) GMath.Sqrt((double) @double);
            }

            public static double Abs(double p0)
            {
                return (double)GMath.Abs((double)p0);
            }

            public static double Acos(double p0)
            {
                return (double)GMath.Acos((double)p0);
            }

            public static double Max(double width, double height)
            {
                return (double)GMath.Max(width, height);
            }

            public static double Log(double max)
            {
                return (double)GMath.Log((double)max);
            }

            public static double Min(double x0, double p1)
            {
                return (double)GMath.Min(x0, p1);
            }

            public static int Sign(double d2)
            {
                return GMath.Sign(d2);
            }
        }
        public static Bitmap bmp;
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int x0, y0;
            int x1, y1;
            pparser.Fetch(out x0, out y0);
            pparser.Fetch(out x1, out y1);
            var clake = pparser.Fetch<int>();

            int width = x1 - x0;
            int height = y1 - y0;
            double scale = 1;
            if (width > height)
            {
                if (width > 2000)
                    scale = 2000.0 / width;
            }
            else if (height > 2000)
                scale = 2000.0 / height;

            width = (int)(width * scale);
            height = (int)(height * scale);

            bmp = new Bitmap(width, height);

            //var sq1 = new Sq(new Rectd(0,0,100,100));
            //sq1.Add(new Lake1(50, 50, 20));
            //sq1.Add(new Lake1(50, 60, 20));
            Sq sq = new Sq(new Rectd(x0, y0, x1, y1));

            using (var g = Graphics.FromImage(bmp))
            {
               
                for (int ilake = 0; ilake < clake; ilake++)
                {
                    float x, y, r;
                    pparser.Fetch(out x, out y, out r);

                    sq.Add(new Lake1((double)x, (double)y, (double)r));

                }
                sq.RemoveEmpty();


                g.FillRectangle(Brushes.White, 0, 0, width, height);
                g.TranslateTransform(-x0, -y0);
                g.ScaleTransform((float)scale, (float)scale);
                sq.Tsto(g);
                bmp.Tsto();


                pparser = new Pparser(FpatIn.Replace(".in", ".refout"));
                pparser.Fetch(out x0, out y0);
                pparser.Fetch(out x1, out y1);
                Console.WriteLine(sq.CutFromLine(new Line(x0,y0,x1,y1)));

               var line = Do(sq, g);
               g.DrawLine(Pens.Red, (float)line.x0, (float)line.y0, (float)line.x1, (float)line.y1);

                Console.WriteLine("{0} {1}", line.x0, line.y0);
                Console.WriteLine("{0} {1}", line.x1, line.y1);
            }
            
            bmp.Tsto();
            
            
        }

        private Sq sq;

        private Line Do(Sq sq, Graphics g)
        {

            Cut cutMin = null;
            Line lineBest = null;
            LineBest(sq, sq.Rectd.x0, sq.Rectd.x1, (z1,z2) => new Line(z1, sq.Rectd.y0, z2, sq.Rectd.y1), ref cutMin, ref lineBest);
            LineBest(sq, sq.Rectd.y0, sq.Rectd.y1, (z1,z2) => new Line(sq.Rectd.x0, z1, sq.Rectd.x1, z2), ref cutMin, ref lineBest);


            Console.WriteLine();
            return lineBest;

        }

        private Line LineBest(Sq sq, double zMin, double zMax, Func<double, double, Line> lineFromZ, ref Cut cutMin, ref Line lineBest)
        {
            const int epsilon = 10;

            for (var z1 = zMin; z1 <= zMax; z1 += 1)
            {
                if (cutMin != null && cutMin.Through < 0.1 && Math.Abs(cutMin.Left - cutMin.Right) < 1)
                    break;

                Console.Write("\r" + z1*100/(sq.Rectd.x1 - sq.Rectd.x0));

                Func<double, double> diffFromZ = z =>
                                                     {
                                                         var cut = sq.CutFromLine(lineFromZ(z1, z));
                                                         return cut.Left - cut.Right;
                                                     };

                var zA = (int) zMin;
                var zB = (int) zMax;
                var diffA = diffFromZ(zA);
                var diffB = diffFromZ(zB);
                while (zB - zA > epsilon && Math.Sign(diffA) != Math.Sign(diffB))
                {
                    var z = (zB + zA)/2;
                    var diffZ = diffFromZ(z);
                    if (Math.Sign(diffA) == Math.Sign(diffZ))
                    {
                        zA = z;
                        diffA = diffZ;
                    }
                    else
                    {
                        zB = z;
                        diffB = diffZ;
                    }
                }

                for (var z = zA; z <= zB; z++)
                {
                    var line = lineFromZ(z1, z);
                    var cut = sq.CutFromLine(line);
                    if (cutMin == null || Better(cut, cutMin))
                    {
                        lineBest = line;
                        cutMin = cut;
                        Console.Write("\r\t\t                                                                                            ");
                        Console.Write("\r\t\t" + cutMin);
                    }


                    z++;
                }

                
            }
            return lineBest;
        }


        private bool Better(Cut cut, Cut cutMin)
        {
            return cut.Through*cut.Through + (cut.Left - cut.Right)*(cut.Left - cut.Right) < 
                cutMin.Through*cutMin.Through + (cutMin.Left - cutMin.Right)*(cutMin.Left - cutMin.Right);
        }



        class Rectd
        {
            public readonly double x0;
            public readonly double y0;
            public readonly double x1;
            public readonly double y1;

            public double width { get { return x1 - x0; } }
            public double height { get { return y1 - y0; } }
            public Rectd(double x0, double y0, double x1, double y1)
            {
                this.x0 = x0;
                this.y0 = y0;
                this.x1 = x1;
                this.y1 = y1;

                if (x0 > x1 || y1 < y0)
                    throw new Exception();
            }

            public bool Contains(Rectd rectd)
            {
                return x0 <= rectd.x0 && rectd.x1 <= x1 && y0 <= rectd.y0 && rectd.y1 <= y1;
            }
        }
        abstract class Region
        {
            public abstract double Oil();

            public abstract Rectd Rectd { get; }


            public abstract void Tsto(Graphics g);
        }

        class Cut
        {
            public double Left;
            public double Right;
            public double Through;

            public Cut(double left, double right, double through)
            {
                Left = left;
                Right = right;
                Through = through;
            }

            public static Cut operator+(Cut cutA, Cut cutB)
            {
                return new Cut(cutA.Left + cutB.Left, cutA.Right + cutB.Right, cutA.Through + cutB.Through);
            }
            public override string ToString()
            {
                return string.Format("{0}; {1}", Math.Abs(Left -Right), Through);
            }
        }

        class Line
        {
            public readonly double x0, y0, x1, y1, a, b, c,m;
            
            public Line(double x0, double y0, double x1, double y1)
            {
                this.x0 = x0;
                this.y0 = y0;
                this.x1 = x1;
                this.y1 = y1;
              

                //v2(x-x0) = v1(y-y0)
                var v2 = y1 - y0;
                var v1 = x1 - x0;
                 a = v2;
                 b = -v1;
                 c = -x0*v2 + y0*v1;
                 m = Math.Sqrt(a*a + b*b);
                a /= m;
                b /= m;
                c /= m;
            }

        
            public double D(double x, double y)
            {
                return a*x + b*y + c;
            }

            public void Tsto(Graphics g)
            {
                g.DrawLine(Pens.Red, (float) x0, (float) y0, (float) x1, (float) y1);
            }
        }

        class Lake2 : Lake
        {
            private Lake1 lake1;
            private Lake1 lake2;
            private double oil;
            private Rectd rectd;

            public Lake2(Lake1 lake1, Lake1 lake2)
            {
                throw new Exception();
                this.lake1 = lake1;
                this.lake2 = lake2;
                var dx = lake1.x - lake2.x;
                var dy = lake1.y - lake2.y;
                var d = Math.Sqrt(dx * dx + dy * dy);
                var r = lake1.r;
                var R = lake2.r;

                oil = 
                    r * r * Math.Acos((d * d + r * r - R * R) / (2 * d * r)) + 
                    R * R * Math.Acos((d * d + R * R - r * r) / (2 * d * r)) -
                    0.5 * Math.Sqrt((-d + r + R) * (d + r - R) * (d - r + R) * (d + r + R));

                rectd = new Rectd(
                        Math.Min(lake1.Rectd.x0, lake2.Rectd.x0), 
                        Math.Min(lake1.Rectd.y0, lake2.Rectd.y0),
                        Math.Max(lake1.Rectd.x1, lake2.Rectd.x1), 
                        Math.Max(lake1.Rectd.y1, lake2.Rectd.y1));
 
            }

            public override double Oil() { return oil; }

            public override Rectd Rectd { get { return rectd; } }

            public override Cut CutFromLine(Line line)
            {
                var cut1 = lake1.CutFromLine(line);
                var cut2 = lake2.CutFromLine(line);

                if (cut1.Through > 0 || cut2.Through > 0)
                    return new Cut(0, 0, cut1.Through + cut2.Through);
                if (cut1.Left > 0 && cut2.Left > 0)
                    return new Cut(oil, 0,0);
                if (cut1.Right> 0 && cut2.Right> 0)
                    return new Cut(0, oil, 0);
                throw new Exception("coki");
            }

            public override void Tsto(Graphics g)
            {
                lake1.Tsto(g);
                lake2.Tsto(g);
            }
        }

        class Ercut:Exception
        {
            public Lake lake;

            public Ercut(Lake lake)
            {
                this.lake = lake;
            }
        }

        class Lake1 : Lake
        {
            public double x, y, r;

            public Lake1(double x, double y, double r)
            {
                this.x = x;
                this.y = y;
                this.r = r;
            }

            public override double Oil()
            {
                return r * r * Math.PI;
            }

            public bool Intersects(Lake1 lake)
            {
                var dx = lake.x - x;
                var dy = lake.y - y;

                var d = Math.Sqrt(dx*dx + dy*dy);
                return d < Math.Min(r, lake.r);
            }

            public override Rectd Rectd
            {
                get { return new Rectd(x - r, y - r, x + r, y + r); }
            }

            public override bool FCuts(Line line)
            {
               var d = line.D(x, y);

                return (Math.Abs(d) < r);
            }

            public override Cut CutFromLine(Line line)
            {
                var d = line.D(x, y);

                if (Math.Abs(d) < r)
                {
                    var alpha = Math.Acos(Math.Abs(d) / r);
                    var tKorcikk = r * r * alpha;
                    var tHaromszog = Math.Sqrt(r * r - d * d) * Math.Abs(d);

                    var t1 = tKorcikk - tHaromszog;
                    var t2 = Oil() - t1;
                    if(d<0)
                        return new Cut(t1,t2,t1);
                    else
                        return new Cut(t2,t1,t1);
                }

                if (d < 0)
                    return new Cut(Oil(), 0, 0);

                return new Cut(0, Oil(), 0);
            }

            public override void Tsto(Graphics g)
            {
                var scale = g.Transform.Elements[0];
                r = Math.Max(2 * (double)scale, r);

                g.FillEllipse(Brushes.Blue, (float)(x-r), (float)(y-r), (float)(2*r), (float)(2*r));
            }
        }

        abstract class Lake : Region
        {
            public virtual bool FCuts(Line line)
            {
                throw new NotImplementedException();
            }

            public abstract Cut CutFromLine(Line line);
        }

        class Sq : Region
        {
            private Rectd rectd;
            private List<Sq> rgsq = new List<Sq>();
            private List<Lake> rglake = new List<Lake>();

            public Sq(Rectd rectd)
            {
                this.rectd = rectd;
            }

            private double? ooil;
            public override double Oil()
            {
                if (ooil == null)
                    ooil = rgsq.Sum(region => region.Oil()) + rglake.Sum(lake => lake.Oil());
                return ooil.Value;
            }

            public override Rectd Rectd { get { return rectd; } }

            private bool FIntersects(Line line)
            {
                var d1 = line.D(rectd.x0, rectd.y0);
                var d2 = line.D(rectd.x1, rectd.y0);
                var d3 = line.D(rectd.x0, rectd.y1);
                var d4 = line.D(rectd.x1, rectd.y1);
                if (Math.Sign(d1) == Math.Sign(d2) && Math.Sign(d2) == Math.Sign(d3) && Math.Sign(d3) == Math.Sign(d4) && Math.Abs(d1) > 0 && Math.Abs(d2) > 0 && Math.Abs(d3) > 0 && Math.Abs(d4) > 0)
                    return false;
                return true;
            }
            public Cut CutFromLine(Line line)
            {
                var d1 = line.D(rectd.x0, rectd.y0);
                var d2 = line.D(rectd.x1, rectd.y0);
                var d3 = line.D(rectd.x0, rectd.y1);
                var d4 = line.D(rectd.x1, rectd.y1);
                if (Math.Sign(d1) == Math.Sign(d2) && Math.Sign(d2) == Math.Sign(d3) && Math.Sign(d3) == Math.Sign(d4) && Math.Abs(d1) > 0 && Math.Abs(d2) > 0 && Math.Abs(d3) > 0 && Math.Abs(d4) > 0)
                    return d1 < 0 ? new Cut(Oil(), 0, 0) : new Cut(0, Oil(), 0);

                var cut = new Cut(0, 0, 0);
                foreach (var lake in rglake)
                    cut += lake.CutFromLine(line);

                foreach (var sq in rgsq.OrderBy(sq => !sq.FIntersects(line)))
                    cut += sq.CutFromLine(line);

               
                return cut;
            }

            public void RemoveEmpty()
            {
                
                foreach (var sq in rgsq.ToList())
                {
                    sq.RemoveEmpty();
                    if (sq.FEmpty())
                        rgsq.Remove(sq);
                }
            }

            private bool FEmpty()
            {
                return !rglake.Any() && rgsq.All(sq => sq.FEmpty());
            }

            public void Add(Lake1 lake)
            {
                if (!rectd.Contains(lake.Rectd))
                    throw new Exception("coki");

                if(!rgsq.Any())
                {
                    var dx = rectd.width/2;
                    var dy = rectd.height/2;
                    if(dx > 10 && dy > 10)
                        rgsq = new List<Sq>
                            {
                                new Sq(new Rectd(rectd.x0, rectd.y0, rectd.x0+dx, rectd.y0 + dy)),
                                new Sq(new Rectd(rectd.x0+dx, rectd.y0, rectd.x1, rectd.y0 + dy)),
                                new Sq(new Rectd(rectd.x0, rectd.y0+dy, rectd.x0+dx, rectd.y1)),
                                new Sq(new Rectd(rectd.x0+dx, rectd.y0+dy, rectd.x1, rectd.y1)),
                            };
                }
              
                foreach (var sq in rgsq)
                {
                    if (sq.Rectd.Contains(lake.Rectd))
                    {
                        sq.Add(lake);
                        return;
                    }
                }

           

                foreach (var lakeOld in rglake.Where(lakeT => lakeT is Lake1).Cast<Lake1>())
                {
                    if(lakeOld.Intersects(lake))
                    {
                        rglake.Remove(lakeOld);
                        rglake.Add(new Lake2(lakeOld, lake));
                        return;
                    }
                }

                rglake.Add(lake);
                    
            }

            public override void Tsto(Graphics g)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(1, 0, 255, 0)),
                    (float)rectd.x0, (float)rectd.y0, (float)rectd.width, (float)rectd.height);
                g.DrawRectangle(Pens.Green, (float) rectd.x0, (float) rectd.y0, (float) rectd.width, (float) rectd.height);


                foreach (var sq in rgsq)
                    sq.Tsto(g);

                foreach (var lake in rglake)
                    lake.Tsto(g);
            }
        }



    }
}


