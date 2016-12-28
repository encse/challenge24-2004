using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest14.C;
using Cmn.Util;

namespace Ch24.Contest14.D
{
    public class DVisualProgrammingSolver : Contest.Solver
    {
        public class R
        {
            public int v;

            public R(int v)
            {
                this.v = v;
            }

            public override string ToString()
            {
                return string.Format("r{0}", v);
            }
        }

        public class Tio
        {
            public List<byte> rginput = new List<byte>();
            public List<byte> rgoutput = new List<byte>();
        }

        public abstract class Sol
        {
            public List<Poly> Rgpoly = new List<Poly>();
            
            public Sol()
            {
                process(RgstmCreate());

            }

            protected abstract List<IStm> RgstmCreate();
            public abstract IEnumerable<Tio> RgtioCreate();
            
            //r[a] = (y + r[b] + r[c] * r[d] + r[r[f].low4()]) * x / (t + r[e]) + z;

            protected Poly div(R rTo, R rFrom1, byte v)
            {
                return new Poly(string.Format("{0} = {1} / {2}", rTo, rFrom1, v))
                {
                    a = rTo.v,
                    b = rFrom1.v,
                    c = r0.v,
                    d = r0.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) (ulong)v
                };
            }

            protected Poly mul(R rTo, R rFrom1, byte v)
            {
                return new Poly(string.Format("{0} = {1} * {2}", rTo, rFrom1, v))
                {
                    a = rTo.v,
                    b = rFrom1.v,
                    c = r0.v,
                    d = r0.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) (ulong)v,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) 1
                };
            }

            protected Poly sum(R rTo, R rFrom1, int v)
            {
                return new Poly(string.Format("{0} = {1} + {2}", rTo, rFrom1, v))
                {
                    a = rTo.v,
                    b = rFrom1.v,
                    c = r0.v,
                    d = r0.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) v,
                    t8 = (V32) 1
                };
            }

            protected Poly div(R rTo, R rFrom1, R rFrom2)
            {
                return new Poly(string.Format("{0} = {1} / {2}", rTo, rFrom1, rFrom2))
                {
                    a = rTo.v,
                    b = rFrom1.v,
                    c = r0.v,
                    d = r0.v,
                    e = rFrom2.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) 0
                };
            }

            protected Poly mul(R rTo, R rFrom1, R rFrom2)
            {
                return new Poly(string.Format("{0} = {1} * {2}", rTo, rFrom1, rFrom2))
                {
                    a = rTo.v,
                    b = r0.v,
                    c = rFrom1.v,
                    d = rFrom2.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) 1
                };
            }

            protected Poly div256(R rTo, R rFrom)
            {
                return new Poly(string.Format("{0} = {1} / 256", rTo, rFrom))
                {
                    a = rTo.v,
                    b = rFrom.v,
                    c = r0.v,
                    d = r0.v,
                    e = r1.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) 255
                };
            }

            protected Poly sum(R rTo, R rFrom1, R rFrom2, string n = null)
            {
                return new Poly(n ?? string.Format("{0} = {1} + {2}", rTo, rFrom1, rFrom2))
                {
                    a = rTo.v,
                    b = rFrom1.v,
                    c = rFrom2.v,
                    d = r1.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) 0,
                    t8 = (V32) 1
                };
            }
            
            protected Poly assign(R rTo, R rFrom)
            {
                return sum(rTo, rFrom, r0, (string.Format("{0} = {1}", rTo, rFrom)));
            }

            protected Poly assign(R rTo, int v, string n = null)
            {
                return new Poly(n ?? string.Format("{0} = {1}", rTo, v))
                {
                    a = rTo.v,
                    b = r0.v,
                    c = r0.v,
                    d = r0.v,
                    e = r0.v,
                    f = r0.v,
                    x8 = (V32) 1,
                    y8 = (V32) 0,
                    z16 = (V32) v,
                    t8 = (V32) 1
                };
            }

            protected Poly nop()
            {
                return assign(r0, 0, "nop");
            }

            protected Poly eof()
            {
                return assign(rIO, 0x100, "eof");
            }

            protected Rgstm error()
            {
                return new Rgstm{
                    assign(rIO, 0x45, "error"),
                    assign(rIO, 0x52),
                    assign(rIO, 0x52),
                    assign(rIO, 0x4f),
                    assign(rIO, 0x52),
                    assign(rIO, 0x0a),
                    eof()
                };
            }

            protected void seq(params Poly[] rgpoly)
            {
                for(var i = 0; i < rgpoly.Length-1; i++)
                {
                    var poly1 = rgpoly[i];
                    var poly2 = rgpoly[i+1];
                    lnk(poly1, vMax, r0, 0, poly2);
                }
            }

            protected void lnk(Poly poly1, int min, R r1, int limit, Poly poly2)
            {
                var link = new Link
                {
                    poly1 = poly1,
                    q1 = r1.v,
                    l = (V32) min,
                    h = (V32) limit,
                    poly2 = poly2,
                    q2 = rMAX.v
                };
                poly1.rglink.Add(poly1.rglink.Count, link);
                poly2.rglink.Add(poly2.rglink.Count, link);
            }

            protected void process(List<IStm> rgstmRgLblGt)
            {
                var rgstmLblGt = removerg(rgstmRgLblGt).ToList();

                var rgstmGt = new List<IStm>();
                var mppolyByLbln = new Dictionary<string, Poly>();
                foreach(var vistm in rgstmLblGt.Select((v,i)=>new{v,i}))
                {
                    if(vistm.v is Lbl)
                    {
                        var poly = (Poly) rgstmLblGt[vistm.i + 1];
                        mppolyByLbln[((Lbl) vistm.v).n] = poly;
                    }
                    else
                    {
                        rgstmGt.Add(vistm.v);
                    }
                }

                var rgpoly = new List<Poly>();

                foreach(var vistm in rgstmGt.Select((v,i)=>new{v,i}))
                {
                    if(vistm.v is GotoIf)
                    {
                        var gotoIf = (GotoIf) vistm.v;
                        var poly1 = (Poly)vistm.i.Eni().Select(d => rgstmGt[vistm.i - d]).First(stm => stm is Poly);
                        lnk(poly1, gotoIf.min, gotoIf.r, gotoIf.limit, mppolyByLbln[gotoIf.lbln]);
                    }
                    else
                    {
                        rgpoly.Add((Poly) vistm.v);
                    }
                }

                seq(rgpoly.ToArray());
                Rgpoly = rgpoly;
            }

            private IEnumerable<IStm> removerg(IEnumerable<IStm> enstm)
            {
                foreach(var stm in enstm)
                    if(stm is Rgstm)
                        foreach(var stm1 in removerg((Rgstm) stm))
                            yield return stm1;
                    else
                        yield return stm;
            }
        }

        private const int vMax = 0xffff;

        private static R r0 = new R(0);
        private static R r1 = new R(1);
        private static R rIO = new R(15);
        private static R rMAX = new R(14);
        private static R rw1 = new R(13);
        private static R rw2 = new R(12);
        private static R rw3 = new R(11);
        private static R rw4 = new R(10);
        private static R rw5 = new R(09);
        private static R rw6 = new R(08);
        private static R rw7 = new R(07);
        private static R rw8 = new R(06);
        private static R rw9 = new R(05);
        private static R rw10 = new R(04);

        public interface IStm
        {
        }

        public class Lbl : IStm
        {
            public readonly string n;

            public Lbl(string n)
            {
                this.n = n;
            }
        }

        public class GotoIf : IStm
        {
            public readonly int min;
            public readonly R r;
            public readonly int limit;
            public readonly string lbln;

            public GotoIf(int min, R r, int limit, string lbln)
            {
                this.min = min;
                this.r = r;
                this.limit = limit;
                this.lbln = lbln;
            }

            public GotoIf(int min, R r, string lbln) : this(min, r, min+1, lbln)
            {
                
            }

            public GotoIf(string lbln) : this(0, r0, vMax, lbln)
            {
                
            }
        }

        public class Rgstm : List<IStm>, IStm
        {
            public Rgstm()
            {
            }

            public Rgstm(IEnumerable<IStm> collection) : base(collection)
            {
            }
        }

        public class SolAddBytes : Sol
        {
            const byte vPlus = 0x2B;

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rSum = rw2;
                var rOp = rw3;

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    assign(rSum, 0),
                    new Lbl("loop"),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "ok"),
                    new Lbl("err"),
                    error(),
                    new Lbl("ok"),
                    sum(rSum, rSum, rRead),
                    assign(rOp, rIO),
                    new GotoIf(vPlus, rOp, "loop"),
                    new GotoIf(0, rOp, 256, "err"),
                    div256(rIO, rSum),
                    assign(rRead, 256),
                    mul(rSum, rSum, rRead),
                    div256(rIO, rSum),
                    eof()
                };
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var rgerror = new List<byte> {0x45, 0x52, 0x52, 0x4F, 0x52, 0x0A};

                yield return new Tio
                {
                    rginput = new List<byte> {1},
                    rgoutput = new List<byte> {0, 1}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, 0, 55},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, vPlus, 55, vPlus},
                    rgoutput = rgerror
                };

                var rand = new Random(0);
                for(var i = 1; i < 100; i++)
                {
                    var tio = new Tio();
                    int s = 0;
                    tio.rginput.Add((byte) rand.Next(256));
                    s += tio.rginput.Last();
                    for(int j = 0; j < i; j++)
                    {
                        tio.rginput.Add(vPlus);
                        tio.rginput.Add((byte) rand.Next(256));
                        s += tio.rginput.Last();
                    }

                    tio.rgoutput = new List<byte> {(byte) (s / 256), (byte) (s % 256)};

                    yield return tio;
                }
            }
        }

        public class SolAddSubBytes : Sol
        {
            const byte vPlus = 0x2B;
            const byte vMinus = 0x2D;

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rSum = rw2;
                var rOp = rw3;

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    assign(rSum, 0),
                    assign(rOp, vPlus),
                    new Lbl("loop"),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "doop"),
                    new Lbl("err"),
                    error(),
                    new Lbl("doop"),
                    nop(),
                    new GotoIf(vPlus, rOp, "plus"),
                    mul(rRead, rRead, rMAX),
                    new Lbl("plus"),
                    sum(rSum, rSum, rRead),
                    assign(rOp, rIO),
                    new GotoIf(vPlus, rOp, "loop"),
                    new GotoIf(vMinus, rOp, "loop"),
                    new GotoIf(0, rOp, 256, "err"),
                    div256(rIO, rSum),
                    assign(rRead, 256),
                    mul(rSum, rSum, rRead),
                    div256(rIO, rSum),
                    eof()
                };
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var rgerror = new List<byte> {0x45, 0x52, 0x52, 0x4F, 0x52, 0x0A};

                yield return new Tio
                {
                    rginput = new List<byte> {1},
                    rgoutput = new List<byte> {0, 1}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, 0, 55},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, vPlus, 55, vPlus},
                    rgoutput = rgerror
                };

                var rand = new Random(0);
                for(var i = 1; i < 100; i++)
                {
                    var tio = new Tio();
                    int s = 0;
                    tio.rginput.Add((byte) rand.Next(256));
                    s += tio.rginput.Last();
                    for(int j = 0; j < i; j++)
                    {
                        int vOp = rand.Next(2) == 0 ? vPlus : vMinus;

                        var bNext = (byte) rand.Next(256);
                        switch(vOp)
                        {
                            case vPlus:
                                s += bNext;
                                tio.rginput.Add(vPlus);
                                break;
                            case vMinus:
                                s -= bNext;
                                tio.rginput.Add(vMinus);
                                break;
                        }
                        tio.rginput.Add(bNext);
                    }
                    Console.WriteLine(s);
                    tio.rgoutput = new List<byte> {(byte) ((s >> 8) & 0xff), (byte) (s & 0xff)};

                    yield return tio;
                }
            }
        }

        public class SolAllOpBytes : Sol
        {
            const byte vPlus = 0x2B;
            const byte vMinus = 0x2D;
            const byte vMul = (byte) '*';
            const byte vDiv = (byte) '/';

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rSum = rw2;
                var rOp = rw3;

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    assign(rSum, 0),
                    assign(rOp, vPlus),
                    new Lbl("loop"),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "doop"),
                    new Lbl("err"),
                    error(),
                    new Lbl("doop"),
                    nop(),
                    new GotoIf(vPlus, rOp, "plus"),
                    new GotoIf(vMul, rOp, "mul"),
                    new GotoIf(vDiv, rOp, "div"),
                    mul(rRead, rRead, rMAX),
                    new Lbl("plus"),
                    sum(rSum, rSum, rRead),
                    new GotoIf("readop"),
                    new Lbl("mul"),
                    mul(rSum, rSum, rRead),
                    new GotoIf("readop"),
                    new Lbl("div"),
                    nop(),
                    new GotoIf(0, rRead, "err"),
                    new GotoIf(0, rSum, 1 << 15, "divpos"),
                    mul(rSum, rSum, rMAX),
                    div(rSum, rSum, rRead),
                    mul(rSum, rSum, rMAX),
                    new GotoIf("readop"),
                    new Lbl("divpos"),
                    div(rSum, rSum, rRead),
                    new Lbl("readop"),
                    assign(rOp, rIO),
                    new GotoIf(vPlus, rOp, "loop"),
                    new GotoIf(vMinus, rOp, "loop"),
                    new GotoIf(vMul, rOp, "loop"),
                    new GotoIf(vDiv, rOp, "loop"),
                    new GotoIf(0, rOp, 256, "err"),
                    div256(rIO, rSum),
                    assign(rRead, 256),
                    mul(rSum, rSum, rRead),
                    div256(rIO, rSum),
                    eof()
                };
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var rgerror = new List<byte> {0x45, 0x52, 0x52, 0x4F, 0x52, 0x0A};

                yield return new Tio
                {
                    rginput = new List<byte> {0, vMinus, 100, vDiv, 7},
                    rgoutput = new List<byte>{((-100/7)>>8) & 0xff, (-100/7) & 0xff}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {1},
                    rgoutput = new List<byte> {0, 1}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, 0, 55},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, vPlus, 55, vPlus},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, vDiv, 0},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte> {100, vDiv, 7},
                    rgoutput = new List<byte>{0, 100/7}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {9, vMul, 7},
                    rgoutput = new List<byte>{0, 9*7}
                };

                var rand = new Random(0);
                for(var i = 1; i < 100; i++)
                {
                    var tio = new Tio();
                    int s = 0;
                    tio.rginput.Add((byte) rand.Next(256));
                    s += tio.rginput.Last();
                    for(var j = 0; j < i; j++)
                    {
                        byte vOp;
                        byte bNext;
                        int sT;
                        for(;;)
                        {
                            vOp = new[] {vPlus, vMinus, vMul, vDiv}[rand.Next(4)];

                            for(;;)
                            {
                                bNext = (byte) rand.Next(256);
                                if(vOp != vDiv || bNext != 0)
                                    break;
                            }

                            sT = s;
                            switch(vOp)
                            {
                                case vPlus:
                                    sT += bNext;
                                    break;
                                case vMinus:
                                    sT -= bNext;
                                    break;
                                case vMul:
                                    sT *= bNext;
                                    break;
                                case vDiv:
                                    sT /= bNext;
                                    break;
                                default:
                                    throw new Exception();
                            }

                            if(-32768<=sT && sT <= 32767)
                                break;
                        }

                        tio.rginput.Add(vOp);
                        tio.rginput.Add(bNext);
                        
                        s = sT;
                    }
                    Console.WriteLine(s);
                    tio.rgoutput = new List<byte> {(byte) ((s >> 8) & 0xff), (byte) (s & 0xff)};

                    yield return tio;
                }
            }
        }

        public class SolReadNum : Sol
        {
            const byte v0 = (byte) '0';

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rNum = rw4;

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    assign(rNum, 0),
                    new Lbl("numloop"),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "donum"),
                    assign(rIO, rNum),
                    new Lbl("eof"),
                    eof(),
                    new Lbl("donum"),
                    sum(rRead, rRead, vMax + 1 - v0),
                    new GotoIf(0, rRead, 10, "num"),
                    new GotoIf("eof"),
                    new Lbl("num"),
                    mul(rNum, rNum, 10),
                    sum(rNum, rNum, rRead),
                    new GotoIf("numloop")
                };
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                yield return new Tio
                {
                    rginput = new List<byte>(),
                    rgoutput = new List<byte>{0}
                };

                yield return new Tio
                {
                    rginput = "25#".Select(ch => (byte)ch).ToList(),
                    rgoutput = new List<byte>{}
                };

                yield return new Tio
                {
                    rginput = "25a".Select(ch => (byte)ch).ToList(),
                    rgoutput = new List<byte>{}
                };

                var rand = new Random(0);
                for(var i = 0; i < 256; i++)
                {
                    yield return new Tio
                    {
                        rginput = i.ToString().Select(ch => (byte)ch).ToList(),
                        rgoutput = new List<byte>{(byte) i}
                    };

                    yield return new Tio
                    {
                        rginput = rand.Next(1,10).Eni().Select(_ => v0).Concat(i.ToString().Select(ch => (byte)ch)).ToList(),
                        rgoutput = new List<byte>{(byte) i}
                    };

                }
            }
        }

        public class SolPrintNum : Sol
        {
            const byte v0 = (byte) '0';
            const byte vNL = 10;

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rSum = rw2;
                var rMul = rw3;
                var rDigit = rw4;

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    new Lbl("next"),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "digit1"),
                    eof(),
                    new Lbl("digit1"),
                    assign(rMul, 256),
                    mul(rSum, rRead, rMul),
                    assign(rRead, rIO),
                    new GotoIf(0, rRead, 256, "digit2"),
                    error(),
                    new Lbl("digit2"),
                    sum(rSum, rSum, rRead),
                    assign(rMul, 10000),
                    new Lbl("printdigit"),
                    div(rDigit, rSum, rMul),
                    sum(rIO, rDigit, v0),
                    mul(rDigit, rDigit, rMul),
                    mul(rDigit, rDigit, rMAX),
                    sum(rSum, rSum, rDigit),
                    div(rMul, rMul, 10),
                    new GotoIf(0, rMul, "printnl"),
                    new GotoIf("printdigit"),
                    new Lbl("printnl"),
                    assign(rIO, vNL),
                    new GotoIf("next"),
                };
            }

            private List<byte> x(int n)
            {
                return new List<byte>{ (byte) ((n >> 8) & 0xff), (byte) (n & 0xff)};
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var rgerror = new List<byte> {0x45, 0x52, 0x52, 0x4F, 0x52, 0x0A};

                yield return new Tio
                {
                    rginput = new List<byte>{1,2},
                    rgoutput = "00258\n".Select(ch => (byte)ch).ToList()
                };

                yield return new Tio
                {
                    rginput = new List<byte>(),
                    rgoutput = new List<byte>{}
                };

                yield return new Tio
                {
                    rginput = new List<byte>{1},
                    rgoutput = rgerror
                };

                yield return new Tio
                {
                    rginput = new List<byte>{1,2,3},
                    rgoutput = "00258\n".Select(ch => (byte)ch).Concat(rgerror).ToList()
                };

                for(var i=0;i<65536;i++)
                {
                    yield return new Tio
                    {
                        rginput = x(i),
                        rgoutput = string.Format("{0,5:D5}\n", i).Select(ch => (byte) ch).ToList()
                    };
                }

                var rand = new Random(0);
                for(var i = 2; i < 100; i++)
                {
                    var tio = new Tio();
                    for(var j = 0;j<i;j++)
                    {
                        var n = rand.Next(65536);
                        tio.rginput.AddRange(x(n));
                        tio.rgoutput.AddRange(string.Format("{0,5:D5}\n", n).Select(ch => (byte) ch));
                    }
                    yield return tio;
                }
            }
        }

        public class SolAllOpNum : Sol
        {
            const byte vPlus = 0x2B;
            const byte vMinus = 0x2D;
            const byte vMul = (byte) '*';
            const byte vDiv = (byte) '/';

            const byte v0 = (byte) '0';
            const byte vNL = 10;

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rNum = rw4;
                var rSum = rw2;
                var rOp = rw3;

                R rMul = rRead;
                R rDigit = rOp;


                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    assign(rSum, 0),
                    assign(rOp, vPlus),
                    new Lbl("loop"),
                    assign(rNum, 0),
                    assign(rRead, rIO),
                    new GotoIf(vNL, rRead, "error"),
                    new GotoIf(0, rRead, 256, "donum"),
                    new GotoIf("error"),

                    new Lbl("donum"),
                    sum(rRead, rRead, vMax + 1 - v0),
                    new GotoIf(0, rRead, 10, "num"),
                    new GotoIf("error"),
                    new Lbl("num"),
                    mul(rNum, rNum, 10),
                    sum(rNum, rNum, rRead),
                    assign(rRead, rIO),
                    new GotoIf(vNL, rRead, "numreadend"),
                    new GotoIf(0, rRead, 256, "donum"),
                    new GotoIf("error"),

                    new Lbl("numreadend"),

                    nop(),
                    new GotoIf(vPlus, rOp, "plus"),
                    new GotoIf(vMul, rOp, "mul"),
                    new GotoIf(vDiv, rOp, "div"),
                    //rOp == vMinus
                    mul(rNum, rNum, rMAX),
                    new Lbl("plus"),
                    sum(rSum, rSum, rNum),
                    new GotoIf("readop"),
                    new Lbl("mul"),
                    mul(rSum, rSum, rNum),
                    new GotoIf("readop"),
                    new Lbl("div"),
                    nop(),
                    new GotoIf(0, rNum, "error"),
                    new GotoIf(0, rSum, 1 << 15, "divpos"),
                    mul(rSum, rSum, rMAX),
                    div(rSum, rSum, rNum),
                    mul(rSum, rSum, rMAX),
                    new GotoIf("readop"),
                    new Lbl("divpos"),
                    div(rSum, rSum, rNum),
                    new Lbl("readop"),
                    assign(rOp, rIO),
                    new GotoIf(0, rOp, 256, "readnlafterop"),
                    new GotoIf("print"),
                    new Lbl("readnlafterop"),
                    assign(rRead, rIO),
                    new GotoIf(vNL, rRead, "switchop"),
                    new GotoIf("error"),
                    new Lbl("switchop"),
                    nop(),
                    new GotoIf(vPlus, rOp, "loop"),
                    new GotoIf(vMinus, rOp, "loop"),
                    new GotoIf(vMul, rOp, "loop"),
                    new GotoIf(vDiv, rOp, "loop"),
                    new GotoIf("error"),
                    
                    new Lbl("print"),

                    assign(rMul, 10000),
                    new Lbl("printdigit"),
                    div(rDigit, rSum, rMul),
                    sum(rIO, rDigit, v0),
                    mul(rDigit, rDigit, rMul),
                    mul(rDigit, rDigit, rMAX),
                    sum(rSum, rSum, rDigit),
                    div(rMul, rMul, 10),
                    new GotoIf(0, rMul, "printeof"),
                    new GotoIf("printdigit"),
                    new Lbl("printeof"),
                    assign(rIO, vNL),
                    eof(),

                    new Lbl("error"),
                    error(),
                };
            }

            private Tio TioCreate(string stin, string stout)
            {
                //Console.WriteLine("'{0}' = '{1}'", stin, stout);

                return new Tio
                {
                    rginput = stin.Replace(' ', '\n').Select(ch => (byte) ch).ToList(),
                    rgoutput = (stout+"\n").Select(ch => (byte) ch).ToList()
                };
            }
            
            Random rand = new Random(0);

            private string stFromN(int n)
            {
                return new string('0', rand.Next(3)) + n.ToString()+" ";
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var sterr = "ERROR";

                yield return TioCreate("", sterr);
                yield return TioCreate(" ", sterr);
                yield return TioCreate("1 ", "00001");
                yield return TioCreate("11111 ", "11111");
                yield return TioCreate("0011111 ", "11111");
                yield return TioCreate("1", sterr);
                yield return TioCreate("1+ 1 ", sterr);
                yield return TioCreate("1 +", sterr);
                yield return TioCreate("1 + ", sterr);
                yield return TioCreate("1 + 1", sterr);
                yield return TioCreate("1 + 1 ", "00002");
                yield return TioCreate("1 % 1 ", sterr);
                yield return TioCreate("1 +1 ", sterr);
                yield return TioCreate("100 / 1 ", "00100");
                yield return TioCreate("100 / 0 ", sterr);

                for(var i = 1; i < 1000; i++)
                {
                    var s = rand.Next(32767);
                    var stin = stFromN(s);
                    for(var j = 0; j < i; j++)
                    {
                        byte vOp;
                        int next;
                        int sT;
                        for(;;)
                        {
                            vOp = new[] {vPlus, vMinus, vMul, vDiv}[rand.Next(4)];

                            for(;;)
                            {
                                next = rand.Next(32767);
                                if(vOp != vDiv || next != 0)
                                    break;
                            }

                            sT = s;
                            switch(vOp)
                            {
                                case vPlus:
                                    sT += next;
                                    break;
                                case vMinus:
                                    sT -= next;
                                    break;
                                case vMul:
                                    sT *= next;
                                    break;
                                case vDiv:
                                    sT /= next;
                                    break;
                                default:
                                    throw new Exception();
                            }

                            if(-32768<=sT && sT <= 32767)
                                break;
                        }

                        stin += string.Format("{0} {1}", (char) vOp, stFromN(next));

                        s = sT;
                    }
                    yield return TioCreate(stin, string.Format("{0,5:D5}", (65536 + s) % 65536));
                }
            }
        }

        public class SolAddSub64 : Sol
        {
            const byte vPlus = 0x2B;
            const byte vMinus = 0x2D;

            protected override List<IStm> RgstmCreate()
            {
                var rRead = rw1;
                var rOp = rw2;
                var rgrSum =new[]{rw10,rw9,rw8,rw7,rw6,rw5,rw4,rw3};

                return new List<IStm>
                {
                    assign(rMAX, vMax),
                    new Rgstm(8.Eni().Select(i => assign(rgrSum[i],0))),
                    assign(rOp, vPlus),
                    
                    new Lbl("read"),
                    new Rgstm(8.Eni().SelectMany(i => new IStm[]
                    {
                        assign(rRead, rIO),
                        new GotoIf(0, rRead, 256, "digit"+i),
                        new GotoIf("error"),
                        new Lbl("digit"+i),
                        nop(),
                        new GotoIf(vPlus, rOp, "plus"+i),
                        mul(rRead, rRead, rMAX),
                        sum(rRead, rRead, 65536-255*256-1),
                        new Lbl("plus"+i),
                        sum(rgrSum[i], rgrSum[i], rRead),
                        new GotoIf(0, rgrSum[i], 256, "nextdigit"+i),
                        new Rgstm(i == 7 ? new IStm[0] : new IStm[]{sum(rgrSum[i+1],rgrSum[i+1],1)}), 
                        sum(rgrSum[i], rgrSum[i], 65536-256),
                        new Lbl("nextdigit"+i), 
                    })),

                    assign(rOp, rIO),
                    new GotoIf(vPlus, rOp, "read"),
                    new GotoIf(vMinus, rOp, "add1"),
                    new GotoIf(0, rOp, 256, "error"),

                    new Rgstm(8.Eni().Select(i => assign(rIO, rgrSum[i]))),
                    eof(),
                    new Lbl("add1"),
                    sum(rgrSum[0], rgrSum[0], 1),
                    new GotoIf("read"),
                    new Lbl("error"),
                    error(),
                };
            }

            Random rand = new Random(0);

            private decimal nextRand()
            {
                return 64.Eni().Aggregate((decimal)0, (current, i) => 2 * current + rand.Next(2));
            }

            private IEnumerable<byte> to64(decimal n)
            {
                foreach(var i in 8.Eni())
                {
                    yield return (byte) (n % 256);
                    n /= 256;
                }
            }

            public override IEnumerable<Tio> RgtioCreate()
            {
                var rgerr = "ERROR\n".Select(ch => (byte)ch).ToList();

                yield return new Tio
                {
                    rginput = new List<byte> {},
                    rgoutput = rgerr
                };

                yield return new Tio
                {
                    rginput = new List<byte> {1,2,3,4,5,6,7},
                    rgoutput = rgerr
                };

                yield return new Tio
                {
                    rginput = new List<byte> {1,2,3,4,5,6,7,8},
                    rgoutput = new List<byte> {1,2,3,4,5,6,7,8}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {1,2,3,4,5,6,7,8, vPlus, 1,2,3,4,5,6,7,8},
                    rgoutput = new List<byte> {2,4,6,8,10,12,14,16}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {2,4,6,8,10,12,14,16, vMinus, 1,2,3,4,5,6,7,8},
                    rgoutput = new List<byte> {1,2,3,4,5,6,7,8}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {1,2,3,4,5,6,7,8, 0, 1,2,3,4,5,6,7,8},
                    rgoutput = rgerr
                };

                yield return new Tio
                {
                    rginput = new List<byte> {0x86, 0x47, 0xF0, 0x75, 0x12, 0x60, 0x3F, 0x13, 0x2B, 0x7D, 0x0D, 0xE5, 0x60, 0x3F, 0x83, 0xC0, 0x2F},
                    rgoutput = new List<byte>{0x03, 0x55, 0xD5, 0xD6, 0x51, 0xE3, 0xFF, 0x42}
                };

                yield return new Tio
                {
                    rginput = new List<byte> {0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2D, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
                    rgoutput = new List<byte>{0xFC, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}
                };


                decimal mod = 64.Eni().Aggregate((decimal)1, (modT, i) => modT * 2);

                for(var i = 1; i < 1000; i++)
                {
                    var tio = new Tio();
                    var s = nextRand();
                    tio.rginput.AddRange(to64(s));
                    for(var j = 0; j < i; j++)
                    {
                        byte vOp;
                        decimal next = nextRand();
                            vOp = new[] {vPlus, vMinus}[rand.Next(2)];

                            switch(vOp)
                            {
                                case vPlus:
                                    s = (s + next) % mod;
                                    break;
                                case vMinus:
                                    s = (s + mod - next) % mod;
                                    break;
                                default:
                                    throw new Exception();
                            }

                        tio.rginput.Add(vOp);
                        tio.rginput.AddRange(to64(next));
                    }
                    tio.rgoutput = to64(s).ToList();
                    yield return tio;
                }
            }
        }

        public override void Solve()
        {
            Sol sol;
            switch(IdProblem+1)
            {
                case 1:sol = new SolAddBytes();break;
                case 2:sol = new SolAddSubBytes();break;
                case 3:sol = new SolAllOpBytes();break;
                case 4:sol = new SolReadNum();break;
                case 5:sol = new SolPrintNum();break;
                case 6:sol = new SolAllOpNum();break;
                case 7:
                case 8:
                case 9:
                    return;
                case 10:sol = new SolAddSub64();break;
                default:
                    throw new Exception();
            }


            bool fDebug = IdProblem == -1;

            if(fDebug)
                Console.WriteLine(sol.Rgpoly.Select(poly => poly.TSTOFull()).StJoin(Environment.NewLine));

            foreach(var tio in sol.RgtioCreate())
            {
                var vm = new Vpvm(sol.Rgpoly.First(), tio.rginput, fDebug);
                var cstep = 0;
                for(;!vm.FEnded();)
                {
                    vm.Step();
                    cstep++;
                }
                
                var rgout = vm.Enoutput().ToArray();
                if(!rgout.SequenceEqual(tio.rgoutput))
                    throw new Exception("AJAJAJAJAJAJJ");
            }


        }

    }
}
