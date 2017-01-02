using System;
using System.Collections.Generic;
using System.Linq;

namespace Wecomp.Util
{
    public class SuffNode
    {
        public readonly Dictionary<char, SuffEdge> MpEdgeByCh = new Dictionary<char, SuffEdge>();
        public SuffNode NodeSuffix;

        public void AddEdge(int ichFirst, int ichLast, SuffNode node, char ch)
        {
            MpEdgeByCh[ch] = new SuffEdge(ichFirst,ichLast, node);
        }

        public void SetSuffixLink(SuffNode suffix)
        {
            NodeSuffix = suffix;
        }

        public string TSTO(SuffTree tree)
        {
            var st="";
            foreach(var visuffg in MpEdgeByCh.Values.Select((v,i)=>new{v,i}))
            {
                if(visuffg.i==0)
                {
                    var pre = "#--" + tree.StFromSuffG(visuffg.v) + "-->";
                    st += pre;
                    st += (visuffg.v.Node.TSTO(tree)+Environment.NewLine).Replace(Environment.NewLine, Environment.NewLine + "|" + new string(' ', pre.Length - 1));
                }
                else if(visuffg.i < MpEdgeByCh.Count - 1)
                {
                    st += Environment.NewLine;
                    var pre = "+--" + tree.StFromSuffG(visuffg.v) + "-->";
                    st += pre;
                    st += (visuffg.v.Node.TSTO(tree)+Environment.NewLine).Replace(Environment.NewLine, Environment.NewLine + "|" + new string(' ', pre.Length - 1));
                }
                else 
                {
                    st += Environment.NewLine;
                    var pre = "+--" + tree.StFromSuffG(visuffg.v) + "-->";
                    st += pre;
                    st += visuffg.v.Node.TSTO(tree).Replace(Environment.NewLine, Environment.NewLine + " " + new string(' ', pre.Length - 1));
                }
            }
            return st;
        }

        public int CRepeat()
        {
            return MpEdgeByCh.Values
                .Where(edge => edge.Node.MpEdgeByCh.Any())
                .Select(edge => edge.IchLast - edge.IchFirst + 1 + edge.Node.CRepeat())
                .Concat(new[] {0})
                .Max();
        }
    }

    public class SuffEdge
    {
        public readonly int IchFirst;
        public int IchLast;
        public SuffNode Node;

        public SuffEdge(int ichFirst, int ichLast, SuffNode node)
        {
            IchFirst = ichFirst;
            IchLast = ichLast;
            Node = node;
        }
    }

    public class SuffTree
    {
        private readonly List<char> rgv;
        public readonly SuffNode Root;
        private readonly SuffNode node0;

        public string StFromSuffG(SuffEdge suffEdge)
        {
            var st = "";
            for(var i = suffEdge.IchFirst; i <= suffEdge.IchLast ; i++)
            {
                if(i-1==rgv.Count)
                    break;
                var ch = rgv[i - 1];
                st += ch;
            }
            return st;
        }

        public SuffTree(List<char> rgabc, List<char> rgv)
        {
            this.rgv = rgv;

            Root = new SuffNode();
            node0 = new SuffNode();
            for(var j = 1; j <= rgabc.Count; j++)
                node0.AddEdge(-j, -j, Root, rgabc[j - 1]);
            Root.SetSuffixLink(node0);

            var node = Root;
            var ich = 1;
            var ichNext = 0;
            while(ichNext != rgv.Count)
            {
                ichNext++;
                Update(ref node, ref ich, ichNext);
                Canonize(node, ich, ichNext, out node, out ich);

                //Console.WriteLine(t(i));
                //Console.WriteLine(root.TSTO(this));
                //Console.ReadLine();
            }
        }

        private void Update(ref SuffNode node, ref int ich, int ichNext)
        {
            var nodePrev = Root;
            bool fEnd;
            SuffNode nodeNext;
            TestAndsplit(node, ich, ichNext - 1, rgv[ichNext - 1], out fEnd, out nodeNext);
            while(!fEnd)
            {
                nodeNext.AddEdge(ichNext,rgv.Count,new SuffNode(), rgv[ichNext - 1]);
                if(nodePrev != Root)
                    nodePrev.SetSuffixLink(nodeNext);
                nodePrev = nodeNext;
                Canonize(node.NodeSuffix, ich, ichNext - 1, out node, out ich);
                TestAndsplit(node, ich, ichNext - 1, rgv[ichNext - 1], out fEnd, out nodeNext);
            }
            if(nodePrev != Root)
                nodePrev.SetSuffixLink(node);
        }

        private void Canonize(SuffNode node, int ichFirst, int ichLast, out SuffNode nodeNew, out int ichFirstNew)
        {
            if(ichLast<ichFirst)
            {
                nodeNew = node;
                ichFirstNew = ichFirst;
                return;
            }
            
            var edge = node.MpEdgeByCh[rgv[ichFirst - 1]];
            while(edge.IchLast - edge.IchFirst <= ichLast - ichFirst)
            {
                ichFirst = ichFirst + edge.IchLast - edge.IchFirst + 1;
                node = edge.Node;
                if(ichFirst <= ichLast)
                    edge = node.MpEdgeByCh[rgv[ichFirst - 1]];
            }

            nodeNew = node;
            ichFirstNew = ichFirst;
        }

        private void TestAndsplit(SuffNode node, int ichFirst, int ichLast, char ch, out bool fEnd, out SuffNode nodeNext)
        {
            if(ichFirst<=ichLast)
            {
                var edge = node.MpEdgeByCh[rgv[ichFirst - 1]];
                if(ch == rgv[edge.IchFirst + ichLast - ichFirst])
                {
                    fEnd = true;
                    nodeNext = node;
                    return;
                }
                
                nodeNext = new SuffNode();
                nodeNext.AddEdge(edge.IchFirst + ichLast - ichFirst + 1, edge.IchLast, edge.Node, rgv[edge.IchFirst + ichLast - ichFirst]);
                edge.IchLast = edge.IchFirst + ichLast - ichFirst;
                edge.Node = nodeNext;

                fEnd = false;
                return;
            }
            
            if(!node.MpEdgeByCh.ContainsKey(ch))
            {
                fEnd = false;
                nodeNext = node;
                return;
            }
            
            fEnd = true;
            nodeNext = node;
        }
    }
}
