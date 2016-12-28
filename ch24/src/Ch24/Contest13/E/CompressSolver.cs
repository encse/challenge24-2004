using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ch24.Contest;
using Ch24.Contest13.A;
using Cmn.Util;

namespace Ch24.Contest13.E
{
    class CompressSolver : Solver
    {
        public override void Solve()
        {

            var st = File.ReadAllText(FpatIn, Encoding.ASCII);



            var rutc = RutcGenerate(
                st,
                CeoSolver.mpcoccByStSubGet(st));
                
            using(var solwrt = new Solwrt(FpatOut))
            {
                foreach (var stm in rutc.Rgstm)
                {
                    solwrt.WriteLine(stm.Disasm());
                }
            }
        
        }

        Rut RutcGenerate(string st, Dictionary<string, int> mpcstByst)
        {
            var mprutBySt = new Dictionary<string, Rut>();
            foreach (var kvp in mpcstByst)
            {
            
                if(kvp.Value>1)
                    mprutBySt[kvp.Key] = RutFromSt(kvp.Key);
            }
            return RutcGenerateI(st, mprutBySt);
        }

        private Rut RutFromSt(string st)
        {
            var rut = new Rut(new List<Stm>());
            foreach (var ch in st)
            {
                rut.Rgstm.Add(new Push(ch));
                rut.Rgstm.Add(new Out());
            }
            rut.Rgstm.Add(new Ret());
            return rut;
        }

        Rut RutcGenerateI(string st, Dictionary<string, Rut> mprutByst)
        {
            var rutMain = new Rut(new List<Stm>());
            for (int ich = 0; ich < st.Length; )
            {
                var stLm = StLmAt(st, ich, mprutByst.Keys);
                if (stLm != null)
                {
                    rutMain.Rgstm.Add(new Call(mprutByst[stLm]));
                    ich += stLm.Length;
                }
                else
                {
                    rutMain.Rgstm.Add(new Push(st[ich]));
                    rutMain.Rgstm.Add(new Out());
                    ich++;
                }
            }

            return Compile(rutMain, mprutByst.Values.ToArray());

        }

        private string StLmAt(string st, int ich, IEnumerable<string> rgst)
        {
            int cchLongest = 0;
            string stLongest = null;
            var stSub = st.Substring(ich);
            foreach (var stT in rgst)
            {
                if(stSub.StartsWith(stT))
                {
                    if(cchLongest<stT.Length)
                    {
                        cchLongest = stT.Length;
                        stLongest = stT;
                    }
                }
            }
            return stLongest;
        }

      

        //void Foo()
        //{
        //    var rut0 = new Rut(
        //    new Out('a'),
        //    new Out('l'),
        //    new Out('m'),
        //    new Out('a'),
        //    new Ret()
        //    );

        //    var rut1 = new Rut(
        //        new Out('k'),
        //        new Out('o'),
        //        new Out('r'),
        //        new Out('t'),
        //        new Out('f'),
        //        new Out('a'),
        //        new Ret()
        //        );
        //    var rutc = Compile(
        //            new Rut(
        //                new Call(rut0),
        //                new Call(rut1),
        //                new Call(rut0)
        //                ),
        //            rut0,
        //            rut1
        //        );

        //    foreach (var stm in rutc.Rgstm)
        //    {
        //        Console.WriteLine(stm.Ip + " " + stm.Disasm());
        //    }
        //}

        private const int cstmCall = 10;
            
        public Rut Compile(Rut rutMain, params Rut[] rgrutSub)
        {
            var rutcResult = new Rut(new List<Stm>());
            

            //melyik rutinokat hívják meg?
            var mpccallByrutSub = GetCalls(rutMain);
            
            //ebben lesznek a lefordított subrutinok
            var mprutcSubByrutSub = CompileRelevantRutSub(mpccallByrutSub);

            var push0 = new Push(-1);
            rutcResult.Rgstm.Add(new Push(1));
            rutcResult.Rgstm.Add(push0);
            rutcResult.Rgstm.Add(new Jgz());
            
            var ip = rutcResult.Rgstm.Count;
            foreach (var rut in mprutcSubByrutSub.Keys)
            {
                var rutc = mprutcSubByrutSub[rut];
                
                rutc.Ip = ip; //ez lesz a rut címe
                foreach (var stm in rutc.Rgstm)
                {
                    stm.Ip = ip;
                    rutcResult.Rgstm.Add(stm);
                    ip++;
                }
                rutc.ipLast = ip -1;
               
            }

            //itt indul a program
            var ipMain = ip;
            push0.c = ipMain -3;

            foreach (var stm in rutMain.Rgstm)
            {
                stm.Ip = ip;
                if(stm is Call)
                {
                    var call = (Call) stm;

                    if (mprutcSubByrutSub.ContainsKey(call.rut))
                    {
                        //ha lesz ilyen függvényünk akkor call:
                        rutcResult.Rgstm.AddRange(
                            new List<Stm>
                            {
                                new Push((ip+4) - mprutcSubByrutSub[call.rut].ipLast-1), //majd ide ugrunk vissza
                                new Push(1),     //ez kell a jgz-nek
                                new Push(mprutcSubByrutSub[call.rut].Ip-(ip + 3 )-1),     //ide ugrunk
                                new Jgz(),     //ugrás
                            });
                        ip += 4;
                    }
                    else
                    {
                        //ha nem, akkor inlineoljuk
                        var rgstm = RgstmInlineSubroutine(call.rut);

                        rutcResult.Rgstm.AddRange(rgstm);
                        ip += rgstm.Count;
                    }

                }
                else
                {
                    rutcResult.Rgstm.Add(stm);
                    ip++;    
                }
                
            }

            return rutcResult;
        }

      

        private Dictionary<Rut, Rut> CompileRelevantRutSub(Dictionary<Rut, int> mpccallByrutSub)
        {
            var mprutcSubByrutSub = new Dictionary<Rut, Rut>();
            foreach (var rutSub in mpccallByrutSub.Keys)
            {
                var ccall = mpccallByrutSub[rutSub];
                if (ccall > cstmCall)
                {
                    mprutcSubByrutSub[rutSub] = CompileSubroutine(rutSub);
                }
            }
            return mprutcSubByrutSub;
        }

        private List<Stm> RgstmInlineSubroutine(Rut rut)
        {
            var rgstm = new List<Stm>();
            foreach (var stm in rut.Rgstm)
            {
                if (stm is Ret)
                    continue;
                else if (stm is Call)
                    throw new Exception("calling a subrut from a subrut");
                else
                    rgstm.Add(stm);
            }
            return rgstm;
        }

        public Rut CompileSubroutine(Rut rut)
        {
            var rutc = new Rut(new List<Stm>());
            foreach (var stm in rut.Rgstm)
            {
                if (stm is Ret)
                {
                    rutc.Rgstm.Add(new Push(1));
                    rutc.Rgstm.Add(new Jgz());
                }
                else if (stm is Call)
                    throw new Exception("calling a subrut from a subrut");
                else
                    rutc.Rgstm.Add(stm);
            }
            return rutc;

        }

        private static Dictionary<Rut, int> GetCalls(Rut rutMain)
        {
            var mpccallByRoutine = new Dictionary<Rut, int>();
            foreach (var stm in rutMain.Rgstm)
            {
                if (stm is Call)
                {
                    var call = (Call) stm;
                    if (!mpccallByRoutine.ContainsKey(call.rut))
                        mpccallByRoutine[call.rut] = 1;
                    else
                        mpccallByRoutine[call.rut]++;
                }
            }
            return mpccallByRoutine;
        }

        public abstract class Stm
        {
            public int Ip=-1;

            public abstract string Disasm();

        }

        public class Push : Stm
        {
            public int c;

            public Push(int c)
            {
                this.c = c;
            }

            public override string Disasm()
            {
                return "PUSH {0}".StFormat(c);
            }
        }

        public class Jgz : Stm
        {
            public override string Disasm()
            {
                return "JGZ";
            }
        }

        public class Out : Stm
        {
            public override string Disasm()
            {
                return "OUT";
            }
        }

        public class Rut :Stm
        {
            public List<Stm> Rgstm;
            public int ipLast;

            public Rut(params Stm[] rgstm)
            {
                Rgstm = rgstm.ToList();
            }
            public Rut(List<Stm> rgstm)
            {
                Rgstm = rgstm;
            }

            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }
        }

        public class Read : Stm
        {
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }
        }

        public class Add :Stm
        {
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }
        }

        public class Mul : Stm
        {
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }
        }

        public class Div : Stm
        {
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }

        }

        public class Call : Stm
        {
            public Rut rut;

            public Call(Rut rut)
            {
                this.rut = rut;
            }
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}" + GetType().Name);
            }
        }

        public class Ret : Stm
        {
            public override string Disasm()
            {
                throw new ArgumentException("cant disass {0}"+GetType().Name);
            }
        }

    }
}
