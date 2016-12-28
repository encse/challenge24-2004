using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.QR.C
{
    class MinesweeperMasterSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int yCount;
            int xCount;
            int cMine;
            Fetch(out yCount, out xCount, out cMine);

            yield return Solwrt.NewLine;

            var m = new char[xCount, yCount];
            foreach(var vxy in m.Envxy())
            {
                m[vxy.x, vxy.y] = '.';
            }
            m[0, 0] = 'c';

            var cEmpty = xCount * yCount - cMine;
            
            if(xCount==1)
            {
                for(var y = yCount - cMine; y < yCount; y++)
                    m[0, y] = '*';
            }
            else if(yCount==1)
            {
                for(var x = xCount - cMine; x < xCount; x++)
                    m[x, 0] = '*';
            }
            else if(cEmpty==1)
            {
                foreach(var vxy in m.Envxy())
                {
                    if(vxy.x==0&&vxy.y==0)
                        continue;
                    m[vxy.x, vxy.y] = '*';
                }
            }
            else if(cEmpty.FIn(2,3,5,7))
            {
                yield return "Impossible";
                yield break;
            }
            else if(xCount==2)
            {
                if(cMine%2!=0)
                {
                    yield return "Impossible";
                    yield break;
                }

                for (var y = yCount - cMine / 2; y < yCount; y++)
                {
                    m[0, y] = '*';
                    m[1, y] = '*';
                }
            }
            else if(yCount==2)
            {
                if(cMine%2!=0)
                {
                    yield return "Impossible";
                    yield break;
                }

                for (var x = xCount - cMine / 2; x < xCount; x++)
                {
                    m[x, 0] = '*';
                    m[x, 1] = '*';
                }
            }
            else
            {
                var cFullRow = cMine / xCount;

                if(cFullRow <= yCount - 4)
                {
                    var yFirstFull = yCount - cFullRow;
                    for(var y = yFirstFull;y<yCount;y++)
                    {
                        for(var x=0;x<xCount;x++)
                        {
                            m[x, y] = '*';
                        }
                    }

                    var cLeft = cMine - cFullRow * xCount;
                    if(xCount-cLeft==1)
                    {
                        cLeft--;
                        m[xCount - 1, yFirstFull - 2] = '*';
                    }
                    for(var x = xCount - cLeft; x < xCount; x++)
                        m[x, yFirstFull - 1] = '*';
                }
                else
                {
                    for (var y = 3; y < yCount; y++)
                    {
                        for (var x = 0; x < xCount; x++)
                        {
                            m[x, y] = '*';
                        }
                    }

                    var cLeft = cMine - (yCount - 3) * xCount;

                    var cFullCol = cLeft / 3;

                    var xFirstFull = xCount - cFullCol;
                    for (var x = xFirstFull; x < xCount; x++)
                    {
                        for (var y = 0; y < 3; y++)
                        {
                            m[x, y] = '*';
                        }
                    }

                    if(cLeft % 3 > 0)
                    {
                        m[xFirstFull - 1, 2] = '*';
                    }

                    if(cLeft % 3 > 1)
                    {
                        m[xFirstFull - 2, 2] = '*';
                    }
                }
            }

            Debug.Assert(m.Envxy().Count(vxy => vxy.v == '*') == cMine);
            Debug.Assert(Check((char[,]) m.Clone()));

            for(var y = 0;y<yCount;y++)
            {
                if(y > 0)
                    yield return Solwrt.NewLine;
                var st = "";
                for(var x = 0;x<xCount;x++)
                {
                    st += m[x, y];
                }
                yield return st;
            }
        }

        private bool Check(char[,] m)
        {
            
            for(var rgvxy =new List<U.Vxy<char>>{m.Envxy().Single(vxyT => vxyT.v == 'c')};rgvxy.Any();)
            {
                var vxy = rgvxy.First();
                rgvxy.RemoveAt(0);

                Debug.Assert(m[vxy.x,vxy.y]!='*');
                if(!"c.".Contains(m[vxy.x,vxy.y]))
                    continue;;

                var cMine = 0;
                for(var x = vxy.x - 1; x < vxy.x + 2; x++)
                {
                    if(0 > x || x >= m.XCount())
                        continue;
                    for(var y = vxy.y - 1; y < vxy.y + 2; y++)
                    {
                        if (0 > y || y >= m.YCount())
                            continue;

                        if(m[x, y] == '*')
                            cMine++;
                    }
                }

                m[vxy.x, vxy.y] = cMine.ToString()[0];

                if(cMine==0)
                {
                    for (var x = vxy.x - 1; x < vxy.x + 2; x++)
                    {
                        if (0 > x || x >= m.XCount())
                            continue;
                        for (var y = vxy.y - 1; y < vxy.y + 2; y++)
                        {
                            if (0 > y || y >= m.YCount())
                                continue;

                            rgvxy.Add(new U.Vxy<char>{x=x,y=y});
                        }
                    }

                }
            }

            var fOk = m.Envxy().All(vxy => vxy.v != '.');
            return fOk;
        }
    }
}
