using System;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest14.P
{
    public class PFilmLogisticsSolver : Contest.Solver
    {
        public class Pont
        {
            public double lat;
            public double lng;
        }

        public double Rkm = 6371;

        public override void Solve()
        {
            var cpont = Fetch<int>();

            var rgpont = U.Eni(cpont).Select(_ =>
            {
                var rglatlong = Fetch<double[]>();
                return new Pont {lat = rglatlong[0], lng = rglatlong[1]};
            }).ToList();

            var dkmSum = 0.0;
            for(var i=0;i<rgpont.Count;i++)
            {
                var pont1 = rgpont[i];
                var pont2 = rgpont[(i + 1) % rgpont.Count];
                dkmSum += dkm(pont1, pont2);
            }

            using(Output)
            {
                NufDouble = "0.######";

                WriteLine(dkmSum);
            }
        }

        public double dkm(Pont pont1, Pont pont2)
        {
            var dlambda = Math.Abs(pont1.lng*Math.PI/180 - pont2.lng*Math.PI/180);
            var phi2 = pont2.lat*Math.PI/180;
            var phi1 = pont1.lat*Math.PI/180;
            var ca =Math.Abs(Math.Atan2(
                Math.Sqrt(
                    sq(Math.Cos(phi2)*Math.Sin(dlambda)) + 
                    sq(
                        Math.Cos(phi1)*Math.Sin(phi2) - 
                        Math.Sin(phi1)*Math.Cos(phi2)*Math.Cos(dlambda))
                ),(
                    Math.Sin(phi1)*Math.Sin(phi2) + 
                    Math.Cos(phi1)*Math.Cos(phi2)*Math.Cos(dlambda)
                )
            ));
            var ca2 = Math.Acos(Math.Sin(phi1) * Math.Sin(phi2) + Math.Cos(phi1) * Math.Cos(phi2) * Math.Cos(dlambda));
            return Rkm * ca2;
        }

        public double sq(double a)
        {
            return a * a;
        }
    }
}
