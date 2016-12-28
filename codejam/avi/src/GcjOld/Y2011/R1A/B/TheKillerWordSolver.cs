using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R1A.B
{
    internal class TheKillerWordSolver : GcjSolver
    {
        private class Word
        {
            public string st;
            public int i;
        }

        private class State
        {
            public int score;
            public List<Word> rgword;
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            int cword;
            int cstrat;

            Fetch(out cword, out cstrat);
            
            var rgrgword = new int[cword].Select((_,i) => new Word{ i = i, st = Fetch<string>()}).GroupBy(word => word.st.Length).ToList();
            var rgstrat = new int[cstrat].Select(_ => Fetch<string>()).ToList();

            foreach(var strat in rgstrat)
            {
                var maxScore = -1;
                var maxWord = new Word{i=int.MaxValue};
                foreach(var rgwordAll in rgrgword)
                {

                    var mprgwordbyans = new Dictionary<string, State> {{new string('_', rgwordAll.Key),new State{score = 0, rgword = rgwordAll.ToList()}}};

                    foreach(var tip in strat)
                    {
                        var mprgwordbyansNew = new Dictionary<string, State>();
                        foreach(var kvprgwordbyans in mprgwordbyans)
                        {
                            var ans = kvprgwordbyans.Key;
                            var state = kvprgwordbyans.Value;

                            if(state.rgword.Count == 1)
                            {
                                var word = state.rgword.Single();
                                if(state.score>maxScore || (state.score == maxScore && word.i < maxWord.i))
                                {
                                    maxScore = state.score;
                                    maxWord = word;
                                }
                                continue;
                            }

                            var fSkipTip = true;
                            var fStay = false;
                            foreach(var word in state.rgword)
                            {
                                var ansNew = addMatch(ans, word, tip);
                                if(fSkipTip && ansNew != ans)
                                    fSkipTip = false;
                                if(!fStay && ansNew == ans)
                                    fStay = true;

                                mprgwordbyansNew.EnsureGet(ansNew, () => new State{score = state.score, rgword = new List<Word>()}).rgword.Add(word);
                            }

                            if(!fSkipTip && fStay)
                                mprgwordbyansNew[ans].score += 1;

                        }

                        mprgwordbyans = mprgwordbyansNew;
                    }

                    foreach(var kvprgwordbyans in mprgwordbyans)
                    {
                            var ans = kvprgwordbyans.Key;
                            var state = kvprgwordbyans.Value;

                        Debug.Assert(state.rgword.Count == 1);
                        var word = state.rgword.Single();
                        if(state.score>maxScore || (state.score == maxScore && word.i < maxWord.i))
                        {
                            maxScore = state.score;
                            maxWord = word;
                        }

                    }

                }
                yield return maxWord.st;
            }
        }

        private string addMatch(string ans, Word word, char tip)
        {
            var rgch = ans.ToCharArray();
            for(var ich = word.st.IndexOf(tip);ich!=-1;ich = word.st.IndexOf(tip, ich+1))
            {
                rgch[ich] = tip;
            }
            return new string(rgch);
        }
    }
}
