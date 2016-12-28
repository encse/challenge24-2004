using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest13.P
{
    public class PWhistles2Solver : Contest.Solver
    {
        public override void Solve()
        {
            //sample freq / blocksize = f0
            //sampleFreq/f0 = blocksize
            var rgsample = Wavu.Rgsample8bit(FpatIn);

            var mpcByIfreq = new Dictionary<int, int>();
            
            foreach(var block in Wavu.RgsampleBlock(rgsample, 0, 10))
            {
                var rgamp = Wavu.RgampFromSample(block.Rgsample.ToArray(), 0, 100, 44100); 
                rgamp = rgamp.Skip(1).ToArray(); //levágjuk a DC frekevenciát
                var rgampMax = rgamp.Where(amp => amp > 10).ToArray();
                if (rgampMax.Length != 1)
                    throw new Exception("nem tudjuk eldönteni a frekvenciát");

                var iFreq = MinMaxKer.MaxAt(rgamp);
                if (!mpcByIfreq.ContainsKey(iFreq))
                    mpcByIfreq[iFreq] = 1;
                else
                    mpcByIfreq[iFreq]++;
            }

            using (Output)
                Output.WriteLine(mpcByIfreq.Count);

        }
    }
}
