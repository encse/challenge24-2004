using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest05.A
{
    public class AArchipelagoSolver : Solver
    {
        private static Color colIsland = Color.FromArgb(255, 0,0,0);
        private static Color colWater = Color.FromArgb(255, 255, 255, 255);

        public override void Solve()
        {
            var bmpSrc = (Bitmap)Image.FromFile(FpatIn.Replace(".in", ".png"));
            
            var mpc = DiscoverAll(bmpSrc);
            using(Output)
            {
                var r = mpc.OrderBy(kvp => kvp.Key);
                foreach (var kvp in r)
                    Solwrt.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }

        }


        private Dictionary<int, int> DiscoverAll(Bitmap bmp)
        {
            bmp = AForge.Imaging.Image.Clone(bmp, PixelFormat.Format24bppRgb);
            var mpc = new Dictionary<int, int>();
            
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y) == colIsland)
                    {
                        var area = 0;
                        AreaGetDiscover(bmp, x, y, ref area);
                        if (!mpc.ContainsKey(area))
                            mpc[area] = 1;
                        else
                            mpc[area]++;
                    }
                }
            }

            return mpc;
        }

        private void AreaGetDiscover(Bitmap bmp, int x, int y, ref int a)
        {
            var q =  new Queue<Tuple<int, int>>();
            q.Enqueue(new Tuple<int, int>(x, y));
            while(q.Any())
            {
                var k = q.Dequeue();
                x = k.Item1;
                y = k.Item2;

                if (bmp.GetPixel(x, y) == colIsland)
                {
                    a++;
                    bmp.SetPixel(x, y, colWater);
                    if (x > 0)
                        q.Enqueue(new Tuple<int, int>(x - 1, y));
                    if (x < bmp.Width - 1)
                        q.Enqueue(new Tuple<int, int>(x + 1, y));
                    if (y > 0)
                        q.Enqueue(new Tuple<int, int>(x, y - 1));
                    if (y < bmp.Height - 1)
                        q.Enqueue(new Tuple<int, int>(x, y + 1));
                }    
            }
            
        }

    }
}


