using System.Linq;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.F
{
    public class FMovieSolver : Solver
    {

        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int cactor, cfilm;
            pparser.Fetch(out cactor, out cfilm);
            var rgprice = pparser.FetchN<int>(cactor).ToArray();

            var mplayed = new bool[cactor,cactor];

            for(int ifilm=0;ifilm<cfilm;ifilm++)
            {
                var rgactor = pparser.Fetch<int[]>().Skip(1).ToArray();
                for(int i = 0;i<rgactor.Length;i++)
                for(int j = 0;j<rgactor.Length;j++)
                {
                    if (i == j)
                        continue;

                    //1 based
                    mplayed[rgactor[i] - 1, rgactor[j]-1] = true;
                }
            }


            int priceMin = int.MaxValue;
            for (int iactor = 0; iactor < cactor; iactor++)
            {
                for (int jactor = 0; jactor < cactor; jactor++)
                {
                    if (iactor == jactor || mplayed[iactor, jactor])
                        continue;

                    var price = rgprice[iactor] + rgprice[jactor];
                    if(price<priceMin)
                    {
                        priceMin = price;
                    }
                }
            }

            using (Output)
            {
                if(priceMin == int.MaxValue)
                    Solwrt.WriteLine("No solution.");
                else
                    Solwrt.WriteLine(priceMin);
            }
        }

       
    }
}


