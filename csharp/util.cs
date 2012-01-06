/*
Copyright (C) 2011 by Vbitz

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/*
 Thank you for NorthernGate for the full name
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.IO;

using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace Utilikit
{
    class Program
    {

        // version
        const string Version = "1.0";


        static void PrintUsage()
        {
            Console.WriteLine(@"File Commands
-f          filename                        creates a empty file with filename

Image Commands
-i          width height filename           creates a empty image of width/height
-ic         width height r g b filename     creates a image of width/height filled with r g b
-ig         width height tile filename      creates a checkerboard image of 
                                                width/height with altinating squeres ever tile

Template Commands
-tmp        type filename                   writes a templete to filename
-tl                                         Lists all templates

Utilty Commands
-e          string                          Escapes string
-vss        projectfile                     Changes a C# project to building in a bin folder in the solution directory
-s          filename                        Prints all readable strings from the file

.Net Framework Commands
-ccompile   filename special                compliles filename into a dll with the same name
-cconsole   filename special                compliles filename into a console exe with the same name
-sym        filename                        Prints all Symbols in a DLL

Common Code Store Commands
-ccr        filename                        Registers a codefile with clr
-cca        filename projectfile            Loads a Common Code File into a VS2010 project
-ccl                                        Lists all Common Code Files");
        }

        static string TemplatesPath = @"templates";
        static string CodePath = @"code\";

        static void Main(string[] args)
        {
            Console.WriteLine("Utilikit Command Line Utiltiy - " + Version);
            Console.WriteLine();

            TemplatesPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\" + TemplatesPath;
            CodePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\" + CodePath;

            if (!Directory.Exists(TemplatesPath))
            {
                Directory.CreateDirectory(TemplatesPath);
            }

            if (!Directory.Exists(CodePath))
            {
                Directory.CreateDirectory(CodePath);
            }

            // In a future version I may not load the whole load into memory
            // just in case someone adds a game mod template
            Dictionary<string, string> CodeTemplates = new Dictionary<string, string>();

            foreach (string item in Directory.GetFiles(TemplatesPath))
            {
                FileInfo info = new FileInfo(item);
                CodeTemplates.Add(info.Name, File.ReadAllText(item));
            }

            // if they don't enter anything then give them usage, there's no why to call the usage using a switch becuase it's un-needed
            if (args.Length < 1)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            try
            {
                // I will make it register commands using reflection but thats a long way away
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-f":
                            {
                                // just writes a empty file, good if you want to create a whole load of files in one setting
                                File.WriteAllText(args[i + 1], "");
                                i += 1;
                            }
                            break;
                        case "-i":
                            {
                                // all of these being seprate methods was due to how i was making
                                // the switchs
                                CreateImage(args, i);
                                i += 3;
                            }
                            break;
                        case "-ic":
                            {
                                CreateColoredImage(args, i);
                                i += 6;
                            }
                            break;
                        case "-ig":
                            {
                                CreateCheckerboardImage(args, i);
                                i += 4;
                            }
                            break;
                        case "-tmp":
                            {
                                // this is really simple
                                if (CodeTemplates.ContainsKey(args[i + 1]))
                                {
                                    File.WriteAllText(args[i + 2], CodeTemplates[args[i + 1]]);
                                }
                                else
                                {
                                    // technicly the exeption handle should say the same thing but this is more useful
                                    Console.WriteLine("ERROR: Template does not Exist");
                                }
                                i += 2;
                            }
                            break;
                        case "-ccompile":
                            {
                                CompileDLL(args, i);
                                i += 2;
                            }
                            break;
                        case "-cconsole":
                            {
                                CompileEXE(args, i);
                                i += 2;
                            }
                            break;
                        case "-sym":
                            {
                                PrintSymbols(args, i);
                                i += 1;
                            }
                            break;
                        case "-e":
                            {
                                Escape(args, i);
                                i += 1;
                            }
                            break;
                        case "-vss":
                            {
                                ModVSProject(args, i);
                                i += 1;
                            }
                            break;
                        case "-tl":
                            {
                                // just lists all of the templates in memory
                                foreach (string item in CodeTemplates.Keys)
                                {
                                    Console.WriteLine(item);
                                }
                            }
                            break;
                        case "-ccr":
                            {
                                // this is really simple but i do need the finfo
                                FileInfo finfo = new FileInfo(args[i + 1]);
                                finfo.CopyTo(CodePath + finfo.Name);
                                i += 1;
                            }
                            break;
                        case "-cca":
                            {
                                // prety simple here
                                FileInfo finfo = new FileInfo(CodePath + args[i + 1]);
                                finfo.CopyTo(Environment.CurrentDirectory + "\\" + finfo.Name);
                                File.Copy(args[i + 2], args[i + 2] + ".bak");
                                // this is almost a hack but parsing the xml and inseting it in the right place would of been 10 lines of code
                                // that i can quicker acomplish in 4
                                File.WriteAllText(args[i + 2], File.ReadAllText(args[i + 2]).Replace(@"  <ItemGroup>
    <Compile ", @"  <ItemGroup>
    <Compile Include=" + "\"" + args[i + 1] + "\"" + @" />
    <Compile "));
                                i += 2;
                            }
                            break;
                        case "-ccl":
                            {
                                foreach (string item in Directory.GetFiles(CodePath))
                                {
                                    Console.WriteLine(item);
                                }
                            }
                            break;
                        case "-s":
                            {
                                PrintStrings(args, i);
                                i += 1;
                            }
                            break;
                        default:
                            {
                                // again more useful then the exeption handle
                                Console.WriteLine("ERROR: Command not Found: " + args[i]);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return;
            }
        }

        private static void PrintStrings(string[] args, int i)
        {
            // this is a masivly useful tool from one of my older programs
            string printableCharactors = "\n\t<>\"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789[]{}:'.,/?\\$@=- ";
            char[] array = File.ReadAllText(args[i + 1]).ToCharArray();
            StreamWriter writer = new StreamWriter(new FileInfo(args[i + 1]).Name + ".strings");
            string str = "";
            int count = 0;
            for (int x = 0; x < array.Length; x++)
            {
                if (printableCharactors.Contains(array[x]))
                {
                    str += array[x];
                }
                else
                {
                    if (str.Length > 5)
                    {
                        writer.WriteLine(count++.ToString() + " : " + str);
                    }
                    str = "";
                }
            }
            writer.Close();
        }

        private static void ModVSProject(string[] args, int i)
        {
            // just back it up incase of massive fail
            File.Copy(args[i + 1], args[i + 1] + ".bak");
            File.WriteAllText(args[i + 1], File.ReadAllText(args[i + 1]).Replace("OutputPath>bin\\", "OutputPath>..\\bin\\"));
        }

        private static void PrintSymbols(string[] args, int i)
        {
            /*
             * This method can fail in a million different ways and there is no real way around some of them
             * I first tried resolving the dependincys using a appdomain event but that failed to load stuff
             * Also if the things more complex like Paint.Net this will not work
             */
            string filename = args[i + 1];
            Assembly asm = Assembly.LoadFile(Environment.CurrentDirectory + "\\" + filename);

            foreach (Type item in asm.GetTypes())
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("[Public : " + item.IsPublic.ToString() + "] [Parent : " + item.BaseType.FullName + "] : " + item.FullName);
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (FieldInfo item2 in item.GetFields(flags))
                    {
                        Console.WriteLine("\tFIELD : [Public : " + item2.IsPublic.ToString() + "] [Static : " + item2.IsStatic.ToString() + "] : " + item2.FieldType.Name + " " + item2.Name);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (PropertyInfo item2 in item.GetProperties(flags))
                    {
                        Console.WriteLine("\tPROPERTY : [Read\\Write : " + item2.CanRead.ToString() + " \\ " + item2.CanWrite + "] : " + item2.PropertyType.Name + " " + item2.Name);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    foreach (MethodInfo item2 in item.GetMethods(flags))
                    {
                        Console.Write("\tMETHOD : [Public : " + item2.IsPublic.ToString() + "] [Static : " + item2.IsStatic.ToString() + "] : " + item2.Name + "(");
                        foreach (ParameterInfo item3 in item2.GetParameters())
                        {
                            Console.Write(item3.ParameterType.Name + " " + item3.Name + ", ");
                        }
                        Console.WriteLine(")");
                    }
                }
                catch (Exception ex)
                {
                    Console.ResetColor();
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
            Console.ResetColor();
        }

        private static void CompileDLL(string[] args, int i)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters prams = new CompilerParameters();
            prams.GenerateInMemory = false;
            prams.OutputAssembly = args[i + 1] + ".dll";
            prams.GenerateExecutable = false;
            prams.CompilerOptions = args[i + 2];
            CompilerResults res = codeProvider.CompileAssemblyFromFile(prams, args[i + 1]);
            if (res.Errors.Count > 0)
            {
                foreach (CompilerError item in res.Errors)
                {
                    Console.WriteLine(item.Line.ToString() + " : " + item.ErrorNumber + " : " + item.ErrorText);
                }
            }
        }

        private static void CompileEXE(string[] args, int i)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters prams = new CompilerParameters();
            prams.GenerateInMemory = false;
            prams.OutputAssembly = args[i + 1] + ".exe";
            prams.GenerateExecutable = true;
            prams.CompilerOptions = args[i + 2];
            CompilerResults res = codeProvider.CompileAssemblyFromFile(prams, args[i + 1]);
            if (res.Errors.Count > 0)
            {
                foreach (CompilerError item in res.Errors)
                {
                    Console.WriteLine(item.Line.ToString() + " : " + item.ErrorNumber + " : " + item.ErrorText);
                }
            }
        }

        private static void CreateCheckerboardImage(string[] args, int i)
        {
            Bitmap img = new Bitmap(Convert.ToInt32(args[i + 1]), Convert.ToInt32(args[i + 2]));
            Graphics gra = Graphics.FromImage(img);
            bool xStart = false;
            int tileSize = Convert.ToInt32(args[i + 3]);
            for (int x = 0; x < Convert.ToInt32(args[i + 1]) / tileSize; x++)
            {
                bool invert = xStart;
                for (int y = 0; y < Convert.ToInt32(args[i + 1]) / tileSize; y++)
                {
                    if (invert)
                    {
                        gra.FillRectangle(Brushes.LightGray, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                    else
                    {
                        gra.FillRectangle(Brushes.White, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                    invert = !invert;
                }
                xStart = !xStart;
            }
            img.Save(args[i + 4]);
        }

        private static void CreateColoredImage(string[] args, int i)
        {
            Bitmap img = new Bitmap(Convert.ToInt32(args[i + 1]), Convert.ToInt32(args[i + 2]));
            Graphics gra = Graphics.FromImage(img);
            gra.Clear(Color.FromArgb(Convert.ToInt32(args[i + 3]), Convert.ToInt32(args[i + 4]), Convert.ToInt32(args[i + 4])));
            img.Save(args[i + 6]);
        }

        private static void CreateImage(string[] args, int i)
        {
            Bitmap img;
            img = new Bitmap(Convert.ToInt32(args[i + 1]), Convert.ToInt32(args[i + 2]));
            img.Save(args[i + 3]);
        }

        private static void Escape(string[] args, int i)
        {
            Console.WriteLine("Escaped String: " + args[i + 1].Escape());
        }
    }

    // taken from http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
    // dictonary taken ICR and Iuvieere
    // that regex stuff was clever and quick but it did not really work for me, so hello foreach, it's really costly but
    // it works fine
    public static class StringHelpers
    {
        private static Dictionary<string, string> escapeMapping = new Dictionary<string, string>()
        {
            {"\a", @"\a"},
            {"\b", @"\b"},
            {"\f", @"\f"},
            {"\n", @"\n"},
            {"\r", @"\r"},
            {"\t", @"\t"},
            {"\v", @"\v"},
            {"\0", @"\0"},
            {"\\", @"\\"},
        };

        public static string Escape(this string s)
        {
            string str = s;
            foreach (string item in escapeMapping.Keys)
            {
                s = s.Replace(item, escapeMapping[item]);
            }
            return s;
        }
    }
}
