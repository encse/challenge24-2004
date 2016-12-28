using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2013.QR.D
{
    public class TreasureSolver : IConcurrentSolver
    {
       
        public int CCaseGet(Pparser pparser)
        {
            return pparser.Fetch<int>();
        }

        public ConcurrentGcjSolver.DgSolveCase DgSolveCase(Pparser pparser)
        {
         

            /*
             * The first line of the input gives the number of test cases, T. T test cases follow. Each test case begins with a single 
             * line containing two positive integers K and N, representing the number of keys you start with and the number of chests you need to open.
             * 
             * This is followed by a line containing K integers, representing the types of the keys that you start with.
             * 
             * After that, there will be N lines, each representing a single chest. Each line will begin with integers Ti and Ki,
             * indicating the key type needed to open the chest and the number of keys inside the chest. These two integers will be followed
             * by Ki more integers, indicating the types of the keys contained within the chest.
             */


            int ckey;
            int cchest;
            pparser.Fetch(out ckey, out cchest);
            var rgkey = pparser.Fetch<int[]>();
            var rgchest = new List<Chest>();
            for(int ichest=0;ichest<cchest;ichest++)
            {
                var rgxxx = pparser.Fetch<int[]>();
                var keyToOpen = rgxxx.First();
                var rgkeyInside = rgxxx.Skip(2);
                rgchest.Add(new Chest(ichest+1, keyToOpen, rgkeyInside.ToArray()));
            }

            return () => Solve(new Keyring(rgkey), rgchest);
        }

        public class Chest
        {
            public readonly int id;
            public readonly int keyToOpen;
            public readonly int[] rgkeyInside;

            public Chest(int id, int keyToOpen, int[] rgkeyInside)
            {
                this.id = id;
                this.keyToOpen = keyToOpen;
                this.rgkeyInside = rgkeyInside;
            }

            public void Open(Keyring keyring)
            {
                keyring.Remove(keyToOpen);
                foreach (var key in rgkeyInside)
                    keyring.Add(key);
            }

            public void Close(Keyring keyring)
            {
                keyring.Add(keyToOpen);
                foreach (var key in rgkeyInside)
                    keyring.Remove(key);
            }
        }

        public class Keyring
        {
            private Dictionary<int, int> mpckeybykey = new Dictionary<int, int>();
            public IEnumerable<int> Keys
            {
                get { return mpckeybykey.Keys; }
            }

            public Keyring()
            {
                
            }
            public Keyring(IEnumerable<int> rgkey)
            {
                foreach (var key in rgkey)
                    Add(key);
            }

            public Keyring(Keyring keyring)
            {
                mpckeybykey = new Dictionary<int, int>(keyring.mpckeybykey);
            }

            public void Add(int key)
            {
                if (!mpckeybykey.ContainsKey(key))
                    mpckeybykey[key] = 1;
                else
                    mpckeybykey[key]++;
            }

            public void Remove(int key)
            {
                if (mpckeybykey[key] == 1)
                    mpckeybykey.Remove(key);
                else
                    mpckeybykey[key]--;
            }

            public bool FContains(int key)
            {
                return mpckeybykey.ContainsKey(key);
            }


            public int Ckey(int keyToOpen)
            {
                return mpckeybykey.ContainsKey(keyToOpen) ? mpckeybykey[keyToOpen] : 0;
            }

            public void AddRange(IEnumerable<int> rgkey)
            {
                foreach (var key in rgkey)
                    Add(key);
            }
        }

        private void Tsto(Keyring keyring, List<Chest>rgchest)
        {
            /*
            Chest Number  |  Key Type To Open Chest  |  Key Types Inside
            --------------+--------------------------+------------------
1             |  1                       |  None
2             |  1                       |  1, 3
3             |  2                       |  None
4             |  3                       |  2
             * 
             * */

            var sb = new StringBuilder();
            sb.AppendLine("key type  | ckey");
            sb.AppendLine("----------+-----");
            foreach (var key in keyring.Keys)
                sb.AppendLine(" {0,-9}| {1,-4}".StFormat(key, keyring.Ckey(key)));

            sb.AppendLine();
            sb.AppendLine("Chest Number  |  Key Type To Open Chest  |  Key Types Inside");
            sb.AppendLine("--------------+--------------------------+------------------");
            foreach (var chest in rgchest)
            {
                sb.AppendLine("  {0,-12}|  {1,-24}|  {2,-15}".StFormat(chest.id, chest.keyToOpen, chest.rgkeyInside.Select(x => x.ToString()).StJoin(" ")));
            }
            Debug.WriteLine(sb.ToString());
        }
        private IEnumerable<object> Solve(Keyring keyring, List<Chest> rgchest)
        {
            xxx = 0;
            var stackChest = new Stack<Chest>();
            if (FSolveRecursive(rgchest.ToArray(), keyring, stackChest))
            {
                foreach (var i in stackChest.Reverse())
                    yield return i.id;
            }
            else
            {
                yield return "IMPOSSIBLE";
            }
        }

        private bool FSolveRecursive(Chest[] rgchest, Keyring keyring, Stack<Chest> stackChest)
        {
            if (rgchest.Length == 0)
                return true;
            if (!FPossible(rgchest, keyring))
                return false;

            for(int ichest=0;ichest<rgchest.Length;ichest++)
            {
                var chest = rgchest[ichest];
                if(keyring.FContains(chest.keyToOpen))
                {
                    var rgchestAfterOpen = RemoveChest(ichest, rgchest);
                    chest.Open(keyring);
                    stackChest.Push(chest);
                    if (FSolveRecursive(rgchestAfterOpen, keyring, stackChest))
                        return true;
                    chest.Close(keyring);
                    stackChest.Pop();
                }
            }
            return false;
        }

        private int xxx = 0;
        private bool FPossible(Chest[] rgchest, Keyring keyring)
        {
            var hlmchestStart = new Hlm_Chewbacca<Chest>(rgchest);
            var keyringStart = new Keyring(keyring);

            var hlmchest = new Hlm_Chewbacca<Chest>(rgchest);
            
            var fOpened = true;
            while(fOpened)
            {
                var hlmchestNext = new Hlm_Chewbacca<Chest>();
                var keyringNext = new Keyring(keyring);
                
                fOpened = false;
            
                foreach (var chest in hlmchest)
                {
                    if (keyring.FContains(chest.keyToOpen))
                    {
                        fOpened = true;
                        foreach (var key in chest.rgkeyInside)
                            keyringNext.Add(key);
                    }
                    else
                        hlmchestNext.Add(chest);
                }

                if (!fOpened && hlmchestNext.Any())
                {
                    if(rgchest.Length>=xxx)
                    {
                        xxx = rgchest.Length;
                        //Console.WriteLine("cut: " +xxx);
                    }
                    return false;
                }
                hlmchest = hlmchestNext;
                keyring = keyringNext;
            }

            
            var keyringNeeded = new Keyring();
            foreach (var chest in rgchest)
                keyringNeeded.Add(chest.keyToOpen);

            var keyringAvailable = new Keyring(keyringStart);
            foreach (var chest in rgchest)
                keyringAvailable.AddRange(chest.rgkeyInside);

            foreach (var keyNeeded in keyringNeeded.Keys)
            {
                if(keyringNeeded.Ckey(keyNeeded)>keyringAvailable.Ckey(keyNeeded))
                    return false;
            }
            return true;
        }

        Chest[] RemoveChest(int i, Chest[] rgchest)
        {
            var rgchestResult = new Chest[rgchest.Length - 1];
            if (i > 0)
                Array.Copy(rgchest, 0, rgchestResult, 0, i);
            Array.Copy(rgchest, i + 1, rgchestResult, i, rgchest.Length - i - 1);
            return rgchestResult;
        }
    }
}
