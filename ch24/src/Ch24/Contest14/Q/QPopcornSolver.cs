using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Ch24.Util;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Contest14.Q
{
    public class QPopcornSolver : Contest.Solver
    {
        public override void Solve()
        {

            var rgsample = Rgsample8bit(FpatIn);

            int ctoneMax = 0;
            int isecMax = 0;
            double k = 2;
            for (int isec = 0; isec < rgsample.Length / 44100; isec++)
            {
                var rgw = RgwFromRgsampleFFT(rgsample, isec);
                var ctone = 0;
                for(int i=1; i<rgw.Length/2-1;i++)
                {
                    if(rgw[i]>k)
                        ctone++;
                }

                if(ctone > ctoneMax)
                {
                    isecMax = isec;
                    ctoneMax = ctone;
                    Console.WriteLine(isecMax + "sec " + ctoneMax);

                }
            }

            using(Output)
                WriteLine("{0} {1}", ctoneMax, isecMax);

        }

        double[] RgwFromRgsampleFFT(byte[] rgsample, int iSec)
        {
            
            const int sizeblock = 44100 / 100; //100 herzenkénti felbontás
            var start = iSec*44100;
            var length = rgsample.Length;
            if (length - start < sizeblock)
                return null;
            return RgampFromSample(rgsample, start, Math.Min(sizeblock, length - start));
        }

        

        private byte[] Rgsample8bit(string fpat)
        {
            var wave = new WavInFile(fpat);
            Debug.Assert(wave.GetSampleRate() == 44100);
            Debug.Assert(wave.GetNumChannels() == 1);
            Debug.Assert(wave.GetNumBits() == 8);

            var n = wave.GetNumSamples();

            var rgsample = new byte[n];
            wave.Read(rgsample, n);
            Debug.Assert(wave.Eof());
            wave.Dispose();
            return rgsample;
        }

        private double[] RgampFromSample(byte[] rgsample, int start, int size)
        {
            var rgcplxSample = rgsample.Skip(start).Take(size).Select(sample => (Complex)sample).ToArray();

            new DiscreteFourierTransform().BluesteinForward(rgcplxSample, FourierOptions.Matlab);

            //le kell normálni a magnitudót sizeblockkal, hogy az amplitudókat megkapjuk. (ez a FourierOptions.Matlab paraméter miatt van így)
            return rgcplxSample.Select(d => d.Magnitude / size).ToArray();
        }

    }
}
