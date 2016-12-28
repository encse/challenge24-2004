using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gcj.Util;
using Gcj.Y2013.R1B.C;

namespace GcjOld
{
    class Program
    {
        static void Main(string[] args)
        {
            GcjSolver.Solve<GarbledEmailSolver>();
            //ConcurrentGcjSolver.Solve<OutOfGasConcurrentSolver2>(true);
        }
    }
}
