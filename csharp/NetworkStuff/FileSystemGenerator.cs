using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemsGame
{
    class FileSystemGenerator
    {
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
            return items[Singiltons.rand.Next(items.Length)];
        }

        static string GetRandomVersion()
        {
            return Singiltons.rand.Next(20).ToString() + "_" + Singiltons.rand.Next(9).ToString();
        }

        static string GetRandomDLLName()
        {
            string str = "";
            if (Singiltons.rand.Next(3) > 1)
            {
                if (Singiltons.rand.Next(2) > 0)
                {
                    str = GetRandom(ApplicationNames) + "_" + GetRandom(DLLNames) + ".dll";
                }
                else
                {
                    str = GetRandom(ApplicationNames) + GetRandom(DLLNames) + ".dll";
                }
            }
            else if (Singiltons.rand.Next(5) > 3)
            {
                str = GetRandom(ApplicationNames) + ".dll";
            }
            else
            {
                str = GetRandom(ApplicationNames) + GetRandomVersion() + ".dll";
            }
            if (Singiltons.rand.Next(15) == 1)
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
            switch (Singiltons.rand.Next(4))
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
            if (includeGUID && Singiltons.rand.Next(3) > 1)
            {
                return GetGUID();
            }
            else
            {
                return GetRandom(DirectoryNames);
            }
        }

        public static void GenerateBasicFilesystem(ComputerSystem sys, Session rootUser)
        {
            sys.FSCreateDirectory("Users", rootUser);
            for (int i = 0; i < Singiltons.rand.Next(100) + 40; i++)
            {
                sys.FSCreateFile("", GetRandomDLLName(), Singiltons.rand.Next(2000000), rootUser);
            }
            for (int i = 0; i < Singiltons.rand.Next(5) + 5; i++)
            {
                string exeName = GetRandom(EXENames);
                sys.FSCreateDirectory(exeName, rootUser);
                sys.FSCreateFile(exeName + "\\", exeName + ".exe", Singiltons.rand.Next(6000000), rootUser);
                for (int x = 0; x < Singiltons.rand.Next(4); x++)
                {
                    sys.FSCreateFile(exeName + "\\", GetRandomDATName(), Singiltons.rand.Next(10000000), rootUser);
                }
            }
            sys.FSSweepPremissions("", true, sys.GetGroup("Admins"), new FilePremission(true, false), rootUser);
            sys.FSCreateDirectory("SystemStore", rootUser);
        }
    }
}
