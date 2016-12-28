using System;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2008.R2.B
{
    public class TriangleAreaSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {

            int n, m, a;
            pparser.Fetch(out n, out m, out a);

            return () =>
                       {
                           Console.Write(".");
                           return Solve(n, m, a);
                       };

        }


        private IEnumerable<object> Solve(int n, int m, int a)
        {
            int x0 = 0;
            int y0 = 0;
            for (int y1 = 0; y1 <= m; y1++)
            {
                for (int x1 = x1Min(n,m,a,y1); x1 <= n; x1++)
                {
                    //x2,y2=?
                    //a == Math.Abs(x1 * y2 - x2 * y1)
                    int x2, y2;
                    
                    //I. a == x1*y2-y1*x2;
                    //I. a == -x1*y2+y1*x2;

                    if(FXY(-y1, x1, a, n, m,out x2, out y2))
                    {
                        if (a != Math.Abs(x0 * (y1 - y2) + x1 * (y2 - y0) + x2 * (y0 - y1)))
                            throw new Exception("coki");
                        yield return "{0} {1} {2} {3} {4} {5}".StFormat(x0, y0, x1, y1, x2, y2);
                        
                        yield break;
                    }
                    else if(FXY(y1, -x1, a, n, m, out x2, out y2))
                    {
                        if (a != Math.Abs(x0 * (y1 - y2) + x1 * (y2 - y0) + x2 * (y0 - y1)))
                            throw new Exception("coki");
                        yield return "{0} {1} {2} {3} {4} {5}".StFormat(x0, y0, x1, y1, x2, y2);
                        yield break;
                    }

                    
                }    
            }
            
            yield return "IMPOSSIBLE";
        }

        private bool FXY(int a, int b, int d, int n, int m, out int x, out int y)
        {
            //d == a * x + b * y, 0<=x<=n, 0<=y<=m
            if(a==0 && b==0 )
            {
                x = y = 0;
                return false;
            }
            extended_gcd(a, b, out x, out y);
            var gcd = a*x + b*y;
            if (d % gcd != 0)
                return false;
            int x0 = x * d/gcd;
            int y0 = y * d/gcd;
            
            //x = (x0+kb), y = (y0-ka) mindegyike megoldás
            int kmin, kmax;

            //x0+ k*b >= 0
            // ha b > 0
            //    k >= -x0/b
            // ha b <= 0
            //    k <= -x0/b
            //x0 + k*b <= n
            // ha b > 0
            //   k <= (n-x0)/b
            // ha b <= 0
            //   k >= (n-x0)/b
            if(b<=0)
            {
                //   (n-x0)/b<= k <= -x0/b
                kmin = (n - x0)/b-1;
                kmax = -x0/b+1;
            }
            else
            {
                kmin = -x0 / b -1;
                kmax = (n - x0) / b +1;
            }

            for (int k = kmin; k <= kmax;k++ )
            {
                x = x0 + k*b;
                y = y0 - k*a;
                if(x>=0 && x<=n && y>=0&&y<=m)
                {
                    return true;
                }
            }
                
            return false;
        }

        private void extended_gcd(int a, int b, out int lastx, out int lasty)
        {
            int sgnA = a < 0 ? -1 : 1;
            int sgnB = b < 0 ? -1 : 1;

            a = Math.Abs(a);
            b = Math.Abs(b);
            var x = 0;
            var y = 1;
            lastx = 1;
            lasty = 0;
            while(b != 0)
            {
                int quotient = a / b;
                //(a, b) := (b, a mod b)
                int t = b;
                b = a % b;
                a = t;

                //(x, lastx) := (lastx - quotient*x, x)
                t = lastx - quotient*x;
                lastx = x;
                x = t;
                
                //(y, lasty) := (lasty - quotient*y, y)       
                t = lasty - quotient*y;
                lasty = y;
                y = t;
            }
            lastx *= sgnA;
            lasty *= sgnB;
        }

        private int x1Min(int n, int m, int a, int y)
        {
            if (y * m >= a)
                return y;

            for (int x = y; x < n;x++ )
            {
                if (y * x >= a)
                    return x;
            }

            return y;
        }

        private IEnumerable<object> Solve2(int n, int m, int a)
        {
            int x0 = 0;
            int y0 = 0;
            for (int x1 = 0; x1 <= n; x1++)
                for (int y1 = 0; y1 <= m; y1++)
                    for (int x2 = 0; x2 <= n; x2++)
                        for (int y2 = 0; y2 <= m; y2++)
                        {
                            if (a == Math.Abs(x0*(y1-y2) + x1*(y2-y0)+x2*(y0-y1)))
                            {
                                yield return "{0} {1} {2} {3} {4} {5}".StFormat(x0,y0,x1,y1,x2,y2);
                                yield break;
                            }             
                        }
            yield return "IMPOSSIBLE";
        }
    }
}
