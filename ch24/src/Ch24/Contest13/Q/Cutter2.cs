using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest13.Q
{
    public class Cutter2
    {
        private readonly Func<int, int, bool> FGetAt;

        public struct Sq : IComparable<Sq>
        {
            public int X;
            public int Y;
            public int W;

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}, W: {2}", X, Y, W);
            }

            public bool Equals(Sq other)
            {
                //if(ReferenceEquals(null, other))
                //    return false;
                //if(ReferenceEquals(this, other))
                //    return true;
                return other.X == X && other.Y == Y && other.W == W;
            }

            public override bool Equals(object obj)
            {
                //if(ReferenceEquals(null, obj))
                //    return false;
                //if(ReferenceEquals(this, obj))
                //    return true;
                //if(obj.GetType() != typeof(Sq))
                //    return false;
                return Equals((Sq) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var result = X;
                    result = (result * 397) ^ Y;
                    result = (result * 397) ^ W;
                    return result;
                }
            }

            public int CompareTo(Sq sq)
            {
                var d = sq.W - W;
                if(d!=0)
                    return d;

                d = Y - sq.Y;
                if(d!=0)
                    return d;

                d = X - sq.X;
                return d;
            }
        }

        public Cutter2(Func<int,int,bool> fGetAt)
        {
            FGetAt = fGetAt;
        }

        public IEnumerable<Sq> Doit()
        {

            var rgsqProcess=new LinkedList<Sq>();

            rgsqProcess.AddLast(new Sq {X = 0, Y = 0, W = 16384});

            var mpcBySq = new Dictionary<Sq, long>();

            Func<Sq, long> cGetFromSq=null;
            cGetFromSq = sq =>
            {
                if(sq.X>=16000||sq.Y>=16000)
                    return 0;

                if(sq.W <= 32)
                    return CGet(sq);

                return mpcBySq.EnsureGet(sq,()=> EnsqGet(sq).Sum(sqT => cGetFromSq(sqT)));
            };

            for(;rgsqProcess.Count!=0;)
            {
                var sq = rgsqProcess.First.Value;
                rgsqProcess.RemoveFirst();

                if(sq.X >=16000 || sq.Y>=16000)
                    continue;
                
                var ccol = cGetFromSq(sq);

                long cpx = (Math.Min(16000, sq.X+sq.W)-sq.X)*(Math.Min(16000, sq.Y+sq.W)-sq.Y);

                var rx = cpx*10/100;
                rx = 0;

                if(cpx <= ccol+rx)
                    yield return new Sq {X = sq.X, Y = sq.Y, W = sq.W};
                else if(ccol > rx)
                    foreach(var sqT in EnsqGet(sq))
                        rgsqProcess.AddLast(sqT);
            }
        }

        private IEnumerable<Sq> EnsqGet(Sq sq)
        {
            Debug.Assert(sq.W % 2 == 0);
            var nw = sq.W / 2;
            for(var ix = 0; ix < 2; ix++)
                for(var iy = 0; iy < 2; iy++)
                    yield return new Sq {X = sq.X + ix * nw, Y = sq.Y + iy * nw, W = nw};
        }

        private long CGet(Sq sq)
        {
            long c = 0;

            var cx = Math.Min(sq.X + sq.W, 16000);
            var cy = Math.Min(sq.Y + sq.W, 16000);

            for(var x = sq.X; x < cx; x++)
                for(var y = sq.Y; y < cy; y++)
                    if(FGetAt(x, y))
                        c++;
            return c;
        }
    }



}
