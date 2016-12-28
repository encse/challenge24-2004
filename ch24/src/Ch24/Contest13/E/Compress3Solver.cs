using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest13.E
{
    internal class Compress3Solver : Solver
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
            var sequenceEqual = rgbyIn.SequenceEqual(mach.rgbyOut);
            Info("SequenceEqual: "+sequenceEqual);
            Debug.Assert(sequenceEqual);
            Score = (long)-Math.Round(ScoreGet(mach.rgstm));
        }

        private class Ser
        {
            public readonly int[] rgichSer;
            public readonly List<int> rgiich=new List<int>();
            private int? _scr;

            public Ser(int[] rgichSer)
            {
                this.rgichSer = rgichSer;
            }

            public void Add(int[] rgich, int iich)
            {
                //Debug.Assert(rgichSer.SequenceEqual(rgich.Skip(iich-rgichSer.Length+1).Take(rgichSer.Length)));
                rgiich.Add(iich);
                _scr = null;
            }

            public Dictionary<int,Ser> MpserByIchNext(int[] rgich)
            {
                var mpserByIchNext = new Dictionary<int, Ser>();

                foreach(var iichNext in rgiich.Select(iich => iich + 1).Where(iichNext => iichNext < rgich.Length))
                {
                    var ichNext = rgich[iichNext];
                    mpserByIchNext
                        .EnsureGet(ichNext, () => new Ser(rgichSer.Concat(new[] {ichNext}).ToArray()))
                        .Add(rgich, iichNext);
                }
                return mpserByIchNext;
            }

            public int Scr
            {
                get
                {
                    if(!_scr.HasValue)
                    {
                        var length = rgichSer.Length;
                        var c = rgiich.Count;
                        _scr = length * c - length - c;
                    }
                    return _scr.Value;
                }
            }

        }

        private List<Stm> RgstmFromRgby(byte[] rgch)
        {
            var rgAbc = rgch.GroupBy(by => by).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).ToList();
            var limAbc = rgAbc.Count+1;

            var rgich = rgch.Select(ch => rgAbc.IndexOf(ch) + 1).ToArray();

            var rgser = new List<Ser>();

            for(;;)
            {
                var mpserByIch = new Dictionary<int, Ser>();
                foreach(var t in rgich.Select((ich, iich) => new {ich, iich}))
                    mpserByIch.EnsureGet(t.ich, () => new Ser(new[] {t.ich})).Add(rgich, t.iich);

                var serMax = SerMaxGet(rgich, mpserByIch.Values);

                if(serMax.Scr <= 40)
                    break;

                var ichSer = limAbc + rgser.Count;
                rgser.Add(serMax);

                foreach(var iich in serMax.rgiich.OrderByDescending(iich => iich))
                {
                    if(rgich[iich] == -1)
                        continue;

                    rgich[iich] = ichSer;
                    for(var d = 1; d < serMax.rgichSer.Length; d++)
                        rgich[iich - d] = -1;
                }

                rgich = rgich.Where(ich => ich != -1).ToArray();
            }
            return RgstmCreate(rgich, rgser, rgAbc);
        }

        private List<Stm> RgstmCreate(int[] rgich, List<Ser> rgser, List<byte> rgAbc)
        {
            var limAbc = rgAbc.Count+1;
            var limAll = limAbc + rgser.Count;

            var rgraw = new List<IRaw>();

            rgraw.Add(new Push(0));

            rgraw.AddRange(rgAbc.Select(ch => new Push(ch)));

            rgraw.AddRange(rgser.Select((ser, iser) => new PushJump("jmpSer", "ser" + iser)));

            rgraw.Add(new Push(-1));

            var pushAll = new Action<int[]>(rgichT =>
            {
                long x = 0;

                foreach(var ich in rgichT.Reverse())
                {
                    Debug.Assert(ich > 0);
                    Debug.Assert(ich < limAll);

                    var xNext = x * limAll + ich;
                    if(xNext < int.MaxValue)
                        x = xNext;
                    else
                    {
                        rgraw.Add(new Push((int) x));
                        x = ich;
                    }
                }
                rgraw.Add(new Push((int) x));
            });

            pushAll(rgich);

            rgraw.AddRange(new IRaw[]
            {
                new Push(1),
                new PushJump("jmpStart", "start"),
                new Label("jmpStart", new Jgz()),
            });

            foreach(var t in rgser.Select((ser, iser) => new {ser, iser}))
            {
                var iFirst = rgraw.Count;
                pushAll(t.ser.rgichSer);
                rgraw.AddRange(new IRaw[]
                {
                    new Push(1),
                    new PushJump("jmpStart" + t.iser, "start"),
                    new Label("jmpStart" + t.iser, new Jgz()),
                });

                rgraw[iFirst] = new Label("ser" + t.iser, (Push) rgraw[iFirst]);
            }

            rgraw.AddRange(new IRaw[]
            {
                new Label("start", new Push(-1)),
                new Read(),
                new PushJump("jmpPoz", "div"),
                new Label("jmpPoz", new Jgz()),
                
                new Push(-1),
                new Read(),
                new Push(-1),
                new Mul(),
                new PushJump("jmpEnd", "end"),
                new Label("jmpEnd", new Jgz()),
                
                new Add(),
                
                new Push(1),
                new PushJump("jmpBack", "start"),
                new Label("jmpBack", new Jgz()),
                
                new Label("div", new Push(limAll)),
                new Div(),
                
                new Read(),

                new Push(-1),
                new Read(),
                new Push(1),
                new Add(),
                new PushJump("jmpOutch", "outch"),
                new Label("jmpOutch", new Jgz()),

                new Push(-1),
                new Mul(),
                new Push(-1),
                new Read(),
                new Push(-1),
                new Mul(),
                new Label("jmpSer", new Jgz()),

                new Label("outch", new Out()),
                new Push(1),
                new PushJump("nextinw", "start"),
                new Label("nextinw", new Jgz()),
                
                new Label("end", new Push(0)),
            });
            return RgstmFromRgraw(rgraw);
        }

        private Ser SerMaxGet(int[] rgich, IEnumerable<Ser> rgser)
        {
            Ser serMax = null;

            var setMax = new Action<Ser>(ser =>
            {
                if(ser == null)
                    return;
                if(serMax == null || serMax.Scr < ser.Scr)
                    serMax = ser;
            });

            foreach(var ser in rgser)
            {
                if(ser.rgiich.Count<2)
                    continue;
                setMax(ser);
                setMax(SerMaxGet(rgich, ser.MpserByIchNext(rgich).Values));
            }
            return serMax;
        }

        private List<Stm> RgstmFromRgraw(List<IRaw> rgraw)
        {
            var mpistmByStId = new Dictionary<string, int>();
            return rgraw.Select((raw, istm) =>
            {
                if (!(raw is Label))
                    return raw;
                var label = (Label)raw;

                Debug.Assert(!mpistmByStId.ContainsKey(label.stId));

                mpistmByStId[label.stId] = istm;

                return label.stm;
            }).ToArray().Select(raw =>
            {
                if (!(raw is PushJump))
                    return raw;

                var pj = (PushJump)raw;
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

        private interface IRaw
        {
             
        }

        private class Label : IRaw
        {
            public readonly string stId;
            public readonly Stm stm;

            public Label(string stId, Stm stm)
            {
                this.stId = stId;
                this.stm = stm;
            }
        }

        private class PushJump : IRaw
        {
            public readonly string stJgz;
            public readonly string stTarget;

            public PushJump(string stJgz, string stTarget)
            {
                this.stJgz = stJgz;
                this.stTarget = stTarget;
            }
        }

        private abstract class Stm : IRaw
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

        private double ScoreGet(List<Stm> rgstm)
        {
            var size = rgstm.Sum(stm => stm.size);
            var a = new double[] {0, 5500, 5280, 9080, 17900, 20000, 25500, 40000, 41000, 58000, 65000};
            var b = new double[] {0, 3500, 3600, 6000, 10000, 10000, 15000, 25000, 27000, 34000, 32000};

            return 20 + 80 * (a[IdProblem] - size) / (a[IdProblem] - b[IdProblem]);
        }
    }

}
