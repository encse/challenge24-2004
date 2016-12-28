using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.R2.B
{
    class BSolver : GcjSolver
    {
        private double tolerance = 0.0001;

        private class Csap
        {
            public double rate;
            public double temp;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int n;
            double vTarget, tempTarget;
            Fetch(out n, out vTarget, out tempTarget);
            var rgcsapAll = n.Eni().Select(i =>
            {
                double rate, temp;
                Fetch(out rate, out temp);
                return new Csap {rate = rate, temp = temp};
            }).ToList();

            var rgcsapCold = rgcsapAll.Where(csap => csap.temp < tempTarget).ToList();
            var rgcsapGood = rgcsapAll.Where(csap => csap.temp == tempTarget).ToList();
            var rgcsapHot = rgcsapAll.Where(csap => csap.temp > tempTarget).ToList();

            if(Math.Min(rgcsapCold.Count, rgcsapHot.Count) == 0)
            {
                if(rgcsapGood.Count == 0)
                {
                    yield return "IMPOSSIBLE";
                    yield break;
                }

                yield return vTarget / union(rgcsapGood).rate;
                yield break;

            }

            var csapCold = union(rgcsapCold);
            var csapHot = union(rgcsapHot);

            var rateCold = -csapHot.rate * (csapHot.temp - tempTarget) / (csapCold.temp - tempTarget);
            var rateHot = -csapCold.rate * (csapCold.temp - tempTarget) / (csapHot.temp - tempTarget);

            if(rateCold <= csapCold.rate)
            {
                Debug.Assert(rateHot >= csapHot.rate);
                csapCold.rate = rateCold;
            }
            else
            {
                Debug.Assert(rateHot < csapHot.rate);
                csapHot.rate = rateHot;
            }

            var csapHC = union(new List<Csap> {csapHot, csapCold});
            Debug.Assert(Math.Abs(csapHC.temp - tempTarget) < TOLERANCE);

            var rateAll = csapHC.rate;
            if (rgcsapGood.Any())
            {
                var csapGood = union(rgcsapGood);
                //Debug.Assert(csapGood.temp == tempTarget);
                rateAll += csapGood.rate;
            }

            yield return vTarget / rateAll;
        }

        public double TOLERANCE
        {
            get
            {
                return tolerance;
            }
            set
            {
                tolerance = value;
            }
        }

        private static Csap union(List<Csap> rgcsapCold)
        {
            var rate = rgcsapCold.Sum(csap => csap.rate);
            return new Csap {rate = rate, temp = rgcsapCold.Sum(csap => csap.rate*csap.temp) / rate};
        }
    }
}
