using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Cmn.Util;

namespace Ch24.Contest13.D
{
    public class OctalCnc3Solver : Contest.Solver
    {
        private Bitmap[] rgbmpRefDigit;

        private Bitmap bmpTsto;
        private IEnumerable<Bitmap> ReadDigits(Bitmap bmpOrig)
        {

            var bmp = MakeGrayscale3(bmpOrig);

            bmp = new Invert().Apply(bmp);
            bmp = new Threshold(100).Apply(bmp);
            var bmpX = bmp;
            bmp = new Pixellate().Apply(bmp);
            bmp = new SobelEdgeDetector().Apply(bmp);



            //bmp.Save(Path.Combine(DpatOut, "x1.png"));

            bmp = AForge.Imaging.Image.Clone(bmp, PixelFormat.Format24bppRgb);
            var blobCounter = new BlobCounter();
            blobCounter.MinWidth = 120;
            blobCounter.MinHeight = 120;
            blobCounter.ProcessImage(bmp);
//            using (var g = Graphics.FromImage(bmp))
            bmpTsto = AForge.Imaging.Image.Clone(bmpOrig, PixelFormat.Format24bppRgb);

            //using(Graphics g = Graphics.FromImage(bmp))
            //foreach (var x in blobCounter.GetObjectsRectangles().ToList())
            //{
            //    g.DrawRectangle(Pens.Blue, x);
            //}
            //yield break;


            foreach (var x in Reorder(Graphics.FromImage(bmpTsto), blobCounter.GetObjectsRectangles().ToList()))
            {

                var bmpDigit = new Bitmap(x.Width, x.Height, PixelFormat.Format24bppRgb);
                Graphics.FromImage(bmpDigit).DrawImage(bmpX, 0, 0, x, GraphicsUnit.Pixel);
                yield return bmpDigit;
                //                    g.DrawRectangle(Pens.Blue, x);
            }

        }

     
        private IEnumerable<Rectangle> Reorder(Graphics g, List<Rectangle> rgrect)
        {
            var r0 = new RectangleF(0, 0, 1, 1);

            while (true)
            {
                var x = rgrect.Where(r => r.Y > r0.Bottom).OrderBy(r => ((r.X - r0.X) * (r.X - r0.X) + (r.Y - r0.Bottom) * (r.Y - r0.Bottom))).ToList();

                if (!x.Any())
                    break;

                var rectfFirstInRow = x.First();

                g.DrawRectangle(Pens.Red, rectfFirstInRow);
                g.DrawLine(Pens.Red, r0.Left, r0.Bottom, rectfFirstInRow.Left, rectfFirstInRow.Top);
                yield return rectfFirstInRow;
                rgrect.Remove(rectfFirstInRow);

                var rect = rectfFirstInRow;
                while(true)
                {
                    x = rgrect.Where(r => r.X > rect.Right).OrderBy(r => ((r.X - rect.Right) * (r.X - rect.Right) + (r.Y - rect.Top) * (r.Y - rect.Top))).ToList();

                    if(!x.Any())
                        break;

                    var rectNext = x.First();

                    if (rectNext.Top > rect.Bottom)
                        break; //ez már egy másik sor

                    g.DrawLine(Pens.Blue, rect.Right, rect.Top, rectNext.Left, rectNext.Top);
                    g.DrawRectangle(Pens.Blue, rectNext);

                    yield return rectNext;
                    rgrect.Remove(rectNext);
                    rect = rectNext;
                }

                r0 = rectfFirstInRow;
            }
        }

        class Fv
        {
            private readonly double[] rgf;

            public Fv (Bitmap bmp, int i)
            {
                var dx = bmp.Width/i;
                var dy = bmp.Height/i;
                rgf = new double[i*i];
                for(int ix=0;ix<i;ix++)
                {
                    for(int iy=0;iy<i;iy++)
                    {
                        float g = 0;
                        var d = 0;

                        for(int x = ix*dx;x<bmp.Width && x< (ix+1)*dx; x++)
                        for(int y = iy*dy;y<bmp.Height && y< (iy+1)*dy; y++)
                        {
                            d++;
                            g += bmp.GetPixel(x, y).R;
                        }

                        rgf[ix + iy*i] = g/d;
                    }
                }
            }


            public double D(Fv fv)
            {
                var d = 0.0;
                for (int i = 0; i < rgf.Length; i++)
                    d += (fv.rgf[i] - rgf[i])*(fv.rgf[i] - rgf[i]);
                return Math.Sqrt(d);
            }
        }

        public override void Solve()
        {
            const int cblock = 4;
            try
            {
                var bmpReference = (Bitmap) Bitmap.FromFile(Path.Combine(DpatIn, "reference_digits.png"));
                rgbmpRefDigit = ReadDigits(bmpReference).ToArray();

                rgbmpRefDigit[0].Save("x0.png");
                var rgfvRefDigit = rgbmpRefDigit.Select(bmp => new Fv(bmp, cblock)).ToArray();

                var W = rgbmpRefDigit.Max(b => b.Width);
                var H = rgbmpRefDigit.Max(b => b.Height);

                bmpTsto.Save(Path.Combine(DpatOut, "tsto_ref.png"));
                using (var solwrt = new Solwrt(FpatOut, FpatRefout))
                {
                    int j = 0;

                   
                   
                    foreach (var bmpDigit in ReadDigits((Bitmap) Bitmap.FromFile(FpatIn)))
                    {
                      //  bmpDigit.Save("x.png");
                        var fv = new Fv(bmpDigit, cblock);
                        j++;

                        var min = double.MaxValue;
                        var idigitMin = -1;

                        int idigit = 0;

                        var aMinta = (double)bmpDigit.Width / bmpDigit.Height;

                        foreach (var fvrefDigit in rgfvRefDigit)
                        {
                            var aRef  = (double)rgbmpRefDigit[idigit].Width / rgbmpRefDigit[idigit].Height;

                            if (Math.Abs(1 - (aMinta / aRef)) < 0.2)
                            {
                                var d = fv.D(fvrefDigit);
                                if (min > d)
                                {
                                    min = d;
                                    idigitMin = idigit;
                                }
                            }
                            idigit++;

                        }

                        Graphics.FromImage(bmpTsto).FillRectangle(Brushes.White, 0, 0, W, H);
                        Graphics.FromImage(bmpTsto).DrawImage(rgbmpRefDigit[idigitMin], 0, 0);
                        try
                        {
                            solwrt.Write(idigitMin);
                        }catch(Exception er)
                        {
                            Console.WriteLine(er);
                         //   throw;
                        }

                    }
                    solwrt.WriteLine("");
                }

            }
            finally
            {
                bmpTsto.Save(Path.Combine(DpatOut, "tsto.png"));
            }
        }


        public static Bitmap MakeGrayscale3(Bitmap oldbmp)
        {
            using (var ms = new MemoryStream())
            {
                oldbmp.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                return (Bitmap)Bitmap.FromStream(ms);
            }
        }
      

        private List<Rectangle> RgrectBobberCandidate(Bitmap imgBefore, Bitmap imgAfter)
        {
            var img = XXX(imgBefore, imgAfter);
            var blobCounter = new BlobCounter();
            blobCounter.ProcessImage(img);

            return blobCounter.GetObjectsRectangles().ToList();
        }

        private Bitmap XXX(Bitmap bmpBefore, Bitmap bmpAfter)
        {
            var filter = new Grayscale(0.2125, 0.7154, 0.0721);
            bmpBefore = filter.Apply(bmpBefore);
            bmpAfter = filter.Apply(bmpAfter);

            // create filters
            var differenceFilter = new Difference();
            IFilter thresholdFilter = new Threshold(15);
            // set backgroud frame as an overlay for difference filter
            differenceFilter.OverlayImage = bmpBefore;
            // apply the filters
            Bitmap tmp1 = differenceFilter.Apply(bmpAfter);
            Bitmap tmp2 = thresholdFilter.Apply(tmp1);
            IFilter erosionFilter = new Erosion();
            // apply the filter 
            Bitmap tmp3 = erosionFilter.Apply(tmp2);

            IFilter pixellateFilter = new Pixellate();
            // apply the filter
            Bitmap tmp4 = pixellateFilter.Apply(tmp3);

            return tmp4;
        }
    }
}
