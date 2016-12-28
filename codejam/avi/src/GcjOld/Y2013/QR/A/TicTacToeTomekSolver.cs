using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.QR.A
{
    internal class TicTacToeTomekSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var table = new char[4,4];

            for(var row=0;row<4;row++)
            {
                var line = Fetch<string>();
                foreach(var vich in line.Select((v,i)=>new{v,i}))
                {
                    table[vich.i, row] = vich.v;
                }
            }

            Fetch<string>();

            var fEnded = true;

            for(var y = 0; y < 4; y++)
            {
                var cx = 0;
                var ct = 0;
                var co = 0;
                for(var x=0;x<4;x++)
                {
                    switch(table[x, y])
                    {
                        case 'X':
                            cx++;
                            break;
                        case 'O':
                            co++;
                            break;
                        case 'T':
                            ct++;
                            break;
                        default:
                            fEnded = false;
                            break;
                    }
                }

                if(cx+ct==4)
                {
                    yield return "X won";
                    yield break;
                }

                if(co+ct==4)
                {
                    yield return "O won";
                    yield break;
                }
            }

            for(var x = 0; x < 4; x++)
            {
                var cx = 0;
                var ct = 0;
                var co = 0;

                for(var y=0;y<4;y++)
                {
                    switch(table[x, y])
                    {
                        case 'X':
                            cx++;
                            break;
                        case 'O':
                            co++;
                            break;
                        case 'T':
                            ct++;
                            break;
                    }
                }

                if(cx+ct==4)
                {
                    yield return "X won";
                    yield break;
                }

                if(co+ct==4)
                {
                    yield return "O won";
                    yield break;
                }
            }

            {
                var cx = 0;
                var ct = 0;
                var co = 0;
            
                for(var i = 0; i < 4; i++)
                {
                    switch(table[i, i])
                    {
                        case 'X':
                            cx++;
                            break;
                        case 'O':
                            co++;
                            break;
                        case 'T':
                            ct++;
                            break;
                    }

                }

                if(cx+ct==4)
                {
                    yield return "X won";
                    yield break;
                }

                if(co+ct==4)
                {
                    yield return "O won";
                    yield break;
                }
            }

            {
                var cx = 0;
                var ct = 0;
                var co = 0;
            
                for(var i = 0; i < 4; i++)
                {
                    switch(table[i,3-i])
                    {
                        case 'X':
                            cx++;
                            break;
                        case 'O':
                            co++;
                            break;
                        case 'T':
                            ct++;
                            break;
                    }

                }

                if(cx+ct==4)
                {
                    yield return "X won";
                    yield break;
                }

                if(co+ct==4)
                {
                    yield return "O won";
                    yield break;
                }
            }

            yield return fEnded ? "Draw" : "Game has not completed";
        }

    }
}
