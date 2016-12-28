using System;
using System.Collections.Generic;
using System.Text;

namespace Cmn.Util
{
    public class Erclam : Exception
    {
        public Erclam(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// defhun:clam command line argument map
    /// </summary>
    public class Clam
    {
        //defhun:cln command line name
        //defhun:clv command line value
        private readonly Dictionary<string, string> mpclvBycln;

        public Clam(IEnumerable<string> rgcla)
        {
            
            mpclvBycln = new Dictionary<string, string>();
            foreach (var kvp in EnkvpFromRgcla(rgcla))
                mpclvBycln[kvp.Key] = kvp.Value;
        }

        private IEnumerable<KeyValuePair<string, string>> EnkvpFromRgcla(IEnumerable<string> rgcla)
        {
            string clnCurrent = null;
            var fClaPrevIsCln = false;
            foreach (var cla in rgcla)
            {
                var rgclaPart = cla.Split(new[] { ':', '=' }, 2);

                //if (rgclaPart.Length > 2)
                //    throw new Erclam("Parameter '{0}' should have the form '<parameter name>:<parameter value>'".StFormat(cla));

                var fCln = true;
                var claFirst = rgclaPart[0];
                if (claFirst.StartsWith("--"))
                    clnCurrent = claFirst.Substring(2);
                //else if (claFirst.StartsWith("/"))
                //    clnCurrent = claFirst.Substring(1);
                else if (claFirst.StartsWith("-"))
                    clnCurrent = claFirst.Substring(1);
                else if (fClaPrevIsCln)
                    fCln = false;
                else
                    throw new Erclam("Parameter name '{0}' should start with - or --".StFormat(claFirst));

                string clv;
                if (fCln)
                    clv = rgclaPart.Length == 2 ? rgclaPart[1] : null;
                else
                    clv = cla;

                yield return new KeyValuePair<string, string>(clnCurrent, clv);
                fClaPrevIsCln = fCln;
            }
        }

        public bool FHasCln(string cln)
        {
            return mpclvBycln.ContainsKey(cln);
        }

        public bool Flag(string cln)
        {
            if (!mpclvBycln.ContainsKey(cln))
                return false;
            if (mpclvBycln[cln] != null)
                throw new Erclam("Flag parameter {0} cannot have value".StFormat(cln));
            return true;
        }

        public string StGet(string cln)
        {
            return ClvRequiredFromCln(cln);
        }

        public string DpatGet(string cln)
        {
            var dpat = StGet(cln);
            return dpat.EndsWith("/") ? dpat : dpat + "/";
        }

        public string StGetOrDefault(string cln, string clvDefault)
        {
            return OclvFromCln(cln) ?? clvDefault;
        }

        private string ClvRequiredFromCln(string cln)
        {
            string clv;
            if (!mpclvBycln.TryGetValue(cln, out clv))
                throw new Erclam("Missing required parameter: " + cln);
            if (clv == null)
                throw new Erclam("Missing value for parameter: " + cln);
            return clv;
        }

        private string OclvFromCln(string cln)
        {
            string clv;
            return !mpclvBycln.TryGetValue(cln, out clv) ? null : clv;
        }

        public int IntGet(string cln)
        {
            return IntFromClv(ClvRequiredFromCln(cln), cln);
        }

        private static int IntFromClv(string clv, string cln)
        {
            int i;
            if (!int.TryParse(clv, out i))
                throw new Erclam("{0} must be an int".StFormat(cln));
            return i;
        }

        public int IntGetOrDefault(string cln, int iDefault)
        {
            var oclv = OclvFromCln(cln);
            return oclv == null ? iDefault : IntFromClv(oclv, cln);
        }

        public string[] RgstGet(string cln)
        {
            return StGet(cln).Split(',');
        }

        public string[] RgstGetOrDefault(string cln, string stDefault)
        {
            return StGetOrDefault(cln, stDefault).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string Tsto()
        {
            var sb = new StringBuilder();
            foreach(var kvp in mpclvBycln)
                sb.AppendLine("{0}: {1}".StFormat(kvp.Key, kvp.Value));
            return sb.ToString();
        }
    }
}
