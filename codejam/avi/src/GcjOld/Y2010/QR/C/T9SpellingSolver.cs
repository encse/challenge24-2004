using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2010.QR.C
{
    internal class T9SpellingSolver : GcjSolver
    {
        public Dictionary<char, string> mpstByCh;

        public T9SpellingSolver()
        {
            mpstByCh = new Dictionary<char, string>();
            var cnum = 0;
            var num = '?';
            foreach(var ch in "2abc3def4ghi5jkl6mno7pqrs8tuv9wxyz0 ")
            {
                int t;
                if(int.TryParse(ch.ToString(), out t))
                {
                    cnum = 0;
                    num = ch;
                }
                else
                {
                    cnum++;
                    mpstByCh[ch] = new string(num, cnum);
                }
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var chLast = '?';
            yield return Fetch<string>().Select(ch =>
            {
                var st = mpstByCh[ch];
                if(chLast==st.First())
                    st = " " + st;
                chLast = st.Last();
                return st;
            }).StJoin(string.Empty);
        }
    }
}
