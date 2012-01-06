using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Threading;

namespace IPScanner
{
    class Program
    {
        static Random rand = new Random();
        static Ping ping = new Ping();
        static Stack<string> Messages = new Stack<string>();
        static StreamWriter writer = new StreamWriter("out.txt");

        static void Main(string[] args)
        {
            for (int i = 0; i < 500; i++)
            {
                Thread thread = new Thread(new ThreadStart(Pinger));
                thread.IsBackground = true;
                thread.Start();
            }

            while (true)
            {
                Thread.Sleep(1000);
                while (Messages.Count > 0)
                {
                    string str = Messages.Pop();
                    if (str != null)
                    {
                        if (str.StartsWith("="))
                        {
                            Console.WriteLine(str);
                            File.AppendAllText("success.txt", str + Environment.NewLine);
                        }
                        else
                        {
                            writer.WriteLine(str);
                        }
                    }
                    Thread.Sleep(0);
                }
            }
        }
        
        static void Pinger()
        {
            while (true)
            {
                try
                {
                    string address = rand.Next(6, 200).ToString() + "." + rand.Next(255).ToString() + "." + rand.Next(255).ToString() + "." + rand.Next(255).ToString();
                    WriteLine("Pinging : " + address);
                    PingReply reply = ping.Send(IPAddress.Parse(address));
                    if (reply.Status == IPStatus.Success)
                    {
                        WriteLine("Success : Trying : " + "http://" + address + "/");
                        WebRequest request = HttpWebRequest.Create("http://" + address + "/");
                        ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                        request.GetResponse();
                        WriteLine("==== : " + address + " : SUCCESS");
                    }
                    else
                    {
                        WriteLine("Error : " + address + " : " + reply.Status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("Error : " + ex.Message);
                }
            }
        }

        static void WriteLine(string str)
        {
            Messages.Push(str);
        }
    }
}
