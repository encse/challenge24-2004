using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;


namespace Wecomp.Util
{
    public static partial class U
    {
        public static Func<TResult> ToCached<TResult>(this Func<TResult> func)
        {
            var fCalled = false;
            var result = default (TResult);
            return () =>
            {
                if(!fCalled)
                {
                    result = func();
                    fCalled = true;
                }
                return result;
            };
        }

        public static Func<T, TResult> ToCached<T, TResult>(this Func<T, TResult> func)
        {
            var mpresByarg = new Dictionary<T, TResult>();
            return arg => mpresByarg.EnsureGet(arg, () => func(arg));
        }

        public static Func<T1, T2, TResult> ToCached<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            var f = ToCached<Tuple<T1, T2>, TResult>(tpl => func(tpl.Item1, tpl.Item2));
            return (arg1, arg2) => f(new Tuple<T1, T2>(arg1, arg2));
        }

        public static Func<T1, T2, T3, TResult> ToCached<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            var f = ToCached<Tuple<T1, T2, T3>, TResult>(tpl => func(tpl.Item1, tpl.Item2, tpl.Item3));
            return (arg1, arg2, arg3) => f(new Tuple<T1, T2, T3>(arg1, arg2, arg3));
        }

        public static Func<T1, T2, T3, T4, TResult> ToCached<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            var f = ToCached<Tuple<T1, T2, T3, T4>, TResult>(tpl => func(tpl.Item1, tpl.Item2, tpl.Item3, tpl.Item4));
            return (arg1, arg2, arg3, arg4) => f(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));
        }

        public static Func<T1, T2, T3, T4, T5, TResult> ToCached<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func)
        {
            var f = ToCached<Tuple<T1, T2, T3, T4, T5>, TResult>(tpl => func(tpl.Item1, tpl.Item2, tpl.Item3, tpl.Item4, tpl.Item5));
            return (arg1, arg2, arg3, arg4, arg5) => f(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));
        }

        public static T Binsearch<T>(Func<T,bool> fCheck, T tStart, T tStepFirst, T tPrecision, Func<T,T,T> add, Func<T,T> twice, Func<T,T> half, Func<T,T,T,bool> fOk)
        {
            var tLow = tStart;
            var fLow = fCheck(tLow);
            var tStep = tStepFirst;

            T tHigh;
            for(;;)
            {
                var t = add(tLow, tStep);

                if(fLow == fCheck(t))
                {
                    tLow = t;
                    tStep = twice(tStep);
                }
                else
                {
                    tHigh = t;
                    break;
                }
            }

            Debug.Assert(add(tLow,tStep).Equals(tHigh));

            for(;;)
            {
                tStep = half(tStep);
                var t = add(tLow, tStep);

                if(fOk(tLow,tHigh,tPrecision))
                    return t;

                if(fLow == fCheck(t))
                    tLow = t;
                else
                    tHigh = t;
            }
        }

        public static decimal BinsearchDecimal(Func<decimal,bool> fCheck, decimal tStart, decimal tStepFirst, decimal tPrecision)
        {
            return Binsearch(fCheck, tStart, tStepFirst, tPrecision, (arg1, arg2) => arg1 + arg2, arg => arg*2, arg => arg / 2, (arg1, arg2, arg3) => Math.Abs(arg1 - arg2) < arg3);
        }

        public static double BinsearchDouble(Func<double,bool> fCheck, double tStart, double tStepFirst, double tPrecision)
        {
            return Binsearch(fCheck, tStart, tStepFirst, tPrecision, (arg1, arg2) => arg1 + arg2, arg => arg * 2, arg => arg / 2, (arg1, arg2, arg3) => Math.Abs(arg1 - arg2) < arg3);
        }

        public static BigInteger BinsearchBiginteger(Func<BigInteger,bool> fCheck, BigInteger tStart, BigInteger tStepFirst, BigInteger tPrecision)
        {
            return Binsearch(fCheck, tStart, tStepFirst, tPrecision, (arg1, arg2) => arg1 + arg2, arg => arg * 2, arg => arg / 2, (arg1, arg2, arg3) => Abs(arg1 - arg2) < arg3);
        }

        public static BigInteger Abs(BigInteger a)
        {
            return a >= 0 ? a : -a;
        }

        public static T Max<T>(T v1, T v2) where T : IComparable<T>
        {
            return v1.CompareTo(v2) < 0 ? v2 : v1;
        }

        public static T Min<T>(T v1, T v2) where T : IComparable<T>
        {
            return v1.CompareTo(v2) < 0 ? v1 : v2;
        }

        public static Tuple<T, T> TupleIntersect<T>(Tuple<T, T> rng1, Tuple<T, T> rng2) where T : IComparable<T>
        {
            return new Tuple<T, T>(Max(rng1.Item1, rng2.Item1) , Min(rng1.Item2, rng2.Item2));
        }

        public static void Swap<T>(ref T v1, ref T v2)
        {
            var v = v1;
            v1 = v2;
            v2 = v;
        }

        public static string StFormat(this string stFormat, params object[] args)
        {
            return String.Format(stFormat, args);
        }

        public static string StJoin<T>(this IEnumerable<T> enumerable, string separator, Func<T, string> valueSelector)
        {
            return String.Join(separator, enumerable.Select(valueSelector).ToArray());
        }

        public static string StJoinToString<T>(this IEnumerable<T> enumerable, string separator)
        {
            return String.Join(separator, enumerable.Select(x => x.ToString()).ToArray());
        }

        public static string StJoin(this IEnumerable<string> enst, char chSep)
        {
            return String.Join(chSep.ToString(CultureInfo.InvariantCulture), enst.ToArray());
        }

        public static string StJoin(this IEnumerable<string> enst, string stSep)
        {
            return String.Join(stSep, enst.ToArray());
        }

        public static bool FIn<T>(this object obj, params T[] rgobj)
        {
            return rgobj.Any(objT => objT.Equals(obj));
        }
        public static TValue EnsureGet<TKey,TValue>(this IDictionary<TKey, TValue> mp, TKey key) where TValue : new ()
        {
            return mp.EnsureGet(key, () => new TValue());
        }

        public static TValue EnsureGet<TKey,TValue>(this IDictionary<TKey, TValue> mp, TKey key, Func<TValue> valueCreator)
        {
            TValue value;
            if (mp.TryGetValue(key, out value))
                return value;

            value = valueCreator();
            mp[key]=value;
            
            return value;
        }

        public static TValue GetOrDefault<TKey,TValue>(this IDictionary<TKey, TValue> mp, TKey key, TValue valueDefault = default (TValue))
        {
            TValue value;
            return mp.TryGetValue(key, out value) ? value : valueDefault;
        }

        public static IEnumerable<int> Eni(this int c, int iFirst = 0)
        {
            for(var i = iFirst; i < c; i++)
                yield return i;
        }

        public class Vxy<T>
        {
            public int x;
            public int y;
            public T v;
        }

        public static IEnumerable<Vxy<T>> Envxy<T>(this T[,] m)
        {
            for(var y = 0; y < m.YCount(); y++)
                for(var x = 0; x < m.XCount(); x++)
                    yield return new Vxy<T> {x = x, y = y, v = m[x, y]};
        }

        public static int XCount<T>(this T[,] m)
        {
            return m.GetLength(0);
        }

        public static int YCount<T>(this T[,] m)
        {
            return m.GetLength(1);
        }

        public static bool FValidX<T>(this T[,] m , params int[] rgx)
        {
            var c = m.XCount();
            return rgx.All(x => 0 <= x && x < c);
        }

        public static bool FValidY<T>(this T[,] m , params int[] rgy)
        {
            var c = m.YCount();
            return rgy.All(y => 0 <= y && y < c);
        }

        public static BigInteger Sum(this IEnumerable<BigInteger> source)
        {
            return source.Aggregate<BigInteger, BigInteger>(0, (sum, n) => sum + n);
        }

        public static T Mul<T>(this IEnumerable<T> source, Func<T,T,T> dgmul, T one)
        {
            return source.Aggregate(one, dgmul);
        }

        public  static BigInteger Mul(this IEnumerable<BigInteger> source)
        {
            return source.Mul((a, b) => a * b, 1);
        }

        public  static decimal Mul(this IEnumerable<decimal> source)
        {
            return source.Mul((a, b) => a * b, 1);
        }

        public  static int Mul(this IEnumerable<int> source)
        {
            return source.Mul((a, b) => a * b, 1);
        }

        public static T MulLazy<T>(this IEnumerable<T> source, Func<T, T, T> dgmul, T one, Func<T, bool> dgfzero)
        {
            var res = one;

            foreach (var n in source)
            {
                if (dgfzero(n))
                    return n;
                res = dgmul(res, n);
            }
            return res;
        }

        public static BigInteger MulLazy(this IEnumerable<BigInteger> source)
        {
            return source.MulLazy((a, b) => a * b, 1, a => a == 0);
        }

        public static decimal MulLazy(this IEnumerable<decimal> source)
        {
            return source.MulLazy((a, b) => a * b, 1, a => a == 0);
        }

        public static int MulLazy(this IEnumerable<int> source)
        {
            return source.MulLazy((a, b) => a * b, 1, a => a == 0);
        }

        public static IEnumerable<T> Encons<T>(this T t)
        {
            yield return t;
        }

        public static bool All(this IEnumerable<bool> rgf)
        {
            return rgf.All(f => f);
        }
    }
}
