using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wecomp.Util
{
    public class Wrp<T>
    {
        public T V;

        public bool Equals(Wrp<T> other)
        {
            if(ReferenceEquals(null, other))
                return false;
            if(ReferenceEquals(this, other))
                return true;
            return Equals(other.V, V);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            if(obj.GetType() != typeof(Wrp<T>))
                return false;
            return Equals((Wrp<T>) obj);
        }

        public override int GetHashCode()
        {
            return V.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("V: {0}", V);
        }
    }
}
