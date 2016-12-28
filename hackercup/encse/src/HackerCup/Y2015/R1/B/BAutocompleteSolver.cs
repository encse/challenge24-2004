using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cmn.Util;
using Gcj.Util;
using HackerCup.Y2015.R1.D;

namespace HackerCup.Y2015.R1.B
{
    public class BAutocompleteSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            var cword = pparser.Fetch<int>();
            var rgword = pparser.FetchN<string>(cword);

            return () => Solve(rgword);
        }

        private IEnumerable<object> Solve(List<string> rgword)
        {
            var root = new Node();
            var res = 0;
            foreach (var word in rgword)
                res += Insert(word, 0, root);
            yield return res;
        }

        private int Insert(string word, int ich, Node node)
        {
            if (ich == word.Length)
                return 0;
            if (node.NodeNext.ContainsKey(word[ich]))
            {
                return 1 + Insert(word, ich + 1, node.NodeNext[word[ich]]);
            }

            for (var i = ich; i < word.Length; i++)
            {
                var nodeNew = new Node();
                node.NodeNext[word[i]] = nodeNew;
                node = nodeNew;
            }
            return 1;
        }

        private class Node
        {
            public Dictionary<char, Node> NodeNext = new Dictionary<char, Node>(); 
        }

    }
}
