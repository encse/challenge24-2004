using System;
using System.Security.Cryptography;
using System.Threading;
using Gcj.Util;
using HackerCup.Y2013.QR.A;
using HackerCup.Y2013.QR.B;
using HackerCup.Y2013.QR.C;
using HackerCup.Y2014.QR.A;
using HackerCup.Y2014.QR.B;
using HackerCup.Y2014.QR.C;
using HackerCup.Y2015.QR.A;
using HackerCup.Y2015.QR.C;
using HackerCup.Y2015.R1.A;
using HackerCup.Y2015.R1.B;
using HackerCup.Y2015.R1.C;
using HackerCup.Y2015.R1.D;
using HackerCup.Y2015.R2.A;
using HackerCup.Y2015.R2.B;
using HackerCup.Y2015.R2.C;

namespace HackerCup
{
    class Program
    {
        static void Main(string[] args)
        {
//            var T = new Thread(() => { }, 200000000);
//            T.Start();

            ConcurrentGcjSolver.Solve<BAllCriticalSolver>(true, true);
        }
    }
}
