using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest14.F
{
    public class FForensicsSolver : Contest.Solver
    {
        private const int freqSample = 44100;
        private const double cSound = (double) 343.2;

        public override void Solve()
        {
            byte[] sample = Wavu.Rgsample8bit(FpatIn, freqSample);
            var rStart = RStart(MpFreqObsByTObsGet(sample));
            /*
             * adott a tObs -> freqObs mapping néhány helyen: ekkor meg ekkor ilyen meg ilyen frekvenciát figyeltünk meg.
             * keressük r(tStart)-ot, amit persze nem lehet rendesen kiszámolni csak közelítõ algoritmussal.
             * 
             * a modellünk paraméterei: 
             *    v: sebesség (0..cSound m/s),
             *    rot: sebesség iránya (0..2Pi rad),  
             *    freqEmit: kibocsátott frekvencia (200..20000hz)
             *    r(0): távolság t=0-ban (0.001..2000 m)
             *
             * az alap képlet a doppler effektusból jön:
             *    freqObs(tObs) = cSound * freqEmit / (cSound + vs(tEmit))        (*)
             *    
             *    ahol vs a bogár sebességének origo irányú összetevõje. 
             *    
             * Az egyenlet bal és jobb oldalán másik idõpontok vannak, mert a kibocsátás (tEmit) és a megfigyelés (tObs) ideje nem esik egybe 
             * a hang terjedési sebessége miatt; a kettõt a TEmitFromTObs függvény kapcsolja össze, ami egy rém undorító másodfokú egyenlet a sebességgel meg r0-val
             * 
             * vs(t) meghatározható:
             *    vs(t) = <v,r(t)> / |r(t)|
             *    
             * alakban ahol:
             *    r(t) = (r + v_x*t, 0 + v_y*t); és v_x = v*cos(rot), v_y = v*sin(rot)  (**)
             * 
             * ha ez megvan akkor alglib lsfit-tel megkeressük azokat a modell paraméterket (v,rot, freqEmit, r(0)) ahol a számolt értékek [(*) jobb oldala]
             * és a mért értékek [(*) bal oldala] a legkevésbé térnek el egymástól.
             * 
             * ha sikerült megvan, akkor tObs = 0-hoz kiszámoljuk tStart-ot, és ebbõl meg a sebesség értékekbõl meghatározzuk r(tStart)-ot (**)-gal.
             * 
             * */
            using (Output)
            {
                Console.WriteLine(rStart);
                Solwrt.WriteLine(rStart);
            }
        }

        private const int icV = 0;
        private const int icRot = 1;
        private const int icR0 = 2;
        private const int icFreqEmit = 3;
        
        private const int ixTObs= 0;

        private double RStart(Dictionary<double, double> mpFreqObsByTObs)
        {
            const double diffstep = 0.0001;
            const int epsf = 0;
            const int epsx = 0;
            const int maxits = 1000000;
            
         
            // úgy hogy minimalizáljuk a 

            //kezdeti közelítés (a korlátokon belül)
            var cInit = CMake(v: 1, rot: 0, r0: 200, freqEmit: 4000);

            //alsó és felsõ korlátok a paraméterekre:
            var cLower = CMake(v: 0, rot: 0, r0: 0.001, freqEmit: 200);
            var cUpper = CMake(v: cSound - 2, rot: 2 * Math.PI, r0: 2000, freqEmit: 20000);

            //észlelések idõpontja
            var rgtObs = mpFreqObsByTObs.Keys.OrderBy(k => k).ToArray();
            //észlelt frekvenciák:
            var rgfreqObs = rgtObs.Select(t => mpFreqObsByTObs[t]).ToArray();
            
            var x = new double[mpFreqObsByTObs.Count, 1];
            for(int i = 0;i<rgtObs.Count();i++)
                x[i, ixTObs] = rgtObs[i];

            alglib.lsfitstate state;
            
            //csak az f(x|c)-t kell számolnunk
            alglib.lsfitcreatef(x, rgfreqObs, cInit, diffstep, out state);
            
            //constraintek
            alglib.lsfitsetbc(state, cLower, cUpper);
            
            //leállási feltétel
            alglib.lsfitsetcond(state, epsf, epsx, maxits);
            
            //számoljá
            alglib.lsfitfit(state, FobsComputeI, null, null);

            int info;
            double[] cSolution;
            alglib.lsfitreport rep;
            alglib.lsfitresults(state, out info, out cSolution, out rep);

            if (info != 2)
                throw new Exception("coki");

            var tStart = TEmitFromTObs(0, cSolution[icR0], cSolution[icV], cSolution[icRot]);
            return RFromT(cSolution[icV], cSolution[icRot], cSolution[icR0], tStart);
        }

        private double[] CMake(double v, double rot, double r0, double freqEmit)
        {
            var c = new double[4];
            c[icV] = v;
            c[icRot] = rot;
            c[icFreqEmit] = freqEmit;
            c[icR0] = r0;
            return c;
        }


        public void FobsComputeI(double[] c, double[] x, ref double fObs, object obj)
        {
            fObs = FObsCompute(c[icR0], c[icV], c[icRot], c[icFreqEmit], x[ixTObs]);
        }

        private double FObsCompute(double r0, double v, double rot, double freqEmit, double tObs)
        {
            var tEmit = TEmitFromTObs(tObs, r0, v, rot);
            var vs = VS(r0, v, rot, tEmit);

            /*  f_observed = (c+v_r) / (c+v_s) * f_emitted
             *
             *  v_r  is the velocity of the receiver relative to the medium; 
             *       positive if the receiver is moving towards the source (and negative in the other direction);
             *  v_s  the velocity of the source relative to the medium; 
             *       positive if the source is moving away from the receiver (and negative in the other direction).*/
            return cSound * freqEmit / (cSound + vs);
        }

        private double RFromT(double v, double rot, double r0, double t)
        {
            var vx = v * Math.Cos(rot);
            var vy = v * Math.Sin(rot);

            var rxt = r0 + vx*t;
            var ryt = 0 + vy*t;

            return Math.Sqrt(rxt*rxt + ryt*ryt);
        }
        
        private double TEmitFromTObs(double tObs, double r0, double v, double rot)
        {
            var vx = v*Math.Cos(rot);
            var vy = v*Math.Sin(rot);

            var A = v*v - cSound*cSound;
            var B = 2*(r0*vx + cSound*cSound*tObs);
            var C = r0*r0 - cSound*cSound*tObs*tObs;

            var tEmit = (-B + Math.Sqrt(B*B - 4*A*C))/(2*A);

            if (Math.Abs(tEmit - tObs + Math.Sqrt((r0 + vx * tEmit) * (r0 + vx * tEmit) + (0 + vy * tEmit) * (0 + vy * tEmit)) / cSound) < 0.000001)
                return tEmit;
           
            throw new Exception();

        }

        //v-nek az origo irányú sebessékkomponense t idõpillanatban
        private double VS(double r0, double v, double rot, double t)
        {
            var vx = v * Math.Cos(rot);
            var vy = v * Math.Sin(rot);

            //itt vagyunk t-ben
            var rxt = r0 + vx * t;
            var ryt = 0 + vy * t;

            // ha leosztjuk a <v,r> skaláris szorzatot |r|-rel akkor pont megkapjuk a keresett összetevõt
            var rt = (double)Math.Sqrt((double)(rxt * rxt + ryt * ryt));
            return (vx * rxt + vy * ryt) / rt;
        }

        //tobs -> freqobs mappinget számol a sample adatokból
        private Dictionary<double, double> MpFreqObsByTObsGet(byte[] sample)
        {
            var rgisampleAndFreq = RgisampleAndfreqGetRaw(sample).ToArray();
            return AvgFilter(rgisampleAndFreq).ToDictionary(isampleAndFreq => isampleAndFreq.Item1/freqSample, isampleAndFreq => isampleAndFreq.Item2);
        }

        //kiátlagolja a nyers isample -> freq mapet
        IEnumerable<Tuple<double, double>> AvgFilter(Tuple<double, double>[] rgisampleAndFreq)
        {
            var halfWindowWidth = 100;
            var citem = rgisampleAndFreq.Length;
            var rgisampleAndFreqFiltered = new List<Tuple<double, double>>();
            for (int i = halfWindowWidth; i < citem-halfWindowWidth; i++)
            {
                double sumFreq = 0.0;
                double sumISample = 0.0;
                for (int k = i - halfWindowWidth; k <= i + halfWindowWidth; k++)
                {
                    sumISample += rgisampleAndFreq[k].Item1;
                    sumFreq += rgisampleAndFreq[k].Item2;
                }
                rgisampleAndFreqFiltered.Add(new Tuple<double, double>(sumISample / (2 * halfWindowWidth + 1), sumFreq / (2 * halfWindowWidth + 1)));
            }
            return rgisampleAndFreqFiltered;
        }

        //két zero crossing közötti isample-hez frekvenciákat rendel
        private IEnumerable<Tuple<double, double>> RgisampleAndfreqGetRaw(byte[] sample)
        {
            var rgisampleZeroCrossing = RgisampleZeroCrossing(sample).ToArray();
            int izcPrev = rgisampleZeroCrossing[0];
            for(var i=1;i<rgisampleZeroCrossing.Length;i++)
            {
                int izc = rgisampleZeroCrossing[i];

                var freq = freqSample/(double) (izc - izcPrev) / 2;
                yield return new Tuple<double, double>((izcPrev + izc)/2.0, freq);
                izcPrev = izc;
            }
        }

        //visszaadja a zero crossing utáni következõ isample-kbõl álló listát
        private IEnumerable<int> RgisampleZeroCrossing(byte[] sample)
        {
            int signPrev = sample[0] >= 128 ? 1 : -1;
            for(int i=1; i<sample.Length;i++)
            {
                int sign = sample[i] >= 128 ? 1 : -1;
                if (signPrev != sign)
                    yield return i;
                signPrev = sign;
            }
        }
    }

}
