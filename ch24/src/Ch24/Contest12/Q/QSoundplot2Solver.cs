using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;

namespace Ch24.Contest12.Q
{
    public class QSoundplot2Solver : Solver
    {
        private short[] rgsampleY;
        private short[] rgsampleX;
        const int freqSample = 44100;

        const int window = 400;
     
        public override void Solve()
        {

            var rgrgsample = Wavu.Rgsample16bit(FpatIn, freqSample, 2);
            rgsampleX = rgrgsample[0];
            rgsampleY = rgrgsample[1];


            var pen = new Pen(Color.FromArgb(100, 255, 0, 0));
            int x0 = 0, y0 = 0;

            var rgpt = Rgpoint().ToArray();
            var xMax = rgpt.Select(pt => pt.X).Max();
            var xMin = rgpt.Select(pt => pt.X).Min();
            var yMax = rgpt.Select(pt => pt.Y).Max();
            var yMin = rgpt.Select(pt => pt.Y).Min();
            
            var bmp = new Bitmap(xMax-xMin, yMax-yMin);

            
            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var pt in Rgpoint())
                {
                    var x = pt.X - xMin;
                    var y = bmp.Height - (pt.Y - yMin);
                    
                    if(x<0 || x>= bmp.Width || y<0 || y>= bmp.Height)
                        continue;
            
                 
                    //bmp.SetPixel(x, y, pen.Color);
                    g.DrawLine(pen, x0, y0, x, y);
                    x0 = x;
                    y0 = y;
                }
            }
            
            bmp.Save(FpatOut);
        }

        public IEnumerable<Point> Rgpoint()
        {
            var rgfreq = Rgfreq().ToArray();
         

            var sumX = rgfreq.Take(window).Select(t => t.Item1).Sum();
            var sumY = rgfreq.Take(window).Select(t => t.Item1).Sum();

            for (int i = window; i < rgfreq.Length; i++)
            {
                int x = (int) sumX/window;
                int y = (int) sumY/window;
                yield return new Point(x, y);
                sumX = sumX - rgfreq[i - window].Item1 + rgfreq[i].Item1;
                sumY = sumY - rgfreq[i - window].Item2 + rgfreq[i].Item2;
            }
        } 

        public IEnumerable<Tuple<decimal,decimal>> Rgfreq()
        {

            var rgfreqX = MpFreqObsByTObsGet(rgsampleX).ToArray();
            var rgfreqY = MpFreqObsByTObsGet(rgsampleY).ToArray();
            for(int i=0;i<Math.Min(rgfreqX.Length, rgfreqY.Length);i++)
            {
                yield return new Tuple<decimal, decimal>(rgfreqX[i], rgfreqY[i]);
            }
         
        }



        //tobs -> freqobs mappinget számol a sample adatokból
        private IEnumerable<decimal> MpFreqObsByTObsGet(short[] sample)
        {
            var rgfreq = RgfreqGetRaw(sample).ToArray();
            return AvgFilter(rgfreq);
        }

        //kiátlagolja a nyers isample -> freq mapet
        IEnumerable<decimal> AvgFilter(decimal[] rgfreq)
        {
            var halfWindowWidth = 100;
            var citem = rgfreq.Length;
            var rgisampleAndFreqFiltered = new List<decimal>();

            decimal sumFreq = 0.0m;
            for (int i = 0; i < 2*halfWindowWidth;i++ )
            {
                sumFreq += rgfreq[i];
            }
            
            for (int i = 2*halfWindowWidth+1; i < citem; i++)
            {
                sumFreq += rgfreq[i];
                rgisampleAndFreqFiltered.Add(sumFreq / (2 * halfWindowWidth + 1));
                sumFreq -= rgfreq[i - 2*halfWindowWidth + 1];
            }
            return rgisampleAndFreqFiltered;
        }

        //két zero crossing közötti isample-hez frekvenciákat rendel
        private IEnumerable<decimal> RgfreqGetRaw(short[] sample)
        {
            var rgisampleZeroCrossing = RgisamplePeak(sample).ToArray();
            int izcPrev = rgisampleZeroCrossing[0];
            for (var i = 1; i < rgisampleZeroCrossing.Length; i++)
            {
                int izc = rgisampleZeroCrossing[i];

                for (int k = 0; k < izc - izcPrev;k++ )
                {
                    yield return freqSample / (decimal)(izc - izcPrev - k) / 2;
                }
                    
                izcPrev = izc;
            }
        }

        //visszaadja a zero crossing utáni következő isample-kből álló listát
        private IEnumerable<int> RgisampleZeroCrossing(short[] sample)
        {
            int signPrev = sample[0] > 0? 1 : -1;
            for (int i = 1; i < sample.Length; i++)
            {
                int sign = sample[i] > 0? 1 : -1;
                if (signPrev != sign)
                    yield return i;
                signPrev = sign;
            }
        }


        private IEnumerable<int> RgisamplePeak(short[] sample)
        {
            for (int i = 1; i < sample.Length-1; i++)
            {
                if(sample[i] >= sample[i-1] && sample[i]>sample[i+1])
                    yield return i;
            }
        }

    }
}
