using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1B.C
{
    class GarbledEmailSolver : GcjSolver
    {
        private class Nd
        {
            public string stPre;
            public char ch;
            public readonly Dictionary<char,Nd> mNdByCh=new Dictionary<char, Nd>();

            public override string ToString()
            {
                return stPre;
            }
        }

        private class State : IEquatable<State>, IAstarNd<int>
        {
            public string tsto;
            public int qqq;
            public int ich;
            public Nd nd;

            public int DistFromStart { get; set; }

            public override string ToString()
            {
                return string.Format("TSTO: {0}, Qqq: {1}, Ich: {2}", tsto, qqq, ich);
            }

            public bool Equals(State other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return qqq == other.qqq && ich == other.ich && nd.Equals(other.nd);
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != this.GetType())
                    return false;
                return Equals((State) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = qqq;
                    hashCode = (hashCode * 397) ^ ich;
                    hashCode = (hashCode * 397) ^ nd.GetHashCode();
                    return hashCode;
                }
            }
        }

        private static Nd ndX;

        protected override IEnumerable<object> EnobjSolveCase()
        {
            if(ndX == null)
            {
                ndX = new Nd {stPre = "#", ch = '#'};

                var rgword = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(FpatIn), "garbled_email_dictionary.txt"));
                foreach(var word in rgword)
                {
                    var nd = ndX;
                    foreach(var ch in word)
                    {
                        nd = nd.mNdByCh.EnsureGet(ch, () => new Nd {stPre = nd.stPre + ch, ch = ch});
                    }
                    Debug.Assert(!nd.mNdByCh.ContainsKey('#'));
                    nd.mNdByCh.EnsureGet('#', () => ndX);
                }
            }

            var stMsg = Fetch<string>();

            var cMinChange = Astar2.Find<State,int>(
                new[]
                {
                    new State {ich = -1, nd = ndX, qqq=0, tsto = "*", DistFromStart =  0}
                },
                state => state.ich == stMsg.Length,
                stt => EnNext(stt, stMsg, ndX)
                ).DistFromStart;

            yield return cMinChange;
        }

        private static IEnumerable<State> EnNext(State stt, string stMsg, Nd ndX)
        {
            var state = stt;
            Nd[] rg;
            if(state.ich + 1 == stMsg.Length)
                rg = state.nd.mNdByCh.Values.Where(nd => nd == ndX).ToArray();
            else
                rg = state.nd.mNdByCh.Values.SelectMany(nd => nd == ndX ? ndX.mNdByCh.Values.ToArray() : new[] {nd}).ToArray();

            return rg
                .Select(nd => new {nd, fOk = state.ich + 1 == stMsg.Length || stMsg[state.ich + 1] == nd.ch})
                .Where(x => x.fOk || state.qqq == 0)
                .Select(x => new State
                    {
                        ich = state.ich + 1,
                        nd = x.nd,
                        qqq = x.fOk ? Math.Max(0, state.qqq - 1) : 4,
                        DistFromStart = x.fOk ? stt.DistFromStart : stt.DistFromStart + 1
                        //tsto = state.tsto + " " + x.nd.ToString()
                    });

        }
    }
}
