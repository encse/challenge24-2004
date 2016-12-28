using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1C.C
{
    internal class BoxFactorySolver : GcjSolver
    {
        private class Unit
        {
            public decimal c;
            public int type;

            public Unit(decimal c, int type)
            {
                this.type = type;
                this.c = c;
            }

            public override string ToString()
            {
                return string.Format("C: {0}, Type: {1}", c, type);
            }

            public bool Equals(Unit other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return other.c == c && other.type == type;
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != typeof(Unit))
                    return false;
                return Equals((Unit) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (c.GetHashCode() * 397) ^ type;
                }
            }
        }

        private class State
        {
            public int i1;
            public int i2;
            public Unit unit1;
            public Unit unit2;

            public static readonly State End = new State();

            public bool Equals(State other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return other.i1 == i1 && other.i2 == i2 && Equals(other.unit1, unit1) && Equals(other.unit2, unit2);
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != typeof(State))
                    return false;
                return Equals((State) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = i1;
                    result = (result * 397) ^ i2;
                    result = (result * 397) ^ (unit1 != null ? unit1.GetHashCode() : 0);
                    result = (result * 397) ^ (unit2 != null ? unit2.GetHashCode() : 0);
                    return result;
                }
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            Fetch<int[]>();
            var rgunit1 = RgunitFetch();
            var rgunit2 = RgunitFetch();

            var rgsumAfter1 = rgunit1.Select((_, i) => rgunit1.Skip(i + 1).Sum(unit => unit.c)).ToArray();
            var rgsumAfter2 = rgunit2.Select((_, i) => rgunit2.Skip(i + 1).Sum(unit => unit.c)).ToArray();

            var rgsumByTypeAfter1 = rgunit1.Select((_, i) => rgunit1.Skip(i + 1).GroupBy(unit => unit.type).ToDictionary(grp => grp.Key, grp => grp.Sum(unit => unit.c))).ToArray();
            var rgsumByTypeAfter2 = rgunit2.Select((_, i) => rgunit2.Skip(i + 1).GroupBy(unit => unit.type).ToDictionary(grp => grp.Key, grp => grp.Sum(unit => unit.c))).ToArray();

            var stateNextGet = new Func<State, int, int, State>((state, di1, di2) => state.i1 + di1 < rgunit1.Count && state.i2 + di2 < rgunit2.Count ? new State
            {
                i1 = state.i1 + di1,
                unit1 = di1 == 0 ? state.unit1 : rgunit1[state.i1 + di1],
                i2 = state.i2 + di2,
                unit2 = di2 == 0 ? state.unit2 : rgunit2[state.i2 + di2]
            } : State.End);

            yield return (rgunit1.Concat(rgunit2).Sum(unit => unit.c) -new Astar<State, decimal>(
                new[]
                {
                    new Tuple<State, decimal>(
                        new State{i1 = 0, unit1 = rgunit1.First(), i2 = 0, unit2 = rgunit2.First()}
                        , 0
                        )
                },
                state => state == State.End,
                (stateFrom, _) =>
                {
                    var unit1 = stateFrom.unit1;
                    var unit2 = stateFrom.unit2;

                    if(unit1.type != unit2.type)
                        return new[] {stateNextGet(stateFrom, 1, 0), stateNextGet(stateFrom,0,1)};

                    if(unit1.c == unit2.c)
                        return new[] {stateNextGet(stateFrom, 1, 1)};

                    State stateNext;
                    if(unit1.c < unit2.c)
                    {
                        stateNext = stateNextGet(stateFrom,1,0);
                        if(stateNext != State.End)
                            stateNext.unit2 = new Unit(unit2.c - unit1.c, unit2.type);
                        return new[] {stateNext};
                    }
                    stateNext = stateNextGet(stateFrom, 0, 1);
                    if(stateNext != State.End)
                        stateNext.unit1 = new Unit(unit1.c - unit2.c, unit1.type);
                    return new[] {stateNext};
                },
                (stateFrom, waste, stateTo) =>
                {
                    if(stateTo!=State.End)
                    {
                        if(stateFrom.unit1.type==stateFrom.unit2.type)
                            return waste;
                        if(stateFrom.unit1!=stateTo.unit1)
                            return waste + stateFrom.unit1.c;
                        Debug.Assert(stateFrom.unit1!=stateTo.unit2);
                        return waste + stateFrom.unit2.c;
                    }

                    waste += stateFrom.unit1.c;
                    waste += stateFrom.unit2.c;
                    
                    waste += rgsumAfter1[stateFrom.i1];
                    waste += rgsumAfter2[stateFrom.i2];
                    
                    if(stateFrom.unit1.type == stateFrom.unit2.type)
                        waste -= Math.Min(stateFrom.unit1.c, stateFrom.unit2.c)*2;

                    return waste;
                },
                (state, waste) =>
                {
                    if(state==State.End)
                        return waste;

                    var mpsumByType1 = rgsumByTypeAfter1[state.i1];
                    var mpsumByType2 = rgsumByTypeAfter2[state.i2];
                    foreach(var type in mpsumByType1.Keys.Union(mpsumByType2.Keys).Union(new[]{state.unit1.type,state.unit2.type}))
                    {
                        decimal w = 0;

                        if(type==state.unit1.type)
                            w += state.unit1.c;
                        
                        w += mpsumByType1.GetOrDefault(type, 0);

                        if(type==state.unit2.type)
                            w -= state.unit2.c;
                        
                        w -= mpsumByType2.GetOrDefault(type, 0);

                        waste += Math.Abs(w);
                    }
                    return waste;
                }
                ).Find().Item2)/2;
        }

        private List<Unit> RgunitFetch()
        {
            var rgunit = new List<Unit>();
            var rgnum = Fetch<decimal[]>();
            for(var i = 0; i < rgnum.Length; i += 2)
            {
                rgunit.Add(new Unit(rgnum[i], (int)rgnum[i+1]));
            }
            return rgunit;
        }
    }
}
