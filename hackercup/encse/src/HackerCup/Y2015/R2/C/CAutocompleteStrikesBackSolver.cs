using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Xml.Schema;
using Cmn.Util;
using Gcj.Util;

namespace HackerCup.Y2015.R2.C
{

    public class CAutocompleteStrikesBackSolver : IConcurrentSolver
    {
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
            int n, k;
            pparser.Fetch(out n, out k);
            var rgword = pparser.FetchN<string>(n);
            return () => Solve(k,new HashSet<string>(rgword));
        }

        private IEnumerable<object> Solve(int k, HashSet<string> rgword)
        {
            Node root= null;

            while (rgword.Count > k)
            {
                root = new Node();
                root.suffix = rgword.First();
                foreach (var word in rgword.Skip(1))
                    Insert(word, 0, root);
                var rgwordToRemove = MinMaxKer.RgtMax(rgword, word => Cch(root, word, 0));
                var wordToRemove = MinMaxKer.RgtMin(rgwordToRemove, word =>
                {
                    var nodeParent = NodeParent(root, word, 0);
                    var z = 0;
                    while (nodeParent != null)
                    {
                        z += nodeParent.NodeNext.Count;
                        nodeParent = nodeParent.nodeParent;

                    }
                    return z;
                }).First();
                rgword.Remove(wordToRemove);
            }

            {
                root = new Node();
                root.suffix = rgword.First();
                foreach (var word in rgword.Skip(1))
                    Insert(word, 0, root);
            }
            yield return rgword.Select(word => Cch(root, word, 0)).Sum();
        }

        private Node NodeParent(Node node, string word, int ich)
        {
            if (node.suffix == word.Substring(ich))
                return node.nodeParent;
            return NodeParent(node.NodeNext[word[ich]], word, ich + 1);
        }

        private int Cch(Node node, string word, int ich)
        {
            if (node.suffix == word.Substring(ich))
                return 0;
            return 1 + Cch(node.NodeNext[word[ich]], word, ich + 1);
        }

        private void Insert(string word, int ich, Node node)
        {
            if (ich == word.Length)
                return;

            if (!string.IsNullOrEmpty(node.suffix))
            {
                var nodeNew = new Node();
                node.NodeNext[node.suffix[0]] = nodeNew;
                nodeNew.nodeParent = node;
                nodeNew.suffix = node.suffix.Substring(1);
                node.suffix = null;

            }

            if (node.NodeNext.ContainsKey(word[ich]))
            {
                Insert(word, ich + 1, node.NodeNext[word[ich]]);
            }
            else
            {
                var nodeNew = new Node();
                node.NodeNext[word[ich]] = nodeNew;
                nodeNew.nodeParent = node;
                nodeNew.suffix = word.Substring(ich+1);
            }
        }

        private class Node
        {
            public string suffix;
            public Node nodeParent;
            public Dictionary<char, Node> NodeNext = new Dictionary<char, Node>();
        }

    }
}
