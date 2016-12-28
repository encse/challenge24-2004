using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cmn.Util;

namespace Ch24.Contest04.C
{
    //ez egy ezer éves megoldás még 2004-ből
    public class CTvProgrammingSolver2 : Ch24.Contest.Solver
    {
        public override void Solve()
        {
            new TVProgramming(FpatIn);
        }

        class Graph
        {
            private List<Vertex> rgvertex = new List<Vertex>();
            public IEnumerable<Vertex> Vertices
            {
                get { return rgvertex; }
            }

            public void AddEdge(Vertex v1, Vertex v2, int i)
            {
                var edge = new Edge(v1, v2, i);
                v1.Edges.Add(edge);
                v2.Edges.Add(edge);
            }

            public void AddVertex(Vertex v1)
            {
                rgvertex.Add(v1);
            }
        }

        class Vertex
        {
            public List<Edge> Edges = new List<Edge>();
            public Time Payload;

            public Vertex(Time t)
            {
                this.Payload = t;
            }
        }

        class Edge
        {
            public int Payload;
            public Vertex Source;
            public Vertex Target;

            public Edge(Vertex source, Vertex target, int payload)
            {
                Payload = payload;
                Source = source;
                Target = target;
            }
        }

        class TVProgramming : IComparer
        {
            string mFilenamebase;
            int mShowCount;
            Graph mGraph;

            Hashtable mpTimeVertex = new Hashtable();

            List<Vertex> mSortedVertexList;

            public TVProgramming(string filenamebase)
            {
                mFilenamebase = filenamebase;
                Solve();
            }

            public void Solve()
            {
                InitFromFile();
                InitSortedCollection();

                for (int i = 0; i < mSortedVertexList.Count - 1; i++)
                {
                    Vertex u = mSortedVertexList[i];
                    Vertex v = mSortedVertexList[i + 1];
                    mGraph.AddEdge(u, v, 0);
                }

                int max = -1;
                for (int i = 0; i < mSortedVertexList.Count; i++)
                {
                    int m = SelectMax(i);
                    if (m > max) max = m;
                }
                Console.WriteLine(max);
            }

            private int SelectMax(int iStartVertex)
            {
                Dictionary<Vertex, int> profitTable = new Dictionary<Vertex, int>();

                Vertex start = mSortedVertexList[iStartVertex];
                Time startTime = (start.Payload as Time);
                Time endTime = new Time(startTime.Hour + 24, startTime.Min);

                if (startTime.Hour >= 24) return 0;

                profitTable[start] = 0;
                int globalMax = -1;
                for (int i = iStartVertex + 1; i < mSortedVertexList.Count; i++)
                {
                    Vertex v = mSortedVertexList[i];
                    if (v.Payload as Time > endTime) break;
                    Debug.Assert(!profitTable.ContainsKey(v));

                    int localMax = -1;
                    foreach (Edge e in v.Edges)
                    {
                        if (e.Target == v)
                        {
                            int indexInTable = mSortedVertexList.IndexOf(e.Source);
                            if (indexInTable >= iStartVertex)
                            {
                                int profit = (int)e.Payload + profitTable[e.Source];
                                if (profit > localMax) localMax = profit;
                            }
                        }
                    }
                    profitTable.Add(v, localMax);

                    if (localMax > globalMax) globalMax = localMax;
                }
                return globalMax;
            }

            private void InitSortedCollection()
            {
                ArrayList rgVertices = new ArrayList();
                foreach (Vertex v in mGraph.Vertices)
                    rgVertices.Add(v);

                rgVertices.Sort(this);
                mSortedVertexList = new List<Vertex>();
                foreach (Vertex v in rgVertices)
                {
                    mSortedVertexList.Add(v);
                }
            }

            private void InitFromFile()
            {
                Pparser pparser = new Pparser(mFilenamebase);
                mShowCount = pparser.Fetch<int>();
                mGraph = new Graph();
                for (int i = 0; i < mShowCount; i++)
                {
                    var line = pparser.Fetch<string>();
                    Time t1 = Time.Parse(line.Substring(0, 5));
                    Time t2 = Time.Parse(line.Substring(6, 5));

                    if (t2 < t1) t2.Hour += 24;

                    line = line.Substring(12);
                    int profit = int.Parse(line);

                    Vertex v1 = mpTimeVertex[t1] as Vertex;
                    if (v1 == null)
                    {
                        v1 = new Vertex(t1);
                        mpTimeVertex[t1] = v1;
                        mGraph.AddVertex(v1);
                    }

                    Vertex v2 = mpTimeVertex[t2] as Vertex;
                    if (v2 == null)
                    {
                        v2 = new Vertex(t2);
                        mpTimeVertex[t2] = v2;
                        mGraph.AddVertex(v2);
                    }

                    Debug.Assert(v1 != v2);
                    mGraph.AddEdge(v1, v2, profit);
                }
            }

            public int Compare(object x, object y)
            {
                return ((x as Vertex).Payload as Time).CompareTo((y as Vertex).Payload);
            }

        }

        class Time : IComparable, IComparer
        {
            public int Hour;
            public int Min;

            public Time(int hour, int min)
            {
                Hour = hour;
                Min = min;
            }

            public static Time Parse(string time)
            {
                int hour = Int32.Parse(time.Substring(0, 2));
                int min = Int32.Parse(time.Substring(3, 2));

                return new Time(hour, min);
            }

            public override string ToString()
            {
                return Hour + ":" + Min;
            }

            public override bool Equals(object obj)
            {
                Time t = obj as Time;
                return this.Hour == t.Hour && this.Min == t.Min;
            }

            public override int GetHashCode()
            {
                return Hour * 60 + Min;
            }

            public static bool operator ==(Time t1, Time t2)
            {
                return t1.Equals(t2);
            }

            public static bool operator !=(Time t1, Time t2)
            {
                return !t1.Equals(t2);
            }
            public static bool operator <(Time t1, Time t2)
            {
                return t1.Hour < t2.Hour || (t1.Hour == t2.Hour && t1.Min < t2.Min);
            }
            public static bool operator >(Time t1, Time t2)
            {
                return t1.Hour > t2.Hour || (t1.Hour == t2.Hour && t1.Min > t2.Min);
            }
            public static bool operator <=(Time t1, Time t2)
            {
                return t1.Hour < t2.Hour || (t1.Hour == t2.Hour && t1.Min <= t2.Min);
            }
            public static bool operator >=(Time t1, Time t2)
            {
                return t1.Hour > t2.Hour || (t1.Hour == t2.Hour && t1.Min >= t2.Min);
            }
            #region IComparable Members

            public int CompareTo(object obj)
            {
                return Compare(this, obj);
            }

            #endregion

            #region IComparer Members

            public int Compare(object x, object y)
            {
                if ((x as Time) > (y as Time)) return 1;
                if ((x as Time) < (y as Time)) return -1;
                return 0;
            }

            #endregion
        }
    }


}
