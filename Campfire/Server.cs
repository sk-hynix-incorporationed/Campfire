using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Campfire
{
    internal class Server
    {
        static List<TcpClient> clients = new List<TcpClient>();
        public static void Start(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 32865);
            listener.Start();
            Console.WriteLine("Campfire host running on port 38265");
            while (true)
            {
                var client = listener.AcceptTcpClient();
                lock (clients) { clients.Add(client); }
                new Thread(() => HandleClient(client)).Start();
            }
        }
        public static void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, len);
                    Console.WriteLine(msg);
                    Broadcast(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                lock (clients) { clients.Remove(client); }
                client.Close();
            }
        }
        public static void Broadcast(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            lock (clients)
            {
                foreach (var c in clients)
                {
                    try { c.GetStream().Write(data, 0, data.Length); } catch { }
                }
            }
        }

    }
}
