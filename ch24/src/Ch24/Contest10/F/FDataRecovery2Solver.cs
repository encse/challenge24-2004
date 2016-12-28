using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Ch24.Util;

namespace Ch24.Contest10.F
{
    class FDataRecovery2Solver : Solver
    {

        const int freqSample = 18000;
        public override void Solve()
        {
            var rgsample = Wavu.Rgsample8bit(FpatIn, freqSample);
            rgsample = rgsample.SkipWhile(b => b == 128).ToArray();

            var rgtribit = Entribit(rgsample).ToArray();
            using (Output)
            {
                for (int i = 0; i < rgtribit.Length;)
                {
                    if (rgtribit[i + 0] == 2)
                        i++;
                    else
                    {
                        if (rgtribit[i + 0] != 0 || rgtribit[i + 6] != 1 || rgtribit[i + 7] != 2)
                        {
                            Output.Write("?");
                            i++;
                        }
                        else
                        {
                            int ch = 0;
                            for (int k = 0; k < 5; k++)
                            {
                                ch *= 3;
                                ch += rgtribit[i + k + 1];
                            }
                            Output.Write((char)ch); 
                            i += 8;
                        }
                    }
                }
            }

        }

        private IEnumerable<int> Entribit(byte[] rgsample)
        {
            int freqBaud = 250;
            var blockSize = freqSample/freqBaud;
            int i = 0;
            while (i + blockSize < rgsample.Length)
            {
                int c = CSignChange(rgsample, i, blockSize);

                double f = (double) c*freqBaud/2;
                if (Math.Abs(f - 1000) < 500)
                    yield return 0;
                else if (Math.Abs(f - 2000) < 500)
                    yield return 1;
                else if (Math.Abs(f - 3000) < 500)
                    yield return 2;
                else
                    yield return 3;

                i += blockSize;
            }
        }

        public static int CSignChange(byte[] rgsample, int isample, int csample)
        {
            int c = 0;
            int signPrev = Sign(rgsample[isample]);
            int isampleLim = isample + csample;
            isample++;
            while (isample < isampleLim)
            {
                int sign = Sign(rgsample[isample]);
                if (sign != signPrev)
                    c++;
                signPrev = sign;
                isample++;
            }
            return c;
        }
        private static int Sign(byte sample)
        {
            return sample >= 128 ? 1 : -1;
        }
    }
}
