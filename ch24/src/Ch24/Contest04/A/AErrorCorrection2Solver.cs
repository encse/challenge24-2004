using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ch24.Contest;

namespace Ch24.Contest04.A
{
    internal class AErrorCorrection2Solver : Solver
    {

        public override void Solve()
        {
            Solve(FpatIn, true);
        }

        private Dictionary dictionary;
		public void Solve(string filenameBase, bool writeToFile)
		{
			
			string dictFile = filenameBase.Replace(".in", ".dict");
			string inFile = filenameBase;
            string outFile = filenameBase.Replace(".in", ".out");
			
			dictionary = new Dictionary(dictFile);
			
			FileStream ifs = new FileStream(inFile, FileMode.Open, FileAccess.Read);
			FileStream ofs = new FileStream(outFile, FileMode.Create, FileAccess.Write);
			BinaryReader br = new BinaryReader(ifs);
			BinaryWriter bw = new BinaryWriter(ofs);

			string word = "";
			byte[] bbuf = new byte[1];
			int input = br.Read(bbuf, 0, 1);
			while(input > 0)
			{
				byte c = bbuf[0];
				if((c>='A'&& c <= 'Z') || (c>='a'&& c <= 'z') || c == '*')
				{
					word += (char)c;
				}
				else if(word != "")
				{
					string corrected = dictionary.FindMatch(word);
					foreach(char a in corrected)
						bw.Write(a);
					word = "";
					bw.Write(c);
				}
				else
				{
					bw.Write(c);
				}
				input = br.Read(bbuf, 0, 1);
			}
			Debug.Assert(word == "");
			ifs.Close();
			ofs.Close();
		}
	}

	class Dictionary
	{
		ArrayList items;
		public Dictionary(string filename)
		{
			items = new ArrayList();
			StreamReader sr = new StreamReader(filename);
			
			while(sr.Peek() >= 0)
			{
				string line = sr.ReadLine();
				Debug.Assert(line == line.ToLower());
				items.Add(line);
			}
		}

		public string FindMatch(string word)
		{
			if(word.IndexOf("*") == -1) return word;

			foreach(string item in items)
			{
				if(Matches(word,item))
				{
					StringBuilder sb = new StringBuilder();
					for(int i=0;i<word.Length;i++)
					{
						if(word[i] != '*')
						{
							sb.Append(word[i]);
						}
						else
						{
							sb.Append(item[i]);
						}
					}
					return sb.ToString();
				}
			}
			return null;
		}

		public bool Matches(string wordA, string wordB)
		{
			if(wordA.Length != wordB.Length) return false;
			wordA = wordA.ToLower();
			wordB = wordB.ToLower();
			for(int i=0; i< wordA.Length; i++)
			{
				if(wordA[i] == '*' || wordB[i] == '*') continue;
				if(wordA[i] != wordB[i]) return false;
			}
			return true;
		}


       
    }
}
