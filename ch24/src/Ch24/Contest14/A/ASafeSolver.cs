using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cmn.Util;

namespace Ch24.Contest14.A
{
    public class ASafeSolver : Ch24.Contest.Solver
    {
        class State 
        {
            public BigInteger pack;
            public int ich;
            public int d;
            public ulong flagsRotated;

            public State(BigInteger pack, ulong flagsRotated, int ich, int d)
            {
                this.pack = pack;
                this.ich = ich;
                this.d = d;
                this.flagsRotated = flagsRotated;
            }

            public State Dup()
            {
                return new State(pack, flagsRotated, ich, d);
            }

            public bool Equals(State other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.pack.Equals(pack) && other.ich == ich && other.d == d && other.flagsRotated == flagsRotated;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (State)) return false;
                return Equals((State) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = pack.GetHashCode();
                    result = (result*397) ^ ich;
                    result = (result*397) ^ d;
                    result = (result*397) ^ flagsRotated.GetHashCode();
                    return result;
                }
            }
        }

        BigInteger[] Deg(string[] rgrot)
        {
            var rgdeg = new BigInteger[rgrot.Length];
            BigInteger deg = 1L;
            for(int i=0;i<rgrot.Length;i++)
            {
                rgdeg[i] = deg;
                deg *= rgrot[i].Length;
            }
            return rgdeg;
        }

        int Rotpos(BigInteger pack, int irot, BigInteger[] rgdeg, string[] rgrot)
        {
            return (int)((pack/rgdeg[irot])% rgrot[irot].Length);
        }

        BigInteger RotposInc(BigInteger pack, int irot, BigInteger[] rgdeg, string[] rgrot, int inc)
        {
            var rotpos = Rotpos(pack, irot, rgdeg, rgrot);

            pack -= rotpos*rgdeg[irot];

            rotpos = (rotpos + inc)%rgrot[irot].Length;

            pack += rotpos * rgdeg[irot];

            return pack;
        }
      
        public override void Solve()
        {
            var pp = new Pparser(FpatIn);

            var crot = pp.Fetch<int>();
            var rgrot = pp.FetchN<string>(crot).ToArray();
           
            var ccode = pp.Fetch<int>();
            using (Output)
            {
                for (var i = 0; i < ccode; i++)
                {
                    var stCode = pp.Fetch<string>();
                    Console.Write("(" + stCode.Length + ") ");
                    var dmin = Solve(rgrot, stCode);
                    Solwrt.WriteLine(dmin);
                    Console.WriteLine(dmin);

                }
            }
        }
   
        private int Solve(string[] rgrot, string stCode)
        {
            if (rgrot.Length > 60)
                throw new Exception();
            
            int dPrev = 0;
            var sd = new SortedDictionary<int, HashSet<State>>();
            sd.Add(0, new HashSet<State>{new State(0, 0, 0, 0)});
            
            var deg = Deg(rgrot);

            while (sd.Any())
            {
                var kvpCur = sd.First();
                sd.Remove(kvpCur.Key);
                var hlm = kvpCur.Value;

                if (dPrev != kvpCur.Key)
                    Console.Write(kvpCur.Key + "? ");

                dPrev = kvpCur.Key;

                foreach (var state in hlm)
                {
                    if (state.ich == stCode.Length)
                        return state.d + stCode.Length;

                    for (var irot = 0; irot < rgrot.Length; irot++)
                    {
                        var rot = rgrot[irot];
                        var rotPos = Rotpos(state.pack, irot, deg, rgrot);
                        var rotMask = (ulong) (1L << irot);
                        var fRotated = (state.flagsRotated & rotMask) != 0;

                        if (!fRotated)
                        {
                            for (var i = 0; i < rot.Length; i++)
                            {
                                if (rot[i] == stCode[state.ich])
                                {
                                    var stateNew = state.Dup();
                                    stateNew.d += i;
                                    stateNew.flagsRotated |= rotMask;
                                    stateNew.pack = RotposInc(stateNew.pack, irot, deg, rgrot, i + 1);
                                    stateNew.ich++;
                                    AddState(sd, stateNew);
                                }
                            }
                        }
                        else if (rot[rotPos] == stCode[state.ich])
                        {
                            var stateNew = state.Dup();
                            stateNew.d += 0;
                            stateNew.pack = RotposInc(stateNew.pack, irot, deg, rgrot, 1);
                            stateNew.ich++;
                            AddState(sd, stateNew);
                        }
                    }
                }
            }
            return -1;
        }

        private void AddState(SortedDictionary<int, HashSet<State>> sd, State stateNew)
        {
            if(!sd.ContainsKey(stateNew.d))
                sd.Add(stateNew.d, new HashSet<State>());

            sd[stateNew.d].Add(stateNew);
        }
    }
}
