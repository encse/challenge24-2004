using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cmn.Util
{
    public interface IRanked<out TRank> where TRank : IComparable<TRank>
    {
        TRank Rank { get; }
    }

    public class RankedSet<TItem, TRank> : ISet<TItem> 
        where TItem : IRanked<TRank>
        where TRank : IComparable<TRank>
    {
        private SortedDictionary<TRank, HashSet<TItem>> mphlmitemByRank = new SortedDictionary<TRank, HashSet<TItem>>();
        
        public HashSet<TItem> ToHashSet()
        {
            return new HashSet<TItem>(ToEnumerable());
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return ToEnumerable().GetEnumerator();
        }

        private IEnumerable<TItem> ToEnumerable()
        {
            return mphlmitemByRank.Values.SelectMany(hlmitem => hlmitem);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<TItem>.Add(TItem item)
        {
            Add(item);
        }

        public void UnionWith(IEnumerable<TItem> other)
        {
            foreach(var item in other)
            {
                Add(item);
            }
        }

        public void IntersectWith(IEnumerable<TItem> other)
        {
            var hlm = ToHashSet();
            Clear();
            hlm.IntersectWith(other);
            UnionWith(hlm);
        }

        public void ExceptWith(IEnumerable<TItem> other)
        {
            foreach(var item in other)
            {
                Remove(item);
            }
        }

        public void SymmetricExceptWith(IEnumerable<TItem> other)
        {
            var hlm = ToHashSet();
            Clear();
            hlm.SymmetricExceptWith(other);
            UnionWith(hlm);
        }

        public bool IsSubsetOf(IEnumerable<TItem> other)
        {
            return ToHashSet().IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<TItem> other)
        {
            return ToHashSet().IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<TItem> other)
        {
            return ToHashSet().IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<TItem> other)
        {
            return ToHashSet().IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<TItem> other)
        {
            return other.Any(Contains);
        }

        public bool SetEquals(IEnumerable<TItem> other)
        {
            return other.All(Contains);
        }

        public bool Add(TItem item)
        {
            var fAdded = mphlmitemByRank.EnsureGet(item.Rank).Add(item);
            if(fAdded)
                Count++;
            return fAdded;
        }

        public void Clear()
        {
            mphlmitemByRank.Clear();
            Count = 0;
        }

        public bool Contains(TItem item)
        {
            var ohlmItem = mphlmitemByRank.GetOrDefault(item.Rank);
            return ohlmItem != null && ohlmItem.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            ToHashSet().CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item)
        {
            var rank = item.Rank;
            var ohlmItem = mphlmitemByRank.GetOrDefault(rank);
            if(ohlmItem==null)
                return false;
            
            var fRemoved = ohlmItem.Remove(item);
            if(!fRemoved)
                return false;

            if(ohlmItem.Count == 0)
                mphlmitemByRank.Remove(rank);

            Count--;

            return true;
        }

        public int Count { get; private set; }
        public bool IsReadOnly 
        {
            get
            {
                return false;
            }
        }
    }
}