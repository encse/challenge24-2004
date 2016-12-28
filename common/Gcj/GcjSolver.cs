using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cmn.Util;
using System.Linq;

namespace Gcj.Util
{
    public abstract class GcjSolver
    {
        public static void Solve<T>() where T : GcjSolver, new()
        {
            Lg.dgIlgFromTy = tyT => new LgConsole(tyT);

            Type ty = typeof(T);

            Console.Title = string.Format("Running {0}",ty.Name);

            Console.WriteLine("Running: {0}", ty.Name);
            foreach(var fpat in Directory.EnumerateFiles(ty.Namespace.Substring(5).Replace('.','/'), "*.in").Reverse())
            {
                var solver = new T();

                var fmtfpat = fpat.Substring(0, fpat.Length - 3);
                solver.FpatIn = string.Format("{0}.in", fmtfpat);
                solver.FpatOut = string.Format("{0}.out", fmtfpat);
                solver.FpatRefout = string.Format("{0}.refout", fmtfpat);
                
                solver.InitAndSolve();
            }
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        protected ILg log;

        public string FpatIn;
        public string FpatOut;
        public string FpatRefout;

        public long? Score;

        private Pparser _Pparser;

        protected Pparser Pparser
        {
            get
            {
                return _Pparser ?? (_Pparser = new Pparser(FpatIn));
            }
        }

        private Solwrt Solwrt;

        protected Solwrt Output
        {
            get
            {
                return Solwrt ?? (Solwrt = new Solwrt(FpatOut, FpatRefout));
            }
        }

        protected string DpatOut
        {
            get
            {
                return Path.GetDirectoryName(FpatOut);
            }
        }

        protected string DpatIn
        {
            get
            {
                return Path.GetDirectoryName(FpatIn);
            }
        }

        public void InitAndSolve()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FpatOut));
            log = Lg.GetLogger(GetType());
            Solve();
            if(Score.HasValue)
            {
                var fpatScore = string.Format("{0}.score", FpatOut);
                using(var sw = new StreamWriter(fpatScore))
                {
                    sw.WriteLine(Score.Value.ToString(CultureInfo.InvariantCulture));
                }

                var fBest = true;
                var fpatBestScore = string.Format("{0}.best", fpatScore);
                long? scoreBest = null;
                if(File.Exists(fpatBestScore))
                {
                    using(var sr = new StreamReader(fpatBestScore))
                    {
                        scoreBest = long.Parse(sr.ReadLine(), CultureInfo.InvariantCulture);
                        fBest = scoreBest > Score.Value;
                    }
                }

                if(fBest)
                {
                    log.Info(string.Format("NEW BEST SCORE: {0}{1}", Score, (scoreBest.HasValue ? string.Format(" (was {0})", scoreBest.Value) : "")));
                    var fpatBestOut = string.Format("{0}.best", FpatOut);
                    File.Copy(FpatOut, fpatBestOut, true);
                    File.Copy(fpatScore, fpatBestScore, true);
                }
                else
                {
                    log.Info(string.Format("Score: {0}{1}", Score, string.Format(" Best: {0}", scoreBest.Value)));
                }
            }
        }

        public T Fetch<T>()
        {
            return Pparser.Fetch<T>();
        }

        public List<T> FetchN<T>(int cnode)
        {
            return Pparser.FetchN<T>(cnode);
        }

        public void Fetch<T1, T2>(out T1 t1, out T2 t2)
        {
            Pparser.Fetch(out t1, out t2);
        }

        public void Fetch<T1, T2, T3, T4>(out T1 t1, out T2 t2, out T3 t3, out T4 t4)
        {
            Pparser.Fetch(out t1, out t2, out t3, out t4);
        }

        public void Fetch<T1, T2, T3, T4, T5>(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5)
        {
            Pparser.Fetch(out t1, out t2, out t3, out t4, out t5);
        }

        public void Fetch<T1, T2, T3>(out T1 t1, out T2 t2, out T3 t3)
        {
            Pparser.Fetch(out t1, out t2, out t3);
        }

        public bool FEof()
        {
            return Pparser.FEof();
        }

        public void WriteLine(object obj)
        {
            Solwrt.WriteLine(obj);
        }

        public void Write(object obj)
        {
            Solwrt.Write(obj);
        }

        public void Write(string stFormat, params object[] rgobj)
        {
            Solwrt.Write(stFormat, rgobj);
        }

        public void WriteLine(string stFormat, params object[] rgobj)
        {
            Solwrt.WriteLine(stFormat, rgobj);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        protected virtual void Solve()
        {
            var cCase = CCaseGet();
            using(Output)
            {
                for(var iCase = 1; iCase <= cCase; iCase++)
                    WriteLine(new object[] {string.Format("Case #{0}:", iCase)}.Concat(EnobjSolveCase()));
            }
        }

        protected virtual int CCaseGet()
        {
            return Fetch<int>();
        }

        protected abstract IEnumerable<object> EnobjSolveCase();
    }
}