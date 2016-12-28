using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.B
{
    public class BKnightsSolver2 : Solver
    {
        const char knightFirst = 'a';

        public override void Solve()
        {
            using (Output)
            {
                new ExerciseB(FpatIn).Find(Solwrt);    
            }
            

        }
        class ExerciseB
        {
            public int KnigtsNumber;
            public int RequirementsNumber;
            public ArrayListHashtable hatesMap = new ArrayListHashtable();
            public ArrayListHashtable needsMap = new ArrayListHashtable();

            public ExerciseB(string fileName)
            {
               var pparser = new Pparser(fileName);

                int[] ints = pparser.Fetch<int[]>();

                KnigtsNumber = ints[0];
                RequirementsNumber = ints[1];

                for (int i = 0; i < RequirementsNumber; i++)
                {
                    char knight;
                    int index = -1;
                    var line = pparser.StLineNext();

                    knight = line[0];

                    if ((index = line.IndexOf("hates")) != -1)
                    {
                        line = line.Substring(8);
                        string[] fragments = line.Split(' ');

                        for (int j = 0; j < fragments.Length; j++)
                        {
                            if (fragments[j].Length == 1)
                            {
                                hatesMap.AddToList(knight, fragments[j][0]);
                            }
                        }
                    }
                    else if ((index = line.IndexOf("needs")) != -1)
                    {
                        line = line.Substring(8);
                        string[] fragments = line.Split(' ');

                        for (int j = 0; j < fragments.Length; j++)
                        {
                            if (fragments[j].Length == 1)
                            {
                                needsMap.AddToList(knight, fragments[j][0]);
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }

            }

            public void Print()
            {
                foreach (char knight in needsMap.Keys)
                {
                    Console.Write(knight + " needs ");

                    foreach (char other in (ArrayList)needsMap[knight])
                    {
                        Console.Write(other + " ");
                    }

                    Console.WriteLine();
                }

                foreach (char knight in hatesMap.Keys)
                {
                    Console.Write(knight + " hates ");

                    foreach (char other in (ArrayList)hatesMap[knight])
                    {
                        Console.Write(other + " ");
                    }

                    Console.WriteLine();
                }
            }

            public void Find(Solwrt solwrt)
            {
                SolutionFinder finder = new SolutionFinder();
                IList solutions = finder.FindSolutions(new KnightSolutionEnumerator(hatesMap, needsMap, KnigtsNumber));

                foreach (KnightSolution solution in solutions)
                {
                    solwrt.Write(solution.ToString());
                    
                }
            }

           
        }


        class KnightSolution : ISolution
        {
            public int NextKnightIndex;
            public int KnightsNumber;
            public char[] Knights;

            public KnightSolution(int knightsNumber)
            {
                this.NextKnightIndex = 0;
                this.KnightsNumber = knightsNumber;
                this.Knights = new char[knightsNumber];

                for (int i = 0; i < KnightsNumber; i++)
                {
                    Knights[i] = ' ';
                }
            }

            public ISolution Clone()
            {
                KnightSolution clone = new KnightSolution(KnightsNumber);
                clone.Knights = new char[KnightsNumber];

                for (int i = 0; i < KnightsNumber; i++)
                {
                    clone.Knights[i] = Knights[i];
                }

                clone.NextKnightIndex = NextKnightIndex;

                return clone;
            }

            public bool IsComplete
            {
                get
                {
                    return NextKnightIndex == KnightsNumber;
                }
            }

            public void FillDescendantSolutionEnumerator(ISolutionEnumerator descendantSolutionEnumerator)
            {
                KnightSolutionEnumerator enumerator = (KnightSolutionEnumerator)descendantSolutionEnumerator;
                enumerator.NextKnight = 0;
                enumerator.KnightsNumber = KnightsNumber;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < KnightsNumber; i++)
                {
                    sb.Append(Knights[i]);
                }

                sb.Append(Environment.NewLine);

                return sb.ToString();
            }
        }

        class KnightSolutionEnumerator : ISolutionEnumerator
        {
            public int KnightsNumber;
            public int NextKnight;
            private KnightSolution PartialSolution;
            private ArrayListHashtable hatesMap;
            private ArrayListHashtable needsMap;

            public KnightSolutionEnumerator(ArrayListHashtable hatesMap, ArrayListHashtable needsMap, int knightsNumber)
            {
                this.hatesMap = hatesMap;
                this.needsMap = needsMap;
                this.KnightsNumber = knightsNumber;
                this.PartialSolution = new KnightSolution(KnightsNumber);
            }

            public ISolutionEnumerator Clone()
            {
                return new KnightSolutionEnumerator(hatesMap, needsMap, KnightsNumber);
            }

            public ISolution StartingPartialSolution
            {
                get
                {
                    return PartialSolution;
                }
                set
                {
                    PartialSolution = (KnightSolution)value;
                }
            }

            public bool FillDescendantSolution(ISolution nextSolution)
            {
                KnightSolution solution = (KnightSolution)nextSolution;
                solution.NextKnightIndex = PartialSolution.NextKnightIndex + 1;

                solution.Knights = new char[KnightsNumber];

                for (int i = 0; i < KnightsNumber; i++)
                {
                    solution.Knights[i] = PartialSolution.Knights[i];
                }

                for (; NextKnight < KnightsNumber; NextKnight++)
                {
                    bool knightAlreadyPositioned = false;
                    char newKnight = (char)('a' + NextKnight);

                    for (int j = 0; j < PartialSolution.NextKnightIndex; j++)
                    {
                        // check if the knight already placed
                        if (PartialSolution.Knights[j] == newKnight)
                        {
                            knightAlreadyPositioned = true;
                        }
                    }

                    if (!knightAlreadyPositioned)
                    {
                        int index = PartialSolution.NextKnightIndex;
                        solution.Knights[PartialSolution.NextKnightIndex] = newKnight;

                        // check for previous
                        if (index > 1 && !IsConsistent(solution, index - 2, index - 1, index))
                        {
                            continue;
                        }

                        if (index == KnightsNumber - 1)
                        {
                            if (!IsConsistent(solution, index - 1, index, 0))
                            {
                                continue;
                            }
                            if (!IsConsistent(solution, index, 0, 1))
                            {
                                continue;
                            }
                        }
                        else if (!IsConsistent(solution, index - 1, index, index + 1))
                        {
                            continue;
                        }

                        NextKnight++;

                        return true;
                    }
                }

                return false;
            }

            private bool IsConsistent(KnightSolution solution, int prevIndex, int index, int nextIndex)
            {

                //			if (solution.ToString().IndexOf("acmnbhgediljfk") != -1)
                //			//if (solution.ToString().IndexOf("anmckfjlideghb") != -1)
                //			{
                //				Console.WriteLine();
                //			}

                // get neighbors
                char knight = solution.Knights[index];
                char prevNeighbor = ' ';
                char nextNeighbor = ' ';

                if (prevIndex >= 0)
                {
                    prevNeighbor = solution.Knights[prevIndex];
                }

                if (nextIndex >= 0)
                {
                    nextNeighbor = solution.Knights[nextIndex];
                }

                IList prevNeighborHatesList = hatesMap[prevNeighbor];
                IList prevNeighborNeedsList = needsMap[prevNeighbor];
                IList nextNeighborHatesList = hatesMap[nextNeighbor];
                IList nextNeighborNeedsList = needsMap[nextNeighbor];
                IList knightHatesList = hatesMap[knight];
                IList knightNeedsList = needsMap[knight];

                if ((knightHatesList != null && knightHatesList.Contains(prevNeighbor)) ||
                    (knightHatesList != null && knightHatesList.Contains(nextNeighbor)) ||
                    (prevNeighborHatesList != null && prevNeighborHatesList.Contains(knight)) ||
                    (nextNeighborHatesList != null && nextNeighborHatesList.Contains(knight)))
                {
                    return false;
                }

                if ((prevNeighbor != ' ') && nextNeighbor != ' ')
                {
                    if (knightNeedsList != null &&
                        (!knightNeedsList.Contains(prevNeighbor) && !knightNeedsList.Contains(nextNeighbor)))
                    {
                        //Console.WriteLine(prevNeighbor + ":" + knight + ":" + nextNeighbor);
                        //Console.Write(solution);
                        return false;
                    }
                }

                return true;
            }
        }


        interface ISolution
        {
            ISolution Clone();

            bool IsComplete
            {
                get;
            }

            void FillDescendantSolutionEnumerator(ISolutionEnumerator descendantSolutionEnumerator);
        }

        interface ISolutionEnumerator
        {
            ISolutionEnumerator Clone();

            ISolution StartingPartialSolution
            {
                get;
                set;
            }

            bool FillDescendantSolution(ISolution nextSolution);
        }

        class SolutionFinder
        {
            private ArrayList SolutionEnumerators = new ArrayList();
            private ArrayList Solutions = new ArrayList();

         

            public IList FindSolutions(ISolutionEnumerator enumerator)
            {
                ArrayList result = new ArrayList();

                SolutionEnumerators.Add(enumerator);
                Solutions.Add(enumerator.StartingPartialSolution.Clone());

                FindSolutionsRecursive(result, 0, enumerator);

                return result;
            }

            private ISolutionEnumerator GetSolutionEnumerator(int level)
            {
                if (SolutionEnumerators.Count <= level)
                {
                    SolutionEnumerators.Add((SolutionEnumerators[0] as ISolutionEnumerator).Clone());
                }

                return SolutionEnumerators[level] as ISolutionEnumerator;
            }

            private ISolution GetSolution(int level)
            {
                if (Solutions.Count <= level)
                {
                    Solutions.Add((Solutions[0] as ISolution).Clone());
                }

                return Solutions[level] as ISolution;
            }

            private void FindSolutionsRecursive(ArrayList result, int level, ISolutionEnumerator enumerator)
            {
                ISolution solution = GetSolution(level);

                Debug.Assert(solution != enumerator.StartingPartialSolution);

                while (enumerator.FillDescendantSolution(solution))
                {
                    //Console.Write(solution.ToString());
                    //Console.ReadLine();

                    if (solution.IsComplete)
                    {
                        //Console.Write(solution.ToString());
                        //Console.ReadLine();
                        result.Add(solution.Clone());
                    }
                    else
                    {
                        ISolutionEnumerator descendantEnumerator = GetSolutionEnumerator(level + 1);
                        solution.FillDescendantSolutionEnumerator(descendantEnumerator);
                        descendantEnumerator.StartingPartialSolution = solution;

                        FindSolutionsRecursive(result, level + 1, descendantEnumerator);
                    }
                }
            }
        }
    }

    public abstract class AbstractListHashtable : Hashtable
    {
        protected abstract IList CreateList();

        public void AddToList(object key, object val)
        {
            IList list = (IList)this[key];

            if (list == null) list = CreateList();
            list.Add(val);
            this[key] = list;

        }

        public bool ContainsValueInList(object val)
        {
            foreach (IList list in Values)
            {
                if (list.Contains(val))
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveFromList(object key, object val)
        {
            IList list = (ArrayList)this[key];

            if (list != null)
            {
                list.Remove(val);

                if (list.Count == 0)
                {
                    Remove(key);
                }
            }
        }
    }

    public class ArrayListHashtable : AbstractListHashtable
    {
        protected override IList CreateList()
        {
            return new ArrayList();
        }

        public new ArrayList this[object key]
        {
            get
            {
                return (ArrayList)base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public object this[object key, int index]
        {
            get
            {
                return this[key][index];
            }
            set
            {
                this[key][index] = value;
            }
        }
    }

}


