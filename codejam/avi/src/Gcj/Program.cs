using Gcj.Util;
using Gcj.Y2015.R2.B;
using Gcj.Y2015.R2.A;


namespace Gcj
{
    class Program
    {
        static void Main(string[] args)
        {
            GcjSolver.Solve<BSolver>();
            //ConcurrentGcjSolver.Solve<OutOfGasConcurrentSolver2>(true);
        }
    }
}
