using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest11.C
{
    class CGramophoneSolver : Solver
    {

        public override void Solve()
        {
            var bmp = (Bitmap) Bitmap.FromFile(FpatIn);

            Hd hd;
            switch(IdProblem)
            {
                case 1: hd = new Hd(bmp, 1915, 738,  1000, 1000, 11.9); break;
                case 2: hd = new Hd(bmp, 1013, 85,   1010, 1025, 11.8); break;
                case 3: hd = new Hd(bmp, 1002, 54,   1002, 1004, 11.9); break;
                case 4: hd = new Hd(bmp, 1000, 59,   33+1924/2, 44+1924/2, 11.9); break;
                case 5: hd = new Hd(bmp, 1000, 72,  49+1915/2, 59 +1915/2, 11.8); break;
                case 6: hd = new Hd(bmp, 964, 1952,  20+1903/2, 63+1903/2, 11.8); break;
                case 7: hd = new Hd(bmp, 1000, 52,    28+1917/2, 38 +1917/2, 11.8); break;
                case 8: hd = new Hd(bmp, 1000, 64,   31+1920 /2, 51+1920/2, 11.85); break;
                case 9: hd = new Hd(bmp, 80, 1014,   67+1908/2, 57+1908/2, 11.85); break;
                case 10: hd = new Hd(bmp, 1000, 38, 30+1910/2, 23+1910/2, 11.8); break;

                default:
                    throw new Exception();
            }
            Directory.CreateDirectory(DpatOut);

           // hd.Bmp().Save(FpatOut);
            using (var bw = new BinaryWriter(File.Create(FpatOut)))
                hd.Wav(bw);
         
            log.Info("done");
         
        }

        private class Hd
        {
            private readonly Bitmap bmp;
            private readonly int xCenter;
            private readonly int yCenter;
            private readonly double dbt; //distance between turnings
            private readonly Vector vct0;
            public Hd(Bitmap bmp, int xPin, int yPin, int xCenter, int yCenter, double dbt) 
            {

                this.bmp = bmp;
                this.xCenter = xCenter;
                this.yCenter = yCenter;
                this.dbt = dbt;

                //x' =  x- cx
                //x = x' + cx;

                //y'  = cy - y;
                //y = cy-y'
                this.vct0 =new Vector(xPin-xCenter, yCenter-yPin);
                    
            }
         
            Vector VctFromPolar(double phi, double r)
            {
                return new Vector(r*Math.Cos(phi), r*Math.Sin(phi));
            }

            
            public void Wav(BinaryWriter sw)
            {
                //phi = 60*2*Pi per sec
                //mintavétel: 18000hz
                //
                const int freq = 44000;

                var rgbyte = Go(120 * 2 * Math.PI / 60 / freq).Select(Height).ToArray();

                sw.Write(Encoding.ASCII.GetBytes("RIFF"));
                sw.Write(rgbyte.Length+44);
                sw.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                sw.Write(new byte[] {0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00,});
                sw.Write(freq);
                sw.Write(freq);
                sw.Write(new byte[] {0x01, 0x00, 0x08, 0x00});
                sw.Write(Encoding.ASCII.GetBytes("data"));
                sw.Write(rgbyte.Length);
                sw.Write(rgbyte);

                
            }

            IEnumerable<Vector> Go(double dphi)
            {
                var a = dbt / (2 * Math.PI);
                var phi0 = vct0.Angle() / 180 * Math.PI;
                var r0 = vct0.Length();
                var r = r0;
                yield return vct0;
                for (var phi = 0.0; r > 400; phi += dphi)
                { 
                    r = r0 - phi * a;
                    yield return VctFromPolar(phi0+phi, r);
                }
            }
            public Bitmap Bmp()
            {
                var bmpOut = new Bitmap(bmp);
               
                foreach (var vct in Go(0.001))
                    bmpOut.SetPixel((int)Math.Round(xCenter + vct.X), (int) Math.Round(yCenter - vct.Y), Color.White);
               
                return bmpOut;
            }

          
            private byte Height(Vector vct)
            {
                return bmp.GetPixel((int)Math.Round(xCenter + vct.X), (int)Math.Round(yCenter - vct.Y)).R;
            }
        }
    }
}
