using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cmn.Util;
using System.Linq;

namespace Gcj.Util
{
    public interface IConcurrentSolver
    {
        int CCaseGet(Pparser pparser);
        ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser);
    }
    
    public sealed class ConcurrentGcjSolver
    {
        public delegate IEnumerable<object> DgSolveCase();
        
        public static void Solve<T>(bool fParallel, bool fStats = true) where T : IConcurrentSolver, new()
        {
            Lg.dgIlgFromTy = tyT => new LgConsole(tyT);

            var ty = typeof(T);

            Console.Title = string.Format("Running {0}",ty.Name);

            Console.WriteLine("Running: {0}", ty.Name);
            foreach (var fpat in Directory.EnumerateFiles(ty.Namespace.Substring(ty.Namespace.Split('.')[0].Length+2).Replace('.', '/'), "*.in").Reverse())
            {
                var solver = new ConcurrentGcjSolver();
                var fmtfpat = fpat.Substring(0, fpat.Length - 3);
                solver.FpatIn = string.Format("{0}.in", fmtfpat);
                solver.FpatOut = string.Format("{0}.out", fmtfpat);
                solver.FpatRefout = string.Format("{0}.refout", fmtfpat);

                solver.InitAndSolve(new T(), fParallel, fStats);

            }
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        public string FpatIn;
        public string FpatOut;
        public string FpatRefout;

        public void InitAndSolve(IConcurrentSolver solver, bool fParallel, bool fStats)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FpatOut));
            var pparser = new Pparser(FpatIn);
            var cCase = solver.CCaseGet(pparser);

            var rgresult = new object[cCase][];

            var dt = DateTime.Now;
            var i = 0;
            var rgdgSolve = new Action[cCase];

            var lck = new object();
            for (var iCase = 0; iCase < cCase; iCase++)
            {
                var iCaseT = iCase;
                var dgSolveCase = solver.DgSolveCase(pparser);
                rgdgSolve[iCaseT] = () =>
                {
                    rgresult[iCaseT] = dgSolveCase().ToArray();

                    if (fStats)
                    {
                        lock (lck)
                        {
                            i++;
                            Console.Write("\rSolved: {0}/{1}; time: {2}", i, cCase, (DateTime.Now - dt));
                        }
                    }
                };
            }

            if(fParallel)
                Parallel.Invoke(rgdgSolve);
            else
                foreach (var dgSolve in rgdgSolve)
                    dgSolve();

            Console.WriteLine();
            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                for (var iCase = 0; iCase < cCase; iCase++)
                     solwrt.WriteLine(new object[] { string.Format("Case #{0}:", (iCase+1)) }.Concat(rgresult[iCase]));
            }
        }
    }
}