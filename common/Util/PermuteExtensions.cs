using System.Collections.Generic;
using System.Linq;

namespace Cmn.Util
{
    public static class PermuteExtensions
    {
        /// <summary>
        /// returns each permutation of the items in the enumerable
        /// </summary>
        public static IEnumerable<T[]> EntPermute<T>(this IEnumerable<T> ent)
        {
            var rgt = ent.ToArray();
            return EntPermute(rgt, new List<T>(), new bool[rgt.Length]);
        }

        private static IEnumerable<T[]> EntPermute<T>(IList<T> rgt, List<T> rgtOut, bool[] fseen)
        {
            if (rgtOut.Count == rgt.Count)
            {
                yield return rgtOut.ToArray();
                yield break;
            }

            for (int i = 0; i < rgt.Count; i++)
            {
                if (!fseen[i])
                {
                    fseen[i] = true;
                    rgtOut.Add(rgt[i]);
                    foreach (var x in EntPermute(rgt, rgtOut, fseen)) yield return x;
                    rgtOut.RemoveAt(rgtOut.Count - 1);
                    fseen[i] = false;
                }
            }
        }

        public static IEnumerable<T[]> EntChooseK<T>(this IEnumerable<T> ent, int k)
        {
            return EntChooseK(ent.ToArray(), new List<T>(), 0, k);
        }

        private static IEnumerable<T[]> EntChooseK<T>(IList<T> rgt, List<T> rgtOut, int i, int k)
        {
            if (k == 0)
            {
                yield return rgtOut.ToArray();
                yield break;
            }

            if (i + k <= rgt.Count)
            {
                rgtOut.Add(rgt[i]);
                foreach (var x in EntChooseK(rgt, rgtOut, i + 1, k - 1)) yield return x;
                rgtOut.RemoveAt(rgtOut.Count - 1);
                foreach (var x in EntChooseK(rgt, rgtOut, i + 1, k)) yield return x;
            }
        }

    }
}