using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.R1A.A
{
    internal class RotateSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int n;
            int k;
            Fetch(out n, out k);
            var board = new char[n,n];

            for(var y = 0; y < n; y++)
            {
                var rgbr = Fetch<string>().Where(ch => ch != '.').ToArray();
                for(var x = 0; x < n; x++)
                {
                    var i = x - n + rgbr.Length;
                    board[x, y] = i < 0 ? '.' : rgbr[i];
                }
            }

            var fb = false;
            var fr = false;
            foreach(var d in new[]
            {
                new {x = 1, y = 0},
                new {x = 1, y = 1},
                new {x = 0, y = 1},
                new {x = -1, y = 1},
            })
            {
                for(var x = 0; x < n; x++)
                {
                    for(var y = 0; y < n; y++)
                    {
                        var br = board[x, y];
                        if(br=='.')
                            continue;
                        var fSame = true;
                        for(var i = 0; i < k; i++)
                        {
                            try
                            {
                                var brT = board[x + d.x * i, y + d.y * i];
                                if(br == brT)
                                    continue;
                            }
                            catch(IndexOutOfRangeException er)
                            {
                            }

                            fSame = false;
                            break;
                        }
                        if(!fSame)
                            continue;
                        if(br=='R')
                            fr = true;
                        if(br=='B')
                            fb = true;
                    }
                }
            }

            return  new object[] {
                fr ? fb ? "Both" : "Red" : 
                     fb ? "Blue" : "Neither"};
        }
    }
}
