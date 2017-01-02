using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using Contest.Util;

namespace Contest
{
    public class Pxl
    {
        public int x;
        public int y;

        public RGBA rgba;
    }

    public class RGBA
    {
        public int r;
        public int g;
        public int b;
        public int a = 255;
    }

    public class Pngr : IDisposable
    {
        private readonly PngReader pngr;
        private static int maxRGBA;

        public static T[,] Load<T>(string filn, Func<Pxl,T> dgVFromPxl)
        {
            using(var pngr = new Pngr(filn))
            {
                var m = new T[pngr.Width,pngr.Height];
                foreach(var pxl in pngr.EnPxl())
                    m[pxl.x, pxl.y] = dgVFromPxl(pxl);
                return m;
            }
        }

        public Pngr(string filn)
        {
            pngr = FileHelper.CreatePngReader(filn);
            maxRGBA = (1 << pngr.ImgInfo.BitDepth) - 1;
        }

        public int Width => pngr.ImgInfo.Cols;

        public int Height => pngr.ImgInfo.Rows;

        public IEnumerable<Pxl> EnPxl()
        {
            return EnRGBA().Select((rgba, i) => new Pxl{x = i % Width, y = i / Width, rgba = rgba});
        }

        public IEnumerable<RGBA> EnRGBA()
        {
            PngChunkPLTE oplte = null;
            PngChunkTRNS otrns = null; // transparency metadata, can be null
            if(pngr.ImgInfo.Indexed)
            {
                oplte = pngr.GetMetadata().GetPLTE();
                otrns = pngr.GetMetadata().GetTRNS();
            }

            for(var row = 0; row < pngr.ImgInfo.Rows; row++)
            {
                var line = pngr.ReadRowInt(row);
                if(pngr.ImgInfo.Indexed)
                {
                    var rgrgba = ImageLineHelper.Palette2rgb(line, oplte, otrns, null);
                    for(var irgba = 0; irgba < rgrgba.Length;)
                    {
                        yield return new RGBA
                        {
                            r = rgrgba[irgba++],
                            g = rgrgba[irgba++],
                            b = rgrgba[irgba++],
                            a = (otrns != null ? rgrgba[irgba++] : 255),
                        };
                    }
                }
                else
                {
                    if(pngr.ImgInfo.Packed)
                        line = line.unpackToNewImageLine();
                    for(var col = 0; col < pngr.ImgInfo.Cols; col++)
                    {
                        switch(pngr.ImgInfo.Channels)
                        {
                            case 1:
                                yield return new RGBA
                                {
                                    r = Read8(col, line),
                                    g = Read8(col, line),
                                    b = Read8(col, line),
                                };
                                break;
                            case 2:
                                yield return new RGBA
                                {
                                    r = Read8(col*2, line),
                                    g = Read8(col*2, line),
                                    b = Read8(col*2, line),
                                    a = Read8(col*2+1, line),
                                };
                                break;
                            case 3:
                                yield return new RGBA
                                {
                                    r = Read8(col*3, line),
                                    g = Read8(col*3+1, line),
                                    b = Read8(col*3+2, line),
                                };
                                break;
                            case 4:
                                yield return new RGBA
                                {
                                    r = Read8(col*4, line),
                                    g = Read8(col*4+1, line),
                                    b = Read8(col*4+2, line),
                                    a = Read8(col*4+3, line),
                                };
                                break;
                        }
                    }
                }
            }

        }

        private static int Read8(int col, ImageLine line)
        {
            return 255*line.Scanline[col]/maxRGBA;
        }

        public void Dispose()
        {
            pngr.End();
        }
    }

    public class Pngw : IDisposable
    {
        private readonly PngWriter pngw;

        private int icol;
        private int irow;

        private ImageLine iline;

        public static void Save<T>(string filn, T[,] m, Func<T, RGBA> dgRGBAFromV)
        {
            using(var pngw = new Pngw(filn, m.XCount(), m.YCount()))
                foreach(var vxy in m.Envxy())
                    pngw.Write(dgRGBAFromV(vxy.v));
        }

        public Pngw(string filn, int cols, int rows)
        {
            pngw = FileHelper.CreatePngWriter(filn, new ImageInfo(cols, rows, 8, false), true);
            icol = 0;
            irow = 0;
            iline = new ImageLine(pngw.ImgInfo);
        }

        public void Write(RGBA rgba)
        {
            Debug.Assert(irow<pngw.ImgInfo.Rows);
            Debug.Assert(icol<pngw.ImgInfo.Cols);

            ImageLineHelper.SetPixel(iline, icol, rgba.r, rgba.g, rgba.b, rgba.a);
            icol++;
            
            if(icol != pngw.ImgInfo.Cols)
                return;
            
            icol = 0;
            pngw.WriteRow(iline,irow);
            irow++;
            iline = new ImageLine(pngw.ImgInfo);
        }

        public void Dispose()
        {
            pngw.End();
        }
    }
}
