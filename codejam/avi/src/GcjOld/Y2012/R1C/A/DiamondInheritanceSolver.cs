using System.Collections.Generic;
using System.Diagnostics;
using Gcj.Util;
using System.Linq;

namespace Gcj.Y2012.R1C.A
{
    internal class DiamondInheritanceSolver : GcjSolver
    {
        private class Cls
        {
            public int id;
            public HashSet<Cls> hlmParent=new HashSet<Cls>(); 
            public HashSet<Cls> hlmChild=new HashSet<Cls>();

            public override string ToString()
            {
                return string.Format("Id: {0}", id);
            }
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            var rgcls = new byte[Fetch<int>()].Select((_,i) => new Cls{id=i+1}).ToArray();
            var rgrgiclsParent = rgcls.Select(_ => Fetch<int[]>()).ToArray();

            for(var icls = 0;icls<rgcls.Length;icls++)
            {
                var cls = rgcls[icls];
                Debug.Assert(!cls.hlmParent.Any());
                cls.hlmParent.UnionWith(rgrgiclsParent[icls].Skip(1).Select(iclsT => iclsT - 1).Select(iclsParent => rgcls[iclsParent]));
                foreach(var clsParent in cls.hlmParent.ToArray())
                {
                    if(!FMerge(clsParent.hlmParent, cls.hlmParent))
                        continue;
                    
                    yield return "Yes";
                    yield break;
                }

                foreach(var clsParent in cls.hlmParent)
                {
                    if(!(!clsParent.hlmChild.Add(cls) | FMerge(cls.hlmChild, clsParent.hlmChild)))
                        continue;
                    
                    yield return "Yes";
                    yield break;
                }

                foreach(var clsChild in cls.hlmChild)
                {
                    if(!FMerge(cls.hlmParent, clsChild.hlmParent))
                        continue;
                    
                    yield return "Yes";
                    yield break;
                }
            }
            yield return "No";
        }

        private static bool FMerge(HashSet<Cls> hlmFrom, HashSet<Cls> hlmTo)
        {
            var sizeAll = hlmTo.Count + hlmFrom.Count;
            hlmTo.UnionWith(hlmFrom);
            return hlmTo.Count < sizeAll;
        }
    }
}
