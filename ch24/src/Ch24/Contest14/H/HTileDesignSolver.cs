using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ch24.Util;
using Cmn.Util;

namespace Ch24.Contest14.H
{
    public class HTileDesignSolver : Contest.Solver
    {
        private class Pont
        {
            public readonly int X;
            public readonly int Y;
            public bool FBlack;

            public Kcso Kcso;
            public Ki Ki;

            public Pont(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private struct Center
        {
            public readonly int x;
            public readonly int y;

            public Center(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        }

        private class Step
        {
            public readonly List<Pont> Rgpont;
            public readonly List<Center> Rgcenter;

            public Step(Center center, Pont pont1, Pont pont2)
            {
                Rgpont = new List<Pont> {pont1, pont2};
                Rgcenter = new List<Center>{center};
            }

            public Step(Center center, Pont pont1, Pont pont2, Center center2, Pont pont3, Pont pont4)
            {
                Rgpont = new List<Pont> {pont1, pont2, pont3, pont4};
                Rgcenter = new List<Center>{center, center2};
            }

            public static IEnumerable<Step> FromCenter(Center center, Pont[,] mpont)
            {
                if(mpont.FValidX(center.x + 0, center.x - 1) && mpont.FValidY(center.y + 1, center.y + 2))
                    yield return new Step(
                        new Center(center.x + 0, center.y + 1), mpont[center.x - 1, center.y + 1], mpont[center.x + 0, center.y + 1],
                        new Center(center.x + 0, center.y + 2), mpont[center.x - 1, center.y + 2], mpont[center.x + 0, center.y + 2]
                        );

                if(mpont.FValidX(center.x + 0, center.x - 1) && mpont.FValidY(center.y - 2, center.y - 3))
                    yield return new Step(
                        new Center(center.x + 0, center.y - 1), mpont[center.x - 1, center.y - 2], mpont[center.x + 0, center.y - 2],
                        new Center(center.x + 0, center.y - 2), mpont[center.x - 1, center.y - 3], mpont[center.x + 0, center.y - 3]
                        );

                if(mpont.FValidX(center.x + 1) && mpont.FValidY(center.y + 0, center.y - 1))
                    yield return new Step(
                        new Center(center.x + 1, center.y + 0), mpont[center.x + 1, center.y - 1], mpont[center.x + 1, center.y + 0]);

                if(mpont.FValidX(center.x - 2) && mpont.FValidY(center.y + 0, center.y - 1))
                    yield return new Step(
                        new Center(center.x - 1, center.y + 0), mpont[center.x - 2, center.y - 1], mpont[center.x - 2, center.y + 0]);
            }
        }

        public override void Solve()
        {
            var rgwhl = Fetch<int[]>();
            var w = rgwhl[0];
            var h = rgwhl[1];

            var lck = new object();

            var maxScore = 0;
            Pont[,] maxmpont = null;

            for(var qqq = 0;maxScore!=100;qqq++)
            {
                lock(lck)
                {
                    if(maxScore==100)
                        return;
                }

                var rand = new Random(qqq * 1000 + IdProblem * 37);

                var mpont = new Pont[w,h];
                foreach(var vxy in mpont.Envxy())
                    mpont[vxy.x, vxy.y] = new Pont(vxy.x, vxy.y);

                var c0 = new Center(w / 4 * 2 + 1, h / 4 * 2 + 1);
                mpont[c0.x - 1, c0.y - 1].FBlack = true;
                mpont[c0.x - 1, c0.y - 0].FBlack = true;
                mpont[c0.x - 0, c0.y - 1].FBlack = true;
                mpont[c0.x - 0, c0.y - 0].FBlack = true;

                var mfCenter = new bool[w + 1,h + 1];
                mfCenter[c0.x, c0.y] = true;

                var rgstep = new List<Step>();
                rgstep.AddRange(Step.FromCenter(c0, mpont));

                for(var iii = 0; iii < rgstep.Count; iii++)
                {
                    var inext = rand.Next(iii, rgstep.Count);
                    var step = rgstep[inext];
                    rgstep[inext] = rgstep[iii];

                    if(step.Rgpont.Any(pont => pont.FBlack))
                        continue;

                    foreach(var pont in step.Rgpont)
                    {
                        pont.FBlack = true;
                    }

                    foreach(var center in step.Rgcenter)
                    {
                        mfCenter[center.x, center.y] = true;
                    }

                    rgstep.AddRange(Step.FromCenter(step.Rgcenter.Last(), mpont).Where(stepT => !stepT.Rgpont.Any(pont => pont.FBlack)));
                }

                foreach(var pont in mpont.Envxy().Select(vxy => vxy.v))
                    pont.Kcso = KcsoGet(pont, mfCenter);

                var rgpont = SetKi(mpont).ToList();

                //Console.WriteLine(rgpont.Select(pont => pont.ki.ToString()).StJoin(""));

                var repeat = MaxRepeat(rgpont);
                //Console.WriteLine(TSTORepeat(rgpont, repeat.i1, repeat.c));
                //Console.WriteLine(TSTORepeat(rgpont, repeat.i2, repeat.c));

                int score;
                if(repeat.C <= rgwhl[2])
                    score = 100;
                else if(repeat.C >= rgwhl[2] + 25)
                    score = 0;
                else
                    score = (int) Math.Round(0.16 * Math.Pow(rgwhl[2] + 25 - repeat.C, 2));
                lock(lck)
                {
                    if(maxScore < score)
                    {
                        maxScore = score;
                        maxmpont = mpont;
                    }
                }
                //Console.WriteLine(repeat.C + "/" + rgwhl[2] + " = " + score);
                Console.Write(".");


                //if(StRepeat(rgpont, repeat.I1, repeat.C) != StRepeat(rgpont, repeat.I2, repeat.C))
                //    throw new Exception();


                //display(mpont,rgstep, mfCenter);
            };
            
            Console.WriteLine(maxScore);
            //display(maxmpont,new List<Step>(), new bool[0,0]);
            Score = 100-maxScore;
            using(Output)
            {
                WriteLine(new []{w,h});
                foreach(var y in maxmpont.YCount().Eni())
                {
                    WriteLine(maxmpont.XCount().Eni().Select(x => maxmpont[x,y].Kcso.ToString().Substring(1).Replace('_','-')).StJoin(""));
                }
            }
        }

        private static string TSTORepeat(List<Pont> rgpont, int i1, int c)
        {
            return new string(' ', i1)+StRepeat(rgpont, i1, c);
        }

        private static string StRepeat(List<Pont> rgpont, int i1, int c)
        {
            return c.Eni().Select(i => rgpont[(i1+i)%rgpont.Count].Ki.ToString()).StJoin("");
        }

        private Repeat MaxRepeat(List<Pont> rgpont)
        {
            var repeat = new Repeat {C = 0};

            var rgch0 = rgpont.Select(pont => pont.Ki.ToString().First()).ToArray();
            var cmax = 0;
            for(var rgch = rgch0.ToList();;)
            {
                rgch.Add('$');
                var sufftree = new SuffTree("JBE$".ToList(), rgch);

                var c = sufftree.Root.CRepeat();

                if(c<=cmax)
                    break;

                cmax = c;

                rgch = rgch0.Concat(rgch0.Take(c)).ToList();
            }

            //for(var di = 1;di<rgpont.Count;di++)
            //{
            //    var i0 = 0;
            //    for(;FSameKi(rgpont, i0, di);)
            //        i0++;
            //    var c = 0;
            //    for(var i = 1;i<rgpont.Count;i++)
            //    {
            //        c = FSameKi(rgpont, i0 + i, di) ? c + 1 : 0;
            //        if(c <= repeat.C)
            //            continue;
            //        repeat.C = c;
            //        repeat.I1 = (i0 + i + rgpont.Count - c + 1)%rgpont.Count;
            //        repeat.I2 = (i0 + i + rgpont.Count - c + 1 + di)%rgpont.Count;
            //    }
            //}

            //if(cmax != repeat.C)
            //    throw new Exception();

            repeat.C = cmax;

            return repeat;
        }

        private static bool FSameKi(List<Pont> rgpont, int i, int di)
        {
            return rgpont[i % rgpont.Count].Ki == rgpont[(i+di)%rgpont.Count].Ki;
        }

        private class Repeat
        {
            public int C;
            public int I1;
            public int I2;
        }

        private enum Ki { J, B, E}
        private enum Ks { F, L, J, B}

        private IEnumerable<Pont> SetKi(Pont[,] mpont)
        {
            var pont = mpont[0, 0];
            var ks = Ks.L;

            for(;;)
            {
                var kcso = pont.Kcso;

                switch(kcso)
                {
                    case Kcso.oI:
                    case Kcso.o_:
                        pont.Ki = Ki.E;
                        break;
                    case Kcso.oF:
                        switch(ks)
                        {
                            case Ks.L:
                                ks = Ks.B;
                                pont.Ki = Ki.J;
                                break;
                            default:
                                ks = Ks.F;
                                pont.Ki = Ki.B;
                                break;
                        }
                        break;
                    case Kcso.o7:
                        switch(ks)
                        {
                            case Ks.L:
                                ks = Ks.J;
                                pont.Ki = Ki.B;
                                break;
                            default:
                                ks = Ks.F;
                                pont.Ki = Ki.J;
                                break;
                        }
                        break;
                    case Kcso.oL:
                        switch(ks)
                        {
                            case Ks.F:
                                ks = Ks.B;
                                pont.Ki = Ki.B;
                                break;
                            default:
                                ks = Ks.L;
                                pont.Ki = Ki.J;
                                break;
                        }
                        break;
                    case Kcso.oJ:
                        switch(ks)
                        {
                            case Ks.F:
                                ks = Ks.J;
                                pont.Ki = Ki.J;
                                break;
                            default:
                                ks = Ks.L;
                                pont.Ki = Ki.B;
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                yield return pont;

                switch(ks)
                {
                    case Ks.F:
                        pont = mpont[pont.X + 0, pont.Y + 1];
                        break;
                    case Ks.L:
                        pont = mpont[pont.X + 0, pont.Y - 1];
                        break;
                    case Ks.J:
                        pont = mpont[pont.X - 1, pont.Y + 0];
                        break;
                    case Ks.B:
                        pont = mpont[pont.X + 1, pont.Y + 0];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if(pont.X==0&&pont.Y==0)
                    break;

            }
        }

        private enum Kcso { oF, o7, oL, oJ, oI, o_ }

        private string ChGet(Center c, bool[,] hlmCenter)
        {
            return hlmCenter[c.x, c.y] ? "b" : "-";
        }

        private Kcso KcsoGet(Pont pont, bool[,] mfCenter)
        {
            var st = 
                ChGet(new Center(pont.X+0, pont.Y+0), mfCenter) +
                ChGet(new Center(pont.X+1, pont.Y+0), mfCenter) +
                ChGet(new Center(pont.X+0, pont.Y+1), mfCenter) +
                ChGet(new Center(pont.X+1, pont.Y+1), mfCenter);

            switch(st)
            {
                case "---b":
                case "bbb-":
                    return Kcso.oF;
                case "--b-":
                case "bb-b":
                    return Kcso.o7;
                case "b---":
                case "-bbb":
                    return Kcso.oJ;
                case "-b--":
                case "b-bb":
                    return Kcso.oL;
                case "--bb":
                case "bb--":
                    return Kcso.o_;
                case "-b-b":
                case "b-b-":
                    return Kcso.oI;
                default:
                    throw new Exception();
            }
        }

        private void Display(Pont[,] mpont, IEnumerable<Step> rgstep, bool[,] mfCenter)
        {
            var c = Math.Max(3, (800 / Math.Max(mpont.XCount(), mpont.YCount())) / 3 * 3);
            using(var bmp = new Bitmap(1+c * mpont.XCount(), 1+c * mpont.YCount()))
            {
                using(var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Gray);

                    foreach(var pont in rgstep.SelectMany(step => step.Rgpont))
                    {
                        DrawPont(g, Brushes.LightGreen, pont, c);
                    }

                    foreach(var pont in mpont.Envxy().Select(vxy => vxy.v).Where(pont => pont.FBlack))
                    {
                        DrawPont(g, Brushes.Black, pont, c);
                    }

                    foreach(var vxy in mfCenter.Envxy().Where(vxy => vxy.v))
                    {
                        var r = Math.Max(0, (c-1) / 12);
                        g.FillRectangle(Brushes.LightBlue, vxy.x*c-r,vxy.y*c-r,1+2*r,1+2*r);
                    }

                    foreach(var pont in mpont.Envxy().Select(vxy => vxy.v).Where(pont => pont.FBlack))
                    {
                        DrawCso(g, Brushes.White, pont, c);
                    }

                }
                bmp.Tsto();
            }
        }

        private void DrawPont(Graphics g, Brush brush, Pont pont, int c)
        {
            g.FillRectangle(brush, pont.X * c + 1, pont.Y * c + 1, c - 1, c - 1);
        }

        private int[,] McsoGet(Kcso kcso)
        {
            switch(kcso)
            {
                case Kcso.oF: return new[,]
                    {
                        {0, 0, 0},
                        {0, 1, 1},
                        {0, 1, 0},
                    };
                case Kcso.o7: return new[,]
                    {
                        {0, 0, 0},
                        {1, 1, 0},
                        {0, 1, 0},
                    };
                case Kcso.oL: return new[,]
                    {
                        {0, 1, 0},
                        {0, 1, 1},
                        {0, 0, 0},
                    };
                case Kcso.oJ: return new[,]
                    {
                        {0, 1, 0},
                        {1, 1, 0},
                        {0, 0, 0},
                    };
                case Kcso.oI: return new[,]
                    {
                        {0, 1, 0},
                        {0, 1, 0},
                        {0, 1, 0},
                    };
                case Kcso.o_: return new[,]
                    {
                        {0, 0, 0},
                        {1, 1, 1},
                        {0, 0, 0},
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawCso(Graphics g, Brush brush, Pont pont, int c)
        {
            var wh = (c) / 3;
            var x = pont.X * c;
            var y = pont.Y * c;

            foreach(var vxy in McsoGet(pont.Kcso).Envxy().Where(vxy => vxy.v == 1))
            {
                var dy = vxy.x;
                var dx = vxy.y;
                g.FillRectangle(brush, x+dx*wh, y+dy * wh, wh, wh);
            }
        }

    }
}
