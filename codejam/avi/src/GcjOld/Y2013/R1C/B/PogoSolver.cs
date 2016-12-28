using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1C.B
{
    class PogoSolver : GcjSolver
    {
        public class Board
        {
            private Dictionary<int, Dictionary<int,int>> mpmpvbyyByX = new Dictionary<int, Dictionary<int, int>>();

            public void Set(int x, int y, int v)
            {
                mpmpvbyyByX.EnsureGet(x)[y]=v;
            }

            public bool Contains(int x, int y)
            {
                var mpvByY = mpmpvbyyByX.GetOrDefault(x);
                return mpvByY != null && mpvByY.ContainsKey(y);
            }

            public int Get(int x, int y)
            {
                return mpmpvbyyByX[x][y];
            }

            public void Remove(int x, int y)
            {
                var mpvByY = mpmpvbyyByX.GetOrDefault(x);
                mpvByY.Remove(y);
                if(!mpvByY.Any())
                    mpmpvbyyByX.Remove(x);
            }
        }
        public class Pont
        {
            public int x;
            public int y;
            public int step;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var lim = 100;
            var cTarget = (2 * lim + 1);
            var c = 0;

            var rgpont = new Queue<Pont>();
            rgpont.Enqueue(new Pont{x=0,y=0,step=0});
            var board = new Board();
            board.Set(0,0,0);
            for(;;)
            {
                var pont = rgpont.Dequeue();

                var step = pont.step + 1;

                var dgAdd = new Action<int, int>((x, y) =>
                {
                    if(board.Contains(x, y))
                        return;
                    board.Set(x,y,step);
                    rgpont.Enqueue(new Pont{x=x,y=y,step=step});
                    if(Math.Abs(x) < lim && Math.Abs(y) < lim)
                        c++;
                });

                dgAdd(pont.x + step, pont.y);
                dgAdd(pont.x - step, pont.y);
                dgAdd(pont.x, pont.y + step);
                dgAdd(pont.x, pont.y - step);

                if(c == cTarget)
                    break;
            }

            for(var y=0;y<=lim;y++)
            {
                yield return Solwrt.NewLine;
                for(var x = 0; x <= lim; x++)
                    yield return string.Format("{0,5}", board.Get(x, y));
            }
            yield break;
        }

    }
}
