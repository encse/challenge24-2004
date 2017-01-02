using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Util
{
    /// <summary>
    /// Iterative deepening depth first search
    /// </summary>
    public class Iddf<T>
    {
        private readonly T tStart;
        private readonly Func<T, List<T>> dgentNextByT;
        private readonly Func<Stack<T>, List<T>> dgentNextByPath;
        private readonly Func<Hlm_Chewbacca<T>, T, List<T>> dgentNextByVisited;
        private readonly Func<T, bool> dgfGoal;

        public Iddf(T tStart, Func<T, List<T>> dgentNext, Func<T, bool> dgfGoal)
        {
            this.tStart = tStart;
            this.dgentNextByT = dgentNext;
            this.dgfGoal = dgfGoal;
        }

        public Iddf(T tStart, Func<Stack<T>, List<T>> dgentNext, Func<T, bool> dgfGoal)
        {
            this.tStart = tStart;
            this.dgentNextByPath = dgentNext;
            this.dgfGoal = dgfGoal;
        }

        public Iddf(T tStart, Func<Hlm_Chewbacca<T>, T, List<T>> dgentNext, Func<T, bool> dgfGoal)
        {
            this.tStart = tStart;
            this.dgentNextByVisited = dgentNext;
            this.dgfGoal = dgfGoal;
        }
        public IEnumerable<IEnumerable<T>> EnpathFind()
        {
            try
            {
                var depthMin = Bsrc.Find(0, depth => Find(tStart, depth).Any());
                return Find(tStart, depthMin);
            }
            catch (Nosol)
            {
                return Enumerable.Empty<Stack<T>>();
            }
        }

        public IEnumerable<T> Find()
        {
            return EnpathFind().Select(rgtPath => ((Stack<T>)rgtPath).Peek());
        }

        class Depthmax
        {
            public int D;
            public int ccut;

            public Depthmax(int d)
            {
                D = d;
            }
        }

        private class Nosol : Exception
        {
        }

        private IEnumerable<Stack<T>> Find(T tStart, int depthMax)
        {
            var depthmax = new Depthmax(depthMax);
            bool fAny = false;
            var rgtPathStart = new Stack<T>();
            rgtPathStart.Push(tStart);
            var ohlmtVisited = dgentNextByVisited != null ? new Hlm_Chewbacca<T>() : null;
            foreach (var rgtPath in FindPathRecursive(rgtPathStart, ohlmtVisited, 0, depthmax))
            {
                fAny = true;
                yield return rgtPath;
            }

            if (!fAny && depthmax.ccut == 0)
                throw new Nosol();
        }

        private IEnumerable<Stack<T>> FindPathRecursive(Stack<T> rgt,  Hlm_Chewbacca<T>ohlmtVisited, int depth, Depthmax depthMax)
        {
            var t = rgt.Peek();

            if (dgfGoal(t))
            {
                depthMax.D = depth;
                yield return new Stack<T>(rgt);
                yield break;
            }

            if (depth == depthMax.D)
            {
                depthMax.ccut++;
                yield break;
            }

            foreach (var tNext in EntNext(rgt, ohlmtVisited))
            {
                if (ohlmtVisited != null)
                {
                    if(ohlmtVisited.Contains(tNext))
                        continue;

                    ohlmtVisited.Add(tNext);
                }
                
                rgt.Push(tNext);
                
                foreach (var tSolution in FindPathRecursive(rgt, ohlmtVisited, depth + 1, depthMax))
                    yield return tSolution;

                if (ohlmtVisited != null)
                    ohlmtVisited.Remove(tNext);
                rgt.Pop();
            }
        }

        private IEnumerable<T> EntNext(Stack<T> rgt, Hlm_Chewbacca<T>ohlmtVisited)
        {
            if (dgentNextByPath != null)
                return dgentNextByPath(rgt);
            if (dgentNextByT != null)
                return dgentNextByT(rgt.Peek());
            return dgentNextByVisited(ohlmtVisited, rgt.Peek());
        }
    }
}