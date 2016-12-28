using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2015.QR.C
{
    class DijkstraSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int length;
            decimal count;
            Fetch(out length, out count);
            var sect = Fetch<string>().Select(ch => ch.ToString()).ToList();
            var rgRes = new string[length];

            var x = "1";
            for(var i = length - 1; i>=0;i--)
            {
                x = sect[i].mul(x);
                rgRes[i] = x;
            }
            var resSect = rgRes[0];

            var resAll = "1";
            for(var i=0;i<count % 4; i++)
            {
                resAll = resAll.mul(resSect);
            }

            Func<int, string> resEnd = pos =>
            {
                var i = pos % length;
                var res = rgRes[i];
                var isect = pos / length;
                var rest = count - 1 - isect;
                var mod = rest % 4;

                for(var q = 0; q < mod; q++)
                    res = res.mul(resSect);

                return res;
            };

            Debug.Assert(resAll == resEnd(0));

            if (resAll != "i".mul("j").mul("k"))
            {
                yield return "NO";
                yield break;
            }

            var res1 = "1";
            for(var i1 = 0; i1 < Math.Min(length * 4 + 10, length * count - 2); i1++)
            {
                res1 = res1.mul(sect[i1 % length]);

                Debug.Assert(resAll == res1.mul(resEnd(i1+1)));

                if(res1 != "i")
                    continue;

                if (resEnd(i1 + 1) != "j".mul("k"))
                {
                    continue;
                }

                var res2 = "1";
                for (var i2 = i1 + 1; i2 < Math.Min(i1 + 1 + length * 4 + 10, length * count - 1); i2++)
                {
                    res2 = res2.mul(sect[i2 % length]);

                    Debug.Assert(resAll == res1.mul(res2).mul(resEnd(i2 + 1)));
                    if (res2 != "j")
                        continue;

                    if(resEnd(i2+1) != "k")
                        continue;

                    yield return "YES";
                    yield break;
                }

            }

            yield return "NO";
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
