using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace FSGen
{
    class Program
    {
        private static Random rand = new Random();

        static string[] EXENames = new string[]
            {
                "note",
                "word",
                "deck",
                "devenv",
                "desktop",
                "console",
                "framework",
                "system",
                "disk",
                "diskcheck",
                "firewall",
                "framebuffer",
                "gac",
                "concent",
            };

        static string[] ApplicationNames = new string[]
            {
                "note",
                "direct",
                "app",
                "html",
                "web",
                "deck",
                "devenv",
                "desktop",
                "kernal",
                "action",
                "adv",
                "audio",
                "d",
                "k",
                "boot",
                "browser",
                "console",
                "framework",
                "system",
                "disk",
                "firewall",
                "frame",
                "framebuffer",
                "game",
                "gac",
                "hal",
                "hot",
                "cold",
                "icmp",
                "atlantis",
                "real",
            };

        static string[] DLLNames = new string[]
            {
                "api",
                "pack",
                "8",
                "16",
                "32",
                "64",
                "128",
                "map",
                "client",
                "server",
                "cpl",
                "center",
                "play",
                "snapin",
                "crypt",
                "res",
                "net",
                "copy",
                "plug",
                "snap",
                "frame",
                "tech",
            };

        static string[] RESNames = new string[]
            {
                "data",
                "images",
                "strings",
                "atlas",
                "local_config",
                "global_config",
                "config",
                "support",
                "client",
                "server",
                "database",
                "relations",
                "language",
                "appack",
            };

        static string[] CFGNames = new string[]
            {
                "config",
                "system",
                "app",
                "autoexec",
            };

        static string[] DirectoryNames = new string[]
            {
                "config",
                "lang",
                "temp",
                "store",
                "data",
                "real",
            };

        static string GetRandom(string[] items)
        {
            return items[rand.Next(items.Length)];
        }

        static string GetRandomVersion()
        {
            return rand.Next(20).ToString() + "_" + rand.Next(9).ToString();
        }

        static string GetRandomDLLName()
        {
            string str = "";
            if (rand.Next(3) > 1)
            {
                if (rand.Next(2) > 0)
                {
                    str = GetRandom(ApplicationNames) + "_" + GetRandom(DLLNames) + ".dll";
                }
                else
                {
                    str = GetRandom(ApplicationNames) + GetRandom(DLLNames) + ".dll";
                }
            }
            else if (rand.Next(5) > 3)
            {
                str = GetRandom(ApplicationNames) + ".dll";
            }
            else
            {
                str = GetRandom(ApplicationNames) + GetRandomVersion() + ".dll";
            }
            if (rand.Next(15) == 1)
            {
                str = str.ToUpper();
            }
            return str;
        }

        static string GetRandomEXEName()
        {
            return GetRandom(EXENames) + ".exe";
        }

        static string GetRandomDATName()
        {
            switch (rand.Next(4))
            {
                case 0:
                    return GetRandom(RESNames) + ".res";
                case 1:
                    return GetRandom(RESNames) + ".cab";
                case 2:
                    return GetRandom(RESNames) + ".dat";
                case 3:
                    return GetRandom(RESNames) + ".db";
                default:
                    return GetRandom(RESNames) + ".balls";
            }
        }

        static string GetGUID()
        {
            return Guid.NewGuid().ToString();
        }

        static string GetDirectoryName(bool includeGUID)
        {
            if (includeGUID && rand.Next(3) > 1)
            {
                return GetGUID();
            }
            else
            {
                return GetRandom(DirectoryNames);
            }
        }

        static void CreateFile(string path, string name, int size)
        {
            byte[] fileData = new byte[size];
            rand.NextBytes(fileData);
            File.WriteAllBytes(path + name, fileData);
        }

        static void Main(string[] args)
        {
            Directory.CreateDirectory("testing");
            for (int i = 0; i < rand.Next(100) + 40; i++)
            {
                CreateFile("testing\\", GetRandomDLLName(), rand.Next(2000000));
            }
            for (int i = 0; i < rand.Next(5) + 5; i++)
            {
                string exeName = GetRandom(EXENames);
                Directory.CreateDirectory("testing\\" + exeName);
                CreateFile("testing\\" + exeName + "\\", exeName + ".exe", rand.Next(6000000));
                for (int x = 0; x < rand.Next(4); x++)
                {
                    CreateFile("testing\\" + exeName + "\\", GetRandomDATName(), rand.Next(10000000));
                }
            }
        }
    }
}
