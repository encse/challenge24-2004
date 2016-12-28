using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2015.QR.D
{
    class OminousOminoSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var yes = "RICHARD";
            var no = "GABRIEL";
            int x, w, h;
            Fetch(out x, out w, out h);

            if(x>=7)
            {
                yield return yes;
                yield break;
            }

            if((w*h)%x!=0)
            {
                yield return yes;
                yield break;
            }

            if(Math.Min(w,h) < (x+1)/2)
            {
                yield return yes;
                yield break;
            }

            if(Math.Max(w,h) < x)
            {
                yield return yes;
                yield break;
            }

            if(Math.Min(w,h) == 2 && x == 4)
            {
                yield return yes;
                yield break;
            }

            if(Math.Min(w,h) == 3 && x == 6)
            {
                yield return yes;
                yield break;
            }

            yield return no;
            yield break;
        }

    }

    public static class Q
    {
        public static string neg(this string s)
        {
            return s[0] == '-' ? s.Substring(1) : '-' + s;
        }

        public static string mul(this string s1, string s2)
        {
            if (s1[0] == '-')
            {
                if (s2[0] == '-')
                {
                    return s1.neg().mul(s2.neg());
                }
                else
                {
                    return s1.neg().mul(s2).neg();
                }
            }
            else if (s2[0] == '-')
            {
                return s1.mul(s2.neg()).neg();
            }


            if (s1 == "1")
            {
                if (s2 == "1")
                    return "1";
                if (s2 == "i")
                    return "i";
                if (s2 == "j")
                    return "j";
                if (s2 == "k")
                    return "k";
            }
            if (s1 == "i")
            {
                if (s2 == "1")
                    return "i";
                if (s2 == "i")
                    return "-1";
                if (s2 == "j")
                    return "k";
                if (s2 == "k")
                    return "-j";
            }
            if (s1 == "j")
            {
                if (s2 == "1")
                    return "j";
                if (s2 == "i")
                    return "-k";
                if (s2 == "j")
                    return "-1";
                if (s2 == "k")
                    return "i";
            }
            if (s1 == "k")
            {
                if (s2 == "1")
                    return "k";
                if (s2 == "i")
                    return "j";
                if (s2 == "j")
                    return "-i";
                if (s2 == "k")
                    return "-1";
            }
            throw new Exception();
        }
    }

}
