using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using System.CodeDom.Compiler;
using Microsoft.CSharp;
	
namespace AppDomainLoaders
{
	public static class AppDomainLoader
	{
		
		static SpecialObject CreateInstance(string name)
        {
            Type item = Type.GetType(name);
            return new SpecialObject(item.Assembly.CreateInstance(item.FullName));
        }

        static void InitRuntime()
        {
            AppDomain current = AppDomain.CurrentDomain;
            current.TypeResolve += new ResolveEventHandler(current_TypeResolve);
        }

        static Assembly current_TypeResolve(object sender, ResolveEventArgs args)
        {
            string[] items = args.Name.Split('.');
            string path = Environment.CurrentDirectory + "\\code\\";
            for (int i = 0; i < items.Length - 1; i++)
            {
                path += items[i] + "\\";
            }
            path += items[items.Length - 1] + ".cs";
            Console.WriteLine(path);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters prams = new CompilerParameters();
            prams.GenerateExecutable = false;
            prams.GenerateInMemory = true;
            CompilerResults results = provider.CompileAssemblyFromFile(prams, path);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError item in results.Errors)
                {
                    Console.WriteLine("[RUNTIME - Compiler Error] : " + item.Line.ToString() + " : " + item.ErrorNumber + " " + item.ErrorText);
                }
                return null;
            }
            else
            {
                return results.CompiledAssembly;
            }
        }
    }

    public class SpecialObject
    {
        private object Inner;
        private Type InnerType;

        public SpecialObject(object item)
        {
            this.Inner = item;
            this.InnerType = item.GetType();
        }

        public object RunMethod(string name, params object[] args)
        {
            return this.InnerType.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, this.Inner, args);
        }
    }
}
