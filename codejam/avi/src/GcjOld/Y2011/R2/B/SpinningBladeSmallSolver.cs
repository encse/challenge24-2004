using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R2.B
{
    internal class SpinningBladeSmallSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int h;
            int w;
            int ___;
            Fetch(out h, out w, out ___);

            var sheet = new int[w,h];
            for(var y=0;y<h;y++)
            {
                var st = Fetch<string>();
                for(var x=0;x<w;x++)
                {
                    sheet[x, y] = int.Parse(st[x].ToString()) + 1;
                }
            }

            var xsum = new int[w,h,w+1];
            var ysum = new int[w,h,h+1];

            for(var y=0;y<h;y++)
            {
                for(var x = 0; x < w; x++)
                {
                    xsum[0, y, w] += sheet[x, y];
                    ysum[x, 0, h] += sheet[x, y];
                }
            }

            Func<int, int, int, int> xsumGet=null;
            xsumGet = (x, y, l) =>
            {
                var sum = xsum[x, y, l];
                if(sum>0)
                    return sum;

                if(x+l+1<=w)
                    sum = xsumGet(x, y, l + 1) - sheet[x + l, y];
                else
                    sum = xsumGet(x - 1, y, l + 1) - sheet[x - 1, y];

                xsum[x, y, l] = sum;
                return sum;
            };

            Func<int, int, int, int> ysumGet=null;
            ysumGet = (x, y, l) =>
            {
                var sum = ysum[x, y, l];
                if(sum > 0)
                    return sum;

                if(y + l + 1 <= h)
                    sum = ysumGet(x, y, l + 1) - sheet[x, y + l];
                else
                    sum = ysumGet(x, y - 1, l + 1) - sheet[x, y - 1];

                ysum[x, y, l] = sum;
                return sum;
            };

            for(var size=Math.Min(w,h);size>=3;size--)
            {
                for(var x=0;x<=w-size;x++)
                {
                    for(var y=0;y<=h-size;y++)
                    {
                        decimal xsumAll = 0;
                        decimal ysumAll = 0;
                        for(var d=0;d<size;d++)
                        {
                            if(size%2!=0&&d==size/2)
                                continue;
                            var fEdge = d == 0 || d + 1 == size;
                            decimal p = d - (decimal)(size - 1) / 2;
                            xsumAll += (fEdge ? xsumGet(x + 1, y + d, size - 2) : xsumGet(x, y + d, size))*p;
                            ysumAll += (fEdge ? ysumGet(x + d, y + 1, size - 2) : ysumGet(x + d, y, size))*p;
                        }

                        if(xsumAll == 0 && ysumAll == 0)
                        {
                            yield return size;
                            //yield return "(";
                            //yield return x;
                            //yield return y;
                            //yield return ")";
                            yield break;
                        }
                    }
                }
            }
            yield return "IMPOSSIBLE";
        }
    }
}
