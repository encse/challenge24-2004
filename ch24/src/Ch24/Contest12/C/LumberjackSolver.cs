using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest12.C
{
 
    public class LumberjackSolver : Solver
    {

        public override void Solve()
        {
            var ctree = Pparser.Fetch<int>();
            var rgheightTree = Pparser.Fetch<int[]>();

            var rgccutIfFallsLeft = new int[ctree];
            var rgccutIfFallsRight = new int[ctree];

            for (int i = 0; i < ctree; i++)
                rgccutIfFallsLeft[i] = rgccutIfFallsRight[i] = int.MaxValue;

            rgccutIfFallsLeft[0] = 1;
           // rgccutIfFallsRight[0] = 1;

            log.Info("setup next right");
            InitItreeNextRight(rgheightTree);
            log.Info("setup next left");
            InitItreeNextLeft(rgheightTree);
            log.Info("done");
            for (var itree = 0;itree<ctree;itree++)
            {
                var itreePrev = itree - 1;

                var v = itree == 0 ? 1 : 1 + Math.Min(rgccutIfFallsLeft[itreePrev], rgccutIfFallsRight[itreePrev]);
                if(v<rgccutIfFallsRight[itree])
                {
                    int jtreeNext = ItreeNextRight(itree);
                    for(int jtree=itree;jtree<jtreeNext;jtree++)
                    {
                        Debug.Assert(v < rgccutIfFallsRight[jtree]);
                        rgccutIfFallsRight[jtree] = v;
                    }
                }

                var itreeNext = ItreeNextLeft(itree); 
                
                if (itreeNext == -1)
                    rgccutIfFallsLeft[itree] = 1;
                else
                    rgccutIfFallsLeft[itree] = 1 + Math.Min(rgccutIfFallsLeft[itreeNext], rgccutIfFallsRight[itreeNext]);
            }

            var rgtreecut = new List<int>(Math.Min(rgccutIfFallsLeft.Last(), rgccutIfFallsRight.Last()));
            Console.WriteLine(Math.Min(rgccutIfFallsLeft.Last(), rgccutIfFallsRight.Last()));
            //for (var itree = ctree-1; itree>=0;)
            //{
            //    var fFallToLeft = rgccutIfFallsLeft[itree] < rgccutIfFallsRight[itree];
            //    if(fFallToLeft)
            //    {
            //        rgtreecut.Insert(0, -(itree+1));
            //        itree = ItreeNext(rgccutIfFallsLeft, itree);
            //    }
            //    else
            //    {
            //        rgtreecut.Insert(0, (itree+1));
            //        itree = ItreeNext(rgccutIfFallsRight, itree);
            //    }
            //}

            //using (var solwrt = new Solwrt(FpatOut()))
            //{
            //    solwrt.WriteLine(rgtreecut.Count);
            //    solwrt.WriteLine(rgtreecut);
            //}
        }

        private string tsto(int[] rgheightTree)
        {
            return rgheightTree.Select(x => x.ToString()).StJoin(" ");
        }

        private int[] mpitreeNextRightByItree;
        private int[] mpitreeNextLeftByItree;

        private int ItreeNextLeft(int itreeStart)
        {
             return mpitreeNextLeftByItree[itreeStart];
        }
       
        private int ItreeNextRight(int itreeStart)
        {
           return mpitreeNextRightByItree[itreeStart];
        }

        private void InitItreeNextLeft(int[] rgheightTree)
        {
            var ctree = rgheightTree.Length;

            mpitreeNextLeftByItree = new int[ctree];
            Mmx mmx = new Mmx();
            for (int itree = 0; itree < ctree; itree++)
            {
                if (itree == 0)
                    mpitreeNextLeftByItree[itree] = -1;
                else if (rgheightTree[itree] == 1)
                    mpitreeNextLeftByItree[itree] = itree - 1;
                else
                {
                    mpitreeNextLeftByItree[itree] = mmx.MinOfN(rgheightTree[itree] - 1);
                    //Min(mpitreeNextLeftByItree, itree - rgheightTree[itree] + 1, itree); 
              //      Debug.Assert(mpitreeNextLeftByItree[itree] == Min(mpitreeNextLeftByItree, itree - rgheightTree[itree] + 1, itree));
                }
                mmx.Add(mpitreeNextLeftByItree[itree]);
                //   Debug.Assert(mpitreeNextLeftByItree[itree] == ItreeNextLeftSlow(rgheightTree, itree));
            }
        }

        private void InitItreeNextRight(int[] rgheightTree)
        {
            var ctree = rgheightTree.Length;

            mpitreeNextRightByItree = new int[ctree];

            Mmx mmx = new Mmx();
            for (int itree = ctree-1; itree >= 0; itree--)
            {
                if (itree == ctree - 1)
                    mpitreeNextRightByItree[itree] = ctree;
                else if (rgheightTree[itree] == 1)
                    mpitreeNextRightByItree[itree] = itree + 1;
                else
                {

                    mpitreeNextRightByItree[itree] = mmx.MaxOfN(rgheightTree[itree]-1);
                 //   Debug.Assert(mpitreeNextRightByItree[itree] ==Max(mpitreeNextRightByItree, itree + 1, itree + rgheightTree[itree]));
                }
                mmx.Add(mpitreeNextRightByItree[itree]);
                //     Debug.Assert(mpitreeNextRightByItree[itree] == ItreeNextRightSlow(rgheightTree, itree));
            }
                
        }

        int Max(int[] rgi, int iStart, int iLim)
        {
            iStart = Math.Max(0, iStart);
            iLim = Math.Min(rgi.Length, iLim);

            int m = int.MinValue;
            for(int i=iStart;i<iLim;i++)
            {
                m = Math.Max(m, rgi[i]);
            }
            return m;
        }

        int Min(int[] rgi, int iStart, int iLim)
        {
            iStart = Math.Max(0, iStart);
            iLim = Math.Min(rgi.Length, iLim);

            int m = int.MaxValue;
            for (int i = iStart; i < iLim; i++)
                m = Math.Min(m, rgi[i]);
            return m;
        }
    }

    internal class Mmx
    {
        private class Pr
        {
            public int citemRange;
            public int v;

            public Pr(int citemRange, int v)
            {
                this.citemRange = citemRange;
                this.v = v;
            }
        }


        private readonly Stack<Pr> stackprMax = new Stack<Pr>();
        private readonly Stack<Pr> stackprMin = new Stack<Pr>();
      
        public void Add(int v)
        {
            
            var citemRange = 1;
            while (stackprMin.Any() && stackprMin.Peek().v >=  v)
            {
                citemRange += stackprMin.Peek().citemRange;
                stackprMin.Pop();
            }
            stackprMin.Push(new Pr(citemRange, v));    

            citemRange = 1;
            while (stackprMax.Any() && stackprMax.Peek().v <= v)
            {
                citemRange += stackprMax.Peek().citemRange;
                stackprMax.Pop();
            }
            stackprMax.Push(new Pr(citemRange, v));    
            
        }

        public int MaxOfN(int citem)
        {
            return XXXOfN(stackprMax, citem);
        }

        public int MinOfN(int citem)
        {
            return XXXOfN(stackprMin, citem);
        }
        
        private int XXXOfN(Stack<Pr> stackpr, int citem)
        {
            if (citem == 0)
                return 1;


            var iitem = 0;
            foreach (var pr in stackpr)
            {
                iitem += pr.citemRange;
                if (iitem >= citem)
                    return pr.v;
            }

            return stackpr.Last().v;    
         }
    }

}
