using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1B.C
{
    internal class TheBoredTravelingSalesmanSmallSolver : GcjSolver
    {
        private class Cy
        {
            public int i;
            public string code;
            public Dictionary<string, Cy> mpcyByCode=new Dictionary<string, Cy>(); 
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int ccy;
            int cline;
            Fetch(out ccy, out cline);

            var mpcyByicy = ccy.Eni().Select(icy => new Cy {i = icy, code = Fetch<string>()}).ToDictionary(cy => cy.i+1);

            foreach(var i in cline.Eni())
            {
                int icy1;
                int icy2;
                Fetch(out icy1, out icy2);

                mpcyByicy[icy1].mpcyByCode[mpcyByicy[icy2].code] = mpcyByicy[icy2];
                mpcyByicy[icy2].mpcyByCode[mpcyByicy[icy1].code] = mpcyByicy[icy1];
            }

            var rgcy = mpcyByicy.Values.OrderBy(cy => cy.code).ToList();

            Func<List<Cy>,List<Cy>, bool> x=null;
            x = (List<Cy> cyBack, List<Cy> rgcyLeft) =>
            {
                var cy = cyBack.Last();
                if(!rgcyLeft.Any())
                    return true;
                var cyNext = rgcyLeft.First();
                if(cy.mpcyByCode.ContainsKey(cyNext.code))
                    if(x(cyBack.Concat(new[]{cyNext}).ToList(), rgcyLeft.Skip(1).ToList()))
                        return true;

                if(cyBack.Count == 1)
                    return false;
                return x(cyBack.Take(cyBack.Count-1).ToList(), rgcyLeft);
            };
            x = x.ToCached();

            foreach (var ency in q(rgcy).Select(xx => xx.ToList()))
            {
                if(x(new List<Cy>{ency.First()}, ency.Skip(1).ToList() ))
                {
                    yield return ency.StJoin("", cyT => cyT.code);
                    yield break;
                }
            }

            //{
            //    var rgcyReturn = new List<Cy>();
            //    Cy cy = null;
            //    foreach(var cyNext in ency)
            //    {
            //        if(cy==null)
            //        {
            //            cy = cyNext;
            //        }
            //        else
            //        {
            //            for(;;)
            //            {
            //                if(cy.mpcyByCode.ContainsKey(cyNext.code))
            //                {
            //                    cy = cyNext;
            //                    rgcyReturn.Add(cy);
            //                    break;
            //                }
            //                else
            //                {
            //                    if(rgcyReturn.Any())
            //                    {
            //                        cy = rgcyReturn.Last();
            //                        rgcyReturn.RemoveAt(rgcyReturn.Count - 1);
            //                    }
            //                    else
            //                    {
            //                        cy = null;
            //                        break;
            //                    }
            //                }
            //            }
            //            if(cy == null)
            //                break;
            //        }
            //    }
            //    if(cy != null)
            //    {
            //        yield return ency.StJoin("", cyT => cyT.code);
            //        yield break;
            //    }
            //}

        }

        private IEnumerable<IEnumerable<Cy>> q(List<Cy> rgcy)
        {
            if(rgcy.Count==1)
            {
                yield return new[] {rgcy.Single()};
                yield break;
            }
            for(int icy = 0; icy < rgcy.Count; icy++)
            {
                var cy = rgcy[icy];
                var rgcy2 = new List<Cy>(rgcy);
                rgcy2.RemoveAt(icy);
                foreach(var ency in q(rgcy2))
                {
                    yield return new[] {cy}.Concat(ency);
                }
            }
        }

    }
}
