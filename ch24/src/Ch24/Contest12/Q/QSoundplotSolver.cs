using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;

namespace Ch24.Contest12.Q
{
    class QSoundplotSolver : Solver
    {
        
        public override void Solve()
        {
            var wave = new WavInFile(FpatIn);
            
            // sanity
            Debug.Assert(wave.GetSampleRate() == 44100);
            Debug.Assert(wave.GetNumChannels() == 2);
            Debug.Assert(wave.GetNumBits() == 16);

            var n = wave.GetNumSamples();

            log.Info("Wav file samples: " + n);

            var samples = new short[n*2];
            wave.Read(samples, n*2);
            Debug.Assert(wave.Eof());
            wave.Dispose();

            // first sample is positive in both channels
            Debug.Assert(samples[0] >= 0);
            Debug.Assert(samples[1] >= 0);

            var x = new double[n];
            int xmax = 0;
            int xmin = 22050;
            var y = new double[n];
            int ymax = 0;
            int ymin = 22050;

            var freq = x;
            for (int chan = 0; chan < 2; chan++) // 0-left, 1-right
            {
                int c = 0;
                int ifreq = 0;
                bool fPos = true;
                int max = 0;
                int min = 22050;

                for (int i = 0; i < n; i++)
                {
                    if ((samples[i * 2 + chan] >= 0) == fPos)
                    {
                        // count
                        c++;
                    }
                    else
                    {
                        // emit and reset counter
                        var f = 22050.0 / c; // calculated frequency
                        while (ifreq < i)
                        {
                            freq[ifreq++] = f;
                        }

                        if ((int)f > max) max = (int)f;
                        if ((int)f < min) min = (int)f;
                        c = 1;
                        fPos = !fPos;
                    }
                }
                if (c > 1)
                {
                    var f = 22050.0 / c; // calculated frequency
                    while (ifreq < n)
                    {
                        freq[ifreq++] = f;
                    }

                }
                else if (c == 1)
                {
                    freq[n - 1] = freq[n - 2];
                }
                freq = y;
                if (xmax == 0)
                {
                    xmax = max;
                    xmin = min;
                }
                else
                {
                    ymax = max;
                    ymin = min;
                }
            }

            // TODO
            // smoothen
            int hws = 100;            // half window size
            int fws = 2 * hws + 1;  // full window size

            freq = x;
            while (true) // twice
            {
                double running = freq.Take(fws).Sum(); // running sum of window
                var window = freq.Take(fws).ToArray(); // ring buffer
                int iwin = 0; // ring buffer next slot ptr

                for (int i = hws; i < n - hws; i++)
                {
                    freq[i] = (double)running / (double)fws;
                    if (i + hws + 1 < n)
                    {
                        running += freq[i + hws + 1] - window[iwin];
                        window[iwin] = freq[i + hws + 1];
                        iwin = (iwin + 1) % fws;
                    }
                }
                if (freq == x) 
                    freq = y;
                else
                    break;
            }

            //var l2 = new long[n];
            using (var bmp = new Bitmap(xmax-xmin + 2, ymax-ymin+2))
            {
                var blackPen = new Pen(Color.Black, 3);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Gray);
                    for (int i = 0; i < n-1; i++)
                    {
                        int x1 = (int)x[i] - xmin + 1;
                        int y1 = ymax - (int)y[i] + 1;
                        int x2 = (int)x[i + 1] - xmin + 1;
                        int y2 = ymax - (int)y[i + 1] + 1;

                        //l2[i] = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
                        g.DrawLine(blackPen, x1, y1, x2, y2);
                    }
                }
                bmp.Save(FpatOut, ImageFormat.Png);
            }
           

            //Application.Run(new Form1());
        }
        
    }

    
}
