using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest15.C
{
    class CBoredomSolver : Solver
    {
        public override void Solve()
        {
            var ct = Fetch<int>();
            var eps = 0.00000001;

            using(Output)
            {
                for(var it = 0; it < ct; it++)
                {
                    Fetch<int>();
                    var rgxT = Fetch<double[]>().OrderBy(n => n).ToArray();
                    var rgyT = Fetch<double[]>().OrderBy(n => n).ToArray();

                    //if(mpcbyx.Any(kv => kv.Value > 2) || mpcbyy.Any(kv => kv.Value > 2))
                    //{
                    //    WriteLine("NO");
                    //    continue;
                    //}

                    if(rgxT.Count() < 3)
                    {
                        WriteLine("YES");
                        continue;
                    }

                    //if(mpcbyxT.Count < 3)
                    //{
                    //    if(
                    //        (mpcbyx.Count == 1 && mpcbyy.Count <= 2) ||
                    //        (mpcbyx.Count == 2 && mpcbyy.Count <= 2)
                    //        )
                    //    {
                    //        WriteLine("YES");
                    //        continue;
                    //    }
                    //    WriteLine("NO");
                    //    continue;
                    //}

                    Action<Dictionary<decimal, int>, decimal> rm = (mp, k) =>
                    {
                        var v = mp[k];
                        if(v > 1)
                            mp[k] = v - 1;
                        else
                            mp.Remove(k);
                    };

                    var x1 = rgxT.First();
                    if(rgxT.Last() - x1 <= eps)
                    {
                        var y1 = rgyT.First();
                        if(rgyT.Last() - y1 <= eps)
                        {
                            WriteLine("YES");
                            continue;
                        }
                        var y2 = rgyT.First(y => y - y1 > eps);
                        if (rgyT.Last() - y2 <= eps)
                        {
                            WriteLine("YES");
                            continue;
                        }
                        WriteLine("NO");
                        continue;
                    }

                    var x2 = rgxT.FirstOrDefault(x => x - x1 > eps);

                    if (rgxT.Last() - x2 <= eps)
                    {
                        var y1 = rgyT.First();
                        if (rgyT.Last() - y1 <= eps)
                        {
                            WriteLine("YES");
                            continue;
                        }
                        var y2 = rgyT.First(y => y - y1 > eps);
                        if (rgyT.Last() - y2 <= eps)
                        {
                            WriteLine("YES");
                            continue;
                        }
                        WriteLine("NO");
                        continue;
                    }

                    x2 = rgxT[rgxT.Count() / 2];
                    var x3 = rgxT.Last();

                    var fYes = false;
                    for(var i1 = 0; i1 < rgyT.Count(); i1++)
                    {
                        for(var i2 = 0; i2 < rgyT.Count(); i2++)
                        {
                            if(i1 == i2)
                                continue;

                            for(var i3 = 0; i3 < rgyT.Count(); i3++)
                            {
                                if(i2 == i3 || i1 == i3)
                                    continue;

                                var y1 = rgyT[i1];
                                var y2 = rgyT[i2];
                                var y3 = rgyT[i3];

                                if(Math.Abs(y1 - y2) < eps && Math.Abs(y2 - y3) < eps)
                                    continue;
                                var mr = (y2 - y1) / (x2 - x1);
                                var mt = (y3 - y2) / (x3 - x2);

                                if(Math.Abs(mr - mt) < eps)
                                    continue;

                                var xo = (mr * mt * (y3 - y1) + mr * (x2 + x3) - mt * (x1 + x2)) / (2 * (mr - mt));
                                double yo;
                                if(Math.Abs(mr) > eps)
                                    yo = -1 / mr * (xo - (x1 + x2) / 2) + (y1 + y2) / 2;
                                else
                                    yo = -1 / mt * (xo - (x2 + x3) / 2) + (y2 + y3) / 2;

                                var r2 = (x1 - xo) * (x1 - xo) + (y1 - yo) * (y1 - yo);


                                //Debug.Assert(Math.Abs((x2 - xo) * (x2 - xo) + (y2 - yo) * (y2 - yo) - r2) < eps * 100);
                                //Debug.Assert(Math.Abs((x3 - xo) * (x3 - xo) + (y3 - yo) * (y3 - yo) - r2) < eps * 100);

                                var fOk = true;

                                var enxx = rgxT.Select(x => (x - xo) * (x - xo)).OrderBy(x => x).ToArray();
                                var enyy = rgyT.Select(y => (y - yo) * (y - yo)).OrderBy(y => -y).ToArray();

                                foreach(var xy in enxx.Zip(enyy, (xx,yy) => new {xx, yy}))
                                {
                                    if(Math.Abs(xy.xx + xy.yy - r2) > eps)
                                    {
                                        fOk = false;
                                        break;
                                    }

                                }


                                if(fOk)
                                {
                                    fYes = true;
                                    goto L;
                                }
                            }
                        }
                    }
                L:

                    WriteLine(fYes ? "YES" : "NO");
                }
            }

            // Score = lines.Count;
            //using (Output)
            //{

            //}
        }

        private void FindCircle(decimal ax, decimal ay, decimal bx, decimal by, decimal cx, decimal cy,
            out decimal centerx, out decimal centery, out decimal radius2)
        {
            // Get the perpendicular bisector of (x1, y1) and (x2, y2).
            var x1 = (bx + ax) / 2;
            var y1 = (bx + ay) / 2;
            var dy1 = bx - ax;
            var dx1 = -(by - ay);

            // Get the perpendicular bisector of (x2, y2) and (x3, y3).
            var x2 = (cx + bx) / 2;
            var y2 = (cy + by) / 2;
            var dy2 = cx - bx;
            var dx2 = -(cy - by);

            // See where the lines intersect.
            bool lines_intersect, segments_intersect;
            decimal intersectionx, intersectiony;
            FindIntersection(
                x1, y1, x1 + dx1, y1 + dy1,
                x2, y2, x2 + dx2, y2 + dy2,
                out lines_intersect, out segments_intersect,
                out intersectionx, out intersectiony);
            if (!lines_intersect)
            {
                throw new Exception();
            }
            else
            {
                centerx = intersectionx;
                centery = intersectiony;
                var dx = centerx - ax;
                var dy = centerx - ay;
                radius2 = dx * dx + dy * dy;
            }
        }

        private void FindIntersection(
            decimal p1x, decimal p1y, decimal p2x, decimal p2y, decimal p3x, decimal p3y, decimal p4x, decimal p4y,
            out bool lines_intersect, out bool segments_intersect,
            out decimal intersectionx,
            out decimal intersectiony
            )
        {
            // Get the segments' parameters.
            decimal dx12 = p2x - p1x;
            decimal dy12 = p2y - p1y;
            decimal dx34 = p4x - p3x;
            decimal dy34 = p4y - p3y;

            // Solve for t1 and t2
            decimal denominator = (dy12 * dx34 - dx12 * dy34);

            if (denominator == 0)
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersectionx = 0;
                intersectiony = 0;
                return;
            }
            decimal t1 =
                ((p1x - p3x) * dy34 + (p3y - p1y) * dx34)
                    / denominator;
            lines_intersect = true;

            decimal t2 =
                ((p3x - p1x) * dy12 + (p1y - p3y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersectionx = p1x + dx12 * t1;
            intersectiony = p1y + dy12 * t1;

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

        }
    }
}
