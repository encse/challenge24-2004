using System;
using System.IO;
using Solvers.Contest04.A;
using Solvers.Contest04.B;
using Solvers.Contest04.C;
using Solvers.Contest04.D;
using Solvers.Contest04.E;
using Solvers.Contest04.F;
using Wecomp.Util;

namespace Wecomp
{
	class Program
	{
		[STAThreadAttribute]
		static void Main()
		{
			Lg.dgIlgFromTy = ty => new LgLog4net(ty);

			if (!Directory.Exists("problemset"))
			{
				Console.Error.WriteLine("Cannot find `problemset` folder.");	
				return;
			}

			new Ch24Runner<AErrorCorrection2Solver>("A-{0}.in", "A{0}.out", "A-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			new Ch24Runner<BKnightsSolver>("B-{0}.in", "B{0}.out", "B-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			new Ch24Runner<CTvProgrammingSolver2>("C-{0}.in", "C{0}.out", "C-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			new Ch24Runner<DWizardsSolver>("D-{0}.in", "D{0}.png", "D-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			new Ch24Runner<ENumbers2Solver>("E-{0}.in", "E{0}.out", "E-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			new Ch24Runner<FMovieSolver>("F-{0}.in", "F{0}.out", "F-{0}.refout")
				.SelectProblems()
				.Run(fParallel: false);

			Console.WriteLine("Press Enter to exit");
			Console.ReadLine();
		}

	}
}
