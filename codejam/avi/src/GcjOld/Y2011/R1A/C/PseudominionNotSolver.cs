using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2011.R1A.C
{
    internal class PseudominionNotSolver : GcjSolver
    {
        private class Card
        {
            public int draw;
            public int score;
            public int action;

            public override string ToString()
            {
                return string.Format("Draw: {0}, Score: {1}, Action: {2}", draw, score, action);
            }
        }

        private class State
        {
            public int cactionLeft;
            public HashSet<Card> hlmcardHand;
            public int icardDeckNext;
            public HashSet<Card> hlmcardPlayed;
            public int score;

            public bool Equals(State other)
            {
                if(ReferenceEquals(null, other))
                    return false;
                if(ReferenceEquals(this, other))
                    return true;
                return Equals(other.hlmcardPlayed, hlmcardPlayed);
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;
                if(ReferenceEquals(this, obj))
                    return true;
                if(obj.GetType() != typeof(State))
                    return false;
                return Equals((State) obj);
            }

            public override int GetHashCode()
            {
                return (hlmcardPlayed != null ? hlmcardPlayed.GetHashCode() : 0);
            }

            public override string ToString()
            {
                return string.Format("CactionLeft: {0}, HlmcardHand: {1}, IcardDeckNext: {2}, HlmcardPlayed: {3}, Score: {4}", cactionLeft, hlmcardHand, icardDeckNext, hlmcardPlayed, score);
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var rgcardHand = new int[Fetch<int>()].Select(_ => Fetch<Card>()).ToList();
            var rgcardDeck = new int[Fetch<int>()].Select(_ => Fetch<Card>()).ToList();

            var play = new Action<State, Card>((state, card) =>
            {
                    state.hlmcardHand.Remove(card);
                    state.hlmcardPlayed.Add(card);
                    state.cactionLeft += card.action - 1;
                    state.score += card.score;
                    var draw = Math.Min(card.draw, rgcardDeck.Count - state.icardDeckNext);
                    state.hlmcardHand.UnionWith(rgcardDeck.GetRange(state.icardDeckNext,draw));
                    state.icardDeckNext += draw;
            });

            var close = new Action<State>(state =>
            {
                for(;;)
                {
                    var card = state.hlmcardHand.FirstOrDefault(cardT => cardT.action > 0);
                    
                    if(card==null)
                        break;
                    play(state, card);
                }

                //if(state.icardDeckNext == rgcardDeck.Count && state.cactionLeft >= state.hlmcardHand.Count)
                //{
                //    state.score += state.hlmcardHand.Sum(card => card.score);
                //    state.hlmcardHand.Clear();
                //}
            });

            var fEnd = new Func<State, bool>(state => state.cactionLeft == 0 || state.hlmcardHand.Count == 0);

            var scoreMax = new Func<State, int>(state =>
            {
                var icardDeck = state.icardDeckNext;
                var rgcard = new List<Card>(state.hlmcardHand);

                for(var icard = 0;icard<rgcard.Count && icardDeck < rgcardDeck.Count;icard++)
                {
                    var card = rgcard[icard];
                    for(var idraw = 0;idraw<card.draw;idraw++)
                    {
                        rgcard.Add(rgcardDeck[icardDeck]);
                        icardDeck++;
                    }
                }

                var score = state.score;
                var caction = state.cactionLeft;

                var rgcardAction = rgcard.Where(card => card.action > 0).ToArray();
                score += rgcardAction.Sum(card => card.score);
                caction += rgcardAction.Sum(card => card.action - 1);

                score += rgcard.Where(card => card.action == 0).OrderByDescending(card => card.score).Take(caction).Sum(card => card.score);
                return score;
            });

            var stateStart = new State {cactionLeft = 1, hlmcardHand = new HashSet<Card>(rgcardHand), hlmcardPlayed = new HashSet<Card>(), icardDeckNext = 0};
            close(stateStart);

            var scoreLimit = scoreMax(stateStart);

            yield return new Astar<State, int>(
                new[] {new Tuple<State, int>(stateStart, -stateStart.score)},
                fEnd,
                (state, _) => state.hlmcardHand.Select(card =>
                {
                    var stateNew = new State
                    {
                        cactionLeft = state.cactionLeft,
                        hlmcardHand = new HashSet<Card>(state.hlmcardHand),
                        hlmcardPlayed = new HashSet<Card>(state.hlmcardPlayed),
                        score = state.score,
                        icardDeckNext = state.icardDeckNext
                    };
                    play(stateNew, card);
                    close(stateNew);
                    return stateNew;
                }),
                (_, __, state) => -state.score,
                (state, _) => fEnd(state) ? -state.score : -scoreMax(state)
                ).Find().Item1.score;
            yield break;


            if(fEnd(stateStart))
            {
                yield return stateStart.score;
                yield break;
            }

            var mprgstateBycaction = new SortedDictionary<int, HashSet<State>> {{0, new HashSet<State> {stateStart}}};

            var maxScore = int.MinValue;
            for(;mprgstateBycaction.Count!=0;)
            {

                var kvprgstateBycaction = mprgstateBycaction.First();
                mprgstateBycaction.Remove(kvprgstateBycaction.Key);

                foreach(var state in kvprgstateBycaction.Value)
                {
                    foreach(var card in state.hlmcardHand)
                    {
                        var stateNew = new State
                        {
                            cactionLeft = state.cactionLeft,
                            hlmcardHand = new HashSet<Card>(state.hlmcardHand),
                            hlmcardPlayed = new HashSet<Card>(state.hlmcardPlayed),
                            score = state.score,
                            icardDeckNext = state.icardDeckNext
                        };
                        play(stateNew, card);
                        close(stateNew);

                        if(fEnd(stateNew))
                        {
                            maxScore = Math.Max(maxScore, stateNew.score);
                            if(maxScore == scoreLimit)
                            {
                                yield return maxScore;
                                yield break;
                            }
                        }
                        else if(scoreMax(stateNew)<maxScore)
                        {
                            //skip
                        }
                        else
                        {
                            mprgstateBycaction.EnsureGet(stateNew.hlmcardPlayed.Count).Add(stateNew);
                        }
                    }
                }
            }

            yield return maxScore;
        }

    }
}
