using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest13.G
{
    public class TrainSolver : Contest.Solver
    {
        private double secBetweenTopForSameWheel = 0.018;//0.018;
        const double freq = 44100;
        const int secBetweenTrain = 5;
        const int csampleForWheelToDetect = 10;
            
    
        public override void Solve()
        {
            var rgwagon = AddReverse(RgtrainGet()).ToArray();

            //foreach (var train in rgwagon)
            //    Console.WriteLine(train.Tsto());

            var rgsample = Rgsample(FpatIn);
            int itrain = 0;
            using(var solwrt = new Solwrt(FpatOut, FpatRefout))
            foreach(var trainSample in EntrainSampleGet(rgsample))
            {
                itrain++;
                //Console.WriteLine("itrain: {0}", itrain);
                //Console.WriteLine(trainSample.Tsto());

                var train = RgwagonFromTrainSample(rgwagon, trainSample);
                var stTrain = train.StJoin(" ", w => w.Id.ToString());
                Console.WriteLine(stTrain);
                solwrt.WriteLine(stTrain);
            }

            Console.WriteLine();
        }

        private Wagon[] RgwagonFromTrainSample(Wagon[] rgwagon, TrainSample trainSample)
        {
            var distMin = double.PositiveInfinity;
            Wagon[] rgwagonDetectedBest = null;
            foreach (var wagonFirst in rgwagon)
            {
                var dt = trainSample.RgsecWheelDetected[1] - trainSample.RgsecWheelDetected[0];
                var l0 = wagonFirst.Rgwheel[1] - wagonFirst.Rgwheel[0];
                var speed = l0 / dt;

                double dist;
                var rgwagonDetected = RgwagonFromTrainSample(rgwagon, trainSample, wagonFirst, speed, out dist);
                if(dist<distMin)
                {
                    distMin = dist;
                    rgwagonDetectedBest = rgwagonDetected;
                }
            }

            return rgwagonDetectedBest;
        }


        private Wagon[] RgwagonFromTrainSample(Wagon[] rgwagon, TrainSample trainSample, Wagon wagonFirst, double speed, out double distSum)
        {
            distSum = 0;

            var rgwagonDetected = new List<Wagon> {wagonFirst};
            var cwheel = trainSample.RgsecWheelDetected.Length;

                    
            int iwheel = wagonFirst.Cwheel;
            double tEndOfPrevWagon = (wagonFirst.Length - wagonFirst.Rgwheel.Last()) / speed + trainSample.RgsecWheelDetected[wagonFirst.Cwheel - 1]; 


            for (; iwheel < cwheel; )
            {
                var distMin = double.PositiveInfinity;
                var tEndOfWagonDetected = double.NaN;
                Wagon wagonDetected = null;
                foreach (var wagon in rgwagon)
                {
                    if (iwheel + wagon.Cwheel > cwheel)
                        continue;

                    double tEndOfWagon;
                    var dist = Dist(trainSample, iwheel, wagon, speed, tEndOfPrevWagon, out tEndOfWagon);
                    if (dist < distMin)
                    {
                        wagonDetected = wagon;
                        distMin = dist;
                        tEndOfWagonDetected = tEndOfWagon;
                    }
                }

                distSum += distMin;
                if(wagonDetected == null)
                    break;
                rgwagonDetected.Add(wagonDetected);
                tEndOfPrevWagon = tEndOfWagonDetected;
                iwheel += wagonDetected.Cwheel;
            }
            if (iwheel != cwheel)
                distSum = double.PositiveInfinity;

            return rgwagonDetected.ToArray();
        }

        private double Dist(TrainSample trainSample, int isampleFirst, Wagon wagon, double speed, double tEndOfPrevWagon, out double tEndOfWagon)
        {
            var rgd = new List<double>();


            {
                var lbetweenWheels = wagon.Rgwheel[0];
                var lsample = speed * (trainSample.RgsecWheelDetected[isampleFirst] - tEndOfPrevWagon);
                rgd.Add(lbetweenWheels - lsample);
            }
            for(int i=0;i<wagon.Cwheel-1;i++)
            {
                var lbetweenWheels = wagon.Rgwheel[i + 1] - wagon.Rgwheel[i];
                var lsample = speed* (trainSample.RgsecWheelDetected[isampleFirst + i + 1] - trainSample.RgsecWheelDetected[isampleFirst + i]);
                rgd.Add(lbetweenWheels - lsample);
                
            }

            tEndOfWagon =  (wagon.Length-wagon.Rgwheel.Last()) / speed + trainSample.RgsecWheelDetected[isampleFirst+wagon.Cwheel-1]; 
            //if (isampleFirst + wagon.Cwheel < trainSample.RgsecWheelDetected.Length)
            //{
            //    //a köv kocsi elsõ kereke nem jöhet hamarabb mint a mi ütközõnk

            //    var lsampleToNextWagonFirstWheel = speed *
            //                                  (trainSample.RgsecWheelDetected[isampleFirst + wagon.Cwheel] -
            //                                   trainSample.RgsecWheelDetected[isampleFirst + wagon.Cwheel - 1]);

            //    var ltoEnd = wagon.Length - wagon.Rgwheel.Last();

            //    if (lsampleToNextWagonFirstWheel < ltoEnd)
            //        rgd.Add(10 * (ltoEnd - lsampleToNextWagonFirstWheel));
            //}

            return Math.Sqrt(rgd.Select(d => d*d).Sum()/rgd.Count);

        }

        IEnumerable<TrainSample> EntrainSampleGet(byte[] rgsample)
        {
            var rgsampleOrig = rgsample;
            var rgsampleLP = Lowpass(rgsample, 200);
            var rgsampleHP = Highpass(rgsampleLP, 200);
            var rgsampleAmp = AmpFilter(rgsampleHP);
            var rgsampleHuz = FilterHuz(rgsampleAmp);
            var rgsampleAvg = FilterAvg(rgsampleHuz, 30);
            var rgsampleKuszob = FilterKuszob(rgsampleAvg, 138);

            rgsample = Filter(rgsampleKuszob, 0.015, 0.014, 0);


            using(var bw = new BinaryWriter(new FileStream(FpatOut+".wav", FileMode.Create)))
            {
                Wav(bw, 
                    rgsampleOrig, 
                    rgsampleLP,
                    rgsampleHP,
                    rgsampleAmp,
                    rgsampleHuz,
                    rgsampleAvg,
                    rgsampleKuszob,

                    rgsample
                    );
            }

            var rgsecWheelInTrain = new List<double>();
            foreach (var secWheelSample in EnsecWheelSampleGet(rgsample))
            {
                if (rgsecWheelInTrain.Any() && secWheelSample - rgsecWheelInTrain.Last() > secBetweenTrain)
                {
                    //új vonat kezdõdik
                    yield return new TrainSample(rgsecWheelInTrain.ToArray());
                    rgsecWheelInTrain = new List<double>();
                }
                rgsecWheelInTrain.Add(secWheelSample);
            }

            //adjuk vissza az utolsó vonatot is
            if(rgsecWheelInTrain.Any())
                yield return new TrainSample(rgsecWheelInTrain.ToArray());
        }

        private byte[] FilterHuz(byte[] rgsample)
        {
            var max = rgsample.Max();
            var rgsampleResult = new byte[rgsample.Length];
            for (int i = 0; i < rgsample.Length; i++)
            {
                rgsampleResult[i] =(byte)(255 * (rgsample[i] - 128) / (max - 128));
            }
            return rgsampleResult;
        }


        public void Wav(BinaryWriter sw, params byte[][] rgrgsample)
        {
            const int freq = 44000;

            var cChannel = (byte)rgrgsample.Length;
            var cLength = rgrgsample.Max(rgsample => rgsample.Length);

            var cbyteTotalDataLength = cLength*cChannel;

            sw.Write(Encoding.ASCII.GetBytes("RIFF"));
            sw.Write(cbyteTotalDataLength + 44);
            sw.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            sw.Write(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, cChannel, 0x00, });
            sw.Write(freq);
            sw.Write(freq);
            sw.Write(new byte[] { 0x01, 0x00, 0x08, 0x00 });
            sw.Write(Encoding.ASCII.GetBytes("data"));
            sw.Write(cbyteTotalDataLength);

            for (int i = 0; i < cLength;i++ )
            {
                for(int ichannel = 0;ichannel<cChannel;ichannel++)
                {
                    var rgsample = rgrgsample[ichannel];
                    if (i < rgsample.Length)
                        sw.Write(rgsample[i]);
                    else
                        sw.Write(0);
                }
            }
                
        }

        private byte[] FilterKuszob(byte[] rgsample, int kuszob)
        {
            var rgsampleResult = new byte[rgsample.Length];
            for (int i = 0; i < rgsample.Length; i++)
            {
                rgsampleResult[i] = (byte) ( rgsample[i] >= kuszob
                                                ? 255
                                                : 0);
            }
            return rgsampleResult;
        }


        private byte[] AmpFilter(byte[] rgsample)
        {
            var rgsampleResult = new byte[rgsample.Length];
            var isample = 0;
            while (isample < rgsample.Length)
            {
                byte amp;
                var isampleNext = IsampleSignChange(rgsample, isample, out amp);
                for (; isample < isampleNext; isample++)
                    rgsampleResult[isample] = amp;
            }
            return rgsampleResult;
        }

        int IsampleSignChange(byte[] rgsample, int isample, out byte amp)
        {
            int ampT = Math.Abs(128 - rgsample[isample]);
            var fPosStart = rgsample[isample] > 128;
            while (isample < rgsample.Length)
            {
                var fpos = rgsample[isample] > 128;
                if (fpos != fPosStart)
                    break;

                ampT = Math.Max(ampT, Math.Abs(128 - rgsample[isample]));
                isample++;
            }
            amp = (byte)(ampT + 128);
            return isample;
        }


        byte[] Highpass(byte[] rgsample, int freqCutOff)
        {
            //el kell tolni, mert a byte[] 0-255-ig van, de valójában -128..127-ig reprezentál értékeket.
            var rgsampleShifted = rgsample.Select(sample => sample - 128).ToArray();

            var dt = 1 / freq;
            var RC = 1 / (2 * Math.PI * freqCutOff);
            var rgsampleResult = new double[rgsampleShifted.Length];
            var alpha = RC / (RC + dt);
            rgsampleResult[0] = rgsampleShifted[0];
            for (int i = 1; i < rgsampleShifted.Length; i++)
                rgsampleResult[i] = alpha * (rgsampleResult[i - 1] + rgsampleShifted[i] - rgsampleShifted[i - 1]);


            //aztán vissza kell tolni
            return rgsampleResult.Select(sample => (byte)(sample + 128)).ToArray();
        }

        byte[] Lowpass(byte[] rgsample, int freqCutOff)
        {
            //el kell tolni, mert a byte[] 0-255-ig van, de valójában -128..127-ig reprezentál értékeket.
            var rgsampleShifted = rgsample.Select(sample => sample - 128).ToArray();


            var dt = 1 / freq;
            var RC = 1 / (2 * Math.PI * freqCutOff);
            var rgsampleResult = new double[rgsampleShifted.Length];
            var alpha = dt / (RC + dt);
            rgsampleResult[0] = rgsampleShifted[0];
            for (int i = 1; i < rgsampleShifted.Length; i++)
                rgsampleResult[i] = (1 - alpha) * rgsampleResult[i - 1] + alpha * rgsampleShifted[i];

            //aztán vissza kell tolni
            return rgsampleResult.Select(sample => (byte)(sample + 128)).ToArray();
        }

     

        private byte[] FilterAvg(byte[] rgsample, int widthFilter)
        {
            var rgsampleResult = new byte[rgsample.Length];
            for(int i=0;i<rgsample.Length;i++)
            {
                var s = 0;
               
                for(int j=-widthFilter/2;j<widthFilter/2;j++)
                {
                    if (i + (j*20) >= 0 && i + (j*20) < rgsample.Length)
                        s += rgsample[i + j*20];
                }
                rgsampleResult[i] = (byte)(s/widthFilter);
            }
            return rgsampleResult;
        }


        private byte[] Filter(byte[] rgsample, double tInWheel, double tWheelLength, int cTopToDetect)
        {
            //return rgsample;
            var rgsampleResult = new byte[rgsample.Length];

            for (int i = 0; i < rgsample.Length; )
            {
                var t = i / freq;
                var tStart = t;
                var sample = rgsample[i];
                if (sample == 255)
                {
                    var iStart = i;
                    var tMaxLast = t;
                    var iTopLast = i;

                    while (t - tMaxLast < tInWheel)
                    {

                        i++;
                        t = i / freq;
                        sample = rgsample[i];
                        if (sample == 255)
                        {
                            iTopLast = i;
                            tMaxLast = t;
                        }
                    }
                    i = iTopLast;
                    for (int j = iStart; j <= i; j++)
                    {
                        rgsampleResult[j] = (byte)(tMaxLast - tStart > tWheelLength ? 255 : 0);
                    }
                    i++;


                }
                else
                {
                    rgsampleResult[i] = 0;
                    i++;
                }

            }

            return rgsampleResult;
        }



        
        IEnumerable<double> EnsecWheelSampleGet(byte[] rgsample)
        {
            for (int i = 0; i < rgsample.Length; )
            {
                var sample = rgsample[i];
                if (sample == 255)
                {
                    var tStart = i / freq;
                    while (sample == 255)
                    {
                        i++;
                        if (i == rgsample.Length)
                            break;
                        sample = rgsample[i];
                    }
                    var tEnd = i/freq;

                    yield return tStart; // (tEnd + tStart) / 2;

                }
                else
                    i++;

            }
        }
    
        private class TrainSample
        {
            public readonly double[] RgsecWheelDetected;

            public TrainSample(double[] rgsecWheelDetected)
            {
                this.RgsecWheelDetected = rgsecWheelDetected;
            }

            public string Tsto()
            {
                var sb = new StringBuilder();
                for (int i = 0; i < RgsecWheelDetected.Length;i++ )
                    sb.AppendLine("iwheel: {0} at {1}".StFormat(i, RgsecWheelDetected[i].ToString("0.##")));
                return sb.ToString();
            }
        }

        private byte[] Rgsample(string fpat)
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
        private IEnumerable<double> Fingerprint(Wagon wagon)
        {
            var normalizer = wagon.Rgwheel[1] - wagon.Rgwheel[0];
            for (int i = 0; i < wagon.Rgwheel.Length - 1; i++)
                yield return (wagon.Rgwheel[i + 1] - wagon.Rgwheel[i])/normalizer;
        }

        private IEnumerable<Wagon> AddReverse(Wagon[] rgtrain)
        {
            foreach (var train in rgtrain)
            {
                yield return train;
                yield return new Wagon(train.Id, train.Length, train.Rgwheel.Reverse().Select(wheel => train.Length-wheel) .ToArray());
            }
        }

        private Wagon[] RgtrainGet()
        {
            var pparserTrains = new Pparser(Path.Combine(Path.GetDirectoryName(FpatIn), "UDoW.txt"));
            var ctrain = pparserTrains.Fetch<int>();
            var rgtrain = new Wagon[ctrain];
            for(int itrain=0;itrain<ctrain;itrain++)
            {
                int length;
                int cwheel;
                pparserTrains.Fetch(out length, out cwheel);
                var rgwheel = pparserTrains.FetchN<int>(cwheel).ToArray();

                rgtrain[itrain] = new Wagon(itrain, length, rgwheel);
            }

            return rgtrain;
        }

        class Wagon
        {
            public readonly int Id; 
            public readonly int Length;
            public readonly int[] Rgwheel;
            public readonly double[] Fingerprint;
            
            public Wagon(int id, int length, int[] rgwheel)
            {


                Id = id;
                Length = length;
                Rgwheel = rgwheel;

                Fingerprint = new double[rgwheel.Length-1];
                
                double normalizer = Rgwheel[1] - Rgwheel[0];
                for (int i = 0; i < Rgwheel.Length - 1; i++)
                    Fingerprint[i]  = (Rgwheel[i + 1] - Rgwheel[i]) / normalizer;
            }

            public int Cwheel
            {
                get { return Rgwheel.Length; }
            }

            public string Tsto()
            {
                return Id+ ": " + Fingerprint.StJoin(" ", d => d.ToString("0.##"));
                
            }
        }


    }
}
