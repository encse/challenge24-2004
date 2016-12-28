using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Util
{
    public class Wavu
    {

        public class Block<T>
        {
            public readonly int IsampleStart;
            public readonly T[] Rgsample;
            public int Length { get { return Rgsample.Length; } }

            public Block(int isampleStart, IEnumerable<T> rgsample)
            {
                IsampleStart = isampleStart;
                Rgsample = rgsample.ToArray();
            }
        }

        /// <summary>
        /// Blokkokra vágja a mintát, kivagdosva a csendes részeket
        /// Csendnek számít a 128-as érték d sugarú környezete
        /// Akkor tekinti úgy hogy elkezdődött egy csendes blokk, ha a sample legutóbbi L byte-ja csendes
        /// </summary>
        public static IEnumerable<Block<byte>> RgsampleBlock(byte[] rgsample, int d, int L)
        {
            var i = 0;
            while (i < rgsample.Length)
            {
                while (i < rgsample.Length)
                {
                    if (Math.Abs(rgsample[i] - 128) > d)
                        break;
                    i++;
                }

                if (i < rgsample.Length)
                {
                    var iWhistleStart = i;
                    var iWhistleEnd = -1;
                    
                    var cblockLength = 0;
                    while (i < rgsample.Length)
                    {
                        if (Math.Abs(rgsample[i] - 128) <= d)
                        {
                            cblockLength++;
                            
                            if (cblockLength > L)
                            {
                                iWhistleEnd = i - cblockLength;
                                i = iWhistleEnd+1;
                                break;
                            }
                        }
                        else
                            cblockLength = 0;
                        i++;
                    }

                    if (iWhistleEnd == -1)
                        iWhistleEnd = rgsample.Length - 1;

                    yield return new Block<byte>(iWhistleStart, rgsample.Skip(iWhistleStart - 1).Take(iWhistleEnd - iWhistleStart + 1));
                }

            }
        }


        /// <summary>
        /// Blokkokra vágja a mintát, kivagdosva a csendes részeket
        /// Csendnek számít a 128-as érték d sugarú környezete
        /// Akkor tekinti úgy hogy elkezdődött egy csendes blokk, ha a sample legutóbbi L byte-ja csendes
        /// </summary>
        public static IEnumerable<Block<short>> RgsampleBlock(short[] rgsample, int d, int L)
        {
            var i = 0;
            while (i < rgsample.Length)
            {
                while (i < rgsample.Length)
                {
                    if (Math.Abs(rgsample[i] - 0) > d)
                        break;
                    i++;
                }

                if (i < rgsample.Length)
                {
                    var iWhistleStart = i;
                    var iWhistleEnd = -1;

                    var cblockLength = 0;
                    while (i < rgsample.Length)
                    {
                        if (Math.Abs(rgsample[i] - 0) <= d)
                        {
                            cblockLength++;

                            if (cblockLength > L)
                            {
                                iWhistleEnd = i - cblockLength;
                                i = iWhistleEnd + 1;
                                break;
                            }
                        }
                        else
                            cblockLength = 0;
                        i++;
                    }

                    if (iWhistleEnd == -1)
                        iWhistleEnd = rgsample.Length - 1;
                    var x = new short[iWhistleEnd - iWhistleStart + 1];
                    Array.Copy(rgsample, iWhistleStart-1, x, 0, x.Length);
                    yield return new Block<short>(iWhistleStart, x);
                }

            }
        }

        public static double[] RgampFromSample(IList<byte> rgsample, int istart, int freqStep, int freqSample=44100)
        {
            return RgampFromSample(rgsample, istart, freqStep, b => (Complex) b, freqSample);
        }
        public static double[] RgampFromSample(IList<ushort> rgsample, int istart, int freqStep, int freqSample=44100)
        {
            return RgampFromSample(rgsample, istart, freqStep, b => (Complex)b, freqSample);
        }
        public static double[] RgampFromSample(IList<uint> rgsample, int istart, int freqStep, int freqSample = 44100)
        {
            return RgampFromSample(rgsample, istart, freqStep, b => (Complex)b, freqSample);
        }
        public static double[] RgampFromSample(IList<short> rgsample, int istart, int freqStep, int freqSample = 44100)
        {
            return RgampFromSample(rgsample, istart, freqStep, b => (Complex)b, freqSample);
        }
        public static double[] RgampFromSample(IList<int> rgsample, int istart, int freqStep, int freqSample = 44100)
        {
            return RgampFromSample(rgsample, istart, freqStep, b => (Complex)b, freqSample);
        }

        public static int DftBlockSize(int freqSample, int freqStep)
        {
            if (freqSample % freqStep != 0)
                throw new Exception("nem lehet ilyen felbontással transzformálni");
            
            return freqSample / freqStep;
        }

        /// <summary>
        /// frekvencia-amplitudó értékeket számol a mintából,
        /// a mintavételezést istart-nál kezdi
        /// a kimenő vektor i-edik eleme i * freqStep frekvenciához tartozó amplitudóértéket tartalmazza,
        /// a 0. elem a DC egyenáramhoz tartozik, ez pl 128 lehet akkor ha a minta 128 körül oszcillál
        /// </summary>
        public static double[] RgampFromSample<T>(IList<T> rgsample, int istart, int freqStep, Func<T, Complex> dgToComplex, int freqSample=44100)
        {

            var size = DftBlockSize(freqSample, freqStep);

            var rgcplxSample = new Complex[size];
            for (int i = 0; i < size; i++)
                rgcplxSample[i] = dgToComplex(rgsample[istart + i]);

            new DiscreteFourierTransform().BluesteinForward(rgcplxSample, FourierOptions.Matlab);

            //le kell normálni a magnitudót sizeblockkal, hogy az amplitudókat megkapjuk. (ez a FourierOptions.Matlab paraméter miatt van így)
            var rgamp = rgcplxSample.Select(d => d.Magnitude / size).ToArray();

            //csak az alsó felét adjuk vissza
            return rgamp.Take(rgamp.Length/2 - 1).ToArray();
        }

        /// <summary>
        /// 8 bites mono hangfájl beolvasására szolgál.
        /// </summary>
        public static byte[] Rgsample8bit(string fpat, int? ofreqSample = null)
        {
            var wave = new WavInFile(fpat);
            if (ofreqSample != null && wave.GetSampleRate() != ofreqSample) throw new Exception("nem " + ofreqSample.Value + "Hz-es");
            if (wave.GetNumChannels() != 1) throw new Exception("nem mono");
            if (wave.GetNumBits() != 8) throw new Exception("nem 8 bites");

            var n = wave.GetNumSamples();

            var rgsample = new byte[n];
            wave.Read(rgsample, n);
            Debug.Assert(wave.Eof());
            wave.Dispose();
            return rgsample;
        }

        public static short[][] Rgsample16bit(string fpat, int? ofreq=null, int? occhannel= null)
        {
            var wave = new WavInFile(fpat);

            if (wave.GetNumBits() != 16) throw new Exception("nem 16 bites");
            if (ofreq != null && wave.GetSampleRate() != ofreq) throw new Exception("nem " + ofreq + " frekvenciájú");
            if (occhannel != null && wave.GetNumChannels() != occhannel) throw new Exception("nem " + occhannel + " csatornás");
            
            var n = wave.GetNumSamples();
            var cchannel = wave.GetNumChannels();
            var rgsample = new short[n * cchannel];
            wave.Read(rgsample, rgsample.Length);
            Debug.Assert(wave.Eof());
            var rgrgsample = new short[cchannel][];
            for (int ichannel = 0; ichannel < cchannel;ichannel++ )
                rgrgsample[ichannel] = new short[n];
            
            for (var isample = 0; isample < n; isample++)
                for (int ichannel = 0; ichannel < cchannel; ichannel++)
                    rgrgsample[ichannel][isample] = rgsample[isample * cchannel + ichannel];

            wave.Dispose();
            return rgrgsample;
        }

        /// <summary>
        /// többcsatornás 32 bites wav file kirására szolgál
        /// </summary>
        public static void Wav(BinaryWriter sw, int freq, params int[][] rgrgsample)
        {
            var cChannel = (byte)rgrgsample.Length;
            var cLength = rgrgsample.Max(rgsample => rgsample.Length);
            const int cbytePerSample = 4;
            var cbyteTotalDataLength = cLength * cChannel * cbytePerSample; 

            sw.Write(Encoding.ASCII.GetBytes("RIFF"));
            sw.Write(cbyteTotalDataLength + 44);
            sw.Write(Encoding.ASCII.GetBytes("WAVE"));
            sw.Write(Encoding.ASCII.GetBytes("fmt "));
            sw.Write(16); //PCM
            sw.Write((short) 1); //nem tömörített
            sw.Write((short) cChannel);
            sw.Write(freq);
            sw.Write(freq * cChannel * cbytePerSample);
            sw.Write((short) (cChannel * cbytePerSample));
            sw.Write((short) (cbytePerSample * 8));
            sw.Write(Encoding.ASCII.GetBytes("data"));
            sw.Write(cbyteTotalDataLength);

            for (int i = 0; i < cLength; i++)
            {
                for (int ichannel = 0; ichannel < cChannel; ichannel++)
                {
                    var rgsample = rgrgsample[ichannel];
                    if (i < rgsample.Length)
                        sw.Write(rgsample[i]);
                    else
                        sw.Write(0);
                }
            }

        }
        /// <summary>
        /// többcsatornás 8 bites wav file kirására szolgál
        /// </summary>
        public static void Wav(BinaryWriter sw, int freq, params byte[][] rgrgsample)
        {
            var cChannel = (byte)rgrgsample.Length;
            var cLength = rgrgsample.Max(rgsample => rgsample.Length);

            var cbyteTotalDataLength = cLength * cChannel;

            sw.Write(Encoding.ASCII.GetBytes("RIFF"));
            sw.Write(cbyteTotalDataLength + 44);
            sw.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            sw.Write(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, cChannel, 0x00, });
            sw.Write(freq);
            sw.Write(freq);
            sw.Write(new byte[] { 0x01, 0x00, 0x08, 0x00 });
            sw.Write(Encoding.ASCII.GetBytes("data"));
            sw.Write(cbyteTotalDataLength);

            for (int i = 0; i < cLength; i++)
            {
                for (int ichannel = 0; ichannel < cChannel; ichannel++)
                {
                    var rgsample = rgrgsample[ichannel];
                    if (i < rgsample.Length)
                        sw.Write(rgsample[i]);
                    else
                        sw.Write(0);
                }
            }

        }
    }
}
