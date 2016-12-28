using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest05.E
{
    internal class EasyCompressionSolver : Solver
    {
        public override void Solve()
        {
            var mpcOccurenceByword = new Dictionary<Word, int>();

            byte[] rgbyteText = File.ReadAllBytes(FpatIn);
            foreach (var word in Enword(rgbyteText))
            {
                if (!mpcOccurenceByword.ContainsKey(word))
                    mpcOccurenceByword[word] = 1;
                else
                    mpcOccurenceByword[word]++;
            }
            var v = mpcOccurenceByword.OrderBy(kvp => kvp.Value);

            var wordMostFrequent = mpcOccurenceByword.OrderBy(kvp => kvp.Value).Last().Key;

            Directory.CreateDirectory(DpatOut);
            using (var bw = new BinaryWriter(File.Open(FpatOut, FileMode.Create)))
            {
                foreach (var tit in Entit(rgbyteText))
                {
                    if(!wordMostFrequent.Equals(tit))
                        bw.Write(tit.rgch);
                    else
                        bw.Write((byte) '*');
                }
            }
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
                if (rgbyteText[ibyte].FIn(((byte) ' '), ((byte) '\n'), ((byte) '\r')))
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
