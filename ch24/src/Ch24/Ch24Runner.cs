using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ch24.Contest;
using Cmn.Util;
using log4net;

namespace Ch24
{
    internal class Ch24Runner<TSolver> where TSolver:Solver, new()
    {
        private List<TSolver> rgsolver;

        public Ch24Runner(string fmtfilnIn, string fmtfilnOut, string ofmtfilnRefout)
        {
            rgsolver = CreateSolvers(fmtfilnIn, fmtfilnOut, ofmtfilnRefout);

            Console.Title = string.Format("Running {0}", typeof(TSolver).Name);
        }

        List<TSolver> CreateSolvers(string fmtfilnIn, string fmtfilnOut, string ofmtfilnRefout)
        {
            var nsParts = typeof(TSolver).Namespace.Split('.');
            var contestId = "20" + nsParts[1].Substring(nsParts[1].Length - 2);
            var problemName = nsParts[2];
            var dpat = "data/{0}/{1}".StFormat(contestId, problemName);

            var rgsolver = new List<TSolver>();
            foreach (var filn in Directory.EnumerateFiles(dpat, fmtfilnIn.Replace("{0}", "*")))
            {
                var z =
                    Regex.Match(new FileInfo(filn).Name, fmtfilnIn.Replace("{0}", "(.*)")).Groups[1].Captures[0].Value;
                int idProblem;
                if(!int.TryParse(z, out idProblem))
                    continue;

                rgsolver.Add(
                    new TSolver
                    {
                        IdProblem = idProblem,
                        FpatIn = Path.Combine(dpat, fmtfilnIn.StFormat(idProblem)),
                        FpatOut = Path.Combine(dpat, fmtfilnOut.StFormat(idProblem)),
                        FpatRefout = ofmtfilnRefout != null ? Path.Combine(dpat, ofmtfilnRefout.StFormat(idProblem)) : null
                    });
            }
            return rgsolver.OrderBy(solver => solver.IdProblem).ToList();
        }

        public Ch24Runner<TSolver> Run(bool fParallel) 
        {
            Console.Title = string.Format("Running {0}", typeof(TSolver).Name);

            var log = LogManager.GetLogger(typeof(Solver));

            var stFiltered = rgsolver.Select(solver => solver.IdProblem.ToString()).StJoin(",");
            log.Info("Solving [" + stFiltered + "]");
          
            var rgdgTask = new List<Action>();
            foreach (var solverT in rgsolver)
            {
                var solver = solverT;
                rgdgTask.Add(() =>
                {
                    if(!fParallel)
                        Console.Title = string.Format("Running {0}#{1}", solver.GetType().Name, solver.IdProblem);
                    log.InfoFormat("Current directory: {0}", Directory.GetCurrentDirectory());
                    solver.InitAndSolve();
                });
            }

            if (fParallel)
                Parallel.Invoke(rgdgTask.ToArray());
            else
            {
                new Action(() =>
                {
                    var cki = Console.ReadKey();
                    lock (rgdgTask)
                    {
                        if(rgdgTask.Any() && cki.Key == ConsoleKey.P && cki.Modifiers == ConsoleModifiers.Control)
                        {
                            Console.WriteLine("Change to parallel");
                            Parallel.Invoke(rgdgTask.ToArray());
                            rgdgTask.Clear();
                        }
                    }
                }).BeginInvoke(null, null);
                while(true)
                {
                    Action dgTask;
                    lock (rgdgTask)
                    {
                        if (!rgdgTask.Any())
                            break;
                        dgTask = rgdgTask.First();
                        rgdgTask.RemoveAt(0);
                    }
                    dgTask();
                }
            }

            log.Info("Finished");
            Console.Title = string.Format("Finished {0}", typeof(TSolver).Name);

            return this;
        }

        public Ch24Runner<TSolver> SelectProblems() 
        {
            var stAvailable = rgsolver.Select(solver => solver.IdProblem.ToString()).StJoin(",");
            Console.Write("Problems to solve [" + stAvailable + "] ");

            var stInput = Console.ReadLine();
            if (stInput == "")
                return this;

            var problems = IntRangeParser.Parse(stInput, rgsolver.Min(solver => solver.IdProblem), rgsolver.Max(solver => solver.IdProblem)).ToArray();
            if (problems.Length == 0)
                return this;

            rgsolver = rgsolver.Where(solver => problems.Contains(solver.IdProblem)).ToList();

            return this;
        }
    }
}