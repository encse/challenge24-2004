using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Util;

namespace Ch24.Contest04.C
{
    public class CTvProgrammingSolver : Ch24.Contest.Solver
    {
        public override void Solve()
        {
            var pp = new Pparser(FpatIn);

            var n = pp.Fetch<int>();

            var rgnode = pp.FetchN<Node>(n).ToArray();
           
            rgnode = rgnode.OrderBy(node => node.dtStart).ToArray();

            for (int inode = 0; inode < n; inode++)
                rgnode[inode].inode = inode;
         
            var m = 0; 
            for (int i = 0; i < rgnode.Length; i++)
                m = Math.Max(m, MaxV(rgnode[i], rgnode));
                

            Console.WriteLine(m);
        }


        int InodeJustAfter(Node node, Node[] rgnode)
        {
            var inode = (node.inode + 1)%rgnode.Length;
            for (;;)
            {
                if (!FAOverlapsB(node, rgnode[inode]))
                    return inode;
                inode = (inode + 1)%rgnode.Length;
            }
        }

        private int MaxV(Node nodeStart, Node[] rgnode)
        {
            var inodeStart = nodeStart.inode;
                 
            var rgvmax = new int[rgnode.Length];

            for (int jnode = InodeJustAfter(rgnode[inodeStart], rgnode); inodeStart != jnode ; jnode = (jnode + 1) % rgnode.Length)
            {
                if(!FAOverlapsB(rgnode[inodeStart], rgnode[jnode]) && !FAOverlapsB(rgnode[jnode], rgnode[inodeStart]))
                    rgvmax[jnode] = Math.Max(rgvmax[jnode], rgnode[inodeStart].value + rgvmax[inodeStart]);
            }

            int inodeNext= (inodeStart + 1) % rgnode.Length;
            for (var inode = inodeNext; inode != inodeStart; inode = (inode + 1) % rgnode.Length)
            {
                for (int jnode = InodeJustAfter(rgnode[inode], rgnode); inode != jnode; jnode = (jnode + 1) % rgnode.Length)
                {
                    if (!FAOverlapsB(rgnode[inode], rgnode[jnode]) && !FAOverlapsB(rgnode[jnode], rgnode[inode]))
                    {
                        rgvmax[jnode] = Math.Max(rgvmax[jnode], rgnode[inode].value + rgvmax[inode]);
                        
                    }
                    
                }
            }

            return rgvmax[inodeStart];

        }

        private IEnumerable<Node> EnnodeOverlaps(Node nodeB, IEnumerable<Node> rgnode)
        {
            return rgnode.Where(nodeA => nodeB != nodeA && FAOverlapsB(nodeA, nodeB));
        }


        static DateTime dtMidnight = DateTime.Parse("2014-12-23 23:59:59.9999");
        static DateTime dtDawn = DateTime.Parse("2014-12-23 00:00");


        private bool FAOverlapsB(Node nodeA, Node nodeB)
        {
            if (nodeA.dtStart <= nodeA.dtEnd)
            {
                if (nodeA.dtStart <= nodeB.dtStart && nodeB.dtStart < nodeA.dtEnd)
                    return true;
            }
            else
            {
              
                if (nodeA.dtStart <= nodeB.dtStart && nodeB.dtStart < dtMidnight)
                    return true;
                if (dtDawn <= nodeB.dtStart && nodeB.dtStart < nodeA.dtEnd)
                    return true;
            }
            return false;
        }

        class Node
        {
            public DateTime dtStart;
            public DateTime dtEnd;
            public int value;
            public int inode;
            
            public Node(string stStart, string stEnd, int value)
            {
                dtStart = DateTime.Parse("2014-12-23 " + stStart);
                dtEnd = DateTime.Parse("2014-12-23 " + stEnd);
                this.value = value;
            }
        }


    }
}
