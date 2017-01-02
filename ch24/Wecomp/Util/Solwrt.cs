using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Wecomp.Util
{
    /// <summary>
    /// defhun:solwrt solution writer
    /// </summary>
    public class Solwrt : IDisposable
    {
        public static readonly object NewLine=new object();

        private readonly ILg log = Lg.GetLogger(typeof (Solwrt));
        private readonly StreamWriter sw;
        private readonly StreamReader sr;
        public string StNewLine = "\n";
        public string NufDouble = "0.0000000";
        private int pos;

        public Solwrt(string fpat, string fpatRef = null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fpat));

            if(File.Exists(fpat))
                File.Move(fpat, FpatBak(fpat));
            sw = new StreamWriter(fpat);

            if(fpatRef != null && File.Exists(fpatRef))
                sr = new StreamReader(fpatRef);

            log.Info("Creating solution in: {0}".StFormat(fpat));
        }

        public static string FpatBak(string fpatBase)
        {
            var filnBase = Path.GetFileName(fpatBase);
            var rgfpatVersions = Directory.EnumerateFiles(Path.GetDirectoryName(fpatBase), filnBase + ".*");
            var rgfilnVersions = rgfpatVersions.Select(Path.GetFileName).ToList();
            var verMax = 0;
            foreach (var filnVersion in rgfilnVersions)
            {
                var stSuffix = filnVersion.Substring(filnBase.Length);
                var ver = 0;
                if (stSuffix.StartsWith(".v"))
                    int.TryParse(stSuffix.Substring(2), out ver);
                if (ver > verMax)
                    verMax = ver;
            }
            return fpatBase+".v"+(verMax+1);
        }

        //defhun:solf solution formatted object
        public string SolfFromObject(object obj)
        {
            if(obj == NewLine)
                return StNewLine;
            if(obj is double)
                return ((double)obj).ToString(NufDouble, CultureInfo.InvariantCulture);
            if(obj is decimal)
                return ((decimal)obj).ToString(CultureInfo.InvariantCulture);
            if(obj is BigInteger)
                return ((BigInteger)obj).ToString(CultureInfo.InvariantCulture);
            if (obj is int)
                return ((int) obj).ToString(CultureInfo.InvariantCulture);
            if (obj is long)
                return ((long) obj).ToString(CultureInfo.InvariantCulture);
            if (obj is ulong)
                return ((ulong)obj).ToString(CultureInfo.InvariantCulture);
            if (obj is uint)
                return ((uint)obj).ToString(CultureInfo.InvariantCulture);
            if (obj is char)
                return obj.ToString();
            if (obj is string)
                return obj as string;
            if(obj is IEnumerable)
            {
                var st = "";
                var fSep = false;
                foreach(var objT in (IEnumerable)obj)
                {
                    if(objT == NewLine)
                    {
                        st += StNewLine;
                        fSep = false;
                        continue;
                    }

                    if(fSep)
                        st += " ";
                    fSep = true;
                    st += SolfFromObject(objT);
                }
                return st;
            }
            throw new Exception("Unknown object type: " + obj.GetType());
        }

        public void WriteLine(object obj)
        {
            WriteSolf(SolfFromObject(obj) + StNewLine);
        }

        public void Write(object obj)
        {
            WriteSolf(SolfFromObject(obj));
        }
        public void Write(string stFormat, params object[] rgobj)
        {
            WriteSolf(string.Format(stFormat, rgobj.Select(SolfFromObject).ToArray()));
        }

        public void WriteLine(string stFormat, params object[] rgobj)
        {
            WriteSolf(string.Format(stFormat, rgobj.Select(SolfFromObject).ToArray()) + StNewLine);
        }

        private void WriteSolf(string solf)
        {
            sw.Write(solf);
            sw.Flush();

            if(sr == null)
                return;

            if(solf.All(ch =>
            {
                pos+=1;
                return (int) ch == sr.Read();
            }))
                return;

        //    sw.Dispose();
            throw new Exception(string.Format("Wrong Solution at {0}!", pos));
        }

        public void Dispose()
        {
            sw.Flush();
            sw.Dispose();

            if(sr!=null && !sr.EndOfStream)
                throw new Exception("Wrong Solution!");

            log.Info(string.Format("Solution is {0}", sr == null ? "UNCHECKED" : "OK"));

            if(sr!=null)
                sr.Dispose();
        }
    }
}
