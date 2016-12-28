using System;
using System.Collections.Generic;
using System.Linq;
using Ch24.Contest;

namespace Ch24.Contest03.E
{
    public class ECryptoSolver: Solver
    {
        public override void Solve()
        {
            var st = Fetch<string>();
            int cch = st.Length/2;
            var rgc = new int[cch];

            for (int i = 0; i < cch; i++)
                rgc[i] = (st[2*i] - 'A')*16 + st[2*i + 1] - 'A';

            var rgb = new int[cch];
            for (int i = 0; i < cch; i++)
            {
                int k = i + 1;
                if (FPrime(k))
                    rgb[i] = (rgc[i] - k + 512)%256;
                else
                    rgb[i] = rgc[i];
            }

            var rga = new int[cch];
            rga[0] = rgb[0];
            for (int i = 1; i < cch; i++)
                rga[i] = (rgb[i] - rgb[i - 1] + 512)%256;

            st = new string(rga.Select(a => (char) a).ToArray());
            var rgch = new char[cch];
            int s = (int) Math.Sqrt(cch);


            {
                var rgaPrev = new List<int>();

                for (int ip = cch - 1; ip >= 0;ip-- )
                {
                    if (rgaPrev.Count > 1)
                    {
                        int p = rgaPrev[0];
                        rgaPrev[0] = rgaPrev[1];
                        rgaPrev[1] = p;
                    }

                    if (rgaPrev.Count > 12)
                        rgaPrev = rgaPrev.Skip(rgaPrev.Count - 12).Concat(rgaPrev.Take(rgaPrev.Count - 12)).ToList();

                    rgaPrev.Insert(0, rga[ip]);
                    
                }
                rga = rgaPrev.ToArray();
            }
            
           
            {
                int i = 0;
                int d = 0;
                while(i<cch)
                {
                    for (int l = d; l >= 0; l--)
                    {
                        int x = d - l;
                        int y = d - x;
                        if (x >= s)
                            break;
                        if(x<0 || x>s-1 || y<0||y>s-1)
                            continue;
                        
                        rgch[x + y * s] = (char)rga[i++];
                    }
                    d++;
                }
            }

            st = new string(rgch);
            using (Output)
            {
                Solwrt.Write("{0}", st);
            }

        }

        bool FPrime(int number)
        {
            int boundary = (int) Math.Floor(Math.Sqrt(number));

            if (number == 1) return false;
            if (number == 2) return true;

            for (int i = 2; i <= boundary; ++i)
            {
                if (number % i == 0) return false;
            }

            return true;
        }
    }
}


