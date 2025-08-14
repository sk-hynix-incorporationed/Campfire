using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
namespace Campfire
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0) { Console.WriteLine("Usage: campfire [server|client] [port]"); }

            if (int.TryParse(args[1], out int port));
            Console.WriteLine("");
            if (args[0] == "server") Server.Start(port);
            if (args[0] == "client") Client.Start(); 
        }
        
    }
}
