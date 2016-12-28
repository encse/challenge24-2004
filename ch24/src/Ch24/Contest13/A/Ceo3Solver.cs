using System;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest13.A
{
    public class Ceo3Solver : Contest.Solver
    {
        private Func<Man, int, Tuple<int, int>> f;
        private Func<Man, int, int, Tuple<int, int>> f2;

        private int M;
        public override void Solve()
        {
            f = U.ToCached<Man, int, Tuple<int, int>>(F);
            f2 = U.ToCached<Man, int, int, Tuple<int, int>>(F2);

            var pp = new Pparser(FpatIn);
            int cman;
            pp.Fetch(out cman, out M);

            var rgman = new Man[cman];
            for (int i = 0; i < cman; i++)
                rgman[i] = new Man();

            for (int i = 0; i < cman; i++)
            {
                var man = rgman[i];
                man.Iman = i;
                int csuboNonMan, csuboMan;
                int[] rgImanUnder;

                pp.Fetch(out csuboNonMan, out csuboMan, out rgImanUnder);
                man.CchildNonMan = csuboNonMan;
                man.RgmanUnder = rgImanUnder.Select(iman => rgman[iman]).ToArray();
                foreach (var manT in man.RgmanUnder)
                    manT.ManParent = man;
            }

            //var cm = f2(rgman[0], 0, M - rgman[0].CchildNonMan);
            //Console.WriteLine(cm.Item1);


            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                solwrt.StNewLine = "\r\n";
                solwrt.WriteLine(G(rgman[0]));
            }
            // Console.WriteLine(cm.Item2);
        }

        //ennyit tudunk kir�gni man alatt, ha man-b�l M mehet fel
        private Tuple<int,int> F(Man man, int mLim)
        {
            if(man.CchildMan == 0)
            {  
                //nincsenek alatta managerek
                if (man.Cchild  <= mLim)
                {
                    //�t is ki tudjuk r�gni
                    return new Tuple<int, int>(1, man.Cchild);
                }
                else
                {
                    //�t nem tudjuk kir�gni
                    return new Tuple<int, int>(0, 1);
                }
            }
            else
            {   
                //vannak alatta managerek
                if(man.Cchild <= mLim)
                {
                    //�t is ki tudjuk r�gni
                    var cmChildrenNemKirug = f2(man, 0, M - man.CchildNonMan);    
                    var cmChildrenKirug = f2(man, 0, mLim - man.CchildNonMan);

                    var cNemKirug = cmChildrenNemKirug.Item1;
                    var mNemKirug = cmChildrenNemKirug.Item2;
                    
                    var cKirug = cmChildrenKirug.Item1;
                    var mKirug = cmChildrenKirug.Item2;
                    
                    if (cNemKirug >= cKirug + 1)
                        return new Tuple<int, int>(cNemKirug, 1);
                    else
                        return new Tuple<int, int>(cKirug + 1, mKirug + man.CchildNonMan);
                }
                else
                {
                    //�t nem tudjuk kir�gni
                    var cmChildrenNemKirug = f2(man, 0, M - man.CchildNonMan);
                    return new Tuple<int, int>(cmChildrenNemKirug.Item1, 1);
                }
            }
        }

        //ennyit tudunk kir�gni man i-edik gyerek�t tartalmaz� r�szf�j�b�l, ha MLim mehet fel bel�le, M fog ekkor felmenni
        private Tuple<int, int> F2(Man man, int imanChild, int mLim)
        {
            if (mLim < man.CchildMan - imanChild)
                throw new Exception("wtf");

            //ez az utols� gyerek
            if (imanChild == man.CchildMan - 1)
                return f(man.RgmanUnder[imanChild], mLim);

            var cMax = -1;
            var m = -1;

            for (var i = 1; i <= mLim; i++)
            {
                
                var cmThis = f(man.RgmanUnder[imanChild], i);
                var cThis = cmThis.Item1;
                var mThis = cmThis.Item2;

                //van m�g annyi hely ami el�g a t�bbieknek?
                var cmanRest = man.CchildMan - imanChild - 1;

                var mLimRest = mLim - mThis;
                if(mLimRest < cmanRest)
                    continue;
 
                var cmRest = f2(man, imanChild + 1, mLimRest);
                var cRest = cmRest.Item1;
                var mRest = cmRest.Item2;
                
                if (cThis + cRest > cMax)
                {
                    m = mThis + mRest;
                    cMax = cThis + cRest;
                }
            }
            return new Tuple<int, int>(cMax, m);
        }

        private void Fire(Man man)
        {
            var manParent = man.ManParent;
                
            foreach (var manT in man.RgmanUnder)
                manT.ManParent = manParent;

            manParent.RgmanUnder = manParent.RgmanUnder.Where(manT => manT != man).Concat(man.RgmanUnder).ToArray();
            manParent.CchildNonMan += man.CchildNonMan;

            man.ManParent = null;
        }

        private int G(Man man)
        {
            int cResult= 0;
            foreach (var manChild in man.RgmanUnder)
                cResult += G(manChild);

            //most m�r az �sszes gyerek tel�tett.
            while (true)
            {
                //azt v�lasztjuk a kir�ghat�k k�z�l akinek a legkevesebb gyereke van
                int cchildMin = int.MaxValue;
                Man mantoFire = null;
                foreach (var manChild in man.RgmanUnder)
                {
                    if (manChild.Cchild + man.Cchild - 1 > M)
                        continue;

                    //ezt ki lehet r�gni
                    if (manChild.Cchild < cchildMin)
                    {
                        mantoFire = manChild;
                        cchildMin = manChild.Cchild;
                    }
                }

                //nincs kir�ghat�
                if(mantoFire == null)
                    break;

                cResult++;
                Fire(mantoFire);

            }

            return cResult;
        }


        public class Man
        {
            public Man ManParent { get; set; }
            public Man[] RgmanUnder { get; set; }
            public int CchildNonMan { get; set; }
            public int CchildMan { get { return RgmanUnder.Length; } }
            public int Iman { get; set; }

            public int Cchild { get { return CchildNonMan + CchildMan; } }

        }
    }
}
