using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.B
{
    public class BKnightsSolver : Solver
    {
        const char knightFirst = 'a';

        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int cknight, cconstraint;
            pparser.Fetch(out cknight, out cconstraint);

            var knightLim = knightFirst + cknight;
            var mprgknightNeedsByknight = new Dictionary<char, List<char>>();
            var mprgknightHatesByknight = new Dictionary<char, List<char>>();

            for(var knight=knightFirst;knight < knightLim;knight++)
            {
                mprgknightHatesByknight[knight] = new List<char>();
                mprgknightNeedsByknight[knight] = new List<char>();
            }

            for (var i = 0; i < cconstraint; i++)
            {
                var rgst = pparser.StLineNext().Split(' ');
                char knight = rgst[0][0];
                var rgknight = rgst[1] == "needs" ? mprgknightNeedsByknight[knight] : mprgknightHatesByknight[knight];
                for (int l = 2; l < rgst.Length; l += 2)
                    rgknight.Add(rgst[l][0]);
            }


            var rgknightNotSeen = new List<char>();
            for(var knight=knightFirst; knight<knightLim;knight++)
                rgknightNotSeen.Add(knight);

            using (Output)
            {

                foreach (var knights in Enknights(rgknightNotSeen, null, null, mprgknightNeedsByknight, mprgknightHatesByknight))
                {

                    if (!FOk(knights[cknight - 1], knights[0], knights[1], mprgknightNeedsByknight, mprgknightHatesByknight))
                        continue;
                    
                    if (!FOk(knights[cknight - 2], knights[cknight - 1], knights[0], mprgknightNeedsByknight, mprgknightHatesByknight))
                        continue;

                    for (int i = 0; i < knights.Length;i++ )
                    {
                        if (!FOk(knights[(i - 1 + knights.Length) % knights.Length], knights[i], knights[(i + 1 + knights.Length) % knights.Length],
                            mprgknightNeedsByknight, mprgknightHatesByknight))
                            throw new Exception("coki");
                    }
                  
                    Solwrt.WriteLine(knights);
                }
            }
            
                
        }

        private static IEnumerable<string> Enknights(List<char> rgknightNotSeen, char? knightPrevPrev, char? knightPrev, 
            Dictionary<char, List<char>> mprgknightNeedsByknight, Dictionary<char, List<char>> mprgknightHatesByknight)
        {

            if (rgknightNotSeen.Count == 0)
                yield return "";

            for (int i = 0; i < rgknightNotSeen.Count;i++ )
            {
                var knight = rgknightNotSeen[i];

                if (!FOk(knightPrevPrev, knightPrev, knight, mprgknightNeedsByknight, mprgknightHatesByknight))
                    continue;
                
                rgknightNotSeen.RemoveAt(i);

                foreach (var knights in Enknights(rgknightNotSeen, knightPrev, knight, mprgknightNeedsByknight, mprgknightHatesByknight))
                    yield return knight + knights;

                rgknightNotSeen.Insert(i, knight);
            }
        }

        private static bool FOk(char? knightPrev, char? knight, char knightNext, 
            Dictionary<char, List<char>> mprgknightNeedsByknight, Dictionary<char, List<char>> mprgknightHatesByknight)
        {
            if(!knight.HasValue)
                return true;

            if (mprgknightHatesByknight[knight.Value].Contains(knightNext))
                return false;

            if (mprgknightHatesByknight[knightNext].Contains(knight.Value))
                return false;

            if (!knightPrev.HasValue)
                return true;


            var rgknightNeeds = mprgknightNeedsByknight[knight.Value];
            if (!rgknightNeeds.Any())
                return true;

            if (!rgknightNeeds.Contains(knightPrev.Value) && !rgknightNeeds.Contains(knightNext))
                return false;
            
            return true;
        }
    }
}


