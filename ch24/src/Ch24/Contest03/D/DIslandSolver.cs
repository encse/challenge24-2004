using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ch24.Contest;
using Cmn.Util;
using QuickGraph;

namespace Ch24.Contest03.D
{
    class DIslandSolver : Solver
    {
        public override void Solve()
        {
           var pparser = new Pparser(FpatIn);
             var g = new Graph();
            var cvertex = pparser.Fetch<int>();
            var rgvertex = new Vertex[cvertex];

            for(int ivertex = 0; ivertex<cvertex;ivertex++)
            {
                double x, y;
                pparser.Fetch(out x, out y);
                var vertex = new Vertex(x, y, ivertex);
                rgvertex[ivertex] = vertex;
                g.AddVertex(vertex);
            }
            
            var cedge = pparser.Fetch<int>();

            for (int iedge = 0; iedge < cedge; iedge++)
            {
                int ivertexFrom, ivertexTo;
                pparser.Fetch(out ivertexFrom, out ivertexTo);
                g.AddEdge(new Edge<Vertex>(rgvertex[ivertexFrom], rgvertex[ivertexTo]));
            }

            var bitmap = new Bitmap(1024, 1024);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Blue, new Rectangle(0, 0, 1024, 1024));
           
            foreach (var country in EncountryGet(g))
                country.DrawCountry(graphics);
            bitmap.Save(FpatOut);

        }

        IEnumerable<Country> EncountryGet(Graph g)
        {
            var rgcountry = new List<Country>();
            foreach (var edge in g.Edges)
            {
                foreach (var vertex in new[] {edge.Source, edge.Target})
                {
                    var country = CountryFromEdges(vertex, edge, g);
                    //többször is kijöhet ugyanat az ország
                    if (!rgcountry.Contains(country))
                        rgcountry.Add(country);
                }
            }
           
            //sajnos van +1 ország, ami valójában nem ország, hanem az egész térkép befoglaló poligonja. 
            //Na azt ki kell hagyni, az egyszerűség kedvéért a területe alapján
            var areaMax = rgcountry.Max(country => country.Area);
            return rgcountry.Where(country => country.Area != areaMax);
        }

        private Country CountryFromEdges(Vertex vertexStart, Edge<Vertex> edge, Graph g)
        {
            var rgvertexCountry = new List<Vertex>();

            var vertexSrc = vertexStart;
            rgvertexCountry.Add(vertexSrc);
            while (true)
            {
                var vertexDst = edge.GetOtherVertex(vertexSrc);
                if (vertexDst == vertexStart)
                    break;

                rgvertexCountry.Add(vertexDst);

                var vct1 = vertexDst.Vct - vertexSrc.Vct;
                var angleMax = double.MinValue;
                Edge<Vertex> edgeNext = null;
                foreach (var edgeT in g.AdjacentEdges(vertexDst))
                {
                    if(edgeT == edge)
                        continue;
                    
                    var vertexOther = edgeT.GetOtherVertex(vertexDst);
                    var vct2 = vertexOther.Vct - vertexDst.Vct;

                    var angle1 = vct1.Angle();
                    if (angle1 < 0) angle1 += 360; 

                    var angle2 = vct2.Angle();
                    if (angle2 < 0) angle2 += 360; 

                    if (angle2 < angle1)
                        angle2 += 360;

                    var angle = angle2 - angle1;

                    if (angle > 180)
                        angle -= 360; 
                   
                    if (angle > angleMax)
                    {
                        angleMax = angle;
                        edgeNext = edgeT;
                    }
                }
                edge = edgeNext;
                vertexSrc = vertexDst;
            }
            return new Country(rgvertexCountry.ToArray());
        }

        private class Graph : UndirectedGraph<Vertex, Edge<Vertex>>
        {
        }

        private class Vertex
        {
            public Vector Vct;
            public readonly int Ivertex;

            public Vertex(double x, double y, int ivertex)
            {
                this.Vct = new Vector(x, y);
                this.Ivertex = ivertex;
            }
        }

        private class Country
        {
            public readonly Vertex[] Rgvertex;
            public readonly double Area;

            public Country(Vertex[] rgvertex)
            {
                Rgvertex = RgvertexNormalizeOrder(rgvertex);
                Area = AreaCompute(rgvertex);
            }

            private static double AreaCompute(Vertex[] rgvertex)
            {
                double area = 0;
                for (var i = 0; i < rgvertex.Length; i += 1)
                {
                    var vctA = rgvertex[i].Vct;
                    var vctB = rgvertex[(i + 1) % rgvertex.Length].Vct;
                    area += vctA.X * vctB.Y - vctA.Y * vctB.X;
                }
                return Math.Abs(area / 2);
            }

            private static Vertex[] RgvertexNormalizeOrder(Vertex[] rgvertex)
            {
                var cRotate = -1;
                var vertexMin = Int32.MaxValue;
                for (int i = 0; i < rgvertex.Length; i++)
                {
                    if (rgvertex[i].Ivertex < vertexMin)
                    {
                        vertexMin = rgvertex[i].Ivertex;
                        cRotate = i;
                    }
                }

                var qvertex = new Queue<Vertex>(rgvertex);
                for (int i = 0; i < cRotate; i++)
                    qvertex.Enqueue(qvertex.Dequeue());
                return qvertex.ToArray();
            }

            public void DrawCountry(Graphics graphics)
            {
                var rgpointf = Rgvertex.Select(vertex => new PointF((float) vertex.Vct.X, (float) vertex.Vct.Y)).ToArray();
                graphics.FillPolygon(new SolidBrush(ColorGet()), rgpointf);
                graphics.DrawPolygon(Pens.Black, rgpointf);
            }

            private Color ColorGet()
            {
                Color c;
                if (Area < 500)
                    c = Color.FromArgb(255, 128, 0);
                else if (Area < 1000)
                    c = Color.FromArgb(255, 0, 0);
                else if (Area <= 20000)
                {
                    switch (Rgvertex.Length)
                    {
                        case 3: c = Color.FromArgb(0, 255, 0); break;
                        case 4: c = Color.FromArgb(0, 255, 255); break;
                        case 5: c = Color.FromArgb(128, 0, 0); break;
                        case 6: c = Color.FromArgb(192, 192, 192); break;
                        default: c = Color.FromArgb(255, 0, 255); break;
                    }
                }
                else
                    c = Color.FromArgb(255, 255, 0);
                return c;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Country)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return ((Country)obj).Rgvertex.SequenceEqual(Rgvertex);
            }

            public override int GetHashCode()
            {
                int hash = 0;
                hash = (hash * 17) + Rgvertex.Length;
                foreach (var vertex in Rgvertex)
                {
                    hash *= 17;
                    if (vertex != null) hash = hash + vertex.GetHashCode();
                }
                return hash;
            }

        }

    }
}
