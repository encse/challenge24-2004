using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wecomp.Util
{
    public interface ILg
    {
        void InfoFormat(string format, params object[] args);
        void Info(string msg);
    }

    public static class Lg
    {
        public static Func<Type, ILg> dgIlgFromTy;

        public static ILg GetLogger(Type type)
        {
            if(dgIlgFromTy==null)
                throw new Exception("Lg is not initialized");
            return dgIlgFromTy(type);
        }
    }

    public class LgConsole : ILg
    {
        private readonly Type ty;

        public LgConsole(Type ty)
        {
            this.ty = ty;
        }

        public void InfoFormat(string format, params object[] args)
        {
            Console.Write("{0} {1}: ", DateTime.Now.ToShortTimeString(), ty.FullName);
            Console.WriteLine(format,args);
        }

        public void Info(string msg)
        {

            Console.Write("{0} {1}: ", DateTime.Now.ToString("HH:mm:ss"), ty.FullName);
            Console.WriteLine(msg);
        }
    }
}
