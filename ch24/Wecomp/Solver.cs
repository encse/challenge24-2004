using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wecomp.Util;
using log4net;

namespace Wecomp
{
    public abstract class Solver
    {
        protected ILog log;

        public int IdProblem;
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

        public Solwrt Solwrt;


        protected Solwrt Output
        {
            get
            {
                return Solwrt ?? (Solwrt = new Solwrt(FpatOut, FpatRefout));
            }
        }

        protected string DpatOut
        {
            get { return Path.GetDirectoryName(FpatOut); }
        }
        
        protected string DpatIn
        {
            get { return Path.GetDirectoryName(FpatIn); }
        }

        public void InitAndSolve()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FpatOut));
            log = LogManager.GetLogger(GetType());
            log.Info("Solving problem: {0}".StFormat(IdProblem));
            Solve();
            if(Score.HasValue)
            {
                var fpatScore = string.Format("{0}.score", FpatOut);
                using (var sw = new StreamWriter(fpatScore))
                {
                    sw.WriteLine(Score.Value.ToString(CultureInfo.InvariantCulture));
                }

                var fBest = true;
                var fpatBestScore = string.Format("{0}.best", fpatScore);
                long? scoreBest=null;
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
                    log.Info(string.Format("NEW BEST SCORE: {0}{1}", Score, (scoreBest.HasValue ? string.Format(" (was {0})", scoreBest.Value):"")));
                    var fpatBestOut = string.Format("{0}.best", FpatOut);
                    File.Copy(FpatOut,fpatBestOut,true);
                    File.Copy(fpatScore,fpatBestScore,true);
                }
                else
                {
                    log.Info(string.Format("Score: {0}{1}", Score, (scoreBest.HasValue ? string.Format(" Best: {0}", scoreBest.Value):"")));
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

        public bool FEof()
        {
            return Pparser.FEof();
        }

        public string NufDouble
        {
            get
            {
                return Solwrt.NufDouble;
            }
            set
            {
                Solwrt.NufDouble = value;
            }
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

        public void Info(object message)
        {
            log.Info(message);
        }

        public abstract void Solve();
        
      
    }
}