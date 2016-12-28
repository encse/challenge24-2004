using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R2.D
{
    internal class DescendingInTheDarkNotSolver : GcjSolver
    {
        private class P : IEquatable<P>
        {
            public int x;
            public int y;

            public bool Equals(P other)
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
                if(obj.GetType() != typeof(P))
                    return false;
                return Equals((P) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (x * 397) ^ y;
                }
            }

            public static bool operator ==(P left, P right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(P left, P right)
            {
                return !Equals(left, right);
            }
        }

        private class Ma
        {
            public P p;
            public HashSet<Kdir> hlmkdir=new HashSet<Kdir>();
            public Dictionary<Kdir,Ma> mpmaByKdir = new Dictionary<Kdir, Ma>();
        }

        private enum Kdir {Down,Left,Right}

        private enum Km {Bad, Ok}

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int w;
            int h;
            Fetch(out h, out w);
            var rgpCave = new P[10].ToList();
            var mMaze = new Km[w,h];
            for(var y=0;y<h;y++)
            {
                var st = Fetch<string>();
                for(var x=0;x<w;x++)
                {
                    var ch = st[x];
                    mMaze[x, y] = ch == '#' ? Km.Bad : Km.Ok;
                    switch(ch)
                    {
                        case '#':
                        case '.':
                            break;
                        default:
                            var iCave = int.Parse(ch.ToString());
                            Debug.Assert(rgpCave[iCave]==null);
                            rgpCave[iCave] = new P {x = x, y = y};
                            break;
                    }
                }
            }

            foreach(var vipCave in rgpCave.Select((v,i)=>new{v,i}).Where(vi => vi.v != null))
            {
                var mpmaByP = new Dictionary<P, Ma>();
                var qupOpen=new Queue<P>();
                var addOpen = new Action<P>(p =>
                {
                    if(mpmaByP.ContainsKey(p))
                        return;

                    mpmaByP[p] = new Ma {p = p};

                    qupOpen.Enqueue(p);
                });

                addOpen(vipCave.v);

                for(;qupOpen.Count > 0;)
                {
                    var p = qupOpen.Dequeue();

                    foreach(var dd in new[]
                    {
                        new {x=0,y=-1},
                        new {x=1,y=0},
                        new {x=-1,y=0},
                    })
                    {
                        var pT = new P {x = p.x + dd.x, y = p.y + dd.y};

                        var km = mMaze[pT.x, pT.y];

                        if(km==Km.Bad)
                            continue;

                        if(mpmaByP.ContainsKey(pT))
                            continue;

                        addOpen(pT);
                    }
                }

                foreach(var ma in mpmaByP.Values)
                {
                    foreach(var dd in new[]
                    {
                        new {x=0,y=1, kdir = Kdir.Down},
                        new {x=1,y=0, kdir = Kdir.Right},
                        new {x=-1,y=0, kdir = Kdir.Left},
                    })
                    {
                        var pT = new P {x = ma.p.x + dd.x, y = ma.p.y + dd.y};
                        
                        if(mMaze[pT.x,pT.y] == Km.Bad)
                        {
                            ma.hlmkdir.Add(dd.kdir);
                        }
                        else
                        {
                            var maT = mpmaByP.GetOrDefault(pT, null);

                            if(maT!=null)
                            {
                                ma.hlmkdir.Add(dd.kdir);
                                ma.mpmaByKdir[dd.kdir] = maT;
                            }
                        }
                    }
                }

                var cReach = mpmaByP.Count;

                for(;;)
                {
                    if(mpmaByP.Count == 1)
                        break;

                    var fFoundKdir = false;
                    foreach(var kdir in new[] {Kdir.Down, Kdir.Left, Kdir.Right})
                    {
                        if(!mpmaByP.Values.All(ma => ma.hlmkdir.Contains(kdir)))
                            continue;

                        var mpmaByPT = mpmaByP
                            .Values
                            .Select(ma => ma.mpmaByKdir.GetOrDefault(kdir, ma))
                            .Distinct()
                            .ToDictionary(ma => ma.p);

                        if(mpmaByPT.Count >= mpmaByP.Count)
                            continue;

                        //if(!mpmaByPT.ContainsKey(vipCave.v))
                        //    continue;

                        fFoundKdir = true;
                        mpmaByP = mpmaByPT;
                        break;
                    }

                    //Debug.Assert(mpmaByP.ContainsKey(vipCave.v));

                    if(!fFoundKdir)
                        break;
                }

                var fLucky = mpmaByP.Count == 1;

                yield return Solwrt.NewLine;
                yield return vipCave.i + ":";
                yield return cReach;
                yield return fLucky ? "Lucky" : "Unlucky";
            }
        }
    }
}
