using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ch24.Contest13.A
{
    public class Ceo2Solver : Contest.Solver
    {
        public class Mima
        {
            public Mima MimaParent;
            public List<Mima> RgmimaChild = new List<Mima>();
            public int cmimaLeafChild;
            public int cChild;
            public bool fLoaded;

            public int cAll
            {
                get
                {
                    return cChild + RgmimaChild.Count;
                }
            }

            public bool fAllChildFeaf()
            {
                return RgmimaChild.Any() && cmimaLeafChild == RgmimaChild.Count;
            }
        }

        public override void Solve()
        {
            var rg = Fetch<int[]>();
            var cmima = rg[0];
            var climit = rg[1];

            var rgmima = new Mima[cmima];

            for(var imima = 0; imima < cmima; imima++)
                rgmima[imima] = new Mima();

            for(var imima = 0; imima < cmima; imima++)
            {
                var mima = rgmima[imima];
                mima.fLoaded = true;

                rg = Fetch<int[]>();

                mima.cChild = rg[0];

                if(rg.Length > 2)
                {
                    foreach(var mimaChild in rg.Skip(2).Select(imimaChild => rgmima[imimaChild]))
                    {
                        mima.RgmimaChild.Add(mimaChild);
                        Debug.Assert(mimaChild.MimaParent == null);
                        mimaChild.MimaParent = mima;

                        if(mimaChild.fLoaded && !mimaChild.RgmimaChild.Any())
                            mima.cmimaLeafChild++;
                    }
                }

                if(mima.MimaParent!=null && !mima.RgmimaChild.Any())
                {
                    mima.MimaParent.cmimaLeafChild++;
                }
            }

            var cfired = 0;

            for(var qumima=new Queue<Mima>(rgmima.Where(mima => mima.fAllChildFeaf()));qumima.Count>0;)
            {
                var mima = qumima.Dequeue();

                Debug.Assert(mima.RgmimaChild.All(mimaLeaf => !mimaLeaf.RgmimaChild.Any()));

                mima.RgmimaChild.Sort((mima1,mima2)=>mima2.cChild-mima1.cChild);
                
                for(;mima.RgmimaChild.Any();)
                {
                    var mimaLeaf = mima.RgmimaChild.Last();

                    var cAllNew = mima.cAll - 1 + mimaLeaf.cChild;
                    if(cAllNew > climit)
                        break;

                    cfired++;
                    mima.RgmimaChild.RemoveAt(mima.RgmimaChild.Count - 1);
                    mima.cChild += mimaLeaf.cChild;
                    Debug.Assert(mima.cAll == cAllNew);
                }

                mima.cChild += mima.RgmimaChild.Count;
                mima.RgmimaChild.Clear();

                if(mima.MimaParent==null)
                    break;

                mima.MimaParent.cmimaLeafChild++;
                if(mima.MimaParent.fAllChildFeaf())
                    qumima.Enqueue(mima.MimaParent);
            }

            
            using(Output)
            {
                Output.StNewLine = "\r\n";
                WriteLine(cfired);
            }
        }
    }
}
