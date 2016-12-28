using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.E
{
    internal class Compress2Solver : Solver
    {
        public override void Solve()
        {
            var rgbyIn = File.ReadAllBytes(FpatIn);

            var mach = new Mach
            {
                rgstm = RgstmFromRgby(rgbyIn)
            };

            using(Output)
            {
                WriteLine(mach.rgstm.Select(stm => stm.st).StJoin(Environment.NewLine));
            }
            mach.Run();
            Debug.Assert(rgbyIn.SequenceEqual(mach.rgbyOut));
            Score = -ScoreGet(mach.rgstm);
        }

        private List<Stm> RgstmFromRgby(byte[] rgby)
        {
            var ichPerW = 2;

            var rgAbc = rgby.GroupBy(by => by).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).ToArray();
            var limAbc = rgAbc.Length+1;

            var rgraw = new List<object>();

            rgraw.Add(new Push(0));

            rgraw.AddRange(rgAbc.Select(ch => new Push(ch)));

            var rgich = rgby.Select(ch => Array.IndexOf(rgAbc, ch) + 1).Reverse().ToList();

            var rgw = new List<int>();
            {
                var cch = 0;
                long w = 0;
                foreach (var ich in rgich)
                {
                    Debug.Assert(ich > 0);
                    Debug.Assert(ich < limAbc);

                    if (cch == ichPerW)
                    {
                        rgw.Add((int)w);
                        cch = 0;
                        w = 0;
                    }
                    w = w * limAbc + ich;
                    Debug.Assert(w < int.MaxValue);
                    cch++;
                }
                rgw.Add((int)w);
            }

            var rgAbcW = rgw.GroupBy(w => w).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).ToArray();
            var limAbcW = rgAbcW.Length + 1;

            rgraw.AddRange(rgAbcW.Select(w => new Push(w)));

            var rgiw = rgw.Select(w => Array.IndexOf(rgAbcW, w) + 1).ToList();

            rgraw.Add(new Push(0));
            
            long x = 0;

            foreach(var iw in rgiw)
            {
                Debug.Assert(iw>0);
                Debug.Assert(iw<limAbcW);

                var xNext = x * limAbcW + iw;
                if(xNext < int.MaxValue)
                {
                    x = xNext;
                }
                else
                {
                    rgraw.Add(new Push((int) x));
                    x = iw;
                }

            }
            rgraw.Add(new Push((int) x));

            rgraw.AddRange(new object[]
            {
                new Label("start", new Push(limAbcW)),
                new Div(),
                
                new Push(rgAbc.Length),
                new Add(),
                new Read(),

                new Label("start2", new Push(limAbc)),
                new Div(),

                new Read(),
                new Out(),

                new Push(-1),
                new Read(),
                new PushJump("nextich","start2"),
                new Label("nextich", new Jgz()),

                new Add(),

                new Push(-1),
                new Read(),
                new PushJump("nextinw","start"),
                new Label("nextinw", new Jgz()),

                new Add(),
                
                new Push(-1),
                new Read(),
                new PushJump("next","start"),
                new Label("next", new Jgz()),
            });
            return RgstmFromRgraw(rgraw);
        }

        private List<Stm> RgstmFromRgraw(List<object> rgraw)
        {
            var mpistmByStId = new Dictionary<string, int>();
            return rgraw.Select((raw, istm) =>
            {
                if (!(raw is Label))
                    return raw;
                Label label = (Label)raw;

                Debug.Assert(!mpistmByStId.ContainsKey(label.stId));

                mpistmByStId[label.stId] = istm;

                return label.stm;
            }).ToArray().Select(raw =>
            {
                if (!(raw is PushJump))
                    return raw;

                PushJump pj = (PushJump)raw;
                return new Push(mpistmByStId[pj.stTarget] - mpistmByStId[pj.stJgz] - 1);
            }).Cast<Stm>().ToList();
        }

        private class Mach
        {
            public List<Stm> rgstm;
            public int istmNext = 0;
            public readonly List<byte> rgbyOut = new List<byte>();
            public readonly List<int> stack = new List<int>();

            public void Run()
            {
                for(; 0 <= istmNext && istmNext < rgstm.Count;)
                {
                    var stm = rgstm[istmNext];
                    istmNext++;
                    stm.Run(this);
                }
            }
        }

        private class Label
        {
            public readonly string stId;
            public readonly Stm stm;

            public Label(string stId, Stm stm)
            {
                this.stId = stId;
                this.stm = stm;
            }
        }

        private class PushJump
        {
            public readonly string stJgz;
            public readonly string stTarget;

            public PushJump(string stJgz, string stTarget)
            {
                this.stJgz = stJgz;
                this.stTarget = stTarget;
            }
        }

        private abstract class Stm
        {
            public override string ToString()
            {
                return st;
            }

            public virtual string st
            {
                get
                {
                    return GetType().Name.ToUpperInvariant();
                }
            }

            public virtual int size
            {
                get
                {
                    return 1;
                }
            }

            public abstract void Run(Mach mach);
        }

        private class Push : Stm
        {
            public readonly int x;

            public Push(int x)
            {
                this.x = x;
            }

            public override string st
            {
                get
                {
                    return string.Format("{0} {1}", base.st, x);
                }
            }

            public override int size
            {
                get
                {
                    return 2;
                }
            }

            public override void Run(Mach mach)
            {
                mach.stack.Push(x);
            }
        }

        private class Out : Stm
        {
            public override void Run(Mach mach)
            {
                var ch = mach.stack.Pop();
                Debug.Assert(0 <= ch && ch < 128);
                mach.rgbyOut.Add((byte) ch);
            }
        }

        private class Read : Stm
        {
            public override void Run(Mach mach)
            {
                var i = mach.stack.Pop();
                if(i < 0)
                    i += mach.stack.Count;
                mach.stack.Push(mach.stack[i]);
            }
        }

        private class Jgz : Stm
        {
            public override void Run(Mach mach)
            {
                var a = mach.stack.Pop();
                var b = mach.stack.Pop();
                if(b > 0)
                    mach.istmNext += a;
            }
        }

        private class Add : Stm
        {
            public override void Run(Mach mach)
            {
                var a = mach.stack.Pop();
                var b = mach.stack.Pop();
                mach.stack.Push(a + b);
            }
        }

        private class Mul : Stm
        {
            public override void Run(Mach mach)
            {
                var a = mach.stack.Pop();
                var b = mach.stack.Pop();
                mach.stack.Push(a * b);
            }
        }

        private class Div : Stm
        {
            public override void Run(Mach mach)
            {
                var a = mach.stack.Pop();
                var b = mach.stack.Pop();
                mach.stack.Push(b / a);
                mach.stack.Push(b % a);
            }

        }

        private int ScoreGet(List<Stm> rgstm)
        {
            var size = rgstm.Sum(stm => stm.size);
            var a = new double[] {0, 5500, 5280, 9080, 17900, 20000, 25500, 40000, 41000, 58000, 65000};
            var b = new double[] {0, 3500, 3600, 6000, 10000, 10000, 15000, 25000, 27000, 34000, 32000};

            return (int)Math.Min(100, Math.Max(0, Math.Round(20 + 80 * (a[IdProblem] - size) / (a[IdProblem] - b[IdProblem]))));
        }
    }

    internal static class Stk
    {
        public static T Pop<T>(this List<T> stack)
        {
            var x = stack.Last();
            stack.RemoveAt(stack.Count - 1);
            return x;
        }

        public static void Push<T>(this List<T> stack, T item)
        {
            stack.Add(item);
        }
    }

}
