using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Cmn.Util
{
    // TODO: support 1 bpp bitmaps
    public class BitmapWrapper
    {
        static ILg log = Lg.GetLogger(typeof(BitmapWrapper));

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public PixelFormat Fmt { get; private set; }
        
        // opens an existing one from filesystem
        public BitmapWrapper(string fpath)
        {
            log.InfoFormat("Opening bitmap from {0}", fpath);
            using (Bitmap bmp = new Bitmap(fpath))
            {
                Init(bmp.Width, bmp.Height, bmp.PixelFormat);

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (/*Depth != 1 && */Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

                // Copy data from pointer to array
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, Pixels, 0, Pixels.Length);

                // Unlock bitmap data
                bmp.UnlockBits(bitmapData);
            }
        }

        // create a new one
        public BitmapWrapper(int w, int h, PixelFormat fmt)
        {
            log.Info("Creating new bitmap");
            Init(w, h, fmt);
        }

        private void Init(int w, int h, PixelFormat fmt)
        {
            log.InfoFormat("Bitmap format: {0}x{1} {2}", w, h, fmt);
            Width = w;
            Height = h;
            Fmt = fmt;
            Depth = Bitmap.GetPixelFormatSize(fmt);
            // create byte array to hold pixel values
            long cb = ((long)Width * (long)Height * (long)Depth + 7L) / 8L; // round up to next byte
            Pixels = new byte[cb];
        }

        public void Save(string fpat)
        {
            // with backup support
            if (File.Exists(fpat))
                File.Move(fpat, Solwrt.FpatBak(fpat));

            log.InfoFormat("Saving bitmap to {0}", fpat);

            using (var bmp = new Bitmap(Width, Height, Fmt))
            {
                var bdt = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, Fmt);

                // Copy data from byte array to pointer
                System.Runtime.InteropServices.Marshal.Copy(Pixels, 0, bdt.Scan0, Pixels.Length);

                bmp.UnlockBits(bdt);
                bmp.Save(fpat, ImageFormat.Png);

            }
        }

        public void FillRect(Rectangle r, byte col)
        {
            TransformGrayscaleImageRect(r.X, r.Y, r.Width, r.Height, v => col);
            //int cCount = Depth / 8;
            //int i = ((r.Y * Width) + r.X) * cCount;

            //int rdiff = (Width - r.Width) * cCount;

            //for (int y = r.Top; y < r.Bottom; y++)
            //{
            //    for (int x = r.Left; x < r.Right; x++)
            //    {
            //        SetPixelI(i, color.R, color.G, color.B);
            //        i += cCount;
            //    }
            //    i += rdiff;
            //}
        }

        // transforms in place using 
        public void TransformGrayscaleImageRect(int x, int y, int w, int h, Func<byte, byte> xform)
        {
            int cCount = Depth / 8;
            int i = ((y * Width) + x) * cCount;
            int rdiff = (Width - w) * cCount;
            for (int yT = y; yT < y + h; yT++)
            {
                for (int xT = x; xT < x + w; xT++)
                {
                    byte r, g, b;
                    GetPixelI(i, out r, out g, out b);
                    byte c = xform(r);
                    if (c != r)
                        SetPixelI(i, c, c, c);
                    i += cCount;
                }
                i += rdiff;
            }
        }

        // assume grayscale image
        public byte this[int x, int y]
        {
            get
            {
                return GetGrayscalePixel(x, y);
            }
            set
            {
                SetGrayscalePixel(x, y, value);
            }
        }

        // assume grayscale image
        public byte GetGrayscalePixel(int x, int y)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            //if (i > Pixels.Length - cCount)
            //    throw new IndexOutOfRangeException();

            byte r, g, b;
            GetPixelI(i, out r, out g, out b);
            return r;
        }

        // assume grayscale image
        public void SetGrayscalePixel(int x, int y, byte color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            SetPixelI(i, color, color, color);
        }

        public Color GetPixel(int x, int y)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            byte r, g, b;
            GetPixelI(i, out r, out g, out b);
            return Color.FromArgb(r, g, b);
        }

        private void GetPixelI(int i, out byte r, out byte g, out byte b)
        {
            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                b = Pixels[i];
                g = Pixels[i + 1];
                r = Pixels[i + 2];
                //byte a = Pixels[i + 3]; // a
            } 
            else if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                b = Pixels[i];
                g = Pixels[i + 1];
                r = Pixels[i + 2];
            }
            else //if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                b = g = r = Pixels[i];
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            SetPixelI(i, color.R, color.G, color.B);
        }

        // i is the first byte of the pixel
        private void SetPixelI(int i, byte r, byte g, byte b)
        {
            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = b;
                Pixels[i + 1] = g;
                Pixels[i + 2] = r;
                Pixels[i + 3] = 255;
            }
            else if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = b;
                Pixels[i + 1] = g;
                Pixels[i + 2] = r;
            }
            else if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = r;
            }
        }

    }

#if false

    // from http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp
    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = source.Width;
                Height = source.Height;

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                             source.PixelFormat);

                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[PixelCount * step];
                Iptr = bitmapData.Scan0;

                // Copy data from pointer to array
                System.Runtime.InteropServices.Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Copy data from byte array to pointer
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                // Unlock bitmap data
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
    }
#endif

}
