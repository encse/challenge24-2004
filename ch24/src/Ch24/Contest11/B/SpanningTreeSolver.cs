using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest11.B
{
    public class SpanningTreeSolver : Contest.Solver
    {
        private class Node
        {
            public List<LinkedListNode<Node>> rglnode=new List<LinkedListNode<Node>>();
            public List<string> rgst = new List<string>();
        }

        public override void Solve()
        {
            var rgT = Fetch<int[]>();
            var cLine = rgT[0];
            var cNode = rgT[1];

            var rgstTree = new List<string>();
            var mprgilineByStTree = new Dictionary<string, List<int>>();

            for(var iLine = 0; iLine < cLine; iLine++)
            {
                var rgend = Fetch<int[]>();

                var rgnode = new LinkedList<Node>();
                var rgnodeLoad = new LinkedListNode<Node>[cNode];
                for(var i = 0; i < cNode; i++)
                {
                    var lnode = new LinkedListNode<Node>(new Node());
                    rgnode.AddLast(lnode);
                    rgnodeLoad[i] = lnode;
                }


                for(var i = 0; i < rgend.Length; i += 2)
                {
                    var inode1 = rgend[i];
                    var inode2 = rgend[i + 1];

                    var lnode1 = rgnodeLoad[inode1];
                    var lnode2 = rgnodeLoad[inode2];

                    lnode1.Value.rglnode.Add(lnode2);
                    lnode2.Value.rglnode.Add(lnode1);
                }

                var rgleaf = new LinkedList<Node>();

                for(var lnode = rgnode.First; lnode != null;)
                {
                    var lnodeNext = lnode.Next;
                    if(lnode.Value.rglnode.Count < 2)
                    {
                        rgnode.Remove(lnode);
                        rgleaf.AddLast(lnode);
                    }
                    lnode = lnodeNext;
                }

                for(;;)
                {
                    Debug.Assert(rgleaf.Count>0);

                    if(rgnode.Count == 0)
                    {
                        var stTree = StGet(rgleaf.Select(StGet).ToList());
                        //Info(iLine+": "+stTree);

                        mprgilineByStTree.EnsureGet(stTree, () =>
                        {
                            rgstTree.Add(stTree);
                            return new List<int>();
                        }).Add(iLine);
                        break;
                    }

                    var rgleafNext = new LinkedList<Node>();
                    for(var lleaf = rgleaf.First;lleaf!=null;lleaf=lleaf.Next)
                    {
                        var lparent = lleaf.Value.rglnode.Single();

                        lparent.Value.rglnode.Remove(lleaf);
                        if(lparent.Value.rglnode.Count==1)
                        {
                            rgnode.Remove(lparent);
                            rgleafNext.AddLast(lparent);
                        }
                        var st = StGet(lleaf.Value);
                        lparent.Value.rgst.Add(st);
                    }
                    rgleaf = rgleafNext;
                }

            }

            using(Output)
            {
                foreach(var stTree in rgstTree)
                {
                    WriteLine(mprgilineByStTree[stTree]);
                }
            }
        }



        private static string StGet(Node leaf)
        {
            var rgst = leaf.rgst;
            return StGet(rgst);
        }

        private static string StGet(List<string> rgst)
        {
            if(rgst.Count==0)
                return "x";
            rgst.Sort();
            var st = "(" + rgst.StJoin("") + ")";
            return st;
        }

    }


}
