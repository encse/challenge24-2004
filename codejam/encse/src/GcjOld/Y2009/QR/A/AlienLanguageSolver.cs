using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2009.QR.A
{
    public class AlienLanguageSolver : GcjSolver
    {
        private string[] rgword;
        protected override int CCaseGet()
        {
            /*
             The first line of input contains 3 integers, L, D and N separated by a space. D lines follow, each
             * containing one word of length L. These are the words that are known to exist in the alien language. 
             * N test cases then follow, each on its own line and each consisting of a pattern as described above.
             * You may assume that all known words provided are unique.
             */
            int cword;
            int cchword;
            int cCase;
            Pparser.Fetch(out cchword, out cword, out cCase);
            rgword = Pparser.FetchN<string>(cword).ToArray();
            return cCase;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var pattern = Pparser.StLineNext();
            var rgwordMatch = new List<string>(rgword);
            var ichWord = 0;
            for (var ichPattern = 0; ichPattern < pattern.Length && rgwordMatch.Any();  )
            {
                if (pattern[ichPattern] == '(')
                {
                    var cchPattern = pattern.IndexOf(')', ichPattern) - ichPattern - 1;
                    var rgchAlternatives = pattern.Substring(ichPattern + 1, cchPattern).ToCharArray();
                    rgwordMatch = Filter(rgwordMatch, ichWord, rgchAlternatives);
                    ichPattern += cchPattern + 2;
                }
                else
                {
                    rgwordMatch = Filter(rgwordMatch, ichWord, pattern[ichPattern]);
                    ichPattern++;
                }
                ichWord++;
            }
            yield return rgwordMatch.Count;
        }

        private List<string> Filter(List<string> rgword, int ich, params char[] rgchAlternatives)
        {

            var rgwordResult = new List<string>();
            foreach (var word in rgword)
            {
                if(word[ich].FIn(rgchAlternatives))
                {
                    rgwordResult.Add(word);
                }
            }
            return rgwordResult;
            {
                
            }
        }
    }
}
