using System.Diagnostics;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.E
{
    public class ENumbers2Solver : Solver
    {
        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            rgx = pparser.Fetch<int[]>();
            rgc = pparser.Fetch<int[]>();

            rgGoalA = pparser.Fetch<int[]>();
            rgGoalB = pparser.Fetch<int[]>();

            time = 7;
            Winner winner = Winner.Nil;
            while (winner == Winner.Nil)
                winner = Step();
            string result = winner.ToString() + " wins at " + time.ToString() + ".";

            using(Output)
                Solwrt.WriteLine(result);
        }

        int[] rgx;
		int[] rgc;
		int[] rgGoalA;
		int[] rgGoalB;

		int time;

		public enum Winner {A, B, Nil};

		public void Solve(string fileName, bool writeToFile)
		{
            
		}

		public Winner Step()
		{
			Winner winner = Check();
			if(winner != Winner.Nil) return winner;
			int newx = 0;
			for(int i=0; i<8; i++)
			{
				newx = (newx + (rgx[7-i] * rgc[i])) % 1000;
			}
			
			for(int i=0;i<7;i++)
			{
				rgx[i] = rgx[i+1];
			}
			rgx[7] = newx;
			time++;
			return winner;
		}

		public Winner Check()
		{
			bool fa = true;
			bool fb = true;
			for(int i=0;(fa || fb) && i<rgx.Length; i++)
			{
				if(rgx[i] != rgGoalA[i]) fa = false;
				if(rgx[i] != rgGoalB[i]) fb = false;
			}
			Debug.Assert(!(fa&&fb));
			if(fa) return Winner.A;
			if(fb) return Winner.B;
			return Winner.Nil;
		}
    }
}


