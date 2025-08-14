using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Campfire
{
    internal class Client
    {
        public static void Main()
        {
            Console.Write("Enter fire IP: "); string ip = Console.ReadLine();
            Console.WriteLine("Enter name"); string name = Console.ReadLine();
            TcpClient client = new TcpClient(ip, 32865);
            NetworkStream stream = client.GetStream();

            new Thread(() => { byte[] buffer = new byte[1024]; try { while (true) { int len = stream.Read(buffer, 0, buffer.Length); if (len == 0) break; Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, len)); } } catch { } } ).Start();
            while (true)
            {
                string line = Console.ReadLine();
                string mesg = $"{name} {line}";
                byte[] data = Encoding.UTF8.GetBytes(line);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
