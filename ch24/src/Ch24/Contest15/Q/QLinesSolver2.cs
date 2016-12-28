using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest15.Q
{
    class QLinesSolver2 : Solver
    {

        private class Line
        {
            public Pt ptStart;
            public Pt ptEnd;
            List<Pt> rgpt= new List<Pt>(); 

            public Line(Pt ptStart, Pt ptEnd)
            {
                this.ptStart = ptStart;
                this.ptEnd = ptEnd;
            }

            public bool CanJoin(Line lineB)
            {
                Pt[] rgptA;
                Pt[] rgptB;
                if (this == lineB)
                    return false;
                //if (lineB.ptStart.Equals(this.ptEnd))
                //{
                    rgptA = line(ptStart, ptEnd).Concat(line(lineB.ptStart, lineB.ptEnd)).ToArray();
                    rgptB = line(ptStart, lineB.ptEnd).ToArray();
                //}
                //else
                //{
                 //   return false;
                //}

                if (rgptA.Length != rgptB.Length)
                    return false;

                for(var i=0;i<rgptA.Length;i++)
                    if (!rgptA[i].Equals(rgptB[i]))
                        return false;
                return true;
            }

            public IEnumerable<Pt> line(Pt ptStart, Pt ptEnd)
            {
                int x = ptStart.x;
                int y = ptStart.y;
                int x2 = ptEnd.x;
                int y2 = ptEnd.y;

                int w = x2 - x;
                int h = y2 - y;
                int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
                if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
                if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
                if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
                int longest = Math.Abs(w);
                int shortest = Math.Abs(h);
                if (!(longest > shortest))
                {
                    longest = Math.Abs(h);
                    shortest = Math.Abs(w);
                    if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                    dx2 = 0;
                }
                int numerator = longest >> 1;
                for (int i = 0; i <= longest; i++)
                {
                    yield return new Pt(x, y);
                    numerator += shortest;
                    if (!(numerator < longest))
                    {
                        numerator -= longest;
                        x += dx1;
                        y += dy1;
                    }
                    else
                    {
                        x += dx2;
                        y += dy2;
                    }
                }
            }

            public int Length2()
            {
                var dx = ptStart.x - ptEnd.x;
                var dy = ptStart.y - ptEnd.y;
                return dx*dx + dy*dy;
            }
        }

        private class Pt
        {
            public int x;
            public int y;

            public Pt(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            protected bool Equals(Pt other)
            {
                return x == other.x && y == other.y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Pt) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (x*397) ^ y;
                }
            }
        }


        public override void Solve()
        {
            var img = Pngr.Load(FpatIn, px => px.rgba.r);

            var linesByPtStart = new Dictionary<Pt, HashSet<Line>>();
            var linesByPtEnd = new Dictionary<Pt, HashSet<Line>>();

            var width = img.GetLength(0);
            var height = img.GetLength(1);

            var lines = new List<Line>();
            var linesDeleted = new HashSet<Line>();
            for (var x = 0; x <width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (img[x, y] == 0)
                    {
                        var ptStart = new Pt(x, y);
                        var ptEnd = new Pt(x, y);
                        var line = new Line(ptStart, ptEnd);

                        if(!linesByPtStart.ContainsKey(ptStart))
                            linesByPtStart[ptStart] = new HashSet<Line>();

                        if (!linesByPtEnd.ContainsKey(ptEnd))
                            linesByPtEnd[ptEnd] = new HashSet<Line>();

                        linesByPtStart[ptStart].Add(line);
                        linesByPtEnd[ptEnd].Add(line);
                        lines.Add(line);
                    }
                }
            }

           // lines = Permute(lines);

            var dtEnd = DateTime.Now + TimeSpan.FromMinutes(1);
            var fAny = true;
            while (fAny && DateTime.Now < dtEnd)
            {
                fAny = false;
               
                for(var i=0;i<lines.Count; )
                {
                    var line = lines[i];
                    if (linesDeleted.Contains(line))
                    {
                        i++;
                        continue;
                    }
                    var ptEnd = line.ptEnd;

                    var dBest = -1;
                    Line longest = null;
                    foreach (var lineB in GetNeighbours(ptEnd, linesByPtStart, width, height))
                    {
                        if (linesDeleted.Contains(lineB))
                               continue;
                        if (line.CanJoin(lineB))
                        {
                            var d = lineB.Length2();
                            if (dBest < d)
                            {
                                dBest = d;
                                longest = lineB;
                            }
                        }
                    }
                   
                    foreach (var lineB in GetNeighbours(ptEnd, linesByPtEnd, width, height))
                    {
                        if (linesDeleted.Contains(lineB))
                            continue;
                        if (line.CanJoin(new Line(lineB.ptEnd, lineB.ptStart)))
                        {
                            var d = lineB.Length2();
                            if (dBest < d)
                            {
                                dBest = d;
                                longest = lineB;
                            }
                        }
                    }

                    if (longest != null)
                    {
                        fAny = true;
                        line.ptEnd = IsNeighbour(longest.ptStart, line.ptEnd) ? longest.ptEnd : longest.ptStart;
                        linesDeleted.Add(longest);
                  //      Console.WriteLine(lines.Count - linesDeleted.Count);
                    
                    }
                    else
                        i++;
                }
            }
            lines = lines.Where(line => !linesDeleted.Contains(line)).ToList();
            Console.WriteLine(lines.Count);

            Score = lines.Count;
            using (Output)
            {
                foreach (var line in lines)
                {
                    WriteLine("{0} {1} {2} {3}", line.ptEnd.x, line.ptEnd.y, line.ptStart.x, line.ptStart.y);
                }
            }
        }

        private bool IsNeighbour(Pt ptStart, Pt ptEnd)
        {
            return Math.Abs(ptStart.x - ptEnd.x) + Math.Abs(ptStart.y - ptEnd.y) <= 2;
        }

        Random r = new Random();
        private List<Line> Permute(List<Line> lines)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                var j = r.Next(lines.Count);
                var lineA = lines[i];
                lines[i] = lines[j];
                lines[j] = lineA;
            }

            return lines;
        }

        private IEnumerable<Line> GetNeighbours(Pt pt, Dictionary<Pt, HashSet<Line>> lines, int width, int height)
        {
         

            if (pt.x > 0     && true    )      foreach (var line in Q(pt.x - 1, pt.y    , lines)) yield return line;
            if (true         && true    )      foreach (var line in Q(pt.x   ,  pt.y    , lines)) yield return line;
            if (pt.x < width && true    )      foreach (var line in Q(pt.x + 1, pt.y    , lines)) yield return line;

            if (pt.x > 0     && pt.y < height) foreach (var line in Q(pt.x - 1, pt.y + 1, lines)) yield return line;
            if (true         && pt.y < height) foreach (var line in Q(pt.x    , pt.y + 1, lines)) yield return line;
            if (pt.x < width && pt.y < height) foreach (var line in Q(pt.x + 1, pt.y + 1, lines)) yield return line;


            if (pt.x > 0 && pt.y > 0) foreach (var line in Q(pt.x - 1, pt.y - 1, lines)) yield return line;
            if (true && pt.y > 0) foreach (var line in Q(pt.x, pt.y - 1, lines)) yield return line;
            if (pt.x < width && pt.y > 0) foreach (var line in Q(pt.x + 1, pt.y - 1, lines)) yield return line;

        }

        private IEnumerable<Line> Q(int x, int y, Dictionary<Pt, HashSet<Line>> lines)
        {
            var pt = new Pt(x, y);
            if(!lines.ContainsKey(pt))
                yield break;
            foreach (var line in lines[pt])
                yield return line;
        }
    }
}
