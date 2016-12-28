using System;
using System.Linq;
using Ch24.Contest14.A;
using Ch24.Contest16.A;
using Cmn.Util;

namespace Ch24
{
    class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            Lg.dgIlgFromTy = ty => new LgLog4net(ty);

            new Ch24Runner<ASafeSolver>("{0}.in", "a{0}.out", "a{0}.refout")
                .SelectProblems()
                .Run(fParallel: false);
           
            Console.ReadLine();
        }

//        private static double F(double x, double y)
//        {
//            return 3*x*y - 2*x + 1+y+6*y*y;
//        }

//        public static void f(double[] c, double[] x, ref double y, object obj)
//        {
//            y = c[0]*x[0]*x[0] + c[1]*x[0] + c[2]*x[1]+c[3]*x[1]*x[1]+c[4]*x[0]*x[1]+c[5];
//        }

//        private static void Foo()
//        {
//            const double diffstep = 0.0001;
//            const int epsf = 0;
//            const int epsx = 0;
//            const int maxits = 1000000;


//            // úgy hogy minimalizáljuk a 

//            //kezdeti közelítés (a korlátokon belül)
//            double[] cInit = new double[]{0,0,0,0,0,0};

//            //alsó és felső korlátok a paraméterekre:
//            var cLower = new double[]{-100,-100,-100, -100,-100, -100};
//            var cUpper = new double[] { 100, 100, 100,100,100,100 };

//            //észlelések időpontja
//            var rgX = new double[8,2]{ {-10, -10},{2,3},{0,1},{10,-5},{4,7},{3,2},{2,9},{9,1}};
//            //észlelt frekvenciák:
//            var rgY = new double[rgX.GetLength(0)];
//            for(var i = 0;i<rgX.GetLength(0);i++)
//            {
//                rgY[i] = F(rgX[i,0],rgX[i,1]);
//            }
 
//            alglib.lsfitstate state;

//            //csak az f(x|c)-t kell számolnunk
//            alglib.lsfitcreatef(rgX, rgY, cInit, diffstep, out state);

//            //constraintek
//            alglib.lsfitsetbc(state, cLower, cUpper);

//            //leállási feltétel
//            alglib.lsfitsetcond(state, epsf, epsx, maxits);

//            //számoljá
//            alglib.lsfitfit(state, f, null, null);

//            int info;
//            double[] cSolution;
//            alglib.lsfitreport rep;
//            alglib.lsfitresults(state, out info, out cSolution, out rep);

////            if (info != 2 )
////                throw new Exception("coki");

//            Console.WriteLine("{0}x^2+{1}x+{2}y+{3}y^2+{4}xy+{5}", cSolution[0], cSolution[1], cSolution[2], cSolution[3], cSolution[4], cSolution[5]);
//        }
    }
}
