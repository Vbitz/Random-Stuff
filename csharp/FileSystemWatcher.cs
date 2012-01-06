// This one could need a little expliaion
// It looks all over the file system and whenever a file is changed it trys to make a copy of it,
// This can explode in size really fast

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;

namespace Nothing_Can_Go_Wrong
{
    // everything will go wrong, this is a bad idea

    class Program
    {
        static int i = 0;
        static void Main(string[] args)
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\items\\");
            FileSystemWatcher watcher = new FileSystemWatcher("C:\\", "*.*");
            watcher.IncludeSubdirectories = true;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;
            while (true)
            {
                Thread.Sleep(150);
            }
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
            {
                if (e.FullPath.Contains("NOTHING_items"))
                {
                    return;
                }
                try
                {
                    int num = i++;
                    File.Copy(e.FullPath, Environment.CurrentDirectory + "\\NOTHING_items\\" + num.ToString() + ".bin");
                    Console.WriteLine(e.FullPath);
                    int prepend = 0;
                    FileInfo info = new FileInfo(e.FullPath);
                    while (File.Exists(Environment.CurrentDirectory + "\\items\\" + (prepend++).ToString() + " - " + info.Name))
                    {
                    }
                    Thread.Sleep(10);
                    File.Move(Environment.CurrentDirectory + "\\items\\" + num.ToString() + ".bin",
                        Environment.CurrentDirectory + "\\items\\" + prepend.ToString() + " - " + info.Name);
                    return;
                }
                catch
                {
                }
            }
            else
            {
                if (!Directory.Exists(e.FullPath))
                {
                    Console.WriteLine("Gone: " + e.FullPath);
                }
                return;
            }
        }
    }
}
