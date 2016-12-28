using System;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest14.J
{
    public class JImageCompressionSolver : Contest.Solver
    {

        public override void Solve()
        {
            var img = Pngr.Load(FpatIn, pxl => pxl.rgba.r);

            var c = img.XCount() + img.YCount();

            var hw = (int) Math.Sqrt(c);

            var dx = img.XCount() / hw;
            var dy = img.YCount() / hw;

            using(Output)
            {
                WriteLine(hw * hw);
                for(int x = 0; x < hw; x++)
                {
                    for(int y = 0; y < hw; y++)
                    {
                        int xx = x * dx + dx / 2;
                        int yy = y * dy + dy / 2;

                        WriteLine(new[] {xx, yy, img[xx, yy]});
                    }
                }
            }

        }
    }
}
