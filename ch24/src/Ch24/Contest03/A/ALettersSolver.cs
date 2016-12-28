using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Ch24.Contest;
using Cmn.Util;
using Image = System.Drawing.Image;

namespace Ch24.Contest03.A
{
    public class ALettersSolver : Solver
    {
        private static Color colBg = Color.FromArgb(255, 255, 255, 255);
        private static Color colFg = Color.FromArgb(255, 0,0,0);
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);

            int cletter, ccol, crow;
            pparser.Fetch(out cletter, out ccol, out crow);

            var rgletter = new Letter[cletter];
            for(int i=0;i<cletter;i++)
                rgletter[i] =
                    new Letter(pparser.Fetch<string>(), pparser.FetchN<string>(crow).ToArray());

            var bmpSrc = (Bitmap)Image.FromFile(FpatIn.Replace(".in", ".png"));
            var mp = ReadLetters(bmpSrc, rgletter);
            using(Output)
            foreach (var letter in rgletter)
                Solwrt.WriteLine(letter.Ch + " " + mp[letter]);

        }


        private Dictionary<Letter, int> ReadLetters(Bitmap bmp, Letter[] rgletter)
        {
            bmp = AForge.Imaging.Image.Clone(bmp, PixelFormat.Format24bppRgb);
            var mpcOccurrenceByletter = new Dictionary<Letter, int>();
            foreach (var letter in rgletter)
            {
                mpcOccurrenceByletter[letter] = 0;
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        
                        if (bmp.GetPixel(x, y) == colFg)
                        {
                            foreach (var letter in rgletter)
                                foreach (var bmpTemplate in letter.rgbmp)
                                {
                                    var orect = FMatch(bmp, x, y, bmpTemplate);
                                    if(orect != null)
                                    {
                                        g.FillRectangle(Brushes.Yellow, orect.Value);
                                        mpcOccurrenceByletter[letter]++;
                                        break;
                                    }
                                }
                        }
                    }
                }
            }
            
            return mpcOccurrenceByletter;
        }

        private Rectangle? FMatch(Bitmap bmp, int x, int y, Bitmap bmpTemplate)
        {
            int yT, xT;

            bool fFound = false;
            
            for(yT=0;yT < bmpTemplate.Height;yT++)
            for(xT=0;xT < bmpTemplate.Width;xT++)
            {
                if(!fFound && bmpTemplate.GetPixel(xT,yT) == colFg)
                {
                    fFound = true;
                    x -= xT;
                    y -= yT;

                }

                if(fFound)
                {
                    if (bmp.GetPixel(x + xT, y + yT) != bmpTemplate.GetPixel(xT, yT))
                        return null;
                }
                    
            }

            return new Rectangle(x, y, bmpTemplate.Width, bmpTemplate.Height);
        }

        class Letter
        {
            public string Ch;
            public readonly Bitmap bmp;
            public List<Bitmap> rgbmp;

            public Letter(string ch, string[] mask)
            {
                this.Ch = ch;
                bmp = new Bitmap(mask[0].Length, mask.Length, PixelFormat.Format24bppRgb);
                for(int x=0;x<bmp.Width;x++)
                for(int y=0;y<bmp.Height;y++)
                    bmp.SetPixel(x, y, mask[y][x] == '.' ? colBg : colFg);

                rgbmp = new List<Bitmap>();
                for(int i=0;i<4;i++)
                {
                    rgbmp.Add(bmp);
                    bmp = (Bitmap)bmp.Clone();
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
            }

        }



    }
}


