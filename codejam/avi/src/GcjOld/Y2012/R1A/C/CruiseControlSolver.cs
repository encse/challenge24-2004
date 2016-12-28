using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmn.Util;
using Gcj.Util;

namespace Gcj.Y2012.R1A.C
{
    internal class CruiseControlSolver : GcjSolver
    {
        private enum Klr{L,R}

        private class Car
        {
            public Klr klrStart;
            public decimal speed;
            public decimal posStart;
            public List<Coll> rgcoll=new List<Coll>();

            public Car(Klr klr, decimal speed, decimal posStart)
            {
                this.klrStart = klr;
                this.speed = speed;
                this.posStart = posStart;
            }

            public IEnumerable<Coll> RgcollActive(decimal t)
            {
                return rgcoll.Where(coll => coll.tStart <= t && t < coll.tEnd);
            }
        }

        private class Coll : IComparable<Coll>
        {
            private static int idNext = 0;

            public Car car1;
            public Car car2;
            public decimal tStart;
            public decimal tEnd;
            public readonly int id;

            public Coll()
            {
                id = idNext++;
            }

            public void AddToCar()
            {
                car1.rgcoll.Add(this);
                car2.rgcoll.Add(this);
            }

            public int CompareTo(Coll other)
            {
                var dt = tStart-other.tStart;
                if(dt!=0)
                    return Math.Sign(dt);

                return id - other.id;
            }
        }

        private Klr KlrOther(Klr klr)
        {
            switch(klr)
            {
                case Klr.L:
                    return Klr.R;
                case Klr.R:
                    return Klr.L;
            }
            throw new ArgumentException();
        }

        protected override IEnumerable<object> EnobjSolveCase()
        {
            Output.NufDouble = "0.###########";
            var ccar = Fetch<int>();
            var rgcar = new List<Car>();
            for(var icar=0;icar<ccar;icar++)
            {
                rgcar.Add(Fetch<Car>());
            }

            rgcar = rgcar.OrderBy(car => car.posStart).ToList();
            var rgcoll = new List<Coll>();
            for(var icar1=0;icar1<rgcar.Count;icar1++)
            {
                var car1 = rgcar[icar1];
                for(var icar2=icar1+1;icar2<rgcar.Count;icar2++)
                {
                    var car2 = rgcar[icar2];
                    var coll = OcollGet(car1, car2);
                    if(coll == null)
                        continue;

                    if(coll.tStart > 0)
                        rgcoll.Add(coll);
                    coll.AddToCar();
                }
            }

            rgcoll = rgcoll.OrderBy(coll => coll).ToList();

            Func<int, Dictionary<Car, Klr>, decimal> tGet=null;
            tGet = (icoll, mpklrByCar) =>
            {
                if(icoll==rgcoll.Count)
                    return decimal.MaxValue;

                var coll = rgcoll[icoll];
                var t = coll.tStart;

                var fCanMove1 = !coll.car1.RgcollActive(t).Any(collT => collT.CompareTo(coll) < 0);
                var fCanMove2 = !coll.car2.RgcollActive(t).Any(collT => collT.CompareTo(coll) < 0);

                if(fCanMove1)
                {
                    if(fCanMove2)
                    {
                        var mpklrByCar2 = new Dictionary<Car, Klr>(mpklrByCar);

                        mpklrByCar2[coll.car1]=Klr.L;
                        mpklrByCar2[coll.car2]=Klr.R;

                        mpklrByCar[coll.car1]=Klr.R;
                        mpklrByCar[coll.car2]=Klr.L;

                        return Math.Max(tGet(icoll + 1, mpklrByCar), tGet(icoll + 1, mpklrByCar2));
                    }
                    else
                    {
                        mpklrByCar[coll.car1] = KlrOther(mpklrByCar[coll.car2]);
                        return tGet(icoll + 1, mpklrByCar);
                    }
                }
                else if(fCanMove2)
                {
                    mpklrByCar[coll.car2] = KlrOther(mpklrByCar[coll.car1]);
                    return tGet(icoll + 1, mpklrByCar);
                }
                else
                {
                    if(mpklrByCar[coll.car1]!=mpklrByCar[coll.car2])
                        return tGet(icoll + 1, mpklrByCar);
                    return t;
                }

            };

            var tCollide = tGet(0, rgcar.ToDictionary(car => car, car => car.klrStart));

            yield return decimal.MaxValue == tCollide ? (object)"Possible" : tCollide;
        }

        private static Coll OcollGet(Car car1, Car car2)
        {
            Debug.Assert(car1.posStart<=car2.posStart);

            //never collide
            var fStartCollide = car2.posStart - car1.posStart < 5;
            if(car1.speed == car2.speed)
                return !fStartCollide ? null : new Coll
                {
                    car1 = car1,
                    car2 = car2,
                    tStart = 0,
                    tEnd = decimal.MaxValue
                };

            if(!fStartCollide && car1.speed < car2.speed)
                return null;

            var pos = new Func<Car, decimal, decimal>((car, t) => car.posStart + t * car.speed);


            var tcolTotal = (car1.posStart - car2.posStart) / (car2.speed - car1.speed);
            var coll = new Coll
            {
                car1 = car1,
                car2 = car2,
                tStart = (car1.posStart + 5 - car2.posStart) / (car2.speed - car1.speed),
                tEnd = (car1.posStart - (car2.posStart + 5)) / (car2.speed - car1.speed)
            };
            if(coll.tStart > coll.tEnd)
                U.Swap(ref coll.tStart, ref coll.tEnd);
            return coll;
        }
    }
}
