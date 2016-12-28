using System;
using System.Drawing;
using Ch24.Contest;
using Cmn.Util;

namespace Ch24.Contest03.F
{
    public class FEcologySolver : Solver
    {
        public override void Solve()
        {
            int ccol, crow, cstep;
            
            var pparser = new Pparser(FpatIn);
            pparser.Fetch(out crow, out ccol, out cstep);

            var field = new Kcell[crow,ccol];
            for(int irow=0;irow<crow;irow++)
            {
                var stLine = pparser.StLineNext();
                for(int icol=0;icol<ccol;icol++)
                    field[irow, icol] = KcellFromCh(stLine[icol]);
            }

            var cgrassMin = CCell(field, Kcell.Grass);
            var istepGrassMin = 0;
            var fieldGrassMin = field;

            var cgrassMax = CCell(field, Kcell.Grass);
            var istepGrassMax = 0;
            var fieldGrassMax = field;

            var crabbitMin = CCell(field, Kcell.Rabbit);
            var istepRabbitMin = 0;
            var fieldRabbitMin = field;

            var crabbitMax = CCell(field, Kcell.Rabbit);
            var istepRabbitMax = 0;
            var fieldRabbitMax = field;

            var cfoxMin = CCell(field, Kcell.Fox);
            var istepFoxMin = 0;
            var fieldFoxMin = field;

            var cfoxMax = CCell(field, Kcell.Fox);
            var istepFoxMax = 0;
            var fieldFoxMax = field;

            using (var solwrt = new Solwrt(FpatOut, FpatRefout))
            {
                for (int istep = 0; istep < cstep; )
                {
                    field = Step(istep, field);
                    istep++;
                    var cgrass = CCell(field, Kcell.Grass);
                    var crabbit = CCell(field, Kcell.Rabbit);
                    var cfox = CCell(field, Kcell.Fox);
                  
                    if (crabbit > crabbitMax) { crabbitMax = crabbit; istepRabbitMax = istep; fieldRabbitMax = field; }
                    if (crabbit < crabbitMin) { crabbitMin = crabbit; istepRabbitMin = istep; fieldRabbitMin = field; }

                    if (cgrass > cgrassMax) { cgrassMax = cgrass; istepGrassMax = istep; fieldGrassMax = field; }
                    if (cgrass < cgrassMin) { cgrassMin = cgrass; istepGrassMin = istep; fieldGrassMin = field; }

                    if (cfox > cfoxMax) { cfoxMax = cfox; istepFoxMax = istep; fieldFoxMax = field; }
                    if (cfox < cfoxMin) { cfoxMin = cfox; istepFoxMin = istep; fieldFoxMin = field; }

                    solwrt.WriteLine("{0} {1} {2}", cgrass, crabbit, cfox);
                }

                solwrt.WriteLine("Minimum number of grass: {0} after step {1}.", cgrassMin, istepGrassMin);
                solwrt.WriteLine("Maximum number of grass: {0} after step {1}.", cgrassMax, istepGrassMax);

                solwrt.WriteLine("Minimum number of rabbits: {0} after step {1}.", crabbitMin, istepRabbitMin);
                solwrt.WriteLine("Maximum number of rabbits: {0} after step {1}.", crabbitMax, istepRabbitMax);

                solwrt.WriteLine("Minimum number of foxes: {0} after step {1}.", cfoxMin, istepFoxMin);
                solwrt.WriteLine("Maximum number of foxes: {0} after step {1}.", cfoxMax, istepFoxMax);
            }

            SaveField(fieldGrassMin, FpatOut.Replace(".out", "-1.png"));
            SaveField(fieldGrassMax, FpatOut.Replace(".out", "-2.png"));
            SaveField(fieldRabbitMin, FpatOut.Replace(".out", "-3.png"));
            SaveField(fieldRabbitMax, FpatOut.Replace(".out", "-4.png"));
            SaveField(fieldFoxMin, FpatOut.Replace(".out", "-5.png"));
            SaveField(fieldFoxMax, FpatOut.Replace(".out", "-6.png"));
        }

        private void SaveField(Kcell[,] field, string fpat)
        {
            var bmp = new Bitmap(field.GetLength(1), field.GetLength(0));

            for (var irow = 0; irow < field.GetLength(0); irow++)
            for (var icol = 0; icol < field.GetLength(0); icol++)
            {
                var x = icol;
                var y = irow;
                bmp.SetPixel(x, y, ColorFromKcell(field[irow, icol]));
            }

            bmp.Save(fpat);
        }

        private Color ColorFromKcell(Kcell kcell)
        {
            switch (kcell)
            {
                case Kcell.Grass:
                    return Color.FromArgb(0, 255, 0);
                case Kcell.Rabbit: 
                    return Color.FromArgb(192, 192, 192);
                case Kcell.Fox: 
                    return Color.FromArgb(255, 0, 0);
                default:
                    throw new ArgumentException();
            }
        }

        private int CCell(Kcell[,] field, Kcell kcell)
        {
            var c = 0;
            for (int irow = 0; irow < field.GetLength(0); irow++)
                for (int icol = 0; icol < field.GetLength(1); icol++)
                    c += field[irow, icol] == kcell ? 1 : 0;
            return c;
        }

        private Kcell[,] Step(int istep, Kcell[,] field)
        {
            var ccol = field.GetLength(1);
            var crow = field.GetLength(0);

            var fieldNew = new Kcell[crow,ccol];

            for (int irow = 0; irow < field.GetLength(0); irow++)
            {
                for (int icol = 0; icol < field.GetLength(0); icol++)
                {
                    int i = irow+1;
                    int j = icol+1;

                    var f = (23*i + 87*j + 19*i*i + 61*j*j + 13*i*i*i + 31*j*j*j) % 131;

                    if(istep % 131 == f)
                    {
                        switch(field[irow, icol])
                        {
                            case Kcell.Grass:
                                fieldNew[irow, icol] = FHasKcellAround(field, irow, icol, Kcell.Rabbit) ? Kcell.Rabbit : Kcell.Grass;
                                break;
                            case Kcell.Rabbit:
                                fieldNew[irow, icol] = FHasKcellAround(field, irow, icol, Kcell.Fox) ? Kcell.Fox : Kcell.Rabbit;
                                break;
                            case Kcell.Fox:
                                fieldNew[irow, icol] = FHasKcellAround(field, irow, icol, Kcell.Rabbit) ? Kcell.Fox : Kcell.Grass;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                        fieldNew[irow, icol] = field[irow, icol];
                }
            }
            return fieldNew;
        }

        private bool FHasKcellAround(Kcell[,] field, int irow, int icol, Kcell kcell)
        {
            var crow = field.GetLength(0);
            var ccol = field.GetLength(1);

            for (var irowT = irow-1; irowT <= irow+1; irowT++)
            {
                for (var icolT = icol-1; icolT <= icol+1; icolT++)
                {
                    if(irow == irowT && icol == icolT)
                        continue;
                    if(irowT <0 || irowT >= crow)
                        continue;
                    if(icolT<0 || icolT>=ccol)
                        continue;
                    
                    if(field[irowT, icolT] == kcell)
                        return true;
                }
            }

            return false;
        }
  
        private Kcell KcellFromCh(char ch)
        {
            switch(ch)
            {
                case '.': return Kcell.Grass;
                case '!': return Kcell.Rabbit;
                case '*': return Kcell.Fox;
                default:
                    throw new ArgumentException();
            }

        }

        private enum Kcell
        {
            Grass,
            Rabbit,
            Fox
        }
    }
}
