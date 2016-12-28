using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using Ch24.Contest;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.IntegralTransforms.Algorithms;

namespace Ch24.Contest05.C
{
    class CleaningSolver : Solver
    {

        public override void Solve()
        {
            var br = new BinaryReader(File.OpenRead(FpatIn));

            var img = new Complex[256,256];
            for (int irow = 0; irow < 256; irow++)
            {
                for (int icol = 0; icol < 256; icol++)
                {
                    var rgbyte = br.ReadBytes(4);
                    img[irow, icol] = rgbyte[0] + (rgbyte[1] << 8) + (rgbyte[2] << 16) + (rgbyte[3] << 24);
                }
            }

            var mxBlur =new Complex[256, 256];
            mxBlur[0, 0]     = 1;
            mxBlur[0, 1]     = 1;
            mxBlur[0, 255]   = 1;
            mxBlur[1, 0]     = 1;
            mxBlur[1, 1]     = 1;
            mxBlur[1, 255]   = 1;
            mxBlur[255, 0]   = 1;
            mxBlur[255, 1]   = 1;
            mxBlur[255, 255] = 1;

            var fftBlur = ForwardFFT(mxBlur);
            for (int i = 0; i < 5; i++)
            {
                var imgDeblur = Deblur(img, fftBlur);
                ASSERT_Deblur(img, imgDeblur);
                img = imgDeblur;
            }

            var bmp = new Bitmap(256, 256);

            for (int y = 0; y < 256; y++)
                for (int x = 0; x < 256; x++)
                {
                    Debug.Assert(img[y, x].Real == 1 || img[y, x].Real == 0);
                    var c = img[y, x].Real == 1 ? Color.Black : Color.White;
                    bmp.SetPixel(x, 255-y, c);
                }
            Directory.CreateDirectory(DpatOut);
            bmp.Save(FpatOut);
        }

        void ASSERT_Deblur(Complex[,] imgOrig, Complex[,] imgDeblur)
         {
             for (int irow = 0; irow < imgOrig.GetLength(0); irow++)
                 for (int icol = 0; icol < imgOrig.GetLength(1); icol++)
                 {
                     var xxx = RealGetWrapAround(imgDeblur, irow - 1, icol - 1) + RealGetWrapAround(imgDeblur, irow - 1, icol) + RealGetWrapAround(imgDeblur, irow - 1, icol + 1) +
                               RealGetWrapAround(imgDeblur, irow, icol - 1) + RealGetWrapAround(imgDeblur, irow, icol) + RealGetWrapAround(imgDeblur, irow, icol + 1) +
                               RealGetWrapAround(imgDeblur, irow + 1, icol - 1) + RealGetWrapAround(imgDeblur, irow + 1, icol) + RealGetWrapAround(imgDeblur, irow + 1, icol + 1);

                     Debug.Assert(Math.Abs(imgOrig[irow, icol].Real - xxx) < 0.01);
                 }
         }

        double RealGetWrapAround(Complex[,] c, int irow, int icol)
        {
            if (irow < 0)
                irow += c.GetLength(0);
            else if (irow >= c.GetLength(0))
                irow -= c.GetLength(0);

            if (icol < 0)
                icol += c.GetLength(1);
            else if (icol >= c.GetLength(1))
                icol -= c.GetLength(1);

            return c[irow, icol].Real;
        }

        private Complex[,] Deblur(Complex[,] img, Complex[,] mxfftBlur)
        {

            var imgFft = ForwardFFT(img);

            var ccol = img.GetLength(1);
            var crow = img.GetLength(0);
            for (int irow = 0; irow < crow; irow++)
                for (int icol = 0; icol < ccol; icol++)
                    imgFft[irow, icol] /= mxfftBlur[irow, icol];

            var img2 = InverseFFT(imgFft);
            for (int irow = 0; irow < crow; irow++)
                for (int icol = 0; icol < ccol; icol++)
                    img2[irow, icol] = new Complex(Math.Round(img2[irow, icol].Real), 0);

            return img2;
        }
        private Complex[,] InverseFFT(Complex[,] img)
        {
            return Fft2D(img, false);
        }

        private Complex[,] ForwardFFT(Complex[,] img)
        {
            return Fft2D(img, true);
        }

        private Complex[,] Fft2D(Complex[,] img, bool fForward)
        {
            int ccol = img.GetLength(1);
            int crow = img.GetLength(0);
            
            var rgcomplexResult = new Complex[crow, ccol];
            for (int irow = 0; irow < crow; irow++)
            {
                var rgcomplexT = new Complex[ccol];
                for (int icol = 0; icol < ccol; icol++)
                    rgcomplexT[icol] = img[irow, icol];
                if(fForward)
                    new DiscreteFourierTransform().Radix2Forward(rgcomplexT, FourierOptions.Matlab);
                else
                    new DiscreteFourierTransform().Radix2Inverse(rgcomplexT, FourierOptions.Matlab);
                for (int icol = 0; icol < ccol; icol++)
                    rgcomplexResult[irow, icol] = rgcomplexT[icol];


            }

            for (int icol = 0; icol < ccol; icol++)
            {
                var rgcomplexT = new Complex[crow];
                for (int irow = 0; irow < crow; irow++)
                    rgcomplexT[irow] = rgcomplexResult[irow, icol];
                
                if (fForward)
                    new DiscreteFourierTransform().Radix2Forward(rgcomplexT, FourierOptions.Matlab);
                else
                    new DiscreteFourierTransform().Radix2Inverse(rgcomplexT, FourierOptions.Matlab);

                for (int irow = 0; irow < crow; irow++)
                    rgcomplexResult[irow, icol] = rgcomplexT[irow];
            }
            return rgcomplexResult;
        }
    }
}
