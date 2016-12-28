using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest11.C
{
    public class CGramophone2Solver : Solver
    {

        public override void Solve()
        {
            byte[] rgbyte = null;
            if(FpatIn.EndsWith("1.png"))
                rgbyte = Solve(new Vector(1000, 1000), new Vector(1000, 48), new Vector(1000, 60), 50).ToArray();
            using (var sw = new BinaryWriter(File.Create(FpatOut)))
                Wavu.Wav(sw, 8000, rgbyte);
            
        }

        private IEnumerable<byte> Solve(Vector vectO, Vector vectA, Vector vectB, int rot)
        {
            vectA = vectA - vectO;
       //     vectA = new Vector(vectA.X, -vectA.Y);
            
            vectB = vectB - vectO;
        //    vectB = new Vector(vectB.X, -vectB.Y);

            var bmp = (Bitmap)Bitmap.FromFile(FpatIn);
            
            var t0 = 0.0;
            var dt = 1/8000.0; // 8000.0;

            var f = 120/60.0;
            var T = 1/f;
            var omega = 2*Math.PI*(120/60);
            var tLim = rot*2*Math.PI/omega;
            var t = t0;
            var l0 = vectA.Length();
            var dr = (vectA - vectB).Length();

            try
            {
                while (t < tLim)
                {
                    var phi = t*omega;
                    var l = l0 -t / T * dr;
                    var vectI = vectA.Rotate(-phi);
                    var vect = (l/vectI.Length())*vectI;
                    vect += vectO;
                    yield return bmp.GetPixel((int) vect.X, (int) vect.Y).R;
                    bmp.SetPixel((int) vect.X, (int) vect.Y, Color.White);

                    t += dt;
                }
                  bmp.Save(FpatIn + "tsto.png"); 
            }
            finally
            {
                                bmp.Save(FpatIn + "tsto.png"); 

            }

        }
    }
}
