using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest04.A
{
    internal class AErrorCorrectionSolver : Solver
    {

        public override void Solve()
        {
            var mprgstBycch = new Dictionary<int, List<string>>();
            foreach(var st in File.ReadAllLines(FpatIn.Replace(".in", ".dict")))
            {
                if (!mprgstBycch.ContainsKey(st.Length))
                    mprgstBycch[st.Length] = new List<string>();
                mprgstBycch[st.Length].Add(st);
            }


            using (var bw = new BinaryWriter(File.Open(FpatOut, FileMode.Create)))
            {
                foreach (var tit in Entit(File.ReadAllBytes(FpatIn)))
                {
                    if (!tit.rgch.Contains((byte)'*'))
                    {
                        bw.Write(tit.rgch);
                    }
                    else
                    {
                        var rgch = tit.rgch;
                        bool fFound = false;
                        foreach (var stCandidate in mprgstBycch[rgch.Length])
                        {
                            if(FMatch(rgch, stCandidate))
                            {
                                for (int i = 0; i < rgch.Length; i++)
                                {
                                    if(rgch[i] == (byte)'*')
                                        bw.Write((byte) stCandidate[i]);
                                    else
                                        bw.Write(rgch[i]);
                                }
                                fFound = true;
                                break;
                                ;
                            }
                        }

                        if (!fFound)
                            throw new Exception("coki");
                    }
                   
                }
            }
        }

        private bool FMatch(byte[] rgch, string stCandidate)
        {
            for(int i=0;i<rgch.Length;i++)
            {
                if(rgch[i] == (byte)'*')
                    continue;
                if( char.ToLower((char)rgch[i], CultureInfo.InvariantCulture) != (byte)stCandidate[i])
                    return false;
            }
            return true;
        }

        private IEnumerable<Word> Enword(byte[] rgbyteText)
        {
            return Entit(rgbyteText).Where(tit => tit is Word).Cast<Word>();
        }

        private IEnumerable<Tit> Entit(byte[] rgbyteText)
        {
            var ibyteTitstart = 0;
            var fInWord = false;
            for (int ibyte = 0; ibyte < rgbyteText.Length; ibyte++)
            {
                if (!FWordChar(rgbyteText[ibyte]))
                {
                    if (fInWord)
                    {
                        yield return new Word(rgbyteText.RangeGet(ibyteTitstart, ibyte - ibyteTitstart));
                        fInWord = false;
                        ibyteTitstart = ibyte;
                    }
                }
                else
                {
                    if (!fInWord)
                    {
                        yield return new Ws(rgbyteText.RangeGet(ibyteTitstart, ibyte - ibyteTitstart));
                        fInWord = true;
                        ibyteTitstart = ibyte;
                    }
                }
            }

            if (fInWord)
                yield return new Word(rgbyteText.RangeGet(ibyteTitstart, rgbyteText.Length - ibyteTitstart));
            else
                yield return new Ws(rgbyteText.RangeGet(ibyteTitstart, rgbyteText.Length - ibyteTitstart));
        }

        private bool FWordChar(byte b)
        {
            return 'a' <= b && b <= 'z' || 'A' <= b && b <= 'Z' || b == '*';
        }

        private class Tit
        {
            public readonly byte[] rgch;

            protected Tit(byte[] rgch)
            {
                this.rgch = rgch;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Word)) return false;
                return rgch.SequenceEqual(((Word)obj).rgch);
            }

            public override int GetHashCode()
            {
                return ((IStructuralEquatable) rgch).GetHashCode(EqualityComparer<object>.Default);
            }
            public override string ToString()
            {
                return "#"+Encoding.Default.GetString(rgch)+"#";
            }
        }

        private class Word : Tit
        {
            public Word(byte[] rgch) : base(rgch)
            {
            }
        }

        private class Ws : Tit
        {
            public Ws(byte[] rgch) : base(rgch)
            {
            }
        }

       
    }
}
