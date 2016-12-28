using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cmn.Util
{
    public class Hlm_Chewbacca<T> : ICollection<T>
    {
        private Dictionary<T, bool> mp_element_f = new Dictionary<T, bool>();
        public Hlm_Chewbacca()
        {
        }

        public Hlm_Chewbacca(T element)
        {
            Add(element);
        }

        public Hlm_Chewbacca(params T[] rgelement)
            : this(rgelement as IEnumerable<T>)
        {
        }

        public Hlm_Chewbacca(IEnumerable<T> rgelement)
        {
            AddRange(rgelement);
        }

        public void Add(T element)
        {
            mp_element_f[element] = true;
        }

        public void AddRange(IEnumerable<T> element)
        {
            foreach (T t in element)
                mp_element_f[t] = true;
        }

        public void Clear()
        {
            mp_element_f.Clear();
        }

        public bool Contains(T element)
        {
            return mp_element_f.ContainsKey(element);
        }

        public bool Exists(Predicate<T> pred)
        {
            return mp_element_f.Keys.Any(t => pred(t));
        }

        public void CopyTo(T[] rgelement, int ielement)
        {
            foreach (T element in this)
                rgelement[ielement++] = element;
        }

        public bool Remove(T element)
        {
            return mp_element_f.Remove(element);
        }

        public int RemoveRange(IEnumerable<T> rgelement)
        {
            int i = 0;
            foreach (T elementToRemove in rgelement)
            {
                mp_element_f.Remove(elementToRemove);
                i++;
            }
            return i;
        }

        public int RemoveAll(Predicate<T> pred)
        {
            List<T> rgelementToRemove = mp_element_f.Keys.Where(item => pred(item)).ToList();
            return RemoveRange(rgelementToRemove);
        }

        public int Count
        {
            get { return mp_element_f.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return mp_element_f.Keys.GetEnumerator();
        }

        /// <summary>
        /// Gets the element from the set if its size is exactly 1.
        /// </summary>
        public T GetSingleElement()
        {
            if (Count != 1)
                throw new ArgumentException("Set doesn't contain a single element");
            return GetAnElement();
        }

        public T GetAnElement()
        {
            IEnumerator<T> rgelement = mp_element_f.Keys.GetEnumerator();
            rgelement.MoveNext();
            return rgelement.Current;
        }

        public T Pop()
        {
            IEnumerator<T> rgelement = mp_element_f.Keys.GetEnumerator();
            rgelement.MoveNext();
            T elem = rgelement.Current;
            Remove(elem);
            return elem;
        }

        public void Union(Hlm_Chewbacca<T> set)
        {
            AddRange(set);
        }

        public void Intersect(Hlm_Chewbacca<T> set)
        {
            foreach (T tItem in new List<T>(mp_element_f.Keys))
                if (!set.Contains(tItem))
                    Remove(tItem);
        }

        public void Subtract(Hlm_Chewbacca<T> set)
        {
            foreach (T tItem in new List<T>(mp_element_f.Keys))
                if (set.Contains(tItem))
                    Remove(tItem);
        }

        public ICollection<T> Elements
        {
            get
            {
                var rgelement = new List<T>(this.Count);
                foreach (T element in this)
                    rgelement.Add(element);
                return rgelement;
            }
        }

        public override bool Equals(object obj)
        {
            var setOther = obj as Hlm_Chewbacca<T>;
            if (setOther == null)
                return false;

            if (setOther.Count != this.Count) return false;
            foreach (T element in setOther)
                if (!this.Contains(element))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (T element in this)
                if (element != null) hashCode ^= element.GetHashCode();

            return hashCode;
        }

        public T[] ToArray()
        {
            T[] rgt = new T[this.Count];
            CopyTo(rgt, 0);
            return rgt;
        }
    }


    
}
