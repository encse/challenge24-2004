using System.Collections.Generic;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest11.E
{
    public class Equilateral2Solver : Contest.Solver
    {

        public override void Solve()
        {
            using(Output)
            {
                for(var iq=1;;iq++)
                {
                    var n = Fetch<int>();
                    if(n == 0)
                        break;
                    var rgrod = Fetch<List<int>>();

                    rgrod.Sort();

                    var max = (rgrod.Sum() + 2) / 3;

                    var mplenByLength = new SortedDictionary<int, Len>();

                    Info("Building combo list");
                    for(var irod = 0; irod < rgrod.Count; irod++)
                    {
                        var rod = rgrod[irod];

                        foreach(var kvpcomboByLength in mplenByLength.ToArray())
                        {
                            var lenPrev = kvpcomboByLength.Value;
                            var lengthPrev = kvpcomboByLength.Key;

                            var length = lengthPrev + rod;

                            if(length > max)
                                break;

                            mplenByLength[length] = new Len
                            {
                                Rgcombo = new List<Combo>(mplenByLength.GetOrDefault(length, new Len {Rgcombo = new List<Combo>()}).Rgcombo) {new Combo {LenPrev = lenPrev, Irod = irod}}
                            };

                        }
                        mplenByLength[rod] = new Len
                        {
                            Rgcombo = new List<Combo>(mplenByLength.GetOrDefault(rod, new Len {Rgcombo = new List<Combo>()}).Rgcombo) {new Combo {LenPrev = null, Irod = irod}}
                        };
                    }

                    Info("Check disjunct paths");
                    var maxLength = 0;
                    foreach(var kvplenByLength in mplenByLength.Reverse())
                    {
                        var len = kvplenByLength.Value;
                        var length = kvplenByLength.Key;

                        Info(string.Format("Length: {0}", length));

                        var rgcombo = len.Rgcombo;

                        if(rgcombo.Count < 3)
                            continue;

                        for(var icombo1 = 0; icombo1 < rgcombo.Count - 2; icombo1++)
                        {
                            var combo1 = rgcombo[icombo1];
                            for(var icombo2 = icombo1 + 1; icombo2 < rgcombo.Count - 1; icombo2++)
                            {
                                var combo2 = rgcombo[icombo2];
                                
                                for(var icombo3 = icombo2 + 1; icombo3 < rgcombo.Count; icombo3++)
                                {
                                    var combo3 = rgcombo[icombo3];

                                    if(check(new[]{combo1,combo2,combo3}))
                                        maxLength = length;
                                }
                                
                                if(maxLength != 0)
                                    break;
                            }
                            if(maxLength != 0)
                                break;
                        }
                        if(maxLength != 0)
                            break;
                    }
                    Info(maxLength);

                    WriteLine(maxLength);
                }
            }

        }

        private bool check(Combo[] rgcombo)
        {
            var rgstate = new List<State>
            {
                new State
                {
                    hlmirod = new SortedSet<int>(rgcombo.Select(combo => combo.Irod)),
                    rgcombo = new List<Combo>(rgcombo)
                }
            };

            var ic = 0;
            var icc = -1;
            for(;;)
            {
                var state = rgstate.Last();
                var ostateNext = ostateGetNext(ic, icc, state);
                if(ostateNext != null)
                {
                    rgstate.Add(ostateNext);

                    var fbreadth = false;

                    if(fbreadth)
                    {
                        ic++;
                        if(ic == ostateNext.rgcombo.Count)
                        {
                            if(ostateNext.rgcombo.All(combo => combo == null || combo.LenPrev == null))
                            {
                                return true;
                            }
                            ic = 0;
                        }
                        icc = -1;
                    }
                    else
                    {
                        if(ostateNext.rgcombo[ic]==null)
                        {
                            ic++;
                            if(ic==ostateNext.rgcombo.Count)
                                return true;
                        }
                        icc = -1;
                        
                    }
                }
                else
                {
                    if(rgstate.Count == 1)
                        break;

                    rgstate.RemoveAt(rgstate.Count - 1);
                    ic = state.icombo;
                    icc = state.icomboChild;
                }
            }
            return false;
        }

        private List<Combo> rgcomboCreateSet(List<Combo> rgcomboPrev, int i, Combo combo)
        {
            var rgcomboT = new List<Combo>(rgcomboPrev);
            rgcomboT[i] = combo;
            return rgcomboT;
        }

        private State ostateGetNext(int icombo, int icomboChild, State statePrev)
        {
            var comboPrev = statePrev.rgcombo[icombo];

            if(comboPrev == null || comboPrev.LenPrev == null)
            {
                if(icomboChild == -1)
                    return new State
                    {
                        icombo = icombo,
                        icomboChild = 0,
                        hlmirod = statePrev.hlmirod,
                        rgcombo = rgcomboCreateSet(statePrev.rgcombo, icombo, null)
                    };
                return null;
            }

            var rgcomboNext = comboPrev.LenPrev.Rgcombo;
            for(var i = icomboChild + 1; i < rgcomboNext.Count; i++)
            {
                var comboNext = rgcomboNext[i];

                if(statePrev.hlmirod.Contains(comboNext.Irod))
                    continue;

                return new State
                {
                    icombo = icombo,
                    icomboChild = i,

                    hlmirod = new SortedSet<int>(statePrev.hlmirod) {comboNext.Irod},
                    rgcombo = rgcomboCreateSet(statePrev.rgcombo, icombo, comboNext)
                };
            }
            return null;
        }

        private class Len
        {
            public List<Combo> Rgcombo;
        }

        private class Combo
        {
            public Len LenPrev;
            public int Irod;
        }

        private class State
        {
            public int icombo;
            public int icomboChild;

            public SortedSet<int> hlmirod;
            public List<Combo> rgcombo;
        }
    }


}
