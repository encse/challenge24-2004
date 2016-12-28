using System;
using System.Collections.Generic;
using System.Linq;
using Gcj.Util;

namespace Gcj.Y2010.R1B.A
{
    internal class FileFixItSolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            var rgcoldcnew = Fetch<int[]>();
            var cold = rgcoldcnew[0];
            var cnew = rgcoldcnew[1];
            var hlmdir = new HashSet<string>{""};

            var cmkdir = 0;
            Action<string> add=null;
            add = dir =>
            {
                if(hlmdir.Contains(dir))
                    return;
                var dirParent = dir.Substring(0, dir.LastIndexOf('/'));
                add(dirParent);
                cmkdir++;
                hlmdir.Add(dir);
            };

            for(var idir = 0; idir < cold; idir++)
                add(Fetch<string>());
            cmkdir = 0;
            for(var idir = 0; idir < cnew; idir++)
                add(Fetch<string>());
            yield return cmkdir;
        }
    }
}
