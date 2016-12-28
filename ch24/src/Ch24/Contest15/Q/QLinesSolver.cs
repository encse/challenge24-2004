using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest15.Q
{
    public class QLinesSolver : Contest.Solver
    {
        private class Line
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;
        }

        private class Q : IEquatable<Q>
        {
            public int x;
            public int y;
            public int l;

            public R mmin;
            public R mmax;

            public int ioct;

            public bool Equals(Q other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return x == other.x && y == other.y && l == other.l && Equals(mmin, other.mmin) && Equals(mmax, other.mmax) && ioct == other.ioct;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != this.GetType())
                    return false;
                return Equals((Q) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = x;
                    hashCode = (hashCode * 397) ^ y;
                    hashCode = (hashCode * 397) ^ l;
                    hashCode = (hashCode * 397) ^ (mmin != null ? mmin.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (mmax != null ? mmax.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ ioct;
                    return hashCode;
                }
            }

            public static bool operator ==(Q left, Q right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Q left, Q right)
            {
                return !Equals(left, right);
            }
        }

        private class R : IEquatable<R>
        {
            public int f;
            public int l;

            public bool lessThan(R r)
            {
                if(f * r.l == 0)
                    return true;
                return f * r.l < r.f * l;
            }

            public bool lessOrEqThan(R r)
            {
                if (f * r.l == 0)
                    return true;
                return f * r.l <= r.f * l;
            }

            public static R max(params R[] rgr)
            {
                var max = rgr.First();
                foreach(var r in rgr.Skip(1))
                {
                    if(max.lessThan(r))
                        max = r;
                }
                return max;
            }

            public static R min(params R[] rgr)
            {
                var min = rgr.First();
                foreach (var r in rgr.Skip(1))
                {
                    if (r.lessThan(min))
                        min = r;
                }
                return min;
            }

            public bool Equals(R other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return f == other.f && l == other.l;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != this.GetType())
                    return false;
                return Equals((R) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (f * 397) ^ l;
                }
            }

            public static bool operator ==(R left, R right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(R left, R right)
            {
                return !Equals(left, right);
            }
        }

        private enum Kc { White, Gray, Black}

        public override void Solve()
        {
            var img = Pngr.Load(FpatIn, pxl => pxl.rgba.r == 0 ? Kc.Black : Kc.White);

            var rgline = new List<Line>();
            foreach(var vxy in img.Envxy())
            {
                if(vxy.v != Kc.Black)
                    continue;
                var qbest = new Q { x = 0, y = 0, l = 0, mmin = new R { f = 0, l = 1 }, mmax = new R { f = 1, l = 1 } };

                for(var ioct = 0; ioct < 8; ioct++)
                {

                    Func<int, int, Kc> KcGet = (dxT, dyT) =>
                    {
                        int dx1;
                        int dy1;
                        conv(ioct, dxT, dyT, out dx1, out dy1);
                        var x = vxy.x + dx1;
                        var y = vxy.y + dy1;
                        if (img.FValidX(x) && img.FValidY(y))
                            return img[x, y];
                        return Kc.White;
                    };

                    var qu = new Queue<Q>();
                    var hlm = new HashSet<Q>();
                    qu.Enqueue(new Q {x = 0, y = 0, l = 0, mmin = new R {f = 0, l = 1}, mmax = new R {f = 1, l = 1}});

                    for(; qu.Any();)
                    {
                        var q = qu.Dequeue();

                        var rgqNext = new[]
                        {
                            new Q {x = q.x + 1, y = q.y, l = q.l},
                            new Q {x = q.x + 1, y = q.y + 1, l = q.l},
                        };

                        foreach(var qNext in rgqNext)
                        {
                            qNext.mmin = R.max(new R {f = 0, l = 1}, q.mmin, new R {f = qNext.y * 2 - 1, l = (2 * qNext.x)});
                            qNext.mmax = R.min(new R {f = 1, l = 1}, q.mmax, new R {f = qNext.y * 2 + 1, l = (2 * qNext.x)});
                            var kc = KcGet(qNext.x, qNext.y);
                            if (kc == Kc.Black)
                                qNext.l += 1;
                            if(kc != Kc.White && 
                                qNext.mmin.lessOrEqThan(qNext.mmax) &&
                                !hlm.Contains(qNext))
                            {
                                hlm.Add(qNext);
                                var r = new R {f = qNext.y, l = qNext.x};
                                if(qbest.l < qNext.l && qNext.mmin.lessThan(r) && r.lessOrEqThan(qNext.mmax))
                                {
                                    qbest = qNext;
                                    qbest.ioct = ioct;
                                }
                                qu.Enqueue(qNext);
                            }
                        }
                    }
                }



                int dx, dy;
                conv(qbest.ioct, qbest.x, qbest.y, out dx, out dy);

                var line = new Line
                {
                    x1 = vxy.x,
                    y1 = vxy.y,
                    x2 = vxy.x + dx,
                    y2 = vxy.y + dy,
                };
                rgline.Add(line);
                drawLine(line.x1, line.y1, line.x2, line.y2, (x, y) => img[x,y] = Kc.Gray);
            }

            using (Output)
            {
                foreach(var line in rgline)
                    WriteLine(new[]{line.x1, line.y1, line.x2, line.y2});
            }

            Score = rgline.Count;

            img = Pngr.Load(FpatIn, pxl => pxl.rgba.r == 0 ? Kc.Black : Kc.White);
            foreach(var line in rgline)
            {
                drawLine(line.x1, line.y1, line.x2, line.y2, (x, y) =>
                {
                    if(img[x,y]==Kc.White)
                        throw new Exception();
                    img[x, y] = Kc.Gray;
                });
            }
            
            if(img.Envxy().Any(vxy => vxy.v == Kc.Black))
                throw new Exception();
        }

        private static void conv(int ioct, int dxT, int dyT, out int dx, out int dy)
        {
            switch(ioct)
            {
                case 0:
                    dx = dxT;
                    dy = dyT;
                    break;
                case 1:
                    dx = dxT;
                    dy = -dyT;
                    break;
                case 2:
                    dx = -dxT;
                    dy = dyT;
                    break;
                case 3:
                    dx = -dxT;
                    dy = -dyT;
                    break;
                case 4:
                    dy = dxT;
                    dx = dyT;
                    break;
                case 5:
                    dy = dxT;
                    dx = -dyT;
                    break;
                case 6:
                    dy = -dxT;
                    dx = dyT;
                    break;
                case 7:
                    dy = -dxT;
                    dx = -dyT;
                    break;
                default:
                    throw new Exception();
            }
        }

        public void drawLine(int x2, int y2, int x, int y, Action<int, int> plot)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                plot(x, y);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        //private void drawLine(int x0, int y0, int x1, int y1, Action<int, int> plot)
        //{
        //    var dx = x1 - x0;
        //    var dy = y1 - y0;

        //    var D = 2 * dy - dx;
        //    plot(x0, y0);
        //    var y = y0;

        //    for(var x = x0 + 1; x <= x1; x++)
        //    {
        //        if(D > 0)
        //        {
        //            y = y + 1;
        //            plot(x, y);
        //            D = D + (2 * dy - 2 * dx);
        //        }
        //        else
        //        {
        //            plot(x, y);
        //            D = D + (2 * dy);
        //        }
        //    }
        //}
    }
}
