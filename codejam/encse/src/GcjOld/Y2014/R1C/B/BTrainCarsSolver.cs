using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2014.R1C.B
{
    public class BTrainCarsSolver : IConcurrentSolver
    {
        private const long mod = 1000000007;

        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var ccar = pparser.Fetch<int>();
            var rgcar = pparser.StLineNext().Split(' ');

            return () => Solve(rgcar);
        }

        private IEnumerable<object> Solve(string[] rgst)
        {
            var rgcar = rgst.Select(st => new Car(st)).ToArray();
                 
            //var l = 0;
            //foreach (var perm in rgcar.EntPermute())
            //{
            //    var c = perm.Aggregate("", (x, y) => x + y);
            //    if (FPrecond(c))
            //        l++;

            //}
            //yield return l;

            if (!FPrecond(rgcar))
                yield return 0;
            else
            {
                Megrag(rgcar);
                yield return SolveI(null, rgcar, new HashSet<char>(), new Dictionary<string, long>());
            }
        }

        private void Megrag(Car[] rgcar)
        {
            for(int icar = 0;icar<rgcar.Length;icar++)
            {
                var car = rgcar[icar];
                var st = "";

                for (int i = 0; i < car.st.Length; i++)
                {
                    var ch = car.st[i];
                    if (i < car.st.Length - 1 && car.st[i + 1] == ch)
                        continue;
                    st += car.st[i];
                }
                car.st = st;
            }
        }

        class Car
        {
            public string st;

            public Car(string st)
            {
                this.st = st;
            }
        }

        private bool FPrecond(params Car[] rgcar)
        {
            foreach (var car in rgcar)
            {
                for (int i = 0; i < car.st.Length; i++)
                {
                    var ch = car.st[i];
                    if (i < car.st.Length - 1 && car.st[i + 1] == ch)
                        continue;


                    if (i + 2 < car.st.Length && car.st.IndexOf(ch, i + 2) != -1)
                        return false;
                }
            }
            return true;
        }


        private long SolveI(char? och, Car[] rgcar, HashSet<char> hlmchProcessed, Dictionary<string, long> cache)
        {
            var key = och != null && rgcar.Any(car => car.st.Contains(och.Value))  ? och.Value.ToString() : "<null>";
            key += ":";
            key += rgcar.OrderBy(c => c.st).StJoin(":", c=> c.st);
            key += ":";
            key += hlmchProcessed.Where(ch => rgcar.Any(car => car.st.Contains(ch))).OrderBy(c => c).StJoin(":", c=>c.ToString());

            if (cache.ContainsKey(key))
                return cache[key];
            else
                return cache[key] = SolveII(och, rgcar, hlmchProcessed, cache);
        }
        private long SolveII(char? och, Car[] rgcar, HashSet<char> hlmchProcessed, Dictionary<string, long> cache)
        {
            
            if (!rgcar.Any())
                return 1;

          
            if (och != null)
            {
                var ch = och.Value;

                var rgcarEgyszinuStartsWith = rgcar.Where(car => car.st[0] == ch && FEgyszinu(car)).ToArray();
                if (rgcarEgyszinuStartsWith.Any())
                {
                    var hlmcarNext = rgcar.Where(car => !rgcarEgyszinuStartsWith.Contains(car)).ToArray();
                    var l = ModFact(rgcarEgyszinuStartsWith.Count());
                    return ModMul(l, SolveI(och, hlmcarNext, hlmchProcessed, cache));
                }

                var rgcarNotEgyszinuStartsWith = rgcar.Where(car => car.st[0] == ch && !FEgyszinu(car)).ToArray();
                if (rgcarNotEgyszinuStartsWith.Any())
                {
                    if (rgcarNotEgyszinuStartsWith.Length > 1)
                        return 0;
                    var car = rgcarNotEgyszinuStartsWith.Single();

                    if (FCoki(car, hlmchProcessed))
                        return 0;

                    ProcessRgch(car, hlmchProcessed);

                    var hlmcarNext = rgcar.Where(carT => !rgcarNotEgyszinuStartsWith.Contains(carT)).ToArray();
                    return SolveI(car.st.Last(), hlmcarNext, hlmchProcessed, cache);
                }

                hlmchProcessed.Add(ch);


            }
         
            if (rgcar.Any(car => FCoki(car, hlmchProcessed)))
                return 0;
            
            {
                var s = 0L;
                foreach (var car in rgcar)
                {
                    var ch = car.st.Last();
                    var hlmchProcessedT = new HashSet<char>(hlmchProcessed);
                    ProcessRgch(car, hlmchProcessedT);

                    var hlmcarNext = rgcar.Where(carT => car != carT).ToArray();
                    s = ModPlus(s, SolveI(ch, hlmcarNext, hlmchProcessedT, cache));
                }

                return s;
            }
        }

        private void ProcessRgch(Car car, HashSet<char> hlmchProcessed)
        {
            foreach(var ch in car.st)
                if (ch != car.st.Last())
                    hlmchProcessed.Add(ch);
        }

        private bool FCoki(Car car, HashSet<char> hlmchProcessed)
        {
            return car.st.Any(hlmchProcessed.Contains);
        }

        private long ModMul(long a, long b)
        {
            return (a * b) % mod;
        }
        
        private long ModPlus(long a, long b)
        {
            return (a + b) % mod;
        }

        private static bool FEgyszinu(Car car)
        {
            return car.st.All(ch => ch == car.st[0]);
        }

        private long ModFact(long n)
        {
            long fact = 1;
            var c = 1;
            for (int i = 0; i < n; i++)
            {
                fact = (fact*c)%mod;
                c++;
            }
            return fact;
        }
    }
}
