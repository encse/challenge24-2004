using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest12.A
{
    public class TrianglesSolver : Solver
    {
       
        public override void Solve()
        {
            log.Info("reading through bitmap");
            P[,] ps;
            int w;
            int h;
            using(var bitmap = new Bitmap(FpatIn))
            {
                w = bitmap.Width;
                h = bitmap.Height;
                ps=new P[w,h];
                for(var y = 0; y < h; y++)
                    for(var x = 0; x < w; x++)
                        if(bitmap.GetPixel(x, y).R==0)
                            ps[x, y] = new P();
            }
            log.Info("do the real thing");


            var pGet = new Func<int,int,P>((x, y) => x < 0 || x >= w || y < 0 || y >= h ? null : ps[x, y]);

            var cTri = 0;
            for(var y = 0; y < h; y++)
                for(var x = 0; x < w; x++)
                {
                    var p = pGet(x, y);
                    if(p==null)
                        continue;

                    foreach(var kline in Enum.GetValues(typeof(Kline)).Cast<Kline>())
                    {
                        int dx;
                        int dy;
                        switch(kline)
                        {
                            case Kline.J:
                                dx = -1;
                                dy = 0;
                                break;
                            case Kline.JL:
                                dx = -1;
                                dy = -1;
                                break;
                            case Kline.L:
                                dx = 0;
                                dy = -1;
                                break;
                            case Kline.BL:
                                dx = 1;
                                dy = -1;
                                break;
                            default:
                                throw new Exception();
                        }

                        var pPrev = pGet(x + dx, y + dy);
                        if(pPrev!=null)
                        {
                            var line = pPrev.LineByKline[(int) kline];
                            Debug.Assert(line!=null);
                            p.LineByKline[(int) kline] = line;
                        }
                        else if(pGet(x-dx,y-dy)!=null)
                        {
                            p.LineByKline[(int) kline]=new Line();
                        }
                    }

                    for(var i = 0; i < p.LineByKline.Length; i++)
                    {
                        var line1 = p.LineByKline[i];

                        if(line1==null||!line1.rgCrossed.Any())
                            continue;

                        for(var j = i+1; j < p.LineByKline.Length; j++)
                        {
                            var line2 = p.LineByKline[j];

                            if(line2==null||!line2.rgCrossed.Any())
                                continue;

                            cTri += line1.cCross(line2);
                        }
                    }

                    for(var i = 0; i < p.LineByKline.Length; i++)
                    {
                        var line1 = p.LineByKline[i];

                        if(line1==null)
                            continue;

                        for(var j = i+1; j < p.LineByKline.Length; j++)
                        {
                            var line2 = p.LineByKline[j];

                            if(line2==null)
                                continue;

                            line1.rgCrossed.Add(line2.I);
                            line2.rgCrossed.Add(line1.I);
                        }
                    }
                }

            log.Info("done");

            Console.WriteLine(cTri);

            using (Output)
                WriteLine(cTri);
        }

        private enum Kline{J,JL,L,BL}

        private class P
        {
            public Line[] LineByKline = new Line[Enum.GetValues(typeof(Kline)).Length];
        }

        private class Line
        {
            private static int Inext = 0;

            public readonly int I=Inext++;

            public readonly SortedSet<int> rgCrossed=new SortedSet<int>();

            public int cCross(Line line)
            {
                var c = 0;

                var en1 = rgCrossed.GetEnumerator();
                var en2 = line.rgCrossed.GetEnumerator();

                if(!en1.MoveNext())
                    return 0;
                if(!en2.MoveNext())
                    return 0;
                for(;;)
                {
                    if(en1.Current < en2.Current)
                    {
                        if(!en1.MoveNext())
                            return c;
                    }
                    else if(en1.Current > en2.Current)
                    {
                        if(!en2.MoveNext())
                            return c;
                    }
                    else
                    {
                        c++;
                        if(!en1.MoveNext())
                            return c;
                        if(!en2.MoveNext())
                            return c;
                    }
                }
            }
        }
    }

}
