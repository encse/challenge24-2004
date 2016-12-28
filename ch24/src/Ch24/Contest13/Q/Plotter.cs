using System;
using System.Drawing;
using Cmn.Util;

namespace Ch24.Contest13.Q
{
    public class R
    {
        public const int regPC = 0;
        public const int regZ = 1;
        public const int regO = 2;
    }

    internal class SignalStop : Exception
    {
    }

    public abstract class Stm
    {
        public abstract void Eval(Env env);

        protected void SetRegDstAndFlags(int regDst, Decimal r, Env env)
        {
            while (r > 2147483647)
            {
                env.mem[R.regO] = 1;
                r -= 4294967296;
            }

            while (r < -2147483648)
            {
                env.mem[R.regO] = 1;
                r += 4294967296;
            }

            if (r == 0)
            {
                env.mem[R.regZ] = 1;
            }
            env.mem[regDst] = (int)r;
        }
    }

    public class Movr : Stm
    {
        public readonly int regDst;
        public readonly int regSrc;

        public Movr(int regDst, int regSrc)
        {
            this.regDst = regDst;
            this.regSrc = regSrc;
        }

        public override void Eval(Env env)
        {
            env.mem[regDst] = env.mem[regSrc];
        }
        public override string ToString()
        {
            return "movr R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public class Movc: Stm
    {
        public readonly int regDst;
        public readonly int c;

        public Movc(int regDst, int c)
        {
            this.regDst = regDst;
            this.c = c;
        }

        public override void Eval(Env env)
        {
            env.mem[regDst] = c;
        }
        public override string ToString()
        {
            return "movc R{0},{1}".StFormat(regDst, c);
        }
    }
    public class Movi: Stm
    {
        public readonly int regDst;
        public readonly int regSrc;

        public Movi(int regDst, int regSrc)
        {
            this.regDst = regDst;
            this.regSrc = regSrc;
        }

        public override void Eval(Env env)
        {
            var regDstDeref = env.mem[regDst];
            var regSrcDeref = env.mem[regSrc];
            env.mem[regDstDeref] = env.mem[regSrcDeref];
        }

        public override string ToString()
        {
            return "movi R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public abstract class Binop: Stm
    {
        public readonly int regDst;
        public readonly int regSrc;

        protected Binop(int regDst, int regSrc)
        {
            this.regDst = regDst;
            this.regSrc = regSrc;
        }
    }

    public class Add : Binop
    {
        public Add(int regDst, int regSrc) : base(regDst, regSrc)
        {
        }

        public override void Eval(Env env)
        {
            decimal i = env.mem[regDst];
            decimal j = env.mem[regSrc];
            SetRegDstAndFlags(regDst, i + j, env);
        }

        public override string ToString()
        {
            return "add R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public class Sub : Binop
    {
        public Sub(int regDst, int regSrc) : base(regDst, regSrc)
        {
        }

        public override void Eval(Env env)
        {
            decimal i = env.mem[regDst];
            decimal j = env.mem[regSrc];
            SetRegDstAndFlags(regDst, i - j, env);
        }

        public override string ToString()
        {
            return "sub R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public class Mul : Binop
    {
        public Mul(int regDst, int regSrc) : base(regDst, regSrc)
        {
        }

        public override void Eval(Env env)
        {
            decimal i = env.mem[regDst];
            decimal j = env.mem[regSrc];
            SetRegDstAndFlags(regDst, i * j, env);
        }

        public override string ToString()
        {
            return "mul R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public class Div : Binop
    {
        public Div(int regDst, int regSrc) : base(regDst, regSrc)
        {
        }

        public override void Eval(Env env)
        {
            decimal i = env.mem[regDst];
            decimal j = env.mem[regSrc];
            if (j == 0)
                throw new ArgumentException("devise by zero");
            SetRegDstAndFlags(regDst, Math.Floor(i/j), env);
        }

        public override string ToString()
        {
            return "div R{0},R{1}".StFormat(regDst, regSrc);
        }
    }

    public class Mod : Binop
    {
        public Mod(int regDst, int regSrc) : base(regDst, regSrc)
        {
        }

        public override void Eval(Env env)
        {
            decimal i = env.mem[regDst];
            decimal j = env.mem[regSrc];
            if (j == 0)
                throw new ArgumentException("devise by zero");

            var d = Math.Floor(i/j);
            SetRegDstAndFlags(regDst, i - d*j, env);
        }

        public override string ToString()
        {
            return "mod R{0},R{1}".StFormat(regDst, regSrc);
        }
    }


    public class Brush: Stm
    {
        protected enum Kpaint
        {
            Brush,
            Erase,
        }

        public readonly int RegStart;
            
        public Brush(int regStart)
        {
            RegStart = regStart;
        }

        protected virtual Kpaint KpaintGet()
        {
            return Kpaint.Brush;
        }

        public override void Eval(Env env)
        {
            Paint(env, Kpaint.Brush);
        }

        protected void Paint(Env env, Kpaint kpaint)
        {
            var reg0 = env.mem[RegStart];
            var x0 = Math.Max(0, env.mem[reg0 + 0]);
            var y0 = Math.Max(0, env.mem[reg0 + 1]);
            var s = env.mem[reg0 + 2];

            var xLim = Math.Min(x0 + s, env.bmpw.Width);
            var yLim = Math.Min(y0 + s, env.bmpw.Height);
         
            if (kpaint == Kpaint.Brush)
            {
                env.bmpw.TransformGrayscaleImageRect(x0, y0, xLim - x0, yLim - y0, v =>
                {
                    switch (v)
                    {
                        case 255: return 192;
                        case 192: return 128;
                        case 128: return 64;
                        case 64: return 0;
                        case 0: throw new ArgumentException("already black");
                        default: throw new ArgumentException("invalid color");
                    }
                });
            }
            else
            {
                env.bmpw.TransformGrayscaleImageRect(x0, y0, xLim - x0, yLim - y0, v =>
                {
                    switch (v)
                    {
                        case 255: throw new ArgumentException("already white");
                        case 192: return 255;
                        case 128: return 192;
                        case 64: return 128;
                        case 0: return 64;
                        default: throw new ArgumentException("invalid color");
                    }
                });
            }

            if (s > 0)
                env.cpxBrushed += s * s;
        }

        public override string ToString()
        {
            return "brush R{0}".StFormat(RegStart);
        }
    }

    public class Erase : Brush
    {
        public Erase(int regStart) : base(regStart)
        {
        }

        public override void Eval(Env env)
        {
            Paint(env, Kpaint.Erase);
        }

        public override string ToString()
        {
            return "erase R{0}".StFormat(RegStart);
        }
    }

    public class Exit: Stm
    {
        public override void Eval(Env env)
        {
            throw new SignalStop();
        }

        public override string ToString()
        {
            return "exit".StFormat();
        }
    }

    public class Env
    {
        public readonly int[] mem = new int[1024];
        public readonly BitmapWrapper bmpw;
        public long cpxBrushed;

        public Env(BitmapWrapper bmpw)
        {
            this.bmpw = bmpw;
        }

        public byte ColAt(int x, int y)
        {
            return bmpw[x, y];
        }

        public void SetCol(int x, int y, byte col)
        {
            bmpw[x, y] = col;
        }
    }

    public class Prg
    {
        public readonly Stm[] rgstm;

        public Prg(Stm[] rgstm)
        {
            this.rgstm = rgstm;
        }

        public void Tsto()
        {
            for (int i = 0; i < rgstm.Length; i++)
            {
                Console.WriteLine("{0}\t{1}".StFormat(i, rgstm[i]));
            }
        }
    }

    public class Plotter
    {
        public static void Run(BitmapWrapper bmpw, bool fTrace, Prg prg)
        {
            var env = new Env(bmpw);
            bmpw.FillRect(new Rectangle(0, 0, bmpw.Width, bmpw.Height), 255);
            //for (int y = 0; y < bmpw.Height; y++)
            //for (int x = 0; x < bmpw.Width; x++)
            //    env.SetCol(x, y, 255); // fill white
            

            if (prg.rgstm.Length > 20000)
                throw new Exception("too much instruction");

            var cstmRan = 0;
            try
            {
                while (cstmRan < 1000000)
                {
                    
                    var stm = prg.rgstm[env.mem[R.regPC]];
                    if(fTrace)
                        Console.WriteLine("{0}\t{1}".StFormat(env.mem[R.regPC], stm));

                    env.mem[R.regPC]++;
                    env.mem[R.regO] = 0;
                    env.mem[R.regZ] = 0;
                    
                    stm.Eval(env);
                    if (env.cpxBrushed > bmpw.Width * bmpw.Height * 4L)
                        throw new Exception("too many draw operations");

                    cstmRan++;
                }

                throw new Exception("Too much instruction");
            }
            catch (SignalStop)
            {
                //done
            }
        }
    }
}
