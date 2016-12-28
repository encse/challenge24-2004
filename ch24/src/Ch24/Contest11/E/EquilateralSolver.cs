using System;
using System.Linq;
using Cmn.Util;
using Google.OrTools.LinearSolver;

namespace Ch24.Contest11.E
{
   


    public class EquilateralSolver : Ch24.Contest.Solver
    {
       

        public override void Solve()
        {
            var fpatIn = FpatIn;
            // var fpatIn = "data/2012/F/sample.in";
            var pp = new Pparser(fpatIn);

            var crod = pp.Fetch<int>();
            using (var solwrt = new Solwrt(FpatOut))
            {
                while (crod > 0)
                {
                    var rgrod = pp.Fetch<int[]>();
                    crod = pp.Fetch<int>();
                    double vMin, vMax;
                    Optimize(rgrod.Reverse().ToArray(), out vMin, out vMax);
                    if (Math.Abs(vMin - vMax) > 0)
                    {
                        log.InfoFormat("solution: {0}-{1}", vMin, vMax);
                        solwrt.WriteLine(vMin+" " + vMax);
                    }
                    else
                    {
                        log.InfoFormat("solution: {0}", vMin);
                        solwrt.WriteLine(vMax);
                    }
                    
                    
                }
            }

        }

        private void Optimize(int[] rgrod, out double min, out double max)
        {
            //  tsto(model);
            var solver = Google.OrTools.LinearSolver.Solver.CreateSolver("IntegerProgramming",
                                                                         "CBC_MIXED_INTEGER_PROGRAMMING");

            int crod = rgrod.Length;
           
            var rgrodX = new Variable[crod]; //az első oldalhoz felhasznált rudak
            var rgrodY = new Variable[crod]; //a második oldalhoz felhasznált rudak
            var rgrodZ = new Variable[crod]; //a harmadik oldalhoz felhasznált rudak

            for (int irod = 0; irod < crod; irod++)
            {
                rgrodX[irod] = solver.MakeIntVar(0, 1, "x{0}".StFormat(irod));
                rgrodY[irod] = solver.MakeIntVar(0, 1, "y{0}".StFormat(irod));
                rgrodZ[irod] = solver.MakeIntVar(0, 1, "z{0}".StFormat(irod));
            }
      
            //egy rudat legfeljebb egyszer használunk fel:
            for (int irod = 0; irod < crod; irod++)
                solver.Add(rgrodX[irod] + rgrodY[irod] + rgrodZ[irod] <= 1);

            //egyenlő hosszúak lesznek
            solver.Add(new SumArray(rgrodX.Select((rod, irod) => rod*rgrod[irod]).ToArray()) ==
                       new SumArray(rgrodY.Select((rod, irod) => rod*rgrod[irod]).ToArray()));

            solver.Add(new SumArray(rgrodX.Select((rod, irod) => rod * rgrod[irod]).ToArray()) ==
                       new SumArray(rgrodZ.Select((rod, irod) => rod * rgrod[irod]).ToArray()));


            solver.Maximize(new SumArray(rgrodX.Select((rodX,irod) => rodX * rgrod[irod]).ToArray()));

            solver.SetTimeLimit(60*1000);
            
            var resultStatus = solver.Solve();

            if (resultStatus == Google.OrTools.LinearSolver.Solver.OPTIMAL)
                min = max = solver.Objective().Value();
            else
            {
                min = solver.Objective().Value();
                max = solver.Objective().BestBound();
            }

        }

    }
}
