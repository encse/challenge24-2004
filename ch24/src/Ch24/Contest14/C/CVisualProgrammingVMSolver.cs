using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ch24.Contest;
using Ch24.Contest14.D;
using Cmn.Util;

namespace Ch24.Contest14.C
{
        public class V32
        {
            private const ulong m = 4294967296L;
            private readonly ulong v;

            private V32(ulong v)
            {
                this.v = v;
            }

            public static explicit operator V32(ulong v)
            {
                return new V32(v);
            }

            public static explicit operator V32(long v)
            {
                if(v<0)
                    throw new Exception();
                if(65535<v)
                    throw new Exception();
                return (V32)(ulong)v;
            }

            public static explicit operator V32(int v)
            {
                return (V32)(ulong)v;
            }

            public static explicit operator byte(V32 v)
            {
                if(v>(V32)255L)
                    throw new Exception();
                return (byte)v.v;
            }

            public static V32 operator+ (V32 a, V32 b)
            {
                return (V32) ((a.v + b.v) % m);
            }

            public static V32 operator* (V32 a, V32 b)
            {
                return (V32) ((a.v * b.v) % m);
            }

            public static V32 operator- (V32 a, V32 b)
            {
                return (V32) ((a.v - b.v + m) % m);
            }

            public static V32 operator/ (V32 a, V32 b)
            {
                return (V32) ((a.v / b.v + m) % m);
            }

            public static bool operator<= (V32 a, V32 b)
            {
                return a.v <= b.v;
            }

            public static bool operator>= (V32 a, V32 b)
            {
                return a.v >= b.v;
            }

            public static bool operator< (V32 a, V32 b)
            {
                return a.v < b.v;
            }

            public static bool operator> (V32 a, V32 b)
            {
                return a.v > b.v;
            }

            public V32 to16()
            {
                return (V32) (v % 65536);
            }

            public int low4()
            {
                return (int) (v & 0xf);
            }

            public override string ToString()
            {
                return v.ToString(CultureInfo.InvariantCulture);
            }
        }

        public interface Rgr
        {
            V32 this[int i] { get; set; }
        }

        public class Vpvm : Rgr
        {
            //registers 0-65535
            public V32[] _r;
            public Rgr r;

            public Poly poly;

            public List<V32> rgoutput;
            public List<byte> rginput;

            public bool fInputRead;
            public int iinput;
            private bool fDebug;
            
            public Vpvm(Poly poly, List<byte> rginput, bool fDebug)
            {
                r = this;
                _r = 15.Eni().Select( i => (V32)i).ToArray();
                this.poly = poly;
                this.rginput = rginput;
                this.fDebug = fDebug;
                iinput = 0;
                rgoutput = new List<V32>();
            }

            public V32 this[int i]
            {
                get
                {
                    if(i < 15)
                        return _r[i];

                    fInputRead = true;
                    return inputNext();
                }
                set
                {
                    V32 v = value.to16();
                    if(i<15)
                    {
                        _r[i] = v;
                    }
                    else
                    {
                        if(v==(V32) 0L)
                            throw new Exception();
                        rgoutput.Add(v);
                    }
                }
            }

            private V32 inputNext()
            {
                if(iinput >= rginput.Count)
                    return (V32) 256L;

                return (V32) (ulong) rginput[iinput];
            }

            public void Step()
            {
                if(fDebug)
                {
                    Console.WriteLine(new string('=', 80));
                    Console.WriteLine(TSTORgr());
                    Console.WriteLine(poly.TSTOName());
                    Console.WriteLine(poly.TSTOReg());
                    Console.WriteLine(poly.TSTORegop(r));
                }
                fInputRead = false;
                poly.RegOp(r);
                if(fDebug)
                {
                    Console.WriteLine(TSTORgr());
                    Console.WriteLine(poly.rglink.Values.Select(linkT => linkT.TSTOJump(poly)+Environment.NewLine+"  --> "+linkT.TSTOJumpOp(poly,r)).StJoin(Environment.NewLine));
                    Console.ReadLine();
                }
                Link link = RouSel();
                poly = link.other(poly);
                if(fInputRead && iinput < rginput.Count)
                {
                    iinput++;
                }
            }

            public bool FEnded()
            {
                return rgoutput.Any() && rgoutput.Last() > (V32) 255L;
            }

            public IEnumerable<byte> Enoutput()
            {
                return rgoutput.TakeWhile(v => v <= (V32) 255L).Select(v => (byte)v);
            }

            private Link RouSel()
            {
                return poly.rglink.Values.FirstOrDefault(link => link.FJump(r, poly) ) ?? poly.rglink.Values.Last();
            }

            public static int high4(int b)
            {
                return low4(b >> 4);
            }

            public static int low4(int b)
            {
                return 0xF & b;
            }

            public string TSTORgr()
            {
                string s = "";
                for(int i = 0; i < 15; i++)
                {
                    if(i>0)
                        s += i % 4 == 0 ? Environment.NewLine : " ";
                    s += string.Format("r{0,2:D2}:{1,5}", i, _r[i]);
                }
                s+=string.Format(" r15:{0,5}", inputNext());
                return s;
            }

        }

        public class Pont : IEquatable<Pont>
        {
            //0-65535
            public readonly int x;
            public readonly int y;

            public Pont(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public long key()
            {
                return y*65536+x;
            }

            public bool Equals(Pont other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return other.x == x && other.y == y;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != typeof(Pont))
                    return false;
                return Equals((Pont) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (x * 397) ^ y;
                }
            }

            public static bool operator ==(Pont left, Pont right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Pont left, Pont right)
            {
                return !Equals(left, right);
            }
        }

        public class Poly : DVisualProgrammingSolver.IStm
        {
            public readonly string n;

            public SortedDictionary<long,Link> rglink=new SortedDictionary<long, Link>();

            public int a;
            public int b;
            public int c;
            public int d;
            public int e;
            public int f;
            public V32 x8;
            public V32 y8;
            public V32 z16;
            public V32 t8;

            public Poly(string n)
            {
                this.n = n;
            }

            public Poly(string n, RGBA stroke, RGBA fill)
            {
                this.n = n;
                a = Vpvm.high4(fill.r);              
                b = Vpvm.low4(fill.r);               
                c = Vpvm.high4(fill.g);              
                d = Vpvm.low4(fill.g);               
                e = Vpvm.high4(fill.b);              
                f = Vpvm.low4(fill.b);               
                x8 = (V32)(255-fill.a);                     
                y8 = (V32)stroke.r;                   
                z16 = (V32)(stroke.g * 256 + stroke.b);
                t8 = (V32)(255-stroke.a);                   

            }



            public string TSTOFull()
            {
                return string.Format("{0}{1}{2}{1}{1}{3}", 
                    TSTOName(), 
                    Environment.NewLine, 
                    TSTOReg(), 
                    rglink.Values.Select(link => link.TSTOJump(this)).StJoin(Environment.NewLine));
            }

            public string TSTOName()
            {
                return string.Format("Poly: {0}", n);
            }

            public string TSTOReg()
            {
                return string.Format("r{0} = ({1} + r{2} + r{3} * r{4} + r[r{5}.low4()]) * {6} / ({7} + r{8}) + {9}", a, y8, b, c, d, f, x8, t8, e, z16);
            }

            public string TSTORegop(Rgr r)
            {
                return 
                    string.Format(" --->  ({1} + {2} + {3} * {4} + r{5}) * {6} / ({7} + {8}) + {9}", a, y8, r[b], r[c], r[d], r[f].low4(), x8, t8, r[e], z16) + 
                    Environment.NewLine + 
                    string.Format(" --->  {0}", RightSide(r).to16());
            }

            public void RegOp(Rgr r)
            {
                r[a] = RightSide(r);
            }

            private V32 RightSide(Rgr r)
            {
                return (y8 + r[b] + r[c] * r[d] + r[r[f].low4()]) * x8 / (t8 + r[e]) + z16;
            }
        }

        public class Link
        {
            public Poly poly1;
            public Poly poly2;
            public int q1;
            public int q2;
            public V32 l;
            public V32 h;

            public Link()
            {
            }

            public Link(Pont pont1, Poly poly1, Pont pont2, Poly poly2, RGBA color)
            {
                this.poly1 = poly1;
                this.poly2 = poly2;
                q1 = Vpvm.low4(pont1.x);              
                q2 = Vpvm.low4(pont2.x);              
                l = (V32) (color.r * 256 + color.g);
                h = (V32) (color.b * 256 + color.a);
            }

            public bool FJump(Rgr r, Poly poly)
            {
                var q = qGet(poly);
                return l <= r[q] && r[q] < h;
            }

            public string TSTOJump(Poly poly)
            {
                var q = qGet(poly);
                return string.Format("if({0} <= r{1} < {2}) goto {3}", l, q, h, other(poly).TSTOName());
            }

            public string TSTOJumpOp(Poly poly, Rgr r)
            {
                var q = qGet(poly);
                return string.Format("if({0} <= {1} < {2}) goto {3}", l, r[q], h, other(poly).TSTOName());
            }

            private int qGet(Poly poly)
            {
                int q;
                if(poly == poly1)
                    q = q1;
                else if(poly == poly2)
                    q = q2;
                else
                    throw new Exception();
                return q;
            }

            public Poly other(Poly poly)
            {
                if(poly == poly1)
                    return poly2;
                if(poly == poly2)
                    return poly1;
                throw new Exception();
            }
        }

        public class Line
        {
            public RGBA color;

            //2-16
            public List<Pont> rgpont;
        }

    public class CVisualProgrammingVMSolver : Contest.Solver
    {

        public override void Solve()
        {
            var rgpe = Fetch<int[]>();
            var mppolyByPont = new Dictionary<Pont, Poly>();
            var rgpoly = rgpe[0].Eni().Select(ipoly =>
            {
                var rgp = Fetch<int[]>();
                var poly = new Poly(
                    n: (ipoly + 1).ToString(),
                    stroke: new RGBA
                    {
                        r = rgp[0],
                        g = rgp[1],
                        b = rgp[2],
                        a = rgp[3],
                    },
                    fill: new RGBA
                    {
                        r = rgp[4],
                        g = rgp[5],
                        b = rgp[6],
                        a = rgp[7],
                    });
                for(int i = 9; i < rgp.Count(); i += 2)
                    mppolyByPont.Add(new Pont(rgp[i], rgp[i + 1]), poly);
                return poly;
            }).ToList();

            foreach(var _ in rgpe[1].Eni())
            {
                var rgl = Fetch<int[]>();

                var rgpont = new List<Pont>();
                for(int i = 5; i < rgl.Count(); i += 2)
                    rgpont.Add(new Pont(rgl[i], rgl[i + 1]));

                var color = new RGBA
                {
                    r = rgl[0],
                    g = rgl[1],
                    b = rgl[2],
                    a = rgl[3],
                };

                var pontFirst = rgpont.First();
                var polyFirst = mppolyByPont[pontFirst];
                var pontLast = rgpont.Last();
                var polyLast = mppolyByPont[pontLast];

                var link = new Link(pontFirst, polyFirst, pontLast, polyLast, color);

                polyFirst.rglink.Add(pontFirst.key(), link);
                polyLast.rglink.Add(pontLast.key(),link);
            }

            Console.WriteLine(rgpoly.Select(poly => poly.TSTOFull()).StJoin(Environment.NewLine));

            var polyStart = mppolyByPont.OrderBy(kvp => kvp.Key.key()).First().Value;

            var rginput = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(FpatIn), "serial.in.bin")).ToList();
            var vm = new Vpvm(polyStart, rginput, true);

            for(;!vm.FEnded();)
            {
                vm.Step();
            }

            var rgout = vm.Enoutput().ToArray();
            File.WriteAllBytes(FpatOut,rgout);
            var rgrefout = File.ReadAllBytes(FpatRefout);
            if(!rgout.SequenceEqual(rgrefout))
                log.Error("AJAJAJAJAJAJJ");
        }

    }
}
