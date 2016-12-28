using System;
using System.Collections.Generic;
using System.Drawing;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest10.R
{
    class ColumnsSolver : Solver
    {
        private readonly Random random = new Random();

        public override void Solve()
        {
            var bmp = new Bitmap(DpatIn + IdProblem + ".jpg");

            var hashMin = long.MaxValue;
            var rgxMin = RgxPick(bmp.Width);
            while (hashMin != 0)
            {
                var rgx = RgxRandomSwap(bmp.Width, (int[])rgxMin.Clone());
                var hash = Hash(bmp, rgx);
                if (hash < hashMin)
                {
                    log.InfoFormat("hash: {0}, rgx: {1}", hash, rgx.StJoin(" ", i => i.ToString()));

                    hashMin = hash;
                    rgxMin = rgx;
                    UpdateSolution(rgx);
                }
            }
        }

        private int[] RgxRandomSwap(int width, int[] rgx)
        {
            var cswap = random.Next(5)+1;
            for (var iswap = 0; iswap < cswap; iswap++)
            {
            lChoose:
                var ixNew = random.Next(5);
                var xNew = random.Next(width);

                for (var j = 0; j < 5; j++)
                    if (rgx[j] == xNew)
                        goto lChoose;

                rgx[ixNew] = xNew;
            }
            return rgx;

        }

        private void UpdateSolution(IEnumerable<int> rgx)
        {
           
            using(var solwrt = new Solwrt(DpatOut + IdProblem + ".out"))
                solwrt.Write(rgx.StJoin(" ", x => solwrt.SolfFromObject(x)));
        }

        private long Hash(Bitmap bmp, int[] rgx)
        {
            long sum = 0;
            for(int y=0;y<bmp.Height;y++)
            {
                int red1 = bmp.GetPixel(rgx[0], y).R;
                int green2 = bmp.GetPixel(rgx[1], y).G;
                int red3 = bmp.GetPixel(rgx[2], y).R;
                int green4 = bmp.GetPixel(rgx[3], y).G;
                int blue5 = bmp.GetPixel(rgx[4], y).B;
                sum += ((red1 - green2)*red3 + green4)*blue5;
            }
            return Math.Abs(sum/8);
        }


        int[] RgxPick(int width)
        {
            var rgx = new int[5];
            for(var i=0;i<5;i++)
            {
                lChoose:
                var xT = random.Next(width);
                
                for(var j=0;j<i;j++)
                    if(rgx[j] == xT)
                        goto lChoose;
                
                rgx[i] = xT;
            }
            return rgx;
        }

    }
}
