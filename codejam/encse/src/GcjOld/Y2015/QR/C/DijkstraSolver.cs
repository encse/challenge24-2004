using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.QR.C
{
    public class DijkstraSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int l, x;
            pparser.Fetch(out l, out x);
            var st = pparser.Fetch<string>();

            var stA = "";
            for (var i = 0; i < x; i++)
                stA += st;

            return () => Solve(stA);
        }

        private IEnumerable<object> Solve(string stA)
        {
            var p = new Q[stA.Length];

            var s = Q.E;
            for (var i = 0; i < stA.Length; i++)
            {
                s = Mul(s, FromChar(stA[i]));

                p[i] = s;
            }
            Console.Write(".");
            for (var i = 0; i < stA.Length; i++)
            {
                if (p[i] != Q.I)
                    continue;

                for (var j = i + 1; j < stA.Length; j++)
                {
                    if (Mul(p, i + 1, j) == Q.J && Mul(p, j + 1, stA.Length - 1) == Q.K)
                    {
                        yield return "YES";
                        yield break;
                    }
                }
            }
            yield return "NO";
        }

        private Q Mul(Q[] p, int iFirst, int iLast)
        {
            if (iFirst < 0 || iFirst > p.Length - 1 || iLast < 0 || iLast > p.Length - 1 || iLast < iFirst)
                return Q.E;

            var a = iFirst > 0 ? p[iFirst - 1] : Q.E;
            var b = p[iLast];

            return Div(a, b);
        }

        private Q Div(Q qA, Q qB)
        {
            foreach(var qX in new[]{Q.E, Q.I,Q.J,Q.K,Q.Ne,Q.Ni,Q.Nj,Q.Nk})
                if(Mul(qA, qX) == qB)
                    return qX;
            
            throw new Exception("coki");

        }
        private Q Mul(Q q, Q qB)
        {
            var nA = (int)q < 0;
            var nB = (int)qB < 0;
            
            if (nA) q = (Q)(-(int)q);
            if (nB) qB = (Q)(-(int)qB);

            if      (q == Q.E && qB == Q.E) q = Q.E;
            else if (q == Q.E && qB == Q.I) q = Q.I;
            else if (q == Q.E && qB == Q.J) q = Q.J;
            else if (q == Q.E && qB == Q.K) q = Q.K;

            else if (q == Q.I && qB == Q.E) q = Q.I;
            else if (q == Q.I && qB == Q.I) q = Q.Ne;
            else if (q == Q.I && qB == Q.J) q = Q.K;
            else if (q == Q.I && qB == Q.K) q = Q.Nj;

            else if (q == Q.J && qB == Q.E) q = Q.J;
            else if (q == Q.J && qB == Q.I) q = Q.Nk;
            else if (q == Q.J && qB == Q.J) q = Q.Ne;
            else if (q == Q.J && qB == Q.K) q = Q.I;

            else if (q == Q.K && qB == Q.E) q = Q.K;
            else if (q == Q.K && qB == Q.I) q = Q.J;
            else if (q == Q.K && qB == Q.J) q = Q.Ni;
            else if (q == Q.K && qB == Q.K) q = Q.Ne;

            else
                throw new Exception("wtf");

            if (nA != nB)
                q = (Q) (-(int) q);
            return q;
        }

        private Q FromChar(char p0)
        {
            if (p0 == 'i') return Q.I;
            if (p0 == 'j') return Q.J;
            if (p0 == 'k') return Q.K;
            throw new Exception();
        }

        enum Q
        {
            E=1,I=2,J=3,K=4,Ne=-1,Ni=-2,Nj=-3,Nk=-4
        }
    }
}