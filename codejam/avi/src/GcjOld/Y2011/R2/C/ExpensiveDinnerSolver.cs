using System;
using System.Collections.Generic;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R2.C
{
    internal class ExpensiveDinnerSolver : GcjSolver
    {
        private List<decimal> rgprim=new List<decimal>();


        public ExpensiveDinnerSolver()
        {
            var rgf = new bool[1000001];
            for(var i=2;i<rgf.Length;i++)
            {
                if(rgf[i])
                    continue;
                rgprim.Add(i);
                for(var n=i;n<rgf.Length;n+=i)
                {
                    rgf[n] = true;
                }
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var n = Fetch<decimal>();

            //Info(string.Format("{0}:", n));
            if(n==1)
            {
                yield return 0;
                yield break;
            }

            var spread = 1;

            foreach(var prim in rgprim.TakeWhile(prim => prim*prim <= n))
            {
                var i = prim;
                decimal x = 1;
                for(;;)
                {
                    i *= prim;
                    if(i>n)
                        break;
                    x++;
                    spread++;
                }

                //Info(string.Format("{0} ^ {1}", prim, x));
            }

            yield return spread;
        }
    }
}
