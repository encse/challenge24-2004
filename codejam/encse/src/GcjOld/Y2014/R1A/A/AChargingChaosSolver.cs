using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.R1A.A
{
    public class AChargingChaosSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n;
            int l;
            pparser.Fetch(out n, out l);

            var rgin = pparser.Fetch<string[]>();
            var rgout = pparser.Fetch<string[]>();
            return () => Solve(rgin, rgout);
        }

        private IEnumerable<object> Solve(string[] rgin, string[] rgout)
        {

            var crow = rgout.Length;
            var ccol = rgout[0].Length;
            var lMin = SolveFast(rgin, rgout, crow, ccol);
            Console.Write(".");
            if (lMin == long.MaxValue)
                yield return "NOT POSSIBLE";
            else
                yield return lMin;

        }

        private long SolveFast(string[] confSrc, string[] confDst, int crow, int ccol)
        {
            confDst = confDst.OrderBy(x => x).ToArray();
            var rglnStart = confSrc.Select(st => new Ln(st)).ToArray();

            var lmin = long.MaxValue;
            foreach (var rgln in SolveFast(rglnStart, confDst, 0, crow, ccol))
            {
                if (FMatch(rgln, confDst))
                {
                    lmin = Math.Min(lmin, rgln.First().cflip);
                }
            }
            return lmin;
        }


        class Ln
        {
            public string stSuffix;
            public string stPrefix;
            public int cflip;

            public Ln(string stPrefix)
            {
                this.stPrefix = stPrefix;
            }

            private Ln()
            {
            }

            private char ch { get { return stPrefix.Last(); } }
            private char chOther { get { return ch == '0' ? '1':'0'; } }

            public Ln Dup()
            {
                return new Ln {stPrefix = this.stPrefix.Substring(0, this.stPrefix.Length - 1), stSuffix = ch + stSuffix, cflip = cflip};
            }

            public Ln DupOther()
            {
                return new Ln { stPrefix = this.stPrefix.Substring(0, this.stPrefix.Length - 1), stSuffix = chOther + stSuffix, cflip = cflip+1};
            }
        }

        private IEnumerable<Ln[]> SolveFast(Ln[] confSrc, string[] confDst, int icol, int crow, int ccol)
        {
            if (icol == ccol)
                yield return confSrc;

            if (icol < ccol)
            {
                confDst = confDst.OrderBy(st => st.Substring(icol)).ToArray();
                foreach (var rgln in SolveFast(confSrc, confDst, icol + 1, crow, ccol))
                {
                  //  Tsto(rgln, confDst);
                    

                    var rglnT = rgln.Select(ln => ln.Dup()).OrderBy(x=>x.stSuffix).ToArray();
                  //  Tsto(rglnT, confDst);
                    if (FMatch(rglnT, confDst))
                        yield return rglnT;
                    var rglnTT = rgln.Select(ln => ln.DupOther()).OrderBy(x => x.stSuffix).ToArray();
                 //   Tsto(rglnTT, confDst);
 
                    if (FMatch(rglnTT, confDst))
                        yield return rglnTT;
                }
            }
        }

        private void Tsto(Ln[] rglnT, string[] rgst)
        {
            for(int i=0;i<rglnT.Length;i++)
            {
                var ln = rglnT[i];
                Console.WriteLine(ln.stPrefix + "^" + ln.stSuffix+"  " + rgst[i]);
            }
        }

        private bool FMatch(Ln[] rgln, string[] confDst)
        {
            for (int i = 0; i < rgln.Length ;i++)
            {
                if (!confDst[i].EndsWith(rgln[i].stSuffix))
                    return false;
            }
            return true;
        }

        private long SolveBruteForce(string[] rgin, string[] rgout, int crow, int ccol)
        {
            var lMin = long.MaxValue;

            foreach (var conf in rgin.EntPermute())
            {
                var l = LCompute(Pack(conf), Pack(rgout), crow, ccol);
                if (l < lMin)
                    lMin = l;
            }
            return lMin;
        }

        private int[,] Pack(string[] conf)
        {
            var crow = conf.Length;
            var ccol = conf[0].Length;
            var r = new int[crow, ccol];
            for (int irow = 0; irow < crow; irow++)
            {
                for (int icol = 0; icol < ccol; icol++)
                {
                    r[irow, icol] = conf[irow][icol] == '0' ? 0 : 1;
                }
            }
                
            return r;
        }

        private long LCompute(int[,] conf, int[,] rgout, int crow, int ccol)
        {
            return LComputeRowFirst(conf, rgout, crow, ccol);
        }

        private long LComputeRowFirst(int[,] conf, int[,] rgout, int crow, int ccol)
        {
            int l = 0;

            for (int icol = 0; icol < ccol; icol++)
            {
                var cOk = 0;
                for (int irow = 0; irow < crow; irow++)
                {
                    if (conf[irow,icol] == rgout[irow,icol])
                        cOk++;
                }
                if (cOk == 0)
                    l++;
                else if (cOk != crow)
                    return long.MaxValue;
            }


            return l;
        }
    }
}
