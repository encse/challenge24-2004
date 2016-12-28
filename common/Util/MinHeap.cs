using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmn.Util
{
    public class MinHeap<T> where T : IComparable<T>
    {

        private readonly List<T> Rgt;

        public MinHeap()
        {
            Rgt = new List<T>();
        }

        public MinHeap(IEnumerable<T> ent) : this()
        {
            Rgt.AddRange(ent);
            Heapify();
        }

        public void Add(T e)
        {
            if(FEmpty)
                Rgt.Add(e);
            else
                SiftUp(Rgt.Count, e);
        }

        public bool FEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        public T Peek()
        {
            return Rgt.First();
        }

        public int Count
        {
            get
            {
                return Rgt.Count;
            }
        }

        public void Clear()
        {
            Rgt.Clear();
        }

        public T Poll()
        {
            var result = Peek();
            var last = Rgt.Last();
            Rgt.RemoveAt(Count - 1);
            if(!FEmpty)
                SiftDown(0, last);
            return result;
        }

        private void RemoveAt(int i)
        {
            if(Count - 1 == i) // removed last element
                Rgt.RemoveAt(i);
            else
            {
                var moved = Rgt.Last();
                Rgt.RemoveAt(Count - 1);
                SiftDown(i, moved);
                if(ReferenceEquals(Rgt[i], moved))
                {
                    SiftUp(i, moved);
                }
            }
        }

        private void SiftUp(int i, T t)
        {
            while(i > 0)
            {
                var iParent = (i - 1) / 2;
                var tParent = Rgt[iParent];
                if(tParent.CompareTo(t) <= 0)
                    break;
                Rgt[i] = tParent;
                i = iParent;
            }
            Rgt[i] = t;
        }

        private void SiftDown(int i, T t)
        {
            var cHalf = Count / 2; // loop while a non-leaf
            while(i < cHalf)
            {
                var iLeft = (i * 2) + 1; // assume left child is least
                var tLeft = Rgt[iLeft];
                var iRight = iLeft + 1;

                var iMin=iLeft;
                var tMin=tLeft;
                if(iRight < Count)
                {
                    var tRight = Rgt[iRight];
                    if(tRight.CompareTo(tLeft) < 0)
                    {
                        iMin = iRight;
                        tMin = tRight;
                    }
                }
                if(t.CompareTo(tMin) <= 0)
                    break;
                Rgt[i] = tMin;
                i = iMin;
            }
            Rgt[i] = t;
        }

        private void Heapify()
        {
            for(var i = (Count / 2) - 1; i >= 0; i--)
                SiftDown(i, Rgt[i]);
        }

    }
}
