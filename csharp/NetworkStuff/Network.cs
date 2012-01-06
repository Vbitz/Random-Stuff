using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemsGame
{
    class Network
    {
        private static int LastID = 0;
        public int ID = LastID++;
        public string Name;

        public int LastSystemID = 0;

        public ComputerSystem DomainControler;
        public List<ComputerSystem> AllSystems = new List<ComputerSystem>();

        public Network()
        {
            this.Name = Singiltons.GetRandomWord(10);
            this.GenerateSystems();
        }

        public Network(string name, bool generateSystems)
        {
            this.Name = name;

            if (generateSystems)
            {
                this.GenerateSystems();
            }
        }

        private void GenerateSystems()
        {
            DomainControler = new ComputerSystem(this);
            DomainControler.SetDomainControler();

            for (int i = 0; i < Singiltons.rand.Next(10) + 20; i++)
			{
                this.AllSystems.Add(new ComputerSystem(this));
			}
        }
    }
}
