using System;
using System.Collections.Generic;
using Ch24.Contest;
using Cmn.Util;
using Satsuma;

namespace Ch24.Contest04.D
{
    public class DWizardsSolver : Solver
    {
        enum Kind
        {
            Wizard,
            City,
            Dummy

        }
        class Nod
        {
            public double X, Y;
            public Kind kind;
            public bool w;
            public Satsuma.Node node;

            public Nod(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public override void Solve()
        {
            var pparser = new Pparser(FpatIn);
            int ccity, cwizard;
            pparser.Fetch(out ccity, out cwizard);
            var rgcity = pparser.FetchN<Nod>(ccity);
            var rgwizard = pparser.FetchN<Nod>(cwizard);

            var graph = new CustomGraph();
            var mpnodByinod = new Dictionary<long, Nod>();

            foreach (var wizard in rgwizard)
            {
                wizard.kind = Kind.Wizard;
                wizard.node = graph.AddNode();
                mpnodByinod[wizard.node.Id] = wizard;
            }

            foreach (var city in rgcity)
            {
                city.kind = Kind.City;
                city.node = graph.AddNode();
                mpnodByinod[city.node.Id] = city;
            }

            foreach (var wizard in rgwizard)
            {
                foreach (var city in rgcity)
                {
                    if (Dist(wizard, city) <= 50)
                        graph.AddArc(wizard.node, city.node, Directedness.Directed);
                }
            }

            var mm = new MaximumMatching(graph, node => mpnodByinod[node.Id].kind == Kind.Wizard);
            mm.Run();
           
            using(Output)
            {
                Solwrt.WriteLine(mm.Matching.ArcCount());
            }
                
           
        }

        private double Dist(Nod city, Nod wizard)
        {
            return Math.Sqrt(Math.Pow(city.X - wizard.X, 2) + Math.Pow(city.Y - wizard.Y, 2));
        }
    }
}


