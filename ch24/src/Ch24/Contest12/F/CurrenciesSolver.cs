using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Google.OrTools.LinearSolver;

namespace Ch24.Contest12.F
{
    public static class CurrenciesU
    {
        public static LinearExpr Sum(this IEnumerable<LinearExpr> rgexpr)
        {
            return new SumArray(rgexpr.ToArray());
        }

        public static LinearExpr Sum(this IEnumerable<Variable> rgvar)
        {
            return new SumVarArray(rgvar.ToArray());
        }   
    }
     

    public class CurrenciesSolver : Contest.Solver
    {
       

        public override void Solve()
        {
            var fpatIn = FpatIn;
           // var fpatIn = "data/2012/F/sample.in";
            var pp = new Pparser(fpatIn);

            int ccurrency, coffer, money;
            pp.Fetch(out ccurrency, out coffer, out money);
            var rgoffer = pp.FetchN<Offer>(coffer).ToArray();

        
            Console.WriteLine(Optimize(50, ccurrency, money, rgoffer)- money);
        }


        private double Optimize(int cround, int ccurrency, int money, Offer[] rgoffer)
        {
            var solver = Solver.CreateSolver("IntegerProgramming", "CLP_LINEAR_PROGRAMMING");

            var coffer = rgoffer.Length;

            var rgvarMoney = new Variable[cround, ccurrency]; //az i-edik k�rben a j valut�b�l ennyi p�nz�nk van
            var rgvarExc = new Variable[cround, coffer];  //az i-edik k�rben a j offer felhaszn�l�s�val �tv�ltott p�nz
            var rgvarNExc = new Variable[cround, ccurrency];  //az i-edik k�rben nem �tv�ltott p�nz
                
            for (int iround = 0; iround < cround; iround++)
            {
                for (int icurrency = 0; icurrency < ccurrency; icurrency++)
                {
                    var varP = solver.MakeNumVar(0, double.PositiveInfinity, "_{0}_money_{1}".StFormat(iround, icurrency));
                    rgvarMoney[iround, icurrency] = varP;

                    var varNExc = solver.MakeNumVar(0, double.PositiveInfinity, "_{0}_kept_{1}".StFormat(iround, icurrency)); 
                    rgvarNExc[iround, icurrency] = varNExc;
                }

                for (int ioffer = 0; ioffer < coffer; ioffer++)
                {
                    var varOffer = solver.MakeNumVar(0, double.PositiveInfinity,
                                                     "_{0}_offer_{1}_{2}_{3}".StFormat(iround, ioffer,
                                                                                        rgoffer[ioffer].icurrencyFrom,
                                                                                        rgoffer[ioffer].icurrencyTo));
                    
                    rgvarExc[iround, ioffer] = varOffer;
                }
            }


            //az elej�n a 0 valut�b�l  indulunk
            solver.Add(rgvarMoney[0, 0] == money);
            for (int icurrency = 1; icurrency < ccurrency; icurrency++)
                solver.Add(rgvarMoney[0, icurrency] == 0);

            // a v�g�n a 0-ba �rkez�nk
            for (int icurrency = 1; icurrency < ccurrency; icurrency++)
                solver.Add(rgvarMoney[cround - 1, icurrency] == 0);

            //betartjuk a maxExchange korl�tot
            for (int ioffer = 0; ioffer < coffer; ioffer++)
            {
                var rgvar = new List<Variable>();
                for (int iround = 0; iround < cround; iround++)
                    rgvar.Add(rgvarExc[iround, ioffer]);
                
                solver.Add(rgvar.Sum() >= 0);
                solver.Add(rgvar.Sum() <= rgoffer[ioffer].moneyMax);
            }


            //a p�nz�nk egy r�sz�t meg�rizhetj�k, a t�bbit �t kell v�ltanunk
            for (int iround = 0; iround < cround - 1; iround++)
            {
                for (int icurrencyFrom = 0; icurrencyFrom < ccurrency; icurrencyFrom++)
                {
                    var rgvar = new List<Variable>();
                    for (int ioffer = 0; ioffer < coffer; ioffer++)
                    {
                        if (rgoffer[ioffer].icurrencyFrom == icurrencyFrom)
                            rgvar.Add(rgvarExc[iround, ioffer]);
                    }
                    rgvar.Add(rgvarNExc[iround, icurrencyFrom]);

                    solver.Add(rgvar.Sum() == rgvarMoney[iround, icurrencyFrom]);
                }
            }

            //a k�v l�p�sbe �tv�lt�ssal jutunk, illetve meg�rizhet�nk vmennyi p�nzt
            for (int iround = 1; iround < cround; iround++)
            {
                for (int icurrencyTo = 0; icurrencyTo < ccurrency; icurrencyTo++)
                {
                    var rgexpr = new List<LinearExpr>();
                    for (int ioffer = 0; ioffer < coffer; ioffer++)
                    {
                        if (rgoffer[ioffer].icurrencyTo == icurrencyTo)
                            rgexpr.Add(rgvarExc[iround - 1, ioffer]*rgoffer[ioffer].exchangeRate);
                    }
                    rgexpr.Add(rgvarNExc[iround - 1, icurrencyTo]);

                    solver.Add(rgexpr.Sum() == rgvarMoney[iround, icurrencyTo]);
                }
            }


            solver.Maximize(rgvarMoney[cround - 1, 0]);

            solver.SetTimeLimit(60 * 1000);

            var resultStatus = solver.Solve();
            if (resultStatus == Solver.OPTIMAL)
                return solver.Objective().Value();
            return -1;
        }

        private class Offer
        {
            public int icurrencyFrom;
            public int icurrencyTo;
            public double exchangeRate;
            public int moneyMax;

            public Offer(int icurrencyFrom, int icurrencyTo, double exchangeRate, int moneyMax)
            {
                this.icurrencyFrom = icurrencyFrom;
                this.icurrencyTo = icurrencyTo;
                this.exchangeRate = exchangeRate;
                this.moneyMax = moneyMax;
            }
        }


    }

}
