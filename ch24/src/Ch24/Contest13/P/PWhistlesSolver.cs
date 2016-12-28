using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Ch24.Util;
using Cmn.Util;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Contest13.P
{
    public class PWhistlesSolver : Contest.Solver
    {
        public override void Solve()
        {

            var rgsample = Rgsample8bit(FpatIn);
            int length = rgsample.Length;

            var rgw = RgwFromRgsampleFFT(rgsample);

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                solwrt.Write("{0}", rgw.Skip(1).Where(a => a).Count());
            }

        }

        bool[] RgwFromRgsampleFFT(byte[] rgsample)
        {
            const int sizeblock = 44100;
            const int wmax = 220;

            var rgw = new bool[wmax];

            int length = rgsample.Length;
            double treshold = 1.0;

            for (int start = 0; length - start >= sizeblock; start += sizeblock)
            {
                var rgamp = RgampFromSample(rgsample, start, Math.Min(sizeblock, length - start));
                //if (start == 0)
                //{
                //    double avg50 = rgamp.Skip(50).Take(20).Sum() / 20.0;
                //    treshold = avg50 * 2;
                //}
                for (int iw = 1; iw < wmax; iw++)
                {
                    double aggr = rgamp[iw * 100];
                    if (!rgw[iw]) rgw[iw] = aggr > treshold;
                }
            }
            rgw[0] = false;
            return rgw;
        }

        bool[] RgwFromRgsample(byte[] samples)
        {
            const int wmax = 220;
            var rgw = new bool[wmax];
            
            int c = 0;
            int n = samples.Length;
            bool fPos = samples.FirstOrDefault(x => x != 0) > 0;

            for (int i = 0; i < n; i++)
            {
                // skip silence
                if (samples[i] == 128 && (i==0 || samples[i-1]==128))
                {
                    c = 1;
                    fPos = true;
                    continue;
                }

                if ((samples[i] >= 128) == fPos)
                {
                    // count
                    c++;
                }
                else
                {
                    // emit and reset counter
                    int f = (int)(22050 / c); // calculated frequency
                    int iw = f / 100;
                    rgw[iw] = true;
                    c = 1;
                    fPos = !fPos;
                }
            }
            //if (c > 1)
            //{
            //    var f = 22050.0 / c; // calculated frequency
            //    while (ifreq < n)
            //    {
            //        freq[ifreq++] = f;
            //    }

            //}
            //else if (c == 1)
            //{
            //    freq[n - 1] = freq[n - 2];
            //}
            return rgw;
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
