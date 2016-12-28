using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.R1A.B
{
    class BSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            BigInteger cB;
            BigInteger iMe;
            Fetch(out cB, out iMe);

            var rgb = Fetch<BigInteger[]>();

            Func<BigInteger, BigInteger> cStarted = timeT => rgb.Select(b => 1 + timeT / b).Sum();

            BigInteger timeHigh = 0;
            BigInteger step = 1;

            while(!(cStarted(timeHigh)>=iMe))
            {
                timeHigh += step;
                step *= 2;
            }

            step /= 2;
            BigInteger timeLow = timeHigh - step;
            while(true)
            {
                Debug.Assert(timeHigh >= timeLow);
                Debug.Assert(cStarted(timeHigh)>= iMe);
                Debug.Assert(timeLow == timeHigh || cStarted(timeLow) < iMe);
                if(timeHigh - timeLow <= 1)
                    break;

                BigInteger timeNext = (timeHigh + timeLow) / 2;

                if(cStarted(timeNext) < iMe)
                    timeLow = timeNext;
                else
                    timeHigh = timeNext;
            }

            yield return rgb.Select((b, i) => new {b, i}).Where(bi => timeHigh % bi.b == 0).Reverse().Skip((int) (cStarted(timeHigh) - iMe)).First().i + 1;
        }

    }
}
