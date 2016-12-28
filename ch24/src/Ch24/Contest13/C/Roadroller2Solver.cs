using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Solver = Ch24.Contest.Solver;

namespace Ch24.Contest13.C
{
    public partial class Roadroller2Solver : Solver
    {
        private class Seg
        {
            public Pont PontFrom;
            public Pont PontTo;
        }

        private class Pont : IComparable<Pont>
        {
            public int x;
            public int y;
            public readonly SortedSet<Pont>[] mprgpontByIdsc=new SortedSet<Pont>[rgdiscGet.Length]; 

            public int CompareTo(Pont other)
            {
                int d;
                d = Math.Sign(x - other.x);
                if(d!=0)
                    return d;

                return Math.Sign(y - other.y);
            }

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}", x, y);
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
                    return (x.GetHashCode() * 397) ^ y.GetHashCode();
                }
            }
        }

        private static Func<int, int, int>[] rgdiscGet = new Func<int, int, int>[]
        {
            (x, y) => x,
            (x, y) => x+y,
            (x, y) => y,
            (x, y) => x-y,
        };

        public override void Solve()
        {
            var cpont = Fetch<int>();

            var rgpontAll = new SortedSet<Pont>();
            for(int ipont=0;ipont<cpont;ipont++)
            {
                var rgk = Fetch<int[]>();
                rgpontAll.Add(new Pont{x=rgk[0],y=rgk[1]});
            }

            var rgmprgpontByDisc = rgdiscGet.Select(_ => new SortedDictionary<int, SortedSet<Pont>>()).ToArray();

            foreach(var pont in rgpontAll)
            {
                for(int idsc = 0; idsc < rgmprgpontByDisc.Length; idsc++)
                {
                    var rgpont = rgmprgpontByDisc[idsc].EnsureGet(rgdiscGet[idsc](pont.x, pont.y));
                    rgpont.Add(pont);
                    pont.mprgpontByIdsc[idsc] = rgpont;
                }
            }
            
            var rgpontSol = new List<Pont>();
            Seg segLast = null;

            for(var idscLast = -1;;)
            {
                var t = rgmprgpontByDisc.Select((mprgpontByDisc, idsc) => new{mprgpontByDisc,idsc} ).OrderBy(t2 => t2.mprgpontByDisc.Count).First();

                if(!t.mprgpontByDisc.Any())
                    break;

                var kvrgpontByDiscNext = t.mprgpontByDisc.OrderByDescending(kvrgpontByDisc => kvrgpontByDisc.Value.Count).FirstOrDefault();

                var rgpontNext = kvrgpontByDiscNext.Value;

                t.mprgpontByDisc.Remove(kvrgpontByDiscNext.Key);

                foreach(var pont in rgpontNext)
                {
                    for(var idsc=0;idsc<pont.mprgpontByIdsc.Length;idsc++)
                    {
                        if(idsc==t.idsc)
                            continue;
                        var rgpont = pont.mprgpontByIdsc[idsc];
                        rgpont.Remove(pont);
                        if(rgpont.Any())
                            continue;
                        rgmprgpontByDisc[idsc].Remove(rgdiscGet[idsc](pont.x, pont.y));
                    }
                }

                if(idscLast!=t.idsc)
                {
                    idscLast = t.idsc;
                    segLast = null;
                }

                if(segLast==null)
                {
                    segLast = new Seg {PontFrom = rgpontNext.Min, PontTo = rgpontNext.Max};
                    if(rgpontSol.Any())
                    {
                        rgpontSol.AddRange(rgpontGetWay(rgpontSol.Last(), segLast.PontFrom));
                    }
                    rgpontSol.Add(segLast.PontFrom);
                    rgpontSol.Add(segLast.PontTo);
                    
                }
                else
                {
                    var pontTo = rgpontNext.Max;
                    var pontFrom = rgpontNext.Min;

                    if(pontFrom.Equals(pontTo))
                    {
                        rgpontSol.AddRange(rgpontGetWay(segLast.PontTo, pontFrom));
                    }
                    else if(fChecked(pontFrom.x - segLast.PontTo.x, pontFrom.y - segLast.PontTo.y))
                    {
                        
                    }
                    else if(fChecked(pontTo.x - segLast.PontTo.x, pontTo.y - segLast.PontTo.y))
                    {
                        swap(ref pontFrom,ref pontTo);
                    }
                    else
                    {
                        int dxLast = -Math.Sign(segLast.PontTo.x - segLast.PontFrom.x);
                        int dyLast = Math.Sign(segLast.PontTo.y - segLast.PontFrom.y);

                        int dx = Math.Sign(pontTo.x - pontFrom.x);
                        int dy = Math.Sign(pontTo.y - pontFrom.y);

                        if(dxLast == dx && dyLast == dy)
                        {
                            swap(ref pontFrom, ref pontTo);
                            dx = -dx;
                            dy = -dy;
                        }

                        Debug.Assert(dxLast == -dx && dyLast == -dy);

                        double[,] A;
                        A = new double[,]
                        {
                            {dxLast, -dy},
                            {dyLast, dx}
                        };

                        double[] b;
                        b = new double[]
                        {
                            pontFrom.x - segLast.PontTo.x,
                            pontFrom.y - segLast.PontTo.y,
                        };
                        int info;
                        alglib.densesolverreport rep;
                        double[] x;
                        alglib.rmatrixsolve(A, 2, b, out info, out rep, out x);
                        Debug.Assert(info == 1);

                        Pont pontNew;
                        if(x[0] > 0)
                        {
                            pontNew = new Pont
                            {
                                x = (int) Math.Round(segLast.PontTo.x + dxLast * x[0]),
                                y = (int) Math.Round(segLast.PontTo.y + dyLast * x[0])
                            };
                        }
                        else
                        {
                            pontNew = new Pont
                            {
                                x = (int) Math.Round(segLast.PontTo.x - dy * x[1]),
                                y = (int) Math.Round(segLast.PontTo.y + dx * x[1])
                            };
                        }

                        Checkd(pontNew.x - segLast.PontTo.x, pontNew.y - segLast.PontTo.y);
                        Checkd(pontFrom.x - pontNew.x, pontFrom.y - pontNew.y);
                        if(x[0] > 0)
                        {
                            Debug.Assert(Math.Sign(pontNew.x - segLast.PontTo.x) == dxLast);
                            Debug.Assert(Math.Sign(pontNew.y - segLast.PontTo.y) == dyLast);
                        }
                        else
                        {
                            Debug.Assert(Math.Sign(pontFrom.x - pontNew.x) == dx);
                            Debug.Assert(Math.Sign(pontFrom.y - pontNew.y) == dy);
                        }
                        rgpontSol.Add(pontNew);
                    }
                    segLast = new Seg {PontFrom = pontFrom, PontTo = pontTo};
                    rgpontSol.Add(segLast.PontFrom);
                    rgpontSol.Add(segLast.PontTo);

                }

            }

            rgpontSol = optimize(rgpontSol);

            Check(rgpontAll, rgpontSol);

            Score = rgpontSol.Count - 1;
            using(Output)
            {
                foreach(var pont in rgpontSol)
                {
                    WriteLine(new []{pont.x,pont.y});
                }
            }

        }

        private List<Pont> optimize(List<Pont> rgpontSol)
        {
            var rgpont = new List<Pont>();
            rgpont.Add(rgpontSol.First());
            for(var i=1;i<rgpontSol.Count-1;i++)
            {
                var pontPrev = rgpont.Last();
                var pont = rgpontSol[i];
                var pontNext = rgpontSol[i + 1];

                if(pont.Equals(pontPrev))
                    continue;

                if(Math.Sign(pont.x-pontPrev.x)==Math.Sign(pontNext.x-pont.x)
                    &&Math.Sign(pont.y-pontPrev.y)==Math.Sign(pontNext.y-pont.y))
                    continue;
                rgpont.Add(pont);
            }
            if(!rgpont.Last().Equals(rgpontSol.Last()))
                rgpont.Add(rgpontSol.Last());
            return rgpont;
        }

        private static void swap(ref Pont pontFrom, ref Pont pontTo)
        {
            var pontT = pontFrom;
            pontFrom = pontTo;
            pontTo = pontT;
        }

        private void Check(IEnumerable<Pont> enpont, List<Pont> rgpontSol)
        {
            Info("checking solution");
            var hlmPont = new SortedSet<Pont>(enpont);

            var pontFrom = rgpontSol.First();
            int ipont = 0;
            foreach(var pontTo in rgpontSol.Skip(1))
            {
                var dx = pontTo.x - pontFrom.x;
                var dy = pontTo.y - pontFrom.y;

                Checkd(dx, dy);

                dx = Math.Sign(dx);
                dy = Math.Sign(dy);

                for(var pont = new Pont{x=pontFrom.x, y=pontFrom.y};;pont.x+=dx,pont.y+=dy)
                {
                    hlmPont.Remove(pont);
                    if(pont.Equals(pontTo))
                        break;
                }

                if(++ipont%100==0)
                    log.Info(string.Format("Checked {0}/{1}", ipont, rgpontSol.Count));

                pontFrom = pontTo;
            }

            if(hlmPont.Any())
                throw new Exception();
        }

        private static bool fChecked(int dx, int dy)
        {
            if(dx == 0 && dy == 0)
                return false;

            if(dx != 0 && dy != 0 && Math.Abs(dx) != Math.Abs(dy))
                return false;
            return true;
        }

        private static void Checkd(int dx, int dy)
        {
            if(!fChecked(dx,dy))
                throw new Exception();
        }

        private SortedSet<Pont> RgpontMaxGet(Dictionary<int, SortedSet<Pont>>[] rgmprgpontByDisc)
        {
            SortedSet<Pont> rgpontMax = null;
            foreach(var mprgpontByDisc in rgmprgpontByDisc)
            {
                foreach(var rgpont in mprgpontByDisc.Values)
                {
                    if(rgpontMax == null || rgpont.Count > rgpontMax.Count)
                        rgpontMax = rgpont;
                }
            }
            rgpontMax = new SortedSet<Pont>(rgpontMax);
            foreach(var mprgpontByDisc in rgmprgpontByDisc)
            {
                foreach(var kvrgpontByDisc in mprgpontByDisc.ToArray())
                {
                    kvrgpontByDisc.Value.ExceptWith(rgpontMax);
                    if(!kvrgpontByDisc.Value.Any())
                        mprgpontByDisc.Remove(kvrgpontByDisc.Key);
                }
            }
            return rgpontMax;
        }

        private List<Pont> rgpontGetWay(Pont pontFrom, Pont pontTo)
        {
            var rgpont = new List<Pont>();
            if(pontFrom != null && rgdiscGet.All( discGet => discGet(pontFrom.x,pontFrom.y) != discGet(pontTo.x,pontTo.y)))
                rgpont.Add(new Pont{x=pontTo.x,y=pontFrom.y});
            return rgpont;
        }

    }
}
