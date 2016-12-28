using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cmn.Util
{
    public class IntRangeParser
    {
        public static IEnumerable<int> Parse(String s, int firstPage, int lastPage)
        {
            var parts = s.Split(' ', ';', ',');
            var reRange = new Regex(@"^\s*((?<from>\d+)|(?<from>\d+)(?<sep>(-|\.\.))(?<to>\d+)|(?<sep>(-|\.\.))(?<to>\d+)|(?<from>\d+)(?<sep>(-|\.\.)))\s*$");
            foreach (var part in parts)
            {
                var maRange = reRange.Match(part);
                if (maRange.Success)
                {
                    var gFrom = maRange.Groups["from"];
                    var gTo = maRange.Groups["to"];
                    var gSep = maRange.Groups["sep"];

                    if (gSep.Success)
                    {
                        var from = firstPage;
                        var to = lastPage;
                        if (gFrom.Success)
                            from = int.Parse(gFrom.Value);
                        if (gTo.Success)
                            to = int.Parse(gTo.Value);
                        for (int page = from; page <= to; page++)
                            yield return page;
                    }
                    else
                        yield return int.Parse(gFrom.Value);
                }
            }
        }
    }
}
