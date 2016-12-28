using System.Collections.Generic;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest05.D
{
    class DestructionDroidSolver : Solver
    {
        public override void Solve()
        {
            int ccmd;
            int ccity;
            double xDroid;
            double yDroid;

            Pparser.Fetch(out ccmd, out ccity, out xDroid, out yDroid);
            var droid = new Droid(new Vector(xDroid, yDroid), new Vector(0, 1), 0);

            foreach (var cmd in Pparser.FetchN<Command>(ccmd))
                droid.Do(cmd);

            using (Output)
            {
                var cityAttacked = CityClosest(droid, Pparser.FetchN<City>(ccity));
                WriteLine(cityAttacked.StName);
            }
        }

        private City CityClosest(Droid droid, IEnumerable<City> encity)
        {
            City cityAttacked = null;
            var distMin = double.PositiveInfinity;
            foreach (var city in encity)
            {
                var dist = droid.DistanceFrom(city);
                if (dist < distMin)
                {
                    distMin = dist;
                    cityAttacked = city;
                }
            }
            return cityAttacked;
        }

        public enum Kcommand { LEFT, RIGHT, FASTER, SLOWER, WAIT }

        public class Command
        {

            public Kcommand Kcommand;
            public double P;

            public Command(Kcommand kcommand, double p)
            {
                Kcommand = kcommand;
                P = p;
            }
        }

        public class City
        {
            public readonly string StName;
            public readonly Vector VctPos;

            public City(string stName, double x, double y)
            {
                StName = stName;
                VctPos = new Vector(x, y);
            }
        }

        public class Droid
        {
            public Vector VctPos;
            public Vector VctDir;
            public double Speed;

            public Droid(Vector vctPos, Vector vctDir, double speed)
            {
                VctPos = vctPos;
                VctDir = vctDir;
                Speed = speed;
            }

            public void Do(Command command)
            {
                switch (command.Kcommand)
                {
                    case Kcommand.LEFT: VctDir = VctDir.RotateDeg(command.P); break;
                    case Kcommand.RIGHT: VctDir = VctDir.RotateDeg(-command.P); break;
                    case Kcommand.FASTER: Speed += command.P; break;
                    case Kcommand.SLOWER: Speed -= command.P; break;
                    case Kcommand.WAIT: VctPos = VctPos + Speed * command.P * VctDir; break;
                }
            }

            public double DistanceFrom(City city)
            {
                return (VctPos - city.VctPos).Length();
            }
        }
    }
}
