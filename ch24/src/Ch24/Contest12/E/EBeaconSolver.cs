using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Ch24.Util;
using Cmn.Util;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Contest12.E
{
    public class EBeaconSolver : Contest.Solver
    {
        public override void Solve()
        {
            var rgtower = RgtowerGet();
            var ctower = rgtower.Length;

            var rgsample = Rgsample(Path.Combine(DpatIn, "E0.wav"));
            Calibrate(rgsample, rgtower);

            rgsample = Rgsample(FpatIn);

            //az egyes tornyokb�l �rkez� jelek amplitud�i:
            var rgamp = RgampFromSampleAndRgtower(rgsample, rgtower);

            //a t�vols�gok a tornyokt�l, felhaszn�lva, hogy dist = c / amp
            var rgdist = new double[ctower];
            for (var itower = 0; itower < ctower; itower++)
                rgdist[itower] = rgtower[itower].C / rgamp[itower];

            //valami random ind�ttat�sb�l a koordin�t�t �gy keress�k, hogy el�bb a legk�zelebbi tornyokb�l pr�b�ljuk trilater�lni
            //azt�n ha ez nem siker�l, akkor megy�nk a t�volabbi tornyokra. tal�n �gy pontosabb, nem tudom csak �rzem.

            var rgitower = Seq.Ints(0, ctower).ToList();
            rgitower.Sort((itowerA, itowerB) => rgdist[itowerA].CompareTo(rgdist[itowerB]));

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                for (int itower = 0; itower < ctower - 2; itower++)
                {
                    var tower1 = rgtower[rgitower[itower]];
                    var tower2 = rgtower[rgitower[itower + 1]];
                    var tower3 = rgtower[rgitower[itower + 2]];
                    double x, y;

                    Trilaterate(
                        tower1.X, tower1.Y, rgdist[rgitower[itower]],
                        tower2.X, tower2.Y, rgdist[rgitower[itower + 1]],
                        tower3.X, tower3.Y, rgdist[rgitower[itower + 2]], out x, out y);
               
                    //ha k�t torony egym�s f�l�tt vagy mellett van, akkor itt mindenf�le cs�nya sz�mok j�nnek ki.
                    //keres�nk egy olyat, ahol a kimenet v�ges
                    if (!double.IsNaN(x) && !double.IsNaN(y) && !double.IsInfinity(x) && !double.IsInfinity(y))
                    {
                        solwrt.Write("{0} {1}", x, y);
                        Console.WriteLine("{0} {1}", x, y);
                        break;
                    }
                }
            }

        }

        private Tower[] RgtowerGet()
        {
            var pparserTowers = new Pparser(Path.Combine(Path.GetDirectoryName(FpatIn), "towers"));
            var rgtowerT = new List<Tower>();

            while (!pparserTowers.FEof())
                rgtowerT.Add(pparserTowers.Fetch<Tower>());
            return rgtowerT.ToArray();
        }

        private short[] Rgsample(string fpat)
        {
            var wave = new WavInFile(fpat);
            Debug.Assert(wave.GetSampleRate() == 44100);
            Debug.Assert(wave.GetNumChannels() == 1);
            Debug.Assert(wave.GetNumBits() == 16);

            var n = wave.GetNumSamples();

            var rgsample = new short[n];
            wave.Read(rgsample, n);
            Debug.Assert(wave.Eof());
            wave.Dispose();
            return rgsample;
        }

        private void Calibrate(short[] rgsample, Tower[] rgtower)
        {
            var rgamp = RgampFromSampleAndRgtower(rgsample, rgtower);
            var rgdist = RgdistFromRgtower(rgtower, -179.4, -95.3);

            for (int i = 0; i < rgtower.Length; i++)
                rgtower[i].C = rgamp[i] * rgdist[i];
        }

        private double[] RgdistFromRgtower(Tower[] rgtower, double x, double y)
        {
            var rgdist = new double[rgtower.Length];
            for (int i = 0; i < rgtower.Length; i++)
                rgdist[i] = Math.Sqrt(Math.Pow(rgtower[i].X - x, 2) + Math.Pow(rgtower[i].Y - y, 2));
            return rgdist;
        }

        private double[] RgampFromSampleAndRgtower(short[] rgsample, Tower[] rgtower)
        {
            //1 m�sodpercet vizsg�lunk
            //ez�rt 1hz -es ugr�sok lesznek a dft-ben
            const int sizeblock = 44100;
            var rgcplxSample = rgsample.Select(sample => (Complex)sample).Take(sizeblock).ToArray();

            new DiscreteFourierTransform().BluesteinForward(rgcplxSample, FourierOptions.Matlab);

            //le kell norm�lni a magnitud�t sizeblockkal, hogy az amplitud�kat megkapjuk. (ez a FourierOptions.Matlab param�ter miatt van �gy)
            var rgampAll = rgcplxSample.Select(d => d.Magnitude / sizeblock).ToArray();

            //az egyes tornyokb�l �rkez� jelek m�rt amplitud�ja (indexed by itower)
            var rgamp = new double[rgtower.Length];
            for (int i = 0; i < rgtower.Length; i++)
                rgamp[i] = rgampAll[rgtower[i].Freq];
            
            return rgamp;
        }

        /// <summary>
        /// h�rom toronyt�l vett t�vols�g alapj�n x,y meghat�roz�sa
        /// ezt nem akarod meg�rteni
        /// https://confluence.slac.stanford.edu/display/IEPM/TULIP+Algorithm+Alternative+Trilateration+Method
        /// </summary>
        private void Trilaterate(double x0, double y0, double r0, double x1, double y1, double r1, double x2, double y2, double r2, out double x, out double y)
        {

            x = (((r0 * r0 - r1 * r1) + (x1 * x1 - x0 * x0) + (y1 * y1 - y0 * y0)) * (2 * y2 - 2 * y1) - ((r1 * r1 - r2 * r2) + (x2 * x2 - x1 * x1) + (y2 * y2 - y1 * y1)) * (2 * y1 - 2 * y0)) /
                ((2 * x1 - 2 * x2) * (2 * y1 - 2 * y0) - (2 * x0 - 2 * x1) * (2 * y2 - 2 * y1));

            y = ((r0 * r0 - r1 * r1) + (x1 * x1 - x0 * x0) + (y1 * y1 - y0 * y0) + x * (2 * x0 - 2 * x1)) / (2 * y1 - 2 * y0);

        }


        class Tower
        {
            public readonly double X;
            public readonly double Y;
            public readonly int Freq;

            //a nagy okoss�g az, hogy dist = c / amp, azaz a t�vols�g ford�tottan ar�nyos a m�rt amplitud�val
            //a toronyra jellemz� C-t a kalibr�l�s sor�n sz�m�tjuk ki.
            public double C;

            public Tower(double x, double y, int freq)
            {
                X = x;
                Y = y;
                Freq = freq;
            }
        }
    }
}
