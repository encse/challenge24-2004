using System;
using System.Collections.Generic;

namespace Cmn.Util
{
    public static class MinMaxKer
    {
        public static IEnumerable<T> RgtMax<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        {
            return Opt(ent, dgweight, fMax: true).Item2;
        }

        public static TWeight WMax<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        {
            return Opt(ent, dgweight, fMax: true).Item1;
        }

        public static Tuple<TWeight, IEnumerable<T>> WAndRgtMax<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        {
            return Opt(ent, dgweight, fMax: true);
        }

        public static IEnumerable<T> RgtMin<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        {
            return Opt(ent, dgweight, fMax: false).Item2;
        }

        public static TWeight WMin<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        { 
            return Opt(ent, dgweight, fMax: false).Item1;
        }

        public static Tuple<TWeight, IEnumerable<T>> WAndRgtMin<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight) where TWeight : IComparable<TWeight>
        {
            return Opt(ent, dgweight, fMax: false);
        }

        public static int MaxAt<T>(IList<T> rgt) where T: IComparable<T>
        {
            T tmax = default(T);
            int iMax = -1;
            for(int i=0;i<rgt.Count;i++)
            {
                if (iMax == -1)
                {
                    iMax = i;
                    tmax = rgt[i];
                }
                else if( rgt[i].CompareTo(tmax) > 0)
                {
                    iMax = i;
                    tmax = rgt[i];
                }
            }
            return iMax;
        }

        private static Tuple<TWeight, IEnumerable<T>> Opt<T, TWeight>(IEnumerable<T> ent, Func<T, TWeight> dgweight, bool fMax) where TWeight : IComparable<TWeight>
        {
            var fFirst = true;
            var weightOpt = default(TWeight);
            var rgtOpt = new List<T>();

            foreach (var t in ent)
            {
                var weight = dgweight(t);
                if (fFirst)
                {
                    weightOpt = weight;
                    rgtOpt.Add(t);
                    fFirst = false;
                    continue;
                }

                var compare = weightOpt.CompareTo(weight);

                if (compare == 0)
                {
                    rgtOpt.Add(t);
                }
                else if (fMax && compare < 0 || !fMax && compare > 0)
                {
                    rgtOpt = new List<T> { t };
                    weightOpt = weight;
                }
            }
            return new Tuple<TWeight, IEnumerable<T>>(weightOpt, rgtOpt);
        }
    }
}