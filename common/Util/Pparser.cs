using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cmn.Util
{
    public class Pparser
    {
        private readonly string fpat;
        private readonly string[] rgstLine;
        private int iLine = 0;

        public Pparser(string fpat)
        {
            this.fpat = fpat;
            rgstLine = File.ReadAllLines(fpat);
        }

        public string StLineNext()
        {
            return rgstLine[iLine++];
        }

        public T Parse<T>(string st)
        {
            return (T)Parse(st, typeof (T));
        }

        private object Parse(string st, Type rty)
        {
           
            if(rty.IsGenericType && typeof(List<>).IsAssignableFrom(rty.GetGenericTypeDefinition()))
            {
                var rtyElement = rty.GetGenericArguments().Single();
                var rgobjElement = st.Split(' ').Where(stT => !string.IsNullOrEmpty(stT)).Select(stT => Parse(stT, rtyElement)).ToArray();

                var rgElement = Array.CreateInstance(rtyElement, rgobjElement.Length);
                rgobjElement.ToArray().CopyTo(rgElement, 0);

                return Activator.CreateInstance(rty, rgElement);
            }
            if(rty.IsArray)
            {
                var rtyElement = rty.GetElementType();
                var rgobjElement = st.Split(' ').Where(stT => !string.IsNullOrEmpty(stT)).Select(stT => Parse(stT, rtyElement)).ToArray();
                
                var rgElement = Array.CreateInstance(rtyElement, rgobjElement.Length);
                rgobjElement.ToArray().CopyTo(rgElement, 0);
                
                return rgElement;

            }
            if (typeof(string).IsAssignableFrom(rty))
                return st;
            if (typeof(int).IsAssignableFrom(rty))
                return int.Parse(st);
            if (typeof(uint).IsAssignableFrom(rty))
                return uint.Parse(st);
            if (typeof(long).IsAssignableFrom(rty))
                return long.Parse(st);
            if (typeof(ulong).IsAssignableFrom(rty))
                return ulong.Parse(st);
            if (typeof(double).IsAssignableFrom(rty))
                return double.Parse(st, CultureInfo.InvariantCulture);
            if (typeof(float).IsAssignableFrom(rty))
                return float.Parse(st, CultureInfo.InvariantCulture);
            if (typeof(decimal).IsAssignableFrom(rty))
                return decimal.Parse(st, CultureInfo.InvariantCulture);
            if (typeof(BigInteger).IsAssignableFrom(rty))
                return BigInteger.Parse(st);
            if (rty.IsEnum)
                return Enum.Parse(rty, st);
            
            if (rty.GetConstructors().Length == 1)
            {
                var ci = rty.GetConstructors().Single();

                var rgst = st.Split(' ');
                if(ci.GetParameters().Any())
                {

                    var rgobj = new object[ci.GetParameters().Length];
                    if(rgst.Length != rgobj.Length)
                        throw new ArgumentException();

                    var i = 0;
                    foreach(var pi in ci.GetParameters())
                    {
                        rgobj[i] = Parse(rgst[i], pi.ParameterType);
                        i++;
                    }
                    return ci.Invoke(rgobj);
                }
                else
                {
                    var t = ci.Invoke(null);
                    var rgfields = rty.GetFields();
                    if(rgst.Length != rgfields.Length)
                        throw new ArgumentException();

                    foreach(var vifield in rgfields.Select((v, i) => new {v, i}))
                        vifield.v.SetValue(t, Parse(rgst[vifield.i], vifield.v.FieldType));
                    return t;
                }
            }

            throw new ArgumentException();
        }

        public T Fetch<T>()
        {
            return Parse<T>(StLineNext());
        }

        public void Fetch<T1, T2>(out T1 t1, out T2 t2)
        {
            var rgst = StLineNext().Split(' ');
            t1 = Parse<T1>(rgst[0]);
            t2 = Parse<T2>(rgst.Skip(1).StJoin(" "));
        }
        public void Fetch<T1, T2, T3>(out T1 t1, out T2 t2, out T3 t3)
        {
            var rgst = StLineNext().Split(' ');
            t1 = Parse<T1>(rgst[0]);
            t2 = Parse<T2>(rgst[1]);
            t3 = Parse<T3>(rgst.Skip(2).StJoin(" "));
        }
        public void Fetch<T1, T2, T3, T4>(out T1 t1, out T2 t2, out T3 t3, out T4 t4)
        {
            var rgst = StLineNext().Split(' ');
            t1 = Parse<T1>(rgst[0]);
            t2 = Parse<T2>(rgst[1]);
            t3 = Parse<T3>(rgst[2]);
            t4 = Parse<T4>(rgst.Skip(3).StJoin(" "));
        }

        public void Fetch<T1, T2, T3, T4,T5>(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5)
        {
            var rgst = StLineNext().Split(' ');
            t1 = Parse<T1>(rgst[0]);
            t2 = Parse<T2>(rgst[1]);
            t3 = Parse<T3>(rgst[2]);
            t4 = Parse<T4>(rgst[3]);
            t5 = Parse<T5>(rgst.Skip(4).StJoin(" "));
        }
        public bool FEof()
        {
            return iLine >= rgstLine.Length;
        }

        public List<T> FetchN<T>(int cnode)
        {
            var rgt = new List<T>();
            for (var inode = 0; inode < cnode; inode++)
                rgt.Add(Fetch<T>());
            return rgt;
        }

      
    }
}
