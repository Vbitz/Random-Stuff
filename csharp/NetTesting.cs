using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetF
{
    class BinaryMemoryWriter
    {
        private MemoryStream memStream;
        public BinaryWriter Writer;

        public BinaryMemoryWriter()
        {
            this.memStream = new MemoryStream();
            this.Writer = new BinaryWriter(this.memStream);
        }

        public byte[] GetData()
        {
            this.memStream.Seek(0, SeekOrigin.Begin);
            byte[] dataBytes = new byte[this.memStream.Length];
            this.memStream.Read(dataBytes, 0, (int)this.memStream.Length);
            return dataBytes;
        }
    }

    class BasicNetClient
    {
        protected UdpClient Client;
        static int LastPort = 40000;
        public int Port = LastPort++;

        public BasicNetClient RunThread()
        {
            Thread thread = new Thread(new ThreadStart(this.Run));
            thread.IsBackground = true;
            thread.Start();
            return this;
        }

        private void Run()
        {
            try
            {
                this.Client = new UdpClient(this.Port);
                Program.WriteLine("Starting on Port : " + this.Port.ToString() + " : " + this.GetType().Name);
                this.Init();
                while (true)
                {
                    Thread.Sleep(10);
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] rawdata = this.Client.Receive(ref endpoint);
                    Program.WriteLine("[" + this.Port.ToString() + "] : Recived from " + endpoint + " : " + this.RecievePacket(rawdata, endpoint));
                }
            }
            catch (Exception ex)
            {
                Program.WriteLine("[" + this.Port.ToString() + "] : ERROR : " + ex.Message);
            }
        }

        public virtual string RecievePacket(byte[] data, IPEndPoint sender)
        {
            return "Done Nothing";
        }

        public virtual void Init()
        {

        }

        protected void SendPacket(BinaryMemoryWriter writer, int port)
        {
            this.SendPacket(writer.GetData(), port);
        }

        protected void SendPacket(byte[] data, int port)
        {
            Program.WriteLine("[" + this.Port.ToString() + "] : Sent a packet");
            this.Client.Send(data, data.Length, "127.0.0.1", port);
        }
    }

    class SimpleDDOSClient : BasicNetClient
    {
        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            BinaryMemoryWriter writer = new BinaryMemoryWriter();
            writer.Writer.Write("Stop It");
            byte[] data2 = writer.GetData();
            for (int i = 0; i < 50; i++)
            {
                this.SendPacket(data2, sender.Port);
            }
            return "Sent it right back, leave me alone";
        }
    }

    class Pinger : BasicNetClient
    {
        private int PingPort;
        public Pinger(int port)
        {
            this.PingPort = port;
        }

        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            return "Why Hello";
        }

        public override void Init()
        {
            BinaryMemoryWriter writer = new BinaryMemoryWriter();
            writer.Writer.Write("Hello");
            this.SendPacket(writer, this.PingPort);
        }
    }

    class Passer : BasicNetClient
    {
        private int PassPort;

        public Passer(int port)
        {
            this.PassPort = port;
        }

        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            this.SendPacket(data, PassPort);
            return "Sent it right on";
        }
    }

    class EchoClient : BasicNetClient
    {
        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            this.SendPacket(data, sender.Port);
            return "Sending it Right back";
        }
    }

    class RemoteDDOSClient : BasicNetClient
    {
        private int Target;

        public RemoteDDOSClient(int port)
        {
            this.Target = port;
        }

        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            BinaryMemoryWriter writer = new BinaryMemoryWriter();
            writer.Writer.Write("Stop It");
            byte[] data2 = writer.GetData();
            for (int i = 0; i < 10; i++)
            {
                this.SendPacket(data2, this.Target);
            }
            return "Sent it right on, munch on that";
        }
    }

    class Spliter : BasicNetClient
    {
        private int Target1;
        private int Target2;

        public Spliter(int target1, int target2)
        {
            this.Target1 = target1;
            this.Target2 = target2;
        }

        public override string RecievePacket(byte[] data, IPEndPoint sender)
        {
            this.SendPacket(data, this.Target1);
            this.SendPacket(data, this.Target2);
            return "Sending it on to both";
        }
    }

    class Program
    {
        public static StreamWriter writer = new StreamWriter("out.log");
        private static List<string> LogLines = new List<string>();

        static void Main(string[] args)
        {
            BasicNetClient endpoint = new BasicNetClient().RunThread();
            RemoteDDOSClient ddoser = (RemoteDDOSClient)new RemoteDDOSClient(endpoint.Port).RunThread();
            RemoteDDOSClient ddoser2 = (RemoteDDOSClient)new RemoteDDOSClient(ddoser.Port).RunThread();
            Spliter split = (Spliter)new Spliter(ddoser.Port, ddoser2.Port).RunThread();
            int port = split.Port;
            for (int i = 0; i < 1000; i++)
            {
                Passer pass = (Passer)new Passer(port).RunThread();
                port = pass.Port;
            }
            Pinger ping = (Pinger)new Pinger(port).RunThread();

            Console.ReadKey();

            foreach (string item in LogLines)
            {
                writer.WriteLine(item);
            }

            writer.Close();
        }

        public static void WriteLine(string value)
        {
            LogLines.Add(value);
        }
    }
}
