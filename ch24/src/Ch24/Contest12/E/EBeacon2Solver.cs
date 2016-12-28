using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest12.E
{
    public class EBeacon2Solver : Contest.Solver
    {
        public override void Solve()
        {
            var pparserTowers = new Pparser(Path.Combine(DpatIn, "towers"));
            var rgtower = new List<Tower>();
            while(!pparserTowers.FEof())
                rgtower.Add(pparserTowers.Fetch<Tower>());

            var rgamp = RgampGet(Path.Combine(DpatIn, "E0.wav"));

            //kalibrálás
            var x0 = -179.4;
            var y0 = -95.3;
            foreach (var tower in rgtower)
            {
                //amp*c = r -> c = r/amp
                var amp = rgamp[tower.f];
                var r = Math.Sqrt((x0 - tower.x)*(x0 - tower.x) + (y0 - tower.y)*(y0 - tower.y));
                tower.c = amp*r;
            }

            rgamp = RgampGet(FpatIn);
            var rgs = rgtower.Select(tower => new Tuple<Tower, double>(tower, tower.c / rgamp[tower.f])).ToList();

            rgs = rgs.OrderBy(s => s.Item2).ToList();
            double x=double.NaN, y=double.NaN;
                
            for(int i=0;i<rgs.Count-2;i++)
            {
                Trilaterate(rgs[i].Item1.x, rgs[i].Item1.y, rgs[i].Item2,
                            rgs[i+1].Item1.x, rgs[i+1].Item1.y, rgs[i+1].Item2,
                            rgs[i+2].Item1.x, rgs[i+2].Item1.y, rgs[i+2].Item2,
                            out x, out y);
                if (!(double.IsInfinity(x) || double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(y)))
                    break;
            }
            
            using(Output)
            {
                NufDouble = "0.####";
                WriteLine("{0} {1}", x,y);
            }
            
        }

        private double[] RgampGet(string fpat)
        {
            var rgsample = Wavu.Rgsample16bit(fpat, 44100, 1)[0];

            double[] rgamp = null;
            int blockSize = Wavu.DftBlockSize(44100, 1);
            int cblock = 1; // rgsample.Length / blockSize;

            for (int iblock = 0; iblock < cblock; iblock++)
            {
                var rgamp0 = Wavu.RgampFromSample(rgsample, iblock*blockSize, 1);
                if (rgamp == null)
                    rgamp = rgamp0;
                else
                {
                    for (int i = 0; i < rgamp.Length; i++)
                        rgamp[i] += rgamp0[i];
                }
            }

            for (int i = 0; i < rgamp.Length; i++)
                rgamp[i] /= cblock;
            return rgamp;
        }

        /// <summary>
        /// három toronytól vett távolság alapján x,y meghatározása
        /// ezt nem akarod megérteni
        /// https://confluence.slac.stanford.edu/display/IEPM/TULIP+Algorithm+Alternative+Trilateration+Method
        /// </summary>
        private void Trilaterate(double x0, double y0, double r0, double x1, double y1, double r1, double x2, double y2, double r2, out double x, out double y)
        {

            x = (((r0 * r0 - r1 * r1) + (x1 * x1 - x0 * x0) + (y1 * y1 - y0 * y0)) * (2 * y2 - 2 * y1) - ((r1 * r1 - r2 * r2) + (x2 * x2 - x1 * x1) + (y2 * y2 - y1 * y1)) * (2 * y1 - 2 * y0)) /
                ((2 * x1 - 2 * x2) * (2 * y1 - 2 * y0) - (2 * x0 - 2 * x1) * (2 * y2 - 2 * y1));

            y = ((r0 * r0 - r1 * r1) + (x1 * x1 - x0 * x0) + (y1 * y1 - y0 * y0) + x * (2 * x0 - 2 * x1)) / (2 * y1 - 2 * y0);

        }

        class Tower
        {
            public double x, y;
            public int f;
            public double c;
            public Tower(double x, double y, int f)
            {
                this.x = x;
                this.y = y;
                this.f = f;
            }
        }
    }
}
