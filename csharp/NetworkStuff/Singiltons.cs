using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemsGame
{
    static class Singiltons
    {
        public static Network LocalNetwork = new Network("Local", false);
        public static ComputerSystem LocalHost;
        public static Session LocalSession;

        public static Random rand = new Random();
        public static List<Network> Networks = new List<Network>();

        private static char[] RandomNameCharactors = "0123456789ABCDEF".ToCharArray();

        public static string GetRandomWord(int length)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {
                str += RandomNameCharactors[rand.Next(RandomNameCharactors.Length)];
            }
            return str;
        }

        public static void GenerateNetworks()
        {
            LocalHost = new ComputerSystem(LocalNetwork);
            LocalNetwork.AllSystems.Add(LocalHost);

            LocalSession = new Session(LocalHost.GetUser("Admin"));

            for (int i = 0; i < 5; i++)
            {
                Networks.Add(new Network());
            }
        }
    }
}
