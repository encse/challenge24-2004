using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest14.R
{
    public class RSeatingOrderSolver : Contest.Solver
    {
        private class Pont
        {
            public int h;

            private int?[] rgi=new int?[3];

            public int x;
            public int y;
            public int di;
            
            public int? this[int dy]
            {
                get
                {
                    return rgi[dy + 1];
                }
                set
                {
                    rgi[dy + 1] = value;
                }
            }
        
            public int ToIntKey()
            {
                return -(di * 1000000 + y * 1000 + x);
            }

        }

        public override void Solve()
        {
            //Replay(Path.Combine(DpatIn, "0.png"), Path.Combine(DpatIn, "r0.out"));

            Score = 0;
            var m = new Machine();


            var rgpont = Pngr.Load(FpatIn, pxl => new Pont {x = pxl.x, y = pxl.y, h = pxl.rgba.r});

            for (var y = 0; y < rgpont.YCount(); y++)
            {
                for (var x = 0; x < rgpont.XCount(); x++)
                {
                    InvCount(rgpont, x, y);
                }
            }

            var ssPont = new SortedDictionary<int, Pont>();
            for(var y = 0; y < rgpont.YCount()-1; y++)
            {
                for(var x = 0; x < rgpont.XCount(); x++)
                {
                    CalcDi(rgpont, x, y);
                    ssPont.Add(rgpont[x,y].ToIntKey(), rgpont[x,y] );
                }
            }

            const int k = 3;

            //if(false)
            for(;;)
            {

                var ss = ssPont.First();

                if(ss.Value.di>k)
                {
                    var pont1 = ss.Value;

                    var pont2 = rgpont[pont1.x, pont1.y + 1];

                    ssPont.Remove(pont1.ToIntKey());
                    ssPont.Remove(pont2.ToIntKey());

                    Swap(m, pont1, pont2);
                    InvCount(rgpont, pont1.x, pont1.y);
                    InvCount(rgpont, pont2.x, pont2.y);

                    Changed(rgpont, pont1, pont2.h);
                    Changed(rgpont, pont2, pont1.h);

                    for(var y = Math.Max(0, pont1.y - 1); y < Math.Min(pont1.y + 1, rgpont.YCount() - 1); y++)
                    {
                        for(var x = 0; x < rgpont.XCount(); x++)
                        {
                            ssPont.Remove(rgpont[x, y].ToIntKey());
                            CalcDi(rgpont, x, y);
                            ssPont.Add(rgpont[x, y].ToIntKey(), rgpont[x, y]);
                        }
                    }
                }
                else
                {
                    for(var y = 0; y < rgpont.YCount(); y++)
                    {
                        for(var x = 0; x < rgpont.XCount()-1; x++)
                        {
                            var pont1 = rgpont[x, y];
                            var pont2 = rgpont[x+1, y];
                            if(pont1.h>pont2.h)
                            {
                                Swap(m, pont1, pont2);

                                for (var y2 = Math.Max(0,y-1); y2 <= Math.Min(y+1,rgpont.YCount()-1); y2++)
                                {
                                    for (var x2 = x; x2 <= x+1; x2++)
                                    {
                                        InvCount(rgpont, x2, y2);
                                    }
                                }

                                for(var y2 = Math.Max(0, y - 1); y2 < Math.Min(y + 1, rgpont.YCount() - 1); y2++)
                                {
                                    for(var x2 = x; x2 < x+1; x2++)
                                    {
                                        ssPont.Remove(rgpont[x2, y2].ToIntKey());
                                        CalcDi(rgpont, x2, y2);
                                        ssPont.Add(rgpont[x2, y2].ToIntKey(), rgpont[x2, y2]);
                                    }
                                }

                                if(ssPont.First().Value.di > k)
                                    goto foo;
        
                                x = Math.Max(-1, x - 2);
                            }
                        }
                    }
                    break;
                }
                foo:
                ;
            }

            Save(rgpont);

            for(var y = 0; y < rgpont.YCount(); y++)
            {
                for(var x = 0; x < rgpont.XCount()-1;x++)
                {
                    var pont1 = rgpont[x, y];
                    var pont2 = rgpont[x+1, y];
                    if(pont1.h>pont2.h)
                    {
                        Swap(m, pont1, pont2);
                        x = Math.Max(-1, x - 2);
                    }
                }
            }

            Save(rgpont);

            for (var y = 0; y < rgpont.YCount(); y++)
            {
                for (var x = 0; x < rgpont.XCount()-1; x++)
                {
                    if(rgpont[x,y].h > rgpont[x+1,y].h)
                        throw new Exception();
                }
            }







            //m.AddSwap(2, 3, 1, 3);
            //m.AddSwap(1, 3, 0, 3);
            //m.AddSwap(2, 7, 1, 7);
            //m.AddSwap(1, 7, 0, 7);
            //m.AddSwap(1, 7, 2, 7);


            //m.AddSwap(2, 8, 1, 8);
            //m.AddSwap(1, 8, 0, 8);

            //m.AddSwap(2, 8, 3, 8);
            //m.AddSwap(3, 8, 4, 8);
            //m.AddSwap(4, 8, 5, 8);
            //m.AddSwap(5, 8, 6, 8);
            //m.AddSwap(6, 8, 7, 8);
        

            //m.AddSwap(2, 8, 1, 8);
            
            using(Output)
            {
                m.WriteOutput(Output);
            }
        }

        private void Swap(Machine machine, Pont pont1, Pont pont2)
        {
            machine.AddSwap(pont1.x,pont1.y,pont2.x,pont2.y);
            U.Swap(ref pont1.h, ref pont2.h);
            Score++;
        }

        private static void CalcDi(Pont[,] rgpont, int x, int y)
        {
            rgpont[x, y].di = (int) ((rgpont[x, y][0] + rgpont[x, y + 1][0]) - (rgpont[x, y][1] + rgpont[x, y + 1][-1]));
        }

        private void Changed(Pont[,] rgpont, Pont pont, int hFrom)
        {
            for(var y = Math.Max(pont.y-1,0); y <= Math.Min(pont.y+1,rgpont.GetLength(1)-1); y++)
            {
                for(var x = 0; x < rgpont.GetLength(0); x++)
                {
                    if(x==pont.x)
                        continue;

                    var pontT = rgpont[x, y];
                    var inv = pontT[pont.y - y];
                    var h = pontT.h;
                    if(x<pont.x)
                    {
                        if(h>hFrom && h<=pont.h)
                            inv--;
                        else if(h<=hFrom && h>pont.h)
                            inv++;
                    }
                    else
                    {
                        if(h>=hFrom && h<pont.h)
                            inv++;
                        else if(h<hFrom && h>=pont.h)
                            inv--;
                    }
                    pontT[pont.y - y] = inv;
                }
            }
            
        }

        private void InvCount(Pont[,] rgpont, int x, int y)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                InvCount(rgpont, x, y, dy);
            }
        }

        private class Dixy
        {
            public readonly int di;
            public readonly int x;
            public readonly int y;

            public Dixy(int di, int x, int y)
            {
                this.di = di;
                this.x = x;
                this.y = y;
            }

            public Dixy(int intKey)
            {
                x = intKey % 1000;
                y = (intKey / 1000) % 1000;
                di = intKey / 1000000;
            }

            private int ToIntKey()
            {
                return -((di * 1000) + y * 1000) + x;
            }


        }

        private void InvCount(Pont[,] rgpont, int xFrom, int yFrom, int dy)
        {
            int? inv;
            var y = yFrom + dy;
            if(y<0||y>=rgpont.GetLength(1))
                inv = null;
            else
            {
                inv = 0;
                var h = rgpont[xFrom, yFrom].h;
                for(var x=0;x<rgpont.GetLength(0);x++)
                {
                    if(x==xFrom)
                        continue;
                    bool fInv;
                    if(x<xFrom)
                    {
                        fInv = rgpont[x, y].h > h;
                    }
                    else
                    {
                        fInv = rgpont[x, y].h < h;
                    }
                    if(fInv)
                        inv++;
                }
            }
            rgpont[xFrom, yFrom][dy] = inv;
        }

        private void Save(Pont[,] rgpont)
        {
            Pngw.Save(FpatOut+".tsto.png", rgpont, pont => new RGBA{r=pont.h,g=pont.h,b=pont.h});
        }

        private void Replay(string fpatIn, string fpatSolution)
        {
            
            var bmpw = new Bitmap(fpatIn);
            var pparser = new Pparser(fpatSolution);
            int cline = pparser.Fetch<int>();
            for (int i = 0; i < cline; i++)
            {
                int cswp;
                int x;
                int y;
                int[] rgswp;
                pparser.Fetch(out cswp, out x, out y, out rgswp);


                for (int iswp = 0; iswp < cswp; iswp++)
                {
                    var kdirMove = (Machine.Kdir)rgswp[iswp * 2];
                    var kdirSwap = (Machine.Kdir)rgswp[iswp * 2 + 1];

                    switch (kdirMove)
                    {
                        case Machine.Kdir.Stay: break;
                        case Machine.Kdir.North: y--; break;
                        case Machine.Kdir.East: x++; break;
                        case Machine.Kdir.South: y++; break;
                        case Machine.Kdir.West: x--; break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var xSwap = x;
                    var ySwap = y;
                    switch (kdirSwap)
                    {
                        case Machine.Kdir.Stay: break;
                        case Machine.Kdir.North: ySwap--; break;
                        case Machine.Kdir.East: xSwap++; break;
                        case Machine.Kdir.South: ySwap++; break;
                        case Machine.Kdir.West: xSwap--; break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var c = bmpw.GetPixel(x, y);
                    bmpw.SetPixel(x, y, bmpw.GetPixel(xSwap, ySwap));
                    bmpw.SetPixel(xSwap, ySwap, c);
                }
            }

            bmpw.Save(Path.Combine(DpatOut, "tsto.png"));
            
        }
        
        public class Machine
        {
            public enum Kdir
            {
                Stay = 0,
                North = 1,
                East = 2,
                South = 3,
                West = 4
            }
            class Chain
            {
                internal class Swpi
                {
                    public Kdir kdirMove;
                    public Kdir kdirSwap;

                    public Swpi(Kdir kdirMove, Kdir kdirSwap)
                    {
                        this.kdirMove = kdirMove;
                        this.kdirSwap = kdirSwap;
                    }
                }


                public int x0;
                public int y0;
                public List<Swpi> rgswpi = new List<Swpi>();
            }

            private int x, y;
            private readonly List<Chain> rgchain;

            public Machine()
            {
                x = -1;
                y = -1;
                rgchain = new List<Chain>();
            }

            Chain ChainCur()
            {
                return rgchain.Last();
            }

            Chain ChainNext()
            {
                var chain = new Chain();
                rgchain.Add(chain);
                return chain;
            }

            public void AddSwap(int x1, int y1, int x2, int y2)
            {
                Kdir? okdirMove;
                if ((okdirMove = OkdirNeighbour(x,y,x1,y1)) != null)
                {
                    x = x1;
                    y = y1;
                    var chain = ChainCur();
                    chain.rgswpi.Add(new Chain.Swpi(okdirMove.Value, OkdirNeighbour(x, y, x2, y2).Value));
                }
                else if ((okdirMove = OkdirNeighbour(x, y, x2, y2)) != null)
                {
                    x = x2;
                    y = y2;
                    var chain = ChainCur();
                    chain.rgswpi.Add(new Chain.Swpi(okdirMove.Value, OkdirNeighbour(x, y, x1, y1).Value));
                }
                else
                {
                    var chain = ChainNext();
                    x = x1;
                    y = y1;
                    chain.x0 = x;
                    chain.y0 = y;
                    chain.rgswpi.Add(new Chain.Swpi(Kdir.Stay,  OkdirNeighbour(x, y, x2, y2).Value));
                }
            }

            private Kdir? OkdirNeighbour(int x0, int y0, int x1, int y1)
            {
                if(x0==x1 && y0==y1)
                    return Kdir.Stay;
                
                if (x0 == x1)
                {
                    if(y1 == y0-1)
                        return Kdir.North;
                    if(y1 == y0+1)
                        return Kdir.South;
                }


                if (y0 == y1)
                {
                    if (x1 == x0 - 1)
                        return Kdir.West;
                    if (x1 == x0 + 1)
                        return Kdir.East;
                }

                return null;
            }

            public void WriteOutput(Solwrt solwrt)
            {
                ////ha üres az utolsó chain akkor azt ne
                //if (rgchain[rgchain.Count - 1].rgswpi.Count == 0)
                //    rgchain.RemoveAt(rgchain.Count - 1);

                solwrt.WriteLine(rgchain.Count);
                foreach (var chain in rgchain)
                {
                    solwrt.Write(chain.rgswpi.Count);
                    solwrt.Write(" ");
                    solwrt.Write(chain.x0);
                    solwrt.Write(" ");
                    solwrt.Write(chain.y0);

                    foreach (var swpi in chain.rgswpi)
                    {
                        solwrt.Write(" "); 
                        solwrt.Write((int)swpi.kdirMove);
                        solwrt.Write(" ");
                        solwrt.Write((int)swpi.kdirSwap);
                    }
                    solwrt.WriteLine("");
                }
             
            }
        }

    }
}
