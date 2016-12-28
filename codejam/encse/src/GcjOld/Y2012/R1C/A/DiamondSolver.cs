using System.Linq;
using System.Collections.Generic;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2012.R1C.A
{
    public class DiamondSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            /*
             The first line of each test case gives the number of classes in this diagram, N. 
             * The classes are numbered from 1 to N. N lines follow. 
             * The ith line starts with a non-negative integer Mi indicating the number of classes that class i inherits from. 
             * This is followed by Mi distinct positive integers each from 1 to N representing those classes. 
             * You may assume that:
                  - If there is an inheritance path from X to Y then there is no inheritance path from Y to X.
                  - A class will never inherit from itself.
             */
            var ccls = pparser.Fetch<int>();
            var rgcls = new Cls[ccls];
            for (int id = 0; id < ccls; id++)
                rgcls[id] = new Cls(id);

            for (int id = 0; id < ccls; id++)
                rgcls[id].RgclsInheritsFrom = pparser.Fetch<int[]>().Skip(1).Select(x => rgcls[x - 1]).ToArray();
            return () => EnobjSolveCase(rgcls);
        }

        private IEnumerable<object> EnobjSolveCase(Cls[] rgcls)
        {
            var hlmclsToVisit = rgcls.ToDictionary(cls => cls, cls => true);
              
            while(hlmclsToVisit.Count>0)
            {
                var hlmclsSeen = new Dictionary<Cls, bool>();

                var quecls = new Queue<Cls>();
                quecls.Enqueue(hlmclsToVisit.Keys.First());
                
                while(quecls.Any())
                {
                    var cls = quecls.Dequeue();
                    if(hlmclsSeen.ContainsKey(cls))
                    {
                        yield return "Yes";
                        yield break;
                    }
                    hlmclsSeen.Add(cls, true);

                    foreach (var clsNext in cls.RgclsInheritsFrom)
                        quecls.Enqueue(clsNext);
                }

                foreach (var clsSeen in hlmclsSeen.Keys)
                    hlmclsToVisit.Remove(clsSeen);
            }

            yield return "No";
            yield break;

        }


        class Cls
        {
            public readonly int Id;
            public Cls[] RgclsInheritsFrom;
            public Cls(int id)
            {
                
            }
        }
      


     
    }
}
