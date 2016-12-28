using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2012.R1C.B
{
    public class OutOfGasSolver : IConcurrentSolver
    {
        private static decimal Infinity = new decimal(ulong.MaxValue);
        private static decimal Sqrt(decimal d)
        {
            return (decimal) Math.Sqrt((double) d);
        }
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            double dist;
            int ccp;
            int cacc;

            pparser.Fetch(out dist, out ccp, out cacc);
            var rgcp = pparser.FetchN<Cp>(ccp);
            var rgacc = pparser.Fetch<double[]>().Select(x => (decimal)x).ToArray();
            for (int i = 0; i < rgcp.Count - 1;i++ )
            {
                rgcp[i] = new Cp(rgcp[i].T0, rgcp[i].S0, (rgcp[i + 1].S0 - rgcp[i].S0)/(rgcp[i + 1].T0 - rgcp[i].T0), 0);
            }
            return () => EnobjSolveCase((decimal)dist, rgcp, rgacc);
        }

        protected class Cp
        {
            public readonly decimal T0;
            public readonly decimal S0;
            public readonly decimal V0;
            public readonly decimal A;

            public Cp(double t0, double s0)
            {
                T0 = (decimal)t0;
                S0 = (decimal)s0;
            }

            internal Cp(decimal t0, decimal s0, decimal v0, decimal a)
            {
                T0 = t0;
                S0 = s0;
                V0 = v0;
                A = a;
            }

            public decimal S(decimal t)
            {
                if (t < T0 && T0 - t > (decimal)0.00001)
                    throw new Exception();
                var dt = t - T0;
                return A / 2 * dt * dt + V0 * dt + S0;
            }

            public decimal V(decimal t)
            {
                if (t < T0 && T0 - t > (decimal)0.00001)
                    throw new Exception();
                var dt = t - T0;
                return A * dt + V0;
            }

            public Cp CpAt(decimal t)
            {
                return new Cp(t, S(t), V(t), A);
            }

            public static decimal TUtkozes(Cp cpA, Cp cpB)
            {
                //közös epochra hozzuk őket
                if (cpA.T0 < cpB.T0)
                {
                    var tUtoler = cpA.TGet(cpB.S0);
                    if (tUtoler < cpB.T0)
                        return tUtoler;
                    //if (tUtoler == cpB.T0 )
                    //{
                    //    if (cpA.V(tUtoler) >= cpB.V(tUtoler))
                    //        return tUtoler;
                    //    return Infinity;
                    //}

                    cpA = cpA.CpAt(cpB.T0);
                }
                else
                    cpB = cpB.CpAt(cpA.T0);

                if (cpB.S0 < cpA.S0)
                    throw new Exception("már meg is előzte???");
                
                var a = (cpA.A - cpB.A)/2;
                var b = cpA.V0 - cpB.V0;
                var c = cpA.S0 - cpB.S0;

                if (c == 0)
                    return cpA.T0;

                if (a == 0)
                {
                    var tUtkozes = -c/b + cpA.T0;
                    if (tUtkozes >= cpA.T0 && cpA.V(tUtkozes) >= cpB.V(tUtkozes))
                        return tUtkozes;
                    else
                        return Infinity;
                }

                var discr = b*b - 4*a*c;
                if (discr < 0)
                    return Infinity;


                foreach (var tUtkozes in new[] { cpA.T0 + (-b - Sqrt(discr)) / (2 * a), cpA.T0+(-b + Sqrt(discr)) / (2 * a) })
                {
                    if (tUtkozes >= cpA.T0 && cpA.V(tUtkozes) >= cpB.V(tUtkozes))
                        return tUtkozes;
                }

                return Infinity;
            }

            public decimal TGet(decimal s)
            {
                var a = this.A/2;
                var b = this.V0;
                var c = this.S0 - s;

                if (c == 0)
                    return T0;

                if (a == 0)
                {
                    var tUtkozes = -c / b + T0;
                    if (tUtkozes >= T0)
                        return tUtkozes;
                    else
                        return Infinity;
                }

                var discr = b * b - 4 * a * c;
                if (discr < 0)
                    return Infinity;

                foreach (var tUtkozes in new[] { T0 + (-b - Sqrt(discr)) / (2 * a), T0 + (-b + Sqrt(discr)) / (2 * a) })
                {
                    if (tUtkozes > T0)
                        return tUtkozes;
                }
                return Infinity;
            }

        }
        protected IEnumerable<object> EnobjSolveCase(decimal distance, List<Cp> rgcpB, decimal[]rgacc)
        {
            foreach (var acc in rgacc)
            {
                yield return Solwrt.NewLine;
                decimal tToYield = 0;
              //  try
                {
                    foreach (var cpA in RgcpA(rgcpB, acc))
                    {
                        if (cpA.S0 > distance)
                            break;
                        tToYield = cpA.TGet(distance);

                    }
                 
                }
                //catch(Exception er)
                //{
                //    Console.WriteLine(er);
                //}
                yield return ( (double)tToYield).ToString("0.#######", CultureInfo.InvariantCulture);
            }
        }


        private IEnumerable<Cp> RgcpA(List<Cp> rgcpB, decimal a)
        {
            var cpA = new Cp(0, 0, 0, a);
            yield return cpA;
            for (int icpB = 0; icpB < rgcpB.Count;icpB++)
            {
                var cpB = rgcpB[icpB];
                var t = Cp.TUtkozes(cpA, cpB);

                //ütköznének, de ez a cpB hamarabb véget ér
                if (t != Infinity && icpB < rgcpB.Count - 1 && t >= rgcpB[icpB + 1].T0)
                    t = Infinity;

                if (t == Infinity)
                {
                    cpA = cpA.CpAt(cpB.T0); //előremegyünk T0-ba
                    cpA = new Cp(cpA.T0, cpA.S0, cpA.V0, a); //elkezd gyorsulni
                }
                else
                {
                    cpA = cpB.CpAt(t); //utoléri B-t, és felveszi a sebességét
                }
                    
                    
                yield return cpA;
            }
        }
    }
}
