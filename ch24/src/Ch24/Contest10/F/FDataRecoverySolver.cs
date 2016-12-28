using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Ch24.Contest;
using Cmn.Util;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Contest10.F
{
    class FDataRecoverySolver : Solver
    {
        const int cbyteBlock = 18000 / 250;

        public override void Solve()
        {
            var rgbyteData = File.ReadAllBytes(FpatIn).Skip(44).ToArray();
            rgbyteData = Trim(rgbyteData);
            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                for (int ipos = Skip3000(rgbyteData, 0); ipos <= rgbyteData.Length - cbyteBlock*8;)
                {
                    if (ReadBlock(rgbyteData, ipos, false) != 0)
                        throw new Exception("expected 0");

                    ipos += cbyteBlock;

                    int ch = 0;
                    for (int idigit = 0; idigit < 5; idigit++)
                    {
                        ch = 3*ch;
                        ch += ReadBlock(rgbyteData, ipos, false);

                        
                        ipos += cbyteBlock;
                    }

                    if (ch > 127)
                        throw new Exception("ch > 127");

                    solwrt.Write((char)ch);
                //    Console.Write((char) ch);
                    if (ReadBlock(rgbyteData, ipos, false) != 1)
                        throw new Exception("expected 1");
                    ipos += cbyteBlock;
                    if (ReadBlock(rgbyteData, ipos, false) != 2)
                        throw new Exception("expected 2");
                    ipos += cbyteBlock;
                    ipos = Skip3000(rgbyteData, ipos);

                }
            }

        }

        private byte[] Trim(byte[] rgbyteData)
        {
            rgbyteData = rgbyteData.SkipWhile(b => b == 128).ToArray();
            rgbyteData = rgbyteData.Reverse().ToArray();
            rgbyteData = rgbyteData.SkipWhile(b => b == 128).ToArray();
            rgbyteData = rgbyteData.Reverse().ToArray();
            return rgbyteData;
        }

        public int Skip3000(byte[] rgbyteData, int ipos)
        {

            if (ipos >= rgbyteData.Length)
                return ipos;

            while ( ipos + 8*cbyteBlock <= rgbyteData.Length && (
                ReadBlock(rgbyteData, ipos, true) != 0 ||
                !ReadBlock(rgbyteData, ipos+ cbyteBlock, true).FIn(0,1) ||
                ReadBlock(rgbyteData, ipos + 6 * cbyteBlock, true) != 1 ||
                ReadBlock(rgbyteData, ipos + 7 * cbyteBlock, true) != 2))
                ipos++;
           
            return ipos;
        }
        public int ReadBlock(byte[] rgbyteData, int ipos, bool f)
        {
            //18000 sample per sec, 250 baud
            var rgdataSample = new int[3];
 
            for (int j = 1; j < 8; j*=2)
            {
                var iposT = ipos;
                int cbyteSubBlock = cbyteBlock/j;
                int cSubBlock = cbyteBlock / cbyteSubBlock;
            
                for (int i = 0; i < cSubBlock; i++)
                {
                    var kvpData = ReadSubblock(rgbyteData, iposT, cbyteSubBlock);
                    if (kvpData != -1)
                        rgdataSample[kvpData]++;
                    iposT += cbyteSubBlock;
                }
            }

            if (rgdataSample[0] > rgdataSample[1] && rgdataSample[0] > rgdataSample[2])
                return 0;
            if (rgdataSample[1] > rgdataSample[0] && rgdataSample[1] > rgdataSample[2])
                return 1;
            if (rgdataSample[2] > rgdataSample[0] && rgdataSample[2] > rgdataSample[1])
                return 2;

            if (!f)
                throw new Exception("coki");
            return -1;
        }

        int XXX(byte[] rgbyteData, int ipos, bool f)
        {
            var cplx = new Complex[cbyteBlock];
            for(int i=0;i<cbyteBlock;i++)
            {
                cplx[i] = new Complex(rgbyteData[ipos + i] - 128, 0);
            }
            new DiscreteFourierTransform().BluesteinForward(cplx, FourierOptions.Matlab);
            var cplx2 = cplx.Select(cp => cp.Magnitude).ToArray();

            if (cplx2[4] > cplx2[8] && cplx2[4] > cplx2[12])
                return 0;
            if (cplx2[8] > cplx2[4] && cplx2[8] > cplx2[12])
                return 1;
            if (cplx2[12] > cplx2[4] && cplx2[12] > cplx2[8])
                return 2;
            if (!f)
                throw new Exception("coki");
            return -1;
        }

        private int ReadSubblock(byte[] rgbyteData, int ipos, int cbyteSubBlock)
        {
            int cZero = 0;
            var dataPrev = rgbyteData[ipos] - 128;
            for (int j = 0; j < cbyteSubBlock && ipos + j < rgbyteData.Length; j++)
            {
                var data = rgbyteData[ipos + j] - 128;
                if (data == 0)
                {
                    if (dataPrev != 0)
                        cZero++;
                }
                else if (dataPrev != 0)
                {
                    if (Math.Sign(data) != Math.Sign(dataPrev))
                        cZero++;
                }
                dataPrev = data;
            }
            int s;
            if(cbyteSubBlock == 72) s = 8;
            else if(cbyteSubBlock == 36) s = 4;
            else if (cbyteSubBlock == 18) s = 2;
            else if (cbyteSubBlock == 9) s = 1;
            else
                throw new Exception("s");

            if (cZero >= 1 * s && cZero <= 1 * s)
                return 0;
            if (cZero >= 2 * s && cZero <= 2 * s)
                return 1;
            if (cZero >= 3 * s && cZero <= 3 * s)
                return 2;
            return -1;
        }
        
    }
}
