using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2013.R2.A
{
    internal class TicketSwappingSolver : GcjSolver
    {
        private const long mod = 1000002013;

        protected override IEnumerable<object> EnobjSolveCase()
        {
            long cStop;
            long cX;
            Fetch(out cStop, out cX);

            var prizeGet = new Func<BigInteger, BigInteger>(cTravel => cTravel * cStop - cTravel * (cTravel - 1) / 2);

            var mpIn = new Dictionary<long, Wrp<BigInteger>>();
            var mpOut = new Dictionary<long, Wrp<BigInteger>>();

            BigInteger sumAll=0;
            for(var i=0;i<cX;i++)
            {
                long iIn;
                long iOut;
                BigInteger c;
                Fetch(out iIn, out iOut, out c);

                mpIn.EnsureGet(iIn).V += c;
                mpOut.EnsureGet(iOut).V += c;

                sumAll += prizeGet(iOut - iIn) * c;
            }

            var llIn = new LinkedList<KeyValuePair<long,Wrp<BigInteger>>>(mpIn.OrderBy(kvp => kvp.Key).ToList());

            var nIn = llIn.First;

            BigInteger sumCheat = 0;
            foreach(var kvpOut in mpOut.OrderBy(kvp => kvp.Key))
            {
                var cOut = kvpOut.Value.V;
                var iOut = kvpOut.Key;

                for(;;)
                {
                    var nInNext = nIn.Next;
                    if(nInNext==null)
                        break;
                    if(nInNext.Value.Key > iOut)
                        break;
                    nIn = nInNext;
                }

                for(;;)
                {
                    var c = U.Min(nIn.Value.Value.V, cOut);
                    if(c>0)
                        sumCheat += prizeGet(iOut - nIn.Value.Key) * c;
                    cOut -= c;
                    nIn.Value.Value.V -= c;
                    if(cOut==0)
                        break;
                    var nInT = nIn;
                    nIn = nIn.Previous;
                    if(nIn==null)
                        break;
                    if(nInT.Value.Value.V != 0)
                        continue;
                    nIn.List.Remove(nInT);
                }
            }

            yield return BigInteger.Remainder((sumAll - sumCheat), mod);
        }
    }
}
