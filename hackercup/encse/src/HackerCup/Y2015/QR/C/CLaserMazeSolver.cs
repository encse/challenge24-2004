using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.QR.C
{
    class CLaserMazeSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var maze = FetchMaze(pparser);
            return () => Solve(maze);
        }

        private static IEnumerable<object> Solve(Maze maze)
        {
            var qstate = new Queue<State>();
            var fSeen = new HashSet<State>();
            qstate.Enqueue(maze.StartState);
            while (qstate.Any())
            {
                var state = qstate.Dequeue();

                if (maze.FGoal(state))
                {
                    yield return state.Time;
                    yield break;
                }
                foreach (var stateNext in maze.Steps(state))
                {
                    if (!fSeen.Contains(stateNext))
                        qstate.Enqueue(stateNext);

                    fSeen.Add(stateNext);
                }
            }
            yield return "impossible";
        }

        private static void Trace(Maze maze, State state)
        {
            if(state == null)
                return;
            Trace(maze, state.StatePrev);
            Console.WriteLine("time: " + state.Time);
            maze.Tsto(state);
            Console.WriteLine();
            Console.WriteLine();
        }

        private static Maze FetchMaze(Pparser pparser)
        {
            int crow, ccol;
            pparser.Fetch(out crow, out ccol);
            var lab = new char[crow, ccol];
            int irowStart = -1, icolStart = -1, irowGoal = -1, icolGoal = -1;

            for (var irow = 0; irow < crow; irow++)
            {
                var rgch = pparser.StLineNext().ToCharArray();
                for (var icol = 0; icol < ccol; icol++)
                {
                    var ch = rgch[icol];
                    lab[irow, icol] = ch;
                    if (ch == 'S')
                    {
                        irowStart = irow;
                        icolStart = icol;
                    }
                    if (ch == 'G')
                    {
                        irowGoal = irow;
                        icolGoal = icol;
                    }
                 }
            }

            var lab0 = new Lab(lab);
            var lab1 = lab0.Rotate();
            var lab2 = lab1.Rotate();
            var lab3 = lab2.Rotate();

            return new Maze(
                new State(irowStart, icolStart, 0),
                new[]
                {
                    lab0.Shoot(),
                    lab1.Shoot(),
                    lab2.Shoot(),
                    lab3.Shoot(),
                },
                irowGoal, icolGoal
            );

        }

    }

    class State
    {
        public readonly int Irow;
        public readonly int Icol;
        public readonly int Time;
        public readonly State StatePrev;

        public int Ilab
        {
            get { return Time%4; }
        }


        public State(int irow, int icol, int time, State statePrev = null)
        {
            Irow = irow;
            Icol = icol;
            Time = time;
            StatePrev = statePrev;
        }

        protected bool Equals(State other)
        {
            return Irow == other.Irow && Icol == other.Icol && Ilab == other.Ilab;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((State) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Irow;
                hashCode = (hashCode*397) ^ Icol;
                hashCode = (hashCode*397) ^ Ilab;
                return hashCode;
            }
        }
    }

    internal class Maze
    {
        public readonly State StartState;
        private readonly Lab[] rglab;
        private readonly int irowGoal;
        private readonly int icolGoal;

        public Maze(State startState, Lab[] rglab, int irowGoal, int icolGoal)
        {
            this.StartState = startState;
            this.rglab = rglab;
            this.irowGoal = irowGoal;
            this.icolGoal = icolGoal;
        }

        public bool FGoal(State state)
        {
            return FFree(state) && state.Icol == icolGoal && state.Irow == irowGoal;
        }

        public bool FFree(State state)
        {
            return rglab[state.Ilab].FFree(state.Irow, state.Icol);
        }

        public IEnumerable<State> Steps(State state)
        {
            return new[]
            {
                new State(state.Irow, state.Icol - 1, state.Time + 1, state),
                new State(state.Irow, state.Icol + 1, state.Time + 1, state),
                new State(state.Irow - 1, state.Icol, state.Time + 1, state),
                new State(state.Irow + 1, state.Icol, state.Time + 1, state)
            }.Where(FFree);
        }

        public void Tsto(State state)
        {
            rglab[state.Ilab].WithPlayer(state.Irow, state.Icol).Tsto();
        }
    }

    class Lab
    {
        private readonly char[,] lab;
        private readonly int crow;
        private readonly int ccol;

        public Lab(char[,] lab)
        {
            this.lab = lab;
            crow = lab.GetLength(0);
            ccol = lab.GetLength(1);
        }

        public Lab Rotate()
        {
            var labDst = new char[crow, ccol];

            for (var irow = 0; irow < crow; irow++)
            {
                for (var icol = 0; icol < ccol; icol++)
                {
                    var ch = lab[irow, icol];
                    switch (ch)
                    {
                        case '>': ch = 'v'; break;
                        case 'v': ch = '<'; break;
                        case '<': ch = '^'; break;
                        case '^': ch = '>'; break;
                    }
                    labDst[irow, icol] = ch;
                }
            }
            return new Lab(labDst);
        }

        public Lab WithPlayer(int irowPlayer, int icolPlayer)
        {
            var labDst = new char[crow, ccol];

            for (var irow = 0; irow < crow; irow++)
            {
                for (var icol = 0; icol < ccol; icol++)
                {
                    var ch = irow == irowPlayer && icol == icolPlayer ? '@' : lab[irow, icol];
                    labDst[irow, icol] = ch;
                }
            }
            return new Lab(labDst);
        }

        public Lab Shoot()
        {
            var labDst = new char[crow, ccol];

            for (var irow = 0; irow < crow; irow++)
                for (var icol = 0; icol < ccol; icol++)
                    labDst[irow, icol] = lab[irow, icol];

            for (var irow = 0; irow < crow; irow++)
            {
                for (var icol = 0; icol < ccol; icol++)
                {
                    var dcol = 0;
                    var drow = 0;

                    var chSrc = lab[irow, icol];

                    switch (chSrc)
                    {
                        case '>': dcol = 1; break;
                        case '<': dcol = -1; break;
                        case '^': drow = -1; break;
                        case 'v': drow = 1; break;
                    }

                    if (dcol != 0 || drow != 0)
                    {
                        var irowT = irow;
                        var icolT = icol;
                        while (true)
                        {
                            irowT += drow;
                            icolT += dcol;
                            if (!FFree(irowT, icolT))
                                break;
                            labDst[irowT, icolT] = chSrc;
                        }
                    }
                }
            }
            return new Lab(labDst);
        }


        public bool FFree(int irow, int icol)
        {
            return irow >= 0 && irow < crow && icol >= 0 && icol < ccol && 
                (lab[irow, icol] == '.' || lab[irow, icol] == 'S' || lab[irow, icol] == 'G');
        }

        public void Tsto()
        {
            for (var irow = 0; irow < crow; irow++)
            {
                for (var icol = 0; icol < ccol; icol++)
                    Console.Write(lab[irow, icol]);
                Console.WriteLine();
            }
        }
    }
}
