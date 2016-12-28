using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2015.R2.A
{
    class ASolver : GcjSolver
    {
        protected override IEnumerable<object> EnobjSolveCase()
        {
            int crow, ccol;
            Fetch(out crow, out ccol);
            var m = new char[ccol, crow];
            for(int y = 0; y < crow; y++)
            {
                var line = Fetch<string>();
                Console.WriteLine(line);
                for(int x = 0; x < line.Length; x++)
                {
                    m[x, y] = line[x];
                }
            }

            var cAll =0;
            var rgcByCol = new int[ccol];
            var rgcByRow = new int[crow];
            foreach(var vxy in m.Envxy())
            {
                if(vxy.v == '.')
                    continue;
                cAll++;
                rgcByCol[vxy.x]++;
                rgcByRow[vxy.y]++;
            }

            if(cAll == 0)
            {
                Console.WriteLine(0);
                yield return 0;
                yield break;
            }

            if(ccol.Eni().SelectMany(col => crow.Eni().Select(row => new {col,row})).Any(x => m[x.col,x.row] != '.' &&  rgcByCol[x.col] == 1 && rgcByRow[x.row] == 1))
            {
                Console.WriteLine("IMPOSSIBLE");
                yield return "IMPOSSIBLE";
                yield break;
            }

            var cChange = 0;
            for(var x=0;x<ccol;x++)
            {
                if(rgcByCol[x] == 0)
                    continue;
                var chFirst = crow.Eni().Select(y => m[x, y]).First(ch => ch != '.');
                if(chFirst == '^')
                    cChange++;

                var chLast = crow.Eni().Select(y => m[x, y]).Last(ch => ch != '.');
                if (chLast == 'v')
                    cChange++;
            }

            for (var y = 0; y < crow; y++)
            {
                if (rgcByRow[y] == 0)
                    continue;
                var chFirst = ccol.Eni().Select(x => m[x, y]).First(ch => ch != '.');
                if (chFirst == '<')
                    cChange++;

                var chLast = ccol.Eni().Select(x => m[x, y]).Last(ch => ch != '.');
                if (chLast == '>')
                    cChange++;
            }

            Console.WriteLine(cChange);
            yield return cChange;
        }

    }
}
