using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.R1C.C
{
    public class GreatWallSolver : IConcurrentSolver
    {

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }


        public class Tribe
        {
            private int di; //the day of the tribe's first attack (where 1st January, 250BC, is considered day 0)
            private int ni; //the number of attacks from this tribe
            private int wi; //the westmost and eastmost points respectively of the Wall attacked on the first attack
            private int ei; 
            private int si; //the strength of the first attack
            private int delta_di; // the number of days between subsequent attacks by this tribe
            private int delta_pi; //the distance this tribe travels to the east between subsequent attacks (if this is negative, the tribe travels to the west)
            private int delta_si; //the change in strength between subsequent attacks

            public int D { get; private set;}
            public int W { get; private set;}
            public int E { get; private set;}
            public int S { get; private set; }

            private int iAttack;
            
            public bool Next()
            {
                iAttack++;
                if (iAttack >= ni)
                    return false;

                D += delta_di;
                E += delta_pi;
                W += delta_pi;
                S += delta_si;
                return true;
            }

            public Tribe(int di, int ni, int wi, int ei, int si, int deltaDi, int deltaPi, int deltaSi)
            {
                this.di = di;
                this.ni = ni;
                this.wi = wi;
                this.ei = ei;
                this.si = si;
                delta_di = deltaDi;
                delta_pi = deltaPi;
                delta_si = deltaSi;

                iAttack = 0;
                D = di;
                E = ei;
                W = wi;
                S = si;
            }

            
        }

        public class Szksz
        {
            public long iStart;
            public long iEnd;
            public int height;
            
            public Szksz(long iStart, long iEnd, int height)
            {
                this.iStart = iStart;
                this.iEnd = iEnd;
                this.height = height;
            }
        }
        public class Wall
        {
            private readonly List<Szksz> rgszksz = new List<Szksz>();
            public int Cszksz { get { return rgszksz.Count; } }

            public Wall()
            {
                rgszksz.Add(new Szksz(long.MinValue, long.MaxValue, 0));
            }

            public int Attack(Szksz[] rgszkszAttack)
            {
                int cSucc = rgszkszAttack.Count(FSuccess);
                foreach (var szkszAttack in rgszkszAttack)
                    Raise(szkszAttack);
                return cSucc;
            }

            private string Tsto()
            {
                var sb = new StringBuilder();
                foreach (var szksz in rgszksz)
                {

                    sb.AppendLine(string.Format("{0}-{1}:\t{2}",
                                                szksz.iStart == long.MinValue ? "--" : szksz.iStart.ToString(),
                                                szksz.iEnd == long.MaxValue ? "++" : szksz.iEnd.ToString(),
                                                szksz.height));
                }
                return sb.ToString();
            }

            private void Raise(Szksz szkszAttack)
            {
                
                var iStart = szkszAttack.iStart;
                var iEnd = szkszAttack.iEnd;
                var height = szkszAttack.height;

                var iszkszStart = IszkszLeft(iStart);
                var iszkszEnd = IszkszRight(iEnd);

                while (iStart < iEnd)
                {
                    var iEndT = iszkszEnd == iszkszStart ? iEnd : rgszksz[iszkszStart].iEnd;
                    bool fInsertBefore, fInsertAfter;
                    Raise(iszkszStart, iStart, iEndT, height, out fInsertBefore, out fInsertAfter);

                    iszkszStart++;
                    if(fInsertAfter)
                    {
                        iszkszEnd++;
                    }
                    if(fInsertBefore)
                    {
                        iszkszEnd++;
                        iszkszStart++;
                    }
                    iStart = iEndT;
                }

            }

            private void Raise(int iszksz, long iStart, long iEnd, int height, out bool fInsertBefore, out bool fInsertAfter)
            {
                fInsertAfter = fInsertBefore = false;

                var szksz = rgszksz[iszksz];
                if (iStart < szksz.iStart || iEnd > szksz.iEnd)
                    throw new Exception("wtf");
                if(szksz.height>=height)
                    return;

                if (szksz.iStart == iStart && szksz.iEnd == iEnd)
                {    
                    szksz.height = height;
                }
                else if(szksz.iStart == iStart)
                {
                    var szkszNext = new Szksz(iEnd, szksz.iEnd, szksz.height);
                    if (iszksz < rgszksz.Count - 1 && rgszksz[iszksz + 1].height == szksz.height)
                        rgszksz[iszksz + 1].iStart = szkszNext.iStart;
                    else
                    {
                        rgszksz.Insert(iszksz + 1, szkszNext);
                        fInsertAfter = true;

                    }

                    szksz.height = height;
                    szksz.iEnd = iEnd;
                }
                else if (szksz.iEnd == iEnd)
                {
                    var szkszPrev = new Szksz(szksz.iStart, iStart, szksz.height);
                    if (iszksz > 0 && rgszksz[iszksz - 1].height == szkszPrev.height)
                        rgszksz[iszksz - 1].iEnd = szkszPrev.iEnd;
                    else
                    {
                        rgszksz.Insert(iszksz, szkszPrev);
                        fInsertBefore = true;
                    }

                    szksz.height = height;
                    szksz.iStart = iStart;
                }
                else
                {
                    var szkszNext = new Szksz(iEnd, szksz.iEnd, szksz.height);
                    if (iszksz < rgszksz.Count - 1 && rgszksz[iszksz + 1].height == szksz.height)
                        rgszksz[iszksz + 1].iStart = szkszNext.iStart;
                    else
                    {
                        rgszksz.Insert(iszksz + 1, szkszNext);
                        fInsertAfter = true;

                    }
                    
                    var szkszPrev = new Szksz(szksz.iStart, iStart, szksz.height);
                    if (iszksz > 0 && rgszksz[iszksz - 1].height == szkszPrev.height)
                        rgszksz[iszksz - 1].iEnd = szkszPrev.iEnd;
                    else
                    {
                        rgszksz.Insert(iszksz, szkszPrev);
                        fInsertBefore = true;
                    }

                    szksz.height = height;
                    szksz.iEnd = iEnd;
                    szksz.iStart = iStart;
                }
            }

            private bool FSuccess(Szksz szkszAttack)
            {
                var iszkszEnd = IszkszRight(szkszAttack.iEnd);
                for(int iszksz = IszkszLeft(szkszAttack.iStart); iszksz <= iszkszEnd; iszksz++)
                {
                    if (rgszksz[iszksz].height < szkszAttack.height)
                        return true;
                }
                return false;
            }

            private int IszkszRight(long i)
            {

                var iszkszL = 0;
                var iszkszH = rgszksz.Count - 1;

                while (iszkszH - 1 > iszkszL)
                {
                    var iszkszM = (iszkszL + iszkszH) / 2;
                    var szksz = rgszksz[iszkszM];
                    if (szksz.iStart < i && i <= szksz.iEnd)
                        return iszkszM;

                    if (i > szksz.iEnd)
                        iszkszL = iszkszM;
                    else if (szksz.iStart >= i)
                        iszkszH = iszkszM;
                    else
                        throw new Exception("wtf");
                }

                if (rgszksz[iszkszH].iStart < i && i <= rgszksz[iszkszH].iEnd)
                    return iszkszH;
                else if (rgszksz[iszkszL].iStart < i && i <= rgszksz[iszkszL].iEnd)
                    return iszkszL;
                else
                    throw new Exception("wtf");
            }

            private int IszkszLeft(long i)
            {
                var iszkszL = 0;
                var iszkszH = rgszksz.Count - 1;

                while (iszkszH-1 > iszkszL)
                {
                    var iszkszM = (iszkszL + iszkszH)/2;
                    var szksz = rgszksz[iszkszM];
                    if (szksz.iStart <= i && i < szksz.iEnd)
                        return iszkszM;

                    if (i >= szksz.iEnd)
                        iszkszL = iszkszM;
                    else if (szksz.iStart > i)
                        iszkszH = iszkszM;
                    else
                        throw new Exception("wtf");
                }

                if (rgszksz[iszkszH].iStart <= i && i < rgszksz[iszkszH].iEnd)
                    return iszkszH;
                else if (rgszksz[iszkszL].iStart <= i && i < rgszksz[iszkszL].iEnd)
                    return iszkszL;
                else
                    throw new Exception("wtf");
            }
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             The first line of the input gives the number of test cases, T.
             * T test cases follow. 
             * Each test case begins with a line containing a single integer N: the number of the tribes attacking the Wall.
             * N lines follow, each describing one tribe. 
             * The ith line contains eight integers di, ni, wi, ei, si, delta_di, delta_pi and delta_si separated by spaces, describing a single nomadic tribe:
                  di – the day of the tribe's first attack (where 1st January, 250BC, is considered day 0)
                  ni – the number of attacks from this tribe
                  wi, ei – the westmost and eastmost points respectively of the Wall attacked on the first attack
                  si – the strength of the first attack
                  delta_di – the number of days between subsequent attacks by this tribe
                  delta_pi – the distance this tribe travels to the east between subsequent attacks (if this is negative, the tribe travels to the west)
                  delta_si – the change in strength between subsequent attacks
             */
            var ctribe = pparser.Fetch<int>();
            var rgtribe = pparser.FetchN<Tribe>(ctribe);

            return () => Solve(new Hlm_Chewbacca<Tribe>(rgtribe), new Wall());
        }


        
        void Add(SortedDictionary<int, List<Tribe>> mprgtribeByd, int d, Tribe tribe)
        {
            if (!mprgtribeByd.ContainsKey(d))
                mprgtribeByd.Add(d, new List<Tribe> {tribe});
            else
                mprgtribeByd[d].Add(tribe);

        }

        private IEnumerable<object> Solve(Hlm_Chewbacca<Tribe> rgtribe, Wall wall)
        {

            var mprgtribeByd = new SortedDictionary<int, List<Tribe>>();
            foreach (var tribe in rgtribe)
                Add(mprgtribeByd, tribe.D, tribe);


            long c = 0;
            int i = 0;
            int sum = 0;

            while (mprgtribeByd.Any())
            {
                i++;
                if (i%1000 == 0)
                {
                    Console.WriteLine("{0}, {1}".StFormat(sum/i, wall.Cszksz));
                    sum = 0;
                    i = 0;
                }
            
                var kvp = mprgtribeByd.First();
                mprgtribeByd.Remove(kvp.Key);
                var rgtribeAttack = kvp.Value;
                
                c+=wall.Attack(rgtribeAttack.Select(tribe => new Szksz(tribe.W, tribe.E, tribe.S)).ToArray());

                sum += rgtribeAttack.Count;
                foreach (var tribe in rgtribeAttack)
                {
                    if(tribe.Next())
                        Add(mprgtribeByd, tribe.D, tribe);
                }
            }
            Console.Write(".");
            Console.WriteLine(wall.Cszksz);
            
            yield return c;
        }
    }

}
